# Báo cáo `Payroll.Application.Services`

## 1. Phạm vi và cách đọc

Báo cáo mô tả các lớp trong `src/Payroll/Payroll.Application/Services`. Thư mục có 12 service nghiệp vụ; riêng `PayrollCalculation` còn có `PayrollCalculator` và `PayrollCalculationResult` là lớp tính toán/hỗ trợ, không phải service truy cập dữ liệu.

Các phương thức bất đồng bộ nhận `CancellationToken` thường dùng giá trị mặc định `default`. Token được truyền tiếp vào truy vấn EF Core, lời gọi service ngoài hoặc `SaveChangesAsync` để có thể hủy thao tác.

`IApplicationDbContext` cung cấp các `DbSet` cho kỳ lương, bảng lương, phụ cấp, chi tiết phụ cấp, loại phụ cấp, khoản khấu trừ, chi tiết khấu trừ và lý do khấu trừ. Các service dùng interface này thay vì phụ thuộc trực tiếp vào `DbContext` cụ thể.

## 2. Hợp đồng input/output chính

`CancellationToken` được lược khỏi bảng dưới đây vì chỉ điều khiển việc hủy thao tác, không phải dữ liệu nghiệp vụ.

| Input DTO | Các trường đầu vào |
|---|---|
| `CreatePayrollPeriodRequest` | `Year`, `Month` |
| `CreatePayrollsRequest` | `EmployeeIDs` — danh sách GUID nhân viên |
| `SavePayrollCatalogItemRequest` | `Name`, `Description`, `CalculationType`, `DefaultRate`, `IsActive` |
| `CreateAllowanceRequest` | `AllowanceTypeID`, `Description`, `RateApplied`, `Amount` |
| `CreatePayCutRequest` | `PayCutReasonID`, `Description`, `RateApplied`, `Amount` |

Các phương thức đồng bộ nhận `payrollPeriodID`; tính, duyệt, chi trả và truy vấn detail nhận `payrollID`; lịch sử nhận `employeeID`. Các tham số ID là `Guid`.

| Output DTO / giá trị trả về | Nội dung chính |
|---|---|
| `PayrollPeriodResponse` | ID kỳ, năm, tháng, trạng thái, thời điểm tạo |
| `CreatePayrollsResponse` | ID kỳ, số lượng tạo, ID tạo mới và ID bị bỏ qua |
| `int` từ hai phương thức sync | Số payroll đã đồng bộ |
| `PayrollCatalogItemResponse` | ID, tên, mô tả, kiểu tính, rate mặc định, trạng thái |
| `CreateAllowanceResponse` / `CreatePayCutResponse` | ID detail, ID payroll, ID loại/lý do, mô tả, rate, amount |
| `CalculatePayrollResponse` | ID payroll/nhân viên, loại nhân viên, lương và ngày/giờ công, OT, phụ cấp/khấu trừ, gross/net, trạng thái, thời điểm tính |
| `ApprovePayrollResponse` | ID payroll/nhân viên, trạng thái, thời điểm duyệt |
| `MarkPayrollAsPaidResponse` | ID payroll/nhân viên, net, trạng thái, thời điểm chi trả |
| `UpdatePayrollPeriodStatusResponse` | ID kỳ, năm/tháng, trạng thái, số payroll, thời điểm cập nhật |
| `PayrollListResponse` | ID payroll/nhân viên/kỳ, loại nhân viên, công/lương/phụ cấp/khấu trừ, gross/net, trạng thái và các mốc thời gian |
| `PayrollDetailResponse` | ID và tổng lương/trạng thái của payroll, danh sách allowance/paycut detail gồm ID loại/lý do, tên, mô tả, rate, amount |
| `PayrollHistoryResponse` | ID payroll/kỳ, tháng/năm, tổng lương, trạng thái, thời điểm trả |
| `PayslipResponse` | ID payroll/nhân viên, tháng/năm, lương/công/tổng tiền, trạng thái và thời điểm duyệt/trả; danh sách chi tiết allowance/paycut |

## 3. Tổng quan các service

