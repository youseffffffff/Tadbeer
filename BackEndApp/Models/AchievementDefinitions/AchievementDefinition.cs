namespace BackEndApp.Models.AchievementDefinitions;

public class AchievementDefinition
{
    public Guid AchievementId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? XPReward { get; set; }

    public string? BadgeIcon { get; set; }

    public DateTime? CreatedAt { get; set; }

    public AchievementDefinitionDto ToDto()
    {
        return new AchievementDefinitionDto
        {
            AchievementId = AchievementId,
            Title = Title,
            Description = Description,
            XPReward = XPReward,
            BadgeIcon = BadgeIcon,
            CreatedAt = CreatedAt
        };
    }
}
