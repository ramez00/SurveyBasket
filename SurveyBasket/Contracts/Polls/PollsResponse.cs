namespace SurveyBasket.Contracts.Polls;

public record PollsResponse(int Id,
    string Title,
    string Summary,
    bool IsPublished,
    DateOnly StartsAt,
    DateOnly EndsAt
);

public record PollsResponseV2(int Id,
    string Title,
    string Summary,
    DateOnly StartsAt,
    DateOnly EndsAt
);