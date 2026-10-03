-- Ventas360 · creación de la base de datos (entorno local del curso)
-- Ejecutar conectado a master. Si la base de datos existe, se elimina y se crea de nuevo.
USE master;
GO
IF DB_ID(N'Ventas360') IS NOT NULL
BEGIN
    ALTER DATABASE Ventas360 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE Ventas360;
END
GO
CREATE DATABASE Ventas360;
GO
