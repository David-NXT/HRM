# Rà soát trang web HR và hướng dẫn sử dụng

Ngày đối chiếu mã nguồn: 15/11/2026.

## 1. Phạm vi và cách kiểm tra

Tài liệu này ghi nhận các lỗi và điểm cần hoàn thiện ở bảy trang Razor trong `src/HR/HR.API/Pages`. Các thao tác được đối chiếu với controller, service và repository của HR.

Các tình huống giữ nhầm yêu cầu, lọc sai chức vụ khi mở bằng liên kết và mất nội dung checklist đã được kiểm tra bằng chính JavaScript của trang với dữ liệu và DOM mô phỏng. Việc này chưa thay thế kiểm thử trên trình duyệt cùng database thật. Các nhận xét về hộp thoại, trường bắt buộc và bố cục được xác định từ HTML/JavaScript.

**Các lỗi nghiệp vụ và trạng thái bên dưới chưa được sửa trong đợt Việt hóa giao diện.** Đợt Việt hóa chỉ đổi nhãn hiển thị, thông báo và cách trình bày giá trị mã thành tiếng Việt; giữ nguyên tên biến, thuộc tính JSON, API, enum và logic xử lý.

| Mức độ | Ý nghĩa |
|---|---|
| Ưu tiên cao | Có thể xử lý sai đối tượng hoặc mất nội dung chưa lưu |
| Ưu tiên vừa | Có thể chặn thao tác hợp lệ hoặc khiến giao diện và backend không thống nhất |
| Cần hoàn thiện/thống nhất | Giới hạn hiện tại, thiếu hướng dẫn hoặc quyết định nghiệp vụ |

## 2. Các lỗi cần xử lý

### WEB-HR-01 — Màn duyệt giữ yêu cầu cũ khi tải lại danh sách

- **Mức độ:** Ưu tiên cao.
- **Trang:** `/profile-update-review`.
- **Vị trí:** `Pages/ProfileUpdateReview.cshtml`, các hàm `load`, `showRequest` và sự kiện `submitReview`.
- **Tái hiện:** Mở yêu cầu A đang chờ duyệt; đổi bộ lọc hoặc tải lại danh sách; ô chọn yêu cầu trở về trống; bấm gửi quyết định.
- **Kết quả từ mô phỏng:** Trang vẫn gửi `POST /update-requests/A/review`. Biến `currentRequest` chưa được xóa khi danh sách thay đổi. Chọn lại dòng trống cũng chưa xóa yêu cầu đang giữ.
- **Ảnh hưởng:** Có thể xử lý yêu cầu khác với trạng thái lựa chọn trên màn hình. Nếu A vẫn chờ duyệt, backend có thể chấp nhận thao tác này.
- **Hướng xử lý:** Xóa lựa chọn, nội dung chi tiết và trạng thái yêu cầu khi đổi bộ lọc/tải lại/chọn trống. Chỉ bật thao tác duyệt khi có yêu cầu hợp lệ đang ở `PENDING`; kiểm soát kết quả tải về khi đổi lựa chọn liên tiếp.
- **Điểm liên quan:** Sau khi duyệt, trang mới xóa phần đề xuất và file, còn thông tin hồ sơ hiện tại/tóm tắt cũ có thể tiếp tục hiển thị. Nút quyết định cũng chưa bị khóa khi xem yêu cầu đã xử lý.

### WEB-HR-02 — Trang nhân viên giữ chế độ sửa yêu cầu đã hủy hoặc thuộc nhân viên trước

