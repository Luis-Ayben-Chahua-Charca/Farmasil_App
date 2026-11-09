# Auditoría de coherencia entre mockups y catálogo — AGENTE: claude

## 1. Encabezado

| Campo | Valor |
| --- | --- |
| Fecha | 09/11/2026 |
| Modelo | Claude Opus 5.5 (`claude-opus-5-5`), en Claude Code |
| Nivel de razonamiento | El entorno no expone el nivel configurado; se usó el de la sesión, sin ajustes |
| Commit auditado | `ea42af9` (rama `docs/142-mockups-catalogo`) |
| ¿Se vieron las imágenes? | **Sí.** Se abrieron y revisaron los 15 PNG de `diseno/Mockups/`: los 13 `ART-MKP-<MOD>-0001`, `ART-MKP-CONFIRM-0001` y `DOC-MDL-PREVISUALIZAR-COMPROBANTE`. En Ventas se ampliaron el visor y el panel de selección para leer las etiquetas. |
| Fuentes HTML | Existen, pero en `diseno/Mockups/fuentes HTML/` (no en `diseno/Mockups/fuente/`). Son 15 archivos; dos no se llaman como su PNG: `ART-MKP-VEN-0002.html` y `ART-MKP-DOC-0002.html`. Los modales de baja de doce módulos solo están en el HTML, ocultos con `display:none`, y no aparecen en ningún PNG. |
| Informe 12 | Se leyó **después** de cerrar el inventario propio, solo para la sección 6. |
| Archivos modificados | Ninguno, salvo este informe (regla 2 del encargo). |

**Convenciones de este informe.**

- Las líneas citadas son las del commit `ea42af9`. Las ESP se citan por la línea de su campo (por ejemplo, `03-especificaciones.md:17` es el procedimiento de ESP-0001).
- Cuando el PNG y el HTML no coinciden, se aplica la regla del encargo: el PNG manda en lo que la dueña vio y el HTML en la grafía de los identificadores. Como ambos son la fuente del mismo mockup `0001`, estas diferencias se clasifican bajo **C1** (una sola pantalla y una sola fuente por módulo).
- Que un ID nombre algo distinto de lo que ESP pide (nombre cambiado, tipo de control distinto) se resuelve con **C4**: manda la ESP. **C2** se reserva para un ID que rotula dos o más controles, o un control con dos IDs.
- Las preguntas se citan como `P-NN` (para la dueña) y `L-NN` (para la líder técnica); están en la sección 7.

## 2. Resumen

- Se registran **157 hallazgos**: 33 en Ventas, 114 en los otros doce módulos y 10 transversales (incluido el modal genérico `ART-MKP-CONFIRM-0001`). Los más frecuentes son IDs reutilizados en dos o más controles (filtro de la barra superior y campo de la ficha, lista y tabla) y controles que la ESP pide y la pantalla única no muestra, sobre todo el botón de confirmar actualización.
- **Ventas** es el mockup más alejado de su especificación: de los 20 IDs que la guía define para VEN, el mockup usa 13; 7 de ellos rotulan más de un control, y otros 2 (`VEN-FEC-FECHA-VENTA` y `VEN-BTN-ELIMINAR-VENTA`) rotulan un control que no es el suyo. Además falta todo lo que piden ESP-0002 a ESP-0004 (tabla de ventas, consulta, actualización y eliminación), y hay tres funciones sin respaldo en ningún requisito: visor 3D, ficha de producto e indicaciones/sugerencias.
- El panel de catálogo de Ventas puede resolverse sin una dependencia nueva hacia Inventario, con `ILotes.medicamentosVendibles()` (VL-04). En cambio, el indicador «Pendientes de cotizar» de Reabastecimiento necesitaría la arista REA → COT, que crearía un ciclo con COT → REA.
- Hay cuatro huecos de modelo que el mockup hace visibles y que no se pueden cerrar sin la dueña: la ubicación del producto, la cantidad sugerida de reabastecimiento (que ILA-0050 ya da por existente), los tres niveles de prioridad frente a 'Normal'/'Urgente', y los datos del cliente y el envío a SUNAT en la vista previa del comprobante.
- Por criterio: C1 resuelve 20, C2 resuelve 33, C3 resuelve 23, C4 resuelve 64, y **17 requieren decisión** (sección 5).

## 3. Ventas (`ART-MKP-VEN-0001`)

### 3.1 Paneles, operación que les corresponde e interfaz que usarían

