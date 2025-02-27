using Microsoft.AspNetCore.Identity;
using SurveyBasket.Authentication;
using SurveyBasket.Errors;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Services;

public class AuthService(UserManager<ApplicationUser> userManager,IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly int _refreshTokenExpiryDays = ApplicationConstant.refreshTokenExpiryDays;

    public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(Email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredrntials);

        var IsVaild = await _userManager.CheckPasswordAsync(user, Password);

        if (!IsVaild)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredrntials);

        var (token, expireIn, RefreshToken, RefreshTokenExpiration) = await CreateTokenWithRefreshToken(user);


        var resp = new AuthResponse(user.Id,Email,user.FirstName,user.LastName
            ,token,expireIn * ApplicationConstant.hour,
            RefreshToken,RefreshTokenExpiration);

        return Result.Success(resp);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string RefreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        
        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredrntials);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null) 
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredrntials);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredrntials);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (newtoken, expireIn, RefreshTokenNew, RefreshTokenExpiration) = await CreateTokenWithRefreshToken(user);

        var resp = new AuthResponse(user.Id, user.Email,user.FirstName,user.LastName
            , newtoken, expireIn * ApplicationConstant.hour,
            RefreshTokenNew, RefreshTokenExpiration);

        return Result.Success(resp);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, string RefreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return false;

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return false;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);

        if (userRefreshToken is null)
            return false;

        userRefreshToken.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return true;
    }
    private static string GetRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(ApplicationConstant.RequiredBytes));

    private async Task<(string token, int expire, string RefreshToken, DateTime RefreshTokenExpiration)>
        CreateTokenWithRefreshToken(ApplicationUser user)
    {
        var (token, expireIn) = _jwtProvider.CreateToken(user);

        var RefreshToken = GetRefreshToken();
        var RefreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new Models.RefreshToken
        {
            Token = RefreshToken,
            ExpiresOn = RefreshTokenExpiration,
        });

        await _userManager.UpdateAsync(user);

        return (token, expireIn, RefreshToken, RefreshTokenExpiration);
    }
}