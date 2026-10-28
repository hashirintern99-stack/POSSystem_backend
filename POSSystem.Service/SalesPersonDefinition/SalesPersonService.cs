// POSSystem.Service.SalesPersonDefinition/SalesPersonService.cs

using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.SalesPerson;
using POSSystem.DTO.Responses.SalesPerson;
using POSSystem.Repository.SalesPersonDefinition;

namespace POSSystem.Service.SalesPersonDefinition;

public class SalesPersonService : ISalesPersonService
{
    private readonly ISalesPersonRepository _salesPersonRepository;

    public SalesPersonService(ISalesPersonRepository salesPersonRepository)
    {
        _salesPersonRepository = salesPersonRepository;
    }

    public async Task<IEnumerable<SalesPersonResponse>> GetAllAsync()
    {
        var salesPersons = await _salesPersonRepository.GetAllAsync();
        return salesPersons.Select(MapToResponseDto);
    }

    public async Task<SalesPersonResponse?> GetByIdAsync(int id)
    {
        var salesPerson = await _salesPersonRepository.GetByIdAsync(id);
        if (salesPerson == null) return null;
        return MapToResponseDto(salesPerson);
    }

    public async Task<int> CreateAsync(CreateSalesPersonRequest dto)
    {
        var salesPerson = new SalesPerson
        {
            Code = dto.Code,
            Name = dto.Name,
            Phone = dto.Phone,
            Email = dto.Email,
            CommissionPercentage = dto.CommissionPercentage,
            IsActive = true,
            CreationDate = DateTime.UtcNow
        };

        return await _salesPersonRepository.CreateAsync(salesPerson);
    }

    public async Task<bool> UpdateAsync(int id, UpdateSalesPersonRequest dto)
    {
        var existing = await _salesPersonRepository.GetByIdAsync(id);
        if (existing == null || existing.DeletedAt != null) return false;

        var salesPerson = new SalesPerson
        {
            SalesPersonId = existing.SalesPersonId,
            Code = dto.Code,
            Name = dto.Name,
            Phone = dto.Phone,
            Email = dto.Email,
            CommissionPercentage = dto.CommissionPercentage,
            IsActive = dto.IsActive,
            CreationDate = existing.CreationDate,
            UpdatedDate = DateTime.UtcNow
        };

        return await _salesPersonRepository.UpdateAsync(salesPerson);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _salesPersonRepository.GetByIdAsync(id);
        if (existing == null) return false;
        return await _salesPersonRepository.DeleteAsync(id);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        return await _salesPersonRepository.SoftDeleteAsync(id);
    }

    public async Task<bool> RestoreAsync(int id)
    {
        return await _salesPersonRepository.RestoreAsync(id);
    }

    private static SalesPersonResponse MapToResponseDto(SalesPerson salesPerson)
    {
        return new SalesPersonResponse
        {
            SalesPersonId = salesPerson.SalesPersonId,
            Code = salesPerson.Code,
            Name = salesPerson.Name,
            Phone = salesPerson.Phone,
            Email = salesPerson.Email,
            CommissionPercentage = salesPerson.CommissionPercentage,
            IsActive = salesPerson.IsActive,
            DeletedAt = salesPerson.DeletedAt,
            CreationDate = salesPerson.CreationDate,
            UpdatedDate = salesPerson.UpdatedDate
        };
    }
}