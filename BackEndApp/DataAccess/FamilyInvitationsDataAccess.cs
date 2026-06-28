using System.Data;
using System.Security.Cryptography;
using BackEndApp.Models.FamilyInvitations;
using Microsoft.Data.SqlClient;

namespace BackEndApp.DataAccess;

public class FamilyInvitationsDataAccess
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public FamilyInvitationsDataAccess(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    public async Task<List<FamilyInvitation>> GetAllAsync()
    {
        const string query = """
            SELECT
                InvitationId,
                FamilyId,
                InvitedByUserId,
                EmailOrPhone,
                InvitationToken,
                ExpiresAt,
                CreatedAt,
                InvitationStatusId
            FROM FamilyInvitations;
            """;

        var familyInvitations = new List<FamilyInvitation>();

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            familyInvitations.Add(new FamilyInvitation
            {
                InvitationId = reader.GetGuid(reader.GetOrdinal("InvitationId")),
                FamilyId = reader.GetGuid(reader.GetOrdinal("FamilyId")),
                InvitedByUserId = reader.GetGuid(reader.GetOrdinal("InvitedByUserId")),
                EmailOrPhone = reader.GetString(reader.GetOrdinal("EmailOrPhone")),
                InvitationToken = reader.GetString(reader.GetOrdinal("InvitationToken")),
                ExpiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                InvitationStatusId = reader.GetInt32(reader.GetOrdinal("InvitationStatusId"))
            });
        }

        return familyInvitations;
    }

    public async Task<FamilyInvitation?> GetByIdAsync(Guid id)
    {
        const string query = """
            SELECT
                InvitationId,
                FamilyId,
                InvitedByUserId,
                EmailOrPhone,
                InvitationToken,
                ExpiresAt,
                CreatedAt,
                InvitationStatusId
            FROM FamilyInvitations
            WHERE InvitationId = @InvitationId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@InvitationId", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new FamilyInvitation
        {
            InvitationId = reader.GetGuid(reader.GetOrdinal("InvitationId")),
            FamilyId = reader.GetGuid(reader.GetOrdinal("FamilyId")),
            InvitedByUserId = reader.GetGuid(reader.GetOrdinal("InvitedByUserId")),
            EmailOrPhone = reader.GetString(reader.GetOrdinal("EmailOrPhone")),
            InvitationToken = reader.GetString(reader.GetOrdinal("InvitationToken")),
            ExpiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            InvitationStatusId = reader.GetInt32(reader.GetOrdinal("InvitationStatusId"))
        };
    }

    public async Task<bool> StatusExistsAsync(int statusId)
    {
        const string query = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM Statuses
                    WHERE StatusId = @StatusId
                )
                THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@StatusId", SqlDbType.Int).Value = statusId;

        return (bool)(await command.ExecuteScalarAsync() ?? false);
    }

    public async Task<bool> TokenExistsAsync(string invitationToken)
    {
        const string query = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM FamilyInvitations
                    WHERE InvitationToken = @InvitationToken
                )
                THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@InvitationToken", SqlDbType.NVarChar, 255).Value = invitationToken;

        return (bool)(await command.ExecuteScalarAsync() ?? false);
    }

    public async Task<FamilyInvitation?> AddAsync(FamilyInvitation invitation)
    {
        invitation.InvitationId = Guid.NewGuid();
        invitation.CreatedAt = DateTime.UtcNow;

        do
        {
            invitation.InvitationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        }
        while (await TokenExistsAsync(invitation.InvitationToken));

        const string query = """
            INSERT INTO FamilyInvitations
            (
                InvitationId,
                FamilyId,
                InvitedByUserId,
                EmailOrPhone,
                InvitationToken,
                ExpiresAt,
                CreatedAt,
                InvitationStatusId
            )
            VALUES
            (
                @InvitationId,
                @FamilyId,
                @InvitedByUserId,
                @EmailOrPhone,
                @InvitationToken,
                @ExpiresAt,
                @CreatedAt,
                @InvitationStatusId
            );
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@InvitationId", SqlDbType.UniqueIdentifier).Value = invitation.InvitationId;
        command.Parameters.Add("@FamilyId", SqlDbType.UniqueIdentifier).Value = invitation.FamilyId;
        command.Parameters.Add("@InvitedByUserId", SqlDbType.UniqueIdentifier).Value =
            invitation.InvitedByUserId;
        command.Parameters.Add("@EmailOrPhone", SqlDbType.NVarChar, 255).Value =
            invitation.EmailOrPhone;
        command.Parameters.Add("@InvitationToken", SqlDbType.NVarChar, 255).Value =
            invitation.InvitationToken;
        command.Parameters.Add("@ExpiresAt", SqlDbType.DateTime2).Value = invitation.ExpiresAt;
        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value =
            (object?)invitation.CreatedAt ?? DBNull.Value;
        command.Parameters.Add("@InvitationStatusId", SqlDbType.Int).Value =
            invitation.InvitationStatusId;

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0 ? invitation : null;
    }

    public async Task<bool> UpdateAsync(FamilyInvitation invitation)
    {
        const string query = """
            UPDATE FamilyInvitations
            SET
                EmailOrPhone = @EmailOrPhone,
                ExpiresAt = @ExpiresAt,
                InvitationStatusId = @InvitationStatusId
            WHERE InvitationId = @InvitationId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@InvitationId", SqlDbType.UniqueIdentifier).Value = invitation.InvitationId;
        command.Parameters.Add("@EmailOrPhone", SqlDbType.NVarChar, 255).Value =
            invitation.EmailOrPhone;
        command.Parameters.Add("@ExpiresAt", SqlDbType.DateTime2).Value = invitation.ExpiresAt;
        command.Parameters.Add("@InvitationStatusId", SqlDbType.Int).Value =
            invitation.InvitationStatusId;

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = """
            DELETE FROM FamilyInvitations
            WHERE InvitationId = @InvitationId;
            """;

        await using var connection = _databaseConnectionService.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@InvitationId", SqlDbType.UniqueIdentifier).Value = id;

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
