
using System.Security.Claims;

namespace SurveyBasket.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager,ApplicationDbContext context) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;

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

        var permissions = await _roleManager.GetClaimsAsync(role);

        var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDelted, permissions.Select(p => p.Value));

        return Result.Success(response);
    }

    public async Task<Result<RoleDetailsResponse>> AddAsync(RoleRequest request)
    {
        var roleExists = await _roleManager.RoleExistsAsync(request.Name);

        if(roleExists)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleExist);


        var role = new ApplicationRole
        {
            Name = request.Name,    
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        var result = await _roleManager.CreateAsync(role);
   
        if (!result.Succeeded)
            return Result.Failure<RoleDetailsResponse>(new Error(result.Errors.First().Code,result.Errors.First().Description));

        var claims = request.permissions
            .Select(p => new IdentityRoleClaim<string>
            {
                RoleId = role.Id,
                ClaimType = "permission",
                ClaimValue = p
            });

        await _context.RoleClaims.AddRangeAsync(claims);
        await _context.SaveChangesAsync();

        var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDelted, request.permissions);

        return Result.Success(response);
    }
}
 