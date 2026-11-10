# Diccionario de Datos — DB_FARMASIL

## 1. TBL_USUARIOS

Almacena las credenciales y perfiles del personal de la farmacia que accede al sistema.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_usuario | INT | PK, IDENTITY(1,1) | Identificador único del usuario. |
| nombre_usuario | NVARCHAR(50) | NOT NULL, UNIQUE | Nombre de usuario para el inicio de sesión. |
| contrasena | NVARCHAR(255) | NOT NULL | Hash con sal de la contraseña, nunca reversible, conforme a RNF-0006 y DEC-014. Se guarda como una sola cadena autodescriptiva con el formato `PBKDF2-SHA256$<iteraciones>$<sal Base64>$<hash Base64>`, que cabe en NVARCHAR(255); no hay columna de sal separada (DEC-041). |
| rol | NVARCHAR(20) | NOT NULL, CHECK (rol IN ('Administrador', 'Tecnico')) | Rol en el sistema: 'Administrador' o 'Tecnico'. |
| estado | NVARCHAR(10) | NOT NULL, DEFAULT 'Activo', CHECK (estado IN ('Activo', 'Inactivo')) | Estado de la cuenta: 'Activo' o 'Inactivo'. |

*Educción asociada: EDU-0013. Ilaciones: ILA-0033 a ILA-0036.*

## 2. TBL_PROVEEDORES

Registra las droguerías y distribuidoras farmacéuticas que abastecen a la farmacia.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_proveedor | INT | PK, IDENTITY(1,1) | Identificador único del proveedor. |
| ruc | NVARCHAR(11) | NOT NULL, UNIQUE, CHECK (LEN(ruc) = 11 AND ruc NOT LIKE '%[^0-9]%') | Identificación tributaria de la empresa: exactamente once dígitos. |
| razon_social | NVARCHAR(150) | NOT NULL | Nombre legal de la empresa proveedora. |
| telefono | NVARCHAR(15) | Opcional | Número telefónico de contacto. |
| estado | NVARCHAR(10) | NOT NULL, DEFAULT 'Activo', CHECK (estado IN ('Activo', 'Inactivo')) | Control de proveedores vigentes: 'Activo' o 'Inactivo'. |

*Educción asociada: EDU-0014. Ilaciones: ILA-0037 a ILA-0040.*

## 3. TBL_PRODUCTOS

Catálogo de medicamentos e insumos médicos disponibles en el inventario.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_producto | INT | PK, IDENTITY(1,1) | Identificador único del medicamento. |
| nombre | NVARCHAR(100) | NOT NULL | Nombre de marca del producto. |
| accion_terapeutica | NVARCHAR(150) | NOT NULL | Principio activo o categoría terapéutica; cuál de los dos guarda está por definir (PEN-010). |
| precio_venta | DECIMAL(10,2) | NOT NULL, CHECK (precio_venta >= 0) | Precio al público por unidad o blíster. |
| estado_producto | NVARCHAR(25) | NOT NULL, DEFAULT 'Disponible', CHECK (estado_producto IN ('Disponible', 'Descontinuado')) | Estados: 'Disponible', 'Descontinuado'. |
| id_proveedor | INT | NOT NULL, FK (TBL_PROVEEDORES) | Enlace para saber quién abastece este producto (proveedor habitual del catálogo). |
| stock_minimo | INT | NOT NULL, DEFAULT 0, CHECK (stock_minimo >= 0) | Umbral del stock vendible (ver la sección «Stock físico y stock vendible») por debajo del cual el medicamento se incorpora automáticamente a `TBL_LISTA_REABASTECIMIENTO`. |

*Educción asociada: EDU-0002. El estado 'Descontinuado' lo asigna ILA-0008.*

*Cambio en v04.00: `numero_lote`, `fecha_vencimiento` y `stock_actual` se trasladaron a `TBL_LOTES`. El valor `'Bloqueado por devolucion'` se retiró de este dominio y ahora vive en `estado_lote`: bloquear un medicamento completo porque una de sus remesas vence impedía vender las remesas sanas.*

*Cambio en v05.00: se agrega `stock_minimo`, capturado al crear o actualizar el medicamento (ESP-0005/ESP-0007) y usado por el proceso periódico que alimenta la lista de reabastecimiento (EDU-0016).*

## 4. TBL_LOTES

Remesas físicas de cada medicamento. Un producto del catálogo puede tener varios lotes conviviendo en el estante, con vencimientos y existencias distintos.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_lote | INT | PK, IDENTITY(1,1) | Identificador único de la remesa. |
| id_producto | INT | NOT NULL, FK (TBL_PRODUCTOS) | Medicamento al que pertenece la remesa. |
| numero_lote | NVARCHAR(30) | NOT NULL | Número de lote impreso por el laboratorio. |
| fecha_vencimiento | DATE | NOT NULL | Fecha de caducidad de esta remesa. |
| stock_actual | INT | NOT NULL, CHECK (stock_actual >= 0) | Unidades físicas de esta remesa. Nunca negativo: ESP-0027 valida el stock antes de completar una devolución. Sin valor por defecto: una remesa nueva se registra con stock mayor que cero (ESP-0041). |
| estado_lote | NVARCHAR(25) | NOT NULL, DEFAULT 'Disponible', CHECK (estado_lote IN ('Disponible', 'Bloqueado por devolucion', 'Agotado')) | Estados: 'Disponible', 'Bloqueado por devolucion', 'Agotado'. Se mantiene con una sola regla (DEC-034), no se edita a mano (DEC-010). |
| fecha_ingreso | DATE | NOT NULL | Fecha en que la remesa ingresó al almacén. |
| id_proveedor | INT | NOT NULL, FK (TBL_PROVEEDORES) | Proveedor que efectivamente entregó esta remesa. Puede diferir del proveedor habitual declarado en `TBL_PRODUCTOS.id_proveedor` cuando el medicamento se compra a más de un proveedor. |

Restricción adicional: `CHECK (stock_actual >= 0)`, que impide en el motor un descuento que deje la remesa en negativo.

Restricción adicional: `CHECK (estado_lote <> 'Disponible' OR stock_actual > 0)`. Una remesa 'Disponible' siempre tiene existencias; el motor rechaza la combinación 'Disponible' con cero unidades (DEC-030). La regla que mantiene el estado coherente con el stock es única y la invocan todas las operaciones que cambian existencias: 'Disponible' con stock 0 pasa a 'Agotado'; 'Agotado' con stock mayor que 0 y no vencida pasa a 'Disponible'; 'Bloqueado por devolucion' solo cambia por las operaciones de alertas y devoluciones (DEC-034).

Restricción adicional: `UNIQUE (id_producto, numero_lote)`. El mismo laboratorio no repite número de lote para un mismo medicamento, y admitir duplicados haría ambiguo el rastreo sanitario.

*Educción asociada: EDU-0015. El estado 'Bloqueado por devolucion' lo asigna ILA-0014 y lo restituye ILA-0016, salvo que la remesa ya esté vencida (v06.00).*

