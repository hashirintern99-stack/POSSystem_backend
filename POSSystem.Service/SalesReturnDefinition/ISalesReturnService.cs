using POSSystem.DTO.Requests.SalesReturn;
using POSSystem.DTO.Responses.SalesReturn;

namespace POSSystem.Service.SaleReturnDefinition;

public interface ISalesReturnService
{
    Task<IEnumerable<SalesReturnResponse>> GetAllAsync();
    Task<SalesReturnResponse?> GetByIdAsync(int id);
    Task<IEnumerable<SalesReturnResponse>> GetBySaleIdAsync(int saleId);
    Task<int> CreateAsync(CreateSalesReturnRequest request);
    Task<bool> UpdateAsync(int id, CreateSalesReturnRequest request);
    Task<bool> DeleteAsync(int id);
}