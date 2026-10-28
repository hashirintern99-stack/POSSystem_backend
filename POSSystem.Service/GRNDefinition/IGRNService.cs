using POSSystem.DTO.Requests.GRN;
using POSSystem.DTO.Responses.GRN;

namespace POSSystem.Service.GRNDefinition;

public interface IGRNService
{
    Task<IEnumerable<GRNResponse>> GetAllAsync();

    Task<GRNResponse?> GetByIdAsync(int id);

    Task<int> CreateAsync(CreateGRNRequest request);
    Task<bool> UpdateAsync(
    int id,
    CreateGRNRequest request);
    Task<bool> DeleteAsync(int id);
}