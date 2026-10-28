namespace POSSystem.DTO.Requests.Customer;

public class UpdateCustomerRequest
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? TaxNumber { get; set; }

    public decimal OpeningBalance { get; set; }

    public bool IsActive { get; set; }
}