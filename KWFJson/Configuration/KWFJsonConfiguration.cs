namespace KWFJson.Configuration
{
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using KWFJson.Converters;

    public sealed class KWFJsonConfiguration
    {
        private JsonSerializerOptions options;

        public KWFJsonConfiguration(bool writeIndented = false)
            : this(new JsonSerializerOptions(), writeIndented)
        {
        }

        public KWFJsonConfiguration(JsonSerializerOptions opt, bool writeIndented)
        {
            opt.Converters.Add(new JsonStringEnumConverter());
            opt.Converters.Add(new JsonUtcDateTimeConverter());
            opt.Converters.Add(new JsonUtcNullableDateTimeConverter());
            opt.Converters.Add(new JsonDateOnlyConverter());
            opt.Converters.Add(new JsonNullableDateOnlyConverter());
            opt.Converters.Add(new JsonTimeOnlyConverter());
            opt.Converters.Add(new JsonNullableTimeOnlyConverter());
            opt.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            opt.PropertyNameCaseInsensitive = true;
            opt.AllowTrailingCommas = true;
            opt.WriteIndented = writeIndented;
            opt.IgnoreReadOnlyProperties = false;

            options = opt;
        }

        public JsonSerializerOptions GetJsonSerializerOptions()
        {
            return options;
        }
    }
}
