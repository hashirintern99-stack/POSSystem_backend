// POSSystem.DTO.Requests.Product/CreateProductRequest.cs
namespace POSSystem.DTO.Requests.Product;

public class CreateProductRequest
{
    public string Code { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int? UnitId { get; set; }

    public decimal CostPrice { get; set; }
    public decimal RetailPrice { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal MinimumStock { get; set; }

    // ✅ ADD THIS - CurrentStock (default 0 for new products)
    public decimal CurrentStock { get; set; } = 0;

    public string? ImageURL { get; set; }
}