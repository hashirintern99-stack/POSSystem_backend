using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.Unit;
using POSSystem.DTO.Responses.Unit;
using POSSystem.Repository.UnitDefinition;

namespace POSSystem.Service.UnitDefinition;

public class UnitService : IUnitService
{
    private readonly IUnitRepository _unitRepository;

    public UnitService(IUnitRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }

    public async Task<IEnumerable<UnitResponse>> GetAllAsync()
    {
        var units = await _unitRepository.GetAllAsync();

        return units.Select(MapToResponse);
    }

    public async Task<UnitResponse?> GetByIdAsync(int id)
    {
        var unit = await _unitRepository.GetByIdAsync(id);

        if (unit == null)
            return null;

        return MapToResponse(unit);
    }

    public async Task<int> CreateAsync(CreateUnitRequest request)
    {
        var unit = new Unit
        {
            Name = request.Name,
            ShortName = request.ShortName,
            IsActive = true,
            CreationDate = DateTime.UtcNow
        };

        return await _unitRepository.CreateAsync(unit);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateUnitRequest request)
    {
        var unit = await _unitRepository.GetByIdAsync(id);

        if (unit == null)
            return false;

        unit.Name = request.Name;
        unit.ShortName = request.ShortName;
        unit.IsActive = request.IsActive;

        return await _unitRepository.UpdateAsync(unit);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _unitRepository.DeleteAsync(id);
    }

    private static UnitResponse MapToResponse(Unit unit)
    {
        return new UnitResponse
        {
            UnitId = unit.UnitId,
            Name = unit.Name,
            ShortName = unit.ShortName,
            IsActive = unit.IsActive,
            CreationDate = unit.CreationDate
        };
    }
}