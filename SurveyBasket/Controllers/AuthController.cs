using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> Login(AuthRequest request,CancellationToken cancellationToken)
    {
        var authRes = await _authService.GetTokenAsync(request.Email, request.Password,cancellationToken);

        return authRes is null ? BadRequest("Inavlid Email/Password") : Ok(authRes);
    }
}
