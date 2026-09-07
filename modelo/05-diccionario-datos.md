# Diccionario de Datos — DB_FARMASIL

**Versión:** 04.00
**Fecha:** 05/09/2026
**Autor:** AUT-0001
**Origen:** Diccionario de Datos v02.00 y `Cambios_Modelo_ER_DB_FARMASIL.md`

> **Cambios aplicados en esta versión.** Los cuatro cambios pendientes identificados durante la revisión de las ilaciones quedaron incorporados al esquema:
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

## 1. TBL_USUARIOS

Almacena las credenciales y perfiles del personal de la farmacia que accede al sistema.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_usuario | INT | PK, Auto-increment | Identificador único del usuario. |
| nombre_usuario | NVARCHAR(50) | NOT NULL, UNIQUE | Nombre de usuario para el inicio de sesión. |
| contrasena | NVARCHAR(255) | NOT NULL | Contraseña encriptada por seguridad, conforme a RNF-0006. |
| rol | NVARCHAR(20) | NOT NULL | Rol en el sistema: 'Administrador' o 'Tecnico'. |
| estado | NVARCHAR(10) | DEFAULT 'Activo' | Estado de la cuenta: 'Activo' o 'Inactivo'. |

*Educción asociada: EDU-0013. Ilaciones: ILA-0033 a ILA-0036.*

## 2. TBL_PROVEEDORES

Registra las droguerías y distribuidoras farmacéuticas que abastecen a la farmacia.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_proveedor | INT | PK, Auto-increment | Identificador único del proveedor. |
| ruc | NVARCHAR(11) | NOT NULL, UNIQUE | Identificación tributaria de la empresa. |
| razon_social | NVARCHAR(150) | NOT NULL | Nombre legal de la empresa proveedora. |
| telefono | NVARCHAR(15) | Opcional | Número telefónico de contacto. |
| estado | NVARCHAR(10) | DEFAULT 'Activo' | Control de proveedores vigentes: 'Activo' o 'Inactivo'. |

*Educción asociada: EDU-0014. Ilaciones: ILA-0037 a ILA-0040.*

## 3. TBL_PRODUCTOS

Catálogo de medicamentos e insumos médicos disponibles en el inventario.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_producto | INT | PK, Auto-increment | Identificador único del medicamento. |
| nombre | NVARCHAR(100) | NOT NULL | Nombre de marca del producto. |
| accion_terapeutica | NVARCHAR(150) | NOT NULL | Principio activo o categoría terapéutica. |
| precio_venta | DECIMAL(10,2) | NOT NULL | Precio al público por unidad o blíster. |
| estado_producto | NVARCHAR(25) | DEFAULT 'Disponible' | Estados: 'Disponible', 'Descontinuado'. |
| id_proveedor | INT | FK (TBL_PROVEEDORES) | Enlace para saber quién abastece este producto. |

*Educción asociada: EDU-0002. El estado 'Descontinuado' lo asigna ILA-0008.*

*Cambio en v04.00: `numero_lote`, `fecha_vencimiento` y `stock_actual` se trasladaron a `TBL_LOTES`. El valor `'Bloqueado por devolucion'` se retiró de este dominio y ahora vive en `estado_lote`: bloquear un medicamento completo porque una de sus remesas vence impedía vender las remesas sanas.*

## 4. TBL_LOTES

Remesas físicas de cada medicamento. Un producto del catálogo puede tener varios lotes conviviendo en el estante, con vencimientos y existencias distintos.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_lote | INT | PK, Auto-increment | Identificador único de la remesa. |
| id_producto | INT | NOT NULL, FK (TBL_PRODUCTOS) | Medicamento al que pertenece la remesa. |
| numero_lote | NVARCHAR(30) | NOT NULL | Número de lote impreso por el laboratorio. |
| fecha_vencimiento | DATE | NOT NULL | Fecha de caducidad de esta remesa. |
| stock_actual | INT | NOT NULL, DEFAULT 0 | Unidades físicas disponibles de esta remesa. |
| estado_lote | NVARCHAR(25) | DEFAULT 'Disponible' | Estados: 'Disponible', 'Bloqueado por devolucion', 'Agotado'. |
| fecha_ingreso | DATE | NOT NULL | Fecha en que la remesa ingresó al almacén. |

