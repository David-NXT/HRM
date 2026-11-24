using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models;

public class TimesheetSummary
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(Period))]
    public Guid PeriodId { get; set; }

    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Số ngày công chuẩn của tháng
    /// </summary>
    public decimal StandardWorkingDays { get; set; }

    /// <summary>
    /// Ngày công làm việc thực tế
    /// </summary>
    public decimal ActualWorkingDays { get; set; }

    /// <summary>
    /// Tổng số giờ làm việc thực tế
    /// </summary>
    public decimal TotalWorkingHours { get; set; }

    /// <summary>
    /// Số ngày nghỉ phép hưởng lương
    /// </summary>
    public decimal PaidLeaveDays { get; set; }

    /// <summary>
    /// Số ngày nghỉ không lương
    /// </summary>
    public decimal UnpaidLeaveDays { get; set; }

    /// <summary>
    /// Tổng số phút đi trễ
    /// </summary>
    public int TotalLateMinutes { get; set; }

    /// <summary>
    /// Số lần đi trễ
    /// </summary>
    public int LateOccurrences { get; set; }

    /// <summary>
    /// Tổng số phút về sớm
    /// </summary>
    public int TotalEarlyLeaveMinutes { get; set; }

    /// <summary>
    /// Số lần về sớm
    /// </summary>
    public int EarlyLeaveOccurrences { get; set; }
    
    public decimal OTHours { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public TimesheetPeriod? Period { get; set; }

    public decimal CalculateWorkingDays(IEnumerable<AttendanceRecord> records) =>
        ActualWorkingDays = records.Count(record => record.WorkHour > 0);

    public decimal CalculateWorkingHours(IEnumerable<AttendanceRecord> records) =>
        TotalWorkingHours = records.Sum(record => record.WorkHour);

    public decimal CalculateOTHours(IEnumerable<OTRequest> requests) =>
        OTHours = requests.Sum(request => request.ActualHours ?? request.PlannedHours);
}
