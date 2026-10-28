using POSSystem.DTO.Requests.Customer;
using POSSystem.DTO.Responses.Customer;

namespace POSSystem.Service.CustomerDefinition;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponse>> GetAllAsync();

    Task<CustomerResponse?> GetByIdAsync(int id);

    Task<int> CreateAsync(CreateCustomerRequest dto);

    Task<bool> UpdateAsync(int id, UpdateCustomerRequest dto);

    Task<bool> DeleteAsync(int id);

    //explicit for softdelete
    Task<bool> SoftDeleteAsync(int id);
}