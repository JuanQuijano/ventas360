-- Ventas360 · esquema
USE Ventas360;
GO

CREATE TABLE dbo.Clientes
(
    Id          int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Clientes PRIMARY KEY,
    Nombre      nvarchar(150)     NOT NULL,
    Nif         varchar(15)       NOT NULL CONSTRAINT UQ_Clientes_Nif UNIQUE,
    Email       nvarchar(200)     NULL,
    Provincia   nvarchar(50)      NOT NULL,
    Activo      bit               NOT NULL CONSTRAINT DF_Clientes_Activo DEFAULT (1),
    FechaAlta   datetime2(0)      NOT NULL CONSTRAINT DF_Clientes_FechaAlta DEFAULT (SYSDATETIME())
);
GO

CREATE TABLE dbo.Productos
(
    Id              int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Productos PRIMARY KEY,
    Codigo          varchar(20)       NOT NULL CONSTRAINT UQ_Productos_Codigo UNIQUE,
    Nombre          nvarchar(150)     NOT NULL,
    PrecioUnitario  decimal(10,2)     NOT NULL CONSTRAINT CK_Productos_Precio CHECK (PrecioUnitario >= 0),
    Activo          bit               NOT NULL CONSTRAINT DF_Productos_Activo DEFAULT (1)
);
GO

CREATE TABLE dbo.Pedidos
(
    Id          int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Pedidos PRIMARY KEY,
    ClienteId   int               NOT NULL CONSTRAINT FK_Pedidos_Clientes REFERENCES dbo.Clientes (Id),
    Fecha       datetime2(0)      NOT NULL CONSTRAINT DF_Pedidos_Fecha DEFAULT (SYSDATETIME()),
    Estado      varchar(20)       NOT NULL CONSTRAINT DF_Pedidos_Estado DEFAULT ('Pendiente')
                                  CONSTRAINT CK_Pedidos_Estado CHECK (Estado IN ('Pendiente', 'Enviado', 'Entregado', 'Cancelado')),
    Total       decimal(12,2)     NOT NULL CONSTRAINT DF_Pedidos_Total DEFAULT (0)
);
GO

CREATE INDEX IX_Pedidos_Fecha ON dbo.Pedidos (Fecha) INCLUDE (ClienteId, Estado, Total);
CREATE INDEX IX_Pedidos_ClienteId ON dbo.Pedidos (ClienteId);
GO

CREATE TABLE dbo.LineasPedido
(
    Id              int IDENTITY(1,1) NOT NULL CONSTRAINT PK_LineasPedido PRIMARY KEY,
    PedidoId        int               NOT NULL CONSTRAINT FK_LineasPedido_Pedidos REFERENCES dbo.Pedidos (Id),
    ProductoId      int               NOT NULL CONSTRAINT FK_LineasPedido_Productos REFERENCES dbo.Productos (Id),
    Cantidad        int               NOT NULL CONSTRAINT CK_LineasPedido_Cantidad CHECK (Cantidad > 0),
    PrecioUnitario  decimal(10,2)     NOT NULL
);
GO

CREATE INDEX IX_LineasPedido_PedidoId ON dbo.LineasPedido (PedidoId);
GO
