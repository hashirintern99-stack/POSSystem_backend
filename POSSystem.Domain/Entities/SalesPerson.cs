// POSSystem.Domain.Entities/SalesPerson.cs

namespace POSSystem.Domain.Entities;

public class SalesPerson
{
    public int SalesPersonId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public decimal CommissionPercentage { get; set; }

    public bool IsActive { get; set; }

    // ✅ Use DeletedAt (matches your database column)
    public DateTime? DeletedAt { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}