| Service | Trách nhiệm chính | EF Core tiêu biểu |
|---|---|---|
| [PayrollPeriodService](../src/Payroll/Payroll.Application/Services/PayrollPeriod/PayrollPeriodService.cs) | Tạo và liệt kê kỳ lương | `AnyAsync`, `Include`, `ToListAsync`, `Add`, `SaveChangesAsync` |
| [PayrollManagementService](../src/Payroll/Payroll.Application/Services/PayrollManagement/PayrollManagementService.cs) | Tạo bản ghi bảng lương ban đầu theo kỳ | `FirstOrDefaultAsync`, `Where`, `Select`, `ToListAsync`, `AddRange`, `SaveChangesAsync` |
| [PayrollAttendanceService](../src/Payroll/Payroll.Application/Services/PayrollAttendance/PayrollAttendanceService.cs) | Đồng bộ công và tăng ca từ Attendance | `FirstOrDefaultAsync`, `Where`, `ToListAsync`, `SaveChangesAsync` |
| [PayrollSalarySyncService](../src/Payroll/Payroll.Application/Services/PayrollSalarySync/PayrollSalarySyncService.cs) | Đồng bộ mức lương từ HR và phụ cấp dự án từ Project | `FirstOrDefaultAsync`, `Where`, `ToListAsync`, `SaveChangesAsync` |
| [PayrollCalculationService](../src/Payroll/Payroll.Application/Services/PayrollCalculation/PayrollCalculationService.cs) | Tính lương và lưu kết quả | `Include`, `ThenInclude`, `FirstOrDefaultAsync`, `SaveChangesAsync` |
| [PayrollCalculator](../src/Payroll/Payroll.Application/Services/PayrollCalculation/PayrollCalculator.cs) | Tính các con số lương trong bộ nhớ | Không dùng EF Core |
| [PayrollAdjustmentService](../src/Payroll/Payroll.Application/Services/PayrollAdjustment/PayrollAdjustmentService.cs) | Thêm chi tiết phụ cấp hoặc khấu trừ | `FirstOrDefaultAsync`, `Add`, `SaveChangesAsync` |
| [PayrollApprovalService](../src/Payroll/Payroll.Application/Services/PayrollApproval/PayrollApprovalService.cs) | Phê duyệt một bảng lương đã tính | `FirstOrDefaultAsync`, `SaveChangesAsync` |
| [PayrollPaymentService](../src/Payroll/Payroll.Application/Services/PayrollPayment/PayrollPaymentService.cs) | Đánh dấu bảng lương đã chi trả | `FirstOrDefaultAsync`, `SaveChangesAsync` |
| [PayrollPeriodManagementService](../src/Payroll/Payroll.Application/Services/PayrollPeriodManagement/PayrollPeriodManagementService.cs) | Cập nhật trạng thái tổng hợp của kỳ | `Include`, `FirstOrDefaultAsync`, `SaveChangesAsync` |
| [PayrollCatalogService](../src/Payroll/Payroll.Application/Services/PayrollCatalog/PayrollCatalogService.cs) | CRUD loại phụ cấp và lý do khấu trừ | `AsNoTracking`, `Where`, `OrderBy`, `Select`, `ToListAsync`, `FirstOrDefaultAsync`, `AnyAsync`, `Add`, `SaveChangesAsync` |
| [PayrollQueryService](../src/Payroll/Payroll.Application/Services/PayrollQuery/PayrollQueryService.cs) | Truy vấn danh sách, chi tiết và lịch sử lương | `AsNoTracking`, `Where`, `OrderBy`, `ThenBy`, `Select`, `Include`, `ThenInclude`, `FirstOrDefaultAsync`, `ToListAsync` |
| [PayslipService](../src/Payroll/Payroll.Application/Services/Payslip/PayslipService.cs) | Lấy phiếu lương đã phê duyệt/chi trả | `AsNoTracking`, `Where`, `Select`, `Include`, `ThenInclude`, `FirstOrDefaultAsync`, `ToListAsync` |

Các service được đăng ký scoped trong [DependencyInjection.cs](../src/Payroll/Payroll.Application/DependencyInjection.cs).

## 4. Chi tiết từng service

### 4.1 `PayrollPeriodService`

- `Validate(CreatePayrollPeriodRequest request) -> void`
  Kiểm tra tháng từ 1–12 và năm từ 2000 đến năm UTC hiện tại. Sai điều kiện thì ném `ArgumentOutOfRangeException`. Không truy cập DB.
- `IsDuplicateAsync(CreatePayrollPeriodRequest request, CancellationToken) -> Task<bool>`
  Kiểm tra đã tồn tại kỳ có cùng tháng và năm chưa. Dùng `PayrollPeriods.AnyAsync(...)`.
- `CreateAsync(CreatePayrollPeriodRequest request, CancellationToken) -> Task<PayrollPeriodResponse>`
  Validate request → kiểm tra trùng kỳ → tạo `PayrollPeriod` → thêm vào context → lưu → trả ID, năm, tháng, trạng thái và thời điểm tạo. Ném lỗi nếu kỳ đã tồn tại.
- `GetAllAsync(CancellationToken) -> Task<List<PayrollPeriodResponse>>`
  Đọc tất cả kỳ, nạp quan hệ `Payrolls`, sắp xếp năm/tháng giảm dần, rồi ánh xạ sang response. Trạng thái trả về được tính bởi `GetCurrentStatus`.
- `GetCurrentStatus(PayrollPeriod period) -> string`
  Hàm thuần trong bộ nhớ: kỳ không có payroll thì giữ trạng thái kỳ; nếu tất cả payroll cùng trạng thái thì dùng trạng thái đó; nếu trạng thái lẫn nhau mà có `draft` thì trả `draft`, còn lại trả `in_progress`.

### 4.2 `PayrollManagementService`

- `CreatePayrollsAsync(Guid payrollPeriodID, CreatePayrollsRequest request, CancellationToken) -> Task<CreatePayrollsResponse>`
  Loại GUID rỗng và loại ID trùng → tìm kỳ lương → chỉ cho tạo khi kỳ `draft` → lấy các nhân viên đã có payroll trong kỳ → tạo payroll rỗng cho các ID còn thiếu với các tổng tiền/ngày công ban đầu bằng 0 và trạng thái `draft` → lưu nếu có bản ghi mới.
  Response có kỳ, số lượng tạo, ID được tạo và ID bị bỏ qua vì đã tồn tại. Ném lỗi nếu không còn ID hợp lệ, kỳ không tồn tại hoặc kỳ không ở trạng thái draft. Phần `Distinct`/`Except` sau khi dữ liệu được tải là xử lý trong bộ nhớ.

### 4.3 `PayrollAttendanceService`

- `SyncAttendanceAsync(Guid payrollPeriodID, CancellationToken) -> Task<int>`
  Tìm kỳ và kiểm tra kỳ cùng tất cả payroll đều `draft` → lấy hệ số tăng ca từ Attendance → với từng payroll, lấy summary theo nhân viên/tháng/năm và xác thực đúng nhân viên, đúng kỳ, kỳ công đã khóa, ngày công chuẩn dương và các số giờ/ngày không âm → chỉ sau khi mọi summary hợp lệ mới cập nhật ngày công, giờ làm, OT và hệ số OT → lưu một lần → trả số payroll đã cập nhật.
  Dữ liệu Attendance và hệ số OT đến từ `IAttendanceServiceClient`, không phải EF.

### 4.4 `PayrollSalarySyncService`

- `SyncSalaryAsync(Guid payrollPeriodID, CancellationToken) -> Task<int>`
  Tìm kỳ, xác nhận kỳ và payroll đều `draft` → lần lượt lấy hồ sơ lương của từng nhân viên từ HR → xác thực ID, trạng thái hoạt động và dữ liệu lương tương ứng với `FULL_TIME`/`PART_TIME` → lấy tổng thu nhập dự án của khoảng ngày thuộc kỳ từ Project → cộng theo nhân viên → cập nhật loại hình làm việc, lương cơ bản/giờ và phụ cấp dự án (làm tròn 2 chữ số) → lưu → trả số payroll.
  HR và Project được gọi qua `IHRServiceClient` và `IProjectServiceClient`; EF chỉ đọc/ghi dữ liệu Payroll.

