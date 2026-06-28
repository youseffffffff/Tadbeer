using System.Data;
using BackEndApp.Models.Rewards;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class RewardsDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public RewardsDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<Reward>> GetAllAsync()
    {
        const string query = """
            SELECT
                RewardId,
                RewardName,
                Description,
                PointsCost,
                CreatedAt
            FROM Rewards;
            """;

        var rewards = new List<Reward>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            rewards.Add(new Reward
            {
                RewardId = reader.GetGuid(reader.GetOrdinal("RewardId")),
                RewardName = reader.GetString(reader.GetOrdinal("RewardName")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),
                PointsCost = reader.IsDBNull(reader.GetOrdinal("PointsCost"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("PointsCost")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }

        return rewards;
    }

    public async Task<Reward?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                RewardId,
                RewardName,
                Description,
                PointsCost,
                CreatedAt
            FROM Rewards
            WHERE RewardId = @RewardId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@RewardId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Reward
        {
            RewardId = reader.GetGuid(reader.GetOrdinal("RewardId")),
            RewardName = reader.GetString(reader.GetOrdinal("RewardName")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                ? null
                : reader.GetString(reader.GetOrdinal("Description")),
            PointsCost = reader.IsDBNull(reader.GetOrdinal("PointsCost"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("PointsCost")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }

    public async Task<bool> ExistsByNameAsync(string rewardName, Guid? excludedRewardId = null)
    {
        const string query = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM Rewards
                    WHERE LOWER(LTRIM(RTRIM(RewardName))) = LOWER(@RewardName)
                      AND (@ExcludedRewardId IS NULL OR RewardId <> @ExcludedRewardId)
                )
                THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@RewardName", SqlDbType.NVarChar, 255).Value = rewardName;
        command.Parameters.Add("@ExcludedRewardId", SqlDbType.UniqueIdentifier).Value =
            (object?)excludedRewardId ?? DBNull.Value;

        return (bool)(await command.ExecuteScalarAsync() ?? false);
    }

    public async Task<Reward?> AddAsync(Reward reward)
    {
        reward.RewardId = Guid.NewGuid();
        reward.CreatedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO Rewards
            (
                RewardId,
                RewardName,
                Description,
                PointsCost,
                CreatedAt
            )
            VALUES
            (
                @RewardId,
                @RewardName,
                @Description,
                @PointsCost,
                @CreatedAt
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@RewardId", SqlDbType.UniqueIdentifier).Value = reward.RewardId;
        command.Parameters.Add("@RewardName", SqlDbType.NVarChar, 255).Value = reward.RewardName;
        command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value =
            (object?)reward.Description ?? DBNull.Value;
        command.Parameters.Add("@PointsCost", SqlDbType.Int).Value =
            (object?)reward.PointsCost ?? DBNull.Value;
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value =
            (object?)reward.CreatedAt ?? DBNull.Value;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? reward : null;
    }

    public async Task<bool> UpdateAsync(Reward reward)
    {
        const string query = """
            UPDATE Rewards
            SET
                RewardName = @RewardName,
                Description = @Description,
                PointsCost = @PointsCost
            WHERE RewardId = @RewardId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@RewardId", SqlDbType.UniqueIdentifier).Value = reward.RewardId;
        command.Parameters.Add("@RewardName", SqlDbType.NVarChar, 255).Value = reward.RewardName;
        command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value =
            (object?)reward.Description ?? DBNull.Value;
        command.Parameters.Add("@PointsCost", SqlDbType.Int).Value =
            (object?)reward.PointsCost ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM Rewards
            WHERE RewardId = @RewardId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@RewardId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
