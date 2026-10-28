// POSSystem.Service.GRNDefinition/GRNService.cs
using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.GRN;
using POSSystem.DTO.Responses.GRN;
using POSSystem.Repository.GRNDefinition;
using POSSystem.Repository.PurchaseDefinition;  // ✅ ADD THIS

namespace POSSystem.Service.GRNDefinition;

public class GRNService : IGRNService
{
    private readonly IGRNRepository _grnRepository;
    private readonly IPurchaseRepository _purchaseRepository;  // ✅ ADD THIS

    public GRNService(
        IGRNRepository grnRepository,
        IPurchaseRepository purchaseRepository)  // ✅ ADD THIS
    {
        _grnRepository = grnRepository;
        _purchaseRepository = purchaseRepository;  // ✅ ADD THIS
    }

    public async Task<IEnumerable<GRNResponse>> GetAllAsync()
    {
        var grns = await _grnRepository.GetAllAsync();
        return grns.Select(MapToResponse);
    }

    public async Task<GRNResponse?> GetByIdAsync(int id)
    {
        var grn = await _grnRepository.GetByIdAsync(id);
        if (grn == null)
            return null;
        return MapToResponse(grn);
    }

    public async Task<int> CreateAsync(CreateGRNRequest request)
    {
        var grn = new GRN
        {
            GRNNumber = request.GRNNumber,
            PurchaseId = request.PurchaseId,
            SupplierId = request.SupplierId,
            GRNDate = request.GRNDate,
            ReceivedBy = request.ReceivedBy,
            Comments = request.Comments,
            CreationDate = DateTime.UtcNow
        };

        var details = request.Details
            .Select(detail => new GRNDetail
            {
                ProductId = detail.ProductId,
                OrderedQuantity = detail.OrderedQuantity,
                ReceivedQuantity = detail.ReceivedQuantity,
                DamagedQuantity = detail.DamagedQuantity,
                AcceptedQuantity = detail.AcceptedQuantity,
                CostPrice = detail.CostPrice,
                Total = detail.Total
            })
            .ToList();

        // Create GRN
        var grnId = await _grnRepository.CreateAsync(grn, details);

        // ✅ If this GRN is against a Purchase Order, update PO status to "Received"
        if (request.PurchaseId.HasValue && request.PurchaseId.Value > 0)
        {
            try
            {
                await _purchaseRepository.UpdateStatusAsync(
                    request.PurchaseId.Value,
                    "Received"
                );
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the GRN creation
                Console.WriteLine($"Failed to update PO status: {ex.Message}");
                // Optionally: throw or handle as needed
            }
        }

        return grnId;
    }

    public async Task<bool> UpdateAsync(int id, CreateGRNRequest request)
    {
        var grn = new GRN
        {
            GRNId = id,
            GRNNumber = request.GRNNumber,
            PurchaseId = request.PurchaseId,
            SupplierId = request.SupplierId,
            GRNDate = request.GRNDate,
            ReceivedBy = request.ReceivedBy,
            Comments = request.Comments
        };

        var details = request.Details
            .Select(detail => new GRNDetail
            {
                GRNDetailId = detail.GRNDetailId,
                ProductId = detail.ProductId,
                OrderedQuantity = detail.OrderedQuantity,
                ReceivedQuantity = detail.ReceivedQuantity,
                DamagedQuantity = detail.DamagedQuantity,
                AcceptedQuantity = detail.AcceptedQuantity,
                CostPrice = detail.CostPrice,
                Total = detail.Total,
                RowState = detail.RowState
            })
            .ToList();

        var result = await _grnRepository.UpdateAsync(grn, details);

        // ✅ If this GRN is against a Purchase Order, update PO status to "Received"
        if (request.PurchaseId.HasValue && request.PurchaseId.Value > 0)
        {
            try
            {
                await _purchaseRepository.UpdateStatusAsync(
                    request.PurchaseId.Value,
                    "Received"
                );
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the GRN update
                Console.WriteLine($"Failed to update PO status: {ex.Message}");
            }
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _grnRepository.DeleteAsync(id);
    }

    private static GRNResponse MapToResponse(GRN grn)
    {
        return new GRNResponse
        {
            GRNId = grn.GRNId,
            GRNNumber = grn.GRNNumber,
            PurchaseId = grn.PurchaseId,
            SupplierId = grn.SupplierId,
            SupplierName = grn.SupplierName,
            GRNDate = grn.GRNDate,
            ReceivedBy = grn.ReceivedBy,
            Comments = grn.Comments,
            CreationDate = grn.CreationDate,
            Details = grn.Details.Select(detail => new GRNDetailResponse
            {
                GRNDetailId = detail.GRNDetailId,
                ProductId = detail.ProductId,
                OrderedQuantity = detail.OrderedQuantity,
                ReceivedQuantity = detail.ReceivedQuantity,
                DamagedQuantity = detail.DamagedQuantity,
                AcceptedQuantity = detail.AcceptedQuantity,
                CostPrice = detail.CostPrice,
                Total = detail.Total
            }).ToList()
        };
    }
}