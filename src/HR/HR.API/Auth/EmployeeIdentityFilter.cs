using HR.Application.Interfaces.Repositories;
using HR.Domain.Entities;
using HR.Domain.Constants;
using HR.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HR.API.Auth;

public sealed class EmployeeIdentityFilter : IAsyncAuthorizationFilter
{
    public const string EmployeeIdItem = "AuthenticatedEmployeeId";
    public const string EmployeeGuidHeader = "X-Employee-Guid";
    private readonly IEmployeeRepository _employees;

    public EmployeeIdentityFilter(IEmployeeRepository employees) => _employees = employees;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(EmployeeGuidHeader, out var value)
            || !Guid.TryParse(value.ToString(), out var employeeId))
        {
            context.Result = new UnauthorizedObjectResult(new ApiError("Unauthorized", "Thiếu định danh nhân viên."));
            return;
        }

        var employee = await _employees.GetByIdAsync(employeeId, context.HttpContext.RequestAborted);
        if (employee is null || employee.Status != EmployeeStatuses.Active)
        {
            context.Result = new NotFoundObjectResult(new ApiError("EmployeeNotFound", "Không tìm thấy nhân viên đang hoạt động."));
            return;
        }
        context.HttpContext.Items[EmployeeIdItem] = employee.EmployeeId;
    }
}
