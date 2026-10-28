using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSSystem.DTO.Requests.Unit;
using POSSystem.Service.UnitDefinition;

namespace POSSystem.API.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _unitService;

    public UnitsController(IUnitService unitService)
    {
        _unitService = unitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var units = await _unitService.GetAllAsync();

        return Ok(units);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var unit = await _unitService.GetByIdAsync(id);

        if (unit == null)
            return NotFound();

        return Ok(unit);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateUnitRequest request)
    {
        var unitId = await _unitService.CreateAsync(request);

        return Ok(unitId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateUnitRequest request)
    {
        var updated = await _unitService.UpdateAsync(id, request);

        if (!updated)
            return NotFound();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _unitService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}