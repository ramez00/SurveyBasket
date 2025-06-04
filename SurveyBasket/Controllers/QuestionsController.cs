using SurveyBasket.Contracts.Common;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;

    [HttpGet("GetAll")]
    public async Task<IActionResult> getAll(int pollId , [FromQuery] RequestFilters filters ,CancellationToken cancellationToken)
    {
        var ques = await _questionService.GetAllAsync(pollId,filters, cancellationToken);

        return ques.IsSuccess 
            ? Ok(ques) 
            : ques.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(int PollId,int Id,CancellationToken cancellation)
    {
        var getQuestion = await _questionService.GetByIdAsync(PollId, Id, cancellation);

        return getQuestion.IsSuccess 
            ? Ok(getQuestion.Value)
            : getQuestion.ToProblem(StatusCodes.Status404NotFound);
    }
    
    [HttpGet("{id}")]
    public IActionResult Get()
    {
        return Ok();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add(int pollId,
                                            [FromBody] QuestionRequest request ,
                                            CancellationToken cancellationToken)
    {
        var res = await _questionService.AddAsync(pollId, request , cancellationToken);

        return res.IsSuccess 
            ? CreatedAtAction(nameof(Get),new {pollId  = pollId , id = res.Value!.Id },res.Value) 
            : res.ToProblem(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("")]
    public async Task<IActionResult> Update(int pollId,int id,
                                           [FromBody] QuestionRequest request,
                                           CancellationToken cancellationToken)
    {
        var res = await _questionService.UpdateAsync(pollId,id, request, cancellationToken);

        return res.IsSuccess
            ? NoContent()
            : res.ToProblem(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("{id}/toggleStatus")]
    public async Task<IActionResult> toggleStatus(int PollId, int Id, CancellationToken cancellation)
    {
        var isToggled = await _questionService.ToggleStatusAsync(PollId, Id, cancellation);

        return isToggled.IsSuccess
            ? NoContent()
            : isToggled.ToProblem(StatusCodes.Status404NotFound);
    }
}
