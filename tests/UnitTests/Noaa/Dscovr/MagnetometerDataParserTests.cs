using AuroraScienceHub.Integrations.Noaa.Dscovr.Extensions;
using AuroraScienceHub.Integrations.Noaa.Dscovr.Responses;
using AuroraScienceHub.Integrations.UnitTests.Utils;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.Noaa.Dscovr;

/// <summary>
/// Unit tests for <see cref="MagnetometerDataParser"/>.
/// </summary>
public sealed class MagnetometerDataParserTests
{
    [Theory]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.Noaa/Dscovr/Samples/DscovrMagnetometerSample.json")]
    public void Parse_WhenTextIsValid_ReturnsRecords(string text)
    {
        // Arrange, Act
        var result = MagnetometerDataParser.Parse(text);

        // Assert
        result.ShouldNotBeEmpty();
        result.ShouldBe([
            new MagnetometerRecord
            (
                DateTime: new DateTime(2024, 5, 19, 17, 7, 0, DateTimeKind.Utc),
                Bx: -2.15f,
                By: 8.51f,
                Bz: -2.98f,
                Bt: 9.27f,
                Latitude: -18.76f,
                Longitude: 104.17f
            ),
            new MagnetometerRecord
            (
                DateTime: new DateTime(2024, 5, 19, 17, 8, 0, DateTimeKind.Utc),
                Bx: -2.38f,
                By: 8.31f,
                Bz: -3.24f,
                Bt: 9.23f,
                Latitude: -20.54f,
                Longitude: 105.97f
            )
        ]);
    }
}
