using HR.Domain.Constants;

namespace HR.Domain.Entities;

public sealed class PartTimeEmployee : Employee
{
    public PartTimeEmployee() : base(EmploymentTypes.PartTime) { }

    // Hourly pay rate in VND/hour. The property name is retained for the existing database column.
    public decimal PartTimeSalary { get; set; }
}
