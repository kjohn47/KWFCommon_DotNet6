namespace KWFJson.Converters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using KWFExtensions;

    public class JsonUtcDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly DateTime _default = DateTime.MinValue.ToUtcKind();

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var date = reader.GetString();
            if (string.IsNullOrEmpty(date) || !DateTime.TryParse(date, out var parsedDate))
            {
                return _default;
            }

            return parsedDate.Kind == DateTimeKind.Unspecified 
                   ? parsedDate.ToUtcKind()
                   : parsedDate;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value.Kind == DateTimeKind.Unspecified)
            {
                writer.WriteStringValue(value.ToUtcKind());
            }
            else
            {
                writer.WriteStringValue(value);
            }
        }
    }
}
