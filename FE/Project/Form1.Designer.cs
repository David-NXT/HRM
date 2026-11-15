namespace Project
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private GroupBox grpFilter;
        private Label lblSearch;
        private TextBox txtSearch;
        private Label lblStatus;
        private ComboBox cboStatus;
        private Button btnRefresh;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private ListView lvProjects;
        private ColumnHeader colProjectId;
        private ColumnHeader colProjectName;
        private ColumnHeader colStartDate;
        private ColumnHeader colEndDate;
        private ColumnHeader colStatus;
        private ColumnHeader colBudget;
        private ColumnHeader colDescription;
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
            pnlHeader = new Panel();
            lblSubtitle = new Label();
            lblTitle = new Label();
            grpFilter = new GroupBox();
            btnDelete = new Button();
            btnEdit = new Button();
            btnAdd = new Button();
            btnRefresh = new Button();
            cboStatus = new ComboBox();
            lblStatus = new Label();
            txtSearch = new TextBox();
            lblSearch = new Label();
            lvProjects = new ListView();
            colProjectId = new ColumnHeader();
            colProjectName = new ColumnHeader();
            colStartDate = new ColumnHeader();
            colEndDate = new ColumnHeader();
            colStatus = new ColumnHeader();
            colBudget = new ColumnHeader();
            colDescription = new ColumnHeader();
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
            lblTitle.Size = new Size(300, 41);
            lblTitle.Text = "PROJECT MANAGEMENT";
            // lblSubtitle
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = Color.FromArgb(220, 235, 248);
            lblSubtitle.Location = new Point(32, 57);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(277, 19);
            lblSubtitle.Text = "Track your projects and monitor their progress";
            // grpFilter
            grpFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpFilter.Controls.Add(btnDelete);
            grpFilter.Controls.Add(btnEdit);
            grpFilter.Controls.Add(btnAdd);
            grpFilter.Controls.Add(btnRefresh);
            grpFilter.Controls.Add(cboStatus);
            grpFilter.Controls.Add(lblStatus);
            grpFilter.Controls.Add(txtSearch);
            grpFilter.Controls.Add(lblSearch);
            grpFilter.Location = new Point(24, 108);
            grpFilter.Name = "grpFilter";
            grpFilter.Size = new Size(1136, 86);
            grpFilter.TabStop = false;
            grpFilter.Text = "Filters and Actions";
            // lblSearch
            lblSearch.AutoSize = true;
            lblSearch.Location = new Point(20, 38);
            lblSearch.Text = "Search:";
            // txtSearch
            txtSearch.Location = new Point(91, 34);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Project name, description, or ID";
            txtSearch.Size = new Size(258, 23);
            // lblStatus
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(370, 38);
            lblStatus.Text = "Status:";
            // cboStatus
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.Items.AddRange(new object[] { "All", "Planning", "InProgress", "OnHold", "Completed", "Cancelled" });
            cboStatus.Location = new Point(443, 34);
            cboStatus.Name = "cboStatus";
            cboStatus.Size = new Size(145, 23);
            cboStatus.SelectedIndex = 0;
            // btnRefresh
            btnRefresh.BackColor = Color.FromArgb(108, 117, 125);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(608, 32);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(95, 28);
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // btnAdd
            btnAdd.BackColor = Color.FromArgb(25, 135, 84);
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(721, 32);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(105, 28);
            btnAdd.Text = "Add Project";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAdd_Click;
            // btnEdit
            btnEdit.BackColor = Color.FromArgb(13, 110, 253);
            btnEdit.Enabled = false;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.ForeColor = Color.White;
            btnEdit.Location = new Point(844, 32);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(105, 28);
            btnEdit.Text = "Edit Project";
            btnEdit.UseVisualStyleBackColor = false;
            btnEdit.Click += btnEdit_Click;
            // btnDelete
            btnDelete.BackColor = Color.FromArgb(220, 53, 69);
            btnDelete.Enabled = false;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.ForeColor = Color.White;
            btnDelete.Location = new Point(967, 32);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(105, 28);
            btnDelete.Text = "Delete Project";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // lvProjects
            lvProjects.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvProjects.Columns.AddRange(new ColumnHeader[] { colProjectId, colProjectName, colStartDate, colEndDate, colStatus, colBudget, colDescription });
            lvProjects.FullRowSelect = true;
            lvProjects.GridLines = true;
            lvProjects.HideSelection = false;
            lvProjects.Location = new Point(24, 214);
            lvProjects.Name = "lvProjects";
            lvProjects.Size = new Size(1136, 390);
            lvProjects.View = View.Details;
            lvProjects.SelectedIndexChanged += lvProjects_SelectedIndexChanged;
            // columns
            colProjectId.Text = "Project ID";
            colProjectId.Width = 100;
            colProjectName.Text = "Project Name";
            colProjectName.Width = 190;
            colStartDate.Text = "Start Date";
            colStartDate.Width = 110;
            colEndDate.Text = "End Date";
            colEndDate.Width = 115;
            colStatus.Text = "Status";
            colStatus.Width = 120;
            colBudget.Text = "Budget";
            colBudget.Width = 130;
            colDescription.Text = "Description";
            colDescription.Width = 340;
            // lblTotal
            lblTotal.AutoSize = true;
            lblTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblTotal.Location = new Point(24, 620);
            lblTotal.Text = "Showing: 0 / 0 projects";
            // Form1
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1184, 661);
            Controls.Add(lblTotal);
            Controls.Add(lvProjects);
            Controls.Add(grpFilter);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(1000, 550);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Project Management";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            grpFilter.ResumeLayout(false);
            grpFilter.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
