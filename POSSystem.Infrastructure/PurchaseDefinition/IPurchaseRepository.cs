// POSSystem.Repository.PurchaseDefinition/IPurchaseRepository.cs
using POSSystem.Domain.Entities;

namespace POSSystem.Repository.PurchaseDefinition;

public interface IPurchaseRepository
{
    Task<IEnumerable<Purchase>> GetAllAsync();

    Task<Purchase?> GetByIdAsync(int id);

    Task<int> CreateAsync(
        Purchase purchase,
        IEnumerable<PurchaseDetail> details);

    Task<bool> UpdateAsync(
        Purchase purchase,
        IEnumerable<PurchaseDetail> details);

    Task<bool> DeleteAsync(int id);

    // ✅ ADD THIS METHOD
    Task<bool> UpdateStatusAsync(int purchaseId, string status);
}