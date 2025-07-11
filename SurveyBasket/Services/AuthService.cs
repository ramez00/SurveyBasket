﻿using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasket.Authentication;
using SurveyBasket.Errors;
using SurveyBasket.Helpers;
using System.Security.Cryptography;
using System.Text;
using SurveyBasket.Contracts.Auth;
using SurveyBasket.Abstractions;

namespace SurveyBasket.Services;

public class AuthService(UserManager<ApplicationUser> userManager
                        , IJwtProvider jwtProvider
                        , IEmailSender emailSender
                        , SignInManager<ApplicationUser> signInManager                          
                        , IHttpContextAccessor httpContextAccessor
                        , ApplicationDbContext context
                        , ILogger<AuthService> logger) : IAuthService
{
   
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly int _refreshTokenExpiryDays = ApplicationConstant.refreshTokenExpiryDays;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ApplicationDbContext _context = context;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

    public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(Email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredrntials);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.UserIsDisabled);

        var IsVaild = await _signInManager.PasswordSignInAsync(user, Password,false,true);

        if (IsVaild.IsLockedOut)
            return Result.Failure<AuthResponse>(UserErrors.UserLockedOut);

        if (!user.EmailConfirmed)
            return Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed);

        if (IsVaild.IsNotAllowed)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredrntials);

        var (token, expireIn, RefreshToken, RefreshTokenExpiration) = await CreateTokenWithRefreshToken(user);

        var resp = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName
            , token, expireIn * ApplicationConstant.hour,
            RefreshToken, RefreshTokenExpiration);

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

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.UserIsDisabled);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.UserLockedOut);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (newtoken, expireIn, RefreshTokenNew, RefreshTokenExpiration) = await CreateTokenWithRefreshToken(user);

        var resp = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName
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
            return Result.Failure(Errors.UserErrors.UserNotFound);

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
        var (userRoles, userPermissions) = await GetUserRoleAndPermission(user);

        var (token, expireIn) = _jwtProvider.CreateToken(user,userRoles,userPermissions);

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

    public async Task<Result> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken = default)
    {
        var isEmailExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (isEmailExist)
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
            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description));
        }

        await _userManager.AddToRoleAsync(user,DefaultRole.Member);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = Convert.ToBase64String(Encoding.UTF8.GetBytes(code));

        var callbackUrl = $"auth/confirm-email?userId={user.Id}&code={code}";

        _logger.LogInformation("Email Confirmation Link: {callbackUrl}", callbackUrl);

        await SendConfirmationEmailAsync(user, callbackUrl);

        return Result.Success();
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Failure(Errors.UserErrors.UserNotFound);

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


    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Success();

        if(!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = Convert.ToBase64String(Encoding.UTF8.GetBytes(code));

        var callbackUrl = $"/auth/forget-password?userId={user.Id}&code={code}";

        _logger.LogInformation("Email Reset Password Link: {callbackUrl}", callbackUrl);

        await SendResetPasswordAsync(user, callbackUrl);

        return Result.Success();
    }


    public async Task<Result> ResendConfirmEmailAsync(ResendConfirmationEmail request)
    {

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Failure(UserErrors.EmailNotFound);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailCofirmed);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = Convert.ToBase64String(Encoding.UTF8.GetBytes(code));

        var callbackUrl = $"/auth/confirm-email?userId={user.Id}&code={code}";

        _logger.LogInformation("Email Confirmation Link: {callbackUrl}", callbackUrl);

        await SendConfirmationEmailAsync(user, callbackUrl);

        return Result.Success();
    }


    private async Task SendConfirmationEmailAsync(ApplicationUser user,string confirmationLink)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var body = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            new Dictionary<string, string>
            {
                { "{name}" , $"{user.FirstName} {user.LastName}" },
                { "{verficationCode}" , $"{origin}/{confirmationLink}" },
            }
        );
        
        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket : Confirmation Email", body));
        
        await Task.CompletedTask;
    }


    private async Task SendResetPasswordAsync(ApplicationUser user, string confirmationLink)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var body = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
            new Dictionary<string, string>
            {
                { "{name}" , $"{user.FirstName} {user.LastName}" },
                { "{Code}" , $"{origin}/{confirmationLink}" },
            }
        );

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket : Change Password Email", body));

        await Task.CompletedTask;
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.email);

        if(user is null || !user.EmailConfirmed)
            return Result.Failure(Errors.UserErrors.InvalidToken);

        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(Convert.FromBase64String(request.code));
            result = await _userManager.ResetPasswordAsync(user, code, request.newPassword);
        }
        catch (Exception)
        {
            return Result.Failure(Errors.UserErrors.InvalidToken);
        }

        if(result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description));
    }

    private async Task<(IEnumerable<string> userRole ,IEnumerable<string> userPermission)>
        GetUserRoleAndPermission(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var userPermission = await (from role in _context.Roles
                                    join permission in _context.RoleClaims on role.Id equals permission.RoleId
                                    where userRoles.Contains(role.Name!)
                                    select permission.ClaimValue)
                                    .Distinct()
                                    .ToListAsync();

        return (userRoles, userPermission);
    }
}