using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Contracts.Polls;
using SurveyBasket.Persistence;
using System.Reflection;

namespace SurveyBasket.Controllers;

[ApiVersion(1)]
[ApiVersion(2)]
[Authorize]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public class PollsController(IPollServices polls) : ControllerBase
{
    private readonly IPollServices _pollService = polls;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        return Ok(await _pollService.GetAllAsync(token));
    }

    [MapToApiVersion(1)]
    [HttpGet("Current")]
    [EnableRateLimiting(RateLimiterConstant.UserRateLimiter)]
    public async Task<IActionResult> GetCurrent(CancellationToken token)
    {
        return Ok(await _pollService.GetCurretnAsync(token));
    }

    [MapToApiVersion(2)]
    [HttpGet("Current")]
    [EnableRateLimiting(RateLimiterConstant.UserRateLimiter)]
    public async Task<IActionResult> GetCurrentV2(CancellationToken token)
    {
        return Ok(await _pollService.GetCurretnAsyncV2(token));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id,CancellationToken token)
    {
        var res = await _pollService.GetByIdAsync(id,token);

        return res.IsSuccess 
            ? Ok(res.Value)
            : res.ToProblem(StatusCodes.Status400BadRequest);
    }

    [HttpPost("")]
    public async Task<IActionResult> add(PollsRequest request, CancellationToken token)
    {
        var res = await _pollService.AddPollAsync(request, token);

        return res.IsSuccess 
            ? CreatedAtAction(nameof(Get),new { id = res.Value!.Id } , res.Value) 
            : res.ToProblem(StatusCodes.Status409Conflict);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PollsRequest request, CancellationToken token)
    {
        var resp = await _pollService.UpdateAsync(id, request, token);

        return resp.IsSuccess
            ? NoContent()
            : resp.ToProblem(StatusCodes.Status400BadRequest);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id, CancellationToken token)
    {
        var res = await _pollService.DeleteAsync(id, token);
        return res.IsSuccess 
            ? Ok()
            : res.ToProblem(StatusCodes.Status400BadRequest);
    }

    [HttpPut("TogglePublishAsync/{Id}")]
    public async Task<IActionResult> TogglePublishAsync(int Id, CancellationToken token = default)
    {
        var isUpdated = await _pollService.TogglePublishAsync(Id, token);

        return isUpdated.IsSuccess
            ? Ok()
            : isUpdated.ToProblem(StatusCodes.Status400BadRequest);
    }
}
