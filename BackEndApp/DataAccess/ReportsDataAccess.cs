using System.Data;
using BackEndApp.Models.Reports;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class ReportsDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public ReportsDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<Report>> GetAllAsync()
    {
        const string query = """
            SELECT
                ReportId,
                ReportDataJson,
                GeneratedAt,
                AllocationId
            FROM Reports;
            """;

        var reports = new List<Report>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            reports.Add(new Report
            {
                ReportId = reader.GetGuid(reader.GetOrdinal("ReportId")),
                ReportDataJson = reader.IsDBNull(reader.GetOrdinal("ReportDataJson"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ReportDataJson")),
                GeneratedAt = reader.IsDBNull(reader.GetOrdinal("GeneratedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("GeneratedAt")),
                AllocationId = reader.GetGuid(reader.GetOrdinal("AllocationId"))
            });
        }

        return reports;
    }

    public async Task<Report?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                ReportId,
                ReportDataJson,
                GeneratedAt,
                AllocationId
            FROM Reports
            WHERE ReportId = @ReportId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ReportId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Report
        {
            ReportId = reader.GetGuid(reader.GetOrdinal("ReportId")),
            ReportDataJson = reader.IsDBNull(reader.GetOrdinal("ReportDataJson"))
                ? null
                : reader.GetString(reader.GetOrdinal("ReportDataJson")),
            GeneratedAt = reader.IsDBNull(reader.GetOrdinal("GeneratedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("GeneratedAt")),
            AllocationId = reader.GetGuid(reader.GetOrdinal("AllocationId"))
        };
    }

    public async Task<bool> ExistsByAllocationIdAsync(Guid allocationId)
    {
        const string query = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM Reports
                    WHERE AllocationId = @AllocationId
                )
                THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@AllocationId", SqlDbType.UniqueIdentifier).Value = allocationId;

        return (bool)(await command.ExecuteScalarAsync() ?? false);
    }

    public async Task<Report?> AddAsync(Report report)
    {
        report.ReportId = Guid.NewGuid();
        report.GeneratedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO Reports
            (
                ReportId,
                ReportDataJson,
                GeneratedAt,
                AllocationId
            )
            VALUES
            (
                @ReportId,
                @ReportDataJson,
                @GeneratedAt,
                @AllocationId
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ReportId", SqlDbType.UniqueIdentifier).Value = report.ReportId;
        command.Parameters.Add("@ReportDataJson", SqlDbType.NVarChar, -1).Value =
            (object?)report.ReportDataJson ?? DBNull.Value;
        command.Parameters.Add("@GeneratedAt", SqlDbType.DateTime2).Value =
            (object?)report.GeneratedAt ?? DBNull.Value;
        command.Parameters.Add("@AllocationId", SqlDbType.UniqueIdentifier).Value = report.AllocationId;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? report : null;
    }

    public async Task<bool> UpdateAsync(Report report)
    {
        const string query = """
            UPDATE Reports
            SET ReportDataJson = @ReportDataJson
            WHERE ReportId = @ReportId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ReportId", SqlDbType.UniqueIdentifier).Value = report.ReportId;
        command.Parameters.Add("@ReportDataJson", SqlDbType.NVarChar, -1).Value =
            (object?)report.ReportDataJson ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM Reports
            WHERE ReportId = @ReportId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ReportId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
