using BackEndApp.BusinessLogic;
using BackEndApp.Models.Families;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/families")]
public class FamiliesController : ControllerBase
{
    private readonly FamilyService _familyService;

    public FamiliesController(FamilyService familyService)
    {
        _familyService = familyService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var families = await _familyService.GetAllAsync();

        return Ok(families.Select(family => family.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var family = await _familyService.GetByIdAsync(id);

        if (family is null)
        {
            return NotFound();
        }

        return Ok(family.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] FamilyDto? familyDto)
    {
        if (familyDto is null)
        {
            return BadRequest();
        }

        var family = familyDto.ToModel();
        var createdFamily = await _familyService.AddAsync(family);

        if (createdFamily is null)
        {
            return BadRequest();
        }

        return Ok(createdFamily.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] FamilyDto? familyDto)
    {
        if (familyDto is null)
        {
            return BadRequest();
        }

        var existingFamily = await _familyService.GetByIdAsync(id);

        if (existingFamily is null)
        {
            return NotFound();
        }

        var family = familyDto.ToModel();
        family.FamilyId = id;

        var wasUpdated = await _familyService.UpdateAsync(family);

        if (!wasUpdated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingFamily = await _familyService.GetByIdAsync(id);

        if (existingFamily is null)
        {
            return NotFound();
        }

        var wasDeleted = await _familyService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