### 4.5 `PayrollCalculationService` và `PayrollCalculator`

- `PayrollCalculationService.CalculateAsync(Guid payrollID, CancellationToken) -> Task<CalculatePayrollResponse>`
  Tìm payroll, kỳ lương và nạp các nhóm phụ cấp/khấu trừ cùng detail → chỉ cho tính khi `draft`, OT đã đồng bộ và bảng công Attendance đã khóa, đúng kỳ → tự thêm một detail phụ cấp cố định nếu có danh mục active `Phụ cấp thâm niên`, nhân viên có ngày bắt đầu làm việc từ HR và đã đủ một năm tính đến cuối kỳ → tự thêm detail khấu trừ bằng `DefaultRate` của danh mục active `Phạt đi muộn / về sớm` nhân số lần đi trễ → cộng amount các detail active → gọi `PayrollCalculator.Calculate` → cập nhật tổng phụ cấp, khấu trừ, OT pay, gross, net, trạng thái `calculated` và thời điểm tính → lưu → trả snapshot kết quả tính. HR cung cấp `EmploymentStartDate` từ WorkHistory; nếu chưa cấu hình danh mục thâm niên thì bỏ qua phụ cấp này.
- `PayrollCalculator.Calculate(...) -> PayrollCalculationResult`
  Đầu vào gồm loại nhân viên, lương tháng hoặc đơn giá giờ, ngày công chuẩn/thực tế, giờ làm, giờ OT, hệ số OT, phụ cấp, khấu trừ và phụ cấp dự án. Hàm kiểm tra số âm/đầu vào không hợp lệ, sau đó:
  - Full-time: lương theo công = lương cơ bản × ngày làm / công chuẩn; giá giờ OT = lương cơ bản / (công chuẩn × 8).
  - Part-time: lương thường = đơn giá giờ × tổng giờ; giá giờ OT bằng đơn giá giờ.
  - OT pay = giá giờ OT × giờ OT × hệ số OT.
  - Tổng phụ cấp cộng thêm phụ cấp dự án; gross = lương theo công + tổng phụ cấp + OT; net = gross − khấu trừ.
  - Các hàm hỗ trợ tính thâm niên theo năm làm việc trọn vẹn và khấu trừ theo số lần đi trễ cùng mức cố định cấu hình.
  Các số tiền được làm tròn 2 chữ số theo `MidpointRounding.AwayFromZero`. Hàm này không dùng EF.
- `PayrollCalculationResult`
  Chứa `ProratedSalary`, `OTPay`, `TotalAllowance`, `TotalPayCut`, `GrossSalary`, `NetSalary`; đây là kiểu kết quả, không có phương thức nghiệp vụ.

### 4.6 `PayrollAdjustmentService`

- `AddAllowanceAsync(Guid payrollID, CreateAllowanceRequest request, CancellationToken) -> Task<CreateAllowanceResponse>`
  Kiểm tra amount dương và rate không âm → tìm payroll và yêu cầu trạng thái `draft` → tìm loại phụ cấp đang active → tìm nhóm phụ cấp active của payroll, nếu chưa có thì tạo nhóm → thêm `AllowanceDetail` → lưu → trả ID detail, payroll, loại phụ cấp, mô tả, rate và amount.
- `AddPayCutAsync(Guid payrollID, CreatePayCutRequest request, CancellationToken) -> Task<CreatePayCutResponse>`
  Quy trình tương tự, nhưng tìm `PayCutReason` active, tạo nhóm `PayCut` khi cần và thêm `PayCutDetail`. Trả ID detail, payroll, lý do, mô tả, rate và amount.

### 4.7 `PayrollApprovalService`

- `ApproveAsync(Guid payrollID, CancellationToken) -> Task<ApprovePayrollResponse>`
  Tìm payroll → yêu cầu payroll tồn tại và trạng thái `calculated` → đổi trạng thái thành `approved`, gán `ApprovedAt` theo UTC → lưu → trả ID payroll, ID nhân viên, trạng thái và thời điểm duyệt.

### 4.8 `PayrollPaymentService`

- `MarkAsPaidAsync(Guid payrollID, CancellationToken) -> Task<MarkPayrollAsPaidResponse>`
  Tìm payroll → yêu cầu trạng thái `approved` → đổi thành `paid`, gán `PaidAt` theo UTC → lưu → trả ID payroll, ID nhân viên, tiền net, trạng thái và thời điểm trả.

### 4.9 `PayrollPeriodManagementService`

- `UpdateStatusAsync(Guid payrollPeriodID, CancellationToken) -> Task<UpdatePayrollPeriodStatusResponse>`
  Tìm kỳ và nạp toàn bộ payroll → không cho cập nhật kỳ rỗng → nếu tất cả payroll `paid`, kỳ thành `paid`; nếu tất cả `approved`, thành `approved`; nếu tất cả `calculated`, thành `calculated`; trường hợp khác bị từ chối → lưu và trả thông tin kỳ, trạng thái, số payroll và thời điểm cập nhật. Việc kiểm tra `.All(...)` thực hiện trên danh sách đã nạp vào bộ nhớ.

### 4.10 `PayrollCatalogService`

Service quản lý hai danh mục: `AllowanceType` và `PayCutReason`.

- `GetAllowanceTypesAsync(bool activeOnly, CancellationToken) -> Task<List<PayrollCatalogItemResponse>>`
  Đọc loại phụ cấp, tùy chọn lọc active, sắp xếp theo tên, chiếu các trường cần trả về.
- `GetPayCutReasonsAsync(bool activeOnly, CancellationToken) -> Task<List<PayrollCatalogItemResponse>>`
  Tương tự với danh sách lý do khấu trừ.
