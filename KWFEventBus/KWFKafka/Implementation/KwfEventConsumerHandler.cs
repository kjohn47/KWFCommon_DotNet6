namespace KWFEventBus.KWFKafka.Implementation
{
    using Confluent.Kafka;

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.KWFKafka.Interfaces;

    internal class KwfEventConsumerHandler<THandler, TPayload> : IDisposable
        where THandler : class, IKwfEventHandler<TPayload>
        where TPayload : class
    {
        private readonly IKwfEventHandler<TPayload> _kwfEventHandler;
        private readonly IKwfKafkaBus _kwfKafkaBus;
        private IConsumer<string, byte[]>? _consumer;
        bool _disposed;

        public KwfEventConsumerHandler(IKwfEventHandler<TPayload> kwfEventHandler, IKwfKafkaBus kwfKafkaBus)
        {
            _kwfKafkaBus = kwfKafkaBus;
            _kwfEventHandler = kwfEventHandler;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_consumer != null)
                {
                    _consumer.Close();
                }

            }
        }

        public async Task<KwfEventConsumerHandler<THandler, TPayload>> StartConsuming(string topic, string? topipConfigurationKey = null)
        {
            _consumer = await _kwfKafkaBus.RegisterConsumer(_kwfEventHandler, topic, topipConfigurationKey);
            return this;
        }
    }
}
