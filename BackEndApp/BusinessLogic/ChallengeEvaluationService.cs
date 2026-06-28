using BackEndApp.DataAccess;
using BackEndApp.Models.ChallengeEvaluations;

namespace BackEndApp.BusinessLogic;

public class ChallengeEvaluationService
{
    private const int SuccessLevelMaxLength = 50;

    private readonly ChallengeEvaluationsDataAccess _challengeEvaluationsDataAccess;
    private readonly ChallengesDataAccess _challengesDataAccess;

    public ChallengeEvaluationService(
        ChallengeEvaluationsDataAccess challengeEvaluationsDataAccess,
        ChallengesDataAccess challengesDataAccess)
    {
        _challengeEvaluationsDataAccess = challengeEvaluationsDataAccess;
        _challengesDataAccess = challengesDataAccess;
    }

    public async Task<List<ChallengeEvaluation>> GetAllAsync()
    {
        return await _challengeEvaluationsDataAccess.GetAllAsync();
    }

    public async Task<ChallengeEvaluation?> GetByIdAsync(Guid id)
    {
        return await _challengeEvaluationsDataAccess.GetByIdAsync(id);
    }

    public async Task<ChallengeEvaluation?> AddAsync(ChallengeEvaluation evaluation)
    {
        if (!await PrepareAndValidateAsync(evaluation))
        {
            return null;
        }

        if (await _challengeEvaluationsDataAccess.ExistsByChallengeIdAsync(evaluation.ChallengeId))
        {
            return null;
        }

        return await _challengeEvaluationsDataAccess.AddAsync(evaluation);
    }

    public async Task<bool> UpdateAsync(ChallengeEvaluation evaluation)
    {
        var existingEvaluation = await _challengeEvaluationsDataAccess.GetByIdAsync(
            evaluation.EvaluationId);

        if (existingEvaluation is null)
        {
            return false;
        }

        evaluation.ChallengeId = existingEvaluation.ChallengeId;

        if (!await PrepareAndValidateAsync(evaluation))
        {
            return false;
        }

        return await _challengeEvaluationsDataAccess.UpdateAsync(evaluation);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _challengeEvaluationsDataAccess.DeleteAsync(id);
    }

    private async Task<bool> PrepareAndValidateAsync(ChallengeEvaluation evaluation)
    {
        var challenge = await _challengesDataAccess.GetByIdAsync(evaluation.ChallengeId);

        if (challenge is null
            || evaluation.CompletionPercentage is null
            || evaluation.CompletionPercentage < 0
            || evaluation.CompletionPercentage > 100
            || string.IsNullOrWhiteSpace(evaluation.SuccessLevel))
        {
            return false;
        }

        evaluation.SuccessLevel = evaluation.SuccessLevel.Trim();
        evaluation.EvaluationSummary = evaluation.EvaluationSummary?.Trim();
        evaluation.Recommendations = evaluation.Recommendations?.Trim();

        return evaluation.SuccessLevel.Length <= SuccessLevelMaxLength;
    }
}
