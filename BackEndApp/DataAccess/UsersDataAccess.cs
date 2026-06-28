using System.Data;
using BackEndApp.Models.Users;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class UsersDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public UsersDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<User>> GetAllAsync()
    {
        const string query = """
            SELECT
                UserId,
                FamilyId,
                RoleId,
                FullName,
                Email,
                PasswordHash,
                PhoneNumber,
                DateOfBirth,
                AvatarPath,
                LastLoginAt,
                CreatedAt,
                UpdatedAt,
                IsActive,
                TotalPoints
            FROM Users;
            """;

        var users = new List<User>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                FamilyId = reader.GetGuid(reader.GetOrdinal("FamilyId")),
                RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                AvatarPath = reader.IsDBNull(reader.GetOrdinal("AvatarPath"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("AvatarPath")),
                LastLoginAt = reader.IsDBNull(reader.GetOrdinal("LastLoginAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("LastLoginAt")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive"))
                    ? null
                    : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                TotalPoints = reader.GetInt32(reader.GetOrdinal("TotalPoints"))
            });
        }

        return users;
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        const string query = """
            SELECT
                UserId,
                FamilyId,
                RoleId,
                FullName,
                Email,
                PasswordHash,
                PhoneNumber,
                DateOfBirth,
                AvatarPath,
                LastLoginAt,
                CreatedAt,
                UpdatedAt,
                IsActive,
                TotalPoints
            FROM Users
            WHERE UserId = @UserId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new User
        {
            UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
            FamilyId = reader.GetGuid(reader.GetOrdinal("FamilyId")),
            RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
            FullName = reader.GetString(reader.GetOrdinal("FullName")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber"))
                ? null
                : reader.GetString(reader.GetOrdinal("PhoneNumber")),
            DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
            AvatarPath = reader.IsDBNull(reader.GetOrdinal("AvatarPath"))
                ? null
                : reader.GetString(reader.GetOrdinal("AvatarPath")),
            LastLoginAt = reader.IsDBNull(reader.GetOrdinal("LastLoginAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("LastLoginAt")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
            IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive"))
                ? null
                : reader.GetBoolean(reader.GetOrdinal("IsActive")),
            TotalPoints = reader.GetInt32(reader.GetOrdinal("TotalPoints"))
        };
    }

    public async Task<User?> AddAsync(User user)
    {
        user.UserId = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO Users
            (
                UserId,
                FamilyId,
                RoleId,
                FullName,
                Email,
                PasswordHash,
                PhoneNumber,
                DateOfBirth,
                AvatarPath,
                LastLoginAt,
                CreatedAt,
                UpdatedAt,
                IsActive,
                TotalPoints
            )
            VALUES
            (
                @UserId,
                @FamilyId,
                @RoleId,
                @FullName,
                @Email,
                @PasswordHash,
                @PhoneNumber,
                @DateOfBirth,
                @AvatarPath,
                @LastLoginAt,
                @CreatedAt,
                @UpdatedAt,
                @IsActive,
                @TotalPoints
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = user.UserId;
        command.Parameters.Add("@FamilyId", SqlDbType.UniqueIdentifier).Value = user.FamilyId;
        command.Parameters.Add("@RoleId", SqlDbType.Int).Value = user.RoleId;
        command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = user.FullName;
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = user.Email;
        command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, -1).Value = user.PasswordHash;
        command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value =
            (object?)user.PhoneNumber ?? DBNull.Value;
        command.Parameters.Add("@DateOfBirth", SqlDbType.Date).Value =
            (object?)user.DateOfBirth ?? DBNull.Value;
        command.Parameters.Add("@AvatarPath", SqlDbType.NVarChar, -1).Value =
            (object?)user.AvatarPath ?? DBNull.Value;
        command.Parameters.Add("@LastLoginAt", SqlDbType.DateTime2).Value =
            (object?)user.LastLoginAt ?? DBNull.Value;
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value =
            (object?)user.CreatedAt ?? DBNull.Value;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value =
            (object?)user.UpdatedAt ?? DBNull.Value;
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
            (object?)user.IsActive ?? DBNull.Value;
        command.Parameters.Add("@TotalPoints", SqlDbType.Int).Value = user.TotalPoints;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? user : null;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        const string query = """
            UPDATE Users
            SET
                FamilyId = @FamilyId,
                RoleId = @RoleId,
                FullName = @FullName,
                Email = @Email,
                PasswordHash = @PasswordHash,
                PhoneNumber = @PhoneNumber,
                DateOfBirth = @DateOfBirth,
                AvatarPath = @AvatarPath,
                LastLoginAt = @LastLoginAt,
                UpdatedAt = @UpdatedAt,
                IsActive = @IsActive,
                TotalPoints = @TotalPoints
            WHERE UserId = @UserId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = user.UserId;
        command.Parameters.Add("@FamilyId", SqlDbType.UniqueIdentifier).Value = user.FamilyId;
        command.Parameters.Add("@RoleId", SqlDbType.Int).Value = user.RoleId;
        command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = user.FullName;
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = user.Email;
        command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, -1).Value = user.PasswordHash;
        command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value =
            (object?)user.PhoneNumber ?? DBNull.Value;
        command.Parameters.Add("@DateOfBirth", SqlDbType.Date).Value =
            (object?)user.DateOfBirth ?? DBNull.Value;
        command.Parameters.Add("@AvatarPath", SqlDbType.NVarChar, -1).Value =
            (object?)user.AvatarPath ?? DBNull.Value;
        command.Parameters.Add("@LastLoginAt", SqlDbType.DateTime2).Value =
            (object?)user.LastLoginAt ?? DBNull.Value;
        user.UpdatedAt = DateTime.UtcNow;

        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value =
            (object?)user.UpdatedAt ?? DBNull.Value;
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
            (object?)user.IsActive ?? DBNull.Value;
        command.Parameters.Add("@TotalPoints", SqlDbType.Int).Value = user.TotalPoints;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid userId)
    {
        const string query = """
            DELETE FROM Users
            WHERE UserId = @UserId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
