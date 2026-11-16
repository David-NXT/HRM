#nullable disable
using System.ComponentModel;

namespace Admin.WinForms.Controls.HR;

public sealed partial class OrganizationManagementControl
{
    private IContainer components = null;
    private TableLayoutPanel layout;
    private FlowLayoutPanel Toolbar;
    private Label Status;
    private Panel Body;
    private FlowLayoutPanel Footer;

    private TabControl _tabs;
    private TabPage _departmentsPage;
    private TabPage _positionsPage;
    private DataGridView _departments;
    private DataGridView _positions;
    private Button _refreshButton;
    private Button _addButton;
    private Button _editButton;
    private Button _deactivateButton;
    private DataGridViewTextBoxColumn _departmentCodeColumn;
    private DataGridViewTextBoxColumn _departmentNameColumn;
    private DataGridViewTextBoxColumn _departmentDescriptionColumn;
    private DataGridViewTextBoxColumn _departmentStatusColumn;
    private DataGridViewTextBoxColumn _positionCodeColumn;
    private DataGridViewTextBoxColumn _positionNameColumn;
    private DataGridViewTextBoxColumn _positionSeniorityColumn;
    private DataGridViewTextBoxColumn _positionDescriptionColumn;
    private DataGridViewTextBoxColumn _positionStatusColumn;

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

        _tabs = new TabControl();
        _departmentsPage = new TabPage();
        _positionsPage = new TabPage();
        _departments = new DataGridView();
        _positions = new DataGridView();
        _refreshButton = new Button();
        _addButton = new Button();
        _editButton = new Button();
        _deactivateButton = new Button();
        _departmentCodeColumn = new DataGridViewTextBoxColumn();
        _departmentNameColumn = new DataGridViewTextBoxColumn();
        _departmentDescriptionColumn = new DataGridViewTextBoxColumn();
        _departmentStatusColumn = new DataGridViewTextBoxColumn();
        _positionCodeColumn = new DataGridViewTextBoxColumn();
        _positionNameColumn = new DataGridViewTextBoxColumn();
        _positionSeniorityColumn = new DataGridViewTextBoxColumn();
        _positionDescriptionColumn = new DataGridViewTextBoxColumn();
        _positionStatusColumn = new DataGridViewTextBoxColumn();
        ((ISupportInitialize)_departments).BeginInit();
        ((ISupportInitialize)_positions).BeginInit();
        _tabs.SuspendLayout();
        _departmentsPage.SuspendLayout();
        _positionsPage.SuspendLayout();
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


        _refreshButton.AutoSize = true;
        _refreshButton.MinimumSize = new Size(95, 34);
        _refreshButton.Text = "Làm mới";
        _refreshButton.Name = "_refreshButton";
        _refreshButton.Margin = new Padding(3, 3, 8, 3);
        _addButton.AutoSize = true;
        _addButton.MinimumSize = new Size(95, 34);
        _addButton.Text = "Thêm";
        _addButton.Name = "_addButton";
        _addButton.Margin = new Padding(3, 3, 8, 3);
        _addButton.BackColor = Color.FromArgb(35, 83, 166);
        _addButton.ForeColor = Color.White;
        _addButton.FlatStyle = FlatStyle.Flat;
        _editButton.AutoSize = true;
        _editButton.MinimumSize = new Size(95, 34);
        _editButton.Text = "Sửa";
        _editButton.Name = "_editButton";
        _editButton.Margin = new Padding(3, 3, 8, 3);
        _deactivateButton.AutoSize = true;
        _deactivateButton.MinimumSize = new Size(120, 34);
        _deactivateButton.Text = "Ngừng hoạt động";
        _deactivateButton.Name = "_deactivateButton";
        _deactivateButton.Margin = new Padding(3, 3, 8, 3);
        Toolbar.Controls.AddRange(new Control[] { _refreshButton, _addButton, _editButton, _deactivateButton });