Restricción adicional: `UNIQUE (id_producto, numero_lote)`. El mismo laboratorio no repite número de lote para un mismo medicamento, y admitir duplicados haría ambiguo el rastreo sanitario.

*Educción asociada: EDU-0015. El estado 'Bloqueado por devolucion' lo asigna ILA-0014 y lo restituye ILA-0016.*

## 5. TBL_REGISTRO_VENTAS

Cabecera de las ventas realizadas en el punto de venta.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_venta | INT | PK, Auto-increment | Identificador único de la transacción de venta. |
| fecha_hora | DATETIME2 | NOT NULL | Fecha y hora exacta en la que se procesó el cobro. |
| monto_total | DECIMAL(10,2) | NOT NULL | Suma total cobrada al cliente, calculada a partir del detalle. |
| id_metodo_pago | INT | FK (TBL_METODOS_PAGO) | Método utilizado para la transacción. |
| **id_usuario** | **INT** | **NOT NULL, FK (TBL_USUARIOS)** | **Personal que registró la venta. Campo agregado en v03.00.** |

*Educción asociada: EDU-0001. El campo `id_usuario` lo alimenta ILA-0001 a partir de la sesión activa y lo consume ILA-0021 para el reporte.*

## 6. TBL_DETALLE_VENTAS

Desglosa los productos vendidos dentro de cada transacción. Es la tabla que permite vender varios productos en una sola venta.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_detalle_venta | INT | PK, Auto-increment | Identificador único de la línea de detalle. |
| id_venta | INT | FK (TBL_REGISTRO_VENTAS) | Enlace a la cabecera de la venta. |
| id_lote | INT | NOT NULL, FK (TBL_LOTES) | Remesa exacta de la que salió el producto vendido. |
| cantidad | INT | NOT NULL | Cantidad de unidades vendidas. |
| precio_unitario | DECIMAL(10,2) | NOT NULL | Precio del producto al momento de la venta. |

*Relación identificadora: el detalle no existe sin su cabecera. La eliminación de una venta arrastra sus líneas, según ILA-0004.*

*Cambio en v04.00: `id_producto` se sustituyó por `id_lote`. El medicamento se obtiene navegando `TBL_LOTES.id_producto`, y a cambio el sistema puede responder qué se vendió de una remesa concreta cuando DIGEMID retira un lote.*

## 7. TBL_COMPROBANTES_TRIBUTARIOS

Documentos fiscales emitidos formalmente por cada venta efectuada.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_comprobante | INT | PK, Auto-increment | Identificador interno del documento tributario. |
| **numero_comprobante** | **NVARCHAR(20)** | **NOT NULL, UNIQUE** | **Número de serie y correlativo impreso en el documento fiscal. Campo agregado en v03.00.** |
| tipo_comprobante | NVARCHAR(20) | NOT NULL | Valores: 'Boleta' o 'Factura'. |
| fecha_emision | DATE | NOT NULL | Fecha de emisión fiscal. |
| id_venta | INT | FK (TBL_REGISTRO_VENTAS) | Enlace directo a la venta que originó el documento. |
| estado_documento | NVARCHAR(25) | DEFAULT 'Emitido' | Estados: 'Emitido', 'Anulado'. |

*Relación uno a uno con la venta. Un comprobante emitido nunca se elimina físicamente: ILA-0012 lo anula.*

## 8. TBL_METODOS_PAGO

Catálogo de las modalidades de pago aceptadas por el establecimiento.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_metodo_pago | INT | PK, Auto-increment | Identificador único del método de pago. |
| nombre_metodo | NVARCHAR(50) | NOT NULL, UNIQUE | Ejemplos: 'Efectivo', 'Tarjeta de Credito', 'Yape/Plin'. |
| estado | NVARCHAR(10) | DEFAULT 'Activo' | Control de disponibilidad del método de pago. |

*Educción asociada: EDU-0009. La baja se resuelve como desactivación en ILA-0020 cuando existen ventas asociadas.*

## 9. TBL_ALERTAS_VENCIMIENTO

