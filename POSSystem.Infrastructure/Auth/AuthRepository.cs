using Microsoft.Data.SqlClient;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Data;
using System.Data;

namespace POSSystem.Repository.Auth;

public class AuthRepository : IAuthRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public AuthRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    // =============================================
    // SIGNUP
    // =============================================
    public async Task<(bool Success, string Message, int? UserId)> SignupUserAsync(
        string username,
        string email,
        string passwordHash,
        string fullName,
        string roleName)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("uspAuth_Signup", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@PasswordHash", passwordHash);
        command.Parameters.AddWithValue("@FullName", fullName);
        command.Parameters.AddWithValue("@RoleName", roleName);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return (false, "Signup failed", null);

        var success = reader.GetBoolean(reader.GetOrdinal("Success"));
        var message = reader.IsDBNull(reader.GetOrdinal("Message"))
            ? string.Empty
            : reader.GetString(reader.GetOrdinal("Message"));

        int? newUserId = reader.IsDBNull(reader.GetOrdinal("UserId"))
            ? null
            : reader.GetInt32(reader.GetOrdinal("UserId"));

        return (success, message, newUserId);
    }

    // =============================================
    // GET USER BY USERNAME (for login)
    // =============================================
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("uspAuth_GetUserByUsername", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Username", username);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapUser(reader, includePassword: true);
    }

    // =============================================
    // CREATE SESSION
    // =============================================
    public async Task<bool> CreateSessionAsync(int userId, string token, DateTime expiresAt)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("uspAuth_CreateSession", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@Token", token);
        command.Parameters.AddWithValue("@ExpiresAt", expiresAt);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return Convert.ToBoolean(result);
    }

    // =============================================
    // VALIDATE SESSION
    // =============================================
    public async Task<User?> ValidateSessionAsync(string token)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("uspAuth_ValidateSession", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Token", token);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapUser(reader, includePassword: false);
    }

    // =============================================
    // DELETE SESSION
    // =============================================
    public async Task<bool> DeleteSessionAsync(string token)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("uspAuth_DeleteSession", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Token", token);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        return true;
    }

    // =============================================
    // GET USER COUNT
    // =============================================
    public async Task<int> GetUserCountAsync()
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("uspAuth_GetUserCount", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return result == null || result == DBNull.Value
            ? 0
            : Convert.ToInt32(result);
    }

    // =============================================
    // SHARED MAPPER
    // =============================================
    private static User MapUser(SqlDataReader reader, bool includePassword)
    {
        var user = new User
        {
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            Username = reader.IsDBNull(reader.GetOrdinal("Username"))
                ? string.Empty
                : reader.GetString(reader.GetOrdinal("Username")),
            Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                ? string.Empty
                : reader.GetString(reader.GetOrdinal("Email")),
            FullName = reader.IsDBNull(reader.GetOrdinal("FullName"))
                ? string.Empty
                : reader.GetString(reader.GetOrdinal("FullName")),
            IsActive = !reader.IsDBNull(reader.GetOrdinal("IsActive"))
                && reader.GetBoolean(reader.GetOrdinal("IsActive"))
        };

        // CreationDate — only present in GetUserByUsernameAsync result set
        if (HasColumn(reader, "CreationDate") && !reader.IsDBNull(reader.GetOrdinal("CreationDate")))
        {
            user.CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"));
        }

        // UpdatedDate — optional
        if (HasColumn(reader, "UpdatedDate") && !reader.IsDBNull(reader.GetOrdinal("UpdatedDate")))
        {
            user.UpdatedDate = reader.GetDateTime(reader.GetOrdinal("UpdatedDate"));
        }

        // Role — comes from join
        if (HasColumn(reader, "RoleName") && !reader.IsDBNull(reader.GetOrdinal("RoleName")))
        {
            user.Role = reader.GetString(reader.GetOrdinal("RoleName"));
        }

        // PasswordHash — only for login flow
        if (includePassword
            && HasColumn(reader, "PasswordHash")
            && !reader.IsDBNull(reader.GetOrdinal("PasswordHash")))
        {
            user.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
        }

        return user;
    }

    private static bool HasColumn(SqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}