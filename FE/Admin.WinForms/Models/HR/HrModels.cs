namespace Admin.WinForms.Models.HR;

public sealed record EmployeeRow(Guid EmployeeId, string EmployeeCode, string FullName,
    DateOnly DateOfBirth, string Gender, string Email, string Phone, string CitizenId,
    string? Address, string Status, DateTime CreatedAt, DateTime UpdatedAt,
    Guid? DepartmentId, Guid? PositionId, string? DepartmentName, string? PositionName,
    string EmploymentType, decimal? BaseSalary, decimal? PartTimeSalary)
{
    public string EmploymentTypeLabel => EmploymentType == "PART_TIME" ? "Part-time" : "Full-time";
    public string SalaryLabel => EmploymentType == "PART_TIME"
        ? $"{PartTimeSalary.GetValueOrDefault():N2} ₫ (part-time)"
        : $"{BaseSalary.GetValueOrDefault():N2} ₫ (full-time)";
    public string DisplayName => $"{EmployeeCode} — {FullName}";
}
public sealed record EmployeePage(List<EmployeeRow> Items, int Page, int PageSize, int Total);
public sealed record PersonalInfo(string FullName, DateOnly DateOfBirth, string Gender,
    string Email, string Phone, string CitizenId, string? Address);
public sealed record CreateEmployee(string EmployeeCode, string FullName, DateOnly DateOfBirth,
    string Gender, string Email, string Phone, string CitizenId, string? Address, string EmploymentType,
    decimal? BaseSalary, decimal? PartTimeSalary);
public sealed record SalaryInput(decimal? BaseSalary, decimal? PartTimeSalary);
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
    string Status, DateTime CreatedAt, DateTime UpdatedAt)
{
    public string DisplayName => $"{Code} — {Name} ({Status})";
}
public sealed record DepartmentInput(string Code, string Name, string? Description, Guid? ParentDepartmentId);
public sealed record PositionInput(string Code, string Name, string? Description);
public sealed record ContractRow(Guid ContractId, Guid EmployeeId, string ContractNumber,
    string ContractType, DateOnly StartDate, DateOnly? EndDate, string Status, string? Notes,
    Guid? RenewalOfContractId, DateTime CreatedAt, DateTime UpdatedAt);
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
    DateTime? ReviewedAt, DateTime CreatedAt, DateTime UpdatedAt, List<AttachmentRow> Attachments);
public sealed record AlertRow(Guid ContractAlertId, Guid ContractId, Guid EmployeeId,
    string EmployeeCode, string EmployeeName, string EmployeeStatus, string ContractNumber,
    string ContractType, string ContractStatus, DateOnly ContractStartDate,
    DateOnly? CurrentContractEndDate, DateOnly ExpiryDate, int DaysRemaining, bool IsCurrentCycle,
    string Status, int Version, DateTime CreatedAt, DateTime UpdatedAt);
public sealed record AlertPage(List<AlertRow> Items, int Page, int PageSize, int Total);
public sealed record Choice(object? Value, string Text)
{
    public override string ToString() => Text;
}
