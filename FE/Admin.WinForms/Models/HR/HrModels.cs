using System.Text.Json.Serialization;

namespace Admin.WinForms.Models.HR;

public sealed record EmployeeRow(Guid EmployeeId, string EmployeeCode, string FullName,
    DateOnly DateOfBirth, string Gender, string Email, string Phone, string CitizenId,
    string? Address, string Status, DateTime CreatedAt, DateTime UpdatedAt,
    Guid? DepartmentId, Guid? PositionId, string? DepartmentName, string? PositionName,
    string EmploymentType, decimal? BaseSalary,
    [property: JsonPropertyName("partTimeSalary")] decimal? PartTimeHourlyRate, int Version = 1)
{
    public string EmploymentTypeLabel => EmploymentType == "PART_TIME" ? "Part-time" : "Full-time";
    public string SalaryLabel => EmploymentType == "PART_TIME"
        ? $"{PartTimeHourlyRate.GetValueOrDefault():N2} VND/giờ (part-time)"
        : $"{BaseSalary.GetValueOrDefault():N2} VND/tháng (full-time)";
    public string DisplayName => $"{EmployeeCode} — {FullName}";
}
public sealed record EmployeePage(List<EmployeeRow> Items, int Page, int PageSize, int Total);
public sealed record EmployeeStatistics(int TotalEmployees, int ActiveEmployees, int LockedEmployees,
    int ResignedEmployees, int FullTimeEmployees, int PartTimeEmployees);
public sealed record PersonalInfo(string FullName, DateOnly DateOfBirth, string Gender,
    string Email, string Phone, string CitizenId, string? Address);
public sealed record CreateEmployee(string EmployeeCode, string FullName, DateOnly DateOfBirth,
    string Gender, string Email, string Phone, string CitizenId, string? Address, string EmploymentType,
    decimal? BaseSalary, [property: JsonPropertyName("partTimeSalary")] decimal? PartTimeHourlyRate);
public sealed record SalaryInput(decimal? BaseSalary,
    [property: JsonPropertyName("partTimeSalary")] decimal? PartTimeHourlyRate);
public sealed record OnboardEmployee(CreateEmployee Employee, DateOnly StartDate,
    Guid DepartmentId, Guid PositionId, string? Notes);
public sealed record OnboardingResult(Guid EmployeeId, string EmployeeCode, string Status,
    Guid WorkHistoryId, DateOnly StartDate, Guid DepartmentId, Guid PositionId, string EmploymentType);
public sealed record DepartmentRow(Guid DepartmentId, string Code, string Name, string? Description,
    Guid? ParentDepartmentId, string Status, DateTime CreatedAt, DateTime UpdatedAt)
{
    public string DisplayName => $"{Code} — {Name} ({Status})";
}
public sealed record PositionRow(Guid PositionId, string Code, string Name, string? Description,
    string Status, DateTime CreatedAt, DateTime UpdatedAt, string? SeniorityLevel = null)
{
    public string DisplayName => $"{Code} — {Name} ({Status})";
    public string SeniorityLevelLabel => HrChoiceOptions.SeniorityLevelLabel(SeniorityLevel);
}
public sealed record DepartmentInput(string Code, string Name, string? Description, Guid? ParentDepartmentId);
public sealed record PositionInput(string Code, string Name, string? Description, string? SeniorityLevel = null);
public sealed record ContractRow(Guid ContractId, Guid EmployeeId, string ContractNumber,
    string ContractType, DateOnly StartDate, DateOnly? EndDate, string Status, string? Notes,
    Guid? RenewalOfContractId, DateTime CreatedAt, DateTime UpdatedAt,
    DateOnly? ActualEndDate = null, string? TerminationReason = null);
public sealed record ContractInput(string ContractNumber, string ContractType, DateOnly StartDate,
    DateOnly? EndDate, string? Notes, Guid? RenewalOfContractId = null);
public sealed record AssignmentRow(Guid WorkHistoryId, Guid EmployeeId, Guid DepartmentId,
    string DepartmentCode, string DepartmentName, Guid PositionId, string PositionCode,
    string PositionName, DateOnly StartDate, DateOnly? EndDate, string? Notes);
