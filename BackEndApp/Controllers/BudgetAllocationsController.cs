using BackEndApp.BusinessLogic;
using BackEndApp.Models.BudgetAllocations;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/budget-allocations")]
public class BudgetAllocationsController : ControllerBase
{
    private readonly BudgetAllocationService _budgetAllocationService;

    public BudgetAllocationsController(BudgetAllocationService budgetAllocationService)
    {
        _budgetAllocationService = budgetAllocationService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var budgetAllocations = await _budgetAllocationService.GetAllAsync();

        return Ok(budgetAllocations.Select(allocation => allocation.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var budgetAllocation = await _budgetAllocationService.GetByIdAsync(id);

        if (budgetAllocation is null)
        {
            return NotFound();
        }

        return Ok(budgetAllocation.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] BudgetAllocationDto? budgetAllocationDto)
    {
        if (budgetAllocationDto is null)
        {
            return BadRequest();
        }

        var budgetAllocation = budgetAllocationDto.ToModel();
        var createdBudgetAllocation = await _budgetAllocationService.AddAsync(budgetAllocation);

        if (createdBudgetAllocation is null)
        {
            return BadRequest();
        }

        return Ok(createdBudgetAllocation.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        [FromBody] BudgetAllocationDto? budgetAllocationDto)
    {
        if (budgetAllocationDto is null)
        {
            return BadRequest();
        }

        var existingBudgetAllocation = await _budgetAllocationService.GetByIdAsync(id);

        if (existingBudgetAllocation is null)
        {
            return NotFound();
        }

        var budgetAllocation = budgetAllocationDto.ToModel();
        budgetAllocation.AllocationId = id;

        var wasUpdated = await _budgetAllocationService.UpdateAsync(budgetAllocation);

        if (!wasUpdated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingBudgetAllocation = await _budgetAllocationService.GetByIdAsync(id);

        if (existingBudgetAllocation is null)
        {
            return NotFound();
        }

        var wasDeleted = await _budgetAllocationService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
