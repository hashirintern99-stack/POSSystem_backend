namespace POSSystem.Domain.Entities;

public class Product
{
    public int ProductId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int CategoryId { get; set; }      // ✅ NOT NULL - matches DB
    public int? BrandId { get; set; }         // ✅ Nullable - matches DB
    public int? UnitId { get; set; }          // ✅ Nullable - matches DB

    public decimal CostPrice { get; set; }
    public decimal RetailPrice { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal CurrentStock { get; set; }
    public string? ImageURL { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}