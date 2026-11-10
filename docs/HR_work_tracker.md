# HR Work Tracker

Ngày cập nhật: 10/11/2026

Tài liệu này theo dõi phần HR đã có trong source, phần còn thiếu theo [HR sequence diagrams](HR_Sequence_Diagrams_improvement.md), và các điểm kỹ thuật cần quyết định trước khi coi HR sẵn sàng cho các service khác.

Đọc [hướng dẫn toàn bộ HR Service](HR_service_guide.md) để hiểu từng folder/file, các luồng xử lý và code cốt lõi.

## Tổ chức lại HR theo 4 project – đã thực hiện

HR hiện theo cách phân lớp của Payroll, với 4 `.csproj` độc lập được đăng ký trong `HRM.slnx`:

| Project | Trách nhiệm và nơi tìm code |
| --- | --- |
| `src/HR/HR.API` | Controllers, Auth, Middleware, Pages, Program.cs, appsettings, launchSettings và HTTP mẫu. Đây là project khởi động. |
| `src/HR/HR.Application` | Services và DTOs chia theo Employees, Organization, Contracts, ProfileUpdates, Lifecycle, ContractAlerts; Interfaces chia Services/Repositories/Storage; Exceptions. |
| `src/HR/HR.Domain` | Entities và Constants; không phụ thuộc API, Application hoặc EF Core. |
| `src/HR/HR.Infrastructure` | Persistence gồm DbContext/factory/Configurations/Migrations; Repositories, Storage và Jobs. |

- Hướng tham chiếu: API → Application/Infrastructure; Application → Domain; Infrastructure → Application/Domain.
- `Program.cs` gọi `AddApplication()` và `AddInfrastructure(configuration)`; mỗi lớp có `DependencyInjection.cs` riêng để đăng ký dependency.
- Namespace đã khớp lớp/folder; đã cập nhật project reference của test và các đường dẫn trong tài liệu HR.
- Đã chuyển toàn bộ 9 migration cùng DbContext sang Infrastructure và cập nhật namespace/type name trong metadata; **không tạo migration mới hoặc thay đổi schema vì sắp xếp folder**.
- Pages vẫn chạy trong API; `FE/HR` là WinForms riêng. Các URL, payload GUID và nghiệp vụ hiện hữu được giữ.
- Các thư mục source cũ đã trống cùng bin/obj cũ ở gốc `src/HR` đã dọn; thư mục này hiện chỉ gồm 4 project trên.

Từ root repository, chạy HR bằng:

```powershell
dotnet run --project src/HR/HR.API/HR.API.csproj --launch-profile http
```

Connection string vẫn là `ConnectionStrings__HrDatabase`. Khi dùng EF CLI, chuyển tới `src/HR/HR.API` để factory đọc đúng appsettings:

```powershell
dotnet ef migrations has-pending-model-changes --project ../HR.Infrastructure/HR.Infrastructure.csproj --startup-project HR.API.csproj
# Khi cần cập nhật database đã chọn:
dotnet ef database update --project ../HR.Infrastructure/HR.Infrastructure.csproj --startup-project HR.API.csproj
```

Nghiệm thu sau tổ chức lại ngày 10/11/2026:

- `dotnet build HRM.slnx --no-restore`: thành công, không warning/error; gồm backend, test project và các FE WinForms.
- `dotnet test HRM.slnx --no-restore`: **HR 78/78, Payroll 9/9**, tổng 87 test pass, không skip. Thêm `ArchitectureTests.cs` kiểm tra hướng tham chiếu và khả năng tạo tất cả controller/service/job qua DI, không kết nối database.
- EF `has-pending-model-changes`: model khớp migration; `migrations list --no-connect` tìm được đủ 9 migration.
- Chạy đủ 9 migration vào PostgreSQL 18 tạm từ cấu trúc mới thành công. Không sửa database dự án.
- HTTP smoke: 7 trang Razor và OpenAPI trả `200`; employees/contract-alerts/internal directory/self-service thiếu xác thực trả `401`.
- HTTP với database tạm: tạo phòng ban/chức vụ → onboarding → đọc employee/directory/self-service và công tác → tạo hợp đồng → đọc danh sách cảnh báo → mở offboarding, hoàn thành checklist và xác nhận `RESIGNED`; lịch sử được giữ/đóng ngày, directory sau nghỉ trả `404`. Job tắt trong lần kiểm tra này; chưa nghiệm thu tương tác JavaScript bằng browser.
- Đã dừng HR.API kiểm tra/PostgreSQL tạm và xóa dữ liệu, script, log tạm sau nghiệm thu.

