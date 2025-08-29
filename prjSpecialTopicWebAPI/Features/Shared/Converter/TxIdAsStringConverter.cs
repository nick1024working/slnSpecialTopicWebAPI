using System.Text.Json;
using System.Text.Json.Serialization;

namespace prjSpecialTopicWebAPI.Features.Shared.Converter
{
    public sealed class TxIdAsStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString()!,
                JsonTokenType.Number => reader.TryGetInt64(out var n) ? n.ToString() : reader.GetDecimal().ToString(),
                _ => throw new JsonException("transactionId must be string or number")
            };
        }
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            => writer.WriteStringValue(value);
    }
}
