namespace KWFEventBus.Abstractions.Interfaces
{ 

    public interface IKwfConsumerAccessor
    {
        IKwfEventConsumerHandler? GetConsumerService<TPayload>()
            where TPayload : class;

        IEnumerable<IKwfEventConsumerHandler> GetAllConsumers();

        void StartConsumingAll();

        void StopConsumingAll();

        void StartConsuming<TPayload>()
            where TPayload : class;

        void StopConsuming<TPayload>()
            where TPayload : class;
    }
}
