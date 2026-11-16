#nullable disable
using System.ComponentModel;

namespace Admin.WinForms.Controls.HR;

public sealed partial class ContractAlertsControl
{
    private IContainer components = null;
    private TableLayoutPanel layout;
    private FlowLayoutPanel Toolbar;
    private Label Status;
    private Panel Body;
    private FlowLayoutPanel Footer;

    private ComboBox _filter;
    private DataGridView _grid;
    private Label _summary;
    private Button _previous;
    private Button _next;
    private Button _refreshButton;
    private Button _scanButton;
    private Button _updateButton;
    private DataGridViewTextBoxColumn _employeeCodeColumn;
    private DataGridViewTextBoxColumn _employeeNameColumn;
    private DataGridViewTextBoxColumn _contractNumberColumn;
    private DataGridViewTextBoxColumn _expiryDateColumn;
    private DataGridViewTextBoxColumn _daysRemainingColumn;
    private DataGridViewCheckBoxColumn _currentCycleColumn;
    private DataGridViewTextBoxColumn _statusColumn;

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

        _filter = new ComboBox();
        _grid = new DataGridView();
        _summary = new Label();
        _previous = new Button();
        _next = new Button();
        _refreshButton = new Button();
        _scanButton = new Button();
        _updateButton = new Button();
        _employeeCodeColumn = new DataGridViewTextBoxColumn();
        _employeeNameColumn = new DataGridViewTextBoxColumn();
        _contractNumberColumn = new DataGridViewTextBoxColumn();
        _expiryDateColumn = new DataGridViewTextBoxColumn();
        _daysRemainingColumn = new DataGridViewTextBoxColumn();
        _currentCycleColumn = new DataGridViewCheckBoxColumn();
        _statusColumn = new DataGridViewTextBoxColumn();
        ((ISupportInitialize)_grid).BeginInit();
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


        _filter.DropDownStyle = ComboBoxStyle.DropDownList;
        _filter.Name = "_filter";
        _filter.Width = 220;
        _filter.Margin = new Padding(3, 6, 8, 3);

        _refreshButton.AutoSize = true;
        _refreshButton.MinimumSize = new Size(95, 34);
        _refreshButton.Text = "Làm mới";
        _refreshButton.Name = "_refreshButton";
        _refreshButton.Margin = new Padding(3, 3, 8, 3);

        _scanButton.AutoSize = true;
        _scanButton.MinimumSize = new Size(110, 34);
        _scanButton.Text = "Quét ngay";
        _scanButton.Name = "_scanButton";
        _scanButton.Margin = new Padding(3, 3, 8, 3);
        _scanButton.TabIndex = 2;

        _updateButton.AutoSize = true;
        _updateButton.MinimumSize = new Size(120, 34);
        _updateButton.Text = "Cập nhật trạng thái";
        _updateButton.Name = "_updateButton";
        _updateButton.Margin = new Padding(3, 3, 8, 3);
        _updateButton.BackColor = Color.FromArgb(35, 83, 166);
        _updateButton.ForeColor = Color.White;
        _updateButton.FlatStyle = FlatStyle.Flat;
        Toolbar.Controls.AddRange(new Control[] { _filter, _refreshButton, _scanButton, _updateButton });

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
        _employeeCodeColumn.MinimumWidth = 75;
        _employeeCodeColumn.Name = "EmployeeCode";
        _employeeCodeColumn.FillWeight = 75F;
        _employeeNameColumn.DataPropertyName = "EmployeeName";
        _employeeNameColumn.HeaderText = "Họ tên";
        _employeeNameColumn.MinimumWidth = 100;
        _employeeNameColumn.Name = "EmployeeName";
        _employeeNameColumn.FillWeight = 110F;
        _contractNumberColumn.DataPropertyName = "ContractNumber";
        _contractNumberColumn.HeaderText = "Hợp đồng";
        _contractNumberColumn.MinimumWidth = 100;
        _contractNumberColumn.Name = "ContractNumber";
        _expiryDateColumn.DataPropertyName = "ExpiryDate";
        _expiryDateColumn.HeaderText = "Hết hạn";
        _expiryDateColumn.MinimumWidth = 90;
        _expiryDateColumn.Name = "ExpiryDate";
        _daysRemainingColumn.DataPropertyName = "DaysRemaining";
        _daysRemainingColumn.HeaderText = "Còn ngày";
        _daysRemainingColumn.MinimumWidth = 80;
        _daysRemainingColumn.Name = "DaysRemaining";
        _currentCycleColumn.DataPropertyName = "IsCurrentCycle";
        _currentCycleColumn.HeaderText = "Chu kỳ hiện tại";
        _currentCycleColumn.MinimumWidth = 95;
        _currentCycleColumn.Name = "IsCurrentCycle";
        _statusColumn.DataPropertyName = "Status";
        _statusColumn.HeaderText = "Trạng thái";
        _statusColumn.MinimumWidth = 85;
        _statusColumn.Name = "Status";
        _grid.Columns.AddRange(new DataGridViewColumn[]
        {
            _employeeCodeColumn, _employeeNameColumn, _contractNumberColumn, _expiryDateColumn,
            _daysRemainingColumn, _currentCycleColumn, _statusColumn
        });
        _grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
        Body.Controls.Add(_grid);

        _previous.AutoSize = true;
        _previous.Text = "Trang trước";
        _previous.Name = "_previous";
        _previous.Margin = new Padding(3);
        _next.AutoSize = true;
        _next.Text = "Trang sau";
        _next.Name = "_next";
        _next.Margin = new Padding(3);
        _summary.AutoSize = true;
        _summary.Name = "_summary";
        _summary.Margin = new Padding(10);
        Footer.Controls.AddRange(new Control[] { _previous, _summary, _next });

        Name = "ContractAlertsControl";
        Size = new Size(1100, 650);
        ((ISupportInitialize)_grid).EndInit();
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
