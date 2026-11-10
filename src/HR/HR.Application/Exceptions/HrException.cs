namespace HR.Application.Exceptions;

public sealed class HrException : Exception
{
    public HrException(int statusCode, string code, string message) : base(message)
    {
        StatusCode = statusCode;
        Code = code;
    }

    public int StatusCode { get; }

    public string Code { get; }
}

public sealed record ApiError(string Code, string Message);
