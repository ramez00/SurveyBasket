namespace SurveyBasket.Services;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId);
}
