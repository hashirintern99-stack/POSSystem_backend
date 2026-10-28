using Microsoft.Data.SqlClient;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Repository.SupplierDefinition;

public class SupplierRepository : ISupplierRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public SupplierRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        var suppliers = new List<Supplier>();

        using var connection = _databaseConnection.CreateConnection();

        const string query = """
        SELECT
            SupplierId,
            Code,
            Name,
            ContactPerson,
            Phone,
            Email,
            Address,
            City,
            TaxNumber,
            OpeningBalance,
            IsActive,
            CreationDate,
            UpdatedDate
        FROM Suppliers
        ORDER BY SupplierId DESC
        """;

        using var command = new SqlCommand(query, connection);

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            suppliers.Add(new Supplier
            {
                SupplierId = reader.GetInt32(
                    reader.GetOrdinal("SupplierId")),

                Code = reader.GetString(
                    reader.GetOrdinal("Code")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

                ContactPerson = reader.IsDBNull(
                    reader.GetOrdinal("ContactPerson"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("ContactPerson")),

                Phone = reader.IsDBNull(
                    reader.GetOrdinal("Phone"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("Phone")),

                Email = reader.IsDBNull(
                    reader.GetOrdinal("Email"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("Email")),

                Address = reader.IsDBNull(
                    reader.GetOrdinal("Address"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("Address")),

                City = reader.IsDBNull(
                    reader.GetOrdinal("City"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("City")),

                TaxNumber = reader.IsDBNull(
                    reader.GetOrdinal("TaxNumber"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("TaxNumber")),

                OpeningBalance = reader.GetDecimal(
                    reader.GetOrdinal("OpeningBalance")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreationDate = reader.GetDateTime(
                    reader.GetOrdinal("CreationDate")),

                UpdatedDate = reader.IsDBNull(
                    reader.GetOrdinal("UpdatedDate"))
                    ? null
                    : reader.GetDateTime(
                        reader.GetOrdinal("UpdatedDate"))
            });
        }

        return suppliers;
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();

        const string query = """
        SELECT
            SupplierId,
            Code,
            Name,
            ContactPerson,
            Phone,
            Email,
            Address,
            City,
            TaxNumber,
            OpeningBalance,
            IsActive,
            CreationDate,
            UpdatedDate
        FROM Suppliers
        WHERE SupplierId = @SupplierId
        """;

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@SupplierId", id);

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Supplier
            {
                SupplierId = reader.GetInt32(
                    reader.GetOrdinal("SupplierId")),

                Code = reader.GetString(
                    reader.GetOrdinal("Code")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

                ContactPerson = reader.IsDBNull(
                    reader.GetOrdinal("ContactPerson"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("ContactPerson")),

                Phone = reader.IsDBNull(
                    reader.GetOrdinal("Phone"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("Phone")),

                Email = reader.IsDBNull(
                    reader.GetOrdinal("Email"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("Email")),

                Address = reader.IsDBNull(
                    reader.GetOrdinal("Address"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("Address")),

                City = reader.IsDBNull(
                    reader.GetOrdinal("City"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("City")),

                TaxNumber = reader.IsDBNull(
                    reader.GetOrdinal("TaxNumber"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("TaxNumber")),

                OpeningBalance = reader.GetDecimal(
                    reader.GetOrdinal("OpeningBalance")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreationDate = reader.GetDateTime(
                    reader.GetOrdinal("CreationDate")),

                UpdatedDate = reader.IsDBNull(
                    reader.GetOrdinal("UpdatedDate"))
                    ? null
                    : reader.GetDateTime(
                        reader.GetOrdinal("UpdatedDate"))
            };
        }

        return null;
    }

    public async Task<int> CreateAsync(Supplier supplier)
    {
        using var connection = _databaseConnection.CreateConnection();

        const string query = """
        INSERT INTO Suppliers
        (
            Code,
            Name,
            ContactPerson,
            Phone,
            Email,
            Address,
            City,
            TaxNumber,
            OpeningBalance,
            IsActive,
            CreationDate
        )
        VALUES
        (
            @Code,
            @Name,
            @ContactPerson,
            @Phone,
            @Email,
            @Address,
            @City,
            @TaxNumber,
            @OpeningBalance,
            @IsActive,
            @CreationDate
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Code", supplier.Code);
        command.Parameters.AddWithValue("@Name", supplier.Name);
        command.Parameters.AddWithValue(
            "@ContactPerson",
            (object?)supplier.ContactPerson ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@Phone",
            (object?)supplier.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@Email",
            (object?)supplier.Email ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@Address",
            (object?)supplier.Address ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@City",
            (object?)supplier.City ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@TaxNumber",
            (object?)supplier.TaxNumber ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@OpeningBalance",
            supplier.OpeningBalance);
        command.Parameters.AddWithValue(
            "@IsActive",
            supplier.IsActive);
        command.Parameters.AddWithValue(
            "@CreationDate",
            supplier.CreationDate);

        await connection.OpenAsync();

        try
        {
            var supplierId = await command.ExecuteScalarAsync();
            return Convert.ToInt32(supplierId);
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            throw new InvalidOperationException(
                $"Supplier code '{supplier.Code}' already exists.");
        }
    }

    public async Task<bool> UpdateAsync(Supplier supplier)
    {
        using var connection = _databaseConnection.CreateConnection();

        const string query = """
        UPDATE Suppliers
        SET
            Code = @Code,
            Name = @Name,
            ContactPerson = @ContactPerson,
            Phone = @Phone,
            Email = @Email,
            Address = @Address,
            City = @City,
            TaxNumber = @TaxNumber,
            OpeningBalance = @OpeningBalance,
            IsActive = @IsActive,
            UpdatedDate = @UpdatedDate
        WHERE SupplierId = @SupplierId
        """;

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue(
            "@SupplierId",
            supplier.SupplierId);

        command.Parameters.AddWithValue(
            "@Code",
            supplier.Code);

        command.Parameters.AddWithValue(
            "@Name",
            supplier.Name);

        command.Parameters.AddWithValue(
            "@ContactPerson",
            (object?)supplier.ContactPerson ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@Phone",
            (object?)supplier.Phone ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@Email",
            (object?)supplier.Email ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@Address",
            (object?)supplier.Address ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@City",
            (object?)supplier.City ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@TaxNumber",
            (object?)supplier.TaxNumber ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@OpeningBalance",
            supplier.OpeningBalance);

        command.Parameters.AddWithValue(
            "@IsActive",
            supplier.IsActive);

        command.Parameters.AddWithValue(
            "@UpdatedDate",
            DateTime.UtcNow);

        await connection.OpenAsync();

        try
        {
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            throw new InvalidOperationException(
                $"Supplier code '{supplier.Code}' already exists.");
        }
    }
    //check for related records for soft delete
    public async Task<bool> HasRelatedRecords(int supplierId)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = @"
            SELECT 
                CASE 
                    WHEN EXISTS (SELECT 1 FROM PurchaseMaster WHERE SupplierId = @SupplierId)
                    OR EXISTS (SELECT 1 FROM GRNMaster WHERE SupplierId = @SupplierId)
                    THEN 1 
                    ELSE 0 
                END AS HasRelatedRecords
        ";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@SupplierId", supplierId);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToBoolean(result);
    }

    // ✅ ADD THIS METHOD - Soft delete (mark as inactive)
    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = @"
            UPDATE Suppliers 
            SET IsActive = 0, UpdatedDate = @UpdatedDate 
            WHERE SupplierId = @SupplierId
        ";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@SupplierId", id);
        command.Parameters.AddWithValue("@UpdatedDate", DateTime.UtcNow);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    // ✅ UPDATE DeleteAsync method
    public async Task<bool> DeleteAsync(int id)
    {
        // ✅ Check if supplier has related records
        var hasRelated = await HasRelatedRecords(id);

        if (hasRelated)
        {
            // ✅ Supplier is in use - Soft Delete (mark as inactive)
            return await SoftDeleteAsync(id);
        }

        // ✅ No related records - Hard Delete
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = "DELETE FROM Suppliers WHERE SupplierId = @SupplierId";
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@SupplierId", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}