using System.Data;
using BackEndApp.Models.BudgetPeriods;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class BudgetPeriodsDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public BudgetPeriodsDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<BudgetPeriod>> GetAllAsync()
    {
        const string query = """
            SELECT
                PeriodId,
                BudgetId,
                PeriodStart,
                PeriodEnd,
                ActualBudgetAmount
            FROM BudgetPeriods;
            """;

        var budgetPeriods = new List<BudgetPeriod>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            budgetPeriods.Add(new BudgetPeriod
            {
                PeriodId = reader.GetGuid(reader.GetOrdinal("PeriodId")),
                BudgetId = reader.GetGuid(reader.GetOrdinal("BudgetId")),
                PeriodStart = reader.GetDateTime(reader.GetOrdinal("PeriodStart")),
                PeriodEnd = reader.GetDateTime(reader.GetOrdinal("PeriodEnd")),
                ActualBudgetAmount = reader.GetDecimal(reader.GetOrdinal("ActualBudgetAmount"))
            });
        }

        return budgetPeriods;
    }

    public async Task<BudgetPeriod?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                PeriodId,
                BudgetId,
                PeriodStart,
                PeriodEnd,
                ActualBudgetAmount
            FROM BudgetPeriods
            WHERE PeriodId = @PeriodId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@PeriodId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new BudgetPeriod
        {
            PeriodId = reader.GetGuid(reader.GetOrdinal("PeriodId")),
            BudgetId = reader.GetGuid(reader.GetOrdinal("BudgetId")),
            PeriodStart = reader.GetDateTime(reader.GetOrdinal("PeriodStart")),
            PeriodEnd = reader.GetDateTime(reader.GetOrdinal("PeriodEnd")),
            ActualBudgetAmount = reader.GetDecimal(reader.GetOrdinal("ActualBudgetAmount"))
        };
    }

    public async Task<bool> HasOverlappingPeriodAsync(
        Guid budgetId,
        DateTime periodStart,
        DateTime periodEnd)
    {
        const string query = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM BudgetPeriods
                    WHERE BudgetId = @BudgetId
                      AND PeriodStart <= @PeriodEnd
                      AND PeriodEnd >= @PeriodStart
                )
                THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@BudgetId", SqlDbType.UniqueIdentifier).Value = budgetId;
        command.Parameters.Add("@PeriodStart", SqlDbType.Date).Value = periodStart;
        command.Parameters.Add("@PeriodEnd", SqlDbType.Date).Value = periodEnd;

        return (bool)(await command.ExecuteScalarAsync() ?? false);
    }

    public async Task<BudgetPeriod?> AddAsync(BudgetPeriod period)
    {
        period.PeriodId = Guid.NewGuid();

        const string query = """
            INSERT INTO BudgetPeriods
            (
                PeriodId,
                BudgetId,
                PeriodStart,
                PeriodEnd,
                ActualBudgetAmount
            )
            VALUES
            (
                @PeriodId,
                @BudgetId,
                @PeriodStart,
                @PeriodEnd,
                @ActualBudgetAmount
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@PeriodId", SqlDbType.UniqueIdentifier).Value = period.PeriodId;
        command.Parameters.Add("@BudgetId", SqlDbType.UniqueIdentifier).Value = period.BudgetId;
        command.Parameters.Add("@PeriodStart", SqlDbType.Date).Value = period.PeriodStart;
        command.Parameters.Add("@PeriodEnd", SqlDbType.Date).Value = period.PeriodEnd;

        var amountParameter = command.Parameters.Add("@ActualBudgetAmount", SqlDbType.Decimal);
        amountParameter.Precision = 18;
        amountParameter.Scale = 2;
        amountParameter.Value = period.ActualBudgetAmount;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? period : null;
    }

    public async Task<bool> UpdateAsync(BudgetPeriod period)
    {
        const string query = """
            UPDATE BudgetPeriods
            SET
                BudgetId = @BudgetId,
                PeriodStart = @PeriodStart,
                PeriodEnd = @PeriodEnd,
                ActualBudgetAmount = @ActualBudgetAmount
            WHERE PeriodId = @PeriodId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@PeriodId", SqlDbType.UniqueIdentifier).Value = period.PeriodId;
        command.Parameters.Add("@BudgetId", SqlDbType.UniqueIdentifier).Value = period.BudgetId;
        command.Parameters.Add("@PeriodStart", SqlDbType.Date).Value = period.PeriodStart;
        command.Parameters.Add("@PeriodEnd", SqlDbType.Date).Value = period.PeriodEnd;

        var amountParameter = command.Parameters.Add("@ActualBudgetAmount", SqlDbType.Decimal);
        amountParameter.Precision = 18;
        amountParameter.Scale = 2;
        amountParameter.Value = period.ActualBudgetAmount;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM BudgetPeriods
            WHERE PeriodId = @PeriodId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@PeriodId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
