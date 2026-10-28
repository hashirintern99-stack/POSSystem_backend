using POSSystem.Domain.Entities;

namespace POSSystem.Repository.CategoryDefinition;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();

    Task<Category?> GetByIdAsync(int id);

    Task<int> CreateAsync(Category category);

    Task<bool> UpdateAsync(Category category);

    Task<bool> DeleteAsync(int id);
}