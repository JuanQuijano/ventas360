using FluentValidation;
using Ventas.Core.Datos;
using Ventas.Core.Modelos;

namespace Ventas.Api.Pedidos;

public class CrearPedidoValidator : AbstractValidator<CrearPedidoRequest>
{
    public const int CantidadMinima = 1;
    public const int CantidadMaxima = 999;

    public CrearPedidoValidator(IClienteRepositorio clientes)
    {
        RuleFor(p => p.ClienteId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0).WithMessage("El cliente es obligatorio.")
            .MustAsync((id, ct) => clientes.ExisteActivoAsync(id, ct))
                .WithMessage("El cliente no existe o no está activo.");

        RuleFor(p => p.Lineas)
            .NotEmpty().WithMessage("El pedido debe tener al menos una línea.");

        RuleFor(p => p.Canal)
            .Must(CanalesVenta.EsValido)
            .WithMessage($"El canal debe ser uno de estos valores: {string.Join(", ", CanalesVenta.Todos)}.")
            .When(p => p.Canal is not null);

        RuleForEach(p => p.Lineas).ChildRules(linea =>
        {
            linea.RuleFor(l => l.ProductoId)
                .GreaterThan(0).WithMessage("Cada línea debe indicar un producto.");

            linea.RuleFor(l => l.Cantidad)
                .InclusiveBetween(CantidadMinima, CantidadMaxima)
                .WithMessage($"La cantidad debe estar entre {CantidadMinima} y {CantidadMaxima}.");
        });
    }
}
