# Guía de Estilo de Nomenclatura UI — Farmasil_App

## Historial de versiones

| Fecha | Versión | Descripción | Autor |
| --- | --- | --- | --- |
| 02/06/2026 | 01.00 | Primera versión. Se establecen las convenciones para la elaboración de los mockups. | AUT-0001 |
| 24/06/2026 | 02.00 | Se cambia el separador de los mockups de guion bajo a guion, manteniendo el guion bajo para tablas y atributos de la base de datos. | AUT-0001 |
| 05/09/2026 | 03.00 | Se incorporan los módulos 9 (usuarios, prefijo **USR**) y 10 (proveedores, prefijo **PRV**). Se resuelve la contradicción entre la regla de mayúsculas y los ejemplos de la versión anterior. Se oficializan los tipos de componente **FEC**, **NUM** y **PWD**. Se incorpora la regla de correspondencia entre el tipo del campo en la base de datos y el tipo del componente, y el inventario completo por módulo. | AUT-0001 |

| 05/09/2026 | 04.00 | Se incorpora el módulo 11 (lotes, prefijo **LOT**), derivado de la incorporación de `TBL_LOTES` al Diccionario de Datos v04.00. Los componentes de lote, vencimiento y stock se trasladan del módulo de inventario al nuevo módulo, y los módulos 1, 4 y 7 pasan a operar sobre la remesa en lugar del medicamento. | AUT-0001 |

---

## 1. Convención general

La estructura oficial se mantiene sin cambios:

```
[MODULO]-[COMPONENTE]-[FUNCION]
```

| Segmento | Descripción |
| --- | --- |
| MÓDULO | Identificador abreviado del módulo funcional del sistema. Tres letras. |
| COMPONENTE | Tipo de elemento de interfaz. |
| FUNCIÓN | Acción o propósito específico del componente. |

### Corrección respecto a la versión 02.00

La versión anterior presentaba dos inconsistencias que conviene dejar resueltas:

1. La regla general indicaba que **todos los nombres se escriben en mayúsculas**, pero los ejemplos usaban notación mixta (`VEN-PAGE-RegistroVenta`, `VEN-BTN-ConfirmarVenta`). Los más de cien componentes ya definidos en las especificaciones siguen la regla, no los ejemplos.
   **Se adopta como oficial la notación en mayúsculas con guion**, y las palabras del segmento de función se separan también con guion: `VEN-BTN-CONFIRMAR-VENTA`, no `VEN-BTN-ConfirmarVenta`.
2. La regla decía "se utilizará el carácter guion bajo (-)", mezclando el nombre de un separador con el símbolo del otro.
   **Separador oficial en interfaz: guion (`-`). Separador oficial en base de datos: guion bajo (`_`).** `VEN-BTN-CREAR-VENTA` frente a `DB_FARMASIL.TBL_REGISTRO_VENTAS`.

---

## 2. Tabla de equivalencias de módulos

| # | Nombre técnico del módulo | Nombre de nomenclatura | Prefijo oficial | Educción |
| --- | --- | --- | --- | --- |
| 1 | Gestión de Ventas | ventas | **VEN** | EDU-0001 |
| 2 | Gestión de Inventario | inventario | **INV** | EDU-0002 |
| 3 | Gestión de Documentación Tributaria | documentacion | **DOC** | EDU-0003 |
| 4 | Gestión de Alertas de Productos Vencidos | alertas_vencimiento | **ALV** | EDU-0004 |
| 5 | Gestión de Métodos de Pago | pagos | **PAG** | EDU-0009 |
| 6 | Gestión de Reportes de Ventas | reportes | **REP** | EDU-0010 |
| 7 | Gestión de Devoluciones a Proveedores | devoluciones | **DEV** | EDU-0011 |
| 8 | Gestión de Alertas de Restricciones de Venta | restricciones | **RES** | EDU-0012 |
| 9 | Gestión de Usuarios | usuarios | **USR** | EDU-0013 |
| 10 | Gestión de Proveedores | proveedores | **PRV** | EDU-0014 |
| 11 | Gestión de Lotes | lotes | **LOT** | EDU-0015 |

El inicio y cierre de sesión no constituyen un módulo funcional: están cubiertos por RNF-0006. Las pantallas de autenticación que se diseñen usan el prefijo **USR**, por operar sobre la misma entidad.

---

## 3. Tipos de componente

