using BackEndApp.DataAccess;
using BackEndApp.Models.GoalContributions;

namespace BackEndApp.BusinessLogic;

public class GoalContributionService
{
    private readonly GoalContributionsDataAccess _goalContributionsDataAccess;
    private readonly SavingGoalsDataAccess _savingGoalsDataAccess;

    public GoalContributionService(
        GoalContributionsDataAccess goalContributionsDataAccess,
        SavingGoalsDataAccess savingGoalsDataAccess)
    {
        _goalContributionsDataAccess = goalContributionsDataAccess;
        _savingGoalsDataAccess = savingGoalsDataAccess;
    }

    public async Task<List<GoalContribution>> GetAllAsync()
    {
        return await _goalContributionsDataAccess.GetAllAsync();
    }

    public async Task<GoalContribution?> GetByIdAsync(Guid id)
    {
        return await _goalContributionsDataAccess.GetByIdAsync(id);
    }

    public async Task<GoalContribution?> AddAsync(GoalContribution contribution)
    {
        if (!await ValidateAsync(contribution))
        {
            return null;
        }

        return await _goalContributionsDataAccess.AddAsync(contribution);
    }

    public async Task<bool> UpdateAsync(GoalContribution contribution)
    {
        var existingContribution = await _goalContributionsDataAccess.GetByIdAsync(
            contribution.ContributionId);

        if (existingContribution is null)
        {
            return false;
        }

        contribution.GoalId = existingContribution.GoalId;

        if (!await ValidateAsync(contribution))
        {
            return false;
        }

        return await _goalContributionsDataAccess.UpdateAsync(contribution);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _goalContributionsDataAccess.DeleteAsync(id);
    }

    private async Task<bool> ValidateAsync(GoalContribution contribution)
    {
        var savingGoal = await _savingGoalsDataAccess.GetByIdAsync(contribution.GoalId);

        if (savingGoal is null || contribution.Amount <= 0)
        {
            return false;
        }

        return contribution.ContributionDate is null
            || contribution.ContributionDate.Value <= DateTime.UtcNow;
    }
}
