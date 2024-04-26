using apbd7.Models;

namespace apbd7.Interfaces;

public interface IDbSerice
{
    Task<Product?> GetProductDetailsById(int idProduct);
    Task<Warehouse?> GetWarehouseDetailsById(int idWarehouse);
    Task<Order?> GetOrderWhereProductAndAmountMatch(int idProduct, int amount);
    Task<Product_Warehouse?> GetProduct_WarehouseByIdOrder(int idOrder);
    Task<int> UpdatedOrderToFulfilled(int idOrder);
    Task<int> AddProduct_WarehouseElement(int idProduct,float productPrice ,int idWarehouse, int idOrder, int amount);
    Task<Product_Warehouse?> GetProduct_WarehouseById(int idProductWarehouse);

}