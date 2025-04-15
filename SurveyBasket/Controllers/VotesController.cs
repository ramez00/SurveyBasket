namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class VotesController(IQuestionService questionService,IVoteServices voteServices) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IVoteServices _voteService = voteServices;

    [HttpGet]
    [ResponseCache(Duration = 120)]
    public async Task<IActionResult> GetVote(int pollId,CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var vote = await _questionService.GetAvailableAsync(pollId, userId!, cancellationToken);

        if(vote.IsFailure)
            return vote.Error.Equals(VoteErrors.VoteDuplicated)
                  ? vote.ToProblem(StatusCodes.Status409Conflict)
                  : vote.ToProblem(StatusCodes.Status404NotFound);

        return Ok(vote.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Vote(int pollId, VoteRequest request,CancellationToken cancellationToken)
    {
        var result = await _voteService.AddUserAnswerAsync(pollId,User.GetUserId()!,request,cancellationToken);

        if (result.IsSuccess)
            return Created();

        return result.ToProblem(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("GetVotedPoll")]
    public async Task<IActionResult> GetVotedPoll(int pollId, CancellationToken cancellation)
    {
      var resp = await _voteService.GetPollVotesAsync(pollId, cancellation);
      
      return resp.IsSuccess ? Ok(resp) : resp.ToProblem(StatusCodes.Status500InternalServerError);

    }

    [HttpGet("GetVotesPerDay")]
    public async Task<IActionResult> GetVotesPerDay(int pollId, CancellationToken cancellation)
    {
        var resp = await _voteService.GetVotesPerDayAsync(pollId, cancellation);

        return resp.IsSuccess ? Ok(resp) : resp.ToProblem(StatusCodes.Status500InternalServerError);

    }
}
