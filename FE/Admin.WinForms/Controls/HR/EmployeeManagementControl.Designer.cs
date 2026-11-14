#nullable disable
using System.ComponentModel;

namespace Admin.WinForms.Controls.HR;

public sealed partial class EmployeeManagementControl
{
    private IContainer components = null;
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
        components = new Container();
        _search = new TextBox();
        _grid = new DataGridView();
        _summary = new Label();
        _previous = new Button();
        _next = new Button();
        _searchButton = new Button();
        _refreshButton = new Button();
        _addButton = new Button();
        _editButton = new Button();
        _lockButton = new Button();
        _detailsButton = new Button();
        _salaryButton = new Button();
        _employeeCodeColumn = new DataGridViewTextBoxColumn();
        _fullNameColumn = new DataGridViewTextBoxColumn();
        _employmentTypeColumn = new DataGridViewTextBoxColumn();
        _salaryColumn = new DataGridViewTextBoxColumn();
        _departmentColumn = new DataGridViewTextBoxColumn();
        _positionColumn = new DataGridViewTextBoxColumn();
        _emailColumn = new DataGridViewTextBoxColumn();
        _statusColumn = new DataGridViewTextBoxColumn();
        ((ISupportInitialize)_grid).BeginInit();
        SuspendLayout();

        _search.Name = "_search";
        _search.PlaceholderText = "Mã, tên, email, điện thoại, CCCD";
        _search.Size = new Size(280, 27);
        _search.Margin = new Padding(3, 8, 8, 3);

        _searchButton.AutoSize = true;
        _searchButton.BackColor = Color.FromArgb(35, 83, 166);
        _searchButton.FlatStyle = FlatStyle.Flat;
        _searchButton.ForeColor = Color.White;
        _searchButton.Margin = new Padding(3, 3, 8, 3);
        _searchButton.MinimumSize = new Size(95, 34);
        _searchButton.Name = "_searchButton";
        _searchButton.Text = "Tìm kiếm";

        _refreshButton.AutoSize = true;
        _refreshButton.Margin = new Padding(3, 3, 8, 3);
        _refreshButton.MinimumSize = new Size(95, 34);
        _refreshButton.Name = "_refreshButton";
        _refreshButton.Text = "Làm mới";

        _addButton.AutoSize = true;
        _addButton.Margin = new Padding(3, 3, 8, 3);
        _addButton.MinimumSize = new Size(95, 34);
        _addButton.Name = "_addButton";
        _addButton.Text = "Thêm / Tiếp nhận";

        _editButton.AutoSize = true;
        _editButton.Margin = new Padding(3, 3, 8, 3);
        _editButton.MinimumSize = new Size(95, 34);
        _editButton.Name = "_editButton";
        _editButton.Text = "Sửa hồ sơ";

        _lockButton.AutoSize = true;
        _lockButton.Margin = new Padding(3, 3, 8, 3);
        _lockButton.MinimumSize = new Size(95, 34);
        _lockButton.Name = "_lockButton";
        _lockButton.Text = "Khóa hồ sơ";

        _detailsButton.AutoSize = true;
        _detailsButton.Margin = new Padding(3, 3, 8, 3);
        _detailsButton.MinimumSize = new Size(95, 34);
        _detailsButton.Name = "_detailsButton";
        _detailsButton.Text = "Xem chi tiết";

        _salaryButton.AutoSize = true;
        _salaryButton.Margin = new Padding(3, 3, 8, 3);
        _salaryButton.MinimumSize = new Size(95, 34);
        _salaryButton.Name = "_salaryButton";
        _salaryButton.Text = "Cập nhật lương";

        Toolbar.Controls.AddRange(new Control[]
        {
            _search, _searchButton, _refreshButton, _addButton, _editButton,
            _lockButton, _detailsButton, _salaryButton
        });

        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AutoGenerateColumns = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.BackgroundColor = Color.White;
        _grid.BorderStyle = BorderStyle.FixedSingle;
        _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 236, 246);
        _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _grid.EnableHeadersVisualStyles = false;
        _grid.MultiSelect = false;
        _grid.ReadOnly = true;
        _grid.RowHeadersVisible = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.Dock = DockStyle.Fill;
        _grid.Name = "_grid";
        _employeeCodeColumn.DataPropertyName = "EmployeeCode";
        _employeeCodeColumn.HeaderText = "Mã NV";
        _employeeCodeColumn.MinimumWidth = 70;
        _employeeCodeColumn.Name = "EmployeeCode";
        _employeeCodeColumn.FillWeight = 75F;
        _employeeCodeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

        _fullNameColumn.DataPropertyName = "FullName";
        _fullNameColumn.HeaderText = "Họ tên";
        _fullNameColumn.MinimumWidth = 70;
        _fullNameColumn.Name = "FullName";
        _fullNameColumn.FillWeight = 125F;
        _fullNameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

        _employmentTypeColumn.DataPropertyName = "EmploymentTypeLabel";
        _employmentTypeColumn.HeaderText = "Loại";
        _employmentTypeColumn.MinimumWidth = 70;
        _employmentTypeColumn.Name = "EmploymentTypeLabel";
        _employmentTypeColumn.FillWeight = 75F;
        _employmentTypeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

        _salaryColumn.DataPropertyName = "SalaryLabel";
        _salaryColumn.HeaderText = "Lương";
        _salaryColumn.MinimumWidth = 70;
        _salaryColumn.Name = "SalaryLabel";
        _salaryColumn.FillWeight = 110F;
        _salaryColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

        _departmentColumn.DataPropertyName = "DepartmentName";
        _departmentColumn.HeaderText = "Phòng ban";
        _departmentColumn.MinimumWidth = 70;
        _departmentColumn.Name = "DepartmentName";
        _departmentColumn.FillWeight = 110F;
        _departmentColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

        _positionColumn.DataPropertyName = "PositionName";
        _positionColumn.HeaderText = "Chức vụ";
        _positionColumn.MinimumWidth = 70;
        _positionColumn.Name = "PositionName";
        _positionColumn.FillWeight = 100F;
        _positionColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

        _emailColumn.DataPropertyName = "Email";
        _emailColumn.HeaderText = "Email";
        _emailColumn.MinimumWidth = 70;
        _emailColumn.Name = "Email";
        _emailColumn.FillWeight = 145F;
        _emailColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

        _statusColumn.DataPropertyName = "Status";
        _statusColumn.HeaderText = "Trạng thái";
        _statusColumn.MinimumWidth = 70;
        _statusColumn.Name = "Status";
        _statusColumn.FillWeight = 80F;
        _statusColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

        _grid.Columns.AddRange(new DataGridViewColumn[]
        {
            _employeeCodeColumn, _fullNameColumn, _employmentTypeColumn, _salaryColumn,
            _departmentColumn, _positionColumn, _emailColumn, _statusColumn
        });
        _grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
        Body.Controls.Add(_grid);

        _previous.AutoSize = true;
        _previous.Height = 34;
        _previous.Name = "_previous";
        _previous.Text = "Trang trước";
        _previous.Margin = new Padding(3);

        _summary.AutoSize = true;
        _summary.Name = "_summary";
        _summary.Margin = new Padding(12, 12, 8, 3);

        _next.AutoSize = true;
        _next.Height = 34;
        _next.Name = "_next";
        _next.Text = "Trang sau";
        _next.Margin = new Padding(3);

        Footer.Controls.AddRange(new Control[] { _previous, _summary, _next });

        Name = "EmployeeManagementControl";
        Size = new Size(1100, 650);
        ((ISupportInitialize)_grid).EndInit();
        ResumeLayout(false);
    }

}
