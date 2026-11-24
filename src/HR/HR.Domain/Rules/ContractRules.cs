using HR.Domain.Constants;
using HR.Domain.Entities;

namespace HR.Domain.Rules;

public static class ContractRules
{
    // Kiểm tra hai khoảng ngày bao gồm cả ngày đầu/cuối; null là không xác định ngày hết hạn.
    public static bool Overlaps(DateOnly start, DateOnly? end, DateOnly otherStart, DateOnly? otherEnd) =>
        (!end.HasValue || otherStart <= end.Value)
        && (!otherEnd.HasValue || start <= otherEnd.Value);

    // Hợp đồng đã hủy không chiếm khoảng làm việc; hợp đồng chấm dứt vẫn giữ khoảng hiệu lực lịch sử tới ActualEndDate.
    public static bool ConflictsWith(EmploymentContract existing, DateOnly start, DateOnly? end) =>
        (existing.Status is ContractStatuses.Active or ContractStatuses.Renewed or ContractStatuses.Terminated)
        && Overlaps(start, end, existing.StartDate,
            existing.Status == ContractStatuses.Terminated ? existing.ActualEndDate ?? existing.EndDate : existing.EndDate);

    // Đồng bộ hợp đồng khi nghỉ việc: chấm dứt khoảng đang bao phủ ngày nghỉ, hủy hợp đồng tương lai và giữ bản đã hết hạn.
    public static void CloseForDeparture(IEnumerable<EmploymentContract> contracts, DateOnly lastWorkingDate, DateTime now)
    {
        foreach (var contract in contracts.Where(item => item.Status is ContractStatuses.Active or ContractStatuses.Renewed))
        {
            if (contract.StartDate > lastWorkingDate)
            {
                contract.Status = ContractStatuses.Cancelled;
                contract.TerminationReason = "Hủy do nhân viên nghỉ việc trước ngày bắt đầu hợp đồng.";
            }
            else if (!contract.EndDate.HasValue || contract.EndDate.Value >= lastWorkingDate)
            {
                contract.Status = ContractStatuses.Terminated;
                contract.ActualEndDate = lastWorkingDate;
                contract.TerminationReason = "Chấm dứt theo hồ sơ xác nhận nghỉ việc.";
            }
            else
            {
                continue;
            }
            contract.UpdatedAt = now;
        }
    }
}
