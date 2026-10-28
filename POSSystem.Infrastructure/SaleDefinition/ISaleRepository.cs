using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.Sale;
using POSSystem.DTO.Responses.Sale;
using POSSystem.DTO.Responses.SalesReturn;

namespace POSSystem.Repository.SaleDefinition;

public interface ISaleRepository
{
    // Existing CRUD
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<Sale?> GetByIdAsync(int id);
    Task<int> CreateAsync(Sale sale, IEnumerable<SaleDetail> details);
    Task<bool> UpdateAsync(Sale sale, IEnumerable<SaleDetail> details);
    Task<bool> DeleteAsync(int id);

    // NEW: Search Methods
    Task<IEnumerable<SearchSaleResponse>> SearchAsync(SearchSaleRequest request);
    Task<SaleFullDetailResponse?> GetSaleDetailsForSearchAsync(int saleId);
    Task<IEnumerable<SalesReturnResponse>> GetReturnsForSaleAsync(int saleId);
    Task<bool> HasReturnsAsync(int id);
}