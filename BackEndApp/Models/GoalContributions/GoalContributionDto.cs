namespace BackEndApp.Models.GoalContributions;

public class GoalContributionDto
{
    public Guid ContributionId { get; set; }

    public Guid GoalId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? ContributionDate { get; set; }

    public GoalContribution ToModel()
    {
        return new GoalContribution
        {
            GoalId = GoalId,
            Amount = Amount,
            ContributionDate = ContributionDate
        };
    }
}
