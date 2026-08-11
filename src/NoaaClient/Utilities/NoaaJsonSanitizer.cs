namespace AuroraScienceHub.Integrations.NoaaClient.Utilities;

/// <summary>
/// Replaces non-standard NOAA JSON numeric literals with RFC-compliant null tokens.
/// </summary>
internal static class NoaaJsonSanitizer
{
    private static ReadOnlySpan<byte> NullLiteral => "null"u8;
    private static ReadOnlySpan<byte> NanLiteral => "NaN"u8;
    private static ReadOnlySpan<byte> InfinityLiteral => "Infinity"u8;
    private static ReadOnlySpan<byte> NegativeInfinityLiteral => "-Infinity"u8;

    /// <summary>
    /// Sanitizes NOAA JSON payload by replacing bare and quoted NaN/Infinity literals with null.
    /// </summary>
    public static SanitizeResult Sanitize(ReadOnlyMemory<byte> source)
    {
        var span = source.Span;
        if (span.Length == 0)
        {
            return new SanitizeResult(source, 0);
        }

        if (!MayContainNonStandardNumericLiteral(span))
        {
            return new SanitizeResult(source, 0);
        }

        var output = new byte[span.Length + 64];
        var writeIndex = 0;
        var replacements = 0;
        var inString = false;
        var i = 0;

        while (i < span.Length)
        {
            EnsureCapacity(ref output, writeIndex + 16);

            var current = span[i];

            if (inString)
            {
                output[writeIndex++] = current;

                if (current == (byte)'\\' && i + 1 < span.Length)
                {
                    output[writeIndex++] = span[i + 1];
                    i += 2;
                    continue;
                }

                if (current == (byte)'"')
                {
                    inString = false;
                }

                i++;
                continue;
            }

            if (current == (byte)'"')
            {
                if (TryReplaceQuotedLiteral(span, ref i, NegativeInfinityLiteral, output, ref writeIndex, ref replacements)
                    || TryReplaceQuotedLiteral(span, ref i, InfinityLiteral, output, ref writeIndex, ref replacements)
                    || TryReplaceQuotedLiteral(span, ref i, NanLiteral, output, ref writeIndex, ref replacements))
                {
                    continue;
                }

                inString = true;
                output[writeIndex++] = current;
                i++;
                continue;
            }

            if (TryReplaceBareLiteral(span, ref i, NegativeInfinityLiteral, output, ref writeIndex, ref replacements)
                || TryReplaceBareLiteral(span, ref i, InfinityLiteral, output, ref writeIndex, ref replacements)
                || TryReplaceBareLiteral(span, ref i, NanLiteral, output, ref writeIndex, ref replacements))
            {
                continue;
            }

            output[writeIndex++] = current;
            i++;
        }

        if (replacements == 0)
        {
            return new SanitizeResult(source, 0);
        }

        return new SanitizeResult(output.AsMemory(0, writeIndex), replacements);
    }

    private static bool MayContainNonStandardNumericLiteral(ReadOnlySpan<byte> span)
    {
        for (var i = 0; i < span.Length; i++)
        {
            if (span[i] is (byte)'N' or (byte)'I')
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryReplaceQuotedLiteral(
        ReadOnlySpan<byte> source,
        ref int index,
        ReadOnlySpan<byte> literal,
        byte[] output,
        ref int writeIndex,
        ref int replacements)
    {
        var endIndex = index + 1 + literal.Length;

        if (endIndex >= source.Length || source[endIndex] != (byte)'"')
        {
            return false;
        }

        if (!source.Slice(index + 1, literal.Length).SequenceEqual(literal))
        {
            return false;
        }

        WriteNullLiteral(output, ref writeIndex);
        replacements++;
        index = endIndex + 1;
        return true;
    }

    private static bool TryReplaceBareLiteral(
        ReadOnlySpan<byte> source,
        ref int index,
        ReadOnlySpan<byte> literal,
        byte[] output,
        ref int writeIndex,
        ref int replacements)
    {
        if (index + literal.Length > source.Length)
        {
            return false;
        }

        if (!source.Slice(index, literal.Length).SequenceEqual(literal))
        {
            return false;
        }

        if (!IsBareLiteralStart(source, index) || !IsBareLiteralEnd(source, index + literal.Length))
        {
            return false;
        }

        WriteNullLiteral(output, ref writeIndex);
        replacements++;
        index += literal.Length;
        return true;
    }

    private static bool IsBareLiteralStart(ReadOnlySpan<byte> source, int index)
    {
        if (index == 0)
        {
            return true;
        }

        return IsDelimiter(source[index - 1]);
    }

    private static bool IsBareLiteralEnd(ReadOnlySpan<byte> source, int index)
    {
        if (index >= source.Length)
        {
            return true;
        }

        return IsDelimiter(source[index]);
    }

    private static bool IsDelimiter(byte value)
        => value is (byte)' ' or (byte)'\t' or (byte)'\r' or (byte)'\n'
            or (byte)':' or (byte)',' or (byte)'[' or (byte)']' or (byte)'{' or (byte)'}';

    private static void WriteNullLiteral(byte[] output, ref int writeIndex)
    {
        EnsureCapacity(ref output, writeIndex + NullLiteral.Length);
        NullLiteral.CopyTo(output.AsSpan(writeIndex));
        writeIndex += NullLiteral.Length;
    }

    private static void EnsureCapacity(ref byte[] output, int requiredLength)
    {
        if (requiredLength <= output.Length)
        {
            return;
        }

        Array.Resize(ref output, Math.Max(requiredLength, output.Length * 2));
    }

    internal readonly record struct SanitizeResult(ReadOnlyMemory<byte> Bytes, int ReplacementCount);
}
