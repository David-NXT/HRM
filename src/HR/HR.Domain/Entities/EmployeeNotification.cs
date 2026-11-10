namespace HR.Domain.Entities;

public sealed class EmployeeNotification
{
    public Guid EmployeeNotificationId { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public Guid? ProfileUpdateRequestId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }

    public Employee? Employee { get; set; }
    public ProfileUpdateRequest? ProfileUpdateRequest { get; set; }
}
