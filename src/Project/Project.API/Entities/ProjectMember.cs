namespace Project.API.Entities;

public class ProjectMember
{
    public Guid ProjectMemberId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ProjectRoleId { get; set; }

    public DateOnly JoinDate { get; set; }
    public DateOnly? LeaveDate { get; set; }

    public string Status { get; set; } = "Active";

    public Project Project { get; set; } = null!;
    public ProjectRole ProjectRole { get; set; } = null!;

    public ICollection<HourlyRate> HourlyRates { get; set; } = new List<HourlyRate>();
    public ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();

    // 1. Kiểm tra thành viên đang hoạt động
    public bool IsActive()
    {
        return Status == "Active" && LeaveDate == null;
    }

    // 2. Ghi nhận thành viên rời dự án
    public void Leave(DateOnly leaveDate)
    {
        LeaveDate = leaveDate;
        Status = "Inactive";
    }

    // 3. Điều chỉnh thời gian tham gia
    public void AdjustPeriod(DateOnly joinDate, DateOnly leaveDate)
    {
        JoinDate = joinDate;
        LeaveDate = leaveDate;
    }

    // 4. Thiết lập đơn giá giờ làm mới
    public HourlyRate SetHourlyRate(decimal rate, DateOnly effectiveFrom)
    {
        var hourlyRate = new HourlyRate
        {
            HourlyRateId = Guid.NewGuid(),
            ProjectMemberId = ProjectMemberId,
            Rate = rate,
            EffectiveFrom = effectiveFrom
        };

        HourlyRates.Add(hourlyRate);
        return hourlyRate;
    }

    // 5. Lấy đơn giá có hiệu lực tại ngày chỉ định
    public HourlyRate? GetRateAt(DateOnly date)
    {
        return HourlyRates.FirstOrDefault(x => x.IsEffectiveAt(date));
    }

    // 6. Kiểm tra có được khai báo Timesheet không
    public bool CanLogTimesheet(DateOnly workDate)
    {
        return IsActive()
            && workDate >= JoinDate
            && (LeaveDate == null || workDate <= LeaveDate.Value);
    }

    // 7. Khai báo giờ làm
    public Timesheet LogTimesheet(
        Guid taskId,
        DateOnly workDate,
        decimal hours,
        string? description)
    {
        var timesheet = new Timesheet
        {
            TimesheetId = Guid.NewGuid(),
            ProjectMemberId = ProjectMemberId,
            TaskId = taskId,
            WorkDate = workDate,
            HoursWorked = hours,
            Description = description,
            Status = "SUBMITTED"
        };

        Timesheets.Add(timesheet);
        return timesheet;
    }

    // 8. Tính tổng số giờ đã duyệt
    public decimal GetApprovedHours(DateOnly from, DateOnly to)
    {
        return Timesheets
            .Where(x => x.WorkDate >= from
                     && x.WorkDate <= to
                     && x.IsApproved())
            .Sum(x => x.HoursWorked);
    }
}