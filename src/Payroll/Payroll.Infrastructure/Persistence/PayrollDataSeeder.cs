using Microsoft.EntityFrameworkCore;
using Payroll.Domain.Entities;

namespace Payroll.Infrastructure.Persistence;

public static class PayrollDataSeeder
{
    public static async Task SeedAsync(PayrollDbContext context)
    {
        // 1. Seed Allowance Types
        if (!await context.AllowanceTypes.AnyAsync())
        {
            var allowanceTypes = new List<AllowanceType>
            {
                new()
                {
                    AllowanceTypeID = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                    Name = "Phụ cấp ăn trưa",
                    Description = "Hỗ trợ bữa ăn giữa ca, miễn thuế theo quy định",
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
                    Description = "Tiền phụ cấp làm thêm ngoài giờ",
                    CalculationType = "percentage",
                    DefaultRate = 1.5m,
                    Status = "active"
                }
            };

            await context.AllowanceTypes.AddRangeAsync(allowanceTypes);
            await context.SaveChangesAsync();
        }

        // 2. Seed Pay Cut Reasons
        if (!await context.PayCutReasons.AnyAsync())
        {
            var payCutReasons = new List<PayCutReason>
            {
                new()
                {
                    PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222201"),
                    Name = "Bảo hiểm xã hội (BHXH 8%)",
                    CalculationType = "percentage",
                    DefaultRate = 0.0800m,
                    Status = "active"
                },
                new()
                {
                    PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222202"),
                    Name = "Bảo hiểm y tế (BHYT 1.5%)",
                    CalculationType = "percentage",
                    DefaultRate = 0.0150m,
                    Status = "active"
                },
                new()
                {
                    PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222203"),
                    Name = "Bảo hiểm thất nghiệp (BHTN 1%)",
                    CalculationType = "percentage",
                    DefaultRate = 0.0100m,
                    Status = "active"
                },
                new()
                {
                    PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222204"),
                    Name = "Thuế thu nhập cá nhân (TNCN)",
                    CalculationType = "formula",
                    DefaultRate = 0m,
                    Status = "active"
                },
                new()
                {
                    PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222205"),
                    Name = "Phạt đi muộn / về sớm",
                    CalculationType = "fixed",
                    DefaultRate = 100000m,
                    Status = "active"
                }
            };

            await context.PayCutReasons.AddRangeAsync(payCutReasons);
            await context.SaveChangesAsync();
        }
    }
}