| Tipo de componente | Prefijo |
| --- | --- |
| Página | PAGE |
| Formulario | FRM |
| Panel | PNL |
| Modal | MDL |
| Tabla | TBL |
| Botón | BTN |
| Campo de texto | TXT |
| Área de texto | TXA |
| ComboBox | CMB |
| Checkbox | CHK |
| Etiqueta | LBL |
| Selector de fecha | **FEC** |
| Campo numérico | **NUM** |
| Campo de contraseña | **PWD** |
| Barra de búsqueda | SRH |
| Menú | MNU |
| Tarjeta | CRD |

Tres tipos se incorporan en esta versión. **FEC** ya se usaba en `DOC-FEC-FECHA-EMISION` sin estar documentado. **NUM** distingue los campos que solo admiten valores numéricos, que en WPF requieren validación y formato distintos a un TextBox común. **PWD** corresponde al control PasswordBox de WPF, que enmascara el contenido y no expone el texto en claro; usar TXT para una contraseña contradice el atributo de seguridad de RNF-0006.

**LBL** se reserva para valores calculados o generados por el sistema que el usuario no edita, como `VEN-LBL-MONTO-TOTAL` o `INV-LBL-ID-PRODUCTO`.

### 3.1 Regla de correspondencia con el tipo del dato

El tipo del componente no se elige por criterio visual: lo determina el tipo del campo en el Diccionario de Datos v03.00 y el control que le corresponde en WPF según el stack definido en RNF-0008.

| Tipo lógico del dato | SQL Server Express | Tipo en C# / .NET 8 | Control WPF | Prefijo |
| --- | --- | --- | --- | --- |
| Número entero | `INT` | `int` | TextBox con validación numérica | **NUM** |
| Importe monetario | `DECIMAL(10,2)` | `decimal` | TextBox con formato de moneda | **NUM** |
| Texto corto | `NVARCHAR(n)` | `string` | TextBox | **TXT** |
| Texto extenso | `NVARCHAR(MAX)` | `string` | TextBox multilínea | **TXA** |
| Fecha | `DATE` | `DateOnly` o `DateTime` | DatePicker | **FEC** |
| Fecha y hora | `DATETIME2` | `DateTime` | DatePicker, o solo lectura si la asigna el sistema | **FEC** o **LBL** |
| Valor lógico | `BIT` | `bool` | CheckBox | **CHK** |
| Dominio cerrado de valores | `NVARCHAR(n)` con CHECK | `string` o `enum` | ComboBox | **CMB** |
| Referencia a otra entidad | `INT` con clave foránea | `int` | ComboBox | **CMB** |
| Contraseña | `NVARCHAR(255)` encriptada | `string` | PasswordBox | **PWD** |
| Valor calculado o autogenerado | no se captura | según el cálculo | TextBlock | **LBL** |

Dos consecuencias prácticas de esta regla:

- Un campo con dominio cerrado (`rol`, `estado`, `tipo_comprobante`, `motivo_devolucion`, `tipo_restriccion`, y todos los campos de estado) se captura siempre con **CMB**, nunca escribiéndolo.
- Un campo que es clave foránea se captura con **CMB**, nunca como texto libre: la ilación necesita el identificador de la entidad, no su nombre.

---

## 4. Convención para artefactos de mockup

```
[ART]-[MKP]-[MODULO]-[NUMERO]
```

Numeración de cuatro dígitos, correlativa dentro de cada módulo. Por convención del catálogo, el orden sigue las fases del CRUD:

| Número | Contenido habitual |
| --- | --- |
| 0001 | Pantalla principal del módulo, con la tabla de registros y el formulario de creación |
| 0002 | Pantalla o panel de consulta y filtros |
| 0003 | Formulario de actualización |
| 0004 | Modal de confirmación de eliminación |

---

## 5. Convención para funciones del CRUD

Para que los nombres sean predecibles, la función de los componentes que implementan cada fase se estandariza así:

| Fase | Botón principal | Confirmación |
| --- | --- | --- |
| Crear | `<MOD>-BTN-CREAR-<ENTIDAD>` | El propio botón de crear confirma la operación |
| Leer | `<MOD>-BTN-LEER-<ENTIDAD>` | No aplica |
| Actualizar | `<MOD>-BTN-ACTUALIZAR-<ENTIDAD>` | `<MOD>-BTN-CONFIRMAR-ACTUALIZACION` |
| Eliminar | `<MOD>-BTN-ELIMINAR-<ENTIDAD>` | `<MOD>-MDL-CONFIRMAR-ELIMINACION` con `<MOD>-BTN-CONFIRMAR-SI` y `<MOD>-BTN-CONFIRMAR-NO` |

El texto que se muestra dentro de un modal se nombra `<MOD>-TXT-MSJ`. La tabla principal de registros del módulo se nombra `<MOD>-TBL-<ENTIDAD-EN-PLURAL>`.

---

