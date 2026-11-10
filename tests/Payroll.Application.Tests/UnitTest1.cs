using Payroll.Application.Services.PayrollCalculation;

namespace Payroll.Application.Tests;

public class PayrollCalculatorTests
{
    private readonly PayrollCalculator _calculator = new();

    [Fact]
    public void Calculate_FullWorkingDays_ReturnsCorrectSalary()
    {
        // Arrange
        decimal baseSalary = 10_000_000m;
        decimal workingDays = 22m;
        decimal standardWorkingDay = 22m;
        decimal totalAllowance = 500_000m;
        decimal totalPayCut = 50_000m;

        // Act
        var result = _calculator.Calculate(
            baseSalary,
            workingDays,
            standardWorkingDay,
            totalAllowance,
            totalPayCut);

        // Assert
        Assert.Equal(10_000_000m, result.ProratedSalary);
        Assert.Equal(500_000m, result.TotalAllowance);
        Assert.Equal(50_000m, result.TotalPayCut);
        Assert.Equal(10_500_000m, result.GrossSalary);
        Assert.Equal(10_450_000m, result.NetSalary);
    }

    [Fact]
    public void Calculate_HalfWorkingDays_ReturnsHalfSalary()
    {
        var result = _calculator.Calculate(
            10_000_000m,
            11m,
            22m,
            500_000m,
            50_000m);

        Assert.Equal(5_000_000m, result.ProratedSalary);
        Assert.Equal(5_500_000m, result.GrossSalary);
        Assert.Equal(5_450_000m, result.NetSalary);
    }

    [Fact]
    public void Calculate_ZeroWorkingDays_ReturnsZeroSalary()
    {
        var result = _calculator.Calculate(
            10_000_000m,
            0m,
            22m,
            0m,
            0m);

        Assert.Equal(0m, result.ProratedSalary);
        Assert.Equal(0m, result.GrossSalary);
        Assert.Equal(0m, result.NetSalary);
    }

    [Fact]
    public void Calculate_DeductionEqualsGrossSalary_ReturnsZeroNetSalary()
    {
        var result = _calculator.Calculate(
            10_000_000m,
            22m,
            22m,
            500_000m,
            10_500_000m);

        Assert.Equal(10_500_000m, result.GrossSalary);
        Assert.Equal(0m, result.NetSalary);
    }

    [Theory]
    [InlineData(-1, 22, 22, 0, 0)]
    [InlineData(10_000_000, -1, 22, 0, 0)]
    [InlineData(10_000_000, 22, 0, 0, 0)]
    [InlineData(10_000_000, 22, 22, -1, 0)]
    [InlineData(10_000_000, 22, 22, 0, -1)]
    public void Calculate_InvalidInput_ThrowsArgumentException(
        decimal baseSalary,
        decimal workingDays,
        decimal standardWorkingDay,
        decimal totalAllowance,
        decimal totalPayCut)
    {
        Assert.Throws<ArgumentException>(() =>
            _calculator.Calculate(
                baseSalary,
                workingDays,
                standardWorkingDay,
                totalAllowance,
                totalPayCut));
    }
}