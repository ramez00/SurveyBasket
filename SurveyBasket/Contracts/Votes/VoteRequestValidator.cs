namespace SurveyBasket.Contracts.Votes;

public class VoteRequestValidator : AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {
        RuleFor(x => x.Answers)
            .NotEmpty();

        // to validate all child in parent  
        RuleForEach(x => x.Answers).SetInheritanceValidator(v => v.Add(new VoteAnswerRequestValidator()));
    }
}