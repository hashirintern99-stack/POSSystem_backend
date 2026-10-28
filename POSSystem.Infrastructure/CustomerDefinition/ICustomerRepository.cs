using POSSystem.Domain.Entities;

namespace POSSystem.Repository.CustomerDefinition;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();

    Task<Customer?> GetByIdAsync(int id);

    Task<int> CreateAsync(Customer customer);

    Task<bool> UpdateAsync(Customer customer);

    Task<bool> DeleteAsync(int id);

    //methods for softdelete
    Task<bool> HasRelatedRecords(int customerId);
    Task<bool> SoftDeleteAsync(int id);
}