- `CreateAllowanceTypeAsync(SavePayrollCatalogItemRequest, CancellationToken) -> Task<PayrollCatalogItemResponse>`
  Validate → kiểm tra tên không trùng → tạo entity, chuẩn hóa dữ liệu bằng `Apply` → thêm và lưu → map sang response.
- `UpdateAllowanceTypeAsync(Guid id, SavePayrollCatalogItemRequest, CancellationToken) -> Task<PayrollCatalogItemResponse>`
  Validate → tìm entity → kiểm tra tên không trùng với bản ghi khác → cập nhật bằng `Apply` → lưu và trả response.
- `DeactivateAllowanceTypeAsync(Guid id, CancellationToken) -> Task`
  Tìm entity → đặt status `inactive` → lưu.
- `CreatePayCutReasonAsync(...) -> Task<PayrollCatalogItemResponse>`
  Quy trình tạo danh mục tương tự, áp dụng cho lý do khấu trừ.
- `UpdatePayCutReasonAsync(...) -> Task<PayrollCatalogItemResponse>`
  Tìm, kiểm tra tên duy nhất, cập nhật, lưu và trả response.
- `DeactivatePayCutReasonAsync(...) -> Task`
  Tìm lý do → đặt `inactive` → lưu.
- `EnsureUniqueNameAsync(string name, Guid? exceptID, bool isAllowance, CancellationToken) -> Task`
  Chuẩn hóa tên bằng trim/lowercase, chọn DbSet theo loại danh mục, loại trừ `exceptID` khi cập nhật, rồi kiểm tra tồn tại. Tên trùng thì ném `InvalidOperationException`.
- `Validate(SavePayrollCatalogItemRequest) -> void`
  Kiểm tra tên bắt buộc/tối đa 120 ký tự, mô tả tối đa 500 ký tự, kiểu tính thuộc `fixed`, `percentage`, `formula`, rate nằm trong giới hạn và percentage không quá 100%.
- Hai overload `Apply(AllowanceType|PayCutReason, SavePayrollCatalogItemRequest) -> void`
  Trim tên/mô tả, chuyển kiểu tính về lowercase và đặt trạng thái theo `IsActive`.
- Hai overload `Map(AllowanceType|PayCutReason) -> PayrollCatalogItemResponse`
  Ánh xạ entity sang DTO gồm ID, tên, mô tả, kiểu tính, rate mặc định và trạng thái.

### 4.11 `PayrollQueryService`

- `GetByPeriodAsync(Guid payrollPeriodID, CancellationToken) -> Task<List<PayrollListResponse>>`
  Tìm các payroll thuộc kỳ, sắp theo EmployeeID, chiếu sang DTO danh sách và trả list.
- `GetByIDAsync(Guid payrollID, CancellationToken) -> Task<PayrollDetailResponse?>`
  Tìm payroll kèm cây quan hệ phụ cấp/chi tiết/loại phụ cấp và khấu trừ/chi tiết/lý do → nếu không có trả `null` → map tổng lương và chỉ lấy chi tiết thuộc nhóm active.
- `GetEmployeeHistoryAsync(Guid employeeID, CancellationToken) -> Task<List<PayrollHistoryResponse>>`
  Lọc theo nhân viên, sắp kỳ mới trước (năm rồi tháng), chiếu thông tin tổng lương, trạng thái và ngày trả thành lịch sử.

### 4.12 `PayslipService`

- `GetByPayrollIDAsync(Guid payrollID, CancellationToken) -> Task<PayslipResponse?>`
  Đọc payroll cùng kỳ lương, phụ cấp và khấu trừ → trả `null` nếu không tồn tại hoặc chưa `approved`/`paid` → yêu cầu có `ApprovedAt` → trả dữ liệu phiếu lương và chi tiết active. Nếu payroll đã duyệt mà thiếu timestamp thì ném lỗi.
- `GetByPeriodAsync(Guid payrollPeriodID, CancellationToken) -> Task<List<PayslipResponse>>`
  Truy vấn ID payroll trong kỳ → lần lượt gọi `GetByPayrollIDAsync` → chỉ thêm các phiếu được phát hành vào kết quả.

## 5. EF Core được sử dụng như thế nào?

### Các phương thức truy vấn

- `Where(predicate)` — toán tử LINQ trên `IQueryable`; EF thường dịch điều kiện thành SQL `WHERE`. Dùng lọc theo kỳ, trạng thái, ID payroll hoặc ID nhân viên.
- `OrderBy` / `OrderByDescending` / `ThenBy` — LINQ được EF dịch sang `ORDER BY`; `ThenBy` bổ sung khóa sắp xếp phụ.
- `Select(projection)` — chiếu entity thành DTO ở truy vấn; EF chỉ lấy các cột cần thiết nếu projection được thực thi trước khi materialize.
- `AnyAsync(predicate, token)` — chạy truy vấn bất đồng bộ kiểm tra có ít nhất một hàng thỏa điều kiện, thường thành `EXISTS`; phù hợp kiểm tra trùng tên/kỳ mà không tải cả danh sách.
- `FirstOrDefaultAsync(predicate, token)` — lấy hàng đầu tiên thỏa điều kiện hoặc `null` nếu không có; dùng khi cần entity để kiểm tra/cập nhật.
- `ToListAsync(token)` — thực thi truy vấn và materialize kết quả thành `List<T>`. Trước điểm này, các toán tử LINQ trên `IQueryable` thường được ghép thành SQL.

### Nạp quan hệ

- `Include(navigation)` — eager-load navigation property cùng entity gốc trong truy vấn, tránh phải truy cập từng navigation rồi phát sinh truy vấn riêng.
- `ThenInclude(navigation)` — tiếp tục nạp navigation ở tầng kế tiếp. Ví dụ Payroll → Allowances → AllowanceDetails → AllowanceType trong chi tiết payroll/phiếu lương.

