namespace KWFEventBus.Abstractions.Models
{
    public class EventBusEndpoint
    {
        public string Url { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}
