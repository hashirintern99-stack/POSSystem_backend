// POSSystem.Repository.SaleDefinition/SaleRepository.cs

using Microsoft.Data.SqlClient;
using POSSystem.Domain.Entities;
using POSSystem.DTO.Requests.Sale;
using POSSystem.DTO.Responses.Sale;
using POSSystem.DTO.Responses.SalesReturn;
using POSSystem.Infrastructure.Data;
using System.Data;
using System.Text.Json;

namespace POSSystem.Repository.SaleDefinition;

public class SaleRepository : ISaleRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public SaleRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        var sales = new List<Sale>();

        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Sale_GetAll", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            sales.Add(new Sale
            {
                SaleId = reader.GetInt32(reader.GetOrdinal("SaleId")),
                InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerId")),
                CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName")) ? null : reader.GetString(reader.GetOrdinal("CustomerName")), // ✅ Add this
                SalesPersonId = reader.IsDBNull(reader.GetOrdinal("SalesPersonId")) ? null : reader.GetInt32(reader.GetOrdinal("SalesPersonId")),
                SalesPersonName = reader.IsDBNull(reader.GetOrdinal("SalesPersonName")) ? null : reader.GetString(reader.GetOrdinal("SalesPersonName")), // ✅ Add this
                SaleDate = reader.GetDateTime(reader.GetOrdinal("SaleDate")),
                SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                PaidAmount = reader.GetDecimal(reader.GetOrdinal("PaidAmount")),
                RemainingAmount = reader.GetDecimal(reader.GetOrdinal("RemainingAmount")),
                PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetInt32(reader.GetOrdinal("CreatedBy"))
            });
        }

        // =====================================
        // RESULT SET 2 → SALE DETAILS
        // =====================================
        await reader.NextResultAsync();

        while (await reader.ReadAsync())
        {
            var detail = new SaleDetail
            {
                SaleDetailId = reader.GetInt32(reader.GetOrdinal("SaleDetailId")),
                SaleId = reader.GetInt32(reader.GetOrdinal("SaleId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                ProductCode = reader.IsDBNull(reader.GetOrdinal("ProductCode")) ? null : reader.GetString(reader.GetOrdinal("ProductCode")),
                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
                RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total"))
            };

            var sale = sales.FirstOrDefault(x => x.SaleId == detail.SaleId);
            if (sale != null)
                sale.Details.Add(detail);
        }

        return sales;
    }

    public async Task<Sale?> GetByIdAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Sale_GetById", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add("@SaleId", SqlDbType.Int).Value = id;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        // =====================================
        // READ MASTER
        // =====================================
        Sale? sale = null;

        if (await reader.ReadAsync())
        {
            sale = new Sale
            {
                SaleId = reader.GetInt32(reader.GetOrdinal("SaleId")),
                InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerId")),
                SalesPersonId = reader.IsDBNull(reader.GetOrdinal("SalesPersonId")) ? null : reader.GetInt32(reader.GetOrdinal("SalesPersonId")),
                SaleDate = reader.GetDateTime(reader.GetOrdinal("SaleDate")),
                SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                PaidAmount = reader.GetDecimal(reader.GetOrdinal("PaidAmount")),
                RemainingAmount = reader.GetDecimal(reader.GetOrdinal("RemainingAmount")),
                PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetInt32(reader.GetOrdinal("CreatedBy"))
            };
        }

        if (sale == null)
            return null;

        // =====================================
        // READ DETAILS
        // =====================================
        await reader.NextResultAsync();

        while (await reader.ReadAsync())
        {
            sale.Details.Add(new SaleDetail
            {
                SaleDetailId = reader.GetInt32(reader.GetOrdinal("SaleDetailId")),
                SaleId = reader.GetInt32(reader.GetOrdinal("SaleId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                ProductCode = reader.IsDBNull(reader.GetOrdinal("ProductCode")) ? null : reader.GetString(reader.GetOrdinal("ProductCode")),
                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
                RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total"))
            });
        }

        return sale;
    }

    public async Task<int> CreateAsync(Sale sale, IEnumerable<SaleDetail> details)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Sale_Create", connection);
        command.CommandType = CommandType.StoredProcedure;

        // =====================================
        // MASTER PARAMETERS
        // =====================================
        command.Parameters.AddWithValue("@InvoiceNumber", sale.InvoiceNumber);
        command.Parameters.AddWithValue("@CustomerId", (object?)sale.CustomerId ?? DBNull.Value);
        command.Parameters.AddWithValue("@SalesPersonId", (object?)sale.SalesPersonId ?? DBNull.Value);
        command.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
        command.Parameters.AddWithValue("@SubTotal", sale.SubTotal);
        command.Parameters.AddWithValue("@Discount", sale.Discount);
        command.Parameters.AddWithValue("@Tax", sale.Tax);
        command.Parameters.AddWithValue("@Total", sale.Total);
        command.Parameters.AddWithValue("@PaidAmount", sale.PaidAmount);
        command.Parameters.AddWithValue("@RemainingAmount", sale.RemainingAmount);
        command.Parameters.AddWithValue("@PaymentMethod", sale.PaymentMethod);
        command.Parameters.AddWithValue("@Status", sale.Status);
        command.Parameters.AddWithValue("@Comments", (object?)sale.Comments ?? DBNull.Value);
        command.Parameters.AddWithValue("@CreatedBy", (object?)sale.CreatedBy ?? DBNull.Value);

        // =====================================
        // DETAILS → JSON with camelCase
        // =====================================
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var detailsJson = JsonSerializer.Serialize(details, options);

        // =====================================
        // SEND JSON TO SQL
        // =====================================
        command.Parameters.Add("@Details", SqlDbType.NVarChar, -1).Value = detailsJson;

        // =====================================
        // EXECUTE
        // =====================================
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateAsync(Sale sale, IEnumerable<SaleDetail> details)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Sale_Update", connection);
        command.CommandType = CommandType.StoredProcedure;

        // =====================================
        // MASTER PARAMETERS
        // =====================================
        command.Parameters.AddWithValue("@SaleId", sale.SaleId);
        command.Parameters.AddWithValue("@InvoiceNumber", sale.InvoiceNumber);
        command.Parameters.AddWithValue("@CustomerId", (object?)sale.CustomerId ?? DBNull.Value);
        command.Parameters.AddWithValue("@SalesPersonId", (object?)sale.SalesPersonId ?? DBNull.Value);
        command.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
        command.Parameters.AddWithValue("@SubTotal", sale.SubTotal);
        command.Parameters.AddWithValue("@Discount", sale.Discount);
        command.Parameters.AddWithValue("@Tax", sale.Tax);
        command.Parameters.AddWithValue("@Total", sale.Total);
        command.Parameters.AddWithValue("@PaidAmount", sale.PaidAmount);
        command.Parameters.AddWithValue("@RemainingAmount", sale.RemainingAmount);
        command.Parameters.AddWithValue("@PaymentMethod", sale.PaymentMethod);
        command.Parameters.AddWithValue("@Status", sale.Status);
        command.Parameters.AddWithValue("@Comments", (object?)sale.Comments ?? DBNull.Value);

        // =====================================
        // DETAILS → JSON with camelCase
        // =====================================
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var detailsJson = JsonSerializer.Serialize(details, options);

        // =====================================
        // SEND JSON TO SQL
        // =====================================
        command.Parameters.Add("@Details", SqlDbType.NVarChar, -1).Value = detailsJson;

        // =====================================
        // EXECUTE
        // =====================================
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return Convert.ToBoolean(result);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Sale_Delete", connection);
        command.CommandType = CommandType.StoredProcedure;

        // =====================================
        // PARAMETERS
        // =====================================
        command.Parameters.AddWithValue("@SaleId", id);

        // =====================================
        // EXECUTE
        // =====================================
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return Convert.ToBoolean(result);
    }

    // =============================================
    // SEARCH ASYNC
    // =============================================
    public async Task<IEnumerable<SearchSaleResponse>> SearchAsync(SearchSaleRequest request)
    {
        var results = new List<SearchSaleResponse>();

        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Sale_Search", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@InvoiceNumber", (object?)request.InvoiceNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@CustomerId", (object?)request.CustomerId ?? DBNull.Value);
        command.Parameters.AddWithValue("@SalesPersonId", (object?)request.SalesPersonId ?? DBNull.Value);
        command.Parameters.AddWithValue("@FromDate", (object?)request.FromDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@ToDate", (object?)request.ToDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@Status", (object?)request.Status ?? DBNull.Value);
        command.Parameters.AddWithValue("@PaymentMethod", (object?)request.PaymentMethod ?? DBNull.Value);
        command.Parameters.AddWithValue("@MinTotal", (object?)request.MinTotal ?? DBNull.Value);
        command.Parameters.AddWithValue("@MaxTotal", (object?)request.MaxTotal ?? DBNull.Value);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(new SearchSaleResponse
            {
                SaleId = reader.GetInt32(reader.GetOrdinal("SaleId")),
                InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerId")),
                CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName")) ? null : reader.GetString(reader.GetOrdinal("CustomerName")),
                CustomerPhone = reader.IsDBNull(reader.GetOrdinal("CustomerPhone")) ? null : reader.GetString(reader.GetOrdinal("CustomerPhone")),
                SalesPersonId = reader.IsDBNull(reader.GetOrdinal("SalesPersonId")) ? null : reader.GetInt32(reader.GetOrdinal("SalesPersonId")),
                SalesPersonName = reader.IsDBNull(reader.GetOrdinal("SalesPersonName")) ? null : reader.GetString(reader.GetOrdinal("SalesPersonName")),
                SaleDate = reader.GetDateTime(reader.GetOrdinal("SaleDate")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                PaidAmount = reader.GetDecimal(reader.GetOrdinal("PaidAmount")),
                RemainingAmount = reader.GetDecimal(reader.GetOrdinal("RemainingAmount")),
                PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                TotalItems = reader.GetInt32(reader.GetOrdinal("TotalItems")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"))
            });
        }

        return results;
    }

    // =============================================
    // GET SALE DETAILS FOR SEARCH
    // =============================================
    public async Task<SaleFullDetailResponse?> GetSaleDetailsForSearchAsync(int saleId)
    {
        SaleFullDetailResponse? result = null;

        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Sale_GetDetailsForSearch", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@SaleId", saleId);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        bool hasRows = false;

        while (await reader.ReadAsync())
        {
            if (!hasRows)
            {
                result = new SaleFullDetailResponse
                {
                    SaleId = reader.GetInt32(reader.GetOrdinal("SaleId")),
                    InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                    CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerId")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerId")),
                    CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName")) ? null : reader.GetString(reader.GetOrdinal("CustomerName")),
                    CustomerPhone = reader.IsDBNull(reader.GetOrdinal("CustomerPhone")) ? null : reader.GetString(reader.GetOrdinal("CustomerPhone")),
                    CustomerAddress = reader.IsDBNull(reader.GetOrdinal("CustomerAddress")) ? null : reader.GetString(reader.GetOrdinal("CustomerAddress")),
                    SalesPersonId = reader.IsDBNull(reader.GetOrdinal("SalesPersonId")) ? null : reader.GetInt32(reader.GetOrdinal("SalesPersonId")),
                    SalesPersonName = reader.IsDBNull(reader.GetOrdinal("SalesPersonName")) ? null : reader.GetString(reader.GetOrdinal("SalesPersonName")),
                    SaleDate = reader.GetDateTime(reader.GetOrdinal("SaleDate")),
                    SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                    Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                    Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                    Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                    PaidAmount = reader.GetDecimal(reader.GetOrdinal("PaidAmount")),
                    RemainingAmount = reader.GetDecimal(reader.GetOrdinal("RemainingAmount")),
                    PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments")),
                    Details = new List<SaleDetailItemResponse>()
                };
                hasRows = true;
            }

            // Check if there's a detail row (not null)
            if (!reader.IsDBNull(reader.GetOrdinal("SaleDetailId")))
            {
                result.Details.Add(new SaleDetailItemResponse
                {
                    SaleDetailId = reader.GetInt32(reader.GetOrdinal("SaleDetailId")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                    ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                    ProductCode = reader.IsDBNull(reader.GetOrdinal("ProductCode")) ? null : reader.GetString(reader.GetOrdinal("ProductCode")),
                    Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
                    RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                    Discount = reader.GetDecimal(reader.GetOrdinal("DetailDiscount")),
                    Tax = reader.GetDecimal(reader.GetOrdinal("DetailTax")),
                    Total = reader.GetDecimal(reader.GetOrdinal("DetailTotal"))
                });
            }
        }

        return result;
    }

    // =============================================
    // GET RETURNS FOR SALE
    // =============================================
    public async Task<IEnumerable<SalesReturnResponse>> GetReturnsForSaleAsync(int saleId)
    {
        var returns = new List<SalesReturnResponse>();

        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Sale_GetReturnsForSearch", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@SaleId", saleId);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            returns.Add(new SalesReturnResponse
            {
                SalesReturnId = reader.GetInt32(reader.GetOrdinal("SalesReturnId")),
                ReturnNumber = reader.GetString(reader.GetOrdinal("ReturnNumber")),
                ReturnDate = reader.GetDateTime(reader.GetOrdinal("ReturnDate")),
                Total = reader.GetDecimal(reader.GetOrdinal("ReturnTotal")),
                Comments = reader.IsDBNull(reader.GetOrdinal("ReturnComments")) ? null : reader.GetString(reader.GetOrdinal("ReturnComments"))
            });
        }

        return returns;
    }
    //check is sale is used 
    public async Task<bool> HasReturnsAsync(int saleId)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = @"
        SELECT CASE 
            WHEN EXISTS (SELECT 1 FROM SalesReturnMaster WHERE SaleId = @SaleId) 
            THEN 1 
            ELSE 0 
        END
    ";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@SaleId", saleId);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToBoolean(result);
    }
}