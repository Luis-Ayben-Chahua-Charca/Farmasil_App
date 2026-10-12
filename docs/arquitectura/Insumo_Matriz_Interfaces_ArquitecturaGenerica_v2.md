# Insumo — Matriz de Módulos e Interfaces para el Rediseño de la Arquitectura Genérica (v2.00, 13 módulos)

> Documento de trabajo, no el entregable final. Es el insumo textual (matriz + explicación de interfaces) para que corrijas manualmente el diagrama de componentes de `ArquitecturaGenerica.md` / FigJam, sin arrastrar la deuda técnica de la versión anterior (11 módulos, previa a la integración de la Entrevista 3). No contiene diagramas: solo la matriz de dependencias y el detalle de cada interfaz, en el mismo formato ya usado en este chat para la primera versión del diagrama.
>
> **Fecha:** 02/10/2026. **Base:** `01-educciones.md`, `02-ilaciones.md`, `03-especificaciones.md`, `DiccionarioDatos.md` v05.00, `07-matriz-permisos.md` — ya actualizados con los módulos 12 (Reabastecimiento) y 13 (Comparación de Precios de Proveedores) derivados de la Entrevista 3.

---

## 1. Qué cambió respecto a la versión anterior del diagrama (deuda técnica a corregir)

Si tu diagrama en FigJam todavía refleja la versión de 11 módulos, estos son los puntos concretos que quedaron obsoletos y que esta matriz corrige:

1. **Ventas ya no depende directamente de Inventario.** Desde que `TBL_DETALLE_VENTAS.id_lote` apunta a `TBL_LOTES` (no a `TBL_PRODUCTOS`), la dependencia real es **Ventas → Lotes → Inventario**, no una flecha directa Ventas→Inventario. Si tu diagrama tiene esa flecha directa, bórrala; es la arista ya resuelta en la conversación anterior.
2. **Lotes es un componente propio, no un atributo de Inventario.** A partir del Diccionario de Datos v04.00, `TBL_LOTES` es su propia tabla con su propio módulo (EDU-0015, Módulo 11). Si tu diagrama todavía trata "vencimiento y stock" como parte del componente Inventario, está desactualizado.
3. **Usuarios y Proveedores deben aparecer como componentes explícitos, aunque Usuarios se trata como aspecto transversal.** Ambos tienen módulo propio (EDU-0013, EDU-0014) con sus cuatro operaciones CRUD. Usuarios, sin embargo, **no se dibuja con flechas de dependencia de negocio**: su relación con Ventas, Reportes, Devoluciones y Cotizaciones es de autoría/control de acceso (RNF-0006), no de lógica de negocio — se representa como aspecto transversal (ver Sección 3), igual que ya se decidió para la primera versión del diagrama.
4. **Dos módulos nuevos: Reabastecimiento (12) y Comparación de Precios de Proveedores (13).** Ninguno existía en el diagrama anterior. Ambos son exclusivos de ACT-0001 (ver `07-matriz-permisos.md`) y traen dependencias nuevas hacia Inventario, Lotes y Proveedores (Sección 2 y 4).
5. **Lotes ahora también depende de Proveedores.** `TBL_LOTES.id_proveedor` (v05.00) es una arista nueva que no existía: antes Lotes solo dependía de Inventario.

---

## 2. Los 13 módulos (componente ↔ educción ↔ tabla(s) propia(s))

| # | Componente | Educción | Tabla(s) que escribe | Rol de acceso (`07-matriz-permisos.md`) |
| --- | --- | --- | --- | --- |
| 1 | **Ventas** (VEN) | EDU-0001 | `TBL_REGISTRO_VENTAS`, `TBL_DETALLE_VENTAS` | A, T |
| 2 | **Inventario** (INV) | EDU-0002 | `TBL_PRODUCTOS` | A (T solo lectura) |
| 3 | **Documentación Tributaria** (DOC) | EDU-0003 | `TBL_COMPROBANTES_TRIBUTARIOS` | A, T |
| 4 | **Alertas de Vencimiento** (ALV) | EDU-0004 | `TBL_ALERTAS_VENCIMIENTO`, `TBL_PRODUCTOS_RETIRADOS` | A, T |
| 5 | **Métodos de Pago** (PAG) | EDU-0009 | `TBL_METODOS_PAGO` | A (T solo lectura) |
| 6 | **Reportes de Ventas** (REP) | EDU-0010 | `TBL_REPORTES_VENTAS` | A |
| 7 | **Devoluciones a Proveedores** (DEV) | EDU-0011 | `TBL_ORDENES_DEVOLUCION`, `TBL_DETALLE_DEVOLUCION` | A |
| 8 | **Restricciones de Venta** (RES) | EDU-0012 | `TBL_RESTRICCIONES_VENTA` | A (T solo lectura) |
| 9 | **Usuarios** (USR) | EDU-0013 | `TBL_USUARIOS` | A — **transversal**, ver Sección 3 |
| 10 | **Proveedores** (PRV) | EDU-0014 | `TBL_PROVEEDORES` | A |
| 11 | **Lotes** (LOT) | EDU-0015 | `TBL_LOTES` | A (T solo lectura) |
| 12 | **Reabastecimiento** (REA) | EDU-0016 | `TBL_LISTA_REABASTECIMIENTO` | A exclusivo |
| 13 | **Comparación de Precios de Proveedores** (COT) | EDU-0017 | `TBL_COTIZACIONES_PROVEEDOR` | A exclusivo |