- **Mức độ:** Ưu tiên vừa.
- **Trang:** `/profile-updates`.
- **Vị trí:** `Pages/ProfileUpdates.cshtml`, `loadProfile`, `send` và các nút sửa/hủy trong `loadRequests`.
- **Tái hiện 1:** Sửa bản nháp A, hủy A, rồi thử lưu nội dung mới.
- **Tái hiện 2:** Sửa bản nháp của nhân viên A, đổi GUID và tải hồ sơ nhân viên B, rồi lưu.
- **Kết quả từ mô phỏng:** `editingRequestId` vẫn trỏ đến bản nháp cũ. Trang gửi `PUT` vào yêu cầu đã hủy hoặc thuộc nhân viên khác.
- **Ảnh hưởng:** Backend chặn bằng lỗi `409` hoặc `404`; người dùng khó tạo yêu cầu mới nếu chưa tải lại trang. Đây không phải bằng chứng cho phép sửa yêu cầu của người khác.
- **Hướng xử lý:** Thêm thao tác tạo mới/thoát chỉnh sửa; xóa trạng thái sửa và file đang chọn khi đổi nhân viên, hủy yêu cầu đang sửa hoặc bắt đầu một yêu cầu khác. Hiển thị rõ yêu cầu đang được chỉnh sửa.

### WEB-HR-03 — Lưu một dòng checklist làm mất thay đổi chưa lưu ở nơi khác

- **Mức độ:** Ưu tiên cao.
- **Trang:** `/lifecycle`, phần theo dõi nghỉ việc.
- **Vị trí:** `Pages/Lifecycle.cshtml`, `renderRecord` và nút lưu từng mục checklist.
- **Tái hiện:** Sửa ghi chú hồ sơ, sửa ghi chú/đánh dấu nhiều dòng checklist, sau đó chỉ bấm lưu một dòng.
- **Kết quả từ mô phỏng:** Sau phản hồi của API, `renderRecord(updated)` dựng lại toàn bộ chi tiết. Ghi chú hồ sơ và các dòng chưa lưu bị thay bằng dữ liệu đã lưu trên server.
- **Ảnh hưởng:** Mất nội dung đang nhập mà chưa có cảnh báo.
- **Hướng xử lý:** Giữ các chỉnh sửa chưa lưu khi cập nhật một dòng, hoặc tổ chức thao tác lưu toàn bộ với cơ chế API phù hợp. Hiển thị rõ trạng thái chưa lưu.
- **Cách dùng tạm:** Sửa và lưu từng phần riêng; tránh sửa đồng thời nhiều dòng trước khi lưu.

### WEB-HR-04 — Lọc chức vụ part-time chưa đúng khi mở trang hợp đồng bằng liên kết

- **Mức độ:** Ưu tiên vừa.
- **Trang:** `/employment?employeeId=...`, bao gồm liên kết từ cảnh báo hợp đồng.
- **Vị trí:** `Pages/Employment.cshtml`, `initialize` và `refreshPositionOptions`.
- **Tái hiện:** Mở liên kết có `employeeId` của một nhân viên part-time.
- **Kết quả từ mô phỏng:** Danh sách có thể vẫn hiện chức vụ cấp Giám đốc. Sau khi chọn nhân viên từ URL, trang chưa gọi lại phần lọc chức vụ. Nếu nhân viên nằm ngoài danh sách 100 kết quả đầu, trang bổ sung lựa chọn nhưng chưa bổ sung dữ liệu nhân viên vào mảng `employees`.
- **Ảnh hưởng:** Người dùng chọn được chức vụ mà backend sẽ từ chối khi lưu. Backend vẫn kiểm tra ràng buộc part-time chỉ giữ chức vụ cấp `STAFF`.
- **Hướng xử lý:** Đồng bộ dữ liệu nhân viên lấy từ liên kết và lọc lại chức vụ sau khi hoàn tất chọn nhân viên.

### WEB-HR-05 — Reset form tiếp nhận chưa đồng bộ danh sách chức vụ

