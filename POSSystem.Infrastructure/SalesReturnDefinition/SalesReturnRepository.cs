using Microsoft.Data.SqlClient;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Data;
using System.Data;
using System.Text.Json;

namespace POSSystem.Repository.SalesReturnDefinition;

public class SalesReturnRepository : ISalesReturnRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public SalesReturnRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<SalesReturn>> GetAllAsync()
    {
        var returns = new List<SalesReturn>();

        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("SalesReturn_GetAll", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            returns.Add(new SalesReturn
            {
                SalesReturnId = reader.GetInt32(reader.GetOrdinal("SalesReturnId")),
                ReturnNumber = reader.GetString(reader.GetOrdinal("ReturnNumber")),
                SaleId = reader.IsDBNull(reader.GetOrdinal("SaleId")) ? null : reader.GetInt32(reader.GetOrdinal("SaleId")),
                CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerId")),
                ReturnDate = reader.GetDateTime(reader.GetOrdinal("ReturnDate")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                // ✅ ADD THESE
                IsManualReturn = reader.GetBoolean(reader.GetOrdinal("IsManualReturn")),
                OriginalInvoiceNumber = reader.IsDBNull(reader.GetOrdinal("OriginalInvoiceNumber")) ? null : reader.GetString(reader.GetOrdinal("OriginalInvoiceNumber")),
                ReturnType = reader.IsDBNull(reader.GetOrdinal("ReturnType")) ? null : reader.GetString(reader.GetOrdinal("ReturnType")),
                ExchangeSaleId = reader.IsDBNull(reader.GetOrdinal("ExchangeSaleId")) ? null : reader.GetInt32(reader.GetOrdinal("ExchangeSaleId")),
                ReturnReason = reader.IsDBNull(reader.GetOrdinal("ReturnReason")) ? null : reader.GetString(reader.GetOrdinal("ReturnReason"))
            });
        }

        await reader.NextResultAsync();

        while (await reader.ReadAsync())
        {
            var detail = new SalesReturnDetail
            {
                SalesReturnDetailId = reader.GetInt32(reader.GetOrdinal("SalesReturnDetailId")),
                SalesReturnId = reader.GetInt32(reader.GetOrdinal("SalesReturnId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
                RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                Reason = reader.IsDBNull(reader.GetOrdinal("Reason")) ? null : reader.GetString(reader.GetOrdinal("Reason"))
            };

            var salesReturn = returns.FirstOrDefault(x => x.SalesReturnId == detail.SalesReturnId);
            if (salesReturn != null)
                salesReturn.Details.Add(detail);
        }

        return returns;
    }

    public async Task<SalesReturn?> GetByIdAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("SalesReturn_GetById", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@SalesReturnId", SqlDbType.Int).Value = id;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        SalesReturn? salesReturn = null;

        if (await reader.ReadAsync())
        {
            salesReturn = new SalesReturn
            {
                SalesReturnId = reader.GetInt32(reader.GetOrdinal("SalesReturnId")),
                ReturnNumber = reader.GetString(reader.GetOrdinal("ReturnNumber")),
                SaleId = reader.IsDBNull(reader.GetOrdinal("SaleId")) ? null : reader.GetInt32(reader.GetOrdinal("SaleId")),
                CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerId")),
                ReturnDate = reader.GetDateTime(reader.GetOrdinal("ReturnDate")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                // ✅ ADD THESE
                IsManualReturn = reader.GetBoolean(reader.GetOrdinal("IsManualReturn")),
                OriginalInvoiceNumber = reader.IsDBNull(reader.GetOrdinal("OriginalInvoiceNumber")) ? null : reader.GetString(reader.GetOrdinal("OriginalInvoiceNumber")),
                ReturnType = reader.IsDBNull(reader.GetOrdinal("ReturnType")) ? null : reader.GetString(reader.GetOrdinal("ReturnType")),
                ExchangeSaleId = reader.IsDBNull(reader.GetOrdinal("ExchangeSaleId")) ? null : reader.GetInt32(reader.GetOrdinal("ExchangeSaleId")),
                ReturnReason = reader.IsDBNull(reader.GetOrdinal("ReturnReason")) ? null : reader.GetString(reader.GetOrdinal("ReturnReason"))
            };
        }

        if (salesReturn == null) return null;

        await reader.NextResultAsync();

        while (await reader.ReadAsync())
        {
            salesReturn.Details.Add(new SalesReturnDetail
            {
                SalesReturnDetailId = reader.GetInt32(reader.GetOrdinal("SalesReturnDetailId")),
                SalesReturnId = reader.GetInt32(reader.GetOrdinal("SalesReturnId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
                RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                Reason = reader.IsDBNull(reader.GetOrdinal("Reason")) ? null : reader.GetString(reader.GetOrdinal("Reason"))
            });
        }

        return salesReturn;
    }

    public async Task<IEnumerable<SalesReturn>> GetBySaleIdAsync(int saleId)
    {
        var returns = new List<SalesReturn>();

        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("SalesReturn_GetBySaleId", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@SaleId", SqlDbType.Int).Value = saleId;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var salesReturnId = reader.GetInt32(reader.GetOrdinal("SalesReturnId"));
            var salesReturn = returns.FirstOrDefault(x => x.SalesReturnId == salesReturnId);

            if (salesReturn == null)
            {
                salesReturn = new SalesReturn
                {
                    SalesReturnId = salesReturnId,
                    ReturnNumber = reader.GetString(reader.GetOrdinal("ReturnNumber")),
                    SaleId = reader.IsDBNull(reader.GetOrdinal("SaleId")) ? null : reader.GetInt32(reader.GetOrdinal("SaleId")),
                    CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerId")),
                    ReturnDate = reader.GetDateTime(reader.GetOrdinal("ReturnDate")),
                    Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                    Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments")),
                    CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                    CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                    // ✅ ADD THESE
                    IsManualReturn = reader.GetBoolean(reader.GetOrdinal("IsManualReturn")),
                    OriginalInvoiceNumber = reader.IsDBNull(reader.GetOrdinal("OriginalInvoiceNumber")) ? null : reader.GetString(reader.GetOrdinal("OriginalInvoiceNumber")),
                    ReturnType = reader.IsDBNull(reader.GetOrdinal("ReturnType")) ? null : reader.GetString(reader.GetOrdinal("ReturnType")),
                    ExchangeSaleId = reader.IsDBNull(reader.GetOrdinal("ExchangeSaleId")) ? null : reader.GetInt32(reader.GetOrdinal("ExchangeSaleId")),
                    ReturnReason = reader.IsDBNull(reader.GetOrdinal("ReturnReason")) ? null : reader.GetString(reader.GetOrdinal("ReturnReason"))
                };
                returns.Add(salesReturn);
            }

            salesReturn.Details.Add(new SalesReturnDetail
            {
                SalesReturnDetailId = reader.GetInt32(reader.GetOrdinal("SalesReturnDetailId")),
                SalesReturnId = salesReturnId,
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
                RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                Reason = reader.IsDBNull(reader.GetOrdinal("Reason")) ? null : reader.GetString(reader.GetOrdinal("Reason"))
            });
        }

        return returns;
    }

    public async Task<int> CreateAsync(SalesReturn salesReturn, IEnumerable<SalesReturnDetail> details)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("SalesReturn_Create", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@ReturnNumber", salesReturn.ReturnNumber);
        command.Parameters.AddWithValue("@SaleId", salesReturn.SaleId.HasValue ? (object)salesReturn.SaleId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@CustomerId", (object?)salesReturn.CustomerId ?? DBNull.Value);
        command.Parameters.AddWithValue("@ReturnDate", salesReturn.ReturnDate);
        command.Parameters.AddWithValue("@Total", salesReturn.Total);
        command.Parameters.AddWithValue("@Comments", (object?)salesReturn.Comments ?? DBNull.Value);
        command.Parameters.AddWithValue("@CreatedBy", (object?)salesReturn.CreatedBy ?? DBNull.Value);

        // ✅ ADD THESE NEW PARAMETERS
        command.Parameters.AddWithValue("@IsManualReturn", salesReturn.IsManualReturn);
        command.Parameters.AddWithValue("@OriginalInvoiceNumber", (object?)salesReturn.OriginalInvoiceNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@ReturnType", (object?)salesReturn.ReturnType ?? DBNull.Value);
        command.Parameters.AddWithValue("@ExchangeSaleId", salesReturn.ExchangeSaleId.HasValue ? (object)salesReturn.ExchangeSaleId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@ReturnReason", (object?)salesReturn.ReturnReason ?? DBNull.Value);

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

    public async Task<bool> UpdateAsync(SalesReturn salesReturn, IEnumerable<SalesReturnDetail> details)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("SalesReturn_Update", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@SalesReturnId", salesReturn.SalesReturnId);
        command.Parameters.AddWithValue("@ReturnNumber", salesReturn.ReturnNumber);
        command.Parameters.AddWithValue("@SaleId", salesReturn.SaleId.HasValue ? (object)salesReturn.SaleId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@CustomerId", (object?)salesReturn.CustomerId ?? DBNull.Value);
        command.Parameters.AddWithValue("@ReturnDate", salesReturn.ReturnDate);
        command.Parameters.AddWithValue("@Total", salesReturn.Total);
        command.Parameters.AddWithValue("@Comments", (object?)salesReturn.Comments ?? DBNull.Value);

        // ✅ ADD THESE NEW PARAMETERS
        command.Parameters.AddWithValue("@IsManualReturn", salesReturn.IsManualReturn);
        command.Parameters.AddWithValue("@OriginalInvoiceNumber", (object?)salesReturn.OriginalInvoiceNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@ReturnType", (object?)salesReturn.ReturnType ?? DBNull.Value);
        command.Parameters.AddWithValue("@ExchangeSaleId", salesReturn.ExchangeSaleId.HasValue ? (object)salesReturn.ExchangeSaleId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@ReturnReason", (object?)salesReturn.ReturnReason ?? DBNull.Value);

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
        using var command = new SqlCommand("SalesReturn_Delete", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@SalesReturnId", id);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return Convert.ToBoolean(result);
    }
}