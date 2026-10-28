using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSSystem.DTO.Requests.Auth;
using POSSystem.DTO.Responses.Auth;
using POSSystem.Service.Auth;
using System.Security.Claims;

namespace POSSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequest request)
    {
        try
        {
            var response = await _authService.SignupAsync(request);
            return Ok(new { success = true, message = "Account created", data = response });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Signup error");
            return StatusCode(500, new { success = false, message = "An error occurred" });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(new { success = true, message = "Login successful", data = response });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error");
            return StatusCode(500, new { success = false, message = "An error occurred" });
        }
    }
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        try
        {
            var response = await _authService.RefreshAsync(request.RefreshToken);
            return Ok(new { success = true, data = response });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
    }

    // NOW uses JWT validation via [Authorize]
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;
        var username = User.FindFirst("username")?.Value;
        var fullName = User.FindFirst("fullName")?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userId is null)
            return Unauthorized(new { success = false, message = "Invalid token" });

        return Ok(new
        {
            success = true,
            data = new AuthUserResponse
            {
                UserId = int.Parse(userId),
                Username = username ?? "",
                FullName = fullName ?? "",
                Role = role ?? "Cashier"
            }
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(request?.RefreshToken))
                await _authService.LogoutAsync(request.RefreshToken);

            return Ok(new { success = true, message = "Logged out" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout error");
            return Ok(new { success = true, message = "Logged out" });
        }
    }
}