using POSSystem.Domain.Entities;

namespace POSSystem.Repository.Auth;

public interface IAuthRepository
{
    Task<(bool Success, string Message, int? UserId)> SignupUserAsync(
        string username,
        string email,
        string passwordHash,
        string fullName,
        string roleName);

    Task<User?> GetUserByUsernameAsync(string username);

    Task<bool> CreateSessionAsync(int userId, string token, DateTime expiresAt);

    Task<User?> ValidateSessionAsync(string token);

    Task<bool> DeleteSessionAsync(string token);

    Task<int> GetUserCountAsync();
}