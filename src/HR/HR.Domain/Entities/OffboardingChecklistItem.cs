namespace HR.Domain.Entities;

public sealed class OffboardingChecklistItem
{
    public Guid OffboardingChecklistItemId { get; set; } = Guid.NewGuid();
    public Guid OffboardingRecordId { get; set; }
    public int SortOrder { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsCompleted { get; set; }
    public string? CompletionNote { get; set; }
    public DateTime? CompletedAt { get; set; }
    public OffboardingRecord? OffboardingRecord { get; set; }
}
