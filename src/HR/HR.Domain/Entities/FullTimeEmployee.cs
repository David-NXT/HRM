using HR.Domain.Constants;

namespace HR.Domain.Entities;

public sealed class FullTimeEmployee : Employee
{
    public FullTimeEmployee() : base(EmploymentTypes.FullTime) { }

    // Current contractual base salary; payroll periods keep their own salary snapshot.
    public decimal BaseSalary { get; set; }
}