| Panel | Lo que muestra el mockup (texto visible → ID del mockup) | Operación del catálogo | Interfaz o dependencia |
| --- | --- | --- | --- |
| Barra superior | Combo «Todo» → `VEN-CMB-PRODUCTO-VENTA`; buscador «Buscar» → `VEN-CMB-PRODUCTO-VENTA`; botón «Observar información del producto (texto)» → `VEN-BTN-LEER-VENTA`; botón «Ver producto (3D)» → `VEN-BTN-LEER-VENTA`; recuadro «Carrito de compras» (en el PNG parece un botón; en el HTML es un `span`) → `VEN-TBL-DETALLE-VENTA` | Filtros de la selección de producto de ESP-0001. Pese al ID, **no** es la consulta de ventas de ESP-0002. | `ILotes` (VEN → LOT) |
| Izquierda: «Catálogo» | Título → `VEN-CMB-PRODUCTO-VENTA`; 9 tarjetas sin ID propio (imagen, «Paracetamol», «Paracetamol 500mg - 20 tabs», «S/ 12.50»); botón «Añadir» en cada tarjeta → `VEN-BTN-AGREGAR-PRODUCTO`; recuadro «Indicaciones / Sugerencias» → `VEN-LBL-MONTO-TOTAL` | Paso «seleccionar medicamento» de ESP-0001 (`03-especificaciones.md:17`) | Se parece a la consulta de Inventario (ESP-0006), pero VEN → INV no existe (`Insumo_Matriz…v2.md:58`). Puede resolverse con `ILotes.medicamentosVendibles()` (`VL-04_MVC_Ventas.puml:78`, nota `:145-150`). Ver 030. |
| Centro arriba: visor | Envase en 3D; controles sin ID: girar, mover, ojo, papelera (izquierda) y +, −, papelera (derecha); etiqueta «VEN-CMB-PRODUCTO-VENTA · VEN-TBL-DETALLE-VENTA» | Ninguna | — |
| Centro abajo: «Opciones de selección» | Título → `VEN-CMB-LOTE-VENTA`; «Lote» (campo de texto en el PNG, `select` en el HTML) → `VEN-CMB-LOTE-VENTA`; «Vencimiento» (selector de fecha) → `VEN-FEC-FECHA-VENTA`; «Ubicación» (combo) → `VEN-CMB-LOTE-VENTA`; «Cantidad» → `VEN-NUM-CANTIDAD-VENTA`; «Agregar producto» → `VEN-BTN-AGREGAR-PRODUCTO`; leyenda «ESP-0001 · Precondición FEFO sobre TBL_LOTES» | Sugerencia FEFO, cantidad y «agregar línea» de ESP-0001 | `ILotes.sugerirFEFO()`; `IRestricciones` al agregar la línea (VEN → RES) |
| Derecha: «Carrito de compras» | Título y tabla → `VEN-TBL-DETALLE-VENTA` (columnas Código #, Descripción, Precio, QTY con − y +, Total; papelera por fila solo en el PNG); etiqueta «VEN-TBL-DETALLE-VENTA · VEN-NUM-CANTIDAD-VENTA · VEN-BTN-QUITAR-PRODUCTO»; «Total Items: 3 · Total: S/ 51.00» → `VEN-LBL-MONTO-TOTAL`; «Cancelar» → `VEN-BTN-ELIMINAR-VENTA`; «Método de pago» (dibujado como botón) → `VEN-CMB-METODO-PAGO`; «Registrar Venta» → `VEN-BTN-CREAR-VENTA`; «Fecha: 05/09/2026 14:32» → `VEN-LBL-FECHA-VENTA` | Detalle, total, método de pago y registro de ESP-0001 | `IPagos` (VEN → PAG) |
| No representado | `VEN-TBL-REGISTRO-VENTAS`; filtros y botón de ESP-0002; botones de ESP-0003; botón y modal de ESP-0004 (tampoco hay un modal oculto en el HTML) | ESP-0002, ESP-0003, ESP-0004 | `IVentas` (provista a DOC y REP) |

### 3.2 Diferencias con ESP-0001 a ESP-0004 e impacto en el catálogo

| # | Diferencia (mockup frente a ESP-0001 a ESP-0004) | EDU | ILA | ESP | Diccionario | Criterio | Hallazgo |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | El medicamento se elige en un catálogo de tarjetas, no en el combo `VEN-CMB-PRODUCTO-VENTA` | EDU-0001 no cambia: ya pide «productos vendidos» (`01-educciones.md:41`) | ILA-0001 paso 3 (`02-ilaciones.md:17`): «selecciona el medicamento en el catálogo `VEN-PNL-CATALOGO`», que lista solo los medicamentos con remesa vendible | ESP-0001 (`:17`): `producto = VEN-CRD-PRODUCTO seleccionada EN VEN-PNL-CATALOGO`, conservando `ES_VENDIBLE`. ESP-0002 conserva `VEN-CMB-PRODUCTO-VENTA` solo como filtro de la consulta. | No cambia (`nombre`, `precio_venta`: `05-diccionario-datos.md:38,40`) | C2 | 001, 030 |
| 2 | Buscador por nombre y filtro «Todo» (¿categoría?) | No cambia | ILA-0001 paso 3: filtros por nombre y, si se confirma, por categoría | ESP-0001: `VEN-SRH-PRODUCTO` y `VEN-CMB-FILTRO-CATEGORIA` antes de la selección. Son filtros de la **selección** (ESP-0001), no de la consulta de ventas (ESP-0002). | La categoría no tiene columna: `accion_terapeutica` es texto libre y su contenido está en PEN-010 (`:39`) | C3 (buscador); requiere decisión (categoría, P-07) | 021 |
| 3 | Botón «Observar información del producto (texto)» | Ninguna lo pide | — | — | Habría datos (`TBL_PRODUCTOS`) vía `ILotes`, sin arista nueva | C3, salvo que la dueña lo confirme (P-02) | 019 |
| 4 | Visor 3D y botón «Ver producto (3D)» | Ninguna lo pide | — | — | No hay columna de imagen ni de modelo | C3, salvo P-01 | 020 |
| 5 | Botón «Añadir» en cada tarjeta, además de «Agregar producto» | — | Si agrega directamente: un segundo camino en el paso 6 | ESP-0001: definir si «Añadir» solo selecciona el producto o agrega una línea con la remesa FEFO y cantidad 1 | — | Requiere decisión (P-05) | 004 |
| 6 | Se muestra el vencimiento de la remesa | No cambia (EDU-0015, `:231`) | ILA-0001 paso 4: «el sistema muestra el vencimiento de la remesa propuesta» | ESP-0001: `MOSTRAR lote.fecha_vencimiento EN VEN-LBL-FECHA-VENCIMIENTO` (solo lectura, igual que `ALV-LBL-FECHA-VENCIMIENTO`) | `TBL_LOTES.fecha_vencimiento` (`:60`), sin cambio | C2 | 007 |
| 7 | «Lote» dibujado como campo de texto en el PNG | — | — | No cambia: es un combo que solo lista remesas vendibles | — | C4 | 006 |
| 8 | «Ubicación» de la remesa o del producto | EDU-0015 (`:231`) o EDU-0002 (`:60`): añadir la ubicación a los datos que se gestionan, **solo si la dueña la pide** | ILA-0041 y ILA-0043: capturarla; ILA-0001: mostrarla | ESP-0041 y ESP-0043: nuevo campo (por ejemplo `LOT-TXT-UBICACION`); ESP-0001: `VEN-LBL-UBICACION` | Columna nueva (por ejemplo en `TBL_LOTES`), hoy inexistente (`:55-64`) | Requiere decisión (P-03) | 008 |
| 9 | Recuadro «Indicaciones / Sugerencias» | EDU-0012 (`:174`) solo respalda las advertencias de restricción; nada respalda recomendaciones de uso ni sugerencias | ILA-0001 paso 8 no cambia | ESP-0001: `MOSTRAR … EN VEN-LBL-ADVERTENCIAS` para los mensajes de restricción y de vendibilidad que hoy no tienen lugar | `motivo_advertencia` (`:219`), sin cambio | C3 (recomendaciones y sugerencias); C4 (área de advertencias) | 010, 011 |
| 10 | La cantidad de una línea se edita en la tabla (− y +) | Ya respaldada («Cantidades», EDU-0001) | ILA-0001: paso nuevo «modifica la cantidad de una línea», con la validación del paso 7 | ESP-0001: `VEN-NUM-CANTIDAD-LINEA`, validando `cantidadTotal <= stock_actual` y `ES_VENDIBLE` | — | C2 | 016 |
| 11 | Columnas del detalle: Código #, Descripción, Precio, QTY, Total | — | ILA-0002 paso 8 (`:36`) ya pide el lote de cada línea | ESP-0001: `CARGAR VEN-TBL-DETALLE-VENTA CON (nombre, numero_lote, cantidad, precio_unitario, subtotal)` | `numero_lote` (`:59`), `cantidad` y `precio_unitario` (`:99-100`); «Código #» no tiene columna | C4; requiere decisión para «Código #» (P-06) | 014, 015 |
| 12 | «Total Items» (unidades) | — | — | Opcional: `VEN-LBL-TOTAL-UNIDADES`, valor calculado | Calculado, sin columna | C3 | 018 |
| 13 | Botón «Cancelar» (descartar la venta en curso) | — | ILA-0001: paso «descarta la venta en curso sin registrar nada» | ESP-0001: `SI PRESIONA VEN-BTN-CANCELAR-VENTA ENTONCES detalle = LISTA_VACIA` (no escribe en la base) | — | C2 | 012 |
| 14 | «Método de pago» dibujado como botón | — | — | No cambia: combo de métodos activos | — | C4 | 013 |
| 15 | No hay tabla de ventas, consulta, actualización ni eliminación | — | ILA-0002 a ILA-0004: citar `ART-MKP-VEN-0001` | ESP-0002 a ESP-0004 no cambian, salvo citar `ART-MKP-VEN-0001` en lugar de 0002 a 0004 | — | C4 y C1 | 023 a 027 |
| 16 | EDU-0004 pide gestionar alertas «en el módulo de ventas» y el mockup no lo muestra | EDU-0004 (`:98`): precisar el alcance | ILA-0001: dejar escrita la interpretación (la venta muestra el bloqueo; el CRUD sigue en ALV) | Sin cambio si se adopta esa interpretación | — | Requiere decisión (L-02) | 031 |

### 3.3 Rótulo «Carrito de compras» frente a `VEN-TBL-DETALLE-VENTA`

- El catálogo llama a ese componente «detalle»: la guía lo define como «Líneas de productos de la venta en curso» (`06-guia-nomenclatura-mockups.md:248`), las ilaciones hablan de «línea de detalle» (`02-ilaciones.md:17`, paso 14) y la tabla es `TBL_DETALLE_VENTAS` (`05-diccionario-datos.md:90-92`). «Carrito de compras» es un término de comercio electrónico que no aparece en ningún artefacto.
- El rótulo aparece dos veces: en la barra superior, como recuadro con aspecto de botón, y como título del panel derecho. Las dos llevan `VEN-TBL-DETALLE-VENTA`, igual que la tabla y el visor (hallazgo 003).
- **Propuesta:** el rótulo visible pasa a ser **«Detalle de la venta»** y el ID `VEN-TBL-DETALLE-VENTA` se reserva solo para la tabla. El recuadro de la barra superior se retira o recibe su propio ID si es un botón. Las columnas pasan a «Medicamento», «Lote», «Cantidad», «Precio unitario» y «Subtotal» («QTY» está en inglés, y «Total» se confunde con `VEN-LBL-MONTO-TOTAL`). Como cambia un texto que la dueña aprobó, se le consulta (P-08).

### 3.4 Qué alimenta al issue «VEN: documentar el cambio del módulo de ventas pedido por la dueña»

1. **ILA-0001 y ESP-0001.** Selección por catálogo, con buscador y filtro; vencimiento visible; edición de cantidad por línea; cancelar la venta en curso; área de advertencias; columnas del detalle con lote. Diferencias 1, 2, 6 y 9 a 13.
2. **ILA-0002 a ILA-0004 y ESP-0002 a ESP-0004.** Solo cambian el mockup citado (C1). El contenido queda igual y el mockup no se rehace (C4).
3. **Guía de nomenclatura.** IDs nuevos para VEN: `VEN-PNL-CATALOGO`, `VEN-CRD-PRODUCTO`, `VEN-SRH-PRODUCTO`, `VEN-CMB-FILTRO-CATEGORIA`, `VEN-PNL-OPCIONES-SELECCION`, `VEN-LBL-FECHA-VENCIMIENTO`, `VEN-NUM-CANTIDAD-LINEA`, `VEN-BTN-CANCELAR-VENTA`, `VEN-LBL-ADVERTENCIAS` y, según lo que responda la dueña, `VEN-BTN-SELECCIONAR-PRODUCTO`, `VEN-LBL-UBICACION` y `VEN-LBL-TOTAL-UNIDADES`. Son propuestas que siguen el formato de la guía y no se usan todavía en ningún artefacto.
4. **Diccionario.** Sin cambios, salvo que la dueña confirme la ubicación (P-03) o un código de producto (P-06).
5. **Arquitectura.** Sin aristas nuevas. Hay que documentar `medicamentosVendibles()` entre las operaciones que expone `ILotes` en `Insumo_Matriz_Interfaces_ArquitecturaGenerica_v2.md:91`; hoy solo figura en VL-04.
6. **Fuera de alcance (C3), salvo confirmación:** visor 3D, ficha de producto, recomendaciones y sugerencias, y total de unidades.
7. **Decisiones previas:** P-01 a P-08, L-01 y L-02.

## 4. Hallazgos por módulo

Cada módulo empieza con sus paneles y la operación que corresponde a cada uno. Tipos: (a) en la ESP y no en el mockup; (b) en el mockup y en ninguna ESP; (c) nombre distinto; (d) campo sin columna; (e) mockup citado que no existe; (f) salto EDU → ILA → ESP; (g) permiso o rol; (h) dependencia que la arquitectura no tiene.

### 4.1 VEN — `ART-MKP-VEN-0001.png` / `fuentes HTML/ART-MKP-VEN-0002.html`

Paneles y operaciones: ver la sección 3.1.

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-001 | `VEN-CMB-PRODUCTO-VENTA` rotula cuatro cosas: combo «Todo», buscador «Buscar», título «Catálogo» y visor (junto con `VEN-TBL-DETALLE-VENTA`) | ESP-0001 `03-especificaciones.md:17` (combo de selección); ESP-0002 `:35` (filtro); guía `06-guia…:264` | c | C2 | `VEN-CMB-FILTRO-CATEGORIA` (combo «Todo»), `VEN-SRH-PRODUCTO` (buscador), `VEN-PNL-CATALOGO` con `VEN-CRD-PRODUCTO` (catálogo; CRD es «Tarjeta», guía `:82`), `VEN-PNL-VISOR` si se conserva. `VEN-CMB-PRODUCTO-VENTA` queda solo como filtro de ESP-0002. | Alta |
| AUD-MKP-claude-002 | `VEN-BTN-LEER-VENTA` rotula «Observar información del producto (texto)» y «Ver producto (3D)» | ESP-0002 `:35`: es el botón que consulta ventas | c | C2 | Si se conservan (P-01, P-02): `VEN-BTN-VER-FICHA-PRODUCTO` y `VEN-BTN-VER-PRODUCTO-3D`. `VEN-BTN-LEER-VENTA` vuelve a la consulta de ventas. | Alta |
| AUD-MKP-claude-003 | `VEN-TBL-DETALLE-VENTA` rotula el recuadro «Carrito de compras» de la barra superior, el título del panel derecho, la tabla y el visor | Guía `:248` (tabla de líneas); ESP-0001 `:17` | c | C2 | Reservarlo para la tabla. Retirar el recuadro superior o darle su propio ID si es un botón. | Alta |
| AUD-MKP-claude-004 | 9 botones «Añadir» en las tarjetas y el botón «Agregar producto» llevan `VEN-BTN-AGREGAR-PRODUCTO` | ESP-0001 `:17`: un único camino (producto → remesa FEFO → cantidad → agregar) | c | Requiere decisión | Si «Añadir» solo selecciona el producto: `VEN-BTN-SELECCIONAR-PRODUCTO`. Si agrega directo (remesa FEFO y cantidad 1): ID propio y paso nuevo en ESP-0001 con las mismas validaciones. P-05. | Alta |
| AUD-MKP-claude-005 | `VEN-CMB-LOTE-VENTA` rotula el título «Opciones de selección», el campo «Lote» y el combo «Ubicación» | ESP-0001 `:17`; guía `:236` | c | C2 | `VEN-PNL-OPCIONES-SELECCION` para el panel; `VEN-CMB-LOTE-VENTA` solo para el lote; ID propio para la ubicación si se conserva (008). | Alta |
| AUD-MKP-claude-006 | En el PNG, «Lote» es un campo de texto («LOTE-2026-001», sin flecha); en el HTML es un `select` | ESP-0001 `:17` (`MOSTRAR lote EN VEN-CMB-LOTE-VENTA, QUE LISTA SOLO LAS REMESAS CON ES_VENDIBLE`); guía `:109` (una clave foránea se captura con CMB); DEC-033 | c | C4 | Se implementa como combo que solo lista remesas vendibles, con la sugerencia FEFO. | Alta |
| AUD-MKP-claude-007 | «Vencimiento» es un selector de fecha editable rotulado `VEN-FEC-FECHA-VENTA` | `VEN-FEC-FECHA-VENTA` es el filtro de fecha de ESP-0002 `:35` y de la guía `:202`; `fecha_vencimiento` pertenece a la remesa (`05-diccionario-datos.md:60`) | c | C2 | `VEN-LBL-FECHA-VENCIMIENTO`, de solo lectura y derivado de la remesa elegida (como `ALV-LBL-FECHA-VENCIMIENTO`, guía `:238`); añadirlo a ILA-0001 paso 4 y a ESP-0001. | Alta |
| AUD-MKP-claude-008 | Combo «Ubicación» | No hay columna de ubicación en `TBL_LOTES` (`:55-64`) ni en `TBL_PRODUCTOS` (`:35-43`); ninguna EDU la menciona (EDU-0015 `01-educciones.md:231`) | d | Requiere decisión | P-03. Si la dueña la pide: EDU-0015 (o EDU-0002), ILA y ESP-0041/0043, columna nueva y `LOT-TXT-UBICACION`; en Ventas, `VEN-LBL-UBICACION` de solo lectura. Si no la pide: C3, fuera de alcance. | Alta |
| AUD-MKP-claude-009 | `VEN-LBL-MONTO-TOTAL` rotula el recuadro «Indicaciones / Sugerencias», además del total | Guía `:251` (suma del detalle) | c | C2 | Reservarlo para el total. El recuadro recibe otro ID (ver 011). | Alta |
| AUD-MKP-claude-010 | Textos «Análisis y recomendaciones de uso para los ítems» y «Recomendaciones en función los ítems seleccionados» | Ninguna EDU, ILA ni ESP; no hay columna de indicaciones de uso | b | C3 | Fuera de alcance salvo que la dueña lo haya pedido (P-04). | Media |
| AUD-MKP-claude-011 | Ningún control recibe los mensajes que ESP-0001 muestra al agregar una línea: restricciones 'Informativa' y 'Bloqueante', remesa vencida, medicamento descontinuado | ESP-0001 `:17` (`MOSTRAR restriccion.motivo_advertencia`, `MOSTRAR "La remesa está vencida…"`); EDU-0012 `01-educciones.md:174`; ILA-0001 paso 8 | a | C4 | Usar el recuadro del panel izquierdo como área de advertencias: `VEN-LBL-ADVERTENCIAS`, que ESP-0001 nombra en sus `MOSTRAR`. | Alta |
| AUD-MKP-claude-012 | Botón «Cancelar» rotulado `VEN-BTN-ELIMINAR-VENTA` | ESP-0004 `:71`: elimina una venta registrada; la matriz la reserva al Administrador (`07-matriz-permisos.md:53`). ESP-0001 no prevé cancelar la venta en curso. | c, g | C2 | `VEN-BTN-CANCELAR-VENTA`, disponible para ambos roles, y un paso en ILA-0001 y ESP-0001 que vacía el detalle sin escribir en la base. `VEN-BTN-ELIMINAR-VENTA` queda para ESP-0004. | Alta |
| AUD-MKP-claude-013 | «Método de pago» dibujado como botón, con `VEN-CMB-METODO-PAGO` | ESP-0001 `:17`; guía `:101` (dominio cerrado o referencia → CMB) | c | C4 | Se implementa como combo de métodos activos (`IPagos`). | Media |
| AUD-MKP-claude-014 | La tabla del detalle no tiene columna de lote: las dos filas «Paracetamol 500mg» no se distinguen | DEC-036 (`11-registro-pendientes.md:116`): una línea por remesa; ILA-0002 paso 8 (`02-ilaciones.md:36`): el detalle muestra el número de lote; guía `:248` | a | C4 | Columnas «Medicamento», «Lote», «Cantidad», «Precio unitario» y «Subtotal», y declararlas en ESP-0001 al cargar `VEN-TBL-DETALLE-VENTA`. | Alta |
| AUD-MKP-claude-015 | Columna «Código #» (PNG: «00000000 00000203» y «…201»; HTML: «00000020») | `id_producto` e `id_lote` son `INT IDENTITY` (`05-diccionario-datos.md:37,57`); no hay columna de código de barras ni de código comercial | d | Requiere decisión | P-06. Si no hay un código propio, mostrar el número de lote. | Media |
| AUD-MKP-claude-016 | Controles − y + de cantidad en cada fila, con el mismo ID que el campo «Cantidad» (`VEN-NUM-CANTIDAD-VENTA`) | ESP-0001 `:17`: en el alta la cantidad solo se acumula al volver a agregar; editar la cantidad de una línea solo existe en ESP-0003 `:53` | c | C2 | `VEN-NUM-CANTIDAD-LINEA` y un paso en ESP-0001 con la misma validación de stock acumulado y `ES_VENDIBLE` (DEC-033, DEC-036). | Alta |
| AUD-MKP-claude-017 | Subtotales incoherentes: 2 × S/ 12.50 = «S/ 39.00»; 1 × S/ 12.50 = «S/ 12.00»; total «S/ 51.00» (PNG y HTML) | ESP-0001 `:17` (`montoTotal = SUMA(cantidad * precioUnitario)`) | c | C4 | Datos de ejemplo erróneos; la implementación sigue la ESP. No se rehace el mockup. | Alta |
| AUD-MKP-claude-018 | «Total Items: 3» (suma de unidades) | Sin respaldo en ESP-0001 | b | C3 | Fuera de alcance, o se acepta como valor calculado sin regla nueva (`VEN-LBL-TOTAL-UNIDADES`) si la dueña lo quiere. | Baja |
| AUD-MKP-claude-019 | Botón «Observar información del producto (texto)» | Ninguna EDU, ILA ni ESP de Ventas; EDU-0001 `01-educciones.md:41` | b | C3 | Fuera de alcance salvo P-02. Si entra, los datos llegan por `ILotes` (ver 030), no por Inventario. | Media |
| AUD-MKP-claude-020 | Visor 3D con 7 controles sin ID y botón «Ver producto (3D)» | Ninguna EDU, ILA ni ESP; no hay columna de imagen o modelo; RNF-0008 fija WPF | b, d | C3 | Fuera de alcance salvo P-01. | Alta |
| AUD-MKP-claude-021 | Buscador «Buscar» y combo «Todo» en la barra superior | ESP-0001 `:17` no tiene filtros; `accion_terapeutica` es texto libre y PEN-010 decide su contenido (`05-diccionario-datos.md:39`; `11-registro-pendientes.md:53`) | b | C3 | Se aceptan como filtros extra, pero van a la **selección** de ESP-0001 e ILA-0001 paso 3, no a ESP-0002. El filtro por categoría espera a PEN-010 (P-07). | Media |
| AUD-MKP-claude-022 | Tarjeta con encabezado «Paracetamol», descripción «Paracetamol 500mg - 20 tabs» e imagen | `nombre` es una sola columna (`:38`); presentación y tipo de producto son el vacío 4 (`:415`) y PEN-010 | d | Requiere decisión | Mostrar solo `nombre` hasta que PEN-010 decida si hay presentación (P-07). La imagen, ver 154. | Media |
| AUD-MKP-claude-023 | `VEN-TBL-REGISTRO-VENTAS` no aparece | ESP-0001 `:17` (paso final); ESP-0002 `:35`; ESP-0003 `:53`; ESP-0004 `:70-71`; guía `:264` | a | C4 | La implementación añade la tabla de ventas registradas en la pantalla 0001, sin rehacer el mockup. | Alta |
| AUD-MKP-claude-024 | La consulta de ventas no aparece: los IDs `VEN-FEC-FECHA-VENTA`, `VEN-CMB-PRODUCTO-VENTA` y `VEN-BTN-LEER-VENTA` existen, pero rotulan otros controles | ESP-0002 `:35`; ILA-0002 `02-ilaciones.md:36` | a | C4 | Filtros de fecha y producto y botón de consulta sobre la tabla de 023. | Alta |
| AUD-MKP-claude-025 | La actualización no aparece: `VEN-BTN-ACTUALIZAR-VENTA` y `VEN-BTN-CONFIRMAR-ACTUALIZACION` | ESP-0003 `:53`; ILA-0003 `:55` | a | C4 | Se añaden en la implementación, visibles pero deshabilitados para la Técnica (033). | Alta |
| AUD-MKP-claude-026 | La eliminación no aparece: botón, modal `VEN-MDL-CONFIRMAR-ELIMINACION`, `VEN-TXT-MSJ`, `VEN-BTN-CONFIRMAR-SI` y `VEN-BTN-CONFIRMAR-NO`. A diferencia de los demás módulos, el HTML tampoco tiene un modal oculto. | ESP-0004 `:71`; ILA-0004 `:74` | a | C4 | Se añaden en la implementación con el texto de ESP-0004. | Alta |
| AUD-MKP-claude-027 | Mockups citados que no existen: `ART-MKP-VEN-0002`, `0003` y `0004` | ESP-0002 `:35,37`; ESP-0003 `:53,55`; ESP-0004 `:71,73`; ILA-0002 a ILA-0004 (`02-ilaciones.md:36,55,74`) | e | C1 | Citar `ART-MKP-VEN-0001` en el procedimiento (`CARGAR …`) y en los artefactos asociados. | Alta |
| AUD-MKP-claude-028 | La fuente HTML se llama `ART-MKP-VEN-0002.html`, con `<title>` «ART-MKP-VEN-0002 - Punto de Venta (Ventas)»; el PNG es `ART-MKP-VEN-0001.png` | Guía `:116` y C1 | c | C1 | Renombrar la fuente a `ART-MKP-VEN-0001.html` y corregir el `<title>`. | Alta |
| AUD-MKP-claude-029 | Diferencias entre PNG y HTML: (1) leyenda «ESP-0001 · Precondición FEFO sobre TBL_LOTES» (PNG) frente a «FEFO-0001 · Preselección FEFO sobre TBL_LOTES» (HTML); (2) papelera por fila solo en el PNG; (3) controles del visor: 4 a la izquierda en el PNG (girar, mover, ojo, papelera) y 3 en el HTML (girar, mover, lupa); (4) «Código #» distinto; (5) lote: texto o `select` (006) | FEFO está en el **procedimiento** de ESP-0001 (`:17`, `SUGERIR_FEFO`), no en la precondición (`:16`); `FEFO-0001` no es un código del catálogo | c | C1 | Alinear el HTML con el PNG. La leyenda pasa a «ESP-0001 · Sugerencia FEFO (procedimiento)». | Alta |
| AUD-MKP-claude-030 | El panel «Catálogo» (nombre, presentación y precio de cada medicamento) se parece a la consulta de Inventario | ESP-0006 `:111`; matriz VEN → INV = 0 (`Insumo_Matriz…v2.md:58`; `VL-01_Paquetes.puml:91-93`); `ILotes.medicamentosVendibles()` (`VL-04_MVC_Ventas.puml:78`, nota `:145-150`); `ILotes` en el insumo `:90-97` no la lista | h | Requiere decisión | Resolverlo con `ILotes.medicamentosVendibles()`, que trae nombre y precio por LOT → INV, si el catálogo muestra **solo medicamentos vendibles** (DEC-033). El filtro por categoría y la ficha (019) requieren ampliar ese DTO, no una arista nueva. Documentar la operación en el insumo `:91`. No se propone VEN → INV. L-01. | Alta |
| AUD-MKP-claude-031 | Ningún control permite gestionar alertas de vencimiento desde Ventas | EDU-0004 `01-educciones.md:98` («crear, actualizar y eliminar alertas, tanto en un panel de alertas como en el módulo de ventas»); ninguna ILA del módulo 1 lo interpreta; VEN → ALV no existe (`Insumo_Matriz…v2.md:58`) | f, h | Requiere decisión | Registrar la interpretación de que «en el módulo de ventas» significa mostrar el bloqueo al vender (ya está en ESP-0001: «La remesa tiene una alerta activa…») y que el CRUD queda en ALV. Así no hace falta la arista VEN → ALV. L-02. | Alta |
| AUD-MKP-claude-032 | Rótulo «Carrito de compras» | Guía `:248`; `TBL_DETALLE_VENTAS` (`05-diccionario-datos.md:90-92`); ILA-0001 `:17` | c | Requiere decisión | «Detalle de la venta» (sección 3.3). Cambia un texto aprobado: P-08. | Alta |
| AUD-MKP-claude-033 | Ningún control se identifica como reservado al Administrador | Matriz `07-matriz-permisos.md:50-53` (actualizar y eliminar ventas: solo A); RNF-0006 condición 4 (`08-requisitos-no-funcionales.md:84`) | g | C4 | Al añadir 025 y 026, mostrarlos deshabilitados para la Técnica, con el motivo. «Cancelar» no debe heredar el permiso de eliminar (012). | Media |

### 4.2 INV — `ART-MKP-INV-0001`

Paneles: barra superior (proveedor, búsqueda y «Buscar») → consulta ESP-0006. Izquierda «Catálogo General» (tarjetas con «Seleccionar») → resultado de ESP-0006 y selección para ESP-0007/0008. Centro «Ficha Técnica del Producto» (visor y formulario) → alta y actualización, ESP-0005 y ESP-0007. Derecha «Gestión CRUD» (tabla y botones Crear, Guardar y Eliminar) → ESP-0005, ESP-0007 y ESP-0008. Modal de baja, solo en el HTML → ESP-0008. Dependencia usada: `IProveedores` (INV → PRV).

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-034 | `INV-CMB-PROVEEDOR` rotula el filtro «-- Criterio / Proveedor --», el filtro «Filtrar por Categoría / Tipo» y el combo «Proveedor» de la ficha | ESP-0005 `:93`; ESP-0007 `:129`; guía `:253` | c | C2 | `INV-CMB-FILTRO-PROVEEDOR`, `INV-CMB-FILTRO-CATEGORIA` e `INV-CMB-PROVEEDOR` solo en la ficha. | Alta |
| AUD-MKP-claude-035 | Un solo buscador rotulado «INV-TXT-NOMBRE-PRODUCTO / INV-TXT-ID-PRODUCTO» (dos IDs para un control) | ESP-0006 `:111` (dos campos); guía `:197` | c | C2 | `INV-SRH-PRODUCTO` (SRH, guía `:80`) y ESP-0006 con un criterio único por identificador o nombre. | Alta |
| AUD-MKP-claude-036 | Los textos de cada tarjeta llevan `INV-TXT-NOMBRE-PRODUCTO`, `INV-TXT-ACCION-TERAPEUTICA` e `INV-NUM-PRECIO-PRODUCTO`, y el botón «Seleccionar» lleva `INV-BTN-LEER-PRODUCTO` | ESP-0005 `:93` (campos de captura); ESP-0006 `:111` (botón de búsqueda) | c | C2 | Tarjeta `INV-CRD-PRODUCTO`, sin reutilizar IDs de captura; botón `INV-BTN-SELECCIONAR-PRODUCTO` o selección por clic. | Alta |
| AUD-MKP-claude-037 | `INV-TBL-PRODUCTOS` rotula el «Catálogo General» (tarjetas) y la tabla «Gestión CRUD» | ESP-0006 `:111`; guía `:265` | c | C2 | Una sola lista de resultados; si se conservan ambas, ID propio para la segunda. | Alta |
| AUD-MKP-claude-038 | Panel «[ Visor 3D / Detalle de Medicamento ]» con `INV-PNL-PRODUCTO` | No está en la guía (`:265`), en ninguna ESP del módulo ni en el diccionario | b | C3 | Fuera de alcance salvo P-01. | Alta |
| AUD-MKP-claude-039 | Filtro por proveedor | ESP-0006 `:111` no lo tiene; `id_proveedor` existe (`05-diccionario-datos.md:42`) | b | C3 | Filtro extra aceptado: incorporarlo a ESP-0006 e ILA-0006. | Alta |
| AUD-MKP-claude-040 | Filtro «Filtrar por Categoría / Tipo» | No hay columna de categoría ni de tipo: `accion_terapeutica` es texto libre (`:39`); tipo genérico o comercial es el vacío 4 (`:415`); PEN-010 (`11-registro-pendientes.md:53`) | d | Requiere decisión | Esperar a PEN-010 (P-07). Si la categoría es `accion_terapeutica`, el filtro va a ESP-0006 sobre esa columna. | Alta |
| AUD-MKP-claude-041 | No hay campo de stock mínimo | ESP-0005 `:93` e ESP-0007 `:129` (`INV-NUM-STOCK-MINIMO`); ILA-0005 `02-ilaciones.md:97`; `stock_minimo` (`:43`) | a | C4 | Añadirlo a la ficha en la implementación. | Alta |
| AUD-MKP-claude-042 | Ni la tabla (ID, Producto, Precio) ni las tarjetas muestran estado ni stock físico | ESP-0006 `:111` (`estado_producto, STOCK_FISICO(id_producto)`); DEC-040 | a | C4 | Añadir estado y stock físico al resultado de la consulta. | Alta |
| AUD-MKP-claude-043 | «Guardar» lleva `INV-BTN-ACTUALIZAR-PRODUCTO`, el mismo ID que ESP-0007 usa para abrir la edición | ESP-0007 `:129` presiona `INV-BTN-ACTUALIZAR-PRODUCTO` dos veces; la guía (`:138`) pide `INV-BTN-CONFIRMAR-ACTUALIZACION` para confirmar, y no la incluye en el inventario de INV (`:265`) | c | C2 | Con pantalla única, la selección carga la ficha; «Guardar» pasa a `INV-BTN-CONFIRMAR-ACTUALIZACION`, que se agrega a la guía, y ESP-0007 se ajusta. | Media |
| AUD-MKP-claude-044 | Mockups citados que no existen: `ART-MKP-INV-0002`, `0003` y `0004` | ESP-0006 `:110,113`; ESP-0007 `:129,131`; ESP-0008 `:146,149`; ILA-0006 a ILA-0008 | e | C1 | Citar `ART-MKP-INV-0001`. | Alta |
| AUD-MKP-claude-045 | «Crear», «Guardar» y «Eliminar» no se identifican como reservados al Administrador | Matriz `:54-57`: la Técnica solo consulta; RNF-0006 condición 4 | g | C4 | Deshabilitados para la Técnica, con el motivo. | Media |

### 4.3 DOC — `ART-MKP-DOC-0001` / `fuentes HTML/ART-MKP-DOC-0002.html` y `DOC-MDL-PREVISUALIZAR-COMPROBANTE`

Paneles: barra superior (tipo, número y «Consultar») → consulta ESP-0010. Izquierda «Ventas Disponibles» (filtro y tarjetas de venta) → selección de venta de ESP-0009 por `IVentas` (DOC → VEN). Centro «Emisión de Comprobante» → ESP-0009 y ESP-0011. Derecha «Historial de Emisión» (tabla, monto y botones Emitir, Modificar y Anular) → ESP-0010, ESP-0009, ESP-0011 y ESP-0012. Modal de vista previa → sin ESP. Su «Forma de Pago» y las líneas con lote pueden venir en el DTO de `IVentas` (VEN → PAG y VEN → LOT existen), sin arista DOC → PAG.

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-046 | La fuente HTML se llama `ART-MKP-DOC-0002.html` («ART-MKP-DOC-0002 - Documentación Tributaria (v002)»); el PNG es `ART-MKP-DOC-0001.png` | Guía `:116`; C1 | c | C1 | Renombrar a `ART-MKP-DOC-0001.html`. | Alta |
| AUD-MKP-claude-047 | `DOC-CMB-CODIGO-VENTA` rotula cinco cosas: el filtro «Filtrar Venta por Código o Fecha», las tres tarjetas de venta y el combo «Venta Seleccionada» | ESP-0009 `:169`; guía `:266` | c | C2 | `DOC-CMB-FILTRO-VENTA` para el filtro, `DOC-CRD-VENTA` para las tarjetas y `DOC-CMB-CODIGO-VENTA` solo en el formulario. | Alta |
| AUD-MKP-claude-048 | `DOC-LBL-MONTO-TOTAL` rotula los montos de las tres tarjetas y «Monto Total Calculado» | ESP-0009 `:169` | c | C2 | Reservarlo para el monto de la venta seleccionada. | Alta |
| AUD-MKP-claude-049 | `DOC-CMB-TIPO-COMPROBANTE` y `DOC-TXT-NUMERO-COMPROBANTE` rotulan tanto los filtros de la barra superior como los campos del formulario | ESP-0009 `:169` (captura) y ESP-0010 `:187` (filtros), que antes estaban en mockups distintos | c | C2 | `DOC-CMB-FILTRO-TIPO` y `DOC-SRH-NUMERO-COMPROBANTE` en la barra superior. | Alta |
| AUD-MKP-claude-050 | El tipo ofrece «Boleta Electrónica» y «Factura Electrónica» | Dominio `('Boleta', 'Factura')` (`05-diccionario-datos.md:116`); ESP-0009 `:169` | c | C4 | El combo guarda 'Boleta' o 'Factura'. Si el texto visible lleva «Electrónica», es solo un rótulo (ver 056). | Media |
| AUD-MKP-claude-051 | No hay filtro de fecha de emisión en la barra superior | ESP-0010 `:187` (`DOC-FEC-FECHA-EMISION` como filtro); ILA-0010 `02-ilaciones.md:195` | a | C4 | Añadirlo en la implementación. | Alta |
| AUD-MKP-claude-052 | La tabla (N° Comp., Tipo, Total) no muestra fecha ni estado | ESP-0010 `:187` (`fecha_emision`, `estado_documento`); ILA-0010, postcondición 2: los anulados quedan visibles | a | C4 | Añadir fecha y estado. | Alta |
| AUD-MKP-claude-053 | Filtro «Filtrar Venta por Código o Fecha» sobre las ventas disponibles | ESP-0009 `:169` no lo prevé | b | C3 | Filtro extra aceptado; va a la selección de ESP-0009, porque filtra ventas y no comprobantes. | Media |
| AUD-MKP-claude-054 | La emisión se confirma con «Confirmar y Emitir» (`DOC-BTN-CONFIRMAR-EMISION`) | ESP-0009 `:169` confirma con `DOC-BTN-CONFIRMAR-SI`, el mismo ID que confirma la anulación en ESP-0012 `:223`; guía `:136` (el propio botón de crear confirma) | c | C2 | ESP-0009 confirma con `DOC-BTN-CREAR-COMPROBANTE` o, si se mantiene la vista previa (055), con `DOC-BTN-CONFIRMAR-EMISION`. `DOC-BTN-CONFIRMAR-SI` queda para la anulación. | Alta |
| AUD-MKP-claude-055 | Vista previa del comprobante (`DOC-MDL-PREVISUALIZAR-COMPROBANTE`) con DNI/RUC y nombre del cliente, datos del emisor (RUC 20512345678, dirección y teléfono), subtotal más IGV 18 %, importe en letras y hash | Ninguna ESP ni ILA (ESP-0009 `:169`); EDU-0003 `01-educciones.md:79` pide registrar y conservar comprobantes; no hay tabla de clientes (`05-diccionario-datos.md:413`; PEN-010) ni columnas o configuración del emisor | b, d | Requiere decisión | P-09. Si se conserva, la vista previa entra en ESP-0009 solo con datos existentes (venta, detalle, número, tipo, fecha). Cliente y emisor dependen de PEN-010 y de una configuración que hoy no existe. | Alta |
| AUD-MKP-claude-056 | El PNG dice: «Al confirmar se generará el comprobante oficial y se enviará al OSE/SUNAT. No se podrá modificar después». El segundo modal del HTML habla de corregir solo con nota de crédito. | ESP-0011 `:205` permite al Administrador modificar un comprobante 'Emitido'; ESP-0012 `:223` lo anula; ninguna EDU, ILA ni ESP prevé envío a SUNAT u OSE; PEN-003 (`11-registro-pendientes.md:46`) | c | Requiere decisión | Llevarlo a la dueña junto con PEN-003 (P-09). Hasta entonces, el texto contradice ESP-0011 y no debe implementarse. | Alta |
| AUD-MKP-claude-057 | Botones `DOC-BTN-DESCARGAR-PDF`, `DOC-BTN-CANCELAR-EMISION` y `DOC-BTN-CONFIRMAR-EMISION` | No están en la guía (`:266`) ni en ninguna ESP | b | C3 | Descargar el borrador en PDF queda fuera de alcance. Volver y confirmar entran en ESP-0009 solo si se conserva la vista previa (055). | Media |
| AUD-MKP-claude-058 | La vista previa no coincide entre PNG y HTML: el PNG tiene un solo paso y el botón «✓ Confirmar y Emitir»; ambos HTML dicen «Paso 1 de 2» y «Continuar →», y `ART-MKP-DOC-0002.html` añade el modal «Confirmar Emisión Definitiva» con `DOC-BTN-EMITIR-NO` y `DOC-BTN-EMITIR-SI`, que no aparece en ningún PNG | Regla del encargo (manda el PNG) | c | C1 | Alinear los HTML con el PNG: un solo paso; retirar `DOC-BTN-EMITIR-SI` y `DOC-BTN-EMITIR-NO`. | Alta |
| AUD-MKP-claude-059 | Artefacto con nombre de componente: `DOC-MDL-PREVISUALIZAR-COMPROBANTE.png` y su HTML | Guía `:116` (`ART-MKP-<MOD>-<NUM>`); ninguna ESP lo cita | c | C1 | Tratarlo como un modal de la pantalla `ART-MKP-DOC-0001`, cuyo ID de componente es válido, y no como un mockup aparte. | Media |
| AUD-MKP-claude-060 | No hay `DOC-BTN-CONFIRMAR-ACTUALIZACION`, ni se indica que la venta quede de solo lectura al modificar | ESP-0011 `:205`; ILA-0011 `02-ilaciones.md:214` | a | C4 | Añadirlo en la implementación; `DOC-CMB-CODIGO-VENTA` deshabilitado al editar. | Alta |
| AUD-MKP-claude-061 | Mockups citados que no existen: `ART-MKP-DOC-0002`, `0003` y `0004` | ESP-0010 `:187,189`; ESP-0011 `:205,207`; ESP-0012 `:223,225`; ILA-0010 a ILA-0012 | e | C1 | Citar `ART-MKP-DOC-0001`. | Alta |
| AUD-MKP-claude-062 | «Modificar» y «Anular» no se identifican como reservados al Administrador | Matriz `:60-61`; RNF-0006 condición 4 | g | C4 | Deshabilitados para la Técnica; «Emitir» queda para ambos roles. | Media |

### 4.4 ALV — `ART-MKP-ALV-0001`

Paneles: barra superior (remesa y «Filtrar Alertas») → consulta ESP-0013. Izquierda «Lotes en Riesgo» (filtro y tarjetas) → remesas próximas a vencer de ESP-0013, por `ILotes` (ALV → LOT). Centro «Control Sanitario de Lote» → ESP-0014 y ESP-0015. Derecha «Tablero de Alertas» y botones → ESP-0013 a ESP-0016. Modal, solo en el HTML → ESP-0016.

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-063 | `ALV-CMB-LOTE-ALERTA` rotula seis cosas: combo superior, filtro «Filtrar por Estado de Vencimiento», tres tarjetas y combo de la ficha | ESP-0014 `:263`; guía `:267` | c | C2 | `ALV-CMB-FILTRO-LOTE`, `ALV-CMB-FILTRO-ESTADO`, `ALV-CRD-LOTE` y `ALV-CMB-LOTE-ALERTA` solo en la ficha. | Alta |
| AUD-MKP-claude-064 | `ALV-LBL-FECHA-VENCIMIENTO` rotula las tres tarjetas y la ficha | ESP-0014 `:263` | c | C2 | Reservarlo para la ficha. | Alta |
| AUD-MKP-claude-065 | Filtros por remesa y por estado de vencimiento | ESP-0013 `:245` no tiene filtros | b | C3 | Filtros extra aceptados en ESP-0013. | Media |
| AUD-MKP-claude-066 | La tabla (Lote, Venc., Defect.) no muestra el medicamento | ILA-0013, postcondición 1 (`02-ilaciones.md:257`): «identificadas por medicamento y número de lote» | a | C4 | Añadir la columna del medicamento. | Media |
| AUD-MKP-claude-067 | Casilla rotulada «Lote Defectuoso / Bloqueado» | `es_lote_defectuoso` significa que falló el control de calidad (`05-diccionario-datos.md:166`); el bloqueo lo produce toda alerta (ESP-0014 `:263`) | c | C4 | Rotularla «Lote defectuoso». | Media |
| AUD-MKP-claude-068 | Botón «Liberar Lote / Eliminar Alerta»; el HTML dice «Sí, Liberar» | ESP-0016 `:299` y DEC-034 (`11-registro-pendientes.md:114`): la remesa no se libera si tiene otro retiro, devoluciones pendientes o está vencida | c | C4 | Rotularlo «Eliminar alerta» y que el mensaje no prometa la liberación. | Media |
| AUD-MKP-claude-069 | Mockups citados que no existen: `ART-MKP-ALV-0002` y `0003` | ESP-0015 `:281,283`; ESP-0016 `:299,301`; ILA-0015 e ILA-0016 | e | C1 | Citar `ART-MKP-ALV-0001`. | Alta |
| AUD-MKP-claude-070 | «Modificar», «Confirmar Actualización» y «Liberar Lote / Eliminar Alerta» no se identifican como reservados al Administrador | Matriz `:64-65` (ILA-0015 e ILA-0016 solo A; ILA-0014 A y T); RNF-0006 condición 4 | g | C4 | Deshabilitados para la Técnica; «Bloquear Lote» queda para ambos. | Media |

### 4.5 PAG — `ART-MKP-PAG-0001`

Paneles: barra superior (estado, búsqueda y «Buscar») → ESP-0018. Izquierda (tarjetas de métodos) → resultado de ESP-0018 y selección. Centro (ficha y «Previsualización») → ESP-0017 y ESP-0019. Derecha (tabla y botones) → ESP-0017 a ESP-0020. No usa otros módulos.

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-071 | `PAG-CMB-ESTADO-METODO` y `PAG-TXT-METODO-PAGO` rotulan el filtro y el campo de la ficha; `PAG-TBL-METODOS-PAGO` rotula las tarjetas y la tabla | ESP-0017 `:321`; ESP-0018 `:339`; guía `:268` | c | C2 | `PAG-CMB-FILTRO-ESTADO`, `PAG-SRH-METODO` y un ID propio para la lista de tarjetas si se conserva. | Alta |
| AUD-MKP-claude-072 | Buscador por nombre y botón «Buscar» (`PAG-BTN-LEER-METODO`) | ESP-0018 `:339` solo filtra por estado y no tiene botón de lectura; la guía sí lo lista (`:268`) | b | C3 | Filtro extra aceptado: incorporar nombre y botón a ESP-0018. | Media |
| AUD-MKP-claude-073 | Descripciones «Pago en mostrador», «Billetera digital», «POS / Visa» y «No habilitado», íconos y panel «Previsualización del método seleccionado» | `TBL_METODOS_PAGO` solo tiene `nombre_metodo` y `estado` (`05-diccionario-datos.md:131-133`); ninguna ESP | b, d | C3 | Fuera de alcance; la tarjeta muestra nombre y estado. | Media |
| AUD-MKP-claude-074 | No hay `PAG-BTN-CONFIRMAR-ACTUALIZACION`; «Guardar» lleva `PAG-BTN-ACTUALIZAR-METODO` | ESP-0019 `:357` | a | C4 | Añadir la confirmación (o ver L-03). | Alta |
| AUD-MKP-claude-075 | Mockups citados que no existen: `ART-MKP-PAG-0002`, `0003` y `0004` | ESP-0018 `:339,341`; ESP-0019 `:357,359`; ESP-0020 `:375,377`; ILA-0018 a ILA-0020 | e | C1 | Citar `ART-MKP-PAG-0001`. | Alta |
| AUD-MKP-claude-076 | «Crear», «Guardar» y «Eliminar» no se identifican como reservados al Administrador | Matriz `:66-69`; RNF-0006 condición 4 | g | C4 | Deshabilitados para la Técnica. | Media |

### 4.6 REP — `ART-MKP-REP-0001`

Paneles: barra superior (tipo, formato, fechas y «Generar») → ESP-0021. Izquierda «Parámetros y Búsqueda» (buscador, «Filtrar Historial» y «Reportes Recientes») → ESP-0022. Centro «Vista Previa del Reporte» → visualización de ESP-0022, paso final. Derecha «Historial de Reportes» y botones → ESP-0021 a ESP-0024. Usa `IVentas` (REP → VEN) y Usuarios como aspecto transversal.

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-077 | El campo «Buscar reporte» se rotula «Campo Texto: REP-BTN-LEER-REPORTE», el mismo ID que el botón «Filtrar Historial» | ESP-0022 `:415` (`REP-BTN-LEER-REPORTE` es un botón) | c | C2 | `REP-SRH-REPORTE` para el campo. | Alta |
| AUD-MKP-claude-078 | `REP-BTN-GENERAR-REPORTE` rotula dos botones «Generar» (barra superior y panel derecho) | ESP-0021 `:397` | c | C2 | Conservar uno solo. | Media |
| AUD-MKP-claude-079 | `REP-TBL-REPORTES` rotula la «Vista Previa» y la tabla «Historial» | ESP-0022 `:415` (tabla de índices; abre el archivo en solo lectura) | c | C2 | `REP-PNL-VISTA-PREVIA` para la visualización. | Alta |
| AUD-MKP-claude-080 | Lista «Reportes Recientes», sin ID | Duplica `REP-TBL-REPORTES`; ninguna ESP | b | C3 | Retirarla o fundirla con el historial. | Baja |
| AUD-MKP-claude-081 | Buscador «Código o tipo…» | ESP-0022 `:415` no tiene filtros | b | C3 | Filtro extra aceptado en ESP-0022. | Media |
| AUD-MKP-claude-082 | El historial (Tipo, Fecha, Formato) no muestra identificador, período ni total recaudado | ESP-0022 `:415` (`id_reporte, tipo_reporte, fecha_inicio, fecha_fin, total_recaudado`) | a | C4 | Columnas según ESP-0022; «Fecha» es ambigua. | Alta |
| AUD-MKP-claude-083 | La vista previa (Fecha, Producto, Cantidad, Total) no identifica al personal | EDU-0010 `01-educciones.md:136`; ESP-0021 `:397` (`usuario_responsable`) | a | C4 | Añadir la columna del usuario responsable en el archivo generado. | Media |
| AUD-MKP-claude-084 | Tipos de reporte: el HTML ofrece «Ventas por Fecha», «Ventas por Producto» y «Consolidado General»; el PNG muestra «Por Fecha», «Por Producto», «Ventas por Fecha» y «Ventas por Producto» en historial y recientes | PEN-001 abierto (`11-registro-pendientes.md:44`); `tipo_reporte` sin dominio cerrado (`05-diccionario-datos.md:231`) | c | Requiere decisión | P-10: confirmar si la aprobación del mockup cierra PEN-001 con esos valores. «Consolidado General» solo aparece en el HTML. | Alta |
| AUD-MKP-claude-085 | El formato solo ofrece PDF y Excel (HTML) | `CHECK (formato IN ('PDF', 'Excel', 'HTML'))` (`:232`); ESP-0021 `:397` | c | C4 | Ofrecer también HTML, o reducir el dominio por decisión registrada. | Baja |
| AUD-MKP-claude-086 | No hay `REP-BTN-CONFIRMAR-ACTUALIZACION` | ESP-0023 `:433` | a | C4 | Añadirlo (o ver L-03). | Alta |
| AUD-MKP-claude-087 | Mockups citados que no existen: `ART-MKP-REP-0002` y `0003` | ESP-0022 `:414,417`; ESP-0023 `:432-433,435`; ESP-0024 `:450,453`; ILA-0022 a ILA-0024 | e | C1 | Citar `ART-MKP-REP-0001`. | Alta |

### 4.7 DEV — `ART-MKP-DEV-0001`

Paneles: barra superior (proveedor, búsqueda y «Buscar») → ESP-0026. Izquierda «Órdenes de Devolución» → `DEV-TBL-ORDENES-DEVOLUCION` de ESP-0026. Centro «Nueva Orden de Devolución» → ESP-0025 y ESP-0027. Derecha «Detalle de la Orden» y botones → ESP-0025 a ESP-0028. Usa `IProveedores` y `ILotes`; el nombre del medicamento de cada remesa llega por `ILotes` (LOT → INV), sin DEV → INV.

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-088 | `DEV-CMB-PROVEEDOR` rotula el filtro de la barra superior y el combo del formulario | ESP-0025 `:473`; guía `:270` | c | C2 | `DEV-CMB-FILTRO-PROVEEDOR` en la barra superior. | Alta |
| AUD-MKP-claude-089 | Buscador «Buscar por N° de orden o lote…» (`DEV-TXT-BUSCAR-ORDEN`, marcado «Campo Texto*») y filtro por proveedor | No están en la guía (`:270`) ni en ESP-0026 `:491` | b | C3 | Filtros extra aceptados en ESP-0026; el buscador pasa a `DEV-SRH-ORDEN`. | Media |
| AUD-MKP-claude-090 | No hay control para agregar la remesa y la cantidad al detalle de la orden | ESP-0025 `:473` (`AGREGAR (lote, cantidad) A detalle … HASTA QUE USUARIO NO AGREGUE MAS REMESAS`) sin nombrar el control; ILA-0025 paso 6 (`02-ilaciones.md:494`) | a | C4 | Nombrarlo en la guía y en ESP-0025/0027 (por ejemplo `DEV-BTN-AGREGAR-REMESA` y `DEV-BTN-QUITAR-REMESA`, como en Ventas). | Alta |
| AUD-MKP-claude-091 | No hay control para pasar la orden a 'Completada' | ESP-0027 `:509` (`estadoNuevo = ESTADO_SELECCIONADO`, sin ID); ILA-0027 paso 6 | a | C4 | `DEV-CMB-ESTADO-ORDEN` en la guía, en ESP-0027 y en la implementación. | Alta |
| AUD-MKP-claude-092 | No hay `DEV-BTN-CONFIRMAR-ACTUALIZACION` | ESP-0027 `:509`; guía `:270` | a | C4 | Añadirlo (o ver L-03). | Alta |
| AUD-MKP-claude-093 | La lista de órdenes no muestra el motivo | ESP-0026 `:491` (`motivo_devolucion`) | a | C4 | Añadirlo a cada tarjeta. | Baja |
| AUD-MKP-claude-094 | Rótulo «Fecha de Devolución» para `DEV-FEC-FECHA-DEVOLUCION` | `fecha_creacion`: «fecha en que el usuario registró la orden» (`05-diccionario-datos.md:181`); ILA-0025 paso 2 | c | C4 | Rotularla «Fecha de la orden». | Baja |
| AUD-MKP-claude-095 | Mockups citados que no existen: `ART-MKP-DEV-0002`, `0003` y `0004` | ESP-0026 `:491,493`; ESP-0027 `:509,511`; ESP-0028 `:527,529`; ILA-0026 a ILA-0028 | e | C1 | Citar `ART-MKP-DEV-0001`. | Alta |

### 4.8 RES — `ART-MKP-RES-0001`

Paneles: barra superior (buscador, tipo, estado y «Buscar») → ESP-0030. Izquierda «Restricciones Activas» → resultado de ESP-0030. Centro «Ficha de Restricción» → ESP-0029 y ESP-0031. Derecha (tabla y botones) → ESP-0029 a ESP-0032. Usa `IInventario` (RES → INV).

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-096 | El buscador de texto se rotula «Campo Texto: RES-CMB-MEDICAMENTO», el mismo ID que el combo de la ficha | ESP-0030 `:567` (combo); la guía retiró `RES-TXT-MEDICAMENTO` (`:287`) | c | C2 | `RES-SRH-MEDICAMENTO`, o un combo como pide ESP-0030. | Alta |
| AUD-MKP-claude-097 | `RES-CMB-TIPO-RESTRICCION` y `RES-CMB-ESTADO-ALERTA` rotulan filtros y campos de la ficha; `RES-TBL-RESTRICCIONES-VENTA` rotula la lista y la tabla | ESP-0029 `:549`; guía `:272` | c | C2 | `RES-CMB-FILTRO-TIPO`, `RES-CMB-FILTRO-ESTADO` e ID propio para la lista. | Alta |
| AUD-MKP-claude-098 | Filtros por tipo y estado | ESP-0030 `:567` solo filtra por medicamento | b | C3 | Filtros extra aceptados en ESP-0030. | Media |
| AUD-MKP-claude-099 | La tabla (Medicamento, Tipo, Estado) no muestra condición del cliente ni motivo | ESP-0030 `:567` (`condicion_cliente, motivo_advertencia`) | a | C4 | Añadir las columnas. | Media |
| AUD-MKP-claude-100 | No hay `RES-BTN-CONFIRMAR-ACTUALIZACION` | ESP-0031 `:585` | a | C4 | Añadirlo (o ver L-03). | Alta |
| AUD-MKP-claude-101 | Botón «Eliminar»; el HTML pregunta «¿Está seguro de eliminar esta restricción de venta?» | ESP-0032 `:603` desactiva (`estado_alerta = 'Inactivo'`) | c | C4 | Rotularlo «Desactivar» y usar el mensaje de ESP-0032. | Media |
| AUD-MKP-claude-102 | Datos de ejemplo: «Mayor de 18 años con receta», «Requiere receta médica», «Venta bajo receta retenida» | EDU-0012 `01-educciones.md:174` (poblaciones vulnerables: VIH, cáncer, embarazadas, neonatos); `condicion_cliente` (`05-diccionario-datos.md:217`); PEN-025 (`11-registro-pendientes.md:61`) | c | Requiere decisión | P-13: ¿las condiciones por receta o por edad entran en EDU-0012? Se responde junto con PEN-025. | Media |
| AUD-MKP-claude-103 | Mockups citados que no existen: `ART-MKP-RES-0002`, `0003` y `0004`. Además, ESP-0030 a ESP-0032 no citan `ART-MKP-RES-0001`. | ESP-0030 `:566,569`; ESP-0031 `:584,587`; ESP-0032 `:602,605`; ILA-0030 a ILA-0032 | e | C1 | Citar `ART-MKP-RES-0001`. | Alta |
| AUD-MKP-claude-104 | «Crear», «Actualizar» y «Eliminar» no se identifican como reservados al Administrador | Matriz `:78-81`; RNF-0006 condición 4 | g | C4 | Deshabilitados para la Técnica, que solo consulta. | Media |

### 4.9 USR — `ART-MKP-USR-0001`

Paneles: barra superior (nombre, rol y «Buscar») → ESP-0034. Izquierda «Cuentas de Usuario» → resultado de ESP-0034. Centro «Ficha de Usuario» → ESP-0033 y ESP-0035. Derecha (tabla y botones) → ESP-0033 a ESP-0036. Módulo exclusivo del Administrador.

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-105 | `USR-TXT-NOMBRE-USUARIO` y `USR-CMB-ROL` rotulan filtros y campos de la ficha; `USR-TBL-USUARIOS` rotula la lista y la tabla | ESP-0033 `:625`; ESP-0034 `:643`; guía `:151-155` | c | C2 | `USR-SRH-USUARIO`, `USR-CMB-FILTRO-ROL` e ID propio para la lista. | Alta |
| AUD-MKP-claude-106 | Con la cuenta existente «tecnico_01» cargada en la ficha, la contraseña aparece con contenido («••••••••») | ESP-0035 `:661` e ILA-0035 `02-ilaciones.md:692` (en blanco al actualizar); guía `:153` | c | C4 | En edición, el campo se muestra vacío. | Media |
| AUD-MKP-claude-107 | La tabla muestra «Admin» | Dominio `('Administrador', 'Tecnico')` (`05-diccionario-datos.md:12`) | c | C4 | Mostrar «Administrador». | Baja |
| AUD-MKP-claude-108 | La tabla no muestra el identificador | ESP-0034 `:643` (`id_usuario`) | a | C4 | Añadirlo. | Baja |
| AUD-MKP-claude-109 | No hay `USR-BTN-CONFIRMAR-ACTUALIZACION` | ESP-0035 `:661`; guía `:159` | a | C4 | Añadirlo (o ver L-03). | Alta |
| AUD-MKP-claude-110 | Mockups citados que no existen: `ART-MKP-USR-0002`, `0003` y `0004`. La guía sigue marcando los cuatro como «Por diseñar». | ESP-0034 `:642,645`; ESP-0035 `:660,663`; ESP-0036 `:678,681`; ILA-0034 a ILA-0036; guía `:152-164`, `:318-321` | e | C1 | Citar `ART-MKP-USR-0001`. | Alta |

### 4.10 PRV — `ART-MKP-PRV-0001`

Paneles: barra superior (buscador, estado y «Buscar») → ESP-0038. Izquierda «Proveedores» → resultado de ESP-0038. Centro «Ficha de Proveedor» → ESP-0037 y ESP-0039. Derecha (tabla y botones) → ESP-0037 a ESP-0040. Módulo exclusivo del Administrador.

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-111 | Un solo buscador rotulado «PRV-TXT-RUC / PRV-TXT-RAZON-SOCIAL» (dos IDs para un control, que además rotulan los campos de la ficha) | ESP-0038 `:719` (dos campos) | c | C2 | `PRV-SRH-PROVEEDOR` y un criterio único en ESP-0038. | Alta |
| AUD-MKP-claude-112 | `PRV-CMB-ESTADO-PROVEEDOR` rotula el filtro y el campo de la ficha; `PRV-TBL-PROVEEDORES` rotula la lista y la tabla | ESP-0037 `:701`; guía `:170-174` | c | C2 | `PRV-CMB-FILTRO-ESTADO` e ID propio para la lista. | Alta |
| AUD-MKP-claude-113 | Filtro por estado | ESP-0038 `:719` no lo tiene | b | C3 | Filtro extra aceptado en ESP-0038. | Media |
| AUD-MKP-claude-114 | La tabla (RUC, Razón Social, Estado) no muestra teléfono ni identificador | ESP-0038 `:719` (`id_proveedor, … telefono`) | a | C4 | Añadirlos. | Baja |
| AUD-MKP-claude-115 | No hay `PRV-BTN-CONFIRMAR-ACTUALIZACION` | ESP-0039 `:737` | a | C4 | Añadirlo (o ver L-03). | Alta |
| AUD-MKP-claude-116 | Mockups citados que no existen: `ART-MKP-PRV-0002`, `0003` y `0004` | ESP-0038 `:718,721`; ESP-0039 `:736,739`; ESP-0040 `:754,757`; ILA-0038 a ILA-0040; guía `:322-325` | e | C1 | Citar `ART-MKP-PRV-0001`. | Alta |

### 4.11 LOT — `ART-MKP-LOT-0001`

Paneles: barra superior (número de lote, producto, estado y «Buscar») → ESP-0042. Izquierda «Remesas / Lotes» → resultado de ESP-0042. Centro «Ficha de Remesa» → ESP-0041 y ESP-0043. Derecha (tabla y botones) → ESP-0041 a ESP-0044. Usa `IInventario` e `IProveedores`.

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-117 | No hay combo de proveedor de la remesa | ESP-0041 `:777` y ESP-0043 `:813` (`LOT-CMB-PROVEEDOR-LOTE`); ILA-0041 paso 3 (`02-ilaciones.md:861`); `TBL_LOTES.id_proveedor NOT NULL` (`05-diccionario-datos.md:64`) | a | C4 | Añadirlo; es obligatorio y sostiene la validación de ESP-0025. | Alta |
| AUD-MKP-claude-118 | «Estado» de la ficha es un combo editable con `LOT-CMB-ESTADO-LOTE` | DEC-010 (`11-registro-pendientes.md:90`): el estado no se edita; ESP-0041 `:777` asigna 'Disponible'; ESP-0043 `:813` lo muestra de solo lectura | c | C4 | En la ficha, solo lectura (por ejemplo `LOT-LBL-ESTADO-LOTE`); el combo queda como filtro. | Alta |
| AUD-MKP-claude-119 | `LOT-TXT-NUMERO-LOTE`, `LOT-CMB-PRODUCTO` y `LOT-CMB-ESTADO-LOTE` rotulan filtros y campos de la ficha; `LOT-TBL-LOTES` rotula la lista y la tabla | ESP-0041 `:777`; ESP-0042 `:795`; guía `:212-218` | c | C2 | `LOT-SRH-LOTE`, `LOT-CMB-FILTRO-PRODUCTO`, `LOT-CMB-FILTRO-ESTADO` e ID propio para la lista. | Alta |
| AUD-MKP-claude-120 | La tabla (Lote, Vence, Stock) no muestra medicamento ni estado | ESP-0042 `:795` (`nombre, … estado_lote`) | a | C4 | Añadirlos. | Baja |
| AUD-MKP-claude-121 | Orden de ejemplo 15/10, 02/11 y 01/10: no es ascendente por vencimiento | ESP-0042 `:795` (`ORDENAR POR fecha_vencimiento ASCENDENTE`); ILA-0042 | c | C4 | La implementación ordena por vencimiento ascendente. | Baja |
| AUD-MKP-claude-122 | No hay `LOT-BTN-CONFIRMAR-ACTUALIZACION` | ESP-0043 `:813`; guía `:222` | a | C4 | Añadirlo (o ver L-03). | Alta |
| AUD-MKP-claude-123 | Mockups citados que no existen: `ART-MKP-LOT-0002`, `0003` y `0004` | ESP-0042 `:794,797`; ESP-0043 `:812,815`; ESP-0044 `:830,833`; ILA-0042 a ILA-0044; guía `:326-329` | e | C1 | Citar `ART-MKP-LOT-0001`. | Alta |
| AUD-MKP-claude-124 | «Crear», «Actualizar» y «Eliminar» no se identifican como reservados al Administrador | Matriz `:84-87` (la Técnica solo consulta); RNF-0006 condición 4 | g | C4 | Deshabilitados para la Técnica. | Media |

### 4.12 REA — `ART-MKP-REA-0001`

Paneles: encabezado con «Generar Lista» → ninguna ESP. Tres indicadores → ninguna ESP. Izquierda «Lista de Reabastecimiento» (filtro y tabla) → ESP-0046. Derecha «Acciones» → ESP-0045, ESP-0047 y ESP-0048. Usa `IInventario` (stock mínimo) e `ILotes` (stock vendible), y requeriría Cotizaciones para «Pendientes de cotizar» (127).

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-125 | Botón «⚙ Generar Lista» (`REA-BTN-GENERAR-LISTA*`) | EDU-0016 `01-educciones.md:250` (se alimenta con un proceso periódico, condición 2 de RNF-0010); ninguna ESP prevé ejecutarlo a mano | b | Requiere decisión | P-11: si la dueña quiere lanzarlo a mano, añadirlo a EDU, ILA y ESP; si no, C3. | Media |
| AUD-MKP-claude-126 | Indicadores «3 Productos bajo mínimo» (`REA-NUM-STOCK-ACTUAL*`), «5 Prioridad alta» (`REA-CMB-PRIORIDAD*`) y «2 Pendientes de cotizar» (`REA-TBL-LISTA-REABASTECIMIENTO*`): IDs de otros controles sobre contadores | Ninguna ESP; guía `:86` (LBL para valores calculados) | b, c | C3 | Fuera de alcance. Si la dueña los quiere, `REA-LBL-…` en ESP-0046. | Alta |
| AUD-MKP-claude-127 | Indicador «Pendientes de cotizar» | Necesita saber qué ítems tienen cotización (`TBL_COTIZACIONES_PROVEEDOR`); REA → COT no existe (`Insumo_Matriz…v2.md:69`) y COT → REA sí (`:70`), de modo que la arista crearía un ciclo (`VL-01_Paquetes.puml:121-124`) | h | C3 | Retirarlo de REA. Si se quiere, calcularlo en la pantalla de COT, que ya depende de REA. | Alta |
| AUD-MKP-claude-128 | El campo de texto «Filtrar por producto…» se rotula `REA-CMB-PRODUCTO*`, igual que el combo «Producto Seleccionado» | ESP-0045 `:853`; ESP-0046 `:871` | c | C2 | `REA-SRH-PRODUCTO` para el filtro (extra aceptado en ESP-0046). | Alta |
| AUD-MKP-claude-129 | No hay `REA-CMB-FILTRO-PRIORIDAD`, `REA-CMB-FILTRO-ORIGEN` ni `REA-BTN-LEER-ITEM` | ESP-0046 `:871`; ILA-0046 `02-ilaciones.md:960` | a | C4 | Añadirlos. | Alta |
| AUD-MKP-claude-130 | Columna «Sugerido» y campo «Cantidad Sugerida» (`REA-NUM-CANTIDAD-SUGERIDA*`) | `TBL_LISTA_REABASTECIMIENTO` no tiene columna de cantidad (`05-diccionario-datos.md:251-255`); sin embargo, ILA-0050 paso 9 (`02-ilaciones.md:1040`) exporta «la cantidad pendiente en TBL_LISTA_REABASTECIMIENTO» y el Anexo C.5 (`:843`) dice que el CSV lleva «medicamento, cantidad, precio»; PEN-002 | d | Requiere decisión | P-11: ¿la cantidad la escribe la dueña o la calcula el sistema? Según la respuesta, columna nueva en `TBL_LISTA_REABASTECIMIENTO` y captura en ESP-0045/0047, o corregir ILA-0050. No se propone una fórmula. | Alta |
| AUD-MKP-claude-131 | Prioridades «CRÍTICO», «ALTO» y «MEDIO», e indicador «Prioridad alta» | Dominio `('Normal', 'Urgente')` (`:254`); EDU-0016 `:250` (marcar como urgente, en rojo); ESP-0047 `:889` | c | Requiere decisión | P-11: dos niveles (catálogo) o tres (mockup). Si son tres, cambian EDU-0016, el dominio y ESP-0045 a ESP-0047. | Alta |
| AUD-MKP-claude-132 | No hay combo de prioridad para capturar, ni `REA-LBL-FECHA-INCLUSION` ni `REA-LBL-ORIGEN`; la tabla no muestra origen ni fecha de inclusión | ESP-0045 `:853`; ESP-0046 `:871` (`nombre, origen, prioridad, fecha_inclusion`); ESP-0047 `:889` | a | C4 | Añadirlos. | Alta |
| AUD-MKP-claude-133 | «Actualizar Ítem» modifica la cantidad sugerida | ESP-0047 `:889`: solo se modifica la prioridad; producto y origen son de solo lectura | c | C4 | Actualizar modifica la prioridad; la cantidad depende de 130. | Alta |
| AUD-MKP-claude-134 | Columnas «Stock Actual» y «Stock Mínimo» | EDU-0016 `:250` las respalda («stock consolidado … por debajo de su stock_minimo»), pero ESP-0046 `:871` no las carga; DEC-040: el reabastecimiento usa el stock vendible | f | C3 | Hueco de ESP, no fuera de alcance: añadirlas a ILA-0046 y ESP-0046 por `ILotes` e `IInventario`, rotulando «Stock vendible» (provisional hasta PEN-024). | Media |
| AUD-MKP-claude-135 | No hay `REA-BTN-CONFIRMAR-ACTUALIZACION`, y el modal del HTML no tiene `REA-BTN-CONFIRMAR-NO` | ESP-0047 `:889`; ESP-0048 `:907` | a | C4 | Añadirlos. | Alta |
| AUD-MKP-claude-136 | Mockups citados que no existen: `ART-MKP-REA-0002`, `0003` y `0004` | ESP-0046 `:870,873`; ESP-0047 `:888-889,891`; ESP-0048 `:906,909`; ILA-0046 a ILA-0048 | e | C1 | Citar `ART-MKP-REA-0001`. | Alta |

### 4.13 COT — `ART-MKP-COT-0001`

Paneles: encabezado con «Exportar CSV» → exportación de ESP-0050. Izquierda «Matriz Comparativa» (buscador y tabla) → comparación de ESP-0050. Derecha «Registrar Cotización» → ESP-0049, ESP-0051 y ESP-0052. Usa `IReabastecimiento`, `IInventario` e `IProveedores`, todas existentes.

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-137 | «⬇ Exportar CSV» lleva `COT-BTN-EXPORTAR-CSV*` | ESP-0050 `:947` (`COT-BTN-EXPORTAR-PROVEEDOR`) | c | C4 | `COT-BTN-EXPORTAR-PROVEEDOR`. | Alta |
| AUD-MKP-claude-138 | No hay selector del proveedor destinatario: el mockup exporta un único CSV | ESP-0050 `:947` (`COT-CMB-PROVEEDOR-EXPORTAR`); EDU-0017 `01-educciones.md:269` («un archivo CSV por proveedor») | a | C4 | Añadirlo. | Alta |
| AUD-MKP-claude-139 | No hay forma de elegir, por medicamento, a qué proveedor se compra | ESP-0050 `:947` (`proveedor_seleccionado`, sin ID); ILA-0050 paso 6 (`02-ilaciones.md:1040`) | a | C4 | Nombrar el control en la guía y en ESP-0050 y añadirlo en la implementación (por ejemplo, una selección por fila de la matriz). | Alta |
| AUD-MKP-claude-140 | El buscador de texto se rotula «Campo Texto: COT-BTN-LEER-COTIZACION*» | ESP-0050 `:947` (`COT-CMB-FILTRO-PRODUCTO` y botón `COT-BTN-LEER-COTIZACION`) | c | C2 | El campo usa `COT-CMB-FILTRO-PRODUCTO` (o `COT-SRH-PRODUCTO`); `COT-BTN-LEER-COTIZACION` es el botón. | Alta |
| AUD-MKP-claude-141 | Matriz rotulada `COT-TBL-COTIZACIONES-PROVEEDOR*` | ESP-0050 `:947` (`COT-TBL-COMPARATIVA-PRECIOS`) | c | C4 | `COT-TBL-COMPARATIVA-PRECIOS`. | Alta |
| AUD-MKP-claude-142 | No hay historial de cotizaciones: la matriz solo muestra la vigente, así que no se puede seleccionar una cotización concreta para actualizarla o eliminarla | ESP-0049 `:929`, ESP-0051 `:965` y ESP-0052 `:983` (`COT-TBL-COTIZACIONES`) | a | C4 | Añadir la tabla del historial. | Alta |
| AUD-MKP-claude-143 | «Precio Ofrecido (S/)» con `COT-NUM-PRECIO-OFRECIDO*` | ESP-0049 `:929` (`COT-NUM-PRECIO-COTIZADO`); `precio_cotizado` (`05-diccionario-datos.md:272`) | c | C4 | `COT-NUM-PRECIO-COTIZADO`. | Alta |
| AUD-MKP-claude-144 | «Tiempo de Entrega (días)» con `COT-NUM-TIEMPO-ENTREGA*` | Sin columna (`:269-275`), sin EDU (`01-educciones.md:269`), sin ILA ni ESP | b, d | C3 | Fuera de alcance salvo P-12. | Alta |
| AUD-MKP-claude-145 | No hay campo de promoción | ESP-0049 `:929` y ESP-0051 `:965` (`COT-TXA-PROMOCION`); `promocion` (`:274`) | a | C4 | Añadirlo. | Alta |
| AUD-MKP-claude-146 | No hay `COT-BTN-CONFIRMAR-ACTUALIZACION` | ESP-0051 `:965` | a | C4 | Añadirlo (o ver L-03). | Alta |
| AUD-MKP-claude-147 | Mockups citados que no existen: `ART-MKP-COT-0002`, `0003` y `0004`. PEN-002 también cita `ART-MKP-COT-0002`. | ESP-0050 `:946,949`; ESP-0051 `:964,967`; ESP-0052 `:982,985`; ILA-0050 a ILA-0052; `11-registro-pendientes.md:45` | e | C1 | Citar `ART-MKP-COT-0001`. | Alta |

### 4.14 Transversales y `ART-MKP-CONFIRM-0001`

| ID | Componente del mockup | Referencia del catálogo | Tipo | Criterio | Propuesta | Confianza |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-MKP-claude-148 | Los 13 mockups son pantallas únicas, pero la guía sigue repartiendo cada módulo en 0001 a 0004, su inventario cita 0002 a 0004 y su sección 8 marca USR, PRV, LOT, REA y COT como «Por diseñar» | Guía `:119-126`, `:151-227`, `:248-256` y `:316-335`; `04-trazabilidad.md:8-58` (cita 0002 a 0004) | e | C1 | Actualizar la guía (una pantalla por módulo; columna «Mockup» = 0001; sección 8 con el estado real) y la trazabilidad. | Alta |
| AUD-MKP-claude-149 | Los mockups de REA y COT usan prefijos y componentes que la guía no recoge | Guía `:44-56` (solo módulos 1 a 11) y `:262-272` (sin inventario REA ni COT); `02-ilaciones.md:1125` (prefijos hasta RES) | c | C2 | Añadir los módulos 12 (REA) y 13 (COT) y su inventario. | Alta |
| AUD-MKP-claude-150 | Los modales de baja solo existen en los HTML (ocultos con `display:none`), no en los PNG aprobados, y su texto difiere del de las ESP. Por ejemplo: INV «¿Está seguro de eliminar este producto del inventario?» frente a ESP-0008 «¿Está seguro de dar de baja este producto?»; lo mismo en DOC, ALV, PAG, REP, DEV, RES, USR, PRV, LOT, REA y COT | ESP-0008 `:147`; ESP-0012 `:223`; ESP-0016 `:299`; ESP-0020 `:375`; ESP-0024 `:451`; ESP-0028 `:527`; ESP-0032 `:603`; ESP-0036 `:679`; ESP-0040 `:755`; ESP-0044 `:831`; ESP-0048 `:907`; ESP-0052 `:983` | c | C4 | Usar el texto de cada ESP en la implementación. | Alta |
| AUD-MKP-claude-151 | `ART-MKP-CONFIRM-0001`: «eliminación / anulación permanente» y «Nota del Sistema: Esta operación no se podrá deshacer y quedará auditada en el historial de eventos del sistema» | Varias bajas son lógicas (DEC-009, `11-registro-pendientes.md:89`; ESP-0012, ESP-0020, ESP-0032 y ESP-0036) y el diccionario no tiene ninguna tabla de historial de eventos | b, d | C3 | Retirar la nota y el «permanente»; cada modal usa el mensaje de su ESP (150). | Alta |
| AUD-MKP-claude-152 | `ART-MKP-CONFIRM-0001` no sigue la nomenclatura: «CONFIRM» no es un módulo; es una plantilla con `[MODULO]` | PEN-006 (`11-registro-pendientes.md:49`); guía `:116` y `:139` | c | C1 | Declararlo en la guía como plantilla común de `<MOD>-MDL-CONFIRMAR-ELIMINACION` y no citarlo como mockup de un módulo. L-04. | Media |
| AUD-MKP-claude-153 | Asterisco al final de los IDs: en todas las etiquetas de los PNG de REA y COT (no en sus HTML) y en «Campo Texto*: DEV-TXT-BUSCAR-ORDEN» (PNG y HTML) | La guía (`:17-38`) no define el asterisco | c | Requiere decisión | L-04: definir su significado (por ejemplo, «componente propuesto que el catálogo aún no tiene») o retirarlo, y alinear PNG y HTML. | Media |
| AUD-MKP-claude-154 | Marcadores de imagen: tarjetas de VEN, «[ IMG ]» de INV, «[ LOTE IMG ]» de ALV, «[ TICKET ]» de DOC e íconos de PAG | Ninguna columna de imagen en el diccionario (`05-diccionario-datos.md:3-275`); ninguna ESP | b, d | C3 | Fuera de alcance; se implementan sin imagen. | Media |
| AUD-MKP-claude-155 | Códigos con formato que ninguna columna respalda: «PROD-00842» (INV), «VEN-2026-091» (DOC), «ORD-2026-014» (DEV) | Las claves son `INT IDENTITY` (`:37`, `:82`, `:180`); no hay columnas de código ni de serie | d | Requiere decisión | L-05: mostrar el identificador o definir un formato de presentación en la guía. | Baja |
| AUD-MKP-claude-156 | Literales con tilde en combos: «Por Depuración de Error» (HTML de DEV) y «Bloqueado por devolución» (PNG y HTML de LOT) | DEC-031 (`11-registro-pendientes.md:111`): literales sin tilde; `05-diccionario-datos.md:182` y `:62` | c | C4 | El valor guardado es el literal sin tilde. Si se quiere mostrar con tilde, el texto visible se mapea (L-06). | Baja |
| AUD-MKP-claude-157 | No hay un mockup de navegación o menú que muestre los módulos exclusivos del Administrador (REP, DEV, USR, PRV, REA y COT) deshabilitados para la Técnica | Matriz `07-matriz-permisos.md:70-89`; RNF-0006 condición 4 (`08-requisitos-no-funcionales.md:84`) | g | C4 | La implementación sigue RNF-0006: las opciones se ven pero se deshabilitan e indican el motivo. | Media |

## 5. Resumen por módulo y por criterio

| Módulo | Hallazgos | C1 | C2 | C3 | C4 | Requiere decisión |
| --- | --- | --- | --- | --- | --- | --- |
| VEN | 33 | 3 | 8 | 5 | 10 | 7 |
| INV | 12 | 1 | 5 | 2 | 3 | 1 |
| DOC | 17 | 4 | 4 | 2 | 5 | 2 |
| ALV | 8 | 1 | 2 | 1 | 4 | 0 |
| PAG | 6 | 1 | 1 | 2 | 2 | 0 |
| REP | 11 | 1 | 3 | 2 | 4 | 1 |
| DEV | 8 | 1 | 1 | 1 | 5 | 0 |
| RES | 9 | 1 | 2 | 1 | 4 | 1 |
| USR | 6 | 1 | 1 | 0 | 4 | 0 |
| PRV | 6 | 1 | 2 | 1 | 2 | 0 |
| LOT | 8 | 1 | 1 | 0 | 6 | 0 |
| REA | 12 | 1 | 1 | 3 | 4 | 3 |
| COT | 11 | 1 | 1 | 1 | 8 | 0 |
| Transversales | 10 | 2 | 1 | 2 | 3 | 2 |
| **Total** | **157** | **20** | **33** | **23** | **64** | **17** |

Los 17 que requieren decisión son 004, 008, 015, 022, 030, 031, 032, 040, 055, 056, 084, 102, 125, 130, 131, 153 y 155. Sus preguntas están en la sección 7.

Por tipo (nueve hallazgos tienen dos tipos y cuentan en ambos): (a) 37 · (b) 23 · (c) 66 · (d) 12 · (e) 14 · (f) 2 · (g) 9 · (h) 3.

Por confianza: alta 105, media 40, baja 12.

Varias ILA y ESP de USR, PRV, LOT, REA y COT siguen en estado «Pendiente» porque dicen que su mockup «aún no ha sido diseñado» (por ejemplo, ILA-0033, `02-ilaciones.md:659`). Ese motivo ya no aplica, pero cerrarlas depende de los hallazgos de cada tabla.

## 6. Comparación con el informe 12 (`seguimiento/12-revision-mockups.md`, 01/11/2026)

### 6.1 Resultado

- El informe 12 tiene **120 filas**. De ellas, **117 siguen vigentes** y **3 ya no aplican**. Además, una parte de las 13 filas (e) quedó obsoleta (ver 6.3).
- Este informe añade **41 hallazgos nuevos** y convierte en hallazgo **4 «otras observaciones»** que el 12 dejó fuera de sus categorías (013, 017, 043 y 102).
- Hay tres diferencias de método que explican casi todos los nuevos:
  1. Las fuentes HTML entraron con el PR #144 (08/11/2026), después del informe 12. De ahí salen las diferencias entre PNG y HTML (028, 029, 046, 058 y parte de 153) y la comprobación de que los modales de baja existen, ocultos, en doce HTML.
  2. Este encargo añade los tipos (f), (g) y (h). Con ellos aparecen ocho hallazgos de permisos (el noveno, 012, ya estaba en el 12 como nombre distinto) y los de arquitectura y trazabilidad 030, 031 y 127. 134 reclasifica como (f) una fila (d) del 12.
  3. El informe 12 metía en su fila (e) el botón de confirmar actualización de cada módulo («sin pantalla quedan…»). Con C1, la pantalla 0001 existe y a ese botón simplemente le falta un control: aquí es un hallazgo (a) propio (C4) en cada módulo. No se cuentan como nuevos.

### 6.2 Correspondencia por módulo

| Módulo | Filas del 12 | Vigentes → hallazgo de este informe | Ya no aplican | Nuevos en este informe |
| --- | :-: | --- | --- | --- |
| VEN | 17 | Todas: 001, 021, 002, 019 y 020, 020 y 154, 003, 015, 007, 005, 008, 010, 009, 018, 012, 023, 025, 027. Las IDs de 024 y 026 estaban dentro de su fila (e). | — | 004, 006, 011, 014, 016, 022, 028, 029, 030, 031, 032, 033 (y 013 y 017, que eran observaciones) |
| INV | 11 | Todas: 034, 039, 040, 035, 036, 038, 038 y 154, 155, 042, 041, 044 | — | 037, 045 (y 043, que era observación) |
| DOC | 14 | Todas: 047, 053, 050, 052, 054, 059, 055, 057 (dos filas), 054, 055 (dos filas), 056, 061 | — | 046, 048, 049, 051, 058, 062 |
| ALV | 5 | 063, 065, 067, 069 | «Confirmar Actualización» en la pantalla principal (12, línea 190) | 064, 066, 068, 070 |
| PAG | 7 | Todas: 071, 072 (dos filas), 073 (dos filas), 074, 075 | — | 076 |
| REP | 10 | Todas: 079, 078, 077, 081, 084, 084 (vista previa por producto; depende de PEN-001), 083, 082, 080, 087 | — | 085 |
| DEV | 6 | Todas: 088, 089, 155 (solo el código de orden; ver 6.3), 090, 091, 095 | — | 093, 094 |
| RES | 5 | Todas: 096, 098, 097, 099, 103 | — | 101, 104 (y 102, que era observación) |
| USR | 5 | Todas: 105 (dos filas), 107, 108, 110 | — | 106 |
| PRV | 5 | Todas: 111, 112, 113, 114, 116 | — | — |
| LOT | 6 | Todas: 117, 119 (dos filas), 118, 120, 123 | — | 121, 124 |
| REA | 13 | Todas: 153, 125, 126 (dos filas), 128, 131, 134 y 130, 130, 132 (tres filas), 129, 136 | — | 127, 133 |
| COT | 11 | 153, 143, 141, 140, 137, 144, 145, 138 y 140, 142, 147 | Columna «Mejor Precio» (12, línea 449) | 139 |
| CONFIRM | 5 | 152 (dos filas), 151 (dos filas) | Fila (e) de los 13 modales (12, línea 476) | — |
| Transversales | — | — | — | 148, 149, 150, 156, 157 |
| **Total** | **120** | **117** | **3** | **41** (más 4 que eran observaciones) |

### 6.3 Lo que ya no aplica, total o parcialmente

1. **ALV, «Confirmar Actualización» en la pantalla principal** (12, línea 190). Con C1 la confirmación pertenece a `ART-MKP-ALV-0001`; lo incorrecto es que ESP-0015 cite `ALV-0002`, y eso ya lo recoge 069.
2. **COT, columna «Mejor Precio»** (12, línea 449). ESP-0050 ya pide «DESTACAR EN comparativa EL PRECIO MAS BAJO POR FILA» (`03-especificaciones.md:947`). La columna es una forma de destacarlo y no se considera una función nueva.
3. **CONFIRM, fila (e) de los 13 modales** (12, línea 476). El propio 12 la marcaba como repetida. Con C1, cada modal de baja forma parte de la pantalla 0001 de su módulo, y doce HTML ya lo incluyen.
4. **Parcial: la parte de las 13 filas (e)** que dice que `<MOD>-MDL-CONFIRMAR-ELIMINACION`, `<MOD>-TXT-MSJ` y `<MOD>-BTN-CONFIRMAR-SI/NO` «quedan sin pantalla» (por ejemplo, 12, líneas 163 y 191). Desde el PR #144, esos componentes existen en los HTML de INV, DOC, ALV, PAG, REP, DEV, RES, USR, PRV, LOT, REA y COT, ocultos y sin mostrarse en el PNG. Siguen faltando en VEN (026) y falta `REA-BTN-CONFIRMAR-NO` (135). Sus textos difieren de las ESP (150). Lo que sigue vigente de esas filas es la cita a mockups 0002 a 0004 inexistentes.
5. **Parcial: DEV, «ORD-2026-014» y «3 líneas»** (12, línea 278). El número de líneas se calcula contando `TBL_DETALLE_DEVOLUCION`, así que no le falta una columna. Sigue vigente el formato del código (155).
6. **Matiz: DOC, nombre `DOC-MDL-PREVISUALIZAR-COMPROBANTE`** (12, línea 155). El argumento de que «0001 a 0004 ya están ocupados» deja de valer con C1, pero el hallazgo se mantiene por otro motivo: el modal es parte de `ART-MKP-DOC-0001` (059).
7. **Matiz: REA, «Cantidad Sugerida»** (12, línea 414). La «cantidad pendiente» que no tiene columna la menciona ILA-0050, paso 9 (`02-ilaciones.md:1040`), no ESP-0050 (`03-especificaciones.md:947`), que no habla de cantidad (130).

### 6.4 Nuevos en este informe, por motivo

- **Diferencias entre PNG y HTML:** 006, 028, 029, 046 y 058.
- **IDs repetidos que el 12 no listó:** 037 (`INV-TBL-PRODUCTOS`), 048 (`DOC-LBL-MONTO-TOTAL`), 049 (tipo y número de DOC), 064 (`ALV-LBL-FECHA-VENCIMIENTO`) y 016 (`VEN-NUM-CANTIDAD-VENTA`).
- **Lo que la ESP pide y no se ve, sin fila en el 12:** 011 (área de advertencias de ESP-0001), 014 (lote en el detalle de la venta), 051 (filtro de fecha de DOC), 066 (medicamento en la tabla de ALV) y 093 (motivo en la lista de DEV).
- **Rótulos y valores que contradicen la ESP o una decisión:** 068, 085, 094, 101, 106, 121, 133 y 156.
- **Decisiones de producto que el mockup hace visibles:** 004 (semántica de «Añadir»), 022 (presentación, PEN-010) y 032 (rótulo «Carrito de compras»).
- **Permisos (g):** 033, 045, 062, 070, 076, 104, 124 y 157.
- **Saltos y arquitectura (f, h):** 030, 031 y 127. 134 ya estaba en el 12 como (d).
- **Selección del proveedor por medicamento en COT:** 139.
- **Guía y documentos del catálogo:** 148 (guía y trazabilidad aún con 0001 a 0004), 149 (guía sin REA ni COT) y 150 (texto de los modales).

## 7. Preguntas

### 7.1 Para la dueña

| Código | Pregunta | Hallazgos |
| --- | --- | --- |
| P-01 | El visor 3D y el botón «Ver producto (3D)» (en Ventas y en Inventario), ¿son una función que usted necesita o solo ilustraban la idea? | 020, 038 |
| P-02 | ¿Qué datos quiere ver al pulsar «Observar información del producto (texto)»? | 019 |
| P-03 | La «Ubicación» de la pantalla de ventas, ¿es el estante o lugar donde está el producto? ¿Cambia según la remesa o es la misma para todo el medicamento? ¿Quién la registra? | 008 |
| P-04 | ¿Qué espera ver en «Indicaciones» y «Sugerencias»? ¿Le basta con que ahí aparezcan las advertencias de restricción y los bloqueos? | 010, 011 |
| P-05 | El botón «Añadir» de cada tarjeta, ¿agrega directamente una unidad a la venta o solo elige el producto para escoger lote y cantidad? | 004 |
| P-06 | ¿Qué código identifica a los productos en la venta («Código #»)? ¿Es el código de barras de la caja? | 015 |
| P-07 | ¿Por qué categoría quiere filtrar los productos: acción terapéutica, genérico o comercial, u otra? ¿La presentación (500 mg, 20 tabletas) debe registrarse aparte del nombre? Se responde junto con PEN-010. | 021, 022, 040 |
| P-08 | ¿Está de acuerdo con rotular «Detalle de la venta» lo que hoy dice «Carrito de compras»? | 032 |
| P-09 | Comprobantes: ¿necesita la vista previa con los datos del cliente y el IGV? ¿El sistema debe enviar el comprobante a SUNAT? Una vez emitido, ¿se puede modificar? Se responde junto con PEN-003. | 055, 056 |
| P-10 | Reportes: ¿los tipos son «Ventas por fecha», «Ventas por producto» y «Consolidado general»? Se responde junto con PEN-001. | 084 |
| P-11 | Reabastecimiento: ¿la prioridad tiene dos niveles (Normal y Urgente) o tres (Crítico, Alto y Medio)? La cantidad a pedir, ¿la escribe usted o la calcula el sistema? ¿Quiere generar la lista a mano, además del proceso automático? | 125, 130, 131 |
| P-12 | Cotizaciones: ¿necesita registrar el tiempo de entrega de cada proveedor? | 144 |
| P-13 | Restricciones: los medicamentos que exigen receta o mayoría de edad, ¿se registran como restricciones de venta? Se responde junto con PEN-025. | 102 |

### 7.2 Para la líder técnica

| Código | Pregunta | Hallazgos |
| --- | --- | --- |
| L-01 | ¿El catálogo de Ventas muestra solo medicamentos vendibles, con `ILotes.medicamentosVendibles()` documentada en el insumo de interfaces, o se pretende mostrar todo el inventario (lo que exigiría VEN → INV, que no se propone)? | 030 |
| L-02 | ¿Se adopta la interpretación de EDU-0004 según la cual «en el módulo de ventas» significa mostrar el bloqueo al vender y el CRUD de alertas queda en ALV? | 031 |
| L-03 | En la pantalla única, ¿se añade el botón `<MOD>-BTN-CONFIRMAR-ACTUALIZACION` que piden las ESP (C4), o se simplifican las ESP para que el botón «Actualizar» o «Guardar» confirme? | 025, 043, 060, 074, 086, 092, 100, 109, 115, 122, 135, 146 |
| L-04 | ¿Qué significa el asterisco en las etiquetas? ¿`ART-MKP-CONFIRM-0001` se declara plantilla común? | 152, 153 |
| L-05 | ¿Los códigos visibles (PROD-, VEN-AAAA-, ORD-AAAA-) son el identificador con formato o un dato nuevo? | 155 |
| L-06 | ¿Se permite mostrar con tilde un literal que se guarda sin tilde? | 156 |

### 7.3 Pendientes que habría que registrar

AGENTS.md §7 pide registrar como `PEN-NNN` lo que requiere decisión, pero este encargo solo permite crear este informe. Se dejan propuestos, sin registrar: la ubicación del producto (P-03); los tres niveles de prioridad y la cantidad sugerida de reabastecimiento (P-11), que además corrige ILA-0050; la vista previa y el envío a SUNAT (P-09), a sumar a PEN-003; la interpretación de EDU-0004 (L-02); y la confirmación de actualización en la pantalla única (L-03).
