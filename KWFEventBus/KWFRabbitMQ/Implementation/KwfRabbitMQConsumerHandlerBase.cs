﻿namespace KWFEventBus.KWFRabbitMQ.Implementation
{    
    using System;
    using System.Data.Common;
    using System.Text.Json;
    using System.Threading.Tasks;

    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFRabbitMQ.Constants;
    using KWFEventBus.KWFRabbitMQ.Interfaces;
    using KWFEventBus.KWFRabbitMQ.Models;

    using Microsoft.Extensions.Logging;

    using RabbitMQ.Client;

    public abstract class KwfRabbitMQConsumerHandlerBase<THandler, TPayload> : IKwfRabbitMQConsumerHandler, IDisposable
        where THandler : class, IKwfRabbitMQEventHandler<TPayload>
        where TPayload : class
    {
        private readonly Func<IConnection> _getConnection;
        protected readonly ILogger? _logger;
        protected readonly JsonSerializerOptions _jsonSettings;
        protected readonly KwfRabbitMQConfiguration _configuration;
        protected readonly IKwfRabbitMQEventHandler<TPayload> _kwfEventHandler;
        protected readonly string _topic;
        protected readonly int _maxRetry;
        protected readonly string _exchangeName;
        protected readonly string _exchangeLog;
        protected readonly bool _exchangeDurable;
        protected readonly bool _exchangeAutoDelete;
        protected readonly bool _topicDurable;
        protected readonly bool _topicExclusive;
        protected readonly bool _topicAutoDelete;
        protected readonly bool _autoTopicCreate;
        protected readonly bool _autoCommit;
        protected readonly bool _requeue;
        protected readonly bool _dlqEnabled;
        protected readonly string _exchangeType;
        protected readonly IEnumerable<EventBusProperty>? _arguments;
        protected readonly IDictionary<string, object>? _exchangeArgs;
        protected IModel? _channel;
        protected bool _consumeEnabled = false;
        protected bool _disposed;

        public KwfRabbitMQConsumerHandlerBase(
            IKwfRabbitMQEventHandler<TPayload> kwfEventHandler,
            string topic,
            Func<IConnection> getConnection,
            KwfRabbitMQConfiguration configuration,
            JsonSerializerOptions jsonSettings,
            ILogger? logger,
            string? configurationKey = null)
        {
            _kwfEventHandler = kwfEventHandler;
            _jsonSettings = jsonSettings;
            _topic = topic;
            _getConnection = getConnection;
            _configuration = configuration;
            _logger = logger;
            _maxRetry = _configuration.ConsumerMaxRetries;

            var (exchangeName, exchangeDurable, exchangeAutoDelete, exchangeType, exchangeArgs) = _configuration.GetExchangeSettings(configurationKey);
            var (_, topicDurable, topicExclusive, topicAutoDelete, autoTopicCreate, _, dlqEnabled, autoCommit, requeue, _, arguments) = _configuration.GetTopicSettings(configurationKey);
            _exchangeName = exchangeName;
            _exchangeDurable = exchangeDurable;
            _exchangeAutoDelete = exchangeAutoDelete;
            _topicDurable = topicDurable;
            _topicExclusive = topicExclusive;
            _topicAutoDelete = topicAutoDelete;
            _autoTopicCreate = autoTopicCreate;
            _autoCommit = autoCommit;
            _requeue = requeue;
            _dlqEnabled = dlqEnabled;
            _exchangeLog = exchangeName ?? KwfConstants.DefaultExchangeNameLog;
            _exchangeType = exchangeType;
            _arguments = arguments;
            _exchangeArgs = exchangeArgs;
        }

        public bool IsStarted => _consumeEnabled && !_disposed;

        protected IModel GetChannel()
        {
            if (_channel is not null && _channel.IsOpen)
            {
                return _channel;
            }

            IConnection? connection = null;
            try
            {
                connection = _getConnection();
            }
            catch (Exception ex)
            {
                if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(KwfConstants.RabbitMQ_log_eventId, ex, "Error occured on consumer for topic {TOPIC}. Could not open connection or channel", _topic);
                }
            }

            if (connection is null || !connection.IsOpen)
            {
                throw new KwfRabbitMQException("RABBITMQCONERR", "Could not stablish connection with RabbitMQ server or connection was closed");
            }

            var channel = connection.CreateModel();

            if (_autoTopicCreate)
            {
                IDictionary<string, object> args = new Dictionary<string, object>();
                if (_arguments is not null)
                {
                    foreach (var arg in _arguments)
                    {
                        args.Add(arg.PropertyName, arg.PropertyValue);
                    }
                }

                if (_dlqEnabled)
                {
                    var dlqExchangeName = string.IsNullOrEmpty(_exchangeName) ? KwfConstants.DefaultExchangeNameDlq : _exchangeName;
                    var dlqExchange = $"{_configuration.DlqExchangeTag}.{dlqExchangeName}.{_configuration.DlqTag}";
                    var dlqTopic = $"{_topic}.{_configuration.DlqTag}";

                    args.Add(KwfConstants.DlqExchangeKey, dlqExchange);
                    args.Add(KwfConstants.DlqRouteKey, dlqTopic);

                    channel.ExchangeDeclare(dlqExchange, ExchangeType.Direct, true, false);
                    channel.QueueDeclare(dlqTopic, true, false, false);
                    channel.QueueBind(dlqTopic, dlqExchange, dlqTopic);
                }

                channel.QueueDeclare(_topic, _topicDurable, _topicExclusive, _topicAutoDelete, args);

                if (!string.IsNullOrEmpty(_exchangeName))
                {
                    channel.ExchangeDeclare(_exchangeName, _exchangeType, _exchangeDurable, _exchangeAutoDelete, _exchangeArgs);
                    channel.QueueBind(_topic, _exchangeName, _topic, args);
                }
            }

            channel.BasicQos(0, 1, false);
            var properties = channel.CreateBasicProperties();
            properties.AppId = _configuration.AppName;
            properties.ClusterId = _configuration.AppName;

            _channel = channel;
            return _channel;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_channel is not null)
                {
                    try
                    {
                        if (_channel.IsOpen)
                        {
                            _channel.Close();
                        }
                    }
                    catch { }
                    _channel.Dispose();
                }
            }
        }

        public abstract void StartConsuming();
        
        public abstract void StopConsuming();
    }
}
