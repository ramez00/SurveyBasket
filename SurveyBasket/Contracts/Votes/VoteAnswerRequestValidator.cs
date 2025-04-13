namespace SurveyBasket.Contracts.Votes;

public class VoteAnswerRequestValidator : AbstractValidator<VoteAnswerRequest>
{
    public VoteAnswerRequestValidator()
	{
		RuleFor(x => x.QuestionId)
			.GreaterThan(1);

		RuleFor(x => x.AnswerId)
			.GreaterThan(1);
	}
}
