# 📋 Payroll Service — Tài Liệu Handover

> **Dành cho:** Người mới chưa biết gì về .NET và Microservice  
> **Mục tiêu:** Hiểu kiến trúc Payroll hiện tại, lần theo luồng xây dựng từ yêu cầu đến UI/API/database, và dùng quy trình này làm mẫu để tạo service mới.
> **Cập nhật:** 16/11/2026

## 0. Lộ trình từ con số 0 đến Payroll hiện tại

Hãy đọc theo thứ tự này nếu đây là lần đầu tiếp cận repo:

1. **Hiểu ranh giới nghiệp vụ:** Payroll sở hữu kỳ lương, bản ghi lương, khoản cộng/trừ và trạng thái thanh toán; HR sở hữu thông tin nhân viên; Attendance sở hữu công; Project sở hữu timesheet dự án.
2. **Thiết kế dữ liệu Payroll:** bắt đầu từ `PayrollPeriod`, `Payroll`, `Allowance`, `PayCut` và quan hệ của chúng. Không lưu trùng dữ liệu sở hữu bởi service khác; chỉ lưu snapshot cần cho kỳ lương.
3. **Chia lớp:** Domain giữ entity; Application giữ DTO, interface và nghiệp vụ; Infrastructure hiện thực EF Core/HTTP clients; API nhận HTTP request và gọi Application.
4. **Tạo nghiệp vụ theo luồng:** mở kỳ → tạo payroll từng nhân viên → đồng bộ công và lương → tính → duyệt → chi trả → xuất phiếu.
5. **Kết nối service ngoài qua HTTP:** khai báo interface/response DTO ở Application, tạo typed client ở Infrastructure, cấu hình BaseUrl rồi inject client vào nghiệp vụ.
6. **Lưu schema bằng migration:** thay đổi entity → cập nhật `PayrollDbContext` → sinh migration → build → khi Payroll API khởi động, `Database.MigrateAsync()` áp dụng migration.
7. **Kết nối UI:** WinForms gọi Payroll API qua `PayrollApiService`; UI không gọi database trực tiếp. Khi thêm field mới, đồng bộ entity → response DTO → WinForms DTO → màn hình.
8. **Kiểm chứng:** chạy unit tests, build API/UI, sau đó kiểm tra API phụ thuộc đang chạy đúng URL.

Luồng đồng bộ lương hiện tại là ví dụ đầy đủ của các lớp trên:

```text
Payroll UI: "Đồng Bộ Mức Lương"
  → FE/Admin.WinForms/Services/Payroll/PayrollApiService
  → POST /api/payroll-salary/periods/{periodId}/sync
  → PayrollSalarySyncController
  → PayrollSalarySyncService
      ├─ HR API: lấy BaseSalary/HourlyRate và loại nhân viên
      └─ Project API: GET /api/PayrollData/summary theo tháng
           → cộng TotalProjectIncome theo EmployeeId
           → lưu Payroll.TotalProjectAllowance

Nút "Tính Lương"
  → PayrollCalculationService
  → tự thêm phụ cấp thâm niên từ danh mục nếu đủ 1 năm làm việc
  → tự thêm khấu trừ theo số lần đi trễ trong kỳ công đã khóa
  → TotalAllowance = phụ cấp active + TotalProjectAllowance
  → tính GrossSalary / NetSalary
```

Để tự tính phụ cấp thâm niên, cần có danh mục active tên **Phụ cấp thâm niên** với kiểu `fixed`; mức `DefaultRate` là khoản phụ cấp tháng. Nhân viên đủ ít nhất một năm tính đến ngày cuối kỳ mới được áp dụng. Khấu trừ đi trễ lấy `DefaultRate` của danh mục active **Phạt đi muộn / về sớm** cho mỗi lần đi trễ. Nếu có đi trễ mà thiếu danh mục hoặc bảng công chưa khóa, tính lương báo lỗi thay vì bỏ qua khoản khấu trừ.

Project API local mặc định là `http://localhost:5032/`; Payroll API là `http://localhost:5267/` (HTTPS `https://localhost:7043/`). Xem các phần 7, 8, 9 và 10 để theo dõi chi tiết runtime, nghiệp vụ, routes và cách chạy.

---

## Mục lục

