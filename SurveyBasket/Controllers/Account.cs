
namespace SurveyBasket.Controllers;
[Route("UserInfo")]
[ApiController]
[Authorize]
public class Account(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    [Route("GetUserProfile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _userService.GetUserProfileAsync(User.GetUserId()!);

        return Ok(result.Value);
    }

    [HttpPut]
    [Route("UpdateUserProfile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfileRequest request)
    {
        await _userService.UpdateProfileAsync(User.GetUserId()!,request);

        return NoContent();
    }

    [HttpPut]
    [Route("Change-Password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var res = await _userService.ChangePasswordAsync(User.GetUserId()!,request);

        return res.IsSuccess ? NoContent() : res.ToProblem(StatusCodes.Status400BadRequest);
    }
}