---

## 3. Usuarios como aspecto transversal (no como nodo de dependencia regular)

Igual que RNF-0006 (control de acceso) se trató como aspecto transversal en la primera versión del diagrama — no como un componente más en la cadena de dependencias — **Usuarios se mantiene fuera de las flechas de dependencia de negocio**, por dos razones que ya se habían fijado:

- Su relación con otros módulos es de **autoría** (`id_usuario` como FK en `TBL_REGISTRO_VENTAS`, `TBL_ORDENES_DEVOLUCION` y, ahora, `TBL_COTIZACIONES_PROVEEDOR`), no de lógica de negocio: ningún módulo necesita "preguntarle" a Usuarios una regla de negocio para poder operar, solo registra quién hizo la operación.
- Dibujarlo como nodo regular obligaría a trazar flechas desde casi todos los módulos hacia Usuarios, saturando el diagrama con una relación que es la misma en todos los casos (sesión activa + autoría), exactamente el motivo por el que RNF-0006 ya se representa como aspecto transversal y no como componente en cadena.

En la matriz de la Sección 4, las celdas donde un módulo usa Usuarios solo para autoría están marcadas con **`T`** (transversal), no con `1`, para que al dibujar el diagrama se representen con la notación que ya usaste para el aspecto de seguridad (por ejemplo, una nota o un ícono de candado adosado al módulo, no una flecha de dependencia con lollipop).

---

## 4. Matriz de dependencias (13×13)

Lectura: la fila **depende de** la columna (necesita esa interfaz para completar al menos una de sus operaciones). `1` = dependencia de negocio regular. `T` = dependencia transversal (autoría/control de acceso vía Usuarios, no lógica de negocio). `0` = sin dependencia directa.

| ↓ depende de → | VEN | INV | DOC | ALV | PAG | REP | DEV | RES | USR | PRV | LOT | REA | COT |
| --- | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| **VEN** | — | 0 | 0 | 0 | 1 | 0 | 0 | 1 | T | 0 | 1 | 0 | 0 |
| **INV** | 0 | — | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 0 |
| **DOC** | 1 | 0 | — | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
| **ALV** | 0 | 0 | 0 | — | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 0 |
| **PAG** | 0 | 0 | 0 | 0 | — | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
| **REP** | 1 | 0 | 0 | 0 | 0 | — | 0 | 0 | T | 0 | 0 | 0 | 0 |
| **DEV** | 0 | 0 | 0 | 0 | 0 | 0 | — | 0 | T | 1 | 1 | 0 | 0 |
| **RES** | 0 | 1 | 0 | 0 | 0 | 0 | 0 | — | 0 | 0 | 0 | 0 | 0 |
| **USR** | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | — | 0 | 0 | 0 | 0 |
| **PRV** | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | — | 0 | 0 | 0 |
| **LOT** | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | — | 0 | 0 |
| **REA** | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | — | 0 |
| **COT** | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | T | 1 | 0 | 1 | — |

**Lectura rápida de filas en cero:** Métodos de Pago, Usuarios y Proveedores son los tres componentes **sin dependencias salientes** (nodos hoja / catálogo puro): no necesitan de ningún otro módulo para completar sus propias operaciones. Son, junto con Inventario (que solo depende de Proveedores), la capa más estable del sistema — la que menos cambia cuando se agrega un módulo nuevo, como de hecho ocurrió con Reabastecimiento y Cotizaciones, que se apoyaron en Inventario, Lotes y Proveedores sin que estos tres tuvieran que cambiar su propio comportamiento.

---

## 5. Explicación de cada interfaz (provista por la columna, requerida por la fila)

