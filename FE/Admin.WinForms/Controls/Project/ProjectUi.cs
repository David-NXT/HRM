using System.Globalization;

namespace Admin.WinForms.Controls.Project;

internal static class ProjectUi
{
    internal static readonly Color Background = Color.White;
    internal static readonly Color Surface = Color.White;
    internal static readonly Color Border = Color.FromArgb(215, 220, 228);
    internal static readonly Color Text = Color.FromArgb(37, 48, 74);
    internal static readonly Color MutedText = Color.FromArgb(96, 107, 121);
    internal static readonly Color Blue = Color.FromArgb(219, 234, 254);
    internal static readonly Color Accent = Color.FromArgb(35, 83, 166);

    internal static Button CreateButton(string text, bool primary = false)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = false,
            Height = 38,
            FlatStyle = FlatStyle.Flat,
            BackColor = primary ? Accent : Background,
            ForeColor = primary ? Color.White : Text,
            Font = new Font("Segoe UI", 10F),
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderColor = primary ? Accent : Border;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.MouseOverBackColor = primary ? Color.FromArgb(29, 69, 139) : Color.FromArgb(238, 242, 247);
        return button;
    }

    internal static void SetTabButtonState(Button button, bool selected)
    {
        button.BackColor = selected ? Blue : Background;
        button.ForeColor = selected ? Accent : Text;
        button.FlatAppearance.BorderColor = selected ? Accent : Border;
    }

    internal static TextBox CreateSearchBox(string placeholder)
    {
        return new TextBox
        {
            PlaceholderText = placeholder,
            BackColor = Background,
            ForeColor = Text,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 10F),
            Height = 36
        };
    }

    internal static DataGridView CreateGrid()
    {
        return new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Background,
            BorderStyle = BorderStyle.FixedSingle,
            CellBorderStyle = DataGridViewCellBorderStyle.Single,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
            ColumnHeadersHeight = 38,
            EnableHeadersVisualStyles = false,
            GridColor = Color.FromArgb(205, 205, 205),
            RowHeadersVisible = false,
            RowTemplate = { Height = 38 },
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ColumnHeadersDefaultCellStyle =
            {
                BackColor = Surface,
                ForeColor = Text,
                Font = new Font("Segoe UI", 10F),
                SelectionBackColor = Surface,
                SelectionForeColor = Text,
                Padding = new Padding(6, 0, 0, 0)
            },
            DefaultCellStyle =
            {
                BackColor = Background,
                ForeColor = Text,
                Font = new Font("Segoe UI", 10F),
                SelectionBackColor = Blue,
                SelectionForeColor = Text,
                Padding = new Padding(6, 0, 0, 0)
            }
        };
    }

    internal static DataGridViewTextBoxColumn AddColumn(DataGridView grid, string name, string title, float width)
    {
        var column = new DataGridViewTextBoxColumn
        {
            Name = name,
            HeaderText = title,
            FillWeight = width,
            SortMode = DataGridViewColumnSortMode.NotSortable
        };
        grid.Columns.Add(column);
        return column;
    }

    internal static Label CreateSectionTitle(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            ForeColor = Text,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
    }

    internal static Label CreateLabel(string text, bool muted = false)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            ForeColor = muted ? MutedText : Text,
            Font = new Font("Segoe UI", 10F)
        };
    }

    internal static ComboBox CreateComboBox()
    {
        return new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Background,
            ForeColor = Text,
            FlatStyle = FlatStyle.Standard,
            Font = new Font("Segoe UI", 10F)
        };
    }

    internal static string ShortId(Guid id) => id.ToString("N")[..8].ToUpperInvariant();

    internal static string Status(string? status) => status?.Trim().ToLowerInvariant() switch
    {
        "đang chạy" => "Active",
        "đang làm" or "inprogress" => "In Progress",
        "hoàn thành" or "done" => "Completed",
        "chưa bắt đầu" => "Not Started",
        "chờ duyệt" or "submitted" => "Pending",
        "đã duyệt" => "Approved",
        "từ chối" => "Rejected",
        "lập kế hoạch" => "Planning",
        "tạm dừng" => "On Hold",
        null or "" => "—",
        _ => status.Trim()
    };

    internal static string Priority(string? priority) => priority?.Trim().ToLowerInvariant() switch
    {
        "cao" => "High",
        "trung bình" => "Medium",
        "thấp" => "Low",
        null or "" => "—",
        _ => priority.Trim()
    };

    internal static string Money(decimal amount) =>
        amount.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));

    internal static string Date(DateOnly date) => date.ToString("dd/MM/yyyy");

    internal static string Date(DateOnly? date) => date?.ToString("dd/MM/yyyy") ?? "—";

    internal static string Date(DateTime? date) => date?.ToString("dd/MM/yyyy") ?? "—";

    internal static Label CreateStatusLabel()
    {
        return new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = MutedText,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };
    }

    internal static void SetError(Label label, string message)
    {
        label.Text = message;
        label.ForeColor = Color.FromArgb(255, 115, 115);
    }
}
