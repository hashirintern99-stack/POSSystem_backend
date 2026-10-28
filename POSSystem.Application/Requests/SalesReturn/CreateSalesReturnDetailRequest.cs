// POSSystem.DTO.Requests.SalesReturn/CreateSalesReturnDetailRequest.cs

namespace POSSystem.DTO.Requests.SalesReturn;

public class CreateSalesReturnDetailRequest
{
    public int SalesReturnDetailId { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal RetailPrice { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }

    public string? Reason { get; set; }

    public string RowState { get; set; } = "Original";
}