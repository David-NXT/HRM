namespace Payroll.Domain.Entities;

public class Payroll
{
    public Guid PayrollID { get; set; } = Guid.NewGuid();
    public Guid PayrollPeriodID { get; set; }
    public Guid EmployeeID { get; set; }
    public string? EmploymentType { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal WorkingDays { get; set; }
    public decimal TotalWorkingHours { get; set; }
    public decimal? OTHours { get; set; }
    public decimal? OTMultiplier { get; set; }
    public decimal OTPay { get; set; }
    public decimal StandardWorkingDay { get; set; } = 22m;
    public decimal TotalAllowance { get; set; }
    public decimal TotalProjectAllowance { get; set; }
    public decimal TotalPayCut { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }
    public string Status { get; set; } = "draft"; // draft, calculated, approved, paid
    public DateTime? CalculatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PaidAt { get; set; }

    // Relationships
    public PayrollPeriod? PayrollPeriod { get; set; }
    public List<PayCut> PayCuts { get; set; } = new();
    public List<Allowance> Allowances { get; set; } = new();

    public decimal CalculateRegularSalary()
    {
        if (string.Equals(EmploymentType, "PART_TIME", StringComparison.OrdinalIgnoreCase))
        {
            decimal hourlyRate = HourlyRate ?? 0m;
            return Math.Round(hourlyRate * TotalWorkingHours, 2, MidpointRounding.AwayFromZero);
        }

        if (StandardWorkingDay <= 0)
            throw new InvalidOperationException("Standard working days must be greater than zero.");

        return Math.Round(
            BaseSalary * WorkingDays / StandardWorkingDay,
            2,
            MidpointRounding.AwayFromZero);
    }

    public decimal CalculateOTPay()
    {
        decimal otHours = OTHours ?? 0m;
        decimal otMultiplier = OTMultiplier ?? 0m;
        if (otHours <= 0m || otMultiplier <= 0m)
            return 0m;

        decimal otHourlyRate;
        if (string.Equals(EmploymentType, "PART_TIME", StringComparison.OrdinalIgnoreCase))
        {
            otHourlyRate = HourlyRate ?? 0m;
        }
        else
        {
            if (StandardWorkingDay <= 0)
                throw new InvalidOperationException("Standard working days must be greater than zero.");

            otHourlyRate = BaseSalary / (StandardWorkingDay * 8m);
        }

        return Math.Round(otHourlyRate * otHours * otMultiplier, 2, MidpointRounding.AwayFromZero);
    }

    public decimal CalculateGrossSalary()
    {
        return Math.Round(
            CalculateRegularSalary() + TotalAllowance + OTPay,
            2,
            MidpointRounding.AwayFromZero);
    }

    public decimal CalculateNetSalary()
    {
        return Math.Round(
            CalculateGrossSalary() - TotalPayCut,
            2,
            MidpointRounding.AwayFromZero);
    }

    public void UpdateTotals()
    {
        decimal allowancesTotal = Allowances
            .Where(a => string.Equals(a.Status, "active", StringComparison.OrdinalIgnoreCase))
            .Sum(a => a.CalculateTotal());

        TotalAllowance = Math.Round(
            allowancesTotal + TotalProjectAllowance,
            2,
            MidpointRounding.AwayFromZero);

        TotalPayCut = Math.Round(
            PayCuts
                .Where(pc => string.Equals(pc.Status, "active", StringComparison.OrdinalIgnoreCase))
                .Sum(pc => pc.CalculateTotal()),
            2,
            MidpointRounding.AwayFromZero);

        OTPay = CalculateOTPay();
        GrossSalary = CalculateGrossSalary();
        NetSalary = CalculateNetSalary();
    }

    public void Approve()
    {
        if (!string.Equals(Status, "calculated", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only calculated payroll can be approved.");

        Status = "approved";
        ApprovedAt = DateTime.UtcNow;
    }

    public void MarkAsPaid()
    {
        if (!string.Equals(Status, "approved", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only approved payroll can be marked as paid.");

        Status = "paid";
        PaidAt = DateTime.UtcNow;
    }
}
