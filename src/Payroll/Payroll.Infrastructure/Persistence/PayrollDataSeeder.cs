using Microsoft.EntityFrameworkCore;
using Payroll.Domain.Entities;
using PayrollEntity = global::Payroll.Domain.Entities.Payroll;

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

        // 3. Seed Sample Payroll Period & Payroll Records
        if (!await context.PayrollPeriods.AnyAsync())
        {
            var periodId = Guid.Parse("33333333-3333-3333-3333-333333333301");
            var samplePeriod = new PayrollPeriod
            {
                PayrollPeriodID = periodId,
                Year = 2026,
                Month = 11,
                Status = "draft",
                CreatedAt = DateTime.UtcNow
            };

            await context.PayrollPeriods.AddAsync(samplePeriod);

            // Sample Employee 1
            var emp1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var payroll1Id = Guid.Parse("44444444-4444-4444-4444-444444444401");
            var payroll1 = new PayrollEntity
            {
                PayrollID = payroll1Id,
                PayrollPeriodID = periodId,
                EmployeeID = emp1Id,
                BaseSalary = 15000000m,
                WorkingDays = 22m,
                StandardWorkingDay = 22m,
                TotalAllowance = 1230000m,
                TotalPayCut = 1575000m,
                GrossSalary = 16230000m,
                NetSalary = 14655000m,
                Status = "draft"
            };

            // Allowance for Emp 1
            var allowance1 = new Allowance
            {
                AllowanceID = Guid.NewGuid(),
                PayrollID = payroll1Id,
                Status = "active",
                AllowanceDetails = new List<AllowanceDetail>
                {
                    new()
                    {
                        AllowanceTypeID = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                        Description = "Ăn trưa tháng 11/2026",
                        RateApplied = 1m,
                        Amount = 730000m
                    },
                    new()
                    {
                        AllowanceTypeID = Guid.Parse("11111111-1111-1111-1111-111111111102"),
                        Description = "Xăng xe tháng 11/2026",
                        RateApplied = 1m,
                        Amount = 500000m
                    }
                }
            };
            payroll1.Allowances.Add(allowance1);

            // PayCut for Emp 1
            var paycut1 = new PayCut
            {
                PayCutID = Guid.NewGuid(),
                PayrollID = payroll1Id,
                Status = "active",
                Details = new List<PayCutDetail>
                {
                    new()
                    {
                        PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222201"),
                        Description = "BHXH 8%",
                        RateApplied = 0.08m,
                        Amount = 1200000m
                    },
                    new()
                    {
                        PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222202"),
                        Description = "BHYT 1.5%",
                        RateApplied = 0.015m,
                        Amount = 225000m
                    },
                    new()
                    {
                        PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222203"),
                        Description = "BHTN 1%",
                        RateApplied = 0.01m,
                        Amount = 150000m
                    }
                }
            };
            payroll1.PayCuts.Add(paycut1);

            // Sample Employee 2
            var emp2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var payroll2Id = Guid.Parse("44444444-4444-4444-4444-444444444402");
            var payroll2 = new PayrollEntity
            {
                PayrollID = payroll2Id,
                PayrollPeriodID = periodId,
                EmployeeID = emp2Id,
                BaseSalary = 12000000m,
                WorkingDays = 20m,
                StandardWorkingDay = 22m,
                TotalAllowance = 730000m,
                TotalPayCut = 1360000m,
                GrossSalary = 11639090.91m,
                NetSalary = 10279090.91m,
                Status = "draft"
            };

            var allowance2 = new Allowance
            {
                AllowanceID = Guid.NewGuid(),
                PayrollID = payroll2Id,
                Status = "active",
                AllowanceDetails = new List<AllowanceDetail>
                {
                    new()
                    {
                        AllowanceTypeID = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                        Description = "Ăn trưa tháng 11/2026",
                        RateApplied = 1m,
                        Amount = 730000m
                    }
                }
            };
            payroll2.Allowances.Add(allowance2);

            var paycut2 = new PayCut
            {
                PayCutID = Guid.NewGuid(),
                PayrollID = payroll2Id,
                Status = "active",
                Details = new List<PayCutDetail>
                {
                    new()
                    {
                        PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222201"),
                        Description = "BHXH 8%",
                        RateApplied = 0.08m,
                        Amount = 960000m
                    },
                    new()
                    {
                        PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222202"),
                        Description = "BHYT 1.5%",
                        RateApplied = 0.015m,
                        Amount = 180000m
                    },
                    new()
                    {
                        PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222203"),
                        Description = "BHTN 1%",
                        RateApplied = 0.01m,
                        Amount = 120000m
                    },
                    new()
                    {
                        PayCutReasonID = Guid.Parse("22222222-2222-2222-2222-222222222205"),
                        Description = "Đi muộn 1 lần",
                        RateApplied = 1m,
                        Amount = 100000m
                    }
                }
            };
            payroll2.PayCuts.Add(paycut2);

            await context.Payrolls.AddRangeAsync(new[] { payroll1, payroll2 });
            await context.SaveChangesAsync();
        }
    }
}
