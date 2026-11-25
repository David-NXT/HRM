# Hướng dẫn giao diện Attendance và Payroll

Tài liệu này mô tả các màn hình WinForms, chức năng nút và luồng gọi API hiện có.

## 1. Khởi chạy và điều hướng

- **Admin WinForms**: chọn **Attendance** hoặc **Payroll** ở thanh điều hướng chính. Các nút này mở `AttendanceOverviewControl` và `PayrollOverviewControl`.
- **Attendance WinForms**: ứng dụng riêng cho nhân viên, có các mục **Chấm công**, **Đơn từ** và **Nội quy**.
- Admin kết nối Attendance API tại `http://localhost:5231/` và Payroll API tại `http://localhost:5267/`. Payroll URL có thể ghi đè bằng biến môi trường `Services__Payroll__BaseUrl`.

Vị trí mã: [MainForm.cs](../FE/Admin.WinForms/MainForm.cs#L133), [MainForm.cs](../FE/Attendance.WinForms/MainForm.cs#L119), [ApiConfig.cs](../FE/Admin.WinForms/Config/ApiConfig.cs#L8), [Program.cs](../FE/Admin.WinForms/Program.cs#L18).

## 2. Attendance trong Admin WinForms

Thanh điều hướng Attendance có bốn trang:

| Trang/nút | Chức năng và API |
|---|---|
| **Tổng quan** | Tải danh sách chấm công và thống kê theo ngày. Tìm kiếm gửi `POST /api/Attendance/search`; tải danh sách dùng `GET /api/Attendance`; thống kê dùng `GET /api/attendance/analytics/overview`. |
| **Phê duyệt** | Tải các đơn giải trình và OT; lọc danh sách trên giao diện; **Duyệt/Từ chối** cập nhật đơn tương ứng bằng `PUT /api/AttendanceExplanations/{id}` hoặc `PUT /api/OTRequests/{id}`. |
| **Báo cáo** | Tạo báo cáo theo kỳ/năm, tải tổng hợp chấm công và biểu đồ. Dùng `GET /api/Timesheets/periods`, `GET /api/Timesheets/periods/{id}/summaries`, `GET /api/attendance/analytics/trends` và `/departments`. |
| **Cấu hình** | Tải, thêm, sửa, xóa ca làm, quy tắc chấm công và quy tắc OT qua các API `WorkShifts`, `AttendanceRules`, `OvertimeRules`. |

Nút **Xuất Excel/CSV** xuất dữ liệu cục bộ, không gọi API riêng. Nút **In** ở trang Tổng quan chỉ hiển thị thông báo; nút **Xuất PDF** ở trang Báo cáo cũng đang là placeholder, chưa tạo tệp PDF.

Vị trí mã: điều hướng và tạo trang con ở [AttendanceOverviewControl.cs](../FE/Admin.WinForms/Controls/Attendance/AttendanceOverviewControl.cs#L18); thao tác Tổng quan ở [AttendanceOverviewPageControl.cs](../FE/Admin.WinForms/Controls/Attendance/AttendanceOverviewPageControl.cs#L243); phê duyệt ở [AttendanceApprovalControl.cs](../FE/Admin.WinForms/Controls/Attendance/AttendanceApprovalControl.cs#L56); báo cáo ở [AttendanceReportControl.cs](../FE/Admin.WinForms/Controls/Attendance/AttendanceReportControl.cs#L99); cấu hình ở [AttendanceConfigControl.cs](../FE/Admin.WinForms/Controls/Attendance/AttendanceConfigControl.cs#L72).

## 3. Attendance trong ứng dụng nhân viên

| Mục/nút | Chức năng và API |
|---|---|
| **Chấm công** | Tìm nhân viên, chọn ca, tải bản ghi hôm nay; **Check-in** tạo bản ghi bằng `POST /api/Attendance`, **Check-out** cập nhật bằng `PUT /api/Attendance/{id}`. Dữ liệu nhân viên/ca tải từ `GET /api/employees` và `GET /api/WorkShifts`. |
| **Đơn từ** | Gửi đơn nghỉ phép, đăng ký OT, giải trình chấm công; các nút gửi lần lượt gọi `POST /api/LeaveRequests`, `POST /api/OTRequests`, `POST /api/AttendanceExplanations`. **Tải lại lịch sử** tải lại các danh sách đơn từ API. |
| **Nội quy** | Hiển thị nội dung hướng dẫn trong ứng dụng; hiện không gọi API. |

Vị trí mã: điều hướng ở [MainForm.cs](../FE/Attendance.WinForms/MainForm.cs#L119); check-in/out ở [AttendanceCheckInControl.cs](../FE/Attendance.WinForms/Controls/AttendanceCheckInControl.cs#L215); gửi đơn và tải lịch sử ở [AttendanceRequestsControl.cs](../FE/Attendance.WinForms/Controls/AttendanceRequestsControl.cs#L182); client và ánh xạ route ở [AttendanceApiService.cs](../FE/Attendance.WinForms/Services/AttendanceApiService.cs#L27).

**Lưu ý:** `AttendanceCheckInControl` và `AttendanceRequestsControl` thuộc ứng dụng Attendance dành cho nhân viên. Menu Attendance của Admin hiện mở các trang Tổng quan, Phê duyệt, Báo cáo và Cấu hình.

## 4. Payroll trong Admin WinForms

Thanh Payroll có bốn trang:

| Trang/nút | Chức năng và API |
|---|---|
| **Bảng lương** | Chọn kỳ để tải bảng lương. **Mở kỳ lương** gọi `POST /api/payroll-periods`; **Chốt & Đồng Bộ Công** gọi `POST /api/payroll-attendance/periods/{id}/sync`; **Đồng bộ lương** gọi `POST /api/payroll-salary/periods/{id}/sync`, lấy lương từ HR và tổng hợp phụ cấp từ Project API (`GET /api/PayrollData/summary`) theo kỳ, lưu vào `TotalProjectAllowance`; bảng lương hiển thị cột **Phụ cấp dự án**. **Tạo bảng lương** lấy nhân viên từ Attendance rồi gọi `POST /api/payroll-management/periods/{id}/payrolls`; **Tính lương** gọi `POST /api/payroll-calculation/{id}/calculate` cho từng nhân viên, tự thêm phụ cấp thâm niên theo danh mục đã cấu hình và khấu trừ theo số lần đi trễ trong bảng công đã khóa, rồi tính Gross/Net. **Xuất Excel/CSV** ghi dữ liệu hiện có ra tệp, không gọi API xuất. |
| **Phụ cấp & Khấu trừ** | Chọn kỳ/nhân viên để tải chi tiết; thêm phụ cấp gọi `POST /api/payroll-adjustments/{id}/allowances`, thêm khấu trừ gọi `POST /api/payroll-adjustments/{id}/paycuts`. |
| **Phê duyệt & Quyết toán** | Duyệt hoặc đánh dấu đã chi trả cho nhân viên được chọn/toàn kỳ bằng `POST /api/payroll-approval/{id}/approve` và `POST /api/payroll-payment/{id}/mark-as-paid`; khóa kỳ cập nhật trạng thái kỳ lương. |
| **Phiếu lương** | Chọn kỳ/nhân viên để tải phiếu bằng `GET /api/payslips/{id}`. **Xuất phiếu** lưu nội dung cục bộ; nút **In** hiện chỉ hiển thị thông báo, chưa thực hiện in thực tế. |

Nút **Chốt & Đồng Bộ Công** được khai báo trong Designer và gắn trực tiếp với handler `button_SyncAttendance_Click`; handler chuyển tiếp đến xử lý đồng bộ, kiểm tra kỳ lương, hỏi xác nhận rồi gọi Payroll API.

Vị trí mã: điều hướng các trang ở [PayrollOverviewControl.Designer.cs](../FE/Admin.WinForms/Controls/Payroll/PayrollOverviewControl.Designer.cs#L60); các thao tác bảng lương và handler đồng bộ ở [PayrollTableControl.cs](../FE/Admin.WinForms/Controls/Payroll/PayrollTableControl.cs#L207); khai báo nút và đăng ký sự kiện ở [PayrollTableControl.Designer.cs](../FE/Admin.WinForms/Controls/Payroll/PayrollTableControl.Designer.cs#L299); phụ cấp/khấu trừ ở [PayrollAdjustmentControl.cs](../FE/Admin.WinForms/Controls/Payroll/PayrollAdjustmentControl.cs#L186); phê duyệt/chi trả ở [PayrollApprovalPaymentControl.cs](../FE/Admin.WinForms/Controls/Payroll/PayrollApprovalPaymentControl.cs#L178); phiếu lương ở [PayslipViewerControl.cs](../FE/Admin.WinForms/Controls/Payroll/PayslipViewerControl.cs#L112).

## 5. API client và bố cục

- Admin dùng [AttendanceApiService.cs](../FE/Admin.WinForms/Services/Attendance/AttendanceApiService.cs#L19) cho Attendance; Payroll được chia thành các service nghiệp vụ trong `Services/Payroll` và lớp truy cập API [PayrollApiClient.cs](../FE/Admin.WinForms/Data/Payroll/PayrollApiClient.cs).
- Khi đồng bộ mức lương, Payroll API gọi [PayrollData/summary của Project API](../src/Project/Project.API/Controllers/PayrollDataController.cs#L57); địa chỉ local mặc định là `http://localhost:5032/` trong [appsettings.Development.json](../src/Payroll/Payroll.API/appsettings.Development.json). Môi trường khác cấu hình `ProjectService__BaseUrl`.
- Thanh điều hướng Attendance/Payroll được phép tự xuống dòng khi cửa sổ hẹp.