*Cambio en v05.00: se agrega `id_proveedor`, que resuelve la limitación del modelo para el caso de remesas de un mismo medicamento provenientes de proveedores distintos. Exige corregir ESP-0025 (que hasta v02.00 validaba el proveedor contra `TBL_PRODUCTOS` en vez de contra la remesa) y ampliar ESP-0041/ESP-0043 para capturarlo.*

## 5. TBL_REGISTRO_VENTAS

Cabecera de las ventas realizadas en el punto de venta.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_venta | INT | PK, IDENTITY(1,1) | Identificador único de la transacción de venta. |
| fecha_hora | DATETIME2(0) | NOT NULL | Fecha y hora exacta en la que se procesó el cobro. |
| monto_total | DECIMAL(10,2) | NOT NULL, CHECK (monto_total >= 0) | Suma total cobrada al cliente, calculada a partir del detalle. |
| id_metodo_pago | INT | NOT NULL, FK (TBL_METODOS_PAGO) | Método utilizado para la transacción. |
| **id_usuario** | **INT** | **NOT NULL, FK (TBL_USUARIOS)** | **Personal que registró la venta. Campo agregado en v03.00.** |

*Educción asociada: EDU-0001. El campo `id_usuario` lo alimenta ILA-0001 a partir de la sesión activa y lo consume ILA-0021 para el reporte.*

## 6. TBL_DETALLE_VENTAS

Desglosa los productos vendidos dentro de cada transacción. Es la tabla que permite vender varios productos en una sola venta.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_detalle_venta | INT | PK, IDENTITY(1,1) | Identificador único de la línea de detalle. |
| id_venta | INT | NOT NULL, FK (TBL_REGISTRO_VENTAS) | Enlace a la cabecera de la venta. |
| id_lote | INT | NOT NULL, FK (TBL_LOTES) | Remesa exacta de la que salió el producto vendido. |
| cantidad | INT | NOT NULL, CHECK (cantidad > 0) | Cantidad de unidades vendidas. |
| precio_unitario | DECIMAL(10,2) | NOT NULL, CHECK (precio_unitario >= 0) | Precio del producto al momento de la venta. |

Restricción adicional: `UNIQUE (id_venta, id_lote)`. Una remesa aparece una sola vez por venta; si se agrega de nuevo, se acumula la cantidad en su línea (DEC-036).

*Dependencia de existencia: el detalle no existe sin su cabecera. La eliminación de una venta elimina sus líneas por la operación (ESP-0004, ILA-0004), no por el motor: la clave foránea es `ON DELETE NO ACTION`. La relación es no identificadora porque el detalle tiene clave propia (`id_detalle_venta`).*

*Cambio en v04.00: `id_producto` se sustituyó por `id_lote`. El medicamento se obtiene navegando `TBL_LOTES.id_producto`, y a cambio el sistema puede responder qué se vendió de una remesa concreta cuando DIGEMID retira un lote.*

## 7. TBL_COMPROBANTES_TRIBUTARIOS

Documentos fiscales emitidos formalmente por cada venta efectuada.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_comprobante | INT | PK, IDENTITY(1,1) | Identificador interno del documento tributario. |
| **numero_comprobante** | **NVARCHAR(20)** | **NOT NULL, UNIQUE** | **Número de serie y correlativo impreso en el documento fiscal. Campo agregado en v03.00.** |
| tipo_comprobante | NVARCHAR(20) | NOT NULL, CHECK (tipo_comprobante IN ('Boleta', 'Factura')) | Valores: 'Boleta' o 'Factura'. |
| fecha_emision | DATE | NOT NULL | Fecha de emisión fiscal. |
| id_venta | INT | NOT NULL, FK (TBL_REGISTRO_VENTAS) | Enlace directo a la venta que originó el documento. |
| estado_documento | NVARCHAR(25) | NOT NULL, DEFAULT 'Emitido', CHECK (estado_documento IN ('Emitido', 'Anulado')) | Estados: 'Emitido', 'Anulado'. |

Restricción adicional: índice único filtrado `UX_TBL_COMPROBANTES_TRIBUTARIOS_VentaEmitido (id_venta) WHERE estado_documento = 'Emitido'`. Hace cumplir en el motor «a lo sumo un comprobante 'Emitido' por venta» (DEC-017) sin impedir conservar los anulados; por eso no es un `UNIQUE` sobre toda la columna.

*Relación uno a muchos con la venta: una venta puede acumular comprobantes anulados, pero tiene a lo sumo uno 'Emitido' (ESP-0009). Un comprobante emitido nunca se elimina físicamente: ILA-0012 lo anula.*

## 8. TBL_METODOS_PAGO

Catálogo de las modalidades de pago aceptadas por el establecimiento.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_metodo_pago | INT | PK, IDENTITY(1,1) | Identificador único del método de pago. |
| nombre_metodo | NVARCHAR(50) | NOT NULL, UNIQUE | Ejemplos: 'Efectivo', 'Tarjeta de Credito', 'Yape/Plin'. |
| estado | NVARCHAR(10) | NOT NULL, DEFAULT 'Activo', CHECK (estado IN ('Activo', 'Inactivo')) | Control de disponibilidad del método de pago: 'Activo' o 'Inactivo'. |

*Educción asociada: EDU-0009. La baja se resuelve como desactivación en ILA-0020 cuando existen ventas asociadas.*

## 9. TBL_ALERTAS_VENCIMIENTO

Configuración del motor de alertas automáticas de vida útil de los medicamentos.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_alerta | INT | PK, IDENTITY(1,1) | Identificador de la regla de alerta. |
| descripcion | NVARCHAR(150) | NOT NULL | Nombre de la regla, por ejemplo 'Alerta Base Cuarentena'. |
| umbral_meses | INT | NOT NULL, DEFAULT 2, CHECK (umbral_meses > 0) | Margen crítico de vida útil, en meses. |
| estado | NVARCHAR(10) | NOT NULL, DEFAULT 'Activo', CHECK (estado IN ('Activo', 'Inactivo')) | Define si el motor de alertas ejecuta esta regla. Hay a lo sumo una regla 'Activo'. |
| fecha_ultima_verif | DATETIME2(0) | NULL | Fecha y hora de la última evaluación de vencimientos. La actualiza el proceso 1 de RNF-0010 al terminar cada evaluación (DEC-018). NULL significa «todavía no evaluada»: nunca se carga una fecha ficticia (DEC-032). |

Restricción adicional: índice único filtrado `UX_TBL_ALERTAS_VENCIMIENTO_Activa (estado) WHERE estado = 'Activo'`. La tabla es de configuración y el sistema lee una sola regla activa (ESP-0013, RNF-0010); el motor impide que haya dos (DEC-032).

La fila inicial ('Alerta Base Cuarentena', 2 meses, 'Activo', `fecha_ultima_verif` NULL) la carga el script de datos iniciales (PEN-009); ninguna operación crea filas.

*Tabla de configuración global, sin claves foráneas. Actualmente ninguna educción gestiona su contenido: ILA-0013 solo lee `umbral_meses`. Ver la sección final.*

