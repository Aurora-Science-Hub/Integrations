using AuroraScienceHub.Integrations.FmiClient.FmiElectrojet.Extensions;
using AuroraScienceHub.Integrations.FmiClient.FmiElectrojet.Responses;
using AuroraScienceHub.Integrations.UnitTests.Utils;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.FmiClient.FmiElectrojet;

public sealed class FmiElectrojetTextParserTests
{
    [Theory(DisplayName = "FMI electrojet parser reads a real FMI payload including a second-60 timestamp")]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.FmiClient/FmiElectrojet/Samples/FmiElectrojetRealtimeSample.txt")]
    public void Parse_WhenRealPayload_ReturnsUtcRecordsWithBothIndices(string text)
    {
        // Arrange / Act
        var result = FmiElectrojetTextParser.Parse(text);

        // Assert
        result.Count.ShouldBe(8);
        result[0].ShouldBe(new FmiElectrojetRecord(
            new DateTime(2026, 11, 4, 15, 15, 0, DateTimeKind.Utc),
            -4.2f,
            -8.0f,
            10));
        result[^1].ShouldBe(new FmiElectrojetRecord(
            new DateTime(2026, 11, 4, 15, 22, 0, DateTimeKind.Utc),
            -1.4f,
            -7.3f,
            10));
        result.ShouldAllBe(record => record.DateTime.Kind == DateTimeKind.Utc);
    }

    [Fact(DisplayName = "FMI electrojet parser maps NaN indices to null and accepts CRLF")]
    public void Parse_WhenMissingValues_ReturnsNullIndices()
    {
        // Arrange
        const string text = "% header\r\n2026 11 04 15 15 00 NaN NaN 8\r\n";

        // Act
        var result = FmiElectrojetTextParser.Parse(text);

        // Assert
        var record = result.ShouldHaveSingleItem();
        record.Iu.ShouldBeNull();
        record.Il.ShouldBeNull();
        record.StationCount.ShouldBe(8);
    }

    [Fact(DisplayName = "FMI electrojet parser skips malformed rows without losing valid data")]
    public void Parse_WhenMalformedRows_SkipsThem()
    {
        // Arrange
        const string text = """
            garbage
            2026 13 04 15 15 00 1.0 2.0 10
            2026 11 04 15 15 61 1.0 2.0 10
            2026 11 04 15 15 00 invalid 2.0 10
            2026 11 04 15 15 00 1.0 2.0
            2026 11 04 15 16 00 1.0 2.0 10 extra
            2026 11 04 15 17 00 1.0 2.0 10
            """;

        // Act
        var result = FmiElectrojetTextParser.Parse(text);

        // Assert
        result.ShouldBe([
            new FmiElectrojetRecord(new DateTime(2026, 11, 4, 15, 17, 0, DateTimeKind.Utc), 1.0f, 2.0f, 10),
        ]);
    }
}
