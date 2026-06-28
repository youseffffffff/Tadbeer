namespace BackEndApp.Models.AchievementDefinitions;

public class AchievementDefinitionDto
{
    public Guid AchievementId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? XPReward { get; set; }

    public string? BadgeIcon { get; set; }

    public DateTime? CreatedAt { get; set; }

    public AchievementDefinition ToModel()
    {
        return new AchievementDefinition
        {
            Title = Title,
            Description = Description,
            XPReward = XPReward,
            BadgeIcon = BadgeIcon
        };
    }
}
