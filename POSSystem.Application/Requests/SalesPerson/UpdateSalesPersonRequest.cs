namespace POSSystem.DTO.Requests.SalesPerson;

public class UpdateSalesPersonRequest
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public decimal CommissionPercentage { get; set; }

    public bool IsActive { get; set; }
}