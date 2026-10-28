// POSSystem.DTO.Responses.Product/ProductResponse.cs
namespace POSSystem.DTO.Responses.Product;

public class ProductResponse
{
    public int ProductId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public int? BrandId { get; set; }
    public string? BrandName { get; set; }

    public int? UnitId { get; set; }
    public string? UnitName { get; set; }

    public decimal CostPrice { get; set; }
    public decimal RetailPrice { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal MinimumStock { get; set; }

    // ✅ ADD THIS - CurrentStock
    public decimal CurrentStock { get; set; }

    public string? ImageURL { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}