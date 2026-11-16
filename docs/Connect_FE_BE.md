# HANDOVER – HRM WINFORMS FRONTEND & MICROSERVICES API INTEGRATION

## 1. Tổng quan dự án

### 1.1. Mục tiêu

Phát triển ứng dụng HRM Admin bằng C# WinForms, giao tiếp với hệ thống Backend được xây dựng theo kiến trúc Microservices.

Frontend chịu trách nhiệm:

- Xây dựng giao diện quản trị bằng WinForms UserControl.
- Hiển thị dữ liệu từ Backend.
- Gửi request đến các REST API.
- Xử lý thao tác thêm, sửa, xóa, tìm kiếm và thống kê.
- Quản lý trạng thái giao diện và thông báo lỗi.

Backend chịu trách nhiệm:

- Cung cấp REST API cho từng nghiệp vụ.
- Xử lý logic nghiệp vụ.
- Quản lý dữ liệu PostgreSQL.
- Giao tiếp giữa các Microservices.
- Xác thực và phân quyền người dùng.

### 1.2. Công nghệ

| Thành phần        | Công nghệ             |
| ----------------- | --------------------- |
| Frontend          | C# WinForms           |
| Backend           | ASP.NET Core Web API  |
| Framework         | .NET 10               |
| Database          | PostgreSQL            |
| ORM               | Entity Framework Core |
| API communication | HTTP / JSON           |
| Architecture      | Microservices         |

## 2. Cấu trúc Frontend

Đường dẫn:

`D:/C#/HRM/FE/Admin.WinForms`

Cấu trúc đề xuất:

```text
Admin.WinForms/
│
├── Config/
│   ├── ApiConfig.cs
│   └── ServiceEndpoints.cs
│
├── Controls/
│   ├── Attendance/
│   ├── Employee/
│   ├── Leave/
│   ├── Shift/
│   ├── Timesheet/
│   ├── Payroll/
│   └── ...
│
├── DTOs/
│   ├── Attendance/
│   ├── Employee/
│   ├── Leave/
│   ├── Shift/
│   ├── Timesheet/
│   ├── Payroll/
│   └── ...
│
├── Services/
│   ├── Attendance/
│   ├── Employee/
│   ├── Leave/
│   ├── Shift/
│   ├── Timesheet/
│   ├── Payroll/
│   └── ...
│
├── MainForm.cs
├── MainForm.Designer.cs
├── Program.cs
└── Admin.WinForms.csproj
```

Các thư mục nghiệp vụ sẽ được tạo theo từng giai đoạn phát triển.

### 2.1. Trách nhiệm từng tầng

| Thành phần | Trách nhiệm                                 |
| ---------- | ------------------------------------------- |
| Controls   | Giao diện UserControl                       |
| DTOs       | Mô hình dữ liệu request/response            |
| Services   | Gọi API Backend                             |
| Config     | Quản lý địa chỉ và cấu hình API             |
| Program.cs | Khởi tạo ứng dụng và các service dùng chung |
| MainForm   | Form chính và điều hướng giao diện          |

Frontend không cần tạo Controller, DbContext hoặc Repository để truy cập Database Backend.

## 3. Danh sách Microservices Backend

Dựa trên cấu trúc dự án HRM hiện tại, cần tổ chức Frontend theo các nhóm nghiệp vụ sau.

### 3.1. Attendance Service

Đường dẫn Backend:

`D:/C#/HRM/src/Attendance`

Các Controller đã xác định:

| Controller                          | Nghiệp vụ                                  |
| ----------------------------------- | ------------------------------------------ |
| AttendanceController                | Quản lý bản ghi chấm công                  |
| AttendanceExplanationsController    | Giải trình chấm công                       |
| AttendanceRulesController           | Quy tắc chấm công                          |
| AttendanceConfigAuditLogsController | Nhật ký thay đổi cấu hình                  |
| HolidaysController                  | Ngày nghỉ lễ                               |
| LeaveBalancesController             | Số dư phép                                 |
| LeaveRequestsController             | Đơn xin nghỉ phép                          |
| OTRequestsController                | Đơn đăng ký tăng ca                        |
| ShiftAssignmentsController          | Phân ca nhân viên                          |
| TimesheetsController                | Bảng công                                  |
| WorkShiftsController                | Ca làm việc                                |
| OvertimeRulesController             | Quy tắc tăng ca                            |
| EmployeesController                 | Tra cứu nhân viên liên quan đến Attendance |

Cấu trúc Frontend:

