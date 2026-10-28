using Microsoft.Data.SqlClient;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Repository.BrandDefinition;

public class BrandRepository : IBrandRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public BrandRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<Brand>> GetAllAsync()
    {
        var brands = new List<Brand>();

        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            SELECT BrandId, Name, IsActive, CreationDate
            FROM Brands
            ORDER BY BrandId DESC";

        using var command = new SqlCommand(query, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            brands.Add(new Brand
            {
                BrandId = reader.GetInt32(
                    reader.GetOrdinal("BrandId")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreationDate = reader.GetDateTime(
                    reader.GetOrdinal("CreationDate"))
            });
        }

        return brands;
    }

    public async Task<Brand?> GetByIdAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            SELECT BrandId, Name, IsActive, CreationDate
            FROM Brands
            WHERE BrandId = @BrandId";

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@BrandId", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Brand
            {
                BrandId = reader.GetInt32(
                    reader.GetOrdinal("BrandId")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreationDate = reader.GetDateTime(
                    reader.GetOrdinal("CreationDate"))
            };
        }

        return null;
    }

    public async Task<int> CreateAsync(Brand brand)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            INSERT INTO Brands
            (
                Name,
                IsActive,
                CreationDate
            )
            VALUES
            (
                @Name,
                @IsActive,
                @CreationDate
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Name", brand.Name);
        command.Parameters.AddWithValue("@IsActive", brand.IsActive);
        command.Parameters.AddWithValue(
            "@CreationDate",
            brand.CreationDate);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateAsync(Brand brand)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            UPDATE Brands
            SET
                Name = @Name,
                IsActive = @IsActive
            WHERE BrandId = @BrandId";

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@BrandId", brand.BrandId);
        command.Parameters.AddWithValue("@Name", brand.Name);
        command.Parameters.AddWithValue("@IsActive", brand.IsActive);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            DELETE FROM Brands
            WHERE BrandId = @BrandId";

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@BrandId", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }
}