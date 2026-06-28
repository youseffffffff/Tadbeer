using System.Data;
using BackEndApp.Models.ChallengeEvaluations;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class ChallengeEvaluationsDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public ChallengeEvaluationsDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<ChallengeEvaluation>> GetAllAsync()
    {
        const string query = """
            SELECT
                EvaluationId,
                ChallengeId,
                CompletionPercentage,
                SuccessLevel,
                EvaluationSummary,
                Recommendations,
                EvaluatedAt
            FROM ChallengeEvaluations;
            """;

        var challengeEvaluations = new List<ChallengeEvaluation>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            challengeEvaluations.Add(new ChallengeEvaluation
            {
                EvaluationId = reader.GetGuid(reader.GetOrdinal("EvaluationId")),
                ChallengeId = reader.GetGuid(reader.GetOrdinal("ChallengeId")),
                CompletionPercentage = reader.IsDBNull(reader.GetOrdinal("CompletionPercentage"))
                    ? null
                    : reader.GetDecimal(reader.GetOrdinal("CompletionPercentage")),
                SuccessLevel = reader.IsDBNull(reader.GetOrdinal("SuccessLevel"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("SuccessLevel")),
                EvaluationSummary = reader.IsDBNull(reader.GetOrdinal("EvaluationSummary"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("EvaluationSummary")),
                Recommendations = reader.IsDBNull(reader.GetOrdinal("Recommendations"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Recommendations")),
                EvaluatedAt = reader.IsDBNull(reader.GetOrdinal("EvaluatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("EvaluatedAt"))
            });
        }

        return challengeEvaluations;
    }

    public async Task<ChallengeEvaluation?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                EvaluationId,
                ChallengeId,
                CompletionPercentage,
                SuccessLevel,
                EvaluationSummary,
                Recommendations,
                EvaluatedAt
            FROM ChallengeEvaluations
            WHERE EvaluationId = @EvaluationId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@EvaluationId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new ChallengeEvaluation
        {
            EvaluationId = reader.GetGuid(reader.GetOrdinal("EvaluationId")),
            ChallengeId = reader.GetGuid(reader.GetOrdinal("ChallengeId")),
            CompletionPercentage = reader.IsDBNull(reader.GetOrdinal("CompletionPercentage"))
                ? null
                : reader.GetDecimal(reader.GetOrdinal("CompletionPercentage")),
            SuccessLevel = reader.IsDBNull(reader.GetOrdinal("SuccessLevel"))
                ? null
                : reader.GetString(reader.GetOrdinal("SuccessLevel")),
            EvaluationSummary = reader.IsDBNull(reader.GetOrdinal("EvaluationSummary"))
                ? null
                : reader.GetString(reader.GetOrdinal("EvaluationSummary")),
            Recommendations = reader.IsDBNull(reader.GetOrdinal("Recommendations"))
                ? null
                : reader.GetString(reader.GetOrdinal("Recommendations")),
            EvaluatedAt = reader.IsDBNull(reader.GetOrdinal("EvaluatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("EvaluatedAt"))
        };
    }

    public async Task<bool> ExistsByChallengeIdAsync(Guid challengeId)
    {
        const string query = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM ChallengeEvaluations
                    WHERE ChallengeId = @ChallengeId
                )
                THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ChallengeId", SqlDbType.UniqueIdentifier).Value = challengeId;

        return (bool)(await command.ExecuteScalarAsync() ?? false);
    }

    public async Task<ChallengeEvaluation?> AddAsync(ChallengeEvaluation evaluation)
    {
        evaluation.EvaluationId = Guid.NewGuid();
        evaluation.EvaluatedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO ChallengeEvaluations
            (
                EvaluationId,
                ChallengeId,
                CompletionPercentage,
                SuccessLevel,
                EvaluationSummary,
                Recommendations,
                EvaluatedAt
            )
            VALUES
            (
                @EvaluationId,
                @ChallengeId,
                @CompletionPercentage,
                @SuccessLevel,
                @EvaluationSummary,
                @Recommendations,
                @EvaluatedAt
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@EvaluationId", SqlDbType.UniqueIdentifier).Value = evaluation.EvaluationId;
        command.Parameters.Add("@ChallengeId", SqlDbType.UniqueIdentifier).Value = evaluation.ChallengeId;

        var percentageParameter = command.Parameters.Add("@CompletionPercentage", SqlDbType.Decimal);
        percentageParameter.Precision = 5;
        percentageParameter.Scale = 2;
        percentageParameter.Value = (object?)evaluation.CompletionPercentage ?? DBNull.Value;

        command.Parameters.Add("@SuccessLevel", SqlDbType.NVarChar, 50).Value =
            (object?)evaluation.SuccessLevel ?? DBNull.Value;
        command.Parameters.Add("@EvaluationSummary", SqlDbType.NVarChar, -1).Value =
            (object?)evaluation.EvaluationSummary ?? DBNull.Value;
        command.Parameters.Add("@Recommendations", SqlDbType.NVarChar, -1).Value =
            (object?)evaluation.Recommendations ?? DBNull.Value;
        command.Parameters.Add("@EvaluatedAt", SqlDbType.DateTime2).Value =
            (object?)evaluation.EvaluatedAt ?? DBNull.Value;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? evaluation : null;
    }

    public async Task<bool> UpdateAsync(ChallengeEvaluation evaluation)
    {
        const string query = """
            UPDATE ChallengeEvaluations
            SET
                CompletionPercentage = @CompletionPercentage,
                SuccessLevel = @SuccessLevel,
                EvaluationSummary = @EvaluationSummary,
                Recommendations = @Recommendations
            WHERE EvaluationId = @EvaluationId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@EvaluationId", SqlDbType.UniqueIdentifier).Value = evaluation.EvaluationId;

        var percentageParameter = command.Parameters.Add("@CompletionPercentage", SqlDbType.Decimal);
        percentageParameter.Precision = 5;
        percentageParameter.Scale = 2;
        percentageParameter.Value = (object?)evaluation.CompletionPercentage ?? DBNull.Value;

        command.Parameters.Add("@SuccessLevel", SqlDbType.NVarChar, 50).Value =
            (object?)evaluation.SuccessLevel ?? DBNull.Value;
        command.Parameters.Add("@EvaluationSummary", SqlDbType.NVarChar, -1).Value =
            (object?)evaluation.EvaluationSummary ?? DBNull.Value;
        command.Parameters.Add("@Recommendations", SqlDbType.NVarChar, -1).Value =
            (object?)evaluation.Recommendations ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM ChallengeEvaluations
            WHERE EvaluationId = @EvaluationId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@EvaluationId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
