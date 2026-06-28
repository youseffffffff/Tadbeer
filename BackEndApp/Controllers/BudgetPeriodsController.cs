using BackEndApp.BusinessLogic;
using BackEndApp.Models.BudgetPeriods;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/budget-periods")]
public class BudgetPeriodsController : ControllerBase
{
    private readonly BudgetPeriodService _budgetPeriodService;

    public BudgetPeriodsController(BudgetPeriodService budgetPeriodService)
    {
        _budgetPeriodService = budgetPeriodService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var budgetPeriods = await _budgetPeriodService.GetAllAsync();

        return Ok(budgetPeriods.Select(period => period.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var budgetPeriod = await _budgetPeriodService.GetByIdAsync(id);

        if (budgetPeriod is null)
        {
            return NotFound();
        }

        return Ok(budgetPeriod.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] BudgetPeriodDto? budgetPeriodDto)
    {
        if (budgetPeriodDto is null)
        {
            return BadRequest();
        }

        var budgetPeriod = budgetPeriodDto.ToModel();
        var createdBudgetPeriod = await _budgetPeriodService.AddAsync(budgetPeriod);

        if (createdBudgetPeriod is null)
        {
            return BadRequest();
        }

        return Ok(createdBudgetPeriod.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] BudgetPeriodDto? budgetPeriodDto)
    {
        if (budgetPeriodDto is null)
        {
            return BadRequest();
        }

        var existingBudgetPeriod = await _budgetPeriodService.GetByIdAsync(id);

        if (existingBudgetPeriod is null)
        {
            return NotFound();
        }

        var budgetPeriod = budgetPeriodDto.ToModel();
        budgetPeriod.PeriodId = id;

        var wasUpdated = await _budgetPeriodService.UpdateAsync(budgetPeriod);

        if (!wasUpdated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingBudgetPeriod = await _budgetPeriodService.GetByIdAsync(id);

        if (existingBudgetPeriod is null)
        {
            return NotFound();
        }

        var wasDeleted = await _budgetPeriodService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
