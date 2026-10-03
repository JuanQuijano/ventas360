namespace Ventas.Core.Modelos;

public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public decimal Total { get; set; }
    public List<LineaPedido> Lineas { get; set; } = new();
}