## 10. TBL_PRODUCTOS_RETIRADOS

Historial de mermas y productos removidos de la venta por vencimiento, daño o alerta regulatoria.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_retiro | INT | PK, IDENTITY(1,1) | Identificador único del registro de retiro. |
| **id_lote** | **INT** | **FK (TBL_LOTES), ON DELETE SET NULL** | **Remesa retirada. Campo agregado en v06.00: ILA-0014 bloquea esta remesa e ILA-0016 la restituye, y sin el identificador ninguna de las dos podía localizarla. Queda nulo si la remesa se elimina físicamente (ILA-0044).** |
| nombre_producto | NVARCHAR(100) | NOT NULL | Copia del nombre del producto, conservada deliberadamente como texto para mantener el histórico aunque el producto maestro se elimine. |
| **numero_lote** | **NVARCHAR(30)** | **NOT NULL** | **Copia del número de lote afectado. Campo agregado en v04.00, con el mismo criterio de conservación que el nombre.** |
| fecha_vencimiento | DATE | NOT NULL | Fecha de expiración que causó el retiro. |
| es_lote_defectuoso | BIT | NOT NULL, DEFAULT 0 | Indica si falló el control de calidad físico. |
| alerta_digemid | NVARCHAR(100) | Opcional | Código de resolución o alerta sanitaria de DIGEMID. |
| fecha_registro | DATE | NOT NULL | Fecha en la que el sistema procesó la baja o retiro. |

*Educción asociada: EDU-0004. La ausencia de `id_producto` es una decisión de diseño ya evaluada, no un error pendiente: el medicamento se obtiene navegando `id_lote` → `TBL_LOTES.id_producto`, y las copias de texto se conservan para la lectura histórica.*

*Cambio en v06.00: se agrega `id_lote`. Es anulable a propósito: si ILA-0044 elimina físicamente una remesa sin ventas ni devoluciones, el retiro conserva su histórico gracias a las copias de texto, y el módulo de Lotes no necesita consultar esta tabla (lo que habría creado una dependencia Lotes → Alertas de Vencimiento que la arquitectura no tiene).*

## 11. TBL_ORDENES_DEVOLUCION

Cabeceras de los trámites de devolución despachados a los laboratorios y proveedores.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_devolucion | INT | PK, IDENTITY(1,1) | Identificador único de la orden de devolución. |
| fecha_creacion | DATE | NOT NULL | Fecha en que el usuario registró la orden. |
| motivo_devolucion | NVARCHAR(50) | NOT NULL, CHECK (motivo_devolucion IN ('Por Vencimiento', 'Por Depuracion de Error')) | Criterios: 'Por Vencimiento', 'Por Depuracion de Error'. |
| id_proveedor | INT | NOT NULL, FK (TBL_PROVEEDORES) | Proveedor al que se remite la mercancía. Una vez registrada la orden no se modifica (DEC-038). |
| comentario | NVARCHAR(MAX) | Opcional | Notas aclaratorias u observaciones. Obligatorio cuando el motivo es la depuración de un error. |
| estado_orden | NVARCHAR(15) | NOT NULL, DEFAULT 'Pendiente', CHECK (estado_orden IN ('Pendiente', 'Completada', 'Eliminada')) | Estados: 'Pendiente', 'Completada', 'Eliminada'. |
| id_usuario | INT | NOT NULL, FK (TBL_USUARIOS) | Usuario que registró el movimiento. |

Restricción adicional: `CHECK (motivo_devolucion <> 'Por Depuracion de Error' OR (comentario IS NOT NULL AND LEN(LTRIM(RTRIM(comentario))) > 0))`. La expresión comprueba la presencia del comentario de forma explícita, para que un NULL no la deje pasar como desconocido (DEC-030).

*Educción asociada: EDU-0011. El paso a 'Completada' descuenta el stock en ILA-0027; 'Eliminada' es la baja lógica de ILA-0028.*

## 12. TBL_DETALLE_DEVOLUCION

Desglosa qué medicamentos van dentro de una orden de devolución.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_detalle_dev | INT | PK, IDENTITY(1,1) | Identificador de la línea de devolución. |
| id_devolucion | INT | NOT NULL, FK (TBL_ORDENES_DEVOLUCION) | Enlace a la cabecera de la devolución. |
| id_lote | INT | NOT NULL, FK (TBL_LOTES) | Remesa que se extraerá del almacén. |
| cantidad_devolver | INT | NOT NULL, CHECK (cantidad_devolver > 0) | Unidades físicas devueltas al proveedor. |

Restricción adicional: `UNIQUE (id_devolucion, id_lote)`. Una remesa aparece una sola vez por orden; si se agrega de nuevo, se acumula la cantidad (DEC-036).

*Dependencia de existencia: el detalle no existe sin su cabecera. La relación es no identificadora porque el detalle tiene clave propia (`id_detalle_dev`).*

*Cambio en v04.00: `id_producto` se sustituyó por `id_lote`. El proveedor tramita la devolución contra un número de lote; una orden que solo nombrara el medicamento no servía para el trámite real.*

## 13. TBL_RESTRICCIONES_VENTA

Reglas de seguridad clínica para protección en el punto de venta.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_restriccion | INT | PK, IDENTITY(1,1) | Identificador único de la regla restrictiva. |
| id_producto | INT | NOT NULL, FK (TBL_PRODUCTOS) | Medicamento afectado por la condición sanitaria. Un producto puede tener varias restricciones activas y la venta las evalúa todas (DEC-036). |
| condicion_cliente | NVARCHAR(50) | NOT NULL | Grupo de riesgo, por ejemplo 'Gestante', 'Neonato', 'Oncologico'. Texto libre mientras la dueña no confirme si es una lista cerrada (PEN-025). |
| tipo_restriccion | NVARCHAR(25) | NOT NULL, CHECK (tipo_restriccion IN ('Informativa', 'Bloqueante')) | Modos operacionales: 'Informativa' o 'Bloqueante'. |
| motivo_advertencia | NVARCHAR(MAX) | NOT NULL | Texto explícito que se muestra en la pantalla del cajero. |
| estado_alerta | NVARCHAR(10) | NOT NULL, DEFAULT 'Activo', CHECK (estado_alerta IN ('Activo', 'Inactivo')) | Vigencia de la restricción: 'Activo' o 'Inactivo'. |

*Educción asociada: EDU-0012. ILA-0001 consulta esta tabla antes de agregar un producto a la venta.*

## 14. TBL_REPORTES_VENTAS

