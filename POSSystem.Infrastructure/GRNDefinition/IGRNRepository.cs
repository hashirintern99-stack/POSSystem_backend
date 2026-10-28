using POSSystem.Domain.Entities;

namespace POSSystem.Repository.GRNDefinition;

public interface IGRNRepository
{
    Task<IEnumerable<GRN>> GetAllAsync();

    Task<GRN?> GetByIdAsync(int id);

    Task<int> CreateAsync(
        GRN grn,
        IEnumerable<GRNDetail> details);

    Task<bool> UpdateAsync(
        GRN grn,
        IEnumerable<GRNDetail> details);
    Task<bool> DeleteAsync(int id);
}