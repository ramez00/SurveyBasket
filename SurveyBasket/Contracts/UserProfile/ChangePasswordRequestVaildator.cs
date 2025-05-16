namespace SurveyBasket.Contracts.UserProfile;

public class ChangePasswordRequestVaildator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestVaildator()
    {
        RuleFor(x => x.currentPassword).NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(UserErrors.FieldRequired("New Password"))
            .NotEqual(x => x.currentPassword)
            .WithMessage(UserErrors.PasswordEqualPrevious.Description);
    }
}
