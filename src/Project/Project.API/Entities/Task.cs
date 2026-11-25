namespace Project.API.Entities;

public class Task
{
    public Guid TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string? Description { get; set; }
public DateTime StartDate { get; set; }
public DateTime? DueDate { get; set; }
    public string Status { get; set; } = "Todo";
    public string Priority { get; set; } = "Medium";

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();

    // 1. Đổi tên công việc
    public void Rename(string name)
    {
        TaskName = name;
    }

    // 2. Thay đổi trạng thái
    public void ChangeStatus(string status)
    {
        Status = status;
    }

    // 3. Thay đổi độ ưu tiên
    public void ChangePriority(string priority)
    {
        Priority = priority;
    }

    // 4. Ngừng sử dụng công việc
    public void Deactivate()
    {
        Status = "Inactive";
    }

    // 5. Kiểm tra đã có Timesheet chưa
    public bool HasTimesheet()
    {
        return Timesheets.Any();
    }

    // 6. Tính tổng số giờ đã duyệt
    public decimal GetTotalApprovedHours()
    {
        return Timesheets
            .Where(x => x.IsApproved())
            .Sum(x => x.HoursWorked);
    }
}