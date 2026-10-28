using POSSystem.Domain.Entities;
using POSSystem.DTO.Responses.Product;

namespace POSSystem.Repository.ProductDefinition;  

public interface IProductRepository
{
    Task<IEnumerable<ProductWithDetails>> GetAllAsync();
    Task<ProductWithDetails?> GetByIdAsync(int id);
    Task<int> CreateAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);

    //methods for softdelete
    Task<bool> HasRelatedRecords(int productId);
    Task<bool> SoftDeleteAsync(int id);

    //lowstock methods 
    Task<IEnumerable<LowStockProductDto>> GetLowStockProductsAsync();
    Task<int> GetLowStockCountAsync();
}