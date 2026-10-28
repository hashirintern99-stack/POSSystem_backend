// POSSystem.Service.SaleDefinition/SaleService.cs

using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.Sale;
using POSSystem.DTO.Responses.Sale;
using POSSystem.DTO.Responses.SalesReturn;
using POSSystem.Repository.SaleDefinition;

namespace POSSystem.Service.SaleDefinition;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;

    public SaleService(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<IEnumerable<SaleResponse>> GetAllAsync()
    {
        var sales = await _saleRepository.GetAllAsync();
        return sales.Select(MapToResponse);
    }

    public async Task<SaleResponse?> GetByIdAsync(int id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        return sale == null ? null : MapToResponse(sale);
    }

    public async Task<int> CreateAsync(CreateSaleRequest request)
    {
        // Validate
        if (request.Details == null || !request.Details.Any())
            throw new ArgumentException("At least one product is required.");

        foreach (var detail in request.Details)
        {
            if (detail.ProductId <= 0)
                throw new ArgumentException($"Invalid ProductId: {detail.ProductId}");
            if (detail.Quantity <= 0)
                throw new ArgumentException($"Quantity must be greater than 0 for ProductId: {detail.ProductId}");
        }

        var sale = new Sale
        {
            InvoiceNumber = request.InvoiceNumber,
            CustomerId = request.CustomerId,
            SalesPersonId = request.SalesPersonId,
            SaleDate = request.SaleDate,
            SubTotal = request.SubTotal,
            Discount = request.Discount,
            Tax = request.Tax,
            Total = request.Total,
            PaidAmount = request.PaidAmount,
            RemainingAmount = request.RemainingAmount,
            PaymentMethod = request.PaymentMethod,
            Status = request.Status,
            Comments = request.Comments,
            CreatedBy = request.CreatedBy,
            CreationDate = DateTime.UtcNow
        };

        var details = request.Details.Select(d => new SaleDetail
        {
            ProductId = d.ProductId,
            ProductName = d.ProductName,
            ProductCode = d.ProductCode,
            Quantity = d.Quantity,
            RetailPrice = d.RetailPrice,
            Discount = d.Discount,
            Tax = d.Tax,
            Total = d.Total
        }).ToList();

        return await _saleRepository.CreateAsync(sale, details);
    }

    public async Task<bool> UpdateAsync(int id, CreateSaleRequest request)
    {
        var existing = await _saleRepository.GetByIdAsync(id);
        if (existing == null)
            throw new ArgumentException($"Sale with ID {id} does not exist.");

        var sale = new Sale
        {
            SaleId = id,
            InvoiceNumber = request.InvoiceNumber,
            CustomerId = request.CustomerId,
            SalesPersonId = request.SalesPersonId,
            SaleDate = request.SaleDate,
            SubTotal = request.SubTotal,
            Discount = request.Discount,
            Tax = request.Tax,
            Total = request.Total,
            PaidAmount = request.PaidAmount,
            RemainingAmount = request.RemainingAmount,
            PaymentMethod = request.PaymentMethod,
            Status = request.Status,
            Comments = request.Comments
        };

        var details = request.Details.Select(d => new SaleDetail
        {
            SaleDetailId = d.SaleDetailId,
            ProductId = d.ProductId,
            ProductName = d.ProductName,
            ProductCode = d.ProductCode,
            Quantity = d.Quantity,
            RetailPrice = d.RetailPrice,
            Discount = d.Discount,
            Tax = d.Tax,
            Total = d.Total,
            RowState = d.RowState
        }).ToList();

        return await _saleRepository.UpdateAsync(sale, details);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _saleRepository.GetByIdAsync(id);
        if (existing == null)
            throw new ArgumentException($"Sale with ID {id} does not exist.");

        // ✅ Prevent deleting completed sales
        if (existing.Status?.ToLower() == "completed")
        {
            throw new InvalidOperationException("Cannot delete a completed sale.");
        }

        // ✅ Check if sale has returns
        var hasReturns = await _saleRepository.HasReturnsAsync(id);
        if (hasReturns)
        {
            throw new InvalidOperationException("Cannot delete a sale that has returns.");
        }

        return await _saleRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<SearchSaleResponse>> SearchAsync(SearchSaleRequest request)
    {
        return await _saleRepository.SearchAsync(request);
    }

    public async Task<SaleFullDetailResponse?> GetSaleDetailsForSearchAsync(int saleId)
    {
        return await _saleRepository.GetSaleDetailsForSearchAsync(saleId);
    }

    public async Task<IEnumerable<SalesReturnResponse>> GetReturnsForSaleAsync(int saleId)
    {
        return await _saleRepository.GetReturnsForSaleAsync(saleId);
    }

    private static SaleResponse MapToResponse(Sale sale)
    {
        return new SaleResponse
        {
            SaleId = sale.SaleId,
            InvoiceNumber = sale.InvoiceNumber,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,  // ✅ Add this
            SalesPersonId = sale.SalesPersonId,
            SalesPersonName = sale.SalesPersonName,  // ✅ Add this
            SaleDate = sale.SaleDate,
            SubTotal = sale.SubTotal,
            Discount = sale.Discount,
            Tax = sale.Tax,
            Total = sale.Total,
            PaidAmount = sale.PaidAmount,
            RemainingAmount = sale.RemainingAmount,
            PaymentMethod = sale.PaymentMethod,
            Status = sale.Status,
            Comments = sale.Comments,
            CreationDate = sale.CreationDate,
            CreatedBy = sale.CreatedBy,
            Details = sale.Details.Select(d => new SaleDetailResponse
            {
                SaleDetailId = d.SaleDetailId,
                ProductId = d.ProductId,
                ProductName = d.ProductName,
                ProductCode = d.ProductCode,
                Quantity = d.Quantity,
                RetailPrice = d.RetailPrice,
                Discount = d.Discount,
                Tax = d.Tax,
                Total = d.Total
            }).ToList()
        };
    }
}