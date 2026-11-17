#nullable disable
using System.ComponentModel;

namespace Admin.WinForms.GUI.Controls.HR;

public sealed partial class OffboardingControl
{
    private IContainer components = null;
    private TableLayoutPanel layout;
    private FlowLayoutPanel Toolbar;
    private Label Status;
    private Panel Body;
    private FlowLayoutPanel Footer;

    private ComboBox _records;
    private DataGridView _checklist;
    private Button _refreshButton;
    private Button _createButton;
    private Button _editDateButton;
    private Button _checklistButton;
    private Button _completeButton;
    private DataGridViewTextBoxColumn _titleColumn;
    private DataGridViewCheckBoxColumn _requiredColumn;
    private DataGridViewCheckBoxColumn _completedColumn;
    private DataGridViewTextBoxColumn _noteColumn;
    private DataGridViewTextBoxColumn _completedAtColumn;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new Container();
        layout = new TableLayoutPanel();
        Toolbar = new FlowLayoutPanel();
        Status = new Label();
        Body = new Panel();
        Footer = new FlowLayoutPanel();
        layout.SuspendLayout();
        Toolbar.SuspendLayout();
        Body.SuspendLayout();
        Footer.SuspendLayout();

        _records = new ComboBox();
        _checklist = new DataGridView();
        _refreshButton = new Button();
        _createButton = new Button();
        _editDateButton = new Button();
        _checklistButton = new Button();
        _completeButton = new Button();
        _titleColumn = new DataGridViewTextBoxColumn();
        _requiredColumn = new DataGridViewCheckBoxColumn();
        _completedColumn = new DataGridViewCheckBoxColumn();
        _noteColumn = new DataGridViewTextBoxColumn();
        _completedAtColumn = new DataGridViewTextBoxColumn();
        ((ISupportInitialize)_checklist).BeginInit();
        SuspendLayout();
        layout.ColumnCount = 1;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowCount = 4;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Dock = DockStyle.Fill;
        layout.Name = "layout";
        layout.Padding = new Padding(16);

        Toolbar.AutoSize = true;
        Toolbar.Dock = DockStyle.Fill;
        Toolbar.Name = "Toolbar";
        Toolbar.WrapContents = true;

        Status.AutoSize = true;
        Status.Dock = DockStyle.Fill;
        Status.ForeColor = Color.FromArgb(80, 96, 115);
        Status.Margin = new Padding(3, 8, 3, 12);
        Status.Name = "Status";

        Body.Dock = DockStyle.Fill;
        Body.Name = "Body";

        Footer.AutoSize = true;
        Footer.Dock = DockStyle.Fill;
        Footer.Name = "Footer";

        layout.Controls.Add(Toolbar, 0, 0);
        layout.Controls.Add(Status, 0, 1);
        layout.Controls.Add(Body, 0, 2);
        layout.Controls.Add(Footer, 0, 3);
        Controls.Add(layout);

        BackColor = Color.FromArgb(244, 246, 249);
        Font = new Font("Segoe UI", 9F);


        _records.DropDownStyle = ComboBoxStyle.DropDownList;
        _records.Name = "_records";
        _records.Width = 470;
        _records.Margin = new Padding(3, 6, 8, 3);

        _refreshButton.AutoSize = true;
        _refreshButton.MinimumSize = new Size(95, 34);
        _refreshButton.Text = "Làm mới";
        _refreshButton.Name = "_refreshButton";
        _refreshButton.Margin = new Padding(3, 3, 8, 3);

        _createButton.AutoSize = true;
        _createButton.MinimumSize = new Size(95, 34);
        _createButton.Text = "Tạo hồ sơ nghỉ";
        _createButton.Name = "_createButton";
        _createButton.Margin = new Padding(3, 3, 8, 3);
        _createButton.BackColor = Color.FromArgb(35, 83, 166);
        _createButton.ForeColor = Color.White;
        _createButton.FlatStyle = FlatStyle.Flat;

        _editDateButton.AutoSize = true;
        _editDateButton.MinimumSize = new Size(95, 34);
        _editDateButton.Text = "Sửa ngày nghỉ";
        _editDateButton.Name = "_editDateButton";
        _editDateButton.Margin = new Padding(3, 3, 8, 3);

        _checklistButton.AutoSize = true;
        _checklistButton.MinimumSize = new Size(95, 34);
        _checklistButton.Text = "Cập nhật bàn giao";
        _checklistButton.Name = "_checklistButton";
        _checklistButton.Margin = new Padding(3, 3, 8, 3);

        _completeButton.AutoSize = true;
        _completeButton.MinimumSize = new Size(95, 34);
        _completeButton.Text = "Xác nhận nghỉ việc";
        _completeButton.Name = "_completeButton";
        _completeButton.Margin = new Padding(3, 3, 8, 3);

        Toolbar.Controls.AddRange(new Control[]
        {
            _records, _refreshButton, _createButton, _editDateButton, _checklistButton, _completeButton
        });

        _checklist.AllowUserToAddRows = false;
        _checklist.AllowUserToDeleteRows = false;
        _checklist.AutoGenerateColumns = false;
        _checklist.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _checklist.BackgroundColor = Color.White;
        _checklist.BorderStyle = BorderStyle.FixedSingle;
        _checklist.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 236, 246);
        _checklist.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _checklist.EnableHeadersVisualStyles = false;
        _checklist.MultiSelect = false;
        _checklist.ReadOnly = true;
        _checklist.RowHeadersVisible = false;
        _checklist.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _checklist.Dock = DockStyle.Fill;
        _checklist.Name = "_checklist";

        _titleColumn.DataPropertyName = "Title";
        _titleColumn.HeaderText = "Hạng mục";
        _titleColumn.Name = "Title";
        _titleColumn.MinimumWidth = 120;
        _titleColumn.FillWeight = 150F;
        _requiredColumn.DataPropertyName = "IsRequired";
        _requiredColumn.HeaderText = "Bắt buộc";
        _requiredColumn.Name = "IsRequired";
        _completedColumn.DataPropertyName = "IsCompleted";
        _completedColumn.HeaderText = "Hoàn thành";
        _completedColumn.Name = "IsCompleted";
        _noteColumn.DataPropertyName = "CompletionNote";
        _noteColumn.HeaderText = "Ghi chú";
        _noteColumn.Name = "CompletionNote";
        _noteColumn.MinimumWidth = 120;
        _noteColumn.FillWeight = 150F;
        _completedAtColumn.DataPropertyName = "CompletedAt";
        _completedAtColumn.HeaderText = "Thời điểm";
        _completedAtColumn.Name = "CompletedAt";
        _completedAtColumn.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
        _checklist.Columns.AddRange(new DataGridViewColumn[]
        {
            _titleColumn, _requiredColumn, _completedColumn, _noteColumn, _completedAtColumn
        });
        _checklist.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
        Body.Controls.Add(_checklist);

        Name = "OffboardingControl";
        Size = new Size(1100, 650);
        ((ISupportInitialize)_checklist).EndInit();
        layout.ResumeLayout(false);
        layout.PerformLayout();
        Toolbar.ResumeLayout(false);
        Toolbar.PerformLayout();
        Body.ResumeLayout(false);
        Footer.ResumeLayout(false);
        Footer.PerformLayout();
        ResumeLayout(false);
    }
}
