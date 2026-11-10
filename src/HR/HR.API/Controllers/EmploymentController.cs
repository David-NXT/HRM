using HR.Application.DTOs.Contracts;
using HR.Application.Services.Contracts;
using HR.API.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Route("")]
[ServiceFilter(typeof(HrRoleFilter))]
public sealed class EmploymentController : ControllerBase
{
    private readonly ContractService _service;

    public EmploymentController(ContractService service) => _service = service;

    [HttpGet("employees/{employeeId:guid}/contracts", Name = "GetEmployeeContracts")]
    public async Task<ActionResult<IReadOnlyList<EmploymentContractDto>>> GetContracts(
        Guid employeeId,
        CancellationToken cancellationToken) =>
        Ok(await _service.GetContractsAsync(employeeId, cancellationToken));

    [HttpPost("employees/{employeeId:guid}/contracts")]
    public async Task<ActionResult<EmploymentContractDto>> CreateContract(
        Guid employeeId,
        CreateEmploymentContractRequest request,
        CancellationToken cancellationToken)
    {
        var contract = await _service.CreateContractAsync(employeeId, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, contract);
    }

    [HttpPut("contracts/{contractId:guid}")]
    public async Task<ActionResult<EmploymentContractDto>> UpdateContract(
        Guid contractId,
        UpdateEmploymentContractRequest request,
        CancellationToken cancellationToken) =>
        Ok(await _service.UpdateContractAsync(contractId, request, cancellationToken));

    [HttpGet("employees/{employeeId:guid}/work-history", Name = "GetEmployeeWorkHistory")]
    public async Task<ActionResult<IReadOnlyList<WorkHistoryDto>>> GetWorkHistory(
        Guid employeeId,
        CancellationToken cancellationToken) =>
        Ok(await _service.GetWorkHistoryAsync(employeeId, cancellationToken));

    [HttpPost("employees/{employeeId:guid}/work-history")]
    public async Task<ActionResult<WorkHistoryDto>> RecordAssignment(
        Guid employeeId,
        CreateWorkAssignmentRequest request,
        CancellationToken cancellationToken) =>
        StatusCode(StatusCodes.Status201Created,
            await _service.RecordAssignmentAsync(employeeId, request, cancellationToken));
}
