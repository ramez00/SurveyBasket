namespace SurveyBasket.Contracts.Auth.ConfirmEmail;

public record ConfirmEmailRequest(
   string Email,
   string Code
);
