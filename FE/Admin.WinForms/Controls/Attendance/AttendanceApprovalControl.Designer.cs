namespace Admin.WinForms.Controls.Attendance
{
    partial class AttendanceApprovalControl
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
            panel_List = new Panel();
            comboBox_TypeFilter = new ComboBox();
            button_Approve = new Button();
            button_Reject = new Button();
            button_Reload = new Button();
            button_Filter = new Button();
            label_List = new Label();
            listView_Approvals = new ListView();
            col_Appr_EmpId = new ColumnHeader();
            col_Appr_Date = new ColumnHeader();
            col_Appr_Type = new ColumnHeader();
            col_Appr_Reason = new ColumnHeader();
            col_Appr_ReqIn = new ColumnHeader();
            col_Appr_ReqOut = new ColumnHeader();
            col_Appr_Status = new ColumnHeader();
            col_Appr_ApprovedBy = new ColumnHeader();
            col_Appr_ApprovedAt = new ColumnHeader();
            col_Appr_Attachment = new ColumnHeader();
            panel_List.SuspendLayout();
            SuspendLayout();
            // 
            // panel_List
            // 
            panel_List.Controls.Add(comboBox_TypeFilter);
            panel_List.Controls.Add(button_Approve);
            panel_List.Controls.Add(button_Reject);
            panel_List.Controls.Add(button_Reload);
            panel_List.Controls.Add(button_Filter);
            panel_List.Controls.Add(label_List);
            panel_List.Controls.Add(listView_Approvals);
            panel_List.Dock = DockStyle.Fill;
            panel_List.Location = new Point(0, 0);
            panel_List.Name = "panel_List";
            panel_List.Size = new Size(1815, 884);
            panel_List.TabIndex = 11;
            // 
            // comboBox_TypeFilter
            // 
            comboBox_TypeFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_TypeFilter.Font = new Font("Segoe UI", 10F);
            comboBox_TypeFilter.FormattingEnabled = true;
            comboBox_TypeFilter.Location = new Point(240, 9);
            comboBox_TypeFilter.Name = "comboBox_TypeFilter";
            comboBox_TypeFilter.Size = new Size(240, 36);
            comboBox_TypeFilter.TabIndex = 6;
            // 
            // button_Approve
            // 
            button_Approve.BackColor = Color.FromArgb(40, 167, 69);
            button_Approve.FlatStyle = FlatStyle.Flat;
            button_Approve.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_Approve.ForeColor = Color.White;
            button_Approve.Location = new Point(1150, 5);
            button_Approve.Name = "button_Approve";
            button_Approve.Size = new Size(130, 46);
            button_Approve.TabIndex = 4;
            button_Approve.Text = "✓ Duyệt";
            button_Approve.UseVisualStyleBackColor = false;
            // 
            // button_Reject
            // 
            button_Reject.BackColor = Color.FromArgb(220, 53, 69);
            button_Reject.FlatStyle = FlatStyle.Flat;
            button_Reject.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button_Reject.ForeColor = Color.White;
            button_Reject.Location = new Point(1290, 5);
            button_Reject.Name = "button_Reject";
            button_Reject.Size = new Size(130, 46);
            button_Reject.TabIndex = 5;
            button_Reject.Text = "✗ Từ chối";
            button_Reject.UseVisualStyleBackColor = false;
            // 
            // button_Reload
            // 
            button_Reload.Location = new Point(1697, 5);
            button_Reload.Name = "button_Reload";
            button_Reload.Size = new Size(105, 46);
            button_Reload.TabIndex = 3;
            button_Reload.Text = "Reload";
            button_Reload.UseVisualStyleBackColor = true;
            // 
            // button_Filter
            // 
            button_Filter.Location = new Point(1537, 5);
            button_Filter.Name = "button_Filter";
            button_Filter.Size = new Size(150, 46);
            button_Filter.TabIndex = 2;
            button_Filter.Text = "Filter";
            button_Filter.UseVisualStyleBackColor = true;
            // 
            // label_List
            // 
            label_List.AutoSize = true;
            label_List.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label_List.Location = new Point(11, 12);
            label_List.Name = "label_List";
            label_List.Size = new Size(213, 30);
            label_List.TabIndex = 1;
            label_List.Text = "Pending Approvals";
            // 
            // listView_Approvals
            // 
            listView_Approvals.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView_Approvals.Columns.AddRange(new ColumnHeader[] { col_Appr_EmpId, col_Appr_Date, col_Appr_Type, col_Appr_Reason, col_Appr_ReqIn, col_Appr_ReqOut, col_Appr_Status, col_Appr_ApprovedBy, col_Appr_ApprovedAt, col_Appr_Attachment });
            listView_Approvals.BackColor = Color.White;
            listView_Approvals.BorderStyle = BorderStyle.FixedSingle;
            listView_Approvals.Font = new Font("Segoe UI", 9F);
            listView_Approvals.ForeColor = Color.FromArgb(37, 48, 74);
            listView_Approvals.FullRowSelect = true;
            listView_Approvals.GridLines = true;
            listView_Approvals.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView_Approvals.HideSelection = false;
            listView_Approvals.Location = new Point(6, 54);
            listView_Approvals.Name = "listView_Approvals";
            listView_Approvals.Size = new Size(1800, 815);
            listView_Approvals.TabIndex = 0;
            listView_Approvals.UseCompatibleStateImageBehavior = false;
            listView_Approvals.View = View.Details;
            // 
            // col_Appr_EmpId
            // 
            col_Appr_EmpId.Text = "Mã NV";
            col_Appr_EmpId.Width = 110;
            // 
            // col_Appr_Date
            // 
            col_Appr_Date.Text = "Ngày";
            col_Appr_Date.Width = 110;
            // 
            // col_Appr_Type
            // 
            col_Appr_Type.Text = "Loại đơn / Yêu cầu";
            col_Appr_Type.Width = 170;
            // 
            // col_Appr_Reason
            // 
            col_Appr_Reason.Text = "Lý do / Nội dung";
            col_Appr_Reason.Width = 220;
            // 
            // col_Appr_ReqIn
            // 
            col_Appr_ReqIn.Text = "Giờ bắt đầu";
            col_Appr_ReqIn.Width = 180;
            // 
            // col_Appr_ReqOut
            // 
            col_Appr_ReqOut.Text = "Giờ kết thúc";
            col_Appr_ReqOut.Width = 180;
            // 
            // col_Appr_Status
            // 
            col_Appr_Status.Text = "Trạng thái";
            col_Appr_Status.Width = 130;
            // 
            // col_Appr_ApprovedBy
            // 
            col_Appr_ApprovedBy.Text = "Người duyệt";
            col_Appr_ApprovedBy.Width = 150;
            // 
            // col_Appr_ApprovedAt
            // 
            col_Appr_ApprovedAt.Text = "Ngày duyệt";
            col_Appr_ApprovedAt.Width = 150;
            // 
            // col_Appr_Attachment
            // 
            col_Appr_Attachment.Text = "Minh chứng / Ghi chú";
            col_Appr_Attachment.Width = 170;
            // 
            // AttendanceApprovalControl
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel_List);
            Name = "AttendanceApprovalControl";
            Size = new Size(1815, 884);
            panel_List.ResumeLayout(false);
            panel_List.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel_List;
        private Button button_Reload;
        private Button button_Filter;
        private Button button_Approve;
        private Button button_Reject;
        private ComboBox comboBox_TypeFilter;
        private Label label_List;
        private ListView listView_Approvals;
        private ColumnHeader col_Appr_EmpId;
        private ColumnHeader col_Appr_Date;
        private ColumnHeader col_Appr_Type;
        private ColumnHeader col_Appr_Reason;
        private ColumnHeader col_Appr_ReqIn;
        private ColumnHeader col_Appr_ReqOut;
        private ColumnHeader col_Appr_Status;
        private ColumnHeader col_Appr_ApprovedBy;
        private ColumnHeader col_Appr_ApprovedAt;
        private ColumnHeader col_Appr_Attachment;
    }
}
