using HR.Core.Exceptions;

namespace HR.Infrastructure.Middleware;

public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (HrException exception)
        {
            if (exception.StatusCode >= 500)
            {
                _logger.LogError(exception, "Lỗi nghiệp vụ khi xử lý {Method} {Path}", context.Request.Method, context.Request.Path);
            }

            await WriteErrorAsync(context, exception.StatusCode, exception.Code, exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Lỗi không dự kiến khi xử lý {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "InternalError", "Không thể hoàn tất thao tác. Vui lòng thử lại.");
        }
    }

    private static Task WriteErrorAsync(HttpContext context, int statusCode, string code, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(new ApiError(code, message));
    }
}
