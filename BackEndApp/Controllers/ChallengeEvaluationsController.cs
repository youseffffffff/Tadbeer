using BackEndApp.BusinessLogic;
using BackEndApp.Models.ChallengeEvaluations;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/challenge-evaluations")]
public class ChallengeEvaluationsController : ControllerBase
{
    private readonly ChallengeEvaluationService _challengeEvaluationService;

    public ChallengeEvaluationsController(ChallengeEvaluationService challengeEvaluationService)
    {
        _challengeEvaluationService = challengeEvaluationService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var challengeEvaluations = await _challengeEvaluationService.GetAllAsync();

        return Ok(challengeEvaluations.Select(evaluation => evaluation.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var challengeEvaluation = await _challengeEvaluationService.GetByIdAsync(id);

        if (challengeEvaluation is null)
        {
            return NotFound();
        }

        return Ok(challengeEvaluation.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync(
        [FromBody] ChallengeEvaluationDto? challengeEvaluationDto)
    {
        if (challengeEvaluationDto is null)
        {
            return BadRequest();
        }

        var challengeEvaluation = challengeEvaluationDto.ToModel();
        var createdEvaluation = await _challengeEvaluationService.AddAsync(challengeEvaluation);

        if (createdEvaluation is null)
        {
            return BadRequest();
        }

        return Ok(createdEvaluation.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        [FromBody] ChallengeEvaluationDto? challengeEvaluationDto)
    {
        if (challengeEvaluationDto is null)
        {
            return BadRequest();
        }

        var existingEvaluation = await _challengeEvaluationService.GetByIdAsync(id);

        if (existingEvaluation is null)
        {
            return NotFound();
        }

        var challengeEvaluation = challengeEvaluationDto.ToModel();
        challengeEvaluation.EvaluationId = id;
        challengeEvaluation.ChallengeId = existingEvaluation.ChallengeId;

        var wasUpdated = await _challengeEvaluationService.UpdateAsync(challengeEvaluation);

        if (!wasUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingEvaluation = await _challengeEvaluationService.GetByIdAsync(id);

        if (existingEvaluation is null)
        {
            return NotFound();
        }

        var wasDeleted = await _challengeEvaluationService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
