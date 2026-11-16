namespace HR.Application.DTOs.Employees;

public sealed record EmployeeExportFile(byte[] Content, string FileName)
{
    public const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
