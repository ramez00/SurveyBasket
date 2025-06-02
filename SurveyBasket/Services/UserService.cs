

namespace SurveyBasket.Services;

public class UserService(UserManager<ApplicationUser> userManager,
    IRoleService roleService,
    ApplicationDbContext context) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;
    private readonly IRoleService _roleService = roleService;

    public async Task<Result<IEnumerable<UserResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await (from u in _context.Users
                           join ur in _context.UserRoles on u.Id equals ur.UserId
                           join r in _context.Roles on ur.RoleId equals r.Id into roles
                           where roles.Any(role => role.Name != DefaultRole.Member) // Exclude Members
                           select new
                           {
                               u.Id,
                               u.FirstName,
                               u.LastName,
                               u.Email,
                               u.IsDisabled,
                               Roles = roles.Select(x => x.Name!).ToList()
                           })
                           .GroupBy(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsDisabled })
                           .Select(u => new UserResponse
                           (
                               u.Key.Id,
                               u.Key.FirstName,
                               u.Key.LastName,
                               u.Key.Email,
                               u.Key.IsDisabled,
                               u.SelectMany(x => x.Roles)
                           )).ToListAsync();

        return Result.Success<IEnumerable<UserResponse>>(users);
    }

    public async Task<Result<UserResponse>> GetUserDetialsAsync(string userId)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        var roles = await _userManager.GetRolesAsync(user);

        var response = (user, roles)
            .Adapt<UserResponse>();

        return Result.Success(response);
    }

    public async Task<Result<UserResponse>> AddUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var IsUserExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (IsUserExist)
            return Result.Failure<UserResponse>(UserErrors.UserExist);

        var user = request.Adapt<ApplicationUser>();

        var roles = await _roleService.GetRolesAsync(cancellationToken: cancellationToken);

        if (request.roles.Except(roles.Select(x => x.Name!)).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<UserResponse>(new Error(error.Code, error.Description));
        }

        var roleResult = await _userManager.AddToRolesAsync(user, request.roles);
        if (!roleResult.Succeeded)
        {
            var error = roleResult.Errors.First();
            return Result.Failure<UserResponse>(new Error(error.Code, error.Description));
        }

        // Return the created user response
        var userResponse = (user, request.roles)
            .Adapt<UserResponse>();

        return Result.Success(userResponse);
    }

    public async Task<Result> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var IsUserExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != userId, cancellationToken);

        if (IsUserExist)
            return Result.Failure(UserErrors.UserExist);

        var roles = await _roleService.GetRolesAsync(cancellationToken: cancellationToken);

        if (request.roles.Except(roles.Select(x => x.Name!)).Any())
            return Result.Failure(UserErrors.InvalidRoles);

        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        user = request.Adapt(user);

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description));
        }

        // Remove existing roles
        await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .ExecuteDeleteAsync();

        var roleResult = await _userManager.AddToRolesAsync(user, request.roles);
        if (!roleResult.Succeeded)
        {
            var error = roleResult.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description));
        }

        return Result.Success();
    }

    public async Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId)
    {
        var user = await _userManager.Users
                        .Where(x => x.Id == userId)
                        .ProjectToType<UserProfileResponse>()  // Change to DbContext Instead User Managment  to Get Needed Data Only  
                        .SingleAsync();

        return Result.Success(user);
    }

    public async Task<Result> UpdateProfileAsync(string userId, UserProfileRequest request)
    {
        //var user = await _userManager.FindByIdAsync(userId);

        //if (user == null)
        //    return Result.Failure(new Error("UserNotFound", "User not found"));

        //user = request.Adapt(user);

        //await _userManager.UpdateAsync(user);

        // to Update without Loading the User Object in Memory

        await _userManager.Users
            .Where(usr => usr.Id == userId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(usr => usr.FirstName, request.FirstName)
                .SetProperty(usr => usr.LastName, request.LastName)
            );

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var Res = await _userManager.ChangePasswordAsync(user!, request.currentPassword, request.NewPassword);

        if (Res.Succeeded)
            return Result.Success();

        var error = Res.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description));
    }

    public async Task<Result> ToggleStatus(string userId, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        user.IsDisabled = !user.IsDisabled;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description));
        }
        return Result.Success();
    }

    public async Task<Result> UnlockUser(string userId, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        user.LockoutEnabled = true;
        user.LockoutEnd = null; // Unlock the user by setting LockoutEnd to null
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description));
        }
        return Result.Success();
    }
}
