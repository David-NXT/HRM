using Microsoft.EntityFrameworkCore;
using Payroll.Application.DTOs.PayrollCatalog;
using Payroll.Application.Interfaces;
using Payroll.Domain.Entities;

namespace Payroll.Application.Services.PayrollCatalog;

public class PayrollCatalogService
{
    private static readonly HashSet<string> SupportedCalculationTypes =
        new(StringComparer.OrdinalIgnoreCase) { "fixed", "percentage", "formula" };

    private readonly IApplicationDbContext _context;

    public PayrollCatalogService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PayrollCatalogItemResponse>> GetAllowanceTypesAsync(
        bool activeOnly,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AllowanceTypes.AsNoTracking();
        if (activeOnly)
        {
            query = query.Where(item => item.Status == "active");
        }

        return await query
            .OrderBy(item => item.Name)
            .Select(item => new PayrollCatalogItemResponse
            {
                ID = item.AllowanceTypeID,
                Name = item.Name,
                Description = item.Description,
                CalculationType = item.CalculationType,
                DefaultRate = item.DefaultRate,
                Status = item.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PayrollCatalogItemResponse>> GetPayCutReasonsAsync(
        bool activeOnly,
        CancellationToken cancellationToken = default)
    {
        var query = _context.PayCutReasons.AsNoTracking();
        if (activeOnly)
        {
            query = query.Where(item => item.Status == "active");
        }

        return await query
            .OrderBy(item => item.Name)
            .Select(item => new PayrollCatalogItemResponse
            {
                ID = item.PayCutReasonID,
                Name = item.Name,
                Description = item.Description,
                CalculationType = item.CalculationType,
                DefaultRate = item.DefaultRate,
                Status = item.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PayrollCatalogItemResponse> CreateAllowanceTypeAsync(
        SavePayrollCatalogItemRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);
        await EnsureUniqueNameAsync(request.Name, null, true, cancellationToken);

        var item = new AllowanceType();
        Apply(item, request);
        _context.AllowanceTypes.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return Map(item);
    }

    public async Task<PayrollCatalogItemResponse> UpdateAllowanceTypeAsync(
        Guid id,
        SavePayrollCatalogItemRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);
        var item = await _context.AllowanceTypes.FirstOrDefaultAsync(
            value => value.AllowanceTypeID == id,
            cancellationToken) ?? throw new KeyNotFoundException("Không tìm thấy loại phụ cấp.");
        await EnsureUniqueNameAsync(request.Name, id, true, cancellationToken);

        Apply(item, request);
        await _context.SaveChangesAsync(cancellationToken);
        return Map(item);
    }

    public async Task DeactivateAllowanceTypeAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var item = await _context.AllowanceTypes.FirstOrDefaultAsync(
            value => value.AllowanceTypeID == id,
            cancellationToken) ?? throw new KeyNotFoundException("Không tìm thấy loại phụ cấp.");
        item.Status = "inactive";
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PayrollCatalogItemResponse> CreatePayCutReasonAsync(
        SavePayrollCatalogItemRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);
        await EnsureUniqueNameAsync(request.Name, null, false, cancellationToken);

        var item = new PayCutReason();
        Apply(item, request);
        _context.PayCutReasons.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return Map(item);
    }

    public async Task<PayrollCatalogItemResponse> UpdatePayCutReasonAsync(
        Guid id,
        SavePayrollCatalogItemRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);
        var item = await _context.PayCutReasons.FirstOrDefaultAsync(
            value => value.PayCutReasonID == id,
            cancellationToken) ?? throw new KeyNotFoundException("Không tìm thấy khoản khấu trừ.");
        await EnsureUniqueNameAsync(request.Name, id, false, cancellationToken);

        Apply(item, request);
        await _context.SaveChangesAsync(cancellationToken);
        return Map(item);
    }

    public async Task DeactivatePayCutReasonAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var item = await _context.PayCutReasons.FirstOrDefaultAsync(
            value => value.PayCutReasonID == id,
            cancellationToken) ?? throw new KeyNotFoundException("Không tìm thấy khoản khấu trừ.");
        item.Status = "inactive";
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUniqueNameAsync(
        string name,
        Guid? exceptID,
        bool isAllowance,
        CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim().ToLower();
        bool exists;
        if (isAllowance)
        {
            var query = _context.AllowanceTypes.Where(item => item.Name.ToLower() == normalizedName);
            if (exceptID.HasValue)
            {
                query = query.Where(item => item.AllowanceTypeID != exceptID.Value);
            }
            exists = await query.AnyAsync(cancellationToken);
        }
        else
        {
            var query = _context.PayCutReasons.Where(item => item.Name.ToLower() == normalizedName);
            if (exceptID.HasValue)
            {
                query = query.Where(item => item.PayCutReasonID != exceptID.Value);
            }
            exists = await query.AnyAsync(cancellationToken);
        }

        if (exists)
        {
            throw new InvalidOperationException("Tên danh mục đã tồn tại.");
        }
    }

    private static void Validate(SavePayrollCatalogItemRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Tên danh mục không được để trống.");
        }

        if (request.Name.Trim().Length > 120)
        {
            throw new ArgumentException("Tên danh mục không được vượt quá 120 ký tự.");
        }

        if (request.Description?.Length > 500)
        {
            throw new ArgumentException("Mô tả không được vượt quá 500 ký tự.");
        }

        if (!SupportedCalculationTypes.Contains(request.CalculationType))
        {
            throw new ArgumentException("Kiểu tính phải là fixed, percentage hoặc formula.");
        }

        if (request.DefaultRate < 0 || request.DefaultRate > 100_000_000m)
        {
            throw new ArgumentException("Giá trị mặc định phải nằm trong khoảng 0 đến 100.000.000.");
        }

        if (request.CalculationType.Equals("percentage", StringComparison.OrdinalIgnoreCase)
            && request.DefaultRate > 100m)
        {
            throw new ArgumentException("Tỷ lệ phần trăm không được vượt quá 100%.");
        }
    }

    private static void Apply(AllowanceType item, SavePayrollCatalogItemRequest request)
    {
        item.Name = request.Name.Trim();
        item.Description = request.Description?.Trim() ?? string.Empty;
        item.CalculationType = request.CalculationType.ToLowerInvariant();
        item.DefaultRate = request.DefaultRate;
        item.Status = request.IsActive ? "active" : "inactive";
    }

    private static void Apply(PayCutReason item, SavePayrollCatalogItemRequest request)
    {
        item.Name = request.Name.Trim();
        item.Description = request.Description?.Trim() ?? string.Empty;
        item.CalculationType = request.CalculationType.ToLowerInvariant();
        item.DefaultRate = request.DefaultRate;
        item.Status = request.IsActive ? "active" : "inactive";
    }

    private static PayrollCatalogItemResponse Map(AllowanceType item) => new()
    {
        ID = item.AllowanceTypeID,
        Name = item.Name,
        Description = item.Description,
        CalculationType = item.CalculationType,
        DefaultRate = item.DefaultRate,
        Status = item.Status
    };

    private static PayrollCatalogItemResponse Map(PayCutReason item) => new()
    {
        ID = item.PayCutReasonID,
        Name = item.Name,
        Description = item.Description,
        CalculationType = item.CalculationType,
        DefaultRate = item.DefaultRate,
        Status = item.Status
    };
}
