namespace KWFEventBus.KWFRabbitMQ.Constants
{
    using Microsoft.Extensions.Logging;

    public static class KwfConstants
    {
        public static EventId RabbitMQ_log_eventId = new(147002, "RabbitMQ Event Bus");
    }
}
