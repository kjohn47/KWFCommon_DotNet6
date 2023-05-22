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

                while (IsStarted)
                {
                    await Task.Delay(_pollingInterval);
                    try
                    {
                        IModel? channel = null;
                        while (IsStarted)
                        {
                            try
                            {
                                channel = GetChannel();
                                break;
                            }
                            catch (Exception ex)
                            {
                                var kwfEx = new KwfRabbitMQException("RABBITMQCONSUMEERR", $"Error occured openning connection for consumer of topic {_topic}", ex);
                                if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                                {
                                    _logger.LogError(KwfConstants.RabbitMQ_log_eventId, kwfEx, "Error occured openning connection for consumer of topic {TOPIC}", _topic);
                                }

                                if (!_configuration.RetryConsumerReconnect)
                                {
                                    if (_logger is not null && _logger.IsEnabled(LogLevel.Critical))
                                    {
                                        _logger.LogCritical(KwfConstants.RabbitMQ_log_eventId, "Consumer for topic {TOPIC} has stoped due to failed connection", _topic);
                                    }

                                    _consumeEnabled = false;
                                    throw kwfEx;
                                }

                                await Task.Delay(_configuration.HeartBeat);
                            }
                        }

                        if (channel is null)
                        {
                            throw new ArgumentNullException(nameof(channel));
                        }

                        var message = channel.BasicGet(_topic, _autoCommit);

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

                                await TryComminMessage(channel, message);
                                retry = _maxRetry;
                            }
                            catch
                            {
                                await TryComminMessage(channel, message, true);
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

                        await Task.Delay(_configuration.HeartBeat);
                    }
                }
            });
        }

        private async Task TryComminMessage(IModel channel, BasicGetResult message, bool notAck = false)
        {
            if (!_autoCommit && message != null)
            {
                try
                {
                    if (notAck)
                    {
                        channel.BasicNack(message.DeliveryTag, false, _requeue && !message.Redelivered);
                        await Task.Delay(_configuration.ConsumerRetryDelay);
                        return;
                    }

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

        public override void StopConsuming()
        {
            {
                _consumeEnabled = false;
                try
                {
                    if (_channel != null)
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
                catch
                { }
                _channel = null;
            }
        }
    }
}