Cada fila es una interfaz nombrada con el prefijo `I` seguido del nombre del módulo que la provee, siguiendo la convención ya usada (`IInventario`, `ILotes`, etc.). Para cada una: qué operaciones expone, quién la requiere y por qué (con el ESP que lo sustenta), y el nivel de certeza de la arista (todas estas son de **confianza alta**: están extraídas de FK declaradas en el diccionario de datos o de consultas explícitas en el pseudocódigo de las especificaciones, no son supuestas).

### IPagos (provista por Métodos de Pago)
- **Expone:** consulta de métodos de pago activos.
- **Requerida por:** Ventas (ESP-0001: `id_metodo_pago` como FK en `TBL_REGISTRO_VENTAS`, y el combo `VEN-CMB-METODO-PAGO` se alimenta de `TBL_METODOS_PAGO` con `estado='Activo'`).
- **Confianza:** alta.

### IRestricciones (provista por Restricciones de Venta)
- **Expone:** consulta de restricciones activas por medicamento (bloqueante/informativa).
- **Requerida por:** Ventas (ESP-0001: `CONSULTAR TBL_RESTRICCIONES_VENTA DONDE id_producto = producto Y estado_alerta = 'Activo'` antes de agregar la línea de venta).
- **Confianza:** alta.

### ILotes (provista por Lotes)
- **Expone:** consulta de remesas disponibles (criterio FEFO), descuento/restitución de `stock_actual`, cambio de `estado_lote`.
- **Requerida por:**
  - Ventas (ESP-0001: selección de remesa por FEFO y descuento de stock al confirmar la venta).
  - Alertas de Vencimiento (ESP-0014: bloquea la remesa cambiando `estado_lote` a `'Bloqueado por devolucion'`; ESP-0016 la restituye a `'Disponible'`).
  - Devoluciones (ESP-0025: valida `lote.id_proveedor = proveedor` y descuenta `stock_actual` al completar la orden en ESP-0027).
  - Reabastecimiento (proceso periódico: compara el stock consolidado — `SUMA(TBL_LOTES.stock_actual)` por producto — contra `TBL_PRODUCTOS.stock_minimo`).
- **Confianza:** alta.

### IVentas (provista por Ventas)
- **Expone:** consulta de ventas y su detalle, por fecha/producto.
- **Requerida por:** Documentación Tributaria (ESP-0009: el comprobante recupera `monto_total` y valida que la venta no tenga ya un comprobante emitido, vía `id_venta` FK) y Reportes de Ventas (ESP-0021: consolidación de `TBL_REGISTRO_VENTAS` UNIENDO `TBL_DETALLE_VENTAS` por rango de fechas).
- **Confianza:** alta.
- **Nota:** esta es la arista que **reemplaza** a la antigua Ventas→Inventario directa: Documentación y Reportes dependen de Ventas, no de Inventario, y Ventas a su vez depende de Lotes, no de Inventario directamente (ver Sección 1, punto 1).

### IInventario (provista por Inventario)
- **Expone:** consulta/validación de medicamentos del catálogo (nombre, precio, estado, `stock_minimo`).
- **Requerida por:**
  - Restricciones de Venta (ESP-0029: `id_producto` FK en `TBL_RESTRICCIONES_VENTA`).
  - Lotes (ESP-0041: `id_producto` FK en `TBL_LOTES`, cada remesa pertenece a un medicamento del catálogo).
  - Reabastecimiento (ESP-0045: `id_producto` FK en `TBL_LISTA_REABASTECIMIENTO`, y lectura de `stock_minimo` para la generación automática).
  - Comparación de Precios (ESP-0049: `id_producto` FK en `TBL_COTIZACIONES_PROVEEDOR`).
- **Confianza:** alta.

### IProveedores (provista por Proveedores)
- **Expone:** consulta de proveedores activos (RUC, razón social, teléfono).
- **Requerida por:**
  - Inventario (ESP-0005: `id_proveedor` FK en `TBL_PRODUCTOS`, proveedor habitual del catálogo).
  - Lotes (ESP-0041, v02.00: `id_proveedor` FK en `TBL_LOTES`, proveedor real de la remesa — arista **nueva** en v05.00).
  - Devoluciones (ESP-0025: `id_proveedor` FK en `TBL_ORDENES_DEVOLUCION`, y validación contra `lote.id_proveedor`).
  - Comparación de Precios (ESP-0049: `id_proveedor` FK en `TBL_COTIZACIONES_PROVEEDOR`).
- **Confianza:** alta.