### Theo dõi và ghi dữ liệu

- `AsNoTracking()` — tắt change tracking cho truy vấn chỉ đọc; giảm overhead khi kết quả chỉ dùng để hiển thị/trả DTO, không sửa và lưu entity đó.
- `DbSet.Add(entity)` — đánh dấu entity mới là `Added`; chưa ghi SQL ngay. INSERT được gửi khi gọi `SaveChangesAsync`.
- `DbSet.AddRange(entities)` — tương tự `Add`, nhưng đánh dấu nhiều entity mới trong một lần gọi.
- `SaveChangesAsync(token)` — gửi các thay đổi đang được context theo dõi xuống DB (INSERT/UPDATE/DELETE) và trả số entry đã ghi. Các service thường gọi một lần sau khi hoàn tất validation và cập nhật entity.

### Phân biệt xử lý trong DB và trong bộ nhớ

Các truy vấn xây trên `DbSet` trước `ToListAsync`/`FirstOrDefaultAsync` là `IQueryable`, nên EF có thể dịch sang SQL. Sau khi dữ liệu đã được materialize, các phép như `Select`, `Where`, `Distinct`, `Except`, `Sum`, `All`, hoặc ánh xạ response chạy trên đối tượng trong bộ nhớ.

Ví dụ trong `PayrollQueryService.GetByIDAsync`, các navigation được nạp trước; sau đó lọc nhóm active và tạo danh sách detail từ graph đã nạp. Trong `PayrollPeriodManagementService`, `All` kiểm tra trạng thái các payroll đã được Include và tải về.

## 6. Luồng nghiệp vụ tổng quát

1. Tạo kỳ lương bằng `PayrollPeriodService`.
2. Tạo bản ghi payroll ban đầu theo danh sách EmployeeID bằng `PayrollManagementService`.
3. Đồng bộ attendance/OT bằng `PayrollAttendanceService`.
4. Đồng bộ mức lương và thu nhập dự án bằng `PayrollSalarySyncService`.
5. Tạo allowance/paycut detail khi kỳ còn draft bằng `PayrollAdjustmentService`.
6. Tính lương bằng `PayrollCalculationService` và `PayrollCalculator`.
7. Phê duyệt từng payroll bằng `PayrollApprovalService`.
8. Đánh dấu chi trả bằng `PayrollPaymentService`.
9. Cập nhật trạng thái kỳ bằng `PayrollPeriodManagementService`.
10. Đọc danh sách/chi tiết/lịch sử bằng `PayrollQueryService`; chỉ đọc phiếu lương đã phê duyệt hoặc đã trả qua `PayslipService`.

## 7. Mã nguồn minh họa theo từng service

Các đoạn dưới đây được trích gọn từ mã nguồn hiện tại để tập trung vào luồng xử lý. Chúng minh họa cách các service phối hợp với EF Core; không phải các file `.cs` độc lập để biên dịch. Phần ánh xạ DTO lặp lại được lược bớt ở một số đoạn.

### 7.1 Tạo và đọc kỳ lương

Trong `CreateAsync`, service kiểm tra dữ liệu trước, kiểm tra trùng bằng SQL `EXISTS`, rồi thêm entity và lưu:

```csharp
private async Task<bool> IsDuplicateAsync(
    CreatePayrollPeriodRequest request,
    CancellationToken cancellationToken)
{
    return await _context.PayrollPeriods.AnyAsync(
        p => p.Month == request.Month && p.Year == request.Year,
        cancellationToken);
}

public async Task<PayrollPeriodResponse> CreateAsync(
    CreatePayrollPeriodRequest request,
    CancellationToken cancellationToken = default)
{
    Validate(request);

    if (await IsDuplicateAsync(request, cancellationToken))
        throw new InvalidOperationException("Payroll period already exists.");

    var period = new PayrollPeriodEntity
    {
        Month = request.Month,
        Year = request.Year
    };

    _context.PayrollPeriods.Add(period);
    await _context.SaveChangesAsync(cancellationToken);

    return new PayrollPeriodResponse
    {
        PayrollPeriodID = period.PayrollPeriodID,
        Year = period.Year,
        Month = period.Month,
        Status = period.Status,
        CreatedAt = period.CreatedAt
    };
}
```

`Add` chỉ đánh dấu entity mới; `SaveChangesAsync` mới thực thi việc ghi. `AnyAsync` phù hợp để kiểm tra có bản ghi trùng mà không tải entity về.

Đọc danh sách dùng eager loading vì status hiển thị được tính từ trạng thái các payroll con:

```csharp
var periods = await _context.PayrollPeriods
    .Include(p => p.Payrolls)
    .OrderByDescending(p => p.Year)
    .ThenByDescending(p => p.Month)
    .ToListAsync(cancellationToken);
```

Sau `ToListAsync`, dữ liệu đã ở trong bộ nhớ; `GetCurrentStatus` xử lý collection `Payrolls` ở phía C#, không phải câu SQL.

### 7.2 Tạo payroll ban đầu

```csharp
var period = await _context.PayrollPeriods.FirstOrDefaultAsync(
    p => p.PayrollPeriodID == payrollPeriodID,
    cancellationToken);

if (period == null)
    throw new InvalidOperationException("Payroll period does not exist.");
if (period.Status != "draft")
    throw new InvalidOperationException("Cannot create payrolls for a period that is not draft.");

var existingIDs = await _context.Payrolls
    .Where(p => p.PayrollPeriodID == payrollPeriodID &&
                employeeIDs.Contains(p.EmployeeID))
    .Select(p => p.EmployeeID)
    .ToListAsync(cancellationToken);

var newIDs = employeeIDs.Except(existingIDs).ToList();
var newPayrolls = newIDs.Select(employeeID => new PayrollEntity
{
    PayrollPeriodID = payrollPeriodID,
    EmployeeID = employeeID,
    BaseSalary = 0,
    WorkingDays = 0,
    TotalAllowance = 0,
    TotalPayCut = 0,
    GrossSalary = 0,
    NetSalary = 0,
    Status = "draft"
}).ToList();

if (newPayrolls.Count > 0)
{
    _context.Payrolls.AddRange(newPayrolls);
    await _context.SaveChangesAsync(cancellationToken);
}
```

