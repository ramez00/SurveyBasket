namespace SurveyBasket.Contracts.Auth.ConfirmEmail;

public record ConfirmEmailRequest(
   string UserID,
   string Code
);
