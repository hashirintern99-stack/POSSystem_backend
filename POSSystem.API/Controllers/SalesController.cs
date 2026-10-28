using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSSystem.DTO.Requests.Sale;
using POSSystem.DTO.Responses.Sale;
using POSSystem.Service.SaleDefinition;

namespace POSSystem.API.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    // =============================================
    // GET: api/Sales
    // =============================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sales = await _saleService.GetAllAsync();
        return Ok(sales);
    }

    // =============================================
    // GET: api/Sales/5
    // =============================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sale = await _saleService.GetByIdAsync(id);
        if (sale == null)
            return NotFound();
        return Ok(sale);
    }

    // =============================================
    // GET: api/Sales/5/details (FOR SEARCH)
    // =============================================
    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetails(int id)
    {
        var details = await _saleService.GetSaleDetailsForSearchAsync(id);
        if (details == null)
            return NotFound();
        return Ok(details);
    }

    // =============================================
    // POST: api/Sales/search
    // =============================================
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchSaleRequest request)
    {
        var results = await _saleService.SearchAsync(request);
        return Ok(results);
    }

    // =============================================
    // POST: api/Sales
    // =============================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest request)
    {
        var saleId = await _saleService.CreateAsync(request);
        return Ok(new { saleId });
    }

    // =============================================
    // PUT: api/Sales/5
    // =============================================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateSaleRequest request)
    {
        var result = await _saleService.UpdateAsync(id, request);
        if (!result)
            return NotFound();
        return NoContent();
    }

    // =============================================
    // DELETE: api/Sales/5
    // =============================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _saleService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}