## Trạng thái các use case

| Use case | Trạng thái | Ghi chú |
| --- | --- | --- |
| HR01 – Hồ sơ nhân viên | Đã có bản cơ bản | CRUD hồ sơ, tìm kiếm, khóa mềm, GUID ổn định, validation và UI. |
| HR02 – Yêu cầu cập nhật thông tin cá nhân | Đã có bản cơ bản | Draft/submit/edit/cancel, HR review, file minh chứng, notification inbox và migration đã có. Cần hoàn thiện identity/auth, audit review và kiểm thử. |
| HR03 – Hợp đồng và quá trình công tác | Đã có bản cơ bản | CRUD hợp đồng, gia hạn, ghi nhận lịch sử theo transaction; UI tại `/employment`. |
| HR04 – Cơ cấu tổ chức | Đã có bản cơ bản | Phòng ban dạng cây, chức vụ, ngừng hoạt động mềm; UI tại `/organization`. |
| HR05 – Onboarding/offboarding | Đã có bản cơ bản | Onboarding nguyên tử; offboarding có checklist, kiểm tra ngày hiệu lực, đóng công tác và chuyển `RESIGNED`; API, UI `/lifecycle`, migration và test đã có. |
| HR06 – Cảnh báo hợp đồng hết hạn | Đã có bản cơ bản | Job hàng ngày, biên 0–30 ngày, unique `(contractId, endDate)`, log lỗi, API trạng thái/phân trang, UI `/contract-expiry` và link gia hạn HR03. |
| HR–Project contract | Đã có bản cơ bản | Directory ACTIVE theo GUID, tên phòng ban/chức vụ, shared service key, timeout và từ chối khi HR lỗi/không hợp lệ; có consumer contract tests. |
| HR–Payroll | Ngoài phạm vi đợt này | Người dùng chọn HR06 + HR–Project; chưa bổ sung hồ sơ lương/người phụ thuộc hay endpoint Payroll. |

## Đã triển khai trong source

