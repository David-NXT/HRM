#nullable disable
namespace Admin.WinForms.Forms.HR;

partial class HrEditorForm
{
    protected override void Dispose(bool disposing)
    {
        if (disposing && !IsDisposed) _lifetime.Cancel();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        layout = new TableLayoutPanel();
        lblTitle = new Label();
        scroll = new Panel();
        fields = new TableLayoutPanel();
        lblError = new Label();
        buttons = new FlowLayoutPanel();
        btnSave = new Button();
        btnCancel = new Button();
        SuspendLayout();
        layout.Dock = DockStyle.Fill;
        layout.Padding = new Padding(20);
        layout.ColumnCount = 1;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowCount = 4;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        lblTitle.Text = "Thông tin HR";
        lblTitle.Font = new Font("Segoe UI", 15, FontStyle.Bold);
        lblTitle.AutoSize = true;
        lblTitle.Margin = new Padding(0, 0, 0, 16);
        scroll.Dock = DockStyle.Fill;
        scroll.AutoScroll = true;
        fields.Dock = DockStyle.Top;
        fields.AutoSize = true;
        fields.ColumnCount = 2;
        fields.RowCount = 0;
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        scroll.Controls.Add(fields);
        lblError.AutoSize = true;
        lblError.MaximumSize = new Size(640, 0);
        lblError.ForeColor = Color.Firebrick;
        lblError.Margin = new Padding(0, 10, 0, 10);
        buttons.Dock = DockStyle.Fill;
        buttons.AutoSize = true;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        btnSave.Text = "Lưu";
        btnSave.Size = new Size(100, 36);
        btnSave.BackColor = Color.FromArgb(35, 83, 166);
        btnSave.ForeColor = Color.White;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Click += btnSave_Click;
        btnCancel.Text = "Hủy";
        btnCancel.Size = new Size(100, 36);
        btnCancel.DialogResult = DialogResult.Cancel;
        buttons.Controls.Add(btnSave);
        buttons.Controls.Add(btnCancel);
        layout.Controls.Add(lblTitle, 0, 0);
        layout.Controls.Add(scroll, 0, 1);
        layout.Controls.Add(lblError, 0, 2);
        layout.Controls.Add(buttons, 0, 3);
        Controls.Add(layout);
        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 9);
        ClientSize = new Size(720, 680);
        MinimumSize = new Size(600, 460);
        BackColor = Color.White;
        CancelButton = btnCancel;
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        Text = "HR";
        ResumeLayout(false);
    }
    private TableLayoutPanel layout;
    private Label lblTitle;
    private Panel scroll;
    private TableLayoutPanel fields;
    private Label lblError;
    private FlowLayoutPanel buttons;
    private Button btnSave;
    private Button btnCancel;
}
