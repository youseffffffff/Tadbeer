using BackEndApp.BusinessLogic;
using BackEndApp.Models.Rewards;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/rewards")]
public class RewardsController : ControllerBase
{
    private readonly RewardService _rewardService;

    public RewardsController(RewardService rewardService)
    {
        _rewardService = rewardService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var rewards = await _rewardService.GetAllAsync();

        return Ok(rewards.Select(reward => reward.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var reward = await _rewardService.GetByIdAsync(id);

        if (reward is null)
        {
            return NotFound();
        }

        return Ok(reward.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] RewardDto? rewardDto)
    {
        if (rewardDto is null)
        {
            return BadRequest();
        }

        var reward = rewardDto.ToModel();
        var createdReward = await _rewardService.AddAsync(reward);

        if (createdReward is null)
        {
            return BadRequest();
        }

        return Ok(createdReward.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] RewardDto? rewardDto)
    {
        if (rewardDto is null)
        {
            return BadRequest();
        }

        var existingReward = await _rewardService.GetByIdAsync(id);

        if (existingReward is null)
        {
            return NotFound();
        }

        var reward = rewardDto.ToModel();
        reward.RewardId = id;

        var wasUpdated = await _rewardService.UpdateAsync(reward);

        if (!wasUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingReward = await _rewardService.GetByIdAsync(id);

        if (existingReward is null)
        {
            return NotFound();
        }

        var wasDeleted = await _rewardService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
