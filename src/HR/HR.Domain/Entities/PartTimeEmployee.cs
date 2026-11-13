using HR.Domain.Constants;

namespace HR.Domain.Entities;

public sealed class PartTimeEmployee : Employee
{
    public PartTimeEmployee() : base(EmploymentTypes.PartTime) { }

    public decimal PartTimeSalary { get; set; }
}
