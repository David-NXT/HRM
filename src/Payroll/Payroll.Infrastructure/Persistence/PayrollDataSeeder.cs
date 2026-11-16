using Microsoft.EntityFrameworkCore;
using Payroll.Domain.Entities;

namespace Payroll.Infrastructure.Persistence;

public static class PayrollDataSeeder
{
    public static async Task SeedAsync(PayrollDbContext context)
    {
        var allowanceTypes = new List<AllowanceType>
        {
            new()
            {
                AllowanceTypeID = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                Name = "Phụ cấp ăn trưa",
                Description = "Hỗ trợ bữa ăn giữa ca",
                CalculationType = "fixed",
                DefaultRate = 730000m,
                Status = "active"
            },
            new()
            {
                AllowanceTypeID = Guid.Parse("11111111-1111-1111-1111-111111111102"),
                Name = "Phụ cấp xăng xe & đi lại",
                Description = "Hỗ trợ chi phí đi lại làm việc",
                CalculationType = "fixed",
                DefaultRate = 500000m,
                Status = "active"
            },
            new()
            {
                AllowanceTypeID = Guid.Parse("11111111-1111-1111-1111-111111111103"),
                Name = "Phụ cấp trách nhiệm",
                Description = "Phụ cấp cho vị trí quản lý / trưởng nhóm",
                CalculationType = "fixed",
                DefaultRate = 1000000m,
                Status = "active"
            },
            new()
            {
                AllowanceTypeID = Guid.Parse("11111111-1111-1111-1111-111111111104"),
                Name = "Làm thêm giờ (OT)",
                Description = "Phụ cấp làm thêm ngoài giờ",
                CalculationType = "fixed",
                DefaultRate = 500000m,
                Status = "active"
            },
            new()
            {
                AllowanceTypeID = Guid.Parse("11111111-1111-1111-1111-111111111105"),
                Name = "Phụ cấp điện thoại",
                Description = "Hỗ trợ chi phí liên lạc phục vụ công việc",
                CalculationType = "fixed",
                DefaultRate = 300000m,
                Status = "active"
            }
        };

        var payCutReasons = new List<PayCutReason>
        {
            new()
            {
                PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222201"),
                Name = "Bảo hiểm xã hội (BHXH 8%)",
                Description = "Khấu trừ bảo hiểm xã hội theo tỷ lệ cấu hình",
                CalculationType = "percentage",
                DefaultRate = 0.0800m,
                Status = "active"
            },
            new()
            {
                PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222202"),
                Name = "Bảo hiểm y tế (BHYT 1.5%)",
                Description = "Khấu trừ bảo hiểm y tế theo tỷ lệ cấu hình",
                CalculationType = "percentage",
                DefaultRate = 0.0150m,
                Status = "active"
            },
            new()
            {
                PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222203"),
                Name = "Bảo hiểm thất nghiệp (BHTN 1%)",
                Description = "Khấu trừ bảo hiểm thất nghiệp theo tỷ lệ cấu hình",
                CalculationType = "percentage",
                DefaultRate = 0.0100m,
                Status = "active"
            },
            new()
            {
                PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222204"),
                Name = "Thuế thu nhập cá nhân (TNCN)",
                Description = "Khoản thuế thu nhập cá nhân nhập theo số tiền tính toán",
                CalculationType = "formula",
                DefaultRate = 0m,
                Status = "active"
            },
            new()
            {
                PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222205"),
                Name = "Phạt đi muộn / về sớm",
                Description = "Khoản phạt theo quy định chấm công",
                CalculationType = "fixed",
                DefaultRate = 100000m,
                Status = "active"
            }
        };

        foreach (var type in allowanceTypes)
        {
            if (!await context.AllowanceTypes.AnyAsync(item =>
                item.AllowanceTypeID == type.AllowanceTypeID || item.Name == type.Name))
            {
                context.AllowanceTypes.Add(type);
            }
        }

        foreach (var reason in payCutReasons)
        {
            if (!await context.PayCutReasons.AnyAsync(item =>
                item.PayCutReasonID == reason.PayCutReasonID || item.Name == reason.Name))
            {
                context.PayCutReasons.Add(reason);
            }
        }

        await context.SaveChangesAsync();
    }
}