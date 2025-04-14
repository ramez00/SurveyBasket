namespace SurveyBasket.Contracts.Result;

public record VoteResponse(
    string VoterName,
    DateTime VoteDate,
    IEnumerable<QuestionAnswerResponse> SelectedAnswers
);