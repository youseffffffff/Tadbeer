using BackEndApp.DataAccess;
using BackEndApp.Models.Rewards;

namespace BackEndApp.BusinessLogic;

public class RewardService
{
    private const int RewardNameMaxLength = 255;

    private readonly RewardsDataAccess _rewardsDataAccess;

    public RewardService(RewardsDataAccess rewardsDataAccess)
    {
        _rewardsDataAccess = rewardsDataAccess;
    }

    public async Task<List<Reward>> GetAllAsync()
    {
        return await _rewardsDataAccess.GetAllAsync();
    }

    public async Task<Reward?> GetByIdAsync(Guid id)
    {
        return await _rewardsDataAccess.GetByIdAsync(id);
    }

    public async Task<Reward?> AddAsync(Reward reward)
    {
        if (!PrepareAndValidate(reward))
        {
            return null;
        }

        if (await _rewardsDataAccess.ExistsByNameAsync(reward.RewardName))
        {
            return null;
        }

        return await _rewardsDataAccess.AddAsync(reward);
    }

    public async Task<bool> UpdateAsync(Reward reward)
    {
        if (!PrepareAndValidate(reward))
        {
            return false;
        }

        var duplicateExists = await _rewardsDataAccess.ExistsByNameAsync(
            reward.RewardName,
            reward.RewardId);

        if (duplicateExists)
        {
            return false;
        }

        return await _rewardsDataAccess.UpdateAsync(reward);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _rewardsDataAccess.DeleteAsync(id);
    }

    private static bool PrepareAndValidate(Reward reward)
    {
        if (string.IsNullOrWhiteSpace(reward.RewardName)
            || reward.PointsCost is null
            || reward.PointsCost <= 0)
        {
            return false;
        }

        reward.RewardName = reward.RewardName.Trim();
        reward.Description = reward.Description?.Trim();

        return reward.RewardName.Length <= RewardNameMaxLength;
    }
}
