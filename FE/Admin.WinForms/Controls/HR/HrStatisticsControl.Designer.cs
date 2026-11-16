#nullable disable
namespace Admin.WinForms.Controls.HR;

public sealed partial class HrStatisticsControl
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel layout;
    private FlowLayoutPanel Toolbar;
    private Label Status;
    private TableLayoutPanel cards;
    private Label _titleLabel;
    private Button _refreshButton;
    private Label _updatedLabel;
    private Label _scopeLabel;
    private Panel _totalCard;
    private Label _totalTitle;
    private Label _totalValue;
    private Panel _activeCard;
    private Label _activeTitle;
    private Label _activeValue;
    private Panel _lockedCard;
    private Label _lockedTitle;
    private Label _lockedValue;
    private Panel _resignedCard;
    private Label _resignedTitle;
    private Label _resignedValue;
    private Panel _fullTimeCard;
    private Label _fullTimeTitle;
    private Label _fullTimeValue;
    private Panel _partTimeCard;
    private Label _partTimeTitle;
    private Label _partTimeValue;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        layout = new TableLayoutPanel();
        Toolbar = new FlowLayoutPanel();
        Status = new Label();
        cards = new TableLayoutPanel();
        _titleLabel = new Label();
        _refreshButton = new Button();
        _updatedLabel = new Label();
        _scopeLabel = new Label();
        _totalCard = new Panel();
        _totalTitle = new Label();
        _totalValue = new Label();
        _activeCard = new Panel();
        _activeTitle = new Label();
        _activeValue = new Label();
        _lockedCard = new Panel();
        _lockedTitle = new Label();
        _lockedValue = new Label();
        _resignedCard = new Panel();
        _resignedTitle = new Label();
        _resignedValue = new Label();
        _fullTimeCard = new Panel();
        _fullTimeTitle = new Label();
        _fullTimeValue = new Label();
        _partTimeCard = new Panel();
        _partTimeTitle = new Label();
        _partTimeValue = new Label();
        layout.SuspendLayout();
        Toolbar.SuspendLayout();
        cards.SuspendLayout();
        _totalCard.SuspendLayout();
        _activeCard.SuspendLayout();
        _lockedCard.SuspendLayout();
        _resignedCard.SuspendLayout();
        _fullTimeCard.SuspendLayout();
        _partTimeCard.SuspendLayout();
        SuspendLayout();
        //
        // layout
        //
        layout.ColumnCount = 1;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.Controls.Add(Toolbar, 0, 0);
        layout.Controls.Add(Status, 0, 1);
        layout.Controls.Add(cards, 0, 2);
        layout.Controls.Add(_scopeLabel, 0, 3);
        layout.Dock = DockStyle.Fill;
        layout.Name = "layout";
        layout.Padding = new Padding(16);
        layout.RowCount = 4;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Size = new Size(1100, 650);
        layout.TabIndex = 0;
        //
        // Toolbar
        //
        Toolbar.AutoSize = true;
        Toolbar.Controls.Add(_titleLabel);
        Toolbar.Controls.Add(_refreshButton);
        Toolbar.Controls.Add(_updatedLabel);
        Toolbar.Dock = DockStyle.Fill;
        Toolbar.Name = "Toolbar";
        Toolbar.TabIndex = 0;
        Toolbar.WrapContents = true;
        //
        // _titleLabel
        //
        _titleLabel.AutoSize = true;
        _titleLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        _titleLabel.ForeColor = Color.FromArgb(35, 83, 166);
        _titleLabel.Margin = new Padding(3, 3, 24, 3);
        _titleLabel.Name = "_titleLabel";
        _titleLabel.TabIndex = 0;
        _titleLabel.Text = "Thống kê nhân sự HR";
        //
        // _refreshButton
        //
        _refreshButton.AutoSize = true;
        _refreshButton.Margin = new Padding(3, 3, 16, 3);
        _refreshButton.MinimumSize = new Size(110, 34);
        _refreshButton.Name = "_refreshButton";
        _refreshButton.Size = new Size(110, 34);
        _refreshButton.TabIndex = 1;
        _refreshButton.Text = "Làm mới";
        _refreshButton.UseVisualStyleBackColor = true;
        //
        // _updatedLabel
        //
        _updatedLabel.AutoSize = true;
        _updatedLabel.Margin = new Padding(3, 12, 3, 3);
        _updatedLabel.Name = "_updatedLabel";
        _updatedLabel.TabIndex = 2;
        _updatedLabel.Text = "Chưa cập nhật dữ liệu";
        //
        // Status
        //
        Status.AutoSize = true;
        Status.Dock = DockStyle.Fill;
        Status.ForeColor = Color.FromArgb(80, 96, 115);
        Status.Margin = new Padding(3, 8, 3, 12);
        Status.Name = "Status";
        Status.TabIndex = 1;
        Status.Text = "Bấm Làm mới để tải số liệu nhân sự.";
        //
        // cards
        //
        cards.ColumnCount = 3;
        cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
        cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
        cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
        cards.Dock = DockStyle.Fill;
        cards.Name = "cards";
        cards.RowCount = 2;
        cards.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        cards.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        cards.TabIndex = 2;
        //
        // _totalCard
        //
        cards.Controls.Add(_totalCard, 0, 0);
        _totalCard.BackColor = Color.White;
        _totalCard.BorderStyle = BorderStyle.FixedSingle;
        _totalCard.Controls.Add(_totalValue);
        _totalCard.Controls.Add(_totalTitle);
        _totalCard.Dock = DockStyle.Fill;
        _totalCard.Margin = new Padding(8);
        _totalCard.Name = "_totalCard";
        _totalCard.TabIndex = 0;
        //
        // _totalTitle
        //
        _totalTitle.Dock = DockStyle.Top;
        _totalTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _totalTitle.ForeColor = Color.FromArgb(51, 65, 85);
        _totalTitle.Height = 56;
        _totalTitle.Name = "_totalTitle";
        _totalTitle.Padding = new Padding(16, 8, 16, 0);
        _totalTitle.TabIndex = 0;
        _totalTitle.Text = "Tổng nhân viên";
        _totalTitle.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _totalValue
        //
        _totalValue.Dock = DockStyle.Fill;
        _totalValue.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
        _totalValue.ForeColor = Color.FromArgb(35, 83, 166);
        _totalValue.Name = "_totalValue";
        _totalValue.Padding = new Padding(16, 0, 16, 12);
        _totalValue.TabIndex = 1;
        _totalValue.Text = "—";
        _totalValue.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _activeCard
        //
        cards.Controls.Add(_activeCard, 1, 0);
        _activeCard.BackColor = Color.White;
        _activeCard.BorderStyle = BorderStyle.FixedSingle;
        _activeCard.Controls.Add(_activeValue);
        _activeCard.Controls.Add(_activeTitle);
        _activeCard.Dock = DockStyle.Fill;
        _activeCard.Margin = new Padding(8);
        _activeCard.Name = "_activeCard";
        _activeCard.TabIndex = 1;
        //
        // _activeTitle
        //
        _activeTitle.Dock = DockStyle.Top;
        _activeTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _activeTitle.ForeColor = Color.FromArgb(51, 65, 85);
        _activeTitle.Height = 56;
        _activeTitle.Name = "_activeTitle";
        _activeTitle.Padding = new Padding(16, 8, 16, 0);
        _activeTitle.TabIndex = 0;
        _activeTitle.Text = "Đang hoạt động (ACTIVE)";
        _activeTitle.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _activeValue
        //
        _activeValue.Dock = DockStyle.Fill;
        _activeValue.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
        _activeValue.ForeColor = Color.FromArgb(22, 101, 52);
        _activeValue.Name = "_activeValue";
        _activeValue.Padding = new Padding(16, 0, 16, 12);
        _activeValue.TabIndex = 1;
        _activeValue.Text = "—";
        _activeValue.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _lockedCard
        //
        cards.Controls.Add(_lockedCard, 2, 0);
        _lockedCard.BackColor = Color.White;
        _lockedCard.BorderStyle = BorderStyle.FixedSingle;
        _lockedCard.Controls.Add(_lockedValue);
        _lockedCard.Controls.Add(_lockedTitle);
        _lockedCard.Dock = DockStyle.Fill;
        _lockedCard.Margin = new Padding(8);
        _lockedCard.Name = "_lockedCard";
        _lockedCard.TabIndex = 2;
        //
        // _lockedTitle
        //
        _lockedTitle.Dock = DockStyle.Top;
        _lockedTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lockedTitle.ForeColor = Color.FromArgb(51, 65, 85);
        _lockedTitle.Height = 56;
        _lockedTitle.Name = "_lockedTitle";
        _lockedTitle.Padding = new Padding(16, 8, 16, 0);
        _lockedTitle.TabIndex = 0;
        _lockedTitle.Text = "Đã khóa (LOCKED)";
        _lockedTitle.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _lockedValue
        //
        _lockedValue.Dock = DockStyle.Fill;
        _lockedValue.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
        _lockedValue.ForeColor = Color.FromArgb(180, 83, 9);
        _lockedValue.Name = "_lockedValue";
        _lockedValue.Padding = new Padding(16, 0, 16, 12);
        _lockedValue.TabIndex = 1;
        _lockedValue.Text = "—";
        _lockedValue.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _resignedCard
        //
        cards.Controls.Add(_resignedCard, 0, 1);
        _resignedCard.BackColor = Color.White;
        _resignedCard.BorderStyle = BorderStyle.FixedSingle;
        _resignedCard.Controls.Add(_resignedValue);
        _resignedCard.Controls.Add(_resignedTitle);
        _resignedCard.Dock = DockStyle.Fill;
        _resignedCard.Margin = new Padding(8);
        _resignedCard.Name = "_resignedCard";
        _resignedCard.TabIndex = 3;
        //
        // _resignedTitle
        //
        _resignedTitle.Dock = DockStyle.Top;
        _resignedTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _resignedTitle.ForeColor = Color.FromArgb(51, 65, 85);
        _resignedTitle.Height = 56;
        _resignedTitle.Name = "_resignedTitle";
        _resignedTitle.Padding = new Padding(16, 8, 16, 0);
        _resignedTitle.TabIndex = 0;
        _resignedTitle.Text = "Đã nghỉ việc (RESIGNED)";
        _resignedTitle.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _resignedValue
        //
        _resignedValue.Dock = DockStyle.Fill;
        _resignedValue.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
        _resignedValue.ForeColor = Color.FromArgb(100, 116, 139);
        _resignedValue.Name = "_resignedValue";
        _resignedValue.Padding = new Padding(16, 0, 16, 12);
        _resignedValue.TabIndex = 1;
        _resignedValue.Text = "—";
        _resignedValue.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _fullTimeCard
        //
        cards.Controls.Add(_fullTimeCard, 1, 1);
        _fullTimeCard.BackColor = Color.White;
        _fullTimeCard.BorderStyle = BorderStyle.FixedSingle;
        _fullTimeCard.Controls.Add(_fullTimeValue);
        _fullTimeCard.Controls.Add(_fullTimeTitle);
        _fullTimeCard.Dock = DockStyle.Fill;
        _fullTimeCard.Margin = new Padding(8);
        _fullTimeCard.Name = "_fullTimeCard";
        _fullTimeCard.TabIndex = 4;
        //
        // _fullTimeTitle
        //
        _fullTimeTitle.Dock = DockStyle.Top;
        _fullTimeTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _fullTimeTitle.ForeColor = Color.FromArgb(51, 65, 85);
        _fullTimeTitle.Height = 56;
        _fullTimeTitle.Name = "_fullTimeTitle";
        _fullTimeTitle.Padding = new Padding(16, 8, 16, 0);
        _fullTimeTitle.TabIndex = 0;
        _fullTimeTitle.Text = "Full-time";
        _fullTimeTitle.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _fullTimeValue
        //
        _fullTimeValue.Dock = DockStyle.Fill;
        _fullTimeValue.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
        _fullTimeValue.ForeColor = Color.FromArgb(35, 83, 166);
        _fullTimeValue.Name = "_fullTimeValue";
        _fullTimeValue.Padding = new Padding(16, 0, 16, 12);
        _fullTimeValue.TabIndex = 1;
        _fullTimeValue.Text = "—";
        _fullTimeValue.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _partTimeCard
        //
        cards.Controls.Add(_partTimeCard, 2, 1);
        _partTimeCard.BackColor = Color.White;
        _partTimeCard.BorderStyle = BorderStyle.FixedSingle;
        _partTimeCard.Controls.Add(_partTimeValue);
        _partTimeCard.Controls.Add(_partTimeTitle);
        _partTimeCard.Dock = DockStyle.Fill;
        _partTimeCard.Margin = new Padding(8);
        _partTimeCard.Name = "_partTimeCard";
        _partTimeCard.TabIndex = 5;
        //
        // _partTimeTitle
        //
        _partTimeTitle.Dock = DockStyle.Top;
        _partTimeTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _partTimeTitle.ForeColor = Color.FromArgb(51, 65, 85);
        _partTimeTitle.Height = 56;
        _partTimeTitle.Name = "_partTimeTitle";
        _partTimeTitle.Padding = new Padding(16, 8, 16, 0);
        _partTimeTitle.TabIndex = 0;
        _partTimeTitle.Text = "Part-time";
        _partTimeTitle.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _partTimeValue
        //
        _partTimeValue.Dock = DockStyle.Fill;
        _partTimeValue.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
        _partTimeValue.ForeColor = Color.FromArgb(35, 83, 166);
        _partTimeValue.Name = "_partTimeValue";
        _partTimeValue.Padding = new Padding(16, 0, 16, 12);
        _partTimeValue.TabIndex = 1;
        _partTimeValue.Text = "—";
        _partTimeValue.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _scopeLabel
        //
        _scopeLabel.AutoSize = true;
        _scopeLabel.Dock = DockStyle.Fill;
        _scopeLabel.ForeColor = Color.FromArgb(80, 96, 115);
        _scopeLabel.Margin = new Padding(8, 12, 8, 3);
        _scopeLabel.Name = "_scopeLabel";
        _scopeLabel.TabIndex = 3;
        _scopeLabel.Text = "Phạm vi: toàn bộ hồ sơ hiện có, gồm nhân viên đã nghỉ việc; không gồm hồ sơ đã xóa.\r\nFull-time và Part-time tính trên tất cả trạng thái. Đang hoạt động chỉ tính ACTIVE.";
        //
        // HrStatisticsControl
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(244, 246, 249);
        Controls.Add(layout);
        Font = new Font("Segoe UI", 9F);
        Name = "HrStatisticsControl";
        Size = new Size(1100, 650);
        layout.ResumeLayout(false);
        layout.PerformLayout();
        Toolbar.ResumeLayout(false);
        Toolbar.PerformLayout();
        cards.ResumeLayout(false);
        _totalCard.ResumeLayout(false);
        _activeCard.ResumeLayout(false);
        _lockedCard.ResumeLayout(false);
        _resignedCard.ResumeLayout(false);
        _fullTimeCard.ResumeLayout(false);
        _partTimeCard.ResumeLayout(false);
        ResumeLayout(false);
    }
}

