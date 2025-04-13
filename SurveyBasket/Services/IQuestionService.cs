
namespace SurveyBasket.Services;

public interface IQuestionService
{
    Task<Result<QuestionResponse>> AddAsync(int PollId,QuestionRequest request,CancellationToken cancellationToken);
    Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int PollId,string userId,CancellationToken cancellationToken);
    Task<Result> UpdateAsync(int PollId,int Id,QuestionRequest request,CancellationToken cancellationToken);
    Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int PollId,CancellationToken cancellationToken);
    Task<Result<QuestionResponse>> GetByIdAsync(int PollId, int Id,CancellationToken cancellationToken);
    Task<Result> ToggleStatusAsync(int PollId,int Id, CancellationToken cancellationToken);
}
