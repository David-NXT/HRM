namespace Admin.WinForms.GUI.Controls.Attendance
{
    partial class AttendanceRequestsControl
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
            label_Title = new Label();
            label_SubTitle = new Label();

            tabControl_Requests = new TabControl();
            tabPage_Leave = new TabPage();
            tabPage_OT = new TabPage();
            tabPage_Explanation = new TabPage();
            tabPage_History = new TabPage();

            panel_Header.SuspendLayout();
            tabControl_Requests.SuspendLayout();
            SuspendLayout();

            // 
            // panel_Header
            // 
            panel_Header.BackColor = Color.FromArgb(30, 58, 138);
            panel_Header.Controls.Add(label_SubTitle);
            panel_Header.Controls.Add(label_Title);
            panel_Header.Dock = DockStyle.Top;
            panel_Header.Location = new Point(0, 0);
            panel_Header.Name = "panel_Header";
            panel_Header.Size = new Size(1800, 85);
            panel_Header.TabIndex = 0;

            label_Title.AutoSize = true;
            label_Title.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label_Title.ForeColor = Color.White;
            label_Title.Location = new Point(25, 12);
            label_Title.Name = "label_Title";
            label_Title.Size = new Size(620, 41);
            label_Title.Text = "TẠO VÀ QUẢN LÝ ĐƠN TỪ CHẤM CÔNG";

            label_SubTitle.AutoSize = true;
            label_SubTitle.Font = new Font("Segoe UI", 9.5F);
            label_SubTitle.ForeColor = Color.FromArgb(191, 219, 254);
            label_SubTitle.Location = new Point(28, 52);
            label_SubTitle.Name = "label_SubTitle";
            label_SubTitle.Size = new Size(720, 25);
            label_SubTitle.Text = "Nộp đơn xin nghỉ phép (UC-ATT03), đăng ký tăng ca (UC-ATT04), giải trình quẹt thẻ (UC-ATT05)";

            // 
            // tabControl_Requests
            // 
            tabControl_Requests.Controls.Add(tabPage_Leave);
            tabControl_Requests.Controls.Add(tabPage_OT);
            tabControl_Requests.Controls.Add(tabPage_Explanation);
            tabControl_Requests.Controls.Add(tabPage_History);
            tabControl_Requests.Dock = DockStyle.Fill;
            tabControl_Requests.Font = new Font("Segoe UI", 10.5F);
            tabControl_Requests.Location = new Point(0, 85);
            tabControl_Requests.Name = "tabControl_Requests";
            tabControl_Requests.SelectedIndex = 0;
            tabControl_Requests.Size = new Size(1800, 795);
            tabControl_Requests.TabIndex = 1;

            // 
            // tabPage_Leave
            // 
            tabPage_Leave.Location = new Point(4, 39);
            tabPage_Leave.Name = "tabPage_Leave";
            tabPage_Leave.Padding = new Padding(25);
            tabPage_Leave.Size = new Size(1792, 752);
            tabPage_Leave.TabIndex = 0;
            tabPage_Leave.Text = "📝 1. Đơn Xin Nghỉ Phép (Leave)";
            tabPage_Leave.UseVisualStyleBackColor = true;

            // 
            // tabPage_OT
            // 
            tabPage_OT.Location = new Point(4, 39);
            tabPage_OT.Name = "tabPage_OT";
            tabPage_OT.Padding = new Padding(25);
            tabPage_OT.Size = new Size(1792, 752);
            tabPage_OT.TabIndex = 1;
            tabPage_OT.Text = "⏰ 2. Đơn Đăng Ký Làm Thêm Giờ (OT)";
            tabPage_OT.UseVisualStyleBackColor = true;

            // 
            // tabPage_Explanation
            // 
            tabPage_Explanation.Location = new Point(4, 39);
            tabPage_Explanation.Name = "tabPage_Explanation";
            tabPage_Explanation.Padding = new Padding(25);
            tabPage_Explanation.Size = new Size(1792, 752);
            tabPage_Explanation.TabIndex = 2;
            tabPage_Explanation.Text = "✍ 3. Đơn Giải Trình Chấm Công";
            tabPage_Explanation.UseVisualStyleBackColor = true;

            // 
            // tabPage_History
            // 
            tabPage_History.Location = new Point(4, 39);
            tabPage_History.Name = "tabPage_History";
            tabPage_History.Padding = new Padding(20);
            tabPage_History.Size = new Size(1792, 752);
            tabPage_History.TabIndex = 3;
            tabPage_History.Text = "📋 4. Danh Sách Đơn Đã Gửi & Trạng Thái Duyệt";
            tabPage_History.UseVisualStyleBackColor = true;

            // 
            // AttendanceRequestsControl
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl_Requests);
            Controls.Add(panel_Header);
            Name = "AttendanceRequestsControl";
            Size = new Size(1800, 880);

            panel_Header.ResumeLayout(false);
            panel_Header.PerformLayout();
            tabControl_Requests.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_Header;
        private Label label_Title;
        private Label label_SubTitle;
        private TabControl tabControl_Requests;
        private TabPage tabPage_Leave;
        private TabPage tabPage_OT;
        private TabPage tabPage_Explanation;
        private TabPage tabPage_History;
    }
}
