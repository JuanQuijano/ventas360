using FluentValidation;
using Ventas.Core.Modelos;
using Ventas.Core.Servicios;

namespace Ventas.Api.Clientes;

public static class ClientesEndpoints
{
    public static IEndpointRouteBuilder MapClientes(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/clientes");

        grupo.MapGet("/", async (ClienteService servicio, CancellationToken ct) =>
            Results.Ok(await servicio.ObtenerActivosAsync(ct)));

        grupo.MapGet("/{id:int}", async (int id, ClienteService servicio, CancellationToken ct) =>
            await servicio.ObtenerPorIdAsync(id, ct) is { } cliente
                ? Results.Ok(cliente)
                : Results.NotFound());

        grupo.MapGet("/resumen-provincias", async (ClienteService servicio, CancellationToken ct) =>
            Results.Ok(await servicio.ObtenerResumenPorProvinciaAsync(ct)));

        grupo.MapPost("/", async (
            CrearClienteRequest peticion,
            IValidator<CrearClienteRequest> validador,
            ClienteService servicio,
            CancellationToken ct) =>
        {
            var resultado = await validador.ValidateAsync(peticion, ct);
            if (!resultado.IsValid)
            {
                return Results.ValidationProblem(resultado.ToDictionary());
            }

            var cliente = new Cliente
            {
                Nombre = peticion.Nombre.Trim(),
                Nif = peticion.Nif.Trim().ToUpperInvariant(),
                Email = peticion.Email?.Trim(),
                Provincia = peticion.Provincia.Trim()
            };

            var id = await servicio.CrearAsync(cliente, ct);
            return Results.Created($"/clientes/{id}", new { id });
        });

        return app;
    }
}