```text
Controls/
└── Attendance/

DTOs/
└── Attendance/
    ├── AttendanceRecord/
    ├── AttendanceExplanation/
    ├── AttendanceRule/
    ├── AttendanceConfigAuditLog/
    ├── Holiday/
    ├── LeaveBalance/
    ├── LeaveRequest/
    ├── OTRequest/
    ├── ShiftAssignment/
    ├── Timesheet/
    ├── WorkShift/
    └── OvertimeRule/

Services/
└── Attendance/
    ├── AttendanceApiService.cs
    ├── AttendanceExplanationApiService.cs
    ├── AttendanceRuleApiService.cs
    ├── AttendanceConfigAuditLogApiService.cs
    ├── HolidayApiService.cs
    ├── LeaveBalanceApiService.cs
    ├── LeaveRequestApiService.cs
    ├── OTRequestApiService.cs
    ├── ShiftAssignmentApiService.cs
    ├── TimesheetApiService.cs
    ├── WorkShiftApiService.cs
    └── OvertimeRuleApiService.cs
```

Lưu ý: Các tên trên là cách tổ chức Frontend đề xuất, không phải xác nhận rằng tất cả endpoint đã hoàn thiện.

### 3.2. HR / Employee Service

Nghiệp vụ dự kiến:

- Quản lý hồ sơ nhân viên.
- Quản lý phòng ban.
- Quản lý chức vụ.
- Tra cứu thông tin nhân viên.
- Quản lý thông tin tổ chức.

Cấu trúc đề xuất:

```text
Controls/
└── Employee/

DTOs/
└── Employee/
    ├── EmployeeDto.cs
    ├── CreateEmployeeDto.cs
    ├── UpdateEmployeeDto.cs
    └── DepartmentDto.cs

Services/
└── Employee/
    └── EmployeeApiService.cs
```

Cần kiểm tra Controller và DTO thực tế trong HR.API trước khi triển khai.

### 3.3. Leave Service

Nếu hệ thống tách quản lý nghỉ phép thành Microservice riêng, cần phân biệt với các API nghỉ phép hiện đang nằm trong Attendance.

Nghiệp vụ có thể bao gồm:

- Quản lý đơn nghỉ phép.
- Duyệt đơn nghỉ phép.
- Quản lý loại nghỉ phép.
- Theo dõi số ngày phép.

Cấu trúc đề xuất:

```text
Controls/
└── Leave/

DTOs/
└── Leave/

Services/
└── Leave/
    └── LeaveApiService.cs
```

Không tạo API Service trùng lặp nếu Leave Request và Leave Balance vẫn được quản lý hoàn toàn bởi Attendance Service. Cần xác định ranh giới nghiệp vụ từ Backend trước.

### 3.4. Các Microservices khác

Tiếp tục áp dụng cấu trúc tương tự cho các service đã có hoặc sẽ được xác nhận trong solution:

| Nhóm nghiệp vụ | Frontend Controls | Frontend DTOs  | Frontend Services    |
| -------------- | ----------------- | -------------- | -------------------- |
| Employee       | Employee          | Employee       | EmployeeApiService   |
| Attendance     | Attendance        | Attendance     | AttendanceApiService |
| Leave          | Leave             | Leave          | LeaveApiService      |
| Shift          | Shift             | Shift          | ShiftApiService      |
| Timesheet      | Timesheet         | Timesheet      | TimesheetApiService  |
| Payroll        | Payroll           | Payroll        | PayrollApiService    |
| Authentication | Authentication    | Authentication | AuthApiService       |

Các nhóm Payroll và Authentication là định hướng tổ chức, cần đối chiếu với các project Backend thực tế trước khi triển khai.

## 4. Quản lý URL các Microservices

### 4.1. ApiConfig.cs

Tập trung khai báo URL Backend tại:

`Config/ApiConfig.cs`

Ví dụ:

```csharp
namespace Admin.WinForms.Config;

public static class ApiConfig
{
    public const string AttendanceServiceUrl =
        "http://localhost:5231/";

    public const string PayrollServiceUrl =
        "http://localhost:5267/";
}
```

Không tự đặt port cho các service chưa xác minh.

`Payroll.API` hiện chạy mặc định tại `http://localhost:5267/` theo
`src/Payroll/Payroll.API/Properties/launchSettings.json`. Có thể ghi đè địa chỉ
này bằng biến môi trường `Services__Payroll__BaseUrl` khi chạy WinForms.

### 4.2. ServiceEndpoints.cs

Có thể dùng file này để tập trung khai báo các đường dẫn API khi hệ thống lớn hơn.

Ví dụ:

```csharp
namespace Admin.WinForms.Config;

public static class ServiceEndpoints
{
    public const string Attendance = "api/Attendance";
    public const string AttendanceById = "api/Attendance/{0}";
}
```

Cần duy trì endpoint đúng với `[Route]`, `[HttpGet]`, `[HttpPost]`... trong Backend Controller.

### 4.3. Gửi timesheet theo dữ liệu attendance

