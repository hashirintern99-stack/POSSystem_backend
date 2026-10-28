using POSSystem.Domain.Entities;

namespace POSSystem.Repository.UnitDefinition;

public interface IUnitRepository
{
    Task<IEnumerable<Unit>> GetAllAsync();

    Task<Unit?> GetByIdAsync(int id);

    Task<int> CreateAsync(Unit unit);

    Task<bool> UpdateAsync(Unit unit);

    Task<bool> DeleteAsync(int id);
}