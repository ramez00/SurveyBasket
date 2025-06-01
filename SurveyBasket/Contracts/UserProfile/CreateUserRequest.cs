namespace SurveyBasket.Contracts.UserProfile;

public record CreateUserRequest(
    string firstName,
    string lastName,
    string Email,
    string Password,
    IList<string> roles
);
