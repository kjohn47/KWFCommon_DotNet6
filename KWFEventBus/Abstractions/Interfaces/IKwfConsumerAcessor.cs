namespace KWFEventBus.Abstractions.Interfaces
{ 

    public interface IKwfConsumerAcessor
    {
        IKwfEventConsumerHandler? GetConsumerService<THandler, TPayload>()
            where THandler : class, IKwfEventHandler<TPayload>
            where TPayload : class;

        IEnumerable<IKwfEventConsumerHandler> GetAllConsumers();

        void StartConsumingAll();

        void StopConsumingAll();

        void StartConsuming<THandler, TPayload>()
            where THandler : class, IKwfEventHandler<TPayload>
            where TPayload : class;

        void StopConsuming<THandler, TPayload>()
            where THandler : class, IKwfEventHandler<TPayload>
            where TPayload : class;
    }
}
