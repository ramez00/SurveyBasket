

namespace SurveyBasket.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId)
    {
        var user = await _userManager.Users
                        .Where(x => x.Id == userId)
                        .ProjectToType<UserProfileResponse>()  // Change to DbContext Instead User Managment  to Get Needed Data Only  
                        .SingleAsync();
       
        return Result.Success(user);
    }

    public async Task<Result> UpdateProfileAsync(string userId, UserProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        user = request.Adapt(user);
        await _userManager.UpdateAsync(user!);

        return Result.Success();

    }
}
