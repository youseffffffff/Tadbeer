using BackEndApp.DataAccess;
using BackEndApp.Models.BudgetPeriods;

namespace BackEndApp.BusinessLogic;

public class BudgetPeriodService
{
    private readonly BudgetPeriodsDataAccess _budgetPeriodsDataAccess;
    private readonly FamilyBudgetsDataAccess _familyBudgetsDataAccess;

    public BudgetPeriodService(
        BudgetPeriodsDataAccess budgetPeriodsDataAccess,
        FamilyBudgetsDataAccess familyBudgetsDataAccess)
    {
        _budgetPeriodsDataAccess = budgetPeriodsDataAccess;
        _familyBudgetsDataAccess = familyBudgetsDataAccess;
    }

    public async Task<List<BudgetPeriod>> GetAllAsync()
    {
        return await _budgetPeriodsDataAccess.GetAllAsync();
    }

    public async Task<BudgetPeriod?> GetByIdAsync(Guid id)
    {
        return await _budgetPeriodsDataAccess.GetByIdAsync(id);
    }

    public async Task<BudgetPeriod?> AddAsync(BudgetPeriod period)
    {
        var familyBudget = await _familyBudgetsDataAccess.GetByIdAsync(period.BudgetId);

        if (familyBudget is null)
        {
            return null;
        }

        if (period.PeriodStart >= period.PeriodEnd)
        {
            return null;
        }

        if (period.ActualBudgetAmount <= 0)
        {
            return null;
        }

        var hasOverlap = await _budgetPeriodsDataAccess.HasOverlappingPeriodAsync(
            period.BudgetId,
            period.PeriodStart,
            period.PeriodEnd);

        if (hasOverlap)
        {
            return null;
        }

        return await _budgetPeriodsDataAccess.AddAsync(period);
    }

    public async Task<bool> UpdateAsync(BudgetPeriod period)
    {
        return await _budgetPeriodsDataAccess.UpdateAsync(period);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _budgetPeriodsDataAccess.DeleteAsync(id);
    }
}
