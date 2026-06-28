namespace BackEndApp.Models.Statuses;

public class StatusDto
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public string? Description { get; set; }
}
