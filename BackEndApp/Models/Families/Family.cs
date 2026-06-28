namespace BackEndApp.Models.Families;

public class Family
{
    public Guid FamilyId { get; set; }

    public string FamilyName { get; set; } = string.Empty;

    public Guid? CreatedByUserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public FamilyDto ToDto()
    {
        return new FamilyDto
        {
            FamilyId = FamilyId,
            FamilyName = FamilyName,
            CreatedByUserId = CreatedByUserId,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            IsActive = IsActive
        };
    }
}
