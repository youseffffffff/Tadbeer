using System.Data;
using BackEndApp.Models.Statuses;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class StatusesDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public StatusesDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<Status>> GetAllAsync()
    {
        const string query = """
            SELECT
                StatusId,
                StatusName,
                Description
            FROM Statuses;
            """;

        var statuses = new List<Status>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            statuses.Add(new Status
            {
                StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")),
                StatusName = reader.GetString(reader.GetOrdinal("StatusName")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description"))
            });
        }

        return statuses;
    }

    public async Task<Status?> GetByIdAsync(int id)
    {
        const string query = """
            SELECT
                StatusId,
                StatusName,
                Description
            FROM Statuses
            WHERE StatusId = @StatusId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@StatusId", SqlDbType.Int).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Status
        {
            StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")),
            StatusName = reader.GetString(reader.GetOrdinal("StatusName")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                ? null
                : reader.GetString(reader.GetOrdinal("Description"))
        };
    }
}
