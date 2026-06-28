namespace BackEndApp.Models.FamilyInvitations;

public class FamilyInvitationDto
{
    public Guid InvitationId { get; set; }

    public Guid FamilyId { get; set; }

    public Guid InvitedByUserId { get; set; }

    public string EmailOrPhone { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int InvitationStatusId { get; set; }

    public FamilyInvitation ToModel()
    {
        return new FamilyInvitation
        {
            FamilyId = FamilyId,
            InvitedByUserId = InvitedByUserId,
            EmailOrPhone = EmailOrPhone,
            ExpiresAt = ExpiresAt,
            InvitationStatusId = InvitationStatusId
        };
    }
}
