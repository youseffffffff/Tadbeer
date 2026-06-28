using BackEndApp.BusinessLogic;
using BackEndApp.Models.SavingGoals;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/saving-goals")]
public class SavingGoalsController : ControllerBase
{
    private readonly SavingGoalService _savingGoalService;

    public SavingGoalsController(SavingGoalService savingGoalService)
    {
        _savingGoalService = savingGoalService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var savingGoals = await _savingGoalService.GetAllAsync();

        return Ok(savingGoals.Select(goal => goal.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var savingGoal = await _savingGoalService.GetByIdAsync(id);

        if (savingGoal is null)
        {
            return NotFound();
        }

        return Ok(savingGoal.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] SavingGoalDto? savingGoalDto)
    {
        if (savingGoalDto is null)
        {
            return BadRequest();
        }

        var savingGoal = savingGoalDto.ToModel();
        var createdSavingGoal = await _savingGoalService.AddAsync(savingGoal);

        if (createdSavingGoal is null)
        {
            return BadRequest();
        }

        return Ok(createdSavingGoal.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] SavingGoalDto? savingGoalDto)
    {
        if (savingGoalDto is null)
        {
            return BadRequest();
        }

        var existingSavingGoal = await _savingGoalService.GetByIdAsync(id);

        if (existingSavingGoal is null)
        {
            return NotFound();
        }

        var savingGoal = savingGoalDto.ToModel();
        savingGoal.GoalId = id;

        var wasUpdated = await _savingGoalService.UpdateAsync(savingGoal);

        if (!wasUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingSavingGoal = await _savingGoalService.GetByIdAsync(id);

        if (existingSavingGoal is null)
        {
            return NotFound();
        }

        var wasDeleted = await _savingGoalService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
