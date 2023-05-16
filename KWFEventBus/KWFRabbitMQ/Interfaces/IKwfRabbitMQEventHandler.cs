namespace KWFEventBus.KWFRabbitMQ.Interfaces
{
    using KWFEventBus.Abstractions.Interfaces;

    public interface IKwfRabbitMQEventHandler<TPayload> : IKwfEventHandler<TPayload> where TPayload : class
    {
    }
}
