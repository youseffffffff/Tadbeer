namespace BackEndApp.Models.Statuses;

public class Status
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public StatusDto ToDto()
    {
        return new StatusDto
        {
            StatusId = StatusId,
            StatusName = StatusName,
            Description = Description
        };
    }
}
