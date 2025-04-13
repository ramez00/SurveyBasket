using SurveyBasket.Contracts.Votes;

namespace SurveyBasket.Services;

public class VoteServices(ApplicationDbContext context) : IVoteServices
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> AddUserAnswerAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
    {

        var hasVote = await _context.Votes.AnyAsync(x => x.PollID == pollId && x.UserId == userId,cancellationToken);

        if (hasVote)
            return Result.Failure(VoteErrors.VoteNotFound);

        var pollIsExist = await _context.Polls
            .AnyAsync(x => x.Id == pollId &&
                x.IsPublished 
                && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) 
                && x.EndsAt <= DateOnly.FromDateTime(DateTime.UtcNow),cancellationToken);

        if (!pollIsExist)
            return Result.Failure(PollErrors.PollNotFound);
        
        var questionIds = await _context.Questions
            .Where(x => x.PollId == pollId && x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if(!request.Answers.Select(a => a.QuestionId).SequenceEqual(questionIds))
            return Result.Failure(VoteErrors.InvalidQuestions);

        var vote = new Vote
        {
            PollID = pollId,
            UserId = userId,
            VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };

        await _context.AddAsync(vote,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
