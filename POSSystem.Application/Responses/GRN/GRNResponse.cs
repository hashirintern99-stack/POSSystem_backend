// POSSystem.DTO.Responses.GRN/GRNResponse.cs

namespace POSSystem.DTO.Responses.GRN;

public class GRNResponse
{
    public int GRNId { get; set; }
    public string GRNNumber { get; set; } = string.Empty;
    public int? PurchaseId { get; set; }
    public int SupplierId { get; set; }

    // ✅ ADD THIS
    public string? SupplierName { get; set; }

    public DateTime GRNDate { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Comments { get; set; }
    public DateTime CreationDate { get; set; }
    public List<GRNDetailResponse> Details { get; set; } = new();
}