using System.Text.RegularExpressions;
using BackEndApp.DataAccess;
using BackEndApp.Models.ExpenseCategories;

namespace BackEndApp.BusinessLogic;

public class ExpenseCategoryService
{
    private const string ColorHexPattern = "^#[0-9A-Fa-f]{6}$";

    private readonly ExpenseCategoriesDataAccess _expenseCategoriesDataAccess;

    public ExpenseCategoryService(ExpenseCategoriesDataAccess expenseCategoriesDataAccess)
    {
        _expenseCategoriesDataAccess = expenseCategoriesDataAccess;
    }

    public async Task<List<ExpenseCategory>> GetAllAsync()
    {
        return await _expenseCategoriesDataAccess.GetAllAsync();
    }

    public async Task<ExpenseCategory?> GetByIdAsync(int id)
    {
        return await _expenseCategoriesDataAccess.GetByIdAsync(id);
    }

    public async Task<ExpenseCategory?> AddAsync(ExpenseCategory category)
    {
        if (!PrepareCategory(category))
        {
            return null;
        }

        if (await _expenseCategoriesDataAccess.ExistsByNameAsync(category.CategoryName))
        {
            return null;
        }

        return await _expenseCategoriesDataAccess.AddAsync(category);
    }

    public async Task<bool> UpdateAsync(ExpenseCategory category)
    {
        if (!PrepareCategory(category))
        {
            return false;
        }

        var duplicateExists = await _expenseCategoriesDataAccess.ExistsByNameAsync(
            category.CategoryName,
            category.CategoryId);

        if (duplicateExists)
        {
            return false;
        }

        return await _expenseCategoriesDataAccess.UpdateAsync(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _expenseCategoriesDataAccess.DeleteAsync(id);
    }

    private static bool PrepareCategory(ExpenseCategory category)
    {
        if (string.IsNullOrWhiteSpace(category.CategoryName))
        {
            return false;
        }

        category.CategoryName = category.CategoryName.Trim();
        category.IconName = category.IconName?.Trim();
        category.ColorHex = category.ColorHex?.Trim();

        return category.ColorHex is null || Regex.IsMatch(category.ColorHex, ColorHexPattern);
    }
}
