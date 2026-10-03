namespace Ventas.Core.Modelos;

public class FiltroPedidos
{
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public string? TextoCliente { get; set; }
    public string? Estado { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public int? Pagina { get; set; }
    public int? TamanoPagina { get; set; }
}
