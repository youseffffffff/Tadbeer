using BackEndApp.DataAccess;
using BackEndApp.Models.FamilyBudgets;

namespace BackEndApp.BusinessLogic;

public class FamilyBudgetService
{
    private readonly FamilyBudgetsDataAccess _familyBudgetsDataAccess;
    private readonly FamiliesDataAccess _familiesDataAccess;

    public FamilyBudgetService(
        FamilyBudgetsDataAccess familyBudgetsDataAccess,
        FamiliesDataAccess familiesDataAccess)
    {
        _familyBudgetsDataAccess = familyBudgetsDataAccess;
        _familiesDataAccess = familiesDataAccess;
    }

    public async Task<List<FamilyBudget>> GetAllAsync()
    {
        return await _familyBudgetsDataAccess.GetAllAsync();
    }

    public async Task<FamilyBudget?> GetByIdAsync(Guid id)
    {
        return await _familyBudgetsDataAccess.GetByIdAsync(id);
    }

    public async Task<FamilyBudget?> AddAsync(FamilyBudget familyBudget)
    {
        var family = await _familiesDataAccess.GetByIdAsync(familyBudget.FamilyId);

        if (family is null)
        {
            return null;
        }

        if (await _familyBudgetsDataAccess.ExistsByFamilyIdAsync(familyBudget.FamilyId))
        {
            return null;
        }

        return await _familyBudgetsDataAccess.AddAsync(familyBudget);
    }

    public async Task<bool> UpdateAsync(FamilyBudget familyBudget)
    {
        return await _familyBudgetsDataAccess.UpdateAsync(familyBudget);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _familyBudgetsDataAccess.DeleteAsync(id);
    }
}
