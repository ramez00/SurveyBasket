namespace SurveyBasket.Services;

public interface IVoteServices
{
    Task<Result> AddUserAnswerAsync(int pollId,string userId,VoteRequest request , CancellationToken cancellationToken = default);

    Task<Result<PollVoteResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VotePerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default);
}