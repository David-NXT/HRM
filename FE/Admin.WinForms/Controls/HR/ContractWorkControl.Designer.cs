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
    private Label _contractFilterLabel;
    private ComboBox _contractFilter;
    private TableLayoutPanel _contractLayout;
    private Label _contractSummary;
    private Label _contractLegend;
    private Label _contractCountLabel;
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
    private DataGridViewTextBoxColumn _contractDateStatusColumn;
    private DataGridViewTextBoxColumn _contractRemainingColumn;
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
        DataGridViewCellStyle contractEndStyle = new DataGridViewCellStyle();
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
        _contractFilterLabel = new Label();
        _contractFilter = new ComboBox();
        _contractLayout = new TableLayoutPanel();
        _contractSummary = new Label();
        _contractLegend = new Label();
        _contractCountLabel = new Label();
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
        _contractDateStatusColumn = new DataGridViewTextBoxColumn();
        _contractRemainingColumn = new DataGridViewTextBoxColumn();
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
        _contractLayout.SuspendLayout();
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
        //
        // _contractFilterLabel
        //
        _contractFilterLabel.AutoSize = true;
        _contractFilterLabel.Margin = new Padding(3, 12, 3, 3);
        _contractFilterLabel.Name = "_contractFilterLabel";
        _contractFilterLabel.Text = "Lọc hợp đồng:";
        //
        // _contractFilter
        //
        _contractFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        _contractFilter.Items.AddRange(new object[]
        {
            "Tất cả", "Đang hiệu lực (gồm sắp hết hạn)", "Sắp hết hạn (0–30 ngày)", "Đã hết hạn", "Chưa bắt đầu"
        });
        _contractFilter.Margin = new Padding(3, 6, 8, 3);
        _contractFilter.Name = "_contractFilter";
        _contractFilter.Size = new Size(250, 23);
        _contractFilter.TabIndex = 6;
        Toolbar.Controls.AddRange(new Control[]
        {
            _employees, _refreshButton, _addContractButton, _editContractButton, _renewButton, _assignmentButton,
            _contractFilterLabel, _contractFilter
        });
        Toolbar.SetFlowBreak(_assignmentButton, true);

        _contracts.AllowUserToAddRows = false;
        _contracts.AllowUserToDeleteRows = false;
        _contracts.AutoGenerateColumns = false;
        _contracts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _contracts.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
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
        _contractNumberColumn.FillWeight = 85F;
        _contractNumberColumn.MinimumWidth = 85;
        _contractTypeColumn.DataPropertyName = "ContractType";
        _contractTypeColumn.HeaderText = "Loại";
        _contractTypeColumn.Name = "ContractType";
        _contractTypeColumn.FillWeight = 85F;
        _contractStartColumn.DataPropertyName = "StartDate";
        _contractStartColumn.HeaderText = "Bắt đầu";
        _contractStartColumn.Name = "StartDate";
        _contractStartColumn.MinimumWidth = 85;
        _contractStartColumn.FillWeight = 80F;
        _contractEndColumn.DataPropertyName = "EndDate";
        _contractEndColumn.HeaderText = "Kết thúc";
        _contractEndColumn.Name = "EndDate";
        contractEndStyle.NullValue = "Không thời hạn";
        _contractEndColumn.DefaultCellStyle = contractEndStyle;
        _contractEndColumn.MinimumWidth = 95;
        _contractEndColumn.FillWeight = 95F;
        _contractStatusColumn.DataPropertyName = "Status";
        _contractStatusColumn.HeaderText = "Trạng thái quản lý";
        _contractStatusColumn.Name = "Status";
        _contractStatusColumn.FillWeight = 100F;
        _contractStatusColumn.MinimumWidth = 90;
        //
        // _contractDateStatusColumn
        //
        _contractDateStatusColumn.DataPropertyName = "PeriodLabel";
        _contractDateStatusColumn.HeaderText = "Tình trạng theo ngày";
        _contractDateStatusColumn.Name = "_contractDateStatusColumn";
        _contractDateStatusColumn.FillWeight = 120F;
        _contractDateStatusColumn.MinimumWidth = 115;
        _contractDateStatusColumn.ReadOnly = true;
        _contractDateStatusColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // _contractRemainingColumn
        //
        _contractRemainingColumn.DataPropertyName = "TimeRemaining";
        _contractRemainingColumn.HeaderText = "Thời hạn";
        _contractRemainingColumn.Name = "_contractRemainingColumn";
        _contractRemainingColumn.FillWeight = 130F;
        _contractRemainingColumn.MinimumWidth = 125;
        _contractRemainingColumn.ReadOnly = true;
        _contractRemainingColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        _contractNotesColumn.DataPropertyName = "Notes";
        _contractNotesColumn.HeaderText = "Ghi chú";
        _contractNotesColumn.Name = "Notes";
        _contracts.Columns.AddRange(new DataGridViewColumn[]
        {
            _contractNumberColumn, _contractTypeColumn, _contractStartColumn,
            _contractEndColumn, _contractDateStatusColumn, _contractRemainingColumn,
            _contractStatusColumn, _contractNotesColumn
        });
        //
        // _contractLayout
        //
        _contractLayout.ColumnCount = 1;
        _contractLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _contractLayout.Controls.Add(_contractSummary, 0, 0);
        _contractLayout.Controls.Add(_contracts, 0, 1);
        _contractLayout.Controls.Add(_contractLegend, 0, 2);
        _contractLayout.Dock = DockStyle.Fill;
        _contractLayout.Name = "_contractLayout";
        _contractLayout.RowCount = 3;
        _contractLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _contractLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _contractLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _contractLayout.TabIndex = 0;
        //
        // _contractSummary
        //
        _contractSummary.AutoSize = true;
        _contractSummary.Dock = DockStyle.Fill;
        _contractSummary.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _contractSummary.ForeColor = Color.FromArgb(80, 96, 115);
        _contractSummary.Margin = new Padding(6, 12, 6, 12);
        _contractSummary.Name = "_contractSummary";
        _contractSummary.TabIndex = 0;
        _contractSummary.Text = "Chọn nhân viên để xem tổng quan hợp đồng.";
        //
        // _contractLegend
        //
        _contractLegend.AutoSize = true;
        _contractLegend.Dock = DockStyle.Fill;
        _contractLegend.ForeColor = Color.FromArgb(80, 96, 115);
        _contractLegend.Margin = new Padding(6, 10, 6, 6);
        _contractLegend.Name = "_contractLegend";
        _contractLegend.TabIndex = 2;
        _contractLegend.Text = "Tình trạng theo ngày tính theo ngày bắt đầu và kết thúc; ngày kết thúc vẫn được tính là còn hiệu lực.\r\nACTIVE/RENEWED là trạng thái quản lý. Hợp đồng đã gia hạn vẫn có thể bao phủ hôm nay trước khi hợp đồng mới bắt đầu.";
        //
        // _contractCountLabel
        //
        _contractCountLabel.AutoSize = true;
        _contractCountLabel.Margin = new Padding(3, 8, 3, 3);
        _contractCountLabel.Name = "_contractCountLabel";
        _contractCountLabel.TabIndex = 0;
        Footer.Controls.Add(_contractCountLabel);
        _contractsPage.Controls.Add(_contractLayout);
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
        _contractLayout.ResumeLayout(false);
        _contractLayout.PerformLayout();
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
