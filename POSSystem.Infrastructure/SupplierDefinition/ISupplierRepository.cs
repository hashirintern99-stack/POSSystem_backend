using POSSystem.Domain.Entities;

namespace POSSystem.Repository.SupplierDefinition;

public interface ISupplierRepository
{
    Task<IEnumerable<Supplier>> GetAllAsync();

    Task<Supplier?> GetByIdAsync(int id);

    Task<int> CreateAsync(Supplier supplier);

    Task<bool> UpdateAsync(Supplier supplier);

    Task<bool> DeleteAsync(int id);

    //methods for softdelete
    Task<bool> HasRelatedRecords(int supplierId);
    Task<bool> SoftDeleteAsync(int id);
}