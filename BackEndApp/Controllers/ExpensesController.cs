using BackEndApp.BusinessLogic;
using BackEndApp.Models.Expenses;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpensesController : ControllerBase
{
    private readonly ExpenseService _expenseService;

    public ExpensesController(ExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var expenses = await _expenseService.GetAllAsync();

        return Ok(expenses.Select(expense => expense.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var expense = await _expenseService.GetByIdAsync(id);

        if (expense is null)
        {
            return NotFound();
        }

        return Ok(expense.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] ExpenseDto? expenseDto)
    {
        if (expenseDto is null)
        {
            return BadRequest();
        }

        var expense = expenseDto.ToModel();
        var createdExpense = await _expenseService.AddAsync(expense);

        if (createdExpense is null)
        {
            return BadRequest();
        }

        return Ok(createdExpense.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] ExpenseDto? expenseDto)
    {
        if (expenseDto is null)
        {
            return BadRequest();
        }

        var existingExpense = await _expenseService.GetByIdAsync(id);

        if (existingExpense is null)
        {
            return NotFound();
        }

        var expense = expenseDto.ToModel();
        expense.ExpenseId = id;

        var wasUpdated = await _expenseService.UpdateAsync(expense);

        if (!wasUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingExpense = await _expenseService.GetByIdAsync(id);

        if (existingExpense is null)
        {
            return NotFound();
        }

        var wasDeleted = await _expenseService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
