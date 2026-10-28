using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.Purchase;
using POSSystem.DTO.Responses.Purchase;
using POSSystem.Repository.PurchaseDefinition;

namespace POSSystem.Service.PurchaseDefinition;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;

    public PurchaseService(
        IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }


    // =========================
    // GET ALL PURCHASES
    // =========================

    public async Task<IEnumerable<PurchaseResponse>> GetAllAsync()
    {
        var purchases =
            await _purchaseRepository.GetAllAsync();

        return purchases.Select(MapToResponse);
    }


    // =========================
    // GET PURCHASE BY ID
    // =========================

    public async Task<PurchaseResponse?> GetByIdAsync(int id)
    {
        var purchase =
            await _purchaseRepository.GetByIdAsync(id);

        if (purchase == null)
            return null;

        return MapToResponse(purchase);
    }


    // =========================
    // CREATE PURCHASE
    // =========================

    public async Task<int> CreateAsync(
        CreatePurchaseRequest request)
    {
        var purchase = new Purchase
        {
            PurchaseNumber = request.PurchaseNumber,

            SupplierId = request.SupplierId,

            PurchaseDate = request.PurchaseDate,

            ExpectedDeliveryDate =
                request.ExpectedDeliveryDate,

            Status = request.Status,

            SubTotal = request.SubTotal,

            Discount = request.Discount,

            Tax = request.Tax,

            Total = request.Total,

            Comments = request.Comments,

            CreatedBy = request.CreatedBy,

            CreationDate = DateTime.UtcNow
        };


        var details = request.Details
            .Select(detail => new PurchaseDetail
            {
                PurchaseDetailId = detail.PurchaseDetailId,

                ProductId = detail.ProductId,

                Quantity = detail.Quantity,

                CostPrice = detail.CostPrice,

                Discount = detail.Discount,

                Tax = detail.Tax,

                Total = detail.Total,

                RowState = detail.RowState
            })
            .ToList();


        return await _purchaseRepository.CreateAsync(
            purchase,
            details);
    }


    // =========================
    // UPDATE PURCHASE
    // =========================

    public async Task<bool> UpdateAsync(
        int id,
        CreatePurchaseRequest request)
    {
        var purchase = new Purchase
        {
            PurchaseId = id,

            PurchaseNumber = request.PurchaseNumber,

            SupplierId = request.SupplierId,

            PurchaseDate = request.PurchaseDate,

            ExpectedDeliveryDate =
                request.ExpectedDeliveryDate,

            Status = request.Status,

            SubTotal = request.SubTotal,

            Discount = request.Discount,

            Tax = request.Tax,

            Total = request.Total,

            Comments = request.Comments,

            UpdatedDate = DateTime.UtcNow
        };


        var details = request.Details
     .Select(detail => new PurchaseDetail
     {
         PurchaseDetailId = detail.PurchaseDetailId,

         ProductId = detail.ProductId,

         Quantity = detail.Quantity,

         CostPrice = detail.CostPrice,

         Discount = detail.Discount,

         Tax = detail.Tax,

         Total = detail.Total,

         RowState = detail.RowState
     })
     .ToList();


        return await _purchaseRepository.UpdateAsync(
            purchase,
            details);
    }


    // =========================
    // DELETE PURCHASE
    // =========================

    public async Task<bool> DeleteAsync(int id)
    {
        return await _purchaseRepository.DeleteAsync(id);
    }


    // =========================
    // ENTITY → RESPONSE DTO
    // =========================

    private static PurchaseResponse MapToResponse(Purchase purchase)
    {
        return new PurchaseResponse
        {
            PurchaseId = purchase.PurchaseId,
            PurchaseNumber = purchase.PurchaseNumber,
            SupplierId = purchase.SupplierId,
            SupplierName = purchase.SupplierName,  // ✅ ADD THIS
            PurchaseDate = purchase.PurchaseDate,
            ExpectedDeliveryDate = purchase.ExpectedDeliveryDate,
            Status = purchase.Status,
            SubTotal = purchase.SubTotal,
            Discount = purchase.Discount,
            Tax = purchase.Tax,
            Total = purchase.Total,
            Comments = purchase.Comments,
            CreatedBy = purchase.CreatedBy,
            CreationDate = purchase.CreationDate,
            UpdatedDate = purchase.UpdatedDate,
            Details = purchase.Details.Select(detail =>
                new PurchaseDetailResponse
                {
                    PurchaseDetailId = detail.PurchaseDetailId,
                    ProductId = detail.ProductId,
                    ProductName = detail.ProductName,  // ✅ ADD THIS
                    Quantity = detail.Quantity,
                    CostPrice = detail.CostPrice,
                    Discount = detail.Discount,
                    Tax = detail.Tax,
                    Total = detail.Total
                }).ToList()
        };
    }
}