- **Mức độ:** Ưu tiên vừa.
- **Trang:** `/lifecycle`, phần tiếp nhận nhân viên.
- **Vị trí:** `Pages/Lifecycle.cshtml`, sự kiện gửi `onboardingForm` và `updatePositionOptions`.
- **Tái hiện:** Chọn part-time rồi tiếp nhận thành công.
- **Kết quả từ mô phỏng:** Form trở về full-time nhưng danh sách chức vụ vẫn chỉ có các lựa chọn `STAFF` đã lọc cho part-time. Reset form chưa gọi lại phần lọc này.
- **Ảnh hưởng:** Lần thêm full-time tiếp theo có thể thiếu lựa chọn chức vụ phù hợp.
- **Hướng xử lý:** Đồng bộ lại toàn bộ trường phụ thuộc sau khi reset form.

### WEB-HR-06 — Hai ô lương đều bắt buộc dù chỉ dùng một loại

- **Mức độ:** Ưu tiên vừa.
- **Trang:** `/` và `/lifecycle`.
- **Vị trí:** `Pages/Index.cshtml` và `Pages/Lifecycle.cshtml`, các input `baseSalary`, `partTimeSalary`.
- **Tái hiện:** Chọn full-time, xóa giá trị ô lương part-time rồi lưu; hoặc thực hiện ngược lại.
- **Nguyên nhân:** Hai ô đều có `required`; chọn loại nhân viên chưa ẩn/vô hiệu hóa ô không sử dụng.
- **Ảnh hưởng:** Trình duyệt có thể chặn thao tác vì một trường không phù hợp vẫn bị coi là bắt buộc.
- **Phần đang đúng:** Khi tạo payload, JavaScript đã chọn trường lương phù hợp với loại nhân viên. Full-time là VND/tháng; part-time là đơn giá VND/giờ.
- **Hướng xử lý:** Chỉ cho nhập và kiểm tra ô lương tương ứng với loại nhân viên.

### WEB-HR-07 — Lỗi nhập liệu xuất hiện phía sau hộp thoại

- **Mức độ:** Ưu tiên vừa.
- **Trang:** `/`, các hộp thoại hồ sơ và cập nhật lương.
- **Vị trí:** `Pages/Index.cshtml`, `statusBox` và các sự kiện gửi form.
- **Tái hiện:** Mở hộp thoại thêm nhân viên, gửi mã nhân viên đã tồn tại hoặc dữ liệu không hợp lệ.
- **Nguyên nhân:** Hộp thoại vẫn mở nhưng thông báo được ghi vào vùng trạng thái ở trang phía sau.
- **Ảnh hưởng:** Người dùng dễ tưởng nút lưu không hoạt động.
- **Hướng xử lý:** Đặt thông báo ngay trong hộp thoại và chỉ rõ trường không hợp lệ khi có đủ thông tin lỗi.

### WEB-HR-08 — Danh tính và vai trò đang là cơ chế mô phỏng

- **Loại:** Cần hoàn thiện ở luồng đăng nhập/phân quyền.
- **Vị trí:** `Pages/ProfileUpdates.cshtml`, các hàm gọi API ở trang quản trị, `Auth/EmployeeIdentityFilter.cs`, `Auth/HrRoleFilter.cs`.
- **Hiện trạng:** Nhân viên nhập GUID thủ công; trang HR tự gửi `X-Role: HR`. Filter nhân viên kiểm tra GUID tồn tại và trạng thái `ACTIVE`; filter HR kiểm tra header vai trò. Chưa có phiên đăng nhập ràng buộc các thông tin này với người đang dùng trình duyệt.
- **Ảnh hưởng:** Hai actor chưa được phân tách bằng đăng nhập thực. Trang nhân viên cũng có liên kết đến các trang quản trị.
- **Hướng xử lý:** Thống nhất tích hợp danh tính với phần tài khoản/phân quyền của project, rồi điều hướng và cấp quyền tương ứng. Chỉ ẩn nút trên giao diện không thay thế việc kiểm tra quyền ở backend.

## 3. Các giới hạn và điểm cần thống nhất

