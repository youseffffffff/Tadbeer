using BackEndApp.BusinessLogic;
using BackEndApp.Models.FamilyBudgets;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/family-budgets")]
public class FamilyBudgetsController : ControllerBase
{
    private readonly FamilyBudgetService _familyBudgetService;

    public FamilyBudgetsController(FamilyBudgetService familyBudgetService)
    {
        _familyBudgetService = familyBudgetService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var familyBudgets = await _familyBudgetService.GetAllAsync();

        return Ok(familyBudgets.Select(familyBudget => familyBudget.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var familyBudget = await _familyBudgetService.GetByIdAsync(id);

        if (familyBudget is null)
        {
            return NotFound();
        }

        return Ok(familyBudget.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] FamilyBudgetDto? familyBudgetDto)
    {
        if (familyBudgetDto is null)
        {
            return BadRequest();
        }

        var familyBudget = familyBudgetDto.ToModel();
        var createdFamilyBudget = await _familyBudgetService.AddAsync(familyBudget);

        if (createdFamilyBudget is null)
        {
            return BadRequest();
        }

        return Ok(createdFamilyBudget.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] FamilyBudgetDto? familyBudgetDto)
    {
        if (familyBudgetDto is null)
        {
            return BadRequest();
        }

        var existingFamilyBudget = await _familyBudgetService.GetByIdAsync(id);

        if (existingFamilyBudget is null)
        {
            return NotFound();
        }

        var familyBudget = familyBudgetDto.ToModel();
        familyBudget.BudgetId = id;

        var wasUpdated = await _familyBudgetService.UpdateAsync(familyBudget);

        if (!wasUpdated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingFamilyBudget = await _familyBudgetService.GetByIdAsync(id);

        if (existingFamilyBudget is null)
        {
            return NotFound();
        }

        var wasDeleted = await _familyBudgetService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
