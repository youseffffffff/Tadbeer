namespace BackEndApp.Models.Challenges;

public class Challenge
{
    public Guid ChallengeId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid AllocationId { get; set; }

    public ChallengeDto ToDto()
    {
        return new ChallengeDto
        {
            ChallengeId = ChallengeId,
            AllocationId = AllocationId,
            Title = Title,
            Description = Description,
            CreatedAt = CreatedAt
        };
    }
}
