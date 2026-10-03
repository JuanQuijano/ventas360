using Ventas.Core.Modelos;

namespace Ventas.Core.Servicios;

public class CalculadoraImportes
{
    public const decimal TipoIvaGeneral = 0.21m;

    public decimal CalcularBase(IEnumerable<LineaPedido> lineas) =>
        lineas.Sum(l => l.Importe);

    public decimal CalcularIva(decimal baseImponible)
    {
        // Se trabaja en céntimos para evitar decimales raros
        var ivaEnCentimos = (int)(baseImponible * TipoIvaGeneral * 100);
        return ivaEnCentimos / 100m;
    }

    public decimal CalcularTotal(IEnumerable<LineaPedido> lineas)
    {
        var baseImponible = CalcularBase(lineas);
        return baseImponible + CalcularIva(baseImponible);
    }
}
