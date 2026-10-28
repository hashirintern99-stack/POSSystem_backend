using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSSystem.DTO.Requests.Purchase;
using POSSystem.Service.PurchaseDefinition;

namespace POSSystem.API.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchaseController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    // GET: api/Purchase
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var purchases = await _purchaseService.GetAllAsync();

        return Ok(purchases);
    }

    // GET: api/Purchase/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var purchase = await _purchaseService.GetByIdAsync(id);

        if (purchase == null)
            return NotFound();

        return Ok(purchase);
    }

    // POST: api/Purchase
    [HttpPost]
    public async Task<IActionResult> Create(
        CreatePurchaseRequest request)
    {
        var purchaseId =
            await _purchaseService.CreateAsync(request);

        return Ok(purchaseId);
    }

    // PUT: api/Purchase/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        CreatePurchaseRequest request)
    {
        var updated =
            await _purchaseService.UpdateAsync(id, request);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    // DELETE: api/Purchase/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted =
            await _purchaseService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}