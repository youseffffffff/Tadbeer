namespace BackEndApp.Models.Challenges;

public class ChallengeDto
{
    public Guid ChallengeId { get; set; }

    public Guid AllocationId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Challenge ToModel()
    {
        return new Challenge
        {
            AllocationId = AllocationId,
            Title = Title,
            Description = Description
        };
    }
}
