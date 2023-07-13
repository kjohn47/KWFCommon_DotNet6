namespace KWFEventBus.Abstractions.Models
{
    using KWFJson.Configuration;

    using System.Text.Json;

    public static class EventsJsonOptions
    {
        public static JsonSerializerOptions GetJsonOptions()
        {
            return new KWFJsonConfiguration().GetJsonSerializerOptions();
        }
    }
}
