using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Utilities;

namespace SurveyBasket.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    public async Task<IActionResult> GetAllasync()
    {
        var users = await _userService.GetAllAsync();
        return users.IsSuccess
            ? Ok(users.Value)
            : users.ToProblem(StatusCodes.Status500InternalServerError);
    }
}