Configuración del motor de alertas automáticas de vida útil de los medicamentos.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_alerta | INT | PK, Auto-increment | Identificador de la regla de alerta. |
| descripcion | NVARCHAR(150) | NOT NULL | Nombre de la regla, por ejemplo 'Alerta Base Cuarentena'. |
| umbral_meses | INT | NOT NULL, DEFAULT 2 | Margen crítico de vida útil, en meses. |
| estado | NVARCHAR(10) | DEFAULT 'Activo' | Define si el motor de alertas ejecuta esta regla. |
| fecha_ultima_verif | DATETIME2 | NOT NULL | Registro de cuándo se ejecutó el último escaneo automático. |

*Tabla de configuración global, sin claves foráneas. Actualmente ninguna educción gestiona su contenido: ILA-0013 solo lee `umbral_meses`. Ver la sección final.*

## 10. TBL_PRODUCTOS_RETIRADOS

Historial de mermas y productos removidos de la venta por vencimiento, daño o alerta regulatoria.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_retiro | INT | PK, Auto-increment | Identificador único del registro de retiro. |
| nombre_producto | NVARCHAR(100) | NOT NULL | Copia del nombre del producto, conservada deliberadamente como texto para mantener el histórico aunque el producto maestro se elimine. |
| **numero_lote** | **NVARCHAR(30)** | **NOT NULL** | **Copia del número de lote afectado. Campo agregado en v04.00, con el mismo criterio de conservación que el nombre.** |
| fecha_vencimiento | DATE | NOT NULL | Fecha de expiración que causó el retiro. |
| es_lote_defectuoso | BIT | DEFAULT FALSE | Indica si falló el control de calidad físico. |
| alerta_digemid | NVARCHAR(100) | Opcional | Código de resolución o alerta sanitaria de DIGEMID. |
| fecha_registro | DATE | NOT NULL | Fecha en la que el sistema procesó la baja o retiro. |

*Educción asociada: EDU-0004. La ausencia de `id_producto` es una decisión de diseño ya evaluada, no un error pendiente.*

## 11. TBL_ORDENES_DEVOLUCION

Cabeceras de los trámites de devolución despachados a los laboratorios y proveedores.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_devolucion | INT | PK, Auto-increment | Identificador único de la orden de devolución. |
| fecha_creacion | DATE | NOT NULL | Fecha en que el usuario registró la orden. |
| motivo_devolucion | NVARCHAR(50) | NOT NULL | Criterios: 'Por Vencimiento', 'Por Depuracion de Error'. |
| id_proveedor | INT | FK (TBL_PROVEEDORES) | Proveedor al que se remite la mercancía. |
| comentario | NVARCHAR(MAX) | Opcional | Notas aclaratorias u observaciones. Obligatorio cuando el motivo es la depuración de un error. |
| estado_orden | NVARCHAR(15) | DEFAULT 'Pendiente' | Estados: 'Pendiente', 'Completada', 'Eliminada'. |
| id_usuario | INT | FK (TBL_USUARIOS) | Usuario que registró el movimiento. |

*Educción asociada: EDU-0011. El paso a 'Completada' descuenta el stock en ILA-0027; 'Eliminada' es la baja lógica de ILA-0028.*

## 12. TBL_DETALLE_DEVOLUCION

Desglosa qué medicamentos van dentro de una orden de devolución.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_detalle_dev | INT | PK, Auto-increment | Identificador de la línea de devolución. |
| id_devolucion | INT | FK (TBL_ORDENES_DEVOLUCION) | Enlace a la cabecera de la devolución. |
| id_lote | INT | NOT NULL, FK (TBL_LOTES) | Remesa que se extraerá del almacén. |
| cantidad_devolver | INT | NOT NULL | Unidades físicas devueltas al proveedor. |

*Relación identificadora: el detalle no existe sin su cabecera.*

*Cambio en v04.00: `id_producto` se sustituyó por `id_lote`. El proveedor tramita la devolución contra un número de lote; una orden que solo nombrara el medicamento no servía para el trámite real.*

## 13. TBL_RESTRICCIONES_VENTA

