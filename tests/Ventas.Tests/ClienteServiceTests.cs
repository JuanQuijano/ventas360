using Ventas.Core.Servicios;
using Ventas.Tests.Dobles;

namespace Ventas.Tests;

public class ClienteServiceTests
{
    [Fact]
    public async Task ObtenerResumenPorProvincia_AgrupaYOrdenaDeMayorAMenor()
    {
        var repositorio = new ClienteRepositorioFalso()
            .ConCliente(1, "Madrid")
            .ConCliente(2, "Barcelona")
            .ConCliente(3, "Madrid")
            .ConCliente(4, "Valencia")
            .ConCliente(5, "Madrid")
            .ConCliente(6, "Barcelona");
        var servicio = new ClienteService(repositorio);

        var resumen = await servicio.ObtenerResumenPorProvinciaAsync();

        Assert.Equal(3, resumen.Count);
        Assert.Equal("Madrid", resumen[0].Provincia);
        Assert.Equal(3, resumen[0].NumClientes);
        Assert.Equal("Barcelona", resumen[1].Provincia);
        Assert.Equal("Valencia", resumen[2].Provincia);
    }

    [Fact]
    public async Task ObtenerResumenPorProvincia_IgnoraClientesInactivos()
    {
        var repositorio = new ClienteRepositorioFalso()
            .ConCliente(1, "Sevilla")
            .ConCliente(2, "Sevilla", activo: false);
        var servicio = new ClienteService(repositorio);

        var resumen = await servicio.ObtenerResumenPorProvinciaAsync();

        Assert.Single(resumen);
        Assert.Equal(1, resumen[0].NumClientes);
    }
}
