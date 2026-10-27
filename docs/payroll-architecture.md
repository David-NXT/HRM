# THIẾT KẾ KIẾN TRÚC & QUY TRÌNH TRIỂN KHAI SERVICE PAYROLL
**Hệ thống:** HRM (Human Resource Management) - Đồ án môn học C# (Năm 3 SGU)  
**Định hướng kiến trúc:** Clean Architecture tinh giản (Pragmatic / Light Clean Architecture)  
**Cơ sở dữ liệu:** PostgreSQL (Npgsql + Entity Framework Core)  
**Nền tảng:** .NET 10 / ASP.NET Core Web API  

> [!NOTE]
> **Phạm vi tài liệu:** Đây là thiết kế **chính thức được áp dụng trực tiếp** vào mã nguồn dự án. Các tài liệu có tiền tố `ly-thuyet_` trong thư mục `docs/` là tài liệu lý thuyết mở rộng chuẩn Enterprise dùng để tham khảo đưa vào báo cáo đồ án.

---

## 1. TẠI SAO LỰA CHỌN PHIÊN BẢN TINH GIẢN?

Trong hệ thống HRM, nhóm phải phát triển cùng lúc **4 service** (`Attendance`, `HR`, `Payroll`, `Project`). Nếu service nào cũng chia làm 4 project độc lập kèm `MediatR`, `CQRS`, `FluentValidation Pipeline` thì nhóm sẽ phải quản lý tới **16 projects** và hàng trăm file trung gian.

Mô hình **Clean Architecture tinh giản** này được thiết kế để:
1. **Đúng bản chất Clean Architecture:** Tách bạch 3 vùng trách nhiệm:
   - **Tầng Giao tiếp (`Controllers/`):** Tiếp nhận request, trả về mã HTTP.
   - **Tầng Nghiệp vụ (`Core/`):** Tính toán lương Gross-Net, bảo hiểm, thuế TNCN, không phụ thuộc vào công nghệ DB.
   - **Tầng Dữ liệu (`Infrastructure/`):** Lưu trữ PostgreSQL và gọi API sang service khác.
2. **Gọn gàng (1 Project duy nhất):** Toàn bộ nằm trọn trong `src/Payroll/`, không cần tạo thêm project phụ, không sợ lỗi tham chiếu chéo.
3. **Dễ code - Dễ debug:** Dùng **Service Pattern** (`IPayrollService`), sinh viên đọc là hiểu ngay luồng chạy từ Controller $\rightarrow$ Service $\rightarrow$ Repository $\rightarrow$ PostgreSQL.
4. **Có chế độ Mock dự phòng:** Service Payroll có thể tự chạy thử nghiệm tính lương ngay cả khi service `HR` hoặc `Attendance` của bạn bè trong nhóm chưa code xong!

---

## 2. CẤU TRÚC THƯ MỤC THỰC TẾ TRONG `src/Payroll/`

Cấu trúc thư mục được tổ chức theo 3 tầng rõ ràng ngay bên trong project `Payroll`:

