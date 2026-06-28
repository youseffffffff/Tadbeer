using System.Data;
using BackEndApp.Models.RewardRedemptions;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class RewardRedemptionsDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public RewardRedemptionsDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<RewardRedemption>> GetAllAsync()
    {
        const string query = """
            SELECT
                RedemptionId,
                UserId,
                RewardId,
                RedeemedAt,
                ApprovedByUserId
            FROM RewardRedemptions;
            """;

        var rewardRedemptions = new List<RewardRedemption>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            rewardRedemptions.Add(new RewardRedemption
            {
                RedemptionId = reader.GetGuid(reader.GetOrdinal("RedemptionId")),
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                RewardId = reader.GetGuid(reader.GetOrdinal("RewardId")),
                RedeemedAt = reader.IsDBNull(reader.GetOrdinal("RedeemedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("RedeemedAt")),
                ApprovedByUserId = reader.IsDBNull(reader.GetOrdinal("ApprovedByUserId"))
                    ? null
                    : reader.GetGuid(reader.GetOrdinal("ApprovedByUserId"))
            });
        }

        return rewardRedemptions;
    }

    public async Task<RewardRedemption?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                RedemptionId,
                UserId,
                RewardId,
                RedeemedAt,
                ApprovedByUserId
            FROM RewardRedemptions
            WHERE RedemptionId = @RedemptionId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@RedemptionId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new RewardRedemption
        {
            RedemptionId = reader.GetGuid(reader.GetOrdinal("RedemptionId")),
            UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
            RewardId = reader.GetGuid(reader.GetOrdinal("RewardId")),
            RedeemedAt = reader.IsDBNull(reader.GetOrdinal("RedeemedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("RedeemedAt")),
            ApprovedByUserId = reader.IsDBNull(reader.GetOrdinal("ApprovedByUserId"))
                ? null
                : reader.GetGuid(reader.GetOrdinal("ApprovedByUserId"))
        };
    }

    public async Task<RewardRedemption?> AddAsync(RewardRedemption redemption)
    {
        redemption.RedemptionId = Guid.NewGuid();
        redemption.RedeemedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO RewardRedemptions
            (
                RedemptionId,
                UserId,
                RewardId,
                RedeemedAt,
                ApprovedByUserId
            )
            VALUES
            (
                @RedemptionId,
                @UserId,
                @RewardId,
                @RedeemedAt,
                @ApprovedByUserId
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@RedemptionId", SqlDbType.UniqueIdentifier).Value = redemption.RedemptionId;
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = redemption.UserId;
        command.Parameters.Add("@RewardId", SqlDbType.UniqueIdentifier).Value = redemption.RewardId;
        command.Parameters.Add("@RedeemedAt", SqlDbType.DateTime2).Value =
            (object?)redemption.RedeemedAt ?? DBNull.Value;
        command.Parameters.Add("@ApprovedByUserId", SqlDbType.UniqueIdentifier).Value =
            (object?)redemption.ApprovedByUserId ?? DBNull.Value;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? redemption : null;
    }

    public async Task<bool> UpdateAsync(RewardRedemption redemption)
    {
        const string query = """
            UPDATE RewardRedemptions
            SET ApprovedByUserId = @ApprovedByUserId
            WHERE RedemptionId = @RedemptionId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@RedemptionId", SqlDbType.UniqueIdentifier).Value = redemption.RedemptionId;
        command.Parameters.Add("@ApprovedByUserId", SqlDbType.UniqueIdentifier).Value =
            (object?)redemption.ApprovedByUserId ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM RewardRedemptions
            WHERE RedemptionId = @RedemptionId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@RedemptionId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
