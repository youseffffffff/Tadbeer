using System.Data;
using BackEndApp.Models.UserAchievements;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class UserAchievementsDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public UserAchievementsDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<UserAchievement>> GetAllAsync()
    {
        const string query = """
            SELECT
                UserAchievementId,
                UserId,
                AchievementId,
                EarnedAt,
                AwardedByUserId
            FROM UserAchievements;
            """;

        var userAchievements = new List<UserAchievement>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            userAchievements.Add(new UserAchievement
            {
                UserAchievementId = reader.GetGuid(reader.GetOrdinal("UserAchievementId")),
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                AchievementId = reader.GetGuid(reader.GetOrdinal("AchievementId")),
                EarnedAt = reader.IsDBNull(reader.GetOrdinal("EarnedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("EarnedAt")),
                AwardedByUserId = reader.IsDBNull(reader.GetOrdinal("AwardedByUserId"))
                    ? null
                    : reader.GetGuid(reader.GetOrdinal("AwardedByUserId"))
            });
        }

        return userAchievements;
    }

    public async Task<UserAchievement?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                UserAchievementId,
                UserId,
                AchievementId,
                EarnedAt,
                AwardedByUserId
            FROM UserAchievements
            WHERE UserAchievementId = @UserAchievementId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@UserAchievementId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new UserAchievement
        {
            UserAchievementId = reader.GetGuid(reader.GetOrdinal("UserAchievementId")),
            UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
            AchievementId = reader.GetGuid(reader.GetOrdinal("AchievementId")),
            EarnedAt = reader.IsDBNull(reader.GetOrdinal("EarnedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("EarnedAt")),
            AwardedByUserId = reader.IsDBNull(reader.GetOrdinal("AwardedByUserId"))
                ? null
                : reader.GetGuid(reader.GetOrdinal("AwardedByUserId"))
        };
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid achievementId)
    {
        const string query = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM UserAchievements
                    WHERE UserId = @UserId
                      AND AchievementId = @AchievementId
                )
                THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;
        command.Parameters.Add("@AchievementId", SqlDbType.UniqueIdentifier).Value = achievementId;

        return (bool)(await command.ExecuteScalarAsync() ?? false);
    }

    public async Task<UserAchievement?> AddAsync(UserAchievement achievement)
    {
        achievement.UserAchievementId = Guid.NewGuid();
        achievement.EarnedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO UserAchievements
            (
                UserAchievementId,
                UserId,
                AchievementId,
                EarnedAt,
                AwardedByUserId
            )
            VALUES
            (
                @UserAchievementId,
                @UserId,
                @AchievementId,
                @EarnedAt,
                @AwardedByUserId
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@UserAchievementId", SqlDbType.UniqueIdentifier).Value =
            achievement.UserAchievementId;
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = achievement.UserId;
        command.Parameters.Add("@AchievementId", SqlDbType.UniqueIdentifier).Value =
            achievement.AchievementId;
        command.Parameters.Add("@EarnedAt", SqlDbType.DateTime2).Value =
            (object?)achievement.EarnedAt ?? DBNull.Value;
        command.Parameters.Add("@AwardedByUserId", SqlDbType.UniqueIdentifier).Value =
            (object?)achievement.AwardedByUserId ?? DBNull.Value;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? achievement : null;
    }

    public async Task<bool> UpdateAsync(UserAchievement achievement)
    {
        const string query = """
            UPDATE UserAchievements
            SET AwardedByUserId = @AwardedByUserId
            WHERE UserAchievementId = @UserAchievementId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@UserAchievementId", SqlDbType.UniqueIdentifier).Value =
            achievement.UserAchievementId;
        command.Parameters.Add("@AwardedByUserId", SqlDbType.UniqueIdentifier).Value =
            (object?)achievement.AwardedByUserId ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM UserAchievements
            WHERE UserAchievementId = @UserAchievementId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@UserAchievementId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
