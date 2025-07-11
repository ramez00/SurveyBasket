﻿
using Microsoft.AspNetCore.Http.HttpResults;
using SurveyBasket.Errors;
using SurveyBasket.Models;
using SurveyBasket.Persistence;
using System.Collections;
using System.Collections.Generic;

namespace SurveyBasket.Services;

public class PollServices(ApplicationDbContext context) : IPollServices
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<PollsResponse>> GetAllAsync(CancellationToken token = default)
    {
        var polls = await _context.Polls.AsNoTracking().ToListAsync(token);
        return polls.Adapt<IEnumerable<PollsResponse>>();
    }

    public async Task<IEnumerable<PollsResponse>> GetCurretnAsync(CancellationToken token = default)
    {
        return await _context.Polls
            .Where(x => x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) 
                                      && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
            .AsNoTracking()
            .ProjectToType<PollsResponse>()
            .ToListAsync(token);
    }

    public async Task<IEnumerable<PollsResponseV2>> GetCurretnAsyncV2(CancellationToken token = default)
    {
        return await _context.Polls
            .Where(x => x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
                                      && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
            .AsNoTracking()
            .ProjectToType<PollsResponseV2>()
            .ToListAsync(token);
    }

    public async Task<Result<PollsResponse>> GetByIdAsync(int Id,CancellationToken token = default) 
    {
        var poll = await _context.Polls.FindAsync(Id, token);

        if(poll is not null)
            return Result.Success(poll.Adapt<PollsResponse>());
        else
            return Result.Failure<PollsResponse>(PollErrors.PollNotFound);
    }
        

    public async Task<Result<PollsResponse>> AddPollAsync(PollsRequest request, CancellationToken token = default)
    {
        var isExisting = await _context.Polls.AnyAsync(x => x.Title == request.Title, cancellationToken: token);

        if (isExisting)
            return Result.Failure<PollsResponse>(PollErrors.PollDuplicated);

        var poll = request.Adapt<Poll>();

        await _context.Polls.AddAsync(poll, token);
        await _context.SaveChangesAsync(token);

        return Result.Success(poll.Adapt<PollsResponse>());
    }

    public async Task<Result> UpdateAsync(int id, PollsRequest request, CancellationToken token = default)
    {
        var isExisting = await _context.Polls.AnyAsync(x => x.Title == request.Title && x.Id != id, cancellationToken: token);

        if (isExisting)
            return Result.Failure<PollsResponse>(PollErrors.PollDuplicated);

        var currentPoll = await _context.Polls.FindAsync(id, token);

        if (currentPoll is null)
            return Result.Failure(PollErrors.PollNotFound);

        currentPoll.Title = request.Title;
        currentPoll.StartsAt = request.StartsAt;
        currentPoll.EndsAt = request.EndsAt;
        currentPoll.Summary = request.Summary;

        _context.Polls.Update(currentPoll);
        await _context.SaveChangesAsync(token);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int Id, CancellationToken token = default)
    {
        var poll = await _context.Polls.FindAsync(Id, token);

        if (poll is null)
            return Result.Failure(Errors.PollErrors.PollNotFound);

        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync(token);

        return Result.Success();
    }

    public async Task<Result> TogglePublishAsync(int Id, CancellationToken token = default)
    {
        var poll = await _context.Polls.FindAsync(Id, token);

        if (poll is null)
            return Result.Failure(Errors.PollErrors.PollNotFound);

        poll.IsPublished = !poll.IsPublished;
        await _context.SaveChangesAsync(token);

        return Result.Success();

    }
}