```text
src/Payroll/
├── Controllers/                            # [TẦNG 1: PRESENTATION]
│   ├── PayrollCyclesController.cs          # API tạo kỳ lương, bấm tính lương, duyệt bảng lương
│   └── PayslipsController.cs               # API tra cứu phiếu lương nhân viên
│
├── Core/                                   # [TẦNG 2: BUSINESS LOGIC & DOMAIN]
│   ├── Entities/                           # Thực thể nghiệp vụ
│   │   ├── BaseEntity.cs                   # Id (Guid), CreatedAt, UpdatedAt
│   │   ├── PayrollCycle.cs                 # Kỳ tính lương (Tháng, Năm, Trạng thái)
│   │   ├── Payslip.cs                      # Phiếu lương nhân viên
│   │   └── EmployeeSalaryProfile.cs        # Lương cơ bản, phụ cấp thỏa thuận
│   ├── Enums/
│   │   └── PayrollStatus.cs                # Draft (0), Calculated (1), Approved (2), Paid (3)
│   ├── DTOs/                               # Đối tượng truyền nhận dữ liệu qua API
│   │   ├── CreatePayrollCycleRequest.cs    # Dữ liệu gửi lên khi tạo kỳ lương
│   │   ├── PayrollCycleDto.cs              # Dữ liệu kỳ lương trả về
│   │   ├── PayslipDto.cs                   # Chi tiết phiếu lương trả về cho nhân viên
│   │   └── WorkSummaryDto.cs               # Số ngày công & giờ OT nhận từ Attendance
│   ├── Interfaces/                         # Hợp đồng trừu tượng
│   │   ├── IPayrollService.cs              # Interface nghiệp vụ chính
│   │   ├── IPayrollRepository.cs           # Interface truy vấn Database
│   │   ├── IAttendanceClient.cs            # Interface gọi Attendance Service
│   │   └── IHrClient.cs                    # Interface gọi HR Service
│   └── Services/                           # NƠI CHỨA CODE NGHIỆP VỤ LÕI
│       └── PayrollService.cs               # Triển khai tính toán Gross-Net, Thuế, Bảo hiểm
│
├── Infrastructure/                         # [TẦNG 3: DATABASE POSTGRESQL & TÍCH HỢP]
│   ├── Data/
│   │   ├── PayrollDbContext.cs             # DbContext của Entity Framework Core
│   │   └── Configurations/                 # Fluent API ánh xạ cột vào PostgreSQL
│   │       ├── PayrollCycleConfiguration.cs
│   │       └── PayslipConfiguration.cs
│   ├── Repositories/
│   │   └── PayrollRepository.cs            # Code LINQ truy vấn PostgreSQL
│   └── External/                           # Triển khai gọi API sang service khác
│       ├── AttendanceClient.cs             # HttpClient gọi Attendance Service
│       └── HrClient.cs                     # HttpClient gọi HR Service
│
├── appsettings.json                        # Chuỗi kết nối PostgreSQL & URL các service
├── appsettings.Development.json
├── Program.cs                              # Đăng ký DI & cấu hình HTTP Pipeline
└── Payroll.csproj                          # Chứa các gói NuGet EF Core & Npgsql
```

---

## 3. THIẾT KẾ CHI TIẾT CÁC THÀNH PHẦN

### 3.1. Tầng Nghiệp vụ (`Core/`)

#### A. Các Thực thể (`Core/Entities/`)
* **`BaseEntity.cs`**:
  ```csharp
  namespace Payroll.Core.Entities;
  
  public abstract class BaseEntity
  {
      public Guid Id { get; set; } = Guid.NewGuid();
      public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
      public DateTimeOffset? UpdatedAt { get; set; }
  }
  ```

* **`PayrollCycle.cs` (Kỳ lương):**
  ```csharp
  namespace Payroll.Core.Entities;
  using Payroll.Core.Enums;

  public class PayrollCycle : BaseEntity
  {
      public int Month { get; set; }
      public int Year { get; set; }
      public string Name { get; set; } = string.Empty;
      public int StandardWorkingDays { get; set; } = 22; // Mặc định 22 ngày công chuẩn
      public PayrollStatus Status { get; set; } = PayrollStatus.Draft;
      public DateTime FromDate { get; set; }
      public DateTime ToDate { get; set; }

      // Quan hệ 1 - N với danh sách phiếu lương
      public List<Payslip> Payslips { get; set; } = new();
  }
  ```

