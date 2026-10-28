using POSSystem.DTO.Requests.Sale;
using POSSystem.DTO.Responses.Sale;
using POSSystem.DTO.Responses.SalesReturn;

namespace POSSystem.Service.SaleDefinition;

public interface ISaleService
{
    // Existing CRUD
    Task<IEnumerable<SaleResponse>> GetAllAsync();
    Task<SaleResponse?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateSaleRequest request);
    Task<bool> UpdateAsync(int id, CreateSaleRequest request);
    Task<bool> DeleteAsync(int id);

    // NEW: Search Methods
    Task<IEnumerable<SearchSaleResponse>> SearchAsync(SearchSaleRequest request);
    Task<SaleFullDetailResponse?> GetSaleDetailsForSearchAsync(int saleId);
}