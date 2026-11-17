#nullable disable
using System.ComponentModel;

namespace Admin.WinForms.GUI.Controls.HR;

public sealed partial class EmployeeManagementControl
{
    private IContainer components = null;
    private TableLayoutPanel layout;
    private FlowLayoutPanel Toolbar;
    private Label Status;
    private Panel Body;
    private FlowLayoutPanel Footer;

    private TextBox _search;
    private DataGridView _grid;
    private Label _summary;
    private Button _previous;
    private Button _next;
    private Button _searchButton;
    private Button _refreshButton;
    private Button _addButton;
    private Button _editButton;
    private Button _lockButton;
    private Button _detailsButton;
    private Button _salaryButton;
    private Button _deleteButton;
    private Button _exportButton;
    private DataGridViewTextBoxColumn _employeeCodeColumn;
    private DataGridViewTextBoxColumn _fullNameColumn;
    private DataGridViewTextBoxColumn _employmentTypeColumn;
    private DataGridViewTextBoxColumn _salaryColumn;
    private DataGridViewTextBoxColumn _departmentColumn;
    private DataGridViewTextBoxColumn _positionColumn;
    private DataGridViewTextBoxColumn _emailColumn;
    private DataGridViewTextBoxColumn _statusColumn;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        layout = new TableLayoutPanel();
        Toolbar = new FlowLayoutPanel();
        _search = new TextBox();
        _searchButton = new Button();
        _refreshButton = new Button();
        _addButton = new Button();
        _editButton = new Button();
        _lockButton = new Button();
        _detailsButton = new Button();
        _salaryButton = new Button();
        _deleteButton = new Button();
        _exportButton = new Button();
        Status = new Label();
        Body = new Panel();
        _grid = new DataGridView();
        _employeeCodeColumn = new DataGridViewTextBoxColumn();
        _fullNameColumn = new DataGridViewTextBoxColumn();
        _employmentTypeColumn = new DataGridViewTextBoxColumn();
        _salaryColumn = new DataGridViewTextBoxColumn();
        _departmentColumn = new DataGridViewTextBoxColumn();
        _positionColumn = new DataGridViewTextBoxColumn();
        _emailColumn = new DataGridViewTextBoxColumn();
        _statusColumn = new DataGridViewTextBoxColumn();
        Footer = new FlowLayoutPanel();
        _previous = new Button();
        _summary = new Label();
        _next = new Button();
        layout.SuspendLayout();
        Toolbar.SuspendLayout();
        Body.SuspendLayout();
        ((ISupportInitialize)_grid).BeginInit();
        Footer.SuspendLayout();
        SuspendLayout();
        // 
        // layout
        // 
        layout.ColumnCount = 1;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.Controls.Add(Toolbar, 0, 0);
        layout.Controls.Add(Status, 0, 1);
        layout.Controls.Add(Body, 0, 2);
        layout.Controls.Add(Footer, 0, 3);
        layout.Dock = DockStyle.Fill;
        layout.Location = new Point(0, 0);
        layout.Name = "layout";
        layout.Padding = new Padding(16);
        layout.RowCount = 4;
        layout.RowStyles.Add(new RowStyle());
        layout.RowStyles.Add(new RowStyle());
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle());
        layout.Size = new Size(1100, 650);
        layout.TabIndex = 0;
        // 
        // Toolbar
        // 
        Toolbar.AutoSize = true;
        Toolbar.Controls.Add(_search);
        Toolbar.Controls.Add(_searchButton);
        Toolbar.Controls.Add(_refreshButton);
        Toolbar.Controls.Add(_addButton);
        Toolbar.Controls.Add(_editButton);
        Toolbar.Controls.Add(_lockButton);
        Toolbar.Controls.Add(_detailsButton);
        Toolbar.Controls.Add(_salaryButton);
        Toolbar.Controls.Add(_deleteButton);
        Toolbar.Controls.Add(_exportButton);
        Toolbar.Dock = DockStyle.Fill;
        Toolbar.Location = new Point(19, 19);
        Toolbar.Name = "Toolbar";
        Toolbar.Size = new Size(1062, 80);
        Toolbar.TabIndex = 0;
        // 
        // _search
        // 
        _search.Location = new Point(3, 8);
        _search.Margin = new Padding(3, 8, 8, 3);
        _search.Name = "_search";
        _search.PlaceholderText = "Mã, tên, email, điện thoại, CCCD";
        _search.Size = new Size(280, 27);
        _search.TabIndex = 0;
        // 
        // _searchButton
        // 
        _searchButton.AutoSize = true;
        _searchButton.BackColor = Color.FromArgb(35, 83, 166);
        _searchButton.FlatStyle = FlatStyle.Flat;
        _searchButton.ForeColor = Color.White;
        _searchButton.Location = new Point(294, 3);
        _searchButton.Margin = new Padding(3, 3, 8, 3);
        _searchButton.MinimumSize = new Size(95, 34);
        _searchButton.Name = "_searchButton";
        _searchButton.Size = new Size(95, 34);
        _searchButton.TabIndex = 1;
        _searchButton.Text = "Tìm kiếm";
        _searchButton.UseVisualStyleBackColor = false;
        // 
        // _refreshButton
        // 
        _refreshButton.AutoSize = true;
        _refreshButton.Location = new Point(400, 3);
        _refreshButton.Margin = new Padding(3, 3, 8, 3);
        _refreshButton.MinimumSize = new Size(95, 34);
        _refreshButton.Name = "_refreshButton";
        _refreshButton.Size = new Size(95, 34);
        _refreshButton.TabIndex = 2;
        _refreshButton.Text = "Làm mới";
        // 
        // _addButton
        // 
        _addButton.AutoSize = true;
        _addButton.Location = new Point(506, 3);
        _addButton.Margin = new Padding(3, 3, 8, 3);
        _addButton.MinimumSize = new Size(95, 34);
        _addButton.Name = "_addButton";
        _addButton.Size = new Size(135, 34);
        _addButton.TabIndex = 3;
        _addButton.Text = "Thêm / Tiếp nhận";
        // 
        // _editButton
        // 
        _editButton.AutoSize = true;
        _editButton.Location = new Point(652, 3);
        _editButton.Margin = new Padding(3, 3, 8, 3);
        _editButton.MinimumSize = new Size(95, 34);
        _editButton.Name = "_editButton";
        _editButton.Size = new Size(95, 34);
        _editButton.TabIndex = 4;
        _editButton.Text = "Sửa hồ sơ";
        // 
        // _lockButton
        // 
        _lockButton.AutoSize = true;
        _lockButton.Location = new Point(758, 3);
        _lockButton.Margin = new Padding(3, 3, 8, 3);
        _lockButton.MinimumSize = new Size(95, 34);
        _lockButton.Name = "_lockButton";
        _lockButton.Size = new Size(95, 34);
        _lockButton.TabIndex = 5;
        _lockButton.Text = "Khóa hồ sơ";
        // 
        // _detailsButton
        // 
        _detailsButton.AutoSize = true;
        _detailsButton.Location = new Point(864, 3);
        _detailsButton.Margin = new Padding(3, 3, 8, 3);
        _detailsButton.MinimumSize = new Size(95, 34);
        _detailsButton.Name = "_detailsButton";
        _detailsButton.Size = new Size(98, 34);
        _detailsButton.TabIndex = 6;
        _detailsButton.Text = "Xem chi tiết";
        // 
        // _salaryButton
        // 
        _salaryButton.AutoSize = true;
        _salaryButton.Location = new Point(3, 43);
        _salaryButton.Margin = new Padding(3, 3, 8, 3);
        _salaryButton.MinimumSize = new Size(95, 34);
        _salaryButton.Name = "_salaryButton";
        _salaryButton.Size = new Size(121, 34);
        _salaryButton.TabIndex = 7;
        _salaryButton.Text = "Cập nhật lương";
        //
        // _deleteButton
        //
        _deleteButton.AutoSize = true;
        _deleteButton.ForeColor = Color.DarkRed;
        _deleteButton.Location = new Point(135, 43);
        _deleteButton.Margin = new Padding(3, 3, 8, 3);
        _deleteButton.MinimumSize = new Size(110, 34);
        _deleteButton.Name = "_deleteButton";
        _deleteButton.Size = new Size(120, 34);
        _deleteButton.TabIndex = 8;
        _deleteButton.Text = "Xóa nhân viên";
        _deleteButton.UseVisualStyleBackColor = true;
        //
        // _exportButton
        //
        _exportButton.AutoSize = true;
        _exportButton.Location = new Point(266, 43);
        _exportButton.Margin = new Padding(3, 3, 8, 3);
        _exportButton.MinimumSize = new Size(110, 34);
        _exportButton.Name = "_exportButton";
        _exportButton.Size = new Size(110, 34);
        _exportButton.TabIndex = 9;
        _exportButton.Text = "Xuất Excel";
        _exportButton.UseVisualStyleBackColor = true;
        // 
        // Status
        // 
        Status.AutoSize = true;
        Status.Dock = DockStyle.Fill;
        Status.ForeColor = Color.FromArgb(80, 96, 115);
        Status.Location = new Point(19, 110);
        Status.Margin = new Padding(3, 8, 3, 12);
        Status.Name = "Status";
        Status.Size = new Size(1062, 20);
        Status.TabIndex = 1;
        // 
        // Body
        // 
        Body.Controls.Add(_grid);
        Body.Dock = DockStyle.Fill;
        Body.Location = new Point(19, 145);
        Body.Name = "Body";
        Body.Size = new Size(1062, 440);
        Body.TabIndex = 2;
        // 
        // _grid
        // 
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AutoGenerateColumns = false;
        dataGridViewCellStyle1.BackColor = Color.FromArgb(247, 249, 252);
        _grid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.BackgroundColor = Color.White;
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle2.BackColor = Color.FromArgb(230, 236, 246);
        dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
        dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
        _grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
        _grid.ColumnHeadersHeight = 29;
        _grid.Columns.AddRange(new DataGridViewColumn[]
        {
            _employeeCodeColumn, _fullNameColumn, _employmentTypeColumn, _salaryColumn,
            _departmentColumn, _positionColumn, _emailColumn, _statusColumn
        });
        _grid.Dock = DockStyle.Fill;
        _grid.EnableHeadersVisualStyles = false;
        _grid.Location = new Point(0, 0);
        _grid.MultiSelect = false;
        _grid.Name = "_grid";
        _grid.ReadOnly = true;
        _grid.RowHeadersVisible = false;
        _grid.RowHeadersWidth = 51;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.Size = new Size(1062, 440);
        _grid.TabIndex = 0;
        //
        // _employeeCodeColumn
        //
        _employeeCodeColumn.DataPropertyName = "EmployeeCode";
        _employeeCodeColumn.FillWeight = 75F;
        _employeeCodeColumn.HeaderText = "Mã NV";
        _employeeCodeColumn.MinimumWidth = 70;
        _employeeCodeColumn.Name = "_employeeCodeColumn";
        _employeeCodeColumn.ReadOnly = true;
        _employeeCodeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // _fullNameColumn
        //
        _fullNameColumn.DataPropertyName = "FullName";
        _fullNameColumn.FillWeight = 125F;
        _fullNameColumn.HeaderText = "Họ tên";
        _fullNameColumn.MinimumWidth = 70;
        _fullNameColumn.Name = "_fullNameColumn";
        _fullNameColumn.ReadOnly = true;
        _fullNameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // _employmentTypeColumn
        //
        _employmentTypeColumn.DataPropertyName = "EmploymentTypeLabel";
        _employmentTypeColumn.FillWeight = 75F;
        _employmentTypeColumn.HeaderText = "Loại";
        _employmentTypeColumn.MinimumWidth = 70;
        _employmentTypeColumn.Name = "_employmentTypeColumn";
        _employmentTypeColumn.ReadOnly = true;
        _employmentTypeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // _salaryColumn
        //
        _salaryColumn.DataPropertyName = "SalaryLabel";
        _salaryColumn.FillWeight = 110F;
        _salaryColumn.HeaderText = "Lương";
        _salaryColumn.MinimumWidth = 70;
        _salaryColumn.Name = "_salaryColumn";
        _salaryColumn.ReadOnly = true;
        _salaryColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // _departmentColumn
        //
        _departmentColumn.DataPropertyName = "DepartmentName";
        _departmentColumn.FillWeight = 110F;
        _departmentColumn.HeaderText = "Phòng ban";
        _departmentColumn.MinimumWidth = 70;
        _departmentColumn.Name = "_departmentColumn";
        _departmentColumn.ReadOnly = true;
        _departmentColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // _positionColumn
        //
        _positionColumn.DataPropertyName = "PositionName";
        _positionColumn.FillWeight = 100F;
        _positionColumn.HeaderText = "Chức vụ";
        _positionColumn.MinimumWidth = 70;
        _positionColumn.Name = "_positionColumn";
        _positionColumn.ReadOnly = true;
        _positionColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // _emailColumn
        //
        _emailColumn.DataPropertyName = "Email";
        _emailColumn.FillWeight = 145F;
        _emailColumn.HeaderText = "Email";
        _emailColumn.MinimumWidth = 70;
        _emailColumn.Name = "_emailColumn";
        _emailColumn.ReadOnly = true;
        _emailColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // _statusColumn
        //
        _statusColumn.DataPropertyName = "Status";
        _statusColumn.FillWeight = 80F;
        _statusColumn.HeaderText = "Trạng thái";
        _statusColumn.MinimumWidth = 70;
        _statusColumn.Name = "_statusColumn";
        _statusColumn.ReadOnly = true;
        _statusColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        // 
        // Footer
        // 
        Footer.AutoSize = true;
        Footer.Controls.Add(_previous);
        Footer.Controls.Add(_summary);
        Footer.Controls.Add(_next);
        Footer.Dock = DockStyle.Fill;
        Footer.Location = new Point(19, 591);
        Footer.Name = "Footer";
        Footer.Size = new Size(1062, 40);
        Footer.TabIndex = 3;
        // 
        // _previous
        // 
        _previous.AutoSize = true;
        _previous.Location = new Point(3, 3);
        _previous.Name = "_previous";
        _previous.Size = new Size(95, 34);
        _previous.TabIndex = 0;
        _previous.Text = "Trang trước";
        // 
        // _summary
        // 
        _summary.AutoSize = true;
        _summary.Location = new Point(113, 12);
        _summary.Margin = new Padding(12, 12, 8, 3);
        _summary.Name = "_summary";
        _summary.Size = new Size(0, 20);
        _summary.TabIndex = 1;
        // 
        // _next
        // 
        _next.AutoSize = true;
        _next.Location = new Point(124, 3);
        _next.Name = "_next";
        _next.Size = new Size(82, 34);
        _next.TabIndex = 2;
        _next.Text = "Trang sau";
        // 
        // EmployeeManagementControl
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        BackColor = Color.FromArgb(244, 246, 249);
        Controls.Add(layout);
        Font = new Font("Segoe UI", 9F);
        Name = "EmployeeManagementControl";
        layout.ResumeLayout(false);
        layout.PerformLayout();
        Toolbar.ResumeLayout(false);
        Toolbar.PerformLayout();
        Body.ResumeLayout(false);
        ((ISupportInitialize)_grid).EndInit();
        Footer.ResumeLayout(false);
        Footer.PerformLayout();
        ResumeLayout(false);
    }

}
