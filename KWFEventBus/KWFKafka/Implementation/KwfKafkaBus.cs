namespace KWFEventBus.KWFKafka.Implementation
{
    using Confluent.Kafka;

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFKafka.Interfaces;
    using KWFEventBus.KWFKafka.Models;
    using KWFEventBus.KWFKafka.Constants;

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

        public Task ProduceMultipleAsync<T>(T payload, string[] topics, CancellationToken? cancellationToken = null) where T : class
        {
            return ProduceMultipleAsync(payload, topics, null, cancellationToken);
        }

        public async Task ProduceAsync<T>(T payload, string topic, string? key, CancellationToken? cancellationToken = null) where T : class
        {
            try
            {
                var envelope = new EventPayloadEnvelope<T>(payload);
                if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation(Constants.Kafka_log_eventId, "Producing event to topic {TOPIC} with id {ID} and key {KEY}",
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
                    _logger.LogError(Constants.Kafka_log_eventId, "Error occured on producer for topic {TOPIC}\n Reason: {EXCEPTION}", topic, ex.Message);
                }
                
                throw new KwfKafkaBusException("KAFKAPRODERR", $"Error occured during prodution of topic {topic}", ex);
            }
        }

        public Task ProduceMultipleAsync<T>(T payload, string[] topics, string? key, CancellationToken? cancellationToken = null) where T : class
        {
            var produceTasks = new List<Task>();
            try
            {
                var envelope = new EventPayloadEnvelope<T>(payload);
                var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope, _jsonSerializerOptions));

                foreach (var topic in topics)
                {
                    produceTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation(Constants.Kafka_log_eventId, "Producing event to topic {TOPIC} with id {ID} and key {KEY}",
                                    topic,
                                    envelope.Id,
                                    key);
                            }

                            await _producer.ProduceAsync(topic, new Message<string, byte[]>
                            {
                                Headers = _producerHeaders,
                                Key = key!,
                                Value = message,
                                Timestamp = new Timestamp(envelope.TimeStamp)
                            },
                            cancellationToken ?? default);
                        }
                        catch (Exception ex)
                        {
                            if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError(Constants.Kafka_log_eventId, "Error occured on producer for topic {TOPIC}\n Reason: {EXCEPTION}", topic, ex.Message);
                            }

                            throw new KwfKafkaBusException("KAFKAPRODERR", $"Error occured during prodution of topic {topic}", ex);
                        }
                    }));
                }

                return Task.WhenAll(produceTasks);
            }
            catch (Exception ex)
            {
                if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(Constants.Kafka_log_eventId, "Error occured on producer, check exception for details\n{EXCEPTION}", ex.Message);
                }

                throw new KwfKafkaBusException("KAFKAPRODERR", "Error occured on producer", ex);
            }
        }

        public IKwfKafkaEventConsumerHandler CreateConsumer<THandler, TPayload>(
            THandler eventHandler, 
            string topic,
            string? topipConfigurationKey = null)
        where THandler : class, IKwfKafkaEventHandler<TPayload>
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

            return new KwfKafkaConsumerHandler<THandler, TPayload>(
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
            if (_logger is not null && log is not null)
            {
                if (_logger.IsEnabled(LogLevel.Debug) && log.Level == SyslogLevel.Debug)
                {
                    _logger.LogDebug(Constants.Kafka_log_eventId, "{MESSAGE}", log.Message);
                }

                if (_logger.IsEnabled(LogLevel.Information) &&
                    (log.Level == SyslogLevel.Info || log.Level == SyslogLevel.Notice))
                {
                    _logger.LogInformation(Constants.Kafka_log_eventId, "{MESSAGE}", log.Message);
                }

                if (_logger.IsEnabled(LogLevel.Warning) &&
                    (log.Level == SyslogLevel.Warning || log.Level == SyslogLevel.Alert))
                {
                    _logger.LogWarning(Constants.Kafka_log_eventId, "{MESSAGE}", log.Message);
                }

                if (_logger.IsEnabled(LogLevel.Critical) && 
                    (log.Level == SyslogLevel.Critical || log.Level == SyslogLevel.Emergency))

                if (_logger.IsEnabled(LogLevel.Error) && log.Level == SyslogLevel.Error)
                {
                    _logger.LogError(Constants.Kafka_log_eventId, "{MESSAGE}", log.Message);
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
                    _logger.LogCritical(Constants.Kafka_log_eventId, "{MESSAGE}", message);
                    return;
                }

                if ((err.IsError || err.IsLocalError || err.IsBrokerError) && _logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(Constants.Kafka_log_eventId, "{MESSAGE}", message);
                    return;
                }
            }

            throw new KwfKafkaBusException("KAFKAERR", message);
        }
    }
}
