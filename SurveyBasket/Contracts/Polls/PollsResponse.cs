namespace SurveyBasket.Contracts.Polls;

public record PollsResponse(int Id,
    string Title,
    string Summary,
    bool IsPublished,
    DateOnly StartsAt,
    DateOnly EndsAt);
