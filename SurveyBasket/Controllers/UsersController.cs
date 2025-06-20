﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Utilities;

namespace SurveyBasket.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    public async Task<IActionResult> GetAllasync()
    {
        var users = await _userService.GetAllAsync();
        return users.IsSuccess
            ? Ok(users.Value)
            : users.ToProblem(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserDetailsAsync(string userId)
    {
        var user = await _userService.GetUserDetialsAsync(userId);
        return user.IsSuccess
            ? Ok(user.Value)
            : user.ToProblem(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("")]
    public async Task<IActionResult> AddAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.AddUserAsync(request,cancellationToken);

        return user.IsSuccess
            ? Ok(user.Value)
            : user.ToProblem(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUserAsync(string userId,UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateUserAsync(userId,request,cancellationToken);
        return user.IsSuccess
            ? NoContent()
            : user.ToProblem(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("toggle-status/{userId}")]
    public async Task<IActionResult> ToggleStatusUserAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userService.ToggleStatus(userId,cancellationToken);
        return user.IsSuccess
            ? NoContent()
            : user.ToProblem(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("Unlock/{userId}")]
    public async Task<IActionResult> UnlockUserAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userService.UnlockUser(userId,cancellationToken);
        return user.IsSuccess
            ? NoContent()
            : user.ToProblem(StatusCodes.Status500InternalServerError);
    }
}
