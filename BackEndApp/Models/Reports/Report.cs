namespace BackEndApp.Models.Reports;

public class Report
{
    public Guid ReportId { get; set; }

    public string? ReportDataJson { get; set; }

    public DateTime? GeneratedAt { get; set; }

    public Guid AllocationId { get; set; }

    public ReportDto ToDto()
    {
        return new ReportDto
        {
            ReportId = ReportId,
            AllocationId = AllocationId,
            ReportDataJson = ReportDataJson,
            GeneratedAt = GeneratedAt
        };
    }
}
