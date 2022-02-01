namespace KWFCommon.Implementation.Json
{
    using System.Text.Json;
    using System.Text.Json.Serialization;

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
            opt.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            opt.PropertyNameCaseInsensitive = true;
            opt.AllowTrailingCommas = true;
            opt.WriteIndented = writeIndented;

            options = opt;
        }

        public JsonSerializerOptions GetJsonSerializerOptions()
        {
            return options;
        }
    }
}
