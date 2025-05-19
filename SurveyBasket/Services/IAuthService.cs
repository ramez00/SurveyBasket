
namespace SurveyBasket.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string RefreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string token, string RefreshToken, CancellationToken cancellationToken = default);
    Task<Result> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<Result> ResendConfirmEmailAsync(ResendConfirmationEmail request);
    Task<Result> SendResetPasswordCodeAsync(string email);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
}