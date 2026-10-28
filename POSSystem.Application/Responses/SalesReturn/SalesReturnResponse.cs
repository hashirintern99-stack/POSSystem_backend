// POSSystem.DTO.Responses.SalesReturn/SalesReturnResponse.cs

namespace POSSystem.DTO.Responses.SalesReturn;

public class SalesReturnResponse
{
    public int SalesReturnId { get; set; }

    public string ReturnNumber { get; set; } = string.Empty;

    // ✅ CHANGE THIS - Make SaleId nullable
    public int? SaleId { get; set; }  // Changed from int to int?

    public int? CustomerId { get; set; }

    public DateTime ReturnDate { get; set; }

    public decimal Total { get; set; }

    public string? Comments { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreationDate { get; set; }

    // ✅ ADD THESE
    public bool IsManualReturn { get; set; }
    public string? OriginalInvoiceNumber { get; set; }
    public string? ReturnType { get; set; }
    public int? ExchangeSaleId { get; set; }
    public string? ReturnReason { get; set; }

    public List<SalesReturnDetailResponse> Details { get; set; } = new();
}