Índice, trazabilidad y almacenamiento de los informes generados.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_reporte | INT | PK, IDENTITY(1,1) | Identificador único del reporte emitido. |
| tipo_reporte | NVARCHAR(50) | NOT NULL | Criterio de agrupación de las ventas consolidadas, por ejemplo 'Ventas Diarias'. El dominio cerrado está pendiente de confirmación por la dueña (ver ESP-0021). |
| **formato** | **NVARCHAR(10)** | **NOT NULL, CHECK (formato IN ('PDF', 'Excel', 'HTML'))** | **Formato del archivo generado: 'PDF', 'Excel' o 'HTML'. Campo agregado en v06.00: ESP-0023 regenera el reporte en este formato.** |
| fecha_inicio | DATE | NOT NULL | Fecha desde la cual se extrajeron los datos consolidados. |
| fecha_fin | DATE | NOT NULL, CHECK (fecha_fin >= fecha_inicio) | Fecha hasta la cual se extrajeron los datos consolidados (inclusive: las ventas de ese día entran completas, DEC-037). |
| **total_recaudado** | **DECIMAL(10,2)** | **NOT NULL, CHECK (total_recaudado >= 0)** | **Suma de `monto_total` de las ventas del intervalo. Campo agregado en v06.00: ESP-0022 lo muestra en REP-TBL-REPORTES y ESP-0023 lo recalcula.** |
| fecha_generacion | DATETIME2(0) | NOT NULL | Auditoría de cuándo se procesó el archivo. |
| ruta_archivo | NVARCHAR(255) | NOT NULL | Dirección física o URL de descarga del documento. |

*Educción asociada: EDU-0010. Es un log de salida sin referencias foráneas, por lo que su eliminación física en ILA-0024 es legítima.*

*Aclaración sobre `usuario_responsable`: el reporte incluye la identificación del personal que realizó cada venta (EDU-0010), pero **`usuario_responsable` no es una columna de esta tabla ni de ninguna otra**. Es un valor calculado al vuelo, que ESP-0021 obtiene al generar el reporte uniendo `TBL_REGISTRO_VENTAS.id_usuario` con `TBL_USUARIOS`, y que solo existe dentro del archivo de salida (`ruta_archivo`). Nunca se persiste en `TBL_REPORTES_VENTAS`, que conserva únicamente el índice del reporte.*

*Cambio en v06.00: se descarta el tipo 'Mermas', que no tenía respaldo en ningún requisito, y se agregan `formato` y `total_recaudado`, que ESP-0022 y ESP-0023 ya usaban sin que existieran en la tabla.*

## 15. TBL_LISTA_REABASTECIMIENTO

**Nueva en v05.00.** Mantiene los medicamentos que deben cotizarse y reabastecerse, ya sea porque el proceso periódico del sistema detectó que su stock vendible cayó por debajo de `stock_minimo`, o porque la dueña los agregó manualmente.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_reabastecimiento | INT | PK, IDENTITY(1,1) | Identificador único del ítem de la lista. |
| id_producto | INT | NOT NULL, FK (TBL_PRODUCTOS), UNIQUE | Medicamento a reabastecer. Un medicamento figura a lo sumo una vez en la lista. |
| origen | NVARCHAR(20) | NOT NULL, DEFAULT 'Automatico', CHECK (origen IN ('Automatico', 'Manual')) | Valores: 'Automatico' (generado por el proceso periódico), 'Manual' (agregado por la dueña, ESP-0045). |
| prioridad | NVARCHAR(10) | NOT NULL, DEFAULT 'Normal', CHECK (prioridad IN ('Normal', 'Urgente')) | Valores: 'Normal', 'Urgente'. Determina el resaltado visual en rojo. |
| fecha_inclusion | DATE | NOT NULL | Fecha en que el ítem entró a la lista. |

*Educción asociada: EDU-0016. Ilaciones: ILA-0045 a ILA-0048.*

*Restricción de unicidad sobre `id_producto`: un medicamento figura a lo sumo una vez en la lista. La restricción no impide que reaparezca después de que su fila se eliminó (por ejemplo, tras ser pedido y volver a caer bajo el umbral), porque la fila anterior ya no existe. ESP-0045 valida que el medicamento no figure antes de agregarlo, y el proceso 2 de RNF-0010, que se ejecuta en segundo plano y es la única ejecución concurrente del sistema, trata el rechazo por duplicado como "ya figura" y continúa con el siguiente medicamento.*

*Fuente: ESP-0045 (creación manual), ESP-0046 (consulta), ESP-0047 (actualización), ESP-0048 (eliminación). La escribe también el proceso periódico de generación automática, documentado en la condición 2 de RNF-0010.*

## 16. TBL_COTIZACIONES_PROVEEDOR

**Nueva en v05.00.** Conserva el historial completo de precios que cada proveedor ha cotizado por cada medicamento, permitiendo comparar el precio vigente de cada proveedor sin perder las cotizaciones anteriores.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_cotizacion | INT | PK, IDENTITY(1,1) | Identificador único de la cotización. |
| id_producto | INT | NOT NULL, FK (TBL_PRODUCTOS) | Medicamento cotizado. |
| id_proveedor | INT | NOT NULL, FK (TBL_PROVEEDORES) | Proveedor que ofreció el precio. |
| precio_cotizado | DECIMAL(10,2) | NOT NULL, CHECK (precio_cotizado >= 0) | Precio ofrecido por el proveedor para este medicamento. |
| fecha_cotizacion | DATE | NOT NULL | Fecha en que se obtuvo la cotización. Determina cuál es la "vigente" por proveedor (la más reciente; si hay dos del mismo día, la de mayor `id_cotizacion`, DEC-039). |
| promocion | NVARCHAR(MAX) | Opcional | Notas sobre condiciones de descuento o promoción asociadas al precio cotizado. |
| id_usuario | INT | NOT NULL, FK (TBL_USUARIOS) | Usuario que registró la cotización. |

*Educción asociada: EDU-0017. Ilaciones: ILA-0049 a ILA-0052.*

*Sin restricción de unicidad sobre (`id_producto`, `id_proveedor`): es intencional, porque permite el historial de cotizaciones (decisión confirmada con la dueña). La cotización vigente de cada proveedor se resuelve en consulta como la de `fecha_cotizacion` más reciente para esa combinación de medicamento y proveedor; si hay empate de fecha, gana la de mayor `id_cotizacion` (el registro más reciente, no necesariamente la cotización comercial posterior). Es provisional hasta que la dueña responda (PEN-023).*

*Fuente: ESP-0049 (registro), ESP-0050 (consulta comparativa y exportación), ESP-0051 (actualización), ESP-0052 (eliminación).*

---

## Correspondencia de tipos entre base de datos, código e interfaz

Esta tabla es el criterio único para decidir el tipo de un campo en las tres capas. Toda ilación y toda especificación debe respetarla.

