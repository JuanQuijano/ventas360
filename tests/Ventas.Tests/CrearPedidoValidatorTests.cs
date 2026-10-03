using Ventas.Api.Pedidos;
using Ventas.Tests.Dobles;

namespace Ventas.Tests;

public class CrearPedidoValidatorTests
{
    private const int ClienteActivo = 1;
    private const int ClienteInactivo = 2;

    private readonly CrearPedidoValidator _validador;

    public CrearPedidoValidatorTests()
    {
        var clientes = new ClienteRepositorioFalso()
            .ConCliente(ClienteActivo, "Madrid")
            .ConCliente(ClienteInactivo, "Madrid", activo: false);
        _validador = new CrearPedidoValidator(clientes);
    }

    private static CrearPedidoRequest PeticionValida() =>
        new(ClienteActivo, new List<LineaPedidoRequest> { new(ProductoId: 1, Cantidad: 2) });

    [Fact]
    public async Task Validar_PeticionCorrecta_EsValida()
    {
        var resultado = await _validador.ValidateAsync(PeticionValida());

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public async Task Validar_SinLineas_DevuelveError()
    {
        var peticion = PeticionValida() with { Lineas = new List<LineaPedidoRequest>() };

        var resultado = await _validador.ValidateAsync(peticion);

        Assert.Contains(resultado.Errors, e => e.ErrorMessage == "El pedido debe tener al menos una línea.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    [InlineData(1000)]
    public async Task Validar_CantidadFueraDeRango_DevuelveError(int cantidad)
    {
        var peticion = PeticionValida() with { Lineas = new List<LineaPedidoRequest> { new(1, cantidad) } };

        var resultado = await _validador.ValidateAsync(peticion);

        Assert.Contains(resultado.Errors, e => e.ErrorMessage == "La cantidad debe estar entre 1 y 999.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(999)]
    public async Task Validar_CantidadEnLosLimites_EsValida(int cantidad)
    {
        var peticion = PeticionValida() with { Lineas = new List<LineaPedidoRequest> { new(1, cantidad) } };

        var resultado = await _validador.ValidateAsync(peticion);

        Assert.True(resultado.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Web")]
    [InlineData("Tienda")]
    [InlineData("Telefono")]
    public async Task Validar_CanalPermitidoOSinCanal_EsValida(string? canal)
    {
        var resultado = await _validador.ValidateAsync(PeticionValida() with { Canal = canal });

        Assert.True(resultado.IsValid);
    }

    [Theory]
    [InlineData("Fax")]
    [InlineData("web")]
    [InlineData("")]
    public async Task Validar_CanalNoPermitido_DevuelveError(string canal)
    {
        var resultado = await _validador.ValidateAsync(PeticionValida() with { Canal = canal });

        Assert.Contains(resultado.Errors, e => e.PropertyName == "Canal");
    }

    [Fact]
    public async Task Validar_ClienteInactivo_DevuelveError()
    {
        var resultado = await _validador.ValidateAsync(PeticionValida() with { ClienteId = ClienteInactivo });

        Assert.Contains(resultado.Errors, e => e.ErrorMessage == "El cliente no existe o no está activo.");
    }

    [Fact]
    public async Task Validar_ClienteInexistente_DevuelveError()
    {
        var resultado = await _validador.ValidateAsync(PeticionValida() with { ClienteId = 999 });

        Assert.Contains(resultado.Errors, e => e.ErrorMessage == "El cliente no existe o no está activo.");
    }
}
