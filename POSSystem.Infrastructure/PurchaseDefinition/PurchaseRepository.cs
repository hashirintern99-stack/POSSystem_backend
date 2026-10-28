using Microsoft.Data.SqlClient;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Data;
using System.Text.Json;
using System.Data;

namespace POSSystem.Repository.PurchaseDefinition;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public PurchaseRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    // =========================
    // GET ALL PURCHASES
    // =========================
    public async Task<IEnumerable<Purchase>> GetAllAsync()
    {
        var purchases = new List<Purchase>();

        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Purchase_GetAll", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            purchases.Add(new Purchase
            {
                PurchaseId = reader.GetInt32(reader.GetOrdinal("PurchaseId")),
                PurchaseNumber = reader.GetString(reader.GetOrdinal("PurchaseNumber")),
                SupplierId = reader.GetInt32(reader.GetOrdinal("SupplierId")),

                // ✅ ADD THIS - SupplierName
                SupplierName = reader.IsDBNull(reader.GetOrdinal("SupplierName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("SupplierName")),

                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                ExpectedDeliveryDate = reader.IsDBNull(reader.GetOrdinal("ExpectedDeliveryDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("ExpectedDeliveryDate")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                Comments = reader.IsDBNull(reader.GetOrdinal("Comments"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Comments")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
            });
        }

        return purchases;
    }

    // =========================
    // GET PURCHASE BY ID
    // =========================
    public async Task<Purchase?> GetByIdAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Purchase_GetById", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = id;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        Purchase? purchase = null;

        // -------------------------
        // Read PurchaseMaster
        // -------------------------
        if (await reader.ReadAsync())
        {
            purchase = new Purchase
            {
                PurchaseId = reader.GetInt32(reader.GetOrdinal("PurchaseId")),
                PurchaseNumber = reader.GetString(reader.GetOrdinal("PurchaseNumber")),
                SupplierId = reader.GetInt32(reader.GetOrdinal("SupplierId")),

                // ✅ ADD THIS - SupplierName
                SupplierName = reader.IsDBNull(reader.GetOrdinal("SupplierName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("SupplierName")),

                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                ExpectedDeliveryDate = reader.IsDBNull(reader.GetOrdinal("ExpectedDeliveryDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("ExpectedDeliveryDate")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                Comments = reader.IsDBNull(reader.GetOrdinal("Comments"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Comments")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
            };
        }

        if (purchase == null) return null;

        // -------------------------
        // Move to PurchaseDetail
        // -------------------------
        await reader.NextResultAsync();

        // -------------------------
        // Read PurchaseDetails
        // -------------------------
        while (await reader.ReadAsync())
        {
            purchase.Details.Add(new PurchaseDetail
            {
                PurchaseDetailId = reader.GetInt32(reader.GetOrdinal("PurchaseDetailId")),
                PurchaseId = reader.GetInt32(reader.GetOrdinal("PurchaseId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),

                // ✅ ADD THIS - ProductName
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ProductName")),

                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
                CostPrice = reader.GetDecimal(reader.GetOrdinal("CostPrice")),
                Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total"))
            });
        }

        return purchase;
    }

    // =========================
    // CREATE PURCHASE
    // =========================
    public async Task<int> CreateAsync(Purchase purchase, IEnumerable<PurchaseDetail> details)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Purchase_Create", connection);
        command.CommandType = CommandType.StoredProcedure;

        // Master parameters
        command.Parameters.AddWithValue("@PurchaseNumber", purchase.PurchaseNumber);
        command.Parameters.AddWithValue("@SupplierId", purchase.SupplierId);
        command.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
        command.Parameters.AddWithValue("@ExpectedDeliveryDate", (object?)purchase.ExpectedDeliveryDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@Status", purchase.Status);
        command.Parameters.AddWithValue("@SubTotal", purchase.SubTotal);
        command.Parameters.AddWithValue("@Discount", purchase.Discount);
        command.Parameters.AddWithValue("@Tax", purchase.Tax);
        command.Parameters.AddWithValue("@Total", purchase.Total);
        command.Parameters.AddWithValue("@Comments", (object?)purchase.Comments ?? DBNull.Value);
        command.Parameters.AddWithValue("@CreatedBy", (object?)purchase.CreatedBy ?? DBNull.Value);
        command.Parameters.AddWithValue("@CreationDate", purchase.CreationDate);

        // Create Table-Valued Parameter
        var detailTable = new DataTable();
        detailTable.Columns.Add("ProductId", typeof(int));
        detailTable.Columns.Add("Quantity", typeof(decimal));
        detailTable.Columns.Add("CostPrice", typeof(decimal));
        detailTable.Columns.Add("Discount", typeof(decimal));
        detailTable.Columns.Add("Tax", typeof(decimal));
        detailTable.Columns.Add("Total", typeof(decimal));

        foreach (var detail in details)
        {
            detailTable.Rows.Add(
                detail.ProductId,
                detail.Quantity,
                detail.CostPrice,
                detail.Discount,
                detail.Tax,
                detail.Total);
        }

        var detailsParameter = command.Parameters.AddWithValue("@Details", detailTable);
        detailsParameter.SqlDbType = SqlDbType.Structured;
        detailsParameter.TypeName = "dbo.PurchaseDetailType";

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    // =========================
    // UPDATE PURCHASE using RowState
    // =========================
    public async Task<bool> UpdateAsync(Purchase purchase, IEnumerable<PurchaseDetail> details)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Purchase_Update", connection);
        command.CommandType = CommandType.StoredProcedure;

        // Purchase Master Parameters
        command.Parameters.AddWithValue("@PurchaseId", purchase.PurchaseId);
        command.Parameters.AddWithValue("@PurchaseNumber", purchase.PurchaseNumber);
        command.Parameters.AddWithValue("@SupplierId", purchase.SupplierId);
        command.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
        command.Parameters.AddWithValue("@ExpectedDeliveryDate", (object?)purchase.ExpectedDeliveryDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@Status", purchase.Status);
        command.Parameters.AddWithValue("@SubTotal", purchase.SubTotal);
        command.Parameters.AddWithValue("@Discount", purchase.Discount);
        command.Parameters.AddWithValue("@Tax", purchase.Tax);
        command.Parameters.AddWithValue("@Total", purchase.Total);
        command.Parameters.AddWithValue("@Comments", (object?)purchase.Comments ?? DBNull.Value);

        // Convert Details to JSON
        var detailsJson = JsonSerializer.Serialize(details);
        command.Parameters.Add("@Details", SqlDbType.NVarChar, -1).Value = detailsJson;

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToBoolean(result);
    }

    // =========================
    // DELETE PURCHASE
    // =========================
    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Purchase_Delete", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = id;

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToBoolean(result);
    }
    //update status of po 
    public async Task<bool> UpdateStatusAsync(int purchaseId, string status)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("Purchase_UpdateStatus", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@PurchaseId", purchaseId);
        command.Parameters.AddWithValue("@Status", status);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToBoolean(result);
    }
}