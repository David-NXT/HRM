#nullable disable
using System.ComponentModel;

namespace Admin.WinForms.Controls.HR;

public sealed partial class ContractWorkControl
{
    private IContainer components = null;
    private TableLayoutPanel layout;
    private FlowLayoutPanel Toolbar;
    private Label Status;
    private Panel Body;
    private FlowLayoutPanel Footer;

    private ComboBox _employees;
    private TabControl _tabs;
    private TabPage _contractsPage;
    private TabPage _historyPage;
    private DataGridView _contracts;
    private DataGridView _history;
    private Button _refreshButton;
    private Button _addContractButton;
    private Button _editContractButton;
    private Button _renewButton;
    private Button _assignmentButton;
    private DataGridViewTextBoxColumn _contractNumberColumn;
    private DataGridViewTextBoxColumn _contractTypeColumn;
    private DataGridViewTextBoxColumn _contractStartColumn;
    private DataGridViewTextBoxColumn _contractEndColumn;
    private DataGridViewTextBoxColumn _contractStatusColumn;
    private DataGridViewTextBoxColumn _contractNotesColumn;
    private DataGridViewTextBoxColumn _historyDepartmentColumn;
    private DataGridViewTextBoxColumn _historyPositionColumn;
    private DataGridViewTextBoxColumn _historyStartColumn;
    private DataGridViewTextBoxColumn _historyEndColumn;
    private DataGridViewTextBoxColumn _historyNotesColumn;

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

        _employees = new ComboBox();
        _tabs = new TabControl();
        _contractsPage = new TabPage();
        _historyPage = new TabPage();
        _contracts = new DataGridView();
        _history = new DataGridView();
        _refreshButton = new Button();
        _addContractButton = new Button();
        _editContractButton = new Button();
        _renewButton = new Button();
        _assignmentButton = new Button();
        _contractNumberColumn = new DataGridViewTextBoxColumn();
        _contractTypeColumn = new DataGridViewTextBoxColumn();
        _contractStartColumn = new DataGridViewTextBoxColumn();
        _contractEndColumn = new DataGridViewTextBoxColumn();
        _contractStatusColumn = new DataGridViewTextBoxColumn();
        _contractNotesColumn = new DataGridViewTextBoxColumn();
        _historyDepartmentColumn = new DataGridViewTextBoxColumn();
        _historyPositionColumn = new DataGridViewTextBoxColumn();
        _historyStartColumn = new DataGridViewTextBoxColumn();
        _historyEndColumn = new DataGridViewTextBoxColumn();
        _historyNotesColumn = new DataGridViewTextBoxColumn();
        ((ISupportInitialize)_contracts).BeginInit();
        ((ISupportInitialize)_history).BeginInit();
        _tabs.SuspendLayout();
        _contractsPage.SuspendLayout();
        _historyPage.SuspendLayout();
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


        _employees.DropDownStyle = ComboBoxStyle.DropDownList;
        _employees.Name = "_employees";
        _employees.Width = 320;
        _employees.Margin = new Padding(3, 6, 8, 3);

        _refreshButton.AutoSize = true;
        _refreshButton.MinimumSize = new Size(95, 34);
        _refreshButton.Text = "Làm mới";
        _refreshButton.Name = "_refreshButton";
        _refreshButton.Margin = new Padding(3, 3, 8, 3);
        _addContractButton.AutoSize = true;
        _addContractButton.MinimumSize = new Size(95, 34);
        _addContractButton.Text = "Thêm hợp đồng";
        _addContractButton.Name = "_addContractButton";
        _addContractButton.Margin = new Padding(3, 3, 8, 3);
        _addContractButton.BackColor = Color.FromArgb(35, 83, 166);
        _addContractButton.ForeColor = Color.White;
        _addContractButton.FlatStyle = FlatStyle.Flat;
        _editContractButton.AutoSize = true;
        _editContractButton.MinimumSize = new Size(95, 34);
        _editContractButton.Text = "Sửa hợp đồng";
        _editContractButton.Name = "_editContractButton";
        _editContractButton.Margin = new Padding(3, 3, 8, 3);
        _renewButton.AutoSize = true;
        _renewButton.MinimumSize = new Size(95, 34);
        _renewButton.Text = "Gia hạn";
        _renewButton.Name = "_renewButton";
        _renewButton.Margin = new Padding(3, 3, 8, 3);
        _assignmentButton.AutoSize = true;
        _assignmentButton.MinimumSize = new Size(120, 34);
        _assignmentButton.Text = "Phân công / Điều chuyển";
        _assignmentButton.Name = "_assignmentButton";
        _assignmentButton.Margin = new Padding(3, 3, 8, 3);
        Toolbar.Controls.AddRange(new Control[]
        {
            _employees, _refreshButton, _addContractButton, _editContractButton, _renewButton, _assignmentButton
        });

