using BackEndApp.DataAccess;
using BackEndApp.Models.FamilyInvitations;

namespace BackEndApp.BusinessLogic;

public class FamilyInvitationService
{
    private const int EmailOrPhoneMaxLength = 255;

    private readonly FamilyInvitationsDataAccess _familyInvitationsDataAccess;
    private readonly FamiliesDataAccess _familiesDataAccess;
    private readonly UsersDataAccess _usersDataAccess;

    public FamilyInvitationService(
        FamilyInvitationsDataAccess familyInvitationsDataAccess,
        FamiliesDataAccess familiesDataAccess,
        UsersDataAccess usersDataAccess)
    {
        _familyInvitationsDataAccess = familyInvitationsDataAccess;
        _familiesDataAccess = familiesDataAccess;
        _usersDataAccess = usersDataAccess;
    }

    public async Task<List<FamilyInvitation>> GetAllAsync()
    {
        return await _familyInvitationsDataAccess.GetAllAsync();
    }

    public async Task<FamilyInvitation?> GetByIdAsync(Guid id)
    {
        return await _familyInvitationsDataAccess.GetByIdAsync(id);
    }

    public async Task<FamilyInvitation?> AddAsync(FamilyInvitation invitation)
    {
        if (!await PrepareAndValidateAsync(invitation, validateImmutableReferences: true))
        {
            return null;
        }

        return await _familyInvitationsDataAccess.AddAsync(invitation);
    }

    public async Task<bool> UpdateAsync(FamilyInvitation invitation)
    {
        var existingInvitation = await _familyInvitationsDataAccess.GetByIdAsync(
            invitation.InvitationId);

        if (existingInvitation is null)
        {
            return false;
        }

        invitation.FamilyId = existingInvitation.FamilyId;
        invitation.InvitedByUserId = existingInvitation.InvitedByUserId;
        invitation.InvitationToken = existingInvitation.InvitationToken;
        invitation.CreatedAt = existingInvitation.CreatedAt;

        if (!await PrepareAndValidateAsync(invitation, validateImmutableReferences: false))
        {
            return false;
        }

        return await _familyInvitationsDataAccess.UpdateAsync(invitation);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _familyInvitationsDataAccess.DeleteAsync(id);
    }

    private async Task<bool> PrepareAndValidateAsync(
        FamilyInvitation invitation,
        bool validateImmutableReferences)
    {
        if (string.IsNullOrWhiteSpace(invitation.EmailOrPhone)
            || invitation.ExpiresAt <= DateTime.UtcNow)
        {
            return false;
        }

        invitation.EmailOrPhone = invitation.EmailOrPhone.Trim();

        if (invitation.EmailOrPhone.Length > EmailOrPhoneMaxLength
            || !await _familyInvitationsDataAccess.StatusExistsAsync(
                invitation.InvitationStatusId))
        {
            return false;
        }

        if (!validateImmutableReferences)
        {
            return true;
        }

        var family = await _familiesDataAccess.GetByIdAsync(invitation.FamilyId);

        if (family is null)
        {
            return false;
        }

        return await _usersDataAccess.GetByIdAsync(invitation.InvitedByUserId) is not null;
    }
}
