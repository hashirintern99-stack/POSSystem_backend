using Microsoft.Extensions.Configuration;      // ⬅️ for IConfiguration
using Microsoft.Extensions.Logging;
using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.Auth;
using POSSystem.DTO.Responses.Auth;
using POSSystem.Repository.Auth;

namespace POSSystem.Service.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repo;
    private readonly IJwtService _jwt;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthRepository repo,
        IJwtService jwt,
        IConfiguration config,
        ILogger<AuthService> logger)
    {
        _repo = repo;
        _jwt = jwt;
        _config = config;
        _logger = logger;
    }

    // =============================================
    // SIGNUP
    // =============================================
    public async Task<LoginResponse> SignupAsync(SignupRequest request)
    {
        var userCount = await _repo.GetUserCountAsync();
        var assignedRole = userCount == 0
            ? "Admin"
            : (!string.IsNullOrWhiteSpace(request.Role) ? request.Role! : "Cashier");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var result = await _repo.SignupUserAsync(
            request.Username.Trim(),
            request.Email.Trim(),
            passwordHash,
            request.FullName.Trim(),
            assignedRole);

        if (!result.Success || result.UserId is null)
            throw new InvalidOperationException(result.Message);

        var user = await _repo.ValidateSessionAsync("__never_matches__"); // placeholder
        user = await _repo.GetUserByUsernameAsync(request.Username.Trim());
        if (user is null) throw new InvalidOperationException("User creation failed");

        return await IssueTokensAsync(user);
    }

    // =============================================
    // LOGIN
    // =============================================
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _repo.GetUserByUsernameAsync(request.Username.Trim());

        if (user is null)
        {
            _logger.LogWarning("Login failed: user not found. Username: {Username}", request.Username);
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Your account has been deactivated");

        bool passwordValid;
        try
        {
            passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying password for UserId: {UserId}", user.UserId);
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        if (!passwordValid)
        {
            _logger.LogWarning("Login failed: invalid password. UserId: {UserId}", user.UserId);
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        return await IssueTokensAsync(user);
    }

    // =============================================
    // REFRESH — trade a valid refresh token for a new JWT
    // =============================================
    public async Task<LoginResponse> RefreshAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("No refresh token");

        var user = await _repo.ValidateSessionAsync(refreshToken);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        // Issue new access token but REUSE the same refresh token
        var accessToken = _jwt.GenerateAccessToken(user, out var accessExpiresAt);

        return new LoginResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(
                int.Parse(_config["Jwt:RefreshTokenDays"] ?? "30")),
            User = MapToAuthUser(user)
        };
    }

    // =============================================
    // ME — reads from JWT claims, no DB call needed
    // =============================================
    public async Task<AuthUserResponse?> GetCurrentUserAsync(string token)
    {
        // With JWT, [Authorize] middleware already validated the token.
        // This method is now mostly vestigial but kept for compatibility.
        if (string.IsNullOrWhiteSpace(token)) return null;
        var user = await _repo.ValidateSessionAsync(token);
        return user is null ? null : MapToAuthUser(user);
    }

    // =============================================
    // LOGOUT — kills the refresh token in the DB
    // =============================================
    public async Task<bool> LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return false;
        return await _repo.DeleteSessionAsync(refreshToken);
    }

    // =============================================
    // HELPERS
    // =============================================
    private async Task<LoginResponse> IssueTokensAsync(User user)
    {
        // 1. Generate JWT (short-lived)
        var accessToken = _jwt.GenerateAccessToken(user, out var accessExpiresAt);

        // 2. Generate opaque refresh token (long-lived, DB-stored)
        var refreshToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        var refreshExpiresAt = DateTime.UtcNow.AddDays(
            int.Parse(_config["Jwt:RefreshTokenDays"] ?? "30"));

        await _repo.CreateSessionAsync(user.UserId, refreshToken, refreshExpiresAt);

        _logger.LogInformation("Tokens issued. UserId: {UserId}", user.UserId);

        return new LoginResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt,
            User = MapToAuthUser(user)
        };
    }

    private static AuthUserResponse MapToAuthUser(User user) => new()
    {
        UserId = user.UserId,
        Username = user.Username,
        Email = user.Email,
        FullName = user.FullName,
        Role = user.Role
    };
}