        _contracts.AllowUserToAddRows = false;
        _contracts.AllowUserToDeleteRows = false;
        _contracts.AutoGenerateColumns = false;
        _contracts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _contracts.BackgroundColor = Color.White;
        _contracts.BorderStyle = BorderStyle.FixedSingle;
        _contracts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 236, 246);
        _contracts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _contracts.Dock = DockStyle.Fill;
        _contracts.EnableHeadersVisualStyles = false;
        _contracts.MultiSelect = false;
        _contracts.ReadOnly = true;
        _contracts.RowHeadersVisible = false;
        _contracts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _contracts.Name = "_contracts";
        _contracts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
        _contractNumberColumn.DataPropertyName = "ContractNumber";
        _contractNumberColumn.HeaderText = "Số hợp đồng";
        _contractNumberColumn.Name = "ContractNumber";
        _contractTypeColumn.DataPropertyName = "ContractType";
        _contractTypeColumn.HeaderText = "Loại";
        _contractTypeColumn.Name = "ContractType";
        _contractStartColumn.DataPropertyName = "StartDate";
        _contractStartColumn.HeaderText = "Bắt đầu";
        _contractStartColumn.Name = "StartDate";
        _contractEndColumn.DataPropertyName = "EndDate";
        _contractEndColumn.HeaderText = "Kết thúc";
        _contractEndColumn.Name = "EndDate";
        _contractStatusColumn.DataPropertyName = "Status";
        _contractStatusColumn.HeaderText = "Trạng thái";
        _contractStatusColumn.Name = "Status";
        _contractNotesColumn.DataPropertyName = "Notes";
        _contractNotesColumn.HeaderText = "Ghi chú";
        _contractNotesColumn.Name = "Notes";
        _contracts.Columns.AddRange(new DataGridViewColumn[]
        {
            _contractNumberColumn, _contractTypeColumn, _contractStartColumn,
            _contractEndColumn, _contractStatusColumn, _contractNotesColumn
        });
        _contractsPage.Controls.Add(_contracts);
        _contractsPage.Name = "_contractsPage";
        _contractsPage.Padding = new Padding(3);
        _contractsPage.Text = "Hợp đồng";

        _history.AllowUserToAddRows = false;
        _history.AllowUserToDeleteRows = false;
        _history.AutoGenerateColumns = false;
        _history.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _history.BackgroundColor = Color.White;
        _history.BorderStyle = BorderStyle.FixedSingle;
        _history.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 236, 246);
        _history.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _history.Dock = DockStyle.Fill;
        _history.EnableHeadersVisualStyles = false;
        _history.MultiSelect = false;
        _history.ReadOnly = true;
        _history.RowHeadersVisible = false;
        _history.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _history.Name = "_history";
        _history.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
        _historyDepartmentColumn.DataPropertyName = "DepartmentName";
        _historyDepartmentColumn.HeaderText = "Phòng ban";
        _historyDepartmentColumn.Name = "DepartmentName";
        _historyPositionColumn.DataPropertyName = "PositionName";
        _historyPositionColumn.HeaderText = "Chức vụ";
        _historyPositionColumn.Name = "PositionName";
        _historyStartColumn.DataPropertyName = "StartDate";
        _historyStartColumn.HeaderText = "Từ ngày";
        _historyStartColumn.Name = "StartDate";
        _historyEndColumn.DataPropertyName = "EndDate";
        _historyEndColumn.HeaderText = "Đến ngày";
        _historyEndColumn.Name = "EndDate";
        _historyNotesColumn.DataPropertyName = "Notes";
        _historyNotesColumn.HeaderText = "Ghi chú";
        _historyNotesColumn.Name = "Notes";
        _history.Columns.AddRange(new DataGridViewColumn[]
        {
            _historyDepartmentColumn, _historyPositionColumn, _historyStartColumn, _historyEndColumn, _historyNotesColumn
        });
        _historyPage.Controls.Add(_history);
        _historyPage.Name = "_historyPage";
        _historyPage.Padding = new Padding(3);
        _historyPage.Text = "Lịch sử công tác";

        _tabs.Dock = DockStyle.Fill;
        _tabs.Name = "_tabs";
        _tabs.TabPages.AddRange(new TabPage[] { _contractsPage, _historyPage });
        Body.Controls.Add(_tabs);

        Name = "ContractWorkControl";
        Size = new Size(1100, 650);
        ((ISupportInitialize)_contracts).EndInit();
        ((ISupportInitialize)_history).EndInit();
        _contractsPage.ResumeLayout(false);
        _historyPage.ResumeLayout(false);
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
