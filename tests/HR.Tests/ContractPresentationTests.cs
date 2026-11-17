using Admin.WinForms.BUS.Models.HR;
using Xunit;

namespace HR.Tests;

public sealed class ContractPresentationTests
{
    private static readonly DateOnly Today = new(2026, 11, 16);

    [Theory]
    [InlineData(1, 60, ContractPeriodState.NotStarted, "Bắt đầu sau 1 ngày")]
    [InlineData(-10, 60, ContractPeriodState.InEffect, "Còn 60 ngày")]
    [InlineData(-10, 31, ContractPeriodState.InEffect, "Còn 31 ngày")]
    [InlineData(-10, 30, ContractPeriodState.ExpiringSoon, "Còn 30 ngày")]
    [InlineData(-10, 1, ContractPeriodState.ExpiringSoon, "Còn 1 ngày")]
    [InlineData(-10, 0, ContractPeriodState.ExpiringSoon, "Hết hạn hôm nay")]
    [InlineData(-10, -1, ContractPeriodState.Expired, "Quá hạn 1 ngày")]
    [InlineData(-10, null, ContractPeriodState.InEffect, "Không thời hạn")]
    [InlineData(1, null, ContractPeriodState.NotStarted, "Bắt đầu sau 1 ngày")]
    public void DateBoundaries_ProduceCorrectPeriodAndRemainingTime(
        int startOffset, int? endOffset, ContractPeriodState state, string remaining)
    {
        var row = View("HD01", startOffset, endOffset);
        Assert.Equal(state, row.PeriodState);
        Assert.Equal(remaining, row.TimeRemaining);
    }

    [Fact]
    public void LastContractDay_IsStillCovered_ButFollowingDayIsExpired()
    {
        var row = View("HD01", -100, 0);
        Assert.True(row.IsEffective);
        var followingDay = row with { ReferenceDate = Today.AddDays(1) };
        Assert.False(followingDay.IsEffective);
        Assert.Equal(ContractPeriodState.Expired, followingDay.PeriodState);
    }

    [Fact]
    public void FirstContractDay_IsAlreadyCovered_IncludingUnlimitedContract()
    {
        var row = View("HD01", 0, null);
        Assert.True(row.IsEffective);
        Assert.False((row with { ReferenceDate = Today.AddDays(-1) }).IsEffective);
    }

    [Theory]
    [InlineData("ACTIVE")]
    [InlineData("RENEWED")]
    public void ManagementStatus_DoesNotHideAnExpiredDate(string status)
    {
        var row = View("HD01", -100, -1, status);
        Assert.Equal(ContractPeriodState.Expired, row.PeriodState);
        Assert.Equal(status, row.Contract.Status);
    }

    [Fact]
    public void RenewedPredecessor_StillCoversToday_BeforeFutureSuccessorStarts()
    {
        ContractDisplayRow[] rows = [View("HD-CU", -100, 1, "RENEWED"), View("HD-MOI", 2, 366)];
        var result = ContractPresentation.Summarize(rows);
        Assert.True(result.HasEffectiveContract);
        Assert.Contains("HD-CU", result.Text);
        Assert.DoesNotContain("HD-MOI", result.Text);
    }

    [Fact]
    public void ExpiredOldContract_DoesNotMakeEmployeeUncovered_WhenNewContractStartsToday()
    {
        ContractDisplayRow[] rows = [View("HD-CU", -100, -1, "RENEWED"), View("HD-MOI", 0, 365)];
        var result = ContractPresentation.Summarize(rows);
        Assert.True(result.HasEffectiveContract);
        Assert.Contains("HD-MOI", result.Text);
        Assert.DoesNotContain("HD-CU", result.Text);
    }

    [Fact]
    public void GapBeforeFutureContract_IsNotReportedAsCurrentlyCovered()
    {
        ContractDisplayRow[] rows = [View("HD-CU", -100, -1, "RENEWED"), View("HD-MOI", 1, 365)];
        var result = ContractPresentation.Summarize(rows);
        Assert.False(result.HasEffectiveContract);
        Assert.Contains("Chưa có hợp đồng có hiệu lực hôm nay", result.Text);
        Assert.Contains("15/11/2026", result.Text);
        Assert.Contains("17/11/2026", result.Text);
    }

    [Fact]
    public void NoContracts_AndOnlyFutureContracts_HaveDifferentSummaries()
    {
        Assert.Equal("Chưa có hợp đồng.", ContractPresentation.Summarize([]).Text);
        var future = ContractPresentation.Summarize([View("HD-MOI", 1, null)]);
        Assert.False(future.HasEffectiveContract);
        Assert.Contains("Hợp đồng tiếp theo bắt đầu", future.Text);
    }

    [Fact]
    public void Filters_IncludeExpiringInEffective_AndKeepAllContractsForOverview()
    {
        ContractDisplayRow[] rows = [View("OLD", -100, -1, "RENEWED"), View("SOON", -10, 10),
            View("CURRENT", -10, 100), View("FUTURE", 1, null)];
        Assert.Equal(4, rows.Count(row => row.Matches(ContractPeriodFilter.All)));
        Assert.Equal(new[] { "SOON", "CURRENT" }, rows.Where(row => row.Matches(ContractPeriodFilter.InEffect)).Select(row => row.ContractNumber));
        Assert.Equal("SOON", Assert.Single(rows, row => row.Matches(ContractPeriodFilter.ExpiringSoon)).ContractNumber);
        Assert.Equal("OLD", Assert.Single(rows, row => row.Matches(ContractPeriodFilter.Expired)).ContractNumber);
        Assert.Equal("FUTURE", Assert.Single(rows, row => row.Matches(ContractPeriodFilter.NotStarted)).ContractNumber);
        Assert.True(ContractPresentation.Summarize(rows).HasEffectiveContract);
    }

    private static ContractDisplayRow View(string number, int startOffset, int? endOffset, string status = "ACTIVE") => new(
        new ContractRow(Guid.NewGuid(), Guid.NewGuid(), number, "Hợp đồng thử", Today.AddDays(startOffset),
            endOffset is int days ? Today.AddDays(days) : null, status, null, null, DateTime.UtcNow, DateTime.UtcNow), Today);
}
