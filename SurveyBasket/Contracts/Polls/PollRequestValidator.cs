
using FluentValidation;

namespace SurveyBasket.Contracts.Polls;

public class PollRequestValidator : AbstractValidator<PollsRequest>
{
    public PollRequestValidator()
    {
        RuleFor(x => x.StartsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

        RuleFor(x => x.EndsAt)
            .NotEmpty();

        RuleFor(x => x)
            .Must(HasValidDate)
            .WithName(nameof(PollsRequest.EndsAt))
            .WithMessage("{PropertyName} should be Greater than or Equal Start Date");
    }

    private bool HasValidDate(PollsRequest polls) => polls.EndsAt >= polls.StartsAt;
}