| Tipo lógico del dato | SQL Server 2025 Express | Tipo en C# / .NET 10 LTS | Control WPF | Prefijo de nomenclatura |
| --- | --- | --- | --- | --- |
| Número entero | `INT` | `int` | TextBox con validación numérica | **NUM** |
| Clave autogenerada | `INT IDENTITY(1,1)` | `int` | No se captura | **LBL** |
| Importe monetario | `DECIMAL(10,2)` | `decimal` | TextBox con formato de moneda | **NUM** |
| Texto corto | `NVARCHAR(n)` | `string` | TextBox | **TXT** |
| Texto extenso | `NVARCHAR(MAX)` | `string` | TextBox multilínea | **TXA** |
| Fecha | `DATE` | `DateOnly` o `DateTime` | DatePicker | **FEC** |
| Fecha y hora | `DATETIME2(0)` (sin fracciones de segundo) | `DateTime` | DatePicker, o solo lectura si la asigna el sistema | **FEC** o **LBL** |
| Valor lógico | `BIT NOT NULL DEFAULT 0` (el valor por defecto se escribe `0`, no `FALSE`) | `bool` | CheckBox | **CHK** |
| Dominio cerrado de valores | `NVARCHAR(n)` con restricción CHECK | `string` o `enum` | ComboBox | **CMB** |
| Referencia a otra entidad | `INT` con clave foránea | `int` | ComboBox | **CMB** |
| Contraseña | `NVARCHAR(255)` con el hash de la contraseña | `string` | PasswordBox | **PWD** |
| Valor calculado por el sistema | No se persiste, o se persiste sin captura | según el cálculo | TextBlock | **LBL** |

**Nunca usar `float` ni `double` para importes.** `DECIMAL(10,2)` corresponde a `decimal` en C#, y cualquier otra elección introduce errores de redondeo en `monto_total` y `precio_unitario`.

**Campos con dominio cerrado.** Los siguientes campos admiten un conjunto fijo de valores y se capturan siempre con un ComboBox, nunca escribiéndolos: `rol`, `estado`, `estado_producto`, `estado_documento`, `estado_orden`, `estado_alerta`, `tipo_comprobante`, `tipo_restriccion`, `motivo_devolucion`, `origen` y `prioridad`. La lista completa incluye también `estado_lote` y `formato`. Todos se declaran con una restricción `CHECK (columna IN (...))` en el motor, con los literales exactos de cada tabla: es obligatorio, para que la validación no dependa únicamente de la interfaz (DEC-030). `tipo_reporte` queda abierto hasta cerrar PEN-001, y `condicion_cliente` y `accion_terapeutica` son texto libre. No hace falta una tabla de catálogo para estos dominios: son fijos y gobiernan reglas del código.

**Literales sin tilde.** Los valores de dominio se escriben sin tilde para que coincidan con los `enum` de C#: `'Automatico'` y no `'Automático'` (DEC-031).

---

## Resumen de relaciones

| Relación | Tipo | Justificación |
| --- | --- | --- |
| `TBL_PRODUCTOS` → `TBL_LOTES` | No identificadora obligatoria, 1:N | Nueva en v04.00. Un medicamento tiene varias remesas; una remesa no existe sin su medicamento (FK obligatoria), pero tiene clave propia (`id_lote`). |
| `TBL_REGISTRO_VENTAS` → `TBL_DETALLE_VENTAS` | No identificadora obligatoria, 1:N | El detalle no existe sin la venta cabecera (dependencia de existencia), pero tiene clave propia. |
| `TBL_ORDENES_DEVOLUCION` → `TBL_DETALLE_DEVOLUCION` | No identificadora obligatoria, 1:N | El detalle no existe sin la orden cabecera (dependencia de existencia), pero tiene clave propia. |
| `TBL_REGISTRO_VENTAS` → `TBL_COMPROBANTES_TRIBUTARIOS` | No identificadora, 1:N | Un comprobante corresponde siempre a una venta, y una venta puede tener varios comprobantes con a lo sumo uno 'Emitido' (ESP-0009). El comprobante tiene clave propia. |
| `TBL_USUARIOS` → `TBL_REGISTRO_VENTAS` | No identificadora, 1:N | Nueva en v03.00. Un usuario registra muchas ventas. |
| `TBL_USUARIOS` → `TBL_ORDENES_DEVOLUCION` | No identificadora, 1:N | Un usuario registra muchas órdenes de devolución. |
| `TBL_PROVEEDORES` → `TBL_PRODUCTOS` | No identificadora, 1:N | Un proveedor abastece muchos productos. |
| `TBL_PROVEEDORES` → `TBL_ORDENES_DEVOLUCION` | No identificadora, 1:N | Un proveedor recibe muchas devoluciones. |
| `TBL_METODOS_PAGO` → `TBL_REGISTRO_VENTAS` | No identificadora, 1:N | Un método de pago se usa en muchas ventas. |
| `TBL_LOTES` → `TBL_DETALLE_VENTAS` | No identificadora, 1:N | Modificada en v04.00. Una remesa aparece en muchas líneas de venta. |
| `TBL_LOTES` → `TBL_DETALLE_DEVOLUCION` | No identificadora, 1:N | Modificada en v04.00. Una remesa aparece en muchas líneas de devolución. |
| `TBL_PRODUCTOS` → `TBL_RESTRICCIONES_VENTA` | No identificadora, 1:N | Un producto puede tener varias restricciones sanitarias. |
| `TBL_PROVEEDORES` → `TBL_LOTES` | No identificadora, 1:N | Nueva en v05.00. Un proveedor entrega muchas remesas; cada remesa registra su proveedor real. |
| `TBL_PRODUCTOS` → `TBL_LISTA_REABASTECIMIENTO` | No identificadora, 1:0..1 | Nueva en v05.00. En cada momento un medicamento figura a lo sumo una vez (UNIQUE, DEC-020); puede reaparecer después de eliminarse su ítem. Corregida en v10.00. |
| `TBL_PRODUCTOS` → `TBL_COTIZACIONES_PROVEEDOR` | No identificadora, 1:N | Nueva en v05.00. Un medicamento acumula cotizaciones de varios proveedores y en varias fechas. |
| `TBL_PROVEEDORES` → `TBL_COTIZACIONES_PROVEEDOR` | No identificadora, 1:N | Nueva en v05.00. Un proveedor ofrece muchas cotizaciones. |
| `TBL_USUARIOS` → `TBL_COTIZACIONES_PROVEEDOR` | No identificadora, 1:N | Nueva en v05.00. Un usuario registra muchas cotizaciones. |
| `TBL_LOTES` → `TBL_PRODUCTOS_RETIRADOS` | No identificadora, 1:N | Nueva en v06.00. Una remesa puede acumular varios retiros sanitarios a lo largo del tiempo. |

`TBL_ALERTAS_VENCIMIENTO` y `TBL_REPORTES_VENTAS` no participan en relaciones foráneas, por las razones documentadas en cada tabla.

---

## Acciones referenciales

Todas las claves foráneas se declaran `ON DELETE NO ACTION ON UPDATE NO ACTION`, salvo `TBL_PRODUCTOS_RETIRADOS.id_lote`, que es `ON DELETE SET NULL ON UPDATE NO ACTION` (DEC-006). Ninguna clave foránea usa `CASCADE`: los detalles de una venta o de una orden los elimina la operación (ESP-0004, ESP-0028), no el motor. Los identificadores son autogenerados y no se editan, así que `ON UPDATE NO ACTION` no limita ninguna operación.

---

## Convención de nombres de restricciones

