namespace SurveyBasket.Contracts.Auth;

public record ResetPasswordRequest(
    string email,
    string code,
    string newPassword
);
