using Ventas.Core.Datos;
using Ventas.Core.Modelos;

namespace Ventas.Core.Servicios;

public class ClienteService
{
    private readonly IClienteRepositorio _repositorio;

    public ClienteService(IClienteRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<IReadOnlyList<Cliente>> ObtenerActivosAsync(CancellationToken ct = default) =>
        _repositorio.ObtenerActivosAsync(ct);

    public Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        _repositorio.ObtenerPorIdAsync(id, ct);

    public Task<int> CrearAsync(Cliente cliente, CancellationToken ct = default) =>
        _repositorio.InsertarAsync(cliente, ct);

    /// <summary>Número de clientes activos por provincia, de mayor a menor.</summary>
    public async Task<IReadOnlyList<ResumenProvincia>> ObtenerResumenPorProvinciaAsync(CancellationToken ct = default)
    {
        var clientesActivos = await _repositorio.ObtenerActivosAsync(ct);

        var resumen = clientesActivos
            .GroupBy(c => c.Provincia)
            .Select(g => new ResumenProvincia(g.Key, g.Count()))
            .OrderByDescending(r => r.NumClientes)
            .ThenBy(r => r.Provincia)
            .ToList();

        return resumen;
    }
}
