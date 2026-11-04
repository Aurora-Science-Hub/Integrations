using AuroraScienceHub.Integrations.Samples.WdcClientSample;
using AuroraScienceHub.Integrations.WdcClient.DstIndex.Responses;
using Shouldly;

namespace AuroraScienceHub.Integrations.UnitTests.WdcClient;

public sealed class SampleDataTests
{
    [Fact(DisplayName = "Today sample excludes future placeholders while retaining elapsed missing hours")]
    public void GetElapsedHours_WhenFullDay_ReturnsOnlyElapsedHours()
    {
        // Arrange
        var now = new DateTime(2026, 11, 4, 11, 30, 0, DateTimeKind.Utc);
        DstIndexRecord[] records = [new(now.Date.AddHours(9), -27), new(now.Date.AddHours(10), null),
            new(now.Date.AddHours(11), -20), new(now.Date.AddHours(14), null), new(now.Date.AddHours(23), null)];

        // Act
        var result = SampleData.GetElapsedHours(records, now);

        // Assert
        result.ShouldBe(records.Take(3));
    }

    [Theory(DisplayName = "Sample rejects historical, future and reversed ranges before HTTP")]
    [InlineData(2026, 5, 1, 2026, 6, 23, false)]
    [InlineData(2026, 10, 1, 2026, 11, 4, true)]
    [InlineData(2026, 9, 30, 2026, 11, 4, false)]
    [InlineData(2026, 11, 1, 2026, 11, 5, false)]
    [InlineData(2026, 11, 4, 2026, 11, 3, false)]
    public void IsSupportedRange_WhenGivenDates_ValidatesArchiveWindow(
        int fromYear,
        int fromMonth,
        int fromDay,
        int toYear,
        int toMonth,
        int toDay,
        bool expected)
    {
        // Arrange
        var today = new DateOnly(2026, 11, 4);

        // Act
        var valid = SampleData.IsSupportedRange(new DateOnly(fromYear, fromMonth, fromDay), new DateOnly(toYear, toMonth, toDay), today);

        // Assert
        valid.ShouldBe(expected);
    }

    [Fact(DisplayName = "Sample archive window crosses the UTC year boundary")]
    public void GetEarliestDate_WhenJanuary_ReturnsPreviousDecember()
    {
        // Arrange
        var today = new DateOnly(2027, 1, 1);

        // Act
        var earliest = SampleData.GetEarliestDate(today);

        // Assert
        earliest.ShouldBe(new DateOnly(2026, 12, 1));
    }
}
