namespace KWFEventBus.KWFRabbitMQ.Implementation
{
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFRabbitMQ.Interfaces;
    using KWFEventBus.KWFRabbitMQ.Models;

    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;

    public class KwfRabbitMQBus : IKwfRabbitMQBus, IDisposable
    {
        private readonly KwfRabbitMQConfiguration _configuration;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;
        private readonly string _producer;
        private readonly string? _defaultProducerProperties;
        private readonly string? _defaultConsumerProperties;
        private readonly ILogger? _logger;
        private bool _disposed;

        public KwfRabbitMQBus(KwfRabbitMQConfiguration configuration, ILoggerFactory? loggerFactory, JsonSerializerOptions? jsonSerializerOptions = null) 
        {
            _configuration = configuration;
            _jsonSerializerOptions = jsonSerializerOptions ?? EventsJsonOptions.GetJsonOptions();
            //_defaultProducerProperties = _configuration.GetProducerConfiguration();
            //_defaultConsumerProperties = _configuration.GetConsumerConfiguration();

            //_producer = ;

            if (loggerFactory is not null)
            {
                _logger = loggerFactory.CreateLogger<KwfRabbitMQBus>();
            }
        }

        public Task ProduceAsync<T>(T payload, string topic, CancellationToken? cancellationToken = null) where T : class
        {
            return ProduceAsync(payload, topic, null, cancellationToken);
        }

        public async Task ProduceAsync<T>(T payload, string topic, string? key, CancellationToken? cancellationToken = null) where T : class
        {
            await Task.Yield();
            /*
            try
            {
                var envelope = new EventPayloadEnvelope<T>(payload);
                if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation(Constants.RabbitMQ_log_eventId, "Producing event to topic {0} with id {1} and key {2}",
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
                    _logger.LogError(Constants.RabbitMQ_log_eventId, "Error occured on producer for topic {0}\n Reason: {1}", topic, ex.Message);
                }
                
                throw new KwfKafkaBusException("KAFKAPRODERR", $"Error occured during prodution of topic {topic}", ex);
            }
            */
        }

        public IKwfRabbitMQConsumerHandler CreateConsumer<THandler, TPayload>(THandler eventHandler, string topic, string? topipConfigurationKey = null)
            where THandler : class, IKwfRabbitMQEventHandler<TPayload>
            where TPayload : class
        {
            /*
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
            */

            return new KwfRabbitMQConsumerHandler<THandler, TPayload>(
                    eventHandler,
                    topic,
                    "consumer", //consumer from MQ client
                    _configuration, //to replace with 'config' from key above
                    _configuration.ConsumerTimeout,
                    _configuration.ConsumerMaxRetries,
                    _jsonSerializerOptions!,
                    _logger);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                // Dispose additional
            }
        }
    }
}
