using POSSystem.Domain.Entities;

namespace POSSystem.Repository.SalesReturnDefinition;

public interface ISalesReturnRepository
{
    Task<IEnumerable<SalesReturn>> GetAllAsync();
    Task<SalesReturn?> GetByIdAsync(int id);
    Task<IEnumerable<SalesReturn>> GetBySaleIdAsync(int saleId);
    Task<int> CreateAsync(SalesReturn salesReturn, IEnumerable<SalesReturnDetail> details);
    Task<bool> UpdateAsync(SalesReturn salesReturn, IEnumerable<SalesReturnDetail> details);
    Task<bool> DeleteAsync(int id);
}



