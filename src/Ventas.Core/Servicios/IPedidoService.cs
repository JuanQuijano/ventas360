using Ventas.Core.Modelos;

namespace Ventas.Core.Servicios;

public interface IPedidoService
{
    List<Pedido> BuscarPedidos(FiltroPedidos filtro);
    Pedido? ObtenerPorId(int id);
    int Crear(int clienteId, string canal, List<(int ProductoId, int Cantidad)> lineas);
}
