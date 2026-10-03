using Ventas.Core.Modelos;

namespace Ventas.Core.Servicios;

public interface IPedidoService
{
    List<Pedido> BuscarPedidos(FiltroPedidos filtro);
    Pedido? ObtenerPorId(int id);
    int Crear(int clienteId, List<(int ProductoId, int Cantidad)> lineas);
}
