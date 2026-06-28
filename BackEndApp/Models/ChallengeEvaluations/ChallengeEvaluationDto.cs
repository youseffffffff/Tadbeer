namespace BackEndApp.Models.ChallengeEvaluations;

public class ChallengeEvaluationDto
{
    public Guid EvaluationId { get; set; }

    public Guid ChallengeId { get; set; }

    public decimal? CompletionPercentage { get; set; }

    public string? SuccessLevel { get; set; }

    public string? EvaluationSummary { get; set; }

    public string? Recommendations { get; set; }

    public DateTime? EvaluatedAt { get; set; }

    public ChallengeEvaluation ToModel()
    {
        return new ChallengeEvaluation
        {
            ChallengeId = ChallengeId,
            CompletionPercentage = CompletionPercentage,
            SuccessLevel = SuccessLevel,
            EvaluationSummary = EvaluationSummary,
            Recommendations = Recommendations
        };
    }
}
