namespace BackEndApp.Models.Roles;

public class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public RoleDto ToDto()
    {
        return new RoleDto
        {
            RoleId = RoleId,
            RoleName = RoleName,
            Description = Description
        };
    }
}
