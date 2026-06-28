using System.Data;
using BackEndApp.Models.FamilyBudgets;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class FamilyBudgetsDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public FamilyBudgetsDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<FamilyBudget>> GetAllAsync()
    {
        const string query = """
            SELECT
                BudgetId,
                FamilyId,
                BudgetCycleType,
                IsActive,
                CreatedAt,
                UpdatedAt
            FROM FamilyBudgets;
            """;

        var familyBudgets = new List<FamilyBudget>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            familyBudgets.Add(new FamilyBudget
            {
                BudgetId = reader.GetGuid(reader.GetOrdinal("BudgetId")),
                FamilyId = reader.GetGuid(reader.GetOrdinal("FamilyId")),
                BudgetCycleType = reader.GetByte(reader.GetOrdinal("BudgetCycleType")),
                IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive"))
                    ? null
                    : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            });
        }

        return familyBudgets;
    }

    public async Task<FamilyBudget?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                BudgetId,
                FamilyId,
                BudgetCycleType,
                IsActive,
                CreatedAt,
                UpdatedAt
            FROM FamilyBudgets
            WHERE BudgetId = @BudgetId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@BudgetId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new FamilyBudget
        {
            BudgetId = reader.GetGuid(reader.GetOrdinal("BudgetId")),
            FamilyId = reader.GetGuid(reader.GetOrdinal("FamilyId")),
            BudgetCycleType = reader.GetByte(reader.GetOrdinal("BudgetCycleType")),
            IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive"))
                ? null
                : reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
        };
    }

    public async Task<bool> ExistsByFamilyIdAsync(Guid familyId)
    {
        const string query = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM FamilyBudgets
                    WHERE FamilyId = @FamilyId
                )
                THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@FamilyId", SqlDbType.UniqueIdentifier).Value = familyId;

        return (bool)(await command.ExecuteScalarAsync() ?? false);
    }

    public async Task<FamilyBudget?> AddAsync(FamilyBudget familyBudget)
    {
        familyBudget.BudgetId = Guid.NewGuid();
        familyBudget.CreatedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO FamilyBudgets
            (
                BudgetId,
                FamilyId,
                BudgetCycleType,
                IsActive,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @BudgetId,
                @FamilyId,
                @BudgetCycleType,
                @IsActive,
                @CreatedAt,
                @UpdatedAt
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@BudgetId", SqlDbType.UniqueIdentifier).Value = familyBudget.BudgetId;
        command.Parameters.Add("@FamilyId", SqlDbType.UniqueIdentifier).Value = familyBudget.FamilyId;
        command.Parameters.Add("@BudgetCycleType", SqlDbType.TinyInt).Value = familyBudget.BudgetCycleType;
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
            (object?)familyBudget.IsActive ?? DBNull.Value;
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value =
            (object?)familyBudget.CreatedAt ?? DBNull.Value;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value =
            (object?)familyBudget.UpdatedAt ?? DBNull.Value;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? familyBudget : null;
    }

    public async Task<bool> UpdateAsync(FamilyBudget familyBudget)
    {
        familyBudget.UpdatedAt = DateTime.UtcNow;

        const string query = """
            UPDATE FamilyBudgets
            SET
                FamilyId = @FamilyId,
                BudgetCycleType = @BudgetCycleType,
                IsActive = @IsActive,
                UpdatedAt = @UpdatedAt
            WHERE BudgetId = @BudgetId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@BudgetId", SqlDbType.UniqueIdentifier).Value = familyBudget.BudgetId;
        command.Parameters.Add("@FamilyId", SqlDbType.UniqueIdentifier).Value = familyBudget.FamilyId;
        command.Parameters.Add("@BudgetCycleType", SqlDbType.TinyInt).Value = familyBudget.BudgetCycleType;
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value =
            (object?)familyBudget.IsActive ?? DBNull.Value;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value =
            (object?)familyBudget.UpdatedAt ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM FamilyBudgets
            WHERE BudgetId = @BudgetId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@BudgetId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
