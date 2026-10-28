// POSSystem.Domain.Entities/GRN.cs

namespace POSSystem.Domain.Entities;

public class GRN
{
    public int GRNId { get; set; }
    public string GRNNumber { get; set; } = string.Empty;
    public int? PurchaseId { get; set; }
    public int SupplierId { get; set; }

    // ✅ ADD THIS PROPERTY
    public string? SupplierName { get; set; }  // From Suppliers table

    public DateTime GRNDate { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Comments { get; set; }
    public DateTime CreationDate { get; set; }
    public List<GRNDetail> Details { get; set; } = new();
}