using SurveyBasket.Contracts.Votes;

namespace SurveyBasket.Services;

public interface IVoteServices
{
    Task<Result> AddUserAnswerAsync(int pollId,string userId,VoteRequest request , CancellationToken cancellationToken = default);
}