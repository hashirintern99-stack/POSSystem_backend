public class CreateGRNDetailRequest
{
    public int GRNDetailId { get; set; }

    public int ProductId { get; set; }

    public decimal OrderedQuantity { get; set; }

    public decimal ReceivedQuantity { get; set; }

    public decimal DamagedQuantity { get; set; }

    public decimal AcceptedQuantity { get; set; }

    public decimal CostPrice { get; set; }

    public decimal Total { get; set; }

    public string RowState { get; set; } = "Original";
}