namespace BackEndApp.Models.Families;

public class FamilyDto
{
    public Guid FamilyId { get; set; }

    public string FamilyName { get; set; } = string.Empty;

    public Guid? CreatedByUserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public Family ToModel()
    {
        return new Family
        {
            FamilyName = FamilyName,
            CreatedByUserId = CreatedByUserId,
            IsActive = IsActive
        };
    }
}
