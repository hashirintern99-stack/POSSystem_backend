using Microsoft.AspNetCore.Mvc;
using POSSystem.Infrastructure.Data;

namespace POSSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseTestController : ControllerBase
{
    private readonly DatabaseConnection _databaseConnection;

    public DatabaseTestController(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    [HttpGet]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            using var connection = _databaseConnection.CreateConnection();

            await connection.OpenAsync();

            return Ok("Database connection successful!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Database connection failed: {ex.Message}");
        }
    }
}