using BackEndApp.BusinessLogic;
using BackEndApp.Models.Reports;
using Microsoft.AspNetCore.Mvc;

namespace BackEndApp.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportsController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var reports = await _reportService.GetAllAsync();

        return Ok(reports.Select(report => report.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var report = await _reportService.GetByIdAsync(id);

        if (report is null)
        {
            return NotFound();
        }

        return Ok(report.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] ReportDto? reportDto)
    {
        if (reportDto is null)
        {
            return BadRequest();
        }

        var report = reportDto.ToModel();
        var createdReport = await _reportService.AddAsync(report);

        if (createdReport is null)
        {
            return BadRequest();
        }

        return Ok(createdReport.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] ReportDto? reportDto)
    {
        if (reportDto is null)
        {
            return BadRequest();
        }

        var existingReport = await _reportService.GetByIdAsync(id);

        if (existingReport is null)
        {
            return NotFound();
        }

        var report = reportDto.ToModel();
        report.ReportId = id;
        report.AllocationId = existingReport.AllocationId;
        report.GeneratedAt = existingReport.GeneratedAt;

        var wasUpdated = await _reportService.UpdateAsync(report);

        if (!wasUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingReport = await _reportService.GetByIdAsync(id);

        if (existingReport is null)
        {
            return NotFound();
        }

        var wasDeleted = await _reportService.DeleteAsync(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
