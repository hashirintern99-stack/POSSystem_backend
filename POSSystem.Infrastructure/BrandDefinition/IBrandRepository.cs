using POSSystem.Domain.Entities;

namespace POSSystem.Repository.BrandDefinition;

public interface IBrandRepository
{
    Task<IEnumerable<Brand>> GetAllAsync();

    Task<Brand?> GetByIdAsync(int id);

    Task<int> CreateAsync(Brand brand);

    Task<bool> UpdateAsync(Brand brand);

    Task<bool> DeleteAsync(int id);
}