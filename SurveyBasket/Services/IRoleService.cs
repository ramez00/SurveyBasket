namespace SurveyBasket.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetRolesAsync(bool? isActive = false, CancellationToken cancellationToken = default);

    Task<Result<RoleDetailsResponse>> GetByIdAsync(string id);
}
