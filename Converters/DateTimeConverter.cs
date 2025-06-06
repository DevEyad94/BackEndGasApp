using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackEndGasApp.Converters
{
    // DateTime converter to handle default dates
    public class DateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            var value = reader.GetString();
            if (string.IsNullOrEmpty(value) || value == "0001-01-01T00:00:00")
                return null;

            return DateTime.Parse(value);
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateTime? value,
            JsonSerializerOptions options
        )
        {
            if (value == null || value == DateTime.MinValue)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
            }
        }
    }
}
