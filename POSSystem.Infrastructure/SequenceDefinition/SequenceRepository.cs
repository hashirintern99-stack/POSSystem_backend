using Microsoft.Data.SqlClient;
using POSSystem.Infrastructure.Data;
using System.Data;

namespace POSSystem.Repository.SequenceDefinition;

public class SequenceRepository : ISequenceRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public SequenceRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<string> GetNextCodeAsync(string entityName, string prefix)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("usp_GetNextSequence", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@EntityName", entityName);
        command.Parameters.AddWithValue("@Prefix", prefix);

        var outputParam = new SqlParameter("@Code", SqlDbType.NVarChar, 50)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputParam);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        return outputParam.Value as string
               ?? throw new InvalidOperationException("Sequence procedure returned no code");
    }
    public async Task<string> PeekNextCodeAsync(string entityName, string prefix)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("usp_PeekNextSequence", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@EntityName", entityName);
        command.Parameters.AddWithValue("@Prefix", prefix);

        var outputParam = new SqlParameter("@Code", SqlDbType.NVarChar, 50)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputParam);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        return outputParam.Value as string
               ?? throw new InvalidOperationException("Peek procedure returned no code");
    }
}