Reglas de seguridad clínica para protección en el punto de venta.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_restriccion | INT | PK, Auto-increment | Identificador único de la regla restrictiva. |
| id_producto | INT | FK (TBL_PRODUCTOS) | Medicamento afectado por la condición sanitaria. |
| condicion_cliente | NVARCHAR(50) | NOT NULL | Grupo de riesgo, por ejemplo 'Gestante', 'Neonato', 'Oncologico'. |
| tipo_restriccion | NVARCHAR(25) | NOT NULL | Modos operacionales: 'Informativa' o 'Bloqueante'. |
| motivo_advertencia | NVARCHAR(MAX) | NOT NULL | Texto explícito que se muestra en la pantalla del cajero. |
| estado_alerta | NVARCHAR(10) | DEFAULT 'Activo' | Vigencia de la restricción: 'Activo' o 'Inactivo'. |

*Educción asociada: EDU-0012. ILA-0001 consulta esta tabla antes de agregar un producto a la venta.*

## 14. TBL_REPORTES_VENTAS

Índice, trazabilidad y almacenamiento de los informes generados.

| Campo | Tipo de dato | Restricciones | Descripción |
| --- | --- | --- | --- |
| id_reporte | INT | PK, Auto-increment | Identificador único del reporte emitido. |
| tipo_reporte | NVARCHAR(50) | NOT NULL | Criterio de agrupación, por ejemplo 'Ventas Diarias' o 'Mermas'. |
| fecha_inicio | DATE | NOT NULL | Fecha desde la cual se extrajeron los datos consolidados. |
| fecha_fin | DATE | NOT NULL | Fecha hasta la cual se extrajeron los datos consolidados. |
| fecha_generacion | DATETIME2 | NOT NULL | Auditoría de cuándo se procesó el archivo. |
| ruta_archivo | NVARCHAR(255) | NOT NULL | Dirección física o URL de descarga del documento. |

*Educción asociada: EDU-0010. Es un log de salida sin referencias foráneas, por lo que su eliminación física en ILA-0024 es legítima.*

---

## Correspondencia de tipos entre base de datos, código e interfaz

Esta tabla es el criterio único para decidir el tipo de un campo en las tres capas. Toda ilación y toda especificación debe respetarla.

| Tipo lógico del dato | SQL Server Express | Tipo en C# / .NET 8 | Control WPF | Prefijo de nomenclatura |
| --- | --- | --- | --- | --- |
| Número entero | `INT` | `int` | TextBox con validación numérica | **NUM** |
| Importe monetario | `DECIMAL(10,2)` | `decimal` | TextBox con formato de moneda | **NUM** |
| Texto corto | `NVARCHAR(n)` | `string` | TextBox | **TXT** |
| Texto extenso | `NVARCHAR(MAX)` | `string` | TextBox multilínea | **TXA** |
| Fecha | `DATE` | `DateOnly` o `DateTime` | DatePicker | **FEC** |
| Fecha y hora | `DATETIME2` | `DateTime` | DatePicker, o solo lectura si la asigna el sistema | **FEC** o **LBL** |
| Valor lógico | `BIT` | `bool` | CheckBox | **CHK** |
| Dominio cerrado de valores | `NVARCHAR(n)` con restricción CHECK | `string` o `enum` | ComboBox | **CMB** |
| Referencia a otra entidad | `INT` con clave foránea | `int` | ComboBox | **CMB** |
| Contraseña | `NVARCHAR(255)` con el valor ya encriptado | `string` | PasswordBox | **PWD** |
| Valor calculado por el sistema | No se persiste, o se persiste sin captura | según el cálculo | TextBlock | **LBL** |

**Nunca usar `float` ni `double` para importes.** `DECIMAL(10,2)` corresponde a `decimal` en C#, y cualquier otra elección introduce errores de redondeo en `monto_total` y `precio_unitario`.

**Campos con dominio cerrado.** Los siguientes campos admiten un conjunto fijo de valores y se capturan siempre con un ComboBox, nunca escribiéndolos: `rol`, `estado`, `estado_producto`, `estado_documento`, `estado_orden`, `estado_alerta`, `tipo_comprobante`, `tipo_restriccion` y `motivo_devolucion`. Se recomienda declararlos con una restricción CHECK en el motor para que la validación no dependa únicamente de la interfaz.

---

## Resumen de relaciones

