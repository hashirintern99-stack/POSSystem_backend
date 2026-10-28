// POSSystem.DTO.Responses.Sale/SaleDetailResponse.cs

namespace POSSystem.DTO.Responses.Sale;

public class SaleDetailResponse
{
    public int SaleDetailId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductCode { get; set; }
    public decimal Quantity { get; set; }
    public decimal RetailPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
}