## 6. Inventario de componentes por módulo

### Módulo 9 — Usuarios (USR) · nuevo

| Componente | Tipo | Función | Mockup |
| --- | --- | --- | --- |
| USR-TBL-USUARIOS | Tabla | Listado de cuentas: identificador, nombre de usuario, rol y estado. Nunca muestra la contraseña. | ART-MKP-USR-0001 |
| USR-TXT-NOMBRE-USUARIO | Campo de texto | Nombre de usuario para el inicio de sesión. Único. | ART-MKP-USR-0001, 0002, 0003 |
| USR-PWD-CONTRASENA | Campo de contraseña | Contraseña, con el contenido enmascarado. En el formulario de actualización se muestra vacío: si no se completa, la contraseña vigente se conserva. | ART-MKP-USR-0001, 0003 |
| USR-CMB-ROL | ComboBox | Rol de la cuenta: 'Administrador' o 'Tecnico'. | ART-MKP-USR-0001, 0002, 0003 |
| USR-CMB-ESTADO-USUARIO | ComboBox | Estado de la cuenta: 'Activo' o 'Inactivo'. | ART-MKP-USR-0001, 0003 |
| USR-BTN-CREAR-USUARIO | Botón | Valida y registra la cuenta nueva. | ART-MKP-USR-0001 |
| USR-BTN-LEER-USUARIO | Botón | Ejecuta la búsqueda por nombre de usuario o rol. | ART-MKP-USR-0002 |
| USR-BTN-ACTUALIZAR-USUARIO | Botón | Abre el formulario de actualización de la cuenta seleccionada. | ART-MKP-USR-0001 |
| USR-BTN-CONFIRMAR-ACTUALIZACION | Botón | Guarda los cambios de la cuenta. | ART-MKP-USR-0003 |
| USR-BTN-ELIMINAR-USUARIO | Botón | Inicia la baja de la cuenta seleccionada. | ART-MKP-USR-0001 |
| USR-MDL-CONFIRMAR-ELIMINACION | Modal | Confirmación de la baja. | ART-MKP-USR-0004 |
| USR-TXT-MSJ | Campo de texto | Mensaje del modal. | ART-MKP-USR-0004 |
| USR-BTN-CONFIRMAR-SI | Botón | Confirma la baja. | ART-MKP-USR-0004 |
| USR-BTN-CONFIRMAR-NO | Botón | Cancela la baja. | ART-MKP-USR-0004 |

### Módulo 10 — Proveedores (PRV) · nuevo

| Componente | Tipo | Función | Mockup |
| --- | --- | --- | --- |
| PRV-TBL-PROVEEDORES | Tabla | Listado de proveedores: identificador, RUC, razón social, teléfono y estado. | ART-MKP-PRV-0001 |
| PRV-TXT-RUC | Campo de texto | RUC de la empresa. Once dígitos numéricos, único. | ART-MKP-PRV-0001, 0002, 0003 |
| PRV-TXT-RAZON-SOCIAL | Campo de texto | Nombre legal de la droguería o distribuidora. | ART-MKP-PRV-0001, 0002, 0003 |
| PRV-TXT-TELEFONO | Campo de texto | Teléfono de contacto. Opcional. | ART-MKP-PRV-0001, 0003 |
| PRV-CMB-ESTADO-PROVEEDOR | ComboBox | Estado del proveedor: 'Activo' o 'Inactivo'. | ART-MKP-PRV-0001, 0003 |
| PRV-BTN-CREAR-PROVEEDOR | Botón | Valida y registra el proveedor nuevo. | ART-MKP-PRV-0001 |
| PRV-BTN-LEER-PROVEEDOR | Botón | Ejecuta la búsqueda por RUC o razón social. | ART-MKP-PRV-0002 |
| PRV-BTN-ACTUALIZAR-PROVEEDOR | Botón | Abre el formulario de actualización del proveedor seleccionado. | ART-MKP-PRV-0001 |
| PRV-BTN-CONFIRMAR-ACTUALIZACION | Botón | Guarda los cambios del proveedor. | ART-MKP-PRV-0003 |
| PRV-BTN-ELIMINAR-PROVEEDOR | Botón | Inicia la baja del proveedor seleccionado. | ART-MKP-PRV-0001 |
| PRV-MDL-CONFIRMAR-ELIMINACION | Modal | Confirmación de la baja. | ART-MKP-PRV-0004 |
| PRV-TXT-MSJ | Campo de texto | Mensaje del modal. | ART-MKP-PRV-0004 |
| PRV-BTN-CONFIRMAR-SI | Botón | Confirma la baja. | ART-MKP-PRV-0004 |
| PRV-BTN-CONFIRMAR-NO | Botón | Cancela la baja. | ART-MKP-PRV-0004 |

