using SurveyBasket.Contracts.Votes;
using System.Collections.Generic;

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
                && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow),cancellationToken);

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

    public async Task<Result<PollVoteResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls
              .Where(p => p.Id == pollId)
              .Select(p => new PollVoteResponse(
                  p.Title,
                  p.Votes.Select(v => new VoteResponse(
                      $"{v.User.FirstName} {v.User.LastName}",
                      v.SubmittedOn,
                      v.VoteAnswers.Select(a => new QuestionAnswerResponse(
                          a.Question.Content,
                          a.Answer.Content
                          ))
                      ))
              ))
              .SingleOrDefaultAsync(cancellationToken);

        return poll is null
            ? Result.Failure<PollVoteResponse>(PollErrors.PollNotFound)
            : Result.Success<PollVoteResponse>(poll);
    }

    public async Task<Result<IEnumerable<VotePerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var votePerDay = await _context.Votes
            .Where(v => v.PollID == pollId)
            .GroupBy(v => new { votedDate = DateOnly.FromDateTime(v.SubmittedOn) })
            .Select(g => new VotePerDayResponse(g.Key.votedDate, g.Count()))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotePerDayResponse>>(votePerDay);

    }
}
