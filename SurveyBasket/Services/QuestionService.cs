using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace SurveyBasket.Services;

public class QuestionService(ApplicationDbContext dbContext,HybridCache hybridCache) : IQuestionService
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly HybridCache _hybridCache = hybridCache;

    private const string _cachePrefix = "AvliableQuestion";

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int PollId, CancellationToken cancellationToken = default)
    {
        var IsPollExist = await _dbContext.Polls.AnyAsync(x => x.Id == PollId, cancellationToken);

        if (!IsPollExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _dbContext.Questions
            .Where(x => x.PollId == PollId)
            .Include(quest => quest.Answers)
            .ProjectToType<QuestionResponse>()
            //.Select(a => new QuestionResponse (
            //    a.Id,
            //    a.Content,
            //    a.Answers.Select(ans => new AnswerResponse(ans.Id,ans.Content))
            //    ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (questions.Count == 0)
            return Result.Failure<IEnumerable<QuestionResponse>>(Errors.QuestionErrors.QuestionNotFoundWithPollId);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int PollId, string userId, CancellationToken cancellationToken)
    {
        //var hasVote = await _dbContext.Votes.AnyAsync(x => x.PollID == PollId && x.UserId == userId, cancellationToken);

        //if (hasVote)
        //    return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.VoteDuplicated);

        //var pollExist = await _dbContext.Polls
        //        .AnyAsync(x => x.Id == PollId &&
        //                  x.IsPublished &&
        //                  x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) &&
        //                  x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken
        //        );

        //if (!pollExist)
        //    return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var cacheKey = $"{_cachePrefix}-{PollId}";

        var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(
            cacheKey,
            async entry => await _dbContext.Questions
                .Where(x => x.PollId == PollId && x.IsActive)
                .Include(x => x.Answers)
                .Select(q => new QuestionResponse(
                    q.Id,
                    q.Content,
                    q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(a.Id, a.Content))
                ))
                .AsNoTracking()
                .ToListAsync(cancellationToken),
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5),
            }
        );

        return Result.Success<IEnumerable<QuestionResponse>>(questions!);
    }

    public async Task<Result<QuestionResponse>> GetByIdAsync(int PollId, int Id, CancellationToken cancellationToken = default)
    {
        var question = await _dbContext.Questions
                    .Where(x => x.Id == Id && x.PollId == PollId)
                    .Include(quest => quest.Answers)
                    .ProjectToType<QuestionResponse>()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(cancellationToken);

        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Success(question);
    }

    public async Task<Result<QuestionResponse>> AddAsync(int PollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var IsPollExist = await _dbContext.Polls.AnyAsync(x => x.Id == PollId, cancellationToken);

        if (!IsPollExist)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var IsQuestionExist = await _dbContext.Questions.AnyAsync(x => x.Content == request.Content, cancellationToken);

        if (IsQuestionExist)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionDuplicated);

        var question = request.Adapt<Question>();
        question.PollId = PollId;

        await _dbContext.AddAsync(question, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _hybridCache.RemoveAsync($"{_cachePrefix}-{PollId}", cancellationToken); // to Refresh the cache delete old one Once Updated Questions

        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result> UpdateAsync(int PollId, int Id, QuestionRequest request, CancellationToken cancellationToken)
    {

        var questionIsExist = await _dbContext.Questions
            .AnyAsync(x => x.PollId == PollId && x.Id != Id && x.Content == request.Content, cancellationToken);

        if (questionIsExist)
            return Result.Failure(QuestionErrors.QuestionDuplicated);

        var question = await _dbContext.Questions
            .Include(x => x.Answers)
            .SingleOrDefaultAsync(x => x.PollId == PollId && x.Id == Id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        var currentAnswer = question.Answers.Select(x => x.Content).ToList();

        var newAnswer = request.Answers.Except(currentAnswer).ToList();

        newAnswer.ForEach(ans =>
        {
            question.Answers.Add(new Answer { Content = ans });
        });

        question.Answers.ToList().ForEach(ans =>
        {
            ans.IsActive = request.Answers.Contains(ans.Content);
        });
        await _dbContext.SaveChangesAsync();

        await _hybridCache.RemoveAsync($"{_cachePrefix}-{PollId}", cancellationToken); // to Refresh the cache delete old one Once Updated Questions

        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int PollId, int Id, CancellationToken cancellationToken)
    {

        var question = await _dbContext.Questions
                    .SingleOrDefaultAsync(x => x.Id == Id && x.PollId == PollId, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _hybridCache.RemoveAsync($"{_cachePrefix}-{PollId}", cancellationToken); // to Refresh the cache delete old one Once Updated Questions

        return Result.Success();
    }
}
