using AuroraScienceHub.Integrations.NoaaClient.Rtsw.Extensions;
using AuroraScienceHub.Integrations.NoaaClient.Rtsw.Responses;
using AuroraScienceHub.Integrations.UnitTests.Utils;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.NoaaClient.Rtsw;

/// <summary>
/// Unit tests for <see cref="MagnetometerDataParser"/>.
/// </summary>
public sealed class MagnetometerDataParserTests
{
    [Theory(DisplayName = "RTSW magnetometer parser returns expected records for valid payload")]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.NoaaClient/Rtsw/Samples/RtswMagnetometerSample.json")]
    public void Parse_WhenTextIsValid_ReturnsRecords(string text)
    {
        // Arrange, Act
        var result = MagnetometerDataParser.Parse(text);

        // Assert
        result.ShouldNotBeEmpty();
        result.ShouldBe([
            new MagnetometerRecord(
                DateTime: new DateTime(2026, 7, 5, 19, 47, 0, DateTimeKind.Utc),
                Active: true,
                Source: "SOLAR1",
                Range: null,
                Scale: null,
                Sensitivity: null,
                ManualMode: false,
                SampleSize: 60,
                Bt: 5.4f,
                BxGse: -2.99f,
                ByGse: 0.11f,
                BzGse: -4.51f,
                ThetaGse: -56.4f,
                PhiGse: 177.79f,
                BxGsm: -2.99f,
                ByGsm: 0.21f,
                BzGsm: -4.51f,
                ThetaGsm: -56.36f,
                PhiGsm: 175.97f,
                MaxTelemetryFlag: 0,
                MaxDataFlag: -9999,
                OverallQuality: 0),
            new MagnetometerRecord(
                DateTime: new DateTime(2026, 7, 5, 19, 46, 0, DateTimeKind.Utc),
                Active: false,
                Source: "ACE",
                Range: null,
                Scale: null,
                Sensitivity: null,
                ManualMode: false,
                SampleSize: 60,
                Bt: 4.8f,
                BxGse: -1.2f,
                ByGse: 2.3f,
                BzGse: -3.9f,
                ThetaGse: -54.1f,
                PhiGse: 117.5f,
                BxGsm: null,
                ByGsm: 2.4f,
                BzGsm: -3.9f,
                ThetaGsm: -54.0f,
                PhiGsm: 116.2f,
                MaxTelemetryFlag: 0,
                MaxDataFlag: 0,
                OverallQuality: 0),
            new MagnetometerRecord(
                DateTime: new DateTime(2026, 7, 5, 19, 45, 0, DateTimeKind.Utc),
                Active: false,
                Source: "IMAP",
                Range: null,
                Scale: null,
                Sensitivity: null,
                ManualMode: false,
                SampleSize: 60,
                Bt: 6.1f,
                BxGse: -3.5f,
                ByGse: -0.8f,
                BzGse: -4.9f,
                ThetaGse: -53.2f,
                PhiGse: 192.9f,
                BxGsm: -3.5f,
                ByGsm: -0.6f,
                BzGsm: -4.9f,
                ThetaGsm: -53.1f,
                PhiGsm: 189.7f,
                MaxTelemetryFlag: 0,
                MaxDataFlag: 0,
                OverallQuality: 0)
        ]);
    }

    [Theory(DisplayName = "RTSW magnetometer parser returns empty list for empty payload")]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.NoaaClient/Rtsw/Samples/EmptyArray.json")]
    public void Parse_WhenTextContainsEmptyArray_ReturnsEmpty(string text)
    {
        // Arrange, Act
        var result = MagnetometerDataParser.Parse(text);

        // Assert
        result.ShouldBeEmpty();
    }

    [Theory(DisplayName = "RTSW magnetometer parser throws for malformed payload")]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.NoaaClient/Rtsw/Samples/RtswMagnetometerMalformedSample.json")]
    public void Parse_WhenPayloadIsMalformed_ThrowsInvalidOperationException(string text)
    {
        // Arrange, Act
        var exception = Should.Throw<InvalidOperationException>(() => MagnetometerDataParser.Parse(text));

        // Assert
        exception.Message.ShouldContain("time_tag");
    }
}