### IDevoluciones (provista por Devoluciones)
- **Expone:** consulta de órdenes de devolución pendientes por remesa.
- **Requerida por:** Alertas de Vencimiento (ESP-0016: `VALIDAR NO EXISTE(TBL_DETALLE_DEVOLUCION, id_lote = alerta.lote Y orden.estado_orden = 'Pendiente')` antes de permitir eliminar una alerta).
- **Confianza:** alta.
- **Nota de arquitectura:** esta es la única arista donde un módulo "más viejo" en el orden del catálogo (Alertas, EDU-0004) depende de uno "más nuevo" (Devoluciones, EDU-0011). No es un error: el orden de las educciones no implica orden de dependencia arquitectónica.

### IReabastecimiento (provista por Reabastecimiento)
- **Expone:** consulta de la lista de medicamentos pendientes de cotizar/reabastecer, con prioridad.
- **Requerida por:** Comparación de Precios (ESP-0050: la exportación CSV por proveedor une `TBL_LISTA_REABASTECIMIENTO` con la comparativa de cotizaciones para incluir solo los medicamentos pendientes).
- **Confianza:** alta.

### IUsuarios (provista por Usuarios) — transversal
- **Expone:** verificación de sesión activa y rol, registro de autoría (`id_usuario`).
- **Requerida transversalmente por:** Ventas (`TBL_REGISTRO_VENTAS.id_usuario`), Reportes de Ventas (consolidación por `usuario_responsable`), Devoluciones (`TBL_ORDENES_DEVOLUCION.id_usuario`) y Comparación de Precios (`TBL_COTIZACIONES_PROVEEDOR.id_usuario`).
- **Confianza:** alta (las cuatro FK están declaradas en el diccionario de datos).
- **Representación sugerida en el diagrama:** no dibujar como flecha de dependencia con lollipop; usar la misma convención visual que RNF-0006 (aspecto transversal adosado a los módulos que aplican, por ejemplo un ícono o un marco punteado alrededor de todo el diagrama), consistente con la Sección 3.

### IDocumentacion, IAlertasVencimiento, IReportes — sin consumidores
Documentación Tributaria, Alertas de Vencimiento y Reportes de Ventas no son requeridas por ningún otro módulo (filas de sus columnas en 0, salvo la celda transversal de Usuarios/Reportes ya contada). Son módulos **terminales**: consumen de otros pero nadie consume de ellos. Esto es coherente con su naturaleza: son los tres módulos más cercanos a "vista de salida" del sistema (comprobante fiscal, alerta operativa, reporte gerencial), no infraestructura que otros módulos necesiten para funcionar.

---

## 6. Resumen por capas (para ordenar el diagrama visualmente)

Agrupación informal según cuántos módulos dependen de cada uno (no es un artefacto nuevo, solo una guía de disposición espacial para el diagrama):

- **Capa base (catálogo puro, 0 dependencias salientes):** Métodos de Pago, Usuarios*, Proveedores. (*Usuarios es base pero transversal, no se posiciona en la cadena vertical.)
- **Capa de inventario físico:** Inventario (depende solo de Proveedores) → Lotes (depende de Inventario y Proveedores).
- **Capa operativa diaria:** Ventas (depende de Pagos, Restricciones, Lotes), Restricciones de Venta (depende de Inventario).
- **Capa derivada de la operación diaria:** Documentación Tributaria y Reportes de Ventas (ambos dependen de Ventas).
- **Capa de gestión de proveedores:** Devoluciones (depende de Lotes, Proveedores), Alertas de Vencimiento (depende de Lotes, Devoluciones).
- **Capa nueva (Entrevista 3):** Reabastecimiento (depende de Inventario, Lotes) → Comparación de Precios de Proveedores (depende de Inventario, Proveedores, Reabastecimiento).

Esta es la misma lógica de capas por dependencia (no por responsabilidad técnica, que es la propuesta alternativa de "arquitectura por capas" ya registrada como pendiente en `Insumo_Diagramas_Arquitectura_FARMASIL.md`, punto A.3) que ya se había usado para ordenar los 11 módulos originales; se extiende aquí a los 13 sin alterar el criterio.

---

## 7. Siguiente paso manual

Con esta matriz y el detalle de interfaces, el único trabajo que queda es visual: abrir el diagrama de componentes en FigJam y, por cada módulo nuevo (Reabastecimiento, Comparación de Precios) y cada arista corregida (Sección 1), ajustar las cajas y los conectores de bola-enchufe. Esta matriz es el insumo textual completo para ese ajuste — no requiere volver a derivar ninguna dependencia desde el catálogo de requisitos.
