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

        if (!user.EmailConfirmed)
            return Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed);

        var (token, expireIn, RefreshToken, RefreshTokenExpiration) = await CreateTokenWithRefreshToken(user);

        var resp = new AuthResponse(user.Id,user.Email,user.FirstName,user.LastName
            ,token,expireIn * ApplicationConstant.hour,
            RefreshToken,RefreshTokenExpiration);

        return Result.Success(resp);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string RefreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        
        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null) 
            return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (newtoken, expireIn, RefreshTokenNew, RefreshTokenExpiration) = await CreateTokenWithRefreshToken(user);

        var resp = new AuthResponse(user.Id, user.Email,user.FirstName,user.LastName
            , newtoken, expireIn * ApplicationConstant.hour,
            RefreshTokenNew, RefreshTokenExpiration);

        return Result.Success(resp);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string RefreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure(Errors.UserErrors.InvalidToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(Errors.UserErrors.InvalidUser);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(Errors.UserErrors.InvalidToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Result.Success();
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

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken = default)
    {
        var isEmailExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if(isEmailExist)
            return Result.Failure<AuthResponse>(UserErrors.UserExist);

        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<AuthResponse>(new Error(error.Code,error.Description));
        }


        var (token, expireIn, RefreshToken, RefreshTokenExpiration) = await CreateTokenWithRefreshToken(user);

        var resp = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName
            , token, expireIn * ApplicationConstant.hour,
            RefreshToken, RefreshTokenExpiration);

        return Result.Success(resp);

    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserID);

        if (user is null)
            return Result.Failure(Errors.UserErrors.InvalidUser);

        if (user.EmailConfirmed)
            return Result.Failure(Errors.UserErrors.EmailAlreadyConfirmed);

        var code = request.Code;

        try
        {
            code = Encoding.UTF8.GetString(Convert.FromBase64String(code));
        }
        catch (Exception)
        {

            return Result.Failure(Errors.UserErrors.InvalidToken);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (!result.Succeeded)
            return Result.Failure(Errors.UserErrors.InvalidToken);

        return Result.Success();
    }
}