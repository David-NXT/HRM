using HR.Application.DTOs.Employees;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using System.Net.Mail;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
namespace HR.Application.Services.Employees;

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

    public Task<EmployeeStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken) =>
        _employees.GetStatisticsAsync(cancellationToken);

    public async Task<EmployeeDirectoryListDto> ListActiveDirectoryAsync(
        string? keyword,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        ValidatePage(page, pageSize);
        var (items, total) = await _employees.SearchActiveAsync(keyword, page, pageSize, cancellationToken);
        return new EmployeeDirectoryListDto(items.Select(ToDirectoryEntry).ToArray(), page, pageSize, total);
    }

    public async Task<EmployeeDirectoryEntry> GetActiveDirectoryEntryAsync(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null || !string.Equals(employee.Status, EmployeeStatuses.Active, StringComparison.OrdinalIgnoreCase))
        {
            throw new HrException(404, "EmployeeNotFound", "Không tìm thấy nhân viên đang hoạt động.");
        }

        return ToDirectoryEntry(employee);
    }

    public async Task<EmployeeDto> GetAsync(Guid id, CancellationToken cancellationToken)
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
        var employee = PrepareNewEmployee(request);
        if (await _employees.CodeExistsAsync(employee.EmployeeCode, cancellationToken))
        {
            throw new HrException(409, "DuplicateEmployeeCode", "Mã nhân viên đã tồn tại. Nhập mã khác.");
        }
        await _employees.AddAsync(employee, cancellationToken);
        await _employees.SaveChangesAsync(cancellationToken);
        return ToDto(employee);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            throw new HrException(404, "EmployeeNotFound", "Không tìm thấy hồ sơ.");
        }

        if (await _employees.HasRelatedRecordsAsync(id, cancellationToken))
        {
            throw new HrException(409, "EmployeeHasRelatedRecords",
                "Không thể xóa nhân viên đã có hợp đồng, lịch sử công tác, hồ sơ cập nhật, thông báo hoặc hồ sơ nghỉ việc.");
        }

        _employees.Remove(employee);
        await _employees.SaveChangesAsync(cancellationToken);
    }

    public Employee PrepareNewEmployee(CreateEmployeeRequest request)
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

        var now = DateTime.UtcNow;
        Employee employee = (request.EmploymentType ?? string.Empty).Trim().ToUpperInvariant() switch
        {
            EmploymentTypes.FullTime => new FullTimeEmployee(),
            EmploymentTypes.PartTime => new PartTimeEmployee(),
            _ => throw new HrException(400, "InvalidEmploymentType", "Loại nhân viên phải là FULL_TIME hoặc PART_TIME.")
        };
        employee.EmployeeCode = profile.EmployeeCode;
        employee.FullName = profile.FullName;
        employee.DateOfBirth = profile.DateOfBirth;
        employee.Gender = profile.Gender;
        employee.Email = profile.Email;
        employee.Phone = profile.Phone;
        employee.CitizenId = profile.CitizenId;
        employee.Address = profile.Address;
        ApplySalary(employee, request.BaseSalary, request.PartTimeHourlyRate, allowMissingSalary: true);
        employee.Status = EmployeeStatuses.Active;
        employee.CreatedAt = now;
        employee.UpdatedAt = now;
        return employee;
    }

    public async Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            throw new HrException(404, "EmployeeNotFound", "Không tìm thấy hồ sơ.");
        }
        ApplyPersonalInfoUpdate(employee, request);
        await _employees.SaveChangesAsync(cancellationToken);
        return ToDto(employee);
    }

    public async Task<EmployeeDto> UpdateSalaryAsync(
        Guid id,
        UpdateEmployeeSalaryRequest request,
        CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            throw new HrException(404, "EmployeeNotFound", "Không tìm thấy hồ sơ.");
        }

        ApplySalary(employee, request.BaseSalary, request.PartTimeHourlyRate, allowMissingSalary: false);
        employee.UpdatedAt = DateTime.UtcNow;
        await _employees.SaveChangesAsync(cancellationToken);
        return ToDto(employee);
    }

    private static void ValidateSalary(decimal baseSalary)
    {
        if (baseSalary < 0m || baseSalary > 9_999_999_999_999_999.99m || decimal.Round(baseSalary, 2) != baseSalary)
        {
            throw new HrException(400, "ValidationError", "Lương cơ bản phải từ 0 đến 9.999.999.999.999.999,99 và có tối đa 2 chữ số thập phân.");
        }
    }

    private static void ApplySalary(
        Employee employee,
        decimal? baseSalary,
        decimal? partTimeHourlyRate,
        bool allowMissingSalary)
    {
        switch (employee)
        {
            case FullTimeEmployee fullTime:
                if (partTimeHourlyRate.HasValue)
                {
                    throw new HrException(400, "InvalidSalaryForEmploymentType", "Nhân viên full-time chỉ nhận lương cơ bản.");
                }
                if (!baseSalary.HasValue && !allowMissingSalary)
                {
                    throw new HrException(400, "ValidationError", "Cần cung cấp lương cơ bản cho nhân viên full-time.");
                }
                fullTime.BaseSalary = baseSalary ?? 0m;
                ValidateSalary(fullTime.BaseSalary);
                break;

            case PartTimeEmployee partTime:
                if (baseSalary is > 0m)
                {
                    throw new HrException(400, "InvalidSalaryForEmploymentType", "Nhân viên part-time dùng trường partTimeSalary (đơn giá VND/giờ).");
                }
                if (!partTimeHourlyRate.HasValue && !allowMissingSalary)
                {
                    throw new HrException(400, "ValidationError", "Cần cung cấp đơn giá part-time theo giờ (VND/giờ).");
                }
                partTime.PartTimeSalary = partTimeHourlyRate ?? 0m;
                ValidateSalary(partTime.PartTimeSalary);
                break;

            default:
                throw new HrException(400, "InvalidEmploymentType", "Loại nhân viên không được hỗ trợ.");
        }
    }

    public void ValidatePersonalInfoUpdate(UpdateEmployeeRequest request) =>
        _ = Normalize(null, request.FullName, request.DateOfBirth, request.Gender, request.Email,
            request.Phone, request.CitizenId, request.Address, requireCode: false);

    public void ApplyPersonalInfoUpdate(Employee employee, UpdateEmployeeRequest request)
    {
        var profile = Normalize(null, request.FullName, request.DateOfBirth, request.Gender, request.Email,
            request.Phone, request.CitizenId, request.Address, requireCode: false);
        employee.FullName = profile.FullName;
        employee.DateOfBirth = profile.DateOfBirth;
        employee.Gender = profile.Gender;
        employee.Email = profile.Email;
        employee.Phone = profile.Phone;
        employee.CitizenId = profile.CitizenId;
        employee.Address = profile.Address;
        employee.UpdatedAt = DateTime.UtcNow;
    }

    public async Task<EmployeeDto> LockAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            throw new HrException(404, "EmployeeNotFound", "Không tìm thấy hồ sơ.");
        }

        if (employee.Status == EmployeeStatuses.Resigned)
        {
            throw new HrException(409, "InvalidEmployeeState", "Không thể khóa lại hồ sơ đã nghỉ việc.");
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
            employee.UpdatedAt,
            employee.DepartmentId,
            employee.PositionId,
            employee.Department?.Name,
            employee.Position?.Name,
            employee.EmploymentType,
            employee is FullTimeEmployee fullTime ? fullTime.BaseSalary : null,
            employee is PartTimeEmployee partTime ? partTime.PartTimeSalary : null);

    private static EmployeeDirectoryEntry ToDirectoryEntry(Employee employee) =>
        new(employee.EmployeeId, employee.EmployeeCode, employee.FullName, employee.Status,
            employee.Department?.Name, employee.Position?.Name, employee.EmploymentType);

    private static void ValidatePage(int page, int pageSize)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            throw new HrException(400, "ValidationError", "Số trang phải từ 1 và kích thước trang từ 1 đến 100.");
        }
    }

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
