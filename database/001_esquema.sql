/*
    FARMASIL_APP
    Issue #70 - Script de creación del esquema
    Base de datos: DB_FARMASIL

    Este archivo crea:
    - 16 tablas
    - claves primarias y foráneas
    - restricciones CHECK y UNIQUE
    - índices
    - 2 vistas de stock

    La creación/eliminación de la base se realiza desde recrear.ps1.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* =========================================================
   1. TBL_USUARIOS
   ========================================================= */
CREATE TABLE dbo.TBL_USUARIOS
(
    id_usuario INT IDENTITY(1,1) NOT NULL,
    nombre_usuario NVARCHAR(50) NOT NULL,
    contrasena NVARCHAR(255) NOT NULL,
    rol NVARCHAR(20) NOT NULL,
    estado NVARCHAR(10) NOT NULL
        CONSTRAINT DF_TBL_USUARIOS_estado DEFAULT 'Activo',

    CONSTRAINT PK_TBL_USUARIOS
        PRIMARY KEY (id_usuario),

    CONSTRAINT UQ_TBL_USUARIOS_nombre_usuario
        UNIQUE (nombre_usuario),

    CONSTRAINT CK_TBL_USUARIOS_rol
        CHECK (rol IN ('Administrador', 'Tecnico')),

    CONSTRAINT CK_TBL_USUARIOS_estado
        CHECK (estado IN ('Activo', 'Inactivo'))
);
GO


/* =========================================================
   2. TBL_PROVEEDORES
   ========================================================= */
CREATE TABLE dbo.TBL_PROVEEDORES
(
    id_proveedor INT IDENTITY(1,1) NOT NULL,
    ruc NVARCHAR(11) NOT NULL,
    razon_social NVARCHAR(150) NOT NULL,
    telefono NVARCHAR(15) NULL,
    estado NVARCHAR(10) NOT NULL
        CONSTRAINT DF_TBL_PROVEEDORES_estado DEFAULT 'Activo',

    CONSTRAINT PK_TBL_PROVEEDORES
        PRIMARY KEY (id_proveedor),

    CONSTRAINT UQ_TBL_PROVEEDORES_ruc
        UNIQUE (ruc),

    CONSTRAINT CK_TBL_PROVEEDORES_ruc
        CHECK (LEN(ruc) = 11 AND ruc NOT LIKE '%[^0-9]%'),

    CONSTRAINT CK_TBL_PROVEEDORES_estado
        CHECK (estado IN ('Activo', 'Inactivo'))
);
GO


/* =========================================================
   3. TBL_METODOS_PAGO
   ========================================================= */
CREATE TABLE dbo.TBL_METODOS_PAGO
(
    id_metodo_pago INT IDENTITY(1,1) NOT NULL,
    nombre_metodo NVARCHAR(50) NOT NULL,
    estado NVARCHAR(10) NOT NULL
        CONSTRAINT DF_TBL_METODOS_PAGO_estado DEFAULT 'Activo',

    CONSTRAINT PK_TBL_METODOS_PAGO
        PRIMARY KEY (id_metodo_pago),

    CONSTRAINT UQ_TBL_METODOS_PAGO_nombre_metodo
        UNIQUE (nombre_metodo),

    CONSTRAINT CK_TBL_METODOS_PAGO_estado
        CHECK (estado IN ('Activo', 'Inactivo'))
);
GO


/* =========================================================
   4. TBL_PRODUCTOS
   ========================================================= */
