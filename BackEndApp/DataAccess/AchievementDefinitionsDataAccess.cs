using System.Data;
using BackEndApp.Models.AchievementDefinitions;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class AchievementDefinitionsDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public AchievementDefinitionsDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<AchievementDefinition>> GetAllAsync()
    {
        const string query = """
            SELECT
                AchievementId,
                Title,
                Description,
                XPReward,
                BadgeIcon,
                CreatedAt
            FROM AchievementDefinitions;
            """;

        var achievementDefinitions = new List<AchievementDefinition>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            achievementDefinitions.Add(new AchievementDefinition
            {
                AchievementId = reader.GetGuid(reader.GetOrdinal("AchievementId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),
                XPReward = reader.IsDBNull(reader.GetOrdinal("XPReward"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("XPReward")),
                BadgeIcon = reader.IsDBNull(reader.GetOrdinal("BadgeIcon"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("BadgeIcon")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }

        return achievementDefinitions;
    }

    public async Task<AchievementDefinition?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                AchievementId,
                Title,
                Description,
                XPReward,
                BadgeIcon,
                CreatedAt
            FROM AchievementDefinitions
            WHERE AchievementId = @AchievementId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@AchievementId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new AchievementDefinition
        {
            AchievementId = reader.GetGuid(reader.GetOrdinal("AchievementId")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                ? null
                : reader.GetString(reader.GetOrdinal("Description")),
            XPReward = reader.IsDBNull(reader.GetOrdinal("XPReward"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("XPReward")),
            BadgeIcon = reader.IsDBNull(reader.GetOrdinal("BadgeIcon"))
                ? null
                : reader.GetString(reader.GetOrdinal("BadgeIcon")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }

    public async Task<bool> ExistsByTitleAsync(string title, Guid? excludedAchievementId = null)
    {
        const string query = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM AchievementDefinitions
                    WHERE LOWER(LTRIM(RTRIM(Title))) = LOWER(@Title)
                      AND (@ExcludedAchievementId IS NULL OR AchievementId <> @ExcludedAchievementId)
                )
                THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@Title", SqlDbType.NVarChar, 255).Value = title;
        command.Parameters.Add("@ExcludedAchievementId", SqlDbType.UniqueIdentifier).Value =
            (object?)excludedAchievementId ?? DBNull.Value;

        return (bool)(await command.ExecuteScalarAsync() ?? false);
    }

    public async Task<AchievementDefinition?> AddAsync(AchievementDefinition achievement)
    {
        achievement.AchievementId = Guid.NewGuid();
        achievement.CreatedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO AchievementDefinitions
            (
                AchievementId,
                Title,
                Description,
                XPReward,
                BadgeIcon,
                CreatedAt
            )
            VALUES
            (
                @AchievementId,
                @Title,
                @Description,
                @XPReward,
                @BadgeIcon,
                @CreatedAt
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@AchievementId", SqlDbType.UniqueIdentifier).Value =
            achievement.AchievementId;
        command.Parameters.Add("@Title", SqlDbType.NVarChar, 255).Value = achievement.Title;
        command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value =
            (object?)achievement.Description ?? DBNull.Value;
        command.Parameters.Add("@XPReward", SqlDbType.Int).Value =
            (object?)achievement.XPReward ?? DBNull.Value;
        command.Parameters.Add("@BadgeIcon", SqlDbType.NVarChar, 255).Value =
            (object?)achievement.BadgeIcon ?? DBNull.Value;
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value =
            (object?)achievement.CreatedAt ?? DBNull.Value;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? achievement : null;
    }

    public async Task<bool> UpdateAsync(AchievementDefinition achievement)
    {
        const string query = """
            UPDATE AchievementDefinitions
            SET
                Title = @Title,
                Description = @Description,
                XPReward = @XPReward,
                BadgeIcon = @BadgeIcon
            WHERE AchievementId = @AchievementId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@AchievementId", SqlDbType.UniqueIdentifier).Value =
            achievement.AchievementId;
        command.Parameters.Add("@Title", SqlDbType.NVarChar, 255).Value = achievement.Title;
        command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value =
            (object?)achievement.Description ?? DBNull.Value;
        command.Parameters.Add("@XPReward", SqlDbType.Int).Value =
            (object?)achievement.XPReward ?? DBNull.Value;
        command.Parameters.Add("@BadgeIcon", SqlDbType.NVarChar, 255).Value =
            (object?)achievement.BadgeIcon ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM AchievementDefinitions
            WHERE AchievementId = @AchievementId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@AchievementId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
