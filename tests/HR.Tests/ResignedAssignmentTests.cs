using HR.Application.DTOs.Contracts;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Application.Services.Contracts;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
using Xunit;

namespace HR.Tests;

public sealed class ResignedAssignmentTests
{
    [Fact]
    public async Task ResignedEmployeeCannotOpenNewWorkHistoryAfterOffboarding()
    {
        var service = new ContractService(new ResignedEmployeeRepository(), new UnusedAlertGenerator());
        var error = await Assert.ThrowsAsync<HrException>(() => service.RecordAssignmentAsync(
            Id(1), new CreateWorkAssignmentRequest(Id(1), Id(1), new(2026, 11, 10), null), CancellationToken.None));

        Assert.Equal(409, error.StatusCode);
        Assert.Equal("EmployeeResigned", error.Code);
    }

    private sealed class UnusedAlertGenerator : IContractAlertGenerator
    {
        public Task GenerateForContractAsync(EmploymentContract contract, CancellationToken token) =>
            throw new InvalidOperationException("Recording work history must not generate a contract alert.");
    }

    private sealed class ResignedEmployeeRepository : IEmploymentRepository
    {
        public Task<Employee?> GetEmployeeAsync(Guid id, CancellationToken ct) =>
            Task.FromResult<Employee?>(new FullTimeEmployee { EmployeeId = id, Status = EmployeeStatuses.Resigned });
        public Task<EmploymentContract?> GetContractAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<IReadOnlyList<EmploymentContract>> GetContractsAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<IReadOnlyList<WorkHistory>> GetWorkHistoryAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<WorkHistory?> GetOpenWorkHistoryAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<Department?> GetActiveDepartmentAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<Position?> GetActivePositionAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> ContractNumberExistsAsync(string number, Guid? id, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> RenewalAlreadyExistsAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task AddContractAsync(EmploymentContract contract, CancellationToken ct) => throw new NotSupportedException();
        public Task AddWorkHistoryAsync(WorkHistory history, CancellationToken ct) => throw new NotSupportedException();
        public Task SaveChangesAsync(CancellationToken ct) => throw new NotSupportedException();
        public Task SaveChangesInTransactionAsync(CancellationToken ct) => throw new NotSupportedException();
    }
}
