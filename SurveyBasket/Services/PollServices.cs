
using Microsoft.AspNetCore.Http.HttpResults;
using SurveyBasket.Persistence;

namespace SurveyBasket.Services;

public class PollServices(ApplicationDbContext context) : IPollServices
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IList<Poll>> GetAllAsync(CancellationToken token = default) =>
        await _context.Polls.AsNoTracking().ToListAsync(token);
    
    public async Task<Poll?> GetByIdAsync(int Id,CancellationToken token = default) =>
        await _context.Polls.FindAsync(Id,token);

    public async Task<Poll> AddAsync(Poll poll, CancellationToken token = default)
    {
        await _context.Polls.AddAsync(poll,token);
        await _context.SaveChangesAsync();
        return poll;
    }

    public async Task<bool> UpdateAsync(int id,Poll request,CancellationToken token)
    {
        var currentPoll = await GetByIdAsync(id,token);

        if (currentPoll is null)
            return false;

        currentPoll.Title = request.Title;
        currentPoll.IsPublished = request.IsPublished;
        currentPoll.StartsAt = request.StartsAt;
        currentPoll.EndsAt = request.EndsAt;
        currentPoll.Summary = request.Summary;

        _context.Polls.Update(currentPoll);
        await _context.SaveChangesAsync(token);

        return true;
    }

    public async Task<bool> DeleteAsync(int Id,CancellationToken token = default)
    {
        var poll = await GetByIdAsync(Id,token);
        
        if (poll is null)
            return false;

        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> TogglePublishAsync(int Id, CancellationToken token = default)
    {
        var currentPoll = await GetByIdAsync(Id,token);

        if (currentPoll is null)
            return false;

        currentPoll.IsPublished = !currentPoll.IsPublished;
        await _context.SaveChangesAsync(token);
        return true;

    }
}
