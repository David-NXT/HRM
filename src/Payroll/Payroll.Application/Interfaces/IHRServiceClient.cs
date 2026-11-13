using Payroll.Application.DTOs.HR;

namespace Payroll.Application.Interfaces;

public interface IHRServiceClient
{
    Task<HREmployeeResponse?> GetEmployeeAsync(
        Guid employeeID,
        CancellationToken cancellationToken = default);
}