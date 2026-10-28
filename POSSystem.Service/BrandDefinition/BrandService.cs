using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.Brand;
using POSSystem.DTO.Responses.Brand;
using POSSystem.Repository.BrandDefinition;

namespace POSSystem.Service.BrandDefinition;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;

    public BrandService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<IEnumerable<BrandResponse>> GetAllAsync()
    {
        var brands = await _brandRepository.GetAllAsync();

        return brands.Select(MapToResponse);
    }

    public async Task<BrandResponse?> GetByIdAsync(int id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);

        if (brand == null)
            return null;

        return MapToResponse(brand);
    }

    public async Task<int> CreateAsync(CreateBrandRequest request)
    {
        var brand = new Brand
        {
            Name = request.Name,
            IsActive = true,
            CreationDate = DateTime.UtcNow
        };

        return await _brandRepository.CreateAsync(brand);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateBrandRequest request)
    {
        var brand = await _brandRepository.GetByIdAsync(id);

        if (brand == null)
            return false;

        brand.Name = request.Name;
        brand.IsActive = request.IsActive;

        return await _brandRepository.UpdateAsync(brand);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _brandRepository.DeleteAsync(id);
    }

    private static BrandResponse MapToResponse(Brand brand)
    {
        return new BrandResponse
        {
            BrandId = brand.BrandId,
            Name = brand.Name,
            IsActive = brand.IsActive,
            CreationDate = brand.CreationDate
        };
    }
}