namespace KWFJson.Converters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using KWFExtensions;

    public class JsonTimeOnlyConverter : JsonConverter<TimeOnly>
    {
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var time = reader.GetString();
            if (string.IsNullOrEmpty(time) || !TimeOnly.TryParse(time, out var parsedTime))
            {
                return TimeOnly.MinValue;
            }

            return parsedTime;
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.GetTimeOnlyString());
        }
    }
}
