namespace SurveyBasket.Contracts.UserProfile;

public class UserProfileRequestValidator : AbstractValidator<UserProfileRequest>
{
    public UserProfileRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
    }
}