- HR01 API và giao diện hồ sơ nhân viên.
- HR04 entity, migration, API danh sách/tạo/sửa/ngừng hoạt động, cây phòng ban và giao diện.
- HR03 hợp đồng, gia hạn, work history, transaction khi đổi công tác, API và giao diện.
- Employee directory đọc phòng ban/chức vụ có hiệu lực theo ngày hiện tại.
- HR04 kiểm tra nhân viên đang dùng phòng ban/chức vụ và work history hiện tại/đang mở trước khi ngừng hoạt động.
- HR02 self-service cho nhân viên; HR duyệt/chấp thuận/từ chối/yêu cầu bổ sung; thông báo nội bộ chỉ xuất hiện sau khi commit.
- HR02 lưu file minh chứng ngoài web root, chỉ cho PDF/JPG/JPEG/PNG, tối đa 5 file mỗi yêu cầu và 10 MB mỗi file.
- Thêm migrations `AddProfileUpdateWorkflow` và `AddProfileUpdateRequestConcurrency`; chưa áp dụng vào database.
- HR05 tạo nhân viên `ACTIVE` và công tác đầu tiên trong cùng transaction; dùng lại validation/chuẩn hóa HR01.
- HR05 lưu hồ sơ nghỉ việc và checklist do HR nhập; theo dõi từng mục bắt buộc/tùy chọn và ghi chú hoàn thành.
- Hoàn tất nghỉ việc kiểm tra các mục bắt buộc, ngày làm việc cuối cùng, đóng công tác theo ngày đó và chuyển nhân viên sang `RESIGNED` trong cùng transaction. Không xóa nhân viên hoặc lịch sử.
- Bổ sung version cho Employee và hồ sơ nghỉ việc; dữ liệu cũ hoặc cập nhật đồng thời trả `409`. HR03 không mở công tác mới cho nhân viên `RESIGNED`; HR01 không chuyển `RESIGNED` thành `LOCKED`.
- Thêm migration `20261110120837_AddEmployeeLifecycle`; chưa áp dụng vào database.
- HR06 quét ACTIVE trong 0–30 ngày tính cả ngày hết hạn, không tạo cảnh báo cho hợp đồng thiếu/ngày hạn sai hoặc RENEWED.
- HR06 dùng unique index và INSERT có kiểm tra lại hợp đồng + `ON CONFLICT DO NOTHING`; lỗi từng hợp đồng không retry trong cùng lần chạy.
- Thêm migration `20261110134410_AddContractExpiryAlerts`; chưa áp dụng vào database.
- API internal directory có filter service key; client Project đọc đủ tên tổ chức, kiểm tra GUID/status và từ chối khi timeout/5xx/JSON sai. Các nhánh tạo/sửa ProjectMember qua controller hoặc service đều kiểm tra HR.
- Cấu hình caller Attendance hỗ trợ cùng directory key để không làm hỏng tích hợp đang có khi HR bật key.
- Thống nhất toàn bộ khóa chính/ngoại HR sang GUID trong entity, DTO, controller, service/repository, UI và test. Dùng một EmployeeId chung; không còn EmployeeGuid riêng.
- Thêm migration `20261110143701_UnifyHrIdentifiersAsGuid`: tái sử dụng employee_guid cũ, ánh xạ các ID số còn lại, giữ quan hệ và hỗ trợ rollback. Đã thử trên PostgreSQL tạm, chưa áp dụng vào database dự án.

## Quy ước GUID đã thống nhất với nhóm

