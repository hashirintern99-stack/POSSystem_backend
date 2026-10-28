using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.Category;
using POSSystem.DTO.Responses.Category;
using POSSystem.Repository.CategoryDefinition;

namespace POSSystem.Service.CategoryDefinition;

public class CategoryService : ICategoryService
{
    //Injecting Repository
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    //Get all categories
    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return categories.Select(MapToResponse);
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return null;

        return MapToResponse(category);
    }

    public async Task<int> CreateAsync(CreateCategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name,
            IsActive = true,
            CreationDate = DateTime.UtcNow
        };

        return await _categoryRepository.CreateAsync(category);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateCategoryRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return false;

        category.Name = request.Name;
        category.IsActive = request.IsActive;

        return await _categoryRepository.UpdateAsync(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _categoryRepository.DeleteAsync(id);
    }

    private static CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            IsActive = category.IsActive,
            CreationDate = category.CreationDate
        };
    }
}