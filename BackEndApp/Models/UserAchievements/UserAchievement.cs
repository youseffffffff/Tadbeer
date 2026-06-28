namespace BackEndApp.Models.UserAchievements;

public class UserAchievement
{
    public Guid UserAchievementId { get; set; }

    public Guid UserId { get; set; }

    public Guid AchievementId { get; set; }

    public DateTime? EarnedAt { get; set; }

    public Guid? AwardedByUserId { get; set; }

    public UserAchievementDto ToDto()
    {
        return new UserAchievementDto
        {
            UserAchievementId = UserAchievementId,
            UserId = UserId,
            AchievementId = AchievementId,
            EarnedAt = EarnedAt,
            AwardedByUserId = AwardedByUserId
        };
    }
}
