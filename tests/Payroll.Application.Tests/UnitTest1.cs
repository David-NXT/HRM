using Payroll.Application.Services.PayrollCalculation;

namespace Payroll.Application.Tests;

public class PayrollCalculatorTests
{
    private readonly PayrollCalculator _calculator = new();

    [Fact]
    public void Calculate_FullTimeFullWorkingDays_ReturnsCorrectSalary()
    {
        var result = _calculator.Calculate(
            "FULL_TIME",
            10_000_000m,
            null,
            22m,
            22m,
            176m,
            0m,
            1.5m,
            500_000m,
            50_000m);

        Assert.Equal(10_000_000m, result.ProratedSalary);
        Assert.Equal(0m, result.OTPay);
        Assert.Equal(500_000m, result.TotalAllowance);
        Assert.Equal(50_000m, result.TotalPayCut);
        Assert.Equal(10_500_000m, result.GrossSalary);
        Assert.Equal(10_450_000m, result.NetSalary);
    }

    [Fact]
    public void Calculate_ProjectAllowance_IsIncludedInGrossAndTotalAllowance()
    {
        var result = _calculator.Calculate(
            "FULL_TIME",
            10_000_000m,
            null,
            22m,
            22m,
            176m,
            0m,
            1.5m,
            500_000m,
            50_000m,
            totalProjectAllowance: 1_250_000m);

        Assert.Equal(1_750_000m, result.TotalAllowance);
        Assert.Equal(11_750_000m, result.GrossSalary);
        Assert.Equal(11_700_000m, result.NetSalary);
    }

    [Fact]
    public void Calculate_NegativeProjectAllowance_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _calculator.Calculate(
            "FULL_TIME",
            10_000_000m,
            null,
            22m,
            22m,
            176m,
            0m,
            1.5m,
            0m,
            0m,
            totalProjectAllowance: -1m));
    }

    [Fact]
    public void Calculate_FullTimeHalfWorkingDays_ReturnsHalfSalary()
    {
        var result = _calculator.Calculate(
            "FULL_TIME",
            10_000_000m,
            null,
            11m,
            22m,
            88m,
            0m,
            1.5m,
            0m,
            0m);

        Assert.Equal(5_000_000m, result.ProratedSalary);
        Assert.Equal(5_000_000m, result.GrossSalary);
        Assert.Equal(5_000_000m, result.NetSalary);
    }

    [Fact]
    public void Calculate_PartTime_ReturnsCorrectSalary()
    {
        var result = _calculator.Calculate(
            "PART_TIME",
            0m,
            50_000m,
            12m,
            22m,
            48m,
            0m,
            1.5m,
            0m,
            0m);

        Assert.Equal(2_400_000m, result.ProratedSalary);
        Assert.Equal(0m, result.OTPay);
        Assert.Equal(2_400_000m, result.GrossSalary);
        Assert.Equal(2_400_000m, result.NetSalary);
    }

    [Fact]
    public void Calculate_FullTimeWithOvertime_ReturnsCorrectOTPay()
    {
        var result = _calculator.Calculate(
            "FULL_TIME",
            15_000_000m,
            null,
            20m,
            22m,
            160m,
            10m,
            1.5m,
            0m,
            0m);

        Assert.Equal(13_636_363.64m, result.ProratedSalary);
        Assert.Equal(1_278_409.09m, result.OTPay);
        Assert.Equal(14_914_772.73m, result.GrossSalary);
        Assert.Equal(14_914_772.73m, result.NetSalary);
    }

    [Fact]
    public void Calculate_PartTimeWithOvertime_ReturnsCorrectOTPay()
    {
        var result = _calculator.Calculate(
            "PART_TIME",
            0m,
            50_000m,
            12m,
            22m,
            48m,
            5m,
            1.5m,
            0m,
            0m);

        Assert.Equal(2_400_000m, result.ProratedSalary);
        Assert.Equal(375_000m, result.OTPay);
        Assert.Equal(2_775_000m, result.GrossSalary);
        Assert.Equal(2_775_000m, result.NetSalary);
    }

    [Fact]
    public void Calculate_DeductionEqualsGrossSalary_ReturnsZeroNetSalary()
    {
        var result = _calculator.Calculate(
            "FULL_TIME",
            10_000_000m,
            null,
            22m,
            22m,
            176m,
            0m,
            1.5m,
            500_000m,
            10_500_000m);

        Assert.Equal(10_500_000m, result.GrossSalary);
        Assert.Equal(0m, result.NetSalary);
    }

    [Theory]
    [InlineData("2025-11-30", "2026-11-29", 0)]
    [InlineData("2025-11-30", "2026-11-30", 1)]
    [InlineData("2025-11-30", "2027-11-30", 2)]
    public void CalculateCompletedServiceYears_UsesCompletedAnniversaries(
        string employmentStartDate,
        string periodEndDate,
        int expectedYears)
    {
        int actual = _calculator.CalculateCompletedServiceYears(
            DateOnly.Parse(employmentStartDate),
            DateOnly.Parse(periodEndDate));

        Assert.Equal(expectedYears, actual);
    }

    [Fact]
    public void CalculateSeniorityAllowance_AppliesFixedAmountAfterOneCompletedYear()
    {
        decimal amount = _calculator.CalculateSeniorityAllowance(
            new DateOnly(2025, 11, 30),
            new DateOnly(2026, 11, 30),
            250_000m);

        Assert.Equal(250_000m, amount);
    }

    [Fact]
    public void CalculateSeniorityAllowance_DoesNotApplyBeforeFirstAnniversary()
    {
        decimal amount = _calculator.CalculateSeniorityAllowance(
            new DateOnly(2025, 11, 30),
            new DateOnly(2026, 11, 29),
            250_000m);

        Assert.Equal(0m, amount);
    }

    [Fact]
    public void CalculateLateDeduction_MultipliesConfiguredRateByOccurrences()
    {
        decimal amount = _calculator.CalculateLateDeduction(3, 100_000m);

        Assert.Equal(300_000m, amount);
    }

    [Theory]
    [InlineData("FULL_TIME", -1, 22, 22, 176, 0, 1.5, 0, 0)]
    [InlineData("FULL_TIME", 10_000_000, -1, 22, 176, 0, 1.5, 0, 0)]
    [InlineData("FULL_TIME", 10_000_000, 22, 0, 176, 0, 1.5, 0, 0)]
    [InlineData("FULL_TIME", 10_000_000, 22, 22, -1, 0, 1.5, 0, 0)]
    [InlineData("FULL_TIME", 10_000_000, 22, 22, 176, -1, 1.5, 0, 0)]
    [InlineData("FULL_TIME", 10_000_000, 22, 22, 176, 0, 0, 0, 0)]
    [InlineData("FULL_TIME", 10_000_000, 22, 22, 176, 0, 1.5, -1, 0)]
    [InlineData("FULL_TIME", 10_000_000, 22, 22, 176, 0, 1.5, 0, -1)]
    public void Calculate_InvalidInput_ThrowsArgumentException(
        string employmentType,
        decimal baseSalary,
        decimal workingDays,
        decimal standardWorkingDay,
        decimal totalWorkingHours,
        decimal otHours,
        decimal otMultiplier,
        decimal totalAllowance,
        decimal totalPayCut)
    {
        Assert.Throws<ArgumentException>(() =>
            _calculator.Calculate(
                employmentType,
                baseSalary,
                null,
                workingDays,
                standardWorkingDay,
                totalWorkingHours,
                otHours,
                otMultiplier,
                totalAllowance,
                totalPayCut));
    }
}