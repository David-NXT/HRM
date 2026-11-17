# HR Service — Tài liệu handover API

Tài liệu này mô tả các endpoint và trang web HR đang có trong source hiện tại. Nội dung được đối chiếu với controller, DTO, service và middleware trong src/HR. Payroll-handover.md được dùng làm tham khảo về bố cục và mức độ chi tiết.

## Mục lục

1. [Tổng quan](#1-tổng-quan)
2. [Khởi động và cấu hình](#2-khởi-động-và-cấu-hình)
3. [Quy ước API và xác thực](#3-quy-ước-api-và-xác-thực)
4. [Danh sách endpoint](#4-danh-sách-endpoint)
5. [Chi tiết endpoint](#5-chi-tiết-endpoint)
6. [JSON response DTO](#6-json-response-dto)
7. [Mã lỗi và quy tắc nghiệp vụ](#7-mã-lỗi-và-quy-tắc-nghiệp-vụ)
8. [Razor Pages](#8-razor-pages)
9. [Database và dữ liệu file](#9-database-và-dữ-liệu-file)
10. [File tham khảo trong source](#10-file-tham-khảo-trong-source)

## 1. Tổng quan

HR là nguồn dữ liệu chính cho nhân viên. Service quản lý hồ sơ, loại nhân viên, lương theo loại nhân viên, tổ chức, hợp đồng, lịch sử công tác, tiếp nhận, nghỉ việc, cập nhật hồ sơ cá nhân và cảnh báo hợp đồng.

HR API chạy mặc định tại:

| Profile | URL |
|---|---|
| HTTP | http://localhost:5012 |
| HTTPS | https://localhost:7115 |

Các API controller hiện dùng route ở root, ví dụ /employees và /departments; không có prefix /api. OpenAPI được map trong môi trường Development tại /openapi/v1.json.

Solution dùng .NET 10. HR backend được chia thành:

| Project | Trách nhiệm |
|---|---|
| HR.API | Controllers, Razor Pages, auth filters, exception middleware |
| HR.Application | DTO, service nghiệp vụ và interface |
| HR.Domain | Entity, status và employment type |
| HR.Infrastructure | EF Core, PostgreSQL, repositories, migration, scheduled job, lưu file |

Employee dùng GUID làm định danh liên dịch vụ. EmploymentType là FULL_TIME hoặc PART_TIME. Theo model hiện tại, full-time có BaseSalary (VND/tháng); part-time có PartTimeSalary (VND/giờ). API trả một giá trị lương có số và trường còn lại là null. Cả hai trường là decimal, ánh xạ database numeric(18,2). Tên JSON partTimeSalary được giữ để tương thích consumer hiện tại; DTO HR đặt tên rõ nghĩa PartTimeHourlyRate và ánh xạ về JSON partTimeSalary.

Ràng buộc đơn vị và luồng tính của Payroll được mô tả tại [hr-payroll-salary-contract.md](hr-payroll-salary-contract.md).

## 2. Khởi động và cấu hình

HR API đọc cấu hình từ appsettings, environment variables và file .env gần thư mục chạy. Connection string bắt buộc:

    ConnectionStrings__HrDatabase=Host=...;Port=5432;Database=...;Username=...;Password=...

Environment variable dùng dấu gạch dưới kép để ánh xạ ConnectionStrings:HrDatabase. EF design-time factory cũng đọc cùng cấu hình này. Nếu thiếu, dotnet ef database update dừng ở bước tạo HrDbContext, trước khi kết nối database.

Mặc định DatabaseInitialization:MigrateOnStartup là false. Có thể áp dụng migration thủ công từ thư mục gốc repo:

    dotnet ef database update --project src/HR/HR.Infrastructure/HR.Infrastructure.csproj --startup-project src/HR/HR.API/HR.API.csproj

Nếu bật MigrateOnStartup, API chạy Database.MigrateAsync trước khi nhận request.

ContractExpiryJob mặc định bật, chạy ngay khi service khởi động và sau đó quét hằng ngày lúc 01:00 UTC. Cấu hình nằm trong ContractExpiryJob: Enabled, RunOnStartup và DailyRunTimeUtc.

## 3. Quy ước API và xác thực

### 3.1 JSON và kiểu dữ liệu

- JSON request/response dùng camelCase.
- Guid dùng chuỗi GUID.
- DateOnly dùng định dạng YYYY-MM-DD.
- DateTime được lưu/trả theo UTC; response thường có hậu tố Z.
- Các số tiền là JSON number, kiểu decimal, tối đa 2 chữ số thập phân.
- Query phân trang mặc định page=1, pageSize=20; các API phân trang giới hạn pageSize tối đa 100.
- DELETE /employees/{id} xóa vật lý hồ sơ chưa có dữ liệu liên quan. Hồ sơ có lịch sử công tác, hợp đồng, yêu cầu cập nhật, thông báo hoặc offboarding trả 409 để bảo toàn lịch sử.

### 3.2 Header truy cập

Các endpoint HR quản trị và duyệt hồ sơ yêu cầu:

    X-Role: HR

Thiếu header trả 401; giá trị khác HR trả 403. Đây là filter kiểm tra header, chưa phải xác thực JWT hay ASP.NET Authentication scheme.

Nhóm self-service /me yêu cầu:

    X-Employee-Guid: <employee-guid>

Filter tra nhân viên trong HR và chỉ cho phép nhân viên có status ACTIVE. Đây là cơ chế định danh qua header trong code hiện tại.

Nhóm danh mục nội bộ /internal/employees dùng:

    X-HR-Directory-Key: <configured-key>

Key được cấu hình tại InternalServices:EmployeeDirectoryApiKey. Nếu key chưa cấu hình ở Development, API cho phép fallback X-Role: HR. Ngoài Development, thiếu key cấu hình trả 503; thiếu key request trả 401; key sai trả 403.

### 3.3 Lỗi

Lỗi nghiệp vụ do HR tạo trả JSON:

    {
      "code": "ValidationError",
      "message": "Thông tin gửi lên không hợp lệ."
    }

Middleware trả status code gắn với lỗi. Lỗi không dự kiến trả 500 với code InternalError và message tổng quát. Một số lỗi validation/model binding do ASP.NET xử lý có thể dùng ProblemDetails thay vì ApiError.

## 4. Danh sách endpoint

Tất cả route dưới đây nằm dưới host HR, ví dụ http://localhost:5012.

| Method | Route | Quyền | Chức năng | Response thành công |
|---|---|---|---|---|
| GET | /employees | HR | Tìm kiếm, phân trang toàn bộ hồ sơ | EmployeeListDto |
| GET | /employees/statistics | HR | Thống kê tổng số nhân viên theo trạng thái và loại hình làm việc | EmployeeStatisticsDto |
| GET | /employees/{id} | HR | Chi tiết hồ sơ | EmployeeDto |
| POST | /employees | HR | Tạo hồ sơ trực tiếp | 201 EmployeeDto |
| DELETE | /employees/{id} | HR | Xóa hồ sơ chưa có dữ liệu liên quan | 204 |
| PUT | /employees/{id} | HR | Sửa thông tin cá nhân | EmployeeDto |
| PUT | /employees/{id}/salary | HR | Sửa đúng trường lương theo loại nhân viên | EmployeeDto |
| POST | /employees/{id}/lock | HR | Khóa hồ sơ | EmployeeDto |
| GET | /departments | HR | Danh sách phòng ban | DepartmentDto[] |
| GET | /departments/tree | HR | Cây phòng ban | DepartmentTreeDto[] |
| POST | /departments | HR | Tạo phòng ban | 201 DepartmentDto |
| PUT | /departments/{id} | HR | Sửa phòng ban | DepartmentDto |
| POST | /departments/{id}/deactivate | HR | Ngừng phòng ban | DepartmentDto |
| GET | /positions | HR | Danh sách chức vụ | PositionDto[] |
| POST | /positions | HR | Tạo chức vụ | 201 PositionDto |
| PUT | /positions/{id} | HR | Sửa chức vụ | PositionDto |
| POST | /positions/{id}/deactivate | HR | Ngừng chức vụ | PositionDto |
| GET | /employees/{employeeId}/contracts | HR | Danh sách hợp đồng | EmploymentContractDto[] |
| POST | /employees/{employeeId}/contracts | HR | Tạo hợp đồng hoặc hợp đồng gia hạn | 201 EmploymentContractDto |
| PUT | /contracts/{contractId} | HR | Sửa hợp đồng ACTIVE | EmploymentContractDto |
| GET | /employees/{employeeId}/work-history | HR | Lịch sử phòng ban/chức vụ | WorkHistoryDto[] |
| POST | /employees/{employeeId}/work-history | HR | Ghi nhận phân công mới | 201 WorkHistoryDto |
| GET | /lifecycle/options | HR | Danh sách phòng ban/chức vụ ACTIVE cho onboarding | LifecycleOptionsDto |
| POST | /onboarding | HR | Tạo nhân viên và bản ghi công tác đầu tiên | 201 OnboardingResultDto |
| POST | /offboarding | HR | Bắt đầu hồ sơ nghỉ việc | OffboardingDto |
| GET | /offboarding | HR | Lọc hồ sơ nghỉ việc | OffboardingDto[] |
| GET | /offboarding/{id} | HR | Chi tiết hồ sơ nghỉ việc | OffboardingDto |
| PUT | /offboarding/{id} | HR | Sửa ngày cuối/ghi chú, cần ExpectedVersion | OffboardingDto |
| PUT | /offboarding/{id}/checklist/{itemId} | HR | Cập nhật một mục bàn giao, cần ExpectedVersion | OffboardingDto |
| POST | /offboarding/{id}/complete | HR | Hoàn tất nghỉ việc, cần ExpectedVersion | OffboardingDto |
| GET | /contract-alerts | HR | Lọc cảnh báo hợp đồng | ContractAlertListDto |
| GET | /contract-alerts/{id} | HR | Chi tiết cảnh báo | ContractAlertDto |
| PATCH | /contract-alerts/{id} | HR | Cập nhật trạng thái cảnh báo, cần ExpectedVersion | ContractAlertDto |
| GET | /me/profile | Employee | Hồ sơ của nhân viên trong header | EmployeeDto |
| GET | /me/update-requests | Employee | Các yêu cầu cập nhật của chính nhân viên | ProfileUpdateRequestDto[] |
| POST | /me/update-requests | Employee | Tạo draft hoặc gửi yêu cầu và file minh chứng | 201 ProfileUpdateRequestDto |
| PUT | /me/update-requests/{requestId} | Employee | Sửa draft/need-info và file minh chứng | ProfileUpdateRequestDto |
| POST | /me/update-requests/{requestId}/cancel | Employee | Hủy yêu cầu đủ điều kiện | ProfileUpdateRequestDto |
| GET | /me/update-requests/{requestId}/attachments/{attachmentId} | Employee | Tải file của yêu cầu thuộc nhân viên | File stream |
| GET | /me/notifications | Employee | Danh sách thông báo của nhân viên | EmployeeNotificationDto[] |
| GET | /update-requests | HR | Danh sách yêu cầu chờ duyệt hoặc lọc theo status | ProfileUpdateRequestDto[] |
| GET | /update-requests/{requestId} | HR | Chi tiết yêu cầu duyệt | ProfileUpdateRequestDto |
| POST | /update-requests/{requestId}/review | HR | Chấp thuận, từ chối hoặc yêu cầu bổ sung | ProfileUpdateRequestDto |
| GET | /update-requests/{requestId}/attachments/{attachmentId} | HR | Tải file minh chứng để duyệt | File stream |
| GET | /internal/employees | Service | Danh mục nhân viên ACTIVE có phân trang | EmployeeDirectoryListDto |
| GET | /internal/employees/{employeeId} | Service | Một nhân viên ACTIVE | EmployeeDirectoryEntry |

## 5. Chi tiết endpoint

### 5.1 Nhân viên

#### GET /employees

Header X-Role: HR. Query keyword tìm theo mã, tên, email, điện thoại hoặc CCCD; page mặc định 1; pageSize mặc định 20, tối đa 100. API gồm cả ACTIVE, LOCKED và RESIGNED.

Response 200: EmployeeListDto, xem schema mục 6.1.

#### GET /employees/statistics

Header X-Role: HR. Trả tổng số nhân viên, số ACTIVE/LOCKED/RESIGNED và số FULL_TIME/PART_TIME trong một lần truy vấn.

Response 200: EmployeeStatisticsDto.

#### GET /employees/{id}

Header X-Role: HR. Trả EmployeeDto; 404 nếu không có hồ sơ.

#### POST /employees

Header X-Role: HR. Tạo hồ sơ nhưng không tạo lịch sử công tác; dùng POST /onboarding nếu cần tạo luôn phân công đầu tiên.

Request:

    {
      "employeeCode": "NV001",
      "fullName": "Nguyễn Văn A",
      "dateOfBirth": "1998-05-12",
      "gender": "Male",
      "email": "vana@example.com",
      "phone": "0901234567",
      "citizenId": "079198001234",
      "address": "Quận 1, TP.HCM",
      "employmentType": "FULL_TIME",
      "baseSalary": 15000000.00
    }

Ví dụ nhân viên part-time:

    {
      "employeeCode": "NVPT001",
      "fullName": "Nguyễn Thị B",
      "dateOfBirth": "2000-01-01",
      "gender": "Female",
      "email": "b@example.com",
      "phone": "0901234568",
      "citizenId": "079200001234",
      "address": null,
      "employmentType": "PART_TIME",
      "partTimeSalary": 35000.00
    }

EmploymentType mặc định FULL_TIME nếu bỏ qua. BaseSalary thuộc FULL_TIME và tính theo VND/tháng; PartTimeSalary thuộc PART_TIME và là đơn giá VND/giờ. Lương bỏ trống khi tạo hiện được khởi tạo bằng 0 để tương thích request cũ. Client nên gửi trường đúng loại. Mã nhân viên chuẩn hóa chữ hoa; mã dài 3–20 ký tự, gồm chữ/số/_/-. Mã trùng trả 409.

Các trường chung được kiểm tra trước khi lưu: fullName dài 2–150; dateOfBirth từ năm 1900 đến ngày hiện tại; email hợp lệ, tối đa 200 ký tự; phone sau chuẩn hóa phải có 9–15 chữ số; citizenId phải có 9 hoặc 12 chữ số; address tối đa 500 ký tự. Gender nhận Male/Female/Other và các giá trị tiếng Việt tương ứng, rồi chuẩn hóa về tiếng Anh.

Response 201: EmployeeDto và Location trỏ tới GET /employees/{id}.

#### PUT /employees/{id}

Header X-Role: HR. Chỉ cập nhật dữ liệu cá nhân; employeeCode, status, employmentType và lương không đổi.

Request:

    {
      "fullName": "Nguyễn Văn A",
      "dateOfBirth": "1998-05-12",
      "gender": "Male",
      "email": "vana@example.com",
      "phone": "+84901234567",
      "citizenId": "079198001234",
      "address": "Quận 3, TP.HCM"
    }

Response 200: EmployeeDto.

#### PUT /employees/{id}/salary

Header X-Role: HR. Phải gửi trường phù hợp EmploymentType:

    { "baseSalary": 15000000.00 }

hoặc:

    { "partTimeSalary": 35000.00 }

Sai trường theo loại nhân viên, lương âm, vượt numeric(18,2), hoặc có trên 2 chữ số thập phân sẽ bị từ chối. Response 200: EmployeeDto. Dù tên JSON giữ là partTimeSalary để tương thích, giá trị này là VND/giờ.

#### POST /employees/{id}/lock

Header X-Role: HR. Chuyển hồ sơ sang LOCKED. Hồ sơ RESIGNED không thể khóa lại; trường hợp đó trả 409. Response 200: EmployeeDto.

#### DELETE /employees/{id}

Header X-Role: HR. Xóa vật lý hồ sơ chỉ khi nhân viên chưa có lịch sử công tác, hợp đồng, yêu cầu cập nhật hồ sơ, thông báo hoặc hồ sơ nghỉ việc. Nếu còn dữ liệu liên quan, trả 409 EmployeeHasRelatedRecords; không tìm thấy nhân viên trả 404. Thành công trả 204 No Content.

### 5.2 Phòng ban và chức vụ

#### GET /departments

Header X-Role: HR. Trả mảng DepartmentDto.

#### GET /departments/tree

Header X-Role: HR. Trả cây DepartmentTreeDto; mỗi node có Children. Cây bao gồm các phòng ban được lưu, kể cả INACTIVE.

#### POST /departments

Header X-Role: HR.

Request:

    {
      "code": "HR",
      "name": "Nhân sự",
      "description": "Phòng nhân sự",
      "parentDepartmentId": null
    }

Mã chuẩn hóa uppercase, dài 2–20 ký tự chữ/số/_/-. Tên dài 2–150; description tối đa 500 ký tự. Parent nếu có phải tồn tại và ACTIVE; không được tự làm cha hoặc chuyển vào descendant. Mã trùng trả 409. Response 201: DepartmentDto.

#### PUT /departments/{id}

Header X-Role: HR. Request cùng schema POST. Response 200: DepartmentDto. Không tìm thấy trả 404.

#### POST /departments/{id}/deactivate

Header X-Role: HR. Không xóa bản ghi. Không thể deactivate khi còn phòng ban con ACTIVE, nhân viên ACTIVE, lịch sử công tác đang mở hoặc đang có hiệu lực. Trả 409 DepartmentInUse nếu còn quan hệ đang dùng. Gọi lại trên phòng ban INACTIVE trả chính bản ghi hiện tại. Response 200: DepartmentDto.

#### GET /positions

Header X-Role: HR. Trả mảng PositionDto.

#### POST /positions

Header X-Role: HR.

Request:

    {
      "code": "DEV",
      "name": "Lập trình viên",
      "description": null,
      "seniorityLevel": "STAFF"
    }

Validation mã/tên/description giống phòng ban. seniorityLevel bắt buộc nhận STAFF, MANAGER, DIRECTOR hoặc EXECUTIVE. Position cũ chưa phân loại trả null cho đến khi HR cập nhật. Response 201: PositionDto.

#### PUT /positions/{id}

Header X-Role: HR. Request cùng schema POST; chỉ cập nhật position đang có. Không thể chuyển chức vụ lên quản lý khi còn nhân viên part-time đang đảm nhiệm. Khi phân loại chức vụ là DIRECTOR, các quá trình công tác đang mở phải không tạo ra Giám đốc thứ hai trong một phòng ban. Response 200: PositionDto.

#### POST /positions/{id}/deactivate

Header X-Role: HR. Không thể deactivate nếu còn nhân viên ACTIVE, work history đang mở hoặc assignment có hiệu lực. Trả 409 PositionInUse. Response 200: PositionDto.

### 5.3 Hợp đồng và lịch sử công tác

#### GET /employees/{employeeId}/contracts

Header X-Role: HR. Trả EmploymentContractDto[] theo nhân viên; nhân viên không tồn tại trả 404.

#### POST /employees/{employeeId}/contracts

Header X-Role: HR.

Request hợp đồng thường:

    {
      "contractNumber": "HD-2026-001",
      "contractType": "Labor",
      "startDate": "2026-01-01",
      "endDate": "2026-12-31",
      "notes": null,
      "renewalOfContractId": null
    }

Số và loại hợp đồng dài 1–50 ký tự; startDate trước endDate nếu endDate có giá trị; notes tối đa 1000 ký tự; số hợp đồng phải duy nhất. Response 201: EmploymentContractDto.

Gia hạn dùng renewalOfContractId. Hợp đồng nguồn phải thuộc cùng nhân viên, còn ACTIVE, chưa có gia hạn; ngày bắt đầu hợp đồng mới phải sau thời gian hợp đồng nguồn. Hợp đồng cũ chuyển RENEWED; nếu chưa có endDate, hệ thống đặt bằng ngày trước startDate của hợp đồng mới. Lỗi trạng thái/gia hạn trả 409; hợp đồng nguồn không tìm thấy trả 404.

#### PUT /contracts/{contractId}

Header X-Role: HR.

Request:

    {
      "contractNumber": "HD-2026-001",
      "contractType": "Labor",
      "startDate": "2026-01-01",
      "endDate": "2026-12-31",
      "notes": "Đã cập nhật"
    }

Chỉ sửa hợp đồng ACTIVE. Response 200: EmploymentContractDto.

#### GET /employees/{employeeId}/work-history

Header X-Role: HR. Trả WorkHistoryDto[] theo nhân viên.

#### POST /employees/{employeeId}/work-history

Header X-Role: HR.

Request:

    {
      "departmentId": "00000000-0000-0000-0000-000000000001",
      "positionId": "00000000-0000-0000-0000-000000000002",
      "effectiveDate": "2026-07-01",
      "notes": "Điều chuyển nội bộ"
    }

Nhân viên RESIGNED không nhận assignment mới. Department và Position phải ACTIVE. Part-time chỉ nhận position có seniorityLevel=STAFF; position null/chưa phân loại và cấp MANAGER trở lên bị từ chối. Mỗi phòng ban tối đa một assignment DIRECTOR trong cùng khoảng hiệu lực. Ngày hiệu lực phải sau bản ghi công tác gần nhất. Vi phạm quy tắc part-time trả 400 PartTimePositionNotAllowed; trùng Giám đốc trả 409 DepartmentDirectorAlreadyAssigned. Tạo 201: WorkHistoryDto.

### 5.4 Onboarding và offboarding

#### GET /lifecycle/options

Header X-Role: HR. Trả LifecycleOptionsDto gồm departments và positions đang ACTIVE.

#### POST /onboarding

Header X-Role: HR. Tạo employee và work history đầu tiên trong cùng transaction.

Request:

    {
      "employee": {
        "employeeCode": "NV001",
        "fullName": "Nguyễn Văn A",
        "dateOfBirth": "1998-05-12",
        "gender": "Male",
        "email": "vana@example.com",
        "phone": "0901234567",
        "citizenId": "079198001234",
        "address": null,
        "employmentType": "FULL_TIME",
        "baseSalary": 15000000.00
      },
      "startDate": "2026-07-01",
      "departmentId": "00000000-0000-0000-0000-000000000001",
      "positionId": "00000000-0000-0000-0000-000000000002",
      "notes": null
    }

employee dùng cùng schema CreateEmployeeRequest. Ngày bắt đầu phải hợp lệ và sau ngày sinh; đơn vị và chức vụ phải ACTIVE; mã nhân viên phải duy nhất. Part-time chỉ được chọn chức vụ STAFF đã phân loại. Position MANAGER, DIRECTOR, EXECUTIVE hoặc chưa phân loại bị từ chối cho part-time. Một phòng ban không nhận thêm Giám đốc nếu đã có assignment DIRECTOR trùng khoảng hiệu lực. Nếu ngày bắt đầu đã đến, DepartmentId/PositionId hiện tại trên employee được thiết lập ngay; dù ngày tương lai, WorkHistory đầu tiên vẫn được tạo. Response 201: OnboardingResultDto.

#### POST /offboarding

Header X-Role: HR. Tạo hồ sơ nghỉ việc và checklist; chưa chuyển employee sang RESIGNED.

Request:

    {
      "employeeId": "00000000-0000-0000-0000-000000000010",
      "lastWorkingDate": "2026-07-31",
      "notes": "Bàn giao thiết bị",
      "checklist": [
        { "title": "Bàn giao laptop", "isRequired": true },
        { "title": "Thu hồi thẻ", "isRequired": false }
      ]
    }

Checklist cần 1–50 mục, title 1–200 ký tự và không trùng (không phân biệt hoa thường). Chỉ được có một offboarding IN_PROGRESS cho mỗi nhân viên. Nhân viên đã RESIGNED không thể bắt đầu lại. Response 200: OffboardingDto.

#### GET /offboarding

Header X-Role: HR. Query filter tùy chọn employeeId và status (IN_PROGRESS hoặc COMPLETED). Trả OffboardingDto[]; status rỗng nghĩa là không lọc.

#### GET /offboarding/{id}

Header X-Role: HR. Trả OffboardingDto; không tồn tại trả 404.

#### PUT /offboarding/{id}

Header X-Role: HR.

Request:

    {
      "lastWorkingDate": "2026-07-31",
      "notes": "Đã thống nhất ngày nghỉ",
      "expectedVersion": 2
    }

Chỉ sửa hồ sơ IN_PROGRESS. expectedVersion phải khớp Version mới nhất. Trả 409 LifecycleChanged nếu stale; hồ sơ COMPLETED không sửa được. Response 200: OffboardingDto.

#### PUT /offboarding/{id}/checklist/{itemId}

Header X-Role: HR.

Request:

    {
      "isCompleted": true,
      "completionNote": "Đã bàn giao",
      "expectedVersion": 2
    }

Response 200: OffboardingDto với Version mới sau lưu.

#### POST /offboarding/{id}/complete

Header X-Role: HR.

Request:

    { "expectedVersion": 3 }

Chỉ hoàn tất khi đã đến hoặc qua LastWorkingDate và mọi mục bắt buộc hoàn tất. Khi hoàn tất, employee chuyển RESIGNED, các assignment đang mở/sau ngày cuối được đóng tại LastWorkingDate, offboarding chuyển COMPLETED. Thiếu checklist trả 422 ChecklistIncomplete; chưa đến ngày cuối trả 409 OffboardingNotEffective. Gọi lại trên hồ sơ COMPLETED trả kết quả hiện tại. Response 200: OffboardingDto.

### 5.5 Cảnh báo hợp đồng

#### GET /contract-alerts

Header X-Role: HR. Query tùy chọn:

- status: NEW, VIEWED hoặc IN_PROGRESS
- employeeId
- expiresFrom, expiresTo: YYYY-MM-DD
- page mặc định 1; pageSize mặc định 20, tối đa 100

Response 200: ContractAlertListDto.

#### GET /contract-alerts/{id}

Header X-Role: HR. Trả ContractAlertDto; không tìm thấy trả 404.

#### PATCH /contract-alerts/{id}

Header X-Role: HR.

Request:

    {
      "status": "IN_PROGRESS",
      "expectedVersion": 1
    }

Status chỉ nhận NEW, VIEWED, IN_PROGRESS. expectedVersion phải khớp để tránh ghi đè cập nhật mới; stale trả 409 ContractAlertChanged. Response 200: ContractAlertDto.

Job nền tạo cảnh báo cho hợp đồng ACTIVE có EndDate hợp lệ còn 0–30 ngày tính theo UTC. Hợp đồng vô thời hạn, đã gia hạn hoặc hết hạn bị bỏ qua. Cảnh báo chống trùng theo cặp contract/expiry date; cảnh báo chu kỳ cũ vẫn giữ được để tra cứu.

### 5.6 Self-service hồ sơ cá nhân

Các route dưới đây yêu cầu X-Employee-Guid với employee status ACTIVE.

#### GET /me/profile

Trả EmployeeDto thuộc nhân viên trong header.

#### GET /me/update-requests

Trả ProfileUpdateRequestDto[] của nhân viên hiện tại, mới nhất trước.

#### POST /me/update-requests

Content-Type: multipart/form-data. Form fields:

| Field | Kiểu | Bắt buộc | Mô tả |
|---|---|---|---|
| ChangesJson | string | Có | JSON của UpdateEmployeeRequest |
| Submit | bool | Không | true tạo trạng thái PENDING; false tạo DRAFT |
| Attachments | file[] | Không | File minh chứng, tối đa 5 file trên yêu cầu |

ChangesJson ví dụ:

    {
      "fullName": "Nguyễn Văn A",
      "dateOfBirth": "1998-05-12",
      "gender": "Male",
      "email": "vana@example.com",
      "phone": "0901234567",
      "citizenId": "079198001234",
      "address": "Quận 3"
    }

Request body tối đa 55,000,000 bytes; ChangesJson tối đa 32,000 ký tự. Mỗi file phải là PDF, JPG/JPEG hoặc PNG, có chữ ký nội dung phù hợp, kích thước lớn hơn 0 và không quá 10 MB. Response 201: ProfileUpdateRequestDto.

#### PUT /me/update-requests/{requestId}

Multipart form cùng POST. Chỉ sửa DRAFT hoặc NEED_INFO. Submit=true chuyển sang PENDING. Có thể thêm file nhưng tổng file của yêu cầu không quá 5. Response 200: ProfileUpdateRequestDto.

#### POST /me/update-requests/{requestId}/cancel

Chỉ hủy DRAFT hoặc PENDING; trạng thái mới CANCELLED. Response 200: ProfileUpdateRequestDto.

#### GET /me/update-requests/{requestId}/attachments/{attachmentId}

Chỉ tải được file thuộc yêu cầu của employee hiện tại. Trả file stream với Content-Type và tên file gốc; không trả JSON khi thành công.

#### GET /me/notifications

Trả EmployeeNotificationDto[] của employee hiện tại. Response chứa ReadAt; hiện không có endpoint đánh dấu đã đọc.

### 5.7 HR duyệt cập nhật hồ sơ

Các route yêu cầu X-Role: HR.

#### GET /update-requests

Query status tùy chọn; mặc định PENDING. Các giá trị: DRAFT, PENDING, NEED_INFO, APPROVED, REJECTED, CANCELLED. Trả ProfileUpdateRequestDto[].

#### GET /update-requests/{requestId}

Trả yêu cầu và metadata file; không tìm thấy trả 404.

#### POST /update-requests/{requestId}/review

Request:

    {
      "decision": "APPROVE",
      "reason": null
    }

decision nhận APPROVE, REJECT hoặc REQUEST_MORE_INFO. Chỉ review PENDING. REJECT và REQUEST_MORE_INFO bắt buộc có reason, tối đa 1000 ký tự. APPROVE áp dụng thay đổi cá nhân vào hồ sơ employee. Mọi quyết định tạo notification cho employee. Response 200: ProfileUpdateRequestDto. Trạng thái không còn PENDING trả 409.

#### GET /update-requests/{requestId}/attachments/{attachmentId}

Tải file minh chứng của yêu cầu để HR xem; response thành công là file stream.

### 5.8 Danh mục nhân viên nội bộ

Các route trả dữ liệu tối thiểu cho service khác; chỉ nhân viên ACTIVE được liệt kê/tra cứu. Response không chứa email, số điện thoại, CCCD hoặc lương.

EmployeeDirectoryEntry có employmentType để consumer phân biệt FULL_TIME/PART_TIME. API này không có BaseSalary hoặc PartTimeSalary; hiện chưa có endpoint payroll-info riêng.

#### GET /internal/employees

Header X-HR-Directory-Key; Development có fallback X-Role: HR nếu chưa cấu hình key. Query keyword tìm mã/tên, page mặc định 1, pageSize mặc định 20, tối đa 100. Response 200: EmployeeDirectoryListDto.

#### GET /internal/employees/{employeeId}

Cùng xác thực với list. Trả EmployeeDirectoryEntry; nhân viên không tồn tại hoặc không ACTIVE trả 404.

## 6. JSON response DTO

Tên property dưới đây viết theo JSON camelCase. Các endpoint được tham chiếu tới DTO tương ứng trong mục 5.

### 6.1 EmployeeDto và danh sách nhân viên

EmployeeDto:

    {
      "employeeId": "00000000-0000-0000-0000-000000000010",
      "employeeCode": "NV001",
      "fullName": "Nguyễn Văn A",
      "dateOfBirth": "1998-05-12",
      "gender": "Male",
      "email": "vana@example.com",
      "phone": "0901234567",
      "citizenId": "079198001234",
      "address": "Quận 1, TP.HCM",
      "status": "ACTIVE",
      "createdAt": "2026-07-01T02:00:00Z",
      "updatedAt": "2026-07-01T02:00:00Z",
      "departmentId": "00000000-0000-0000-0000-000000000001",
      "positionId": "00000000-0000-0000-0000-000000000002",
      "departmentName": "Nhân sự",
      "positionName": "Chuyên viên",
      "employmentType": "FULL_TIME",
      "baseSalary": 15000000.00,
      "partTimeSalary": null
    }

Với PART_TIME, response có employmentType=PART_TIME, baseSalary=null và partTimeSalary là số theo đơn vị VND/giờ.

EmployeeListDto:

    {
      "items": [],
      "page": 1,
      "pageSize": 20,
      "total": 0
    }

### 6.2 EmployeeDirectoryEntry và danh sách

    {
      "employeeId": "00000000-0000-0000-0000-000000000010",
      "employeeCode": "NV001",
      "fullName": "Nguyễn Văn A",
      "status": "ACTIVE",
      "departmentName": "Nhân sự",
      "positionName": "Chuyên viên",
      "employmentType": "FULL_TIME"
    }

EmployeeDirectoryListDto bọc mảng items cùng page, pageSize, total như EmployeeListDto.

### 6.3 DepartmentDto, DepartmentTreeDto và PositionDto

DepartmentDto:

    {
      "departmentId": "00000000-0000-0000-0000-000000000001",
      "code": "HR",
      "name": "Nhân sự",
      "description": null,
      "parentDepartmentId": null,
      "status": "ACTIVE",
      "createdAt": "2026-07-01T02:00:00Z",
      "updatedAt": "2026-07-01T02:00:00Z"
    }

DepartmentTreeDto có departmentId, code, name, status và children (mảng DepartmentTreeDto lồng nhau).

PositionDto:

    {
      "positionId": "00000000-0000-0000-0000-000000000002",
      "code": "HR-SPECIALIST",
      "name": "Chuyên viên nhân sự",
      "description": null,
      "status": "ACTIVE",
      "createdAt": "2026-07-01T02:00:00Z",
      "updatedAt": "2026-07-01T02:00:00Z",
      "seniorityLevel": "STAFF"
    }

seniorityLevel có thể là STAFF, MANAGER, DIRECTOR, EXECUTIVE hoặc null với chức vụ cũ chưa được HR phân loại. Chỉ STAFF đủ điều kiện cho nhân viên part-time.

### 6.4 Hợp đồng và lịch sử công tác

EmploymentContractDto có:

- contractId, employeeId: GUID
- contractNumber, contractType, status: string
- startDate: date
- endDate: date hoặc null
- notes: string hoặc null
- renewalOfContractId: GUID hoặc null
- createdAt, updatedAt: datetime UTC

WorkHistoryDto có:

- workHistoryId, employeeId, departmentId, positionId: GUID
- departmentCode, departmentName, positionCode, positionName: string
- startDate: date; endDate: date hoặc null
- notes: string hoặc null

Các GET collection trả JSON array, kể cả khi rỗng:

    []

### 6.5 Onboarding và offboarding

OnboardingResultDto:

    {
      "employeeId": "00000000-0000-0000-0000-000000000010",
      "employeeCode": "NV001",
      "status": "ACTIVE",
      "workHistoryId": "00000000-0000-0000-0000-000000000011",
      "startDate": "2026-07-01",
      "departmentId": "00000000-0000-0000-0000-000000000001",
      "positionId": "00000000-0000-0000-0000-000000000002",
      "employmentType": "FULL_TIME"
    }

OffboardingDto:

    {
      "offboardingRecordId": "00000000-0000-0000-0000-000000000020",
      "employeeId": "00000000-0000-0000-0000-000000000010",
      "employeeCode": "NV001",
      "employeeName": "Nguyễn Văn A",
      "employeeStatus": "ACTIVE",
      "lastWorkingDate": "2026-07-31",
      "status": "IN_PROGRESS",
      "notes": null,
      "version": 1,
      "createdAt": "2026-07-01T02:00:00Z",
      "updatedAt": "2026-07-01T02:00:00Z",
      "completedAt": null,
      "checklist": [
        {
          "offboardingChecklistItemId": "00000000-0000-0000-0000-000000000021",
          "title": "Bàn giao laptop",
          "isRequired": true,
          "isCompleted": false,
          "completionNote": null,
          "completedAt": null
        }
      ]
    }

### 6.6 Cảnh báo hợp đồng

ContractAlertDto có:

- contractAlertId, contractId, employeeId: GUID
- employeeCode, employeeName, employeeStatus
- contractNumber, contractType, contractStatus
- contractStartDate, currentContractEndDate, expiryDate
- daysRemaining: integer tính từ ngày UTC hiện tại
- isCurrentCycle: cảnh báo có khớp hợp đồng hết hạn hiện hành không
- status: NEW, VIEWED hoặc IN_PROGRESS
- version: integer concurrency token
- createdAt, updatedAt: datetime UTC

ContractAlertListDto có items (ContractAlertDto[]), page, pageSize, total.

### 6.7 Cập nhật hồ sơ và notification

ProfileUpdateRequestDto:

    {
      "profileUpdateRequestId": "00000000-0000-0000-0000-000000000030",
      "employeeId": "00000000-0000-0000-0000-000000000010",
      "changes": {
        "fullName": "Nguyễn Văn A",
        "dateOfBirth": "1998-05-12",
        "gender": "Male",
        "email": "vana@example.com",
        "phone": "0901234567",
        "citizenId": "079198001234",
        "address": "Quận 3"
      },
      "status": "PENDING",
      "reviewReason": null,
      "submittedAt": "2026-07-01T02:00:00Z",
      "reviewedAt": null,
      "createdAt": "2026-07-01T02:00:00Z",
      "updatedAt": "2026-07-01T02:00:00Z",
      "attachments": [
        {
          "profileUpdateAttachmentId": "00000000-0000-0000-0000-000000000031",
          "originalFileName": "cccd.png",
          "contentType": "image/png",
          "length": 12345,
          "uploadedAt": "2026-07-01T02:00:00Z"
        }
      ]
    }

EmployeeNotificationDto:

    {
      "employeeNotificationId": "00000000-0000-0000-0000-000000000040",
      "profileUpdateRequestId": "00000000-0000-0000-0000-000000000030",
      "title": "Yêu cầu cập nhật được chấp thuận",
      "message": "Yêu cầu cập nhật hồ sơ của bạn đã được chấp thuận.",
      "createdAt": "2026-07-01T02:00:00Z",
      "readAt": null
    }

### 6.8 Các response khác

LifecycleOptionsDto:

    {
      "departments": [],
      "positions": []
    }

EmployeeStatisticsDto:

    {
      "totalEmployees": 10,
      "activeEmployees": 7,
      "lockedEmployees": 2,
      "resignedEmployees": 1,
      "fullTimeEmployees": 8,
      "partTimeEmployees": 2
    }

ContractAlertListDto, EmployeeListDto và EmployeeDirectoryListDto có dạng wrapper phân trang riêng mô tả ở trên. Những collection không phân trang trả JSON array.

Các endpoint tải minh chứng trả nội dung file, không phải JSON.

## 7. Mã lỗi và quy tắc nghiệp vụ

Status thường gặp:

| HTTP | Ý nghĩa |
|---|---|
| 200 | Đọc hoặc cập nhật thành công |
| 201 | Tạo hồ sơ/hợp đồng/phân công/yêu cầu thành công |
| 400 | Validation hoặc request không hợp lệ |
| 401 | Thiếu header định danh/quyền/service key |
| 403 | Header role/key không hợp lệ |
| 404 | Không tìm thấy hồ sơ, hợp đồng, alert hoặc file |
| 409 | Trùng mã, trạng thái đã thay đổi, concurrency hoặc bản ghi đang được sử dụng |
| 422 | Quy tắc nghiệp vụ không cho hoàn tất, ví dụ checklist bắt buộc chưa xong |
| 500 | Lỗi không dự kiến; response có code InternalError |
| 503 | Internal directory chưa có key trong môi trường không phải Development |

Status dùng trong domain:

| Nhóm | Giá trị |
|---|---|
| Employee | ACTIVE, LOCKED, RESIGNED |
| EmploymentType | FULL_TIME, PART_TIME |
| Organization | ACTIVE, INACTIVE |
| Contract | ACTIVE, RENEWED |
| Offboarding | IN_PROGRESS, COMPLETED |
| ContractAlert | NEW, VIEWED, IN_PROGRESS |
| Profile update | DRAFT, PENDING, NEED_INFO, APPROVED, REJECTED, CANCELLED |

Gender được chuẩn hóa thành Male, Female hoặc Other; API cũng nhận Nam/Nữ/Khác. Phone được chuẩn hóa số và mã +84/84 sang đầu 0. Employee code và mã phòng ban/chức vụ được trim và uppercase.

Concurrency được áp dụng cho employee, offboarding và contract alert. Với offboarding/alert, client đọc Version rồi gửi làm ExpectedVersion trong lệnh cập nhật. Mismatch trả 409 để client tải dữ liệu mới.

## 8. Razor Pages

HR API cũng map Razor Pages phục vụ giao diện HTML. Đây không phải JSON endpoints:

| Route | Trang |
|---|---|
| / | Danh sách/thêm/sửa hồ sơ nhân viên |
| /organization | Phòng ban và chức vụ |
| /employment | Hợp đồng và lịch sử công tác |
| /lifecycle | Onboarding/offboarding |
| /contract-expiry | Cảnh báo hết hạn hợp đồng |
| /profile-updates | Self-service cập nhật hồ sơ |
| /profile-update-review | HR duyệt cập nhật hồ sơ |

UI quản trị gọi các API quản trị bằng X-Role: HR. Self-service truyền X-Employee-Guid. Giao diện desktop Admin.WinForms cũng gọi cùng HR API qua HrApiClient; HR UI được tích hợp trong app Admin.

## 9. Database và dữ liệu file

HR dùng PostgreSQL qua HrDbContext. Migration nằm tại src/HR/HR.Infrastructure/Persistence/Migrations. Cột tiền dùng numeric(18,2). Employee là TPH trên bảng employee, dùng employment_type làm discriminator; base_salary dành cho FULL_TIME và part_time_salary dành cho PART_TIME.

File minh chứng của profile update lưu ở local filesystem, không lưu byte file trong PostgreSQL. Mặc định thư mục là LocalApplicationData/HRM/EmployeeUpdateEvidence; có thể đổi bằng Storage:EmployeeUpdateEvidencePath. Metadata, storage key và trạng thái request lưu trong database.

ContractExpiryJob tạo ContractAlert nền theo cấu hình. Lỗi trên từng hợp đồng được ghi log và xét lại ở lần quét sau; một hợp đồng lỗi không dừng các hợp đồng khác.

## 10. File tham khảo trong source

| Nội dung | File |
|---|---|
| Khởi động API, middleware, DI, migration startup | src/HR/HR.API/Program.cs |
| Error response | src/HR/HR.API/Middleware/ApiExceptionMiddleware.cs |
| Controller routes | src/HR/HR.API/Controllers/ |
| DTO request/response | src/HR/HR.Application/DTOs/ |
| Nghiệp vụ nhân viên | src/HR/HR.Application/Services/Employees/EmployeeService.cs |
| Tổ chức | src/HR/HR.Application/Services/Organization/OrgStructureService.cs |
| Hợp đồng/lịch sử công tác | src/HR/HR.Application/Services/Contracts/ContractService.cs |
| Lifecycle | src/HR/HR.Application/Services/Lifecycle/LifecycleService.cs |
| Profile update | src/HR/HR.Application/Services/ProfileUpdates/ProfileUpdateService.cs |
| Contract alerts | src/HR/HR.Application/Services/ContractAlerts/ContractAlertService.cs |
| EF model | src/HR/HR.Infrastructure/Persistence/HrDbContext.cs |
| API examples | src/HR/HR.API/HR.http |
| HR web pages | src/HR/HR.API/Pages/ |
| HR forms in desktop app | FE/Admin.WinForms/GUI/Controls/HR/ |
