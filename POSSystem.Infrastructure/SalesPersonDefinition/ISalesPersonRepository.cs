// POSSystem.Repository.SalesPersonDefinition/ISalesPersonRepository.cs

using POSSystem.Domain.Entities;

namespace POSSystem.Repository.SalesPersonDefinition;

public interface ISalesPersonRepository
{
    Task<IEnumerable<SalesPerson>> GetAllAsync();

    Task<SalesPerson?> GetByIdAsync(int id);

    Task<int> CreateAsync(SalesPerson salesPerson);

    Task<bool> UpdateAsync(SalesPerson salesPerson);

    Task<bool> DeleteAsync(int id);

    // ✅ Methods for soft delete using DeletedAt
    Task<bool> HasRelatedRecords(int salesPersonId);
    Task<bool> SoftDeleteAsync(int id);
    Task<bool> RestoreAsync(int id);  // Optional: Restore soft-deleted record
}