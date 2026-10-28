namespace POSSystem.DTO.Responses.Sale;

public class SearchSaleResponse
{
    public int SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public int? SalesPersonId { get; set; }
    public string? SalesPersonName { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public DateTime? CreatedDate { get; set; }
}