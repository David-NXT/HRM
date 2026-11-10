using System.Net.Mail;
using HR.Core.DTOs;
using HR.Core.Employees;
using HR.Core.Entities;
using HR.Core.Exceptions;
using HR.Core.Interfaces;

namespace HR.Core.Services;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employees;

    public EmployeeService(IEmployeeRepository employees)
    {
        _employees = employees;
    }

    public async Task<EmployeeListDto> ListAsync(
        string? keyword,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            throw new HrException(400, "ValidationError", "Số trang phải từ 1 và kích thước trang từ 1 đến 100.");
        }

        var (items, total) = await _employees.SearchAsync(keyword, page, pageSize, cancellationToken);
        return new EmployeeListDto(items.Select(ToDto).ToArray(), page, pageSize, total);
    }

    public async Task<EmployeeDto> GetAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            throw new HrException(404, "EmployeeNotFound", "Không tìm thấy hồ sơ.");
        }

        return ToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var profile = Normalize(
            request.EmployeeCode,
            request.FullName,
            request.DateOfBirth,
            request.Gender,
            request.Email,
            request.Phone,
            request.CitizenId,
            request.Address,
            requireCode: true);

        if (await _employees.CodeExistsAsync(profile.EmployeeCode, cancellationToken))
        {
            throw new HrException(409, "DuplicateEmployeeCode", "Mã nhân viên đã tồn tại. Nhập mã khác.");
        }

        var now = DateTime.UtcNow;
        var employee = new Employee
        {
            EmployeeCode = profile.EmployeeCode,
            FullName = profile.FullName,
            DateOfBirth = profile.DateOfBirth,
            Gender = profile.Gender,
            Email = profile.Email,
            Phone = profile.Phone,
            CitizenId = profile.CitizenId,
            Address = profile.Address,
            Status = EmployeeStatuses.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _employees.AddAsync(employee, cancellationToken);
        await _employees.SaveChangesAsync(cancellationToken);
        return ToDto(employee);
    }

    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var profile = Normalize(
            employeeCode: null,
            request.FullName,
            request.DateOfBirth,
            request.Gender,
            request.Email,
            request.Phone,
            request.CitizenId,
            request.Address,
            requireCode: false);

        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            throw new HrException(404, "EmployeeNotFound", "Không tìm thấy hồ sơ.");
        }

        employee.FullName = profile.FullName;
        employee.DateOfBirth = profile.DateOfBirth;
        employee.Gender = profile.Gender;
        employee.Email = profile.Email;
        employee.Phone = profile.Phone;
        employee.CitizenId = profile.CitizenId;
        employee.Address = profile.Address;
        employee.UpdatedAt = DateTime.UtcNow;

        await _employees.SaveChangesAsync(cancellationToken);
        return ToDto(employee);
    }

    public async Task<EmployeeDto> LockAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            throw new HrException(404, "EmployeeNotFound", "Không tìm thấy hồ sơ.");
        }

        employee.Status = EmployeeStatuses.Locked;
        employee.UpdatedAt = DateTime.UtcNow;
        await _employees.SaveChangesAsync(cancellationToken);
        return ToDto(employee);
    }

    private static EmployeeProfile Normalize(
        string? employeeCode,
        string? fullName,
        DateOnly dateOfBirth,
        string? gender,
        string? email,
        string? phone,
        string? citizenId,
        string? address,
        bool requireCode)
    {
        var errors = new List<string>();
        var code = (employeeCode ?? string.Empty).Trim().ToUpperInvariant();
        if (requireCode && (code.Length < 3 || code.Length > 20 || !code.All(IsCodeChar)))
        {
            errors.Add("Mã nhân viên phải từ 3 đến 20 ký tự chữ, số, gạch dưới hoặc gạch ngang.");
        }

        var name = (fullName ?? string.Empty).Trim();
        if (name.Length < 2 || name.Length > 150)
        {
            errors.Add("Họ tên phải từ 2 đến 150 ký tự.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (dateOfBirth.Year < 1900 || dateOfBirth > today)
        {
            errors.Add("Ngày sinh không hợp lệ.");
        }

        if (!Genders.TryNormalize(gender, out var normalizedGender))
        {
            errors.Add("Giới tính phải là Male, Female, Other hoặc Nam, Nữ, Khác.");
        }

        var mail = (email ?? string.Empty).Trim();
        if (mail.Length > 200 || !IsEmail(mail))
        {
            errors.Add("Email không đúng định dạng.");
        }

        var normalizedPhone = NormalizePhone(phone);
        if (normalizedPhone.Length is < 9 or > 15)
        {
            errors.Add("Số điện thoại không hợp lệ.");
        }

        var citizen = (citizenId ?? string.Empty).Trim();
        if (citizen.Length is not (9 or 12) || !citizen.All(char.IsDigit))
        {
            errors.Add("CCCD/CMND phải gồm 9 hoặc 12 chữ số.");
        }

        var normalizedAddress = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        if (normalizedAddress is { Length: > 500 })
        {
            errors.Add("Địa chỉ tối đa 500 ký tự.");
        }

        if (errors.Count > 0)
        {
            throw new HrException(400, "ValidationError", string.Join(" ", errors));
        }

        return new EmployeeProfile(code, name, dateOfBirth, normalizedGender, mail, normalizedPhone, citizen, normalizedAddress);
    }

    private static bool IsCodeChar(char value) => char.IsLetterOrDigit(value) || value is '_' or '-';

    private static bool IsEmail(string value) =>
        MailAddress.TryCreate(value, out var address) && address.Address.Equals(value, StringComparison.OrdinalIgnoreCase);

    private static string NormalizePhone(string? phone)
    {
        var text = (phone ?? string.Empty).Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace(".", string.Empty);
        if (text.StartsWith("+84", StringComparison.Ordinal))
        {
            text = "0" + text[3..];
        }
        else if (text.StartsWith("84", StringComparison.Ordinal) && text.Length >= 11)
        {
            text = "0" + text[2..];
        }

        return text.All(char.IsDigit) ? text : string.Empty;
    }

    private static EmployeeDto ToDto(Employee employee) =>
        new(
            employee.EmployeeId,
            employee.EmployeeCode,
            employee.FullName,
            employee.DateOfBirth,
            employee.Gender,
            employee.Email,
            employee.Phone,
            employee.CitizenId,
            employee.Address,
            employee.Status,
            employee.CreatedAt,
            employee.UpdatedAt);

    private sealed record EmployeeProfile(
        string EmployeeCode,
        string FullName,
        DateOnly DateOfBirth,
        string Gender,
        string Email,
        string Phone,
        string CitizenId,
        string? Address);
}
