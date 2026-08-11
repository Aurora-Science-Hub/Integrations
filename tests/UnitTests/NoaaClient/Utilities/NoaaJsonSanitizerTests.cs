using System.Text;
using AuroraScienceHub.Integrations.NoaaClient.Utilities;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.NoaaClient.Utilities;

/// <summary>
/// Unit tests for <see cref="NoaaJsonSanitizer"/>.
/// </summary>
public sealed class NoaaJsonSanitizerTests
{
    [Fact(DisplayName = "Sanitize replaces bare NaN with null")]
    public void Sanitize_WhenBareNaN_ReplacesWithNull()
    {
        // Arrange
        var source = Encoding.UTF8.GetBytes("""{"proton_speed": NaN}""");

        // Act
        var result = NoaaJsonSanitizer.Sanitize(source);

        // Assert
        result.ReplacementCount.ShouldBe(1);
        result.Bytes.Span.ToArray().ShouldBe(Encoding.UTF8.GetBytes("""{"proton_speed": null}"""));
    }

    [Fact(DisplayName = "Sanitize replaces bare -Infinity with null")]
    public void Sanitize_WhenBareNegativeInfinity_ReplacesWithNull()
    {
        // Arrange
        var source = Encoding.UTF8.GetBytes("""{"proton_speed": -Infinity}""");

        // Act
        var result = NoaaJsonSanitizer.Sanitize(source);

        // Assert
        result.ReplacementCount.ShouldBe(1);
        result.Bytes.Span.ToArray().ShouldBe(Encoding.UTF8.GetBytes("""{"proton_speed": null}"""));
    }

    [Fact(DisplayName = "Sanitize replaces quoted NaN with null")]
    public void Sanitize_WhenQuotedNaN_ReplacesWithNull()
    {
        // Arrange
        var source = Encoding.UTF8.GetBytes("""{"proton_speed": "NaN"}""");

        // Act
        var result = NoaaJsonSanitizer.Sanitize(source);

        // Assert
        result.ReplacementCount.ShouldBe(1);
        result.Bytes.Span.ToArray().ShouldBe(Encoding.UTF8.GetBytes("""{"proton_speed": null}"""));
    }

    [Fact(DisplayName = "Sanitize does not modify NaN inside string values")]
    public void Sanitize_WhenNaNInsideStringValue_DoesNotModify()
    {
        // Arrange
        var source = Encoding.UTF8.GetBytes("""{"source": "NaN sensor"}""");

        // Act
        var result = NoaaJsonSanitizer.Sanitize(source);

        // Assert
        result.ReplacementCount.ShouldBe(0);
        result.Bytes.ShouldBe(source);
    }

    [Fact(DisplayName = "Sanitize leaves valid JSON unchanged")]
    public void Sanitize_WhenValidJson_ReturnsOriginalMemory()
    {
        // Arrange
        var source = Encoding.UTF8.GetBytes("""
            [
              {
                "time_tag": "2026-08-10T09:45:00Z",
                "active": true,
                "source": "SOLAR1",
                "proton_speed": 420.0,
                "proton_density": 5.0
              }
            ]
            """);

        // Act
        var result = NoaaJsonSanitizer.Sanitize(source);

        // Assert
        result.ReplacementCount.ShouldBe(0);
        result.Bytes.ShouldBe(source);
    }
}
