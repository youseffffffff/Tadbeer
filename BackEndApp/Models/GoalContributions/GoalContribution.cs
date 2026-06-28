namespace BackEndApp.Models.GoalContributions;

public class GoalContribution
{
    public Guid ContributionId { get; set; }

    public Guid GoalId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? ContributionDate { get; set; }

    public GoalContributionDto ToDto()
    {
        return new GoalContributionDto
        {
            ContributionId = ContributionId,
            GoalId = GoalId,
            Amount = Amount,
            ContributionDate = ContributionDate
        };
    }
}