EF dịch `Where` và `Select` thành truy vấn chọn ID đã có. Sau khi `ToListAsync` hoàn tất, `Except` và tạo entity mới chạy trong bộ nhớ. Chỉ ID chưa tồn tại được thêm.

### 7.3 Đồng bộ công từ Attendance

```csharp
var period = await _context.PayrollPeriods.FirstOrDefaultAsync(
    p => p.PayrollPeriodID == payrollPeriodID, cancellationToken);
if (period == null)
    throw new InvalidOperationException("Payroll period not found.");
if (period.Status != "draft")
    throw new InvalidOperationException("Cannot sync attendance for a non-draft payroll period.");

var payrolls = await _context.Payrolls
    .Where(p => p.PayrollPeriodID == payrollPeriodID)
    .ToListAsync(cancellationToken);

var overtimeMultiplier = await _attendanceClient.GetOvertimeMultiplierAsync(cancellationToken);
var attendanceByEmployee = new Dictionary<Guid, (
    decimal WorkingDays,
    decimal StandardWorkingDays,
    decimal TotalWorkingHours,
    decimal OTHours)>();

foreach (var payroll in payrolls)
{
    var attendance = await _attendanceClient.GetEmployeeMonthlySummaryAsync(
        payroll.EmployeeID, period.Month, period.Year, cancellationToken);

    if (attendance == null)
        throw new InvalidOperationException("Attendance summary not found.");
    if (attendance.EmployeeId != payroll.EmployeeID)
        throw new InvalidOperationException("Attendance employee ID does not match payroll employee.");
    if (attendance.Month != period.Month || attendance.Year != period.Year)
        throw new InvalidOperationException("Attendance period does not match payroll period.");
    if (!string.Equals(attendance.PeriodStatus, "LOCKED", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Attendance period is not locked.");

    attendanceByEmployee.Add(payroll.EmployeeID, (
        attendance.ActualWorkingDays,
        attendance.StandardWorkingDays,
        attendance.TotalWorkingHours,
        attendance.OTHours));
}

foreach (var payroll in payrolls)
{
    var attendance = attendanceByEmployee[payroll.EmployeeID];
    payroll.WorkingDays = attendance.WorkingDays;
    payroll.StandardWorkingDay = attendance.StandardWorkingDays;
    payroll.TotalWorkingHours = attendance.TotalWorkingHours;
    payroll.OTHours = attendance.OTHours;
    payroll.OTMultiplier = overtimeMultiplier;
}

await _context.SaveChangesAsync(cancellationToken);
return payrolls.Count;
```

Đây là phần cốt lõi; source còn kiểm tra trạng thái `draft`, ngày/giờ không âm, công chuẩn và hệ số OT dương. Ý tưởng quan trọng là lấy và xác thực **tất cả** dữ liệu ngoài trước, rồi mới cập nhật các entity và gọi `SaveChangesAsync` một lần.

### 7.4 Đồng bộ lương HR và phụ cấp dự án

Service nạp payroll của kỳ trước, sau đó dùng client bên ngoài để lấy dữ liệu theo EmployeeID:

```csharp
var payrolls = await _context.Payrolls
    .Where(p => p.PayrollPeriodID == payrollPeriodID)
    .ToListAsync(cancellationToken);

var salaryByEmployee = new Dictionary<Guid, (
    string EmploymentType,
    decimal BaseSalary,
    decimal? HourlyRate)>();

foreach (var payroll in payrolls)
{
    var employee = await _hrClient.GetEmployeeAsync(payroll.EmployeeID, cancellationToken);
    if (employee == null || employee.EmployeeId != payroll.EmployeeID)
        throw new InvalidOperationException($"Employee {payroll.EmployeeID} was not found in HR.");
    if (!string.Equals(employee.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException($"Employee {payroll.EmployeeID} is not active.");

    var employmentType = employee.EmploymentType.Trim().ToUpperInvariant();
    decimal baseSalary;
    decimal? hourlyRate;

    switch (employmentType)
    {
        case "FULL_TIME":
            if (!employee.BaseSalary.HasValue || employee.BaseSalary.Value < 0)
                throw new InvalidOperationException("Invalid base salary.");
            if (employee.PartTimeSalary.HasValue)
                throw new InvalidOperationException("Unexpected part-time salary.");
            baseSalary = employee.BaseSalary.Value;
            hourlyRate = null;
            break;
        case "PART_TIME":
            if (!employee.PartTimeSalary.HasValue || employee.PartTimeSalary.Value < 0)
                throw new InvalidOperationException("Invalid part-time salary.");
            if (employee.BaseSalary.HasValue)
                throw new InvalidOperationException("Unexpected base salary.");
            baseSalary = 0m;
            hourlyRate = employee.PartTimeSalary.Value;
            break;
        default:
            throw new InvalidOperationException("Unsupported employment type.");
    }

    salaryByEmployee.Add(payroll.EmployeeID, (employmentType, baseSalary, hourlyRate));
}

var from = new DateOnly(period.Year, period.Month, 1);
var to = from.AddMonths(1).AddDays(-1);
var projectRows = await _projectClient.GetProjectAllowanceSummaryAsync(
    from, to, cancellationToken);

var projectAllowanceByEmployee = new Dictionary<Guid, decimal>();
foreach (var row in projectRows)
{
    projectAllowanceByEmployee[row.EmployeeId] =
        projectAllowanceByEmployee.GetValueOrDefault(row.EmployeeId) + row.TotalProjectIncome;
}

foreach (var payroll in payrolls)
{
    var salary = salaryByEmployee[payroll.EmployeeID];
    payroll.EmploymentType = salary.EmploymentType;
    payroll.BaseSalary = salary.BaseSalary;
    payroll.HourlyRate = salary.HourlyRate;
    payroll.TotalProjectAllowance = Math.Round(
        projectAllowanceByEmployee.GetValueOrDefault(payroll.EmployeeID),
        2,
        MidpointRounding.AwayFromZero);
}

await _context.SaveChangesAsync(cancellationToken);
return payrolls.Count;
```

