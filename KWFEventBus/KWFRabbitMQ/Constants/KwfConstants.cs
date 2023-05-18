namespace KWFEventBus.KWFRabbitMQ.Constants
{
    using Microsoft.Extensions.Logging;

    public static class KwfConstants
    {
        public const string DefaultExchangeNameLog = "default exchange";
        public const string HostNameHeader = "host-name";
        public const string ApplicationNameHeader = "application-name";

        public static EventId RabbitMQ_log_eventId = new(147002, "RabbitMQ Event Bus");
    }
}
