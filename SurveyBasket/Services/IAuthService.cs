
namespace SurveyBasket.Services;

public interface IAuthService
{
    Task<AuthResponse> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default);
}