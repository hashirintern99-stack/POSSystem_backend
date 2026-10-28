namespace POSSystem.DTO.Requests.Sale;

public class SearchSaleRequest
{
    public string? InvoiceNumber { get; set; }
    public int? CustomerId { get; set; }
    public int? SalesPersonId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal? MinTotal { get; set; }
    public decimal? MaxTotal { get; set; }
}