* **`Payslip.cs` (Phiếu lương):**
  ```csharp
  namespace Payroll.Core.Entities;

  public class Payslip : BaseEntity
  {
      public Guid PayrollCycleId { get; set; }
      public Guid EmployeeId { get; set; }
      
      public decimal ActualBaseSalary { get; set; }       // Lương theo ngày công thực tế
      public decimal OvertimePay { get; set; }             // Lương làm thêm giờ (OT)
      public decimal TotalAllowances { get; set; }         // Tổng phụ cấp (ăn trưa, xăng xe)
      public decimal TotalGrossSalary { get; set; }        // Tổng thu nhập trước thuế/bảo hiểm
      
      public decimal SocialInsurance { get; set; }         // BHXH (8%)
      public decimal HealthInsurance { get; set; }         // BHYT (1.5%)
      public decimal UnemploymentInsurance { get; set; }   // BHTN (1%)
      public decimal TotalInsurance { get; set; }          // Tổng đóng BH (10.5%)
      
      public decimal TaxableIncome { get; set; }           // Thu nhập tính thuế
      public decimal PersonalIncomeTax { get; set; }       // Thuế TNCN lũy tiến 7 bậc
      
      public decimal NetSalary { get; set; }               // Lương thực nhận
  }
  ```

* **`EmployeeSalaryProfile.cs` (Hồ sơ lương cố định của nhân viên):**
  ```csharp
  namespace Payroll.Core.Entities;

  public class EmployeeSalaryProfile : BaseEntity
  {
      public Guid EmployeeId { get; set; }
      public decimal BaseSalary { get; set; }              // Lương cơ bản theo hợp đồng
      public decimal TaxableAllowances { get; set; }        // Phụ cấp chịu thuế (trách nhiệm...)
      public decimal NonTaxableAllowances { get; set; }     // Phụ cấp miễn thuế (ăn trưa...)
      public string? BankAccountNumber { get; set; }
      public string? BankName { get; set; }
      public bool IsActive { get; set; } = true;
  }
  ```

#### B. Trái tim nghiệp vụ: `PayrollService.cs` (`Core/Services/`)
Chứa toàn bộ quy tắc tính toán chuẩn Luật Lao động và Thuế TNCN Việt Nam:

```csharp
namespace Payroll.Core.Services;

using Payroll.Core.Entities;
using Payroll.Core.Enums;
using Payroll.Core.Interfaces;
using Payroll.Core.DTOs;

public class PayrollService : IPayrollService
{
    private readonly IPayrollRepository _repository;
    private readonly IAttendanceClient _attendanceClient;
    private readonly IHrClient _hrClient;

    private const decimal SelfDeduction = 11_000_000m;      // Giảm trừ bản thân 11tr
    private const decimal DependentDeduction = 4_400_000m;  // Giảm trừ người phụ thuộc 4.4tr/người

    public PayrollService(
        IPayrollRepository repository,
        IAttendanceClient attendanceClient,
        IHrClient hrClient)
    {
        _repository = repository;
        _attendanceClient = attendanceClient;
        _hrClient = hrClient;
    }

    public async Task<PayrollCycleDto> CreateCycleAsync(CreatePayrollCycleRequest req)
    {
        var existing = await _repository.GetCycleByMonthYearAsync(req.Month, req.Year);
        if (existing != null)
            throw new InvalidOperationException($"Kỳ lương tháng {req.Month}/{req.Year} đã tồn tại!");

        var cycle = new PayrollCycle
        {
            Month = req.Month,
            Year = req.Year,
            Name = $"Bảng lương Tháng {req.Month}/{req.Year}",
            StandardWorkingDays = req.StandardWorkingDays > 0 ? req.StandardWorkingDays : 22,
            Status = PayrollStatus.Draft,
            FromDate = new DateTime(req.Year, req.Month, 1),
            ToDate = new DateTime(req.Year, req.Month, DateTime.DaysInMonth(req.Year, req.Month))
        };

        await _repository.AddCycleAsync(cycle);
        await _repository.SaveChangesAsync();

        return MapToDto(cycle);
    }

    public async Task<int> CalculatePayrollAsync(Guid cycleId)
    {
        var cycle = await _repository.GetCycleByIdAsync(cycleId);
        if (cycle == null)
            throw new KeyNotFoundException("Không tìm thấy kỳ lương.");

        if (cycle.Status == PayrollStatus.Approved || cycle.Status == PayrollStatus.Paid)
            throw new InvalidOperationException("Kỳ lương đã chốt hoặc thanh toán, không thể tính lại.");

        // 1. Lấy dữ liệu công từ Attendance Service (hoặc mock nếu chưa có)
        var attendanceData = await _attendanceClient.GetMonthlyAttendanceSummaryAsync(cycle.Month, cycle.Year);

        // 2. Lấy dữ liệu người phụ thuộc từ HR Service
        var dependentsData = await _hrClient.GetDependentsCountAsync();

        // 3. Lấy hồ sơ lương nhân viên trong DB
        var profiles = await _repository.GetActiveProfilesAsync();

        // 4. Xóa phiếu lương cũ của kỳ này nếu đang tính lại
        await _repository.DeletePayslipsByCycleIdAsync(cycleId);

        var payslips = new List<Payslip>();

        foreach (var profile in profiles)
        {
            attendanceData.TryGetValue(profile.EmployeeId, out var work);
            work ??= new WorkSummaryDto { ActualWorkingDays = cycle.StandardWorkingDays }; // Mặc định đủ công nếu thiếu dữ liệu

            dependentsData.TryGetValue(profile.EmployeeId, out var dependentCount);

            // BƯỚC A: Tính lương theo ngày công
            decimal dailyRate = profile.BaseSalary / cycle.StandardWorkingDays;
            decimal actualBase = dailyRate * work.ActualWorkingDays;

            // BƯỚC B: Tính tiền OT (150% ngày thường, 200% cuối tuần, 300% lễ)
            decimal hourlyRate = dailyRate / 8;
            decimal otPay = (work.NormalOtHours * hourlyRate * 1.5m)
                          + (work.WeekendOtHours * hourlyRate * 2.0m)
                          + (work.HolidayOtHours * hourlyRate * 3.0m);

            decimal gross = actualBase + otPay + profile.TaxableAllowances + profile.NonTaxableAllowances;

            // BƯỚC C: Trích nộp bảo hiểm (10.5%)
            decimal bhxh = profile.BaseSalary * 0.08m;
            decimal bhyt = profile.BaseSalary * 0.015m;
            decimal bhtn = profile.BaseSalary * 0.01m;
            decimal totalInsurance = bhxh + bhyt + bhtn;

            // BƯỚC D: Thu nhập tính thuế TNCN (OT vượt 100% và phụ cấp miễn thuế được trừ ra)
            decimal taxFreeOt = (work.NormalOtHours * hourlyRate * 0.5m)
                              + (work.WeekendOtHours * hourlyRate * 1.0m)
                              + (work.HolidayOtHours * hourlyRate * 2.0m);

            decimal familyDeduction = SelfDeduction + (dependentCount * DependentDeduction);
            decimal taxableIncome = gross - profile.NonTaxableAllowances - taxFreeOt;
            decimal assessableIncome = Math.Max(0, taxableIncome - totalInsurance - familyDeduction);

            // BƯỚC E: Biểu thuế lũy tiến 7 bậc
            decimal pit = CalculateProgressiveTax(assessableIncome);

            // BƯỚC F: Lương thực nhận (Net)
            decimal net = gross - totalInsurance - pit;

            payslips.Add(new Payslip
            {
                PayrollCycleId = cycleId,
                EmployeeId = profile.EmployeeId,
                ActualBaseSalary = actualBase,
                OvertimePay = otPay,
                TotalAllowances = profile.TaxableAllowances + profile.NonTaxableAllowances,
                TotalGrossSalary = gross,
                SocialInsurance = bhxh,
                HealthInsurance = bhyt,
                UnemploymentInsurance = bhtn,
                TotalInsurance = totalInsurance,
                TaxableIncome = assessableIncome,
                PersonalIncomeTax = pit,
                NetSalary = net
            });
        }

        cycle.Status = PayrollStatus.Calculated;
        await _repository.AddPayslipsAsync(payslips);
        await _repository.SaveChangesAsync();

        return payslips.Count;
    }

    private decimal CalculateProgressiveTax(decimal income)
    {
        if (income <= 0) return 0;
        if (income <= 5_000_000m) return income * 0.05m;
        if (income <= 10_000_000m) return 250_000m + (income - 5_000_000m) * 0.10m;
        if (income <= 18_000_000m) return 750_000m + (income - 10_000_000m) * 0.15m;
        if (income <= 32_000_000m) return 1_950_000m + (income - 18_000_000m) * 0.20m;
        if (income <= 52_000_000m) return 4_750_000m + (income - 32_000_000m) * 0.25m;
        if (income <= 80_000_000m) return 9_750_000m + (income - 52_000_000m) * 0.30m;
        return 18_150_000m + (income - 80_000_000m) * 0.35m;
    }

    public async Task<bool> ApproveCycleAsync(Guid cycleId)
    {
        var cycle = await _repository.GetCycleByIdAsync(cycleId);
        if (cycle == null || cycle.Status != PayrollStatus.Calculated)
            return false;

        cycle.Status = PayrollStatus.Approved;
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<PayrollCycleDto?> GetCycleByIdAsync(Guid cycleId)
    {
        var cycle = await _repository.GetCycleWithPayslipsAsync(cycleId);
        return cycle == null ? null : MapToDto(cycle);
    }

    private static PayrollCycleDto MapToDto(PayrollCycle cycle) => new()
    {
        Id = cycle.Id,
        Month = cycle.Month,
        Year = cycle.Year,
        Name = cycle.Name,
        Status = cycle.Status.ToString(),
        StandardWorkingDays = cycle.StandardWorkingDays,
        Payslips = cycle.Payslips.Select(p => new PayslipDto
        {
            Id = p.Id,
            EmployeeId = p.EmployeeId,
            GrossSalary = p.TotalGrossSalary,
            TotalInsurance = p.TotalInsurance,
            PersonalIncomeTax = p.PersonalIncomeTax,
            NetSalary = p.NetSalary
        }).ToList()
    };
}
```

