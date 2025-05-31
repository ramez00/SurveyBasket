namespace SurveyBasket.Services;

public interface IUserService
{
    Task<Result<UserResponse>> GetUserDetialsAsync(string userId);
    Task<Result<IEnumerable<UserResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId);
    Task<Result> UpdateProfileAsync(string userId, UserProfileRequest request);
    Task<Result> ChangePasswordAsync(string userId,ChangePasswordRequest request);
}
