using BackEndApp.BusinessLogic;
using BackEndApp.Models.RewardRedemptions;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/reward-redemptions")]
public class RewardRedemptionsController : ControllerBase
{
    private readonly RewardRedemptionService _rewardRedemptionService;

    public RewardRedemptionsController(RewardRedemptionService rewardRedemptionService)
    {
        _rewardRedemptionService = rewardRedemptionService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var rewardRedemptions = await _rewardRedemptionService.GetAllAsync();

        return Ok(rewardRedemptions.Select(redemption => redemption.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var rewardRedemption = await _rewardRedemptionService.GetByIdAsync(id);

        if (rewardRedemption is null)
        {
            return NotFound();
        }

        return Ok(rewardRedemption.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] RewardRedemptionDto? rewardRedemptionDto)
    {
        if (rewardRedemptionDto is null)
        {
            return BadRequest();
        }

        var rewardRedemption = rewardRedemptionDto.ToModel();
        var createdRedemption = await _rewardRedemptionService.AddAsync(rewardRedemption);

        if (createdRedemption is null)
        {
            return BadRequest();
        }

        return Ok(createdRedemption.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        [FromBody] RewardRedemptionDto? rewardRedemptionDto)
    {
        if (rewardRedemptionDto is null)
        {
            return BadRequest();
        }

        var existingRedemption = await _rewardRedemptionService.GetByIdAsync(id);

        if (existingRedemption is null)
        {
            return NotFound();
        }

        var rewardRedemption = rewardRedemptionDto.ToModel();
        rewardRedemption.RedemptionId = id;
        rewardRedemption.UserId = existingRedemption.UserId;
        rewardRedemption.RewardId = existingRedemption.RewardId;
        rewardRedemption.RedeemedAt = existingRedemption.RedeemedAt;

        var wasUpdated = await _rewardRedemptionService.UpdateAsync(rewardRedemption);

        if (!wasUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingRedemption = await _rewardRedemptionService.GetByIdAsync(id);

        if (existingRedemption is null)
        {
            return NotFound();
        }

        var wasDeleted = await _rewardRedemptionService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
