// POSSystem.Service.SupplierDefinition/SupplierService.cs

using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.Supplier;
using POSSystem.DTO.Responses.Supplier;
using POSSystem.Repository.SupplierDefinition;
using POSSystem.Service.SequenceDefinition;

namespace POSSystem.Service.SupplierDefinition;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISequenceService _sequenceService;

    public SupplierService(
        ISupplierRepository supplierRepository,
        ISequenceService sequenceService)
    {
        _supplierRepository = supplierRepository;
        _sequenceService = sequenceService;
    }

    public async Task<IEnumerable<SupplierResponse>> GetAllAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers.Select(MapToResponseDto);
    }

    public async Task<SupplierResponse?> GetByIdAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null) return null;
        return MapToResponseDto(supplier);
    }

    // ═══════════════════════════════════════════════════════
    // CREATE — backend assigns the code
    // ═══════════════════════════════════════════════════════
    public async Task<int> CreateAsync(CreateSupplierRequest dto)
    {
        // Generate code server-side (atomic, no collisions)
        var code = await _sequenceService.GetNextCodeAsync("supplier");

        var supplier = new Supplier
        {
            Code = code,                 // ← server-generated, dto.Code ignored
            Name = dto.Name,
            ContactPerson = dto.ContactPerson,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            City = dto.City,
            TaxNumber = dto.TaxNumber,
            OpeningBalance = dto.OpeningBalance,
            IsActive = true,
            CreationDate = DateTime.UtcNow
        };

        return await _supplierRepository.CreateAsync(supplier);
    }

    // ═══════════════════════════════════════════════════════
    // PEEK NEXT CODE — for the "New Supplier" form preview
    // Does NOT consume a number
    // ═══════════════════════════════════════════════════════
    public Task<string> PeekNextCodeAsync()
    {
        return _sequenceService.PeekNextSupplierCodeAsync();
    }

    public async Task<bool> UpdateAsync(int id, UpdateSupplierRequest dto)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null) return false;

        supplier.Code = dto.Code;   // ✅ code preserved on update
        supplier.Name = dto.Name;
        supplier.ContactPerson = dto.ContactPerson;
        supplier.Phone = dto.Phone;
        supplier.Email = dto.Email;
        supplier.Address = dto.Address;
        supplier.City = dto.City;
        supplier.TaxNumber = dto.TaxNumber;
        supplier.OpeningBalance = dto.OpeningBalance;
        supplier.IsActive = dto.IsActive;
        supplier.UpdatedDate = DateTime.UtcNow;

        return await _supplierRepository.UpdateAsync(supplier);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingSupplier = await _supplierRepository.GetByIdAsync(id);
        if (existingSupplier == null) return false;

        return await _supplierRepository.DeleteAsync(id);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        return await _supplierRepository.SoftDeleteAsync(id);
    }

    private static SupplierResponse MapToResponseDto(Supplier supplier)
    {
        return new SupplierResponse
        {
            SupplierId = supplier.SupplierId,
            Code = supplier.Code,
            Name = supplier.Name,
            ContactPerson = supplier.ContactPerson,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Address = supplier.Address,
            City = supplier.City,
            TaxNumber = supplier.TaxNumber,
            OpeningBalance = supplier.OpeningBalance,
            IsActive = supplier.IsActive,
            CreationDate = supplier.CreationDate,
            UpdatedDate = supplier.UpdatedDate
        };
    }
}