using BackEndApp.BusinessLogic;
using BackEndApp.Models.AchievementDefinitions;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/achievement-definitions")]
public class AchievementDefinitionsController : ControllerBase
{
    private readonly AchievementDefinitionService _achievementDefinitionService;

    public AchievementDefinitionsController(
        AchievementDefinitionService achievementDefinitionService)
    {
        _achievementDefinitionService = achievementDefinitionService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var achievements = await _achievementDefinitionService.GetAllAsync();

        return Ok(achievements.Select(achievement => achievement.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var achievement = await _achievementDefinitionService.GetByIdAsync(id);

        if (achievement is null)
        {
            return NotFound();
        }

        return Ok(achievement.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync(
        [FromBody] AchievementDefinitionDto? achievementDefinitionDto)
    {
        if (achievementDefinitionDto is null)
        {
            return BadRequest();
        }

        var achievement = achievementDefinitionDto.ToModel();
        var createdAchievement = await _achievementDefinitionService.AddAsync(achievement);

        if (createdAchievement is null)
        {
            return BadRequest();
        }

        return Ok(createdAchievement.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        [FromBody] AchievementDefinitionDto? achievementDefinitionDto)
    {
        if (achievementDefinitionDto is null)
        {
            return BadRequest();
        }

        var existingAchievement = await _achievementDefinitionService.GetByIdAsync(id);

        if (existingAchievement is null)
        {
            return NotFound();
        }

        var achievement = achievementDefinitionDto.ToModel();
        achievement.AchievementId = id;

        var wasUpdated = await _achievementDefinitionService.UpdateAsync(achievement);

        if (!wasUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingAchievement = await _achievementDefinitionService.GetByIdAsync(id);

        if (existingAchievement is null)
        {
            return NotFound();
        }

        var wasDeleted = await _achievementDefinitionService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