### Correcciones de tipo aplicadas

Al contrastar cada componente contra el tipo de su campo aparecieron estas incoherencias, ya corregidas en las ilaciones.

| Nombre anterior | Nombre corregido | Campo y tipo | Motivo |
| --- | --- | --- | --- |
| ALV-CMB-LOTE-DEFECTUOSO | **ALV-CHK-LOTE-DEFECTUOSO** | `es_lote_defectuoso BIT` | Un valor lógico se marca, no se elige de una lista. |
| ALV-CMB-ALERTA-DIGEMID | **ALV-TXT-ALERTA-DIGEMID** | `alerta_digemid NVARCHAR(100)` | Es el código de una resolución sanitaria, texto libre sin dominio cerrado. |
| ALV-TXT-PRODUCTO-ALERTA | **ALV-CMB-PRODUCTO-ALERTA** | `id_producto` para el bloqueo | ILA-0014 debe bloquear el producto afectado, y para eso necesita su identificador, no un nombre escrito a mano. |
| VEN-TXT-CANTIDAD-VENTA | **VEN-NUM-CANTIDAD-VENTA** | `cantidad INT` | Valor numérico. |
| INV-TXT-PRECIO-PRODUCTO | **INV-NUM-PRECIO-PRODUCTO** | `precio_venta DECIMAL(10,2)` | Importe monetario. |
| INV-TXT-STOCK-PRODUCTO | **INV-NUM-STOCK-PRODUCTO** | `stock_actual INT` | Valor numérico. |
| INV-TXT-ID-PRODUCTO (en alta y actualización) | **INV-LBL-ID-PRODUCTO** | `id_producto` autoincremental | El identificador lo genera el motor; el usuario no lo escribe. En la pantalla de consulta se conserva INV-TXT-ID-PRODUCTO, donde sí es un criterio de búsqueda que se teclea. |
| DOC-TXT-MONTO-TOTAL | **DOC-LBL-MONTO-TOTAL** | derivado de `monto_total` | Se recupera de la venta; no se captura. |
| USR-TXT-CONTRASENA | **USR-PWD-CONTRASENA** | `contrasena NVARCHAR(255)` | PasswordBox, por RNF-0006. |
| RES-TXT-MOTIVO-ADVERTENCIA | **RES-TXA-MOTIVO-ADVERTENCIA** | `motivo_advertencia NVARCHAR(MAX)` | Texto extenso. |
| Campos de fecha con prefijo TXT | **FEC** | `DATE` | DatePicker. |
| VEN-TXT-FECHA-VENTA (en el alta) | **VEN-LBL-FECHA-VENTA** | `fecha_hora DATETIME2` | La asigna el sistema al confirmar la venta. En la consulta se conserva VEN-FEC-FECHA-VENTA como filtro. |

### Componentes que faltaban por completo

`TBL_DETALLE_DEVOLUCION.cantidad_devolver` está declarado NOT NULL y `TBL_ORDENES_DEVOLUCION.comentario` es obligatorio cuando el motivo es la depuración de un error, pero ninguna ilación los capturaba: la orden se registraba sin decir cuántas unidades se devuelven. Se incorporaron **DEV-NUM-CANTIDAD-DEVOLVER** y **DEV-TXA-COMENTARIO**.

### Módulo 11 — Lotes (LOT) · nuevo

