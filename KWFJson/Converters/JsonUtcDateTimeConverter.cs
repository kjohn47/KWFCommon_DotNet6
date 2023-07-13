namespace KWFJson.Converters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using KWFExtensions;

    public class JsonUtcDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly DateTime _default = DateTime.MinValue.ToUtc();

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var date = reader.GetString();
            if (string.IsNullOrEmpty(date) || !DateTime.TryParse(date, out var parsedDate))
            {
                return _default;
            }

            return parsedDate.ToUtc();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUtc());
        }
    }
}
