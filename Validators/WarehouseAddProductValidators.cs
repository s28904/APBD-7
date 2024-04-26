using apbd7.DTOs;
using FluentValidation;

namespace apbd7.Validators;

public class WarehouseAddProductValidator : AbstractValidator<WarehouseAddProduct>
{
    public WarehouseAddProductValidator()
    {
        RuleFor(e => e.IdProduct).NotNull().NotEmpty();
        RuleFor(e => e.IdWarehouse).NotNull().NotEmpty();
        RuleFor(e => e.Amount).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(e => e.CreatedAt).NotNull().NotEmpty().LessThanOrEqualTo(DateTime.Now);
    }
}