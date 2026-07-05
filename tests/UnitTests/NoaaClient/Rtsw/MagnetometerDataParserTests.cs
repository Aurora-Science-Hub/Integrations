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
                DateTime: new DateTime(2024, 5, 19, 17, 7, 0, DateTimeKind.Utc),
                Bx: -2.15f,
                By: 8.51f,
                Bz: -2.98f,
                Bt: 9.27f,
                Latitude: -18.76f,
                Longitude: 104.17f,
                Active: true,
                Source: "IMAP"),
            new MagnetometerRecord(
                DateTime: new DateTime(2024, 5, 19, 17, 8, 0, DateTimeKind.Utc),
                Bx: null,
                By: 8.31f,
                Bz: -3.24f,
                Bt: 9.23f,
                Latitude: -20.54f,
                Longitude: 105.97f,
                Active: false,
                Source: "SOLAR1")
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
