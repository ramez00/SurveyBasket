namespace SurveyBasket.Contracts.UserProfile;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    IList<string> roles,
    bool IsActive = true
);