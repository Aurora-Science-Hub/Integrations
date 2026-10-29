using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuroraScienceHub.Integrations.NoaaClient.Json;

/// <summary>
/// Reads a nullable <see cref="int"/> from JSON numbers that may carry a fractional part
/// (for example <c>60.0</c>) or from numeric strings. NOAA occasionally encodes the RTSW
/// <c>sample_size</c> fields this way, which the built-in Int32 converter rejects.
/// Non-integral or out-of-range values are read as <see langword="null"/> so that a single
/// metadata field cannot fail the whole feed import.
/// </summary>
internal sealed class FlexibleNullableInt32Converter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;

            case JsonTokenType.Number:
                if (reader.TryGetInt32(out var intValue))
                {
                    return intValue;
                }

                return reader.TryGetDouble(out var doubleValue) && IsInInt32Range(doubleValue)
                    ? (int)Math.Round(doubleValue, MidpointRounding.AwayFromZero)
                    : null;

            case JsonTokenType.String:
                var raw = reader.GetString();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return null;
                }

                if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedInt))
                {
                    return parsedInt;
                }

                return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedDouble)
                       && IsInInt32Range(parsedDouble)
                    ? (int)Math.Round(parsedDouble, MidpointRounding.AwayFromZero)
                    : null;

            default:
                throw new JsonException(
                    $"Unexpected token '{reader.TokenType}' when reading a nullable Int32 value.");
        }
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }

    private static bool IsInInt32Range(double value)
        => !double.IsNaN(value)
           && !double.IsInfinity(value)
           && value >= int.MinValue
           && value <= int.MaxValue;
}
