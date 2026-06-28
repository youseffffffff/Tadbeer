using System.Data;
using BackEndApp.Models.ExpenseCategories;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class ExpenseCategoriesDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public ExpenseCategoriesDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<ExpenseCategory>> GetAllAsync()
    {
        const string query = """
            SELECT
                CategoryId,
                CategoryName,
                IconName,
                ColorHex,
                CreatedAt
            FROM ExpenseCategories;
            """;

        var expenseCategories = new List<ExpenseCategory>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            expenseCategories.Add(new ExpenseCategory
            {
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                IconName = reader.IsDBNull(reader.GetOrdinal("IconName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("IconName")),
                ColorHex = reader.IsDBNull(reader.GetOrdinal("ColorHex"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ColorHex")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }

        return expenseCategories;
    }

    public async Task<ExpenseCategory?> GetByIdAsync(int id)
    {
        const string query = """
            SELECT
                CategoryId,
                CategoryName,
                IconName,
                ColorHex,
                CreatedAt
            FROM ExpenseCategories
            WHERE CategoryId = @CategoryId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new ExpenseCategory
        {
            CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
            CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
            IconName = reader.IsDBNull(reader.GetOrdinal("IconName"))
                ? null
                : reader.GetString(reader.GetOrdinal("IconName")),
            ColorHex = reader.IsDBNull(reader.GetOrdinal("ColorHex"))
                ? null
                : reader.GetString(reader.GetOrdinal("ColorHex")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }

    public async Task<bool> ExistsByNameAsync(string categoryName, int? excludedCategoryId = null)
    {
        const string query = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM ExpenseCategories
                    WHERE LOWER(LTRIM(RTRIM(CategoryName))) = LOWER(@CategoryName)
                      AND (@ExcludedCategoryId IS NULL OR CategoryId <> @ExcludedCategoryId)
                )
                THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = categoryName;
        command.Parameters.Add("@ExcludedCategoryId", SqlDbType.Int).Value =
            (object?)excludedCategoryId ?? DBNull.Value;

        return (bool)(await command.ExecuteScalarAsync() ?? false);
    }

    public async Task<ExpenseCategory?> AddAsync(ExpenseCategory category)
    {
        category.CreatedAt = DateTime.UtcNow;

        const string query = """
            INSERT INTO ExpenseCategories
            (
                CategoryName,
                IconName,
                ColorHex,
                CreatedAt
            )
            VALUES
            (
                @CategoryName,
                @IconName,
                @ColorHex,
                @CreatedAt
            );

            SELECT CAST(SCOPE_IDENTITY() AS int);
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = category.CategoryName;
        command.Parameters.Add("@IconName", SqlDbType.NVarChar, 100).Value =
            (object?)category.IconName ?? DBNull.Value;
        command.Parameters.Add("@ColorHex", SqlDbType.NVarChar, 20).Value =
            (object?)category.ColorHex ?? DBNull.Value;
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value =
            (object?)category.CreatedAt ?? DBNull.Value;

        var generatedId = await command.ExecuteScalarAsync();

        if (generatedId is null || generatedId is DBNull)
        {
            return null;
        }

        category.CategoryId = Convert.ToInt32(generatedId);

        return category;
    }

    public async Task<bool> UpdateAsync(ExpenseCategory category)
    {
        const string query = """
            UPDATE ExpenseCategories
            SET
                CategoryName = @CategoryName,
                IconName = @IconName,
                ColorHex = @ColorHex
            WHERE CategoryId = @CategoryId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = category.CategoryId;
        command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = category.CategoryName;
        command.Parameters.Add("@IconName", SqlDbType.NVarChar, 100).Value =
            (object?)category.IconName ?? DBNull.Value;
        command.Parameters.Add("@ColorHex", SqlDbType.NVarChar, 20).Value =
            (object?)category.ColorHex ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = """
            DELETE FROM ExpenseCategories
            WHERE CategoryId = @CategoryId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
