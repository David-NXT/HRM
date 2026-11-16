using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Admin.WinForms.DTOs.Payroll;
using Admin.WinForms.Models.HR;

namespace Admin.WinForms.Controls.Payroll;

internal sealed class PayrollEmployeeDetailsDialog : Form
{
    public PayrollEmployeeDetailsDialog(
        PayrollListDto payroll,
        PayrollDetailDto detail,
        EmployeeRow? employee,
        string? employeeLoadError)
    {
        Text = $"Chi tiết nhân viên · {payroll.EmployeeCode}";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(1100, 800);
        MinimumSize = new Size(850, 650);
        Font = new Font("Segoe UI", 9F);

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            ColumnCount = 2,
            RowCount = 3
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var employeeGroup = new GroupBox
        {
            Text = "Thông tin nhân viên",
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(12)
        };
        var employeeInfo = CreateInfoTable();
        if (employee is null)
        {
            AddInfo(employeeInfo, "Trạng thái tải hồ sơ", "Không thể tải hồ sơ nhân viên.");
            AddInfo(employeeInfo, "Chi tiết lỗi", employeeLoadError ?? "Không có thông tin lỗi.");
            AddInfo(employeeInfo, "Mã nhân viên", payroll.EmployeeCode ?? "--");
            AddInfo(employeeInfo, "Họ và tên", payroll.EmployeeName ?? "--");
        }
        else
        {
            var culture = CultureInfo.GetCultureInfo("vi-VN");
            AddInfo(employeeInfo, "Mã nhân viên", employee.EmployeeCode);
            AddInfo(employeeInfo, "Họ và tên", employee.FullName);
            AddInfo(employeeInfo, "Ngày sinh", employee.DateOfBirth.ToString("dd/MM/yyyy"));
            AddInfo(employeeInfo, "Giới tính", FormatGender(employee.Gender));
            AddInfo(employeeInfo, "CCCD", employee.CitizenId);
            AddInfo(employeeInfo, "Điện thoại", employee.Phone);
            AddInfo(employeeInfo, "Email", employee.Email);
            AddInfo(employeeInfo, "Địa chỉ", DisplayOr(employee.Address));
            AddInfo(employeeInfo, "Loại nhân viên", employee.EmploymentTypeLabel);
            AddInfo(
                employeeInfo,
                employee.EmploymentType == "PART_TIME" ? "Đơn giá giờ" : "Lương cơ bản",
                FormatSalary(employee, culture));
            AddInfo(employeeInfo, "Phòng ban", DisplayOr(employee.DepartmentName));
            AddInfo(employeeInfo, "Chức vụ", DisplayOr(employee.PositionName));
            AddInfo(employeeInfo, "Trạng thái", FormatEmployeeStatus(employee.Status));
            AddInfo(employeeInfo, "Ngày tạo hồ sơ", employee.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm"));
            AddInfo(employeeInfo, "Cập nhật gần nhất", employee.UpdatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm"));
        }
        employeeGroup.Controls.Add(employeeInfo);
        content.Controls.Add(employeeGroup, 0, 0);
        content.SetColumnSpan(employeeGroup, 2);

        var payrollGroup = new GroupBox
        {
            Text = "Thông tin lương kỳ này",
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(12)
        };
        var payrollInfo = CreateInfoTable();
        AddInfo(payrollInfo, "Loại nhân viên", FormatEmploymentType(payroll.EmploymentType));
        AddInfo(payrollInfo, "Lương cơ bản", FormatMoney(detail.BaseSalary));
        AddInfo(payrollInfo, "Công chuẩn", $"{detail.StandardWorkingDay:0.##} ngày");
        AddInfo(payrollInfo, "Công thực tế", $"{detail.WorkingDays:0.##} ngày");
        AddInfo(payrollInfo, "Tổng phụ cấp dự án", FormatMoney(detail.TotalProjectAllowance));
        AddInfo(payrollInfo, "Tổng phụ cấp", FormatMoney(detail.TotalAllowance));
        AddInfo(payrollInfo, "Tổng khấu trừ", FormatMoney(detail.TotalPayCut));
        AddInfo(payrollInfo, "Lương Gross", FormatMoney(detail.GrossSalary));
        AddInfo(payrollInfo, "Thực lĩnh (Net)", FormatMoney(detail.NetSalary));
        AddInfo(payrollInfo, "Trạng thái", detail.Status);
        payrollGroup.Controls.Add(payrollInfo);
        content.Controls.Add(payrollGroup, 0, 1);
        content.SetColumnSpan(payrollGroup, 2);

        var allowanceGroup = CreateDetailGroup("Chi tiết phụ cấp", new[]
        {
            ("Khoản phụ cấp", 230),
            ("Mô tả", 300),
            ("Tỷ lệ", 100),
            ("Số tiền", 140)
        });
        var allowanceList = (ListView)allowanceGroup.Controls[0];
        if (detail.TotalProjectAllowance > 0)
        {
            AddDetailRow(allowanceList, "Phụ cấp dự án", "--", "--", FormatMoney(detail.TotalProjectAllowance));
        }
        foreach (var allowance in detail.Allowances)
        {
            AddDetailRow(
                allowanceList,
                allowance.AllowanceTypeName,
                DisplayOr(allowance.Description),
                allowance.RateApplied.ToString("0.##"),
                FormatMoney(allowance.Amount));
        }

        var payCutGroup = CreateDetailGroup("Chi tiết khấu trừ", new[]
        {
            ("Khoản khấu trừ", 230),
            ("Mô tả", 300),
            ("Tỷ lệ", 100),
            ("Số tiền", 140)
        });
        var payCutList = (ListView)payCutGroup.Controls[0];
        foreach (var payCut in detail.PayCuts)
        {
            AddDetailRow(
                payCutList,
                payCut.PayCutReasonName,
                DisplayOr(payCut.Description),
                payCut.RateApplied.ToString("0.##"),
                FormatMoney(payCut.Amount));
        }

        content.Controls.Add(allowanceGroup, 0, 2);
        content.Controls.Add(payCutGroup, 1, 2);
        Controls.Add(content);
    }

    private static TableLayoutPanel CreateInfoTable()
    {
        var table = new TableLayoutPanel
        {
            AutoSize = true,
            ColumnCount = 4,
            Dock = DockStyle.Top,
            Padding = new Padding(8)
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
        return table;
    }

    private static void AddInfo(TableLayoutPanel table, string caption, string value)
    {
        var fieldIndex = table.Controls.Count / 2;
        var row = fieldIndex / 2;
        var column = fieldIndex % 2 * 2;
        if (column == 0)
        {
            table.RowCount++;
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        table.Controls.Add(new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Text = caption,
            MaximumSize = new Size(200, 0),
            Padding = new Padding(0, 3, 10, 3)
        }, column, row);
        table.Controls.Add(new Label
        {
            AutoSize = true,
            MaximumSize = new Size(350, 0),
            Text = string.IsNullOrWhiteSpace(value) ? "--" : value,
            Padding = new Padding(0, 3, 0, 3)
        }, column + 1, row);
    }

    private static GroupBox CreateDetailGroup(string title, (string Name, int Width)[] columns)
    {
        var list = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            HeaderStyle = ColumnHeaderStyle.Nonclickable
        };
        foreach (var column in columns)
        {
            list.Columns.Add(column.Name, column.Width);
        }

        var group = new GroupBox
        {
            Text = title,
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        group.Controls.Add(list);
        return group;
    }

    private static void AddDetailRow(ListView list, string name, string description, string rate, string amount)
    {
        var row = new ListViewItem(name);
        row.SubItems.Add(description);
        row.SubItems.Add(rate);
        row.SubItems.Add(amount);
        list.Items.Add(row);
    }

    private static string FormatSalary(EmployeeRow employee, CultureInfo culture)
    {
        var amount = employee.EmploymentType == "PART_TIME"
            ? employee.PartTimeHourlyRate
            : employee.BaseSalary;
        var unit = employee.EmploymentType == "PART_TIME" ? " VND/giờ" : " VND/tháng";
        return amount.HasValue ? amount.Value.ToString("N2", culture) + unit : "Chưa cập nhật";
    }

    private static string FormatMoney(decimal amount) =>
        amount.ToString("#,##0 đ", CultureInfo.GetCultureInfo("vi-VN"));

    private static string FormatGender(string? gender) => gender switch
    {
        "Male" => "Nam",
        "Female" => "Nữ",
        "Other" => "Khác",
        _ => DisplayOr(gender)
    };

    private static string FormatEmployeeStatus(string? status) => status switch
    {
        "ACTIVE" => "Đang hoạt động",
        "LOCKED" => "Đã khóa",
        "RESIGNED" => "Đã nghỉ việc",
        _ => DisplayOr(status)
    };

    private static string FormatEmploymentType(string? employmentType) => employmentType switch
    {
        "FULL_TIME" => "Chính thức",
        "PART_TIME" => "Part-time",
        _ => DisplayOr(employmentType)
    };

    private static string DisplayOr(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "--" : value;
}
