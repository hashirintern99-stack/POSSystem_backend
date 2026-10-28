using Microsoft.Data.SqlClient;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Data;


namespace POSSystem.Repository.UnitDefinition;

public class UnitRepository : IUnitRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public UnitRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<Unit>> GetAllAsync()
    {
        var units = new List<Unit>();

        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            SELECT UnitId, Name, ShortName, IsActive, CreationDate
            FROM Units
            ORDER BY UnitId DESC";

        using var command = new SqlCommand(query, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            units.Add(new Unit
            {
                UnitId = reader.GetInt32(
                    reader.GetOrdinal("UnitId")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

                ShortName = reader.GetString(
                    reader.GetOrdinal("ShortName")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreationDate = reader.GetDateTime(
                    reader.GetOrdinal("CreationDate"))
            });
        }

        return units;
    }

    public async Task<Unit?> GetByIdAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            SELECT UnitId, Name, ShortName, IsActive, CreationDate
            FROM Units
            WHERE UnitId = @UnitId";

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@UnitId", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Unit
            {
                UnitId = reader.GetInt32(
                    reader.GetOrdinal("UnitId")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

                ShortName = reader.GetString(
                    reader.GetOrdinal("ShortName")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreationDate = reader.GetDateTime(
                    reader.GetOrdinal("CreationDate"))
            };
        }

        return null;
    }

    public async Task<int> CreateAsync(Unit unit)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            INSERT INTO Units
            (
                Name,
                ShortName,
                IsActive,
                CreationDate
            )
            VALUES
            (
                @Name,
                @ShortName,
                @IsActive,
                @CreationDate
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Name", unit.Name);
        command.Parameters.AddWithValue("@ShortName", unit.ShortName);
        command.Parameters.AddWithValue("@IsActive", unit.IsActive);
        command.Parameters.AddWithValue(
            "@CreationDate",
            unit.CreationDate);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateAsync(Unit unit)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            UPDATE Units
            SET
                Name = @Name,
                ShortName = @ShortName,
                IsActive = @IsActive
            WHERE UnitId = @UnitId";

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@UnitId", unit.UnitId);
        command.Parameters.AddWithValue("@Name", unit.Name);
        command.Parameters.AddWithValue("@ShortName", unit.ShortName);
        command.Parameters.AddWithValue("@IsActive", unit.IsActive);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();

        await connection.OpenAsync();

        const string query = @"
            DELETE FROM Units
            WHERE UnitId = @UnitId";

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@UnitId", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }
}