Ứng dụng nhân viên tại `FE/Project` sử dụng Project API (`http://localhost:5032/`) để tải nhân viên, dự án, công việc và gửi timesheet; dữ liệu giờ chấm công được đọc từ Attendance API (`http://localhost:5231/`).

- WinForms kiểm tra giờ khai báo với `GET api/Attendance/work-hours?employeeId={id}&date=yyyy-MM-dd`.
- Project API kiểm tra lại cùng dữ liệu attendance trước khi lưu `POST api/Timesheets`.
- Chỉ chấp nhận gửi khi có bản ghi attendance và tổng số giờ khớp chính xác; thiếu hoặc lệch giờ bị từ chối, Attendance API không khả dụng thì không lưu.
- Có thể cấu hình địa chỉ Attendance API của Project API bằng `Services:Attendance:BaseUrl`; mặc định là `http://localhost:5231/`.

### 4.4. API Gateway

Cần thống nhất một phương án:

**Phương án A – Gọi trực tiếp Microservices**

Frontend tạo HttpClient riêng cho từng Microservice.

**Phương án B – Gọi qua API Gateway**

Frontend chỉ sử dụng một địa chỉ Gateway. Gateway chịu trách nhiệm định tuyến đến các Microservices.

Ưu tiên sử dụng Gateway nếu Backend đã cấu hình định tuyến đầy đủ và có cơ chế xác thực phù hợp.

## 5. Quy chuẩn phát triển DTO

DTO Frontend đại diện cho dữ liệu API mà giao diện cần sử dụng.

Ví dụ:

```csharp
public class AttendanceRecordDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly? TimeIn { get; set; }
    public TimeOnly? TimeOut { get; set; }
    public string Status { get; set; } = string.Empty;
}
```

Nguyên tắc:

1. Đọc DTO Backend trước khi tạo DTO Frontend.
2. Kiểu dữ liệu phải tương thích với JSON API.
3. Có thể tạo DTO rút gọn cho từng màn hình.
4. Không cần sao chép tất cả DTO Backend.
5. Tách DTO Request và Response khi cấu trúc dữ liệu khác nhau.
6. Không tự ý thay đổi tên thuộc tính JSON nếu chưa cấu hình mapping tương ứng.

Ví dụ quy ước:

```text
DTOs/
└── Attendance/
    └── AttendanceRecord/
        ├── AttendanceRecordDto.cs
        ├── CreateAttendanceRecordDto.cs
        └── UpdateAttendanceRecordDto.cs
```

## 6. Quy chuẩn phát triển API Service

Mỗi API Service chịu trách nhiệm gọi các endpoint của nhóm nghiệp vụ tương ứng.

Ví dụ:

```text
Services/
└── Attendance/
    ├── AttendanceApiService.cs
    ├── LeaveRequestApiService.cs
    └── OTRequestApiService.cs
```

Các phương thức thường dùng:

| Phương thức  | HTTP Method | Mục đích      |
| ------------ | ----------- | ------------- |
| GetAllAsync  | GET         | Lấy danh sách |
| GetByIdAsync | GET         | Lấy chi tiết  |
| CreateAsync  | POST        | Tạo mới       |
| UpdateAsync  | PUT         | Cập nhật      |
| DeleteAsync  | DELETE      | Xóa           |

Không bắt buộc mỗi Controller Backend phải có một class API Service riêng. Có thể gom các API có liên quan vào cùng một service nếu cấu trúc dễ bảo trì và đúng ranh giới nghiệp vụ.

## 7. Quy chuẩn gọi API từ UserControl

UserControl không gọi HttpClient trực tiếp.

Luồng đúng:

```text
UserControl
    |
    v
API Service
    |
    v
HttpClient
    |
    v
Backend Controller
    |
    v
Backend Service
    |
    v
Database
```

Ví dụ:

```csharp
private async Task LoadAttendanceAsync()
{
    try
    {
        var records = await Program.AttendanceApi.GetAllAsync();

        listView_Attendance.Items.Clear();

        foreach (var record in records)
        {
            var item = new ListViewItem(
                record.Date.ToString("dd/MM/yyyy"));

            item.SubItems.Add(record.EmployeeId.ToString());
            item.SubItems.Add(
                record.TimeIn?.ToString("HH:mm") ?? "--:--");

            item.SubItems.Add(
                record.TimeOut?.ToString("HH:mm") ?? "--:--");

            item.SubItems.Add(record.Status);

            listView_Attendance.Items.Add(item);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }
}
```

Đây là ví dụ cho Attendance. Các nghiệp vụ khác triển khai tương tự theo DTO và API Service tương ứng.

## 8. Quy trình tích hợp một Microservice mới

Khi phát triển một chức năng mới, thực hiện theo thứ tự:

