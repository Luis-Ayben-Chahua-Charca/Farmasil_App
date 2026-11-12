/* =========================================================
   FARMASIL - Datos de prueba
   Issue #71
   ========================================================= */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

------------------------------------------------------------
-- 1. USUARIOS
-- Contraseña de prueba para ambas cuentas: Farmasil2026!
-- Formato: PBKDF2-SHA256$iteraciones$sal$hash
------------------------------------------------------------

INSERT INTO dbo.TBL_USUARIOS
    (nombre_usuario, contrasena, rol, estado)
VALUES
(
    N'admin_prueba',
    N'PBKDF2-SHA256$100000$RkFSTUFTSUwtQURNSU4tMDE=$+LkYPDp3ujISJg57fcX4TI4mONdCJVKbJKmrKWl5nzo=',
    N'Administrador',
    N'Activo'
),
(
    N'tecnico_prueba',
    N'PBKDF2-SHA256$100000$RkFSTUFTSUwtVEVDTklDTzE=$nrqRBgiwG5d0X78IdfcJNAFb2Pnc75NJ8jOAf1l3rBs=',
    N'Tecnico',
    N'Activo'
);

------------------------------------------------------------
-- 2. PROVEEDORES
------------------------------------------------------------

INSERT INTO dbo.TBL_PROVEEDORES
    (ruc, razon_social, telefono, estado)
VALUES
(N'20100070970', N'Drogueria Andina SAC', N'054223344', N'Activo'),
(N'20481234567', N'Distribuidora Salud Sur SAC', N'054445566', N'Activo'),
(N'20609876543', N'Laboratorios del Pacifico SAC', N'054778899', N'Activo'),
(N'20501112223', N'Proveedor Inactivo SAC', NULL, N'Inactivo');

------------------------------------------------------------
-- 3. METODOS DE PAGO
------------------------------------------------------------

INSERT INTO dbo.TBL_METODOS_PAGO
    (nombre_metodo, estado)
VALUES
(N'Efectivo', N'Activo'),
(N'Yape', N'Activo'),
(N'Tarjeta', N'Inactivo');

------------------------------------------------------------
-- 4. PRODUCTOS
------------------------------------------------------------

INSERT INTO dbo.TBL_PRODUCTOS
    (nombre, accion_terapeutica, precio_venta,
     estado_producto, id_proveedor, stock_minimo)
