namespace KWFEventBus.KWFKafka.Implementation
{
    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.KWFKafka.Interfaces;

    using System.Collections.Generic;
    using System.Linq;

    internal class KwfKafkaConsumerAccessor : IKwfKafkaConsumerAccessor
    {
        private readonly IEnumerable<IKwfKafkaEventConsumerHandler> _consumerHandlers;

        public KwfKafkaConsumerAccessor(IEnumerable<IKwfKafkaEventConsumerHandler> consumerHandlers)
        {
            _consumerHandlers = consumerHandlers;
        }

        public IEnumerable<IKwfEventConsumerHandler> GetAllConsumers()
        {
            return _consumerHandlers;
        }

        public IKwfEventConsumerHandler? GetConsumerService<TPayload>()
            where TPayload : class
        {
            return _consumerHandlers.FirstOrDefault(x => x is KwfKafkaConsumerHandler<IKwfKafkaEventHandler<TPayload>, TPayload>);
        }

        public void StartConsumingAll()
        { 
            foreach (var consumerHandler in _consumerHandlers)
            {
                consumerHandler.StartConsuming();
            }
        }

        public void StopConsumingAll()
        {
            foreach (var consumerHandler in _consumerHandlers)
            {
                consumerHandler.StopConsuming();
            }
        }

        public void StartConsuming<TPayload>()
            where TPayload : class
        {
            GetConsumerService<TPayload>()?.StartConsuming();
        }

        public void StopConsuming<TPayload>()
            where TPayload : class
        {
            GetConsumerService<TPayload>()?.StopConsuming();
        }
    }
}
