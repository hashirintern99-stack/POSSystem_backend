using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSSystem.DTO.Requests.SalesReturn;
using POSSystem.Service.SaleReturnDefinition;

namespace POSSystem.API.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SalesReturnController : ControllerBase
{
    private readonly ISalesReturnService _salesReturnService;

    public SalesReturnController(ISalesReturnService salesReturnService)
    {
        _salesReturnService = salesReturnService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var returns = await _salesReturnService.GetAllAsync();
        return Ok(returns);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var salesReturn = await _salesReturnService.GetByIdAsync(id);
        if (salesReturn == null)
            return NotFound();
        return Ok(salesReturn);
    }

    [HttpGet("sale/{saleId}")]
    public async Task<IActionResult> GetBySaleId(int saleId)
    {
        var returns = await _salesReturnService.GetBySaleIdAsync(saleId);
        return Ok(returns);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSalesReturnRequest request)
    {
        var id = await _salesReturnService.CreateAsync(request);
        return Ok(new { salesReturnId = id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateSalesReturnRequest request)
    {
        var result = await _salesReturnService.UpdateAsync(id, request);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _salesReturnService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}