namespace SurveyBasket.Authentication;

public interface IJwtProvider
{
    (string token, int expiredIn) CreateToken(ApplicationUser user,IEnumerable<string> userRoles,IEnumerable<string> userPermissions);
    string? ValidateToken(string token);
}