| Componente | Tipo | Función | Mockup |
| --- | --- | --- | --- |
| LOT-TBL-LOTES | Tabla | Listado de remesas: medicamento, número de lote, vencimiento, stock y estado. | ART-MKP-LOT-0001 |
| LOT-CMB-PRODUCTO | ComboBox | Medicamento del catálogo al que pertenece la remesa. | ART-MKP-LOT-0001, 0002, 0003 |
| LOT-TXT-NUMERO-LOTE | Campo de texto | Número de lote impreso por el laboratorio. Único por medicamento. | ART-MKP-LOT-0001, 0002, 0003 |
| LOT-FEC-FECHA-VENCIMIENTO | Selector de fecha | Caducidad de esta remesa. | ART-MKP-LOT-0001, 0003 |
| LOT-NUM-STOCK-LOTE | Campo numérico | Unidades físicas disponibles de la remesa. | ART-MKP-LOT-0001, 0003 |
| LOT-FEC-FECHA-INGRESO | Selector de fecha | Fecha de ingreso al almacén. | ART-MKP-LOT-0001, 0003 |
| LOT-CMB-ESTADO-LOTE | ComboBox | 'Disponible', 'Bloqueado por devolucion' o 'Agotado'. | ART-MKP-LOT-0001, 0003 |
| LOT-BTN-CREAR-LOTE | Botón | Valida y registra la remesa nueva. | ART-MKP-LOT-0001 |
| LOT-BTN-LEER-LOTE | Botón | Ejecuta la búsqueda por medicamento, número de lote o estado. | ART-MKP-LOT-0002 |
| LOT-BTN-ACTUALIZAR-LOTE | Botón | Abre el formulario de actualización de la remesa seleccionada. | ART-MKP-LOT-0001 |
| LOT-BTN-CONFIRMAR-ACTUALIZACION | Botón | Guarda los cambios de la remesa. | ART-MKP-LOT-0003 |
| LOT-BTN-ELIMINAR-LOTE | Botón | Inicia la baja de la remesa seleccionada. | ART-MKP-LOT-0001 |
| LOT-MDL-CONFIRMAR-ELIMINACION | Modal | Confirmación de la baja. | ART-MKP-LOT-0004 |
| LOT-TXT-MSJ | Campo de texto | Mensaje del modal. | ART-MKP-LOT-0004 |
| LOT-BTN-CONFIRMAR-SI | Botón | Confirma la baja. | ART-MKP-LOT-0004 |
| LOT-BTN-CONFIRMAR-NO | Botón | Cancela la baja. | ART-MKP-LOT-0004 |

### Reasignación de componentes por la incorporación de lotes

| Componente | Antes | Ahora |
| --- | --- | --- |
| INV-TXT-NUMERO-LOTE | Módulo 2, formulario de producto | **Se retira.** Su función pasa a LOT-TXT-NUMERO-LOTE. |
| INV-FEC-FECHA-VENCIMIENTO | Módulo 2, formulario de producto | **Se retira.** Su función pasa a LOT-FEC-FECHA-VENCIMIENTO. |
| INV-NUM-STOCK-PRODUCTO | Módulo 2, formulario de producto | **Se retira.** Su función pasa a LOT-NUM-STOCK-LOTE. |
| VEN-CMB-LOTE-VENTA | No existía | **Nuevo.** Selección de la remesa de la que se descuenta, propuesta por defecto según el criterio FEFO. |
| ALV-CMB-PRODUCTO-ALERTA | Selección de medicamento | **ALV-CMB-LOTE-ALERTA.** La alerta se levanta sobre una remesa, no sobre el medicamento completo. |
| ALV-FEC-FECHA-VENCIMIENTO | Campo capturado | **ALV-LBL-FECHA-VENCIMIENTO.** Se obtiene de la remesa seleccionada y deja de escribirse. |
| ALV-TXT-PRODUCTO-ALERTA | Ya retirado en v03.00 | Sin cambios. |
| DEV-CMB-MEDICAMENTO | Selección de medicamento | **DEV-CMB-LOTE-DEVOLUCION.** El proveedor tramita la devolución por lote. |

El módulo 2 conserva únicamente los datos de catálogo del medicamento: nombre, acción terapéutica, precio, proveedor y estado.

### Componentes nuevos en módulos existentes

| Componente | Tipo | Función | Mockup afectado |
| --- | --- | --- | --- |
| VEN-TBL-DETALLE-VENTA | Tabla | Líneas de productos de la venta en curso: producto, cantidad, precio unitario y subtotal. Es el componente que hace visible la venta de varios productos. | ART-MKP-VEN-0001, 0002, 0003 |
| VEN-BTN-AGREGAR-PRODUCTO | Botón | Agrega el producto y la cantidad seleccionados como una línea del detalle. | ART-MKP-VEN-0001, 0003 |
| VEN-BTN-QUITAR-PRODUCTO | Botón | Retira del detalle la línea seleccionada. | ART-MKP-VEN-0001, 0003 |
| VEN-LBL-MONTO-TOTAL | Etiqueta | Suma calculada del detalle. No editable. | ART-MKP-VEN-0001, 0003 |
| INV-TXT-STOCK-PRODUCTO | Campo de texto | Cantidad de unidades recibidas o disponibles. | ART-MKP-INV-0001, 0003 |
| INV-CMB-PROVEEDOR | ComboBox | Proveedor que abastece el producto. Se alimenta del módulo 10. | ART-MKP-INV-0001, 0003 |
| DEV-TBL-DETALLE-DEVOLUCION | Tabla | Medicamentos incluidos en la orden: producto, lote y cantidad a devolver. | ART-MKP-DEV-0001, 0002, 0003 |
| DEV-NUM-CANTIDAD-DEVOLVER | Campo numérico | Unidades físicas a devolver por cada línea de la orden. | ART-MKP-DEV-0001, 0003 |
| DEV-TXA-COMENTARIO | Área de texto | Observaciones de la orden. Obligatorio cuando el motivo es la depuración de un error. | ART-MKP-DEV-0001, 0003 |

