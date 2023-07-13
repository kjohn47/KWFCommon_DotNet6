namespace KWFJson.Converters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using KWFExtensions;

    public class JsonNullableTimeOnlyConverter : JsonConverter<TimeOnly?>
    {
        public override TimeOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var time = reader.GetString();
            if (string.IsNullOrEmpty(time) || !TimeOnly.TryParse(time, out var parsedTime))
            {
                return null;
            }

            return parsedTime;
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.GetTimeOnlyString());
            }
        }
    }
}