using System.Data;
using BackEndApp.Models.GoalContributions;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class GoalContributionsDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public GoalContributionsDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<GoalContribution>> GetAllAsync()
    {
        const string query = """
            SELECT
                ContributionId,
                GoalId,
                Amount,
                ContributionDate
            FROM GoalContributions;
            """;

        var goalContributions = new List<GoalContribution>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            goalContributions.Add(new GoalContribution
            {
                ContributionId = reader.GetGuid(reader.GetOrdinal("ContributionId")),
                GoalId = reader.GetGuid(reader.GetOrdinal("GoalId")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                ContributionDate = reader.IsDBNull(reader.GetOrdinal("ContributionDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("ContributionDate"))
            });
        }

        return goalContributions;
    }

    public async Task<GoalContribution?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                ContributionId,
                GoalId,
                Amount,
                ContributionDate
            FROM GoalContributions
            WHERE ContributionId = @ContributionId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ContributionId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new GoalContribution
        {
            ContributionId = reader.GetGuid(reader.GetOrdinal("ContributionId")),
            GoalId = reader.GetGuid(reader.GetOrdinal("GoalId")),
            Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
            ContributionDate = reader.IsDBNull(reader.GetOrdinal("ContributionDate"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("ContributionDate"))
        };
    }

    public async Task<GoalContribution?> AddAsync(GoalContribution contribution)
    {
        contribution.ContributionId = Guid.NewGuid();
        contribution.ContributionDate ??= DateTime.UtcNow;

        const string query = """
            INSERT INTO GoalContributions
            (
                ContributionId,
                GoalId,
                Amount,
                ContributionDate
            )
            VALUES
            (
                @ContributionId,
                @GoalId,
                @Amount,
                @ContributionDate
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ContributionId", SqlDbType.UniqueIdentifier).Value =
            contribution.ContributionId;
        command.Parameters.Add("@GoalId", SqlDbType.UniqueIdentifier).Value = contribution.GoalId;

        var amountParameter = command.Parameters.Add("@Amount", SqlDbType.Decimal);
        amountParameter.Precision = 18;
        amountParameter.Scale = 2;
        amountParameter.Value = contribution.Amount;

        command.Parameters.Add("@ContributionDate", SqlDbType.DateTime2).Value =
            (object?)contribution.ContributionDate ?? DBNull.Value;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? contribution : null;
    }

    public async Task<bool> UpdateAsync(GoalContribution contribution)
    {
        const string query = """
            UPDATE GoalContributions
            SET
                Amount = @Amount,
                ContributionDate = @ContributionDate
            WHERE ContributionId = @ContributionId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ContributionId", SqlDbType.UniqueIdentifier).Value =
            contribution.ContributionId;

        var amountParameter = command.Parameters.Add("@Amount", SqlDbType.Decimal);
        amountParameter.Precision = 18;
        amountParameter.Scale = 2;
        amountParameter.Value = contribution.Amount;

        command.Parameters.Add("@ContributionDate", SqlDbType.DateTime2).Value =
            (object?)contribution.ContributionDate ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM GoalContributions
            WHERE ContributionId = @ContributionId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ContributionId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
