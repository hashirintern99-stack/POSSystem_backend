using POSSystem.DTO.Requests.Auth;
using POSSystem.DTO.Responses.Auth;

namespace POSSystem.Service.Auth;

public interface IAuthService
{
    Task<LoginResponse> SignupAsync(SignupRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RefreshAsync(string refreshToken);
    Task<AuthUserResponse?> GetCurrentUserAsync(string token);
    Task<bool> LogoutAsync(string refreshToken);
}