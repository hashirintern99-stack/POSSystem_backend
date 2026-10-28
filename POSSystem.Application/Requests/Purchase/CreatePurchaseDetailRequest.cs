namespace POSSystem.DTO.Requests.Purchase;

public class CreatePurchaseDetailRequest
{
    public int PurchaseDetailId { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal CostPrice { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }

    public string RowState { get; set; } = "Original";
}