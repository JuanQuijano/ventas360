using FluentValidation;
using Ventas.Core.Datos;

namespace Ventas.Api.Pedidos;

public class PedidosValidator : AbstractValidator<CrearPedidoRequest>
{
    public const int CantidadMinima = 1;
    public const int CantidadMaxima = 999;

    public PedidosValidator(IClienteRepositorio clientes)
    {
        RuleFor(p => p.ClienteId)
            .MustAsync((id, ct) => clientes.ExisteActivoAsync(id, ct))
            .WithMessage("El cliente no existe o no está activo.");

        RuleFor(p => p.Lineas)
            .NotEmpty().WithMessage("El pedido debe tener al menos una línea.");

        RuleForEach(p => p.Lineas).ChildRules(linea =>
        {
            linea.RuleFor(l => l.Cantidad)
                .InclusiveBetween(CantidadMinima, CantidadMaxima)
                .WithMessage($"La cantidad de cada línea debe estar entre {CantidadMinima} y {CantidadMaxima}.");
        });
    }
}