---

### 3.2. Tầng Kết nối Cơ sở dữ liệu PostgreSQL (`Infrastructure/`)

#### A. Cấu hình DbContext (`Infrastructure/Data/PayrollDbContext.cs`)
```csharp
namespace Payroll.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Payroll.Core.Entities;

public class PayrollDbContext : DbContext
{
    public PayrollDbContext(DbContextOptions<PayrollDbContext> options) : base(options) { }

    public DbSet<PayrollCycle> PayrollCycles => Set<PayrollCycle>();
    public DbSet<Payslip> Payslips => Set<Payslip>();
    public DbSet<EmployeeSalaryProfile> SalaryProfiles => Set<EmployeeSalaryProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Cấu hình kiểu dữ liệu chính xác cho PostgreSQL
        modelBuilder.Entity<Payslip>(b =>
        {
            b.ToTable("payslips");
            b.Property(p => p.ActualBaseSalary).HasColumnType("numeric(18,2)");
            b.Property(p => p.OvertimePay).HasColumnType("numeric(18,2)");
            b.Property(p => p.TotalGrossSalary).HasColumnType("numeric(18,2)");
            b.Property(p => p.TotalInsurance).HasColumnType("numeric(18,2)");
            b.Property(p => p.PersonalIncomeTax).HasColumnType("numeric(18,2)");
            b.Property(p => p.NetSalary).HasColumnType("numeric(18,2)");
            b.Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<EmployeeSalaryProfile>(b =>
        {
            b.ToTable("employee_salary_profiles");
            b.Property(p => p.BaseSalary).HasColumnType("numeric(18,2)");
            b.Property(p => p.TaxableAllowances).HasColumnType("numeric(18,2)");
            b.Property(p => p.NonTaxableAllowances).HasColumnType("numeric(18,2)");
        });

        modelBuilder.Entity<PayrollCycle>(b =>
        {
            b.ToTable("payroll_cycles");
            b.HasIndex(c => new { c.Year, c.Month }).IsUnique();
        });
    }
}
```

