// POSSystem.Service.SalesPersonDefinition/ISalesPersonService.cs

using POSSystem.DTO.Requests.SalesPerson;
using POSSystem.DTO.Responses.SalesPerson;

namespace POSSystem.Service.SalesPersonDefinition;

public interface ISalesPersonService
{
    Task<IEnumerable<SalesPersonResponse>> GetAllAsync();

    Task<SalesPersonResponse?> GetByIdAsync(int id);

    Task<int> CreateAsync(CreateSalesPersonRequest dto);

    Task<bool> UpdateAsync(int id, UpdateSalesPersonRequest dto);

    Task<bool> DeleteAsync(int id);

    // ✅ Explicit soft delete and restore
    Task<bool> SoftDeleteAsync(int id);
    Task<bool> RestoreAsync(int id);
}