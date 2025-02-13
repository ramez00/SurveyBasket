using Microsoft.AspNetCore.Identity;
using SurveyBasket.Authentication;

namespace SurveyBasket.Services;

public class AuthService(UserManager<ApplicationUser> userManager,IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    public async Task<AuthResponse?> GetTokenAsync(string Email, string Password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(Email);

        if (user is null)
            return null;

        var IsVaild = await _userManager.CheckPasswordAsync(user, Password);

        if (!IsVaild)
            return null;

        var (token, expireIn) = _jwtProvider.CreateToken(user);

        return new AuthResponse(user.Id,Email,"Ramez","Abdalhamid",token,expireIn * 60);
    }
}