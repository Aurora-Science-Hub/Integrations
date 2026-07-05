using AuroraScienceHub.Framework.Utilities.System;

namespace AuroraScienceHub.Integrations.NoaaClient.Utilities;

internal static class NoaaNumericParser
{
    /// <summary>
    /// Parses nullable float from span value with optional null markers.
    /// </summary>
    public static float? ParseNullableFloat(ReadOnlySpan<char> value, params string[] nullMarkers)
    {
        foreach (var nullMarker in nullMarkers)
        {
            if (value.SequenceEqual(nullMarker))
            {
                return null;
            }
        }

        return value.ParseFloatInvariant();
    }

    /// <summary>
    /// Parses nullable float from string value with optional null markers.
    /// </summary>
    public static float? ParseNullableFloat(string? value, params string[] nullMarkers)
    {
        if (value is null)
        {
            return null;
        }

        foreach (var nullMarker in nullMarkers)
        {
            if (string.Equals(value, nullMarker, StringComparison.Ordinal))
            {
                return null;
            }
        }

        return value.ParseFloatInvariant();
    }
}
