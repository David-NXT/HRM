namespace Project.API.Entities;

public class HourlyRate
{
    public Guid HourlyRateId { get; set; }
    public Guid ProjectMemberId { get; set; }
    public decimal Rate { get; set; }

    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public ProjectMember ProjectMember { get; set; } = null!;

    // 1. Kiểm tra đơn giá có hiệu lực tại ngày chỉ định
    public bool IsEffectiveAt(DateTime date)
    {
        return date >= EffectiveFrom
            && (EffectiveTo == null || date <= EffectiveTo.Value);
    }

    // 2. Đóng đơn giá
    public void Close(DateTime effectiveTo)
    {
        EffectiveTo = effectiveTo;
    }

    // 3. Kiểm tra khoảng hiệu lực có bị chồng lấn không
    public bool OverlapsWith(HourlyRate other)
    {
        var thisEnd = EffectiveTo ?? DateTime.MaxValue;
        var otherEnd = other.EffectiveTo ?? DateTime.MaxValue;

        return EffectiveFrom <= otherEnd
            && other.EffectiveFrom <= thisEnd;
    }
}