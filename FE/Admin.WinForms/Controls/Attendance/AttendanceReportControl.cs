
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Admin.WinForms.Controls.Attendance
{
    public partial class AttendanceReportControl : UserControl
    {
        private readonly Color primaryColor = Color.FromArgb(59, 130, 246);
        private readonly Color secondaryColor = Color.FromArgb(220, 53, 69);
        private readonly Color textColor = Color.FromArgb(55, 65, 81);

        public AttendanceReportControl()
        {
            InitializeComponent();

            GenerateAttendanceTrend();
            GenerateDepartmentTrend();
        }

        // ==========================================
        // 1. ATTENDANCE TREND
        // ==========================================


        private void GenerateAttendanceTrend()
        {
            var weeks = new List<(string Name, int Present, int Absent)>
    {
        ("Week 1", 90, 10),
        ("Week 2", 95, 5),
        ("Week 3", 92, 8)
    };

            int startY = 70;

            foreach (var week in weeks)
            {
                // Week label
                Label labelWeek = new Label
                {
                    Text = week.Name,
                    Location = new Point(20, startY),
                    Size = new Size(100, 25),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = textColor
                };

                // Percentage label
                Label labelPercentage = new Label
                {
                    Text = $"{week.Present}% Present",
                    Location = new Point(460, startY),
                    Size = new Size(150, 25),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = primaryColor,
                    TextAlign = ContentAlignment.MiddleRight
                };

                // Background bar (Absent - red)
                Panel panelBar = new Panel
                {
                    Location = new Point(20, startY + 35),
                    Size = new Size(590, 18),
                    BackColor = Color.FromArgb(220, 53, 69)
                };

                // Present portion (blue)
                Panel panelPresent = new Panel
                {
                    Location = new Point(0, 0),
                    Size = new Size(
                        panelBar.Width * week.Present / 100,
                        panelBar.Height
                    ),
                    BackColor = Color.FromArgb(59, 130, 246)
                };

                panelBar.Controls.Add(panelPresent);

                // Add controls
                panel_Trend.Controls.Add(labelWeek);
                panel_Trend.Controls.Add(labelPercentage);
                panel_Trend.Controls.Add(panelBar);

                startY += 110;
            }
        }


        // ==========================================
        // 2. ATTENDANCE BY DEPARTMENT
        // ==========================================

        private void GenerateDepartmentTrend()
        {
            var departments = new List<(string Name, int Percentage)>
            {
                ("Engineering", 95),
                ("Marketing", 88),
                ("Sales", 92)
            };

            int startY = 75;

            foreach (var department in departments)
            {
                // Department name
                Label labelDepartment = new Label
                {
                    Text = department.Name,
                    Location = new Point(20, startY),
                    Size = new Size(200, 30),
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = textColor
                };

                // Percentage
                Label labelPercentage = new Label
                {
                    Text = $"{department.Percentage}%",
                    Location = new Point(550, startY),
                    Size = new Size(65, 30),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = primaryColor,
                    TextAlign = ContentAlignment.MiddleRight
                };

                // Progress bar
                ProgressBar progressDepartment = new ProgressBar
                {
                    Location = new Point(20, startY + 40),
                    Size = new Size(590, 20),
                    Minimum = 0,
                    Maximum = 100,
                    Value = department.Percentage,
                    Style = ProgressBarStyle.Continuous
                };

                panel_TrendByDepartment.Controls.Add(labelDepartment);
                panel_TrendByDepartment.Controls.Add(labelPercentage);
                panel_TrendByDepartment.Controls.Add(progressDepartment);

                startY += 115;
            }
        }
    }
}
