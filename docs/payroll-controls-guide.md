# Hướng dẫn các Control Payroll và API

Tài liệu này giải thích các control trong thư mục `FE/Admin.WinForms/Controls/Payroll`: chức năng, hàm xử lý, endpoint được gọi, dữ liệu đi qua các lớp nào và những đoạn code tiêu biểu. Endpoint được ghi theo URL tương đối; ứng dụng ghép với `HttpClient.BaseAddress` của Payroll API (mặc định `http://localhost:5267/`, có thể thay bằng `Services__Payroll__BaseUrl`).

## 1. Luồng tổng quát

```text
Control / Dialog
    ↓ gọi
Payroll service theo chức năng hoặc AttendanceApi / HrApiClient
    ↓ nghiệp vụ
PayrollApiClient (Data/Payroll)
    ↓ HTTP
Payroll.API Controller
    ↓ gọi
Payroll.Application Service
    ↓
IApplicationDbContext + service client HR / Attendance / Project (nếu cần)
```

Ví dụ khi control tải danh sách payroll:

```csharp
// PayrollTableControl
var payrolls = await Program.PayrollManagement.GetPayrollsByPeriodAsync(periodId);
```

```csharp
// PayrollManagementService gọi PayrollApiClient
var payrolls = await _apiClient.GetListAsync<PayrollListDto>(
    $"api/payrolls/periods/{periodId}");

// Service còn lấy directory từ HR, ghép theo EmployeeID,
// sau đó điền EmployeeCode và EmployeeName vào từng DTO.
```

```csharp
// PayrollQueryController
[HttpGet("periods/{periodID:guid}")]
public async Task<ActionResult<List<PayrollListResponse>>> GetByPeriod(
    Guid periodID, CancellationToken cancellationToken)
{
    var result = await _service.GetByPeriodAsync(periodID, cancellationToken);
    return Ok(result);
}
```

Controller chuyển tiếp sang Application Service. Control không truy cập DB Payroll trực tiếp.

## 2. Bảng endpoint chính

