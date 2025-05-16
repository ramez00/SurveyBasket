

namespace SurveyBasket.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
       
        return Result.Success(user.Adapt<UserProfileResponse>());
    }
}
