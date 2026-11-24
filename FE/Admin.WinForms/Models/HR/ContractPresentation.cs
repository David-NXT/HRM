namespace Admin.WinForms.Models.HR;

public enum ContractPeriodState
{
    NotStarted,
    InEffect,
    ExpiringSoon,
    Expired,
    Terminated,
    Cancelled
}

public enum ContractPeriodFilter
{
    All,
    InEffect,
    ExpiringSoon,
    Expired,
    NotStarted,
    Terminated,
    Cancelled
}

// Hiển thị riêng hạn đã ký và tình trạng chấm dứt/hủy; không sửa dữ liệu hợp đồng gốc.
public sealed record ContractDisplayRow(ContractRow Contract, DateOnly ReferenceDate)
{
    public const int WarningDays = 30;

    public string ContractNumber => Contract.ContractNumber;
    public string ContractType => Contract.ContractType;
    public DateOnly StartDate => Contract.StartDate;
    public DateOnly? EndDate => Contract.EndDate;
    public string Status => Contract.Status;
    public string StatusLabel => Status switch
    {
        "ACTIVE" => "Đang quản lý (ACTIVE)",
        "RENEWED" => "Đã gia hạn (RENEWED)",
        "TERMINATED" => "Đã chấm dứt (TERMINATED)",
        "CANCELLED" => "Đã hủy (CANCELLED)",
        _ => Status
    };
    public DateOnly? ActualEndDate => Contract.ActualEndDate;
    public string? TerminationReason => Contract.TerminationReason;
    public string? Notes => string.IsNullOrWhiteSpace(TerminationReason) ? Contract.Notes
        : string.Join(Environment.NewLine, new[] { Contract.Notes, TerminationReason }.Where(value => !string.IsNullOrWhiteSpace(value)));

    public ContractPeriodState PeriodState
    {
        get
        {
            if (Status == "TERMINATED") return ContractPeriodState.Terminated;
            if (Status == "CANCELLED") return ContractPeriodState.Cancelled;
            if (ReferenceDate < StartDate) return ContractPeriodState.NotStarted;
            if (EndDate is not DateOnly end) return ContractPeriodState.InEffect;
            var remaining = end.DayNumber - ReferenceDate.DayNumber;
            if (remaining < 0) return ContractPeriodState.Expired;
            return remaining <= WarningDays ? ContractPeriodState.ExpiringSoon : ContractPeriodState.InEffect;
        }
    }

    public bool IsEffective => PeriodState is ContractPeriodState.InEffect or ContractPeriodState.ExpiringSoon;

    public string PeriodLabel => PeriodState switch
    {
        ContractPeriodState.NotStarted => "Chưa bắt đầu",
        ContractPeriodState.InEffect => "Đang hiệu lực",
        ContractPeriodState.ExpiringSoon => "Sắp hết hạn",
        ContractPeriodState.Expired => "Đã hết hạn",
        ContractPeriodState.Terminated => "Đã chấm dứt",
        ContractPeriodState.Cancelled => "Đã hủy",
        _ => throw new InvalidOperationException("Tình trạng thời hạn không hợp lệ.")
    };

    public string TimeRemaining
    {
        get
        {
            if (PeriodState == ContractPeriodState.Cancelled) return "Hủy trước ngày bắt đầu";
            if (PeriodState == ContractPeriodState.Terminated)
                return ActualEndDate.HasValue ? $"Chấm dứt ngày {ActualEndDate:dd/MM/yyyy}" : "Đã chấm dứt";
            if (PeriodState == ContractPeriodState.NotStarted)
                return $"Bắt đầu sau {StartDate.DayNumber - ReferenceDate.DayNumber} ngày";
            if (EndDate is not DateOnly end) return "Không thời hạn";
            var remaining = end.DayNumber - ReferenceDate.DayNumber;
            return remaining switch
            {
                < 0 => $"Quá hạn {-remaining} ngày",
                0 => "Hết hạn hôm nay",
                _ => $"Còn {remaining} ngày"
            };
        }
    }

    public bool Matches(ContractPeriodFilter filter) => filter switch
    {
        ContractPeriodFilter.InEffect => IsEffective,
        ContractPeriodFilter.ExpiringSoon => PeriodState == ContractPeriodState.ExpiringSoon,
        ContractPeriodFilter.Expired => PeriodState == ContractPeriodState.Expired,
        ContractPeriodFilter.NotStarted => PeriodState == ContractPeriodState.NotStarted,
        ContractPeriodFilter.Terminated => PeriodState == ContractPeriodState.Terminated,
        ContractPeriodFilter.Cancelled => PeriodState == ContractPeriodState.Cancelled,
        _ => true
    };
}

public sealed record ContractCoverageSummary(string Text, bool HasEffectiveContract);

public static class ContractPresentation
{
    // Tính bao phủ theo ngày và loại hợp đồng đã hủy/chấm dứt; RENEWED vẫn có thể bao phủ hôm nay trước hợp đồng kế tiếp.
    public static ContractCoverageSummary Summarize(IReadOnlyList<ContractDisplayRow> contracts)
    {
        if (contracts.Count == 0) return new("Chưa có hợp đồng.", false);
        var effective = contracts.Where(contract => contract.IsEffective).ToArray();
        if (effective.Length > 0)
            return new($"Có {effective.Length} hợp đồng đang hiệu lực theo ngày: "
                + string.Join(", ", effective.Select(contract => contract.ContractNumber)) + ".", true);

        var lastEnd = contracts.Select(contract => contract.PeriodState switch
            {
                ContractPeriodState.Expired => contract.EndDate,
                ContractPeriodState.Terminated => contract.ActualEndDate,
                _ => null
            })
            .Where(date => date.HasValue).Select(date => date!.Value)
            .OrderByDescending(date => date).FirstOrDefault();
        var next = contracts.Where(contract => contract.PeriodState == ContractPeriodState.NotStarted)
            .OrderBy(contract => contract.StartDate).FirstOrDefault();
        var text = "Chưa có hợp đồng có hiệu lực hôm nay.";
        if (lastEnd != default) text += $" Hợp đồng gần nhất đã kết thúc ngày {lastEnd:dd/MM/yyyy}.";
        if (next is not null) text += $" Hợp đồng tiếp theo bắt đầu ngày {next.StartDate:dd/MM/yyyy}.";
        return new(text, false);
    }
}
