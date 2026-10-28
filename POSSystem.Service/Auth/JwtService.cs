using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;   // ⬅️ ADD THIS
using Microsoft.IdentityModel.Tokens;
using POSSystem.Domain.Entities;

namespace POSSystem.Service.Auth;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateAccessToken(User user, out DateTime expiresAt)
    {
        var minutes = int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "15");
        expiresAt = DateTime.UtcNow.AddMinutes(minutes);

        var keyString = _config["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new("username", user.Username ?? string.Empty),
            new("fullName", user.FullName ?? string.Empty),
            new(ClaimTypes.Role, user.Role ?? "Cashier"),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}