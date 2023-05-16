namespace KWFEventBus.KWFRabbitMQ.Implementation
{
    using System.Collections.Generic;
    using System.Linq;

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.KWFRabbitMQ.Interfaces;

    internal class KwfRabbitMQConsumerAccessor : IKwfRabbitMQConsumerAccessor
    {
        private readonly IEnumerable<IKwfRabbitMQConsumerHandler> _consumerHandlers;

        public KwfRabbitMQConsumerAccessor(IEnumerable<IKwfRabbitMQConsumerHandler> consumerHandlers)
        {
            _consumerHandlers = consumerHandlers;
        }

        public IEnumerable<IKwfEventConsumerHandler> GetAllConsumers()
        {
            return _consumerHandlers;
        }

        public IKwfEventConsumerHandler? GetConsumerService<TPayload>() where TPayload : class
        {
            return _consumerHandlers.FirstOrDefault(x => x is KwfRabbitMQConsumerHandler<IKwfRabbitMQEventHandler<TPayload>, TPayload>);
        }

        public void StartConsuming<TPayload>() where TPayload : class
        {
            GetConsumerService<TPayload>()?.StartConsuming();
        }

        public void StartConsumingAll()
        {
            foreach (var consumerHandler in _consumerHandlers)
            {
                consumerHandler.StartConsuming();
            }
        }

        public void StopConsuming<TPayload>() where TPayload : class
        {
            GetConsumerService<TPayload>()?.StopConsuming();
        }

        public void StopConsumingAll()
        {
            foreach (var consumerHandler in _consumerHandlers)
            {
                consumerHandler.StopConsuming();
            }
        }
    }
}
