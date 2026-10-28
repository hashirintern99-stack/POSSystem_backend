using POSSystem.Domain.Entities;

namespace POSSystem.Service.Auth;

public interface IJwtService
{
    string GenerateAccessToken(User user, out DateTime expiresAt);
}