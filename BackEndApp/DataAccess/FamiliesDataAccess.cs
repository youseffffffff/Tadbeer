using System.Data;
using BackEndApp.Models.Families;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class FamiliesDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public FamiliesDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<Family>> GetAllAsync()
    {
        const string query = """
            SELECT
                FamilyId,
                FamilyName,
                CreatedByUserId,
                CreatedAt,
                UpdatedAt,
                IsActive
            FROM Families;
            """;

        var families = new List<Family>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            families.Add(new Family
            {
                FamilyId = reader.GetGuid(reader.GetOrdinal("FamilyId")),
                FamilyName = reader.GetString(reader.GetOrdinal("FamilyName")),
                CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("CreatedByUserId"))
                    ? null
                    : reader.GetGuid(reader.GetOrdinal("CreatedByUserId")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive"))
                    ? null
                    : reader.GetBoolean(reader.GetOrdinal("IsActive"))
            });
        }

        return families;
    }

    public async Task<Family?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                FamilyId,
                FamilyName,
                CreatedByUserId,
                CreatedAt,
                UpdatedAt,
                IsActive
            FROM Families
            WHERE FamilyId = @FamilyId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@FamilyId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Family
        {
            FamilyId = reader.GetGuid(reader.GetOrdinal("FamilyId")),
            FamilyName = reader.GetString(reader.GetOrdinal("FamilyName")),
            CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("CreatedByUserId"))
                ? null
                : reader.GetGuid(reader.GetOrdinal("CreatedByUserId")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
            IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive"))
                ? null
                : reader.GetBoolean(reader.GetOrdinal("IsActive"))
        };
    }

    public async Task<Family?> AddAsync(Family family)
    {
        family.FamilyId = Guid.NewGuid();
        family.CreatedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO Families
            (
                FamilyId,
                FamilyName,
                CreatedByUserId,
                CreatedAt,
                UpdatedAt,
                IsActive
            )
            VALUES
            (
                @FamilyId,
                @FamilyName,
                @CreatedByUserId,
                @CreatedAt,
                @UpdatedAt,
                @IsActive
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@FamilyId", SqlDbType.UniqueIdentifier).Value = family.FamilyId;
        command.Parameters.Add("@FamilyName", SqlDbType.NVarChar, 100).Value = family.FamilyName;
        command.Parameters.Add("@CreatedByUserId", SqlDbType.UniqueIdentifier).Value =
            (object?)family.CreatedByUserId ?? DBNull.Value;
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value =
            (object?)family.CreatedAt ?? DBNull.Value;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value =
            (object?)family.UpdatedAt ?? DBNull.Value;
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
            (object?)family.IsActive ?? DBNull.Value;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? family : null;
    }

    public async Task<bool> UpdateAsync(Family family)
    {
        family.UpdatedAt = DateTime.UtcNow;

        const string query = """
            UPDATE Families
            SET
                FamilyName = @FamilyName,
                CreatedByUserId = @CreatedByUserId,
                UpdatedAt = @UpdatedAt,
                IsActive = @IsActive
            WHERE FamilyId = @FamilyId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@FamilyId", SqlDbType.UniqueIdentifier).Value = family.FamilyId;
        command.Parameters.Add("@FamilyName", SqlDbType.NVarChar, 100).Value = family.FamilyName;
        command.Parameters.Add("@CreatedByUserId", SqlDbType.UniqueIdentifier).Value =
            (object?)family.CreatedByUserId ?? DBNull.Value;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value =
            (object?)family.UpdatedAt ?? DBNull.Value;
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
            (object?)family.IsActive ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM Families
            WHERE FamilyId = @FamilyId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@FamilyId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
