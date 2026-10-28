using System;
using System.Collections.Generic;

namespace POSSystem.DTO.Requests.Purchase;

public class CreatePurchaseRequest
{
    public string PurchaseNumber { get; set; } = string.Empty;

    public int SupplierId { get; set; }

    public DateTime PurchaseDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal SubTotal { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }

    public string? Comments { get; set; }

    public int? CreatedBy { get; set; }

    public List<CreatePurchaseDetailRequest> Details { get; set; } = new();
}