using BackEndApp.DataAccess;
using BackEndApp.Models.RewardRedemptions;

namespace BackEndApp.BusinessLogic;

public class RewardRedemptionService
{
    private readonly RewardRedemptionsDataAccess _rewardRedemptionsDataAccess;
    private readonly UsersDataAccess _usersDataAccess;
    private readonly RewardsDataAccess _rewardsDataAccess;

    public RewardRedemptionService(
        RewardRedemptionsDataAccess rewardRedemptionsDataAccess,
        UsersDataAccess usersDataAccess,
        RewardsDataAccess rewardsDataAccess)
    {
        _rewardRedemptionsDataAccess = rewardRedemptionsDataAccess;
        _usersDataAccess = usersDataAccess;
        _rewardsDataAccess = rewardsDataAccess;
    }

    public async Task<List<RewardRedemption>> GetAllAsync()
    {
        return await _rewardRedemptionsDataAccess.GetAllAsync();
    }

    public async Task<RewardRedemption?> GetByIdAsync(Guid id)
    {
        return await _rewardRedemptionsDataAccess.GetByIdAsync(id);
    }

    public async Task<RewardRedemption?> AddAsync(RewardRedemption redemption)
    {
        var user = await _usersDataAccess.GetByIdAsync(redemption.UserId);

        if (user is null)
        {
            return null;
        }

        var reward = await _rewardsDataAccess.GetByIdAsync(redemption.RewardId);

        if (reward is null || !await ApprovingUserExistsAsync(redemption.ApprovedByUserId))
        {
            return null;
        }

        return await _rewardRedemptionsDataAccess.AddAsync(redemption);
    }

    public async Task<bool> UpdateAsync(RewardRedemption redemption)
    {
        var existingRedemption = await _rewardRedemptionsDataAccess.GetByIdAsync(
            redemption.RedemptionId);

        if (existingRedemption is null
            || !await ApprovingUserExistsAsync(redemption.ApprovedByUserId))
        {
            return false;
        }

        redemption.UserId = existingRedemption.UserId;
        redemption.RewardId = existingRedemption.RewardId;
        redemption.RedeemedAt = existingRedemption.RedeemedAt;

        return await _rewardRedemptionsDataAccess.UpdateAsync(redemption);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _rewardRedemptionsDataAccess.DeleteAsync(id);
    }

    private async Task<bool> ApprovingUserExistsAsync(Guid? approvedByUserId)
    {
        if (approvedByUserId is null)
        {
            return true;
        }

        return await _usersDataAccess.GetByIdAsync(approvedByUserId.Value) is not null;
    }
}
