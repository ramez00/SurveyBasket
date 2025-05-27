
namespace SurveyBasket.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<IEnumerable<RoleResponse>> GetRolesAsync(bool? isActive = false, CancellationToken cancellationToken = default)

        => await _roleManager.Roles
             .Where(role => !role.IsDefault && (!role.IsDelted == isActive || (isActive == true)))
             .ProjectToType<RoleResponse>()
             .ToListAsync(cancellationToken);

    public async Task<Result<RoleDetailsResponse>> GetByIdAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

        var permission = await _roleManager.GetClaimsAsync(role);

        var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDelted)
        {
            permissions = permission.Select(p => p.Value).ToList()
        };

        return Result.Success(response);
    }
}
