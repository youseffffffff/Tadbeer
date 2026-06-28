namespace BackEndApp.Models.Users;

public class User
{
    public Guid UserId { get; set; }

    public Guid FamilyId { get; set; }

    public int RoleId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? AvatarPath { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public int TotalPoints { get; set; }

    public UserDto ToDto()
    {
        return new UserDto
        {
            UserId = UserId,
            FamilyId = FamilyId,
            RoleId = RoleId,
            FullName = FullName,
            Email = Email,
            PhoneNumber = PhoneNumber,
            DateOfBirth = DateOfBirth,
            AvatarPath = AvatarPath,
            LastLoginAt = LastLoginAt,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            IsActive = IsActive,
            TotalPoints = TotalPoints
        };
    }
}
