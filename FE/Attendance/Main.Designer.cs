namespace Attendance
{
    partial class Main
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private GroupBox grpFilter;
        private Label lblDate;
        private DateTimePicker dtpDate;
        private Label lblStatus;
        private ComboBox cboStatus;
        private Label lblSearch;
        private TextBox txtSearch;
        private Button btnRefresh;
        private Button btnAdd;
        private ListView lvAttendance;
        private ColumnHeader colEmployeeId;
        private ColumnHeader colEmployeeName;
        private ColumnHeader colDepartment;
        private ColumnHeader colDate;
        private ColumnHeader colCheckIn;
        private ColumnHeader colCheckOut;
        private ColumnHeader colStatus;
        private ColumnHeader colNote;
        private Label lblTotal;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlHeader = new Panel();
            lblSubtitle = new Label();
            lblTitle = new Label();
            grpFilter = new GroupBox();
            btnAdd = new Button();
            btnRefresh = new Button();
            txtSearch = new TextBox();
            lblSearch = new Label();
            cboStatus = new ComboBox();
            lblStatus = new Label();
            dtpDate = new DateTimePicker();
            lblDate = new Label();
            lvAttendance = new ListView();
            colEmployeeId = new ColumnHeader();
            colEmployeeName = new ColumnHeader();
            colDepartment = new ColumnHeader();
            colDate = new ColumnHeader();
            colCheckIn = new ColumnHeader();
            colCheckOut = new ColumnHeader();
            colStatus = new ColumnHeader();
            colNote = new ColumnHeader();
            lblTotal = new Label();
            pnlHeader.SuspendLayout();
            grpFilter.SuspendLayout();
            SuspendLayout();
            // pnlHeader
            pnlHeader.BackColor = Color.FromArgb(31, 78, 121);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1184, 92);
            // lblTitle
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(28, 13);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(304, 41);
            lblTitle.Text = "QUẢN LÝ CHẤM CÔNG";
            // lblSubtitle
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = Color.FromArgb(220, 235, 248);
            lblSubtitle.Location = new Point(32, 57);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(270, 19);
            lblSubtitle.Text = "Theo dõi thời gian làm việc của nhân viên";
            // grpFilter
            grpFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpFilter.Controls.Add(btnAdd);
            grpFilter.Controls.Add(btnRefresh);
            grpFilter.Controls.Add(txtSearch);
            grpFilter.Controls.Add(lblSearch);
            grpFilter.Controls.Add(cboStatus);
            grpFilter.Controls.Add(lblStatus);
            grpFilter.Controls.Add(dtpDate);
            grpFilter.Controls.Add(lblDate);
            grpFilter.Location = new Point(24, 108);
            grpFilter.Name = "grpFilter";
            grpFilter.Size = new Size(1136, 86);
            grpFilter.TabStop = false;
            grpFilter.Text = "Bộ lọc và thao tác";
            // lblDate
            lblDate.AutoSize = true;
            lblDate.Location = new Point(20, 38);
            lblDate.Text = "Ngày:";
            // dtpDate
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Location = new Point(69, 34);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(130, 23);
            dtpDate.Value = new DateTime(2025, 1, 1, 0, 0, 0, 0);
            // lblStatus
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(229, 38);
            lblStatus.Text = "Trạng thái:";
            // cboStatus
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.Items.AddRange(new object[] { "Tất cả", "Đúng giờ", "Đi muộn", "Vắng mặt", "Chưa chấm công" });
            cboStatus.Location = new Point(302, 34);
            cboStatus.Name = "cboStatus";
            cboStatus.Size = new Size(150, 23);
            cboStatus.SelectedIndex = 0;
            // lblSearch
            lblSearch.AutoSize = true;
            lblSearch.Location = new Point(478, 38);
            lblSearch.Text = "Tìm kiếm:";
            // txtSearch
            txtSearch.Location = new Point(550, 34);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Mã hoặc tên nhân viên";
            txtSearch.Size = new Size(205, 23);
            // btnRefresh
            btnRefresh.BackColor = Color.FromArgb(108, 117, 125);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(775, 32);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(105, 28);
            btnRefresh.Text = "Làm mới";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // btnAdd
            btnAdd.BackColor = Color.FromArgb(25, 135, 84);
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(895, 32);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(120, 28);
            btnAdd.Text = "Thêm chấm công";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAdd_Click;
            // lvAttendance
            lvAttendance.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvAttendance.Columns.AddRange(new ColumnHeader[] { colEmployeeId, colEmployeeName, colDepartment, colDate, colCheckIn, colCheckOut, colStatus, colNote });
            lvAttendance.FullRowSelect = true;
            lvAttendance.GridLines = true;
            lvAttendance.Location = new Point(24, 214);
            lvAttendance.Name = "lvAttendance";
            lvAttendance.Size = new Size(1136, 390);
            lvAttendance.View = View.Details;
            // columns
            colEmployeeId.Text = "Mã NV";
            colEmployeeId.Width = 85;
            colEmployeeName.Text = "Họ và tên";
            colEmployeeName.Width = 190;
            colDepartment.Text = "Mã ca";
            colDepartment.Width = 145;
            colDate.Text = "Ngày";
            colDate.Width = 105;
            colCheckIn.Text = "Giờ vào";
            colCheckIn.Width = 90;
            colCheckOut.Text = "Giờ ra";
            colCheckOut.Width = 90;
            colStatus.Text = "Trạng thái";
            colStatus.Width = 150;
            colNote.Text = "Ghi chú";
            colNote.Width = 250;
            // lblTotal
            lblTotal.AutoSize = true;
            lblTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblTotal.Location = new Point(24, 620);
            lblTotal.Text = "Tổng số bản ghi: 0";
            // Main
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1184, 661);
            Controls.Add(lblTotal);
            Controls.Add(lvAttendance);
            Controls.Add(grpFilter);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(1000, 550);
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Attendance - Quản lý chấm công";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            grpFilter.ResumeLayout(false);
            grpFilter.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
