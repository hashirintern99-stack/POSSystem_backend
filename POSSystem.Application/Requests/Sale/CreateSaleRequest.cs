
namespace POSSystem.DTO.Requests.Sale;

public class CreateSaleRequest
{
    public string InvoiceNumber { get; set; } = string.Empty;

    public int? CustomerId { get; set; }

    public int? SalesPersonId { get; set; }

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

    public int? CreatedBy { get; set; }

    public List<CreateSaleDetailRequest> Details { get; set; } = new();
}


public class CreateSaleDetailRequest
{
    public int SaleDetailId { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal RetailPrice { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }

    public string RowState { get; set; } = "Original";
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
}

