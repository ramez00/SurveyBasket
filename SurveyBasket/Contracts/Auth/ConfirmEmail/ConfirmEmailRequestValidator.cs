namespace SurveyBasket.Contracts.Auth.ConfirmEmail;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.UserID)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}
