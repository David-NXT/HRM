using HR.Domain.Constants;

namespace HR.Domain.Entities;

public sealed class FullTimeEmployee : Employee
{
    public FullTimeEmployee() : base(EmploymentTypes.FullTime) { }
}
