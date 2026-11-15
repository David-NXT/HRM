namespace Admin.WinForms.Controls.Dashboard
{
    partial class DashboardControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            panel_Header = new Panel();
            label_Greeting = new Label();
            label_Subtitle = new Label();
            button_Refresh = new Button();
            label_LastUpdated = new Label();

            tableLayout_KPIs = new TableLayoutPanel();
            panel_CardEmployees = new Panel();
            label_KpiEmpTitle = new Label();
            label_KpiEmpVal = new Label();
            label_KpiEmpSub = new Label();

            panel_CardAttendance = new Panel();
            label_KpiAttTitle = new Label();
            label_KpiAttVal = new Label();
            label_KpiAttSub = new Label();

            panel_CardPayroll = new Panel();
            label_KpiPayTitle = new Label();
            label_KpiPayVal = new Label();
            label_KpiPaySub = new Label();

            panel_CardProjects = new Panel();
            label_KpiProjTitle = new Label();
            label_KpiProjVal = new Label();
            label_KpiProjSub = new Label();

            tableLayout_Body = new TableLayoutPanel();
            groupBox_Approvals = new GroupBox();
            listView_Pending = new ListView();
            col_Type = new ColumnHeader();
            col_Sender = new ColumnHeader();
            col_Date = new ColumnHeader();
            col_Detail = new ColumnHeader();
            col_Status = new ColumnHeader();

            panel_RightColumn = new Panel();
            groupBox_QuickActions = new GroupBox();
            tableLayout_QuickButtons = new TableLayoutPanel();
            btnNav_HR = new Button();
            btnNav_Attendance = new Button();
            btnNav_Payroll = new Button();
            btnNav_Project = new Button();

            groupBox_SystemStatus = new GroupBox();
            tableLayout_StatusGrid = new TableLayoutPanel();
            lblStatus_HR = new Label();
            lblStatus_Attendance = new Label();
            lblStatus_Payroll = new Label();
            lblStatus_Project = new Label();

            panel_Header.SuspendLayout();
            tableLayout_KPIs.SuspendLayout();
            panel_CardEmployees.SuspendLayout();
            panel_CardAttendance.SuspendLayout();
            panel_CardPayroll.SuspendLayout();
            panel_CardProjects.SuspendLayout();
            tableLayout_Body.SuspendLayout();
            groupBox_Approvals.SuspendLayout();
            panel_RightColumn.SuspendLayout();
            groupBox_QuickActions.SuspendLayout();
            tableLayout_QuickButtons.SuspendLayout();
            groupBox_SystemStatus.SuspendLayout();
            tableLayout_StatusGrid.SuspendLayout();
            SuspendLayout();

            // 
            // panel_Header
            // 
            panel_Header.Controls.Add(label_Greeting);
            panel_Header.Controls.Add(label_Subtitle);
            panel_Header.Controls.Add(button_Refresh);
            panel_Header.Controls.Add(label_LastUpdated);
            panel_Header.Dock = DockStyle.Top;
            panel_Header.Location = new Point(0, 0);
            panel_Header.Name = "panel_Header";
            panel_Header.Padding = new Padding(15, 10, 15, 10);
            panel_Header.Size = new Size(1800, 75);
            panel_Header.TabIndex = 0;

            label_Greeting.AutoSize = true;
            label_Greeting.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
            label_Greeting.ForeColor = Color.FromArgb(17, 24, 39);
            label_Greeting.Location = new Point(15, 8);
            label_Greeting.Name = "label_Greeting";
            label_Greeting.Size = new Size(330, 38);
            label_Greeting.Text = "Xin chào Quản trị viên! 👋";

            label_Subtitle.AutoSize = true;
            label_Subtitle.Font = new Font("Segoe UI", 9.5F);
            label_Subtitle.ForeColor = Color.FromArgb(107, 114, 128);
            label_Subtitle.Location = new Point(16, 43);
            label_Subtitle.Name = "label_Subtitle";
            label_Subtitle.Size = new Size(540, 25);
            label_Subtitle.Text = "Tổng quan chỉ số hoạt động Nhân sự, Chấm công, Bảng lương và Dự án";

            button_Refresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Refresh.BackColor = Color.FromArgb(37, 99, 235);
            button_Refresh.FlatStyle = FlatStyle.Flat;
            button_Refresh.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_Refresh.ForeColor = Color.White;
            button_Refresh.Location = new Point(1615, 15);
            button_Refresh.Name = "button_Refresh";
            button_Refresh.Size = new Size(170, 42);
            button_Refresh.TabIndex = 0;
            button_Refresh.Text = "🔄 Làm mới";
            button_Refresh.UseVisualStyleBackColor = false;

            label_LastUpdated.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_LastUpdated.AutoSize = true;
            label_LastUpdated.Font = new Font("Segoe UI", 8.5F);
            label_LastUpdated.ForeColor = Color.FromArgb(107, 114, 128);
            label_LastUpdated.Location = new Point(1390, 26);
            label_LastUpdated.Name = "label_LastUpdated";
            label_LastUpdated.Size = new Size(160, 20);
            label_LastUpdated.Text = "Cập nhật lúc: --:--:--";
            label_LastUpdated.TextAlign = ContentAlignment.MiddleRight;

            // 
            // tableLayout_KPIs
            // 
            tableLayout_KPIs.ColumnCount = 4;
            tableLayout_KPIs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout_KPIs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout_KPIs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout_KPIs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout_KPIs.Controls.Add(panel_CardEmployees, 0, 0);
            tableLayout_KPIs.Controls.Add(panel_CardAttendance, 1, 0);
            tableLayout_KPIs.Controls.Add(panel_CardPayroll, 2, 0);
            tableLayout_KPIs.Controls.Add(panel_CardProjects, 3, 0);
            tableLayout_KPIs.Dock = DockStyle.Top;
            tableLayout_KPIs.Location = new Point(0, 75);
            tableLayout_KPIs.Name = "tableLayout_KPIs";
            tableLayout_KPIs.RowCount = 1;
            tableLayout_KPIs.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayout_KPIs.Size = new Size(1800, 135);
            tableLayout_KPIs.TabIndex = 1;

            // Card 1: Employees
            panel_CardEmployees.BackColor = Color.FromArgb(239, 246, 255);
            panel_CardEmployees.BorderStyle = BorderStyle.FixedSingle;
            panel_CardEmployees.Controls.Add(label_KpiEmpTitle);
            panel_CardEmployees.Controls.Add(label_KpiEmpVal);
            panel_CardEmployees.Controls.Add(label_KpiEmpSub);
            panel_CardEmployees.Dock = DockStyle.Fill;
            panel_CardEmployees.Location = new Point(10, 8);
            panel_CardEmployees.Margin = new Padding(10, 8, 10, 8);
            panel_CardEmployees.Name = "panel_CardEmployees";
            panel_CardEmployees.Padding = new Padding(15);
            panel_CardEmployees.Size = new Size(430, 119);
            panel_CardEmployees.TabIndex = 0;

            label_KpiEmpTitle.AutoSize = true;
            label_KpiEmpTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label_KpiEmpTitle.ForeColor = Color.FromArgb(30, 64, 175);
            label_KpiEmpTitle.Location = new Point(12, 10);
            label_KpiEmpTitle.Name = "label_KpiEmpTitle";
            label_KpiEmpTitle.Size = new Size(160, 25);
            label_KpiEmpTitle.Text = "👥 TỔNG NHÂN SỰ";

            label_KpiEmpVal.AutoSize = true;
            label_KpiEmpVal.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            label_KpiEmpVal.ForeColor = Color.FromArgb(29, 78, 216);
            label_KpiEmpVal.Location = new Point(10, 36);
            label_KpiEmpVal.Name = "label_KpiEmpVal";
            label_KpiEmpVal.Size = new Size(54, 48);
            label_KpiEmpVal.Text = "--";

            label_KpiEmpSub.AutoSize = true;
            label_KpiEmpSub.Font = new Font("Segoe UI", 8.5F);
            label_KpiEmpSub.ForeColor = Color.FromArgb(75, 85, 99);
            label_KpiEmpSub.Location = new Point(12, 85);
            label_KpiEmpSub.Name = "label_KpiEmpSub";
            label_KpiEmpSub.Size = new Size(160, 20);
            label_KpiEmpSub.Text = "Đang tải dữ liệu...";

            // Card 2: Attendance
            panel_CardAttendance.BackColor = Color.FromArgb(240, 253, 244);
            panel_CardAttendance.BorderStyle = BorderStyle.FixedSingle;
            panel_CardAttendance.Controls.Add(label_KpiAttTitle);
            panel_CardAttendance.Controls.Add(label_KpiAttVal);
            panel_CardAttendance.Controls.Add(label_KpiAttSub);
            panel_CardAttendance.Dock = DockStyle.Fill;
            panel_CardAttendance.Location = new Point(460, 8);
            panel_CardAttendance.Margin = new Padding(10, 8, 10, 8);
            panel_CardAttendance.Name = "panel_CardAttendance";
            panel_CardAttendance.Padding = new Padding(15);
            panel_CardAttendance.Size = new Size(430, 119);
            panel_CardAttendance.TabIndex = 1;

            label_KpiAttTitle.AutoSize = true;
            label_KpiAttTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label_KpiAttTitle.ForeColor = Color.FromArgb(22, 101, 52);
            label_KpiAttTitle.Location = new Point(12, 10);
            label_KpiAttTitle.Name = "label_KpiAttTitle";
            label_KpiAttTitle.Size = new Size(190, 25);
            label_KpiAttTitle.Text = "⏱️ CÔNG HÔM NAY";

            label_KpiAttVal.AutoSize = true;
            label_KpiAttVal.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            label_KpiAttVal.ForeColor = Color.FromArgb(21, 128, 61);
            label_KpiAttVal.Location = new Point(10, 36);
            label_KpiAttVal.Name = "label_KpiAttVal";
            label_KpiAttVal.Size = new Size(54, 48);
            label_KpiAttVal.Text = "--";

            label_KpiAttSub.AutoSize = true;
            label_KpiAttSub.Font = new Font("Segoe UI", 8.5F);
            label_KpiAttSub.ForeColor = Color.FromArgb(75, 85, 99);
            label_KpiAttSub.Location = new Point(12, 85);
            label_KpiAttSub.Name = "label_KpiAttSub";
            label_KpiAttSub.Size = new Size(160, 20);
            label_KpiAttSub.Text = "Có mặt: 0 | Đi trễ: 0";

            // Card 3: Payroll
            panel_CardPayroll.BackColor = Color.FromArgb(254, 243, 199);
            panel_CardPayroll.BorderStyle = BorderStyle.FixedSingle;
            panel_CardPayroll.Controls.Add(label_KpiPayTitle);
            panel_CardPayroll.Controls.Add(label_KpiPayVal);
            panel_CardPayroll.Controls.Add(label_KpiPaySub);
            panel_CardPayroll.Dock = DockStyle.Fill;
            panel_CardPayroll.Location = new Point(910, 8);
            panel_CardPayroll.Margin = new Padding(10, 8, 10, 8);
            panel_CardPayroll.Name = "panel_CardPayroll";
            panel_CardPayroll.Padding = new Padding(15);
            panel_CardPayroll.Size = new Size(430, 119);
            panel_CardPayroll.TabIndex = 2;

            label_KpiPayTitle.AutoSize = true;
            label_KpiPayTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label_KpiPayTitle.ForeColor = Color.FromArgb(146, 64, 14);
            label_KpiPayTitle.Location = new Point(12, 10);
            label_KpiPayTitle.Name = "label_KpiPayTitle";
            label_KpiPayTitle.Size = new Size(210, 25);
            label_KpiPayTitle.Text = "💰 QUỸ LƯƠNG GẦN NHẤT";

            label_KpiPayVal.AutoSize = true;
            label_KpiPayVal.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            label_KpiPayVal.ForeColor = Color.FromArgb(180, 83, 9);
            label_KpiPayVal.Location = new Point(10, 36);
            label_KpiPayVal.Name = "label_KpiPayVal";
            label_KpiPayVal.Size = new Size(95, 48);
            label_KpiPayVal.Text = "-- đ";

            label_KpiPaySub.AutoSize = true;
            label_KpiPaySub.Font = new Font("Segoe UI", 8.5F);
            label_KpiPaySub.ForeColor = Color.FromArgb(75, 85, 99);
            label_KpiPaySub.Location = new Point(12, 85);
            label_KpiPaySub.Name = "label_KpiPaySub";
            label_KpiPaySub.Size = new Size(160, 20);
            label_KpiPaySub.Text = "Kỳ lương: --";

            // Card 4: Projects
            panel_CardProjects.BackColor = Color.FromArgb(245, 243, 255);
            panel_CardProjects.BorderStyle = BorderStyle.FixedSingle;
            panel_CardProjects.Controls.Add(label_KpiProjTitle);
            panel_CardProjects.Controls.Add(label_KpiProjVal);
            panel_CardProjects.Controls.Add(label_KpiProjSub);
            panel_CardProjects.Dock = DockStyle.Fill;
            panel_CardProjects.Location = new Point(1360, 8);
            panel_CardProjects.Margin = new Padding(10, 8, 10, 8);
            panel_CardProjects.Name = "panel_CardProjects";
            panel_CardProjects.Padding = new Padding(15);
            panel_CardProjects.Size = new Size(430, 119);
            panel_CardProjects.TabIndex = 3;

            label_KpiProjTitle.AutoSize = true;
            label_KpiProjTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label_KpiProjTitle.ForeColor = Color.FromArgb(107, 33, 168);
            label_KpiProjTitle.Location = new Point(12, 10);
            label_KpiProjTitle.Name = "label_KpiProjTitle";
            label_KpiProjTitle.Size = new Size(190, 25);
            label_KpiProjTitle.Text = "📊 DỰ ÁN & NHIỆM VỤ";

            label_KpiProjVal.AutoSize = true;
            label_KpiProjVal.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            label_KpiProjVal.ForeColor = Color.FromArgb(126, 34, 206);
            label_KpiProjVal.Location = new Point(10, 36);
            label_KpiProjVal.Name = "label_KpiProjVal";
            label_KpiProjVal.Size = new Size(54, 48);
            label_KpiProjVal.Text = "--";

            label_KpiProjSub.AutoSize = true;
            label_KpiProjSub.Font = new Font("Segoe UI", 8.5F);
            label_KpiProjSub.ForeColor = Color.FromArgb(75, 85, 99);
            label_KpiProjSub.Location = new Point(12, 85);
            label_KpiProjSub.Name = "label_KpiProjSub";
            label_KpiProjSub.Size = new Size(160, 20);
            label_KpiProjSub.Text = "-- Nhiệm vụ đang chạy";

            // 
            // tableLayout_Body
            // 
            tableLayout_Body.ColumnCount = 2;
            tableLayout_Body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayout_Body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayout_Body.Controls.Add(groupBox_Approvals, 0, 0);
            tableLayout_Body.Controls.Add(panel_RightColumn, 1, 0);
            tableLayout_Body.Dock = DockStyle.Fill;
            tableLayout_Body.Location = new Point(0, 210);
            tableLayout_Body.Name = "tableLayout_Body";
            tableLayout_Body.RowCount = 1;
            tableLayout_Body.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayout_Body.Size = new Size(1800, 670);
            tableLayout_Body.TabIndex = 2;

            // 
            // groupBox_Approvals
            // 
            groupBox_Approvals.Controls.Add(listView_Pending);
            groupBox_Approvals.Dock = DockStyle.Fill;
            groupBox_Approvals.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox_Approvals.ForeColor = Color.FromArgb(31, 41, 55);
            groupBox_Approvals.Location = new Point(10, 10);
            groupBox_Approvals.Margin = new Padding(10);
            groupBox_Approvals.Name = "groupBox_Approvals";
            groupBox_Approvals.Padding = new Padding(10);
            groupBox_Approvals.Size = new Size(1060, 650);
            groupBox_Approvals.TabIndex = 0;
            groupBox_Approvals.TabStop = false;
            groupBox_Approvals.Text = "📋 Yêu cầu & Đơn từ cần xử lý ngay";

            // 
            // listView_Pending
            // 
            listView_Pending.BorderStyle = BorderStyle.FixedSingle;
            listView_Pending.Columns.AddRange(new ColumnHeader[] {
                col_Type,
                col_Sender,
                col_Date,
                col_Detail,
                col_Status
            });
            listView_Pending.Dock = DockStyle.Fill;
            listView_Pending.Font = new Font("Segoe UI", 9.5F);
            listView_Pending.FullRowSelect = true;
            listView_Pending.GridLines = true;
            listView_Pending.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView_Pending.Location = new Point(10, 37);
            listView_Pending.Name = "listView_Pending";
            listView_Pending.Size = new Size(1040, 603);
            listView_Pending.TabIndex = 0;
            listView_Pending.UseCompatibleStateImageBehavior = false;
            listView_Pending.View = View.Details;

            col_Type.Text = "Loại yêu cầu";
            col_Type.Width = 170;

            col_Sender.Text = "Mã NV / Người gửi";
            col_Sender.Width = 190;

            col_Date.Text = "Thời gian";
            col_Date.Width = 140;

            col_Detail.Text = "Lý do / Nội dung chi tiết";
            col_Detail.Width = 350;

            col_Status.Text = "Trạng thái";
            col_Status.Width = 150;

            // 
            // panel_RightColumn
            // 
            panel_RightColumn.Controls.Add(groupBox_QuickActions);
            panel_RightColumn.Controls.Add(groupBox_SystemStatus);
            panel_RightColumn.Dock = DockStyle.Fill;
            panel_RightColumn.Location = new Point(1090, 10);
            panel_RightColumn.Margin = new Padding(10);
            panel_RightColumn.Name = "panel_RightColumn";
            panel_RightColumn.Size = new Size(700, 650);
            panel_RightColumn.TabIndex = 1;

            // 
            // groupBox_QuickActions
            // 
            groupBox_QuickActions.Controls.Add(tableLayout_QuickButtons);
            groupBox_QuickActions.Dock = DockStyle.Top;
            groupBox_QuickActions.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox_QuickActions.ForeColor = Color.FromArgb(31, 41, 55);
            groupBox_QuickActions.Location = new Point(0, 0);
            groupBox_QuickActions.Name = "groupBox_QuickActions";
            groupBox_QuickActions.Padding = new Padding(10);
            groupBox_QuickActions.Size = new Size(700, 240);
            groupBox_QuickActions.TabIndex = 0;
            groupBox_QuickActions.TabStop = false;
            groupBox_QuickActions.Text = "⚡ Phím tắt điều hướng nhanh";

            // 
            // tableLayout_QuickButtons
            // 
            tableLayout_QuickButtons.ColumnCount = 2;
            tableLayout_QuickButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout_QuickButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout_QuickButtons.Controls.Add(btnNav_HR, 0, 0);
            tableLayout_QuickButtons.Controls.Add(btnNav_Attendance, 1, 0);
            tableLayout_QuickButtons.Controls.Add(btnNav_Payroll, 0, 1);
            tableLayout_QuickButtons.Controls.Add(btnNav_Project, 1, 1);
            tableLayout_QuickButtons.Dock = DockStyle.Fill;
            tableLayout_QuickButtons.Location = new Point(10, 37);
            tableLayout_QuickButtons.Name = "tableLayout_QuickButtons";
            tableLayout_QuickButtons.RowCount = 2;
            tableLayout_QuickButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayout_QuickButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayout_QuickButtons.Size = new Size(680, 193);
            tableLayout_QuickButtons.TabIndex = 0;

            btnNav_HR.BackColor = Color.FromArgb(239, 246, 255);
            btnNav_HR.FlatStyle = FlatStyle.Flat;
            btnNav_HR.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnNav_HR.ForeColor = Color.FromArgb(30, 64, 175);
            btnNav_HR.Dock = DockStyle.Fill;
            btnNav_HR.Location = new Point(6, 6);
            btnNav_HR.Margin = new Padding(6);
            btnNav_HR.Name = "btnNav_HR";
            btnNav_HR.Size = new Size(328, 84);
            btnNav_HR.TabIndex = 0;
            btnNav_HR.Text = "👤 Hồ sơ Nhân sự";
            btnNav_HR.UseVisualStyleBackColor = false;

            btnNav_Attendance.BackColor = Color.FromArgb(240, 253, 244);
            btnNav_Attendance.FlatStyle = FlatStyle.Flat;
            btnNav_Attendance.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnNav_Attendance.ForeColor = Color.FromArgb(22, 101, 52);
            btnNav_Attendance.Dock = DockStyle.Fill;
            btnNav_Attendance.Location = new Point(346, 6);
            btnNav_Attendance.Margin = new Padding(6);
            btnNav_Attendance.Name = "btnNav_Attendance";
            btnNav_Attendance.Size = new Size(328, 84);
            btnNav_Attendance.TabIndex = 1;
            btnNav_Attendance.Text = "⏱️ Chấm công & Duyệt đơn";
            btnNav_Attendance.UseVisualStyleBackColor = false;

            btnNav_Payroll.BackColor = Color.FromArgb(254, 243, 199);
            btnNav_Payroll.FlatStyle = FlatStyle.Flat;
            btnNav_Payroll.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnNav_Payroll.ForeColor = Color.FromArgb(146, 64, 14);
            btnNav_Payroll.Dock = DockStyle.Fill;
            btnNav_Payroll.Location = new Point(6, 102);
            btnNav_Payroll.Margin = new Padding(6);
            btnNav_Payroll.Name = "btnNav_Payroll";
            btnNav_Payroll.Size = new Size(328, 85);
            btnNav_Payroll.TabIndex = 2;
            btnNav_Payroll.Text = "💵 Quản lý Bảng lương";
            btnNav_Payroll.UseVisualStyleBackColor = false;

            btnNav_Project.BackColor = Color.FromArgb(245, 243, 255);
            btnNav_Project.FlatStyle = FlatStyle.Flat;
            btnNav_Project.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnNav_Project.ForeColor = Color.FromArgb(107, 33, 168);
            btnNav_Project.Dock = DockStyle.Fill;
            btnNav_Project.Location = new Point(346, 102);
            btnNav_Project.Margin = new Padding(6);
            btnNav_Project.Name = "btnNav_Project";
            btnNav_Project.Size = new Size(328, 85);
            btnNav_Project.TabIndex = 3;
            btnNav_Project.Text = "🚀 Quản lý Dự án & Task";
            btnNav_Project.UseVisualStyleBackColor = false;

            // 
            // groupBox_SystemStatus
            // 
            groupBox_SystemStatus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox_SystemStatus.Controls.Add(tableLayout_StatusGrid);
            groupBox_SystemStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox_SystemStatus.ForeColor = Color.FromArgb(31, 41, 55);
            groupBox_SystemStatus.Location = new Point(0, 250);
            groupBox_SystemStatus.Name = "groupBox_SystemStatus";
            groupBox_SystemStatus.Padding = new Padding(12);
            groupBox_SystemStatus.Size = new Size(700, 395);
            groupBox_SystemStatus.TabIndex = 1;
            groupBox_SystemStatus.TabStop = false;
            groupBox_SystemStatus.Text = "🔌 Tình trạng kết nối Dịch vụ (Microservices)";

            // 
            // tableLayout_StatusGrid
            // 
            tableLayout_StatusGrid.ColumnCount = 1;
            tableLayout_StatusGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayout_StatusGrid.Controls.Add(lblStatus_HR, 0, 0);
            tableLayout_StatusGrid.Controls.Add(lblStatus_Attendance, 0, 1);
            tableLayout_StatusGrid.Controls.Add(lblStatus_Payroll, 0, 2);
            tableLayout_StatusGrid.Controls.Add(lblStatus_Project, 0, 3);
            tableLayout_StatusGrid.Dock = DockStyle.Fill;
            tableLayout_StatusGrid.Location = new Point(12, 39);
            tableLayout_StatusGrid.Name = "tableLayout_StatusGrid";
            tableLayout_StatusGrid.RowCount = 4;
            tableLayout_StatusGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayout_StatusGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayout_StatusGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayout_StatusGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayout_StatusGrid.Size = new Size(676, 344);
            tableLayout_StatusGrid.TabIndex = 0;

            lblStatus_HR.Font = new Font("Segoe UI", 9.5F);
            lblStatus_HR.Dock = DockStyle.Fill;
            lblStatus_HR.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus_HR.Text = "🟢 HR Service (localhost:5012) — Đang kiểm tra...";

            lblStatus_Attendance.Font = new Font("Segoe UI", 9.5F);
            lblStatus_Attendance.Dock = DockStyle.Fill;
            lblStatus_Attendance.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus_Attendance.Text = "🟢 Attendance Service (localhost:5231) — Đang kiểm tra...";

            lblStatus_Payroll.Font = new Font("Segoe UI", 9.5F);
            lblStatus_Payroll.Dock = DockStyle.Fill;
            lblStatus_Payroll.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus_Payroll.Text = "🟢 Payroll Service (localhost:5037) — Đang kiểm tra...";

            lblStatus_Project.Font = new Font("Segoe UI", 9.5F);
            lblStatus_Project.Dock = DockStyle.Fill;
            lblStatus_Project.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus_Project.Text = "🟢 Project Service (localhost:5207) — Đang kiểm tra...";

            // 
            // DashboardControl
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(249, 250, 251);
            Controls.Add(tableLayout_Body);
            Controls.Add(tableLayout_KPIs);
            Controls.Add(panel_Header);
            Name = "DashboardControl";
            Size = new Size(1800, 880);

            panel_Header.ResumeLayout(false);
            panel_Header.PerformLayout();
            tableLayout_KPIs.ResumeLayout(false);
            panel_CardEmployees.ResumeLayout(false);
            panel_CardEmployees.PerformLayout();
            panel_CardAttendance.ResumeLayout(false);
            panel_CardAttendance.PerformLayout();
            panel_CardPayroll.ResumeLayout(false);
            panel_CardPayroll.PerformLayout();
            panel_CardProjects.ResumeLayout(false);
            panel_CardProjects.PerformLayout();
            tableLayout_Body.ResumeLayout(false);
            groupBox_Approvals.ResumeLayout(false);
            panel_RightColumn.ResumeLayout(false);
            groupBox_QuickActions.ResumeLayout(false);
            tableLayout_QuickButtons.ResumeLayout(false);
            groupBox_SystemStatus.ResumeLayout(false);
            tableLayout_StatusGrid.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Header;
        private Label label_Greeting;
        private Label label_Subtitle;
        private Button button_Refresh;
        private Label label_LastUpdated;

        private TableLayoutPanel tableLayout_KPIs;
        private Panel panel_CardEmployees;
        private Label label_KpiEmpTitle;
        private Label label_KpiEmpVal;
        private Label label_KpiEmpSub;

        private Panel panel_CardAttendance;
        private Label label_KpiAttTitle;
        private Label label_KpiAttVal;
        private Label label_KpiAttSub;

        private Panel panel_CardPayroll;
        private Label label_KpiPayTitle;
        private Label label_KpiPayVal;
        private Label label_KpiPaySub;

        private Panel panel_CardProjects;
        private Label label_KpiProjTitle;
        private Label label_KpiProjVal;
        private Label label_KpiProjSub;

        private TableLayoutPanel tableLayout_Body;
        private GroupBox groupBox_Approvals;
        private ListView listView_Pending;
        private ColumnHeader col_Type;
        private ColumnHeader col_Sender;
        private ColumnHeader col_Date;
        private ColumnHeader col_Detail;
        private ColumnHeader col_Status;

        private Panel panel_RightColumn;
        private GroupBox groupBox_QuickActions;
        private TableLayoutPanel tableLayout_QuickButtons;
        private Button btnNav_HR;
        private Button btnNav_Attendance;
        private Button btnNav_Payroll;
        private Button btnNav_Project;

        private GroupBox groupBox_SystemStatus;
        private TableLayoutPanel tableLayout_StatusGrid;
        private Label lblStatus_HR;
        private Label lblStatus_Attendance;
        private Label lblStatus_Payroll;
        private Label lblStatus_Project;
    }
}
