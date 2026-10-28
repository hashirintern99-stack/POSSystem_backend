
namespace POSSystem.Domain.Entities;

public class Sale
{
    public int SaleId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public int? CustomerId { get; set; }

    public int? SalesPersonId { get; set; }
    public string? CustomerName { get; set; }
    public string? SalesPersonName { get; set; }


    public DateTime SaleDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal RemainingAmount { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Comments { get; set; }

    public DateTime CreationDate { get; set; }

    public int? CreatedBy { get; set; }

    public List<SaleDetail> Details { get; set; } = new();
}

