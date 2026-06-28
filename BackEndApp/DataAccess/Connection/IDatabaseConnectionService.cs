using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public interface IDatabaseConnectionService
{
    SqlConnection CreateConnection();
}