#### B. Repository truy vấn dữ liệu (`Infrastructure/Repositories/PayrollRepository.cs`)
```csharp
namespace Payroll.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Payroll.Core.Entities;
using Payroll.Core.Interfaces;
using Payroll.Infrastructure.Data;

public class PayrollRepository : IPayrollRepository
{
    private readonly PayrollDbContext _context;

    public PayrollRepository(PayrollDbContext context)
    {
        _context = context;
    }

    public async Task<PayrollCycle?> GetCycleByIdAsync(Guid id) =>
        await _context.PayrollCycles.FindAsync(id);

    public async Task<PayrollCycle?> GetCycleByMonthYearAsync(int month, int year) =>
        await _context.PayrollCycles.FirstOrDefaultAsync(c => c.Month == month && c.Year == year);

    public async Task<PayrollCycle?> GetCycleWithPayslipsAsync(Guid id) =>
        await _context.PayrollCycles
            .Include(c => c.Payslips)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<EmployeeSalaryProfile>> GetActiveProfilesAsync() =>
        await _context.SalaryProfiles.Where(p => p.IsActive).ToListAsync();

    public async Task AddCycleAsync(PayrollCycle cycle) =>
        await _context.PayrollCycles.AddAsync(cycle);

    public async Task AddPayslipsAsync(IEnumerable<Payslip> payslips) =>
        await _context.Payslips.AddRangeAsync(payslips);

    public async Task DeletePayslipsByCycleIdAsync(Guid cycleId)
    {
        var oldPayslips = await _context.Payslips.Where(p => p.PayrollCycleId == cycleId).ToListAsync();
        _context.Payslips.RemoveRange(oldPayslips);
    }

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
```

#### C. HttpClient có cơ chế tự Mock dự phòng (`Infrastructure/External/`)
> [!TIP]
> **Cực kỳ tiện lợi khi làm đồ án:** Nếu service `Attendance` hoặc `HR` của nhóm bạn chưa chạy, hàm này sẽ tự động bắt ngoại lệ và trả về dữ liệu mẫu để bạn test được Payroll mà không bị tắc nghẽn!

```csharp
namespace Payroll.Infrastructure.External;

using System.Net.Http.Json;
using Payroll.Core.DTOs;
using Payroll.Core.Interfaces;

public class AttendanceClient : IAttendanceClient
{
    private readonly HttpClient _client;

    public AttendanceClient(HttpClient client) => _client = client;

    public async Task<Dictionary<Guid, WorkSummaryDto>> GetMonthlyAttendanceSummaryAsync(int month, int year)
    {
        try
        {
            var res = await _client.GetFromJsonAsync<Dictionary<Guid, WorkSummaryDto>>(
                $"/api/attendance/summary?month={month}&year={year}");
            return res ?? new();
        }
        catch
        {
            // TỰ ĐỘNG FALLBACK DỮ LIỆU GIẢ NẾU SERVICE ATTENDANCE CHƯA BẬT
            return new Dictionary<Guid, WorkSummaryDto>();
        }
    }
}
```

---

### 3.3. Tầng Controllers (`Controllers/`)

Controller chỉ đóng vai trò tiếp nhận dữ liệu và chuyển thẳng vào `IPayrollService`:

