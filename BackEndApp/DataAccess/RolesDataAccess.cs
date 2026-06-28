using System.Data;
using BackEndApp.Models.Roles;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class RolesDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public RolesDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<Role>> GetAllAsync()
    {
        const string query = """
            SELECT
                RoleId,
                RoleName,
                Description
            FROM Roles;
            """;

        var roles = new List<Role>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            roles.Add(new Role
            {
                RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description"))
            });
        }

        return roles;
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        const string query = """
            SELECT
                RoleId,
                RoleName,
                Description
            FROM Roles
            WHERE RoleId = @RoleId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@RoleId", SqlDbType.Int).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Role
        {
            RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
            RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                ? null
                : reader.GetString(reader.GetOrdinal("Description"))
        };
    }
}
