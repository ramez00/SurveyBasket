
namespace SurveyBasket.Services;

public interface IAuthService
{
    Task<AuthResponse> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default);
    Task<AuthResponse?> GetRefreshTokenAsync(string token, string RefreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeRefreshTokenAsync(string token, string RefreshToken, CancellationToken cancellationToken = default);
}