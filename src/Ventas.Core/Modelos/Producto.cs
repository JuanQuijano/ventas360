namespace Ventas.Core.Modelos;

public class Producto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public bool Activo { get; set; } = true;
}
