namespace KWFEventBus.KWFRabbitMQ.Implementation
{
    using System.Collections.Generic;
    using System.Linq;

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.KWFRabbitMQ.Interfaces;

    using Microsoft.Extensions.DependencyInjection;

    internal class KwfRabbitMQConsumerAccessor : IKwfRabbitMQConsumerAccessor
    {
        private readonly IServiceProvider _serviceProvider;

        public KwfRabbitMQConsumerAccessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<IKwfEventConsumerHandler> GetAllConsumers()
        {
            return _serviceProvider.GetRequiredService<IEnumerable<IKwfRabbitMQConsumerHandler>>();
        }

        public IKwfEventConsumerHandler? GetConsumerService<TPayload>() where TPayload : class
        {
            return GetAllConsumers().FirstOrDefault(x => x is KwfRabbitMQConsumerHandlerBase<IKwfRabbitMQEventHandler<TPayload>, TPayload>);
        }

        public void StartConsuming<TPayload>() where TPayload : class
        {
            GetConsumerService<TPayload>()?.StartConsuming();
        }

        public void StartConsumingAll()
        {
            var consumers = GetAllConsumers();
            foreach (var consumerHandler in consumers)
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
            var consumers = GetAllConsumers();
            foreach (var consumerHandler in consumers)
            {
                consumerHandler.StopConsuming();
            }
        }
    }
}
