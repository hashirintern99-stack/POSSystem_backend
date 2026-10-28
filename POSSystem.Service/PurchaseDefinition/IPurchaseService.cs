using POSSystem.DTO.Requests.Purchase;
using POSSystem.DTO.Responses.Purchase;

namespace POSSystem.Service.PurchaseDefinition;

public interface IPurchaseService
{
    Task<IEnumerable<PurchaseResponse>> GetAllAsync();

    Task<PurchaseResponse?> GetByIdAsync(int id);

    Task<int> CreateAsync(CreatePurchaseRequest request);

    Task<bool> UpdateAsync(
        int id,
        CreatePurchaseRequest request);

    Task<bool> DeleteAsync(int id);
}