Las tablas y las columnas no se renombran. Las restricciones se nombran explícitamente, para que el error 547 de la baja condicional (DEC-009, DEC-029) y los demás mensajes del motor sean legibles:

| Tipo | Patrón | Ejemplo |
| --- | --- | --- |
| Clave primaria | `PK_<tabla>` | `PK_TBL_LOTES` |
| Clave foránea | `FK_<hija>_<padre>[_<columna>]` | `FK_TBL_LOTES_TBL_PRODUCTOS` |
| Unicidad | `UQ_<tabla>_<columnas>` | `UQ_TBL_DETALLE_VENTAS_id_venta_id_lote` |
| Único filtrado | `UX_<tabla>_<nombre>` | `UX_TBL_ALERTAS_VENCIMIENTO_Activa` |
| Verificación | `CK_<tabla>_<columna>` | `CK_TBL_LOTES_estado_lote` |
| Índice | `IX_<tabla>_<columnas>` | `IX_TBL_LOTES_FEFO` |
| Vista | `VW_<nombre>` | `VW_STOCK_FISICO` |

Si un `CHECK` involucra más de una columna, se usa un nombre descriptivo en lugar de la columna, por ejemplo `CK_TBL_LOTES_estado_stock`.

---

## Índices

Además de los que crean las claves primarias y las restricciones `UNIQUE`, se declaran solo los que piden las consultas de las especificaciones (DEC-041). No se indexan por defecto todas las claves foráneas ni las columnas de estado.

| Índice | Columnas | Consulta que lo justifica |
| --- | --- | --- |
| `IX_TBL_LOTES_FEFO` | `(id_producto, estado_lote, fecha_vencimiento)` INCLUDE `(stock_actual)` | Selección FEFO de ESP-0001 y ESP-0042. |
| `IX_TBL_LOTES_Proveedor` | `(id_proveedor, fecha_vencimiento)` | Combo de remesas de ESP-0025 (DEC-023). |
| `IX_TBL_REGISTRO_VENTAS_FechaHora` | `(fecha_hora)` | Ventas de un día o de un rango: ESP-0002, ESP-0021, ESP-0023. |
| `IX_TBL_DETALLE_VENTAS_Lote` | `(id_lote)` | Ventas de una remesa: ESP-0043, ESP-0044. |
| `IX_TBL_DETALLE_DEVOLUCION_Lote` | `(id_lote)` | Devoluciones pendientes de una remesa: ESP-0016, ESP-0044. |
| `IX_TBL_COTIZACIONES_PROVEEDOR_Vigente` | `(id_producto, id_proveedor, fecha_cotizacion DESC, id_cotizacion DESC)` | Cotización vigente de ESP-0050 (DEC-039). |

Las búsquedas por `id_venta` y por `id_devolucion` las cubren las restricciones `UNIQUE (id_venta, id_lote)` y `UNIQUE (id_devolucion, id_lote)`. Los índices únicos filtrados de comprobantes y de alertas están en sus tablas.

---

## Stock físico y stock vendible

El stock consolidado de un medicamento no se guarda: se calcula sumando sus remesas. Se distinguen dos definiciones (DEC-040):

- **Stock físico:** suma de `stock_actual` de todas las remesas del medicamento, incluidas las bloqueadas, agotadas o vencidas. Es lo que muestra Inventario (ESP-0006).
- **Stock vendible:** suma de `stock_actual` de las remesas en estado 'Disponible' y no vencidas. Es lo que se compara con `stock_minimo` en el proceso 2 de RNF-0010 (reabastecimiento).

Es provisional hasta que la dueña confirme qué debe mostrar Inventario y qué debe disparar el reabastecimiento (PEN-024).

Ambos valores se obtienen de las vistas `VW_STOCK_FISICO` y `VW_STOCK_VENDIBLE` de la sección siguiente.

---

## Vistas del repositorio

Vistas de solo lectura. No agregan columnas ni persisten valores: el stock consolidado sigue sin guardarse (DEC-040). Cada vista pertenece al módulo dueño de las tablas que consulta, que es el único que puede cambiar su definición; los módulos consumidores solo leen a través de ella (DEC-055, DEC-056).

| Vista | Dueño | Columnas | Definición | La lee |
| --- | --- | --- | --- | --- |
| `VW_STOCK_FISICO` | Lotes (LOT) | `id_producto` INT, `stock_fisico` INT | Suma de `stock_actual` de `TBL_LOTES` agrupada por `id_producto`, sin filtrar por estado ni vencimiento. | Inventario: `STOCK_FISICO(id_producto)` de ESP-0006. |
| `VW_STOCK_VENDIBLE` | Lotes (LOT) | `id_producto` INT, `stock_vendible` INT | Suma de `stock_actual` de `TBL_LOTES` con `estado_lote` = 'Disponible' y `fecha_vencimiento` posterior a la fecha actual, agrupada por `id_producto`. | Reabastecimiento: proceso 2 de RNF-0010, que la compara con `stock_minimo`. |

Las vistas solo leen `TBL_LOTES`. Un medicamento sin remesas, o sin remesas vendibles, no aparece. El consumidor lo trata como 0: Inventario une la vista a su propia `TBL_PRODUCTOS` con `LEFT JOIN` y `COALESCE`, y Reabastecimiento hace lo mismo con los medicamentos que obtiene por `IInventario` (DEC-058).

Si la dueña cambia la definición de alguno de los dos stocks (PEN-024), se modifica solo la vista; los consumidores no cambian.


---

## Plataforma y base de datos

El sistema se construye con .NET 10 LTS y SQL Server 2025 Express (DEC-028). La base `DB_FARMASIL` se crea con la intercalación `Modern_Spanish_CI_AS` (DEC-031): no distingue mayúsculas, pero sí distingue tildes, por lo que los literales de dominio se escriben sin tilde y de forma idéntica en los `CHECK` y en el código.

---

## Regla de selección de lote en la venta

Al existir varias remesas de un mismo medicamento, la venta necesita un criterio para decidir de cuál descuenta. El catálogo adopta **primero el que vence antes** (criterio FEFO, *first expired, first out*), que es la práctica habitual en farmacia y la que minimiza la merma por vencimiento.

Operativamente: al seleccionar un medicamento, el sistema propone por defecto la remesa vendible con la `fecha_vencimiento` más próxima, y permite al usuario elegir otra cuando la remesa física que tiene en la mano es distinta.

**Predicado de vendibilidad** (DEC-033). Una remesa se puede vender solo si se cumplen las cuatro condiciones: el producto está en `'Disponible'`, la remesa está en `'Disponible'`, `stock_actual > 0` y `fecha_vencimiento` es posterior a la fecha de la venta. El mismo predicado se aplica al proponer la remesa FEFO, al llenar el combo de remesas, al agregar una línea (ESP-0001) o una línea nueva al editar la venta (ESP-0003) y al confirmar la venta. Los mensajes son distintos: «La remesa está vencida y no puede venderse» y «El medicamento está descontinuado». Las devoluciones no usan este predicado: conservan sus propias reglas, que sí admiten remesas vencidas (DEC-023).

