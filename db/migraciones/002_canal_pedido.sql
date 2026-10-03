-- 002 · Canal de venta del pedido (práctica E1.3)
-- Valores permitidos: Web, Tienda, Telefono. Los pedidos existentes quedan como 'Web'.
USE Ventas360;
GO

IF COL_LENGTH(N'dbo.Pedidos', N'Canal') IS NULL
BEGIN
    ALTER TABLE dbo.Pedidos
        ADD Canal varchar(20) NOT NULL
            CONSTRAINT DF_Pedidos_Canal DEFAULT ('Web')
            CONSTRAINT CK_Pedidos_Canal CHECK (Canal IN ('Web', 'Tienda', 'Telefono'));
END
GO

-- El procedimiento devuelve también el canal en el conjunto de cabeceras
CREATE OR ALTER PROCEDURE dbo.usp_PedidosPorFecha
    @Desde date,
    @Hasta date
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.Id, p.ClienteId, c.Nombre AS NombreCliente, p.Fecha, p.Estado, p.Canal, p.Total
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
