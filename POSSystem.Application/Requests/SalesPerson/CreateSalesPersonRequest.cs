namespace POSSystem.DTO.Requests.SalesPerson;

public class CreateSalesPersonRequest
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public decimal CommissionPercentage { get; set; }
}