| HTTP và endpoint | Được gọi từ FE qua | Controller → Application Service | Xử lý / kết quả |
|---|---|---|---|
| `GET api/payroll-periods` | `GetPeriodsAsync` | `PayrollPeriodController.GetAll` → `PayrollPeriodService.GetAllAsync` | Lấy các kỳ lương, sắp xếp mới trước |
| `POST api/payroll-periods` | `CreatePeriodAsync` | `PayrollPeriodController.Create` → `PayrollPeriodService.CreateAsync` | Body `Year`, `Month`; tạo kỳ, trả `201` |
| `GET api/payrolls/periods/{periodID}` | `GetPayrollsByPeriodAsync` | `PayrollQueryController.GetByPeriod` → `PayrollQueryService.GetByPeriodAsync` | Danh sách payroll trong kỳ |
| `GET api/payrolls/{payrollID}` | `GetPayrollDetailAsync` | `PayrollQueryController.GetByID` → `PayrollQueryService.GetByIDAsync` | Payroll kèm allowance/paycut detail; `404` khi không có |
| `GET api/payrolls/employees/{employeeID}` | Không được service/control FE hiện tại gọi | `PayrollQueryController.GetEmployeeHistory` → `PayrollQueryService.GetEmployeeHistoryAsync` | Lịch sử lương nhân viên |
| `POST api/payroll-attendance/periods/{periodID}/sync` | `SyncAttendanceAsync` | `PayrollAttendanceController.SyncAttendance` → `PayrollAttendanceService.SyncAttendanceAsync` | Lấy công/OT từ Attendance; trả số payroll đã sync |
| `POST api/payroll-salary/periods/{periodID}/sync` | `SyncSalaryAsync` | `PayrollSalarySyncController.SyncSalary` → `PayrollSalarySyncService.SyncSalaryAsync` | Lấy lương từ HR và phụ cấp dự án từ Project |
| `POST api/payroll-management/periods/{periodID}/payrolls` | `CreatePayrollsAsync` | `PayrollManagementController.CreatePayrolls` → `PayrollManagementService.CreatePayrollsAsync` | Body `EmployeeIDs`; tạo payroll còn thiếu, trả `201` |
| `POST api/payroll-calculation/{payrollID}/calculate` | `CalculatePayrollAsync` | `PayrollCalculationController.Calculate` → `PayrollCalculationService.CalculateAsync` | Tính và lưu gross/net |
| `POST api/payroll-approval/{payrollID}/approve` | `ApprovePayrollAsync` | `PayrollApprovalController.Approve` → `PayrollApprovalService.ApproveAsync` | Chuyển `calculated` → `approved` |
| `POST api/payroll-payment/{payrollID}/mark-as-paid` | `MarkAsPaidAsync` | `PayrollPaymentController.MarkAsPaid` → `PayrollPaymentService.MarkAsPaidAsync` | Chuyển `approved` → `paid` |
| `POST api/payroll-period-management/{periodID}/update-status` | `UpdatePeriodStatusAsync` | `PayrollPeriodManagementController.UpdateStatus` → `PayrollPeriodManagementService.UpdateStatusAsync` | Tổng hợp trạng thái kỳ theo trạng thái payroll |
| `POST api/payroll-adjustments/{payrollID}/allowances` | `AddAllowanceAsync` | `PayrollAdjustmentController.AddAllowance` → `PayrollAdjustmentService.AddAllowanceAsync` | Body loại phụ cấp, mô tả, rate, amount; trả `201` |
| `POST api/payroll-adjustments/{payrollID}/paycuts` | `AddPayCutAsync` | `PayrollAdjustmentController.AddPayCut` → `PayrollAdjustmentService.AddPayCutAsync` | Body lý do, mô tả, rate, amount; trả `201` |
| `GET api/payroll-catalog/allowances?activeOnly={bool}` | `GetAllowanceTypesAsync` | `PayrollCatalogController.GetAllowanceTypes` → `PayrollCatalogService.GetAllowanceTypesAsync` | Lấy danh mục phụ cấp |
| `POST api/payroll-catalog/allowances` | `CreateAllowanceTypeAsync` | `PayrollCatalogController.CreateAllowanceType` → `PayrollCatalogService.CreateAllowanceTypeAsync` | Body `SavePayrollCatalogItemRequest`; tạo danh mục |
| `PUT api/payroll-catalog/allowances/{id}` | `UpdateAllowanceTypeAsync` | `PayrollCatalogController.UpdateAllowanceType` → `PayrollCatalogService.UpdateAllowanceTypeAsync` | Cập nhật danh mục/kích hoạt lại |
| `DELETE api/payroll-catalog/allowances/{id}` | `DeactivateAllowanceTypeAsync` | `PayrollCatalogController.DeactivateAllowanceType` → `PayrollCatalogService.DeactivateAllowanceTypeAsync` | Đặt status `inactive`, trả `204` |
| `GET api/payroll-catalog/pay-cuts?activeOnly={bool}` | `GetPayCutReasonsAsync` | `PayrollCatalogController.GetPayCutReasons` → `PayrollCatalogService.GetPayCutReasonsAsync` | Lấy danh mục khấu trừ |
| `POST api/payroll-catalog/pay-cuts` | `CreatePayCutReasonAsync` | `PayrollCatalogController.CreatePayCutReason` → `PayrollCatalogService.CreatePayCutReasonAsync` | Tạo lý do khấu trừ |
| `PUT api/payroll-catalog/pay-cuts/{id}` | `UpdatePayCutReasonAsync` | `PayrollCatalogController.UpdatePayCutReason` → `PayrollCatalogService.UpdatePayCutReasonAsync` | Cập nhật/kích hoạt lại lý do |
| `DELETE api/payroll-catalog/pay-cuts/{id}` | `DeactivatePayCutReasonAsync` | `PayrollCatalogController.DeactivatePayCutReason` → `PayrollCatalogService.DeactivatePayCutReasonAsync` | Đặt status `inactive`, trả `204` |
| `GET api/payslips/{payrollID}` | `GetPayslipAsync` | `PayslipController.GetByPayrollID` → `PayslipService.GetByPayrollIDAsync` | Chỉ trả phiếu khi payroll `approved` hoặc `paid`; nếu chưa phát hành thì `404` |
| `GET api/payslips/periods/{periodID}` | `GetPayslipsByPeriodAsync`, không được `PayslipViewerControl` hiện tại gọi | `PayslipController.GetByPeriod` → `PayslipService.GetByPeriodAsync` | Lấy các phiếu đã phát hành trong kỳ |

