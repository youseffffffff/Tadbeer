using BackEndApp.BusinessLogic;
using BackEndApp.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var users = await _userService.GetAllAsync();

        return Ok(users.Select(user => user.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] UserDto? userDto)
    {
        if (userDto is null)
        {
            return BadRequest();
        }

        var user = userDto.ToModel();
        var createdUser = await _userService.AddAsync(user);

        if (createdUser is null)
        {
            return BadRequest();
        }

        return Ok(createdUser.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UserDto? userDto)
    {
        if (userDto is null)
        {
            return BadRequest();
        }

        var existingUser = await _userService.GetByIdAsync(id);

        if (existingUser is null)
        {
            return NotFound();
        }

        var user = userDto.ToModel();
        user.UserId = id;
        user.PasswordHash = existingUser.PasswordHash;

        var wasUpdated = await _userService.UpdateAsync(user);

        if (!wasUpdated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingUser = await _userService.GetByIdAsync(id);

        if (existingUser is null)
        {
            return NotFound();
        }

        var wasDeleted = await _userService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
