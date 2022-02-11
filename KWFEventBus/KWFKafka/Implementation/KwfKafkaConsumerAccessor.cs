namespace KWFEventBus.KWFKafka.Implementation
{
    using KWFEventBus.Abstractions.Interfaces;

    using System.Collections.Generic;
    using System.Linq;

    internal class KwfKafkaConsumerAccessor : IKwfConsumerAccessor
    {
        private readonly IEnumerable<IKwfEventConsumerHandler> _consumerHandlers;

        public KwfKafkaConsumerAccessor(IEnumerable<IKwfEventConsumerHandler> consumerHandlers)
        {
            _consumerHandlers = consumerHandlers;
        }

        public IEnumerable<IKwfEventConsumerHandler> GetAllConsumers()
        {
            return _consumerHandlers;
        }

        public IKwfEventConsumerHandler? GetConsumerService<THandler, TPayload>()
            where THandler : class, IKwfEventHandler<TPayload>
            where TPayload : class
        {
            return _consumerHandlers.FirstOrDefault(x => x is KwfKafkaConsumerHandler<THandler, TPayload>);
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

        public void StartConsuming<THandler, TPayload>()
            where THandler : class, IKwfEventHandler<TPayload>
            where TPayload : class
        {
            GetConsumerService<THandler, TPayload>()?.StartConsuming();
        }

        public void StopConsuming<THandler, TPayload>()
            where THandler : class, IKwfEventHandler<TPayload>
            where TPayload : class
        {
            GetConsumerService<THandler, TPayload>()?.StopConsuming();
        }
    }
}
