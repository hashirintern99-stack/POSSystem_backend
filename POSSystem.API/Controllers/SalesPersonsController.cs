using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSSystem.DTO.Requests.SalesPerson;
using POSSystem.Service.SalesPersonDefinition;

namespace POSSystem.API.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SalesPersonsController : ControllerBase
{
    private readonly ISalesPersonService _salesPersonService;

    public SalesPersonsController(
        ISalesPersonService salesPersonService)
    {
        _salesPersonService = salesPersonService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var salesPersons =
            await _salesPersonService.GetAllAsync();

        return Ok(salesPersons);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var salesPerson =
            await _salesPersonService.GetByIdAsync(id);

        if (salesPerson == null)
            return NotFound();

        return Ok(salesPerson);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateSalesPersonRequest dto)
    {
        var salesPersonId =
            await _salesPersonService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = salesPersonId },
            new { SalesPersonId = salesPersonId }
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateSalesPersonRequest dto)
    {
        var updated =
            await _salesPersonService.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted =
            await _salesPersonService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}