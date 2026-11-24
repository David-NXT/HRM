using Admin.WinForms.Forms.HR;
using Admin.WinForms.Models.HR;

namespace Admin.WinForms.Controls.HR;

public sealed partial class ProfileReviewControl : HrPageControl
{
    protected override Label PageStatus => Status;

    private List<ProfileRequestRow> _rows = [];
    private readonly Dictionary<Guid, ProfileRequestRow> _viewedRequests = [];
    private readonly HashSet<Guid> _staleRequests = [];

    public ProfileReviewControl()
    {
        InitializeComponent();
        _filter.Items.AddRange(new object[]
        {
            new Choice("PENDING", "PENDING"),
            new Choice("NEED_INFO", "NEED_INFO"),
            new Choice("APPROVED", "APPROVED"),
            new Choice("REJECTED", "REJECTED"),
            new Choice("CANCELLED", "CANCELLED"),
            new Choice("ALL", "Tất cả trạng thái")
        });
        _filter.SelectedIndex = 0;
        WireEvents();
    }

    private void WireEvents()
    {
        _refreshButton.Click += async (_, _) => await RunAsync(ReloadAsync);
        _viewButton.Click += async (_, _) => await RunAsync(ViewAsync);
        _downloadButton.Click += async (_, _) => await RunAsync(DownloadAsync);
        _approveButton.Click += async (_, _) => await RunAsync(() => ReviewAsync("APPROVE"));
        _rejectButton.Click += async (_, _) => await RunAsync(() => ReviewAsync("REJECT"));
        _requestInfoButton.Click += async (_, _) => await RunAsync(() => ReviewAsync("REQUEST_MORE_INFO"));
        _filter.SelectionChangeCommitted += async (_, _) => await RunAsync(ReloadAsync);
    }

    private ProfileRequestRow Current => _grid.CurrentRow?.Index is int index && index >= 0 && index < _rows.Count
        ? _rows[index] : throw new InvalidOperationException("Hãy chọn yêu cầu cập nhật hồ sơ.");

    protected override async Task ReloadAsync()
    {
        var filter = (_filter.SelectedItem as Choice)?.Value;
        var rows = await Api.GetAsync<List<ProfileRequestRow>>($"update-requests?status={filter}", Token);
        Token.ThrowIfCancellationRequested();
        _rows = rows;
        _viewedRequests.Clear();
        _staleRequests.Clear();
        _grid.DataSource = rows.Select(x => new { x.Changes.FullName, x.Status, x.SubmittedAt, x.ReviewReason }).ToList();
        Status.Text = $"{rows.Count} yêu cầu. Chỉ xử lý yêu cầu PENDING; mở đề xuất và minh chứng trước khi duyệt.";
    }

    // Đối chiếu đề xuất với hồ sơ hiện tại và ghi nhớ đúng phiên bản HR đã xem trước khi cho phép chấp thuận.
    private async Task ViewAsync()
    {
        var request = await Api.GetAsync<ProfileRequestRow>($"update-requests/{Current.ProfileUpdateRequestId}", Token);
        var employee = await Api.GetAsync<EmployeeRow>($"employees/{request.EmployeeId}", Token);
        Token.ThrowIfCancellationRequested();
        using var form = new HrEditorForm($"Đề xuất của {employee.DisplayName}");
        var changes = request.Changes;
        form.TextField("changes", "Hiện tại → Đề xuất", string.Join(Environment.NewLine,
            $"Họ tên: {employee.FullName} → {changes.FullName}", $"Ngày sinh: {employee.DateOfBirth} → {changes.DateOfBirth}",
            $"Giới tính: {employee.Gender} → {changes.Gender}", $"Email: {employee.Email} → {changes.Email}",
            $"Điện thoại: {employee.Phone} → {changes.Phone}", $"CCCD: {employee.CitizenId} → {changes.CitizenId}",
            $"Địa chỉ: {employee.Address} → {changes.Address}"), maxLength: 5000, multiline: true, readOnly: true)
            .TextField("attachments", "Minh chứng", string.Join(Environment.NewLine, request.Attachments.Select(x => x.OriginalFileName)),
                maxLength: 5000, multiline: true, readOnly: true).ReadOnlyView();
        var stale = request.BaseEmployeeVersion != employee.Version;
        form.TextField("versions", "Đối chiếu hồ sơ", stale
                ? "Hồ sơ gốc đã thay đổi hoặc yêu cầu cũ chưa có phiên bản gốc. Hãy chọn Yêu cầu bổ sung để nhân viên đọc hồ sơ mới và gửi lại."
                : $"Phiên bản hồ sơ gốc: {request.BaseEmployeeVersion}; phiên bản yêu cầu: {request.Version}.",
                maxLength: 1000, multiline: true, readOnly: true);
        form.ShowDialog(this);
        _viewedRequests[request.ProfileUpdateRequestId] = request;
        if (stale) _staleRequests.Add(request.ProfileUpdateRequestId);
        else _staleRequests.Remove(request.ProfileUpdateRequestId);
        Status.Text = stale ? "Hồ sơ đã đổi. Hãy yêu cầu bổ sung trước khi chấp thuận." : "Đã xem đề xuất.";
    }

