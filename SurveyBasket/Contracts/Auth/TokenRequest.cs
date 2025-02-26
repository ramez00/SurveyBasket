namespace SurveyBasket.Contracts.Auth;

public record TokenRequest (
    string token,
    string refreshToken
    );
