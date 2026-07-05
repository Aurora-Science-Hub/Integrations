using AuroraScienceHub.Integrations.NoaaClient.Rtsw.Extensions;
using AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;
using AuroraScienceHub.Integrations.UnitTests.Utils;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.NoaaClient.Rtsw;

/// <summary>
/// Unit tests for <see cref="SolarWindPlasmaDataParser"/>.
/// </summary>
public sealed class SolarWindPlasmaDataParserTests
{
    [Theory(DisplayName = "RTSW solar wind parser returns expected records for valid payload")]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.NoaaClient/Rtsw/Samples/RtswSolarWindPlasmaSample.json")]
    public void Parse_WhenTextIsValid_ReturnsRecords(string text)
    {
        // Arrange, Act
        var result = SolarWindPlasmaDataParser.Parse(text);

        // Assert
        result.ShouldNotBeEmpty();
        result.ShouldBe([
            new SolarWindPlasmaRecord(
                DateTime: new DateTime(2026, 7, 5, 19, 45, 0, DateTimeKind.Utc),
                Active: true,
                Source: "SOLAR1",
                ProtonSpeed: 452.5f,
                ProtonTemperature: 55281f,
                ProtonDensity: 1.79f,
                ProtonVxGse: -448.1f,
                ProtonVyGse: -0.1f,
                ProtonVzGse: -63.1f,
                ProtonVxGsm: -448.1f,
                ProtonVyGsm: 1.1f,
                ProtonVzGsm: -63.1f,
                ProtonSampleSize: 1,
                AlphaSpeed: null,
                AlphaTemperature: null,
                AlphaDensity: null,
                AlphaVxGse: null,
                AlphaVyGse: null,
                AlphaVzGse: null,
                AlphaVxGsm: null,
                AlphaVyGsm: null,
                AlphaVzGsm: null,
                AlphaSampleSize: null,
                MaxConvergenceFlag: 0,
                MaxDataFlag: 0,
                MaxErrorCountFlag: 0,
                MaxProcessingFlag: 0,
                MaxRangeFlag: 0,
                MaxSampleCountFlag: 0,
                MaxTelemetryFlag: 0,
                OverallQuality: 0),
            new SolarWindPlasmaRecord(
                DateTime: new DateTime(2026, 7, 5, 19, 44, 0, DateTimeKind.Utc),
                Active: false,
                Source: "ACE",
                ProtonSpeed: 397.4f,
                ProtonTemperature: 44618f,
                ProtonDensity: null,
                ProtonVxGse: -395.2f,
                ProtonVyGse: 1.5f,
                ProtonVzGse: -41.3f,
                ProtonVxGsm: -395.2f,
                ProtonVyGsm: 2.0f,
                ProtonVzGsm: -41.3f,
                ProtonSampleSize: 1,
                AlphaSpeed: null,
                AlphaTemperature: null,
                AlphaDensity: null,
                AlphaVxGse: null,
                AlphaVyGse: null,
                AlphaVzGse: null,
                AlphaVxGsm: null,
                AlphaVyGsm: null,
                AlphaVzGsm: null,
                AlphaSampleSize: null,
                MaxConvergenceFlag: 0,
                MaxDataFlag: 0,
                MaxErrorCountFlag: 0,
                MaxProcessingFlag: 0,
                MaxRangeFlag: 0,
                MaxSampleCountFlag: 0,
                MaxTelemetryFlag: 0,
                OverallQuality: 0),
            new SolarWindPlasmaRecord(
                DateTime: new DateTime(2026, 7, 5, 19, 43, 0, DateTimeKind.Utc),
                Active: false,
                Source: "IMAP",
                ProtonSpeed: 410.2f,
                ProtonTemperature: 50120f,
                ProtonDensity: 2.05f,
                ProtonVxGse: -408.0f,
                ProtonVyGse: -0.5f,
                ProtonVzGse: -42.7f,
                ProtonVxGsm: -408.0f,
                ProtonVyGsm: 0.8f,
                ProtonVzGsm: -42.7f,
                ProtonSampleSize: 1,
                AlphaSpeed: null,
                AlphaTemperature: null,
                AlphaDensity: null,
                AlphaVxGse: null,
                AlphaVyGse: null,
                AlphaVzGse: null,
                AlphaVxGsm: null,
                AlphaVyGsm: null,
                AlphaVzGsm: null,
                AlphaSampleSize: null,
                MaxConvergenceFlag: 0,
                MaxDataFlag: 0,
                MaxErrorCountFlag: 0,
                MaxProcessingFlag: 0,
                MaxRangeFlag: 0,
                MaxSampleCountFlag: 0,
                MaxTelemetryFlag: 0,
                OverallQuality: 0)
        ]);
    }

    [Theory(DisplayName = "RTSW solar wind parser returns empty list for empty payload")]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.NoaaClient/Rtsw/Samples/EmptyArray.json")]
    public void Parse_WhenTextContainsEmptyArray_ReturnsEmpty(string text)
    {
        // Arrange, Act
        var result = SolarWindPlasmaDataParser.Parse(text);

        // Assert
        result.ShouldBeEmpty();
    }

    [Theory(DisplayName = "RTSW solar wind parser throws for malformed payload")]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.NoaaClient/Rtsw/Samples/RtswSolarWindPlasmaMalformedSample.json")]
    public void Parse_WhenPayloadIsMalformed_ThrowsInvalidOperationException(string text)
    {
        // Arrange, Act
        var exception = Should.Throw<InvalidOperationException>(() => SolarWindPlasmaDataParser.Parse(text));

        // Assert
        exception.Message.ShouldContain("time_tag");
    }
}
