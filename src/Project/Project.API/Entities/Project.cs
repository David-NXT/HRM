namespace Project.API.Entities;

public class Project
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "InProgress";
    public decimal Budget { get; set; }

    public ICollection<Task> Tasks { get; set; } = new List<Task>();
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();

    // 1. Cập nhật thông tin dự án
    public void UpdateInfo(string name, string? description, DateTime? endDate, decimal budget)
    {
        ProjectName = name;
        Description = description;
        EndDate = endDate;
        Budget = budget;
    }

    // 2. Thêm thành viên
    public ProjectMember AddMember(Guid employeeId, Guid roleId, DateTime joinDate)
    {
        var member = new ProjectMember
        {
            ProjectMemberId = Guid.NewGuid(),
            ProjectId = ProjectId,
            EmployeeId = employeeId,
            ProjectRoleId = roleId,
            JoinDate = joinDate,
            Status = "Active"
        };

        Members.Add(member);
        return member;
    }

    // 3. Xóa thành viên
    public void RemoveMember(Guid memberId)
    {
        var member = Members.FirstOrDefault(x => x.ProjectMemberId == memberId);

        if (member != null)
            Members.Remove(member);
    }

    // 4. Thêm công việc
    public Task AddTask(string name, DateTime startDate, DateTime? dueDate)
    {
        var task = new Task
        {
            TaskId = Guid.NewGuid(),
            TaskName = name,
            StartDate = startDate,
            DueDate = dueDate,
            ProjectId = ProjectId,
            Status = "New",
            Priority = "Medium"
        };

        Tasks.Add(task);
        return task;
    }

    // 5. Kiểm tra nhân viên có đang tham gia dự án
    public bool HasActiveMember(Guid employeeId)
    {
        return Members.Any(x =>
            x.EmployeeId == employeeId && x.IsActive());
    }

    // 6. Thay đổi trạng thái
    public void ChangeStatus(string status)
    {
        Status = status;
    }

    // 7. Kiểm tra có thể đóng dự án không
    public bool CanClose()
    {
        return Tasks.All(x =>
            x.Status == "Completed" || x.Status == "Inactive");
    }

    // 8. Đóng dự án
    public void Close()
    {
        if (!CanClose())
            throw new InvalidOperationException("Dự án chưa thể đóng.");

        Status = "Completed";
        EndDate = DateTime.Now;
    }

    // 9. Tính chi phí nhân công
    public decimal GetLaborCost(DateTime from, DateTime to)
    {
        decimal total = 0;

        foreach (var member in Members)
        {
            foreach (var timesheet in member.Timesheets)
            {
                if (timesheet.WorkDate >= from &&
                    timesheet.WorkDate <= to &&
                    timesheet.IsApproved())
                {
                    var rate = member.GetRateAt(timesheet.WorkDate);

                    if (rate != null)
                        total += timesheet.CalculateCost(rate);
                }
            }
        }

        return total;
    }

    // 10. Tính chênh lệch ngân sách
    public decimal GetBudgetVariance(DateTime from, DateTime to)
    {
        return Budget - GetLaborCost(from, to);
    }
}