using POSSystem.DTO.Requests.Brand;
using POSSystem.DTO.Responses.Brand;

namespace POSSystem.Service.BrandDefinition;

public interface IBrandService
{
    Task<IEnumerable<BrandResponse>> GetAllAsync();

    Task<BrandResponse?> GetByIdAsync(int id);

    Task<int> CreateAsync(CreateBrandRequest request);

    Task<bool> UpdateAsync(int id, UpdateBrandRequest request);

    Task<bool> DeleteAsync(int id);
}