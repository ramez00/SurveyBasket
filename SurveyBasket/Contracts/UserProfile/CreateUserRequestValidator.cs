namespace SurveyBasket.Contracts.UserProfile;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
                   .NotEmpty()
                   .WithMessage("First name is required.")
                   .Length(2, 90)
                   .WithMessage("First name must be between 2 and 50 characters.");

        RuleFor(x => x.LastName)
            .Length(3, 100)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            //.Matches(expression: @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$") // match Patterns 
            .MinimumLength(6)
            .NotEmpty();

        RuleFor(x => x.roles)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.roles)
            .Must(x => x.Distinct().Count() == x.Count())  // number of role should be Equal Number of Unique Role
            .WithMessage("Cannot add duplicated roles for the same user.")
            .Must(x => x.Count > 0)
            .WithMessage("User should have at least one role.")
            .When(x => x.roles != null);
    }
}
