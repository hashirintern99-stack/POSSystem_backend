using POSSystem.DTO.Requests.Unit;
using POSSystem.DTO.Responses.Unit;

namespace POSSystem.Service.UnitDefinition;

public interface IUnitService
{
    Task<IEnumerable<UnitResponse>> GetAllAsync();

    Task<UnitResponse?> GetByIdAsync(int id);

    Task<int> CreateAsync(CreateUnitRequest request);

    Task<bool> UpdateAsync(int id, UpdateUnitRequest request);

    Task<bool> DeleteAsync(int id);
}