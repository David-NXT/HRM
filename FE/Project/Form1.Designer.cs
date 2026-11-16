namespace Project
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private GroupBox grpSubmission;
        private ComboBox cmbEmployee;
        private ComboBox cmbProject;
        private ComboBox cmbTask;
        private DateTimePicker dtpWorkDate;
        private TextBox txtHours;
        private Label lblAttendanceHours;
        private TextBox txtDescription;
        private Button btnSubmit;
        private Button btnRefresh;
        private ListView lvTimesheets;
        private Label lblSummary;
        private Label lblStatus;

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
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            grpSubmission = new GroupBox();
            cmbEmployee = new ComboBox();
            cmbProject = new ComboBox();
            cmbTask = new ComboBox();
            dtpWorkDate = new DateTimePicker();
            lblAttendanceHours = new Label();
            txtDescription = new TextBox();
            btnSubmit = new Button();
            btnRefresh = new Button();
            lvTimesheets = new ListView();
            lblSummary = new Label();
            lblStatus = new Label();

            SuspendLayout();
            pnlHeader.BackColor = Color.FromArgb(31, 78, 121);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 88;
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 21F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(24, 10);
            lblTitle.Text = "KHAI BÁO TIMESHEET";
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = Color.FromArgb(220, 235, 248);
            lblSubtitle.Location = new Point(29, 55);
            lblSubtitle.Text = "Giờ khai báo chỉ được gửi khi khớp với dữ liệu chấm công attendance.";

            grpSubmission.Dock = DockStyle.Top;
            grpSubmission.Height = 244;
            grpSubmission.Padding = new Padding(12);
            grpSubmission.Text = "Thông tin khai báo";
            grpSubmission.Controls.Add(CreateSubmissionLayout());

            lvTimesheets.Dock = DockStyle.Fill;
            lvTimesheets.FullRowSelect = true;
            lvTimesheets.GridLines = true;
            lvTimesheets.HideSelection = false;
            lvTimesheets.View = View.Details;
            lvTimesheets.Columns.Add("Ngày làm", 100);
            lvTimesheets.Columns.Add("Dự án", 180);
            lvTimesheets.Columns.Add("Công việc", 180);
            lvTimesheets.Columns.Add("Giờ", 65);
            lvTimesheets.Columns.Add("Trạng thái", 105);
            lvTimesheets.Columns.Add("Mô tả", 280);
            lvTimesheets.Columns.Add("Lý do từ chối", 220);

            var footer = new Panel { Dock = DockStyle.Bottom, Height = 58, Padding = new Padding(18, 5, 18, 5) };
            footer.Controls.Add(lblStatus);
            footer.Controls.Add(lblSummary);
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus.Text = "Đang chờ tải dữ liệu...";
            lblSummary.Dock = DockStyle.Right;
            lblSummary.TextAlign = ContentAlignment.MiddleRight;
            lblSummary.Width = 180;
            lblSummary.Text = "Tổng số khai báo: 0";

            Controls.Add(lvTimesheets);
            Controls.Add(footer);
            Controls.Add(grpSubmission);
            Controls.Add(pnlHeader);
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1180, 720);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(1000, 650);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Khai báo Timesheet";
            ResumeLayout(false);
        }

        private Control CreateSubmissionLayout()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 5,
                Padding = new Padding(4)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

            cmbEmployee = CreateComboBox();
            cmbProject = CreateComboBox();
            cmbTask = CreateComboBox();
            dtpWorkDate = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Dock = DockStyle.Fill };
            txtHours = new TextBox
            {
                Dock = DockStyle.Fill,
                MaxLength = 32,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                TabStop = true,
                PlaceholderText = "Ví dụ: 8 hoặc 7,5"
            };
            lblAttendanceHours = new Label
            {
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                ForeColor = Color.FromArgb(90, 90, 90),
                Text = "Chưa kiểm tra"
            };
            txtDescription = new TextBox { Multiline = true, MaxLength = 1000, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };
            btnSubmit = new Button
            {
                Text = "Kiểm tra & gửi",
                Width = 140,
                Height = 32,
                BackColor = Color.FromArgb(25, 135, 84),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh = new Button
            {
                Text = "Tải lại dữ liệu",
                Width = 130,
                Height = 32,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            AddField(layout, "Nhân viên:", cmbEmployee, 0, 0);
            AddField(layout, "Dự án:", cmbProject, 2, 0);
            AddField(layout, "Ngày làm:", dtpWorkDate, 0, 1);
            AddField(layout, "Công việc:", cmbTask, 2, 1);
            AddField(layout, "Số giờ:", txtHours, 0, 2);
            AddField(layout, "Giờ attendance:", lblAttendanceHours, 2, 2);
            layout.Controls.Add(CreateLabel("Mô tả:"), 0, 3);
            layout.Controls.Add(txtDescription, 1, 3);
            layout.SetColumnSpan(txtDescription, 3);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };
            buttons.Controls.Add(btnSubmit);
            buttons.Controls.Add(btnRefresh);
            layout.Controls.Add(buttons, 0, 4);
            layout.SetColumnSpan(buttons, 4);
            return layout;
        }

        private static ComboBox CreateComboBox() => new()
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        private static void AddField(TableLayoutPanel layout, string labelText, Control control, int column, int row)
        {
            layout.Controls.Add(CreateLabel(labelText), column, row);
            layout.Controls.Add(control, column + 1, row);
        }

        private static Label CreateLabel(string text) => new()
        {
            Text = text,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            TextAlign = ContentAlignment.MiddleLeft
        };
    }
}
