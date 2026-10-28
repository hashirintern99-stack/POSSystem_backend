using Microsoft.Data.SqlClient;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Repository.CustomerDefinition;

public class CustomerRepository : ICustomerRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public CustomerRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var customers = new List<Customer>();

        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = """
        SELECT
            CustomerId,
            Code,
            Name,
            Phone,
            Email,
            Address,
            City,
            TaxNumber,
            OpeningBalance,
            IsActive,
            CreationDate,
            UpdatedDate
        FROM Customers
        ORDER BY CustomerId DESC
        """;

        using var command = new SqlCommand(query, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            customers.Add(new Customer
            {
                CustomerId = reader.GetInt32(
                    reader.GetOrdinal("CustomerId")),

                Code = reader.GetString(
                    reader.GetOrdinal("Code")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

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

        return customers;
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = """
        SELECT
            CustomerId,
            Code,
            Name,
            Phone,
            Email,
            Address,
            City,
            TaxNumber,
            OpeningBalance,
            IsActive,
            CreationDate,
            UpdatedDate
        FROM Customers
        WHERE CustomerId = @CustomerId
        """;

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@CustomerId", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Customer
            {
                CustomerId = reader.GetInt32(
                    reader.GetOrdinal("CustomerId")),

                Code = reader.GetString(
                    reader.GetOrdinal("Code")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

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

    public async Task<int> CreateAsync(Customer customer)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = """
        INSERT INTO Customers
        (
            Code,
            Name,
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

        command.Parameters.AddWithValue("@Code", customer.Code);
        command.Parameters.AddWithValue("@Name", customer.Name);
        command.Parameters.AddWithValue(
            "@Phone",
            (object?)customer.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@Email",
            (object?)customer.Email ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@Address",
            (object?)customer.Address ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@City",
            (object?)customer.City ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@TaxNumber",
            (object?)customer.TaxNumber ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "@OpeningBalance",
            customer.OpeningBalance);
        command.Parameters.AddWithValue(
            "@IsActive",
            customer.IsActive);
        command.Parameters.AddWithValue(
            "@CreationDate",
            customer.CreationDate);

        var customerId = await command.ExecuteScalarAsync();

        return Convert.ToInt32(customerId);
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = """
        UPDATE Customers
        SET
            Code = @Code,
            Name = @Name,
            Phone = @Phone,
            Email = @Email,
            Address = @Address,
            City = @City,
            TaxNumber = @TaxNumber,
            OpeningBalance = @OpeningBalance,
            IsActive = @IsActive,
            UpdatedDate = @UpdatedDate
        WHERE CustomerId = @CustomerId
        """;

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue(
            "@CustomerId",
            customer.CustomerId);

        command.Parameters.AddWithValue(
            "@Code",
            customer.Code);

        command.Parameters.AddWithValue(
            "@Name",
            customer.Name);

        command.Parameters.AddWithValue(
            "@Phone",
            (object?)customer.Phone ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@Email",
            (object?)customer.Email ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@Address",
            (object?)customer.Address ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@City",
            (object?)customer.City ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@TaxNumber",
            (object?)customer.TaxNumber ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@OpeningBalance",
            customer.OpeningBalance);

        command.Parameters.AddWithValue(
            "@IsActive",
            customer.IsActive);

        command.Parameters.AddWithValue(
            "@UpdatedDate",
            DateTime.UtcNow);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    // ✅ ADD THIS METHOD - Check if customer has related records
    public async Task<bool> HasRelatedRecords(int customerId)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = @"
            SELECT 
                CASE 
                    WHEN EXISTS (SELECT 1 FROM SalesMaster WHERE CustomerId = @CustomerId)
                    OR EXISTS (SELECT 1 FROM SalesReturnMaster WHERE CustomerId = @CustomerId)
                    THEN 1 
                    ELSE 0 
                END AS HasRelatedRecords
        ";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@CustomerId", customerId);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToBoolean(result);
    }

    // ✅ ADD THIS METHOD - Soft delete (mark as inactive)
    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = @"
            UPDATE Customers 
            SET IsActive = 0, UpdatedDate = @UpdatedDate 
            WHERE CustomerId = @CustomerId
        ";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@CustomerId", id);
        command.Parameters.AddWithValue("@UpdatedDate", DateTime.UtcNow);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    // ✅ UPDATE DeleteAsync method
    public async Task<bool> DeleteAsync(int id)
    {
        // ✅ Check if customer has related records
        var hasRelated = await HasRelatedRecords(id);

        if (hasRelated)
        {
            // ✅ Customer is in use - Soft Delete (mark as inactive)
            return await SoftDeleteAsync(id);
        }

        // ✅ No related records - Hard Delete
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = "DELETE FROM Customers WHERE CustomerId = @CustomerId";
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@CustomerId", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
