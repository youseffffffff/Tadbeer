namespace BackEndApp.Models.FamilyInvitations;

public class FamilyInvitation
{
    public Guid InvitationId { get; set; }

    public Guid FamilyId { get; set; }

    public Guid InvitedByUserId { get; set; }

    public string EmailOrPhone { get; set; } = string.Empty;

    public string InvitationToken { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int InvitationStatusId { get; set; }

    public FamilyInvitationDto ToDto()
    {
        return new FamilyInvitationDto
        {
            InvitationId = InvitationId,
            FamilyId = FamilyId,
            InvitedByUserId = InvitedByUserId,
            EmailOrPhone = EmailOrPhone,
            ExpiresAt = ExpiresAt,
            CreatedAt = CreatedAt,
            InvitationStatusId = InvitationStatusId
        };
    }
}