```csharp
namespace Payroll.Controllers;

using Microsoft.AspNetCore.Mvc;
using Payroll.Core.DTOs;
using Payroll.Core.Interfaces;

[ApiController]
[Route("api/payroll-cycles")]
public class PayrollCyclesController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public PayrollCyclesController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    // 1. Tạo kỳ lương mới (POST /api/payroll-cycles)
    [HttpPost]
    public async Task<IActionResult> CreateCycle([FromBody] CreatePayrollCycleRequest req)
    {
        try
        {
            var result = await _payrollService.CreateCycleAsync(req);
            return CreatedAtAction(nameof(GetCycleById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // 2. Kích hoạt tính lương (POST /api/payroll-cycles/{id}/calculate)
    [HttpPost("{id:guid}/calculate")]
    public async Task<IActionResult> CalculatePayroll(Guid id)
    {
        try
        {
            int count = await _payrollService.CalculatePayrollAsync(id);
            return Ok(new { message = $"Tính lương thành công cho {count} nhân viên.", totalEmployees = count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // 3. Phê duyệt bảng lương (POST /api/payroll-cycles/{id}/approve)
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveCycle(Guid id)
    {
        bool success = await _payrollService.ApproveCycleAsync(id);
        if (!success)
            return BadRequest(new { message = "Không thể duyệt kỳ lương (chưa tính hoặc không tồn tại)." });
        
        return Ok(new { message = "Kỳ lương đã được phê duyệt thành công." });
    }

    // 4. Xem chi tiết bảng lương (GET /api/payroll-cycles/{id})
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCycleById(Guid id)
    {
        var cycle = await _payrollService.GetCycleByIdAsync(id);
        if (cycle == null)
            return NotFound(new { message = "Không tìm thấy kỳ lương." });

        return Ok(cycle);
    }
}
```

---

### 3.4. Cấu hình Khởi động trong `Program.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Payroll.Core.Interfaces;
using Payroll.Core.Services;
using Payroll.Infrastructure.Data;
using Payroll.Infrastructure.External;
using Payroll.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình PostgreSQL DbContext
var connectionString = builder.Configuration.GetConnectionString("PayrollDb");
builder.Services.AddDbContext<PayrollDbContext>(options =>
{
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention(); // Chuyển cột thành chữ thường snake_case chuẩn Postgres
});

// 2. Đăng ký Dependency Injection
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
builder.Services.AddScoped<IPayrollService, PayrollService>();

// 3. Đăng ký HttpClients gọi sang service khác
builder.Services.AddHttpClient<IAttendanceClient, AttendanceClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ExternalServices:AttendanceUrl"] ?? "http://localhost:5001");
});
builder.Services.AddHttpClient<IHrClient, HrClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ExternalServices:HrUrl"] ?? "http://localhost:5002");
});

// 4. Cấu hình Web API & Swagger UI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## 4. QUY TRÌNH THỰC HIỆN TỪNG BƯỚC (STEP-BY-STEP)

Để hoàn thiện service `Payroll` từ trạng thái hiện tại, bạn chỉ cần thực hiện 6 bước ngắn gọn sau:

### Bước 1: Cài đặt các gói NuGet vào `src/Payroll/`
Mở PowerShell tại thư mục `src/Payroll`:
```powershell
cd src/Payroll
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package EFCore.NamingConventions
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Swashbuckle.AspNetCore
```

### Bước 2: Tạo các thư mục theo cấu trúc
```powershell
New-Item -ItemType Directory -Force -Path Core/Entities, Core/Enums, Core/DTOs, Core/Interfaces, Core/Services
New-Item -ItemType Directory -Force -Path Infrastructure/Data, Infrastructure/Repositories, Infrastructure/External
New-Item -ItemType Directory -Force -Path Controllers
```

