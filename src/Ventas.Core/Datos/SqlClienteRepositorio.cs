using Microsoft.Data.SqlClient;
using Ventas.Core.Modelos;

namespace Ventas.Core.Datos;

/// <summary>
/// Repositorio de clientes. Es el ejemplo de referencia del proyecto:
/// consultas parametrizadas, métodos cortos y asíncronos.
/// </summary>
public class SqlClienteRepositorio : IClienteRepositorio
{
    private const string ColumnasCliente = "Id, Nombre, Nif, Email, Provincia, Activo, FechaAlta";

    private readonly ConexionBd _conexion;

    public SqlClienteRepositorio(ConexionBd conexion)
    {
        _conexion = conexion;
    }

    public async Task<IReadOnlyList<Cliente>> ObtenerActivosAsync(CancellationToken ct = default)
    {
        const string sql = $"SELECT {ColumnasCliente} FROM dbo.Clientes WHERE Activo = 1 ORDER BY Nombre";

        await using var cn = new SqlConnection(_conexion.CadenaConexion);
        await cn.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, cn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var clientes = new List<Cliente>();
        while (await reader.ReadAsync(ct))
        {
            clientes.Add(Mapear(reader));
        }
        return clientes;
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        const string sql = $"SELECT {ColumnasCliente} FROM dbo.Clientes WHERE Id = @Id";

        await using var cn = new SqlConnection(_conexion.CadenaConexion);
        await cn.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@Id", id);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        return await reader.ReadAsync(ct) ? Mapear(reader) : null;
    }

    public async Task<bool> ExisteActivoAsync(int id, CancellationToken ct = default)
    {
        const string sql = "SELECT COUNT(1) FROM dbo.Clientes WHERE Id = @Id AND Activo = 1";

        await using var cn = new SqlConnection(_conexion.CadenaConexion);
        await cn.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@Id", id);

        var total = (int)(await cmd.ExecuteScalarAsync(ct) ?? 0);
        return total > 0;
    }

    public async Task<int> InsertarAsync(Cliente cliente, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO dbo.Clientes (Nombre, Nif, Email, Provincia, Activo)
            OUTPUT INSERTED.Id
            VALUES (@Nombre, @Nif, @Email, @Provincia, 1)
            """;

        await using var cn = new SqlConnection(_conexion.CadenaConexion);
        await cn.OpenAsync(ct);
        await using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
        cmd.Parameters.AddWithValue("@Nif", cliente.Nif);
        cmd.Parameters.AddWithValue("@Email", (object?)cliente.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Provincia", cliente.Provincia);

        return (int)(await cmd.ExecuteScalarAsync(ct))!;
    }

    private static Cliente Mapear(SqlDataReader r) => new()
    {
        Id = r.GetInt32(0),
        Nombre = r.GetString(1),
        Nif = r.GetString(2),
        Email = r.IsDBNull(3) ? null : r.GetString(3),
        Provincia = r.GetString(4),
        Activo = r.GetBoolean(5),
        FechaAlta = r.GetDateTime(6)
    };
}