| Mã | Hiện trạng | Hướng hoàn thiện |
|---|---|---|
| WEB-HR-09 | Trang chủ và một số ô chọn chỉ lấy trang 1, tối đa 100 nhân viên | Bổ sung phân trang hoặc tìm/chọn nhân viên có hướng dẫn rõ |
| WEB-HR-10 | Bản nháp vẫn được kiểm tra đầy đủ thông tin cá nhân như yêu cầu gửi duyệt | Ghi rõ đây là thông tin hợp lệ chưa gửi; chỉ cho lưu form dở khi đã thống nhất và điều chỉnh API |
| WEB-HR-11 | Thêm nhân viên ở trang chủ chỉ tạo hồ sơ; tiếp nhận ở `/lifecycle` tạo cả hồ sơ và phân công đầu tiên | Giải thích rõ hai thao tác hoặc thống nhất một lối tiếp nhận chính |
| WEB-HR-12 | Có khóa hồ sơ nhưng chưa có thao tác mở khóa tương ứng | Thống nhất nghiệp vụ khôi phục hồ sơ trước khi bổ sung giao diện và API |
| WEB-HR-13 | Ngày mặc định của trang tiếp nhận dùng `new Date().toISOString().slice(0, 10)` | Nếu dùng ngày Việt Nam, lấy ngày theo múi giờ đã thống nhất; ví dụ 02:00 ngày 15/11 ở UTC+7 cho chuỗi UTC là ngày 14/11 |
| WEB-HR-14 | Một số hàm gọi API chỉ đọc `message`, chưa trình bày chi tiết lỗi kiểm tra dữ liệu theo từng trường | Hiển thị lỗi dễ hiểu, trong đúng form; việc dịch thông báo không tự bổ sung chi tiết validation |
| WEB-HR-15 | Màn duyệt hiển thị mọi thuộc tính hồ sơ hiện tại và đánh dấu cùng kiểu, chưa chỉ rõ trường thực sự thay đổi | Đối chiếu cùng tập trường và chỉ làm nổi bật các giá trị khác nhau |
| WEB-HR-16 | Form sửa yêu cầu chưa hỗ trợ bỏ file minh chứng đã đính kèm; API hiện chỉ bổ sung file | Thống nhất nghiệp vụ thay/xóa file và giới hạn tổng số file trước khi bổ sung |
| WEB-HR-17 | Màn tổ chức vẫn có thể cho sửa mã/cấp hoặc bấm ngừng hoạt động chức vụ hệ thống `SYS_STAFF`, trong khi backend bảo vệ chức vụ này | Khóa/giải thích các thao tác không được phép trên chức vụ hệ thống |

Ngôn ngữ giao diện được Việt hóa trong đợt này. Dữ liệu người dùng đã nhập, tên file và các mã lưu trữ/gửi API không bị đổi theo ngôn ngữ hiển thị.

## 4. Hướng dẫn sử dụng các trang

HR API và database cần được cấu hình, chạy thành công. Địa chỉ HTTP mặc định là `http://localhost:5012`. Cấu hình hiện tại tự áp dụng migration lúc API khởi động; nếu khởi động thất bại thì cần xử lý lỗi cấu hình/database trước khi dùng web.

| Đường dẫn | Actor | Chức năng |
|---|---|---|
| `/` | HR | Tìm nhân viên, thêm hồ sơ, sửa thông tin cá nhân, cập nhật lương, khóa hồ sơ |
| `/organization` | HR | Quản lý phòng ban, cây tổ chức, chức vụ và cấp bậc |
| `/lifecycle` | HR | Tiếp nhận nhân viên; mở và theo dõi hồ sơ nghỉ việc, checklist bàn giao |
| `/employment` | HR | Quản lý hợp đồng, gia hạn, phân công/điều chuyển và lịch sử công tác |
| `/contract-expiry` | HR | Lọc/xem cảnh báo hợp đồng, cập nhật trạng thái xử lý, mở trang gia hạn |
| `/profile-updates` | Nhân viên | Xem hồ sơ của mình, tạo/sửa/gửi/hủy yêu cầu cập nhật, tải minh chứng, xem thông báo |
| `/profile-update-review` | HR | Xem hồ sơ hiện tại và đề xuất, tải minh chứng, duyệt/từ chối/yêu cầu bổ sung |