1. Xác định Microservice Backend phụ trách nghiệp vụ.
2. Kiểm tra Controller và các endpoint.
3. Kiểm tra DTO Request và Response.
4. Kiểm tra URL và port chạy Backend.
5. Khai báo URL trong ApiConfig.
6. Tạo DTO Frontend.
7. Tạo API Service.
8. Khởi tạo HttpClient và đăng ký API Service.
9. Gọi API từ UserControl.
10. Hiển thị dữ liệu lên giao diện.
11. Kiểm tra lỗi và trạng thái HTTP.
12. Kiểm thử với dữ liệu thật từ Backend.

## 9. Các yêu cầu kỹ thuật chung

### 9.1. Bất đồng bộ

Sử dụng async/await khi gọi API để tránh làm treo giao diện WinForms.

### 9.2. HttpClient

Không tạo mới HttpClient liên tục trong từng thao tác. Nên quản lý vòng đời thông qua Dependency Injection và IHttpClientFactory khi hệ thống phát triển.

### 9.3. Error Handling

Cần xử lý:

- Không kết nối được Backend.
- Request timeout.
- HTTP 400.
- HTTP 401/403.
- HTTP 404.
- HTTP 500.
- Response không đúng định dạng.

Không sử dụng dữ liệu giả để che giấu lỗi kết nối.

### 9.4. Authentication

Khi Backend yêu cầu đăng nhập:

- Quản lý access token.
- Gắn Bearer token vào request.
- Xử lý token hết hạn.
- Kiểm soát quyền truy cập các chức năng.

### 9.5. Phân trang và tìm kiếm

Ưu tiên xử lý phân trang và lọc tại Backend khi dữ liệu lớn. Frontend chỉ gửi query parameters và hiển thị kết quả.

Không mặc định mọi API đều đã hỗ trợ các chức năng này; phải kiểm tra endpoint trước.

## 10. Kế hoạch triển khai tổng thể

### Giai đoạn 1 – Nền tảng kết nối

- [ ] Hoàn thiện Config.
- [ ] Thiết lập HttpClient.
- [ ] Tạo cấu trúc DTOs.
- [ ] Tạo cấu trúc Services.
- [ ] Xác định cách gọi trực tiếp hoặc qua Gateway.
- [ ] Kiểm thử kết nối Backend.

### Giai đoạn 2 – Attendance

- [ ] GET Attendance.
- [ ] Hiển thị ListView.
- [ ] Tra cứu tên nhân viên.
- [ ] Tìm kiếm và lọc.
- [ ] Phân trang.
- [ ] POST / PUT / DELETE.
- [ ] Approval.
- [ ] Reports.

### Giai đoạn 3 – Employee và các nghiệp vụ liên quan

- [ ] Employee Management.
- [ ] Department Management.
- [ ] Employee Details.
- [ ] Kết nối dữ liệu nhân viên với Attendance.

### Giai đoạn 4 – Các nghiệp vụ HRM khác

- [ ] Leave Management.
- [ ] Shift Management.
- [ ] Timesheet Management.
- [ ] Payroll Management.
- [ ] Các chức năng khác theo Backend thực tế.

### Giai đoạn 5 – Hoàn thiện

- [ ] Authentication.
- [ ] Authorization.
- [ ] API Gateway.
- [ ] Xử lý lỗi tập trung.
- [ ] Logging.
- [ ] Kiểm thử tích hợp.
- [ ] Cấu hình môi trường triển khai.

## 11. Trạng thái bàn giao

### Đã xác định

- Frontend là C# WinForms.
- Giao diện được xây dựng bằng UserControl.
- Có MainForm làm Form chính.
- Đã có cấu trúc Controls/Attendance.
- Backend sử dụng ASP.NET Core Web API và PostgreSQL.
- Attendance Backend đã có các Controller, DTO và Service.
- Attendance API có CRUD endpoint.
- Attendance API hiện được cấu hình tại `http://localhost:5231/`.

### Cần xác minh

- Danh sách đầy đủ các project Microservices trong solution Backend.
- Port thực tế của từng Microservice.
- Controller và endpoint của từng service.
- Quan hệ giữa các nghiệp vụ đang nằm chung trong Attendance và các service độc lập.
- Cấu hình API Gateway.
- Authentication và Authorization.
- Các API hỗ trợ tìm kiếm, lọc và phân trang.

### Mục tiêu cuối cùng

Xây dựng Frontend HRM theo hướng module hóa, trong đó mỗi nhóm nghiệp vụ có UserControl, DTO và API Service phù hợp; tất cả giao tiếp với Backend thông qua REST API.

Ưu tiên tái sử dụng thành phần chung, không sao chép logic Backend sang Frontend, không hardcode dữ liệu và giữ cho cấu trúc có thể mở rộng khi bổ sung Microservice mới.
