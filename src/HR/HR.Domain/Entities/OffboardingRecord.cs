using HR.Domain.Constants;
namespace HR.Domain.Entities;

public sealed class OffboardingRecord
{
    public Guid OffboardingRecordId { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public DateOnly LastWorkingDate { get; set; }
    public string Status { get; set; } = LifecycleStatuses.InProgress;
    public string? Notes { get; set; }
    public int Version { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Employee? Employee { get; set; }
    public List<OffboardingChecklistItem> Checklist { get; set; } = [];
}
