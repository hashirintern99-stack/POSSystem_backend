namespace POSSystem.Domain.Entities;

public class Purchase
{
    public int PurchaseId { get; set; }

    public string PurchaseNumber { get; set; } = string.Empty;

    public int SupplierId { get; set; }
    public string? SupplierName { get; set; }

    public DateTime PurchaseDate { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal SubTotal { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }

    public string? Comments { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
    public List<PurchaseDetail> Details { get; set; } = new();
   
}