# FARMASIL_APP — Contexto de Arquitectura para Agentes de IA

> Léeme primero, antes de tocar código. Este documento resume la arquitectura genérica ya cerrada del sistema (13 módulos) en un formato pensado para que un agente de codificación entienda rápido los límites del sistema, qué depende de qué, y a qué tabla mapea cada módulo — sin tener que leer el catálogo de requisitos completo (`catalogo/01-educciones.md` a `04-trazabilidad.md`, `modelo/05-diccionario-datos.md`) para empezar a trabajar. Si necesitas el detalle fino de una operación concreta (reglas de negocio, pseudocódigo, validaciones), ve a la especificación (`ESP-XXXX`) citada en cada módulo — este documento no las reemplaza, solo te dice dónde buscar.
>
> **Estado:** arquitectura genérica cerrada y verificada contra el diagrama visual (`docs/arquitectura/ArquitecturaGenerica_Nivel1.png` y `_Nivel2.png`). Fuente: `catalogo/01-educciones.md`, `02-ilaciones.md`, `03-especificaciones.md`, `modelo/05-diccionario-datos.md` v10.00, `diseno/07-matriz-permisos.md`.

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

## 5. Decisiones de diseño ya cerradas — no las reabras

1. **Ventas NO accede a Inventario directamente.** La cadena es `Ventas → Lotes → Inventario`. `TBL_DETALLE_VENTAS.id_lote` es FK a `TBL_LOTES`, no a `TBL_PRODUCTOS`. Al descontar stock en una venta, hazlo contra la remesa (`TBL_LOTES.stock_actual`), no contra el producto.
2. **Lotes es su propia entidad, con proveedor propio.** `TBL_LOTES.id_proveedor` puede ser distinto del proveedor habitual del producto en `TBL_PRODUCTOS`. No asumas que son el mismo valor.
3. **La selección de remesa en una venta sigue criterio FEFO** (primero en vencer, primero en salir) — es responsabilidad de Lotes exponerlo, no de Ventas calcularlo.
4. **Las bajas condicionales no consultan tablas de otros módulos.** En ESP-0008, ESP-0020, ESP-0036, ESP-0040 y ESP-0044 la elección entre borrado físico y baja lógica la decide la base de datos (DEC-009, DEC-029; subactividad VP-B). La operación corre con `XACT_ABORT OFF`, que se deja explícito. Fija un punto de guardado antes del `DELETE` y captura el error 547 de clave foránea. Si `XACT_STATE()` vale 1, vuelve al punto de guardado y aplica la baja lógica en la misma transacción. Si vale -1, revierte toda la transacción e informa el error; no reintenta la baja en otra transacción. Con `XACT_ABORT ON` el punto de guardado no rescata la transacción. No implementes la verificación enumerando tablas: crearía dependencias inversas que la arquitectura no tiene (ver RG-011 en `riesgos/10-gestion-riesgos.md`).
5. **Plataforma (DEC-028).** El sistema se construye con .NET 10 LTS (C#, WPF) y SQL Server 2025 Express, con publicación autocontenida win-x64 y sin recorte de ensamblados (RNF-0008). No uses APIs ni tipos de .NET 8.
6. **Dos definiciones de stock (DEC-040).** El *stock físico* es la suma de `TBL_LOTES.stock_actual` de todas las remesas del medicamento, incluidas las bloqueadas, agotadas o vencidas, y es lo que muestra Inventario (ESP-0006). El *stock vendible* es la suma de las remesas 'Disponible' no vencidas, y es lo que compara el reabastecimiento con `stock_minimo`. Ninguno de los dos se guarda como columna. Es provisional hasta que la dueña responda (PEN-024).
7. **Literales de dominio sin tilde (DEC-031).** Los valores cerrados se escriben igual en los `CHECK` y en los `enum` de C#, sin tilde: `'Automatico'`, `'Tecnico'`, `'Bloqueado por devolucion'`. La base se crea con la intercalación `Modern_Spanish_CI_AS`, que distingue tildes. La interfaz puede mostrar el rótulo con tilde («Automático»); la conversión vive en la capa de presentación (DEC-049).
8. **El catálogo de Ventas se alimenta de `ILotes` (DEC-044).** `ILotes` expone `medicamentosVendibles()`, que devuelve los medicamentos con alguna remesa vendible (DEC-033) y su stock vendible (DEC-040). Ventas lista solo esos medicamentos usando la arista existente `Ventas → Lotes`. No uses ESP-0006 ni crees la dependencia Ventas → Inventario; la matriz de interfaces cerrada no cambia.
9. **Los permisos los manda la matriz (DEC-050).** Los mockups no anotan permisos. La matriz de permisos (`diseno/07-matriz-permisos.md`) decide qué puede usar cada rol, y la interfaz oculta o inhabilita lo que el rol de la sesión no puede usar. Un mismo mockup sirve a los dos roles.

---

## 6. Los 2 módulos nuevos (Entrevista 3) — qué hacen exactamente

- **Reabastecimiento:** proceso que compara el stock vendible de cada producto (la suma de `TBL_LOTES.stock_actual` de sus remesas 'Disponible' no vencidas, DEC-040) contra `TBL_PRODUCTOS.stock_minimo`, y genera `TBL_LISTA_REABASTECIMIENTO` con lo que está bajo mínimo. No es una acción de un usuario puntual (sin `id_usuario`) — modélalo como un proceso programado o un cálculo bajo demanda, no como un formulario CRUD estándar.
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
| Vista lógica (paquetes, clases, mecanismos comunes, MVC) | `docs/arquitectura/VistaLogica_v01.md` (histórico; lo vigente está en el diccionario v10.00 y en DEC-028 a DEC-041) y `docs/diagramas/plaintext-VistaLogica/VL-01` a `VL-04` (`.puml`), con sus PNG en `docs/diagramas/` |
| **No usar para implementar** | `docs/diagramas/VL_01_Trazabilidad.png` y `docs/diagramas/Vista Logica.png`: imágenes hechas a mano solo para la exposición, sin fuente PlantUML. Ignóralas al generar código; si difieren de los `.puml`, prevalecen los `.puml` (DEC-024). |
| Riesgos identificados y su estado | `riesgos/10-gestion-riesgos.md` |
| Cuestiones abiertas y decisiones ya tomadas (no reabrir sin revisar) | `seguimiento/11-registro-pendientes.md` |
| Reglas para agentes de IA trabajando en este repo (commits, git add, atribución) | `AGENTS.md` (raíz del repo) |

---

## 8. Fuera de alcance de este documento

No cubre: atributos/métodos de clases, estados de entidades (`estado_lote`, `estado_producto`, `estado_orden`), diagramas de secuencia de las operaciones, ni decisiones de UI. Para eso, ve a la especificación (`ESP-XXXX`) del módulo correspondiente o a la vista lógica (diagrama de clases) cuando esté disponible.