Source còn kiểm tra tổng dự án hợp lệ. Khi đã hợp lệ, service gán các trường vào entity đang được EF theo dõi rồi `SaveChangesAsync`; các request HR/Project không phải EF query.

### 7.5 Tính lương

Service tính tổng các detail active trong bộ nhớ, gọi calculator thuần C#, rồi lưu các tổng đã tính:

```csharp
var payroll = await _context.Payrolls
    .Include(p => p.Allowances)
        .ThenInclude(a => a.AllowanceDetails)
    .Include(p => p.PayCuts)
        .ThenInclude(p => p.Details)
    .FirstOrDefaultAsync(p => p.PayrollID == payrollID, cancellationToken);

if (payroll == null)
    throw new InvalidOperationException("Payroll does not exist.");
if (payroll.Status != "draft")
    throw new InvalidOperationException("Only draft payroll can be calculated.");

var allowanceTotal = payroll.Allowances
    .Where(a => a.Status == "active")
    .SelectMany(a => a.AllowanceDetails)
    .Sum(d => d.Amount);
var payCutTotal = payroll.PayCuts
    .Where(p => p.Status == "active")
    .SelectMany(p => p.Details)
    .Sum(d => d.Amount);

var result = _calculator.Calculate(
    payroll.EmploymentType ?? string.Empty,
    payroll.BaseSalary, payroll.HourlyRate,
    payroll.WorkingDays, payroll.StandardWorkingDay,
    payroll.TotalWorkingHours, payroll.OTHours!.Value,
    payroll.OTMultiplier!.Value, allowanceTotal, payCutTotal,
    payroll.TotalProjectAllowance);

payroll.GrossSalary = result.GrossSalary;
payroll.NetSalary = result.NetSalary;
payroll.Status = "calculated";
payroll.CalculatedAt = DateTime.UtcNow;
await _context.SaveChangesAsync(cancellationToken);
```

`Include`/`ThenInclude` nạp các navigation cần tính. `FirstOrDefaultAsync` kết thúc truy vấn và trả entity đang được theo dõi; vì vậy khi gán `Status`, `GrossSalary`, `NetSalary`, EF nhận ra thay đổi và ghi khi `SaveChangesAsync`.

Phần công thức trong calculator:

```csharp
if (employmentType.Equals("FULL_TIME", StringComparison.OrdinalIgnoreCase))
{
    regularSalary = baseSalary * workingDays / standardWorkingDay;
    otHourlyRate = baseSalary / (standardWorkingDay * 8m);
}
else if (employmentType.Equals("PART_TIME", StringComparison.OrdinalIgnoreCase))
{
    regularSalary = hourlyRate!.Value * totalWorkingHours;
    otHourlyRate = hourlyRate.Value;
}
else
{
    throw new ArgumentException($"Unsupported employment type: {employmentType}");
}

var otPay = otHourlyRate * otHours * otMultiplier;
var totalAllowanceWithProject = totalAllowance + totalProjectAllowance;
var gross = regularSalary + totalAllowanceWithProject + otPay;
var net = gross - totalPayCut;
```

Source thực tế còn kiểm tra dữ liệu đầu vào và làm tròn từng khoản tiền 2 chữ số với `AwayFromZero`.

### 7.6 Thêm allowance/paycut detail

Ví dụ thêm allowance; thêm paycut dùng cùng mẫu nhưng thay `AllowanceType` bằng `PayCutReason` và `AllowanceDetails` bằng `Details`:

```csharp
var payroll = await _context.Payrolls.FirstOrDefaultAsync(
    p => p.PayrollID == payrollID, cancellationToken);
if (payroll == null)
    throw new InvalidOperationException("Payroll does not exist.");
if (payroll.Status != "draft")
    throw new InvalidOperationException("Cannot adjust a payroll that is not draft.");

var type = await _context.AllowanceTypes.FirstOrDefaultAsync(
    t => t.AllowanceTypeID == request.AllowanceTypeID, cancellationToken);
if (type == null || type.Status != "active")
    throw new InvalidOperationException("Allowance type does not exist or is inactive.");

var group = await _context.Allowances.FirstOrDefaultAsync(
    a => a.PayrollID == payrollID && a.Status == "active", cancellationToken);
if (group == null)
{
    group = new Allowance { PayrollID = payrollID, Status = "active" };
    _context.Allowances.Add(group);
}

group.AllowanceDetails.Add(new AllowanceDetail
{
    AllowanceTypeID = request.AllowanceTypeID,
    Description = request.Description.Trim(),
    RateApplied = request.RateApplied,
    Amount = request.Amount
});

await _context.SaveChangesAsync(cancellationToken);
```

Nhóm allowance được tải dưới dạng tracked entity hoặc được `Add` nếu mới. Detail được nối qua navigation collection; EF theo dõi graph và ghi các thay đổi khi lưu.

### 7.7 Phê duyệt, chi trả và trạng thái kỳ

Hai service chuyển trạng thái một payroll theo cùng mẫu: tải tracked entity, kiểm tra trạng thái hiện tại, sửa và lưu.

```csharp
var payroll = await _context.Payrolls.FirstOrDefaultAsync(
    p => p.PayrollID == payrollID, cancellationToken);
if (payroll == null)
    throw new InvalidOperationException("Payroll does not exist.");
if (payroll.Status != "calculated")
    throw new InvalidOperationException("Only calculated payroll can be approved.");

payroll.Status = "approved";
payroll.ApprovedAt = DateTime.UtcNow;
await _context.SaveChangesAsync(cancellationToken);
```

`PayrollPaymentService` yêu cầu trạng thái `approved`, sau đó gán `paid` và `PaidAt`. `PayrollPeriodManagementService` nạp quan hệ và tổng hợp:

