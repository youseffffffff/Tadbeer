namespace BackEndApp.Models.UserAchievements;

public class UserAchievementDto
{
    public Guid UserAchievementId { get; set; }

    public Guid UserId { get; set; }

    public Guid AchievementId { get; set; }

    public DateTime? EarnedAt { get; set; }

    public Guid? AwardedByUserId { get; set; }

    public UserAchievement ToModel()
    {
        return new UserAchievement
        {
            UserId = UserId,
            AchievementId = AchievementId,
            AwardedByUserId = AwardedByUserId
        };
    }
}
