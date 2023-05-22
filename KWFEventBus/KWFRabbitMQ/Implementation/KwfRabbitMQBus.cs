namespace KWFEventBus.KWFRabbitMQ.Implementation
{
    using System;
    using System.Reflection.PortableExecutable;
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
        private readonly ILogger? _logger;
        private readonly TimeSpan _timeoutProducerAck;
        private IConnection? _connection;
        private bool _disposed;
        private object _connectionOpenLock;

        public KwfRabbitMQBus(KwfRabbitMQConfiguration configuration, ILoggerFactory? loggerFactory, JsonSerializerOptions? jsonSerializerOptions = null) 
        {
            if (configuration?.Endpoints is null || !configuration.Endpoints.Any())
            {
                throw new ArgumentNullException(nameof(configuration), "Configuration endpoints cannot be null");
            }

            _configuration = configuration;
            _jsonSerializerOptions = jsonSerializerOptions ?? EventsJsonOptions.GetJsonOptions();
            _endpoints = _configuration.Endpoints!.Select(e => new AmqpTcpEndpoint(e.Url, e.Port));
            _timeoutProducerAck = TimeSpan.FromMilliseconds(_configuration.ProducerAckTimeout);

            var timeout = TimeSpan.FromMilliseconds(_configuration.Timeout);
            _connectionFactory = new ConnectionFactory
            {
                ClientProvidedName = _configuration.GetClientName(),
                EndpointResolverFactory = _ => {
                    return new DefaultEndpointResolver(_endpoints);
                },
                UserName = _configuration.UserName,
                Password = _configuration.Password,
                RequestedConnectionTimeout = timeout,
                SocketReadTimeout = timeout,
                SocketWriteTimeout = timeout,
                ContinuationTimeout = timeout,
                HandshakeContinuationTimeout = timeout,
                RequestedHeartbeat = TimeSpan.FromMilliseconds(_configuration.HeartBeat)
            };

            _connectionOpenLock = new Object();

            if (loggerFactory is not null)
            {
                _logger = loggerFactory.CreateLogger<KwfRabbitMQBus>();
            }

            try
            {
                GetConnection();
            }
            catch (Exception ex)
            {
                _connection = null;
                if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(KwfConstants.RabbitMQ_log_eventId, "Error occured during connection opening\n Reason: {EXCEPTION}", ex.Message);
                }
            }
        }

        public Task ProduceAsync<T>(T payload, string topic, CancellationToken? cancellationToken = null) where T : class
        {
            return ProduceAsync(payload, topic, null, cancellationToken);
        }

        public Task ProduceAsync<T>(T payload, string topic, string? configurationKey, CancellationToken? cancellationToken = null) where T : class
        {
            return ProduceAsync(payload, topic, topic, configurationKey, cancellationToken);
        }

        public Task ProduceMultipleAsync<T>(T payload, string[] topics, CancellationToken? cancellationToken = null) where T : class
        {
            return ProduceMultipleAsync(payload, topics, null, cancellationToken);
        }

        public Task ProduceAsync<T>(T payload, string topic, string pattern, string? configurationKey, CancellationToken? cancellationToken = null) where T : class
        {
            return Task.Run(() =>
            {
                var (exchangeName, exchangeDurable, exchangeAutoDelete, exchangeType, exchangeArgs) = _configuration.GetExchangeSettings(configurationKey);
                var (messagePersistent, topicDurable, topicExclusive, topicAutoDelete, autoTopicCreate, topicWaitAck, dlqEnabled, _, _, headers, arguments) = _configuration.GetTopicSettings(configurationKey);
                var exchangeLog = exchangeName ?? KwfConstants.DefaultExchangeNameLog;

                try
                {
                    using var channel = GetConnection().CreateModel();
                    var envelope = new EventPayloadEnvelope<T>(payload);
                    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope, _jsonSerializerOptions));

                    if (topicWaitAck)
                    {
                        channel.ConfirmSelect();
                        if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                        {
                            channel.BasicAcks += (_, e) =>
                            {
                                _logger.LogInformation(KwfConstants.RabbitMQ_log_eventId, "Sending Event to topic {TOPIC} with id {ID} and exchange {EXCHANGE} Ack",
                                topic,
                                envelope.Id,
                                exchangeLog);
                            };

                            channel.BasicNacks += (_, e) =>
                            {
                                _logger.LogInformation(KwfConstants.RabbitMQ_log_eventId, "Sending Event to topic {TOPIC} with id {ID} and exchange {EXCHANGE} failed ack or timeout",
                                topic,
                                envelope.Id,
                                exchangeLog);
                            };
                        }
                    }

                    if (autoTopicCreate)
                    {
                        IDictionary<string, object> args = new Dictionary<string, object>();

                        if (arguments is not null)
                        {
                            foreach (var arg in arguments)
                            {
                                args.Add(arg.PropertyName, arg.PropertyValue);
                            }
                        }

                        if (dlqEnabled)
                        {
                            var dlqExchangeName = string.IsNullOrEmpty(exchangeName) ? KwfConstants.DefaultExchangeNameDlq : exchangeName;
                            var dlqExchange = $"{_configuration.DlqExchangeTag}.{dlqExchangeName}.{_configuration.DlqTag}";
                            var dlqTopic = $"{topic}.{_configuration.DlqTag}";

                            args.Add(KwfConstants.DlqExchangeKey, dlqExchange);
                            args.Add(KwfConstants.DlqRouteKey, dlqTopic);

                            channel.ExchangeDeclare(dlqExchange, ExchangeType.Direct, true, false);
                            channel.QueueDeclare(dlqTopic, true, false, false);
                            channel.QueueBind(dlqTopic, dlqExchange, dlqTopic);
                        }

                        channel.QueueDeclare(topic, topicDurable, topicExclusive, topicAutoDelete, args);

                        if (!string.IsNullOrEmpty(exchangeName))
                        {
                            channel.ExchangeDeclare(exchangeName, exchangeType, exchangeDurable, exchangeAutoDelete, exchangeArgs);
                            channel.QueueBind(topic, exchangeName, topic, args);
                        }
                    }

                    var properties = channel.CreateBasicProperties();
                    properties.AppId = _configuration.AppName;
                    properties.ClusterId = _configuration.AppName;
                    properties.Persistent = messagePersistent;
                    properties.MessageId = envelope.Id.ToString();
                    properties.Headers = new Dictionary<string, object>
                    {
                        { KwfConstants.HostNameHeader, _configuration.ClientName },
                        { KwfConstants.ApplicationNameHeader, _configuration.AppName }
                    };

                    if (headers is not null)
                    {
                        foreach (var header in headers)
                        {
                            properties.Headers.Add(header.PropertyName, header.PropertyValue);
                        }
                    }

                    if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation(KwfConstants.RabbitMQ_log_eventId, "Sending event to topic {TOPIC} with id {ID} and exchange {EXCHANGE}",
                            topic,
                            envelope.Id,
                            exchangeLog);
                    }

                    channel.BasicPublish(exchangeName, pattern, properties, message.AsMemory());

                    if (topicWaitAck)
                    {
                        channel.WaitForConfirmsOrDie(_timeoutProducerAck);
                    }
                }
                catch (Exception ex)
                {
                    if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError(KwfConstants.RabbitMQ_log_eventId, "Error occured on producer for topic {TOPIC} on exchange {EXCHANGE}\n Reason: {EXCEPTION}", topic, exchangeLog, ex.Message);
                    }

                    throw new KwfRabbitMQException("RABBITMQPRODERR", $"Error occured during prodution of topic {topic} on exchange {exchangeLog}", ex);
                }
            });
        }

        public Task ProduceMultipleAsync<T>(T payload, string[] topics, string? configurationKey, CancellationToken? cancellationToken = null) where T : class
        {
            return Task.Run(() =>
            {
                var (exchangeName, exchangeDurable, exchangeAutoDelete, exchangeType, exchangeArgs) = _configuration.GetExchangeSettings(configurationKey);
                var (messagePersistent, topicDurable, topicExclusive, topicAutoDelete, autoTopicCreate, topicWaitAck, dlqEnabled, _, _, headers, arguments) = _configuration.GetTopicSettings(configurationKey);
                var exchangeLog = string.IsNullOrEmpty(exchangeName) ?  KwfConstants.DefaultExchangeNameLog : exchangeName;
                var topicsLogString = string.Join(',', topics);
                var dlqExchange = string.Empty;

                try
                {
                    using var channel = GetConnection().CreateModel();
                    var envelope = new EventPayloadEnvelope<T>(payload);
                    var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope, _jsonSerializerOptions));

                    if (topicWaitAck)
                    {
                        channel.ConfirmSelect();
                        if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                        {
                            channel.BasicAcks += (_, e) =>
                            {
                                _logger.LogInformation(KwfConstants.RabbitMQ_log_eventId, "Sending Event with tag {TAG} to topics {TOPIC} with id {ID} and exchange {EXCHANGE} Ack",
                                e.DeliveryTag,
                                topicsLogString,
                                envelope.Id,
                                exchangeLog);
                            };

                            channel.BasicNacks += (_, e) =>
                            {
                                _logger.LogInformation(KwfConstants.RabbitMQ_log_eventId, "Sending Event with tag {TAG} to topics {TOPIC} with id {ID} and exchange {EXCHANGE} failed ack or timeout",
                                e.DeliveryTag,
                                topicsLogString,
                                envelope.Id,
                                exchangeLog);
                            };
                        }
                    }

                    var properties = channel.CreateBasicProperties();
                    properties.AppId = _configuration.AppName;
                    properties.ClusterId = _configuration.AppName;
                    properties.Persistent = messagePersistent;
                    properties.MessageId = envelope.Id.ToString();
                    properties.Headers = new Dictionary<string, object>
                    {
                        { KwfConstants.HostNameHeader, _configuration.ClientName },
                        { KwfConstants.ApplicationNameHeader, _configuration.AppName }
                    };

                    if (headers is not null)
                    {
                        foreach (var header in headers)
                        {
                            properties.Headers.Add(header.PropertyName, header.PropertyValue);
                        }
                    }

                    var batch = channel.CreateBasicPublishBatch();

                    IDictionary<string, object> args = new Dictionary<string, object>();                    
                    if (arguments is not null)
                    {
                        foreach (var arg in arguments)
                        {
                            args.Add(arg.PropertyName, arg.PropertyValue);
                        }
                    }

                    if (autoTopicCreate)
                    {
                        if (dlqEnabled)
                        {
                            var dlqExchangeName = string.IsNullOrEmpty(exchangeName) ? KwfConstants.DefaultExchangeNameDlq : exchangeName;
                            dlqExchange = $"{_configuration.DlqExchangeTag}.{dlqExchangeName}.{_configuration.DlqTag}";

                            args.Add(KwfConstants.DlqExchangeKey, dlqExchange);
                            channel.ExchangeDeclare(dlqExchange, ExchangeType.Direct, true, false);
                        }

                        if (!string.IsNullOrEmpty(exchangeName))
                        {
                            channel.ExchangeDeclare(exchangeName, exchangeType, exchangeDurable, exchangeAutoDelete, exchangeArgs);
                        }
                    }

                    foreach (var topic in topics)
                    {
                        if (autoTopicCreate)
                        {
                            if (dlqEnabled)
                            {
                                var dlqTopic = $"{topic}.{_configuration.DlqTag}";

                                args.Add(KwfConstants.DlqRouteKey, dlqTopic);
                                channel.ExchangeDeclare(dlqExchange, ExchangeType.Direct, true, false);
                                channel.QueueDeclare(dlqTopic, true, false, false);
                                channel.QueueBind(dlqTopic, dlqExchange, dlqTopic);
                            }

                            channel.QueueDeclare(topic, topicDurable, topicExclusive, topicAutoDelete, args);

                            if (!string.IsNullOrEmpty(exchangeName))
                            {
                                channel.QueueBind(topic, exchangeName, topic, args);
                            }
                        }

                        if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation(KwfConstants.RabbitMQ_log_eventId, "Adding topic {TOPIC} with id {ID} and exchange {EXCHANGE} to publish batch",
                                topic,
                                envelope.Id,
                                exchangeLog);
                        }

                        batch.Add(exchangeName, topic, false, properties, message.AsMemory());
                    }

                    batch.Publish();

                    if (topicWaitAck)
                    {
                        channel.WaitForConfirmsOrDie(_timeoutProducerAck);
                    }
                }
                catch (Exception ex)
                {
                    if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError(KwfConstants.RabbitMQ_log_eventId, "Error occured on producer for topic {TOPIC} on exchange {EXCHANGE}\n Reason: {EXCEPTION}", topicsLogString, exchangeLog, ex.Message);
                    }

                    throw new KwfRabbitMQException("RABBITMQPRODERR", $"Error occured during prodution of topic {topicsLogString} on exchange {exchangeLog}", ex);
                }
            });
        }

        public IKwfRabbitMQConsumerHandler CreateConsumer<THandler, TPayload>(THandler eventHandler, string topic, string? configurationKey = null)
            where THandler : class, IKwfRabbitMQEventHandler<TPayload>
            where TPayload : class
        {
            try
            {
                if (_configuration.UsePolling)
                {
                    return new KwfRabbitMQPollingConsumerHandler<THandler, TPayload>(
                            eventHandler,
                            topic,
                            GetConnection,
                            _configuration,
                            _jsonSerializerOptions!,
                            _logger,
                            configurationKey);
                }

                return new KwfRabbitMQEventConsumerHandler<THandler, TPayload>(
                            eventHandler,
                            topic,
                            GetConnection,
                            _configuration,
                            _jsonSerializerOptions!,
                            _logger,
                            configurationKey);

            }
            catch (Exception ex)
            {
                if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(KwfConstants.RabbitMQ_log_eventId, "Error occured on instantiating consumer handler for topic {TOPIC}\n Reason: {EXCEPTION}", topic, ex.Message);
                }

                throw new KwfRabbitMQException("RABBITMQPRODERR", $"Error occured on instantiating consumer handler for topic {topic}", ex);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_connection is not null)
                {
                    try
                    {
                        try
                        {
                            if (_connection.IsOpen)
                            {
                                _connection.Close();
                            }
                        }
                        catch { }

                        _connection.Dispose();
                    }
                    catch { }
                }
            }
        }

        private IConnection GetConnection()
        {
            if (_connection is null)
            {
                lock (_connectionOpenLock)
                {
                    _connection ??= _connectionFactory.CreateConnection();
                }
            }

            if (!_connection.IsOpen)
            {
                lock (_connectionOpenLock)
                {
                    if (!_connection.IsOpen)
                    {
                        try
                        {
                            _connection.Dispose();
                        }
                        catch { }
                        _connection = null;
                        _connection = _connectionFactory.CreateConnection();
                    }
                }
            }

            return _connection;
        }
    }
}
