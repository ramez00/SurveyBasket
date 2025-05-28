namespace SurveyBasket.Contracts.Roles;

public class RoleRequestValidator : AbstractValidator<RoleRequest>
{
    public RoleRequestValidator()
    {
        RuleFor(x =>x.Name)
            .NotEmpty()
            .Length(0,100);

        RuleFor(x => x.permissions)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.permissions)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("You can not duplicated permissions for the same role")
            .When(x => x.permissions != null);
    }
}
