using AuroraScienceHub.Integrations.NoaaClient.Ace.Extensions;
using AuroraScienceHub.Integrations.NoaaClient.Ace.Responses;
using AuroraScienceHub.Integrations.UnitTests.Utils;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.NoaaClient.Ace;

/// <summary>
/// Unit tests for <see cref="SolarWindPlasmaDataParser"/>.
/// </summary>
public sealed class SolarWindPlasmaDataParserTests
{
    [Theory(DisplayName = "ACE SWEPAM parser returns expected records for valid payload")]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.NoaaClient/Ace/Samples/AceSwepamSample.txt")]
    public void Parse_WhenTextIsValid_ReturnsRecords(string text)
    {
        // Arrange, Act
        var result = SolarWindPlasmaDataParser.Parse(text);

        // Assert
        result.ShouldNotBeEmpty();
        result.ShouldBe([
            new SolarWindPlasmaRecord
            (
                DateTime: new DateTime(2024, 5, 6, 16, 57, 0, DateTimeKind.Utc),
                Status: 1,
                ProtonDensity: 4.9f,
                BulkSpeed: 475.0f,
                IonTemperature: 2.38e+05f
            ),
            new SolarWindPlasmaRecord
            (
                DateTime: new DateTime(2024, 5, 6, 16, 58, 0, DateTimeKind.Utc),
                Status: 0,
                ProtonDensity: 4.6f,
                BulkSpeed: 474.2f,
                IonTemperature: 2.01e+05f
            ),
            new SolarWindPlasmaRecord
            (
                DateTime: new DateTime(2024, 5, 6, 16, 59, 0, DateTimeKind.Utc),
                Status: 3,
                ProtonDensity: null,
                BulkSpeed: null,
                IonTemperature: null
            )
        ]);
    }
}
