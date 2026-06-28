namespace BackEndApp.Models.Reports;

public class ReportDto
{
    public Guid ReportId { get; set; }

    public Guid AllocationId { get; set; }

    public string? ReportDataJson { get; set; }

    public DateTime? GeneratedAt { get; set; }

    public Report ToModel()
    {
        return new Report
        {
            AllocationId = AllocationId,
            ReportDataJson = ReportDataJson
        };
    }
}
