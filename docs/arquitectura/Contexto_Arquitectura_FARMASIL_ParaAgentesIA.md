# FARMASIL_APP — Contexto de Arquitectura para Agentes de IA

> Léeme primero, antes de tocar código. Este documento resume la arquitectura genérica ya cerrada del sistema (13 módulos) en un formato pensado para que un agente de codificación entienda rápido los límites del sistema, qué depende de qué, y a qué tabla mapea cada módulo — sin tener que leer el catálogo de requisitos completo (`catalogo/01-educciones.md` a `04-trazabilidad.md`, `modelo/05-diccionario-datos.md`) para empezar a trabajar. Si necesitas el detalle fino de una operación concreta (reglas de negocio, pseudocódigo, validaciones), ve a la especificación (`ESP-XXXX`) citada en cada módulo — este documento no las reemplaza, solo te dice dónde buscar.
>
> **Estado:** arquitectura genérica cerrada y verificada contra el diagrama visual (`docs/arquitectura/ArquitecturaGenerica_Nivel1.png` y `_Nivel2.png`). Fuente: `catalogo/01-educciones.md`, `02-ilaciones.md`, `03-especificaciones.md`, `modelo/05-diccionario-datos.md` v05.00, `diseno/07-matriz-permisos.md`.

---

## 1. Regla de oro para implementar: respeta las dependencias de este documento

Cada módulo de la lista de abajo es un límite de responsabilidad. **Un módulo solo debe invocar/consultar código de los módulos que aparecen en su fila "Depende de"** (Sección 3). Si al implementar una funcionalidad sientes que necesitas que el módulo A llame directamente a un módulo que no está en su lista de dependencias, detente: o la funcionalidad pertenece a otro módulo, o falta una dependencia real que no se documentó — en ambos casos, señálalo en vez de improvisar el acoplamiento.

---

## 2. Los 13 módulos — responsabilidad, tabla(s) y acceso

| # | Módulo | Responsabilidad (en una línea) | Tabla(s) de BD | Accede |
| --- | --- | --- | --- | --- |
| 1 | **Ventas** (VEN) | Registrar una venta y su detalle de líneas | `TBL_REGISTRO_VENTAS`, `TBL_DETALLE_VENTAS` | Dueña + Técnica |
| 2 | **Inventario** (INV) | Catálogo de medicamentos (nombre, precio, estado, stock mínimo) | `TBL_PRODUCTOS` | Dueña (Técnica solo lectura) |
| 3 | **Documentación Tributaria** (DOC) | Emitir comprobante fiscal de una venta | `TBL_COMPROBANTES_TRIBUTARIOS` | Dueña + Técnica |
| 4 | **Alertas de Vencimiento** (ALV) | Avisar y bloquear remesas próximas a vencer | `TBL_ALERTAS_VENCIMIENTO`, `TBL_PRODUCTOS_RETIRADOS` | Dueña + Técnica |
| 5 | **Métodos de Pago** (PAG) | Catálogo de formas de pago aceptadas | `TBL_METODOS_PAGO` | Dueña (Técnica solo lectura) |
| 6 | **Reportes de Ventas** (REP) | Consolidar ventas por rango de fechas/producto | `TBL_REPORTES_VENTAS` | Dueña |
| 7 | **Devoluciones a Proveedores** (DEV) | Gestionar la devolución de una remesa a su proveedor | `TBL_ORDENES_DEVOLUCION`, `TBL_DETALLE_DEVOLUCION` | Dueña |
| 8 | **Restricciones de Venta** (RES) | Bloquear/advertir la venta de un medicamento restringido | `TBL_RESTRICCIONES_VENTA` | Dueña (Técnica solo lectura) |
| 9 | **Usuarios** (USR) | Autenticación, roles, autoría de operaciones | `TBL_USUARIOS` | Dueña — **ver §4, es transversal** |
| 10 | **Proveedores** (PRV) | Catálogo de proveedores (RUC, razón social, contacto) | `TBL_PROVEEDORES` | Dueña |
| 11 | **Lotes** (LOT) | Remesas: stock real, vencimiento, proveedor real | `TBL_LOTES` | Dueña (Técnica solo lectura) |
| 12 | **Reabastecimiento** (REA) | Generar lista de medicamentos bajo su stock mínimo | `TBL_LISTA_REABASTECIMIENTO` | Dueña exclusivo |
| 13 | **Comparación de Precios de Proveedores** (COT) | Comparar cotizaciones de proveedores por medicamento, exportable a CSV | `TBL_COTIZACIONES_PROVEEDOR` | Dueña exclusivo |

