using POSSystem.DTO.Requests.Category;
using POSSystem.DTO.Responses.Category;

namespace POSSystem.Service.CategoryDefinition;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync();

    Task<CategoryResponse?> GetByIdAsync(int id);

    Task<int> CreateAsync(CreateCategoryRequest request);

    Task<bool> UpdateAsync(int id, UpdateCategoryRequest request);

    Task<bool> DeleteAsync(int id);
}