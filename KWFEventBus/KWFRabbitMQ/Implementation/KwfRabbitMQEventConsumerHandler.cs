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
    using RabbitMQ.Client.Events;

    public class KwfRabbitMQEventConsumerHandler<THandler, TPayload> : KwfRabbitMQConsumerHandlerBase<THandler, TPayload>, IKwfRabbitMQConsumerHandler, IDisposable
        where THandler : class, IKwfRabbitMQEventHandler<TPayload>
        where TPayload : class
    {
        private readonly string _consumerTag;
        private int _retryCount;
        private bool _allwaysRetry;
        private bool _processStarted;

        public KwfRabbitMQEventConsumerHandler(
            IKwfRabbitMQEventHandler<TPayload> kwfEventHandler,
            string topic,
            Func<IConnection> getConnection,
            KwfRabbitMQConfiguration configuration,
            JsonSerializerOptions jsonSettings,
            ILogger? logger,
            string? configurationKey = null) 
            : base(kwfEventHandler, topic, getConnection, configuration, jsonSettings, logger, configurationKey)
        {
            _consumerTag = string.Concat(_configuration.GetClientName(), '.', topic);
            _retryCount = configuration.ConsumerMaxRetries;
            _allwaysRetry = configuration.ConsumerMaxRetries < 0;
            _processStarted = false;
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
                
                while (IsStarted)
                {
                    if (!_processStarted)
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

                        try
                        {
                            if (channel is null)
                            {
                                throw new ArgumentNullException(nameof(channel));
                            }

                            var consumer = new EventingBasicConsumer(channel);

                            consumer.Received += async (c, message) =>
                            {
                                var internalConsumer = c as EventingBasicConsumer;
                                if (message?.Body is not null)
                                {
                                    try
                                    {
                                        if (!_allwaysRetry && _retryCount == 0 && internalConsumer?.Model is not null && internalConsumer.Model.IsClosed)
                                        {
                                            internalConsumer.Model.BasicNack(message.DeliveryTag, false, true);
                                            try
                                            {
                                                channel.Close();
                                            }
                                            catch { }
                                            return;
                                        }

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

                                        await TryComminMessage(internalConsumer?.Model ?? channel, message);
                                        _retryCount = _maxRetry;
                                    }
                                    catch (Exception ex)
                                    {
                                        await TryComminMessage(internalConsumer?.Model ?? channel, message, true);
                                        if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                                        {
                                            _logger.LogError(KwfConstants.RabbitMQ_log_eventId, ex, "Error occured on consumer for topic {TOPIC}", _topic);
                                        }

                                        if (!_allwaysRetry)
                                        {
                                            if (_retryCount <= 0)
                                            {
                                                if (_logger is not null && _logger.IsEnabled(LogLevel.Warning))
                                                {
                                                    _logger.LogWarning(KwfConstants.RabbitMQ_log_eventId, "Stoping Channel for consumer of topic {TOPIC} was closed", _topic);
                                                }

                                                try
                                                {
                                                    _consumeEnabled = false;
                                                    if (internalConsumer?.Model is not null && internalConsumer.Model.IsOpen)
                                                    {
                                                        internalConsumer.HandleBasicCancel(_consumerTag);
                                                        internalConsumer.Model.Abort();
                                                    }

                                                    return;
                                                }
                                                catch 
                                                {
                                                    return;
                                                }
                                            }
                                            _retryCount--;
                                        }
                                    }
                                }
                            };

                            consumer.Shutdown += (c, message) =>
                            {
                                var internalConsumer = c as EventingBasicConsumer;
                                var internalChannel = internalConsumer?.Model;
                                _processStarted = false;
                                try
                                {
                                    if (_logger is not null && _logger.IsEnabled(LogLevel.Warning))
                                    {
                                        _logger.LogWarning(KwfConstants.RabbitMQ_log_eventId, "Channel for consumer for topic {TOPIC} was closed", _topic);
                                    }

                                    try
                                    {
                                        if (internalChannel?.IsOpen ?? false)
                                        {
                                            internalChannel?.Close();
                                        }
                                    }
                                    catch { }

                                    internalChannel?.Dispose();
                                    channel.Dispose();
                                }
                                catch 
                                {
                                    channel = null;
                                }

                                if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                                {
                                    _logger.LogError(KwfConstants.RabbitMQ_log_eventId, "Channel consumer for topic {TOPIC} was closed", _topic);
                                }
                            };

                            channel.BasicConsume(_topic, _autoCommit, _consumerTag, consumer);
                            
                            while (consumer?.Model is not null && !consumer.IsRunning && !consumer.Model.IsClosed)
                            {
                                await Task.Delay(_configuration.Timeout);
                            }

                            if (consumer?.Model is null)
                            {
                                throw new ArgumentNullException(nameof(consumer), "Channel or consumer were closed before stablishing connection");
                            }

                            _processStarted = consumer.IsRunning;
                        }
                        catch (Exception ex)
                        {
                            _processStarted = false;
                            var kwfEx = new KwfRabbitMQException("RABBITMQCONSUMEERR", $"Error occured during consumption of topic {_topic}", ex);
                            if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError(KwfConstants.RabbitMQ_log_eventId, kwfEx, "Error occured on consumer for topic {TOPIC}", _topic);
                            }

                            if (!_configuration.RetryConsumerReconnect || (!_allwaysRetry && _retryCount == 0))
                            {
                                if (_logger is not null && _logger.IsEnabled(LogLevel.Critical))
                                {
                                    _logger.LogCritical(KwfConstants.RabbitMQ_log_eventId, "Consumer for topic {TOPIC} has stoped", _topic);
                                }

                                _consumeEnabled = false;
                                throw kwfEx;
                            }

                            if (channel is not null && channel.IsClosed)
                            {
                                try
                                {

                                    try
                                    {
                                        if (channel.IsOpen)
                                        {
                                            channel.Close();
                                        }
                                    }
                                    catch { }
                                 channel.Dispose();
                                }
                                catch { }
                            }
                        }
                    }
                    await Task.Delay(_configuration.Timeout);
                }
            });
        }

        private async Task TryComminMessage(IModel channel, BasicDeliverEventArgs message, bool notAck = false)
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
    }
}
