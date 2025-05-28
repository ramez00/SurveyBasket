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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string id)
    {
        var result = await _roleService.GetByIdAsync(id);
        
        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> AddAsync([FromBody] RoleRequest request)
    {
        var result = await _roleService.AddAsync(request);

        if (result.IsFailure)
            return NotFound(result.Error);

        return CreatedAtAction(nameof(GetAllAsync),result.Value);
    }

    [HttpPut("")]
    public async Task<IActionResult> UpdateAsync(string id,[FromBody] RoleRequest request)
    {
        var result = await _roleService.UpdateAsync(id,request);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPut("Toggle-Status")]
    public async Task<IActionResult> toggleStatus(string id)
    {
        var result = await _roleService.ChangeToggleStatus(id);

        if (result.IsFailure)
            return NotFound(result.Error);

        return NoContent();
    }
}