### Módulos 1 a 8 — componentes vigentes

Los componentes ya definidos en las especificaciones se mantienen. El inventario consolidado, incluyendo los nuevos, queda así:

| Módulo | Componentes |
| --- | --- |
| **VEN** | VEN-TBL-REGISTRO-VENTAS, VEN-TBL-DETALLE-VENTA, VEN-CMB-PRODUCTO-VENTA, VEN-CMB-LOTE-VENTA, VEN-CMB-METODO-PAGO, VEN-NUM-CANTIDAD-VENTA, VEN-LBL-FECHA-VENTA, VEN-FEC-FECHA-VENTA, VEN-LBL-MONTO-TOTAL, VEN-BTN-AGREGAR-PRODUCTO, VEN-BTN-QUITAR-PRODUCTO, VEN-BTN-CREAR-VENTA, VEN-BTN-LEER-VENTA, VEN-BTN-ACTUALIZAR-VENTA, VEN-BTN-CONFIRMAR-ACTUALIZACION, VEN-BTN-ELIMINAR-VENTA, VEN-MDL-CONFIRMAR-ELIMINACION, VEN-TXT-MSJ, VEN-BTN-CONFIRMAR-SI, VEN-BTN-CONFIRMAR-NO |
| **INV** | INV-TBL-PRODUCTOS, INV-LBL-ID-PRODUCTO, INV-TXT-ID-PRODUCTO, INV-TXT-NOMBRE-PRODUCTO, INV-TXT-ACCION-TERAPEUTICA, INV-NUM-PRECIO-PRODUCTO, INV-CMB-PROVEEDOR, INV-BTN-CREAR-PRODUCTO, INV-BTN-LEER-PRODUCTO, INV-BTN-ACTUALIZAR-PRODUCTO, INV-BTN-ELIMINAR-PRODUCTO, INV-MDL-CONFIRMAR-ELIMINACION, INV-TXT-MSJ, INV-BTN-CONFIRMAR-SI, INV-BTN-CONFIRMAR-NO |
| **DOC** | DOC-TBL-COMPROBANTES, DOC-TXT-NUMERO-COMPROBANTE, DOC-CMB-TIPO-COMPROBANTE, DOC-CMB-CODIGO-VENTA, DOC-LBL-MONTO-TOTAL, DOC-FEC-FECHA-EMISION, DOC-BTN-CREAR-COMPROBANTE, DOC-BTN-LEER-COMPROBANTE, DOC-BTN-ACTUALIZAR-COMPROBANTE, DOC-BTN-CONFIRMAR-ACTUALIZACION, DOC-BTN-ELIMINAR-COMPROBANTE, DOC-MDL-CONFIRMAR-ELIMINACION, DOC-TXT-MSJ, DOC-BTN-CONFIRMAR-SI, DOC-BTN-CONFIRMAR-NO |
| **ALV** | ALV-TBL-ALERTAS-VENCIMIENTO, ALV-CMB-LOTE-ALERTA, ALV-LBL-FECHA-VENCIMIENTO, ALV-CHK-LOTE-DEFECTUOSO, ALV-TXT-ALERTA-DIGEMID, ALV-BTN-CREAR-ALERTA, ALV-BTN-LEER-ALERTA, ALV-BTN-ACTUALIZAR-ALERTA, ALV-BTN-CONFIRMAR-ACTUALIZACION, ALV-BTN-ELIMINAR-ALERTA, ALV-MDL-CONFIRMAR-ELIMINACION, ALV-TXT-MSJ, ALV-BTN-CONFIRMAR-SI, ALV-BTN-CONFIRMAR-NO |
| **PAG** | PAG-TBL-METODOS-PAGO, PAG-TXT-METODO-PAGO, PAG-CMB-ESTADO-METODO, PAG-BTN-CREAR-METODO, PAG-BTN-LEER-METODO, PAG-BTN-ACTUALIZAR-METODO, PAG-BTN-CONFIRMAR-ACTUALIZACION, PAG-BTN-ELIMINAR-METODO, PAG-MDL-CONFIRMAR-ELIMINACION, PAG-TXT-MSJ, PAG-BTN-CONFIRMAR-SI, PAG-BTN-CONFIRMAR-NO |
| **REP** | REP-TBL-REPORTES, REP-CMB-TIPO-REPORTE, REP-CMB-FORMATO-REPORTE, REP-FEC-FECHA-INICIO, REP-FEC-FECHA-FIN, REP-BTN-GENERAR-REPORTE, REP-BTN-LEER-REPORTE, REP-BTN-ACTUALIZAR-REPORTE, REP-BTN-CONFIRMAR-ACTUALIZACION, REP-BTN-ELIMINAR-REPORTE, REP-MDL-CONFIRMAR-ELIMINACION, REP-TXT-MSJ, REP-BTN-CONFIRMAR-SI, REP-BTN-CONFIRMAR-NO |
| **DEV** | DEV-TBL-ORDENES-DEVOLUCION, DEV-TBL-DETALLE-DEVOLUCION, DEV-CMB-PROVEEDOR, DEV-CMB-LOTE-DEVOLUCION, DEV-CMB-MOTIVO-DEVOLUCION, DEV-NUM-CANTIDAD-DEVOLVER, DEV-TXA-COMENTARIO, DEV-FEC-FECHA-DEVOLUCION, DEV-BTN-CREAR-DEVOLUCION, DEV-BTN-LEER-DEVOLUCION, DEV-BTN-ACTUALIZAR-DEVOLUCION, DEV-BTN-CONFIRMAR-ACTUALIZACION, DEV-BTN-ELIMINAR-DEVOLUCION, DEV-MDL-CONFIRMAR-ELIMINACION, DEV-TXT-MSJ, DEV-BTN-CONFIRMAR-SI, DEV-BTN-CONFIRMAR-NO |
| **LOT** | LOT-TBL-LOTES, LOT-CMB-PRODUCTO, LOT-TXT-NUMERO-LOTE, LOT-FEC-FECHA-VENCIMIENTO, LOT-NUM-STOCK-LOTE, LOT-FEC-FECHA-INGRESO, LOT-CMB-ESTADO-LOTE, LOT-BTN-CREAR-LOTE, LOT-BTN-LEER-LOTE, LOT-BTN-ACTUALIZAR-LOTE, LOT-BTN-CONFIRMAR-ACTUALIZACION, LOT-BTN-ELIMINAR-LOTE, LOT-MDL-CONFIRMAR-ELIMINACION, LOT-TXT-MSJ, LOT-BTN-CONFIRMAR-SI, LOT-BTN-CONFIRMAR-NO |
| **RES** | RES-TBL-RESTRICCIONES-VENTA, RES-CMB-MEDICAMENTO, RES-CMB-TIPO-RESTRICCION, RES-CMB-ESTADO-ALERTA, RES-TXT-CONDICION-CLIENTE, RES-TXA-MOTIVO-ADVERTENCIA, RES-BTN-CREAR-RESTRICCION, RES-BTN-LEER-RESTRICCION, RES-BTN-ACTUALIZAR-RESTRICCION, RES-BTN-CONFIRMAR-ACTUALIZACION, RES-BTN-ELIMINAR-RESTRICCION, RES-MDL-CONFIRMAR-ELIMINACION, RES-TXT-MSJ, RES-BTN-CONFIRMAR-SI, RES-BTN-CONFIRMAR-NO |

