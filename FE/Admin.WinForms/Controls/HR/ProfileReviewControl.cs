using Admin.WinForms.Forms.HR;
using Admin.WinForms.Models.HR;

namespace Admin.WinForms.Controls.HR;

public sealed class ProfileReviewControl : HrPageControl
{
    private readonly ComboBox _filter = Choices(new[] { "PENDING", "NEED_INFO", "APPROVED", "REJECTED", "CANCELLED", "" }
        .Select(x => new Choice(x, x == "" ? "Tất cả trạng thái" : x)));
    private readonly DataGridView _grid = Table(("Họ tên đề xuất", "FullName"), ("Trạng thái", "Status"),
        ("Ngày gửi", "SubmittedAt"), ("Lý do xử lý", "ReviewReason"));
    private List<ProfileRequestRow> _rows = [];

    public ProfileReviewControl()
    {
        Toolbar.Controls.Add(_filter);
        ActionButton("Làm mới", ReloadAsync);
        ActionButton("Xem đề xuất", ViewAsync);
        ActionButton("Tải minh chứng", DownloadAsync);
        ActionButton("Chấp thuận", () => ReviewAsync("APPROVE"), true);
        ActionButton("Từ chối", () => ReviewAsync("REJECT"));
        ActionButton("Yêu cầu bổ sung", () => ReviewAsync("REQUEST_MORE_INFO"));
        Body.Controls.Add(_grid);
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
        _grid.DataSource = rows.Select(x => new { x.Changes.FullName, x.Status, x.SubmittedAt, x.ReviewReason }).ToList();
        Status.Text = $"{rows.Count} yêu cầu. Chỉ xử lý yêu cầu PENDING; mở đề xuất và minh chứng trước khi duyệt.";
    }

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
        form.ShowDialog(this);
        Status.Text = "Đã xem đề xuất.";
    }

    private async Task ReviewAsync(string decision)
    {
        var row = Current;
        Ensure(row.Status == "PENDING", "Yêu cầu không còn chờ xử lý. Bấm Làm mới để kiểm tra.");
        using var form = new HrEditorForm(decision == "APPROVE" ? "Chấp thuận cập nhật hồ sơ" : decision == "REJECT" ? "Từ chối yêu cầu" : "Yêu cầu bổ sung");
        form.TextField("reason", "Lý do / Ghi chú", required: decision != "APPROVE", maxLength: 1000, multiline: true)
            .OnSave(async (f, token) => await Api.SendAsync<ProfileRequestRow>(HttpMethod.Post,
                $"update-requests/{row.ProfileUpdateRequestId}/review", new { Decision = decision, Reason = f.OptionalText("reason") }, token));
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
            var bytes = await Api.DownloadAsync($"update-requests/{row.ProfileUpdateRequestId}/attachments/{attachment.ProfileUpdateAttachmentId}", token);
            await File.WriteAllBytesAsync(save.FileName, bytes, token);
        });
        Status.Text = select.ShowDialog(this) == DialogResult.OK ? "Đã lưu minh chứng." : "Đã hủy tải minh chứng.";
    }
}
