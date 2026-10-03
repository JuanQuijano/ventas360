using System.Data;
using Microsoft.Data.SqlClient;
using Ventas.Core.Datos;
using Ventas.Core.Modelos;

namespace Ventas.Core.Servicios;

// TODO: revisar esta clase, viene del sistema antiguo de pedidos (2017)
public class PedidoService : IPedidoService
{
    private readonly ConexionBd _conexion;
    private readonly CalculadoraImportes _calculadora;

    public PedidoService(ConexionBd conexion, CalculadoraImportes calculadora)
    {
        _conexion = conexion;
        _calculadora = calculadora;
    }

    public List<Pedido> BuscarPedidos(FiltroPedidos filtro)
    {
        var resultado = new List<Pedido>();
        string sql = "SELECT p.Id, p.ClienteId, c.Nombre, p.Fecha, p.Estado, p.Total FROM Pedidos p INNER JOIN Clientes c ON c.Id = p.ClienteId WHERE 1=1";

        if (filtro.Desde != null)
        {
            sql = sql + " AND p.Fecha >= '" + filtro.Desde.Value.ToString("yyyy-MM-dd") + "'";
        }
        if (filtro.Hasta != null)
        {
            sql = sql + " AND p.Fecha < '" + filtro.Hasta.Value.AddDays(1).ToString("yyyy-MM-dd") + "'";
        }
        if (filtro.TextoCliente != null && filtro.TextoCliente != "")
        {
            sql = sql + " AND c.Nombre LIKE '%" + filtro.TextoCliente + "%'";
        }
        if (filtro.Estado != null && filtro.Estado != "")
        {
            sql = sql + " AND p.Estado = '" + filtro.Estado + "'";
        }
        if (filtro.ImporteMinimo != null)
        {
            sql = sql + " AND p.Total >= " + filtro.ImporteMinimo.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        sql = sql + " ORDER BY p.Fecha DESC";

        int pagina = 1;
        int tamano = 50;
        if (filtro.Pagina != null && filtro.Pagina > 0) pagina = filtro.Pagina.Value;
        if (filtro.TamanoPagina != null && filtro.TamanoPagina > 0 && filtro.TamanoPagina <= 500) tamano = filtro.TamanoPagina.Value;
        sql = sql + " OFFSET " + ((pagina - 1) * tamano) + " ROWS FETCH NEXT " + tamano + " ROWS ONLY";

        Console.WriteLine("SQL: " + sql);

        SqlConnection cn = new SqlConnection(_conexion.CadenaConexion);
        cn.Open();
        try
        {
            SqlCommand cmd = new SqlCommand(sql, cn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Pedido p = new Pedido();
                p.Id = Convert.ToInt32(dr["Id"]);
                p.ClienteId = Convert.ToInt32(dr["ClienteId"]);
                p.NombreCliente = dr["Nombre"].ToString()!;
                p.Fecha = Convert.ToDateTime(dr["Fecha"]);
                p.Estado = dr["Estado"].ToString()!;
                p.Total = Convert.ToDecimal(dr["Total"]);
                resultado.Add(p);
            }
            dr.Close();

            // cargar las lineas de cada pedido
            foreach (Pedido p in resultado)
            {
                SqlCommand cmdLineas = new SqlCommand("SELECT l.Id, l.PedidoId, l.ProductoId, pr.Nombre, l.Cantidad, l.PrecioUnitario FROM LineasPedido l INNER JOIN Productos pr ON pr.Id = l.ProductoId WHERE l.PedidoId = " + p.Id, cn);
                SqlDataReader drLineas = cmdLineas.ExecuteReader();
                while (drLineas.Read())
                {
                    LineaPedido l = new LineaPedido();
                    l.Id = Convert.ToInt32(drLineas["Id"]);
                    l.PedidoId = Convert.ToInt32(drLineas["PedidoId"]);
                    l.ProductoId = Convert.ToInt32(drLineas["ProductoId"]);
                    l.NombreProducto = drLineas["Nombre"].ToString()!;
                    l.Cantidad = Convert.ToInt32(drLineas["Cantidad"]);
                    l.PrecioUnitario = Convert.ToDecimal(drLineas["PrecioUnitario"]);
                    p.Lineas.Add(l);
                }
                drLineas.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error buscando pedidos: " + ex.Message);
            throw;
        }
        finally
        {
            cn.Close();
        }

        return resultado;
    }

    public Pedido? ObtenerPorId(int id)
    {
        var lista = BuscarPedidosPorId(id);
        if (lista.Count == 0) return null;
        return lista[0];
    }

    private List<Pedido> BuscarPedidosPorId(int id)
    {
        var resultado = new List<Pedido>();
        SqlConnection cn = new SqlConnection(_conexion.CadenaConexion);
        cn.Open();
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT p.Id, p.ClienteId, c.Nombre, p.Fecha, p.Estado, p.Total FROM Pedidos p INNER JOIN Clientes c ON c.Id = p.ClienteId WHERE p.Id = " + id, cn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Pedido p = new Pedido();
                p.Id = Convert.ToInt32(dr["Id"]);
                p.ClienteId = Convert.ToInt32(dr["ClienteId"]);
                p.NombreCliente = dr["Nombre"].ToString()!;
                p.Fecha = Convert.ToDateTime(dr["Fecha"]);
                p.Estado = dr["Estado"].ToString()!;
                p.Total = Convert.ToDecimal(dr["Total"]);
                resultado.Add(p);
            }
            dr.Close();

            foreach (Pedido p in resultado)
            {
                SqlCommand cmdLineas = new SqlCommand("SELECT l.Id, l.PedidoId, l.ProductoId, pr.Nombre, l.Cantidad, l.PrecioUnitario FROM LineasPedido l INNER JOIN Productos pr ON pr.Id = l.ProductoId WHERE l.PedidoId = " + p.Id, cn);
                SqlDataReader drLineas = cmdLineas.ExecuteReader();
                while (drLineas.Read())
                {
                    LineaPedido l = new LineaPedido();
                    l.Id = Convert.ToInt32(drLineas["Id"]);
                    l.PedidoId = Convert.ToInt32(drLineas["PedidoId"]);
                    l.ProductoId = Convert.ToInt32(drLineas["ProductoId"]);
                    l.NombreProducto = drLineas["Nombre"].ToString()!;
                    l.Cantidad = Convert.ToInt32(drLineas["Cantidad"]);
                    l.PrecioUnitario = Convert.ToDecimal(drLineas["PrecioUnitario"]);
                    p.Lineas.Add(l);
                }
                drLineas.Close();
            }
        }
        finally
        {
            cn.Close();
        }
        return resultado;
    }

