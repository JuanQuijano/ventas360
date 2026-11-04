using Ventas.Api.Pedidos;
using Ventas.Tests.Dobles;

namespace Ventas.Tests;

public class PedidosValidatorTests
{
    private const int ClienteActivo = 1;
    private const int ClienteInactivo = 2;
    private const int ClienteInexistente = 99;

    private readonly PedidosValidator _validador = new(
        new ClienteRepositorioFalso()
            .ConCliente(ClienteActivo, "Asturias")
            .ConCliente(ClienteInactivo, "Madrid", activo: false));

    private static CrearPedidoRequest PeticionValida() =>
        new(ClienteActivo, [new LineaPedidoRequest(10, 2), new LineaPedidoRequest(11, 1)]);

    [Fact]
    public async Task Validar_PeticionCorrecta_EsValida()
    {
        var resultado = await _validador.ValidateAsync(PeticionValida());

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public async Task Validar_SinLineas_DevuelveError()
    {
        var resultado = await _validador.ValidateAsync(PeticionValida() with { Lineas = [] });

        Assert.Contains(resultado.Errors, e => e.PropertyName == "Lineas"
            && e.ErrorMessage == "El pedido debe tener al menos una línea.");
    }

    [Fact]
    public async Task Validar_LineasNulas_DevuelveError()
    {
        var resultado = await _validador.ValidateAsync(PeticionValida() with { Lineas = null! });

        Assert.Contains(resultado.Errors, e => e.PropertyName == "Lineas");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(999)]
    public async Task Validar_CantidadEnLimites_EsValida(int cantidad)
    {
        var peticion = PeticionValida() with { Lineas = [new LineaPedidoRequest(10, cantidad)] };

        var resultado = await _validador.ValidateAsync(peticion);

        Assert.True(resultado.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1000)]
    public async Task Validar_CantidadFueraDeRango_DevuelveError(int cantidad)
    {
        var peticion = PeticionValida() with { Lineas = [new LineaPedidoRequest(10, cantidad)] };

        var resultado = await _validador.ValidateAsync(peticion);

        Assert.Contains(resultado.Errors, e => e.PropertyName == "Lineas[0].Cantidad"
            && e.ErrorMessage == "La cantidad de cada línea debe estar entre 1 y 999.");
    }

    [Theory]
    [InlineData(ClienteInexistente)]
    [InlineData(ClienteInactivo)]
    public async Task Validar_ClienteInexistenteOInactivo_DevuelveError(int clienteId)
    {
        var resultado = await _validador.ValidateAsync(PeticionValida() with { ClienteId = clienteId });

        Assert.Contains(resultado.Errors, e => e.PropertyName == "ClienteId"
            && e.ErrorMessage == "El cliente no existe o no está activo.");
    }
}
