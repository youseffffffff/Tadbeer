using System.Data;
using BackEndApp.Models.Expenses;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class ExpensesDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public ExpensesDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<Expense>> GetAllAsync()
    {
        const string query = """
            SELECT
                ExpenseId,
                CategoryId,
                Amount,
                Description,
                ExpenseDate,
                SourceType,
                IsFlagged,
                CreatedAt,
                UpdatedAt,
                IsDeleted,
                AllocationId
            FROM Expenses
            WHERE ISNULL(IsDeleted, 0) = 0;
            """;

        var expenses = new List<Expense>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            expenses.Add(new Expense
            {
                ExpenseId = reader.GetGuid(reader.GetOrdinal("ExpenseId")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),
                ExpenseDate = reader.GetDateTime(reader.GetOrdinal("ExpenseDate")),
                SourceType = reader.IsDBNull(reader.GetOrdinal("SourceType"))
                    ? null
                    : reader.GetByte(reader.GetOrdinal("SourceType")),
                IsFlagged = reader.IsDBNull(reader.GetOrdinal("IsFlagged"))
                    ? null
                    : reader.GetBoolean(reader.GetOrdinal("IsFlagged")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.IsDBNull(reader.GetOrdinal("IsDeleted"))
                    ? null
                    : reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                AllocationId = reader.GetGuid(reader.GetOrdinal("AllocationId"))
            });
        }

        return expenses;
    }

    public async Task<Expense?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                ExpenseId,
                CategoryId,
                Amount,
                Description,
                ExpenseDate,
                SourceType,
                IsFlagged,
                CreatedAt,
                UpdatedAt,
                IsDeleted,
                AllocationId
            FROM Expenses
            WHERE ExpenseId = @ExpenseId
              AND ISNULL(IsDeleted, 0) = 0;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ExpenseId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Expense
        {
            ExpenseId = reader.GetGuid(reader.GetOrdinal("ExpenseId")),
            CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
            Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                ? null
                : reader.GetString(reader.GetOrdinal("Description")),
            ExpenseDate = reader.GetDateTime(reader.GetOrdinal("ExpenseDate")),
            SourceType = reader.IsDBNull(reader.GetOrdinal("SourceType"))
                ? null
                : reader.GetByte(reader.GetOrdinal("SourceType")),
            IsFlagged = reader.IsDBNull(reader.GetOrdinal("IsFlagged"))
                ? null
                : reader.GetBoolean(reader.GetOrdinal("IsFlagged")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
            IsDeleted = reader.IsDBNull(reader.GetOrdinal("IsDeleted"))
                ? null
                : reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
            AllocationId = reader.GetGuid(reader.GetOrdinal("AllocationId"))
        };
    }

    public async Task<Expense?> AddAsync(Expense expense)
    {
        expense.ExpenseId = Guid.NewGuid();
        expense.CreatedAt = DateTime.UtcNow;
        expense.IsDeleted = false;

        const string query = """
            INSERT INTO Expenses
            (
                ExpenseId,
                CategoryId,
                Amount,
                Description,
                ExpenseDate,
                SourceType,
                IsFlagged,
                CreatedAt,
                UpdatedAt,
                IsDeleted,
                AllocationId
            )
            VALUES
            (
                @ExpenseId,
                @CategoryId,
                @Amount,
                @Description,
                @ExpenseDate,
                @SourceType,
                @IsFlagged,
                @CreatedAt,
                @UpdatedAt,
                @IsDeleted,
                @AllocationId
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ExpenseId", SqlDbType.UniqueIdentifier).Value = expense.ExpenseId;
        command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = expense.CategoryId;

        var amountParameter = command.Parameters.Add("@Amount", SqlDbType.Decimal);
        amountParameter.Precision = 18;
        amountParameter.Scale = 2;
        amountParameter.Value = expense.Amount;

        command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
            (object?)expense.Description ?? DBNull.Value;
        command.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = expense.ExpenseDate;
        command.Parameters.Add("@SourceType", SqlDbType.TinyInt).Value =
            (object?)expense.SourceType ?? DBNull.Value;
        command.Parameters.Add("@IsFlagged", SqlDbType.Bit).Value =
            (object?)expense.IsFlagged ?? DBNull.Value;
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value =
            (object?)expense.CreatedAt ?? DBNull.Value;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value =
            (object?)expense.UpdatedAt ?? DBNull.Value;
        command.Parameters.Add("@IsDeleted", SqlDbType.Bit).Value = expense.IsDeleted;
        command.Parameters.Add("@AllocationId", SqlDbType.UniqueIdentifier).Value = expense.AllocationId;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? expense : null;
    }

    public async Task<bool> UpdateAsync(Expense expense)
    {
        expense.UpdatedAt = DateTime.UtcNow;

        const string query = """
            UPDATE Expenses
            SET
                CategoryId = @CategoryId,
                Amount = @Amount,
                Description = @Description,
                ExpenseDate = @ExpenseDate,
                SourceType = @SourceType,
                IsFlagged = @IsFlagged,
                UpdatedAt = @UpdatedAt,
                AllocationId = @AllocationId
            WHERE ExpenseId = @ExpenseId
              AND ISNULL(IsDeleted, 0) = 0;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ExpenseId", SqlDbType.UniqueIdentifier).Value = expense.ExpenseId;
        command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = expense.CategoryId;

        var amountParameter = command.Parameters.Add("@Amount", SqlDbType.Decimal);
        amountParameter.Precision = 18;
        amountParameter.Scale = 2;
        amountParameter.Value = expense.Amount;

        command.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value =
            (object?)expense.Description ?? DBNull.Value;
        command.Parameters.Add("@ExpenseDate", SqlDbType.Date).Value = expense.ExpenseDate;
        command.Parameters.Add("@SourceType", SqlDbType.TinyInt).Value =
            (object?)expense.SourceType ?? DBNull.Value;
        command.Parameters.Add("@IsFlagged", SqlDbType.Bit).Value =
            (object?)expense.IsFlagged ?? DBNull.Value;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value =
            (object?)expense.UpdatedAt ?? DBNull.Value;
        command.Parameters.Add("@AllocationId", SqlDbType.UniqueIdentifier).Value = expense.AllocationId;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            UPDATE Expenses
            SET
                IsDeleted = 1,
                UpdatedAt = @UpdatedAt
            WHERE ExpenseId = @ExpenseId
              AND ISNULL(IsDeleted, 0) = 0;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@ExpenseId", SqlDbType.UniqueIdentifier).Value = id;
        command.Parameters.Add("@UpdatedAt", SqlDbType.DateTime2).Value = DateTime.UtcNow;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