Esta regla la aplican ILA-0001 y ESP-0001, y la reutiliza ILA-0027 al completar una devolución.

---

## Vacíos del modelo pendientes de decisión

Estos puntos no se modificaron porque implican decisiones de alcance, no correcciones. Se listan aquí para que el equipo los resuelva de forma consciente y no por omisión.

1. ~~**Múltiples lotes por medicamento.**~~ **Resuelto en la versión 04.00** con la incorporación de `TBL_LOTES` y del módulo 11.
2. **Clientes.** No existe `TBL_CLIENTES` ni relación desde la venta, pese a que en la Entrevista 1 la dueña confirmó que quiere registrar clientes, llevar historial de compras y manejar clientes frecuentes.
3. ~~**Stock mínimo.**~~ **Resuelto en la versión 05.00** con `TBL_PRODUCTOS.stock_minimo` y la tabla `TBL_LISTA_REABASTECIMIENTO` (EDU-0016).
4. **Tipo y presentación del producto.** No existe un campo que distinga medicamento genérico de comercial, ni que permita un precio por unidad y otro por conjunto, ambos solicitados en la entrevista.
5. **Gestión de la configuración de alertas.** `TBL_ALERTAS_VENCIMIENTO` no tiene ninguna educción que la administre: el umbral es hoy un valor fijo que nadie puede cambiar desde el sistema, y la fila inicial la carga el script de datos iniciales (PEN-009, PEN-011, DEC-032).


