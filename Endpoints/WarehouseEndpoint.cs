using apbd7.DTOs;
using apbd7.Interfaces;
using apbd7.Validators;
using FluentValidation;

namespace apbd7.Endpoints;

public static class WarehouseEndpoint
{
    public static void RegisterWarehouseEndpoints(this WebApplication app)
    {
        app.MapPost("api/WarehouseAddProduct", AddWareHouseProduct);
    }

    public static async Task<IResult> AddWareHouseProduct(
        WarehouseAddProduct request, IDbSerice db, IValidator<WarehouseAddProduct> validator
    )
    {
        var validate = await validator.ValidateAsync(request);
        if (!validate.IsValid)
        {
            return Results.ValidationProblem(validate.ToDictionary());
        }

        var product = await db.GetProductDetailsById(request.IdProduct);

        if (product is null) return Results.NotFound($"Product of id {request.IdProduct} could not be found");

        var warehouse = await  db.GetWarehouseDetailsById(request.IdWarehouse);

        if (warehouse is null) return Results.NotFound($"Warehouse of id {request.IdWarehouse} could not be found");

        var order = await db.GetOrderWhereProductAndAmountMatch(request.IdProduct, request.Amount);
        
        if (order is null) return Results.NotFound("Order could not be found");
        
        if (order.CreatedAt > request.CreatedAt) return Results.Conflict($" Order was created after request");

        var productWarehouseElementById = await db.GetProduct_WarehouseByIdOrder(order.IdOrder);
        
        if (productWarehouseElementById is not null) return Results.Conflict($"Order of id {order.IdOrder} has already been placed in that warehouse");

        var addedProductWarehouseElementId = await db.AddProduct_WarehouseElement(request.IdProduct, product.Price,
            request.IdWarehouse, order.IdOrder, request.Amount);
        
        return Results.Created("",addedProductWarehouseElementId);
    }
}