### 2.1 API ngoài Payroll được dùng

Các API sau không phải endpoint của Payroll API:

| API client | Endpoint | Được dùng để |
|---|---|---|
| `Program.AttendanceApi.GetAllAsync()` | `GET api/Attendance` trên Attendance API | Lấy attendance record để gom EmployeeID khi tạo payroll |
| `Program.AttendanceApi.SearchEmployeesAsync(keyword)` | `GET api/employees?keyword={keyword}&page=1&pageSize=100` trên Attendance API | Tìm mã/tên và ghép kết quả với payroll theo EmployeeID |
| `HrApiClient.AllEmployeesAsync()` | `GET employees?page={n}&pageSize=100` trên HR API, lặp trang đến đủ dữ liệu | Bổ sung mã/tên HR vào danh sách payroll; gửi header `X-Role: HR` |
| `HrApiClient.GetAsync<EmployeeRow>("employees/{id}", ...)` | `GET employees/{employeeID}` trên HR API | Lấy hồ sơ đầy đủ cho popup nhân viên |

`HrApiClient` mặc định dùng `http://localhost:5012/`, hoặc cấu hình `Services__HR__BaseUrl`. Chi tiết base URL của Payroll nằm ở `Program.cs` và `Config/ApiConfig.cs`.

## 3. `PayrollOverviewControl`

File: [PayrollOverviewControl.cs](./PayrollOverviewControl.cs)

Đây là control điều hướng, không tự gọi endpoint:

- Constructor tạo mặc định `PayrollTableControl`.
- `button_Table_Click`, `button_Adjustments_Click`, `button_Approval_Click`, `button_Payslip_Click` tạo control tương ứng.
- `ShowPage(UserControl, Button)` đổi trạng thái màu của nút, dispose control cũ và gắn control mới vào `panel_Payroll`.
- Khi control con được nạp, chính control con mới gọi API.

Ví dụ:

```csharp
private void button_Payslip_Click(object? sender, EventArgs e)
{
    ShowPage(new PayslipViewerControl(), (Button)sender!);
}
```

## 4. `PayrollTableControl` — bảng lương kỳ

File: [PayrollTableControl.cs](./PayrollTableControl.cs)

### Tải và hiển thị

- `LoadPeriodsAsync()` → `GET api/payroll-periods`; nạp kỳ vào ComboBox.
- `LoadPayrollsAsync()` → `GET api/payrolls/periods/{periodID}` qua `GetPayrollsByPeriodAsync`.
- `PayrollManagementService.GetPayrollsByPeriodAsync` sau đó gọi HR lấy directory, ghép theo `EmployeeID` và gán `EmployeeCode`/`EmployeeName`. Nếu không tìm thấy employee trong HR, method ném lỗi thay vì lặng lẽ trả mã khác.
- Control render các cột công, lương, allowance, paycut, gross/net và trạng thái; tính thẻ thống kê từ chính danh sách đã tải.
- `ResetStats`, `FormatStatus`, `FormatEmploymentType` chỉ xử lý UI, không gọi endpoint.

### Tạo kỳ, đồng bộ và tính lương

