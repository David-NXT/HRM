using System;
using System.Collections.Generic;

namespace Attendance.WinForms.DTOs
{
    public class EmployeeDirectoryDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string? PositionName { get; set; }

        public string DisplayInfo => $"{EmployeeCode} - {FullName} ({DepartmentName ?? "Chưa phân phòng"})";
    }

    public class EmployeeDirectoryPageDto
    {
        public List<EmployeeDirectoryDto> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }

    public class WorkShiftDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int BreakMins { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateOnly? EffectiveDate { get; set; }
    }

    public class AttendanceRecordDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public Guid ShiftId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly? TimeIn { get; set; }
        public TimeOnly? TimeOut { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal WorkHour { get; set; }
        public int LateMins { get; set; }
        public int EarlyLeaveMins { get; set; }
        public int OTMins { get; set; }
        public int ActualOTMins { get; set; }
        public bool IsLate { get; set; }
        public bool IsEarlyLeave { get; set; }
        public string? CheckInSource { get; set; }
        public string? CheckOutSource { get; set; }
        public double? CheckInLatitude { get; set; }
        public double? CheckInLongitude { get; set; }
        public string? CheckInLocation { get; set; }
        public double? CheckOutLatitude { get; set; }
        public double? CheckOutLongitude { get; set; }
        public string? CheckOutLocation { get; set; }
        public string? DeviceId { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateAttendanceRecordDto
    {
        public Guid EmployeeId { get; set; }
        public Guid ShiftId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly? TimeIn { get; set; }
        public TimeOnly? TimeOut { get; set; }
        public string Status { get; set; } = "Present";
        public decimal WorkHour { get; set; }
        public int LateMins { get; set; }
        public int EarlyLeaveMins { get; set; }
        public int OTMins { get; set; }
        public int ActualOTMins { get; set; }
        public bool IsLate { get; set; }
        public bool IsEarlyLeave { get; set; }
        public string? CheckInSource { get; set; } = "Kiosk/WebTerminal";
        public string? CheckOutSource { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateAttendanceRecordDto
    {
        public Guid? EmployeeId { get; set; }
        public Guid? ShiftId { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly? TimeIn { get; set; }
        public TimeOnly? TimeOut { get; set; }
        public string? Status { get; set; }
        public decimal? WorkHour { get; set; }
        public int? LateMins { get; set; }
        public int? EarlyLeaveMins { get; set; }
        public int? OTMins { get; set; }
        public int? ActualOTMins { get; set; }
        public bool? IsLate { get; set; }
        public bool? IsEarlyLeave { get; set; }
        public string? CheckOutSource { get; set; } = "Kiosk/WebTerminal";
        public string? Note { get; set; }
    }

    public class LeaveRequestDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal TotalDays { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateLeaveRequestDto
    {
        public Guid EmployeeId { get; set; }
        public string LeaveType { get; set; } = "Phép năm";
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal TotalDays { get; set; } = 1.0m;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? ApprovedBy { get; set; }
    }

    public class OTRequestDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal PlannedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public string? DayType { get; set; }
        public decimal Multiplier { get; set; }
        public bool IsUnplanned { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateOTRequestDto
    {
        public Guid EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal PlannedHours { get; set; }
        public string? DayType { get; set; } = "WEEKDAY";
        public decimal Multiplier { get; set; } = 1.5m;
        public bool IsUnplanned { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "Pending";
        public string? ApprovedBy { get; set; }
    }

    public class AttendanceExplanationDto
    {
        public Guid Id { get; set; }
        public Guid? AttendanceId { get; set; }
        public Guid EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public string ExplanationType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public TimeOnly? RequestedTimeIn { get; set; }
        public TimeOnly? RequestedTimeOut { get; set; }
        public string? AttachmentUrl { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateAttendanceExplanationDto
    {
        public Guid? AttendanceId { get; set; }
        public Guid EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public string ExplanationType { get; set; } = "Quên quẹt thẻ Check-in";
        public string Reason { get; set; } = string.Empty;
        public TimeOnly? RequestedTimeIn { get; set; }
        public TimeOnly? RequestedTimeOut { get; set; }
        public string Status { get; set; } = "Pending";
        public string? ApprovedBy { get; set; }
    }
}
