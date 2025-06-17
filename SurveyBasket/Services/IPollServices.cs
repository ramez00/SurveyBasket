namespace SurveyBasket.Services;

public interface IPollServices
{
    Task<IEnumerable<PollsResponse>> GetAllAsync(CancellationToken token = default);
    Task<IEnumerable<PollsResponse>> GetCurretnAsync(CancellationToken token = default);
    Task<IEnumerable<PollsResponseV2>> GetCurretnAsyncV2(CancellationToken token = default);
    Task<Result<PollsResponse>> GetByIdAsync(int Id,CancellationToken token = default);
    Task<Result<PollsResponse>> AddPollAsync(PollsRequest poll,CancellationToken token = default);
    Task<Result> UpdateAsync(int id, PollsRequest request, CancellationToken token = default);
    Task<Result> DeleteAsync(int Id, CancellationToken token = default);
    Task<Result> TogglePublishAsync(int Id, CancellationToken token = default);
}