### 4.1. Chuẩn bị tổ chức và tiếp nhận

1. Vào `/organization`, tạo phòng ban đang hoạt động và chức vụ phù hợp. Chức vụ part-time phải thuộc cấp nhân viên (`STAFF`).
2. Vào `/lifecycle`, nhập hồ sơ nhân viên, loại nhân viên, mức lương, ngày bắt đầu và phân công đầu tiên.
3. Với toàn thời gian, mức lương là VND/tháng. Với bán thời gian, mức nhập là đơn giá VND/giờ.
4. Bấm xác nhận tiếp nhận. Nếu chọn ngày bắt đầu trong tương lai, phân công chưa trở thành phân công hiện tại ngay lúc tạo.
5. Tra lại nhân viên ở trang chủ.

Nếu dùng nút thêm nhân viên tại trang chủ, chỉ hồ sơ được tạo. Có thể phân công sau ở `/employment`.

### 4.2. Hợp đồng và lịch sử công tác

1. Vào `/employment`, tìm rồi chọn nhân viên.
2. Nhập số hợp đồng duy nhất, loại hợp đồng, ngày bắt đầu và ngày kết thúc nếu có. Ngày kết thúc phải sau ngày bắt đầu.
3. Để gia hạn, tạo hợp đồng mới, chọn hợp đồng trước ở ô gia hạn và chọn ngày bắt đầu sau thời gian của hợp đồng trước. Chỉ hợp đồng đang hoạt động được gia hạn.
4. Để phân công/điều chuyển, chọn phòng ban, chức vụ và ngày hiệu lực. Ngày hiệu lực phải sau giai đoạn công tác gần nhất theo kiểm tra của backend.
5. Không ghi nhận công tác mới cho nhân viên đã nghỉ việc. Phân công Giám đốc và part-time vẫn phải thỏa ràng buộc của backend.

### 4.3. Nhân viên đề xuất cập nhật, HR xét duyệt

1. Lấy mã định danh GUID của nhân viên từ hồ sơ. Nhập vào `/profile-updates` rồi bấm tải hồ sơ. Đây là cơ chế nhận diện thử nghiệm hiện tại; GUID khác mã nhân viên.
2. Sửa các thông tin cá nhân được phép: họ tên, ngày sinh, giới tính, email, điện thoại, CCCD/CMND, địa chỉ.
3. Nếu cần, đính kèm minh chứng PDF/JPG/PNG. Tổng số file tối đa là 5, mỗi file tối đa 10 MB; backend còn kiểm tra file thực tế.
4. Lưu nháp nếu chưa gửi; gửi yêu cầu khi muốn HR xử lý. Việc lưu/gửi chưa đổi hồ sơ chính thức.
5. HR mở `/profile-update-review`, chọn yêu cầu chờ duyệt, đối chiếu nội dung và minh chứng rồi chọn quyết định.
6. Từ chối hoặc yêu cầu bổ sung phải có lý do. Chỉ yêu cầu đang chờ duyệt được xử lý.
7. Nhân viên tải lại hồ sơ để xem trạng thái và thông báo. Hồ sơ chính thức chỉ được cập nhật khi HR chấp thuận.

| Mã API | Nhãn hiển thị tiếng Việt | Thao tác chính |
|---|---|---|
| `DRAFT` | Bản nháp | Nhân viên sửa, gửi hoặc hủy |
| `PENDING` | Chờ duyệt | HR xử lý; nhân viên có thể hủy nếu yêu cầu vẫn ở trạng thái này |
| `NEED_INFO` | Cần bổ sung thông tin | Nhân viên sửa/bổ sung và gửi lại |
| `APPROVED` | Đã chấp thuận | Xem kết quả; hồ sơ đã được cập nhật |
| `REJECTED` | Đã từ chối | Xem lý do xử lý |
| `CANCELLED` | Đã hủy | Xem lại trạng thái; không tiếp tục sửa yêu cầu đã hủy |