- `OnNewPeriodAsync()` lấy tháng/năm từ dialog → `POST api/payroll-periods` với `CreatePayrollPeriodDto` → tải lại kỳ.
- `button_SyncAttendance_Click` gọi `OnSyncAttendanceAsync()`: xác nhận người dùng → `POST api/payroll-attendance/periods/{periodID}/sync` → tải lại danh sách.
- `OnSyncSalaryAsync()` → `POST api/payroll-salary/periods/{periodID}/sync` → tải lại danh sách.
- `OnCreatePayrollsAsync()` gọi `GET api/Attendance` để lấy attendance records, gom `EmployeeId` duy nhất, rồi `POST api/payroll-management/periods/{periodID}/payrolls` với `{ EmployeeIDs: [...] }`.
- `OnCalculateAllAsync()` tải list với `includeEmployeeDetails: false` (không cần directory HR cho thao tác tính), sau đó gọi `POST api/payroll-calculation/{payrollID}/calculate` tuần tự cho từng payroll. Lỗi từng nhân viên được gom riêng để báo kết quả một phần.

Đoạn code tạo payroll minh họa việc FE tổng hợp ID rồi gọi endpoint:

```csharp
var attendanceRecords = await Program.AttendanceApi.GetAllAsync();
var employeeIds = attendanceRecords
    .Select(record => record.EmployeeId)
    .Distinct()
    .ToList();

await Program.PayrollManagement.CreatePayrollsAsync(
    _selectedPeriod.PayrollPeriodID,
    employeeIds);
```

### Popup thông tin nhân viên

`ShowEmployeeDetailsAsync(Point)` bắt sự kiện click row, lấy `PayrollListDto` từ `ListViewItem.Tag`, rồi:

1. `GET api/payrolls/{payrollID}` lấy chi tiết payroll/allowance/paycut.
2. `GET employees/{employeeID}` từ HR lấy hồ sơ nhân viên.
3. Truyền hai response vào `PayrollEmployeeDetailsDialog`.

HR lỗi thì dialog vẫn có thể hiển thị chi tiết payroll và thông báo lỗi hồ sơ; lỗi Payroll detail được báo ở control.

```csharp
var detail = await Program.PayrollManagement.GetPayrollDetailAsync(payroll.PayrollID);
using var hr = new HrApiClient();
var employee = await hr.GetAsync<EmployeeRow>(
    $"employees/{payroll.EmployeeID}",
    CancellationToken.None);
using var dialog = new PayrollEmployeeDetailsDialog(
    payroll, detail!, employee, employeeLoadError);
dialog.ShowDialog(FindForm());
```

`PayrollEmployeeDetailsDialog` (`PayrollEmployeeDetailsDialog.cs`) chỉ dựng UI và format dữ liệu được truyền vào; nó không gọi endpoint riêng.

### Xuất CSV

`ExportToCsv()` lấy header và cell hiện có trên `ListView`, escape dấu nháy kép, rồi ghi UTF-8 bằng `File.WriteAllText`. Đây là xử lý local, **không gọi API**.

## 5. `PayrollAdjustmentControl` — điều chỉnh allowance/paycut

File: [PayrollAdjustmentControl.cs](./PayrollAdjustmentControl.cs)

- `LoadPeriodsAsync()` → `GET api/payroll-periods`.
- `LoadEmployeesInPeriodAsync()` → `GET api/payrolls/periods/{periodID}`; danh sách được bổ sung mã/tên HR trong `PayrollManagementService`. Chọn nhân viên đầu tiên rồi nạp detail.
- `FilterEmployeesAsync()`:
  - Không có từ khóa: lọc lại từ `_payrolls` trong bộ nhớ.
  - Có từ khóa: gọi `AttendanceApi.SearchEmployeesAsync`, ghép kết quả với payroll bằng `EmployeeID`, tìm tiếp theo code/name/ID và cập nhật ComboBox.
  - Sau đó gọi `LoadAdjustmentDetailsAsync()`.