---

## 7. Renombramientos

### 7.1 Obligatorios

Estos componentes nombran algo distinto de lo que hacen, están duplicados o usan un nombre genérico prohibido por la regla 5 de la guía.

| Nombre actual | Nombre correcto | Motivo |
| --- | --- | --- |
| DEV-CMB-ESTADO-PRODUCTO | **DEV-CMB-MOTIVO-DEVOLUCION** | El campo captura `motivo_devolucion` ('Por Vencimiento' o 'Por Depuracion de Error'), no el estado del producto. |
| DEV-BTN-CONFIRMAR | **DEV-BTN-CONFIRMAR-ACTUALIZACION** | Nombre genérico, sin función identificable. |
| DEV-BTN-CONFIRMAR-REGISTRO | **DEV-BTN-CREAR-DEVOLUCION** | Duplicaba la función del botón de creación. |
| RES-TXT-MEDICAMENTO | *(se elimina)* | Duplica a RES-CMB-MEDICAMENTO. El medicamento se selecciona del inventario, no se escribe. |
| RES-TXT-MOTIVO-ADVERTENCIA | **RES-TXA-MOTIVO-ADVERTENCIA** | El campo es de tipo TEXT en la base de datos y contiene el texto que ve el cajero; corresponde un área de texto. |
| VEN-TXT-FECHA | **VEN-FEC-FECHA-VENTA** | Nombre incompleto, ya corregido en ILA-0003. |
| VEN-TXT-PRODUCTO | **VEN-CMB-PRODUCTO-VENTA** | Nombre incompleto y tipo incorrecto, ya corregido en ILA-0003. |
| VEN-MDL-MSJ | **VEN-TXT-MSJ** | MDL designa un modal, no el texto que contiene. |
| DOC-MDL-CONFIRMACION-ELIMINACION | **DOC-MDL-CONFIRMAR-ELIMINACION** | Única variante en todo el catálogo. |