VALUES
(
    N'Paracetamol 500 mg',
    N'Analgesico y antipiretico',
    2.50,
    N'Disponible',
    (SELECT id_proveedor
     FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20100070970'),
    10
),
(
    N'Ibuprofeno 400 mg',
    N'Antiinflamatorio',
    4.00,
    N'Disponible',
    (SELECT id_proveedor
     FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20481234567'),
    8
),
(
    N'Amoxicilina 500 mg',
    N'Antibiotico',
    6.50,
    N'Disponible',
    (SELECT id_proveedor
     FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20609876543'),
    10
),
(
    N'Loratadina 10 mg',
    N'Antihistaminico',
    3.00,
    N'Disponible',
    (SELECT id_proveedor
     FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20100070970'),
    5
),
(
    N'Diclofenaco 50 mg',
    N'Antiinflamatorio',
    5.00,
    N'Disponible',
    (SELECT id_proveedor
     FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20609876543'),
    6
),
(
    N'Omeprazol 20 mg',
    N'Gastroprotector',
    4.50,
    N'Disponible',
    (SELECT id_proveedor
     FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20481234567'),
    8
),
(
    N'Producto Descontinuado',
    N'Producto historico',
    2.00,
    N'Descontinuado',
    (SELECT id_proveedor
     FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20100070970'),
    0
);

------------------------------------------------------------
-- 5. LOTES / REMESAS
--
-- Casos cubiertos:
-- Disponible
-- Bloqueado
-- Vencido
-- Por vencer dentro del umbral de 2 meses
-- Agotado
------------------------------------------------------------

-- Paracetamol: stock fisico 25, vendible 20.
INSERT INTO dbo.TBL_LOTES
    (id_producto, numero_lote, fecha_vencimiento,
     stock_actual, estado_lote, fecha_ingreso, id_proveedor)
VALUES
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Paracetamol 500 mg'),
    N'PAR-001',
    DATEADD(MONTH, 12, CAST(GETDATE() AS DATE)),
    20,
    N'Disponible',
    DATEADD(DAY, -30, CAST(GETDATE() AS DATE)),
    (SELECT id_proveedor FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20100070970')
),
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Paracetamol 500 mg'),
    N'PAR-002',
    DATEADD(MONTH, 10, CAST(GETDATE() AS DATE)),
    5,
    N'Bloqueado por devolucion',
    DATEADD(DAY, -25, CAST(GETDATE() AS DATE)),
    (SELECT id_proveedor FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20100070970')
),

-- Vencido: sigue existiendo fisicamente, pero no es vendible.
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Ibuprofeno 400 mg'),
    N'IBU-VENC-001',
    DATEADD(MONTH, -1, CAST(GETDATE() AS DATE)),
    8,
    N'Disponible',
    DATEADD(MONTH, -6, CAST(GETDATE() AS DATE)),
    (SELECT id_proveedor FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20481234567')
),

-- Por vencer dentro del umbral inicial de 2 meses.
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Amoxicilina 500 mg'),
    N'AMO-PROX-001',
    DATEADD(MONTH, 1, CAST(GETDATE() AS DATE)),
    12,
    N'Disponible',
    DATEADD(DAY, -20, CAST(GETDATE() AS DATE)),
    (SELECT id_proveedor FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20609876543')
),

-- Agotado: stock cero permitido porque no está Disponible.
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Loratadina 10 mg'),
    N'LOR-AGOT-001',
    DATEADD(MONTH, 6, CAST(GETDATE() AS DATE)),
    0,
    N'Agotado',
    DATEADD(DAY, -90, CAST(GETDATE() AS DATE)),
    (SELECT id_proveedor FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20100070970')
),

-- Otro lote bloqueado para devoluciones.
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Diclofenaco 50 mg'),
    N'DIC-BLOQ-001',
    DATEADD(MONTH, 8, CAST(GETDATE() AS DATE)),
    6,
    N'Bloqueado por devolucion',
    DATEADD(DAY, -40, CAST(GETDATE() AS DATE)),
    (SELECT id_proveedor FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20609876543')
);

-- Omeprazol queda intencionalmente SIN LOTES.
-- Permite probar producto con stock fisico/vendible igual a cero.

------------------------------------------------------------
-- 6. REGLA DE ALERTA INICIAL
------------------------------------------------------------

INSERT INTO dbo.TBL_ALERTAS_VENCIMIENTO
    (descripcion, umbral_meses, estado, fecha_ultima_verif)
VALUES
(
    N'Alerta inicial de vencimiento',
    2,
    N'Activo',
    NULL
);

------------------------------------------------------------
-- 7. REPORTES HISTORICOS
------------------------------------------------------------

INSERT INTO dbo.TBL_REPORTES_VENTAS
    (tipo_reporte, formato, fecha_inicio, fecha_fin,
     total_recaudado, fecha_generacion, ruta_archivo)
VALUES
(
    N'Ventas Diarias',
    N'PDF',
    DATEADD(DAY, -30, CAST(GETDATE() AS DATE)),
    DATEADD(DAY, -30, CAST(GETDATE() AS DATE)),
    9.00,
    DATEADD(DAY, -29, CAST(GETDATE() AS DATETIME2(0))),
    N'reportes/ventas-diarias-prueba.pdf'
),
(
    N'Ventas Mensuales',
    N'Excel',
    DATEADD(MONTH, -1, CAST(GETDATE() AS DATE)),
    CAST(GETDATE() AS DATE),
    11.50,
    CAST(GETDATE() AS DATETIME2(0)),
    N'reportes/ventas-mensuales-prueba.xlsx'
);

------------------------------------------------------------
-- 8. VENTAS HISTORICAS
------------------------------------------------------------

INSERT INTO dbo.TBL_REGISTRO_VENTAS
    (fecha_hora, monto_total, id_metodo_pago, id_usuario)
VALUES
(
    DATEADD(DAY, -10, CAST(GETDATE() AS DATETIME2(0))),
    5.00,
    (SELECT id_metodo_pago FROM dbo.TBL_METODOS_PAGO
     WHERE nombre_metodo = N'Efectivo'),
    (SELECT id_usuario FROM dbo.TBL_USUARIOS
     WHERE nombre_usuario = N'tecnico_prueba')
),
(
    DATEADD(DAY, -40, CAST(GETDATE() AS DATETIME2(0))),
    4.00,
    (SELECT id_metodo_pago FROM dbo.TBL_METODOS_PAGO
     WHERE nombre_metodo = N'Efectivo'),
    (SELECT id_usuario FROM dbo.TBL_USUARIOS
     WHERE nombre_usuario = N'tecnico_prueba')
),
(
    DATEADD(DAY, -5, CAST(GETDATE() AS DATETIME2(0))),
    2.50,
    (SELECT id_metodo_pago FROM dbo.TBL_METODOS_PAGO
     WHERE nombre_metodo = N'Yape'),
    (SELECT id_usuario FROM dbo.TBL_USUARIOS
     WHERE nombre_usuario = N'admin_prueba')
);

------------------------------------------------------------
-- 9. DETALLE DE VENTAS
------------------------------------------------------------

INSERT INTO dbo.TBL_DETALLE_VENTAS
    (id_venta, id_lote, cantidad, precio_unitario)
VALUES
(
    (SELECT MIN(id_venta) FROM dbo.TBL_REGISTRO_VENTAS),
    (SELECT id_lote FROM dbo.TBL_LOTES
     WHERE numero_lote = N'PAR-001'),
    2,
    2.50
),
(
    (SELECT MIN(id_venta) + 1 FROM dbo.TBL_REGISTRO_VENTAS),
    (SELECT id_lote FROM dbo.TBL_LOTES
     WHERE numero_lote = N'IBU-VENC-001'),
    1,
    4.00
),
(
    (SELECT MAX(id_venta) FROM dbo.TBL_REGISTRO_VENTAS),
    (SELECT id_lote FROM dbo.TBL_LOTES
     WHERE numero_lote = N'PAR-001'),
    1,
    2.50
);

------------------------------------------------------------
-- 10. COMPROBANTES
--
-- Primera venta CON comprobante.
-- Segunda venta queda SIN comprobante intencionalmente.
------------------------------------------------------------

INSERT INTO dbo.TBL_COMPROBANTES_TRIBUTARIOS
    (numero_comprobante, tipo_comprobante,
     fecha_emision, id_venta, estado_documento)
VALUES
(
    N'B001-000001',
    N'Boleta',
    DATEADD(DAY, -10, CAST(GETDATE() AS DATE)),
    (SELECT MIN(id_venta) FROM dbo.TBL_REGISTRO_VENTAS),
    N'Emitido'
),
(
    N'F001-000001',
    N'Factura',
    DATEADD(DAY, -5, CAST(GETDATE() AS DATE)),
    (SELECT MAX(id_venta) FROM dbo.TBL_REGISTRO_VENTAS),
    N'Anulado'
);

------------------------------------------------------------
-- 11. PRODUCTOS RETIRADOS
------------------------------------------------------------

INSERT INTO dbo.TBL_PRODUCTOS_RETIRADOS
    (id_lote, nombre_producto, numero_lote,
     fecha_vencimiento, es_lote_defectuoso,
     alerta_digemid, fecha_registro)
VALUES
(
    (SELECT id_lote FROM dbo.TBL_LOTES
     WHERE numero_lote = N'IBU-VENC-001'),
    N'Ibuprofeno 400 mg',
    N'IBU-VENC-001',
    DATEADD(MONTH, -1, CAST(GETDATE() AS DATE)),
    0,
    NULL,
    DATEADD(DAY, -5, CAST(GETDATE() AS DATE))
),
(
    NULL,
    N'Medicamento historico retirado',
    N'HIST-001',
    DATEADD(MONTH, -3, CAST(GETDATE() AS DATE)),
    1,
    N'Alerta DIGEMID de prueba',
    DATEADD(MONTH, -2, CAST(GETDATE() AS DATE))
);

------------------------------------------------------------
-- 12. ORDENES DE DEVOLUCION
------------------------------------------------------------

INSERT INTO dbo.TBL_ORDENES_DEVOLUCION
    (fecha_creacion, motivo_devolucion, id_proveedor,
     comentario, estado_orden, id_usuario)
VALUES
(
    DATEADD(DAY, -2, CAST(GETDATE() AS DATE)),
    N'Por Vencimiento',
    (SELECT id_proveedor FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20100070970'),
    N'Devolucion de lote bloqueado de prueba.',
    N'Pendiente',
    (SELECT id_usuario FROM dbo.TBL_USUARIOS
     WHERE nombre_usuario = N'admin_prueba')
),
(
    DATEADD(DAY, -20, CAST(GETDATE() AS DATE)),
    N'Por Depuracion de Error',
    (SELECT id_proveedor FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20609876543'),
    N'Registro creado para probar depuracion de error.',
    N'Completada',
    (SELECT id_usuario FROM dbo.TBL_USUARIOS
     WHERE nombre_usuario = N'admin_prueba')
);

------------------------------------------------------------
-- 13. DETALLE DE DEVOLUCION
------------------------------------------------------------

INSERT INTO dbo.TBL_DETALLE_DEVOLUCION
    (id_devolucion, id_lote, cantidad_devolver)
VALUES
(
    (SELECT MIN(id_devolucion)
     FROM dbo.TBL_ORDENES_DEVOLUCION),
    (SELECT id_lote FROM dbo.TBL_LOTES
     WHERE numero_lote = N'PAR-002'),
    2
),
(
    (SELECT MAX(id_devolucion)
     FROM dbo.TBL_ORDENES_DEVOLUCION),
    (SELECT id_lote FROM dbo.TBL_LOTES
     WHERE numero_lote = N'DIC-BLOQ-001'),
    1
);

------------------------------------------------------------
-- 14. RESTRICCIONES DE VENTA
------------------------------------------------------------

INSERT INTO dbo.TBL_RESTRICCIONES_VENTA
    (id_producto, condicion_cliente, tipo_restriccion,
     motivo_advertencia, estado_alerta)
VALUES
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Paracetamol 500 mg'),
    N'Gestante',
    N'Informativa',
    N'Consultar dosis recomendada antes de la venta.',
    N'Activo'
),
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Amoxicilina 500 mg'),
    N'Alergia a penicilinas',
    N'Bloqueante',
    N'No completar la venta si el cliente presenta alergia.',
    N'Activo'
),
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Ibuprofeno 400 mg'),
    N'Antecedente gastrico',
    N'Informativa',
    N'Restriccion historica inactiva para pruebas.',
    N'Inactivo'
);

------------------------------------------------------------
-- 15. LISTA DE REABASTECIMIENTO
------------------------------------------------------------

INSERT INTO dbo.TBL_LISTA_REABASTECIMIENTO
    (id_producto, origen, prioridad, fecha_inclusion)
VALUES
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Ibuprofeno 400 mg'),
    N'Automatico',
    N'Urgente',
    CAST(GETDATE() AS DATE)
),
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Loratadina 10 mg'),
    N'Manual',
    N'Normal',
    CAST(GETDATE() AS DATE)
),
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Omeprazol 20 mg'),
    N'Automatico',
    N'Urgente',
    CAST(GETDATE() AS DATE)
);

------------------------------------------------------------
-- 16. COTIZACIONES DE PROVEEDORES
------------------------------------------------------------

INSERT INTO dbo.TBL_COTIZACIONES_PROVEEDOR
    (id_producto, id_proveedor, precio_cotizado,
     fecha_cotizacion, promocion, id_usuario)
VALUES
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Paracetamol 500 mg'),
    (SELECT id_proveedor FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20100070970'),
    1.50,
    DATEADD(DAY, -30, CAST(GETDATE() AS DATE)),
    N'Precio regular',
    (SELECT id_usuario FROM dbo.TBL_USUARIOS
     WHERE nombre_usuario = N'admin_prueba')
),
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Paracetamol 500 mg'),
    (SELECT id_proveedor FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20100070970'),
    1.40,
    CAST(GETDATE() AS DATE),
    N'Descuento por volumen',
    (SELECT id_usuario FROM dbo.TBL_USUARIOS
     WHERE nombre_usuario = N'admin_prueba')
),
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Paracetamol 500 mg'),
    (SELECT id_proveedor FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20481234567'),
    1.35,
    CAST(GETDATE() AS DATE),
    NULL,
    (SELECT id_usuario FROM dbo.TBL_USUARIOS
     WHERE nombre_usuario = N'admin_prueba')
),
(
    (SELECT id_producto FROM dbo.TBL_PRODUCTOS
     WHERE nombre = N'Amoxicilina 500 mg'),
    (SELECT id_proveedor FROM dbo.TBL_PROVEEDORES
     WHERE ruc = N'20609876543'),
    4.80,
    DATEADD(DAY, -3, CAST(GETDATE() AS DATE)),
    N'Promocion de prueba',
    (SELECT id_usuario FROM dbo.TBL_USUARIOS
     WHERE nombre_usuario = N'admin_prueba')
);

COMMIT TRANSACTION;

PRINT 'Datos de prueba cargados correctamente.';
