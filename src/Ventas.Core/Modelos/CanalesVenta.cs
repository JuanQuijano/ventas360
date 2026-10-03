namespace Ventas.Core.Modelos;

/// <summary>Canales de venta admitidos. Deben coincidir con CK_Pedidos_Canal.</summary>
public static class CanalesVenta
{
    public const string Web = "Web";
    public const string Tienda = "Tienda";
    public const string Telefono = "Telefono";

    public static readonly IReadOnlyList<string> Todos = new[] { Web, Tienda, Telefono };

    public static bool EsValido(string? canal) => canal is not null && Todos.Contains(canal);
}
