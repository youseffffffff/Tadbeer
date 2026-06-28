using BackEndApp.BusinessLogic;
using BackEndApp.Models.GoalContributions;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/goal-contributions")]
public class GoalContributionsController : ControllerBase
{
    private readonly GoalContributionService _goalContributionService;

    public GoalContributionsController(GoalContributionService goalContributionService)
    {
        _goalContributionService = goalContributionService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var goalContributions = await _goalContributionService.GetAllAsync();

        return Ok(goalContributions.Select(contribution => contribution.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var goalContribution = await _goalContributionService.GetByIdAsync(id);

        if (goalContribution is null)
        {
            return NotFound();
        }

        return Ok(goalContribution.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] GoalContributionDto? goalContributionDto)
    {
        if (goalContributionDto is null)
        {
            return BadRequest();
        }

        var goalContribution = goalContributionDto.ToModel();
        var createdContribution = await _goalContributionService.AddAsync(goalContribution);

        if (createdContribution is null)
        {
            return BadRequest();
        }

        return Ok(createdContribution.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        [FromBody] GoalContributionDto? goalContributionDto)
    {
        if (goalContributionDto is null)
        {
            return BadRequest();
        }

        var existingContribution = await _goalContributionService.GetByIdAsync(id);

        if (existingContribution is null)
        {
            return NotFound();
        }

        var goalContribution = goalContributionDto.ToModel();
        goalContribution.ContributionId = id;
        goalContribution.GoalId = existingContribution.GoalId;

        var wasUpdated = await _goalContributionService.UpdateAsync(goalContribution);

        if (!wasUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingContribution = await _goalContributionService.GetByIdAsync(id);

        if (existingContribution is null)
        {
            return NotFound();
        }

        var wasDeleted = await _goalContributionService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
