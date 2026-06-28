namespace BackEndApp.Models.Rewards;

public class Reward
{
    public Guid RewardId { get; set; }

    public string RewardName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? PointsCost { get; set; }

    public DateTime? CreatedAt { get; set; }

    public RewardDto ToDto()
    {
        return new RewardDto
        {
            RewardId = RewardId,
            RewardName = RewardName,
            Description = Description,
            PointsCost = PointsCost,
            CreatedAt = CreatedAt
        };
    }
}
