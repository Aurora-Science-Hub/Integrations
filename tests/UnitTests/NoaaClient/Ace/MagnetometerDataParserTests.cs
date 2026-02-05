using AuroraScienceHub.Integrations.NoaaClient.Ace.Extensions;
using AuroraScienceHub.Integrations.NoaaClient.Ace.Responses;
using AuroraScienceHub.Integrations.UnitTests.Utils;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.NoaaClient.Ace;

/// <summary>
/// Unit tests for <see cref="MagnetometerDataParser"/>.
/// </summary>
public sealed class MagnetometerDataParserTests
{
    [Theory]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.NoaaClient/Ace/Samples/AceMagnetometerSample.txt")]
    public void Parse_WhenTextIsValid_ReturnsRecords(string text)
    {
        // Arrange, Act
        var result = MagnetometerDataParser.Parse(text);

        // Assert
        result.ShouldNotBeEmpty();
        result.ShouldBe([
            new MagnetometerRecord
            (
                DateTime: new DateTime(2024, 5, 5, 10, 42, 0, DateTimeKind.Utc),
                Status: 0,
                Bx: 2.3f,
                By: 1.8f,
                Bz: 2.2f,
                Bt: 3.7f,
                Latitude: 36.3f,
                Longitude: 38.0f
            ),
            new MagnetometerRecord
            (
                DateTime: new DateTime(2024, 5, 5, 10, 43, 0, DateTimeKind.Utc),
                Status: 0,
                Bx: 2.3f,
                By: 1.6f,
                Bz: 2.3f,
                Bt: 3.6f,
                Latitude: 38.7f,
                Longitude: 34.4f
            ),
            new MagnetometerRecord
            (
                DateTime: new DateTime(2024, 5, 5, 10, 44, 0, DateTimeKind.Utc),
                Status: 9,
                Bx: null,
                By: null,
                Bz: null,
                Bt: null,
                Latitude: null,
                Longitude: null
            )
        ]);
    }
}
