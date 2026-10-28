using POSSystem.DTO.Requests.Product;
using POSSystem.DTO.Responses.Product;
using System.Threading.Tasks;

namespace POSSystem.Service.ProductDefinition;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllAsync();
    Task<ProductResponse?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateProductRequest dto);
    Task<bool> UpdateAsync(int id, UpdateProductRequest dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> SoftDeleteAsync(int id);

    // Low stock
    Task<IEnumerable<LowStockProductDto>> GetLowStockProductsAsync();
    Task<int> GetLowStockCountAsync();

    // ✅ NEW — for the "New Product" form to preview a code
    Task<string> PeekNextCodeAsync(int categoryId);
}