- `LoadAdjustmentDetailsAsync()` → `GET api/payrolls/{payrollID}`; render các `Allowances` và `PayCuts` vào hai ListView, tính summary allowance/paycut và lấy project allowance từ detail response.
- `UpdateAdjustmentButtons()` chỉ bật nút thêm nếu trong kỳ còn payroll trạng thái `draft`.
- `OnAddAllowanceAsync()` / `OnAddPayCutAsync()` gọi `AddPayrollAdjustmentAsync(true/false)`.
- `AddPayrollAdjustmentAsync(bool isAllowance)`:
  1. Chọn payroll `draft`.
  2. Tải catalog active: `GET api/payroll-catalog/allowances?activeOnly=true` hoặc `GET api/payroll-catalog/pay-cuts?activeOnly=true`.
  3. Mở `PayrollAdjustmentEditorDialog` để chọn khoản, nhân viên, số tiền/tỷ lệ và mô tả.
  4. Gọi `POST api/payroll-adjustments/{payrollID}/allowances` hoặc `/paycuts` cho từng nhân viên đã chọn.
  5. Báo số thành công/thất bại rồi tải lại detail.
- `PayrollAdjustmentControl` cũng mở `PayrollCatalogDialog` để quản lý danh mục.

Request phụ cấp minh họa:

```csharp
await Program.PayrollManagement.AddAllowanceAsync(payroll.PayrollID,
    new CreateAllowanceDto
    {
        AllowanceTypeID = selectedItem.ID,
        Amount = dialog.GetAmount(payroll),
        RateApplied = dialog.RateApplied,
        Description = dialog.AdjustmentDescription
    });
```

Khi áp dụng nhiều nhân viên, FE gọi endpoint một lần cho mỗi payroll, không phải một request bulk duy nhất.

## 6. `PayrollAdjustmentEditorDialog` — chọn khoản và nhân viên

File: [PayrollAdjustmentEditorDialog.cs](./PayrollAdjustmentEditorDialog.cs)

Dialog không gọi Payroll API trực tiếp; control cha truyền danh sách payroll draft và catalog vào constructor.

- `BuildControls` dựng danh sách employee/catalog, input, checkbox áp dụng toàn bộ và nút xác nhận.
- `SearchEmployeesAsync()` gọi `GET api/employees?keyword=...` qua Attendance API, sau đó lọc trong `_draftPayrolls` theo EmployeeID/code/name.
- `PopulateEmployees` chỉ dựng lại ListView từ dữ liệu local.
- `SelectCatalogItem` cập nhật input theo loại tính.
- `RateApplied`: kiểu `percentage` đổi phần trăm người dùng nhập thành phân số, ví dụ 10% → 0.1; kiểu khác trả 1.
- `GetAmount(payroll)`: percentage = base salary × rate, làm tròn 2 chữ số; kiểu cố định/formula trả số tiền đã nhập. `formula` hiện không chạy expression engine; người dùng nhập số tiền đã tính.
- `ValidateSelection` yêu cầu khoản, nhân viên (trừ khi chọn tất cả) và giá trị lớn hơn 0.
- `SelectedPayrolls` trả danh sách đã chọn hoặc toàn bộ payroll draft khi checkbox bật.

## 7. `PayrollApprovalPaymentControl` — duyệt và chi trả

File: [PayrollApprovalPaymentControl.cs](./PayrollApprovalPaymentControl.cs)

- `LoadPeriodsAsync()` → `GET api/payroll-periods`.
- `LoadPayrollsAsync()` → `GET api/payrolls/periods/{periodID}`; vẽ ListView và đếm các trạng thái.
- `UpdateSelectedActionButtons()` bật:
  - Duyệt một dòng nếu payroll `calculated`.
  - Chi trả một dòng nếu payroll `approved`.
