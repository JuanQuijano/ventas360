using FluentValidation;
using Ventas.Core.Modelos;
using Ventas.Core.Servicios;

namespace Ventas.Api.Pedidos;

public static class PedidosEndpoints
{
    public static IEndpointRouteBuilder MapPedidos(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/pedidos");

        // GET /pedidos?desde=2026-01-01&hasta=2026-03-31&textoCliente=norte&estado=Enviado&pagina=1
        grupo.MapGet("/", ([AsParameters] FiltroPedidos filtro, IPedidoService servicio) =>
            Results.Ok(servicio.BuscarPedidos(filtro)));

        grupo.MapGet("/{id:int}", (int id, IPedidoService servicio) =>
            servicio.ObtenerPorId(id) is { } pedido
                ? Results.Ok(pedido)
                : Results.NotFound());

        grupo.MapPost("/", async (
            CrearPedidoRequest peticion,
            IValidator<CrearPedidoRequest> validador,
            IPedidoService servicio,
            CancellationToken ct) =>
        {
            var resultado = await validador.ValidateAsync(peticion, ct);
            if (!resultado.IsValid)
            {
                return Results.ValidationProblem(resultado.ToDictionary());
            }

            var lineas = peticion.Lineas
                .Select(l => (l.ProductoId, l.Cantidad))
                .ToList();

            var id = servicio.Crear(peticion.ClienteId, lineas);
            return Results.Created($"/pedidos/{id}", new { id });
        });

        return app;
    }
}
