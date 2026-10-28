using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.SalesReturn;
using POSSystem.DTO.Responses.SalesReturn;
using POSSystem.Repository.SalesReturnDefinition;

namespace POSSystem.Service.SaleReturnDefinition;

public class SalesReturnService : ISalesReturnService
{
    private readonly ISalesReturnRepository _salesReturnRepository;

    public SalesReturnService(ISalesReturnRepository salesReturnRepository)
    {
        _salesReturnRepository = salesReturnRepository;
    }

    public async Task<IEnumerable<SalesReturnResponse>> GetAllAsync()
    {
        var returns = await _salesReturnRepository.GetAllAsync();
        return returns.Select(MapToResponse);
    }

    public async Task<SalesReturnResponse?> GetByIdAsync(int id)
    {
        var salesReturn = await _salesReturnRepository.GetByIdAsync(id);
        return salesReturn == null ? null : MapToResponse(salesReturn);
    }

    public async Task<IEnumerable<SalesReturnResponse>> GetBySaleIdAsync(int saleId)
    {
        var returns = await _salesReturnRepository.GetBySaleIdAsync(saleId);
        return returns.Select(MapToResponse);
    }

    public async Task<int> CreateAsync(CreateSalesReturnRequest request)
    {
        // Validate
        if (request.Details == null || !request.Details.Any())
            throw new ArgumentException("At least one product is required for return.");

        foreach (var detail in request.Details)
        {
            if (detail.ProductId <= 0)
                throw new ArgumentException($"Invalid ProductId: {detail.ProductId}");
            if (detail.Quantity <= 0)
                throw new ArgumentException($"Quantity must be greater than 0 for ProductId: {detail.ProductId}");
        }

        var salesReturn = new SalesReturn
        {
            ReturnNumber = request.ReturnNumber,
            SaleId = request.SaleId,  // ✅ Now accepts null
            CustomerId = request.CustomerId,
            ReturnDate = request.ReturnDate,
            Total = request.Total,
            Comments = request.Comments,
            CreatedBy = request.CreatedBy,
            CreationDate = DateTime.UtcNow,
            // ✅ ADD THESE NEW FIELDS
            IsManualReturn = request.IsManualReturn,
            OriginalInvoiceNumber = request.OriginalInvoiceNumber,
            ReturnType = request.ReturnType ?? "Return",
            ExchangeSaleId = request.ExchangeSaleId,
            ReturnReason = request.ReturnReason
        };

        var details = request.Details.Select(d => new SalesReturnDetail
        {
            ProductId = d.ProductId,
            Quantity = d.Quantity,
            RetailPrice = d.RetailPrice,
            Discount = d.Discount,
            Tax = d.Tax,
            Total = d.Total,
            Reason = d.Reason
        }).ToList();

        return await _salesReturnRepository.CreateAsync(salesReturn, details);
    }

    public async Task<bool> UpdateAsync(int id, CreateSalesReturnRequest request)
    {
        var existing = await _salesReturnRepository.GetByIdAsync(id);
        if (existing == null)
            throw new ArgumentException($"Sales Return with ID {id} does not exist.");

        var salesReturn = new SalesReturn
        {
            SalesReturnId = id,
            ReturnNumber = request.ReturnNumber,
            SaleId = request.SaleId,  // ✅ Now accepts null
            CustomerId = request.CustomerId,
            ReturnDate = request.ReturnDate,
            Total = request.Total,
            Comments = request.Comments,
            // ✅ ADD THESE NEW FIELDS
            IsManualReturn = request.IsManualReturn,
            OriginalInvoiceNumber = request.OriginalInvoiceNumber,
            ReturnType = request.ReturnType ?? "Return",
            ExchangeSaleId = request.ExchangeSaleId,
            ReturnReason = request.ReturnReason
        };

        var details = request.Details.Select(d => new SalesReturnDetail
        {
            SalesReturnDetailId = d.SalesReturnDetailId,
            ProductId = d.ProductId,
            Quantity = d.Quantity,
            RetailPrice = d.RetailPrice,
            Discount = d.Discount,
            Tax = d.Tax,
            Total = d.Total,
            Reason = d.Reason,
            RowState = d.RowState
        }).ToList();

        return await _salesReturnRepository.UpdateAsync(salesReturn, details);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _salesReturnRepository.GetByIdAsync(id);
        if (existing == null)
            throw new ArgumentException($"Sales Return with ID {id} does not exist.");

        return await _salesReturnRepository.DeleteAsync(id);
    }

    private static SalesReturnResponse MapToResponse(SalesReturn salesReturn)
    {
        return new SalesReturnResponse
        {
            SalesReturnId = salesReturn.SalesReturnId,
            ReturnNumber = salesReturn.ReturnNumber,
            SaleId = salesReturn.SaleId,  // ✅ Now accepts null
            CustomerId = salesReturn.CustomerId,
            ReturnDate = salesReturn.ReturnDate,
            Total = salesReturn.Total,
            Comments = salesReturn.Comments,
            CreatedBy = salesReturn.CreatedBy,
            CreationDate = salesReturn.CreationDate,
            // ✅ ADD THESE NEW FIELDS TO RESPONSE
            IsManualReturn = salesReturn.IsManualReturn,
            OriginalInvoiceNumber = salesReturn.OriginalInvoiceNumber,
            ReturnType = salesReturn.ReturnType,
            ExchangeSaleId = salesReturn.ExchangeSaleId,
            ReturnReason = salesReturn.ReturnReason,
            Details = salesReturn.Details.Select(d => new SalesReturnDetailResponse
            {
                SalesReturnDetailId = d.SalesReturnDetailId,
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                RetailPrice = d.RetailPrice,
                Discount = d.Discount,
                Tax = d.Tax,
                Total = d.Total,
                Reason = d.Reason
            }).ToList()
        };
    }
}