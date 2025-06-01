namespace SurveyBasket.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetRolesAsync(bool? isActive = true, CancellationToken cancellationToken = default);

    Task<Result<RoleDetailsResponse>> GetByIdAsync(string id);
    Task<Result<RoleDetailsResponse>> AddAsync(RoleRequest request);
    Task<Result<RoleDetailsResponse>> UpdateAsync(string id, RoleRequest request);
    Task<Result> ChangeToggleStatus(string id);
}
