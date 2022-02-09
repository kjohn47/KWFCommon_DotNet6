namespace KWFEventBus.KWFKafka.Implementation
{
    using Confluent.Kafka;

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFKafka.Interfaces;
    using KWFEventBus.KWFKafka.Models;

    using Microsoft.Extensions.Logging;

    using System.Text;
    using System.Text.Json;

    public class KwfKafkaBus : IKwfKafkaBus, IDisposable
    {
        private readonly KwfKafkaConfiguration _configuration;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;
        private readonly IProducer<string, byte[]> _producer;
        private readonly ProducerConfig? _defaultProducerProperties;
        private readonly ConsumerConfig? _defaultConsumerProperties;
        private readonly Headers? _producerHeaders;
        private readonly ILogger? _logger;
        private bool _disposed;

        public KwfKafkaBus(KwfKafkaConfiguration configuration, ILoggerFactory? loggerFactory, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            _configuration = configuration;
            _jsonSerializerOptions = jsonSerializerOptions ?? EventsJsonOptions.GetJsonOptions();
            _defaultProducerProperties = _configuration.GetProducerConfiguration();
            _defaultConsumerProperties = _configuration.GetConsumerConfiguration();
            _producerHeaders = new Headers
            {
                { "application-name", Encoding.UTF8.GetBytes(_configuration.AppName) },
                { "host-name", Encoding.UTF8.GetBytes(_configuration.ClientName) }
            };
            _producer = new ProducerBuilder<string, byte[]>(_defaultProducerProperties)
                .SetLogHandler((p, m) =>
                {
                    ConfigureLogger(m);
                })
                .SetErrorHandler((p, err) =>
                {
                    ConfigureErrorHandler(err, "Producer");
                }).Build();

            if (loggerFactory is not null)
            {
                _logger = loggerFactory.CreateLogger<KwfKafkaBus>();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _producer.Dispose();
            }
        }

        public Task ProduceAsync<T>(T payload, string topic, CancellationToken? cancellationToken = null) where T : class
        {
            return ProduceAsync(payload, topic, null, cancellationToken);
        }

        public async Task ProduceAsync<T>(T payload, string topic, string? key, CancellationToken? cancellationToken = null) where T : class
        {
            try
            {
                var envelope = new EventPayloadEnvelope<T>(payload);
                if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Producing event to topic {0} with id {1} and key {2}",
                        topic,
                        envelope.Id,
                        key);
                }

                await _producer.ProduceAsync(topic, new Message<string, byte[]>
                {
                    Headers = _producerHeaders,
                    Key = key!,
                    Value = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope, _jsonSerializerOptions)),
                    Timestamp = new Timestamp(envelope.TimeStamp)
                },
                cancellationToken ?? default);
            }
            catch (Exception ex)
            {
                if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Error occured on producer for topic {0}\n Reason: {1}", topic, ex.Message);
                }
                else
                {
                    throw new KwfKafkaBusException("KAFKAPRODERR", $"Error occured during consumption of topic {topic}", ex);
                }
            }
        }

        public IKwfEventConsumerHandler CreateConsumer<TPayload>(
            IKwfEventHandler<TPayload> eventHandler, 
            string topic,
            string? topipConfigurationKey = null)
        where TPayload : class
        {
            var config = string.IsNullOrEmpty(topipConfigurationKey)
                        ? _defaultConsumerProperties
                        : _configuration.GetConsumerConfiguration(topipConfigurationKey);
            var consumer = new ConsumerBuilder<string, byte[]>(config)
                                .SetLogHandler((c, m) =>
                                {
                                    ConfigureLogger(m);
                                })
                                .SetErrorHandler((c, err) =>
                                {
                                    ConfigureErrorHandler(err, "Consumer");
                                })
                                .Build();
            consumer.Subscribe(topic);

            return new KwfKafkaConsumerHandler<IKwfEventHandler<TPayload>, TPayload>(
                    eventHandler,
                    topic,
                    consumer,
                    config!,
                    _configuration.ConsumerTimeout,
                    _configuration.ConsumerMaxRetries,
                    _jsonSerializerOptions!,
                    _logger);
        }

        private void ConfigureLogger(LogMessage log)
        {
            if (_logger is not null)
            {
                if (_logger.IsEnabled(LogLevel.Debug) && log.Level == SyslogLevel.Debug)
                {
                    _logger.LogDebug(log.Message);
                }

                if (_logger.IsEnabled(LogLevel.Information) &&
                    (log.Level == SyslogLevel.Info || log.Level == SyslogLevel.Notice))
                {
                    _logger.LogInformation(log.Message);
                }

                if (_logger.IsEnabled(LogLevel.Warning) &&
                    (log.Level == SyslogLevel.Warning || log.Level == SyslogLevel.Alert))
                {
                    _logger.LogWarning(log.Message);
                }

                if (_logger.IsEnabled(LogLevel.Critical) && 
                    (log.Level == SyslogLevel.Critical || log.Level == SyslogLevel.Emergency))

                if (_logger.IsEnabled(LogLevel.Error) && log.Level == SyslogLevel.Error)
                {
                    _logger.LogError(log.Message);
                }
            }
        }

        private void ConfigureErrorHandler(Error err, string aux)
        {
            var message = $"Error occurred on kafka {aux}\n Code: {err.Code}, Reason: {err.Reason}";
            if (_logger is not null)
            {
                if (err.IsFatal && _logger.IsEnabled(LogLevel.Critical))
                {
                    _logger.LogCritical(message);
                    return;
                }

                if ((err.IsError || err.IsLocalError || err.IsBrokerError) && _logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(message);
                    return;
                }
            }

            throw new KwfKafkaBusException("KAFKAERR", message);
        }
    }
}
