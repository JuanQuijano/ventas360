namespace Ventas.Api.Pedidos;

public record CrearPedidoRequest(int ClienteId, List<LineaPedidoRequest> Lineas);

public record LineaPedidoRequest(int ProductoId, int Cantidad);
