namespace KWFEventBus.KWFKafka.Implementation
{
    using Confluent.Kafka;

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFKafka.Interfaces;
    using KWFEventBus.KWFKafka.Models;

    using System.Text;
    using System.Text.Json;

    public class KwfKafkaBus : IKwfKafkaBus, IDisposable
    {
        private readonly KwfKafkaConfiguration _configuration;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;
        private readonly IProducer<string, byte[]> _producer;
        private readonly ProducerConfig? _defaultProducerProperties;
        private readonly ConsumerConfig? _defaultConsumerProperties;
        private readonly string _serverEndpoints;
        private bool _disposed;

        public KwfKafkaBus(KwfKafkaConfiguration configuration, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            if (string.IsNullOrEmpty(configuration.AppName))
            {
                throw new ArgumentNullException(nameof(configuration.AppName));
            }

            _configuration = configuration;
            _jsonSerializerOptions = jsonSerializerOptions ?? EventsJsonOptions.GetJsonOptions();
            _serverEndpoints = configuration.GetServerEndpoints();

            _defaultProducerProperties = new ProducerConfig(configuration.GetProducerProperties())
            {
                BootstrapServers = _serverEndpoints,
                MessageTimeoutMs = 5000,
                RequestTimeoutMs = 5000
            };

            _defaultConsumerProperties = new ConsumerConfig(configuration.GetConsumerProperties())
            {
                BootstrapServers = _serverEndpoints,
                GroupId = _configuration.AppName
            };

            _producer = new ProducerBuilder<string, byte[]>(_defaultProducerProperties).Build();
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
                Key = key!,
                Value = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope, _jsonSerializerOptions)),
                Timestamp = new Timestamp(envelope.TimeStamp)
            },
            cancellationToken ?? default);
        }

        public (IConsumer<string, byte[]>, ConsumerConfig) CreateConsumer(string? topipConfigurationKey = null)
        {
            var config = string.IsNullOrEmpty(topipConfigurationKey)
                        ? _defaultConsumerProperties
                        : new ConsumerConfig(_configuration.GetConsumerProperties(topipConfigurationKey))
                        {
                            BootstrapServers = _serverEndpoints,
                            GroupId = _configuration.AppName
                        };

            return (new ConsumerBuilder<string, byte[]>(config).Build(), config!);
        }
    }
}