- `OnApproveSelectedAsync()` → `POST api/payroll-approval/{payrollID}/approve`; tải lại list.
- `OnPaySelectedAsync()` → `POST api/payroll-payment/{payrollID}/mark-as-paid`; tải lại list.
- `OnApproveAllAsync()` lặp gọi approve từng payroll; gom lỗi và báo kết quả một phần.
- `OnPayAllAsync()` lặp gọi mark-as-paid cho các payroll `approved`.
- `OnLockPeriodAsync()` → `POST api/payroll-period-management/{periodID}/update-status`; tải lại kỳ.
- `ResetCards`, `FormatStatus` và `UpdateSelectedActionButtons` chỉ xử lý UI.

Chú ý theo code hiện tại: `OnApproveAllAsync()` chọn mọi payroll không phải `approved`/`paid`, nên có thể bao gồm `draft`; endpoint chỉ chấp nhận trạng thái `calculated`. Khi đó request cho payroll draft sẽ lỗi và được liệt kê trong kết quả một phần.

## 8. `PayslipViewerControl` — xem và xuất phiếu lương

File: [PayslipViewerControl.cs](./PayslipViewerControl.cs)

- `LoadPeriodsAsync()` → `GET api/payroll-periods`.
- `LoadEmployeesInPeriodAsync()` → `GET api/payrolls/periods/{periodID}`; `PayrollManagementService` bổ sung HR code/name rồi control nạp ComboBox.
- `LoadPayslipAsync()` → `GET api/payslips/{payrollID}`; response chứa số liệu phiếu và chi tiết allowance/paycut.
  - Payroll chưa `approved` hoặc chưa `paid`: server trả `404`; `GetOptionalAsync` chuyển `404` thành `null`, control hiển thị “Phiếu lương chưa được phát hành”.
  - Phiếu có dữ liệu: cập nhật nhãn và hai ListView.
- `ClearSlip()` xóa dữ liệu UI; `FormatStatus()` đổi trạng thái sang nhãn tiếng Việt.
- `ExportPayslip()` kiểm tra đã có phiếu, mở SaveFileDialog rồi dùng QuestPDF dựng PDF tại máy. **Không gọi export endpoint**.
- `AddPdfSection`, `AddPdfRow`, `AddPdfDetailRow`, `SafeFileName` là helper tạo nội dung/tên file.
- `button_Print` hiện chỉ hiển thị MessageBox thông báo; chưa gọi print service hoặc endpoint máy in.

Ví dụ tải phiếu:

```csharp
_currentPayslip = await Program.Payslips.GetPayslipAsync(
    _selectedPayroll.PayrollID);

if (_currentPayslip == null)
{
    ClearSlip();
    label_SlipTitle.Text = "PHIẾU LƯƠNG CHƯA ĐƯỢC PHÁT HÀNH";
    return;
}
```

Ví dụ kết xuất local:

```csharp
Document.Create(document => document.Page(page =>
{
    page.Size(PageSizes.A4);
    page.Content().Column(column =>
    {
        column.Item().Text("PHIẾU LƯƠNG");
        column.Item().Text($"Net: {_currentPayslip.NetSalary:#,##0 đ}");
    });
})).GeneratePdf(saveDialog.FileName);
```

## 9. `PayrollCatalogDialog` — quản lý loại khoản

File: [PayrollCatalogDialog.cs](./PayrollCatalogDialog.cs)

`PayrollCatalogDialog` tạo hai tab. Mỗi tab có `CatalogListControl(true)` cho allowance hoặc `CatalogListControl(false)` cho paycut.

Trong `CatalogListControl`:

- `ReloadAsync()` gọi GET catalog tương ứng, render ListView và tô xám danh mục inactive.
- `EditAsync(null)` mở form tạo mới → POST endpoint tương ứng.
- `EditAsync(item)` mở form sửa → PUT endpoint tương ứng.
- `ToggleStatusAsync(item)`:
  - Đang active: hỏi xác nhận rồi DELETE (server soft-deactivate, status thành inactive).
  - Đang inactive: PUT lại toàn bộ giá trị với `IsActive = true` để kích hoạt.
