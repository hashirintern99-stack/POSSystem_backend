using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSSystem.DTO.Requests.GRN;
using POSSystem.Service.GRNDefinition;

namespace POSSystem.API.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GRNController : ControllerBase
{
    private readonly IGRNService _grnService;

    public GRNController(IGRNService grnService)
    {
        _grnService = grnService;
    }


    // =====================================
    // GET: api/GRN
    // =====================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var grns =
            await _grnService.GetAllAsync();

        return Ok(grns);
    }


    // =====================================
    // GET: api/GRN/5
    // =====================================

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var grn =
            await _grnService.GetByIdAsync(id);

        if (grn == null)
            return NotFound();

        return Ok(grn);
    }


    // =====================================
    // POST: api/GRN
    // =====================================

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateGRNRequest request)
    {
        var grnId =
            await _grnService.CreateAsync(request);

        return Ok(new
        {
            grnId
        });
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
    int id,
    CreateGRNRequest request)
    {
        var result =
            await _grnService.UpdateAsync(
                id,
                request);

        if (!result)
            return NotFound();

        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result =
            await _grnService.DeleteAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }
}