0. [Lộ trình từ con số 0 đến Payroll hiện tại](#0-lộ-trình-từ-con-số-0-đến-payroll-hiện-tại)
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
13. [Tạo một microservice mới từ con số 0](#13-tạo-một-microservice-mới-từ-con-số-0)

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
5. Đồng bộ mức lương từ HR và phụ cấp dự án từ Project Service
6. Tính lương (gross salary, net salary)
7. Phê duyệt lương
8. Đánh dấu đã thanh toán
9. Xuất phiếu lương (payslip)
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
│   ├── PayrollSalarySyncController.cs       → Đồng bộ lương HR và phụ cấp Project
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
│   ├── ProjectPayroll/                      → DTO nhận tổng hợp phụ cấp từ Project API
│   └── Payslip/
├── Interfaces/
│   ├── IApplicationDbContext.cs            → Interface cho DbContext
│   ├── IAttendanceServiceClient.cs         → Interface gọi Attendance Service
│   ├── IHRServiceClient.cs                 → Interface gọi HR Service
│   └── IProjectServiceClient.cs            → Interface gọi Project Service
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
│   ├── PayrollSalarySync/                  → Đồng bộ lương HR và phụ cấp Project
│   └── Payslip/
└── DependencyInjection.cs                 → Đăng ký services vào DI container
```

### Chi tiết thư mục Payroll.Domain

```
Payroll.Domain/
└── Entities/
    ├── Payroll.cs          → Bản ghi lương, gồm TotalProjectAllowance
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
│   ├── AttendanceServiceClient.cs → Gọi API của Attendance Service
│   ├── HRServiceClient.cs         → Lấy thông tin lương nhân viên từ HR
│   └── ProjectServiceClient.cs    → Lấy tổng timesheet/phụ cấp dự án
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
      │  BaseSalary, HourlyRate, WorkingDays, StandardWorkingDay
      │  TotalWorkingHours, OTHours, OTMultiplier, OTPay
      │  TotalProjectAllowance (snapshot từ Project API)
      │  TotalAllowance (tổng phụ cấp ở lần tính gần nhất), TotalPayCut
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

OT thực tế được tính riêng từ `OTHours × OTMultiplier` trong công thức lương; không cộng từ AllowanceDetail.

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
FULL_TIME:
  ProratedSalary = BaseSalary × WorkingDays / StandardWorkingDay
  OT hourly rate = BaseSalary / (StandardWorkingDay × 8)

PART_TIME:
  ProratedSalary = HourlyRate × TotalWorkingHours
  OT hourly rate = HourlyRate

OTPay          = OT hourly rate × OTHours × OTMultiplier
TotalAllowance = tổng AllowanceDetail active (gồm phụ cấp thâm niên tự động) + TotalProjectAllowance
TotalPayCut    = tổng PayCutDetail active (gồm khấu trừ đi trễ tự động)
GrossSalary    = ProratedSalary + OTPay + TotalAllowance
NetSalary      = GrossSalary - TotalPayCut
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

```powershell
# Chạy tại src\Payroll
dotnet ef migrations add InitialPayroll --project .\Payroll.Infrastructure --startup-project .\Payroll.API
dotnet ef database update --project .\Payroll.Infrastructure --startup-project .\Payroll.API
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

    // Đăng ký typed HTTP clients sau khi validate BaseUrl trong cấu hình.
    // Payroll hiện đăng ký Attendance, HR và Project clients tại đây.
    services.AddHttpClient<IAttendanceServiceClient, AttendanceServiceClient>(client =>
    {
        client.BaseAddress = attendanceUri;
        client.Timeout = TimeSpan.FromSeconds(30);
    });
    services.AddHttpClient<IHRServiceClient, HRServiceClient>(client =>
    {
        client.BaseAddress = hrUri;
        client.Timeout = TimeSpan.FromSeconds(30);
    });
    services.AddHttpClient<IProjectServiceClient, ProjectServiceClient>(client =>
    {
        client.BaseAddress = projectUri;
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    return services;
}
```

Đây là đoạn rút gọn để minh họa đăng ký typed clients; cấu hình thật đọc và validate từng BaseUrl trước khi gọi `AddHttpClient`.

### Bước 9: Cấu hình Program.cs (Entry Point)

```csharp
// Program.cs còn gọi FindEnvFile() và Env.Load(...) trước khi tạo builder.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PayrollDbContext>();
    await context.Database.MigrateAsync();
    await PayrollDataSeeder.SeedAsync(context);
}

app.MapControllers();
app.Run();
```

### Bước 10: Tích hợp một service bên ngoài

Payroll không đọc database của Project trực tiếp. Luồng chuẩn là HTTP client:

1. Tạo response DTO chỉ chứa dữ liệu cần dùng trong `Application/DTOs/`.
2. Tạo interface trong `Application/Interfaces/`, ví dụ `IProjectServiceClient`.
3. Implement interface bằng `HttpClient` trong `Infrastructure/Services/`.
4. Đăng ký typed client và kiểm tra BaseUrl trong Infrastructure DI.
5. Đưa interface vào constructor của service nghiệp vụ; truyền `CancellationToken`, kiểm tra lỗi HTTP và dữ liệu trả về.
6. Ghi BaseUrl vào `appsettings.Development.json` cho local; môi trường khác dùng biến môi trường.

Ví dụ hiện tại: [ProjectServiceClient](../src/Payroll/Payroll.Infrastructure/Services/ProjectServiceClient.cs), [IProjectServiceClient](../src/Payroll/Payroll.Application/Interfaces/IProjectServiceClient.cs), [PayrollSalarySyncService](../src/Payroll/Payroll.Application/Services/PayrollSalarySync/PayrollSalarySyncService.cs). Client gọi Project API để lấy tổng theo nhân viên, sau đó service lưu snapshot `TotalProjectAllowance` trong payroll của kỳ. Nếu API lỗi thì đồng bộ thất bại rõ ràng, không tự coi khoản phụ cấp là 0.

### Bước 11: Kiểm thử, nối UI và xác minh end-to-end

- Unit test nghiệp vụ thuần (ví dụ công thức tính) trong `tests/Payroll.Application.Tests/`.
- Với field mới, cập nhật Entity → mapping response DTO → WinForms DTO → cột/phiếu lương; kiểm tra UI không tự tính lại nghiệp vụ thuộc backend.
- Build cả API và UI, chạy test, rồi mới thử request tới API phụ thuộc.
- Kiểm tra API phụ thuộc bằng đúng URL trong cấu hình; không chạy đồng bộ thật nếu chưa xác nhận database và kỳ lương thử nghiệm.

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
    ├── 5. Áp dụng migration, sau đó chạy Database Seeder
    │       ├── Database.MigrateAsync() → cập nhật schema
    │       └── Tạo AllowanceTypes và PayCutReasons mặc định nếu chưa có
    │
    └── 6. app.Run() → lắng nghe HTTP requests
            ├── http://localhost:5267
            └── https://localhost:7043
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
   (các khoản lương và chấm công được điền ở bước đồng bộ)

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

BƯỚC 5: Đồng Bộ Lương HR và Phụ Cấp Dự Án
POST /api/payroll-salary/periods/{payrollPeriodID}/sync
→ Lấy BaseSalary/HourlyRate và loại nhân viên từ HR API
→ Lấy tổng thu nhập timesheet đã duyệt trong kỳ từ Project API
→ Lưu TotalProjectAllowance vào Payroll theo từng EmployeeID
   ⚠️ Hai API HR và Project phải hoạt động; lỗi được trả về để tránh lưu đồng bộ dở dang.

        │
        ▼

BƯỚC 6: Tính Lương
POST /api/payroll-calculation/{payrollID}/calculate   (theo từng nhân viên)
→ Lấy Payroll record cùng Allowances, PayCuts và TotalProjectAllowance
→ Yêu cầu bảng công tương ứng đã khóa; tự tạo detail phụ cấp thâm niên đủ điều kiện
   và detail khấu trừ theo từng lần đi trễ (mức lấy từ danh mục active)
→ TotalAllowance = tổng AllowanceDetail active + TotalProjectAllowance
→ Tổng hợp TotalPayCut từ các PayCutDetail active
→ Tính ProratedSalary, OTPay, GrossSalary và NetSalary theo loại nhân viên
   (xem công thức ở mục 5.3)
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
| GET | `/api/payroll-periods` | Danh sách kỳ lương |
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

### Payroll Salary Sync (Đồng bộ lương HR và phụ cấp Project)

| Method | URL | Mô tả |
|--------|-----|-------|
| POST | `/api/payroll-salary/periods/{payrollPeriodID}/sync` | Lấy lương từ HR và tổng phụ cấp dự án từ Project |

### Payroll Calculation (Tính lương)

| Method | URL | Mô tả |
|--------|-----|-------|
| POST | `/api/payroll-calculation/{payrollID}/calculate` | Tính lương cho 1 nhân viên |

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

OpenAPI JSON (chỉ bật trong Development): `GET /openapi/v1.json`.

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

```powershell
git clone <repository-url>
Set-Location HRM
```

### Bước 2: Tạo file .env

Tạo file `.env` ở thư mục gốc (copy từ `.env.example`):

```powershell
Copy-Item .env.example .env
```

Chỉnh sửa `.env` với thông tin database của bạn:

```env
ConnectionStrings__PayrollConnection="Host=localhost;Port=5432;Database=Payroll;Username=postgres;Password=YOUR_PASSWORD"
ConnectionStrings__ProjectConnection="Host=localhost;Port=5432;Database=YOUR_PROJECT_DATABASE;Username=postgres;Password=YOUR_PASSWORD"
AttendanceService__BaseUrl="http://localhost:5231/"
HRService__BaseUrl="http://localhost:5012/"
ProjectService__BaseUrl="http://localhost:5032/"
```

> **Lưu ý:** Thay `YOUR_PASSWORD` bằng mật khẩu PostgreSQL thực tế của bạn. Ba URL service phía trên đã có mặc định trong `appsettings.Development.json`; chỉ cần khai báo trong `.env` khi muốn ghi đè.
> Project API cần một database có các bảng timesheet/dự án và phải dùng `ConnectionStrings__ProjectConnection` riêng; không dùng database Payroll nếu database đó chưa có các bảng của Project API. `DB_HOST`, `DB_PORT`, `DB_NAME` và `DB_USER` vẫn được hỗ trợ cho cấu hình cũ.

### Bước 3: Tạo database PostgreSQL

```sql
-- Mở psql hoặc pgAdmin và chạy:
CREATE DATABASE "Payroll";
```

### Bước 4: Chạy Migration (Tạo bảng trong database)

```bash
# Di chuyển vào thư mục Payroll.API
Set-Location src\Payroll\Payroll.API

# Chạy migration
dotnet ef database update --project ..\Payroll.Infrastructure
```

> **Nếu chưa cài EF CLI:**
> ```bash
> dotnet tool install --global dotnet-ef
> ```

### Bước 5: Khởi động Payroll Service

```bash
# Vẫn trong thư mục src\Payroll\Payroll.API
dotnet run
```

Bạn sẽ thấy:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5267
      Now listening on: https://localhost:7043
```

### Bước 6: Kiểm tra hoạt động

Mở trình duyệt hoặc dùng `curl`:

```bash
# Kiểm tra kết nối database
curl.exe http://localhost:5267/health/database

# Lấy tài liệu OpenAPI (JSON)
curl.exe https://localhost:7043/openapi/v1.json
```

### Bước 7: Test tạo kỳ lương đầu tiên

```bash
curl.exe -X POST http://localhost:5267/api/payroll-periods `
  -H "Content-Type: application/json" `
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
curl.exe "http://localhost:5267/api/payroll-reports/departments/{departmentID}?periodID={periodID}"
```

---

## 12. Các vấn đề thường gặp

### ❌ Lỗi: "Cannot find .env in the current directory or its parent directories"

**Nguyên nhân:** API tìm `.env` từ current directory và thư mục chứa ứng dụng, đi ngược lên các thư mục cha nhưng không tìm thấy.
**Giải pháp:** Tạo `.env` trong thư mục gốc repository (copy `.env.example`) hoặc kiểm tra thư mục làm việc của tiến trình.

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

**Nguyên nhân:** Thiếu cấu hình URL của Attendance Service.
**Giải pháp:** Mặc định Development là `http://localhost:5231/`; nếu service dùng cổng khác, cập nhật `AttendanceService__BaseUrl` trong `.env` hoặc `AttendanceService:BaseUrl` trong appsettings tương ứng.

### ❌ Lỗi đồng bộ lương: HR API hoặc Project API không truy cập được

**Nguyên nhân:** Payroll cần cả HR API để lấy lương/loại nhân viên và Project API để lấy tổng timesheet đã duyệt. Mặc định Development: HR `http://localhost:5012/`, Project `http://localhost:5032/`.
**Giải pháp:** Khởi động đúng các API, kiểm tra `HRService:BaseUrl` và `ProjectService:BaseUrl`, rồi thử đồng bộ lại. Lỗi phải được xử lý trước khi người dùng tính lương; không thay BaseUrl bằng URL giả hoặc xem lỗi là phụ cấp bằng 0.

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
GET /api/payroll-periods
```

### ❌ Lỗi migration: "relation already exists"

**Nguyên nhân:** Schema thực tế không khớp lịch sử migration.
**Giải pháp:** Kiểm tra log, danh sách migration và bảng `__EFMigrationsHistory`; sao lưu dữ liệu trước khi sửa schema. Không xóa database dùng chung hoặc có dữ liệu cần giữ chỉ để xử lý lỗi này.

```powershell
Set-Location src\Payroll\Payroll.API
dotnet ef migrations list --project ..\Payroll.Infrastructure
```

---

## 13. Tạo một microservice mới từ con số 0

Phần này là quy trình tham khảo để tạo service mới theo cấu trúc Payroll. Hãy thay `NewService` bằng tên nghiệp vụ phù hợp; không sao chép nguyên các entity hay công thức của Payroll.

### Bước 1: Xác định ranh giới và hợp đồng

Trước khi tạo project, ghi rõ:

- Service mới sở hữu nghiệp vụ và dữ liệu nào; service nào là nguồn dữ liệu chuẩn.
- Entity, trạng thái và quy tắc nghiệp vụ cốt lõi.
- Ai gọi API, các endpoint cần có, request/response và lỗi nghiệp vụ.
- Service mới có cần gọi service khác không; chỉ lấy dữ liệu qua API, không truy cập database của service khác.
- Quy tắc xác thực/phân quyền, log, dữ liệu nhạy cảm và các trường hợp cần giao dịch/đồng bộ.

Chốt API contract và boundary trước khi triển khai để tránh trùng lặp dữ liệu, vòng phụ thuộc hoặc UI tự quyết định nghiệp vụ.

### Bước 2: Tạo projects theo Clean Architecture

Tạo solution/project dưới `src\NewService\` theo convention của repository:

Chạy từ thư mục gốc repository; các template sẽ dùng SDK/TargetFramework đang cài, nên đối chiếu `.csproj` hiện có trước khi giữ kết quả:

```powershell
dotnet new classlib -n NewService.Domain -o src\NewService\NewService.Domain
dotnet new classlib -n NewService.Application -o src\NewService\NewService.Application
dotnet new classlib -n NewService.Infrastructure -o src\NewService\NewService.Infrastructure
dotnet new webapi -n NewService.API -o src\NewService\NewService.API --use-controllers
dotnet new xunit -n NewService.Tests -o tests\NewService.Tests

dotnet sln .\HRM.slnx add `
  .\src\NewService\NewService.Domain\NewService.Domain.csproj `
  .\src\NewService\NewService.Application\NewService.Application.csproj `
  .\src\NewService\NewService.Infrastructure\NewService.Infrastructure.csproj `
  .\src\NewService\NewService.API\NewService.API.csproj `
  .\tests\NewService.Tests\NewService.Tests.csproj

dotnet add .\src\NewService\NewService.Application\NewService.Application.csproj reference `
  .\src\NewService\NewService.Domain\NewService.Domain.csproj
dotnet add .\src\NewService\NewService.Infrastructure\NewService.Infrastructure.csproj reference `
  .\src\NewService\NewService.Application\NewService.Application.csproj `
  .\src\NewService\NewService.Domain\NewService.Domain.csproj
dotnet add .\src\NewService\NewService.API\NewService.API.csproj reference `
  .\src\NewService\NewService.Application\NewService.Application.csproj `
  .\src\NewService\NewService.Infrastructure\NewService.Infrastructure.csproj
dotnet add .\tests\NewService.Tests\NewService.Tests.csproj reference `
  .\src\NewService\NewService.Application\NewService.Application.csproj
```

Sau khi scaffolding, điều chỉnh namespace, TargetFramework, nullable settings và packages theo các project hiện có; thêm EF Core/provider phù hợp cho Infrastructure và Design-time package cho migration. Không tham chiếu Payroll.Application để tái sử dụng code nghiệp vụ giữa các service.

```text
NewService.Domain          Entity, enum, quy tắc thuần
NewService.Application     DTO, interface, use-case/service nghiệp vụ
NewService.Infrastructure  DbContext, EF mapping/migration, repository và typed HTTP clients
NewService.API             Controllers, cấu hình DI, middleware và entry point
```

Tạo test project riêng dưới `tests\`. Dùng cùng TargetFramework, nullable settings, package versions và naming convention của các service hiện có; kiểm tra `global.json`, `.csproj` và cấu hình chung trước khi scaffolding. Giữ chiều phụ thuộc: Application → Domain; Infrastructure → Application/Domain; API → Application/Infrastructure. Domain/Application không phụ thuộc ASP.NET, EF Core hay UI.

### Bước 3: Xây dựng Domain và schema

1. Tạo entity nhỏ nhất đáp ứng nghiệp vụ; khai báo khóa, quan hệ, giá trị tiền và trạng thái rõ ràng.
2. Tạo `IApplicationDbContext` ở Application nếu service dùng cùng pattern.
3. Implement `DbContext`/Fluent configuration ở Infrastructure; cấu hình precision cho decimal, khóa và index.
4. Đăng ký DbContext và các dịch vụ Infrastructure trong extension DI.
5. Sinh migration, xem lại migration để xác nhận không xóa/đổi dữ liệu ngoài dự kiến, rồi build.
6. Cấu hình chuỗi kết nối qua biến môi trường hoặc secret store; không ghi mật khẩu thật vào source control.

Mỗi microservice nên quản lý database/schema của chính nó. Không thêm foreign key trực tiếp sang database của service khác; dùng ID ngoài như giá trị tham chiếu và lấy thông tin cần thiết qua API.

### Bước 4: Tạo use-case và API

1. Định nghĩa request/response DTO và interface nghiệp vụ trong Application.
2. Implement nghiệp vụ, validation và truy cập dữ liệu; truyền `CancellationToken` qua các thao tác async.
3. Tạo controller mỏng trong API: nhận/validate request, gọi use-case, trả HTTP status phù hợp.
4. Đăng ký Application/Infrastructure qua DI; cấu hình OpenAPI, health check, logging và error handling theo convention hiện có.
5. Dùng HTTP status/error response nhất quán; không trả thành công giả khi dependency thất bại.

### Bước 5: Tích hợp service khác nếu cần

- Tạo interface và response DTO trong Application.
- Tạo typed `HttpClient` ở Infrastructure, cấu hình timeout/BaseUrl và kiểm tra status code/response.
- Khai báo BaseUrl theo environment (`appsettings.Development.json` cho local, environment variables/secrets cho triển khai).
- Xử lý timeout, lỗi HTTP, response rỗng/sai định dạng và hủy request một cách tường minh.
- Viết unit test nghiệp vụ với dependency giả lập; kiểm tra client qua integration test có kiểm soát nếu phù hợp.

Không thêm fallback mặc định làm dữ liệu sai thành công, đặc biệt với tiền, trạng thái phê duyệt và dữ liệu đồng bộ.

### Bước 6: Nối UI và cấu hình chạy

Nếu có WinForms/Web UI, tạo API client riêng trong UI, dùng DTO tương ứng và hiển thị lỗi API rõ ràng. Cập nhật form/control/Designer khi thêm field; không gọi database trực tiếp và không nhân đôi công thức nghiệp vụ ở UI.

Thiết lập connection string, BaseUrl, cổng và môi trường cho local; thêm service vào solution/compose/launch profiles hoặc cấu hình chạy hiện có khi cần. Kiểm tra migration và quy trình khởi động để không có service phụ thuộc bị bỏ quên.

### Bước 7: Kiểm thử và bàn giao

1. Viết unit tests cho quy tắc nghiệp vụ và edge cases; integration tests cho DB/API khi cần.
2. Chạy test project liên quan, build service và UI có thay đổi.
3. Chạy service và xác minh health endpoint, OpenAPI và endpoint chính bằng dữ liệu thử nghiệm.
4. Kiểm tra cấu hình môi trường, migration, log lỗi và khả năng chạy khi dependency không sẵn sàng.
5. Cập nhật tài liệu: boundary, sơ đồ luồng, endpoints, cấu hình, cách chạy/test, migration và troubleshooting.

Checklist hoàn tất: service có owner dữ liệu rõ ràng; migration kiểm tra được; lỗi không bị nuốt; API contract có tài liệu; test/build đạt; cấu hình không chứa secret; UI hiển thị lỗi đúng; có hướng dẫn chạy cho người mới.

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
