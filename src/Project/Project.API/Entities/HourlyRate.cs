namespace Project.API.Entities;

public class HourlyRate
{
    public Guid HourlyRateId { get; set; }
    public Guid ProjectMemberId { get; set; }
    public decimal Rate { get; set; }

    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }

    public ProjectMember ProjectMember { get; set; } = null!;

    // 1. Kiểm tra đơn giá có hiệu lực tại ngày chỉ định
    public bool IsEffectiveAt(DateOnly date)
    {
        return date >= EffectiveFrom
            && (EffectiveTo == null || date <= EffectiveTo.Value);
    }

    // 2. Đóng đơn giá
    public void Close(DateOnly effectiveTo)
    {
        EffectiveTo = effectiveTo;
    }

    // 3. Kiểm tra khoảng hiệu lực có bị chồng lấn không
    public bool OverlapsWith(HourlyRate other)
    {
        var thisEnd = EffectiveTo ?? DateOnly.MaxValue;
        var otherEnd = other.EffectiveTo ?? DateOnly.MaxValue;

        return EffectiveFrom <= otherEnd
            && other.EffectiveFrom <= thisEnd;
    }
}