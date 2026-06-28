using BackEndApp.DataAccess;
using BackEndApp.Models.Expenses;

namespace BackEndApp.BusinessLogic;

public class ExpenseService
{
    private const int DescriptionMaxLength = 500;

    private readonly ExpensesDataAccess _expensesDataAccess;
    private readonly BudgetAllocationsDataAccess _budgetAllocationsDataAccess;
    private readonly ExpenseCategoriesDataAccess _expenseCategoriesDataAccess;

    public ExpenseService(
        ExpensesDataAccess expensesDataAccess,
        BudgetAllocationsDataAccess budgetAllocationsDataAccess,
        ExpenseCategoriesDataAccess expenseCategoriesDataAccess)
    {
        _expensesDataAccess = expensesDataAccess;
        _budgetAllocationsDataAccess = budgetAllocationsDataAccess;
        _expenseCategoriesDataAccess = expenseCategoriesDataAccess;
    }

    public async Task<List<Expense>> GetAllAsync()
    {
        return await _expensesDataAccess.GetAllAsync();
    }

    public async Task<Expense?> GetByIdAsync(Guid id)
    {
        return await _expensesDataAccess.GetByIdAsync(id);
    }

    public async Task<Expense?> AddAsync(Expense expense)
    {
        if (!await PrepareAndValidateAsync(expense))
        {
            return null;
        }

        return await _expensesDataAccess.AddAsync(expense);
    }

    public async Task<bool> UpdateAsync(Expense expense)
    {
        var existingExpense = await _expensesDataAccess.GetByIdAsync(expense.ExpenseId);

        if (existingExpense is null)
        {
            return false;
        }

        if (!await PrepareAndValidateAsync(expense))
        {
            return false;
        }

        return await _expensesDataAccess.UpdateAsync(expense);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _expensesDataAccess.DeleteAsync(id);
    }

    private async Task<bool> PrepareAndValidateAsync(Expense expense)
    {
        var allocation = await _budgetAllocationsDataAccess.GetByIdAsync(expense.AllocationId);

        if (allocation is null)
        {
            return false;
        }

        var category = await _expenseCategoriesDataAccess.GetByIdAsync(expense.CategoryId);

        if (category is null)
        {
            return false;
        }

        if (expense.Amount <= 0 || expense.ExpenseDate.Date > DateTime.UtcNow.Date)
        {
            return false;
        }

        expense.Description = expense.Description?.Trim();

        return expense.Description is null || expense.Description.Length <= DescriptionMaxLength;
    }
}
