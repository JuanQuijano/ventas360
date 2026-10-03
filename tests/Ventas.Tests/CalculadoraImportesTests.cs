using Ventas.Core.Modelos;
using Ventas.Core.Servicios;

namespace Ventas.Tests;

public class CalculadoraImportesTests
{
    private readonly CalculadoraImportes _calculadora = new();

    [Fact]
    public void CalcularIva_BaseRedonda_Devuelve21PorCiento()
    {
        var iva = _calculadora.CalcularIva(100m);

        Assert.Equal(21.00m, iva);
    }

    [Fact]
    public void CalcularIva_ConDecimales_RedondeaAlCentimoMasProximo()
    {
        // 10,99 x 0,21 = 2,3079 -> debe redondearse a 2,31
        var iva = _calculadora.CalcularIva(10.99m);

        Assert.Equal(2.31m, iva);
    }

    [Fact]
    public void CalcularTotal_VariasLineas_SumaBaseMasIva()
    {
        var lineas = new List<LineaPedido>
        {
            new() { Cantidad = 2, PrecioUnitario = 10m },
            new() { Cantidad = 1, PrecioUnitario = 5m }
        };

        var total = _calculadora.CalcularTotal(lineas);

        Assert.Equal(30.25m, total);
    }

    [Fact]
    public void CalcularTotal_SinLineas_DevuelveCero()
    {
        var total = _calculadora.CalcularTotal(new List<LineaPedido>());

        Assert.Equal(0m, total);
    }
}
