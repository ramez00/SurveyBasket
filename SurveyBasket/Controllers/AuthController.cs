using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SurveyBasket.Authentication;

namespace SurveyBasket.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService,IOptions<JwtOptions> jwtoption) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly JwtOptions _jwtoption = jwtoption.Value;

    [HttpPost("")]
    public async Task<IActionResult> Login(AuthRequest request,CancellationToken cancellationToken)
    {
        var authRes = await _authService.GetTokenAsync(request.Email, request.Password,cancellationToken);

        return authRes is null ? BadRequest("Inavlid Email/Password") : Ok(authRes);
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken(TokenRequest request, CancellationToken cancellationToken)
    {
        var authRes = await _authService.GetRefreshTokenAsync(request.token, request.refreshToken, cancellationToken);

        return authRes is null ? BadRequest("Inavlid Token") : Ok(authRes);
    }

    [HttpPut("Revoke-Refresh-Token")]
    public async Task<IActionResult> RevokeRefreshToken(TokenRequest request, CancellationToken cancellationToken)
    {
        var isRevoked = await _authService.RevokeRefreshTokenAsync(request.token, request.refreshToken, cancellationToken);

        return isRevoked ? Ok() : BadRequest("Inavlid Token");
    }

}