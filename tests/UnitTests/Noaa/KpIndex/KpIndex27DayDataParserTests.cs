using AuroraScienceHub.Integrations.Noaa.KpIndex.Extensions;
using AuroraScienceHub.Integrations.Noaa.KpIndex.Responses;
using AuroraScienceHub.Integrations.UnitTests.Utils;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.Noaa.KpIndex;

/// <summary>
/// Unit tests for <see cref="KpIndex27DayDataParser"/>.
/// </summary>
public sealed class KpIndex27DayDataParserTests
{
    [Theory]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.Noaa/KpIndex/Samples/KpIndex27DayForecastSample.txt")]
    public void Parse_WhenTextIsValid_ReturnsRecords(string text)
    {
        // Arrange, Act
        var result = KpIndex27DayDataParser.Parse(text);

        // Assert
        result.ShouldBe(
        [
            new KpIndex27DayResponse
            (
                Date: new DateOnly(2025, 1, 20),
                KpIndex: 4
            ),
            new KpIndex27DayResponse
            (
                Date: new DateOnly(2025, 1, 21),
                KpIndex: 3
            ),
            new KpIndex27DayResponse
            (
                Date: new DateOnly(2025, 1, 22),
                KpIndex: 2
            ),
            new KpIndex27DayResponse
            (
                Date: new DateOnly(2025, 1, 23),
                KpIndex: 2
            ),
            new KpIndex27DayResponse
            (
                Date: new DateOnly(2025, 1, 24),
                KpIndex: 2
            ),
        ]);
    }
}