        _departments.AllowUserToAddRows = false;
        _departments.AllowUserToDeleteRows = false;
        _departments.AutoGenerateColumns = false;
        _departments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _departments.BackgroundColor = Color.White;
        _departments.BorderStyle = BorderStyle.FixedSingle;
        _departments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 236, 246);
        _departments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _departments.Dock = DockStyle.Fill;
        _departments.EnableHeadersVisualStyles = false;
        _departments.MultiSelect = false;
        _departments.ReadOnly = true;
        _departments.RowHeadersVisible = false;
        _departments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _departments.Name = "_departments";
        _departments.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
        _departmentCodeColumn.DataPropertyName = "Code";
        _departmentCodeColumn.HeaderText = "Mã";
        _departmentCodeColumn.Name = "Code";
        _departmentNameColumn.DataPropertyName = "Name";
        _departmentNameColumn.HeaderText = "Phòng ban";
        _departmentNameColumn.Name = "Name";
        _departmentDescriptionColumn.DataPropertyName = "Description";
        _departmentDescriptionColumn.HeaderText = "Mô tả";
        _departmentDescriptionColumn.Name = "Description";
        _departmentStatusColumn.DataPropertyName = "Status";
        _departmentStatusColumn.HeaderText = "Trạng thái";
        _departmentStatusColumn.Name = "Status";
        _departments.Columns.AddRange(new DataGridViewColumn[]
        {
            _departmentCodeColumn, _departmentNameColumn, _departmentDescriptionColumn, _departmentStatusColumn
        });
        _departmentsPage.Controls.Add(_departments);
        _departmentsPage.Name = "_departmentsPage";
        _departmentsPage.Padding = new Padding(3);
        _departmentsPage.Text = "Phòng ban";

        _positions.AllowUserToAddRows = false;
        _positions.AllowUserToDeleteRows = false;
        _positions.AutoGenerateColumns = false;
        _positions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _positions.BackgroundColor = Color.White;
        _positions.BorderStyle = BorderStyle.FixedSingle;
        _positions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 236, 246);
        _positions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _positions.Dock = DockStyle.Fill;
        _positions.EnableHeadersVisualStyles = false;
        _positions.MultiSelect = false;
        _positions.ReadOnly = true;
        _positions.RowHeadersVisible = false;
        _positions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _positions.Name = "_positions";
        _positions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
        _positionCodeColumn.DataPropertyName = "Code";
        _positionCodeColumn.HeaderText = "Mã";
        _positionCodeColumn.Name = "Code";
        _positionNameColumn.DataPropertyName = "Name";
        _positionNameColumn.HeaderText = "Chức vụ";
        _positionNameColumn.Name = "Name";
        _positionSeniorityColumn.DataPropertyName = "SeniorityLevelLabel";
        _positionSeniorityColumn.HeaderText = "Cấp bậc";
        _positionSeniorityColumn.Name = "SeniorityLevelLabel";
        _positionDescriptionColumn.DataPropertyName = "Description";
        _positionDescriptionColumn.HeaderText = "Mô tả";
        _positionDescriptionColumn.Name = "Description";
        _positionStatusColumn.DataPropertyName = "Status";
        _positionStatusColumn.HeaderText = "Trạng thái";
        _positionStatusColumn.Name = "Status";
        _positions.Columns.AddRange(new DataGridViewColumn[]
        {
            _positionCodeColumn, _positionNameColumn, _positionSeniorityColumn,
            _positionDescriptionColumn, _positionStatusColumn
        });
        _positionsPage.Controls.Add(_positions);
        _positionsPage.Name = "_positionsPage";
        _positionsPage.Padding = new Padding(3);
        _positionsPage.Text = "Chức vụ";

        _tabs.TabPages.AddRange(new TabPage[] { _departmentsPage, _positionsPage });
        _tabs.Dock = DockStyle.Fill;
        _tabs.Name = "_tabs";
        Body.Controls.Add(_tabs);

        Name = "OrganizationManagementControl";
        Size = new Size(1100, 650);
        ((ISupportInitialize)_departments).EndInit();
        ((ISupportInitialize)_positions).EndInit();
        _departmentsPage.ResumeLayout(false);
        _positionsPage.ResumeLayout(false);
        _tabs.ResumeLayout(false);
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
