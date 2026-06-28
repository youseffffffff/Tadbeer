namespace BackEndApp.Models.RewardRedemptions;

public class RewardRedemption
{
    public Guid RedemptionId { get; set; }

    public Guid UserId { get; set; }

    public Guid RewardId { get; set; }

    public DateTime? RedeemedAt { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public RewardRedemptionDto ToDto()
    {
        return new RewardRedemptionDto
        {
            RedemptionId = RedemptionId,
            UserId = UserId,
            RewardId = RewardId,
            RedeemedAt = RedeemedAt,
            ApprovedByUserId = ApprovedByUserId
        };
    }
}
