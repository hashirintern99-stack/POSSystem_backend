// POSSystem.DTO.Responses.Product/LowStockProductDto.cs

namespace POSSystem.DTO.Responses.Product;

public class LowStockProductDto
{
    public int ProductId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public string StockStatus { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
}