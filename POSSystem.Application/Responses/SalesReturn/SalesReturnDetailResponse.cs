// POSSystem.DTO.Responses.SalesReturn/SalesReturnDetailResponse.cs

namespace POSSystem.DTO.Responses.SalesReturn;

public class SalesReturnDetailResponse
{
    public int SalesReturnDetailId { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal RetailPrice { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }

    public string? Reason { get; set; }
}