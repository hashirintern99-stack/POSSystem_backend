using Microsoft.Data.SqlClient;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Data;
using System.Data;
using System.Text.Json;

namespace POSSystem.Repository.GRNDefinition;

public class GRNRepository : IGRNRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public GRNRepository(
        DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<GRN>> GetAllAsync()
    {
        var grns = new List<GRN>();

        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("GRN_GetAll", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        // ✅ Read GRN Master
        while (await reader.ReadAsync())
        {
            grns.Add(new GRN
            {
                GRNId = reader.GetInt32(reader.GetOrdinal("GRNId")),
                GRNNumber = reader.GetString(reader.GetOrdinal("GRNNumber")),
                PurchaseId = reader.IsDBNull(reader.GetOrdinal("PurchaseId"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("PurchaseId")),
                SupplierId = reader.GetInt32(reader.GetOrdinal("SupplierId")),

                // ✅ READ SupplierName
                SupplierName = reader.IsDBNull(reader.GetOrdinal("SupplierName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("SupplierName")),

                GRNDate = reader.GetDateTime(reader.GetOrdinal("GRNDate")),

                // ✅ CHANGED: Read ReceivedBy as string
                ReceivedBy = reader.IsDBNull(reader.GetOrdinal("ReceivedBy"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ReceivedBy")),

                Comments = reader.IsDBNull(reader.GetOrdinal("Comments"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Comments")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"))
            });
        }

        // ✅ Read GRN Details
        await reader.NextResultAsync();
        while (await reader.ReadAsync())
        {
            var detail = new GRNDetail
            {
                GRNDetailId = reader.GetInt32(reader.GetOrdinal("GRNDetailId")),
                GRNId = reader.GetInt32(reader.GetOrdinal("GRNId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                OrderedQuantity = reader.GetDecimal(reader.GetOrdinal("OrderedQuantity")),
                ReceivedQuantity = reader.GetDecimal(reader.GetOrdinal("ReceivedQuantity")),
                DamagedQuantity = reader.GetDecimal(reader.GetOrdinal("DamagedQuantity")),
                AcceptedQuantity = reader.GetDecimal(reader.GetOrdinal("AcceptedQuantity")),
                CostPrice = reader.GetDecimal(reader.GetOrdinal("CostPrice")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total"))
            };

            var grn = grns.FirstOrDefault(x => x.GRNId == detail.GRNId);
            if (grn != null)
            {
                grn.Details.Add(detail);
            }
        }

        return grns;
    }

    public async Task<GRN?> GetByIdAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("GRN_GetById", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@GRNId", SqlDbType.Int).Value = id;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        GRN? grn = null;

        if (await reader.ReadAsync())
        {
            grn = new GRN
            {
                GRNId = reader.GetInt32(reader.GetOrdinal("GRNId")),
                GRNNumber = reader.GetString(reader.GetOrdinal("GRNNumber")),
                PurchaseId = reader.IsDBNull(reader.GetOrdinal("PurchaseId"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("PurchaseId")),
                SupplierId = reader.GetInt32(reader.GetOrdinal("SupplierId")),

                // ✅ READ SupplierName
                SupplierName = reader.IsDBNull(reader.GetOrdinal("SupplierName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("SupplierName")),

                GRNDate = reader.GetDateTime(reader.GetOrdinal("GRNDate")),

                // ✅ CHANGED: Read ReceivedBy as string
                ReceivedBy = reader.IsDBNull(reader.GetOrdinal("ReceivedBy"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ReceivedBy")),

                Comments = reader.IsDBNull(reader.GetOrdinal("Comments"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Comments")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"))
            };
        }

        if (grn == null) return null;

        await reader.NextResultAsync();
        while (await reader.ReadAsync())
        {
            grn.Details.Add(new GRNDetail
            {
                GRNDetailId = reader.GetInt32(reader.GetOrdinal("GRNDetailId")),
                GRNId = reader.GetInt32(reader.GetOrdinal("GRNId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                OrderedQuantity = reader.GetDecimal(reader.GetOrdinal("OrderedQuantity")),
                ReceivedQuantity = reader.GetDecimal(reader.GetOrdinal("ReceivedQuantity")),
                DamagedQuantity = reader.GetDecimal(reader.GetOrdinal("DamagedQuantity")),
                AcceptedQuantity = reader.GetDecimal(reader.GetOrdinal("AcceptedQuantity")),
                CostPrice = reader.GetDecimal(reader.GetOrdinal("CostPrice")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total"))
            });
        }

        return grn;
    }

    public async Task<int> CreateAsync(GRN grn, IEnumerable<GRNDetail> details)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("GRN_Create", connection);
        command.CommandType = CommandType.StoredProcedure;

        // Master parameters
        command.Parameters.AddWithValue("@GRNNumber", grn.GRNNumber);
        command.Parameters.AddWithValue("@PurchaseId", grn.PurchaseId.HasValue ? (object)grn.PurchaseId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@SupplierId", grn.SupplierId);
        command.Parameters.AddWithValue("@GRNDate", grn.GRNDate);

        // ✅ CHANGED: ReceivedBy is now string
        command.Parameters.AddWithValue("@ReceivedBy", (object?)grn.ReceivedBy ?? DBNull.Value);
        command.Parameters.AddWithValue("@Comments", (object?)grn.Comments ?? DBNull.Value);

        // Details to JSON
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        var detailsJson = JsonSerializer.Serialize(details, options);
        command.Parameters.Add("@Details", SqlDbType.NVarChar, -1).Value = detailsJson;

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateAsync(GRN grn, IEnumerable<GRNDetail> details)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("GRN_Update", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@GRNId", grn.GRNId);
        command.Parameters.AddWithValue("@GRNNumber", grn.GRNNumber);
        command.Parameters.AddWithValue("@PurchaseId", grn.PurchaseId.HasValue ? (object)grn.PurchaseId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@SupplierId", grn.SupplierId);
        command.Parameters.AddWithValue("@GRNDate", grn.GRNDate);

        // ✅ CHANGED: ReceivedBy is now string
        command.Parameters.AddWithValue("@ReceivedBy", (object?)grn.ReceivedBy ?? DBNull.Value);
        command.Parameters.AddWithValue("@Comments", (object?)grn.Comments ?? DBNull.Value);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        var detailsJson = JsonSerializer.Serialize(details, options);
        command.Parameters.Add("@Details", SqlDbType.NVarChar, -1).Value = detailsJson;

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToBoolean(result);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("GRN_Delete", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@GRNId", id);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToBoolean(result);
    }
}