### 4.4. Nghỉ việc và bàn giao

1. Vào `/lifecycle`, chọn nhân viên, ngày làm việc cuối cùng và tạo checklist từ 1 đến 50 mục không trùng tên.
2. Tạo hồ sơ nghỉ việc chưa chuyển nhân viên sang đã nghỉ việc ngay.
3. Trong phần theo dõi, chọn hồ sơ, cập nhật ngày/ghi chú hoặc hoàn thành từng mục bàn giao và lưu.
4. Chỉ xác nhận hoàn tất khi đã đến ngày làm việc cuối và mọi mục bắt buộc đều hoàn thành.
5. Khi hoàn tất, trạng thái nhân viên chuyển thành `RESIGNED`, hồ sơ nghỉ việc thành `COMPLETED` và quá trình công tác được đóng theo ngày nghỉ. Lịch sử được giữ lại.

### 4.5. Cảnh báo hợp đồng

- Job xét các hợp đồng `ACTIVE`, có ngày hết hạn hợp lệ và còn từ 0 đến 30 ngày đến hạn.
- Cấu hình mặc định quét khi API khởi động và hằng ngày lúc 01:00 UTC, tức 08:00 tại Việt Nam.
- Tạo hợp đồng mới sau lần quét không bảo đảm có cảnh báo ngay. Thao tác lọc/tải danh sách không tạo lần quét mới.
- Hợp đồng đã quá hạn trước khi được quét không được tạo cảnh báo mới bởi điều kiện 0–30 ngày. Cảnh báo đã có từ trước có thể tiếp tục hiển thị là quá hạn.
- Vào `/contract-expiry`, lọc cảnh báo, bấm xem, chọn trạng thái xử lý rồi lưu. Mở chi tiết không tự động đánh dấu đã xem.
- Liên kết gia hạn đưa sang `/employment` với nhân viên/hợp đồng tương ứng. Cảnh báo đợt cũ vẫn được giữ để tra cứu khi hợp đồng đã đổi.

## 5. Thứ tự xử lý đề xuất

1. Chặn thao tác trên yêu cầu cũ/không còn được chọn; đồng bộ trạng thái khi đổi nhân viên.
2. Giữ nội dung chưa lưu trong hồ sơ và checklist bàn giao.
3. Đồng bộ lựa chọn chức vụ khi mở bằng liên kết hoặc reset form; điều chỉnh ô lương theo loại nhân viên.
4. Hiển thị lỗi tại đúng vị trí, bổ sung hướng dẫn tạo hồ sơ/tiếp nhận và phân trang.
5. Thống nhất luồng đăng nhập, định nghĩa bản nháp và các nghiệp vụ khôi phục/xóa file trước khi mở rộng API.

## 6. Kiểm tra sau khi sửa logic trong tương lai

- Chọn yêu cầu rồi đổi bộ lọc/tải lại/chọn trống: không được gửi quyết định cho lựa chọn cũ.
- Đổi GUID hoặc hủy yêu cầu đang sửa: lần tạo mới không gửi vào ID yêu cầu cũ.
- Sửa nhiều vị trí trên hồ sơ bàn giao: lưu một vị trí không làm mất phần chưa lưu còn lại.
- Mở link nhân viên part-time có/không có trong 100 kết quả đầu: chỉ hiện chức vụ được phép.
- Tiếp nhận part-time thành công rồi thêm full-time: danh sách chức vụ phải đúng loại nhân viên hiện tại.
- Bỏ trống ô lương không áp dụng: không chặn form vì ô đó; payload vẫn giữ đúng khóa JSON và đơn vị.
- Nhập trùng mã hoặc dữ liệu không hợp lệ trong hộp thoại: nhìn thấy thông báo ngay trong hộp thoại.
- Giao diện tiếng Việt không làm thay đổi ID control, route, method HTTP, thuộc tính request hoặc giá trị enum gửi API.
