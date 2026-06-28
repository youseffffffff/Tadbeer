using BackEndApp.BusinessLogic;
using BackEndApp.Models.ExpenseCategories;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/expense-categories")]
public class ExpenseCategoriesController : ControllerBase
{
    private readonly ExpenseCategoryService _expenseCategoryService;

    public ExpenseCategoriesController(ExpenseCategoryService expenseCategoryService)
    {
        _expenseCategoryService = expenseCategoryService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var expenseCategories = await _expenseCategoryService.GetAllAsync();

        return Ok(expenseCategories.Select(category => category.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetByIdAsync(int id)
    {
        var expenseCategory = await _expenseCategoryService.GetByIdAsync(id);

        if (expenseCategory is null)
        {
            return NotFound();
        }

        return Ok(expenseCategory.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] ExpenseCategoryDto? expenseCategoryDto)
    {
        if (expenseCategoryDto is null)
        {
            return BadRequest();
        }

        var expenseCategory = expenseCategoryDto.ToModel();
        var createdExpenseCategory = await _expenseCategoryService.AddAsync(expenseCategory);

        if (createdExpenseCategory is null)
        {
            return BadRequest();
        }

        return Ok(createdExpenseCategory.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] ExpenseCategoryDto? expenseCategoryDto)
    {
        if (expenseCategoryDto is null)
        {
            return BadRequest();
        }

        var existingExpenseCategory = await _expenseCategoryService.GetByIdAsync(id);

        if (existingExpenseCategory is null)
        {
            return NotFound();
        }

        var expenseCategory = expenseCategoryDto.ToModel();
        expenseCategory.CategoryId = id;

        var wasUpdated = await _expenseCategoryService.UpdateAsync(expenseCategory);

        if (!wasUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var existingExpenseCategory = await _expenseCategoryService.GetByIdAsync(id);

        if (existingExpenseCategory is null)
        {
            return NotFound();
        }

        var wasDeleted = await _expenseCategoryService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
