using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSSystem.DTO.Requests.Product;
using POSSystem.Service.ProductDefinition;

namespace POSSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest dto)
    {
        try
        {
            var productId = await _productService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = productId },
                new { ProductId = productId }
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProductRequest dto)
    {
        var updated = await _productService.UpdateAsync(id, dto);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // ✅ NEW — preview next code for the New Product form
    [HttpGet("next-code")]
    public async Task<IActionResult> GetNextCode([FromQuery] int categoryId)
    {
        try
        {
            var code = await _productService.PeekNextCodeAsync(categoryId);
            return Ok(new { code });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ── Low stock ──
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockProducts()
    {
        try
        {
            var products = await _productService.GetLowStockProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("low-stock/count")]
    public async Task<IActionResult> GetLowStockCount()
    {
        try
        {
            var count = await _productService.GetLowStockCountAsync();
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}