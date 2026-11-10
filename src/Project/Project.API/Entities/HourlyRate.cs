namespace Project.API.Entities;
public class HourlyRate
{
    public Guid HourlyRateId { get; set; }

    public Guid ProjectMemberId { get; set; }

    public decimal Rate { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public ProjectMember ProjectMember { get; set; } = null!;
}