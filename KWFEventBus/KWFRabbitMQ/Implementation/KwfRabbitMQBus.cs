namespace KWFEventBus.KWFRabbitMQ.Implementation
{
    using System;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFRabbitMQ.Constants;
    using KWFEventBus.KWFRabbitMQ.Interfaces;
    using KWFEventBus.KWFRabbitMQ.Models;

    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;

    public class KwfRabbitMQBus : IKwfRabbitMQBus, IDisposable
    {
        private readonly KwfRabbitMQConfiguration _configuration;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IEnumerable<AmqpTcpEndpoint> _endpoints;
        private readonly string _producer;
        private readonly string? _defaultProducerProperties;
        private readonly string? _defaultConsumerProperties;
        private readonly ILogger? _logger;
        private bool _disposed;

        public KwfRabbitMQBus(KwfRabbitMQConfiguration configuration, ILoggerFactory? loggerFactory, JsonSerializerOptions? jsonSerializerOptions = null) 
        {
            if (configuration?.Endpoints is null || !configuration.Endpoints.Any())
            {
                throw new ArgumentNullException(nameof(configuration), "Configuration endpoints cannot be null");
            }

            _configuration = configuration;
            _jsonSerializerOptions = jsonSerializerOptions ?? EventsJsonOptions.GetJsonOptions();
            _endpoints = _configuration.Endpoints!.Select(e => new AmqpTcpEndpoint(e.Url, e.Port));

            _connectionFactory = new ConnectionFactory
            {
                ClientProvidedName = _configuration.AppName,
                EndpointResolverFactory = _ => {
                    return new DefaultEndpointResolver(_endpoints);
                },
                UserName = _configuration.UserName,
                Password = _configuration.Password
            };

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

        public Task ProduceAsync<T>(T payload, string topic, string? key, CancellationToken? cancellationToken = null) where T : class
        {
            return Task.Run(() =>
            {
                try
                {
                    using var connection = _connectionFactory.CreateConnection();
                    using var channel = connection.CreateModel();
                    channel.QueueDeclare(topic);
                    var envelope = new EventPayloadEnvelope<T>(payload);
                    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope, _jsonSerializerOptions));

                    var properties = channel.CreateBasicProperties();
                    properties.AppId = _configuration.AppName;
                    properties.MessageId = envelope.Id.ToString();
                    properties.Headers.Add("host-name", _configuration.ClientName);
                    properties.Headers.Add("application-name", _configuration.AppName);
                    channel.BasicPublish(string.Empty, topic, properties, message);
                    channel.WaitForConfirmsOrDie(TimeSpan.FromMilliseconds(_configuration.ProducerTimeout));
                    if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation(KwfConstants.RabbitMQ_log_eventId, "Producing event to topic {0} with id {1} and key {2}",
                            topic,
                            envelope.Id,
                            key);
                    }
                }
                catch (Exception ex)
                {
                    if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError(KwfConstants.RabbitMQ_log_eventId, "Error occured on producer for topic {0}\n Reason: {1}", topic, ex.Message);
                    }

                    throw new KwfRabbitMQException("RABBITMQPRODERR", $"Error occured during prodution of topic {topic}", ex);
                }
            });
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
