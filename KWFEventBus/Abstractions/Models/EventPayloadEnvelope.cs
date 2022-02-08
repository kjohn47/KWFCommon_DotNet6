namespace KWFEventBus.Abstractions.Models
{
    using KWFEventBus.Abstractions.Interfaces;

    using System;

    public class EventPayloadEnvelope<T> : IEventPayloadEnvelope<T>
        where T : class
    {
        public EventPayloadEnvelope(T payload)
        {
            Payload = payload;
        }

        public T Payload { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
