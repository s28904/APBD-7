using System.Data;
using System.Data.SqlClient;
using apbd7.Interfaces;
using apbd7.Models;
using Dapper;

namespace apbd7.Services;




public class DbService(IConfiguration configuration) : IDbSerice
{
    private async Task<SqlConnection> GetConnection()
    {
        var connection = new SqlConnection(configuration.GetConnectionString("Default"));
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return connection;
    }

    public async Task<Product?> GetProductDetailsById(int idProduct)
    {
        await using var connection = await GetConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Product>("select * from Product where IdProduct = @IdProduct", new {IdProduct = idProduct});
        return result;
    }

    public async Task<Warehouse?> GetWarehouseDetailsById(int idWarehouse)
    {
        await using var connection = await GetConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Warehouse>("select * from Warehouse where IdWarehouse = @IdWarehouse", new {IdWarehouse = idWarehouse});
        return result;
    }

    public async Task<Order?> GetOrderWhereProductAndAmountMatch(int idProduct, int amount)
    {
        await using var connection = await GetConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Order>("select * from [Order] where IdProduct = @IdProduct AND Amount = @Amount", new {IdProduct = idProduct, Amount = amount});
        return result;
    }

    public async Task<Product_Warehouse?> GetProduct_WarehouseByIdOrder(int idOrder)
    {
        await using var connection = await GetConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Product_Warehouse>("select * from Product_Warehouse WHERE IdOrder = @IdOrder", new {IdOrder = idOrder});
        return result;
        
    }

    public async Task<int> UpdatedOrderToFulfilled(int idOrder)
    {
     await using var connection = await GetConnection();
     DateTime currentTime = DateTime.Now;
     var affectedRows = await connection.ExecuteAsync("UPDATE [Order] SET FulfilledAt = @CurrentTime WHERE IdOrder=@IdOrder", new
     {
         CurrentTime = currentTime,
         IdOrder=idOrder
     });

     return affectedRows;
    }


    public async Task<Product_Warehouse?> GetProduct_WarehouseById(int idProductWarehouse)
    {
        await using var connection = await GetConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Product_Warehouse>("select * from Product_Warehouse WHERE IdProductWarehouse = @IdProductWarehouse", new {IdProductWarehouse = idProductWarehouse});
        return result;
    }

    public async Task<int> AddProduct_WarehouseElement(int idProduct,float productPrice ,int idWarehouse, int idOrder,
        int amount)
    {
        await using var connection = await GetConnection();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            
            DateTime currentTime = DateTime.Now;
            var affectedRows = await connection.ExecuteAsync("UPDATE [Order] SET FulfilledAt = @CurrentTime WHERE IdOrder=@IdOrder", new
            {
                CurrentTime = currentTime,
                IdOrder=idOrder
            },transaction : transaction);

            var productWarehousePrice = productPrice * amount;

            var productWarehouseId =
                await connection.ExecuteScalarAsync<int>(
                    "INSERT INTO Product_Warehouse VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt);SELECT CAST(SCOPE_IDENTITY() as int)",
                    new
                    {
                        IdWarehouse = @idWarehouse,
                        IdProduct = @idProduct,
                        IdOrder = @idOrder,
                        Amount = @amount,
                        Price = @productWarehousePrice,
                        CreatedAt = @currentTime
                    }, transaction : transaction);

            await transaction.CommitAsync();
            return productWarehouseId;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


}