namespace Attendance.DTOs.AttendanceRecord;

public sealed record HrEmployeeDirectoryDto(
    Guid EmployeeId,
    string EmployeeCode,
    string FullName,
    string Status,
    Guid? DepartmentId,
    string? DepartmentName);