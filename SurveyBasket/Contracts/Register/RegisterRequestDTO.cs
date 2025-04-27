namespace SurveyBasket.Contracts.Register;

public record RegisterRequestDTO(
    string FirstName,
    string LastName,
    string Email,
    string Password
);
