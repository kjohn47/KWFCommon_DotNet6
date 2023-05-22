namespace KWFEventBus.KWFRabbitMQ.Implementation
{    
    using System;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Channels;
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
            _consumerTag = $"{_configuration.GetClientName()}.{topic}.{Guid.NewGuid()}";
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
                        IModel? channel = await GetConsummerChannel();

                        try
                        {
                            if (channel is not null && channel.IsClosed)
                            { 
                                try
                                {
                                    channel.Dispose();
                                }
                                catch 
                                {
                                    channel = null;
                                }

                                continue; 
                            }

                            if (channel is null)
                            {
                                throw new ArgumentNullException(nameof(channel));
                            }

                            var consumer = new EventingBasicConsumer(channel);

                            consumer.Received += ReceivedHandler;

                            consumer.ConsumerCancelled += CancelledHandler;

                            consumer.Shutdown += ShutdownHandler;

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

                            if (channel is not null)
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

                                    if (channel.IsClosed)
                                    {
                                        channel.Dispose();
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    await Task.Delay(_configuration.Timeout);
                }
            });
        }

        public override void StopConsuming()
        {
            {
                _consumeEnabled = false;
                try
                {
                    if (_channel is not null)
                    {
                        try
                        {
                            _channel.BasicCancel(_consumerTag);

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

        private void TryComminMessage(IModel? channel, BasicDeliverEventArgs message, bool notAck = false)
        {
            if (!_autoCommit && message != null && channel is not null && !channel.IsClosed)
            {
                try
                {
                    if (notAck)
                    {
                        channel.BasicNack(message.DeliveryTag, false, _requeue && !message.Redelivered);
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

        private async Task<IModel?> GetConsummerChannel()
        {
            while (IsStarted)
            {
                try
                {
                    return GetChannel();
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

            return null;
        }

        private async void ReceivedHandler(object? c, BasicDeliverEventArgs message)
        {
            var internalConsumer = c as EventingBasicConsumer;
            if (message?.Body is not null)
            {
                try
                {
                    if (message.Redelivered)
                    {
                        await Task.Delay(_configuration.ConsumerRetryDelay);
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

                    TryComminMessage(internalConsumer?.Model, message);
                    _retryCount = _maxRetry;
                }
                catch (Exception ex)
                {
                    TryComminMessage(internalConsumer?.Model, message, true);
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
                                if (internalConsumer?.Model is not null && internalConsumer.Model.IsOpen)
                                {
                                    internalConsumer.Model.BasicCancel(_consumerTag);
                                    await Task.Delay(_configuration.ConsumerRetryDelay);
                                }

                                return;
                            }
                            catch
                            {
                                return;
                            }
                        }
                        _retryCount--;
                        await Task.Delay(_configuration.ConsumerRetryDelay);
                    }
                }
            }
        }

        private void CancelledHandler(object? c, ConsumerEventArgs e)
        {
            var internalConsumer = c as EventingBasicConsumer;
            var internalChannel = internalConsumer?.Model;
            _consumeEnabled = false;
            try
            {
                if (_logger is not null && _logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning(KwfConstants.RabbitMQ_log_eventId, "Channel for consumer for topic {TOPIC} was canceled", _topic);
                }

                try
                {
                    if (internalChannel?.IsOpen ?? false)
                    {
                        internalChannel?.Close();
                    }
                }
                catch
                {
                    internalChannel?.Dispose();
                    _channel?.Dispose();
                }
            }
            catch
            {
                _channel = null;
            }

            if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(KwfConstants.RabbitMQ_log_eventId, "Channel consumer for topic {TOPIC} was closed", _topic);
            }
        }

        private void ShutdownHandler(object? c, ShutdownEventArgs e)
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
                _channel?.Dispose();
            }
            catch
            {
                _channel = null;
            }

            if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(KwfConstants.RabbitMQ_log_eventId, "Channel consumer for topic {TOPIC} was closed", _topic);
            }
        }
    }
}
