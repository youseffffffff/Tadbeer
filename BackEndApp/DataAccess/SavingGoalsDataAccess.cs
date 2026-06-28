using System.Data;
using BackEndApp.Models.SavingGoals;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class SavingGoalsDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public SavingGoalsDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<SavingGoal>> GetAllAsync()
    {
        const string query = """
            SELECT
                GoalId,
                UserId,
                GoalName,
                GoalDescription,
                TargetAmount,
                TargetDate,
                Status,
                CreatedAt,
                UpdatedAt
            FROM SavingGoals;
            """;

        var savingGoals = new List<SavingGoal>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            savingGoals.Add(new SavingGoal
            {
                GoalId = reader.GetGuid(reader.GetOrdinal("GoalId")),
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                GoalName = reader.GetString(reader.GetOrdinal("GoalName")),
                GoalDescription = reader.IsDBNull(reader.GetOrdinal("GoalDescription"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("GoalDescription")),
                TargetAmount = reader.GetDecimal(reader.GetOrdinal("TargetAmount")),
                TargetDate = reader.IsDBNull(reader.GetOrdinal("TargetDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("TargetDate")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status"))
                    ? null
                    : reader.GetByte(reader.GetOrdinal("Status")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            });
        }

        return savingGoals;
    }

    public async Task<SavingGoal?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                GoalId,
                UserId,
                GoalName,
                GoalDescription,
                TargetAmount,
                TargetDate,
                Status,
                CreatedAt,
                UpdatedAt
            FROM SavingGoals
            WHERE GoalId = @GoalId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@GoalId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new SavingGoal
        {
            GoalId = reader.GetGuid(reader.GetOrdinal("GoalId")),
            UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
            GoalName = reader.GetString(reader.GetOrdinal("GoalName")),
            GoalDescription = reader.IsDBNull(reader.GetOrdinal("GoalDescription"))
                ? null
                : reader.GetString(reader.GetOrdinal("GoalDescription")),
            TargetAmount = reader.GetDecimal(reader.GetOrdinal("TargetAmount")),
            TargetDate = reader.IsDBNull(reader.GetOrdinal("TargetDate"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("TargetDate")),
            Status = reader.IsDBNull(reader.GetOrdinal("Status"))
                ? null
                : reader.GetByte(reader.GetOrdinal("Status")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
        };
    }

    public async Task<SavingGoal?> AddAsync(SavingGoal goal)
    {
        goal.GoalId = Guid.NewGuid();
        goal.CreatedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO SavingGoals
            (
                GoalId,
                UserId,
                GoalName,
                GoalDescription,
                TargetAmount,
                TargetDate,
                Status,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @GoalId,
                @UserId,
                @GoalName,
                @GoalDescription,
                @TargetAmount,
                @TargetDate,
                @Status,
                @CreatedAt,
                @UpdatedAt
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@GoalId", SqlDbType.UniqueIdentifier).Value = goal.GoalId;
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = goal.UserId;
        command.Parameters.Add("@GoalName", SqlDbType.NVarChar, 100).Value = goal.GoalName;
        command.Parameters.Add("@GoalDescription", SqlDbType.NVarChar, -1).Value =
            (object?)goal.GoalDescription ?? DBNull.Value;

        var amountParameter = command.Parameters.Add("@TargetAmount", SqlDbType.Decimal);
        amountParameter.Precision = 18;
        amountParameter.Scale = 2;
        amountParameter.Value = goal.TargetAmount;

        command.Parameters.Add("@TargetDate", SqlDbType.Date).Value =
            (object?)goal.TargetDate ?? DBNull.Value;
        command.Parameters.Add("@Status", SqlDbType.TinyInt).Value =
            (object?)goal.Status ?? DBNull.Value;
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value =
            (object?)goal.CreatedAt ?? DBNull.Value;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value =
            (object?)goal.UpdatedAt ?? DBNull.Value;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? goal : null;
    }

    public async Task<bool> UpdateAsync(SavingGoal goal)
    {
        goal.UpdatedAt = DateTime.UtcNow;

        const string query = """
            UPDATE SavingGoals
            SET
                UserId = @UserId,
                GoalName = @GoalName,
                GoalDescription = @GoalDescription,
                TargetAmount = @TargetAmount,
                TargetDate = @TargetDate,
                Status = @Status,
                UpdatedAt = @UpdatedAt
            WHERE GoalId = @GoalId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@GoalId", SqlDbType.UniqueIdentifier).Value = goal.GoalId;
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = goal.UserId;
        command.Parameters.Add("@GoalName", SqlDbType.NVarChar, 100).Value = goal.GoalName;
        command.Parameters.Add("@GoalDescription", SqlDbType.NVarChar, -1).Value =
            (object?)goal.GoalDescription ?? DBNull.Value;

        var amountParameter = command.Parameters.Add("@TargetAmount", SqlDbType.Decimal);
        amountParameter.Precision = 18;
        amountParameter.Scale = 2;
        amountParameter.Value = goal.TargetAmount;

        command.Parameters.Add("@TargetDate", SqlDbType.Date).Value =
            (object?)goal.TargetDate ?? DBNull.Value;
        command.Parameters.Add("@Status", SqlDbType.TinyInt).Value =
            (object?)goal.Status ?? DBNull.Value;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value =
            (object?)goal.UpdatedAt ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM SavingGoals
            WHERE GoalId = @GoalId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@GoalId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
