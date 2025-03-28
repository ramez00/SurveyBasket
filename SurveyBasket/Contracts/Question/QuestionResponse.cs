using SurveyBasket.Contracts.Answers;

namespace SurveyBasket.Contracts.Question;

public record QuestionResponse(
    int Id,
    string Content,
    IEnumerable<AnswerResponse> Answers
);