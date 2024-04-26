using System.ComponentModel.DataAnnotations;

namespace apbd7.DTOs;

public record WarehouseAddProduct
    (
    [Required] int IdProduct,
    [Required] int IdWarehouse,
    [Required] int Amount,
    [Required] DateTime CreatedAt
    );