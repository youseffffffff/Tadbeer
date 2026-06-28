using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public sealed class DatabaseConnectionService : IDatabaseConnectionService
{
    private const string ConnectionStringName = "DefaultConnection";

    private readonly string _connectionString;

    public DatabaseConnectionService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' was not found.");
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