**Versión:** 11.00
**Fecha:** 09/11/2026
**Autor:** AUT-0001
**Origen:** Diccionario de Datos v02.00, `Cambios_Modelo_ER_DB_FARMASIL.md` y Entrevista 3 (v05.00) y auditorías de la base de datos (`seguimiento/13-auditoria-bd-claude.md` y `seguimiento/13-auditoria-bd-gpt.md`, issue #63)

> **Aplicado en v11.00 (09/11/2026, arquitectura centrada en datos):** nueva sección «Vistas del repositorio» con `VW_STOCK_FISICO` y `VW_STOCK_VENDIBLE`, propiedad de Lotes y de solo lectura, sin columnas nuevas (DEC-040, DEC-056). Inventario deja de leer `TBL_LOTES` para mostrar el stock físico. Se agrega el patrón `VW_<nombre>` a la convención de nombres. Las vistas solo leen `TBL_LOTES`, y un medicamento sin remesas no aparece: el consumidor lo trata como 0 (DEC-058).

> **Aplicado en v10.00 (08/11/2026, auditorías de la base de datos, issue #63):**
>
> - **Nulabilidad (AUD-claude-003, 004, 011; AUD-gpt-005, 009).** `NOT NULL` en las diez claves foráneas obligatorias y en todas las columnas de estado que solo tenían `DEFAULT`. La única clave foránea anulable es `TBL_PRODUCTOS_RETIRADOS.id_lote` (DEC-006).
> - **Dominios y valores (AUD-claude-010, 012, 025, 028; AUD-gpt-009, 010, 011, 012, 020).** `CHECK (columna IN (...))` en todos los dominios cerrados, incluidos `estado_lote` y `formato`; `CHECK` numéricos y de fechas; comentario obligatorio con depuración de error; RUC de once dígitos. Literales de dominio sin tilde (`'Automatico'`) e intercalación `Modern_Spanish_CI_AS` (DEC-030, DEC-031).
> - **Estado de la remesa (AUD-claude-001; AUD-gpt-002).** `CHECK (estado_lote <> 'Disponible' OR stock_actual > 0)` y se quita el `DEFAULT 0` de `stock_actual` (DEC-034).
> - **Unicidad (AUD-claude-005, 009, 013; AUD-gpt-006, 014).** `UNIQUE (id_venta, id_lote)` y `UNIQUE (id_devolucion, id_lote)`; índices únicos filtrados para el comprobante 'Emitido' de cada venta y para la regla de alertas activa (DEC-030, DEC-032, DEC-036).
> - **Alertas (AUD-claude-008; AUD-gpt-015).** `fecha_ultima_verif` pasa a anulable; NULL significa «todavía no evaluada» (DEC-032, que actualiza DEC-018).
> - **Tipos (AUD-claude-024; AUD-gpt-021).** `INT IDENTITY(1,1)`, `DATETIME2(0)` y `BIT NOT NULL DEFAULT 0` (DEC-041).
> - **Referencias (AUD-claude-007).** Acciones `ON DELETE` y `ON UPDATE` explícitas, y se corrige la nota que decía que la eliminación de una venta «arrastra» sus líneas.
> - **Convenciones (AUD-claude-026, 027, 029; AUD-gpt-023).** Nombres de restricciones, índices y formato de `contrasena` (DEC-041).
> - **Coherencia con el catálogo (AUD-claude-016, 023; AUD-gpt-001, 016, 019).** Multiplicidad 1:0..1 de reabastecimiento, definición de stock físico y stock vendible (DEC-040) y predicado de vendibilidad en la regla de selección de lote (DEC-033).
> - **Plataforma (AUD-gpt-022).** .NET 10 LTS y SQL Server 2025 Express (DEC-028).

> **Aplicado en v09.00 (01/11/2026):** `TBL_LOTES.stock_actual` incorpora `CHECK (stock_actual >= 0)`, y ESP-0027 valida el stock de cada remesa antes de completar una devolución (DEC-019). `TBL_LISTA_REABASTECIMIENTO.id_producto` pasa a NOT NULL y UNIQUE, y se corrige la nota que justificaba su ausencia (DEC-020).

> **Aplicado en v08.00 (18/10/2026):** las relaciones con clave propia del hijo pasan a no identificadoras (`TBL_PRODUCTOS` → `TBL_LOTES`, cabeceras → detalles); la relación venta–comprobante pasa de 1:1 a 1:N, con a lo sumo un comprobante 'Emitido' por venta (PEN-014); `id_venta` e `id_devolucion` de los detalles pasan a NOT NULL; `fecha_ultima_verif` la actualiza el proceso 1 de RNF-0010 (PEN-015).

> **Aplicado en v07.00 (13/10/2026):** `TBL_USUARIOS.contrasena` guarda un hash con sal y no un valor encriptado, conforme a RNF-0006 v05.00.

> **Aplicado en v06.00 (auditoría de coherencia, 12-13/10/2026):**
>
> - **Regla de integridad referencial.** Todas las claves foráneas se declaran sin borrado en cascada (`ON DELETE NO ACTION`), salvo `TBL_PRODUCTOS_RETIRADOS.id_lote` (`ON DELETE SET NULL`). Las bajas condicionales del catálogo (ESP-0008, ESP-0020, ESP-0036, ESP-0040, ESP-0044) dependen de esta regla: intentan el borrado físico y, si el motor lo rechaza por una referencia, aplican la baja lógica.
> - `TBL_PRODUCTOS_RETIRADOS`: se agrega `id_lote` (FK a `TBL_LOTES`). ESP-0014 la registra y ESP-0016 la usa para restituir la remesa. Es anulable (`ON DELETE SET NULL`) para no crear una dependencia de Lotes hacia Alertas de Vencimiento.
> - `TBL_REPORTES_VENTAS`: se agregan `formato` y `total_recaudado`, que ESP-0022 y ESP-0023 ya usaban, y se retira el ejemplo 'Mermas' de `tipo_reporte`.
> - Se aclara que `usuario_responsable` es un valor calculado al generar el reporte y no una columna.

> **Aplicado en v05.00 (módulo de reabastecimiento y comparación de precios de proveedores, derivado de la Entrevista 3, 02/10/2026):**
>
> - `TBL_PRODUCTOS`: se agrega `stock_minimo`, el umbral de existencias consolidadas por debajo del cual el medicamento se incorpora automáticamente a la lista de reabastecimiento (EDU-0016). Se captura en ESP-0005/ESP-0007.
> - `TBL_LOTES`: se agrega `id_proveedor` (FK a `TBL_PROVEEDORES`), el proveedor real que entregó esa remesa específica. Resuelve una limitación detectada durante el diseño de EDU-0017: un mismo medicamento puede recibirse de más de un proveedor en remesas distintas, algo que `TBL_PRODUCTOS.id_proveedor` (proveedor habitual, a nivel de catálogo) no puede representar. Exige corregir ESP-0025/ILA-0025 y ampliar ESP-0041/ILA-0041 y ESP-0043/ILA-0043.
> - Nueva tabla **`TBL_LISTA_REABASTECIMIENTO`** (EDU-0016).
> - Nueva tabla **`TBL_COTIZACIONES_PROVEEDOR`** (EDU-0017).
>
> **Cambios aplicados en la versión 04.00.** Los cuatro cambios pendientes identificados durante la revisión de las ilaciones quedaron incorporados al esquema:
>
> | # | Tabla | Cambio | Requisito que lo exige |
> | --- | --- | --- | --- |
> | 1 | `TBL_REGISTRO_VENTAS` | Se agregó `id_usuario` como clave foránea hacia `TBL_USUARIOS` | EDU-0010 y EDU-0013. La dueña confirmó que requiere saber qué personal realiza cada venta. `TBL_ORDENES_DEVOLUCION` ya registraba al usuario responsable; la tabla de ventas no. Desbloquea ILA-0021. |
> | 2 | `TBL_PRODUCTOS` | Se agregó `numero_lote` | EDU-0002 y EDU-0004. El campo ya figuraba en el modelo ER pero nunca se trasladó al diccionario. Desbloquea ILA-0005. |
> | 3 | `TBL_PRODUCTOS` | Se amplió el dominio de `estado_producto` con el valor `'Descontinuado'` | ILA-0008. Necesario para la baja lógica de productos con histórico de ventas. |
> | 4 | `TBL_COMPROBANTES_TRIBUTARIOS` | Se agregó `numero_comprobante` con restricción de unicidad | ILA-0009. `id_comprobante` es un identificador interno autoincremental, no el número de serie del documento fiscal, que es el dato que se imprime y se declara. |
>
> **Alineación con el motor de base de datos (v03.00).** El diccionario declaraba tipos genéricos que no existen como tales en SQL Server Express, que es el motor exigido por RNF-0008:
>
> | Tipo anterior | Tipo corregido | Motivo |
> | --- | --- | --- |
> | `BOOLEAN` | `BIT` | SQL Server no tiene tipo booleano; el equivalente es `BIT`. Afectaba a `es_lote_defectuoso`. |
> | `TEXT` | `NVARCHAR(MAX)` | `TEXT` está obsoleto en SQL Server y no admite las operaciones de comparación habituales. Afectaba a `comentario` y `motivo_advertencia`. |
> | `VARCHAR(n)` | `NVARCHAR(n)` | Necesario para almacenar tildes y la letra ñ sin depender de la intercalación del servidor. Afecta a razones sociales, nombres de medicamentos y motivos de advertencia. |
> | `INTEGER` | `INT` | `INTEGER` no es la palabra reservada de SQL Server. |
> | `DATETIME` | `DATETIME2` | `DATETIME` está desaconsejado por Microsoft; `DATETIME2` es el tipo que corresponde a `DateTime` de .NET con precisión completa. |
>
> Adicionalmente se reconstruyó la totalidad del documento en formato de tabla legible: la versión 02.00 provenía de una exportación que había fragmentado las columnas y dejado descripciones partidas fuera de sus celdas, lo que hacía imposible leer varias definiciones.
>
> **Incorporación de `TBL_LOTES` (v04.00).** Tras la decisión del equipo se resolvió el punto más frágil del modelo. Hasta la versión 03.00 una fila de `TBL_PRODUCTOS` era simultáneamente el producto y su único lote, lo que impedía que un mismo medicamento conviviera en el estante en remesas con vencimientos distintos, obligaba a bloquear el producto entero cuando solo vencía una remesa, y hacía imposible rastrear qué lote se vendió ante una alerta sanitaria de DIGEMID, funcionalidad que la dueña pidió expresamente en la Sección 6 del Registro de Entrevista 1.
>
> | # | Tabla | Cambio |
> | --- | --- | --- |
> | 1 | `TBL_LOTES` | **Tabla nueva.** Recibe `numero_lote`, `fecha_vencimiento` y `stock_actual`, que salen de `TBL_PRODUCTOS`, más `estado_lote` y `fecha_ingreso`. Relación 1:N identificadora desde `TBL_PRODUCTOS`. |
> | 2 | `TBL_PRODUCTOS` | Queda como catálogo del medicamento: nombre, acción terapéutica, precio, proveedor y estado. Pierde `numero_lote`, `fecha_vencimiento` y `stock_actual`. El valor `'Bloqueado por devolucion'` se traslada a `estado_lote`, ya que el bloqueo afecta a una remesa concreta y no al medicamento completo. |
> | 3 | `TBL_DETALLE_VENTAS` | `id_producto` se sustituye por `id_lote`. Es el cambio que hace posible el rastreo sanitario: la venta queda vinculada a la remesa exacta que salió del estante. |
> | 4 | `TBL_DETALLE_DEVOLUCION` | `id_producto` se sustituye por `id_lote`. El proveedor tramita devoluciones por número de lote, no por medicamento. |
> | 5 | `TBL_PRODUCTOS_RETIRADOS` | Se agrega `numero_lote` a la copia histórica, para que el registro de retiro conserve la remesa afectada aunque el lote se elimine. |
> | 6 | `TBL_RESTRICCIONES_VENTA` | **Sin cambios.** Una restricción clínica aplica al principio activo, no a la remesa, y sigue referenciando `id_producto`. |
>
> Se incorpora además el **módulo 11, Gestión de lotes (EDU-0015)**, con las cuatro ilaciones y especificaciones que administran la nueva entidad. Los lotes no podían gestionarse dentro del módulo de inventario sin romper la regla de cuatro fases CRUD por educción que estructura este catálogo.

---