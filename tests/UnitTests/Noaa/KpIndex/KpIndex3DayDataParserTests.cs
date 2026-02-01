using AuroraScienceHub.Integrations.Noaa.KpIndex.Extensions;
using AuroraScienceHub.Integrations.Noaa.KpIndex.Responses;
using AuroraScienceHub.Integrations.UnitTests.Utils;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.Noaa.KpIndex;

/// <summary>
/// Unit tests for <see cref="KpIndex3DayDataParser"/>.
/// </summary>
public sealed class KpIndex3DayDataParserTests
{
    [Theory]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.Noaa/KpIndex/Samples/KpIndex3DayForecastSample.json")]
    public void Parse_WhenTextIsValid_ReturnsRecords(string text)
    {
        // Arrange, Act
        var result = KpIndex3DayDataParser.Parse(text);

        // Assert
        result.ShouldBe(
        [
            new KpIndex3DayResponse
            (
                DateTime: new DateTime(2025, 1, 18, 0, 0, 0),
                KpIndex: 2.67F
            ),
            new KpIndex3DayResponse
            (
                DateTime: new DateTime(2025, 1, 18, 3, 0, 0),
                KpIndex: 2.00F
            ),
            new KpIndex3DayResponse
            (
                DateTime: new DateTime(2025, 1, 18, 6, 0, 0),
                KpIndex: 2.67F
            ),
            new KpIndex3DayResponse
            (
                DateTime: new DateTime(2025, 1, 18, 9, 0, 0),
                KpIndex: 2.00F
            ),
            new KpIndex3DayResponse
            (
                DateTime: new DateTime(2025, 1, 18, 12, 0, 0),
                KpIndex: 2.67F
            ),
        ]);
    }
}
