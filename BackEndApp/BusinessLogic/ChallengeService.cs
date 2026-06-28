using BackEndApp.DataAccess;
using BackEndApp.Models.Challenges;

namespace BackEndApp.BusinessLogic;

public class ChallengeService
{
    private const int TitleMaxLength = 255;

    private readonly ChallengesDataAccess _challengesDataAccess;
    private readonly BudgetAllocationsDataAccess _budgetAllocationsDataAccess;

    public ChallengeService(
        ChallengesDataAccess challengesDataAccess,
        BudgetAllocationsDataAccess budgetAllocationsDataAccess)
    {
        _challengesDataAccess = challengesDataAccess;
        _budgetAllocationsDataAccess = budgetAllocationsDataAccess;
    }

    public async Task<List<Challenge>> GetAllAsync()
    {
        return await _challengesDataAccess.GetAllAsync();
    }

    public async Task<Challenge?> GetByIdAsync(Guid id)
    {
        return await _challengesDataAccess.GetByIdAsync(id);
    }

    public async Task<Challenge?> AddAsync(Challenge challenge)
    {
        if (!await PrepareAndValidateAsync(challenge))
        {
            return null;
        }

        return await _challengesDataAccess.AddAsync(challenge);
    }

    public async Task<bool> UpdateAsync(Challenge challenge)
    {
        var existingChallenge = await _challengesDataAccess.GetByIdAsync(challenge.ChallengeId);

        if (existingChallenge is null)
        {
            return false;
        }

        challenge.AllocationId = existingChallenge.AllocationId;

        if (!await PrepareAndValidateAsync(challenge))
        {
            return false;
        }

        return await _challengesDataAccess.UpdateAsync(challenge);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _challengesDataAccess.DeleteAsync(id);
    }

    private async Task<bool> PrepareAndValidateAsync(Challenge challenge)
    {
        var allocation = await _budgetAllocationsDataAccess.GetByIdAsync(challenge.AllocationId);

        if (allocation is null || string.IsNullOrWhiteSpace(challenge.Title))
        {
            return false;
        }

        challenge.Title = challenge.Title.Trim();
        challenge.Description = challenge.Description?.Trim();

        return challenge.Title.Length <= TitleMaxLength;
    }
}
