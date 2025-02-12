namespace SurveyBasket.Services;

public interface IPollServices
{
    Task<IList<Poll>> GetAllAsync(CancellationToken token = default);
    Task<Poll?> GetByIdAsync(int Id,CancellationToken token = default);
    Task<Poll> AddAsync(Poll poll,CancellationToken token = default);
    Task<bool> UpdateAsync(int id,Poll poll, CancellationToken token = default);
    Task<bool> DeleteAsync(int Id,CancellationToken token = default);
    Task<bool> TogglePublishAsync(int Id,CancellationToken token = default);
}
