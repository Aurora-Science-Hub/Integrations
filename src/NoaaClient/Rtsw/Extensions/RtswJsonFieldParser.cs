using System.Text.Json;
using AuroraScienceHub.Framework.Utilities.System;
using AuroraScienceHub.Integrations.NoaaClient.Utilities;

namespace AuroraScienceHub.Integrations.NoaaClient.Rtsw.Extensions;

internal static class RtswJsonFieldParser
{
    /// <summary>
    /// Parses a required date-time field from an RTSW JSON item.
    /// </summary>
    public static DateTime ParseDateTime(JsonElement item, string propertyName, string context)
    {
        if (!item.TryGetProperty(propertyName, out var property))
        {
            throw new InvalidOperationException($"{context} field '{propertyName}' not found.");
        }

        if (property.ValueKind != JsonValueKind.String)
        {
            throw new InvalidOperationException($"{context} field '{propertyName}' must be a string.");
        }

        if (property.TryGetDateTime(out var dateTime))
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Utc => dateTime,
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                _ => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
            };
        }

        var value = property.GetString();
        throw new InvalidOperationException($"{context} field '{propertyName}' has invalid value '{value}'.");
    }

    /// <summary>
    /// Parses a required boolean field from an RTSW JSON item.
    /// </summary>
    public static bool ParseBool(JsonElement item, string propertyName, string context)
    {
        if (!item.TryGetProperty(propertyName, out var property))
        {
            throw new InvalidOperationException($"{context} field '{propertyName}' not found.");
        }

        return property.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String when bool.TryParse(property.GetString(), out var parsed) => parsed,
            _ => throw new InvalidOperationException($"{context} field '{propertyName}' has invalid value.")
        };
    }

    /// <summary>
    /// Parses a required string field from an RTSW JSON item.
    /// </summary>
    public static string ParseRequiredString(JsonElement item, string propertyName, string context)
    {
        if (!item.TryGetProperty(propertyName, out var property))
        {
            throw new InvalidOperationException($"{context} field '{propertyName}' not found.");
        }

        if (property.ValueKind != JsonValueKind.String)
        {
            throw new InvalidOperationException($"{context} field '{propertyName}' must be a string.");
        }

        return property.GetString().Required();
    }

    /// <summary>
    /// Parses an optional nullable float field; missing property or JSON null returns null.
    /// </summary>
    public static float? ParseOptionalNullableFloat(JsonElement item, string propertyName, string context)
    {
        if (!item.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        return property.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.Number => ParseFloatString(property.GetRawText(), propertyName, context),
            JsonValueKind.String => ParseFloatString(property.GetString(), propertyName, context),
            _ => throw new InvalidOperationException($"{context} field '{propertyName}' has invalid value.")
        };
    }

    /// <summary>
    /// Parses an optional nullable integer field; missing property or JSON null returns null.
    /// </summary>
    public static int? ParseOptionalNullableInt(JsonElement item, string propertyName, string context)
    {
        if (!item.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        return property.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.Number when property.TryGetInt32(out var value) => value,
            JsonValueKind.String when int.TryParse(property.GetString(), out var parsed) => parsed,
            _ => throw new InvalidOperationException($"{context} field '{propertyName}' has invalid value.")
        };
    }

    /// <summary>
    /// Parses an optional nullable boolean field; missing property or JSON null returns null.
    /// </summary>
    public static bool? ParseOptionalNullableBool(JsonElement item, string propertyName, string context)
    {
        if (!item.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        return property.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String when bool.TryParse(property.GetString(), out var parsed) => parsed,
            _ => throw new InvalidOperationException($"{context} field '{propertyName}' has invalid value.")
        };
    }

    private static float ParseFloatString(string? value, string propertyName, string context)
    {
        try
        {
            var parsed = NoaaNumericParser.ParseNullableFloat(value);
            return parsed ?? throw new InvalidOperationException($"{context} field '{propertyName}' has invalid null value.");
        }
        catch (Exception exception) when (exception is ArgumentNullException or FormatException)
        {
            throw new InvalidOperationException($"{context} field '{propertyName}' has invalid value '{value}'.", exception);
        }
    }
}
