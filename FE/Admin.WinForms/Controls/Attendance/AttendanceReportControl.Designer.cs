namespace Admin.WinForms.Controls.Attendance
{
    partial class AttendanceReportControl
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
            tableLayout_Statistics = new TableLayoutPanel();
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
            label_TotalRecords = new Label();
            label_NumberRecords = new Label();
            panel_Filter = new Panel();
            button_ExportPDF = new Button();
            button_Generate = new Button();
            comboBox_Status = new ComboBox();
            comboBox_Department = new ComboBox();
            dateTimePicker1 = new DateTimePicker();
            panel_Reports = new Panel();
            tabControl_Reports = new TabControl();
            tabPage_SummaryTable = new TabPage();
            label_ReportList = new Label();
            button_Print = new Button();
            button_ExportExcel = new Button();
            listView_Report = new ListView();
            col_Report_EmpId = new ColumnHeader();
            col_Report_EmpName = new ColumnHeader();
            col_Report_Period = new ColumnHeader();
            col_Report_StandardDays = new ColumnHeader();
            col_Report_ActualDays = new ColumnHeader();
            col_Report_TotalHours = new ColumnHeader();
            col_Report_PaidLeave = new ColumnHeader();
            col_Report_UnpaidLeave = new ColumnHeader();
            col_Report_LateOccurrences = new ColumnHeader();
            col_Report_TotalLateMins = new ColumnHeader();
            col_Report_EarlyLeaveOccurrences = new ColumnHeader();
            col_Report_TotalEarlyLeaveMins = new ColumnHeader();
            col_Report_NormalOT = new ColumnHeader();
            col_Report_WeekendOT = new ColumnHeader();
            col_Report_HolidayOT = new ColumnHeader();
            tabPage_Charts = new TabPage();
            tableLayout_Charts = new TableLayoutPanel();
            panel_TrendByDepartment = new Panel();
            panel_Trend = new Panel();
            panel_Statistics.SuspendLayout();
            tableLayout_Statistics.SuspendLayout();
            panel_TotalAbsent.SuspendLayout();
            panel_TotalLate.SuspendLayout();
            panel_TotalPresent.SuspendLayout();
            panel_TotalEmployees.SuspendLayout();
            panel_Filter.SuspendLayout();
            panel_Reports.SuspendLayout();
            tabControl_Reports.SuspendLayout();
            tabPage_SummaryTable.SuspendLayout();
            tabPage_Charts.SuspendLayout();
            tableLayout_Charts.SuspendLayout();
            SuspendLayout();
            // 
            // panel_Statistics
            // 
            panel_Statistics.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel_Statistics.Controls.Add(tableLayout_Statistics);
            panel_Statistics.Location = new Point(5, 7);
            panel_Statistics.Name = "panel_Statistics";
            panel_Statistics.Size = new Size(1808, 142);
            panel_Statistics.TabIndex = 13;
            // 
            // tableLayout_Statistics
            // 
            tableLayout_Statistics.ColumnCount = 4;
            tableLayout_Statistics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout_Statistics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout_Statistics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout_Statistics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayout_Statistics.Controls.Add(panel_TotalEmployees, 0, 0);
            tableLayout_Statistics.Controls.Add(panel_TotalPresent, 1, 0);
            tableLayout_Statistics.Controls.Add(panel_TotalLate, 2, 0);
            tableLayout_Statistics.Controls.Add(panel_TotalAbsent, 3, 0);
            tableLayout_Statistics.Dock = DockStyle.Fill;
            tableLayout_Statistics.Location = new Point(0, 0);
            tableLayout_Statistics.Name = "tableLayout_Statistics";
            tableLayout_Statistics.RowCount = 1;
            tableLayout_Statistics.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayout_Statistics.Size = new Size(1808, 142);
            tableLayout_Statistics.TabIndex = 0;
            // 
            // panel_TotalAbsent
            // 
            panel_TotalAbsent.BackColor = Color.FromArgb(253, 237, 240);
            panel_TotalAbsent.Controls.Add(label_TotalAbsent);
            panel_TotalAbsent.Controls.Add(label_NumberAbsent);
            panel_TotalAbsent.Dock = DockStyle.Fill;
            panel_TotalAbsent.Location = new Point(1359, 3);
            panel_TotalAbsent.Margin = new Padding(6);
            panel_TotalAbsent.Name = "panel_TotalAbsent";
            panel_TotalAbsent.Size = new Size(443, 136);
            panel_TotalAbsent.TabIndex = 3;
            // 
            // label_TotalAbsent
            // 
            label_TotalAbsent.AutoSize = true;
            label_TotalAbsent.Font = new Font("Segoe UI", 10F);
            label_TotalAbsent.Location = new Point(16, 68);
            label_TotalAbsent.Name = "label_TotalAbsent";
            label_TotalAbsent.Size = new Size(99, 37);
            label_TotalAbsent.TabIndex = 7;
            label_TotalAbsent.Text = "Absent";
            // 
            // label_NumberAbsent
            // 
            label_NumberAbsent.AutoSize = true;
            label_NumberAbsent.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label_NumberAbsent.Location = new Point(16, 12);
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
            panel_TotalLate.Dock = DockStyle.Fill;
            panel_TotalLate.Location = new Point(908, 3);
            panel_TotalLate.Margin = new Padding(6);
            panel_TotalLate.Name = "panel_TotalLate";
            panel_TotalLate.Size = new Size(439, 136);
            panel_TotalLate.TabIndex = 2;
            // 
            // label_TotalLate
            // 
            label_TotalLate.AutoSize = true;
            label_TotalLate.Font = new Font("Segoe UI", 10F);
            label_TotalLate.Location = new Point(16, 68);
            label_TotalLate.Name = "label_TotalLate";
            label_TotalLate.Size = new Size(67, 37);
            label_TotalLate.TabIndex = 5;
            label_TotalLate.Text = "Late";
            // 
            // label_NumberLate
            // 
            label_NumberLate.AutoSize = true;
            label_NumberLate.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label_NumberLate.Location = new Point(16, 12);
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
            panel_TotalPresent.Dock = DockStyle.Fill;
            panel_TotalPresent.Location = new Point(457, 3);
            panel_TotalPresent.Margin = new Padding(6);
            panel_TotalPresent.Name = "panel_TotalPresent";
            panel_TotalPresent.Size = new Size(439, 136);
            panel_TotalPresent.TabIndex = 1;
            // 
            // label_TotalPresent
            // 
            label_TotalPresent.AutoSize = true;
            label_TotalPresent.Font = new Font("Segoe UI", 10F);
            label_TotalPresent.Location = new Point(16, 68);
            label_TotalPresent.Name = "label_TotalPresent";
            label_TotalPresent.Size = new Size(104, 37);
            label_TotalPresent.TabIndex = 3;
            label_TotalPresent.Text = "Present";
            // 
            // label_NumberPresent
            // 
            label_NumberPresent.AutoSize = true;
            label_NumberPresent.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label_NumberPresent.Location = new Point(16, 12);
            label_NumberPresent.Name = "label_NumberPresent";
            label_NumberPresent.Size = new Size(46, 54);
            label_NumberPresent.TabIndex = 2;
            label_NumberPresent.Text = "0";
            // 
            // panel_TotalEmployees
            // 
            panel_TotalEmployees.BackColor = Color.FromArgb(229, 238, 253);
            panel_TotalEmployees.Controls.Add(label_TotalRecords);
            panel_TotalEmployees.Controls.Add(label_NumberRecords);
            panel_TotalEmployees.Dock = DockStyle.Fill;
            panel_TotalEmployees.Font = new Font("Segoe UI", 13F);
            panel_TotalEmployees.Location = new Point(6, 3);
            panel_TotalEmployees.Margin = new Padding(6);
            panel_TotalEmployees.Name = "panel_TotalEmployees";
            panel_TotalEmployees.Size = new Size(439, 136);
            panel_TotalEmployees.TabIndex = 0;
            // 
            // label_TotalRecords
            // 
            label_TotalRecords.AutoSize = true;
            label_TotalRecords.Font = new Font("Segoe UI", 10F);
            label_TotalRecords.Location = new Point(16, 68);
            label_TotalRecords.Name = "label_TotalRecords";
            label_TotalRecords.Size = new Size(174, 37);
            label_TotalRecords.TabIndex = 1;
            label_TotalRecords.Text = "Total Records";
            // 
            // label_NumberRecords
            // 
            label_NumberRecords.AutoSize = true;
            label_NumberRecords.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label_NumberRecords.Location = new Point(16, 12);
            label_NumberRecords.Name = "label_NumberRecords";
            label_NumberRecords.Size = new Size(46, 54);
            label_NumberRecords.TabIndex = 0;
            label_NumberRecords.Text = "0";
            // 
            // panel_Filter
            // 
            panel_Filter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel_Filter.Controls.Add(button_ExportPDF);
            panel_Filter.Controls.Add(button_Generate);
            panel_Filter.Controls.Add(comboBox_Status);
            panel_Filter.Controls.Add(comboBox_Department);
            panel_Filter.Controls.Add(dateTimePicker1);
            panel_Filter.Location = new Point(5, 155);
            panel_Filter.Name = "panel_Filter";
            panel_Filter.Size = new Size(1808, 76);
            panel_Filter.TabIndex = 12;
            // 
            // button_ExportPDF
            // 
            button_ExportPDF.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_ExportPDF.Location = new Point(1647, 16);
            button_ExportPDF.Name = "button_ExportPDF";
            button_ExportPDF.Size = new Size(150, 46);
            button_ExportPDF.TabIndex = 4;
            button_ExportPDF.Text = "Export PDF";
            button_ExportPDF.UseVisualStyleBackColor = true;
            // 
            // button_Generate
            // 
            button_Generate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Generate.Location = new Point(1432, 16);
            button_Generate.Name = "button_Generate";
            button_Generate.Size = new Size(205, 46);
            button_Generate.TabIndex = 3;
            button_Generate.Text = "Generate Report";
            button_Generate.UseVisualStyleBackColor = true;
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
            // panel_Reports
            // 
            panel_Reports.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel_Reports.Controls.Add(tabControl_Reports);
            panel_Reports.Location = new Point(5, 237);
            panel_Reports.Name = "panel_Reports";
            panel_Reports.Size = new Size(1808, 640);
            panel_Reports.TabIndex = 11;
            // 
            // tabControl_Reports
            // 
            tabControl_Reports.Controls.Add(tabPage_SummaryTable);
            tabControl_Reports.Controls.Add(tabPage_Charts);
            tabControl_Reports.Dock = DockStyle.Fill;
            tabControl_Reports.Location = new Point(0, 0);
            tabControl_Reports.Name = "tabControl_Reports";
            tabControl_Reports.SelectedIndex = 0;
            tabControl_Reports.Size = new Size(1808, 640);
            tabControl_Reports.TabIndex = 0;
            // 
            // tabPage_SummaryTable
            // 
            tabPage_SummaryTable.Controls.Add(label_ReportList);
            tabPage_SummaryTable.Controls.Add(button_Print);
            tabPage_SummaryTable.Controls.Add(button_ExportExcel);
            tabPage_SummaryTable.Controls.Add(listView_Report);
            tabPage_SummaryTable.Location = new Point(8, 46);
            tabPage_SummaryTable.Name = "tabPage_SummaryTable";
            tabPage_SummaryTable.Padding = new Padding(3);
            tabPage_SummaryTable.Size = new Size(1792, 586);
            tabPage_SummaryTable.TabIndex = 0;
            tabPage_SummaryTable.Text = "Bảng dữ liệu báo cáo (Table)";
            tabPage_SummaryTable.UseVisualStyleBackColor = true;
            // 
            // label_ReportList
            // 
            label_ReportList.AutoSize = true;
            label_ReportList.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label_ReportList.Location = new Point(6, 12);
            label_ReportList.Name = "label_ReportList";
            label_ReportList.Size = new Size(432, 40);
            label_ReportList.TabIndex = 1;
            label_ReportList.Text = "Monthly Timesheet Summary";
            // 
            // button_Print
            // 
            button_Print.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_Print.Location = new Point(1681, 8);
            button_Print.Name = "button_Print";
            button_Print.Size = new Size(105, 46);
            button_Print.TabIndex = 3;
            button_Print.Text = "Print";
            button_Print.UseVisualStyleBackColor = true;
            // 
            // button_ExportExcel
            // 
            button_ExportExcel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_ExportExcel.Location = new Point(1521, 8);
            button_ExportExcel.Name = "button_ExportExcel";
            button_ExportExcel.Size = new Size(150, 46);
            button_ExportExcel.TabIndex = 2;
            button_ExportExcel.Text = "Export Excel";
            button_ExportExcel.UseVisualStyleBackColor = true;
            // 
            // col_Report_EmpId
            // 
            col_Report_EmpId.Text = "Mã NV";
            col_Report_EmpId.Width = 110;
            // 
            // col_Report_EmpName
            // 
            col_Report_EmpName.Text = "Họ và tên";
            col_Report_EmpName.Width = 180;
            // 
            // col_Report_Period
            // 
            col_Report_Period.Text = "Kỳ công";
            col_Report_Period.Width = 120;
            // 
            // col_Report_StandardDays
            // 
            col_Report_StandardDays.Text = "Công chuẩn";
            col_Report_StandardDays.Width = 100;
            // 
            // col_Report_ActualDays
            // 
            col_Report_ActualDays.Text = "Công thực tế";
            col_Report_ActualDays.Width = 105;
            // 
            // col_Report_TotalHours
            // 
            col_Report_TotalHours.Text = "Tổng giờ làm";
            col_Report_TotalHours.Width = 110;
            // 
            // col_Report_PaidLeave
            // 
            col_Report_PaidLeave.Text = "Phép hưởng lương";
            col_Report_PaidLeave.Width = 135;
            // 
            // col_Report_UnpaidLeave
            // 
            col_Report_UnpaidLeave.Text = "Nghỉ không lương";
            col_Report_UnpaidLeave.Width = 135;
            // 
            // col_Report_LateOccurrences
            // 
            col_Report_LateOccurrences.Text = "Số lần trễ";
            col_Report_LateOccurrences.Width = 95;
            // 
            // col_Report_TotalLateMins
            // 
            col_Report_TotalLateMins.Text = "Phút trễ";
            col_Report_TotalLateMins.Width = 95;
            // 
            // col_Report_EarlyLeaveOccurrences
            // 
            col_Report_EarlyLeaveOccurrences.Text = "Số lần về sớm";
            col_Report_EarlyLeaveOccurrences.Width = 110;
            // 
            // col_Report_TotalEarlyLeaveMins
            // 
            col_Report_TotalEarlyLeaveMins.Text = "Phút về sớm";
            col_Report_TotalEarlyLeaveMins.Width = 110;
            // 
            // col_Report_NormalOT
            // 
            col_Report_NormalOT.Text = "OT Thường (h)";
            col_Report_NormalOT.Width = 125;
            // 
            // col_Report_WeekendOT
            // 
            col_Report_WeekendOT.Text = "OT Cuối tuần (h)";
            col_Report_WeekendOT.Width = 125;
            // 
            // col_Report_HolidayOT
            // 
            col_Report_HolidayOT.Text = "OT Ngày lễ (h)";
            col_Report_HolidayOT.Width = 125;
            // 
            // listView_Report
            // 
            listView_Report.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView_Report.Columns.AddRange(new ColumnHeader[] {
                col_Report_EmpId,
                col_Report_EmpName,
                col_Report_Period,
                col_Report_StandardDays,
                col_Report_ActualDays,
                col_Report_TotalHours,
                col_Report_PaidLeave,
                col_Report_UnpaidLeave,
                col_Report_LateOccurrences,
                col_Report_TotalLateMins,
                col_Report_EarlyLeaveOccurrences,
                col_Report_TotalEarlyLeaveMins,
                col_Report_NormalOT,
                col_Report_WeekendOT,
                col_Report_HolidayOT
            });
            listView_Report.BackColor = Color.White;
            listView_Report.BorderStyle = BorderStyle.FixedSingle;
            listView_Report.Font = new Font("Segoe UI", 9F);
            listView_Report.ForeColor = Color.FromArgb(37, 48, 74);
            listView_Report.FullRowSelect = true;
            listView_Report.GridLines = true;
            listView_Report.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView_Report.HideSelection = false;
            listView_Report.Location = new Point(6, 60);
            listView_Report.Name = "listView_Report";
            listView_Report.Size = new Size(1780, 520);
            listView_Report.TabIndex = 0;
            listView_Report.UseCompatibleStateImageBehavior = false;
            listView_Report.View = View.Details;
            // 
            // tabPage_Charts
            // 
            tabPage_Charts.Controls.Add(tableLayout_Charts);
            tabPage_Charts.Location = new Point(8, 46);
            tabPage_Charts.Name = "tabPage_Charts";
            tabPage_Charts.Padding = new Padding(3);
            tabPage_Charts.Size = new Size(1792, 586);
            tabPage_Charts.TabIndex = 1;
            tabPage_Charts.Text = "Biểu đồ phân tích (Trends)";
            tabPage_Charts.UseVisualStyleBackColor = true;
            // 
            // tableLayout_Charts
            // 
            tableLayout_Charts.ColumnCount = 2;
            tableLayout_Charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout_Charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout_Charts.Controls.Add(panel_Trend, 0, 0);
            tableLayout_Charts.Controls.Add(panel_TrendByDepartment, 1, 0);
            tableLayout_Charts.Dock = DockStyle.Fill;
            tableLayout_Charts.Location = new Point(3, 3);
            tableLayout_Charts.Name = "tableLayout_Charts";
            tableLayout_Charts.RowCount = 1;
            tableLayout_Charts.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayout_Charts.Size = new Size(1786, 580);
            tableLayout_Charts.TabIndex = 0;
            // 
            // panel_Trend
            // 
            panel_Trend.AutoScroll = true;
            panel_Trend.BackColor = Color.White;
            panel_Trend.Dock = DockStyle.Fill;
            panel_Trend.Location = new Point(6, 6);
            panel_Trend.Margin = new Padding(6);
            panel_Trend.Name = "panel_Trend";
            panel_Trend.Size = new Size(881, 568);
            panel_Trend.TabIndex = 2;
            // 
            // panel_TrendByDepartment
            // 
            panel_TrendByDepartment.AutoScroll = true;
            panel_TrendByDepartment.BackColor = Color.White;
            panel_TrendByDepartment.Dock = DockStyle.Fill;
            panel_TrendByDepartment.Location = new Point(899, 6);
            panel_TrendByDepartment.Margin = new Padding(6);
            panel_TrendByDepartment.Name = "panel_TrendByDepartment";
            panel_TrendByDepartment.Size = new Size(881, 568);
            panel_TrendByDepartment.TabIndex = 3;
            // 
            // AttendanceReportControl
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel_Statistics);
            Controls.Add(panel_Filter);
            Controls.Add(panel_Reports);
            Name = "AttendanceReportControl";
            Size = new Size(1815, 884);
            panel_Statistics.ResumeLayout(false);
            tableLayout_Statistics.ResumeLayout(false);
            panel_TotalAbsent.ResumeLayout(false);
            panel_TotalAbsent.PerformLayout();
            panel_TotalLate.ResumeLayout(false);
            panel_TotalLate.PerformLayout();
            panel_TotalPresent.ResumeLayout(false);
            panel_TotalPresent.PerformLayout();
            panel_TotalEmployees.ResumeLayout(false);
            panel_TotalEmployees.PerformLayout();
            panel_Filter.ResumeLayout(false);
            panel_Reports.ResumeLayout(false);
            tabControl_Reports.ResumeLayout(false);
            tabPage_SummaryTable.ResumeLayout(false);
            tabPage_SummaryTable.PerformLayout();
            tabPage_Charts.ResumeLayout(false);
            tableLayout_Charts.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Statistics;
        private TableLayoutPanel tableLayout_Statistics;
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
        private Label label_TotalRecords;
        private Label label_NumberRecords;
        private Panel panel_Filter;
        private Button button_ExportPDF;
        private Button button_Generate;
        private ComboBox comboBox_Status;
        private ComboBox comboBox_Department;
        private DateTimePicker dateTimePicker1;
        private Panel panel_Reports;
        private TabControl tabControl_Reports;
        private TabPage tabPage_SummaryTable;
        private TabPage tabPage_Charts;
        private TableLayoutPanel tableLayout_Charts;
        private Label label_ReportList;
        private Button button_Print;
        private Button button_ExportExcel;
        private ListView listView_Report;
        private ColumnHeader col_Report_EmpId;
        private ColumnHeader col_Report_EmpName;
        private ColumnHeader col_Report_Period;
        private ColumnHeader col_Report_StandardDays;
        private ColumnHeader col_Report_ActualDays;
        private ColumnHeader col_Report_TotalHours;
        private ColumnHeader col_Report_PaidLeave;
        private ColumnHeader col_Report_UnpaidLeave;
        private ColumnHeader col_Report_LateOccurrences;
        private ColumnHeader col_Report_TotalLateMins;
        private ColumnHeader col_Report_EarlyLeaveOccurrences;
        private ColumnHeader col_Report_TotalEarlyLeaveMins;
        private ColumnHeader col_Report_NormalOT;
        private ColumnHeader col_Report_WeekendOT;
        private ColumnHeader col_Report_HolidayOT;
        private Panel panel_TrendByDepartment;
        private Panel panel_Trend;
    }
}
