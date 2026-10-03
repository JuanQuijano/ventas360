namespace Ventas.Core.Modelos;

public class LineaPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    public decimal Importe => Cantidad * PrecioUnitario;
}
