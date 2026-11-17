#nullable disable
using System.ComponentModel;

namespace Admin.WinForms.GUI.Controls.HR;

public sealed partial class ProfileReviewControl
{
    private IContainer components = null;
    private TableLayoutPanel layout;
    private FlowLayoutPanel Toolbar;
    private Label Status;
    private Panel Body;
    private FlowLayoutPanel Footer;

    private ComboBox _filter;
    private DataGridView _grid;
    private Button _refreshButton;
    private Button _viewButton;
    private Button _downloadButton;
    private Button _approveButton;
    private Button _rejectButton;
    private Button _requestInfoButton;
    private DataGridViewTextBoxColumn _fullNameColumn;
    private DataGridViewTextBoxColumn _statusColumn;
    private DataGridViewTextBoxColumn _submittedAtColumn;
    private DataGridViewTextBoxColumn _reviewReasonColumn;

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
        _refreshButton = new Button();
        _viewButton = new Button();
        _downloadButton = new Button();
        _approveButton = new Button();
        _rejectButton = new Button();
        _requestInfoButton = new Button();
        _fullNameColumn = new DataGridViewTextBoxColumn();
        _statusColumn = new DataGridViewTextBoxColumn();
        _submittedAtColumn = new DataGridViewTextBoxColumn();
        _reviewReasonColumn = new DataGridViewTextBoxColumn();
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
        _filter.Width = 200;
        _filter.Margin = new Padding(3, 6, 8, 3);

        _refreshButton.AutoSize = true;
        _refreshButton.MinimumSize = new Size(95, 34);
        _refreshButton.Text = "Làm mới";
        _refreshButton.Name = "_refreshButton";
        _refreshButton.Margin = new Padding(3, 3, 8, 3);

        _viewButton.AutoSize = true;
        _viewButton.MinimumSize = new Size(95, 34);
        _viewButton.Text = "Xem đề xuất";
        _viewButton.Name = "_viewButton";
        _viewButton.Margin = new Padding(3, 3, 8, 3);

        _downloadButton.AutoSize = true;
        _downloadButton.MinimumSize = new Size(95, 34);
        _downloadButton.Text = "Tải minh chứng";
        _downloadButton.Name = "_downloadButton";
        _downloadButton.Margin = new Padding(3, 3, 8, 3);

        _approveButton.AutoSize = true;
        _approveButton.MinimumSize = new Size(95, 34);
        _approveButton.Text = "Chấp thuận";
        _approveButton.Name = "_approveButton";
        _approveButton.Margin = new Padding(3, 3, 8, 3);
        _approveButton.BackColor = Color.FromArgb(35, 83, 166);
        _approveButton.ForeColor = Color.White;
        _approveButton.FlatStyle = FlatStyle.Flat;

        _rejectButton.AutoSize = true;
        _rejectButton.MinimumSize = new Size(95, 34);
        _rejectButton.Text = "Từ chối";
        _rejectButton.Name = "_rejectButton";
        _rejectButton.Margin = new Padding(3, 3, 8, 3);

        _requestInfoButton.AutoSize = true;
        _requestInfoButton.MinimumSize = new Size(95, 34);
        _requestInfoButton.Text = "Yêu cầu bổ sung";
        _requestInfoButton.Name = "_requestInfoButton";
        _requestInfoButton.Margin = new Padding(3, 3, 8, 3);

        Toolbar.Controls.AddRange(new Control[]
        {
            _filter, _refreshButton, _viewButton, _downloadButton, _approveButton, _rejectButton, _requestInfoButton
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

        _fullNameColumn.DataPropertyName = "FullName";
        _fullNameColumn.HeaderText = "Họ tên đề xuất";
        _fullNameColumn.MinimumWidth = 120;
        _fullNameColumn.Name = "FullName";
        _fullNameColumn.FillWeight = 130F;
        _statusColumn.DataPropertyName = "Status";
        _statusColumn.HeaderText = "Trạng thái";
        _statusColumn.MinimumWidth = 80;
        _statusColumn.Name = "Status";
        _statusColumn.FillWeight = 80F;
        _submittedAtColumn.DataPropertyName = "SubmittedAt";
        _submittedAtColumn.HeaderText = "Ngày gửi";
        _submittedAtColumn.MinimumWidth = 100;
        _submittedAtColumn.Name = "SubmittedAt";
        _submittedAtColumn.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
        _reviewReasonColumn.DataPropertyName = "ReviewReason";
        _reviewReasonColumn.HeaderText = "Lý do xử lý";
        _reviewReasonColumn.MinimumWidth = 140;
        _reviewReasonColumn.Name = "ReviewReason";
        _reviewReasonColumn.FillWeight = 150F;
        _grid.Columns.AddRange(new DataGridViewColumn[]
        {
            _fullNameColumn, _statusColumn, _submittedAtColumn, _reviewReasonColumn
        });
        _grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
        Body.Controls.Add(_grid);

        Name = "ProfileReviewControl";
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
