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
                DateTime: new DateTime(2024, 5, 19, 5, 51, 0, DateTimeKind.Utc),
                ProtonDensity: 2.18f,
                BulkSpeed: 393.9f,
                IonTemperature: 36967f,
                Active: true,
                Source: "ACE"),
            new SolarWindPlasmaRecord(
                DateTime: new DateTime(2024, 5, 19, 5, 52, 0, DateTimeKind.Utc),
                ProtonDensity: null,
                BulkSpeed: 397.4f,
                IonTemperature: 44618f,
                Active: false,
                Source: "ACE")
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
