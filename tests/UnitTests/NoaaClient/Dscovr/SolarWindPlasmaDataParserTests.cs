using AuroraScienceHub.Integrations.NoaaClient.Dscovr.Extensions;
using AuroraScienceHub.Integrations.NoaaClient.Dscovr.Responses;
using AuroraScienceHub.Integrations.UnitTests.Utils;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.NoaaClient.Dscovr;

/// <summary>
/// Unit tests for <see cref="SolarWindPlasmaDataParser"/>.
/// </summary>
public sealed class SolarWindPlasmaDataParserTests
{
    [Theory]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.NoaaClient/Dscovr/Samples/DscovrSolarWindPlasmaSample.json")]
    public void Parse_WhenTextIsValid_ReturnsRecords(string text)
    {
        // Arrange, Act
        var result = SolarWindPlasmaDataParser.Parse(text);

        // Assert
        result.ShouldNotBeEmpty();
        result.ShouldBe([
            new SolarWindPlasmaRecord
            (
                DateTime: new DateTime(2024, 5, 19, 5, 51, 0, DateTimeKind.Utc),
                ProtonDensity: 2.18f,
                BulkSpeed: 393.9f,
                IonTemperature: 36967f
            ),
            new SolarWindPlasmaRecord
            (
                DateTime: new DateTime(2024, 5, 19, 5, 52, 0, DateTimeKind.Utc),
                ProtonDensity: 2.60f,
                BulkSpeed: 397.4f,
                IonTemperature: 44618f
            )
        ]);
    }
}
