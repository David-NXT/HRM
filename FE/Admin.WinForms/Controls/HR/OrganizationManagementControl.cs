using Admin.WinForms.Forms.HR;
using Admin.WinForms.Models.HR;

namespace Admin.WinForms.Controls.HR;

public sealed class OrganizationManagementControl : HrPageControl
{
    private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };
    private readonly DataGridView _departments = Table(("Mã", "Code"), ("Phòng ban", "Name"), ("Mô tả", "Description"), ("Trạng thái", "Status"));
    private readonly DataGridView _positions = Table(("Mã", "Code"), ("Chức vụ", "Name"), ("Mô tả", "Description"), ("Trạng thái", "Status"));
    private List<DepartmentRow> _departmentRows = [];

    public OrganizationManagementControl()
    {
        ActionButton("Làm mới", ReloadAsync);
        ActionButton("Thêm", () => EditAsync(false), true);
        ActionButton("Sửa", () => EditAsync(true));
        ActionButton("Ngừng hoạt động", DeactivateAsync);
        var departments = new TabPage("Phòng ban"); departments.Controls.Add(_departments);
        var positions = new TabPage("Chức vụ"); positions.Controls.Add(_positions);
        _tabs.TabPages.AddRange([departments, positions]);
        Body.Controls.Add(_tabs);
    }

    protected override async Task ReloadAsync()
    {
        var departments = Api.GetAsync<List<DepartmentRow>>("departments", Token);
        var positions = Api.GetAsync<List<PositionRow>>("positions", Token);
        await Task.WhenAll(departments, positions);
        Token.ThrowIfCancellationRequested();
        _departmentRows = await departments;
        _departments.DataSource = _departmentRows;
        _positions.DataSource = await positions;
        Status.Text = $"{_departmentRows.Count} phòng ban · {(await positions).Count} chức vụ. Chức vụ là vị trí công tác, không phải quyền đăng nhập.";
    }

    private async Task EditAsync(bool editing)
    {
        var isDepartment = _tabs.SelectedIndex == 0;
        var department = editing && isDepartment ? Selected<DepartmentRow>(_departments) : null;
        var position = editing && !isDepartment ? Selected<PositionRow>(_positions) : null;
        using var form = new HrEditorForm($"{(editing ? "Sửa" : "Thêm")} {(isDepartment ? "phòng ban" : "chức vụ")}");
        form.TextField("code", "Mã *", department?.Code ?? position?.Code, true, 20)
            .TextField("name", "Tên *", department?.Name ?? position?.Name, true, 150)
            .TextField("description", "Mô tả", department?.Description ?? position?.Description, maxLength: 500, multiline: true);
        if (isDepartment)
            form.ChoiceField("parent", "Phòng ban cha", new[] { new Choice(null, "Không có (cấp gốc)") }
                .Concat(_departmentRows.Where(x => x.DepartmentId != department?.DepartmentId)
                    .Select(x => new Choice(x.DepartmentId, x.DisplayName))), department?.ParentDepartmentId);
        form.OnSave(async (f, token) =>
        {
            if (isDepartment)
                await Api.SendAsync<DepartmentRow>(editing ? HttpMethod.Put : HttpMethod.Post,
                    editing ? $"departments/{department!.DepartmentId}" : "departments",
                    new DepartmentInput(f.TextValue("code"), f.TextValue("name"), f.OptionalText("description"), f.ChoiceValue("parent") as Guid?), token);
            else
                await Api.SendAsync<PositionRow>(editing ? HttpMethod.Put : HttpMethod.Post,
                    editing ? $"positions/{position!.PositionId}" : "positions",
                    new PositionInput(f.TextValue("code"), f.TextValue("name"), f.OptionalText("description")), token);
        });
        if (form.ShowDialog(this) == DialogResult.OK) await ReloadAsync();
    }

    private async Task DeactivateAsync()
    {
        if (_tabs.SelectedIndex == 0)
        {
            var row = Selected<DepartmentRow>(_departments);
            if (!Confirm($"Ngừng hoạt động phòng ban {row.Name}?")) return;
            await Api.SendAsync<DepartmentRow>(HttpMethod.Post, $"departments/{row.DepartmentId}/deactivate", null, Token);
        }
        else
        {
            var row = Selected<PositionRow>(_positions);
            if (!Confirm($"Ngừng hoạt động chức vụ {row.Name}?")) return;
            await Api.SendAsync<PositionRow>(HttpMethod.Post, $"positions/{row.PositionId}/deactivate", null, Token);
        }
        await ReloadAsync();
    }
}