```csharp
var period = await _context.PayrollPeriods
    .Include(p => p.Payrolls)
    .FirstOrDefaultAsync(p => p.PayrollPeriodID == payrollPeriodID, cancellationToken);

if (period == null || period.Payrolls.Count == 0)
    throw new InvalidOperationException("Payroll period does not exist or is empty.");

if (period.Payrolls.All(p => p.Status == "paid"))
    period.Status = "paid";
else if (period.Payrolls.All(p => p.Status == "approved"))
    period.Status = "approved";
else if (period.Payrolls.All(p => p.Status == "calculated"))
    period.Status = "calculated";
else
    throw new InvalidOperationException("Not all payrolls have completed the required status.");

await _context.SaveChangesAsync(cancellationToken);
```

`All` ở đây chạy trên `period.Payrolls` đã Include, tức là LINQ-to-Objects, không phải SQL `ALL`.

### 7.8 Danh mục allowance/paycut

Hai hàm đọc danh mục chỉ đọc và project ra DTO:

```csharp
var query = _context.AllowanceTypes.AsNoTracking();
if (activeOnly)
    query = query.Where(item => item.Status == "active");

return await query
    .OrderBy(item => item.Name)
    .Select(item => new PayrollCatalogItemResponse
    {
        ID = item.AllowanceTypeID,
        Name = item.Name,
        Description = item.Description,
        CalculationType = item.CalculationType,
        DefaultRate = item.DefaultRate,
        Status = item.Status
    })
    .ToListAsync(cancellationToken);
```

`GetPayCutReasonsAsync` tương tự nhưng truy vấn `PayCutReasons` và dùng `PayCutReasonID`.

Tên danh mục được kiểm tra trùng trước khi thêm/sửa:

```csharp
var normalizedName = name.Trim().ToLower();
var query = _context.AllowanceTypes
    .Where(item => item.Name.ToLower() == normalizedName);

if (exceptID.HasValue)
    query = query.Where(item => item.AllowanceTypeID != exceptID.Value);

var exists = await query.AnyAsync(cancellationToken);
if (exists)
    throw new InvalidOperationException("Tên danh mục đã tồn tại.");
```

Khi sửa entity đã tìm thấy, không cần gọi `Update` riêng nếu entity đang được tracking; thay đổi thuộc tính và gọi `SaveChangesAsync` là đủ.

### 7.9 Truy vấn danh sách và chi tiết payroll

Danh sách dùng no-tracking và projection để chỉ lấy dữ liệu cần trả:

```csharp
var rows = await _context.Payrolls
    .AsNoTracking()
    .Where(p => p.PayrollPeriodID == payrollPeriodID)
    .OrderBy(p => p.EmployeeID)
    .Select(p => new PayrollListResponse
    {
        PayrollID = p.PayrollID,
        EmployeeID = p.EmployeeID,
        PayrollPeriodID = p.PayrollPeriodID,
        BaseSalary = p.BaseSalary,
        WorkingDays = p.WorkingDays,
        GrossSalary = p.GrossSalary,
        NetSalary = p.NetSalary,
        Status = p.Status
    })
    .ToListAsync(cancellationToken);
```

Chi tiết cần nạp các navigation sâu hơn:

```csharp
var payroll = await _context.Payrolls
    .AsNoTracking()
    .Include(p => p.Allowances)
        .ThenInclude(a => a.AllowanceDetails)
            .ThenInclude(d => d.AllowanceType)
    .Include(p => p.PayCuts)
        .ThenInclude(p => p.Details)
            .ThenInclude(d => d.PayCutReason)
    .FirstOrDefaultAsync(p => p.PayrollID == payrollID, cancellationToken);
```

Sau khi query hoàn thành, service ánh xạ graph thành DTO; `Where(Status == "active")`, `SelectMany` và tạo các response detail ở đoạn này chạy trong bộ nhớ.

### 7.10 Phiếu lương

Phiếu chỉ được trả khi payroll đã phê duyệt hoặc đã chi trả:

```csharp
var payroll = await _context.Payrolls
    .AsNoTracking()
    .Include(p => p.PayrollPeriod)
    .Include(p => p.Allowances)
        .ThenInclude(a => a.AllowanceDetails)
            .ThenInclude(d => d.AllowanceType)
    .Include(p => p.PayCuts)
        .ThenInclude(p => p.Details)
            .ThenInclude(d => d.PayCutReason)
    .FirstOrDefaultAsync(p => p.PayrollID == payrollID, cancellationToken);

if (payroll == null)
    return null;
if (payroll.Status != "approved" && payroll.Status != "paid")
    return null;
if (payroll.ApprovedAt == null)
    throw new InvalidOperationException("Approved payroll must have an approval timestamp.");
```

`GetByPeriodAsync` trước hết lấy danh sách payroll ID, sau đó gọi `GetByPayrollIDAsync` từng ID. Cách viết này dễ hiểu và tái sử dụng quy tắc phát hành phiếu, nhưng tạo thêm một lần truy vấn cho mỗi payroll (mẫu N+1).

## 8. Gợi ý tự học và chạy thử

1. Đọc trước các DTO trong `src/Payroll/Payroll.Application/DTOs` để biết request/response có những trường nào.
2. Lần theo một luồng từ service → interface client → API controller → repository/EF context; ví dụ sync Attendance hoặc tạo kỳ lương.
3. Với truy vấn EF, xác định đoạn nào còn là `IQueryable` và đoạn nào đã materialize sau `ToListAsync`/`FirstOrDefaultAsync`.
4. Thử thay đổi đầu vào (tháng sai, trạng thái payroll sai, ID không tồn tại) để quan sát exception và quy tắc nghiệp vụ.
5. Đối chiếu đoạn code trong tài liệu với file nguồn được liên kết ở phần tổng quan; mã nguồn trong report là trích đoạn học tập và có thể cần cập nhật nếu implementation thay đổi.
