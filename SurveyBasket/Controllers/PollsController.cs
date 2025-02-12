using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SurveyBasket.Contracts.Polls;
using SurveyBasket.Persistence;

namespace SurveyBasket.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollServices polls) : ControllerBase
{
    private readonly IPollServices _pollService = polls;

    [HttpGet("")]
    public async Task<IActionResult> Getall(CancellationToken token)
    {
        var polls = await _pollService.GetAllAsync(token);

        return Ok(polls.Adapt<IEnumerable<PollsResponse>>());
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id,CancellationToken token)
    {
        var poll = await _pollService.GetByIdAsync(id,token);

        if(poll is null)
            return NotFound();

        return Ok(poll.Adapt<PollsResponse>());
    }

    [HttpPost("")]
    public async Task<IActionResult> add(PollsRequest pollsRequest,CancellationToken token)
    {
        var poll = pollsRequest.Adapt<Poll>();
        await _pollService.AddAsync(poll,token);
        return Ok(poll);
    }

    [HttpPut("")]
    public async Task<IActionResult> Update(int id,Poll poll,CancellationToken token)
    {
        await _pollService.UpdateAsync(id, poll,token);
        return Ok(poll);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id,CancellationToken token)
    {
        await _pollService.DeleteAsync(id,token);
        return Ok();
    }

    [HttpPut("TogglePublishAsync/{Id}")]
    public async Task<IActionResult> TogglePublishAsync(int Id, CancellationToken token = default)
    {
        var isUpdated = await _pollService.TogglePublishAsync(Id,token);
        
        if(!isUpdated)
            return NotFound();

        return Ok(isUpdated);
    }
}
