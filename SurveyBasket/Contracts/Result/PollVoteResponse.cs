namespace SurveyBasket.Contracts.Result;

public record PollVoteResponse(
    string title,
    IEnumerable<VoteResponse> Votes
);