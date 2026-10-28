public class PurchaseDetail
{
    public int PurchaseDetailId { get; set; }

    public int PurchaseId { get; set; }

    public int ProductId { get; set; }
    public string? ProductName { get; set; }

    public decimal Quantity { get; set; }

    public decimal CostPrice { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }

    public string RowState { get; set; } = "Original";
}