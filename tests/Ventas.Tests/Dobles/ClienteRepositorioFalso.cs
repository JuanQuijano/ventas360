using Ventas.Core.Datos;
using Ventas.Core.Modelos;

namespace Ventas.Tests.Dobles;

/// <summary>Repositorio en memoria para tests, sin base de datos.</summary>
public class ClienteRepositorioFalso : IClienteRepositorio
{
    private readonly List<Cliente> _clientes = new();

    public ClienteRepositorioFalso ConCliente(int id, string provincia, bool activo = true)
    {
        _clientes.Add(new Cliente
        {
            Id = id,
            Nombre = $"Cliente {id}",
            Nif = $"{id:D8}A",
            Provincia = provincia,
            Activo = activo,
            FechaAlta = new DateTime(2026, 1, 1)
        });
        return this;
    }

    public Task<IReadOnlyList<Cliente>> ObtenerActivosAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Cliente>>(_clientes.Where(c => c.Activo).ToList());

    public Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_clientes.FirstOrDefault(c => c.Id == id));

    public Task<bool> ExisteActivoAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_clientes.Any(c => c.Id == id && c.Activo));

    public Task<int> InsertarAsync(Cliente cliente, CancellationToken ct = default)
    {
        cliente.Id = _clientes.Count == 0 ? 1 : _clientes.Max(c => c.Id) + 1;
        _clientes.Add(cliente);
        return Task.FromResult(cliente.Id);
    }
}
