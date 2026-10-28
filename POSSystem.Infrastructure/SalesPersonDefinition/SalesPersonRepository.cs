// POSSystem.Repository.SalesPersonDefinition/SalesPersonRepository.cs

using Microsoft.Data.SqlClient;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Repository.SalesPersonDefinition;

public class SalesPersonRepository : ISalesPersonRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public SalesPersonRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    // =========================
    // GET ALL (Only not deleted - DeletedAt IS NULL)
    // =========================
    public async Task<IEnumerable<SalesPerson>> GetAllAsync()
    {
        var salesPersons = new List<SalesPerson>();

        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = """
            SELECT
                SalesPersonId,
                Code,
                Name,
                Phone,
                Email,
                CommissionPercentage,
                IsActive,
                DeletedAt,
                CreationDate,
                UpdatedDate
            FROM SalesPersons
            WHERE DeletedAt IS NULL
            ORDER BY SalesPersonId DESC
            """;

        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            salesPersons.Add(new SalesPerson
            {
                SalesPersonId = reader.GetInt32(reader.GetOrdinal("SalesPersonId")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                CommissionPercentage = reader.GetDecimal(reader.GetOrdinal("CommissionPercentage")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                DeletedAt = reader.IsDBNull(reader.GetOrdinal("DeletedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("DeletedAt")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
            });
        }

        return salesPersons;
    }

    // =========================
    // GET BY ID (Includes deleted for checking)
    // =========================
    public async Task<SalesPerson?> GetByIdAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = """
            SELECT
                SalesPersonId,
                Code,
                Name,
                Phone,
                Email,
                CommissionPercentage,
                IsActive,
                DeletedAt,
                CreationDate,
                UpdatedDate
            FROM SalesPersons
            WHERE SalesPersonId = @SalesPersonId
            """;

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@SalesPersonId", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new SalesPerson
            {
                SalesPersonId = reader.GetInt32(reader.GetOrdinal("SalesPersonId")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                CommissionPercentage = reader.GetDecimal(reader.GetOrdinal("CommissionPercentage")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                DeletedAt = reader.IsDBNull(reader.GetOrdinal("DeletedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("DeletedAt")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
            };
        }

        return null;
    }

    // =========================
    // CREATE
    // =========================
    public async Task<int> CreateAsync(SalesPerson salesPerson)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = """
            INSERT INTO SalesPersons
            (
                Code,
                Name,
                Phone,
                Email,
                CommissionPercentage,
                IsActive,
                CreationDate
            )
            VALUES
            (
                @Code,
                @Name,
                @Phone,
                @Email,
                @CommissionPercentage,
                @IsActive,
                @CreationDate
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Code", salesPerson.Code);
        command.Parameters.AddWithValue("@Name", salesPerson.Name);
        command.Parameters.AddWithValue("@Phone", (object?)salesPerson.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue("@Email", (object?)salesPerson.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("@CommissionPercentage", salesPerson.CommissionPercentage);
        command.Parameters.AddWithValue("@IsActive", salesPerson.IsActive);
        command.Parameters.AddWithValue("@CreationDate", salesPerson.CreationDate);

        var salesPersonId = await command.ExecuteScalarAsync();
        return Convert.ToInt32(salesPersonId);
    }

    // =========================
    // UPDATE
    // =========================
    public async Task<bool> UpdateAsync(SalesPerson salesPerson)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = """
            UPDATE SalesPersons
            SET
                Code = @Code,
                Name = @Name,
                Phone = @Phone,
                Email = @Email,
                CommissionPercentage = @CommissionPercentage,
                IsActive = @IsActive,
                UpdatedDate = @UpdatedDate
            WHERE SalesPersonId = @SalesPersonId
            AND DeletedAt IS NULL
            """;

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@SalesPersonId", salesPerson.SalesPersonId);
        command.Parameters.AddWithValue("@Code", salesPerson.Code);
        command.Parameters.AddWithValue("@Name", salesPerson.Name);
        command.Parameters.AddWithValue("@Phone", (object?)salesPerson.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue("@Email", (object?)salesPerson.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("@CommissionPercentage", salesPerson.CommissionPercentage);
        command.Parameters.AddWithValue("@IsActive", salesPerson.IsActive);
        command.Parameters.AddWithValue("@UpdatedDate", DateTime.UtcNow);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    // =========================
    // HAS RELATED RECORDS (Check if used in Sales)
    // =========================
    public async Task<bool> HasRelatedRecords(int salesPersonId)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = """
            SELECT 
                CASE 
                    WHEN EXISTS (SELECT 1 FROM SalesMaster WHERE SalesPersonId = @SalesPersonId)
                    THEN 1 
                    ELSE 0 
                END AS HasRelatedRecords
            """;

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@SalesPersonId", salesPersonId);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToBoolean(result);
    }

    // =========================
    // SOFT DELETE (Set DeletedAt)
    // =========================
    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = """
            UPDATE SalesPersons
            SET 
                DeletedAt = @DeletedAt,
                IsActive = 0,
                UpdatedDate = @UpdatedDate
            WHERE SalesPersonId = @SalesPersonId
            AND DeletedAt IS NULL
            """;

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@SalesPersonId", id);
        command.Parameters.AddWithValue("@DeletedAt", DateTime.UtcNow);
        command.Parameters.AddWithValue("@UpdatedDate", DateTime.UtcNow);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    // =========================
    // RESTORE (Clear DeletedAt)
    // =========================
    public async Task<bool> RestoreAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = """
            UPDATE SalesPersons
            SET 
                DeletedAt = NULL,
                IsActive = 1,
                UpdatedDate = @UpdatedDate
            WHERE SalesPersonId = @SalesPersonId
            AND DeletedAt IS NOT NULL
            """;

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@SalesPersonId", id);
        command.Parameters.AddWithValue("@UpdatedDate", DateTime.UtcNow);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    // =========================
    // DELETE (Smart delete)
    // =========================
    public async Task<bool> DeleteAsync(int id)
    {
        // ✅ Check if sales person has related records
        var hasRelated = await HasRelatedRecords(id);

        if (hasRelated)
        {
            // ✅ In use - Soft Delete (set DeletedAt)
            return await SoftDeleteAsync(id);
        }

        // ✅ No related records - Hard Delete
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = """
            DELETE FROM SalesPersons
            WHERE SalesPersonId = @SalesPersonId
            AND DeletedAt IS NULL
            """;

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@SalesPersonId", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}