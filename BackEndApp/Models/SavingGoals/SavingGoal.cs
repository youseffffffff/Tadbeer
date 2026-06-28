namespace BackEndApp.Models.SavingGoals;

public class SavingGoal
{
    public Guid GoalId { get; set; }

    public Guid UserId { get; set; }

    public string GoalName { get; set; } = string.Empty;

    public string? GoalDescription { get; set; }

    public decimal TargetAmount { get; set; }

    public DateTime? TargetDate { get; set; }

    public byte? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public SavingGoalDto ToDto()
    {
        return new SavingGoalDto
        {
            GoalId = GoalId,
            UserId = UserId,
            GoalName = GoalName,
            GoalDescription = GoalDescription,
            TargetAmount = TargetAmount,
            TargetDate = TargetDate,
            Status = Status,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt
        };
    }
}
