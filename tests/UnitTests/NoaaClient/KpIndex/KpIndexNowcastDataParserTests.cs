using AuroraScienceHub.Integrations.NoaaClient.KpIndex.Extensions;
using AuroraScienceHub.Integrations.NoaaClient.KpIndex.Responses;
using AuroraScienceHub.Integrations.UnitTests.Utils;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.NoaaClient.KpIndex;

/// <summary>
/// Unit tests for <see cref="KpIndexNowcastDataParser"/>.
/// </summary>
public sealed class KpIndexNowcastDataParserTests
{
    [Theory]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.NoaaClient/KpIndex/Samples/KpIndexNowcastSample.json")]
    public void Parse_WhenTextIsValid_ReturnsRecords(string text)
    {
        // Arrange, Act
        var result = KpIndexNowcastDataParser.Parse(text);

        // Assert
        result.ShouldBe(
        [
            new KpIndexNowcastResponse
            (
                DateTime: new DateTime(2025, 1, 18, 0, 0, 0),
                KpIndex: 2.67F,
                StationsCount: 8
            ),
            new KpIndexNowcastResponse
            (
                DateTime: new DateTime(2025, 1, 18, 3, 0, 0),
                KpIndex: 2.00F,
                StationsCount: 10
            ),
            new KpIndexNowcastResponse
            (
                DateTime: new DateTime(2025, 1, 18, 6, 0, 0),
                KpIndex: 2.67F,
                StationsCount: 8
            ),
            new KpIndexNowcastResponse
            (
                DateTime: new DateTime(2025, 1, 18, 9, 0, 0),
                KpIndex: 2.00F,
                StationsCount: 8
            ),
            new KpIndexNowcastResponse
            (
                DateTime: new DateTime(2025, 1, 18, 12, 0, 0),
                KpIndex: 2.67F,
                StationsCount: 9
            ),
        ]);
    }
}
