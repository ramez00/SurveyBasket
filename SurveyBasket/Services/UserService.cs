

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
        //var user = await _userManager.FindByIdAsync(userId);

        //if (user == null)
        //    return Result.Failure(new Error("UserNotFound", "User not found"));

        //user = request.Adapt(user);

        //await _userManager.UpdateAsync(user);

        // to Update without Loading the User Object in Memory

        await _userManager.Users
            .Where(usr => usr.Id == userId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(usr => usr.FirstName, request.FirstName)
                .SetProperty(usr => usr.LastName, request.LastName)
            );

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var Res = await _userManager.ChangePasswordAsync(user!, request.currentPassword, request.NewPassword);

        if(Res.Succeeded)
            return Result.Success();

        var error = Res.Errors.First();

        return Result.Failure( new Error(error.Code,error.Description));
    }
}
