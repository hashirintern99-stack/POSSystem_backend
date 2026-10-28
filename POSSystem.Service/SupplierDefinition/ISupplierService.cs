using POSSystem.DTO.Requests.Supplier;
using POSSystem.DTO.Responses.Supplier;

namespace POSSystem.Service.SupplierDefinition;

public interface ISupplierService
{
    Task<IEnumerable<SupplierResponse>> GetAllAsync();

    Task<SupplierResponse?> GetByIdAsync(int id);

    Task<int> CreateAsync(CreateSupplierRequest dto);

    Task<bool> UpdateAsync(int id, UpdateSupplierRequest dto);

    Task<bool> DeleteAsync(int id);

    //explicit soft delete
    Task<bool> SoftDeleteAsync(int id);
    // ✅ NEW — for the "New Supplier" form to preview a code
    Task<string> PeekNextCodeAsync();

}