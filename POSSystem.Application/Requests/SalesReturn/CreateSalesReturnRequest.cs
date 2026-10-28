// POSSystem.DTO.Requests.SalesReturn/CreateSalesReturnRequest.cs

namespace POSSystem.DTO.Requests.SalesReturn;

public class CreateSalesReturnRequest
{
    public string ReturnNumber { get; set; } = string.Empty;

    // ✅ CHANGE THIS - Make SaleId nullable
    public int? SaleId { get; set; }  // Changed from int to int?

    public int? CustomerId { get; set; }

    public DateTime ReturnDate { get; set; }

    public decimal Total { get; set; }

    public string? Comments { get; set; }

    public int? CreatedBy { get; set; }

    // ✅ ADD THESE for manual return support
    public bool IsManualReturn { get; set; } = false;
    public string? OriginalInvoiceNumber { get; set; }
    public string? ReturnType { get; set; }
    public int? ExchangeSaleId { get; set; }
    public string? ReturnReason { get; set; }

    public List<CreateSalesReturnDetailRequest> Details { get; set; } = new();
}