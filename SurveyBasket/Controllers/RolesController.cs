using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace SurveyBasket.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync(bool? IsActive = false , CancellationToken cancellationToken = default)
    {
        var roles = await _roleService.GetRolesAsync(IsActive, cancellationToken);
        return Ok(roles);   
    }
}
