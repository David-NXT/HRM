using Admin.WinForms.BUS.Models.HR;

namespace Admin.WinForms.GUI.Forms.HR;

// One reusable editor gives all HR dialogs the same validation and save behavior.
public partial class HrEditorForm : Form
{
    private readonly Dictionary<string, Control> _fields = new();
    private readonly HashSet<string> _required = new();
    private readonly CancellationTokenSource _lifetime = new();
    private Func<HrEditorForm, CancellationToken, Task>? _save;
    private bool _saving;

    public HrEditorForm() => InitializeComponent();
    public HrEditorForm(string title) : this() { Text = title; lblTitle.Text = title; }

    public HrEditorForm TextField(string key, string title, string? value = null, bool required = false,
        int maxLength = 500, bool multiline = false, bool readOnly = false)
    {
        var box = new TextBox { Text = value ?? "", MaxLength = maxLength, Multiline = multiline,
            ReadOnly = readOnly, Dock = DockStyle.Fill, ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None };
        AddField(key, title, box, multiline ? 100 : 30);
        if (required) _required.Add(key);
        return this;
    }

    public HrEditorForm DateField(string key, string title, DateOnly? value = null, bool optional = false)
    {
        var picker = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy",
            ShowCheckBox = optional, Value = (value ?? DateOnly.FromDateTime(DateTime.Today)).ToDateTime(TimeOnly.MinValue),
            Checked = !optional || value.HasValue, Dock = DockStyle.Fill, MinDate = new DateTime(1900, 1, 1) };
        AddField(key, title, picker, 30);
        return this;
    }

    public HrEditorForm ChoiceField(string key, string title, IEnumerable<Choice> choices, object? selected = null)
    {
        var box = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        foreach (var choice in choices) box.Items.Add(choice);
        var match = box.Items.Cast<Choice>().FirstOrDefault(x => Equals(x.Value, selected));
        box.SelectedItem = match ?? box.Items.Cast<Choice>().FirstOrDefault();
        AddField(key, title, box, 30);
        return this;
    }

    public HrEditorForm SetChoices(string key, IEnumerable<Choice> choices, object? selected = null)
    {
        var box = (ComboBox)_fields[key];
        box.BeginUpdate();
        try
        {
            box.Items.Clear();
            foreach (var choice in choices) box.Items.Add(choice);
            var match = box.Items.Cast<Choice>().FirstOrDefault(x => Equals(x.Value, selected));
            box.SelectedItem = match ?? box.Items.Cast<Choice>().FirstOrDefault();
        }
        finally
        {
            box.EndUpdate();
        }
        return this;
    }

    public HrEditorForm OnChoiceChanged(string key, Action<object?> changed)
    {
        ((ComboBox)_fields[key]).SelectionChangeCommitted += (_, _) => changed(ChoiceValue(key));
        return this;
    }

    public HrEditorForm AmountField(string key, string title, decimal value = 0m)
    {
        var amount = new NumericUpDown
        {
            DecimalPlaces = 2,
            Minimum = 0m,
            Maximum = 9_999_999_999_999_999.99m,
            Increment = 100_000m,
            ThousandsSeparator = true,
            Value = value,
            Dock = DockStyle.Fill
        };
        AddField(key, title, amount, 30);
        return this;
    }

    public HrEditorForm CheckField(string key, string title, bool value)
    {
        var box = new CheckBox { Text = title, Checked = value, AutoSize = true };
        AddField(key, "", box, 30);
        return this;
    }

    private void AddField(string key, string title, Control field, int height)
    {
        var row = fields.RowCount++;
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, height + 12));
        fields.Controls.Add(new Label { Text = title, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
        field.Margin = new Padding(5, 5, 5, 7);
        fields.Controls.Add(field, 1, row);
        _fields.Add(key, field);
    }

    public string TextValue(string key) => ((TextBox)_fields[key]).Text.Trim();
    public decimal AmountValue(string key) => ((NumericUpDown)_fields[key]).Value;
    public string? OptionalText(string key) => string.IsNullOrWhiteSpace(TextValue(key)) ? null : TextValue(key);
    public DateOnly DateValue(string key) => DateOnly.FromDateTime(((DateTimePicker)_fields[key]).Value);
    public DateOnly? OptionalDate(string key) => ((DateTimePicker)_fields[key]).Checked ? DateValue(key) : null;
    public object? ChoiceValue(string key) => (((ComboBox)_fields[key]).SelectedItem as Choice)?.Value;
    public string ChoiceStringValue(string key) => ChoiceValue(key) as string
        ?? throw new InvalidOperationException("Hãy chọn giá trị trong các danh sách bắt buộc.");
    public Guid GuidValue(string key) => ChoiceValue(key) is Guid id ? id : throw new InvalidOperationException("Hãy chọn đủ phòng ban, chức vụ hoặc nhân viên.");
    public bool CheckedValue(string key) => ((CheckBox)_fields[key]).Checked;

    public HrEditorForm OnSave(Func<HrEditorForm, CancellationToken, Task> save)
    {
        _save = save;
        return this;
    }

    public HrEditorForm ReadOnlyView()
    {
        btnSave.Visible = false;
        btnCancel.Text = "Đóng";
        return this;
    }

    private async void btnSave_Click(object? sender, EventArgs e)
    {
        if (_saving || _save is null) return;
        try
        {
            foreach (var key in _required)
                if (string.IsNullOrWhiteSpace(TextValue(key)))
                {
                    _fields[key].Focus();
                    throw new InvalidOperationException("Vui lòng nhập đầy đủ các trường bắt buộc (*).");
                }
            _saving = true;
            fields.Enabled = btnSave.Enabled = btnCancel.Enabled = false;
            lblError.Text = "Đang lưu...";
            await _save(this, _lifetime.Token);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException
            or System.Text.Json.JsonException or OperationCanceledException or IOException or UnauthorizedAccessException)
        {
            if (!IsDisposed) lblError.Text = ex.Message;
        }
        finally
        {
            _saving = false;
            if (!IsDisposed) fields.Enabled = btnSave.Enabled = btnCancel.Enabled = true;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_saving && DialogResult != DialogResult.OK) e.Cancel = true;
        base.OnFormClosing(e);
    }
}
