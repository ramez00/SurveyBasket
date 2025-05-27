
namespace SurveyBasket.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<IEnumerable<RoleResponse>> GetRolesAsync(bool? isActive = false,CancellationToken cancellationToken = default)
        
        => await _roleManager.Roles
             .Where(role => !role.IsDefault && (!role.IsDelted == isActive || (isActive == true)))
             .ProjectToType<RoleResponse>()
             .ToListAsync(cancellationToken);

}