    // Gửi version của đề xuất đã xem; chỉ chấp thuận sau khi đối chiếu hồ sơ, để API chặn dữ liệu thay đổi đồng thời.
    private async Task ReviewAsync(string decision)
    {
        var row = Current;
        _viewedRequests.TryGetValue(row.ProfileUpdateRequestId, out var viewed);
        Ensure((viewed ?? row).Status == "PENDING", "Yêu cầu không còn chờ xử lý. Bấm Làm mới để kiểm tra.");
        if (decision == "APPROVE")
        {
            Ensure(viewed is not null, "Hãy bấm Xem đề xuất và đối chiếu hồ sơ trước khi chấp thuận.");
            Ensure(!_staleRequests.Contains(row.ProfileUpdateRequestId), "Hồ sơ gốc đã thay đổi. Hãy yêu cầu nhân viên bổ sung và gửi lại từ hồ sơ mới.");
        }
        var expectedVersion = (viewed ?? row).Version;
        using var form = new HrEditorForm(decision == "APPROVE" ? "Chấp thuận cập nhật hồ sơ"
            : decision == "REJECT" ? "Từ chối yêu cầu" : "Yêu cầu bổ sung");
        form.TextField("reason", "Lý do / Ghi chú", required: decision != "APPROVE", maxLength: 1000, multiline: true)
            .OnSave(async (f, token) => await Api.SendAsync<ProfileRequestRow>(HttpMethod.Post,
                $"update-requests/{row.ProfileUpdateRequestId}/review",
                new { Decision = decision, Reason = f.OptionalText("reason"), ExpectedVersion = expectedVersion }, token));
        if (form.ShowDialog(this) == DialogResult.OK) await ReloadAsync();
    }

    private async Task DownloadAsync()
    {
        var row = Current;
        Ensure(row.Attachments.Count > 0, "Yêu cầu này không có minh chứng.");
        using var select = new HrEditorForm("Chọn minh chứng");
        select.ChoiceField("file", "Tệp", row.Attachments.Select(x => new Choice(x.ProfileUpdateAttachmentId, x.OriginalFileName)));
        select.OnSave(async (f, token) =>
        {
            var attachment = row.Attachments.Single(x => x.ProfileUpdateAttachmentId == f.GuidValue("file"));
            using var save = new SaveFileDialog { FileName = Path.GetFileName(attachment.OriginalFileName), Filter = "Tất cả tệp|*.*" };
            if (save.ShowDialog(select) != DialogResult.OK) throw new InvalidOperationException("Chưa chọn vị trí lưu tệp.");
            var bytes = await Api.DownloadAsync(
                $"update-requests/{row.ProfileUpdateRequestId}/attachments/{attachment.ProfileUpdateAttachmentId}", token);
            await File.WriteAllBytesAsync(save.FileName, bytes, token);
        });
        Status.Text = select.ShowDialog(this) == DialogResult.OK ? "Đã lưu minh chứng." : "Đã hủy tải minh chứng.";
    }
}
