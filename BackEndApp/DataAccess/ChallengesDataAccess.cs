using System.Data;
using BackEndApp.Models.Challenges;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class ChallengesDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public ChallengesDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<Challenge>> GetAllAsync()
    {
        const string query = """
            SELECT
                ChallengeId,
                Title,
                Description,
                CreatedAt,
                AllocationId
            FROM Challenges;
            """;

        var challenges = new List<Challenge>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            challenges.Add(new Challenge
            {
                ChallengeId = reader.GetGuid(reader.GetOrdinal("ChallengeId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                AllocationId = reader.GetGuid(reader.GetOrdinal("AllocationId"))
            });
        }

        return challenges;
    }

    public async Task<Challenge?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                ChallengeId,
                Title,
                Description,
                CreatedAt,
                AllocationId
            FROM Challenges
            WHERE ChallengeId = @ChallengeId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ChallengeId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Challenge
        {
            ChallengeId = reader.GetGuid(reader.GetOrdinal("ChallengeId")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                ? null
                : reader.GetString(reader.GetOrdinal("Description")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            AllocationId = reader.GetGuid(reader.GetOrdinal("AllocationId"))
        };
    }

    public async Task<Challenge?> AddAsync(Challenge challenge)
    {
        challenge.ChallengeId = Guid.NewGuid();
        challenge.CreatedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO Challenges
            (
                ChallengeId,
                Title,
                Description,
                CreatedAt,
                AllocationId
            )
            VALUES
            (
                @ChallengeId,
                @Title,
                @Description,
                @CreatedAt,
                @AllocationId
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ChallengeId", SqlDbType.UniqueIdentifier).Value = challenge.ChallengeId;
        command.Parameters.Add("@Title", SqlDbType.NVarChar, 255).Value = challenge.Title;
        command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value =
            (object?)challenge.Description ?? DBNull.Value;
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value =
            (object?)challenge.CreatedAt ?? DBNull.Value;
        command.Parameters.Add("@AllocationId", SqlDbType.UniqueIdentifier).Value = challenge.AllocationId;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? challenge : null;
    }

    public async Task<bool> UpdateAsync(Challenge challenge)
    {
        const string query = """
            UPDATE Challenges
            SET
                Title = @Title,
                Description = @Description
            WHERE ChallengeId = @ChallengeId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ChallengeId", SqlDbType.UniqueIdentifier).Value = challenge.ChallengeId;
        command.Parameters.Add("@Title", SqlDbType.NVarChar, 255).Value = challenge.Title;
        command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value =
            (object?)challenge.Description ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM Challenges
            WHERE ChallengeId = @ChallengeId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ChallengeId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
