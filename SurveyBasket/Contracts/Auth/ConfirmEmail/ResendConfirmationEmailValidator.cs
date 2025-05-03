namespace SurveyBasket.Contracts.Auth.ConfirmEmail;

public class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmail>
{
    public ResendConfirmationEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");
    }
}
