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
            _producer = new ProducerBuilder<string, byte[]>(_defaultProducerProperties).Build();

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

        public Task ProduceAsync<T>(T payload, string topic, string? key, CancellationToken? cancellationToken = null) where T : class
        {
            var envelope = new EventPayloadEnvelope<T>(payload);

            return _producer.ProduceAsync(topic, new Message<string, byte[]>
            {
                Headers = _producerHeaders,
                Key = key!,
                Value = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope, _jsonSerializerOptions)),
                Timestamp = new Timestamp(envelope.TimeStamp)
            },
            cancellationToken ?? default);
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
            var consumer = new ConsumerBuilder<string, byte[]>(config).Build();
            consumer.Subscribe(topic);

            return new KwfKafkaConsumerHandler<IKwfEventHandler<TPayload>, TPayload>(
                    eventHandler,
                    topic,
                    consumer,
                    config!,
                    _configuration.ConsumerTimeout,
                    _jsonSerializerOptions!,
                    _logger);
        }
    }
}
