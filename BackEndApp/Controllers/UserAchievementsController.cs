using BackEndApp.BusinessLogic;
using BackEndApp.Models.UserAchievements;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/user-achievements")]
public class UserAchievementsController : ControllerBase
{
    private readonly UserAchievementService _userAchievementService;

    public UserAchievementsController(UserAchievementService userAchievementService)
    {
        _userAchievementService = userAchievementService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var userAchievements = await _userAchievementService.GetAllAsync();

        return Ok(userAchievements.Select(achievement => achievement.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var userAchievement = await _userAchievementService.GetByIdAsync(id);

        if (userAchievement is null)
        {
            return NotFound();
        }

        return Ok(userAchievement.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] UserAchievementDto? userAchievementDto)
    {
        if (userAchievementDto is null)
        {
            return BadRequest();
        }

        var userAchievement = userAchievementDto.ToModel();
        var createdAchievement = await _userAchievementService.AddAsync(userAchievement);

        if (createdAchievement is null)
        {
            return BadRequest();
        }

        return Ok(createdAchievement.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        [FromBody] UserAchievementDto? userAchievementDto)
    {
        if (userAchievementDto is null)
        {
            return BadRequest();
        }

        var existingAchievement = await _userAchievementService.GetByIdAsync(id);

        if (existingAchievement is null)
        {
            return NotFound();
        }

        var userAchievement = userAchievementDto.ToModel();
        userAchievement.UserAchievementId = id;
        userAchievement.UserId = existingAchievement.UserId;
        userAchievement.AchievementId = existingAchievement.AchievementId;
        userAchievement.EarnedAt = existingAchievement.EarnedAt;

        var wasUpdated = await _userAchievementService.UpdateAsync(userAchievement);

        if (!wasUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingAchievement = await _userAchievementService.GetByIdAsync(id);

        if (existingAchievement is null)
        {
            return NotFound();
        }

        var wasDeleted = await _userAchievementService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
