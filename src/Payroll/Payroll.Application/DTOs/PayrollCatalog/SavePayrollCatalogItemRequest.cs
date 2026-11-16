namespace Payroll.Application.DTOs.PayrollCatalog;

public class SavePayrollCatalogItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CalculationType { get; set; } = "fixed";
    public decimal DefaultRate { get; set; }
    public bool IsActive { get; set; } = true;
}
