namespace KWFEventBus.KWFKafka.Implementation
{
    using Confluent.Kafka;

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFKafka.Interfaces;
    using KWFEventBus.KWFKafka.Models;

    using System.Text;
    using System.Text.Json;

    internal class KwfEventConsumerHandler<THandler, TPayload> : IKwfEventConsumerHandler, IDisposable
        where THandler : class, IKwfEventHandler<TPayload>
        where TPayload : class
    {
        private readonly IKwfEventHandler<TPayload> _kwfEventHandler;
        private readonly IKwfKafkaBus _kwfKafkaBus;
        private readonly JsonSerializerOptions _kafkaJsonSettings;
        private readonly ConsumerConfig _consumerConfig;
        private readonly IConsumer<string, byte[]>? _consumer;
        private readonly string _topic;
        private readonly string? _topicConfigurationKey;
        private bool _consumeEnabled = true;
        bool _disposed;

        public KwfEventConsumerHandler(
            IKwfEventHandler<TPayload> kwfEventHandler, 
            IKwfKafkaBus kwfKafkaBus,
            string topic,
            JsonSerializerOptions kafkaJsonSettings,
            string? topicConfigurationKey = null)
        {
            _kwfKafkaBus = kwfKafkaBus;
            _kwfEventHandler = kwfEventHandler;
            _kafkaJsonSettings = kafkaJsonSettings;
            _topic = topic;
            _topicConfigurationKey = topicConfigurationKey;
            (_consumer, _consumerConfig) = _kwfKafkaBus.CreateConsumer(_topicConfigurationKey);
            _consumer.Subscribe(_topic);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_consumer is not null)
                {
                    _consumer.Close();
                }

            }
        }

        public void StartConsuming()
        {
            if (_consumer is not null)
            {
                Task.Run(async () =>
                {
                    while (!_disposed && _consumeEnabled)
                    {
                        var message = _consumer.Consume(5000);
                        if (message is not null && !message.IsPartitionEOF && message.Message.Value is not null)
                        {
                            try
                            {
                                var payloadObj = JsonSerializer.Deserialize<EventPayloadEnvelope<TPayload>>(
                                                    Encoding.UTF8.GetString(message.Message.Value),
                                                    _kafkaJsonSettings);

                                if (payloadObj is not null)
                                {
                                    await _kwfEventHandler.HandleEventAsync(payloadObj);
                                }

                                if (_consumerConfig?.EnableAutoCommit is null || _consumerConfig.EnableAutoCommit.Value == false)
                                {
                                    try
                                    {
                                        _consumer.Commit(message);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new KwfKafkaBusException("KAFKACONSUMEERR", $"Error occured during consumption of topic {_topic}", ex);
                            }
                        }
                    }
                });
            }
        }

        public void StopConsuming()
        {
            _consumeEnabled = false;
            if (_consumer is not null)
            {
                _consumer.Close();
            }
        }
    }
}
