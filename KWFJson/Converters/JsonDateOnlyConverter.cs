namespace KWFJson.Converters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using KWFExtensions;

    public class JsonDateOnlyConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var date = reader.GetString();
            if (string.IsNullOrEmpty(date) || !DateOnly.TryParse(date, out var parsedDate))
            {
                return DateOnly.MinValue;
            }

            return parsedDate;
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.GetDateOnlyString());
        }
    }
}