---

## 3. Grafo de dependencias (completo, 18 aristas)

`A → B` significa: el código de A puede invocar/consultar servicios de B. No existen aristas fuera de esta lista — si tu implementación necesita una, repórtalo en vez de crearla.

```
Ventas          → Métodos de Pago, Restricciones de Venta, Lotes
Documentación   → Ventas
Alertas Venc.   → Devoluciones, Lotes
Reportes        → Ventas
Devoluciones    → Proveedores, Lotes
Restricciones   → Inventario
Inventario      → Proveedores
Lotes           → Inventario, Proveedores
Reabastecimiento → Inventario, Lotes
Comparación Precios → Inventario, Proveedores, Reabastecimiento
```

**Capa base (nadie depende de nada, otros dependen de ellos):** Métodos de Pago, Usuarios, Proveedores.
**Capa terminal (no tienen dependientes, son "vista de salida"):** Documentación Tributaria, Alertas de Vencimiento, Reportes de Ventas, Comparación de Precios.

---

## 4. Usuarios: no es una dependencia de negocio, es autenticación/auditoría

`Usuarios` **no aparece en el grafo de la Sección 3** a propósito. Su relación con otros módulos es transversal: todo módulo que registra una acción de un usuario lleva un campo `id_usuario` de auditoría (quién hizo la operación), pero ningún módulo le "pregunta" a Usuarios una regla de negocio para poder operar. Al implementar:

- No crees un servicio `UsuarioService` que otros módulos consulten como parte de su lógica de negocio.
- Sí usa el usuario de la sesión activa (autenticación/autorización, válido para **todas** las pantallas, no solo las que tienen FK) para rellenar el campo `id_usuario` donde la tabla lo tenga: `TBL_REGISTRO_VENTAS`, `TBL_ORDENES_DEVOLUCION`, `TBL_COTIZACIONES_PROVEEDOR`.
- `TBL_REPORTES_VENTAS` **no tiene `id_usuario` ni ninguna otra FK**. El `usuario_responsable` que exige EDU-0010 es un valor calculado: ESP-0021 lo obtiene al generar el reporte uniendo `TBL_REGISTRO_VENTAS.id_usuario` con `TBL_USUARIOS`, y solo existe dentro del archivo de salida. No agregues esa columna a la tabla.
- Control de acceso por rol (Dueña vs. Técnica, columna "Accede" de la Sección 2) es una preocupación transversal de autorización (RNF-0006), no de dominio — impleméntalo como middleware/guard, no como dependencia entre módulos.

---

## 5. Cuatro decisiones de diseño ya cerradas — no las reabras

