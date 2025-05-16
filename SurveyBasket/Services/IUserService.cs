namespace SurveyBasket.Services;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId);
    Task<Result> UpdateProfileAsync(string userId, UserProfileRequest request);
}
