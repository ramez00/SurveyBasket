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

    [HttpGet]
    public IActionResult test()
    {
        var obj = new
        {
            jwtKey = _jwtoption.Key,
            Issuer = _jwtoption.Issuer,
        };
        return Ok(obj);
    }
}