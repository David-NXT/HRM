using HR.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HR.API.Auth;

public sealed class HrRoleFilter : IAsyncAuthorizationFilter
{
    public const string RoleHeader = "X-Role";

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(RoleHeader, out var role)
            || string.IsNullOrWhiteSpace(role))
        {
            context.Result = new UnauthorizedObjectResult(new ApiError("Unauthorized", "Thiếu thông tin đăng nhập."));
            return Task.CompletedTask;
        }

        if (!string.Equals(role.ToString(), "HR", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new ObjectResult(new ApiError("Forbidden", "Không đủ quyền truy cập."))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        return Task.CompletedTask;
    }
}
