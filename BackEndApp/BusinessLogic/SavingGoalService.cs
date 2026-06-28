using BackEndApp.DataAccess;
using BackEndApp.Models.SavingGoals;

namespace BackEndApp.BusinessLogic;

public class SavingGoalService
{
    private readonly SavingGoalsDataAccess _savingGoalsDataAccess;
    private readonly UsersDataAccess _usersDataAccess;

    public SavingGoalService(
        SavingGoalsDataAccess savingGoalsDataAccess,
        UsersDataAccess usersDataAccess)
    {
        _savingGoalsDataAccess = savingGoalsDataAccess;
        _usersDataAccess = usersDataAccess;
    }

    public async Task<List<SavingGoal>> GetAllAsync()
    {
        return await _savingGoalsDataAccess.GetAllAsync();
    }

    public async Task<SavingGoal?> GetByIdAsync(Guid id)
    {
        return await _savingGoalsDataAccess.GetByIdAsync(id);
    }

    public async Task<SavingGoal?> AddAsync(SavingGoal goal)
    {
        if (!await PrepareAndValidateAsync(goal))
        {
            return null;
        }

        return await _savingGoalsDataAccess.AddAsync(goal);
    }

    public async Task<bool> UpdateAsync(SavingGoal goal)
    {
        if (!await PrepareAndValidateAsync(goal))
        {
            return false;
        }

        return await _savingGoalsDataAccess.UpdateAsync(goal);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _savingGoalsDataAccess.DeleteAsync(id);
    }

    private async Task<bool> PrepareAndValidateAsync(SavingGoal goal)
    {
        var user = await _usersDataAccess.GetByIdAsync(goal.UserId);

        if (user is null || string.IsNullOrWhiteSpace(goal.GoalName))
        {
            return false;
        }

        goal.GoalName = goal.GoalName.Trim();
        goal.GoalDescription = goal.GoalDescription?.Trim();

        if (goal.TargetAmount <= 0)
        {
            return false;
        }

        return goal.TargetDate is null || goal.TargetDate.Value.Date >= DateTime.UtcNow.Date;
    }
}
