namespace KWFEventBus.KWFKafka.Implementation
{
    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.KWFKafka.Interfaces;

    using Microsoft.Extensions.DependencyInjection;

    using System.Collections.Generic;
    using System.Linq;

    internal class KwfKafkaConsumerAccessor : IKwfKafkaConsumerAccessor
    {
        private readonly IServiceProvider _serviceProvider;

        public KwfKafkaConsumerAccessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<IKwfEventConsumerHandler> GetAllConsumers()
        {
            return _serviceProvider.GetRequiredService<IEnumerable<IKwfKafkaEventConsumerHandler>>();
        }

        public IKwfEventConsumerHandler? GetConsumerService<TPayload>()
            where TPayload : class
        {
            return GetAllConsumers().FirstOrDefault(x => x is KwfKafkaConsumerHandler<IKwfKafkaEventHandler<TPayload>, TPayload>);
        }

        public void StartConsumingAll()
        {
            var consumers = GetAllConsumers();
            foreach (var consumerHandler in consumers)
            {
                consumerHandler.StartConsuming();
            }
        }

        public void StopConsumingAll()
        {
            var consumers = GetAllConsumers();
            foreach (var consumerHandler in consumers)
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
