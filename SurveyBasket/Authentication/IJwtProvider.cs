namespace SurveyBasket.Authentication;

public interface IJwtProvider
{
    (string token, int expiredIn) CreateToken(ApplicationUser user);
    string? ValidateToken(string token);
}