    public int Crear(int clienteId, List<(int ProductoId, int Cantidad)> lineas)
    {
        SqlConnection cn = new SqlConnection(_conexion.CadenaConexion);
        cn.Open();
        try
        {
            // obtener precios
            var lineasPedido = new List<LineaPedido>();
            foreach (var linea in lineas)
            {
                SqlCommand cmdPrecio = new SqlCommand("SELECT Nombre, PrecioUnitario FROM Productos WHERE Id = " + linea.ProductoId, cn);
                SqlDataReader dr = cmdPrecio.ExecuteReader();
                if (dr.Read())
                {
                    lineasPedido.Add(new LineaPedido
                    {
                        ProductoId = linea.ProductoId,
                        NombreProducto = dr["Nombre"].ToString()!,
                        Cantidad = linea.Cantidad,
                        PrecioUnitario = Convert.ToDecimal(dr["PrecioUnitario"])
                    });
                }
                dr.Close();
            }

            decimal total = _calculadora.CalcularTotal(lineasPedido);

            SqlCommand cmdPedido = new SqlCommand("INSERT INTO Pedidos (ClienteId, Fecha, Estado, Total) OUTPUT INSERTED.Id VALUES (" + clienteId + ", SYSDATETIME(), 'Pendiente', " + total.ToString(System.Globalization.CultureInfo.InvariantCulture) + ")", cn);
            int idPedido = (int)cmdPedido.ExecuteScalar();

            foreach (var l in lineasPedido)
            {
                SqlCommand cmdLinea = new SqlCommand("INSERT INTO LineasPedido (PedidoId, ProductoId, Cantidad, PrecioUnitario) VALUES (" + idPedido + ", " + l.ProductoId + ", " + l.Cantidad + ", " + l.PrecioUnitario.ToString(System.Globalization.CultureInfo.InvariantCulture) + ")", cn);
                cmdLinea.ExecuteNonQuery();
            }

            Console.WriteLine("Pedido creado: " + idPedido);
            return idPedido;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creando pedido: " + ex.Message);
            throw;
        }
        finally
        {
            if (cn.State == ConnectionState.Open) cn.Close();
        }
    }
}
