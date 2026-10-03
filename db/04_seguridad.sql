-- Ventas360 · seguridad
-- Rol de solo lectura que usaremos en los módulos de MCP (M5 y M6):
-- puede consultar tablas y ejecutar procedimientos, pero no modificar datos.
USE Ventas360;
GO

IF DATABASE_PRINCIPAL_ID(N'ventas_lector') IS NULL
    CREATE ROLE ventas_lector;
GO

GRANT SELECT ON SCHEMA::dbo TO ventas_lector;
GRANT EXECUTE ON dbo.usp_PedidosPorFecha TO ventas_lector;
GRANT EXECUTE ON dbo.usp_ResumenVentasMensual TO ventas_lector;
DENY INSERT, UPDATE, DELETE ON SCHEMA::dbo TO ventas_lector;
GO

-- El usuario se crea en el módulo 5. Ejemplo (no se ejecuta ahora):
-- CREATE LOGIN mcp_lector WITH PASSWORD = '<contraseña segura>';
-- CREATE USER mcp_lector FOR LOGIN mcp_lector;
-- ALTER ROLE ventas_lector ADD MEMBER mcp_lector;
