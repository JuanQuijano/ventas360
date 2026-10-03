using Ventas.Core.Modelos;

namespace Ventas.Tests;

public class CanalesVentaTests
{
    [Fact]
    public void PedidoNuevo_TieneCanalWebPorDefecto()
    {
        var pedido = new Pedido();

        Assert.Equal("Web", pedido.Canal);
    }

    [Fact]
    public void Todos_CoincideConLaRestriccionDeLaBaseDeDatos()
    {
        // Debe coincidir con CK_Pedidos_Canal en db/migraciones/002_canal_pedido.sql
        Assert.Equal(new[] { "Web", "Tienda", "Telefono" }, CanalesVenta.Todos);
    }
}
