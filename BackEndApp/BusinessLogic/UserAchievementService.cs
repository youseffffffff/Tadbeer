using BackEndApp.DataAccess;
using BackEndApp.Models.UserAchievements;

namespace BackEndApp.BusinessLogic;

public class UserAchievementService
{
    private readonly UserAchievementsDataAccess _userAchievementsDataAccess;
    private readonly UsersDataAccess _usersDataAccess;
    private readonly AchievementDefinitionsDataAccess _achievementDefinitionsDataAccess;

    public UserAchievementService(
        UserAchievementsDataAccess userAchievementsDataAccess,
        UsersDataAccess usersDataAccess,
        AchievementDefinitionsDataAccess achievementDefinitionsDataAccess)
    {
        _userAchievementsDataAccess = userAchievementsDataAccess;
        _usersDataAccess = usersDataAccess;
        _achievementDefinitionsDataAccess = achievementDefinitionsDataAccess;
    }

    public async Task<List<UserAchievement>> GetAllAsync()
    {
        return await _userAchievementsDataAccess.GetAllAsync();
    }

    public async Task<UserAchievement?> GetByIdAsync(Guid id)
    {
        return await _userAchievementsDataAccess.GetByIdAsync(id);
    }

    public async Task<UserAchievement?> AddAsync(UserAchievement achievement)
    {
        var user = await _usersDataAccess.GetByIdAsync(achievement.UserId);

        if (user is null)
        {
            return null;
        }

        var achievementDefinition = await _achievementDefinitionsDataAccess.GetByIdAsync(
            achievement.AchievementId);

        if (achievementDefinition is null)
        {
            return null;
        }

        if (!await AwardingUserExistsAsync(achievement.AwardedByUserId))
        {
            return null;
        }

        if (await _userAchievementsDataAccess.ExistsAsync(
            achievement.UserId,
            achievement.AchievementId))
        {
            return null;
        }

        return await _userAchievementsDataAccess.AddAsync(achievement);
    }

    public async Task<bool> UpdateAsync(UserAchievement achievement)
    {
        var existingAchievement = await _userAchievementsDataAccess.GetByIdAsync(
            achievement.UserAchievementId);

        if (existingAchievement is null)
        {
            return false;
        }

        if (!await AwardingUserExistsAsync(achievement.AwardedByUserId))
        {
            return false;
        }

        achievement.UserId = existingAchievement.UserId;
        achievement.AchievementId = existingAchievement.AchievementId;
        achievement.EarnedAt = existingAchievement.EarnedAt;

        return await _userAchievementsDataAccess.UpdateAsync(achievement);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _userAchievementsDataAccess.DeleteAsync(id);
    }

    private async Task<bool> AwardingUserExistsAsync(Guid? awardedByUserId)
    {
        if (awardedByUserId is null)
        {
            return true;
        }

        return await _usersDataAccess.GetByIdAsync(awardedByUserId.Value) is not null;
    }
}
