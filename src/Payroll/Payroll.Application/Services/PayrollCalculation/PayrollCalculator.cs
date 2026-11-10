namespace Payroll.Application.Services.PayrollCalculation;

public class PayrollCalculator
{
    public PayrollCalculationResult Calculate(
        decimal baseSalary,
        decimal workingDays,
        decimal standardWorkingDay,
        decimal totalAllowance,
        decimal totalPayCut)
    {
        // 1. Validate input
        if (baseSalary < 0)
            throw new ArgumentException("Base salary cannot be negative.");

        if (workingDays < 0)
            throw new ArgumentException("Working days cannot be negative.");

        if (standardWorkingDay <= 0)
            throw new ArgumentException("Standard working days must be greater than zero.");

        if (totalAllowance < 0)
            throw new ArgumentException("Total allowance cannot be negative.");

        if (totalPayCut < 0)
            throw new ArgumentException("Total pay cut cannot be negative.");

        // 2. Calculate salary based on working days
        var proratedSalary = baseSalary
            * workingDays
            / standardWorkingDay;

        // 3. Calculate gross salary
        var grossSalary = proratedSalary + totalAllowance;

        // 4. Calculate net salary
        var netSalary = grossSalary - totalPayCut;

        // 5. Round monetary values to 2 decimal places
        proratedSalary = Math.Round(
            proratedSalary, 2, MidpointRounding.AwayFromZero);

        grossSalary = Math.Round(
            grossSalary, 2, MidpointRounding.AwayFromZero);

        netSalary = Math.Round(
            netSalary, 2, MidpointRounding.AwayFromZero);

        // 6. Return result
        return new PayrollCalculationResult
        {
            ProratedSalary = proratedSalary,
            TotalAllowance = totalAllowance,
            TotalPayCut = totalPayCut,
            GrossSalary = grossSalary,
            NetSalary = netSalary
        };
    }
}