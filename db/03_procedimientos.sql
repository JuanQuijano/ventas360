-- Ventas360 · procedimientos almacenados
USE Ventas360;
GO

-- Pedidos de un rango de fechas. Devuelve dos conjuntos de resultados:
--   1) cabeceras de pedido con el nombre del cliente
--   2) líneas de esos pedidos con el nombre del producto
CREATE OR ALTER PROCEDURE dbo.usp_PedidosPorFecha
    @Desde date,
    @Hasta date
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.Id, p.ClienteId, c.Nombre AS NombreCliente, p.Fecha, p.Estado, p.Total
    FROM dbo.Pedidos AS p
    INNER JOIN dbo.Clientes AS c ON c.Id = p.ClienteId
    WHERE p.Fecha >= @Desde
      AND p.Fecha < DATEADD(DAY, 1, @Hasta)
    ORDER BY p.Fecha;

    SELECT l.PedidoId, l.ProductoId, pr.Nombre AS NombreProducto, l.Cantidad, l.PrecioUnitario,
           CAST(l.Cantidad * l.PrecioUnitario AS decimal(12,2)) AS Importe
    FROM dbo.LineasPedido AS l
    INNER JOIN dbo.Pedidos AS p ON p.Id = l.PedidoId
    INNER JOIN dbo.Productos AS pr ON pr.Id = l.ProductoId
    WHERE p.Fecha >= @Desde
      AND p.Fecha < DATEADD(DAY, 1, @Hasta)
    ORDER BY l.PedidoId, l.Id;
END
GO

-- Resumen mensual de ventas de un año (sin pedidos cancelados).
-- Para cada mes: número de pedidos, clientes distintos, importe, ticket medio
-- y variación respecto al mes anterior.
CREATE OR ALTER PROCEDURE dbo.usp_ResumenVentasMensual
    @Anio int
AS
BEGIN
    SET NOCOUNT ON;

    WITH Mensual AS
    (
        SELECT
            DATEFROMPARTS(YEAR(p.Fecha), MONTH(p.Fecha), 1) AS Mes,
            COUNT(*)                    AS NumPedidos,
            COUNT(DISTINCT p.ClienteId) AS NumClientes,
            SUM(p.Total)                AS Importe
        FROM dbo.Pedidos AS p
        WHERE p.Estado <> 'Cancelado'
          AND p.Fecha >= DATEFROMPARTS(@Anio - 1, 12, 1)
          AND p.Fecha <  DATEFROMPARTS(@Anio + 1, 1, 1)
        GROUP BY DATEFROMPARTS(YEAR(p.Fecha), MONTH(p.Fecha), 1)
    ),
    ConAnterior AS
    (
        SELECT
            Mes, NumPedidos, NumClientes, Importe,
            CAST(Importe / NULLIF(NumPedidos, 0) AS decimal(12,2)) AS TicketMedio,
            LAG(Importe) OVER (ORDER BY Mes) AS ImporteMesAnterior
        FROM Mensual
    )
    SELECT
        FORMAT(Mes, 'yyyy-MM') AS Mes,
        NumPedidos,
        NumClientes,
        Importe,
        TicketMedio,
        ImporteMesAnterior,
        CAST(100.0 * (Importe - ImporteMesAnterior) / NULLIF(ImporteMesAnterior, 0) AS decimal(7,2)) AS VariacionPct
    FROM ConAnterior
    WHERE YEAR(Mes) = @Anio
    ORDER BY Mes;
END
GO
