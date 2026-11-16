namespace Project.API.Entities;
public class HourlyRate
{
    public Guid HourlyRateId { get; set; }

    public Guid ProjectMemberId { get; set; }

    public decimal Rate { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public ProjectMember ProjectMember { get; set; } = null!;
    // Kiểm tra mức lương theo giờ có hiệu lực tại ngày làm việc không
    public bool IsEffectiveAt(DateTime workDate)
    {
        return workDate.Date >= EffectiveFrom.Date &&
               (EffectiveTo == null || workDate.Date <= EffectiveTo.Value.Date);
    }
}