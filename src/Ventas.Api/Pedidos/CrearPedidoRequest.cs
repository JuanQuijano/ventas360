namespace Ventas.Api.Pedidos;

public record CrearPedidoRequest(int ClienteId, List<LineaPedidoRequest> Lineas, string? Canal = null);

public record LineaPedidoRequest(int ProductoId, int Cantidad);
