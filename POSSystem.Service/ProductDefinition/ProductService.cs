// POSSystem.Service.ProductDefinition/ProductService.cs

using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.Product;
using POSSystem.DTO.Responses.Product;
using POSSystem.Repository.CategoryDefinition;
using POSSystem.Repository.ProductDefinition;
using POSSystem.Service.SequenceDefinition;

namespace POSSystem.Service.ProductDefinition;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISequenceService _sequenceService;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ISequenceService sequenceService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _sequenceService = sequenceService;
    }

    // ═══════════════════════════════════════════════════════
    // GET ALL
    // ═══════════════════════════════════════════════════════
    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToResponseDto);
    }

    // ═══════════════════════════════════════════════════════
    // GET BY ID
    // ═══════════════════════════════════════════════════════
    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return null;
        return MapToResponseDto(product);
    }

    // ═══════════════════════════════════════════════════════
    // CREATE — Backend generates the code atomically
    // ═══════════════════════════════════════════════════════
    public async Task<int> CreateAsync(CreateProductRequest dto)
    {
        // 1. Resolve category name → used to build the code prefix
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new InvalidOperationException(
                $"Category with ID {dto.CategoryId} not found");

        // 2. Ask the sequence service for the next code
        //    (server-side, atomic, no collisions)
        var code = await _sequenceService.GetNextProductCodeAsync(category.Name);

        // 3. Build the product — code is server-assigned, dto.Code is ignored
        var product = new Product
        {
            Code = code,
            Barcode = dto.Barcode,
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId,
            UnitId = dto.UnitId,
            CostPrice = dto.CostPrice,
            RetailPrice = dto.RetailPrice,
            TaxPercentage = dto.TaxPercentage,
            MinimumStock = dto.MinimumStock,
            CurrentStock = 0,
            ImageURL = dto.ImageURL,
            IsActive = true,
            CreationDate = DateTime.UtcNow
        };

        return await _productRepository.CreateAsync(product);
    }

    // ═══════════════════════════════════════════════════════
    // PREVIEW NEXT CODE — for the "New Product" form
    // ═══════════════════════════════════════════════════════
    public async Task<string> PeekNextCodeAsync(int categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category == null)
            throw new InvalidOperationException(
                $"Category with ID {categoryId} not found");

        // Peek — does NOT increment the counter
        return await _sequenceService.PeekNextProductCodeAsync(category.Name);
    }

    // ═══════════════════════════════════════════════════════
    // UPDATE
    // ═══════════════════════════════════════════════════════
    public async Task<bool> UpdateAsync(int id, UpdateProductRequest dto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
            return false;

        var product = new Product
        {
            ProductId = existingProduct.ProductId,
            Code = dto.Code,                 // ✅ keep existing code on update
            Barcode = dto.Barcode,
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId,
            UnitId = dto.UnitId,
            CostPrice = dto.CostPrice,
            RetailPrice = dto.RetailPrice,
            TaxPercentage = dto.TaxPercentage,
            MinimumStock = dto.MinimumStock,
            CurrentStock = dto.CurrentStock,
            ImageURL = dto.ImageURL,
            IsActive = dto.IsActive,
            CreationDate = existingProduct.CreationDate,
            UpdatedDate = DateTime.UtcNow
        };

        return await _productRepository.UpdateAsync(product);
    }

    // ═══════════════════════════════════════════════════════
    // DELETE (soft / hard based on relations)
    // ═══════════════════════════════════════════════════════
    public async Task<bool> DeleteAsync(int id)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
            return false;

        var hasRelated = await _productRepository.HasRelatedRecords(id);

        if (hasRelated)
            return await _productRepository.SoftDeleteAsync(id);

        return await _productRepository.DeleteAsync(id);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        return await _productRepository.SoftDeleteAsync(id);
    }

    // ═══════════════════════════════════════════════════════
    // LOW STOCK
    // ═══════════════════════════════════════════════════════
    public async Task<IEnumerable<LowStockProductDto>> GetLowStockProductsAsync()
    {
        return await _productRepository.GetLowStockProductsAsync();
    }

    public async Task<int> GetLowStockCountAsync()
    {
        return await _productRepository.GetLowStockCountAsync();
    }

    // ═══════════════════════════════════════════════════════
    // MAPPER
    // ═══════════════════════════════════════════════════════
    private static ProductResponse MapToResponseDto(ProductWithDetails product)
    {
        return new ProductResponse
        {
            ProductId = product.ProductId,
            Code = product.Code,
            Barcode = product.Barcode,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId,
            CategoryName = product.CategoryName,
            BrandId = product.BrandId,
            BrandName = product.BrandName,
            UnitId = product.UnitId,
            UnitName = product.UnitName,
            CostPrice = product.CostPrice,
            RetailPrice = product.RetailPrice,
            TaxPercentage = product.TaxPercentage,
            MinimumStock = product.MinimumStock,
            CurrentStock = product.CurrentStock,
            ImageURL = product.ImageURL,
            IsActive = product.IsActive,
            CreationDate = product.CreationDate,
            UpdatedDate = product.UpdatedDate
        };
    }
}