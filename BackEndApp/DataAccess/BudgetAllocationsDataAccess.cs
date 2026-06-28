using System.Data;
using BackEndApp.Models.BudgetAllocations;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class BudgetAllocationsDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public BudgetAllocationsDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<BudgetAllocation>> GetAllAsync()
    {
        const string query = """
            SELECT
                AllocationId,
                PeriodId,
                UserId,
                AllocatedAmount,
                CanOverspend,
                ApprovalRequired,
                CreatedAt,
                UpdatedAt
            FROM BudgetAllocations;
            """;

        var budgetAllocations = new List<BudgetAllocation>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            budgetAllocations.Add(new BudgetAllocation
            {
                AllocationId = reader.GetGuid(reader.GetOrdinal("AllocationId")),
                PeriodId = reader.GetGuid(reader.GetOrdinal("PeriodId")),
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                AllocatedAmount = reader.GetDecimal(reader.GetOrdinal("AllocatedAmount")),
                CanOverspend = reader.IsDBNull(reader.GetOrdinal("CanOverspend"))
                    ? null
                    : reader.GetBoolean(reader.GetOrdinal("CanOverspend")),
                ApprovalRequired = reader.IsDBNull(reader.GetOrdinal("ApprovalRequired"))
                    ? null
                    : reader.GetBoolean(reader.GetOrdinal("ApprovalRequired")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            });
        }

        return budgetAllocations;
    }

    public async Task<BudgetAllocation?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                AllocationId,
                PeriodId,
                UserId,
                AllocatedAmount,
                CanOverspend,
                ApprovalRequired,
                CreatedAt,
                UpdatedAt
            FROM BudgetAllocations
            WHERE AllocationId = @AllocationId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@AllocationId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new BudgetAllocation
        {
            AllocationId = reader.GetGuid(reader.GetOrdinal("AllocationId")),
            PeriodId = reader.GetGuid(reader.GetOrdinal("PeriodId")),
            UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
            AllocatedAmount = reader.GetDecimal(reader.GetOrdinal("AllocatedAmount")),
            CanOverspend = reader.IsDBNull(reader.GetOrdinal("CanOverspend"))
                ? null
                : reader.GetBoolean(reader.GetOrdinal("CanOverspend")),
            ApprovalRequired = reader.IsDBNull(reader.GetOrdinal("ApprovalRequired"))
                ? null
                : reader.GetBoolean(reader.GetOrdinal("ApprovalRequired")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
        };
    }

    public async Task<bool> ExistsAsync(Guid periodId, Guid userId)
    {
        const string query = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM BudgetAllocations
                    WHERE PeriodId = @PeriodId
                      AND UserId = @UserId
                )
                THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@PeriodId", SqlDbType.UniqueIdentifier).Value = periodId;
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

        return (bool)(await command.ExecuteScalarAsync() ?? false);
    }

    public async Task<BudgetAllocation?> AddAsync(BudgetAllocation allocation)
    {
        allocation.AllocationId = Guid.NewGuid();
        allocation.CreatedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO BudgetAllocations
            (
                AllocationId,
                PeriodId,
                UserId,
                AllocatedAmount,
                CanOverspend,
                ApprovalRequired,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @AllocationId,
                @PeriodId,
                @UserId,
                @AllocatedAmount,
                @CanOverspend,
                @ApprovalRequired,
                @CreatedAt,
                @UpdatedAt
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@AllocationId", SqlDbType.UniqueIdentifier).Value = allocation.AllocationId;
        command.Parameters.Add("@PeriodId", SqlDbType.UniqueIdentifier).Value = allocation.PeriodId;
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = allocation.UserId;

        var amountParameter = command.Parameters.Add("@AllocatedAmount", SqlDbType.Decimal);
        amountParameter.Precision = 18;
        amountParameter.Scale = 2;
        amountParameter.Value = allocation.AllocatedAmount;

        command.Parameters.Add("@CanOverspend", SqlDbType.Bit).Value =
            (object?)allocation.CanOverspend ?? DBNull.Value;
        command.Parameters.Add("@ApprovalRequired", SqlDbType.Bit).Value =
            (object?)allocation.ApprovalRequired ?? DBNull.Value;
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value =
            (object?)allocation.CreatedAt ?? DBNull.Value;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value =
            (object?)allocation.UpdatedAt ?? DBNull.Value;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? allocation : null;
    }

    public async Task<bool> UpdateAsync(BudgetAllocation allocation)
    {
        allocation.UpdatedAt = DateTime.UtcNow;

        const string query = """
            UPDATE BudgetAllocations
            SET
                PeriodId = @PeriodId,
                UserId = @UserId,
                AllocatedAmount = @AllocatedAmount,
                CanOverspend = @CanOverspend,
                ApprovalRequired = @ApprovalRequired,
                UpdatedAt = @UpdatedAt
            WHERE AllocationId = @AllocationId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@AllocationId", SqlDbType.UniqueIdentifier).Value = allocation.AllocationId;
        command.Parameters.Add("@PeriodId", SqlDbType.UniqueIdentifier).Value = allocation.PeriodId;
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = allocation.UserId;

        var amountParameter = command.Parameters.Add("@AllocatedAmount", SqlDbType.Decimal);
        amountParameter.Precision = 18;
        amountParameter.Scale = 2;
        amountParameter.Value = allocation.AllocatedAmount;

        command.Parameters.Add("@CanOverspend", SqlDbType.Bit).Value =
            (object?)allocation.CanOverspend ?? DBNull.Value;
        command.Parameters.Add("@ApprovalRequired", SqlDbType.Bit).Value =
            (object?)allocation.ApprovalRequired ?? DBNull.Value;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value =
            (object?)allocation.UpdatedAt ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM BudgetAllocations
            WHERE AllocationId = @AllocationId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@AllocationId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
