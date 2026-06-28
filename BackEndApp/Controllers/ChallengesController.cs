using BackEndApp.BusinessLogic;
using BackEndApp.Models.Challenges;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/challenges")]
public class ChallengesController : ControllerBase
{
    private readonly ChallengeService _challengeService;

    public ChallengesController(ChallengeService challengeService)
    {
        _challengeService = challengeService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var challenges = await _challengeService.GetAllAsync();

        return Ok(challenges.Select(challenge => challenge.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var challenge = await _challengeService.GetByIdAsync(id);

        if (challenge is null)
        {
            return NotFound();
        }

        return Ok(challenge.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] ChallengeDto? challengeDto)
    {
        if (challengeDto is null)
        {
            return BadRequest();
        }

        var challenge = challengeDto.ToModel();
        var createdChallenge = await _challengeService.AddAsync(challenge);

        if (createdChallenge is null)
        {
            return BadRequest();
        }

        return Ok(createdChallenge.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] ChallengeDto? challengeDto)
    {
        if (challengeDto is null)
        {
            return BadRequest();
        }

        var existingChallenge = await _challengeService.GetByIdAsync(id);

        if (existingChallenge is null)
        {
            return NotFound();
        }

        var challenge = challengeDto.ToModel();
        challenge.ChallengeId = id;
        challenge.AllocationId = existingChallenge.AllocationId;

        var wasUpdated = await _challengeService.UpdateAsync(challenge);

        if (!wasUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingChallenge = await _challengeService.GetByIdAsync(id);

        if (existingChallenge is null)
        {
            return NotFound();
        }

        var wasDeleted = await _challengeService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
