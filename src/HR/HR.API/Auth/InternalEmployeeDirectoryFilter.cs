using System.Security.Cryptography;
using System.Text;
using HR.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HR.API.Auth;

public sealed class InternalEmployeeDirectoryFilter(
    IConfiguration configuration,
    IWebHostEnvironment environment,
    HrRoleFilter developmentRoleFilter,
    ILogger<InternalEmployeeDirectoryFilter> logger) : IAsyncAuthorizationFilter
{
    public const string ApiKeyHeader = "X-HR-Directory-Key";

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var expectedKey = configuration["InternalServices:EmployeeDirectoryApiKey"];
        if (string.IsNullOrWhiteSpace(expectedKey))
        {
            if (environment.IsDevelopment())
            {
                await developmentRoleFilter.OnAuthorizationAsync(context);
                return;
            }
            logger.LogError("Chưa cấu hình InternalServices:EmployeeDirectoryApiKey cho API nội bộ.");
            context.Result = new ObjectResult(new ApiError("InternalDirectoryUnavailable", "API nhân viên nội bộ chưa được cấu hình."))
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeader, out var suppliedKey)
            || string.IsNullOrWhiteSpace(suppliedKey))
        {
            context.Result = new UnauthorizedObjectResult(new ApiError("Unauthorized", "Thiếu thông tin xác thực service."));
            return;
        }
        if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expectedKey), Encoding.UTF8.GetBytes(suppliedKey.ToString())))
        {
            context.Result = new ObjectResult(new ApiError("Forbidden", "Service không được phép truy cập danh mục nhân viên."))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
