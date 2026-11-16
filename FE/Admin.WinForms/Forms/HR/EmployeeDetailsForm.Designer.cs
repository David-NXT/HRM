#nullable disable
namespace Admin.WinForms.Forms.HR;

public partial class EmployeeDetailsForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel _layout;
    private Label _titleLabel;
    private Label _subtitleLabel;
    private Panel _scrollPanel;
    private TableLayoutPanel _contentLayout;
    private GroupBox _profileGroup;
    private TableLayoutPanel _profileTable;
    private GroupBox _workGroup;
    private TableLayoutPanel _workTable;
    private TableLayoutPanel _footer;
    private Label _hintLabel;
    private Button _closeButton;
    private Label _idLabel;
    private TextBox _idTextBox;
    private Label _codeLabel;
    private TextBox _codeTextBox;
    private Label _statusLabel;
    private TextBox _statusTextBox;
    private Label _nameLabel;
    private TextBox _nameTextBox;
    private Label _birthLabel;
    private TextBox _birthTextBox;
    private Label _genderLabel;
    private TextBox _genderTextBox;
    private Label _citizenLabel;
    private TextBox _citizenTextBox;
    private Label _phoneLabel;
    private TextBox _phoneTextBox;
    private Label _emailLabel;
    private TextBox _emailTextBox;
    private Label _addressLabel;
    private TextBox _addressTextBox;
    private Label _typeLabel;
    private TextBox _typeTextBox;
    private Label _salaryLabel;
    private TextBox _salaryTextBox;
    private Label _departmentLabel;
    private TextBox _departmentTextBox;
    private Label _positionLabel;
    private TextBox _positionTextBox;
    private Label _createdLabel;
    private TextBox _createdTextBox;
    private Label _updatedLabel;
    private TextBox _updatedTextBox;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _layout = new TableLayoutPanel();
        _titleLabel = new Label();
        _subtitleLabel = new Label();
        _scrollPanel = new Panel();
        _contentLayout = new TableLayoutPanel();
        _profileGroup = new GroupBox();
        _profileTable = new TableLayoutPanel();
        _workGroup = new GroupBox();
        _workTable = new TableLayoutPanel();
        _footer = new TableLayoutPanel();
        _hintLabel = new Label();
        _closeButton = new Button();
        _idLabel = new Label();
        _idTextBox = new TextBox();
        _codeLabel = new Label();
        _codeTextBox = new TextBox();
        _statusLabel = new Label();
        _statusTextBox = new TextBox();
        _nameLabel = new Label();
        _nameTextBox = new TextBox();
        _birthLabel = new Label();
        _birthTextBox = new TextBox();
        _genderLabel = new Label();
        _genderTextBox = new TextBox();
        _citizenLabel = new Label();
        _citizenTextBox = new TextBox();
        _phoneLabel = new Label();
        _phoneTextBox = new TextBox();
        _emailLabel = new Label();
        _emailTextBox = new TextBox();
        _addressLabel = new Label();
        _addressTextBox = new TextBox();
        _typeLabel = new Label();
        _typeTextBox = new TextBox();
        _salaryLabel = new Label();
        _salaryTextBox = new TextBox();
        _departmentLabel = new Label();
        _departmentTextBox = new TextBox();
        _positionLabel = new Label();
        _positionTextBox = new TextBox();
        _createdLabel = new Label();
        _createdTextBox = new TextBox();
        _updatedLabel = new Label();
        _updatedTextBox = new TextBox();
        _layout.SuspendLayout();
        _scrollPanel.SuspendLayout();
        _contentLayout.SuspendLayout();
        _profileGroup.SuspendLayout();
        _profileTable.SuspendLayout();
        _workGroup.SuspendLayout();
        _workTable.SuspendLayout();
        _footer.SuspendLayout();
        SuspendLayout();
        //
        // _layout
        //
        _layout.ColumnCount = 1;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_titleLabel, 0, 0);
        _layout.Controls.Add(_subtitleLabel, 0, 1);
        _layout.Controls.Add(_scrollPanel, 0, 2);
        _layout.Controls.Add(_footer, 0, 3);
        _layout.Dock = DockStyle.Fill;
        _layout.Name = "_layout";
        _layout.Padding = new Padding(20);
        _layout.RowCount = 4;
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.Size = new Size(920, 680);
        _layout.TabIndex = 0;
        //
        // _titleLabel
        //
        _titleLabel.AutoSize = true;
        _titleLabel.Dock = DockStyle.Fill;
        _titleLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        _titleLabel.ForeColor = Color.FromArgb(35, 83, 166);
        _titleLabel.Margin = new Padding(0, 0, 0, 4);
        _titleLabel.Name = "_titleLabel";
        _titleLabel.TabIndex = 0;
        _titleLabel.Text = "Chi tiết nhân viên";
        //
        // _subtitleLabel
        //
        _subtitleLabel.AutoSize = true;
        _subtitleLabel.Dock = DockStyle.Fill;
        _subtitleLabel.ForeColor = Color.FromArgb(80, 96, 115);
        _subtitleLabel.Margin = new Padding(0, 0, 0, 14);
        _subtitleLabel.Name = "_subtitleLabel";
        _subtitleLabel.TabIndex = 1;
        _subtitleLabel.Text = "Thông tin hồ sơ, liên hệ, công tác và mức lương";
        //
        // _scrollPanel
        //
        _scrollPanel.AutoScroll = true;
        _scrollPanel.Controls.Add(_contentLayout);
        _scrollPanel.Dock = DockStyle.Fill;
        _scrollPanel.Margin = new Padding(0);
        _scrollPanel.Name = "_scrollPanel";
        _scrollPanel.TabIndex = 2;
        //
        // _contentLayout
        //
        _contentLayout.AutoSize = true;
        _contentLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _contentLayout.ColumnCount = 1;
        _contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _contentLayout.Controls.Add(_profileGroup, 0, 0);
        _contentLayout.Controls.Add(_workGroup, 0, 1);
        _contentLayout.Dock = DockStyle.Top;
        _contentLayout.Name = "_contentLayout";
        _contentLayout.RowCount = 2;
        _contentLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _contentLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _contentLayout.TabIndex = 0;
        //
        // _profileGroup
        //
        _profileGroup.AutoSize = true;
        _profileGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _profileGroup.BackColor = Color.White;
        _profileGroup.Controls.Add(_profileTable);
        _profileGroup.Dock = DockStyle.Top;
        _profileGroup.Margin = new Padding(0, 0, 0, 12);
        _profileGroup.Name = "_profileGroup";
        _profileGroup.Padding = new Padding(12, 24, 12, 12);
        _profileGroup.TabIndex = 0;
        _profileGroup.Text = "Thông tin cá nhân và liên hệ";
        //
        // _profileTable
        //
        _profileTable.AutoSize = true;
        _profileTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _profileTable.ColumnCount = 4;
        _profileTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
        _profileTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _profileTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
        _profileTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _profileTable.Dock = DockStyle.Top;
        _profileTable.Name = "_profileTable";
        _profileTable.RowCount = 7;
        _profileTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _profileTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _profileTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _profileTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _profileTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _profileTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _profileTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _profileTable.TabIndex = 0;
        //
        // _idLabel / _idTextBox
        //
        _profileTable.Controls.Add(_idLabel, 0, 0);
        _profileTable.Controls.Add(_idTextBox, 1, 0);
        _profileTable.SetColumnSpan(_idTextBox, 3);
        _idLabel.Anchor = AnchorStyles.Left;
        _idLabel.AutoSize = true;
        _idLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _idLabel.Margin = new Padding(3, 8, 8, 3);
        _idLabel.Name = "_idLabel";
        _idLabel.TabIndex = 0;
        _idLabel.Text = "ID hệ thống (GUID)";
        _idTextBox.BackColor = Color.White;
        _idTextBox.BorderStyle = BorderStyle.FixedSingle;
        _idTextBox.Dock = DockStyle.Fill;
        _idTextBox.Margin = new Padding(3, 5, 10, 5);
        _idTextBox.Name = "_idTextBox";
        _idTextBox.ReadOnly = true;
        _idTextBox.TabIndex = 1;
        _idTextBox.Font = new Font("Consolas", 9F);
        //
        // _codeLabel / _codeTextBox
        //
        _profileTable.Controls.Add(_codeLabel, 0, 1);
        _profileTable.Controls.Add(_codeTextBox, 1, 1);
        _codeLabel.Anchor = AnchorStyles.Left;
        _codeLabel.AutoSize = true;
        _codeLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _codeLabel.Margin = new Padding(3, 8, 8, 3);
        _codeLabel.Name = "_codeLabel";
        _codeLabel.TabIndex = 2;
        _codeLabel.Text = "Mã nhân viên";
        _codeTextBox.BackColor = Color.White;
        _codeTextBox.BorderStyle = BorderStyle.FixedSingle;
        _codeTextBox.Dock = DockStyle.Fill;
        _codeTextBox.Margin = new Padding(3, 5, 10, 5);
        _codeTextBox.Name = "_codeTextBox";
        _codeTextBox.ReadOnly = true;
        _codeTextBox.TabIndex = 3;

        //
        // _statusLabel / _statusTextBox
        //
        _profileTable.Controls.Add(_statusLabel, 2, 1);
        _profileTable.Controls.Add(_statusTextBox, 3, 1);
        _statusLabel.Anchor = AnchorStyles.Left;
        _statusLabel.AutoSize = true;
        _statusLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _statusLabel.Margin = new Padding(3, 8, 8, 3);
        _statusLabel.Name = "_statusLabel";
        _statusLabel.TabIndex = 4;
        _statusLabel.Text = "Trạng thái";
        _statusTextBox.BackColor = Color.White;
        _statusTextBox.BorderStyle = BorderStyle.FixedSingle;
        _statusTextBox.Dock = DockStyle.Fill;
        _statusTextBox.Margin = new Padding(3, 5, 10, 5);
        _statusTextBox.Name = "_statusTextBox";
        _statusTextBox.ReadOnly = true;
        _statusTextBox.TabIndex = 5;

        //
        // _nameLabel / _nameTextBox
        //
        _profileTable.Controls.Add(_nameLabel, 0, 2);
        _profileTable.Controls.Add(_nameTextBox, 1, 2);
        _profileTable.SetColumnSpan(_nameTextBox, 3);
        _nameLabel.Anchor = AnchorStyles.Left;
        _nameLabel.AutoSize = true;
        _nameLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _nameLabel.Margin = new Padding(3, 8, 8, 3);
        _nameLabel.Name = "_nameLabel";
        _nameLabel.TabIndex = 6;
        _nameLabel.Text = "Họ và tên";
        _nameTextBox.BackColor = Color.White;
        _nameTextBox.BorderStyle = BorderStyle.FixedSingle;
        _nameTextBox.Dock = DockStyle.Fill;
        _nameTextBox.Margin = new Padding(3, 5, 10, 5);
        _nameTextBox.Name = "_nameTextBox";
        _nameTextBox.ReadOnly = true;
        _nameTextBox.TabIndex = 7;

        //
        // _birthLabel / _birthTextBox
        //
        _profileTable.Controls.Add(_birthLabel, 0, 3);
        _profileTable.Controls.Add(_birthTextBox, 1, 3);
        _birthLabel.Anchor = AnchorStyles.Left;
        _birthLabel.AutoSize = true;
        _birthLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _birthLabel.Margin = new Padding(3, 8, 8, 3);
        _birthLabel.Name = "_birthLabel";
        _birthLabel.TabIndex = 8;
        _birthLabel.Text = "Ngày sinh";
        _birthTextBox.BackColor = Color.White;
        _birthTextBox.BorderStyle = BorderStyle.FixedSingle;
        _birthTextBox.Dock = DockStyle.Fill;
        _birthTextBox.Margin = new Padding(3, 5, 10, 5);
        _birthTextBox.Name = "_birthTextBox";
        _birthTextBox.ReadOnly = true;
        _birthTextBox.TabIndex = 9;

        //
        // _genderLabel / _genderTextBox
        //
        _profileTable.Controls.Add(_genderLabel, 2, 3);
        _profileTable.Controls.Add(_genderTextBox, 3, 3);
        _genderLabel.Anchor = AnchorStyles.Left;
        _genderLabel.AutoSize = true;
        _genderLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _genderLabel.Margin = new Padding(3, 8, 8, 3);
        _genderLabel.Name = "_genderLabel";
        _genderLabel.TabIndex = 10;
        _genderLabel.Text = "Giới tính";
        _genderTextBox.BackColor = Color.White;
        _genderTextBox.BorderStyle = BorderStyle.FixedSingle;
        _genderTextBox.Dock = DockStyle.Fill;
        _genderTextBox.Margin = new Padding(3, 5, 10, 5);
        _genderTextBox.Name = "_genderTextBox";
        _genderTextBox.ReadOnly = true;
        _genderTextBox.TabIndex = 11;

        //
        // _citizenLabel / _citizenTextBox
        //
        _profileTable.Controls.Add(_citizenLabel, 0, 4);
        _profileTable.Controls.Add(_citizenTextBox, 1, 4);
        _citizenLabel.Anchor = AnchorStyles.Left;
        _citizenLabel.AutoSize = true;
        _citizenLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _citizenLabel.Margin = new Padding(3, 8, 8, 3);
        _citizenLabel.Name = "_citizenLabel";
        _citizenLabel.TabIndex = 12;
        _citizenLabel.Text = "CCCD/CMND";
        _citizenTextBox.BackColor = Color.White;
        _citizenTextBox.BorderStyle = BorderStyle.FixedSingle;
        _citizenTextBox.Dock = DockStyle.Fill;
        _citizenTextBox.Margin = new Padding(3, 5, 10, 5);
        _citizenTextBox.Name = "_citizenTextBox";
        _citizenTextBox.ReadOnly = true;
        _citizenTextBox.TabIndex = 13;

        //
        // _phoneLabel / _phoneTextBox
        //
        _profileTable.Controls.Add(_phoneLabel, 2, 4);
        _profileTable.Controls.Add(_phoneTextBox, 3, 4);
        _phoneLabel.Anchor = AnchorStyles.Left;
        _phoneLabel.AutoSize = true;
        _phoneLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _phoneLabel.Margin = new Padding(3, 8, 8, 3);
        _phoneLabel.Name = "_phoneLabel";
        _phoneLabel.TabIndex = 14;
        _phoneLabel.Text = "Điện thoại";
        _phoneTextBox.BackColor = Color.White;
        _phoneTextBox.BorderStyle = BorderStyle.FixedSingle;
        _phoneTextBox.Dock = DockStyle.Fill;
        _phoneTextBox.Margin = new Padding(3, 5, 10, 5);
        _phoneTextBox.Name = "_phoneTextBox";
        _phoneTextBox.ReadOnly = true;
        _phoneTextBox.TabIndex = 15;

        //
        // _emailLabel / _emailTextBox
        //
        _profileTable.Controls.Add(_emailLabel, 0, 5);
        _profileTable.Controls.Add(_emailTextBox, 1, 5);
        _profileTable.SetColumnSpan(_emailTextBox, 3);
        _emailLabel.Anchor = AnchorStyles.Left;
        _emailLabel.AutoSize = true;
        _emailLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _emailLabel.Margin = new Padding(3, 8, 8, 3);
        _emailLabel.Name = "_emailLabel";
        _emailLabel.TabIndex = 16;
        _emailLabel.Text = "Email";
        _emailTextBox.BackColor = Color.White;
        _emailTextBox.BorderStyle = BorderStyle.FixedSingle;
        _emailTextBox.Dock = DockStyle.Fill;
        _emailTextBox.Margin = new Padding(3, 5, 10, 5);
        _emailTextBox.Name = "_emailTextBox";
        _emailTextBox.ReadOnly = true;
        _emailTextBox.TabIndex = 17;

        //
        // _addressLabel / _addressTextBox
        //
        _profileTable.Controls.Add(_addressLabel, 0, 6);
        _profileTable.Controls.Add(_addressTextBox, 1, 6);
        _profileTable.SetColumnSpan(_addressTextBox, 3);
        _addressLabel.Anchor = AnchorStyles.Left;
        _addressLabel.AutoSize = true;
        _addressLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _addressLabel.Margin = new Padding(3, 8, 8, 3);
        _addressLabel.Name = "_addressLabel";
        _addressLabel.TabIndex = 18;
        _addressLabel.Text = "Địa chỉ";
        _addressTextBox.BackColor = Color.White;
        _addressTextBox.BorderStyle = BorderStyle.FixedSingle;
        _addressTextBox.Dock = DockStyle.Fill;
        _addressTextBox.Margin = new Padding(3, 5, 10, 5);
        _addressTextBox.Name = "_addressTextBox";
        _addressTextBox.ReadOnly = true;
        _addressTextBox.TabIndex = 19;
        _addressTextBox.AutoSize = false;
        _addressTextBox.Multiline = true;
        _addressTextBox.ScrollBars = ScrollBars.Vertical;
        _addressTextBox.Size = new Size(400, 64);
        _addressTextBox.MinimumSize = new Size(0, 64);
        //
        // _workGroup
        //
        _workGroup.AutoSize = true;
        _workGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _workGroup.BackColor = Color.White;
        _workGroup.Controls.Add(_workTable);
        _workGroup.Dock = DockStyle.Top;
        _workGroup.Margin = new Padding(0, 0, 0, 12);
        _workGroup.Name = "_workGroup";
        _workGroup.Padding = new Padding(12, 24, 12, 12);
        _workGroup.TabIndex = 1;
        _workGroup.Text = "Thông tin công tác và lương";
        //
        // _workTable
        //
        _workTable.AutoSize = true;
        _workTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _workTable.ColumnCount = 4;
        _workTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
        _workTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _workTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
        _workTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _workTable.Dock = DockStyle.Top;
        _workTable.Name = "_workTable";
        _workTable.RowCount = 3;
        _workTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _workTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _workTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _workTable.TabIndex = 0;
        //
        // _typeLabel / _typeTextBox
        //
        _workTable.Controls.Add(_typeLabel, 0, 0);
        _workTable.Controls.Add(_typeTextBox, 1, 0);
        _typeLabel.Anchor = AnchorStyles.Left;
        _typeLabel.AutoSize = true;
        _typeLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _typeLabel.Margin = new Padding(3, 8, 8, 3);
        _typeLabel.Name = "_typeLabel";
        _typeLabel.TabIndex = 0;
        _typeLabel.Text = "Loại nhân viên";
        _typeTextBox.BackColor = Color.White;
        _typeTextBox.BorderStyle = BorderStyle.FixedSingle;
        _typeTextBox.Dock = DockStyle.Fill;
        _typeTextBox.Margin = new Padding(3, 5, 10, 5);
        _typeTextBox.Name = "_typeTextBox";
        _typeTextBox.ReadOnly = true;
        _typeTextBox.TabIndex = 1;

        //
        // _salaryLabel / _salaryTextBox
        //
        _workTable.Controls.Add(_salaryLabel, 2, 0);
        _workTable.Controls.Add(_salaryTextBox, 3, 0);
        _salaryLabel.Anchor = AnchorStyles.Left;
        _salaryLabel.AutoSize = true;
        _salaryLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _salaryLabel.Margin = new Padding(3, 8, 8, 3);
        _salaryLabel.Name = "_salaryLabel";
        _salaryLabel.TabIndex = 2;
        _salaryLabel.Text = "Lương cơ bản";
        _salaryTextBox.BackColor = Color.White;
        _salaryTextBox.BorderStyle = BorderStyle.FixedSingle;
        _salaryTextBox.Dock = DockStyle.Fill;
        _salaryTextBox.Margin = new Padding(3, 5, 10, 5);
        _salaryTextBox.Name = "_salaryTextBox";
        _salaryTextBox.ReadOnly = true;
        _salaryTextBox.TabIndex = 3;

        //
        // _departmentLabel / _departmentTextBox
        //
        _workTable.Controls.Add(_departmentLabel, 0, 1);
        _workTable.Controls.Add(_departmentTextBox, 1, 1);
        _departmentLabel.Anchor = AnchorStyles.Left;
        _departmentLabel.AutoSize = true;
        _departmentLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _departmentLabel.Margin = new Padding(3, 8, 8, 3);
        _departmentLabel.Name = "_departmentLabel";
        _departmentLabel.TabIndex = 4;
        _departmentLabel.Text = "Phòng ban";
        _departmentTextBox.BackColor = Color.White;
        _departmentTextBox.BorderStyle = BorderStyle.FixedSingle;
        _departmentTextBox.Dock = DockStyle.Fill;
        _departmentTextBox.Margin = new Padding(3, 5, 10, 5);
        _departmentTextBox.Name = "_departmentTextBox";
        _departmentTextBox.ReadOnly = true;
        _departmentTextBox.TabIndex = 5;

        //
        // _positionLabel / _positionTextBox
        //
        _workTable.Controls.Add(_positionLabel, 2, 1);
        _workTable.Controls.Add(_positionTextBox, 3, 1);
        _positionLabel.Anchor = AnchorStyles.Left;
        _positionLabel.AutoSize = true;
        _positionLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _positionLabel.Margin = new Padding(3, 8, 8, 3);
        _positionLabel.Name = "_positionLabel";
        _positionLabel.TabIndex = 6;
        _positionLabel.Text = "Chức vụ";
        _positionTextBox.BackColor = Color.White;
        _positionTextBox.BorderStyle = BorderStyle.FixedSingle;
        _positionTextBox.Dock = DockStyle.Fill;
        _positionTextBox.Margin = new Padding(3, 5, 10, 5);
        _positionTextBox.Name = "_positionTextBox";
        _positionTextBox.ReadOnly = true;
        _positionTextBox.TabIndex = 7;

        //
        // _createdLabel / _createdTextBox
        //
        _workTable.Controls.Add(_createdLabel, 0, 2);
        _workTable.Controls.Add(_createdTextBox, 1, 2);
        _createdLabel.Anchor = AnchorStyles.Left;
        _createdLabel.AutoSize = true;
        _createdLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _createdLabel.Margin = new Padding(3, 8, 8, 3);
        _createdLabel.Name = "_createdLabel";
        _createdLabel.TabIndex = 8;
        _createdLabel.Text = "Ngày tạo hồ sơ";
        _createdTextBox.BackColor = Color.White;
        _createdTextBox.BorderStyle = BorderStyle.FixedSingle;
        _createdTextBox.Dock = DockStyle.Fill;
        _createdTextBox.Margin = new Padding(3, 5, 10, 5);
        _createdTextBox.Name = "_createdTextBox";
        _createdTextBox.ReadOnly = true;
        _createdTextBox.TabIndex = 9;

        //
        // _updatedLabel / _updatedTextBox
        //
        _workTable.Controls.Add(_updatedLabel, 2, 2);
        _workTable.Controls.Add(_updatedTextBox, 3, 2);
        _updatedLabel.Anchor = AnchorStyles.Left;
        _updatedLabel.AutoSize = true;
        _updatedLabel.ForeColor = Color.FromArgb(51, 65, 85);
        _updatedLabel.Margin = new Padding(3, 8, 8, 3);
        _updatedLabel.Name = "_updatedLabel";
        _updatedLabel.TabIndex = 10;
        _updatedLabel.Text = "Cập nhật gần nhất";
        _updatedTextBox.BackColor = Color.White;
        _updatedTextBox.BorderStyle = BorderStyle.FixedSingle;
        _updatedTextBox.Dock = DockStyle.Fill;
        _updatedTextBox.Margin = new Padding(3, 5, 10, 5);
        _updatedTextBox.Name = "_updatedTextBox";
        _updatedTextBox.ReadOnly = true;
        _updatedTextBox.TabIndex = 11;

        //
        // _footer
        //
        _footer.AutoSize = true;
        _footer.ColumnCount = 2;
        _footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _footer.Controls.Add(_hintLabel, 0, 0);
        _footer.Controls.Add(_closeButton, 1, 0);
        _footer.Dock = DockStyle.Fill;
        _footer.Margin = new Padding(0, 12, 0, 0);
        _footer.Name = "_footer";
        _footer.RowCount = 1;
        _footer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _footer.TabIndex = 3;
        //
        // _hintLabel
        //
        _hintLabel.Anchor = AnchorStyles.Left;
        _hintLabel.AutoSize = true;
        _hintLabel.ForeColor = Color.FromArgb(80, 96, 115);
        _hintLabel.Name = "_hintLabel";
        _hintLabel.TabIndex = 0;
        _hintLabel.Text = "Chỉ xem. Có thể chọn và sao chép thông tin từng trường.";
        //
        // _closeButton
        //
        _closeButton.Anchor = AnchorStyles.Right;
        _closeButton.DialogResult = DialogResult.Cancel;
        _closeButton.Margin = new Padding(12, 0, 0, 0);
        _closeButton.Name = "_closeButton";
        _closeButton.Size = new Size(105, 36);
        _closeButton.TabIndex = 1;
        _closeButton.Text = "Đóng";
        _closeButton.UseVisualStyleBackColor = true;
        //
        // EmployeeDetailsForm
        //
        AcceptButton = _closeButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(244, 246, 249);
        CancelButton = _closeButton;
        ClientSize = new Size(920, 680);
        Controls.Add(_layout);
        Font = new Font("Segoe UI", 9F);
        MinimizeBox = false;
        MinimumSize = new Size(760, 560);
        Name = "EmployeeDetailsForm";
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Chi tiết nhân viên";
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        _scrollPanel.ResumeLayout(false);
        _scrollPanel.PerformLayout();
        _contentLayout.ResumeLayout(false);
        _contentLayout.PerformLayout();
        _profileGroup.ResumeLayout(false);
        _profileGroup.PerformLayout();
        _profileTable.ResumeLayout(false);
        _profileTable.PerformLayout();
        _workGroup.ResumeLayout(false);
        _workGroup.PerformLayout();
        _workTable.ResumeLayout(false);
        _workTable.PerformLayout();
        _footer.ResumeLayout(false);
        _footer.PerformLayout();
        ResumeLayout(false);
    }
}