- `SelectedItem()` đọc object từ `ListViewItem.Tag`; `UpdateButtons()` bật/tắt nút sửa và trạng thái.
- `FormatCalculationType`, `FormatDefaultRate`, `ToPercent` chỉ format hiển thị.

Trong `CatalogItemEditDialog`:

- `BuildControls` dựng input tên, mô tả, kiểu tính, rate và trạng thái.
- `UpdateRateLabel` cập nhật label và max rate (percentage tối đa 100).
- `Save()` kiểm tra tên không trống rồi dựng `SavePayrollCatalogItemDto`; validate nghiệp vụ đầy đủ vẫn được thực hiện ở server.
- Dialog chỉ trả DTO cho `CatalogListControl`, không gửi HTTP.

Body ví dụ cho POST/PUT catalog:

```json
{
  "name": "Phụ cấp ăn trưa",
  "description": "Khoản phụ cấp theo tháng",
  "calculationType": "fixed",
  "defaultRate": 500000,
  "isActive": true
}
```

## 10. Các lớp FE của Payroll

Payroll nằm trong cùng project `Admin.WinForms`, được phân theo ba lớp:

| Lớp | Vị trí | Trách nhiệm |
|---|---|---|
| Presentation | `Controls/Payroll` và `Controls/Dashboard` | Hiển thị giao diện, nhận thao tác và gọi service phù hợp |
| Business/Services | `Services/Payroll` | Cache list trong vòng đời ứng dụng, phối hợp nghiệp vụ và làm mới dữ liệu sau thao tác ghi |
| Data Access | `Data/Payroll/PayrollApiClient.cs` | Gửi HTTP đến Payroll API, đọc DTO và xử lý phản hồi |

Các service nghiệp vụ được chia theo chức năng: `PayrollPeriodService` (kỳ lương), `PayrollManagementService` (bảng lương và thống kê), `PayrollCatalogService` (danh mục), `PayslipService` (phiếu lương). `Program` tạo một `PayrollApiClient` dùng chung và truyền vào các service; các control gọi service tương ứng thay vì gọi HTTP trực tiếp.

## 11. Những thao tác không gọi endpoint

| Chức năng | Cách xử lý |
|---|---|
| Chuyển tab Payroll | Tạo/dispose UserControl trong `PayrollOverviewControl` |
| Format trạng thái, employment type, tiền | Hàm local trong control |
| Tính các thẻ thống kê | Cộng/đếm dữ liệu đã tải trong bộ nhớ |
| Lọc nhân viên sau directory search | Lọc `_payrolls` local theo ID/code/name |
| Xuất bảng lương CSV | Đọc cell trong ListView và ghi file UTF-8 |
| Xuất phiếu PDF | QuestPDF tạo file trên máy |
| Nút in phiếu | Hiện thông báo; chưa gọi endpoint hay driver in |
| Chọn khoản/nhân viên trong dialog | Lưu selection local rồi trả dữ liệu về control cha |

## 12. Trình tự thao tác bảng lương thường gặp

```text
1. GET api/payroll-periods
2. GET api/Attendance
3. POST api/payroll-management/periods/{id}/payrolls
4. POST api/payroll-attendance/periods/{id}/sync
5. POST api/payroll-salary/periods/{id}/sync
6. (Tùy chọn) POST api/payroll-adjustments/{payrollID}/allowances hoặc /paycuts
7. POST api/payroll-calculation/{payrollID}/calculate
8. POST api/payroll-approval/{payrollID}/approve
9. POST api/payroll-payment/{payrollID}/mark-as-paid
10. POST api/payroll-period-management/{id}/update-status
11. GET api/payslips/{payrollID}
12. Xuất PDF local bằng QuestPDF
```

Mỗi API có quy tắc trạng thái riêng; nếu gọi sai thứ tự server trả lỗi nghiệp vụ. Ví dụ điều chỉnh và tính lương chỉ chấp nhận payroll draft; approve cần calculated; mark-as-paid cần approved; payslip chỉ phát hành khi approved/paid.
