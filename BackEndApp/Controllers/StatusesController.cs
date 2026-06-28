using BackEndApp.BusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/statuses")]
public class StatusesController : ControllerBase
{
    private readonly StatusService _statusService;

    public StatusesController(StatusService statusService)
    {
        _statusService = statusService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var statuses = await _statusService.GetAllAsync();

        return Ok(statuses.Select(status => status.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetByIdAsync(int id)
    {
        var status = await _statusService.GetByIdAsync(id);

        if (status is null)
        {
            return NotFound();
        }

        return Ok(status.ToDto());
    }
}
