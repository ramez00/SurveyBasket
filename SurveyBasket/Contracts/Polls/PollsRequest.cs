namespace SurveyBasket.Contracts.Polls;

public record PollsRequest(string Title,
    string Summary,
    DateOnly StartsAt,
    DateOnly EndsAt);