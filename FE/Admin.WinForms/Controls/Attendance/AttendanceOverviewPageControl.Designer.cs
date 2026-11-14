namespace Admin.WinForms.Controls.Attendance
{
    partial class AttendanceOverviewPageControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel_Statistics = new Panel();
            panel_TotalAbsent = new Panel();
            label_TotalAbsent = new Label();
            label_NumberAbsent = new Label();
            panel_TotalLate = new Panel();
            label_TotalLate = new Label();
            label_NumberLate = new Label();
            panel_TotalPresent = new Panel();
            label_TotalPresent = new Label();
            label_NumberPresent = new Label();
            panel_TotalEmployees = new Panel();
            label_TotalEmployees = new Label();
            label_NumberEmployees = new Label();
            panel_Filter = new Panel();
            button_Search = new Button();
            textBox_Search = new TextBox();
            comboBox_Status = new ComboBox();
            comboBox_Department = new ComboBox();
            dateTimePicker1 = new DateTimePicker();
            panel_List = new Panel();
            button_Print = new Button();
            button_ExportExcel = new Button();
            label_List = new Label();
            listView_Attendance = new ListView();
            col_EmpId = new ColumnHeader();
            col_EmpName = new ColumnHeader();
            col_Date = new ColumnHeader();
            col_Shift = new ColumnHeader();
            col_TimeIn = new ColumnHeader();
            col_TimeOut = new ColumnHeader();
            col_WorkHour = new ColumnHeader();
            col_Status = new ColumnHeader();
            col_LateMins = new ColumnHeader();
            col_EarlyLeaveMins = new ColumnHeader();
            col_OTMins = new ColumnHeader();
            col_CheckInLocation = new ColumnHeader();
            col_DeviceId = new ColumnHeader();
            col_Note = new ColumnHeader();
            panel_Statistics.SuspendLayout();
            panel_TotalAbsent.SuspendLayout();
            panel_TotalLate.SuspendLayout();
            panel_TotalPresent.SuspendLayout();
            panel_TotalEmployees.SuspendLayout();
            panel_Filter.SuspendLayout();
            panel_List.SuspendLayout();
            SuspendLayout();
            // 
            // panel_Statistics
            // 
            panel_Statistics.Controls.Add(panel_TotalAbsent);
            panel_Statistics.Controls.Add(panel_TotalLate);
            panel_Statistics.Controls.Add(panel_TotalPresent);
            panel_Statistics.Controls.Add(panel_TotalEmployees);
            panel_Statistics.Location = new Point(5, 7);
            panel_Statistics.Name = "panel_Statistics";
            panel_Statistics.Size = new Size(1808, 142);
            panel_Statistics.TabIndex = 13;
            // 
            // panel_TotalAbsent
            // 
            panel_TotalAbsent.BackColor = Color.FromArgb(253, 237, 240);
            panel_TotalAbsent.Controls.Add(label_TotalAbsent);
            panel_TotalAbsent.Controls.Add(label_NumberAbsent);
            panel_TotalAbsent.Location = new Point(1364, 13);
            panel_TotalAbsent.Name = "panel_TotalAbsent";
            panel_TotalAbsent.Size = new Size(430, 113);
            panel_TotalAbsent.TabIndex = 3;
            // 
            // label_TotalAbsent
            // 
            label_TotalAbsent.AutoSize = true;
            label_TotalAbsent.Font = new Font("Segoe UI", 10F);
            label_TotalAbsent.Location = new Point(13, 63);
            label_TotalAbsent.Name = "label_TotalAbsent";
            label_TotalAbsent.Size = new Size(99, 37);
            label_TotalAbsent.TabIndex = 7;
            label_TotalAbsent.Text = "Absent";
            // 
            // label_NumberAbsent
            // 
            label_NumberAbsent.AutoSize = true;
            label_NumberAbsent.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label_NumberAbsent.Location = new Point(13, 9);
            label_NumberAbsent.Name = "label_NumberAbsent";
            label_NumberAbsent.Size = new Size(46, 54);
            label_NumberAbsent.TabIndex = 6;
            label_NumberAbsent.Text = "0";
            // 
            // panel_TotalLate
            // 
            panel_TotalLate.BackColor = Color.FromArgb(254, 244, 234);
            panel_TotalLate.Controls.Add(label_TotalLate);
            panel_TotalLate.Controls.Add(label_NumberLate);
            panel_TotalLate.Location = new Point(914, 13);
            panel_TotalLate.Name = "panel_TotalLate";
            panel_TotalLate.Size = new Size(430, 113);
            panel_TotalLate.TabIndex = 2;
            // 
            // label_TotalLate
            // 
            label_TotalLate.AutoSize = true;
            label_TotalLate.Font = new Font("Segoe UI", 10F);
            label_TotalLate.Location = new Point(20, 63);
            label_TotalLate.Name = "label_TotalLate";
            label_TotalLate.Size = new Size(67, 37);
            label_TotalLate.TabIndex = 5;
            label_TotalLate.Text = "Late";
            // 
            // label_NumberLate
            // 
            label_NumberLate.AutoSize = true;
            label_NumberLate.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label_NumberLate.Location = new Point(20, 9);
            label_NumberLate.Name = "label_NumberLate";
            label_NumberLate.Size = new Size(46, 54);
            label_NumberLate.TabIndex = 4;
            label_NumberLate.Text = "0";
            // 
            // panel_TotalPresent
            // 
            panel_TotalPresent.BackColor = Color.FromArgb(236, 249, 240);
            panel_TotalPresent.Controls.Add(label_TotalPresent);
            panel_TotalPresent.Controls.Add(label_NumberPresent);
            panel_TotalPresent.Location = new Point(464, 13);
            panel_TotalPresent.Name = "panel_TotalPresent";
            panel_TotalPresent.Size = new Size(430, 113);
            panel_TotalPresent.TabIndex = 1;
            // 
            // label_TotalPresent
            // 
            label_TotalPresent.AutoSize = true;
            label_TotalPresent.Font = new Font("Segoe UI", 10F);
            label_TotalPresent.Location = new Point(12, 63);
            label_TotalPresent.Name = "label_TotalPresent";
            label_TotalPresent.Size = new Size(104, 37);
            label_TotalPresent.TabIndex = 3;
            label_TotalPresent.Text = "Present";
            // 
            // label_NumberPresent
            // 
            label_NumberPresent.AutoSize = true;
            label_NumberPresent.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label_NumberPresent.Location = new Point(12, 9);
            label_NumberPresent.Name = "label_NumberPresent";
            label_NumberPresent.Size = new Size(46, 54);
            label_NumberPresent.TabIndex = 2;
            label_NumberPresent.Text = "0";
            // 
            // panel_TotalEmployees
            // 
            panel_TotalEmployees.BackColor = Color.FromArgb(229, 238, 253);
            panel_TotalEmployees.Controls.Add(label_TotalEmployees);
            panel_TotalEmployees.Controls.Add(label_NumberEmployees);
            panel_TotalEmployees.Font = new Font("Segoe UI", 13F);
            panel_TotalEmployees.Location = new Point(14, 13);
            panel_TotalEmployees.Name = "panel_TotalEmployees";
            panel_TotalEmployees.Size = new Size(430, 113);
            panel_TotalEmployees.TabIndex = 0;
            // 
            // label_TotalEmployees
            // 
            label_TotalEmployees.AutoSize = true;
            label_TotalEmployees.Font = new Font("Segoe UI", 10F);
            label_TotalEmployees.Location = new Point(13, 63);
            label_TotalEmployees.Name = "label_TotalEmployees";
            label_TotalEmployees.Size = new Size(205, 37);
            label_TotalEmployees.TabIndex = 1;
            label_TotalEmployees.Text = "Total Employees";
            // 
            // label_NumberEmployees
            // 
            label_NumberEmployees.AutoSize = true;
            label_NumberEmployees.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label_NumberEmployees.Location = new Point(13, 9);
            label_NumberEmployees.Name = "label_NumberEmployees";
            label_NumberEmployees.Size = new Size(46, 54);
            label_NumberEmployees.TabIndex = 0;
            label_NumberEmployees.Text = "0";
            // 
            // panel_Filter
            // 
            panel_Filter.Controls.Add(button_Search);
            panel_Filter.Controls.Add(textBox_Search);
            panel_Filter.Controls.Add(comboBox_Status);
            panel_Filter.Controls.Add(comboBox_Department);
            panel_Filter.Controls.Add(dateTimePicker1);
            panel_Filter.Location = new Point(5, 155);
            panel_Filter.Name = "panel_Filter";
            panel_Filter.Size = new Size(1808, 76);
            panel_Filter.TabIndex = 12;
            // 
            // button_Search
            // 
            button_Search.Location = new Point(1647, 16);
            button_Search.Name = "button_Search";
            button_Search.Size = new Size(150, 46);
            button_Search.TabIndex = 4;
            button_Search.Text = "Search";
            button_Search.UseVisualStyleBackColor = true;
            // 
            // textBox_Search
            // 
            textBox_Search.Location = new Point(964, 20);
            textBox_Search.Name = "textBox_Search";
            textBox_Search.PlaceholderText = "Search by name or code...";
            textBox_Search.Size = new Size(662, 39);
            textBox_Search.TabIndex = 3;
            // 
            // comboBox_Status
            // 
            comboBox_Status.FormattingEnabled = true;
            comboBox_Status.Location = new Point(695, 20);
            comboBox_Status.Name = "comboBox_Status";
            comboBox_Status.Size = new Size(242, 40);
            comboBox_Status.TabIndex = 2;
            // 
            // comboBox_Department
            // 
            comboBox_Department.FormattingEnabled = true;
            comboBox_Department.Location = new Point(427, 20);
            comboBox_Department.Name = "comboBox_Department";
            comboBox_Department.Size = new Size(242, 40);
            comboBox_Department.TabIndex = 1;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(11, 21);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(400, 39);
            dateTimePicker1.TabIndex = 0;
            // 
            // panel_List
            // 
            panel_List.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel_List.Controls.Add(button_Print);
            panel_List.Controls.Add(button_ExportExcel);
            panel_List.Controls.Add(label_List);
            panel_List.Controls.Add(listView_Attendance);
            panel_List.Location = new Point(5, 237);
            panel_List.Name = "panel_List";
            panel_List.Size = new Size(1808, 640);
            panel_List.TabIndex = 11;
            // 
            // button_Print
            // 
            button_Print.Location = new Point(1697, 5);
            button_Print.Name = "button_Print";
            button_Print.Size = new Size(105, 46);
            button_Print.TabIndex = 3;
            button_Print.Text = "Print";
            button_Print.UseVisualStyleBackColor = true;
            // 
            // button_ExportExcel
            // 
            button_ExportExcel.Location = new Point(1537, 5);
            button_ExportExcel.Name = "button_ExportExcel";
            button_ExportExcel.Size = new Size(150, 46);
            button_ExportExcel.TabIndex = 2;
            button_ExportExcel.Text = "Export Excel";
            button_ExportExcel.UseVisualStyleBackColor = true;
            // 
            // label_List
            // 
            label_List.AutoSize = true;
            label_List.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label_List.Location = new Point(11, 12);
            label_List.Name = "label_List";
            label_List.Size = new Size(226, 32);
            label_List.TabIndex = 1;
            label_List.Text = "Attendance Records";
            // 
            // listView_Attendance
            // 
            listView_Attendance.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView_Attendance.BackColor = Color.White;
            listView_Attendance.BorderStyle = BorderStyle.FixedSingle;
            listView_Attendance.Columns.AddRange(new ColumnHeader[] { col_EmpId, col_EmpName, col_Date, col_Shift, col_TimeIn, col_TimeOut, col_WorkHour, col_Status, col_LateMins, col_EarlyLeaveMins, col_OTMins, col_CheckInLocation, col_DeviceId, col_Note });
            listView_Attendance.Font = new Font("Segoe UI", 9F);
            listView_Attendance.ForeColor = Color.FromArgb(37, 48, 74);
            listView_Attendance.FullRowSelect = true;
            listView_Attendance.GridLines = true;
            listView_Attendance.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView_Attendance.Location = new Point(6, 54);
            listView_Attendance.Name = "listView_Attendance";
            listView_Attendance.Size = new Size(1796, 580);
            listView_Attendance.TabIndex = 0;
            listView_Attendance.UseCompatibleStateImageBehavior = false;
            listView_Attendance.View = View.Details;
            // 
            // col_EmpId
            // 
            col_EmpId.Text = "Mã NV";
            col_EmpId.Width = 110;
            // 
            // col_EmpName
            // 
            col_EmpName.Text = "Họ và tên";
            col_EmpName.Width = 180;
            // 
            // col_Date
            // 
            col_Date.Text = "Ngày";
            col_Date.Width = 110;
            // 
            // col_Shift
            // 
            col_Shift.Text = "Ca làm việc";
            col_Shift.Width = 120;
            // 
            // col_TimeIn
            // 
            col_TimeIn.Text = "Giờ vào";
            col_TimeIn.Width = 95;
            // 
            // col_TimeOut
            // 
            col_TimeOut.Text = "Giờ ra";
            col_TimeOut.Width = 95;
            // 
            // col_WorkHour
            // 
            col_WorkHour.Text = "Giờ làm";
            col_WorkHour.Width = 90;
            // 
            // col_Status
            // 
            col_Status.Text = "Trạng thái";
            col_Status.Width = 120;
            // 
            // col_LateMins
            // 
            col_LateMins.Text = "Đi muộn (phút)";
            col_LateMins.Width = 120;
            // 
            // col_EarlyLeaveMins
            // 
            col_EarlyLeaveMins.Text = "Về sớm (phút)";
            col_EarlyLeaveMins.Width = 120;
            // 
            // col_OTMins
            // 
            col_OTMins.Text = "Tăng ca (phút)";
            col_OTMins.Width = 120;
            // 
            // col_CheckInLocation
            // 
            col_CheckInLocation.Text = "Vị trí Check-in";
            col_CheckInLocation.Width = 160;
            // 
            // col_DeviceId
            // 
            col_DeviceId.Text = "Thiết bị";
            col_DeviceId.Width = 110;
            // 
            // col_Note
            // 
            col_Note.Text = "Ghi chú";
            col_Note.Width = 180;
            // 
            // AttendanceOverviewPageControl
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel_Statistics);
            Controls.Add(panel_Filter);
            Controls.Add(panel_List);
            Name = "AttendanceOverviewPageControl";
            Size = new Size(1815, 884);
            panel_Statistics.ResumeLayout(false);
            panel_TotalAbsent.ResumeLayout(false);
            panel_TotalAbsent.PerformLayout();
            panel_TotalLate.ResumeLayout(false);
            panel_TotalLate.PerformLayout();
            panel_TotalPresent.ResumeLayout(false);
            panel_TotalPresent.PerformLayout();
            panel_TotalEmployees.ResumeLayout(false);
            panel_TotalEmployees.PerformLayout();
            panel_Filter.ResumeLayout(false);
            panel_Filter.PerformLayout();
            panel_List.ResumeLayout(false);
            panel_List.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Statistics;
        private Panel panel_TotalAbsent;
        private Label label_TotalAbsent;
        private Label label_NumberAbsent;
        private Panel panel_TotalLate;
        private Label label_TotalLate;
        private Label label_NumberLate;
        private Panel panel_TotalPresent;
        private Label label_TotalPresent;
        private Label label_NumberPresent;
        private Panel panel_TotalEmployees;
        private Label label_TotalEmployees;
        private Label label_NumberEmployees;
        private Panel panel_Filter;
        private Button button_Search;
        private TextBox textBox_Search;
        private ComboBox comboBox_Status;
        private ComboBox comboBox_Department;
        private DateTimePicker dateTimePicker1;
        private Panel panel_List;
        private Button button_Print;
        private Button button_ExportExcel;
        private Label label_List;
        private ListView listView_Attendance;
        private ColumnHeader col_EmpId;
        private ColumnHeader col_EmpName;
        private ColumnHeader col_Date;
        private ColumnHeader col_Shift;
        private ColumnHeader col_TimeIn;
        private ColumnHeader col_TimeOut;
        private ColumnHeader col_WorkHour;
        private ColumnHeader col_Status;
        private ColumnHeader col_LateMins;
        private ColumnHeader col_EarlyLeaveMins;
        private ColumnHeader col_OTMins;
        private ColumnHeader col_CheckInLocation;
        private ColumnHeader col_DeviceId;
        private ColumnHeader col_Note;
    }
}
