using Ventas.Core.Modelos;

namespace Ventas.Core.Datos;

public interface IClienteRepositorio
{
    Task<IReadOnlyList<Cliente>> ObtenerActivosAsync(CancellationToken ct = default);
    Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteActivoAsync(int id, CancellationToken ct = default);
    Task<int> InsertarAsync(Cliente cliente, CancellationToken ct = default);
}
