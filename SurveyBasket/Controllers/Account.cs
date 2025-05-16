
namespace SurveyBasket.Controllers;
[Route("UserInfo")]
[ApiController]
[Authorize]
public class Account(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    [Route("GetUserProfile")]
    public async Task<IActionResult> GetProfile(string userId)
    {
        var result = await _userService.GetUserProfileAsync(User.GetUserId()!);

        return Ok(result.Value);
    }
}
