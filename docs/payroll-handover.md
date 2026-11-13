# 📋 Payroll Service — Tài Liệu Handover

> **Dành cho:** Người mới chưa biết gì về .NET và Microservice  
> **Mục tiêu:** Hiểu được luồng xây dựng dự án, luồng chạy chương trình, và cách làm việc với Payroll Service từ đầu đến cuối

---

## Mục lục

1. [Tổng quan hệ thống](#1-tổng-quan-hệ-thống)
2. [Kiến thức nền cần có](#2-kiến-thức-nền-cần-có)
3. [Cấu trúc thư mục dự án](#3-cấu-trúc-thư-mục-dự-án)
4. [Kiến trúc Clean Architecture](#4-kiến-trúc-clean-architecture)
5. [Các Entity (Bảng dữ liệu)](#5-các-entity-bảng-dữ-liệu)
6. [Luồng xây dựng dự án (Build Flow)](#6-luồng-xây-dựng-dự-án-build-flow)
7. [Luồng chạy chương trình (Runtime Flow)](#7-luồng-chạy-chương-trình-runtime-flow)
8. [Luồng nghiệp vụ Payroll (Business Flow)](#8-luồng-nghiệp-vụ-payroll-business-flow)
9. [Danh sách API Endpoints](#9-danh-sách-api-endpoints)
10. [Hướng dẫn setup và chạy dự án](#10-hướng-dẫn-setup-và-chạy-dự-án)
11. [Cách thêm tính năng mới](#11-cách-thêm-tính-năng-mới)
12. [Các vấn đề thường gặp](#12-các-vấn-đề-thường-gặp)

---

## 1. Tổng quan hệ thống

### HRM là gì?

**HRM (Human Resource Management)** là hệ thống quản lý nhân sự gồm nhiều **microservice** hoạt động độc lập:

```
HRM System
├── HR Service         → Quản lý nhân viên, hợp đồng, dự án
├── Attendance Service → Chấm công, bảng chấm công
├── Payroll Service    → Tính lương ← Chúng ta đang làm đây
└── Project Service    → Quản lý dự án
```

### Microservice là gì? (Giải thích đơn giản)

> **Thay vì** làm một ứng dụng khổng lồ chứa hết mọi chức năng, ta **tách ra** thành nhiều ứng dụng nhỏ, mỗi cái lo một việc.

**Ví dụ thực tế:**
- Grabfood = tổng hợp nhiều service: service đặt hàng, service thanh toán, service giao hàng, service nhà hàng...
- Mỗi service chạy độc lập, nói chuyện với nhau qua API (HTTP calls)

### Payroll Service làm gì?

Payroll Service xử lý toàn bộ quy trình **tính và thanh toán lương**:

```
1. Tạo kỳ lương (tháng/năm)
2. Tạo bản ghi lương cho từng nhân viên  
3. Thêm phụ cấp (allowance) và khấu trừ (pay cut)
4. Đồng bộ dữ liệu chấm công từ Attendance Service
5. Tính lương (gross salary, net salary)
6. Phê duyệt lương
7. Đánh dấu đã thanh toán
8. Xuất phiếu lương (payslip)
```

---

## 2. Kiến thức nền cần có

### 2.1 .NET / C# cơ bản

| Khái niệm | Giải thích đơn giản |
|-----------|---------------------|
| **Class** | Bản thiết kế (blueprint) của một đối tượng. Ví dụ: class `Payroll` định nghĩa một bản ghi lương có gì |
| **Interface** | Hợp đồng — định nghĩa _cái gì phải có_ nhưng không viết _làm thế nào_ |
| **async/await** | Làm việc bất đồng bộ — gọi database/API mà không chặn luồng chính |
| **Dependency Injection (DI)** | Thay vì tự tạo đối tượng, ta "nhờ" framework tạo và truyền vào cho mình |
| **GUID** | Mã định danh duy nhất (chuỗi 36 ký tự), dùng thay cho ID số nguyên |

### 2.2 ASP.NET Core Web API

Đây là framework để tạo REST API trong .NET:

```
Client gửi HTTP Request → Controller nhận → Service xử lý → Database lưu → Trả về Response
```

**Controller** = "cửa vào" của API, chỉ nhận request và gọi service  
**Service** = nơi chứa logic nghiệp vụ (business logic)  
**DbContext** = cầu nối giữa code C# và database

### 2.3 Entity Framework Core (EF Core)

ORM (Object-Relational Mapping) — viết code C# thay vì viết SQL:

```csharp
// Thay vì viết SQL:
// SELECT * FROM Payrolls WHERE PayrollPeriodID = '...'

// Viết bằng EF Core:
var payrolls = await _context.Payrolls
    .Where(p => p.PayrollPeriodID == periodID)
    .ToListAsync();
```

### 2.4 PostgreSQL

Database dùng trong dự án này. Mỗi service có **database riêng**:
- `hr_db` → HR Service
- `Payroll` → Payroll Service (database này)

---

## 3. Cấu trúc thư mục dự án

```
HRM/
├── src/
│   ├── Payroll/                    ← Payroll Service (thư mục chính)
│   │   ├── Payroll.API/            ← Lớp 1: Presentation (Controllers, cấu hình app)
│   │   ├── Payroll.Application/    ← Lớp 2: Application (Services, DTOs, Interfaces)
│   │   ├── Payroll.Domain/         ← Lớp 3: Domain (Entities - Bảng DB)
│   │   └── Payroll.Infrastructure/ ← Lớp 4: Infrastructure (DbContext, Migrations, HTTP clients)
│   ├── HR/                         ← HR Service (service khác)
│   ├── Attendance/                 ← Attendance Service (service khác)
│   └── Project/                    ← Project Service (service khác)
├── tests/
│   └── Payroll.Application.Tests/  ← Unit tests cho Payroll
├── docs/                           ← Tài liệu
├── .env                            ← Cấu hình môi trường (connection strings, URLs)
├── .env.example                    ← Mẫu file .env
└── HRM.slnx                        ← Solution file (gom tất cả project)
```

### Chi tiết thư mục Payroll.API

```
Payroll.API/
├── Controllers/
│   ├── DatabaseHealthController.cs         → Kiểm tra kết nối DB
│   ├── PayrollPeriodController.cs          → Tạo kỳ lương
│   ├── PayrollManagementController.cs      → Tạo bản ghi lương cho nhân viên
│   ├── PayrollAdjustmentController.cs      → Thêm phụ cấp / khấu trừ
│   ├── PayrollAttendanceController.cs      → Đồng bộ dữ liệu chấm công
│   ├── PayrollCalculationController.cs     → Tính lương
│   ├── PayrollApprovalController.cs        → Phê duyệt lương
│   ├── PayrollPaymentController.cs         → Đánh dấu đã thanh toán
│   ├── PayrollPeriodManagementController.cs→ Cập nhật trạng thái kỳ lương
│   ├── PayrollQueryController.cs           → Truy vấn / xem lương
│   └── PayslipController.cs               → Phiếu lương
├── Program.cs                              → Entry point, cấu hình app
├── appsettings.json                        → Cấu hình chung
└── appsettings.Development.json           → Cấu hình môi trường dev
```

### Chi tiết thư mục Payroll.Application

```
Payroll.Application/
├── DTOs/                                   → Data Transfer Objects (định dạng request/response)
│   ├── PayrollPeriod/
│   ├── PayrollManagement/
│   ├── PayrollAdjustment/
│   ├── PayrollCalculation/
│   ├── PayrollAttendance/
│   ├── PayrollApproval/
│   ├── PayrollPayment/
│   ├── PayrollPeriodManagement/
│   ├── PayrollQuery/
│   └── Payslip/
├── Interfaces/
│   ├── IApplicationDbContext.cs            → Interface cho DbContext
│   ├── IAttendanceServiceClient.cs         → Interface gọi Attendance Service
│   └── IHRServiceClient.cs                 → Interface gọi HR Service
├── Services/                               → Logic nghiệp vụ
│   ├── PayrollPeriod/
│   ├── PayrollManagement/
│   ├── PayrollAdjustment/
│   ├── PayrollCalculation/
│   ├── PayrollAttendance/
│   ├── PayrollApproval/
│   ├── PayrollPayment/
│   ├── PayrollPeriodManagement/
│   ├── PayrollQuery/
│   └── Payslip/
└── DependencyInjection.cs                 → Đăng ký services vào DI container
```

### Chi tiết thư mục Payroll.Domain

```
Payroll.Domain/
└── Entities/
    ├── Payroll.cs          → Bản ghi lương của 1 nhân viên
    ├── PayrollPeriod.cs    → Kỳ lương (tháng/năm)
    ├── Allowance.cs        → Nhóm phụ cấp
    ├── AllowanceDetail.cs  → Chi tiết từng khoản phụ cấp
    ├── AllowanceType.cs    → Loại phụ cấp (ăn trưa, xăng xe...)
    ├── PayCut.cs           → Nhóm khấu trừ
    ├── PayCutDetail.cs     → Chi tiết từng khoản khấu trừ
    └── PayCutReason.cs     → Lý do khấu trừ (BHXH, thuế TNCN...)
```

### Chi tiết thư mục Payroll.Infrastructure

```
Payroll.Infrastructure/
├── Persistence/
│   ├── PayrollDbContext.cs      → Cấu hình database, quan hệ bảng
│   ├── PayrollDataSeeder.cs     → Tạo dữ liệu mặc định khi app khởi động
│   └── DependencyInjection.cs  → Đăng ký DbContext và HTTP clients
├── Services/
│   └── AttendanceServiceClient.cs → Gọi API của Attendance Service
└── Migrations/                  → Lịch sử thay đổi database schema
```

---

## 4. Kiến trúc Clean Architecture

Dự án dùng **Clean Architecture** — tư tưởng: **lớp trong không phụ thuộc lớp ngoài**.

```
┌─────────────────────────────────────────┐
│           Payroll.API                   │  ← Lớp ngoài cùng
│    (Controllers, Program.cs)            │     Phụ thuộc Application + Infrastructure
├─────────────────────────────────────────┤
│        Payroll.Infrastructure           │  ← Lớp hạ tầng
│  (DbContext, Migrations, HTTP clients)  │     Phụ thuộc Application
├─────────────────────────────────────────┤
│        Payroll.Application              │  ← Lớp ứng dụng
│    (Services, DTOs, Interfaces)         │     Phụ thuộc Domain
├─────────────────────────────────────────┤
│          Payroll.Domain                 │  ← Lớp trong cùng (không phụ thuộc ai)
│            (Entities)                  │
└─────────────────────────────────────────┘
```

**Tại sao làm vậy?**
- Dễ test: có thể test `PayrollCalculationService` mà không cần chạy database thật
- Dễ thay thế: muốn đổi từ PostgreSQL sang SQL Server chỉ cần sửa Infrastructure
- Rõ ràng: nhìn vào thư mục biết ngay mã nằm ở đâu

**Quy tắc phụ thuộc:**
```
API → Application ← Infrastructure
        ↓
      Domain
```
- Application chỉ dùng Interface (`IApplicationDbContext`), không dùng class cụ thể
- Infrastructure implement Interface đó (`PayrollDbContext : IApplicationDbContext`)

---

## 5. Các Entity (Bảng dữ liệu)

### 5.1 Sơ đồ quan hệ

```
PayrollPeriod (Kỳ lương)
│  PayrollPeriodID (PK)
│  Year, Month
│  Status: draft → calculated → approved → paid
│
└─── Payroll (Bản ghi lương từng nhân viên) [1 kỳ có nhiều nhân viên]
      │  PayrollID (PK)
      │  EmployeeID (ID nhân viên từ HR Service)
      │  BaseSalary, WorkingDays, StandardWorkingDay
      │  TotalAllowance, TotalPayCut
      │  GrossSalary, NetSalary
      │  Status: draft → calculated → approved → paid
      │
      ├─── Allowance (Nhóm phụ cấp) [1 payroll có nhiều nhóm]
      │     │  AllowanceID (PK)
      │     │  Status: active / inactive
      │     │
      │     └─── AllowanceDetail (Chi tiết phụ cấp) [1 nhóm có nhiều chi tiết]
      │               AllowanceDetailID (PK)
      │               AllowanceTypeID (FK → AllowanceType)
      │               Description, RateApplied, Amount
      │
      └─── PayCut (Nhóm khấu trừ) [1 payroll có nhiều nhóm]
            │  PayCutID (PK)
            │  Status: active / inactive
            │
            └─── PayCutDetail (Chi tiết khấu trừ) [1 nhóm có nhiều chi tiết]
                      PayCutDetailID (PK)
                      PayCutReasonID (FK → PayCutReason)
                      Description, RateApplied, Amount

AllowanceType (Loại phụ cấp — master data)
│  AllowanceTypeID, Name, Description
│  CalculationType: fixed / percentage / formula
│  DefaultRate, Status

PayCutReason (Lý do khấu trừ — master data)
   PayCutReasonID, Name
   CalculationType: fixed / percentage / formula
   DefaultRate, Status
```

### 5.2 Dữ liệu mặc định (Seed Data)

Khi app khởi động, `PayrollDataSeeder` tự động tạo:

**AllowanceType (Loại phụ cấp):**
| Tên | Loại tính | Giá trị mặc định |
|-----|-----------|-----------------|
| Phụ cấp ăn trưa | fixed | 730,000 đ |
| Phụ cấp xăng xe & đi lại | fixed | 500,000 đ |
| Phụ cấp trách nhiệm | fixed | 1,000,000 đ |
| Làm thêm giờ (OT) | percentage | 1.5x |

**PayCutReason (Lý do khấu trừ):**
| Tên | Loại tính | Tỷ lệ |
|-----|-----------|-------|
| Bảo hiểm xã hội (BHXH 8%) | percentage | 8% |
| Bảo hiểm y tế (BHYT 1.5%) | percentage | 1.5% |
| Bảo hiểm thất nghiệp (BHTN 1%) | percentage | 1% |
| Thuế thu nhập cá nhân (TNCN) | formula | 0 |
| Phạt đi muộn / về sớm | fixed | 100,000 đ |

### 5.3 Công thức tính lương

```
Lương thực tế (Prorated)  = BaseSalary × WorkingDays / StandardWorkingDay
Lương gộp (Gross)         = Prorated + TotalAllowance
Lương thực nhận (Net)     = Gross - TotalPayCut
```

---

## 6. Luồng xây dựng dự án (Build Flow)

> **Giải thích:** Làm thế nào để xây dựng một service từ đầu theo đúng thứ tự.

### Bước 1: Xác định Domain (Entities)

Trả lời câu hỏi: **Hệ thống cần lưu trữ dữ liệu gì?**

```
→ Kỳ lương (PayrollPeriod)
→ Bản ghi lương (Payroll)
→ Phụ cấp (Allowance, AllowanceDetail, AllowanceType)
→ Khấu trừ (PayCut, PayCutDetail, PayCutReason)
```

Tạo các class Entity trong `Payroll.Domain/Entities/`:

```csharp
// Payroll.Domain/Entities/PayrollPeriod.cs
public class PayrollPeriod
{
    public Guid PayrollPeriodID { get; set; } = Guid.NewGuid();
    public int Year { get; set; }
    public int Month { get; set; }
    public string Status { get; set; } = "draft";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Payroll> Payrolls { get; set; } = new();
}
```

### Bước 2: Tạo Interface (Contracts)

Trong `Payroll.Application/Interfaces/`, tạo `IApplicationDbContext.cs`:

```csharp
public interface IApplicationDbContext
{
    DbSet<PayrollPeriod> PayrollPeriods { get; }
    DbSet<Payroll> Payrolls { get; }
    // ... các DbSet khác
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

> **Tại sao cần Interface?**  
> Application layer chỉ biết "cần có database có các bảng này", không quan tâm là PostgreSQL hay SQL Server.

### Bước 3: Tạo DTOs (Request/Response)

DTOs là các class chỉ dùng để **truyền dữ liệu** — không chứa logic.

```csharp
// Payroll.Application/DTOs/PayrollPeriod/CreatePayrollPeriodRequest.cs
public class CreatePayrollPeriodRequest
{
    public int Year { get; set; }
    public int Month { get; set; }
}

// Payroll.Application/DTOs/PayrollPeriod/PayrollPeriodResponse.cs
public class PayrollPeriodResponse
{
    public Guid PayrollPeriodID { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
```

### Bước 4: Tạo Services (Business Logic)

Viết logic nghiệp vụ trong `Payroll.Application/Services/`:

```csharp
// Payroll.Application/Services/PayrollPeriod/PayrollPeriodService.cs
public class PayrollPeriodService
{
    private readonly IApplicationDbContext _context;

    public PayrollPeriodService(IApplicationDbContext context)
    {
        _context = context; // Nhận vào qua Dependency Injection
    }

    public async Task<PayrollPeriodResponse> CreateAsync(
        CreatePayrollPeriodRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validate dữ liệu đầu vào
        // 2. Kiểm tra trùng lặp
        // 3. Tạo entity
        // 4. Lưu vào database
        // 5. Trả về response
    }
}
```

### Bước 5: Implement Infrastructure

Tạo `PayrollDbContext.cs` trong `Payroll.Infrastructure/Persistence/`:

```csharp
public class PayrollDbContext : DbContext, IApplicationDbContext
{
    // Khai báo các bảng
    public DbSet<PayrollPeriod> PayrollPeriods { get; set; } = null!;
    public DbSet<Payroll> Payrolls { get; set; } = null!;
    // ...

    // Cấu hình quan hệ bảng
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PayrollPeriod 1 — N Payroll
        modelBuilder.Entity<Payroll>()
            .HasOne(p => p.PayrollPeriod)
            .WithMany(pp => pp.Payrolls)
            .HasForeignKey(p => p.PayrollPeriodID);
    }
}
```

### Bước 6: Tạo Migration (Tạo schema database)

```bash
# Chạy trong thư mục Payroll.API
dotnet ef migrations add InitialPayroll --project ../Payroll.Infrastructure
dotnet ef database update
```

### Bước 7: Tạo Controllers (API Endpoints)

```csharp
// Payroll.API/Controllers/PayrollPeriodController.cs
[ApiController]
[Route("api/payroll-periods")]
public class PayrollPeriodController : ControllerBase
{
    private readonly PayrollPeriodService _service;

    public PayrollPeriodController(PayrollPeriodService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<PayrollPeriodResponse>> Create(
        CreatePayrollPeriodRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(201, response);
    }
}
```

### Bước 8: Đăng ký Dependency Injection

Trong `Payroll.Application/DependencyInjection.cs`:

```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddScoped<PayrollPeriodService>();
    services.AddScoped<PayrollManagementService>();
    // ... đăng ký tất cả services
    return services;
}
```

Trong `Payroll.Infrastructure/Persistence/DependencyInjection.cs`:

```csharp
public static IServiceCollection AddInfrastructure(
    this IServiceCollection services, IConfiguration configuration)
{
    // Đăng ký DbContext với PostgreSQL
    services.AddDbContext<PayrollDbContext>(options =>
        options.UseNpgsql(connectionString));

    // Nối Interface với Implementation
    services.AddScoped<IApplicationDbContext>(
        provider => provider.GetRequiredService<PayrollDbContext>());

    // Đăng ký HTTP client để gọi Attendance Service
    services.AddHttpClient<IAttendanceServiceClient, AttendanceServiceClient>(
        client => { client.BaseAddress = attendanceUri; });

    return services;
}
```

### Bước 9: Cấu hình Program.cs (Entry Point)

```csharp
// Đọc .env file
Env.Load(envPath);

var builder = WebApplication.CreateBuilder(args);

// Đăng ký các services
builder.Services.AddApplication();        // ← Services từ Application layer
builder.Services.AddInfrastructure(builder.Configuration); // ← DbContext, HTTP clients

builder.Services.AddControllers();
builder.Services.AddOpenApi();            // ← Swagger

var app = builder.Build();

// Chạy seed data khi khởi động
await PayrollDataSeeder.SeedAsync(context);

app.MapControllers();
app.Run();
```

---

## 7. Luồng chạy chương trình (Runtime Flow)

### 7.1 Khởi động ứng dụng

```
dotnet run
    │
    ▼
Program.cs thực thi
    │
    ├── 1. Tìm và load file .env
    │       └── Đọc ConnectionStrings, service URLs
    │
    ├── 2. Khởi tạo WebApplication builder
    │
    ├── 3. Đăng ký services vào DI Container
    │       ├── AddApplication() → đăng ký tất cả Business Services
    │       └── AddInfrastructure() → đăng ký DbContext, HTTP clients
    │
    ├── 4. Build app
    │
    ├── 5. Chạy Database Seeder
    │       └── Tạo AllowanceTypes và PayCutReasons mặc định nếu chưa có
    │
    └── 6. app.Run() → Lắng nghe HTTP requests (mặc định port 5178 hoặc 7073)
```

### 7.2 Luồng xử lý một HTTP Request

Ví dụ: `POST /api/payroll-periods` — Tạo kỳ lương tháng 11/2026

```
Client gửi Request:
POST /api/payroll-periods
Body: { "year": 2026, "month": 11 }
        │
        ▼
[ASP.NET Core Routing]
Khớp với PayrollPeriodController.Create()
        │
        ▼
[PayrollPeriodController.Create()]
Nhận request, gọi _service.CreateAsync(request, cancellationToken)
        │
        ▼
[PayrollPeriodService.CreateAsync()]
    │
    ├── 1. Validate(request)
    │       ├── Kiểm tra Month (1-12)
    │       └── Kiểm tra Year (2000 - năm hiện tại)
    │
    ├── 2. IsDuplicateAsync() 
    │       └── _context.PayrollPeriods.AnyAsync(p => p.Month == 11 && p.Year == 2026)
    │           [EF Core gọi SQL: SELECT EXISTS(...) FROM PayrollPeriods WHERE ...]
    │
    ├── 3. Tạo PayrollPeriodEntity { Month=11, Year=2026, Status="draft" }
    │
    ├── 4. _context.PayrollPeriods.Add(payrollPeriod)
    │
    ├── 5. await _context.SaveChangesAsync()
    │       └── [EF Core gọi SQL: INSERT INTO PayrollPeriods (...) VALUES (...)]
    │
    └── 6. Return PayrollPeriodResponse { PayrollPeriodID, Year, Month, Status, CreatedAt }
        │
        ▼
[Controller] return StatusCode(201, response)
        │
        ▼
Client nhận Response:
HTTP 201 Created
Body: { "payrollPeriodID": "...", "year": 2026, "month": 11, "status": "draft", ... }
```

### 7.3 Luồng Dependency Injection hoạt động

```
Khi Controller được khởi tạo, ASP.NET Core tự động:

PayrollPeriodController cần PayrollPeriodService
    ↓
PayrollPeriodService cần IApplicationDbContext
    ↓
IApplicationDbContext được resolve thành PayrollDbContext
    ↓
PayrollDbContext cần DbContextOptions (connection string từ cấu hình)
    ↓
Tất cả được tạo tự động và truyền vào constructor!
```

---

## 8. Luồng nghiệp vụ Payroll (Business Flow)

Quy trình tính lương đầy đủ theo đúng thứ tự:

```
BƯỚC 1: Tạo Kỳ Lương
POST /api/payroll-periods
Body: { "year": 2026, "month": 11 }
→ Tạo PayrollPeriod với status = "draft"

        │
        ▼

BƯỚC 2: Tạo Bản Ghi Lương Cho Nhân Viên
POST /api/payroll-management/periods/{payrollPeriodID}/payrolls
Body: { "employeeIDs": ["guid1", "guid2", ...] }
→ Tạo Payroll records với status = "draft" cho từng nhân viên
   (BaseSalary = 0, WorkingDays = 0 — sẽ cập nhật ở bước sau)

        │
        ▼

BƯỚC 3: Thêm Phụ Cấp / Khấu Trừ (Tùy chọn, làm trước khi tính lương)
# Thêm phụ cấp
POST /api/payroll-adjustments/{payrollID}/allowances
Body: { "allowanceTypeID": "...", "description": "PC ăn trưa T11", "rateApplied": 1.0, "amount": 730000 }

# Thêm khấu trừ
POST /api/payroll-adjustments/{payrollID}/paycuts
Body: { "payCutReasonID": "...", "description": "BHXH T11", "rateApplied": 0.08, "amount": 800000 }

        │
        ▼

BƯỚC 4: Đồng Bộ Dữ Liệu Chấm Công
POST /api/payroll-attendance/periods/{payrollPeriodID}/sync
→ Gọi API Attendance Service: GET /api/timesheets/employee-summary?employeeId=...&month=11&year=2026
→ Lấy về: ActualWorkingDays, StandardWorkingDays
→ Cập nhật WorkingDays và StandardWorkingDay trong Payroll records
   ⚠️ Yêu cầu: Attendance period phải có status "LOCKED"

        │
        ▼

BƯỚC 5: Nhập Lương Cơ Bản (nếu chưa có)
PUT /api/payroll-period-management/{payrollPeriodID}/update-status
hoặc cập nhật BaseSalary trực tiếp vào từng payroll record

        │
        ▼

BƯỚC 6: Tính Lương
POST /api/payrolls/{payrollID}/calculate   (theo từng nhân viên)
→ Lấy Payroll record (kèm Allowances, PayCuts)
→ Tổng hợp TotalAllowance từ các AllowanceDetail active
→ Tổng hợp TotalPayCut từ các PayCutDetail active
→ Tính:
   ProratedSalary = BaseSalary × WorkingDays / StandardWorkingDay
   GrossSalary    = ProratedSalary + TotalAllowance
   NetSalary      = GrossSalary - TotalPayCut
→ Cập nhật status = "calculated"

        │
        ▼

BƯỚC 7: Phê Duyệt Lương
POST /api/payroll-approval/{payrollID}/approve
→ Kiểm tra status == "calculated"
→ Cập nhật status = "approved", ApprovedAt = now

        │
        ▼

BƯỚC 8: Cập Nhật Trạng Thái Kỳ Lương (khi tất cả nhân viên đã approved)
POST /api/payroll-period-management/{payrollPeriodID}/update-status
→ Kiểm tra tất cả Payroll trong kỳ có cùng status
→ Cập nhật PayrollPeriod.Status tương ứng

        │
        ▼

BƯỚC 9: Đánh Dấu Đã Thanh Toán
POST /api/payroll-payment/{payrollID}/mark-as-paid
→ Kiểm tra status == "approved"
→ Cập nhật status = "paid", PaidAt = now

        │
        ▼

BƯỚC 10: Xem Phiếu Lương
GET /api/payslips/{payrollID}         → Phiếu lương của 1 nhân viên
GET /api/payslips/periods/{periodID}  → Tất cả phiếu lương của kỳ
```

### Sơ đồ trạng thái (State Machine)

```
PayrollPeriod:
draft ──────────────────────────→ calculated ──→ approved ──→ paid
(mới tạo)    (khi tất cả                     (khi tất cả
              payroll = calculated)           payroll = approved)

Payroll (từng nhân viên):
draft ──→ calculated ──→ approved ──→ paid
(mới)   (sau khi tính) (sau phê duyệt) (sau thanh toán)

Quy tắc chuyển trạng thái:
✅ draft        → chỉ draft mới được thêm phụ cấp/khấu trừ
✅ calculated   → chỉ calculated mới được approve
✅ approved     → chỉ approved mới được mark-as-paid
❌ Không thể quay lui (draft ← calculated: không thể)
```

---

## 9. Danh sách API Endpoints

### Health Check

| Method | URL | Mô tả |
|--------|-----|-------|
| GET | `/health/database` | Kiểm tra kết nối database |
| POST | `/health/seed` | Seed dữ liệu mặc định |

### Payroll Period (Kỳ lương)

| Method | URL | Mô tả |
|--------|-----|-------|
| POST | `/api/payroll-periods` | Tạo kỳ lương mới |

### Payroll Management (Quản lý bản ghi lương)

| Method | URL | Mô tả |
|--------|-----|-------|
| POST | `/api/payroll-management/periods/{payrollPeriodID}/payrolls` | Tạo payroll cho nhiều nhân viên |

### Payroll Adjustment (Điều chỉnh lương)

| Method | URL | Mô tả |
|--------|-----|-------|
| POST | `/api/payroll-adjustments/{payrollID}/allowances` | Thêm phụ cấp |
| POST | `/api/payroll-adjustments/{payrollID}/paycuts` | Thêm khấu trừ |

### Payroll Attendance (Đồng bộ chấm công)

| Method | URL | Mô tả |
|--------|-----|-------|
| POST | `/api/payroll-attendance/periods/{payrollPeriodID}/sync` | Đồng bộ dữ liệu chấm công |

### Payroll Calculation (Tính lương)

| Method | URL | Mô tả |
|--------|-----|-------|
| POST | `/api/payrolls/{payrollID}/calculate` | Tính lương cho 1 nhân viên |

### Payroll Approval (Phê duyệt)

| Method | URL | Mô tả |
|--------|-----|-------|
| POST | `/api/payroll-approval/{payrollID}/approve` | Phê duyệt lương |

### Payroll Payment (Thanh toán)

| Method | URL | Mô tả |
|--------|-----|-------|
| POST | `/api/payroll-payment/{payrollID}/mark-as-paid` | Đánh dấu đã thanh toán |

### Payroll Period Management (Quản lý trạng thái kỳ lương)

| Method | URL | Mô tả |
|--------|-----|-------|
| POST | `/api/payroll-period-management/{payrollPeriodID}/update-status` | Cập nhật trạng thái kỳ lương |

### Payroll Query (Truy vấn)

| Method | URL | Mô tả |
|--------|-----|-------|
| GET | `/api/payrolls/periods/{periodID}` | Danh sách lương theo kỳ |
| GET | `/api/payrolls/{payrollID}` | Chi tiết 1 bản ghi lương |
| GET | `/api/payrolls/employees/{employeeID}` | Lịch sử lương của nhân viên |

### Payslip (Phiếu lương)

| Method | URL | Mô tả |
|--------|-----|-------|
| GET | `/api/payslips/{payrollID}` | Phiếu lương của 1 nhân viên |
| GET | `/api/payslips/periods/{periodID}` | Tất cả phiếu lương của kỳ |

---

## 10. Hướng dẫn setup và chạy dự án

### Yêu cầu cài đặt

| Tool | Version | Link tải |
|------|---------|----------|
| .NET SDK | 10.0 | https://dotnet.microsoft.com/download |
| PostgreSQL | 15+ | https://www.postgresql.org/download/ |
| Git | Mới nhất | https://git-scm.com/ |
| IDE | VS Code hoặc Visual Studio | https://code.visualstudio.com/ |

### Bước 1: Clone dự án

```bash
git clone <repository-url>
cd HRM
```

### Bước 2: Tạo file .env

Tạo file `.env` ở thư mục gốc (copy từ `.env.example`):

```bash
cp .env.example .env
```

Chỉnh sửa `.env` với thông tin database của bạn:

```env
ConnectionStrings__PayrollConnection="Host=localhost;Port=5432;Database=Payroll;Username=postgres;Password=YOUR_PASSWORD"
AttendanceService__BaseUrl="http://localhost:5080/"
```

> **Lưu ý:** Thay `YOUR_PASSWORD` bằng mật khẩu PostgreSQL thực tế của bạn.

### Bước 3: Tạo database PostgreSQL

```sql
-- Mở psql hoặc pgAdmin và chạy:
CREATE DATABASE "Payroll";
```

### Bước 4: Chạy Migration (Tạo bảng trong database)

```bash
# Di chuyển vào thư mục Payroll.API
cd src/Payroll/Payroll.API

# Chạy migration
dotnet ef database update --project ../Payroll.Infrastructure
```

> **Nếu chưa cài EF CLI:**
> ```bash
> dotnet tool install --global dotnet-ef
> ```

### Bước 5: Khởi động Payroll Service

```bash
# Vẫn trong thư mục src/Payroll/Payroll.API
dotnet run
```

Bạn sẽ thấy:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5178
      Now listening on: https://localhost:7073
```

### Bước 6: Kiểm tra hoạt động

Mở trình duyệt hoặc dùng `curl`:

```bash
# Kiểm tra kết nối database
curl http://localhost:5178/health/database

# Xem API documentation (Swagger)
# Mở: https://localhost:7073/openapi/v1.json
```

### Bước 7: Test tạo kỳ lương đầu tiên

```bash
curl -X POST http://localhost:5178/api/payroll-periods \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 11}'
```

---

## 11. Cách thêm tính năng mới

> **Ví dụ:** Thêm chức năng "Xuất báo cáo lương theo phòng ban"

### Bước 1: Xác định cần dữ liệu gì

```
Input: DepartmentID (ID phòng ban)
Output: Danh sách { EmployeeID, EmployeeName, NetSalary, Status }
```

### Bước 2: Tạo DTOs

```
Payroll.Application/DTOs/PayrollReport/
├── PayrollDepartmentReportRequest.cs   → { DepartmentID, PayrollPeriodID }
└── PayrollDepartmentReportResponse.cs  → { List<EmployeePayrollSummary> }
```

### Bước 3: Tạo Service

```
Payroll.Application/Services/PayrollReport/
└── PayrollReportService.cs
```

```csharp
public class PayrollReportService
{
    private readonly IApplicationDbContext _context;

    public PayrollReportService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PayrollDepartmentReportResponse>> GetByDepartmentAsync(
        Guid departmentID, Guid periodID, CancellationToken cancellationToken = default)
    {
        // Logic ở đây
    }
}
```

### Bước 4: Đăng ký Service vào DI

Mở `Payroll.Application/DependencyInjection.cs`, thêm:

```csharp
services.AddScoped<PayrollReportService>();
```

### Bước 5: Tạo Controller

```
Payroll.API/Controllers/
└── PayrollReportController.cs
```

```csharp
[ApiController]
[Route("api/payroll-reports")]
public class PayrollReportController : ControllerBase
{
    private readonly PayrollReportService _service;

    public PayrollReportController(PayrollReportService service)
    {
        _service = service;
    }

    [HttpGet("departments/{departmentID}")]
    public async Task<ActionResult<...>> GetByDepartment(
        Guid departmentID,
        [FromQuery] Guid periodID,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByDepartmentAsync(departmentID, periodID, cancellationToken);
        return Ok(result);
    }
}
```

### Bước 6: Test

```bash
curl "http://localhost:5178/api/payroll-reports/departments/{departmentID}?periodID={periodID}"
```

---

## 12. Các vấn đề thường gặp

### ❌ Lỗi: "Cannot find .env in the current directory"

**Nguyên nhân:** Chạy `dotnet run` từ thư mục sai  
**Giải pháp:** Chạy từ thư mục chứa `Payroll.API.csproj` hoặc đảm bảo file `.env` tồn tại ở thư mục gốc dự án

```bash
cd src/Payroll/Payroll.API
dotnet run
```

### ❌ Lỗi: "PayrollConnection connection string is missing"

**Nguyên nhân:** Chưa tạo file `.env` hoặc thiếu dòng `ConnectionStrings__PayrollConnection`  
**Giải pháp:** Kiểm tra file `.env` ở thư mục gốc:

```env
ConnectionStrings__PayrollConnection="Host=localhost;Port=5432;Database=Payroll;Username=postgres;Password=YOUR_PASSWORD"
```

### ❌ Lỗi: "AttendanceService:BaseUrl is missing"

**Nguyên nhân:** Thiếu cấu hình URL của Attendance Service  
**Giải pháp:** Thêm vào `.env`:

```env
AttendanceService__BaseUrl="http://localhost:5080/"
```

### ❌ Lỗi: "Only draft payroll can be calculated"

**Nguyên nhân:** Cố gắng tính lương cho payroll đã được tính rồi  
**Giải pháp:** Kiểm tra trạng thái của payroll trước:

```bash
GET /api/payrolls/{payrollID}
```

Xem field `status`. Nếu không phải `"draft"` thì không thể tính lại.

### ❌ Lỗi khi sync attendance: "Attendance period is not locked"

**Nguyên nhân:** Attendance Service chưa khóa kỳ chấm công  
**Giải pháp:** Yêu cầu team Attendance Service khóa kỳ chấm công trước khi sync

### ❌ Lỗi: "Payroll period already exists"

**Nguyên nhân:** Đã tạo kỳ lương cho tháng/năm này rồi  
**Giải pháp:** Mỗi tháng chỉ tạo 1 kỳ lương. Dùng API query để kiểm tra:

```bash
GET /api/payrolls/periods/{periodID}
```

### ❌ Lỗi migration: "relation already exists"

**Nguyên nhân:** Database đã có bảng nhưng migration chưa được đánh dấu là đã chạy  
**Giải pháp:**

```bash
dotnet ef database drop --project ../Payroll.Infrastructure
dotnet ef database update --project ../Payroll.Infrastructure
```

> ⚠️ **CẢNH BÁO:** Lệnh `database drop` xóa toàn bộ dữ liệu. Chỉ dùng trong môi trường development.

---

## Phụ lục: Giải thích các từ khoá quan trọng

| Từ khóa | Giải thích |
|---------|-----------|
| **Microservice** | Kiến trúc chia ứng dụng thành nhiều service nhỏ, độc lập |
| **REST API** | Giao tiếp qua HTTP với các method GET/POST/PUT/DELETE |
| **DTO** | Data Transfer Object — class chỉ dùng để truyền dữ liệu giữa các lớp |
| **Entity** | Class ánh xạ với bảng database |
| **DbContext** | Cầu nối giữa C# và database |
| **Migration** | Lịch sử thay đổi schema database |
| **DI / IoC** | Dependency Injection — framework tự quản lý việc tạo object |
| **Scoped** | Object được tạo mới mỗi HTTP request |
| **async/await** | Lập trình bất đồng bộ — không block thread khi đợi I/O |
| **GUID** | Globally Unique Identifier — mã định danh 128-bit |
| **Seed Data** | Dữ liệu mặc định được tạo khi app khởi động lần đầu |
| **Clean Architecture** | Kiến trúc phân lớp, lớp trong không phụ thuộc lớp ngoài |
| **EF Core** | Entity Framework Core — ORM để làm việc với database bằng C# |

---

*Tài liệu được tạo: Tháng 11/2026*  
*Phiên bản: 1.0*  
*Liên hệ nếu có thắc mắc: Team Payroll*
