namespace SurveyBasket.Contracts.Result;

public record VotePerDayResponse(
    DateOnly voteDate,
    int totalNumber
);