| Relación | Tipo | Justificación |
| --- | --- | --- |
| `TBL_PRODUCTOS` → `TBL_LOTES` | Identificadora, 1:N | Nueva en v04.00. Un medicamento tiene varias remesas; una remesa no existe sin su medicamento. |
| `TBL_REGISTRO_VENTAS` → `TBL_DETALLE_VENTAS` | Identificadora, 1:N | El detalle no existe sin la venta cabecera. |
| `TBL_ORDENES_DEVOLUCION` → `TBL_DETALLE_DEVOLUCION` | Identificadora, 1:N | El detalle no existe sin la orden cabecera. |
| `TBL_REGISTRO_VENTAS` ↔ `TBL_COMPROBANTES_TRIBUTARIOS` | Identificadora, 1:1 | Un comprobante corresponde siempre a una venta específica. |
| `TBL_USUARIOS` → `TBL_REGISTRO_VENTAS` | No identificadora, 1:N | Nueva en v03.00. Un usuario registra muchas ventas. |
| `TBL_USUARIOS` → `TBL_ORDENES_DEVOLUCION` | No identificadora, 1:N | Un usuario registra muchas órdenes de devolución. |
| `TBL_PROVEEDORES` → `TBL_PRODUCTOS` | No identificadora, 1:N | Un proveedor abastece muchos productos. |
| `TBL_PROVEEDORES` → `TBL_ORDENES_DEVOLUCION` | No identificadora, 1:N | Un proveedor recibe muchas devoluciones. |
| `TBL_METODOS_PAGO` → `TBL_REGISTRO_VENTAS` | No identificadora, 1:N | Un método de pago se usa en muchas ventas. |
| `TBL_LOTES` → `TBL_DETALLE_VENTAS` | No identificadora, 1:N | Modificada en v04.00. Una remesa aparece en muchas líneas de venta. |
| `TBL_LOTES` → `TBL_DETALLE_DEVOLUCION` | No identificadora, 1:N | Modificada en v04.00. Una remesa aparece en muchas líneas de devolución. |
| `TBL_PRODUCTOS` → `TBL_RESTRICCIONES_VENTA` | No identificadora, 1:N | Un producto puede tener varias restricciones sanitarias. |

`TBL_ALERTAS_VENCIMIENTO`, `TBL_PRODUCTOS_RETIRADOS` y `TBL_REPORTES_VENTAS` no participan en relaciones foráneas, por las razones documentadas en cada tabla.

---

## Regla de selección de lote en la venta

Al existir varias remesas de un mismo medicamento, la venta necesita un criterio para decidir de cuál descuenta. El catálogo adopta **primero el que vence antes** (criterio FEFO, *first expired, first out*), que es la práctica habitual en farmacia y la que minimiza la merma por vencimiento.

Operativamente: al seleccionar un medicamento, el sistema propone por defecto el lote disponible con la `fecha_vencimiento` más próxima y con `stock_actual` mayor a cero, y permite al usuario elegir otro cuando la remesa física que tiene en la mano es distinta. Los lotes con `estado_lote` distinto de `'Disponible'` no se ofrecen.

Esta regla la aplican ILA-0001 y ESP-0001, y la reutiliza ILA-0027 al completar una devolución.

---

## Vacíos del modelo pendientes de decisión

Estos puntos no se modificaron porque implican decisiones de alcance, no correcciones. Se listan aquí para que el equipo los resuelva de forma consciente y no por omisión.

1. ~~**Múltiples lotes por medicamento.**~~ **Resuelto en la versión 04.00** con la incorporación de `TBL_LOTES` y del módulo 11.
2. **Clientes.** No existe `TBL_CLIENTES` ni relación desde la venta, pese a que en la Entrevista 1 la dueña confirmó que quiere registrar clientes, llevar historial de compras y manejar clientes frecuentes.
3. **Stock mínimo.** No hay campo que permita alertar por bajo stock, funcionalidad que la dueña pidió expresamente y que corresponde a una de las pérdidas económicas que declaró.
4. **Tipo y presentación del producto.** No existe un campo que distinga medicamento genérico de comercial, ni que permita un precio por unidad y otro por conjunto, ambos solicitados en la entrevista.
5. **Gestión de la configuración de alertas.** `TBL_ALERTAS_VENCIMIENTO` no tiene ninguna educción que la administre: el umbral es hoy un valor fijo que nadie puede cambiar desde el sistema.
