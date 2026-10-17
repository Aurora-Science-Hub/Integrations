using AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex.Extensions;
using AuroraScienceHub.Integrations.DtuSpaceClient.PcIndex.Responses;
using AuroraScienceHub.Integrations.UnitTests.Utils;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.DtuSpaceClient.PcIndex;

/// <summary>
/// Unit tests for <see cref="PcIndexCsvParser"/>.
/// </summary>
public sealed class PcIndexCsvParserTests
{
    [Theory(DisplayName = "PC index CSV parser returns expected records for quick-look payload")]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.DtuSpaceClient/PcIndex/Samples/PcnQuickLookSample.csv")]
    public void Parse_WhenQuickLookPayload_ReturnsRecords(string text)
    {
        // Arrange, Act
        var result = PcIndexCsvParser.Parse(text);

        // Assert
        result.ShouldBe([
            new PcIndexRecord(new DateTime(2026, 9, 18, 2, 46, 0, DateTimeKind.Utc), 0.00f),
            new PcIndexRecord(new DateTime(2026, 9, 18, 2, 47, 0, DateTimeKind.Utc), 0.02f),
            new PcIndexRecord(new DateTime(2026, 9, 18, 2, 48, 0, DateTimeKind.Utc), null),
            new PcIndexRecord(new DateTime(2026, 9, 18, 2, 49, 0, DateTimeKind.Utc), null),
        ]);
    }

    [Fact(DisplayName = "PC index CSV parser returns UTC date times")]
    public void Parse_WhenValidRow_ReturnsUtcDateTime()
    {
        // Arrange
        const string text = "2026-10-17 03:32:00.000,-0.29";

        // Act
        var result = PcIndexCsvParser.Parse(text);

        // Assert
        var record = result.ShouldHaveSingleItem();
        record.DateTime.Kind.ShouldBe(DateTimeKind.Utc);
        record.DateTime.ShouldBe(new DateTime(2026, 10, 17, 3, 32, 0, DateTimeKind.Utc));
        record.Value.ShouldBe(-0.29f);
    }

    [Fact(DisplayName = "PC index CSV parser tolerates trailing separators and CRLF line endings")]
    public void Parse_WhenTrailingSeparatorAndCrLf_ReturnsRecords()
    {
        // Arrange
        const string text = "date,pcn,\r\n2026-09-16 01:38:00.000,2.30,\r\n2026-09-16 01:39:00.000,2.14,\r\n";

        // Act
        var result = PcIndexCsvParser.Parse(text);

        // Assert
        result.ShouldBe([
            new PcIndexRecord(new DateTime(2026, 9, 16, 1, 38, 0, DateTimeKind.Utc), 2.30f),
            new PcIndexRecord(new DateTime(2026, 9, 16, 1, 39, 0, DateTimeKind.Utc), 2.14f),
        ]);
    }

    [Fact(DisplayName = "PC index CSV parser skips malformed rows")]
    public void Parse_WhenMalformedRows_SkipsThem()
    {
        // Arrange
        const string text = """
            date,pcn

            garbage
            2026-10-17 00:00:00.000,
            2026-10-17,0.10
            2026-10-17 00:01:00.000,abc
            2026-10-17 00:02:00.000,0.15
            """;

        // Act
        var result = PcIndexCsvParser.Parse(text);

        // Assert
        result.ShouldBe([
            new PcIndexRecord(new DateTime(2026, 10, 17, 0, 2, 0, DateTimeKind.Utc), 0.15f),
        ]);
    }
}
