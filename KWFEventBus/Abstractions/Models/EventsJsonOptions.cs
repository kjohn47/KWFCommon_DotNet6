namespace KWFEventBus.Abstractions.Models
{
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public static class EventsJsonOptions
    {
        public static JsonSerializerOptions GetJsonOptions()
        {
            var settings = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            settings.Converters.Add(new JsonStringEnumConverter());

            return settings;
        }
    }
}
