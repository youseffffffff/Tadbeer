namespace BackEndApp.Models.RewardRedemptions;

public class RewardRedemptionDto
{
    public Guid RedemptionId { get; set; }

    public Guid UserId { get; set; }

    public Guid RewardId { get; set; }

    public DateTime? RedeemedAt { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public RewardRedemption ToModel()
    {
        return new RewardRedemption
        {
            UserId = UserId,
            RewardId = RewardId,
            ApprovedByUserId = ApprovedByUserId
        };
    }
}
