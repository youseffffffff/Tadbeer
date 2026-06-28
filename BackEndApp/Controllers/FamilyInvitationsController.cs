using BackEndApp.BusinessLogic;
using BackEndApp.Models.FamilyInvitations;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/family-invitations")]
public class FamilyInvitationsController : ControllerBase
{
    private readonly FamilyInvitationService _familyInvitationService;

    public FamilyInvitationsController(FamilyInvitationService familyInvitationService)
    {
        _familyInvitationService = familyInvitationService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var familyInvitations = await _familyInvitationService.GetAllAsync();

        return Ok(familyInvitations.Select(invitation => invitation.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var familyInvitation = await _familyInvitationService.GetByIdAsync(id);

        if (familyInvitation is null)
        {
            return NotFound();
        }

        return Ok(familyInvitation.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] FamilyInvitationDto? familyInvitationDto)
    {
        if (familyInvitationDto is null)
        {
            return BadRequest();
        }

        var familyInvitation = familyInvitationDto.ToModel();
        var createdInvitation = await _familyInvitationService.AddAsync(familyInvitation);

        if (createdInvitation is null)
        {
            return BadRequest();
        }

        return Ok(createdInvitation.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        [FromBody] FamilyInvitationDto? familyInvitationDto)
    {
        if (familyInvitationDto is null)
        {
            return BadRequest();
        }

        var existingInvitation = await _familyInvitationService.GetByIdAsync(id);

        if (existingInvitation is null)
        {
            return NotFound();
        }

        var familyInvitation = familyInvitationDto.ToModel();
        familyInvitation.InvitationId = id;
        familyInvitation.FamilyId = existingInvitation.FamilyId;
        familyInvitation.InvitedByUserId = existingInvitation.InvitedByUserId;
        familyInvitation.InvitationToken = existingInvitation.InvitationToken;
        familyInvitation.CreatedAt = existingInvitation.CreatedAt;

        var wasUpdated = await _familyInvitationService.UpdateAsync(familyInvitation);

        if (!wasUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingInvitation = await _familyInvitationService.GetByIdAsync(id);

        if (existingInvitation is null)
        {
            return NotFound();
        }

        var wasDeleted = await _familyInvitationService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
