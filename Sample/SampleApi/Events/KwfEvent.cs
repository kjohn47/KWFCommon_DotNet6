namespace Sample.SampleApi.Events
{
    using KWFEventBus.Abstractions.Interfaces;

    using System;

    public class KwfEvent : IEventPayloadEnvelope<string>
    {
        public KwfEvent()
        {
        }

        public KwfEvent(string message)
        {
            Payload = message;
        }

        public string Payload { get; set; } = string.Empty;

        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
