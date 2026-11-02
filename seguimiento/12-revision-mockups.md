# Revisión de mockups — FARMASIL

**Fecha:** 01/11/2026
**Alcance:** las 15 imágenes de `diseno/Mockups/` contrastadas con `catalogo/02-ilaciones.md`, `catalogo/03-especificaciones.md`, `modelo/05-diccionario-datos.md` y `diseno/06-guia-nomenclatura-mockups.md`.
**Naturaleza:** informe de solo lectura. No propone soluciones ni modifica ningún otro archivo; las decisiones las toma la instancia maestra. No se hizo commit (AGENTS.md §1).

## 0. Fuente revisada y método

**Estado del repositorio.** La copia local de `main` (`a462c0e`) contiene solo 5 de las 15 imágenes: `ART-MKP-ALV-0001`, `ART-MKP-CONFIRM-0001`, `ART-MKP-DOC-0001`, `ART-MKP-INV-0001` y `ART-MKP-VEN-0001`. Las otras 10 están en `origin/main` (`04f747e`, tras los PR #60 y #61), que es la versión revisada aquí para las imágenes y para el catálogo, el diccionario, la guía y el registro de decisiones. No se fusionó ni se actualizó ninguna rama: los archivos se leyeron con `git show`. Las 5 imágenes locales son idénticas a las de `origin/main`.

**Método.** Cada imagen se inspeccionó visualmente. Se listó cada componente visible con el identificador que la imagen muestra. Después se cotejó contra los identificadores de las ilaciones y especificaciones del módulo, contra las columnas del diccionario (`origin/main`, que incluye `formato` y `total_recaudado` en `TBL_REPORTES_VENTAS`, `stock_minimo` y `TBL_LOTES.id_proveedor`) y contra la guía. El registro de decisiones se consultó para no reabrir lo ya decidido: se citan DEC-001, DEC-004, DEC-009 y DEC-010, y los pendientes PEN-001, PEN-003, PEN-006 y PEN-010.

**Categorías.** Cada fila lleva una sola categoría; cuando un hallazgo encaja en dos, se eligió la principal y la observación menciona la otra.

- (a) En la especificación y no en el mockup
- (b) En el mockup y en ninguna especificación (posible pedido nuevo)
- (c) Nombre distinto entre mockup, especificación o guía
- (d) Campo sin columna en el diccionario
- (e) ART-MKP citado que no existe como imagen

**Convenciones del informe.**

- «Componente» es el rótulo visible o el identificador que muestra la imagen. Los IDs entre comillas francesas o con asterisco son literales de la imagen.
- Cuando un componente no tiene ID visible, el inventario indica un ID propuesto según la guía (tipo de componente y función). Es solo la respuesta al punto 1 de la tarea y no se aplica en ninguna parte.
- La categoría (e) agrupa en una fila por módulo los `ART-MKP` citados que no existen. El resumen cuenta tanto las filas como los artefactos individuales.
- Las ilaciones y especificaciones citan 50 `ART-MKP` distintos: 13 existen (los `0001` de cada módulo) y 37 no existen.

## 1. Anotaciones especiales pedidas

### DOC-MDL-PREVISUALIZAR-COMPROBANTE

- **Archivo:** `diseno/Mockups/DOC-MDL-PREVISUALIZAR-COMPROBANTE.png`. Solo existe en `origin/main`.
- **Nomenclatura:** el nombre es un identificador de componente (`DOC` + `MDL` + función), no un artefacto `ART-MKP-DOC-NNNN`. Los números 0001 a 0004 ya están ocupados por convención con las cuatro fases del CRUD (guía §4).
- **Presencia en el catálogo:** el identificador no aparece en `01`, `02`, `03`, `05`, `06` ni `08`. La única mención en el repositorio es el registro de pendientes (PEN-006). No hay educción, ilación ni especificación del flujo de previsualización antes de emitir.
- **Componentes que aporta el modal:** `DOC-BTN-DESCARGAR-PDF`, `DOC-BTN-CANCELAR-EMISION`, `DOC-BTN-CONFIRMAR-EMISION`, los campos DNI/RUC y nombre del cliente (sin ID) y varios datos sin columna (cliente, IGV, base imponible, hash). Detalle en la sección del módulo 3.

### ART-MKP-CONFIRM-0001

- **Nomenclatura:** el segmento «CONFIRM» no corresponde a ningún módulo de la guía §2.
- **Contenido:** es un modal genérico con identificadores con marcador `[MODULO]`. Las 13 especificaciones de eliminación definen cada una su modal y su mensaje, y citan un `ART-MKP-<MOD>-0004` propio.
- **Detalle:** sección «Artefacto transversal».

---

## Módulo 1 — Ventas (VEN)

**Mockup revisado:** ART-MKP-VEN-0001

### Componentes visibles

- Combo «Todo» → `VEN-CMB-PRODUCTO-VENTA`
- Buscador con lupa «Buscar» → `VEN-CMB-PRODUCTO-VENTA`
- Botones «Observar información del producto (texto)» y «Ver producto (3D)» → `VEN-BTN-LEER-VENTA` (ambos)
- Botón «Carrito de compras» → `VEN-TBL-DETALLE-VENTA`
- Catálogo: rejilla de tarjetas de producto (imagen, nombre, precio) → `VEN-CMB-PRODUCTO-VENTA`; nueve botones «Añadir» → `VEN-BTN-AGREGAR-PRODUCTO`
- Visor 3D (panel azul con controles rotar, mover, ver, papelera, +, −, papelera) → `VEN-CMB-PRODUCTO-VENTA · VEN-TBL-DETALLE-VENTA`
- Panel «Opciones de selección» → `VEN-CMB-LOTE-VENTA`; campo «Lote» → `VEN-CMB-LOTE-VENTA`; selector de fecha «Vencimiento» → `VEN-FEC-FECHA-VENTA`; combo «Ubicación» → `VEN-CMB-LOTE-VENTA`; campo «Cantidad» → `VEN-NUM-CANTIDAD-VENTA`; botón «Agregar producto» → `VEN-BTN-AGREGAR-PRODUCTO`
- Panel «Indicaciones / Sugerencias» → `VEN-LBL-MONTO-TOTAL`
- Tabla «Carrito de compras» (Código #, Descripción, Precio, QTY con − y +, Total, papelera) → `VEN-TBL-DETALLE-VENTA · VEN-NUM-CANTIDAD-VENTA · VEN-BTN-QUITAR-PRODUCTO`
- Franja «Total Items: 3 / Total: S/ 51.00» → `VEN-LBL-MONTO-TOTAL`
- Botón «Cancelar» → `VEN-BTN-ELIMINAR-VENTA`; «Método de pago» → `VEN-CMB-METODO-PAGO`; «Registrar Venta» → `VEN-BTN-CREAR-VENTA`; «Fecha: 05/09/2026 14:32» → `VEN-LBL-FECHA-VENTA`
- Anotación «ESP-0001 · Precondición FEFO sobre TBL_LOTES» (nota de diseño, no es un componente)

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-VEN-0001 | `VEN-CMB-PRODUCTO-VENTA` (combo «Todo», buscador, catálogo y visor) | (c) | ESP-0001, ESP-0002 | Un mismo ID rotula cuatro controles distintos: un combo, una caja de búsqueda, la rejilla de tarjetas y el visor 3D. En la especificación es un único combo de selección del producto. El producto se elige realmente con los botones «Añadir» de cada tarjeta. |
| ART-MKP-VEN-0001 | Buscador con lupa «Buscar» | (b) | ESP-0001 | Ninguna especificación define una búsqueda de productos en la pantalla de venta (la guía define el tipo SRH para barras de búsqueda, pero VEN no lo usa). Posible pedido nuevo. |
| ART-MKP-VEN-0001 | Botones «Observar información del producto (texto)» y «Ver producto (3D)» (`VEN-BTN-LEER-VENTA`) | (c) | ESP-0002 | `VEN-BTN-LEER-VENTA` es el botón que consulta el registro de ventas (ESP-0002, que carga ART-MKP-VEN-0002). Aquí abre información de un producto. |
| ART-MKP-VEN-0001 | Ficha de información del producto y visor 3D con sus controles | (b) | — | Ni EDU-0001 ni ILA-0001 a ILA-0004 describen una ficha informativa ni un visor 3D. Posible pedido nuevo. El mockup de inventario trae el mismo concepto (`INV-PNL-PRODUCTO`). |
| ART-MKP-VEN-0001 | Imagen del producto (tarjetas) y modelo 3D | (d) | TBL_PRODUCTOS | `TBL_PRODUCTOS` no tiene ninguna columna para imagen ni modelo 3D. |
| ART-MKP-VEN-0001 | Botón «Carrito de compras» (`VEN-TBL-DETALLE-VENTA`) | (c) | ESP-0001 | Un botón rotulado con el ID de una tabla. La tabla de detalle es la del panel derecho. |
| ART-MKP-VEN-0001 | Columna «Código #» de la tabla del carrito (valor de 17 dígitos) | (d) | ESP-0001; TBL_DETALLE_VENTAS | `TBL_DETALLE_VENTAS` solo tiene id_detalle_venta, id_venta, id_lote, cantidad y precio_unitario. No se sabe qué identificador muestra la columna (producto, remesa o código de barras). |
| ART-MKP-VEN-0001 | Selector «Vencimiento» (`VEN-FEC-FECHA-VENTA`) | (c) | ESP-0001, ESP-0002 | `VEN-FEC-FECHA-VENTA` es el filtro de fecha de la consulta (ESP-0002). Aquí muestra `fecha_vencimiento` de la remesa, un valor derivado que ESP-0001 no muestra en ningún componente y que la guía rotularía como LBL. Se ve como selector de fecha editable. |
| ART-MKP-VEN-0001 | `VEN-CMB-LOTE-VENTA` (encabezado del panel, campo «Lote» y combo «Ubicación») | (c) | ESP-0001 | Tres controles con el mismo ID. En la especificación es un solo combo que propone la remesa por FEFO. |
| ART-MKP-VEN-0001 | Combo «Ubicación» | (d) | ESP-0001; TBL_LOTES | `TBL_LOTES` no tiene columna de ubicación. Tampoco hay requisito que la pida. |
| ART-MKP-VEN-0001 | Panel «Indicaciones / Sugerencias» (recomendaciones de uso en función de los ítems) | (b) | — | Ninguna educción, ilación ni especificación pide indicaciones o sugerencias. Posible pedido nuevo del cliente. Sin columna que lo sustente. |
| ART-MKP-VEN-0001 | `VEN-LBL-MONTO-TOTAL` sobre el panel «Indicaciones / Sugerencias» | (c) | ESP-0001 | El ID del monto total rotula un panel de texto que no es el monto. El monto está en la franja verde. |
| ART-MKP-VEN-0001 | «Total Items: 3» (contador de ítems en la franja del total) | (b) | ESP-0001 | ESP-0001 solo calcula `montoTotal`. El contador de ítems no está especificado. |
| ART-MKP-VEN-0001 | Botón «Cancelar» (`VEN-BTN-ELIMINAR-VENTA`) | (c) | ESP-0004 | `VEN-BTN-ELIMINAR-VENTA` es la baja de una venta ya registrada (ESP-0004, desde la tabla de ventas). Aquí descarta el carrito en curso, acción que ninguna especificación define. |
| ART-MKP-VEN-0001 | `VEN-TBL-REGISTRO-VENTAS` (listado de ventas registradas) | (a) | ESP-0001, ESP-0002, ESP-0003, ESP-0004 | ESP-0001 lo actualiza al registrar y ESP-0002, ESP-0003 y ESP-0004 seleccionan ahí la venta. El mockup no muestra ningún listado de ventas. |
| ART-MKP-VEN-0001 | `VEN-BTN-ACTUALIZAR-VENTA` | (a) | ESP-0003 | ESP-0003 carga este mockup y lo presiona. No existe en la imagen. |
| ART-MKP-VEN-0002, 0003, 0004 | Pantallas de consulta, actualización y modal de baja | (e) | ESP-0002 (0002), ESP-0003 (0003), ESP-0004 (0004) | Las tres se citan y no existen como imagen. Sin pantalla quedan `VEN-FEC-FECHA-VENTA` como filtro, `VEN-BTN-CONFIRMAR-ACTUALIZACION`, `VEN-MDL-CONFIRMAR-ELIMINACION`, `VEN-TXT-MSJ`, `VEN-BTN-CONFIRMAR-SI` y `VEN-BTN-CONFIRMAR-NO`. |

**Otras observaciones (fuera de las cinco categorías):**

- Datos de ejemplo incoherentes: línea 1 muestra precio S/ 12.50 × 2 = S/ 39.00 y línea 2 muestra S/ 12.50 × 1 = S/ 12.00; el total (S/ 51.00) coincide con la suma de los totales mostrados, no con precio × cantidad.
- «Método de pago» se ve como un botón o caja de texto sin flecha de lista, pese a tener el ID de un combo.
- Las tarjetas muestran el mismo producto nueve veces (datos de ejemplo).

---

## Módulo 2 — Inventario (INV)

**Mockup revisado:** ART-MKP-INV-0001

### Componentes visibles

- Combo «-- Criterio / Proveedor --» → `INV-CMB-PROVEEDOR`
- Caja «Buscar por nombre o ID de producto…» → `INV-TXT-NOMBRE-PRODUCTO / INV-TXT-ID-PRODUCTO` (un solo control, dos IDs); botón «Buscar» → `INV-BTN-LEER-PRODUCTO`
- «Catálogo General» (lista de tarjetas) → `INV-TBL-PRODUCTOS`; combo «Filtrar por Categoría / Tipo» → `INV-CMB-PROVEEDOR`
- Tarjeta de producto: imagen «[IMG]» (sin ID visible; la guía no define un tipo de componente para imágenes, por lo que no se propone ID); nombre → `INV-TXT-NOMBRE-PRODUCTO`; acción terapéutica → `INV-TXT-ACCION-TERAPEUTICA`; precio → `INV-NUM-PRECIO-PRODUCTO`; botón «Seleccionar» → `INV-BTN-LEER-PRODUCTO`
- «Ficha Técnica del Producto»: visor «[ Visor 3D / Detalle de Medicamento ]» → `INV-PNL-PRODUCTO`; «ID Producto» → `INV-LBL-ID-PRODUCTO`; «Nombre Comercial» → `INV-TXT-NOMBRE-PRODUCTO`; «Acción Terapéutica» → `INV-TXT-ACCION-TERAPEUTICA`; «Precio de Venta (S/)» → `INV-NUM-PRECIO-PRODUCTO`; «Proveedor» → `INV-CMB-PROVEEDOR`
- «Gestión CRUD»: tabla (ID, Producto, Precio) → `INV-TBL-PRODUCTOS`; botones «Crear» → `INV-BTN-CREAR-PRODUCTO`, «Guardar» → `INV-BTN-ACTUALIZAR-PRODUCTO`, «Eliminar» → `INV-BTN-ELIMINAR-PRODUCTO`

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-INV-0001 | `INV-CMB-PROVEEDOR` (combo «Criterio / Proveedor», combo «Filtrar por Categoría / Tipo» y combo «Proveedor» de la ficha) | (c) | ESP-0005, ESP-0007 | El mismo ID rotula tres controles. En la especificación es el proveedor que abastece el producto. El segundo combo habla de categoría y tipo, no de proveedor. |
| ART-MKP-INV-0001 | Combo «Criterio / Proveedor» como filtro de búsqueda | (b) | ESP-0006 | ESP-0006 filtra solo por identificador y nombre. No hay filtro por proveedor. |
| ART-MKP-INV-0001 | Combo «Filtrar por Categoría / Tipo» | (d) | ESP-0006; TBL_PRODUCTOS | `TBL_PRODUCTOS` no tiene columna de categoría o tipo. Es el vacío ya conocido de «tipo y presentación del producto» (PEN-010). |
| ART-MKP-INV-0001 | Caja única con `INV-TXT-NOMBRE-PRODUCTO / INV-TXT-ID-PRODUCTO` | (c) | ESP-0006 | ESP-0006 captura dos entradas separadas (`INV-TXT-ID-PRODUCTO` e `INV-TXT-NOMBRE-PRODUCTO`) y valida «id O nombre». El mockup las fusiona en una caja. |
| ART-MKP-INV-0001 | Botón «Seleccionar» (`INV-BTN-LEER-PRODUCTO`) | (c) | ESP-0006 | Comparte ID con el botón «Buscar». En ESP-0006 `INV-BTN-LEER-PRODUCTO` ejecuta la consulta. Seleccionar una tarjeta es una acción distinta. |
| ART-MKP-INV-0001 | Panel `INV-PNL-PRODUCTO` (visor 3D / detalle de medicamento) | (b) | — | No aparece en ninguna especificación ni en el inventario de la guía. Posible pedido nuevo (mismo concepto que «Ver producto (3D)» de ventas). |
| ART-MKP-INV-0001 | Imagen «[IMG]» de la tarjeta y visor 3D | (d) | TBL_PRODUCTOS | Sin columna de imagen ni modelo. La imagen no tiene ID visible. |
| ART-MKP-INV-0001 | `INV-LBL-ID-PRODUCTO` con valor «PROD-00842» | (d) | ESP-0005; TBL_PRODUCTOS.id_producto | `id_producto` es INT autoincremental. El mockup muestra un código con prefijo alfabético. |
| ART-MKP-INV-0001 | Tabla `INV-TBL-PRODUCTOS` (ID, Producto, Precio) | (a) | ESP-0006 | ESP-0006 carga id, nombre, acción terapéutica, precio, estado y stock consolidado. Faltan acción terapéutica, estado y stock. |
| ART-MKP-INV-0001 | `INV-NUM-STOCK-MINIMO` | (a) | ESP-0005, ESP-0007, ILA-0005, ILA-0007 | `stock_minimo` es columna del diccionario y campo del alta y la actualización. La ficha no tiene el campo. |
| ART-MKP-INV-0002, 0003, 0004 | Pantallas de consulta, actualización y modal de baja | (e) | ESP-0006 (0002), ESP-0007 (0003), ESP-0008 (0004) | Las tres se citan y no existen. El mockup 0001 junta en una sola pantalla la consulta, el alta, la actualización y la baja. |

**Otras observaciones (fuera de las cinco categorías):**

- ESP-0007 presiona `INV-BTN-ACTUALIZAR-PRODUCTO` dos veces (abrir y guardar) y no usa `INV-BTN-CONFIRMAR-ACTUALIZACION`, que la guía §5 define para las demás fases. El mockup, con un solo botón «Guardar», es coherente con ESP-0007 pero no con la guía §5.
- «Seleccionar», el visor y las tarjetas describen un catálogo visual que ninguna especificación menciona.

---

## Módulo 3 — Documentación tributaria (DOC)

**Mockup revisado:** ART-MKP-DOC-0001; DOC-MDL-PREVISUALIZAR-COMPROBANTE

### Componentes visibles

- **ART-MKP-DOC-0001.** Combo «-- Tipo de Comprobante --» → `DOC-CMB-TIPO-COMPROBANTE`; caja «Buscar por N° Comprobante…» → `DOC-TXT-NUMERO-COMPROBANTE`; botón «Consultar» → `DOC-BTN-LEER-COMPROBANTE`
- «Ventas Disponibles»: combo «Filtrar Venta por Código o Fecha» → `DOC-CMB-CODIGO-VENTA`; tres tarjetas «Venta #VEN-2026-09x» → `DOC-CMB-CODIGO-VENTA`; monto de cada tarjeta → `DOC-LBL-MONTO-TOTAL`
- «Emisión de Comprobante»: «Venta Seleccionada» → `DOC-CMB-CODIGO-VENTA`; «Tipo de Comprobante» → `DOC-CMB-TIPO-COMPROBANTE`; «Número de Comprobante» → `DOC-TXT-NUMERO-COMPROBANTE`; «Fecha de Emisión» → `DOC-FEC-FECHA-EMISION`
- «Historial de Emisión»: tabla (N° Comp., Tipo, Total) → `DOC-TBL-COMPROBANTES`; «Monto Total Calculado» → `DOC-LBL-MONTO-TOTAL`; botones «Emitir» → `DOC-BTN-CREAR-COMPROBANTE`, «Modificar» → `DOC-BTN-ACTUALIZAR-COMPROBANTE`, «Anular» → `DOC-BTN-ELIMINAR-COMPROBANTE`
- **DOC-MDL-PREVISUALIZAR-COMPROBANTE (modal).** Contenedor del modal «Vista Previa del Comprobante Electrónico» (sin ID dentro de la imagen; el ID figura solo en el nombre del archivo)
- Campos «DNI / RUC del cliente» y «Nombre / Razón Social» (sin ID visible; IDs propuestos según la guía: `DOC-TXT-DOCUMENTO-CLIENTE` y `DOC-TXT-NOMBRE-CLIENTE`)
- Vista del comprobante: datos de la empresa, número «B001-00001284», cliente, fecha con hora, venta de referencia, forma de pago, detalle (Cant., Descripción con lote, P. Unit., Subtotal), Subtotal, IGV (18%), Total, «Son: …», hash «pendiente de generar al emitir» (sin ID; ID propuesto `DOC-PNL-VISTA-PREVIA`)
- Botones «Descargar borrador PDF» → `DOC-BTN-DESCARGAR-PDF`; «← Volver a editar» → `DOC-BTN-CANCELAR-EMISION`; «✓ Confirmar y Emitir» → `DOC-BTN-CONFIRMAR-EMISION`
- Aviso «borrador de previsualización… se enviará al OSE/SUNAT» (texto sin ID; ID propuesto `DOC-LBL-AVISO-PREVISUALIZACION`)

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-DOC-0001 | `DOC-CMB-CODIGO-VENTA` (filtro, tarjetas de venta y «Venta Seleccionada») | (c) | ESP-0009, ESP-0011 | Tres controles con el mismo ID. Las tarjetas son una lista, no un combo. |
| ART-MKP-DOC-0001 | Filtro «Filtrar Venta por Código o Fecha» | (b) | ESP-0009 | ESP-0009 solo selecciona la venta por código. No hay filtro por fecha de venta. |
| ART-MKP-DOC-0001 | `DOC-CMB-TIPO-COMPROBANTE` con valor «Boleta Electrónica» | (d) | ESP-0009; TBL_COMPROBANTES_TRIBUTARIOS.tipo_comprobante | El dominio es 'Boleta' o 'Factura'. El mockup muestra «Boleta Electrónica». |
| ART-MKP-DOC-0001 | Tabla `DOC-TBL-COMPROBANTES` (N° Comp., Tipo, Total) | (a) | ESP-0010 | ESP-0010 carga número, tipo, fecha de emisión, monto y estado del documento. Faltan fecha de emisión y estado, necesario para ver los anulados. |
| ART-MKP-DOC-0001 | `DOC-BTN-CONFIRMAR-SI` | (a) | ESP-0009 | ESP-0009 confirma la emisión con este botón. En la pantalla principal no hay confirmación. Quien confirma es `DOC-BTN-CONFIRMAR-EMISION` del modal de previsualización. |
| DOC-MDL-PREVISUALIZAR-COMPROBANTE.png | Nombre del artefacto | (c) | Guía §4 | No sigue `ART-MKP-[MÓDULO]-[NÚMERO]`: es un ID de componente (`MOD-MDL-FUNCIÓN`) usado como nombre de archivo. Los números DOC-0001 a DOC-0004 ya están asignados por convención a las cuatro fases del CRUD. |
| DOC-MDL-PREVISUALIZAR-COMPROBANTE.png | `DOC-MDL-PREVISUALIZAR-COMPROBANTE` (modal de previsualización) | (b) | ESP-0009, ILA-0009 | El identificador no figura en el catálogo, la guía ni el diccionario. ESP-0009 registra el comprobante directamente con `DOC-BTN-CONFIRMAR-SI`, sin paso de previsualización. Posible pedido nuevo. |
| DOC-MDL-PREVISUALIZAR-COMPROBANTE.png | `DOC-BTN-DESCARGAR-PDF` | (b) | — | Descargar un borrador en PDF no está especificado en ningún lado. |
| DOC-MDL-PREVISUALIZAR-COMPROBANTE.png | `DOC-BTN-CANCELAR-EMISION` («Volver a editar») | (b) | ESP-0009 | ESP-0009 no define cancelar ni volver a editar antes de emitir. |
| DOC-MDL-PREVISUALIZAR-COMPROBANTE.png | `DOC-BTN-CONFIRMAR-EMISION` | (c) | ESP-0009 | Cumple la función de `DOC-BTN-CONFIRMAR-SI` en ESP-0009, con otro nombre. La guía §5 define el patrón CONFIRMAR-SI / CONFIRMAR-NO. |
| DOC-MDL-PREVISUALIZAR-COMPROBANTE.png | Campos «DNI / RUC del cliente» y «Nombre / Razón Social» (sin ID) | (d) | ESP-0009; TBL_COMPROBANTES_TRIBUTARIOS | No hay columnas ni tabla de clientes. El registro de clientes es un pedido de la Entrevista 1 aún sin decidir (PEN-010). Sin ID visible. |
| DOC-MDL-PREVISUALIZAR-COMPROBANTE.png | Subtotal sin IGV, IGV 18%, «Son: …», hash y datos de la empresa | (d) | ESP-0009; TBL_COMPROBANTES_TRIBUTARIOS | El diccionario no tiene columnas para base imponible, IGV, hash ni datos de la empresa. La fecha de emisión muestra hora (14:32) y `fecha_emision` es DATE. |
| DOC-MDL-PREVISUALIZAR-COMPROBANTE.png | Aviso «se enviará al OSE/SUNAT… no podrá modificar después» | (b) | ESP-0009, ESP-0011, ESP-0012 | Ningún requisito describe un envío a OSE/SUNAT. Lo más cercano es PEN-003 (estado «declarado ante SUNAT»), abierto con la dueña. El aviso afirma que luego no se puede modificar, lo que choca con ESP-0011 y ESP-0012 (modificar y anular). |
| ART-MKP-DOC-0002, 0003, 0004 | Pantallas de consulta, actualización y modal de anulación | (e) | ESP-0010 (0002), ESP-0011 (0003), ESP-0012 (0004) | Las tres se citan y no existen. Sin pantalla quedan `DOC-BTN-CONFIRMAR-ACTUALIZACION`, `DOC-MDL-CONFIRMAR-ELIMINACION`, `DOC-TXT-MSJ`, `DOC-BTN-CONFIRMAR-SI` y `DOC-BTN-CONFIRMAR-NO`. |

**Otras observaciones (fuera de las cinco categorías):**

- Datos de ejemplo: el código de venta «VEN-2026-091» y el comprobante «B001-00001284» no son enteros; `id_venta` es INT. Los precios del modal (Paracetamol S/ 3.50) difieren de los del mockup de ventas (S/ 12.50).
- La filigrana «PREVISUALIZACIÓN» de la vista previa no es un componente.

---

## Módulo 4 — Alertas de vencimiento (ALV)

**Mockup revisado:** ART-MKP-ALV-0001

### Componentes visibles

- Combo «-- Seleccionar Remesa / Lote --» → `ALV-CMB-LOTE-ALERTA`; botón «Filtrar Alertas» → `ALV-BTN-LEER-ALERTA`
- «Lotes en Riesgo»: combo «Filtrar por Estado de Vencimiento» → `ALV-CMB-LOTE-ALERTA`; tres tarjetas de lote → `ALV-CMB-LOTE-ALERTA`; fecha «Vence: …» de cada tarjeta → `ALV-LBL-FECHA-VENCIMIENTO`
- «Control Sanitario de Lote»: «Lote / Remesa Afectada» → `ALV-CMB-LOTE-ALERTA`; «Fecha de Vencimiento Registrada» → `ALV-LBL-FECHA-VENCIMIENTO`; casilla «Lote Defectuoso / Bloqueado» → `ALV-CHK-LOTE-DEFECTUOSO`; «Código Alerta DIGEMID» → `ALV-TXT-ALERTA-DIGEMID`
- «Tablero de Alertas»: tabla (Lote, Venc., Defect.) → `ALV-TBL-ALERTAS-VENCIMIENTO`; botones «Bloquear Lote» → `ALV-BTN-CREAR-ALERTA`, «Modificar» → `ALV-BTN-ACTUALIZAR-ALERTA`, «Confirmar Actualización» → `ALV-BTN-CONFIRMAR-ACTUALIZACION`, «Liberar Lote / Eliminar Alerta» → `ALV-BTN-ELIMINAR-ALERTA`

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-ALV-0001 | `ALV-CMB-LOTE-ALERTA` (combo superior, filtro por estado, tarjetas y «Lote / Remesa Afectada») | (c) | ESP-0013, ESP-0014, ESP-0015 | Cuatro controles con el mismo ID. El filtro «Estado de Vencimiento» no es un combo de lote. |
| ART-MKP-ALV-0001 | Combo «Filtrar por Estado de Vencimiento» | (b) | ESP-0013 | ESP-0013 consulta sin filtros: une los lotes próximos a vencer con los retirados. |
| ART-MKP-ALV-0001 | Casilla «Lote Defectuoso / Bloqueado» (`ALV-CHK-LOTE-DEFECTUOSO`) | (c) | ESP-0014, ESP-0015 | El rótulo dice «Bloqueado», pero el campo es `es_lote_defectuoso`. El bloqueo ocurre al registrar la alerta (ESP-0014) y no depende de la casilla. |
| ART-MKP-ALV-0001 | Botón «Confirmar Actualización» (`ALV-BTN-CONFIRMAR-ACTUALIZACION`) en la pantalla principal | (c) | ESP-0015 | ESP-0015 sitúa la confirmación en ART-MKP-ALV-0002, que no existe. En el mockup todo está en una pantalla. |
| ART-MKP-ALV-0002, 0003 | Pantalla de modificación y modal de eliminación | (e) | ESP-0015 (0002), ESP-0016 (0003) | Ambas se citan y no existen. Sin pantalla quedan `ALV-MDL-CONFIRMAR-ELIMINACION`, `ALV-TXT-MSJ`, `ALV-BTN-CONFIRMAR-SI` y `ALV-BTN-CONFIRMAR-NO`. |

**Otras observaciones (fuera de las cinco categorías):**

- Las tarjetas de lote usan franjas roja y amarilla según el riesgo. Ninguna especificación define ese código de color.
- Formatos de fecha mezclados en una misma pantalla: «2026-10-15», «15/10/26» y «02/11/26».

---

## Módulo 5 — Métodos de pago (PAG)

**Mockup revisado:** ART-MKP-PAG-0001

### Componentes visibles

- Combo «-- Filtrar por Estado --» → `PAG-CMB-ESTADO-METODO`; caja «Buscar método de pago…» → `PAG-TXT-METODO-PAGO`; botón «Buscar» → `PAG-BTN-LEER-METODO`
- «Selecciona el Método de Pago»: cuatro tarjetas (icono, nombre, descripción, insignia de estado) → `PAG-TBL-METODOS-PAGO`
- «Ficha del Método de Pago»: «Nombre del Método» → `PAG-TXT-METODO-PAGO`; «Estado» → `PAG-CMB-ESTADO-METODO`; panel «Previsualización del método seleccionado» (sin ID; ID propuesto `PAG-PNL-PREVISUALIZACION-METODO`)
- «Gestión CRUD»: tabla (Método, Estado) → `PAG-TBL-METODOS-PAGO`; botones «Crear» → `PAG-BTN-CREAR-METODO`, «Guardar» → `PAG-BTN-ACTUALIZAR-METODO`, «Eliminar» → `PAG-BTN-ELIMINAR-METODO`

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-PAG-0001 | `PAG-TBL-METODOS-PAGO` (rejilla de tarjetas y tabla «Gestión CRUD») | (c) | ESP-0017 a ESP-0019 | Un mismo ID rotula dos componentes distintos: la lista de tarjetas y la tabla. |
| ART-MKP-PAG-0001 | Caja de búsqueda por nombre (`PAG-TXT-METODO-PAGO`) | (b) | ESP-0018 | ESP-0018 filtra solo por estado. La búsqueda por nombre no está especificada. El mismo ID rotula además el campo de captura de la ficha. |
| ART-MKP-PAG-0001 | `PAG-BTN-LEER-METODO` | (b) | ESP-0018 | El botón está en la guía (recomendado en §7.2) pero ESP-0018 no lo presiona: la especificación carga la tabla directamente al filtrar por estado. |
| ART-MKP-PAG-0001 | Descripción («Pago en mostrador», «Billetera digital», «POS / Visa») e icono de cada método | (d) | ESP-0017; TBL_METODOS_PAGO | `TBL_METODOS_PAGO` solo tiene id_metodo_pago, nombre_metodo y estado. No hay descripción ni icono. |
| ART-MKP-PAG-0001 | Panel «Previsualización del método seleccionado» (sin ID) | (b) | — | Ninguna especificación lo pide. Posible pedido nuevo. Sin ID visible. |
| ART-MKP-PAG-0001 | `PAG-BTN-CONFIRMAR-ACTUALIZACION` | (a) | ESP-0019 | ESP-0019 presiona este botón para guardar. El mockup solo tiene «Guardar», rotulado `PAG-BTN-ACTUALIZAR-METODO`. |
| ART-MKP-PAG-0002, 0003, 0004 | Pantallas de consulta, actualización y modal de baja | (e) | ESP-0018 (0002), ESP-0019 (0003), ESP-0020 (0004) | Las tres se citan y no existen. Sin pantalla quedan `PAG-MDL-CONFIRMAR-ELIMINACION`, `PAG-TXT-MSJ`, `PAG-BTN-CONFIRMAR-SI` y `PAG-BTN-CONFIRMAR-NO`. |

**Otras observaciones (fuera de las cinco categorías):**

- El mockup ya usa `PAG-BTN-CREAR-METODO`, nombre recomendado en la guía §7.2.

---

## Módulo 6 — Reportes de ventas (REP)

**Mockup revisado:** ART-MKP-REP-0001

### Componentes visibles

- Barra superior: combo «-- Tipo de Reporte --» → `REP-CMB-TIPO-REPORTE`; combo «-- Formato --» → `REP-CMB-FORMATO-REPORTE`; fechas 01/09/2026 y 30/09/2026 → `REP-FEC-FECHA-INICIO` y `REP-FEC-FECHA-FIN`; botón «Generar» → `REP-BTN-GENERAR-REPORTE`
- «Parámetros y Búsqueda»: caja «Código o tipo…» → `REP-BTN-LEER-REPORTE`; botón «Filtrar Historial» → `REP-BTN-LEER-REPORTE`; «Reportes Recientes» (tarjetas «Ventas por Fecha», «Ventas por Producto») sin ID (ID propuesto `REP-TBL-REPORTES-RECIENTES`)
- «Vista Previa del Reporte»: tabla (Fecha, Producto, Cantidad, Total) con total S/ 359.00 → `REP-TBL-REPORTES`
- «Historial de Reportes»: tabla (Tipo, Fecha, Formato) → `REP-TBL-REPORTES`; botones «Generar» → `REP-BTN-GENERAR-REPORTE`, «Modificar» → `REP-BTN-ACTUALIZAR-REPORTE`, «Eliminar» → `REP-BTN-ELIMINAR-REPORTE`

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-REP-0001 | `REP-TBL-REPORTES` (vista previa e historial) | (c) | ESP-0021 a ESP-0024 | El mismo ID rotula la tabla de vista previa del contenido y la tabla del historial, que son componentes distintos. |
| ART-MKP-REP-0001 | `REP-BTN-GENERAR-REPORTE` (botón de la barra superior y botón del panel «Historial») | (c) | ESP-0021 | Dos botones con el mismo ID. |
| ART-MKP-REP-0001 | Caja «Código o tipo…» (`REP-BTN-LEER-REPORTE`) | (c) | ESP-0022 | Un campo de texto rotulado con el ID de un botón. ESP-0022 tampoco define ningún criterio de búsqueda. |
| ART-MKP-REP-0001 | Búsqueda por código o tipo y «Filtrar Historial» | (b) | ESP-0022 | ESP-0022 carga el historial completo con `REP-BTN-LEER-REPORTE`, sin filtros. |
| ART-MKP-REP-0001 | Valores de tipo «Por Fecha» y «Por Producto» | (b) | ESP-0021, ILA-0021 | El dominio de `tipo_reporte` sigue sin definir (PEN-001, en espera de la dueña). El diccionario cita 'Ventas Diarias' como ejemplo. Los valores del mockup no figuran en ninguna parte. |
| ART-MKP-REP-0001 | Vista previa por producto (columna «Producto» y filas por venta) | (b) | ESP-0021 | ESP-0021 consolida monto recaudado, cantidad de productos, fechas y usuario responsable. No pide detalle por producto. |
| ART-MKP-REP-0001 | `usuario_responsable` en la vista previa | (a) | ESP-0021, ILA-0021, EDU-0010 (DEC-001) | EDU-0010 exige la identificación del personal que realizó cada venta. La vista previa no muestra ningún usuario. |
| ART-MKP-REP-0001 | Tabla «Historial de Reportes» (Tipo, Fecha, Formato) | (a) | ESP-0022 | ESP-0022 carga id, tipo, fecha de inicio, fecha de fin y total recaudado. Faltan el intervalo (el mockup muestra una sola fecha) y `total_recaudado`. |
| ART-MKP-REP-0001 | «Reportes Recientes» (sin ID) | (b) | — | Lista de tarjetas con los últimos reportes que ninguna especificación define. Sin ID visible. |
| ART-MKP-REP-0002, 0003 | Pantalla de consulta y de actualización de intervalo | (e) | ESP-0022, ESP-0023, ESP-0024 (0002); ESP-0023 e ILA-0023 (0003) | Ambas se citan y no existen. Sin pantalla quedan `REP-BTN-CONFIRMAR-ACTUALIZACION`, `REP-MDL-CONFIRMAR-ELIMINACION`, `REP-TXT-MSJ`, `REP-BTN-CONFIRMAR-SI` y `REP-BTN-CONFIRMAR-NO`. ESP-0023 y ESP-0024 cargan ART-MKP-REP-0002, mientras ILA-0023 cita ART-MKP-REP-0003. |

**Otras observaciones (fuera de las cinco categorías):**

- El mockup ya usa `REP-BTN-LEER-REPORTE`, nombre recomendado en la guía §7.2.

---

## Módulo 7 — Devoluciones a proveedores (DEV)

**Mockup revisado:** ART-MKP-DEV-0001

### Componentes visibles

- Combo «-- Filtrar por Proveedor --» → `DEV-CMB-PROVEEDOR`; caja «Buscar por N° de orden o lote…» → `DEV-TXT-BUSCAR-ORDEN` (ID con asterisco, no definido en la guía); botón «Buscar» → `DEV-BTN-LEER-DEVOLUCION`
- «Órdenes de Devolución»: tarjetas «ORD-2026-014», «-013», «-012» (proveedor, líneas, estado, fecha) → `DEV-TBL-ORDENES-DEVOLUCION`
- «Nueva Orden de Devolución»: «Proveedor» → `DEV-CMB-PROVEEDOR`; «Fecha de Devolución» → `DEV-FEC-FECHA-DEVOLUCION`; «Motivo» → `DEV-CMB-MOTIVO-DEVOLUCION`; «Lote / Remesa» → `DEV-CMB-LOTE-DEVOLUCION`; «Cantidad a Devolver» → `DEV-NUM-CANTIDAD-DEVOLVER`; «Comentario (obligatorio si es error)» → `DEV-TXA-COMENTARIO`
- «Detalle de la Orden»: tabla (Producto, Lote, Cant.) → `DEV-TBL-DETALLE-DEVOLUCION`; botones «Crear Orden» → `DEV-BTN-CREAR-DEVOLUCION`, «Actualizar» → `DEV-BTN-ACTUALIZAR-DEVOLUCION`, «Eliminar» → `DEV-BTN-ELIMINAR-DEVOLUCION`

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-DEV-0001 | `DEV-CMB-PROVEEDOR` (filtro superior y combo del formulario) | (c) | ESP-0025, ESP-0027 | Un mismo ID rotula el filtro de búsqueda y el proveedor de la orden. |
| ART-MKP-DEV-0001 | Caja de búsqueda (`DEV-TXT-BUSCAR-ORDEN*`) | (b) | ESP-0026 | El ID no existe en la guía ni en el catálogo (el asterisco lo marca como propuesto). ESP-0026 no define ninguna búsqueda por número de orden o lote. Posible pedido nuevo. |
| ART-MKP-DEV-0001 | Número de orden «ORD-2026-014» y contador «3 líneas» en las tarjetas | (d) | ESP-0026; TBL_ORDENES_DEVOLUCION | `id_devolucion` es INT. El «N° de orden» con formato alfanumérico y el conteo de líneas no tienen columna. |
| ART-MKP-DEV-0001 | Botón para agregar la línea (remesa y cantidad) al detalle | (a) | ESP-0025, ILA-0025 | ESP-0025 repite «AGREGAR (lote, cantidad) A detalle» hasta que el usuario no agregue más remesas, y ILA-0025 (paso 6) dice que el usuario «agrega la línea al detalle». Ni la especificación ni la guía definen un ID para el botón, y el mockup no lo tiene. |
| ART-MKP-DEV-0001 | Control de estado de la orden (`estadoNuevo = ESTADO_SELECCIONADO`) | (a) | ESP-0027 | ESP-0027 permite pasar la orden a 'Pendiente' o 'Completada'. No hay ID en el catálogo ni control en el mockup. El estado solo aparece como texto en las tarjetas. |
| ART-MKP-DEV-0002, 0003, 0004 | Pantalla de consulta, actualización y modal de baja | (e) | ESP-0026 (0002), ESP-0027 (0003), ESP-0028 (0004) | Las tres se citan y no existen. Sin pantalla quedan `DEV-BTN-CONFIRMAR-ACTUALIZACION`, `DEV-MDL-CONFIRMAR-ELIMINACION`, `DEV-TXT-MSJ`, `DEV-BTN-CONFIRMAR-SI` y `DEV-BTN-CONFIRMAR-NO`. |

**Otras observaciones (fuera de las cinco categorías):**

- Las tarjetas usan franjas naranja y verde por estado. Ninguna especificación define ese código de color.

---

## Módulo 8 — Restricciones de venta (RES)

**Mockup revisado:** ART-MKP-RES-0001

### Componentes visibles

- Caja «Buscar medicamento restringido…» → `RES-CMB-MEDICAMENTO`; combo «-- Tipo --» → `RES-CMB-TIPO-RESTRICCION`; combo «-- Estado --» → `RES-CMB-ESTADO-ALERTA`; botón «Buscar» → `RES-BTN-LEER-RESTRICCION`
- «Restricciones Activas»: tres tarjetas (medicamento, tipo, texto) → `RES-TBL-RESTRICCIONES-VENTA`
- «Ficha de Restricción»: «Medicamento» → `RES-CMB-MEDICAMENTO`; «Tipo de Restricción» → `RES-CMB-TIPO-RESTRICCION`; «Condición del Cliente» → `RES-TXT-CONDICION-CLIENTE`; «Motivo / Advertencia» → `RES-TXA-MOTIVO-ADVERTENCIA`; «Estado» → `RES-CMB-ESTADO-ALERTA`
- «Tabla de Restricciones» (Medicamento, Tipo, Estado) → `RES-TBL-RESTRICCIONES-VENTA`; botones «Crear» → `RES-BTN-CREAR-RESTRICCION`, «Actualizar» → `RES-BTN-ACTUALIZAR-RESTRICCION`, «Eliminar» → `RES-BTN-ELIMINAR-RESTRICCION`

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-RES-0001 | Caja de búsqueda «Buscar medicamento restringido…» (`RES-CMB-MEDICAMENTO`) | (c) | ESP-0030 | Un campo de texto con el ID de un combo. En ESP-0030 el medicamento se elige de una lista. |
| ART-MKP-RES-0001 | Filtros por tipo y por estado (`RES-CMB-TIPO-RESTRICCION`, `RES-CMB-ESTADO-ALERTA`) en la barra de búsqueda | (b) | ESP-0030 | ESP-0030 filtra solo por medicamento. |
| ART-MKP-RES-0001 | `RES-TBL-RESTRICCIONES-VENTA` (tarjetas de «Restricciones Activas» y tabla) | (c) | ESP-0029 a ESP-0032 | El mismo ID rotula dos componentes distintos. |
| ART-MKP-RES-0001 | Tabla «Tabla de Restricciones» (Medicamento, Tipo, Estado) | (a) | ESP-0030 | ESP-0030 carga nombre, condición del cliente, tipo, motivo de advertencia y estado. Faltan condición y motivo. |
| ART-MKP-RES-0002, 0003, 0004 | Pantalla de consulta, modificación y modal de desactivación | (e) | ESP-0030 (0002), ESP-0031 (0003), ESP-0032 (0004) | Las tres se citan y no existen. Sin pantalla quedan `RES-BTN-CONFIRMAR-ACTUALIZACION`, `RES-MDL-CONFIRMAR-ELIMINACION`, `RES-TXT-MSJ`, `RES-BTN-CONFIRMAR-SI` y `RES-BTN-CONFIRMAR-NO`. |

**Otras observaciones (fuera de las cinco categorías):**

- El mockup ya usa los nombres recomendados de la guía §7.2 (`RES-BTN-CREAR-RESTRICCION`, `RES-BTN-ACTUALIZAR-RESTRICCION`, `RES-BTN-ELIMINAR-RESTRICCION`, `RES-BTN-LEER-RESTRICCION`).
- El valor de ejemplo de «Condición del Cliente» («Mayor de 18 años con receta») no es un grupo de riesgo como los del diccionario ('Gestante', 'Neonato', 'Oncologico').

---

## Módulo 9 — Usuarios (USR)

**Mockup revisado:** ART-MKP-USR-0001

### Componentes visibles

- Caja «Buscar por nombre de usuario…» → `USR-TXT-NOMBRE-USUARIO`; combo «-- Rol --» → `USR-CMB-ROL`; botón «Buscar» → `USR-BTN-LEER-USUARIO`
- «Cuentas de Usuario»: tres tarjetas (usuario, rol, estado) → `USR-TBL-USUARIOS`
- «Ficha de Usuario»: «Nombre de Usuario» → `USR-TXT-NOMBRE-USUARIO`; «Contraseña» (enmascarada) → `USR-PWD-CONTRASENA`; «Rol» → `USR-CMB-ROL`; «Estado» → `USR-CMB-ESTADO-USUARIO`
- «Tabla de Usuarios» (Usuario, Rol, Estado) → `USR-TBL-USUARIOS`; botones «Crear» → `USR-BTN-CREAR-USUARIO`, «Actualizar» → `USR-BTN-ACTUALIZAR-USUARIO`, «Eliminar» → `USR-BTN-ELIMINAR-USUARIO`

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-USR-0001 | `USR-TBL-USUARIOS` (tarjetas «Cuentas de Usuario» y tabla) | (c) | ESP-0033 a ESP-0036 | El mismo ID rotula dos componentes distintos. |
| ART-MKP-USR-0001 | `USR-TXT-NOMBRE-USUARIO` y `USR-CMB-ROL` (barra de búsqueda y ficha) | (c) | ESP-0033, ESP-0034, ESP-0035 | Cada ID rotula dos controles: el criterio de búsqueda y el campo de captura. |
| ART-MKP-USR-0001 | Valor «Admin» en la tabla de usuarios | (d) | ESP-0033; TBL_USUARIOS.rol | El dominio es 'Administrador' o 'Tecnico'. Las tarjetas dicen «Administrador», la tabla dice «Admin». |
| ART-MKP-USR-0001 | Columna `id_usuario` de la tabla | (a) | ESP-0034 | ESP-0034 carga id_usuario, nombre_usuario, rol y estado. La tabla no muestra el identificador. |
| ART-MKP-USR-0002, 0003, 0004 | Pantalla de consulta, actualización y modal de baja | (e) | ESP-0034 (0002), ESP-0035 (0003), ESP-0036 (0004) | Las tres se citan y no existen. Sin pantalla quedan `USR-BTN-CONFIRMAR-ACTUALIZACION`, `USR-MDL-CONFIRMAR-ELIMINACION`, `USR-TXT-MSJ`, `USR-BTN-CONFIRMAR-SI` y `USR-BTN-CONFIRMAR-NO`. |

**Otras observaciones (fuera de las cinco categorías):**

- El campo de contraseña aparece enmascarado, como pide la guía (PWD).

---

## Módulo 10 — Proveedores (PRV)

**Mockup revisado:** ART-MKP-PRV-0001

### Componentes visibles

- Caja «Buscar por RUC o razón social…» → `PRV-TXT-RUC / PRV-TXT-RAZON-SOCIAL` (un control, dos IDs); combo «-- Estado --» → `PRV-CMB-ESTADO-PROVEEDOR`; botón «Buscar» → `PRV-BTN-LEER-PROVEEDOR`
- «Proveedores»: tres tarjetas (razón social, RUC, estado) → `PRV-TBL-PROVEEDORES`
- «Ficha de Proveedor»: «RUC» → `PRV-TXT-RUC`; «Razón Social» → `PRV-TXT-RAZON-SOCIAL`; «Teléfono» → `PRV-TXT-TELEFONO`; «Estado» → `PRV-CMB-ESTADO-PROVEEDOR`
- «Tabla de Proveedores» (RUC, Razón Social, Estado) → `PRV-TBL-PROVEEDORES`; botones «Crear» → `PRV-BTN-CREAR-PROVEEDOR`, «Actualizar» → `PRV-BTN-ACTUALIZAR-PROVEEDOR`, «Eliminar» → `PRV-BTN-ELIMINAR-PROVEEDOR`

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-PRV-0001 | Caja única con `PRV-TXT-RUC / PRV-TXT-RAZON-SOCIAL` | (c) | ESP-0038 | ESP-0038 usa dos entradas separadas y valida «RUC O razón social». El mockup tiene una sola caja con ambos IDs. |
| ART-MKP-PRV-0001 | `PRV-TBL-PROVEEDORES` (tarjetas y tabla) | (c) | ESP-0037 a ESP-0040 | El mismo ID rotula dos componentes distintos. |
| ART-MKP-PRV-0001 | Filtro por estado (`PRV-CMB-ESTADO-PROVEEDOR`) en la barra de búsqueda | (b) | ESP-0038 | ESP-0038 filtra por RUC y razón social, no por estado. |
| ART-MKP-PRV-0001 | Tabla «Tabla de Proveedores» (RUC, Razón Social, Estado) | (a) | ESP-0038 | ESP-0038 carga id_proveedor, ruc, razón social, teléfono y estado. Faltan el identificador y el teléfono. |
| ART-MKP-PRV-0002, 0003, 0004 | Pantalla de consulta, actualización y modal de baja | (e) | ESP-0038 (0002), ESP-0039 (0003), ESP-0040 (0004) | Las tres se citan y no existen. Sin pantalla quedan `PRV-BTN-CONFIRMAR-ACTUALIZACION`, `PRV-MDL-CONFIRMAR-ELIMINACION`, `PRV-TXT-MSJ`, `PRV-BTN-CONFIRMAR-SI` y `PRV-BTN-CONFIRMAR-NO`. |

---

## Módulo 11 — Lotes (LOT)

**Mockup revisado:** ART-MKP-LOT-0001

### Componentes visibles

- Caja «Buscar por número de lote…» → `LOT-TXT-NUMERO-LOTE`; combo «-- Producto --» → `LOT-CMB-PRODUCTO`; combo «-- Estado --» → `LOT-CMB-ESTADO-LOTE`; botón «Buscar» → `LOT-BTN-LEER-LOTE`
- «Remesas / Lotes»: tres tarjetas (lote, producto, vencimiento, stock) → `LOT-TBL-LOTES`
- «Ficha de Remesa»: «Producto» → `LOT-CMB-PRODUCTO`; «Número de Lote» → `LOT-TXT-NUMERO-LOTE`; «Fecha de Vencimiento» → `LOT-FEC-FECHA-VENCIMIENTO`; «Stock de la Remesa» → `LOT-NUM-STOCK-LOTE`; «Fecha de Ingreso» → `LOT-FEC-FECHA-INGRESO`; «Estado» → `LOT-CMB-ESTADO-LOTE`
- «Tabla de Lotes» (Lote, Vence, Stock) → `LOT-TBL-LOTES`; botones «Crear» → `LOT-BTN-CREAR-LOTE`, «Actualizar» → `LOT-BTN-ACTUALIZAR-LOTE`, «Eliminar» → `LOT-BTN-ELIMINAR-LOTE`

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-LOT-0001 | `LOT-CMB-PROVEEDOR-LOTE` | (a) | ESP-0041, ESP-0043, ILA-0041, ILA-0043 | `TBL_LOTES.id_proveedor` se captura en el alta y la actualización. La ficha no tiene el campo. |
| ART-MKP-LOT-0001 | `LOT-TBL-LOTES` (tarjetas y tabla) | (c) | ESP-0041 a ESP-0044 | El mismo ID rotula dos componentes distintos. |
| ART-MKP-LOT-0001 | `LOT-TXT-NUMERO-LOTE`, `LOT-CMB-PRODUCTO` y `LOT-CMB-ESTADO-LOTE` (barra de búsqueda y ficha) | (c) | ESP-0041, ESP-0042, ESP-0043 | Cada ID rotula dos controles: el criterio de búsqueda y el campo de captura. |
| ART-MKP-LOT-0001 | Combo «Estado» de la ficha (`LOT-CMB-ESTADO-LOTE`) | (c) | ESP-0043 (DEC-010) | ESP-0043 lo muestra de solo lectura, porque el estado se deriva de las existencias (DEC-010), y ESP-0041 asigna 'Disponible'. En la ficha del mockup es un combo editable. |
| ART-MKP-LOT-0001 | Tabla «Tabla de Lotes» (Lote, Vence, Stock) | (a) | ESP-0042 | ESP-0042 carga nombre del medicamento, número de lote, vencimiento, stock y estado. Faltan el medicamento y el estado. |
| ART-MKP-LOT-0002, 0003, 0004 | Pantalla de consulta, actualización y modal de baja | (e) | ESP-0042 (0002), ESP-0043 (0003), ESP-0044 (0004) | Las tres se citan y no existen. Sin pantalla quedan `LOT-BTN-CONFIRMAR-ACTUALIZACION`, `LOT-MDL-CONFIRMAR-ELIMINACION`, `LOT-TXT-MSJ`, `LOT-BTN-CONFIRMAR-SI` y `LOT-BTN-CONFIRMAR-NO`. |

---

## Módulo 12 — Reabastecimiento (REA)

**Mockup revisado:** ART-MKP-REA-0001

### Componentes visibles

- Los identificadores del mockup llevan un asterisco final (`REA-…*`). Ninguna parte del repositorio define qué significa; se interpreta como «propuesto».
- Encabezado «Panel de Reabastecimiento»; botón «Generar Lista» → `REA-BTN-GENERAR-LISTA*`
- Tres tarjetas de resumen: «3 Productos bajo mínimo» → `REA-NUM-STOCK-ACTUAL*`; «5 Prioridad alta» → `REA-CMB-PRIORIDAD*`; «2 Pendientes de cotizar» → `REA-TBL-LISTA-REABASTECIMIENTO*`
- «Lista de Reabastecimiento»: caja «Filtrar por producto…» → `REA-CMB-PRODUCTO*`; tabla (Producto, Stock Actual, Stock Mínimo, Sugerido, Prioridad con insignias CRÍTICO / ALTO / MEDIO) → `REA-TBL-LISTA-REABASTECIMIENTO*`
- «Acciones»: «Producto Seleccionado» → `REA-CMB-PRODUCTO*`; «Cantidad Sugerida» → `REA-NUM-CANTIDAD-SUGERIDA*`; botones «Actualizar Ítem» → `REA-BTN-ACTUALIZAR-ITEM*`, «Registrar en Lista» → `REA-BTN-CREAR-ITEM*`, «Eliminar de Lista» → `REA-BTN-ELIMINAR-ITEM*`

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-REA-0001 | Sufijo `*` en todos los identificadores | (c) | Guía §1 | Ningún ID de la guía lleva asterisco. Los IDs que coinciden con la especificación (`REA-CMB-PRODUCTO`, `REA-BTN-CREAR-ITEM`, etc.) difieren por ese sufijo. |
| ART-MKP-REA-0001 | Botón «Generar Lista» (`REA-BTN-GENERAR-LISTA*`) | (b) | ESP-0045, RNF-0010 | La lista se alimenta con el proceso periódico (RNF-0010, proceso 2) y con el alta manual de ESP-0045. Ninguna especificación define un botón para generarla a demanda. |
| ART-MKP-REA-0001 | Tarjetas de resumen («3 bajo mínimo», «5 prioridad alta», «2 pendientes de cotizar») | (b) | — | Contadores que ninguna especificación pide. Sus IDs son de otros tipos de componente: `REA-NUM-STOCK-ACTUAL*` (número), `REA-CMB-PRIORIDAD*` (combo) y `REA-TBL-LISTA-REABASTECIMIENTO*` (tabla). Los datos no cuadran con la tabla (4 filas, 2 críticas). |
| ART-MKP-REA-0001 | Caja «Filtrar por producto…» (`REA-CMB-PRODUCTO*`) | (c) | ESP-0045, ESP-0046 | Un campo de texto con el ID de un combo. `REA-CMB-PRODUCTO` es el combo del producto del ítem. |
| ART-MKP-REA-0001 | `REA-TBL-LISTA-REABASTECIMIENTO*` (tarjeta de resumen y tabla) | (c) | ESP-0045 a ESP-0048 | El mismo ID rotula dos componentes distintos. |
| ART-MKP-REA-0001 | Insignias de prioridad CRÍTICO / ALTO / MEDIO | (d) | ESP-0045, ESP-0046; TBL_LISTA_REABASTECIMIENTO.prioridad | El dominio de `prioridad` es 'Normal' o 'Urgente'. El mockup usa tres niveles con otros nombres. |
| ART-MKP-REA-0001 | Columnas «Stock Actual», «Stock Mínimo» y «Sugerido» | (d) | ESP-0046; TBL_LISTA_REABASTECIMIENTO | La tabla solo guarda producto, origen, prioridad y fecha de inclusión. Stock actual y mínimo se obtendrían de `TBL_LOTES` y `TBL_PRODUCTOS`, pero ESP-0046 no los carga. «Sugerido» no tiene ninguna fuente. |
| ART-MKP-REA-0001 | Campo «Cantidad Sugerida» (`REA-NUM-CANTIDAD-SUGERIDA*`) | (d) | ESP-0050; TBL_LISTA_REABASTECIMIENTO | No existe columna de cantidad. ESP-0050 habla de «la cantidad pendiente» de la lista para el CSV, y la tabla no la guarda. |
| ART-MKP-REA-0001 | Selector de prioridad del ítem (`REA-CMB-PRIORIDAD`) | (a) | ESP-0045, ESP-0047 | ESP-0045 y ESP-0047 piden elegir la prioridad. El único `REA-CMB-PRIORIDAD*` del mockup es una tarjeta de resumen. |
| ART-MKP-REA-0001 | `REA-LBL-FECHA-INCLUSION` | (a) | ESP-0045 | ESP-0045 muestra la fecha de inclusión. No aparece en el mockup. |
| ART-MKP-REA-0001 | `REA-CMB-FILTRO-PRIORIDAD`, `REA-CMB-FILTRO-ORIGEN` y `REA-BTN-LEER-ITEM` | (a) | ESP-0046 | ESP-0046 filtra por prioridad y origen con un botón de consulta. No hay ninguno de los tres en el mockup. |
| ART-MKP-REA-0001 | Columnas «origen» y «fecha de inclusión» de la tabla | (a) | ESP-0046 | ESP-0046 carga nombre, origen, prioridad y fecha de inclusión. La tabla no muestra origen ni fecha. |
| ART-MKP-REA-0002, 0003, 0004 | Pantalla de consulta, actualización y modal de eliminación | (e) | ESP-0046 (0002), ESP-0047 (0003), ESP-0048 (0004) | Las tres se citan y no existen. Sin pantalla quedan `REA-LBL-ORIGEN`, `REA-BTN-CONFIRMAR-ACTUALIZACION`, `REA-MDL-CONFIRMAR-ELIMINACION`, `REA-TXT-MSJ`, `REA-BTN-CONFIRMAR-SI` y `REA-BTN-CONFIRMAR-NO`. |

**Otras observaciones (fuera de las cinco categorías):**

- Los IDs coinciden con la especificación salvo el sufijo, excepto `REA-BTN-GENERAR-LISTA`, `REA-NUM-STOCK-ACTUAL` y `REA-NUM-CANTIDAD-SUGERIDA`, que no existen en el catálogo.

---

## Módulo 13 — Comparación de precios de proveedores (COT)

**Mockup revisado:** ART-MKP-COT-0001

### Componentes visibles

- Igual que REA, todos los IDs llevan asterisco final (`COT-…*`).
- Encabezado «Comparador de Cotizaciones»; botón «Exportar CSV» → `COT-BTN-EXPORTAR-CSV*`
- «Matriz Comparativa»: caja «Buscar medicamento en la comparativa…» → `COT-BTN-LEER-COTIZACION*`; matriz (Medicamento, Bayer S.A., Genfar, Portugal, Mejor Precio) con el precio más bajo resaltado en verde → `COT-TBL-COTIZACIONES-PROVEEDOR*`
- «Registrar Cotización»: «Medicamento» → `COT-CMB-PRODUCTO*`; «Proveedor» → `COT-CMB-PROVEEDOR*`; «Precio Ofrecido (S/)» → `COT-NUM-PRECIO-OFRECIDO*`; «Tiempo de Entrega (días)» → `COT-NUM-TIEMPO-ENTREGA*`; «Fecha de Cotización» → `COT-FEC-FECHA-COTIZACION*`
- Botones «Registrar» → `COT-BTN-CREAR-COTIZACION*`, «Actualizar» → `COT-BTN-ACTUALIZAR-COTIZACION*`, «Eliminar» → `COT-BTN-ELIMINAR-COTIZACION*`

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-COT-0001 | Sufijo `*` en todos los identificadores | (c) | Guía §1 | Ningún ID de la guía lleva asterisco. Los IDs que coinciden con la especificación difieren por ese sufijo. |
| ART-MKP-COT-0001 | `COT-NUM-PRECIO-OFRECIDO*` | (c) | ESP-0049, ESP-0051 | La especificación y la tabla usan `COT-NUM-PRECIO-COTIZADO` (`precio_cotizado`). El mockup lo llama «ofrecido». |
| ART-MKP-COT-0001 | `COT-TBL-COTIZACIONES-PROVEEDOR*` (matriz comparativa) | (c) | ESP-0050 | ESP-0050 nombra la matriz `COT-TBL-COMPARATIVA-PRECIOS`. El nombre del mockup es el de la tabla de la base de datos. |
| ART-MKP-COT-0001 | Caja «Buscar medicamento…» (`COT-BTN-LEER-COTIZACION*`) | (c) | ESP-0050 | Un campo de texto con el ID de un botón. ESP-0050 filtra con un combo (`COT-CMB-FILTRO-PRODUCTO`) y presiona `COT-BTN-LEER-COTIZACION`. |
| ART-MKP-COT-0001 | Botón «Exportar CSV» (`COT-BTN-EXPORTAR-CSV*`) | (c) | ESP-0050 (PEN-002) | ESP-0050 exporta por proveedor con `COT-BTN-EXPORTAR-PROVEEDOR` tras elegir proveedor. El mockup tiene un único botón de exportación sin selector de proveedor. |
| ART-MKP-COT-0001 | Campo «Tiempo de Entrega (días)» (`COT-NUM-TIEMPO-ENTREGA*`) | (d) | ESP-0049; TBL_COTIZACIONES_PROVEEDOR | La tabla no tiene columna de tiempo de entrega. Tampoco hay requisito que lo pida. Posible pedido nuevo. |
| ART-MKP-COT-0001 | Columna «Mejor Precio» (insignia con el nombre del proveedor) | (b) | ESP-0050 | ESP-0050 solo destaca el precio más bajo de cada fila. La columna adicional no está especificada. |
| ART-MKP-COT-0001 | `COT-TXA-PROMOCION` | (a) | ESP-0049, ESP-0051 | `promocion` es columna del diccionario y campo del registro y la actualización. El formulario no la tiene. |
| ART-MKP-COT-0001 | `COT-CMB-FILTRO-PRODUCTO` y `COT-CMB-PROVEEDOR-EXPORTAR` | (a) | ESP-0050 | ESP-0050 filtra por medicamento y elige el proveedor destinatario del CSV con dos combos. El mockup no tiene ninguno. |
| ART-MKP-COT-0001 | `COT-TBL-COTIZACIONES` (historial de cotizaciones) | (a) | ESP-0049, ESP-0051, ESP-0052 | ESP-0051 y ESP-0052 seleccionan la cotización en esta tabla. La matriz solo muestra la cotización vigente por proveedor, sin forma de elegir una cotización concreta para actualizar o eliminar. |
| ART-MKP-COT-0002, 0003, 0004 | Pantalla de comparativa, actualización y modal de eliminación | (e) | ESP-0050 (0002), ESP-0051 (0003), ESP-0052 (0004) | Las tres se citan y no existen. Sin pantalla quedan `COT-BTN-CONFIRMAR-ACTUALIZACION`, `COT-MDL-CONFIRMAR-ELIMINACION`, `COT-TXT-MSJ`, `COT-BTN-CONFIRMAR-SI` y `COT-BTN-CONFIRMAR-NO`. |

---

## Artefacto transversal — ART-MKP-CONFIRM-0001 (modal de confirmación genérico)

**Mockup revisado:** ART-MKP-CONFIRM-0001

### Componentes visibles

- Modal «Confirmación de Acción Crítica» con ícono «!» (sin ID) → `[MODULO]-MDL-CONFIRMAR-ELIMINACION`
- Mensaje «¿Está seguro de que desea proceder con la eliminación / anulación permanente del registro seleccionado?» → `[MODULO]-TXT-MSJ`
- Nota del sistema «Esta operación no se podrá deshacer y quedará auditada en el historial de eventos del sistema» (sin ID; ID propuesto `[MODULO]-LBL-NOTA-SISTEMA`)
- Botón «Cancelar» → `[MODULO]-BTN-CONFIRMAR-NO`; botón «Sí, Confirmar Acción» → `[MODULO]-BTN-CONFIRMAR-SI`

### Discrepancias

| Mockup | Componente | Categoría | Especificación o ilación afectada | Observación |
| --- | --- | :-: | --- | --- |
| ART-MKP-CONFIRM-0001 | Nombre del artefacto | (c) | Guía §2 y §4 | El segmento «CONFIRM» no es un prefijo de módulo de la tabla §2 (tres letras por módulo, sin módulo de confirmación). No sigue `ART-MKP-[MÓDULO]-[NÚMERO]`. |
| ART-MKP-CONFIRM-0001 | Identificadores con marcador `[MODULO]-…` | (c) | Guía §5; ESP-0004, 0008, 0012, 0016, 0020, 0024, 0028, 0032, 0036, 0040, 0044, 0048, 0052 | Son plantillas, no identificadores. Cada especificación usa el suyo (`VEN-MDL-CONFIRMAR-ELIMINACION`, `INV-MDL-…`, etc.). El patrón sí coincide con la guía §5. |
| ART-MKP-CONFIRM-0001 | Texto fijo del mensaje del modal | (c) | ESP-0004, 0008, 0012, 0016, 0020, 0024, 0028, 0032, 0036, 0040, 0044, 0048, 0052 | Las trece especificaciones definen un mensaje propio, con consecuencias distintas (por ejemplo ESP-0012: «El documento se conservará con estado Anulado»; ESP-0016: «La remesa volverá a estar disponible»). El mockup dice «eliminación / anulación permanente», lo que contradice las bajas lógicas (ESP-0012, DEC-009). |
| ART-MKP-CONFIRM-0001 | Nota «quedará auditada en el historial de eventos del sistema» | (b) | — | Ningún requisito, tabla ni especificación define un historial de eventos. Solo se habla de conservar registros anulados «para auditoría». Posible pedido nuevo. Sin ID visible. |
| ART-MKP-CONFIRM-0001 | Los 13 modales de baja que citan las especificaciones | (e) | ESP-0004, 0008, 0012, 0016, 0020, 0024, 0028, 0032, 0036, 0040, 0044, 0048, 0052 | El modal genérico existe como imagen única, pero ninguna especificación ni ilación lo cita. Cada una cita su modal por módulo (`ART-MKP-<MOD>-0004`, `ART-MKP-ALV-0003` y `ART-MKP-REP-0002`), que no existe. Esta fila repite artefactos ya contados en las filas de cada módulo. |

---

## Resumen

### Discrepancias por categoría

| Categoría | Cantidad |
| --- | :-: |
| (a) En la especificación y no en el mockup | 23 |
| (b) En el mockup y en ninguna especificación (posible pedido nuevo) | 26 |
| (c) Nombre distinto entre mockup, especificación o guía | 41 |
| (d) Campo sin columna en el diccionario | 16 |
| (e) ART-MKP citado que no existe como imagen | 14 |
| **Total de filas** | **120** |

La categoría (e) cuenta 13 filas por módulo, que agrupan los 37 `ART-MKP` citados que no existen como imagen, más una fila del modal genérico que repite 13 de ellos.

### Discrepancias por módulo

| Módulo | (a) | (b) | (c) | (d) | (e) | Total |
| --- | :-: | :-: | :-: | :-: | :-: | :-: |
| VEN | 2 | 4 | 7 | 3 | 1 | 17 |
| INV | 2 | 2 | 3 | 3 | 1 | 11 |
| DOC | 2 | 5 | 3 | 3 | 1 | 14 |
| ALV | 0 | 1 | 3 | 0 | 1 | 5 |
| PAG | 1 | 3 | 1 | 1 | 1 | 7 |
| REP | 2 | 4 | 3 | 0 | 1 | 10 |
| DEV | 2 | 1 | 1 | 1 | 1 | 6 |
| RES | 1 | 1 | 2 | 0 | 1 | 5 |
| USR | 1 | 0 | 2 | 1 | 1 | 5 |
| PRV | 1 | 1 | 2 | 0 | 1 | 5 |
| LOT | 2 | 0 | 3 | 0 | 1 | 6 |
| REA | 4 | 2 | 3 | 3 | 1 | 13 |
| COT | 3 | 1 | 5 | 1 | 1 | 11 |
| CONFIRM | 0 | 1 | 3 | 0 | 1 | 5 |
| **Total** | **23** | **26** | **41** | **16** | **14** | **120** |

### Patrones que se repiten en varios módulos

Son hechos observados, sin recomendación:

- **IDs repetidos:** en 12 de los 13 módulos (todos menos COT) el mismo ID rotula a la vez la lista de tarjetas y la tabla, o el criterio de búsqueda y el campo de captura (categoría (c)).
- **Pantalla única:** en los 13 módulos el mockup `0001` junta consulta, actualización y alta. Por eso las especificaciones citan los `0002`, `0003` y `0004` que no existen (categoría (e)).
- **Botón de confirmar actualización:** `<MOD>-BTN-CONFIRMAR-ACTUALIZACION` solo aparece en el mockup de ALV. La guía §5 lo define y las especificaciones lo presionan, salvo ESP-0007 (INV), que presiona dos veces `INV-BTN-ACTUALIZAR-PRODUCTO`.
- **Filtros añadidos:** varios mockups agregan filtros de búsqueda que ninguna especificación define (VEN, INV, DOC, ALV, PAG, REP, DEV, RES, PRV).
- **Funcionalidades sin respaldo:** ficha o visor 3D del producto (VEN, INV), indicaciones y sugerencias (VEN), previsualización y envío a OSE/SUNAT (DOC), historial de eventos (CONFIRM), tiempo de entrega (COT) y botón «Generar Lista» (REA). Son candidatas a pedidos nuevos del cliente y se confirmarían con la dueña (PEN-006, DEC-004).
- **Datos de ejemplo:** varias imágenes muestran valores que no cuadran con el modelo (códigos alfanuméricos donde el diccionario tiene INT, importes que no multiplican, estados con otros nombres).

