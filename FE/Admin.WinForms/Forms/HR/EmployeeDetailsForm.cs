using Admin.WinForms.Models.HR;
using System.Globalization;

namespace Admin.WinForms.Forms.HR;

public partial class EmployeeDetailsForm : Form
{
    public EmployeeDetailsForm() => InitializeComponent();

    public EmployeeDetailsForm(EmployeeRow employee) : this()
    {
        Text = $"Chi tiết nhân viên · {employee.EmployeeCode}";
        _idTextBox.Text = employee.EmployeeId.ToString();
        _codeTextBox.Text = employee.EmployeeCode;
        _nameTextBox.Text = employee.FullName;
        _birthTextBox.Text = employee.DateOfBirth.ToString("dd/MM/yyyy");
        _genderTextBox.Text = employee.Gender switch
        {
            "Male" => "Nam",
            "Female" => "Nữ",
            "Other" => "Khác",
            _ => employee.Gender
        };
        _statusTextBox.Text = employee.Status switch
        {
            "ACTIVE" => "Đang hoạt động (ACTIVE)",
            "LOCKED" => "Đã khóa (LOCKED)",
            "RESIGNED" => "Đã nghỉ việc (RESIGNED)",
            _ => employee.Status
        };
        _citizenTextBox.Text = employee.CitizenId;
        _phoneTextBox.Text = employee.Phone;
        _emailTextBox.Text = employee.Email;
        _addressTextBox.Text = DisplayOr(employee.Address, "Chưa cập nhật");
        _typeTextBox.Text = employee.EmploymentTypeLabel;
        _departmentTextBox.Text = DisplayOr(employee.DepartmentName, "Chưa phân công");
        _positionTextBox.Text = DisplayOr(employee.PositionName, "Chưa phân công");

        var partTime = employee.EmploymentType == "PART_TIME";
        var salary = partTime ? employee.PartTimeHourlyRate : employee.BaseSalary;
        _salaryLabel.Text = partTime ? "Đơn giá giờ" : "Lương cơ bản";
        _salaryTextBox.Text = salary.HasValue
            ? salary.Value.ToString("N2", CultureInfo.GetCultureInfo("vi-VN")) + (partTime ? " VND/giờ" : " VND/tháng")
            : "Chưa cập nhật";
        _createdTextBox.Text = employee.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
        _updatedTextBox.Text = employee.UpdatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
    }

    private static string DisplayOr(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value;
}