1. **Ventas NO accede a Inventario directamente.** La cadena es `Ventas → Lotes → Inventario`. `TBL_DETALLE_VENTAS.id_lote` es FK a `TBL_LOTES`, no a `TBL_PRODUCTOS`. Al descontar stock en una venta, hazlo contra la remesa (`TBL_LOTES.stock_actual`), no contra el producto.
2. **Lotes es su propia entidad, con proveedor propio.** `TBL_LOTES.id_proveedor` puede ser distinto del proveedor habitual del producto en `TBL_PRODUCTOS`. No asumas que son el mismo valor.
3. **La selección de remesa en una venta sigue criterio FEFO** (primero en vencer, primero en salir) — es responsabilidad de Lotes exponerlo, no de Ventas calcularlo.
4. **Las bajas condicionales no consultan tablas de otros módulos.** En ESP-0008, ESP-0020, ESP-0036, ESP-0040 y ESP-0044 la elección entre borrado físico y baja lógica la decide la base de datos: intenta el `DELETE` dentro de la transacción con un punto de guardado; si SQL Server lo rechaza por una clave foránea (error 547), revierte al punto de guardado y aplica la baja lógica. No implementes la verificación enumerando tablas: crearía dependencias inversas que la arquitectura no tiene (ver RG-011 en `riesgos/10-gestion-riesgos.md`).

---

## 6. Los 2 módulos nuevos (Entrevista 3) — qué hacen exactamente

- **Reabastecimiento:** proceso que compara `SUMA(TBL_LOTES.stock_actual)` agrupado por producto contra `TBL_PRODUCTOS.stock_minimo`, y genera `TBL_LISTA_REABASTECIMIENTO` con lo que está bajo mínimo. No es una acción de un usuario puntual (sin `id_usuario`) — modélalo como un proceso programado o un cálculo bajo demanda, no como un formulario CRUD estándar.
- **Comparación de Precios de Proveedores:** registra cotizaciones (`TBL_COTIZACIONES_PROVEEDOR`) de distintos proveedores para los medicamentos que están en la lista de reabastecimiento, y exporta el comparativo a CSV filtrado por proveedor. Sí lleva `id_usuario` (acción manual de la Dueña).
- Ambos son **exclusivos de la Dueña** (ningún endpoint/pantalla de Técnica debe exponerlos).

---

## 7. Mapa del repositorio (dónde está cada cosa)

| Qué necesitas | Dónde está |
| --- | --- |
| Requisitos funcionales (qué debe hacer el sistema) | `catalogo/01-educciones.md` → `02-ilaciones.md` → `03-especificaciones.md` (pseudocódigo + validaciones por operación) |
| Trazabilidad EDU↔ILA↔ESP | `catalogo/04-trazabilidad.md` |
| Esquema de base de datos completo (tablas, FKs, tipos) | `modelo/05-diccionario-datos.md` |
| Quién puede hacer qué (permisos por actor) | `diseno/07-matriz-permisos.md` |
| Requisitos no funcionales (RNF-0001 a RNF-0010: operación local, un solo equipo, control de acceso, tiempos de respuesta) | `catalogo/08-requisitos-no-funcionales.md` |
| Diagrama de arquitectura genérica (visual) | `docs/arquitectura/ArquitecturaGenerica_Nivel1.png` (vista simplificada) y `ArquitecturaGenerica_Nivel2.png` (13 componentes con interfaces) |
| Matriz de dependencias e interfaces (texto, la fuente de este documento) | `docs/arquitectura/Insumo_Matriz_Interfaces_ArquitecturaGenerica_v2.md` |
| Mockups de pantallas | `diseno/Mockups/` |
| Diagramas de casos de uso | `docs/diagramas/` |
| Riesgos identificados y su estado | `riesgos/10-gestion-riesgos.md` |
| Cuestiones abiertas y decisiones ya tomadas (no reabrir sin revisar) | `seguimiento/11-registro-pendientes.md` |
| Reglas para agentes de IA trabajando en este repo (commits, git add, atribución) | `AGENTS.md` (raíz del repo) |

---

## 8. Fuera de alcance de este documento

No cubre: atributos/métodos de clases, estados de entidades (`estado_lote`, `estado_producto`, `estado_orden`), diagramas de secuencia de las operaciones, ni decisiones de UI. Para eso, ve a la especificación (`ESP-XXXX`) del módulo correspondiente o a la vista lógica (diagrama de clases) cuando esté disponible.