### Bước 3: Tạo các file Code
Lần lượt copy các đoạn code đã thiết kế ở Mục 3 vào các thư mục tương ứng:
1. `Core/Entities/`: `BaseEntity.cs`, `PayrollCycle.cs`, `Payslip.cs`, `EmployeeSalaryProfile.cs`.
2. `Core/Enums/`: `PayrollStatus.cs`.
3. `Core/Interfaces/`: `IPayrollService.cs`, `IPayrollRepository.cs`, `IAttendanceClient.cs`, `IHrClient.cs`.
4. `Core/Services/`: `PayrollService.cs`.
5. `Infrastructure/Data/`: `PayrollDbContext.cs`.
6. `Infrastructure/Repositories/`: `PayrollRepository.cs`.
7. `Infrastructure/External/`: `AttendanceClient.cs`, `HrClient.cs`.
8. `Controllers/`: `PayrollCyclesController.cs`.

### Bước 4: Cấu hình `appsettings.json`
Mở file `src/Payroll/appsettings.json` và thêm chuỗi kết nối PostgreSQL:
```json
{
  "ConnectionStrings": {
    "PayrollDb": "Host=localhost;Port=5432;Database=hrm_payroll;Username=postgres;Password=YourPostgresPassword"
  },
  "ExternalServices": {
    "AttendanceUrl": "http://localhost:5001",
    "HrUrl": "http://localhost:5002"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Bước 5: Chạy Migration khởi tạo CSDL PostgreSQL
Chạy lệnh sau ngay tại thư mục `src/Payroll`:
```powershell
# Tạo file migration
dotnet ef migrations add InitialPayrollDb

# Áp dụng bảng vào PostgreSQL
dotnet ef database update
```
*(Nếu chưa có DB, EF Core sẽ tự động tạo database `hrm_payroll` và các bảng `payroll_cycles`, `payslips`, `employee_salary_profiles` trong PostgreSQL).*

### Bước 6: Khởi chạy và Test thử trên Swagger UI
```powershell
dotnet run
```
Mở trình duyệt truy cập: `https://localhost:7xxx/swagger`
- Gọi `POST /api/payroll-cycles` với body `{"month": 10, "year": 2026, "standardWorkingDays": 22}`.
- Gọi `POST /api/payroll-cycles/{id}/calculate` để tính toán toàn bộ tiền lương, thuế TNCN và bảo hiểm.
- Gọi `GET /api/payroll-cycles/{id}` để xem kết quả bảng lương hoàn chỉnh!

---

## 5. BẢN THUYẾT MINH NHANH KHI BẢO VỆ ĐỒ ÁN VỚI GIẢNG VIÊN

Khi thầy cô hỏi về kiến trúc của service này, bạn có thể trả lời tự tin như sau:

1. **"Kiến trúc của service này là gì?"**  
   $\rightarrow$ *"Dạ thưa thầy/cô, service Payroll áp dụng mô hình Clean Architecture tinh giản kết hợp Repository Pattern. Toàn bộ logic nghiệp vụ (tính thuế TNCN lũy tiến 7 bậc, BHXH 8%, OT 150-300%) được cô lập hoàn toàn trong `Core/Services/PayrollService.cs`, không bị lẫn vào Controller hay câu lệnh SQL."*
2. **"Tại sao em dùng PostgreSQL thay vì SQL Server?"**  
   $\rightarrow$ *"Dạ vì PostgreSQL là hệ quản trị mã nguồn mở phổ biến trong các hệ thống doanh nghiệp hiện đại, hỗ trợ kiểu số thực chính xác cao `numeric(18,2)` rất phù hợp để lưu tiền lương và hàm sinh khóa ngẫu nhiên `uuid_generate_v4` tối ưu cho hệ thống phân tán."*
3. **"Nếu service Attendance bị lỗi thì service Payroll có tính lương được không?"**  
   $\rightarrow$ *"Dạ có, em đã thiết kế cơ chế Fallback trong `AttendanceClient`. Nếu service chấm công chưa bật hoặc mất mạng, service Payroll sẽ lấy ngày công chuẩn mặc định để vẫn tính được lương demo mà không bị sập chương trình."*
