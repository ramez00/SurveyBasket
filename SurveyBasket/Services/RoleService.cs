
using System.Security.Claims;

namespace SurveyBasket.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager,ApplicationDbContext context) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<RoleResponse>> GetRolesAsync(bool? isActive = false, CancellationToken cancellationToken = default)

        => await _roleManager.Roles
             .Where(role => !role.IsDefault && (!role.IsDeleted == isActive || (isActive == true)))
             .ProjectToType<RoleResponse>()
             .ToListAsync(cancellationToken);

    public async Task<Result<RoleDetailsResponse>> GetByIdAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

        var permissions = await _roleManager.GetClaimsAsync(role);

        var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted, permissions.Select(p => p.Value));

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

        var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted, request.permissions);

        return Result.Success(response);
    }

    public async Task<Result<RoleDetailsResponse>> UpdateAsync(string id, RoleRequest request)
    {
        var roleExistsWithSameName = await _roleManager.RoleExistsAsync(request.Name);

        if (roleExistsWithSameName)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleExist);

        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

        role.Name = request.Name;

        var result = await _roleManager.UpdateAsync(role);

        if (result.Succeeded)
        {
            // 1. new Permission to add 2.variance between current permission and user permission to delete

            var currentPermission = await _context.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == "permission")
                .Select(rc => rc.ClaimValue)
                .ToListAsync();

            var newPermissions =
                currentPermission
                .Except(currentPermission)  // extenstion methoud to get differance 
                .Select(newPermissions => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = "permission",
                    ClaimValue = newPermissions
                });

            var removedPermissions = currentPermission.Except(request.permissions);

            await _context.RoleClaims
                .Where(rc => rc.RoleId == id && rc.ClaimType == "permission" && removedPermissions.Contains(rc.ClaimValue))
                .ExecuteDeleteAsync();

            await _context.RoleClaims.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();

            return Result.Success(new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted, request.permissions));
        }

        var error = result.Errors.First();

        return Result.Failure<RoleDetailsResponse>(new Error(error.Code.ToString(),error.Description));

    }

    public async Task<Result> ChangeToggleStatus(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return Result.Failure(RoleErrors.RoleNotFound);
        
        role.IsDeleted = !role.IsDeleted;

        var result = await _roleManager.UpdateAsync(role);
        
        if (!result.Succeeded)
            return Result.Failure(new Error(result.Errors.First().Code, result.Errors.First().Description));
        
        return Result.Success();
    }
}
 