namespace KWFEventBus.KWFRabbitMQ.Implementation
{    
    using System;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFRabbitMQ.Constants;
    using KWFEventBus.KWFRabbitMQ.Interfaces;
    using KWFEventBus.KWFRabbitMQ.Models;

    using Microsoft.Extensions.Logging;

    using RabbitMQ.Client;

    public class KwfRabbitMQPollingConsumerHandler<THandler, TPayload> : KwfRabbitMQConsumerHandlerBase<THandler, TPayload>, IKwfRabbitMQConsumerHandler, IDisposable
        where THandler : class, IKwfRabbitMQEventHandler<TPayload>
        where TPayload : class
    {
        private readonly int _pollingInterval;

        public KwfRabbitMQPollingConsumerHandler(
            IKwfRabbitMQEventHandler<TPayload> kwfEventHandler,
            string topic,
            Func<IConnection> getConnection,
            KwfRabbitMQConfiguration configuration,
            JsonSerializerOptions jsonSettings,
            ILogger? logger,
            string? configurationKey = null) 
            : base(kwfEventHandler, topic, getConnection, configuration, jsonSettings, logger, configurationKey)
        {
            _pollingInterval = configuration.ConsumerPollingInterval;
        }

        public override void StartConsuming()
        {
            if (IsStarted || _disposed)
            {
                return;
            }

            _consumeEnabled = true;
            Task.Run(async () =>
            {
                var retry = _maxRetry;
                var alwaysRetry = _maxRetry == -1;
                bool messageProcessException = false;

                while (IsStarted)
                {
                    await Task.Delay(_pollingInterval);
                    try
                    {
                        var channel = await GetChannelAsync();
                        var message = channel.BasicGet(_topic, _autoCommit);
                        retry = messageProcessException ? retry : _maxRetry;

                        if (message?.Body is not null)
                        {
                            try
                            {
                                var payloadObj = JsonSerializer.Deserialize<EventPayloadEnvelope<TPayload>>(
                                                        Encoding.UTF8.GetString(message.Body.ToArray()),
                                                        _jsonSettings);

                                if (payloadObj is not null)
                                {
                                    if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                                    {
                                        _logger.LogInformation(KwfConstants.RabbitMQ_log_eventId, "Consuming event from topic {TOPIC} with id {ID} and tag {TAG}",
                                            _topic,
                                            payloadObj.Id,
                                            message.DeliveryTag);
                                    }

                                    await _kwfEventHandler.HandleEventAsync(payloadObj);
                                }

                                TryComminMessage(channel, message);
                                retry = _maxRetry;
                                messageProcessException = false;
                            }
                            catch
                            {
                                TryComminMessage(channel, message);
                                messageProcessException = true;
                                throw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var kwfEx = new KwfRabbitMQException("RABBITMQCONSUMEERR", $"Error occured during consumption of topic {_topic}", ex);
                        if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(KwfConstants.RabbitMQ_log_eventId, kwfEx, "Error occured on consumer for topic {TOPIC}", _topic);
                        }

                        if (!alwaysRetry)
                        {
                            if (retry == 0)
                            {
                                if (_logger is not null && _logger.IsEnabled(LogLevel.Critical))
                                {
                                    _logger.LogCritical(KwfConstants.RabbitMQ_log_eventId, "Consumer for topic {TOPIC} has stoped", _topic);
                                }

                                _consumeEnabled = false;
                                throw kwfEx;
                            }

                            retry--;
                        }
                    }
                }
            });
        }

        private void TryComminMessage(IModel channel, BasicGetResult message)
        {
            if (!_autoCommit && message != null)
            {
                try
                {
                    channel.BasicAck(message.DeliveryTag, false);
                }
                catch
                {
                    if (_logger is not null && _logger.IsEnabled(LogLevel.Warning))
                    {
                        _logger.LogWarning(KwfConstants.RabbitMQ_log_eventId, "Error occurred during commit of topic {TOPIC}", _topic);
                    }
                }
            }
        }
    }
}
