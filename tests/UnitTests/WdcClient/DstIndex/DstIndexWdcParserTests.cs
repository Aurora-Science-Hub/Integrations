using AuroraScienceHub.Integrations.UnitTests.Utils;
using AuroraScienceHub.Integrations.WdcClient.DstIndex.Extensions;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.WdcClient.DstIndex;

public sealed class DstIndexWdcParserTests
{
    [Theory(DisplayName = "Dst parser reads real Kyoto fixed-width data and ignores the daily mean and footer")]
    [EmbeddedResourceData("AuroraScienceHub.Integrations.UnitTests.WdcClient/DstIndex/Samples/DstRealtimeSample.txt")]
    public void Parse_WhenRealPayload_ReturnsHourlyUtcRecords(string text)
    {
        // Arrange / Act
        var result = DstIndexWdcParser.Parse(text);

        // Assert
        result.Count.ShouldBe(120);
        result[0].DateTime.ShouldBe(new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc));
        result[0].DateTime.Kind.ShouldBe(DateTimeKind.Utc);
        result[0].Value.ShouldBe(-45);
        result[23].DateTime.Hour.ShouldBe(23);
        result[23].Value.ShouldBe(-13);
        result[24].Value.ShouldBe(-11);
        result[95].Value.ShouldBeNull();
        result.Skip(96).ShouldAllBe(record => record.Value == null);
    }

    [Fact(DisplayName = "Dst parser applies the base in hundreds of nT and preserves missing values")]
    public void Parse_WhenNonzeroBase_AddsBaseOnlyToPresentValues()
    {
        // Arrange
        var line = "DST2611*01RRX020   2" + " -45" + new string('9', 23 * 4) + " -35";

        // Act
        var result = DstIndexWdcParser.Parse(line);

        // Assert
        result.Count.ShouldBe(24);
        result[0].Value.ShouldBe(155);
        result.Skip(1).ShouldAllBe(record => record.Value == null);
    }

    [Theory(DisplayName = "Dst parser reads explicit and legacy century fields")]
    [InlineData("19", 1999)]
    [InlineData("  ", 1999)]
    [InlineData("20", 2099)]
    public void Parse_WhenCenturyField_ReturnsFullYear(string century, int year)
    {
        // Arrange
        var line = "DST9901*01  X2" + century + "   0" + string.Concat(Enumerable.Repeat("   1", 25));

        // Act
        var result = DstIndexWdcParser.Parse(line);

        // Assert
        result.Count.ShouldBe(24);
        result[0].DateTime.Year.ShouldBe(year);
    }

    [Theory(DisplayName = "Dst parser skips invalid dates and truncated or unrelated lines")]
    [InlineData("DST2613*01RRX020   0")]
    [InlineData("DST2602*30RRX020   0")]
    [InlineData("DST2611*00RRX020   0")]
    [InlineData("DST2611*01RRX0xx   0")]
    [InlineData("DST2611*01RRX020xxxx")]
    [InlineData("XYZ2611*01RRX020   0")]
    [InlineData("DST2611 01RRX020   0")]
    public void Parse_WhenMalformedHeader_SkipsRow(string header)
    {
        // Arrange
        var text = header + string.Concat(Enumerable.Repeat("   1", 25));

        // Act
        var result = DstIndexWdcParser.Parse(text + "\r\nDST2611\r\n[Created at today]\r\n");

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Dst parser skips an invalid hourly field without shifting the remaining hours")]
    public void Parse_WhenMalformedValue_PreservesHourPositions()
    {
        // Arrange
        var line = "DST2611*01RRX020   0xxxx   0" + string.Concat(Enumerable.Repeat("  -1", 23));

        // Act
        var result = DstIndexWdcParser.Parse(line);

        // Assert
        result.Count.ShouldBe(23);
        result[0].DateTime.Hour.ShouldBe(1);
        result[0].Value.ShouldBe(0);
        result[^1].DateTime.Hour.ShouldBe(23);
        result[^1].Value.ShouldBe(-1);
    }
}
