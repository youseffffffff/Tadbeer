namespace BackEndApp.Models.Rewards;

public class RewardDto
{
    public Guid RewardId { get; set; }

    public string RewardName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? PointsCost { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Reward ToModel()
    {
        return new Reward
        {
            RewardName = RewardName,
            Description = Description,
            PointsCost = PointsCost
        };
    }
}
