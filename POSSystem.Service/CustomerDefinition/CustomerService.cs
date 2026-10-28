// POSSystem.Service.CustomerDefinition/CustomerService.cs

using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.Customer;
using POSSystem.DTO.Responses.Customer;
using POSSystem.Repository.CustomerDefinition;

namespace POSSystem.Service.CustomerDefinition;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(MapToResponseDto);
    }

    public async Task<CustomerResponse?> GetByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) return null;
        return MapToResponseDto(customer);
    }

    public async Task<int> CreateAsync(CreateCustomerRequest dto)
    {
        var customer = new Customer
        {
            Code = dto.Code,
            Name = dto.Name,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            City = dto.City,
            TaxNumber = dto.TaxNumber,
            OpeningBalance = dto.OpeningBalance,
            IsActive = true,
            CreationDate = DateTime.UtcNow
        };

        return await _customerRepository.CreateAsync(customer);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCustomerRequest dto)
    {
        var existingCustomer = await _customerRepository.GetByIdAsync(id);
        if (existingCustomer == null) return false;

        var customer = new Customer
        {
            CustomerId = existingCustomer.CustomerId,
            Code = dto.Code,
            Name = dto.Name,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            City = dto.City,
            TaxNumber = dto.TaxNumber,
            OpeningBalance = dto.OpeningBalance,
            IsActive = dto.IsActive,
            CreationDate = existingCustomer.CreationDate,
            UpdatedDate = DateTime.UtcNow
        };

        return await _customerRepository.UpdateAsync(customer);
    }

    // ✅ UPDATED DeleteAsync with soft delete logic
    public async Task<bool> DeleteAsync(int id)
    {
        // ✅ Check if customer exists
        var existingCustomer = await _customerRepository.GetByIdAsync(id);
        if (existingCustomer == null) return false;

        // ✅ Let the repository handle the soft/hard delete logic
        return await _customerRepository.DeleteAsync(id);
    }

    // ✅ OPTIONAL: Explicit soft delete method
    public async Task<bool> SoftDeleteAsync(int id)
    {
        return await _customerRepository.SoftDeleteAsync(id);
    }

    private static CustomerResponse MapToResponseDto(Customer customer)
    {
        return new CustomerResponse
        {
            CustomerId = customer.CustomerId,
            Code = customer.Code,
            Name = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            City = customer.City,
            TaxNumber = customer.TaxNumber,
            OpeningBalance = customer.OpeningBalance,
            IsActive = customer.IsActive,
            CreationDate = customer.CreationDate,
            UpdatedDate = customer.UpdatedDate
        };
    }
}