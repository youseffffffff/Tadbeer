using BackEndApp.DataAccess;
using BackEndApp.Models.AchievementDefinitions;

namespace BackEndApp.BusinessLogic;

public class AchievementDefinitionService
{
    private const int TitleMaxLength = 255;
    private const int BadgeIconMaxLength = 255;

    private readonly AchievementDefinitionsDataAccess _achievementDefinitionsDataAccess;

    public AchievementDefinitionService(
        AchievementDefinitionsDataAccess achievementDefinitionsDataAccess)
    {
        _achievementDefinitionsDataAccess = achievementDefinitionsDataAccess;
    }

    public async Task<List<AchievementDefinition>> GetAllAsync()
    {
        return await _achievementDefinitionsDataAccess.GetAllAsync();
    }

    public async Task<AchievementDefinition?> GetByIdAsync(Guid id)
    {
        return await _achievementDefinitionsDataAccess.GetByIdAsync(id);
    }

    public async Task<AchievementDefinition?> AddAsync(AchievementDefinition achievement)
    {
        if (!PrepareAndValidate(achievement))
        {
            return null;
        }

        if (await _achievementDefinitionsDataAccess.ExistsByTitleAsync(achievement.Title))
        {
            return null;
        }

        return await _achievementDefinitionsDataAccess.AddAsync(achievement);
    }

    public async Task<bool> UpdateAsync(AchievementDefinition achievement)
    {
        if (!PrepareAndValidate(achievement))
        {
            return false;
        }

        var duplicateExists = await _achievementDefinitionsDataAccess.ExistsByTitleAsync(
            achievement.Title,
            achievement.AchievementId);

        if (duplicateExists)
        {
            return false;
        }

        return await _achievementDefinitionsDataAccess.UpdateAsync(achievement);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _achievementDefinitionsDataAccess.DeleteAsync(id);
    }

    private static bool PrepareAndValidate(AchievementDefinition achievement)
    {
        if (string.IsNullOrWhiteSpace(achievement.Title)
            || achievement.XPReward is null
            || achievement.XPReward < 0)
        {
            return false;
        }

        achievement.Title = achievement.Title.Trim();
        achievement.Description = achievement.Description?.Trim();
        achievement.BadgeIcon = achievement.BadgeIcon?.Trim();

        return achievement.Title.Length <= TitleMaxLength
            && (achievement.BadgeIcon is null
                || achievement.BadgeIcon.Length <= BadgeIconMaxLength);
    }
}