### 7.2 Recomendados

Uniformidad con la convención de la sección 5. No corrigen un error, pero eliminan tres formas distintas de nombrar la misma fase del CRUD. Implican retocar los mockups existentes, así que la decisión es del equipo.

| Nombre actual | Nombre propuesto |
| --- | --- |
| PAG-BTN-REGISTRAR-METODO y PAG-BTN-CONFIRMAR-GUARDAR | PAG-BTN-CREAR-METODO |
| PAG-BTN-ELIMINAR-SI / PAG-BTN-ELIMINAR-NO | PAG-BTN-CONFIRMAR-SI / PAG-BTN-CONFIRMAR-NO |
| DOC-BTN-CONSULTAR-COMPROBANTE | DOC-BTN-LEER-COMPROBANTE |
| ALV-BTN-CONSULTAR-ALERTA | ALV-BTN-LEER-ALERTA |
| REP-BTN-VISUALIZAR-REPORTE | REP-BTN-LEER-REPORTE |
| RES-BTN-REGISTRAR-ALERTA | RES-BTN-CREAR-RESTRICCION |
| RES-BTN-CONSULTAR-ALERTA | RES-BTN-LEER-RESTRICCION |
| RES-BTN-MODIFICAR-ALERTA | RES-BTN-ACTUALIZAR-RESTRICCION |
| RES-BTN-CONFIRMAR-MODIFICACION | RES-BTN-CONFIRMAR-ACTUALIZACION |
| RES-BTN-ELIMINAR-ALERTA | RES-BTN-ELIMINAR-RESTRICCION |
| Campos de fecha con prefijo TXT: INV-TXT-FECHA-VENCIMIENTO, ALV-TXT-FECHA-VENCIMIENTO, REP-TXT-FECHA-INICIO, REP-TXT-FECHA-FIN, VEN-TXT-FECHA-VENTA | Mismo nombre con prefijo FEC |

---

## 8. Mockups a diseñar

| Mockup | Estado | Contenido |
| --- | --- | --- |
| ART-MKP-USR-0001 | Por diseñar | Pantalla principal de usuarios: tabla de cuentas y formulario de alta. |
| ART-MKP-USR-0002 | Por diseñar | Consulta de usuarios con filtros por nombre y rol. |
| ART-MKP-USR-0003 | Por diseñar | Formulario de actualización de cuenta. |
| ART-MKP-USR-0004 | Por diseñar | Modal de confirmación de baja de cuenta. |
| ART-MKP-PRV-0001 | Por diseñar | Pantalla principal de proveedores: tabla y formulario de alta. |
| ART-MKP-PRV-0002 | Por diseñar | Consulta de proveedores con filtros por RUC y razón social. |
| ART-MKP-PRV-0003 | Por diseñar | Formulario de actualización de proveedor. |
| ART-MKP-PRV-0004 | Por diseñar | Modal de confirmación de baja de proveedor. |
| ART-MKP-LOT-0001 | Por diseñar | Pantalla principal de lotes: tabla de remesas y formulario de alta. |
| ART-MKP-LOT-0002 | Por diseñar | Consulta de lotes con filtros por medicamento, número de lote y estado. |
| ART-MKP-LOT-0003 | Por diseñar | Formulario de actualización de remesa. |
| ART-MKP-LOT-0004 | Por diseñar | Modal de confirmación de baja de remesa. |
| ART-MKP-VEN-0001, 0002, 0003 | Por modificar | Incorporar la tabla de detalle, los botones de agregar y quitar producto, la etiqueta de monto total y el selector de lote. |
| ART-MKP-INV-0001, 0003 | Por modificar | Incorporar los campos de stock y proveedor. |
| ART-MKP-DEV-0001, 0002, 0003 | Por modificar | Incorporar la tabla de detalle de la orden, el campo de cantidad a devolver y el área de comentario, y renombrar el combo de motivo. |
| ART-MKP-ALV-0001, 0002 | Por modificar | El lote defectuoso pasa de combo a checkbox, la alerta DIGEMID de combo a campo de texto, y la selección pasa del medicamento a la remesa, con el vencimiento como valor derivado. |

Los mockups de los módulos 3, 4, 5, 6 y 8 no requieren cambios estructurales; solo los renombramientos de la sección 7 que el equipo decida adoptar.