public sealed record AssignmentInput(Guid DepartmentId, Guid PositionId, DateOnly EffectiveDate, string? Notes);
public sealed record ChecklistInput(string Title, bool IsRequired);
public sealed record OffboardingInput(Guid EmployeeId, DateOnly LastWorkingDate, string? Notes,
    List<ChecklistInput> Checklist);
public sealed record ChecklistRow(Guid OffboardingChecklistItemId, string Title, bool IsRequired,
    bool IsCompleted, string? CompletionNote, DateTime? CompletedAt);
public sealed record OffboardingRow(Guid OffboardingRecordId, Guid EmployeeId, string EmployeeCode,
    string EmployeeName, string EmployeeStatus, DateOnly LastWorkingDate, string Status,
    string? Notes, int Version, DateTime CreatedAt, DateTime UpdatedAt, DateTime? CompletedAt,
    List<ChecklistRow> Checklist)
{
    public string DisplayName => $"{EmployeeCode} — {EmployeeName} — {LastWorkingDate:dd/MM/yyyy} ({Status})";
}
public sealed record AttachmentRow(Guid ProfileUpdateAttachmentId, string OriginalFileName,
    string ContentType, long Length, DateTime UploadedAt);
public sealed record ProfileRequestRow(Guid ProfileUpdateRequestId, Guid EmployeeId,
    PersonalInfo Changes, string Status, string? ReviewReason, DateTime? SubmittedAt,
    DateTime? ReviewedAt, DateTime CreatedAt, DateTime UpdatedAt, List<AttachmentRow> Attachments,
    int Version = 1, int? BaseEmployeeVersion = null);
public sealed record AlertRow(Guid ContractAlertId, Guid ContractId, Guid EmployeeId,
    string EmployeeCode, string EmployeeName, string EmployeeStatus, string ContractNumber,
    string ContractType, string ContractStatus, DateOnly ContractStartDate,
    DateOnly? CurrentContractEndDate, DateOnly ExpiryDate, int DaysRemaining, bool IsCurrentCycle,
    string Status, int Version, DateTime CreatedAt, DateTime UpdatedAt);
public sealed record AlertPage(List<AlertRow> Items, int Page, int PageSize, int Total);
public sealed record AlertScanResult(DateOnly ScanDate, int ScannedCount, int CreatedCount, int SkippedCount, int FailedCount);
public sealed record Choice(object? Value, string Text)
{
    public override string ToString() => Text;
}

public static class HrChoiceOptions
{
    public const string DefaultStaffPositionCode = "SYS_STAFF";

    public static IReadOnlyList<Choice> PositionChoices(
        IEnumerable<PositionRow> positions,
        string? employmentType,
        bool defaultStaffOnlyForPartTime = false)
    {
        var activePositions = positions.Where(position => position.Status == "ACTIVE");
        var isPartTime = employmentType == "PART_TIME";
        if (isPartTime)
        {
            activePositions = activePositions.Where(position => position.SeniorityLevel == "STAFF");
            if (defaultStaffOnlyForPartTime)
            {
                activePositions = activePositions.Where(position => position.Code == DefaultStaffPositionCode);
            }
        }

        var options = activePositions
            .OrderByDescending(position => isPartTime && position.Code == DefaultStaffPositionCode)
            .ThenBy(position => position.Code)
            .Select(position => new Choice(position.PositionId,
                position.Code == DefaultStaffPositionCode ? position.Name : position.DisplayName))
            .ToArray();

        if (isPartTime && defaultStaffOnlyForPartTime && options.Length > 0)
        {
            return options;
        }

        var prompt = options.Length == 0
            ? isPartTime && defaultStaffOnlyForPartTime
                ? "Thiếu chức vụ Nhân viên mặc định; hãy cập nhật database HR"
                : isPartTime
                    ? "Chưa có chức vụ STAFF đã phân loại"
                : "Chưa có chức vụ đang hoạt động"
            : "Chọn chức vụ *";
        return new[] { new Choice(null, prompt) }.Concat(options).ToArray();
    }

    public static string SeniorityLevelLabel(string? value) => value switch
    {
        "STAFF" => "Nhân viên / Chuyên viên",
        "MANAGER" => "Quản lý",
        "DIRECTOR" => "Giám đốc",
        "EXECUTIVE" => "Ban điều hành / cấp cao hơn",
        _ => "Chưa phân loại"
    };
}
