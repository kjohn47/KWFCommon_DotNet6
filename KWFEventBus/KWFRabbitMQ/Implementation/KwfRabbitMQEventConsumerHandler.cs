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
                bool processStarted = false;
                while (IsStarted)
                {
                    if (!processStarted)
                    {
                        IModel? channel = null;
                        while (IsStarted && retry != 0)
                        {
                            try
                            {
                                channel = GetChannel();
                                retry = _maxRetry;
                                break;
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

                        try
                        {
                            if (channel is null)
                            {
                                throw new ArgumentNullException(nameof(channel));
                            }

                            var consumer = new EventingBasicConsumer(channel);

                            consumer.Received += async (model, message) =>
                            {
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
                            };

                            consumer.Shutdown += (model, message) =>
                            {
                                processStarted = false;
                                try
                                {
                                    if (_logger is not null && _logger.IsEnabled(LogLevel.Warning))
                                    {
                                        _logger.LogWarning(KwfConstants.RabbitMQ_log_eventId, "Channel for consumer for topic {TOPIC} was closed", _topic);
                                    }

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
                                channel = null;

                                if (_logger is not null && _logger.IsEnabled(LogLevel.Error))
                                {
                                    _logger.LogError(KwfConstants.RabbitMQ_log_eventId, "Channel consumer for topic {TOPIC} was closed", _topic);
                                }
                            };

                            channel.BasicConsume(_topic, _autoCommit, _configuration.GetClientName(), consumer);
                            
                            while (consumer?.Model is not null && !consumer.IsRunning && !consumer.Model.IsClosed)
                            {
                                await Task.Delay(_configuration.Timeout);
                            }

                            if (consumer?.Model is null)
                            {
                                throw new ArgumentNullException(nameof(consumer), "Channel or consumer were closed before stablishing connection");
                            }

                            processStarted = consumer.IsRunning;
                        }
                        catch (Exception ex)
                        {
                            processStarted = false;
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

        private void TryComminMessage(IModel channel, BasicDeliverEventArgs message)
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
