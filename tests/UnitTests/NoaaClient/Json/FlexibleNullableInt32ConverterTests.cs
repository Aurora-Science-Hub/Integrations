using System.Text;
using System.Text.Json;
using AuroraScienceHub.Integrations.NoaaClient.Json;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.NoaaClient.Json;

/// <summary>
/// Unit tests for <see cref="FlexibleNullableInt32Converter"/>.
/// </summary>
public sealed class FlexibleNullableInt32ConverterTests
{
    [Theory(DisplayName = "Read converts a JSON number or null to a nullable Int32")]
    [InlineData("60", 60)]
    [InlineData("60.0", 60)]
    [InlineData("-5.0", -5)]
    [InlineData("6e1", 60)]
    [InlineData("2147483647.0", int.MaxValue)]
    [InlineData("60.5", null)]
    [InlineData("3e9", null)]
    [InlineData("-3e9", null)]
    [InlineData("null", null)]
    public void Read_WhenNumberOrNull_ReturnsExpectedValue(string rawJson, int? expected)
    {
        // Arrange
        var sut = new FlexibleNullableInt32Converter();

        // Act
        var result = Read(sut, rawJson);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory(DisplayName = "Read throws JsonException on a non-numeric token")]
    [InlineData("\"60\"")]
    [InlineData("\"\"")]
    [InlineData("true")]
    [InlineData("{}")]
    [InlineData("[]")]
    public void Read_WhenNonNumericToken_ThrowsJsonException(string rawJson)
    {
        // Arrange
        var sut = new FlexibleNullableInt32Converter();

        // Act & Assert
        Should.Throw<JsonException>(() => Read(sut, rawJson));
    }

    private static int? Read(FlexibleNullableInt32Converter converter, string rawJson)
    {
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(rawJson));
        reader.Read();
        return converter.Read(ref reader, typeof(int?), JsonSerializerOptions.Default);
    }
}
