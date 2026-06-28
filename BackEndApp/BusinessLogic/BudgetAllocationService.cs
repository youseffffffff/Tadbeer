using BackEndApp.DataAccess;
using BackEndApp.Models.BudgetAllocations;

namespace BackEndApp.BusinessLogic;

public class BudgetAllocationService
{
    private readonly BudgetAllocationsDataAccess _budgetAllocationsDataAccess;
    private readonly BudgetPeriodsDataAccess _budgetPeriodsDataAccess;
    private readonly UsersDataAccess _usersDataAccess;
    private readonly FamilyBudgetsDataAccess _familyBudgetsDataAccess;

    public BudgetAllocationService(
        BudgetAllocationsDataAccess budgetAllocationsDataAccess,
        BudgetPeriodsDataAccess budgetPeriodsDataAccess,
        UsersDataAccess usersDataAccess,
        FamilyBudgetsDataAccess familyBudgetsDataAccess)
    {
        _budgetAllocationsDataAccess = budgetAllocationsDataAccess;
        _budgetPeriodsDataAccess = budgetPeriodsDataAccess;
        _usersDataAccess = usersDataAccess;
        _familyBudgetsDataAccess = familyBudgetsDataAccess;
    }

    public async Task<List<BudgetAllocation>> GetAllAsync()
    {
        return await _budgetAllocationsDataAccess.GetAllAsync();
    }

    public async Task<BudgetAllocation?> GetByIdAsync(Guid id)
    {
        return await _budgetAllocationsDataAccess.GetByIdAsync(id);
    }

    public async Task<BudgetAllocation?> AddAsync(BudgetAllocation allocation)
    {
        var budgetPeriod = await _budgetPeriodsDataAccess.GetByIdAsync(allocation.PeriodId);

        if (budgetPeriod is null)
        {
            return null;
        }

        var user = await _usersDataAccess.GetByIdAsync(allocation.UserId);

        if (user is null)
        {
            return null;
        }

        if (allocation.AllocatedAmount <= 0)
        {
            return null;
        }

        if (await _budgetAllocationsDataAccess.ExistsAsync(allocation.PeriodId, allocation.UserId))
        {
            return null;
        }

        var familyBudget = await _familyBudgetsDataAccess.GetByIdAsync(budgetPeriod.BudgetId);

        if (familyBudget is null || familyBudget.FamilyId != user.FamilyId)
        {
            return null;
        }

        return await _budgetAllocationsDataAccess.AddAsync(allocation);
    }

    public async Task<bool> UpdateAsync(BudgetAllocation allocation)
    {
        return await _budgetAllocationsDataAccess.UpdateAsync(allocation);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _budgetAllocationsDataAccess.DeleteAsync(id);
    }
}
