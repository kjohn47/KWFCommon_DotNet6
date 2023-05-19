namespace KWFEventBus.KWFKafka.Implementation
{
    using Confluent.Kafka;

    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFKafka.Interfaces;
    using KWFEventBus.KWFKafka.Models;
    using KWFEventBus.KWFKafka.Constants;

    using Microsoft.Extensions.Logging;

    using System.Text;
    using System.Text.Json;

    public class KwfKafkaConsumerHandler<THandler, TPayload> : IKwfKafkaEventConsumerHandler, IDisposable
        where THandler : class, IKwfKafkaEventHandler<TPayload>
        where TPayload : class
    {
        private readonly IKwfKafkaEventHandler<TPayload> _kwfEventHandler;
        private readonly IConsumer<string, byte[]> _consumer;
        private readonly IProducer<string, byte[]> _producer;
        private readonly ConsumerConfig _configuration;
        private readonly int _timeout;
        private readonly ILogger? _logger;
        private readonly JsonSerializerOptions _kafkaJsonSettings;
        private readonly string _topic;
        private readonly int _maxRetry;
        private readonly int _maxRetryDlq;
        private readonly string _dlqTopicRetry = string.Empty;
        private readonly string _dlqTopicFail = string.Empty;
        private readonly bool _dlqEnabled = false;
        private bool _consumeEnabled = false;
        bool _disposed;

        public KwfKafkaConsumerHandler(
            IKwfKafkaEventHandler<TPayload> kwfEventHandler,
            string topic,
            IConsumer<string, byte[]> consumer,
            IProducer<string, byte[]> producer,
            ConsumerConfig configuration,
            int timeout,
            int maxRetries,
            int maxRetryDlq,
            string dlqTag,
            JsonSerializerOptions kafkaJsonSettings,
            ILogger? logger)
        {
            _kwfEventHandler = kwfEventHandler;
            _kafkaJsonSettings = kafkaJsonSettings;
            _topic = topic;
            _consumer = consumer;
            _producer = producer;
            _configuration = configuration;
            _timeout = timeout;
            _logger = logger;
            _maxRetry = maxRetries;
            _maxRetryDlq = maxRetryDlq;
            _dlqEnabled = maxRetryDlq >= 0;

            if (_dlqEnabled)
            {
                _dlqTopicRetry = $"{topic}.{dlqTag}.retry.";
                _dlqTopicFail = $"{topic}.{dlqTag}.fail";
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_consumer is not null)
                {
                    _consumer.Close();
                }
            }
        }

        public bool IsStarted => _consumeEnabled && !_disposed;

        public void StartConsuming()
        {
            if (IsStarted || _disposed)
            {
                return;
            }

            _consumeEnabled = true;
            if (_consumer is not null)
            {
                Task.Run(async () =>
                {
                    var retry = _maxRetry;
                    var alwaysRetry = _maxRetry == -1;
                    bool messageProcessException = false;
                    while (IsStarted)
                    {
                        try
                        {
                            var message = _consumer?.Consume(_timeout);
                            retry = messageProcessException ? retry : _maxRetry;

                            if (message is not null && !message.IsPartitionEOF && message.Message.Value is not null)
                            {
                                try
                                {
                                    var payloadObj = JsonSerializer.Deserialize<EventPayloadEnvelope<TPayload>>(
                                                        Encoding.UTF8.GetString(message.Message.Value),
                                                        _kafkaJsonSettings);

                                    if (payloadObj is not null)
                                    {
                                        if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                                        {
                                            _logger.LogInformation(Constants.Kafka_log_eventId, "Consuming event from topic {TOPIC} with id {ID} and key {KEY}",
                                                message.Topic, 
                                                payloadObj.Id, 
                                                message.Message.Key);
                                        }

                                        await _kwfEventHandler.HandleEventAsync(payloadObj);
                                    }
                                    retry = _maxRetry;
                                    messageProcessException = false;
                                }
                                catch
                                {
                                    if (_dlqEnabled)
                                    {
                                        if (message.Topic == _topic && _maxRetryDlq > 0)
                                        {
                                            await _producer.ProduceAsync(string.Concat(_dlqTopicRetry, '0'), message.Message);
                                        }
                                        else if (message.Topic.StartsWith(_dlqTopicRetry))
                                        {
                                            var dlqRetry = int.TryParse(message.Topic[_dlqTopicRetry.Length..], out int dlqRetryOut) ? dlqRetryOut : 0;
                                            dlqRetry++;
                                            if (dlqRetry < _maxRetryDlq)
                                            {
                                                await _producer.ProduceAsync(string.Concat(_dlqTopicRetry, dlqRetry), message.Message);
                                            }
                                            else
                                            {
                                                await _producer.ProduceAsync(string.Concat(_dlqTopicFail), message.Message);
                                            }
                                        }
                                        else
                                        {
                                            await _producer.ProduceAsync(string.Concat(_dlqTopicFail), message.Message);
                                        }                                        
                                    }

                                    TryComminMessage(message);
                                    messageProcessException = true;
                                    throw;
                                }

                                TryComminMessage(message);
                            }
                        }
                        catch (Exception ex)
                        {
                            if ((_configuration?.AllowAutoCreateTopics ?? false) && 
                                ex is KafkaException kafkaEx && 
                                kafkaEx.Error.Code == ErrorCode.UnknownTopicOrPart)
                            {
                                if (_logger is not null && _logger.IsEnabled(LogLevel.Warning))
                                {
                                    _logger.LogWarning(Constants.Kafka_log_eventId, "{EXCEPTION} for consumer on topic {TOPIC}", kafkaEx.Message, _topic);
                                }
                            }
                            else
                            {
                                var kwfEx = new KwfKafkaBusException("KAFKACONSUMEERR", $"Error occured during consumption of topic {_topic}", ex);
                                if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                                {
                                    _logger.LogError(Constants.Kafka_log_eventId, kwfEx, "Error occured on consumer for topic {TOPIC}", _topic);
                                }

                                if (!alwaysRetry)
                                {

                                    if (retry == 0)
                                    {
                                        if (_logger is not null && _logger.IsEnabled(LogLevel.Critical))
                                        {
                                            _logger.LogCritical(Constants.Kafka_log_eventId, "Consumer for topic {TOPIC} has stoped", _topic);
                                        }

                                        _consumeEnabled = false;
                                        throw kwfEx;
                                    }

                                    retry--;
                                }
                            }
                        }
                    }
                });
            }
        }

        public void StopConsuming()
        {
            _consumeEnabled = false;
            try
            {
                if (_configuration?.EnableAutoCommit is null || _configuration.EnableAutoCommit.Value == false)
                {
                    _consumer.Commit();
                }
            }
            catch
            { }
        }

        private void TryComminMessage(ConsumeResult<string, byte[]> message, Guid? id = null)
        {
            if (_configuration?.EnableAutoCommit is null || _configuration.EnableAutoCommit.Value == false)
            {
                try
                {
                    _consumer?.Commit(message);
                }
                catch
                {
                    if (_logger is not null && _logger.IsEnabled(LogLevel.Warning))
                    {
                        _logger.LogWarning(Constants.Kafka_log_eventId, "Error occurred during commit of topic {TOPIC}, payload id:{ID}", _topic, id);
                    }
                }
            }
        }
    }
}
