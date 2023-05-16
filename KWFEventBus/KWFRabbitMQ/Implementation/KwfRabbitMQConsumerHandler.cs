namespace KWFEventBus.KWFRabbitMQ.Implementation
{    
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    using KWFEventBus.KWFRabbitMQ.Interfaces;
    using KWFEventBus.KWFRabbitMQ.Models;

    using Microsoft.Extensions.Logging;

    public class KwfRabbitMQConsumerHandler<THandler, TPayload> : IKwfRabbitMQConsumerHandler, IDisposable
        where THandler : class, IKwfRabbitMQEventHandler<TPayload>
        where TPayload : class
    {
        private readonly ILogger? _logger;
        private readonly JsonSerializerOptions _jsonSettings;

        private readonly KwfRabbitMQConfiguration _configuration;
        private readonly string? _consumer = null;//TODO

        private readonly IKwfRabbitMQEventHandler<TPayload> _kwfEventHandler;
        private readonly int _timeout;
        private readonly string _topic;
        private readonly int _maxRetry;
        private bool _consumeEnabled = false;
        bool _disposed;

        public KwfRabbitMQConsumerHandler(
            IKwfRabbitMQEventHandler<TPayload> kwfEventHandler,
            string topic,
            string consumer,
            KwfRabbitMQConfiguration configuration,
            int timeout,
            int maxRetries,
            JsonSerializerOptions jsonSettings,
            ILogger? logger)
        {
            _kwfEventHandler = kwfEventHandler;
            _jsonSettings = jsonSettings;
            _topic = topic;
            _consumer = consumer;
            _configuration = configuration;
            _timeout = timeout;
            _logger = logger;
            _maxRetry = maxRetries;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                //close connections and dispose
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
                    bool messageProcessException = false;
                    while (IsStarted)
                    {
                        try
                        {
                            /* IMPLEMENT POLLING
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
                                            _logger.LogInformation(Constants.RabbitMQ_log_eventId, "Consuming event from topic {0} with id {1} and key {2}",
                                                _topic,
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
                                    TryComminMessage(message);
                                    messageProcessException = true;
                                    throw;
                                }

                                TryComminMessage(message);
                            }
                            */
                        }
                        catch (Exception ex)
                        {
                            /* HANDLE EXCEPTION
                            if ((_configuration?.AllowAutoCreateTopics ?? false) &&
                                ex is KafkaException kafkaEx &&
                                kafkaEx.Error.Code == ErrorCode.UnknownTopicOrPart)
                            {
                                if (_logger is not null && _logger.IsEnabled(LogLevel.Warning))
                                {
                                    _logger.LogWarning(Constants.RabbitMQ_log_eventId, "{0} for consumer on topic {1}", kafkaEx.Message, _topic);
                                }
                            }
                            else
                            {
                                var kwfEx = new KwfKafkaBusException("KAFKACONSUMEERR", $"Error occured during consumption of topic {_topic}", ex);
                                if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                                {
                                    _logger.LogError(Constants.RabbitMQ_log_eventId, kwfEx, "Error occured on consumer for topic {0}", _topic);
                                }

                                if (retry == 0)
                                {
                                    if (_logger is not null && _logger.IsEnabled(LogLevel.Critical))
                                    {
                                        _logger.LogCritical(Constants.RabbitMQ_log_eventId, "Consumer for topic {0} has stoped", _topic);
                                    }

                                    _consumeEnabled = false;
                                    throw kwfEx;
                                }

                                retry--;
                            }
                            */
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
                //commit last message?
            }
            catch
            { }
        }
    }
}
