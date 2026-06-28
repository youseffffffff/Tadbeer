using BackEndApp.BusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly RoleService _roleService;

    public RolesController(RoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var roles = await _roleService.GetAllAsync();

        return Ok(roles.Select(role => role.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetByIdAsync(int id)
    {
        var role = await _roleService.GetByIdAsync(id);

        if (role is null)
        {
            return NotFound();
        }

        return Ok(role.ToDto());
    }
}
