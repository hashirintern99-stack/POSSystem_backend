namespace POSSystem.DTO.Requests.GRN;

public class CreateGRNRequest
{
    public string GRNNumber { get; set; } = string.Empty;

    public int? PurchaseId { get; set; }

    public int SupplierId { get; set; }

    public DateTime GRNDate { get; set; }
    public string? ReceivedBy { get; set; }


    public string? Comments { get; set; }

    public List<CreateGRNDetailRequest> Details { get; set; } = new();
}