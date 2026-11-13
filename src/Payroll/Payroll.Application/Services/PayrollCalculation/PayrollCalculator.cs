namespace Payroll.Application.Services.PayrollCalculation;

public class PayrollCalculator
{
    private const decimal StandardWorkingHoursPerDay = 8m;

    public PayrollCalculationResult Calculate(
        string employmentType,
        decimal baseSalary,
        decimal? hourlyRate,
        decimal workingDays,
        decimal standardWorkingDay,
        decimal totalWorkingHours,
        decimal otHours,
        decimal otMultiplier,
        decimal totalAllowance,
        decimal totalPayCut)
    {
        // 1. Validate common inputs
        if (workingDays < 0)
            throw new ArgumentException("Working days cannot be negative.");

        if (totalWorkingHours < 0)
            throw new ArgumentException("Total working hours cannot be negative.");

        if (otHours < 0)
            throw new ArgumentException("OT hours cannot be negative.");

        if (otMultiplier <= 0)
            throw new ArgumentException("OT multiplier must be greater than zero.");

        if (totalAllowance < 0)
            throw new ArgumentException("Total allowance cannot be negative.");

        if (totalPayCut < 0)
            throw new ArgumentException("Total pay cut cannot be negative.");

        decimal regularSalary;
        decimal otHourlyRate;

        // 2. Calculate regular salary based on employment type
        if (string.Equals(
            employmentType,
            "FULL_TIME",
            StringComparison.OrdinalIgnoreCase))
        {
            if (baseSalary < 0)
                throw new ArgumentException("Base salary cannot be negative.");

            if (standardWorkingDay <= 0)
                throw new ArgumentException(
                    "Standard working days must be greater than zero.");

            regularSalary = baseSalary
                * workingDays
                / standardWorkingDay;

            otHourlyRate = baseSalary
                / (standardWorkingDay * StandardWorkingHoursPerDay);
        }
        else if (string.Equals(
            employmentType,
            "PART_TIME",
            StringComparison.OrdinalIgnoreCase))
        {
            if (!hourlyRate.HasValue || hourlyRate.Value < 0)
                throw new ArgumentException("A valid hourly rate is required.");

            regularSalary = hourlyRate.Value * totalWorkingHours;

            otHourlyRate = hourlyRate.Value;
        }
        else
        {
            throw new ArgumentException(
                $"Unsupported employment type: {employmentType}");
        }

        // 3. Calculate overtime pay
        var otPay = otHourlyRate * otHours * otMultiplier;

        // 4. Round monetary values
        regularSalary = Math.Round(
            regularSalary, 2, MidpointRounding.AwayFromZero);

        otPay = Math.Round(
            otPay, 2, MidpointRounding.AwayFromZero);

        totalAllowance = Math.Round(
            totalAllowance, 2, MidpointRounding.AwayFromZero);

        totalPayCut = Math.Round(
            totalPayCut, 2, MidpointRounding.AwayFromZero);

        // 5. Calculate gross and net salary
        var grossSalary = Math.Round(
            regularSalary + totalAllowance + otPay,
            2,
            MidpointRounding.AwayFromZero);

        var netSalary = Math.Round(
            grossSalary - totalPayCut,
            2,
            MidpointRounding.AwayFromZero);

        return new PayrollCalculationResult
        {
            ProratedSalary = regularSalary,
            OTPay = otPay,
            TotalAllowance = totalAllowance,
            TotalPayCut = totalPayCut,
            GrossSalary = grossSalary,
            NetSalary = netSalary
        };
    }
}