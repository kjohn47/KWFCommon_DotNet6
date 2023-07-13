namespace KWFJson.Converters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using KWFExtensions;

    public class JsonUtcNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var date = reader.GetString();
            if (string.IsNullOrEmpty(date) || !DateTime.TryParse(date, out var parsedDate))
            {
                return null;
            }

            return parsedDate.Kind == DateTimeKind.Unspecified 
                   ? parsedDate.ToUtcKind()
                   : parsedDate;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            if (value.Value.Kind == DateTimeKind.Unspecified)
            {
                writer.WriteStringValue(value.Value.ToUtcKind());
            }
            else
            {
                writer.WriteStringValue(value.Value);
            }
        }
    }
}