CREATE TABLE dbo.TBL_PRODUCTOS
(
    id_producto INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(100) NOT NULL,
    accion_terapeutica NVARCHAR(150) NOT NULL,
    precio_venta DECIMAL(10,2) NOT NULL,
    estado_producto NVARCHAR(25) NOT NULL
        CONSTRAINT DF_TBL_PRODUCTOS_estado_producto DEFAULT 'Disponible',
    id_proveedor INT NOT NULL,
    stock_minimo INT NOT NULL
        CONSTRAINT DF_TBL_PRODUCTOS_stock_minimo DEFAULT 0,

    CONSTRAINT PK_TBL_PRODUCTOS
        PRIMARY KEY (id_producto),

    CONSTRAINT CK_TBL_PRODUCTOS_precio_venta
        CHECK (precio_venta >= 0),

    CONSTRAINT CK_TBL_PRODUCTOS_estado_producto
        CHECK (estado_producto IN ('Disponible', 'Descontinuado')),

    CONSTRAINT CK_TBL_PRODUCTOS_stock_minimo
        CHECK (stock_minimo >= 0),

    CONSTRAINT FK_TBL_PRODUCTOS_TBL_PROVEEDORES
        FOREIGN KEY (id_proveedor)
        REFERENCES dbo.TBL_PROVEEDORES(id_proveedor)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO


/* =========================================================
   5. TBL_LOTES
   ========================================================= */
CREATE TABLE dbo.TBL_LOTES
(
    id_lote INT IDENTITY(1,1) NOT NULL,
    id_producto INT NOT NULL,
    numero_lote NVARCHAR(30) NOT NULL,
    fecha_vencimiento DATE NOT NULL,
    stock_actual INT NOT NULL,
    estado_lote NVARCHAR(25) NOT NULL
        CONSTRAINT DF_TBL_LOTES_estado_lote DEFAULT 'Disponible',
    fecha_ingreso DATE NOT NULL,
    id_proveedor INT NOT NULL,

    CONSTRAINT PK_TBL_LOTES
        PRIMARY KEY (id_lote),

    CONSTRAINT FK_TBL_LOTES_TBL_PRODUCTOS
        FOREIGN KEY (id_producto)
        REFERENCES dbo.TBL_PRODUCTOS(id_producto)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_TBL_LOTES_TBL_PROVEEDORES
        FOREIGN KEY (id_proveedor)
        REFERENCES dbo.TBL_PROVEEDORES(id_proveedor)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT CK_TBL_LOTES_stock_actual
        CHECK (stock_actual >= 0),

    CONSTRAINT CK_TBL_LOTES_estado_lote
        CHECK (
            estado_lote IN
            ('Disponible', 'Bloqueado por devolucion', 'Agotado')
        ),

    CONSTRAINT CK_TBL_LOTES_estado_stock
        CHECK (
            estado_lote <> 'Disponible'
            OR stock_actual > 0
        ),

    CONSTRAINT UQ_TBL_LOTES_id_producto_numero_lote
        UNIQUE (id_producto, numero_lote)
);
GO


/* =========================================================
   6. TBL_ALERTAS_VENCIMIENTO
   ========================================================= */
CREATE TABLE dbo.TBL_ALERTAS_VENCIMIENTO
(
    id_alerta INT IDENTITY(1,1) NOT NULL,
    descripcion NVARCHAR(150) NOT NULL,
    umbral_meses INT NOT NULL
        CONSTRAINT DF_TBL_ALERTAS_VENCIMIENTO_umbral_meses DEFAULT 2,
    estado NVARCHAR(10) NOT NULL
        CONSTRAINT DF_TBL_ALERTAS_VENCIMIENTO_estado DEFAULT 'Activo',
    fecha_ultima_verif DATETIME2(0) NULL,

    CONSTRAINT PK_TBL_ALERTAS_VENCIMIENTO
        PRIMARY KEY (id_alerta),

    CONSTRAINT CK_TBL_ALERTAS_VENCIMIENTO_umbral_meses
        CHECK (umbral_meses > 0),

    CONSTRAINT CK_TBL_ALERTAS_VENCIMIENTO_estado
        CHECK (estado IN ('Activo', 'Inactivo'))
);
GO


/* =========================================================
   7. TBL_REPORTES_VENTAS
   ========================================================= */
CREATE TABLE dbo.TBL_REPORTES_VENTAS
(
    id_reporte INT IDENTITY(1,1) NOT NULL,
    tipo_reporte NVARCHAR(50) NOT NULL,
    formato NVARCHAR(10) NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    total_recaudado DECIMAL(10,2) NOT NULL,
    fecha_generacion DATETIME2(0) NOT NULL,
    ruta_archivo NVARCHAR(255) NOT NULL,

    CONSTRAINT PK_TBL_REPORTES_VENTAS
        PRIMARY KEY (id_reporte),

    CONSTRAINT CK_TBL_REPORTES_VENTAS_formato
        CHECK (formato IN ('PDF', 'Excel', 'HTML')),

    CONSTRAINT CK_TBL_REPORTES_VENTAS_fechas
        CHECK (fecha_fin >= fecha_inicio),

    CONSTRAINT CK_TBL_REPORTES_VENTAS_total_recaudado
        CHECK (total_recaudado >= 0)
);
GO


/* =========================================================
   8. TBL_REGISTRO_VENTAS
   ========================================================= */
CREATE TABLE dbo.TBL_REGISTRO_VENTAS
(
    id_venta INT IDENTITY(1,1) NOT NULL,
    fecha_hora DATETIME2(0) NOT NULL,
    monto_total DECIMAL(10,2) NOT NULL,
    id_metodo_pago INT NOT NULL,
    id_usuario INT NOT NULL,

    CONSTRAINT PK_TBL_REGISTRO_VENTAS
        PRIMARY KEY (id_venta),

    CONSTRAINT CK_TBL_REGISTRO_VENTAS_monto_total
        CHECK (monto_total >= 0),

    CONSTRAINT FK_TBL_REGISTRO_VENTAS_TBL_METODOS_PAGO
        FOREIGN KEY (id_metodo_pago)
        REFERENCES dbo.TBL_METODOS_PAGO(id_metodo_pago)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_TBL_REGISTRO_VENTAS_TBL_USUARIOS
        FOREIGN KEY (id_usuario)
        REFERENCES dbo.TBL_USUARIOS(id_usuario)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO


/* =========================================================
   9. TBL_DETALLE_VENTAS
   ========================================================= */
CREATE TABLE dbo.TBL_DETALLE_VENTAS
(
    id_detalle_venta INT IDENTITY(1,1) NOT NULL,
    id_venta INT NOT NULL,
    id_lote INT NOT NULL,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,

    CONSTRAINT PK_TBL_DETALLE_VENTAS
        PRIMARY KEY (id_detalle_venta),

    CONSTRAINT FK_TBL_DETALLE_VENTAS_TBL_REGISTRO_VENTAS
        FOREIGN KEY (id_venta)
        REFERENCES dbo.TBL_REGISTRO_VENTAS(id_venta)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_TBL_DETALLE_VENTAS_TBL_LOTES
        FOREIGN KEY (id_lote)
        REFERENCES dbo.TBL_LOTES(id_lote)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT CK_TBL_DETALLE_VENTAS_cantidad
        CHECK (cantidad > 0),

    CONSTRAINT CK_TBL_DETALLE_VENTAS_precio_unitario
        CHECK (precio_unitario >= 0),

    CONSTRAINT UQ_TBL_DETALLE_VENTAS_id_venta_id_lote
        UNIQUE (id_venta, id_lote)
);
GO


/* =========================================================
   10. TBL_COMPROBANTES_TRIBUTARIOS
   ========================================================= */
CREATE TABLE dbo.TBL_COMPROBANTES_TRIBUTARIOS
(
    id_comprobante INT IDENTITY(1,1) NOT NULL,
    numero_comprobante NVARCHAR(20) NOT NULL,
    tipo_comprobante NVARCHAR(20) NOT NULL,
    fecha_emision DATE NOT NULL,
    id_venta INT NOT NULL,
    estado_documento NVARCHAR(25) NOT NULL
        CONSTRAINT DF_TBL_COMPROBANTES_TRIBUTARIOS_estado_documento
        DEFAULT 'Emitido',

    CONSTRAINT PK_TBL_COMPROBANTES_TRIBUTARIOS
        PRIMARY KEY (id_comprobante),

    CONSTRAINT UQ_TBL_COMPROBANTES_TRIBUTARIOS_numero_comprobante
        UNIQUE (numero_comprobante),

    CONSTRAINT CK_TBL_COMPROBANTES_TRIBUTARIOS_tipo_comprobante
        CHECK (tipo_comprobante IN ('Boleta', 'Factura')),

    CONSTRAINT CK_TBL_COMPROBANTES_TRIBUTARIOS_estado_documento
        CHECK (estado_documento IN ('Emitido', 'Anulado')),

    CONSTRAINT FK_TBL_COMPROBANTES_TRIBUTARIOS_TBL_REGISTRO_VENTAS
        FOREIGN KEY (id_venta)
        REFERENCES dbo.TBL_REGISTRO_VENTAS(id_venta)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO


/* =========================================================
   11. TBL_PRODUCTOS_RETIRADOS
   ========================================================= */
CREATE TABLE dbo.TBL_PRODUCTOS_RETIRADOS
(
    id_retiro INT IDENTITY(1,1) NOT NULL,
    id_lote INT NULL,
    nombre_producto NVARCHAR(100) NOT NULL,
    numero_lote NVARCHAR(30) NOT NULL,
    fecha_vencimiento DATE NOT NULL,
    es_lote_defectuoso BIT NOT NULL
        CONSTRAINT DF_TBL_PRODUCTOS_RETIRADOS_es_lote_defectuoso DEFAULT 0,
    alerta_digemid NVARCHAR(100) NULL,
    fecha_registro DATE NOT NULL,

    CONSTRAINT PK_TBL_PRODUCTOS_RETIRADOS
        PRIMARY KEY (id_retiro),

    CONSTRAINT FK_TBL_PRODUCTOS_RETIRADOS_TBL_LOTES
        FOREIGN KEY (id_lote)
        REFERENCES dbo.TBL_LOTES(id_lote)
        ON DELETE SET NULL
        ON UPDATE NO ACTION
);
GO


/* =========================================================
   12. TBL_ORDENES_DEVOLUCION
   ========================================================= */
CREATE TABLE dbo.TBL_ORDENES_DEVOLUCION
(
    id_devolucion INT IDENTITY(1,1) NOT NULL,
    fecha_creacion DATE NOT NULL,
    motivo_devolucion NVARCHAR(50) NOT NULL,
    id_proveedor INT NOT NULL,
    comentario NVARCHAR(MAX) NULL,
    estado_orden NVARCHAR(15) NOT NULL
        CONSTRAINT DF_TBL_ORDENES_DEVOLUCION_estado_orden DEFAULT 'Pendiente',
    id_usuario INT NOT NULL,

    CONSTRAINT PK_TBL_ORDENES_DEVOLUCION
        PRIMARY KEY (id_devolucion),

    CONSTRAINT CK_TBL_ORDENES_DEVOLUCION_motivo_devolucion
        CHECK (
            motivo_devolucion IN
            ('Por Vencimiento', 'Por Depuracion de Error')
        ),

    CONSTRAINT CK_TBL_ORDENES_DEVOLUCION_estado_orden
        CHECK (
            estado_orden IN
            ('Pendiente', 'Completada', 'Eliminada')
        ),

    CONSTRAINT CK_TBL_ORDENES_DEVOLUCION_comentario
        CHECK (
            motivo_devolucion <> 'Por Depuracion de Error'
            OR (
                comentario IS NOT NULL
                AND LEN(LTRIM(RTRIM(comentario))) > 0
            )
        ),

    CONSTRAINT FK_TBL_ORDENES_DEVOLUCION_TBL_PROVEEDORES
        FOREIGN KEY (id_proveedor)
        REFERENCES dbo.TBL_PROVEEDORES(id_proveedor)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_TBL_ORDENES_DEVOLUCION_TBL_USUARIOS
        FOREIGN KEY (id_usuario)
        REFERENCES dbo.TBL_USUARIOS(id_usuario)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO


/* =========================================================
   13. TBL_DETALLE_DEVOLUCION
   ========================================================= */
CREATE TABLE dbo.TBL_DETALLE_DEVOLUCION
(
    id_detalle_dev INT IDENTITY(1,1) NOT NULL,
    id_devolucion INT NOT NULL,
    id_lote INT NOT NULL,
    cantidad_devolver INT NOT NULL,

    CONSTRAINT PK_TBL_DETALLE_DEVOLUCION
        PRIMARY KEY (id_detalle_dev),

    CONSTRAINT FK_TBL_DETALLE_DEVOLUCION_TBL_ORDENES_DEVOLUCION
        FOREIGN KEY (id_devolucion)
        REFERENCES dbo.TBL_ORDENES_DEVOLUCION(id_devolucion)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_TBL_DETALLE_DEVOLUCION_TBL_LOTES
        FOREIGN KEY (id_lote)
        REFERENCES dbo.TBL_LOTES(id_lote)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT CK_TBL_DETALLE_DEVOLUCION_cantidad_devolver
        CHECK (cantidad_devolver > 0),

    CONSTRAINT UQ_TBL_DETALLE_DEVOLUCION_id_devolucion_id_lote
        UNIQUE (id_devolucion, id_lote)
);
GO


/* =========================================================
   14. TBL_RESTRICCIONES_VENTA
   ========================================================= */
CREATE TABLE dbo.TBL_RESTRICCIONES_VENTA
(
    id_restriccion INT IDENTITY(1,1) NOT NULL,
    id_producto INT NOT NULL,
    condicion_cliente NVARCHAR(50) NOT NULL,
    tipo_restriccion NVARCHAR(25) NOT NULL,
    motivo_advertencia NVARCHAR(MAX) NOT NULL,
    estado_alerta NVARCHAR(10) NOT NULL
        CONSTRAINT DF_TBL_RESTRICCIONES_VENTA_estado_alerta DEFAULT 'Activo',

    CONSTRAINT PK_TBL_RESTRICCIONES_VENTA
        PRIMARY KEY (id_restriccion),

    CONSTRAINT FK_TBL_RESTRICCIONES_VENTA_TBL_PRODUCTOS
        FOREIGN KEY (id_producto)
        REFERENCES dbo.TBL_PRODUCTOS(id_producto)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT CK_TBL_RESTRICCIONES_VENTA_tipo_restriccion
        CHECK (tipo_restriccion IN ('Informativa', 'Bloqueante')),

    CONSTRAINT CK_TBL_RESTRICCIONES_VENTA_estado_alerta
        CHECK (estado_alerta IN ('Activo', 'Inactivo'))
);
GO


/* =========================================================
   15. TBL_LISTA_REABASTECIMIENTO
   ========================================================= */
CREATE TABLE dbo.TBL_LISTA_REABASTECIMIENTO
(
    id_reabastecimiento INT IDENTITY(1,1) NOT NULL,
    id_producto INT NOT NULL,
    origen NVARCHAR(20) NOT NULL
        CONSTRAINT DF_TBL_LISTA_REABASTECIMIENTO_origen DEFAULT 'Automatico',
    prioridad NVARCHAR(10) NOT NULL
        CONSTRAINT DF_TBL_LISTA_REABASTECIMIENTO_prioridad DEFAULT 'Normal',
    fecha_inclusion DATE NOT NULL,

    CONSTRAINT PK_TBL_LISTA_REABASTECIMIENTO
        PRIMARY KEY (id_reabastecimiento),

    CONSTRAINT UQ_TBL_LISTA_REABASTECIMIENTO_id_producto
        UNIQUE (id_producto),

    CONSTRAINT CK_TBL_LISTA_REABASTECIMIENTO_origen
        CHECK (origen IN ('Automatico', 'Manual')),

    CONSTRAINT CK_TBL_LISTA_REABASTECIMIENTO_prioridad
        CHECK (prioridad IN ('Normal', 'Urgente')),

    CONSTRAINT FK_TBL_LISTA_REABASTECIMIENTO_TBL_PRODUCTOS
        FOREIGN KEY (id_producto)
        REFERENCES dbo.TBL_PRODUCTOS(id_producto)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO


/* =========================================================
   16. TBL_COTIZACIONES_PROVEEDOR
   ========================================================= */
CREATE TABLE dbo.TBL_COTIZACIONES_PROVEEDOR
(
    id_cotizacion INT IDENTITY(1,1) NOT NULL,
    id_producto INT NOT NULL,
    id_proveedor INT NOT NULL,
    precio_cotizado DECIMAL(10,2) NOT NULL,
    fecha_cotizacion DATE NOT NULL,
    promocion NVARCHAR(MAX) NULL,
    id_usuario INT NOT NULL,

    CONSTRAINT PK_TBL_COTIZACIONES_PROVEEDOR
        PRIMARY KEY (id_cotizacion),

    CONSTRAINT CK_TBL_COTIZACIONES_PROVEEDOR_precio_cotizado
        CHECK (precio_cotizado >= 0),

    CONSTRAINT FK_TBL_COTIZACIONES_PROVEEDOR_TBL_PRODUCTOS
        FOREIGN KEY (id_producto)
        REFERENCES dbo.TBL_PRODUCTOS(id_producto)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_TBL_COTIZACIONES_PROVEEDOR_TBL_PROVEEDORES
        FOREIGN KEY (id_proveedor)
        REFERENCES dbo.TBL_PROVEEDORES(id_proveedor)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_TBL_COTIZACIONES_PROVEEDOR_TBL_USUARIOS
        FOREIGN KEY (id_usuario)
        REFERENCES dbo.TBL_USUARIOS(id_usuario)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO


/* =========================================================
   ÍNDICES ÚNICOS FILTRADOS
   ========================================================= */

CREATE UNIQUE INDEX UX_TBL_COMPROBANTES_TRIBUTARIOS_VentaEmitido
ON dbo.TBL_COMPROBANTES_TRIBUTARIOS(id_venta)
WHERE estado_documento = 'Emitido';
GO

CREATE UNIQUE INDEX UX_TBL_ALERTAS_VENCIMIENTO_Activa
ON dbo.TBL_ALERTAS_VENCIMIENTO(estado)
WHERE estado = 'Activo';
GO


/* =========================================================
   ÍNDICES DEL DICCIONARIO
   ========================================================= */

CREATE INDEX IX_TBL_LOTES_FEFO
ON dbo.TBL_LOTES
(
    id_producto,
    estado_lote,
    fecha_vencimiento
)
INCLUDE (stock_actual);
GO

CREATE INDEX IX_TBL_LOTES_Proveedor
ON dbo.TBL_LOTES
(
    id_proveedor,
    fecha_vencimiento
);
GO

CREATE INDEX IX_TBL_REGISTRO_VENTAS_FechaHora
ON dbo.TBL_REGISTRO_VENTAS(fecha_hora);
GO

CREATE INDEX IX_TBL_DETALLE_VENTAS_Lote
ON dbo.TBL_DETALLE_VENTAS(id_lote);
GO

CREATE INDEX IX_TBL_DETALLE_DEVOLUCION_Lote
ON dbo.TBL_DETALLE_DEVOLUCION(id_lote);
GO

CREATE INDEX IX_TBL_COTIZACIONES_PROVEEDOR_Vigente
ON dbo.TBL_COTIZACIONES_PROVEEDOR
(
    id_producto,
    id_proveedor,
    fecha_cotizacion DESC,
    id_cotizacion DESC
);
GO


/* =========================================================
   VISTAS DE STOCK
   ========================================================= */

CREATE VIEW dbo.VW_STOCK_FISICO
AS
    SELECT
        id_producto,
        SUM(stock_actual) AS stock_fisico
    FROM dbo.TBL_LOTES
    GROUP BY id_producto;
GO


CREATE VIEW dbo.VW_STOCK_VENDIBLE
AS
    SELECT
        id_producto,
        SUM(stock_actual) AS stock_vendible
    FROM dbo.TBL_LOTES
    WHERE estado_lote = 'Disponible'
      AND fecha_vencimiento > CAST(GETDATE() AS DATE)
    GROUP BY id_producto;
GO