- 11 entity HR dùng khóa chính `Guid`, khóa ngoại `Guid`/`Guid?`; PostgreSQL dùng `uuid`. API trả GUID dưới dạng chuỗi JSON, route dùng `{id:guid}`, giao diện giữ chuỗi ID.
- EmployeeId quản trị và directory có cùng kiểu/cùng giá trị. Project/Attendance tiếp tục dùng GUID nhân viên hiện hữu; migration lấy employee_guid cũ làm employee_id mới.
- Bỏ field EmployeeGuid trùng nghĩa khỏi entity/DTO. Header demo `X-Employee-Guid` giữ tên để tương thích nhưng giá trị chính là EmployeeId.
- Page/PageSize/Total/Version/ExpectedVersion/SortOrder vẫn là số; CitizenId là CCCD và các mã nghiệp vụ vẫn giữ chuỗi.
- `hr_legacy_id_map` chỉ giữ số ID cũ phục vụ đối chiếu/rollback; không được dùng làm định danh nghiệp vụ mới. Migration lịch sử vẫn mô tả schema int cũ.
- Trước khi chuyển database dùng chung: sao lưu, thử bản sao, dừng process ghi HR, cập nhật caller quản trị còn gửi ID số, chạy migration rồi khởi động lại. Xem [hướng dẫn HR](HR_service_guide.md#11-chạy-service-migration-và-test).
- Các khóa nội bộ còn dùng int ở Payroll cần chủ service đó đồng bộ theo thống nhất nhóm; không nằm trong thay đổi HR này.

## HR05 – thao tác và API đã có

Mở `/lifecycle` từ trang hồ sơ HR để tiếp nhận nhân viên, tạo hồ sơ nghỉ việc, chỉnh ngày nghỉ/ghi chú, cập nhật checklist và xác nhận hoàn tất. Không có bước chờ Manager.

| Thao tác | Endpoint | Kết quả |
| --- | --- | --- |
| Lấy phòng ban/chức vụ đang hoạt động | `GET /lifecycle/options` | `200` |
| Tiếp nhận nhân viên mới | `POST /onboarding` | `201`, trả Employee GUID và công tác đầu tiên |
| Mở hồ sơ nghỉ việc | `POST /offboarding` | `200`, trạng thái `IN_PROGRESS` |
| Danh sách/xem hồ sơ nghỉ việc | `GET /offboarding?employeeId=&status=` và `GET /offboarding/{id}` | `200` |
| Sửa ngày nghỉ/ghi chú | `PUT /offboarding/{id}` | Gửi `expectedVersion` từ response gần nhất |
| Hoàn thành/mở lại hạng mục bàn giao | `PUT /offboarding/{id}/checklist/{itemId}` | Gửi `isCompleted`, `completionNote`, `expectedVersion` |
| Xác nhận nghỉ việc | `POST /offboarding/{id}/complete` | Gửi `expectedVersion`; thiếu mục bắt buộc trả `422`; chưa đến ngày nghỉ trả `409` |

- Checklist gồm 1–50 hạng mục do HR khai báo khi mở quy trình; không áp đặt danh sách nghiệp vụ cố định khi tài liệu chưa chốt.
- Chỉ có một hồ sơ `IN_PROGRESS` cho mỗi nhân viên; database có unique index bảo vệ khi tạo đồng thời.
- Có thể lên lịch ngày nghỉ tương lai. Nhân viên chỉ chuyển sang `RESIGNED` khi HR xác nhận hoàn tất vào ngày nghỉ hoặc sau đó; hiện chưa có job tự hoàn tất.
- Ngày nghỉ không được trước ngày bắt đầu của bất kỳ bản ghi công tác nào. Nếu đã có công tác tương lai sau ngày nghỉ dự kiến, API trả `409` thay vì xóa hoặc sửa ngầm lịch sử.
- Hoàn tất lại một hồ sơ `COMPLETED` trả kết quả đã lưu, không thực hiện nghỉ việc lần nữa. Hồ sơ hoàn tất không sửa checklist/ngày nghỉ được.
- HR01 vẫn cho phép tạo hồ sơ cơ bản riêng; HR05 là luồng tạo đầy đủ hồ sơ và công tác đầu tiên. Nhân viên HR01 chưa có lịch sử cũng có thể làm thủ tục nghỉ việc.

## Kiểm tra HR05 (mốc trước HR06)

- `dotnet test tests/HR.Tests/HR.Tests.csproj --no-restore`: **31/31 test pass**, không bỏ qua test (10/11/2026).
- Build HR thành công; `dotnet ef migrations has-pending-model-changes` xác nhận model khớp migration HR05.
- Test bao gồm chuẩn hóa hồ sơ và công tác đầu tiên, tham chiếu không hợp lệ/mã trùng, checklist, ngày nghỉ tương lai, bảo toàn lịch sử, dữ liệu cũ, xử lý lại và chặn công tác sau nghỉ việc.
- Test change tracker kiểm tra version của Employee/hồ sơ nghỉ việc và cấu hình unique index, không kết nối database.
- Chưa kiểm tra transaction rollback/concurrency trên PostgreSQL thật, chưa chạy UI trong browser và chưa apply migration vào DB đang dùng.

Áp dụng các migration còn chờ từ thư mục `src/HR/HR.API`, với connection string môi trường đã cấu hình:

```powershell
dotnet ef database update --project ../HR.Infrastructure/HR.Infrastructure.csproj --startup-project HR.API.csproj
```

## Việc cần cải thiện trước khi triển khai thật

### Bảo mật và dữ liệu

- Thay `X-Role: HR` bằng đăng nhập/authorization thật.
- Không dùng `X-Employee-Guid` làm danh tính đã xác thực; thay bằng claim từ hệ thống đăng nhập.
- HR–Project đã có shared directory key và DTO tối thiểu; cần vận hành key rotation/secret store/HTTPS và kiểm tra end-to-end thật. Payroll chưa nằm trong phạm vi.
- Bảo vệ CCCD và dữ liệu cá nhân trong API, log, file minh chứng và backup.
- Quyết định chính sách lưu trữ/xóa file, giới hạn file, loại file, antivirus scan và quyền tải; giới hạn hiện tại là giá trị mặc định kỹ thuật.
- Bổ sung lịch sử review bất biến (người xử lý, quyết định, lý do, thời điểm) thay vì chỉ lưu quyết định/lý do hiện tại trên request.
- Thêm dispatcher nếu yêu cầu thông báo qua email/SMS; hiện mới có notification inbox trong HR.

### Nghiệp vụ còn cần chốt

- Định nghĩa rõ quy tắc gia hạn BR-HR13; hiện code HR03 mới kiểm tra cùng nhân viên, hợp đồng trước đang hoạt động, một hợp đồng kế tiếp và ngày không chồng lấn.
- Chốt danh sách trường hồ sơ nào nhân viên được tự yêu cầu cập nhật và trường nào cần minh chứng.
- Chốt lưu trữ hồ sơ hợp đồng, mức lương hợp đồng và API Payroll được phép đọc.
- HR05 đã đóng công tác khi xác nhận nghỉ việc. Cần chốt thêm chính sách hợp đồng còn `ACTIVE` sau nghỉ việc để HR06 xử lý đúng; HR05 hiện giữ nguyên hợp đồng.
- Chốt checklist bàn giao chuẩn, mẫu checklist, chính sách hủy hồ sơ nghỉ việc và tái tuyển dụng; hiện HR nhập checklist trực tiếp, chưa có luồng hủy/tái tuyển.
- Cần luồng điều chỉnh/hủy công tác tương lai trong HR03 nếu ngày nghỉ dự kiến sớm hơn lịch phân công đã ghi.
- Chốt múi giờ nghiệp vụ và chính sách ngày bắt đầu/ngày nghỉ (BR-HR21); hiện các kiểm tra ngày dùng UTC, nhất quán với phần HR đang có.

### Kỹ thuật và nghiệm thu

- Đã bổ sung test HR05; cần mở rộng test quyền, transaction rollback/race condition trên PostgreSQL và các luồng HR02–HR04 chưa được kiểm thử đầy đủ.
- Quyết định migration được apply khi deploy như thế nào; đã chạy chuỗi migration HR và kiểm tra đổi GUID trên PostgreSQL tạm, chưa áp dụng vào database dự án.
- Chuẩn hóa lỗi và API contract/OpenAPI giữa HR, Project, Attendance và Payroll.
- Thêm audit trail cho thay đổi hồ sơ, hợp đồng, cơ cấu tổ chức và quyết định HR02.
- Kiểm thử/đồng bộ sâu hơn việc tạo công tác hoặc onboarding đồng thời với ngừng danh mục tổ chức; transaction Save không tự biến mọi kiểm tra đọc thành serializable.
- Kiểm tra cấu hình môi trường, backup/restore, logging và lưu trữ file khi chạy ngoài máy local.

## Giả định tạm thời của code

- HR02 dùng header `X-Employee-Guid` làm định danh demo vì project chưa có login employee. Header này có thể bị giả mạo và không được xem là bảo mật production.
- HR02 hiện gửi toàn bộ bộ trường hồ sơ cá nhân được phép sửa, không phải patch trường riêng lẻ; mã nhân viên, trạng thái và cơ cấu tổ chức không sửa được qua luồng này.
- File minh chứng lưu ngoài web root, tên file trên đĩa được sinh ngẫu nhiên và signature được kiểm tra; giới hạn 5 file, 10 MB mỗi file cùng PDF/JPG/PNG là mặc định kỹ thuật cần được nhóm xác nhận.
- Thông báo HR02 được ghi thành notification/outbox trong cùng transaction để chỉ xuất hiện sau commit; chưa có email/SMS dispatcher.
- Trường hợp HR02 duyệt cập nhật, service dùng chung validation của HR01 và không cho thay mã nhân viên/trạng thái qua request cá nhân.
- HR05 vẫn dùng quyền tạm `X-Role: HR`. Các hạng mục thu hồi tài khoản/tài sản nếu HR nhập checklist chỉ là xác nhận của HR, chưa tự gọi hệ thống khác để thu hồi.

## HR06 và HR–Project – cấu hình và nghiệm thu

### HR06

- Job `ContractExpiryJob` chạy mặc định lúc khởi động và mỗi ngày 01:00 UTC (08:00 UTC+7). App phải đang chạy; có thể đổi Enabled/RunOnStartup/DailyRunTimeUtc qua cấu hình.
- API: `GET /contract-alerts?status=&employeeId=&expiresFrom=&expiresTo=&page=1&pageSize=20`, `GET /contract-alerts/{id}`, `PATCH /contract-alerts/{id}` với `status` và `expectedVersion`.
- Trạng thái cảnh báo: `NEW`, `VIEWED`, `IN_PROGRESS`. Cảnh báo được giữ sau gia hạn; UI chỉ rõ đợt cũ và cho chuyển sang HR03 để xem/gia hạn.
- Chính sách sau nghỉ việc còn chưa chốt: HR06 hiện bám baseline trạng thái hợp đồng `ACTIVE`; không tự đổi contract của employee `RESIGNED` hoặc tự loại nó theo quy tắc mới.

### HR–Project

- HR cấu hình bí mật bằng `InternalServices__EmployeeDirectoryApiKey`; Project/Attendance dùng cùng giá trị ở `Services__HR__ApiKey`, URL HR ở `Services__HR__BaseUrl`.
- Khi đã cấu hình key, bắt buộc header `X-HR-Directory-Key`; `X-Role` không bypass được.
- Không có key: chỉ Development được fallback cơ chế role local cũ; môi trường khác trả `503` cho directory.
- Directory chỉ trả GUID, mã/tên, status và tên phòng ban/chức vụ. `LOCKED`, `RESIGNED` hoặc không có employee trả `404`; không có CCCD/lương/tài khoản ngân hàng/file riêng tư.
- Project timeout 10 giây, không xem lỗi HR/timeout/GUID sai/status sai/JSON sai là nhân viên hợp lệ.

### Mốc kiểm tra GUID trước khi tổ chức lại folder

- `dotnet test tests/HR.Tests/HR.Tests.csproj --no-restore`: **76/76 pass**, không skip; gồm HR06, auth directory, payload/client Project, nhánh Project từ chối employee không hợp lệ và kiểm tra GUID/model/constraint/migration SQL.
- HR, Project, Attendance build thành công; `dotnet ef migrations has-pending-model-changes` xác nhận model khớp migration.
- PostgreSQL 18 tạm: dữ liệu đủ 11 bảng và 15 FK, giữ employee_guid cũ, quan hệ nullable, rollback giữ nguyên dữ liệu gốc, thêm bản ghi UUID rồi rollback/nâng cấp lại giữ ID, sequence sau rollback hoạt động; GUID nhân viên rỗng bị từ chối và transaction rollback đúng. Server tạm đã dừng.
- HTTP smoke trước đổi GUID: 7 trang HR trả HTML `200`; `/contract-alerts` và `/internal/employees` thiếu xác thực trả `401`. Job đã tắt trong lần kiểm tra này, không truy cập database.
- [Hướng dẫn HR](HR_service_guide.md) đã kiểm tra link tương đối và danh mục file; bao quát các file HR/test hiện tại, nhóm Designer migrations được giải thích chung.
- Chưa apply migration lên database người dùng; chưa kiểm thử race nghiệp vụ trên PostgreSQL hoặc browser automation.

## Cách dùng tracker

- Cập nhật trạng thái use case sau mỗi mốc có thể build và migration tương ứng.
- Chỉ đánh dấu “Hoàn tất” khi các nhánh chính/lỗi, quyền, migration, kiểm thử và API contract đều được nghiệm thu.
- Ghi rõ các giả định chưa được chốt để không nhầm với quy tắc từ OOAD.
