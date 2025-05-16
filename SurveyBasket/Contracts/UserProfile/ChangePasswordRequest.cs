namespace SurveyBasket.Contracts.UserProfile;

public record ChangePasswordRequest(
    string currentPassword,
    string NewPassword
);
