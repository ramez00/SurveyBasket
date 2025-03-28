namespace SurveyBasket.Contracts.Question;

public record QuestionRequest(
    string Content,
    List<string> Answers
);
