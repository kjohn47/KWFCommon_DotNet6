namespace KWFEventBus.KWFKafka.Implementation
{
    using Confluent.Kafka;

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFKafka.Models;

    using Microsoft.Extensions.Logging;

    using System.Text;
    using System.Text.Json;

    public class KwfKafkaConsumerHandler<THandler, TPayload> : IKwfEventConsumerHandler, IDisposable
        where THandler : class, IKwfEventHandler<TPayload>
        where TPayload : class
    {
        private readonly IKwfEventHandler<TPayload> _kwfEventHandler;
        private readonly IConsumer<string, byte[]> _consumer;
        private readonly ConsumerConfig _configuration;
        private readonly int _timeout;
        private readonly ILogger? _logger;
        private readonly JsonSerializerOptions _kafkaJsonSettings;
        private readonly string _topic;
        private bool _consumeEnabled = true;
        bool _disposed;

        public KwfKafkaConsumerHandler(
            IKwfEventHandler<TPayload> kwfEventHandler,
            string topic,
            IConsumer<string, byte[]> consumer,
            ConsumerConfig configuration,
            int timeout,
            JsonSerializerOptions kafkaJsonSettings,
            ILogger? logger)
        {
            _kwfEventHandler = kwfEventHandler;
            _kafkaJsonSettings = kafkaJsonSettings;
            _topic = topic;
            _consumer = consumer;
            _configuration = configuration;
            _timeout = timeout;
            _logger = logger;
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

        public void StartConsuming()
        {
            _consumeEnabled = true;
            if (_consumer is not null)
            {
                Task.Run(async () =>
                {
                    while (!_disposed && _consumeEnabled)
                    {
                        var message = _consumer.Consume(_timeout);
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
                                        _logger.LogInformation("Consuming event from topic {0} with id {1}", _topic, payloadObj.Id);
                                    }

                                    await _kwfEventHandler.HandleEventAsync(payloadObj);
                                }

                                if (_configuration?.EnableAutoCommit is null || _configuration.EnableAutoCommit.Value == false)
                                {
                                    try
                                    {
                                        _consumer.Commit(message);
                                    }
                                    catch
                                    {
                                        if (_logger is not null && _logger.IsEnabled(LogLevel.Warning))
                                        {
                                            _logger.LogWarning("Error occurred during commit of topic {0}, payload id:{1}", _topic, payloadObj?.Id);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                var kwfEx = new KwfKafkaBusException("KAFKACONSUMEERR", $"Error occured during consumption of topic {_topic}", ex);
                                if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                                {
                                    _logger.LogError(kwfEx, "Error occured on consumer for topic {0}", _topic);
                                }
                                else
                                {
                                    throw kwfEx;
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
            if (_consumer is not null)
            {
                _consumer.Close();
            }
        }
    }
}
