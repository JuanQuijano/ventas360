namespace Ventas.Core.Modelos;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Nif { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Provincia { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime FechaAlta { get; set; }
}
