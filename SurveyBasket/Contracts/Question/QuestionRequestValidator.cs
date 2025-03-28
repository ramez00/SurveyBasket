namespace SurveyBasket.Contracts.Question;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .Length(3, 1000);

        RuleFor(x => x.Answers)
            .NotNull();

        RuleFor(x => x.Answers)
            .Must(x => x.Count > 1)
            .WithMessage("Question should has at least 2 answers")
            .When(x => x.Answers != null);

        RuleFor(x => x.Answers)
            .Must(x => x.Distinct().Count() == x.Count())  // number of answer should be Equal Number of Unique Answer
            .WithMessage("can not add Dublicated Answers for same question")
            .When(x => x.Answers != null);

    }
}