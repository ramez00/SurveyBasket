namespace SurveyBasket.Contracts.UserProfile;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    IList<string> roles
);
