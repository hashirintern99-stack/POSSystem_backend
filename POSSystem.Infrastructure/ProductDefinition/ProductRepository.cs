using Microsoft.Data.SqlClient;
using POSSystem.Domain.Entities;
using POSSystem.DTO.Responses.Product;
using POSSystem.Infrastructure.Data;
using System.Data;


namespace POSSystem.Repository.ProductDefinition;  // Updated namespace

public class ProductRepository : IProductRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public ProductRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<int> CreateAsync(Product product)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = """
    INSERT INTO Products
    (
        Code, Barcode, Name, Description, CategoryId, BrandId, UnitId,
        CostPrice, RetailPrice, TaxPercentage, MinimumStock, ImageURL,
        IsActive, CreationDate
    )
    VALUES
    (
        @Code, @Barcode, @Name, @Description, @CategoryId, @BrandId, @UnitId,
        @CostPrice, @RetailPrice, @TaxPercentage, @MinimumStock, @ImageURL,
        @IsActive, @CreationDate
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
    """;

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Code", product.Code);
        command.Parameters.AddWithValue("@Barcode", (object?)product.Barcode ?? DBNull.Value);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);

        // ✅ CategoryId is NOT NULL - always has value
        command.Parameters.AddWithValue("@CategoryId", product.CategoryId);

        // ✅ BrandId is NULLABLE
        command.Parameters.AddWithValue("@BrandId", product.BrandId.HasValue ? (object)product.BrandId.Value : DBNull.Value);

        // ✅ UnitId is NULLABLE
        command.Parameters.AddWithValue("@UnitId", product.UnitId.HasValue ? (object)product.UnitId.Value : DBNull.Value);

        command.Parameters.AddWithValue("@CostPrice", product.CostPrice);
        command.Parameters.AddWithValue("@RetailPrice", product.RetailPrice);
        command.Parameters.AddWithValue("@TaxPercentage", product.TaxPercentage);
        command.Parameters.AddWithValue("@MinimumStock", product.MinimumStock);
        command.Parameters.AddWithValue("@ImageURL", (object?)product.ImageURL ?? DBNull.Value);
        command.Parameters.AddWithValue("@IsActive", product.IsActive);
        command.Parameters.AddWithValue("@CreationDate", product.CreationDate);

        try
        {
            var productId = await command.ExecuteScalarAsync();
            return Convert.ToInt32(productId);
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            throw new InvalidOperationException(
                $"Product code '{product.Code}' already exists.");
        }
    }
    //check if product has related records
    public async Task<bool> HasRelatedRecords(int productId)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = @"
            SELECT 
                CASE 
                    WHEN EXISTS (SELECT 1 FROM SalesDetail WHERE ProductId = @ProductId)
                    OR EXISTS (SELECT 1 FROM PurchaseDetail WHERE ProductId = @ProductId)
                    OR EXISTS (SELECT 1 FROM GRNDetail WHERE ProductId = @ProductId)
                    OR EXISTS (SELECT 1 FROM SalesReturnDetail WHERE ProductId = @ProductId)
                    OR EXISTS (SELECT 1 FROM StockTransactions WHERE ProductId = @ProductId)
                    THEN 1 
                    ELSE 0 
                END AS HasRelatedRecords
        ";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@ProductId", productId);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToBoolean(result);
    }

    // ✅ UPDATE DELETE METHOD
    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        // ✅ Check if product has related records
        var hasRelated = await HasRelatedRecords(id);

        if (hasRelated)
        {
            // ✅ Product is in use - Soft Delete (mark as inactive)
            return await SoftDeleteAsync(id);
        }

        // ✅ No related records - Hard Delete
        const string query = "DELETE FROM Products WHERE ProductId = @ProductId";
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@ProductId", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    // ✅ ADD THIS METHOD FOR SOFT DELETE
    public async Task<bool> SoftDeleteAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = @"
            UPDATE Products 
            SET IsActive = 0, UpdatedDate = @UpdatedDate 
            WHERE ProductId = @ProductId
        ";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@ProductId", id);
        command.Parameters.AddWithValue("@UpdatedDate", DateTime.UtcNow);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }





    public async Task<IEnumerable<ProductWithDetails>> GetAllAsync()
    {
        var products = new List<ProductWithDetails>();

        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("GetProductsWithDetails", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new ProductWithDetails
            {
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Code = reader.GetString(reader.GetOrdinal("Code")),

                Barcode = reader.IsDBNull(reader.GetOrdinal("Barcode"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Barcode")),

                Name = reader.GetString(reader.GetOrdinal("Name")),

                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),

                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),

                CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("CategoryName")),

                BrandId = reader.IsDBNull(reader.GetOrdinal("BrandId"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("BrandId")),

                BrandName = reader.IsDBNull(reader.GetOrdinal("BrandName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("BrandName")),

                UnitId = reader.IsDBNull(reader.GetOrdinal("UnitId"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("UnitId")),

                UnitName = reader.IsDBNull(reader.GetOrdinal("UnitName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("UnitName")),

                CostPrice = reader.GetDecimal(reader.GetOrdinal("CostPrice")),
                RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                TaxPercentage = reader.GetDecimal(reader.GetOrdinal("TaxPercentage")),
                MinimumStock = reader.GetDecimal(reader.GetOrdinal("MinimumStock")),

                // ✅ ADD THIS - CurrentStock
                CurrentStock = reader.IsDBNull(reader.GetOrdinal("CurrentStock"))
                    ? 0
                    : reader.GetDecimal(reader.GetOrdinal("CurrentStock")),

                ImageURL = reader.IsDBNull(reader.GetOrdinal("ImageURL"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ImageURL")),

                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),

                UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
            });
        }

        return products;
    }

    public async Task<ProductWithDetails?> GetByIdAsync(int id)
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("GetProductByIdWithDetails", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@ProductId", id);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new ProductWithDetails
            {
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Barcode = reader.IsDBNull(reader.GetOrdinal("Barcode")) ? null : reader.GetString(reader.GetOrdinal("Barcode")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? null : reader.GetString(reader.GetOrdinal("CategoryName")),
                BrandId = reader.IsDBNull(reader.GetOrdinal("BrandId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BrandId")),
                BrandName = reader.IsDBNull(reader.GetOrdinal("BrandName")) ? null : reader.GetString(reader.GetOrdinal("BrandName")),
                UnitId = reader.IsDBNull(reader.GetOrdinal("UnitId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("UnitId")),
                UnitName = reader.IsDBNull(reader.GetOrdinal("UnitName")) ? null : reader.GetString(reader.GetOrdinal("UnitName")),
                CostPrice = reader.GetDecimal(reader.GetOrdinal("CostPrice")),
                RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                TaxPercentage = reader.GetDecimal(reader.GetOrdinal("TaxPercentage")),
                MinimumStock = reader.GetDecimal(reader.GetOrdinal("MinimumStock")),
                CurrentStock = reader.IsDBNull(reader.GetOrdinal("CurrentStock")) ? 0 : reader.GetDecimal(reader.GetOrdinal("CurrentStock")),  // ✅ ADD THIS
                ImageURL = reader.IsDBNull(reader.GetOrdinal("ImageURL")) ? null : reader.GetString(reader.GetOrdinal("ImageURL")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
            };
        }

        return null;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        using var connection = _databaseConnection.CreateConnection();
        await connection.OpenAsync();

        const string query = """
        UPDATE Products
        SET
            Code = @Code, Barcode = @Barcode, Name = @Name, Description = @Description,
            CategoryId = @CategoryId, BrandId = @BrandId, UnitId = @UnitId,
            CostPrice = @CostPrice, RetailPrice = @RetailPrice, TaxPercentage = @TaxPercentage,
            MinimumStock = @MinimumStock, ImageURL = @ImageURL, IsActive = @IsActive,
            UpdatedDate = @UpdatedDate
        WHERE ProductId = @ProductId
        """;

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@ProductId", product.ProductId);
        command.Parameters.AddWithValue("@Code", product.Code);
        command.Parameters.AddWithValue("@Barcode", (object?)product.Barcode ?? DBNull.Value);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
        command.Parameters.AddWithValue("@BrandId", (object?)product.BrandId ?? DBNull.Value);
        command.Parameters.AddWithValue("@UnitId", product.UnitId);
        command.Parameters.AddWithValue("@CostPrice", product.CostPrice);
        command.Parameters.AddWithValue("@RetailPrice", product.RetailPrice);
        command.Parameters.AddWithValue("@TaxPercentage", product.TaxPercentage);
        command.Parameters.AddWithValue("@MinimumStock", product.MinimumStock);
        command.Parameters.AddWithValue("@ImageURL", (object?)product.ImageURL ?? DBNull.Value);
        command.Parameters.AddWithValue("@IsActive", product.IsActive);
        command.Parameters.AddWithValue("@UpdatedDate", DateTime.UtcNow);

        try
        {
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            throw new InvalidOperationException(
                $"Product code '{product.Code}' already exists.");
        }
    }
    public async Task<IEnumerable<LowStockProductDto>> GetLowStockProductsAsync()
    {
        var products = new List<LowStockProductDto>();

        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("GetLowStockProducts", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new LowStockProductDto
            {
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Barcode = reader.IsDBNull(reader.GetOrdinal("Barcode")) ? null : reader.GetString(reader.GetOrdinal("Barcode")),
                CategoryId = reader.IsDBNull(reader.GetOrdinal("CategoryId")) ? null : reader.GetInt32(reader.GetOrdinal("CategoryId")),
                CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? null : reader.GetString(reader.GetOrdinal("CategoryName")),
                BrandId = reader.IsDBNull(reader.GetOrdinal("BrandId")) ? null : reader.GetInt32(reader.GetOrdinal("BrandId")),
                BrandName = reader.IsDBNull(reader.GetOrdinal("BrandName")) ? null : reader.GetString(reader.GetOrdinal("BrandName")),
                CurrentStock = reader.GetDecimal(reader.GetOrdinal("CurrentStock")),
                MinimumStock = reader.GetDecimal(reader.GetOrdinal("MinimumStock")),
                StockStatus = reader.GetString(reader.GetOrdinal("StockStatus")),
                StatusColor = reader.GetString(reader.GetOrdinal("StatusColor"))
            });
        }

        return products;
    }

    public async Task<int> GetLowStockCountAsync()
    {
        using var connection = _databaseConnection.CreateConnection();
        using var command = new SqlCommand("GetLowStockCount", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }
}