using Microsoft.Data.SqlClient;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Data;


namespace POSSystem.Repository.CategoryDefinition;

public class CategoryRepository : ICategoryRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public CategoryRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        var categories = new List<Category>();

        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            SELECT CategoryId, Name, IsActive, CreationDate
            FROM Categories
            ORDER BY CategoryId DESC";

        using var command = new SqlCommand(query, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            categories.Add(new Category
            {
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreationDate = reader.GetDateTime(
                    reader.GetOrdinal("CreationDate"))
            });
        }

        return categories;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            SELECT CategoryId, Name, IsActive, CreationDate
            FROM Categories
            WHERE CategoryId = @CategoryId";

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@CategoryId", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Category
            {
                CategoryId = reader.GetInt32(
                    reader.GetOrdinal("CategoryId")),

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

    public async Task<int> CreateAsync(Category category)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            INSERT INTO Categories
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

        command.Parameters.AddWithValue("@Name", category.Name);
        command.Parameters.AddWithValue("@IsActive", category.IsActive);
        command.Parameters.AddWithValue(
            "@CreationDate",
            category.CreationDate);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            UPDATE Categories
            SET
                Name = @Name,
                IsActive = @IsActive
            WHERE CategoryId = @CategoryId";

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@CategoryId", category.CategoryId);
        command.Parameters.AddWithValue("@Name", category.Name);
        command.Parameters.AddWithValue("@IsActive", category.IsActive);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            DELETE FROM Categories
            WHERE CategoryId = @CategoryId";

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@CategoryId", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }
}