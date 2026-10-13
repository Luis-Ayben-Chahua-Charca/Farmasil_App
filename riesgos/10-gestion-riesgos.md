# Gestión de Riesgos — FARMASIL

**Versión:** 04.00 · **Fecha:** 13/10/2026 · **Base de verificación:** `catalogo/02-ilaciones.md`, `catalogo/03-especificaciones.md` y `modelo/05-diccionario-datos.md` v06.00 de este repositorio.

Insumo para la plantilla de gestión de riesgos GRI_0002: el equipo revisa, ajusta y traslada estas
filas a la hoja de cálculo. Cubre **las 52 ilaciones de los 13 módulos** más un bloque de **riesgos
generales (RG)**, según la observación del profesor.

## Historial de versiones

| Versión | Fecha | Cambio |
|---|---|---|
| 01.00 | 20/07/2026 | Riesgos de los módulos 1 a 8 contra las especificaciones de julio (vivía en el Proyecto de Claude). |
| 02.00 | 12/10/2026 | Reevaluación contra el catálogo vigente: 44 riesgos pasan a Resuelto, ningún riesgo queda Retrasado, se corrige el total declarado (94 → 85 filas reales) y se agregan 4 riesgos nuevos de coherencia. |
| 03.00 | 13/10/2026 | Primera versión en el repositorio. Se resuelven los 4 riesgos nuevos (ILA-0016-R03, ILA-0016-R04, ILA-0022-R03, ILA-0023-R04) con el Diccionario de Datos v06.00; se descarta el tipo de reporte 'Mermas'; se agregan los riesgos de los módulos 9 a 13 y RG-011. |
| 04.00 | 13/10/2026 | Se resuelven RG-011 (bajas condicionales por rechazo del motor), ILA-0036-R01, ILA-0036-R02, ILA-0040-R01, ILA-0043-R01 y, con RNF-0010, ILA-0013-R03, ILA-0045-R01, ILA-0045-R02, ILA-0047-R01 e ILA-0048-R01. |

En la columna *Plan de mitigación*, el prefijo **"v02 —"**, **"v03 —"** o **"v04 —"** indica en qué versión se resolvió el riesgo y cita la especificación que lo resuelve.

## Criterio usado para el campo Estado

El campo **Estado** de este documento NO es el campo "Estado" que ya trae cada tabla de
ilación/especificación (ese es un estado de flujo documental: Concluido/Pendiente del artefacto
en sí). Aquí "Estado" indica si el **riesgo** ya fue mitigado o sigue abierto, según lo que
dice literalmente el texto de la especificación:

- **Resuelto** → la especificación ya define explícitamente la regla que elimina la ambigüedad.
- **Pendiente** → la ambigüedad/riesgo sigue sin resolverse en el texto actual de la especificación.
- **En proceso** → existe una mitigación parcial o en discusión, pero falta cerrarla del todo.
- **Retrasado** → el riesgo bloquea el avance porque depende de algo que aún no existe. Desde la versión 02.00 no queda ningún riesgo en este estado.

Aplica el semáforo de colores que ya usan otros grupos: verde = Resuelto, amarillo = En proceso,
rojo = Retrasado (Pendiente puede quedar sin color o gris, a criterio del equipo).

---

## Bloque RG — Riesgos generales del proyecto (transversales)

Estos no están atados a una ilación específica: son amenazas de infraestructura/entorno que
afectan a **todo** el sistema FARMASIL (todas las especificaciones dependen, por ejemplo, de
`VALIDAR CONEXION(DB_FARMASIL) = EXITOSA`). Se agrega RG-011, detectado al revisar los módulos 9 a 13.

| Clave | Riesgo identificado | Prob. (cualitativa) | Prob. | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| RG-001 | Corte de suministro eléctrico durante una transacción en curso (venta, devolución, etc.), con pérdida de la operación no confirmada. | Media | 50% | 1 | En proceso | Envolver toda operación crítica en transacción atómica con rollback garantizado; evaluar UPS básico. | DB_FARMASIL |
| RG-002 | Interrupción de la red local que impide la conexión con DB_FARMASIL, bloqueando todas las ilaciones que dependen de esa validación. | Media | 50% | 2 | En proceso | Definir mensaje estándar de "sin conexión" y reintento automático en todas las precondiciones. | Todas las ESP |
| RG-003 | Daño físico del equipo (disco, memoria, placa) donde corre el sistema o la BD. | Baja | 10% | 5 | Pendiente | Definir política de respaldo en equipo secundario o nube. | DB_FARMASIL |
| RG-004 | Eliminación accidental de DB_FARMASIL o de tablas críticas por un usuario con privilegios elevados. | Baja | 10% | 5 | Pendiente | Restringir privilegios DROP/DELETE masivo; respaldo automático diario. | DB_FARMASIL |
| RG-005 | Infección por virus o malware que afecte el sistema o corrompa los datos. | Media | 50% | 3 | En proceso | Antivirus actualizado; restringir dispositivos USB no autorizados. | — |
| RG-006 | Acceso no autorizado al equipo, exponiendo datos tributarios y de ventas. | Media | 50% | 4 | Pendiente | Bloqueo de sesión por inactividad; contraseña obligatoria de acceso al equipo. | RNF-0006, Matriz de permisos |
| RG-007 | Corrupción de la BD por cierre inesperado del sistema mientras una transacción está en curso. | Media | 50% | 4 | Pendiente | Verificar que todas las ESP con escritura usen INICIAR/CONFIRMAR TRANSACCION con rollback garantizado. | Todas las ESP con transacción |
| RG-008 | Error del usuario al ingresar información incorrecta (fechas, cantidades, montos) por falta de capacitación. | Media | 50% | 2 | En proceso | Reforzar validaciones de formato por campo; plan de capacitación al personal. | Guía de nomenclatura de mockups |
| RG-009 | Fallas del sistema operativo o incompatibilidad del software con el equipo de la farmacia. | Baja | 10% | 3 | Pendiente | Definir requisitos mínimos de hardware/software antes del despliegue. | 08-requisitos-no-funcionales.md |
| RG-010 | Pérdida de información por ausencia de copias de seguridad periódicas de DB_FARMASIL. | Media | 50% | 5 | Pendiente | Programar respaldo automático diario de la base de datos. | DB_FARMASIL |
| RG-011 | Las bajas condicionales (física si no hay referencias, lógica si las hay) consultan tablas de otros módulos para decidir: ESP-0008 lee Lotes y Restricciones, ESP-0020 lee Ventas, ESP-0036 lee Ventas y Devoluciones, ESP-0040 lee Productos y Devoluciones. Son dependencias inversas que la arquitectura genérica no registra, y las listas ya están incompletas (ILA-0036-R01, ILA-0040-R01). | Baja | 10% | 3 | Resuelto | v04 — ESP-0008, ESP-0020, ESP-0036, ESP-0040 y ESP-0044 intentan el borrado físico y, si el motor lo rechaza por una clave foránea, aplican la baja lógica. Ningún módulo enumera tablas ajenas; la regla de borrado de las FK está en el Diccionario de Datos v06.00 y en la sección 5 del contexto de arquitectura. | ESP-0008, ESP-0020, ESP-0036, ESP-0040, Arquitectura genérica |

---

## Módulo 1 — Gestión de Ventas (EDU-0001) · ILA-0001 a ILA-0004 · prefijo VEN

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0001-R01 | Fallo de conexión con DB_FARMASIL impide crear la venta pese a validaciones previas. | Media | 50% | 2 | Pendiente | Reintento automático y mensaje de error de conexión. | ESP-0001 |
| ILA-0001-R02 | El stock de la remesa (`TBL_LOTES.stock_actual`) se valida al agregar cada línea, fuera de la transacción: otra venta simultánea puede agotarlo antes de confirmar (condición de carrera). | Baja | 10% | 3 | Pendiente | Revalidar `stock_actual` de cada remesa dentro de la misma transacción antes de descontar. | ESP-0001 |
| ILA-0001-R03 | Ambigüedad sobre si la cantidad vendida valida contra el stock disponible. | Baja | 10% | 3 | Resuelto | v02 — ESP-0001 v04.00 valida `cantidad <= lote.stock_actual` y descuenta por remesa (FEFO). | ESP-0001 |
| ILA-0001-R04 | No se especifica rango de fechas permitido para la fecha de venta. | Baja | 10% | 1 | Resuelto | v02 — ESP-0001 toma `FECHA_HORA_SISTEMA` y la muestra en VEN-LBL-FECHA-VENTA: el usuario no la captura. | ESP-0001 |
| ILA-0002-R01 | No se define comportamiento cuando el filtro por fecha/producto no arroja resultados. | Media | 50% | 1 | Pendiente | Mensaje "Sin resultados" en vez de tabla vacía. | ESP-0002 |
| ILA-0002-R02 | Ambigüedad sobre si la búsqueda por producto admite coincidencia parcial o exacta. | Baja | 10% | 2 | Resuelto | v02 — ESP-0002 v03.00 selecciona el producto desde VEN-CMB-PRODUCTO-VENTA y filtra por `id_producto` (coincidencia exacta). | ESP-0002 |
| ILA-0002-R03 | No se define límite de registros mostrados (rendimiento). | Media-alta | 75% | 2 | Pendiente | Implementar paginación. | ART-MKP-VEN-0002 |
| ILA-0003-R01 | No se define si se puede editar una venta cuyo comprobante ya fue emitido. | Baja | 10% | 4 | Resuelto | v02 — ESP-0003 v03.00 bloquea la edición si existe comprobante con `estado_documento = 'Emitido'`. | ESP-0003, ESP-0009 |
| ILA-0003-R02 | Conflicto si dos usuarios actualizan el mismo registro simultáneamente sin bloqueo optimista. | Media | 50% | 2 | Pendiente | Control de concurrencia por versión de fila. | ESP-0003 |
| ILA-0003-R03 | El recálculo de cantidad no valida disponibilidad actual de stock. | Baja | 10% | 3 | Resuelto | v02 — ESP-0003 valida `diferencia <= linea.lote.stock_actual` y ajusta el stock por diferencia. | ESP-0003 |
| ILA-0004-R01 | No se define si la eliminación de una venta es física o lógica. | Baja | 10% | 4 | Resuelto | v02 — ESP-0004 v03.00 define eliminación física en cascada (detalle y cabecera) con restitución del stock, permitida solo si no hay comprobante 'Emitido'. | ESP-0004 |
| ILA-0004-R02 | Falta de validación sobre si una venta con comprobante emitido puede eliminarse. | Baja | 10% | 4 | Resuelto | v02 — ESP-0004 bloquea la eliminación si existe comprobante 'Emitido'. | ESP-0004 |
| ILA-0004-R03 | Una venta con comprobante **'Anulado'** sí puede eliminarse físicamente, pero `TBL_COMPROBANTES_TRIBUTARIOS.id_venta` sigue apuntando a ella: violación de integridad referencial y pérdida del vínculo que la auditoría tributaria necesita. | Media | 50% | 4 | Pendiente | Recomendación de la instancia maestra (13/10/2026): bloquear la eliminación si existe cualquier comprobante asociado, 'Emitido' o 'Anulado'. Pendiente de decisión de Leyni. | ESP-0004, ESP-0012 |

## Módulo 2 — Gestión de Inventario (EDU-0002) · ILA-0005 a ILA-0008 · prefijo INV

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0005-R01 | No se define si idProducto es autogenerado o manual, con riesgo de duplicados. | Baja | 10% | 3 | Resuelto | v02 — ESP-0005 v03.00 lo genera automáticamente (`ULTIMO_ID_GENERADO`) y lo muestra en INV-LBL-ID-PRODUCTO. | ESP-0005 |
| ILA-0005-R02 | La validación de precio admite 0 y no limita a dos decimales (contradice EDU-0003). | Media | 50% | 2 | En proceso | ESP-0005 ya valida `precio = DECIMAL >= 0`; falta exigir `> 0` y máximo dos decimales. | ESP-0005, ESP-0007 |
| ILA-0005-R03 | No se define qué ocurre si numeroLote ya existe para el mismo producto. | Baja | 10% | 2 | Resuelto | v02 — El lote se trasladó a ESP-0041, que valida `NO EXISTE(TBL_LOTES, id_producto = producto Y numero_lote = numeroLote)`. | ESP-0041 |
| ILA-0005-R04 | Ambigüedad sobre si fechaVencimiento admite fechas pasadas al registrar. | Baja | 10% | 1 | Resuelto | v02 — Trasladado a ESP-0041, que valida `fechaVencimiento > FECHA_SISTEMA`. | ESP-0041 |
| ILA-0006-R01 | Ambigüedad sobre si la búsqueda por nombre admite coincidencia parcial (ESP-0006 aplica `nombre = nombre`). | Alta | 90% | 2 | Pendiente | Especificar tipo de búsqueda por campo. | ESP-0006 |
| ILA-0006-R02 | Riesgo de que la consulta no muestre alertas de vencimiento asociadas (dependencia con módulo 4). | Media | 50% | 2 | Pendiente | Indicador visual si alguna remesa del producto tiene alerta activa. | ESP-0006, ESP-0013 |
| ILA-0006-R03 | No se especifica límite de registros mostrados (catálogos grandes). | Media-alta | 75% | 2 | Pendiente | Implementar paginación. | ART-MKP-INV-0002 |
| ILA-0007-R01 | No se define si idProducto puede editarse tras su creación. | Baja | 10% | 3 | Resuelto | v02 — ESP-0007 lo muestra en solo lectura y la postcondición verifica `id_producto = SIN_MODIFICACION`. | ESP-0007 |
| ILA-0007-R02 | Riesgo de que actualizar el precio afecte ventas ya confirmadas con el precio anterior. | Baja | 10% | 2 | Resuelto | v02 — `TBL_DETALLE_VENTAS.precio_unitario` congela el precio de cada línea al momento de la venta (ESP-0001). | ESP-0007, ESP-0001 |
| ILA-0007-R03 | Inconsistencia entre numeroLote actualizado y alertas de vencimiento ya registradas. | Baja | 10% | 2 | Resuelto | v02 — No aplica: el producto ya no guarda lote; las remesas viven en `TBL_LOTES` (ESP-0041 a ESP-0044). | ESP-0007 |
| ILA-0008-R01 | No se define si la eliminación es física o lógica. | Baja | 10% | 4 | Resuelto | v02 — ESP-0008 v02.00: física si no hay movimientos, baja lógica 'Descontinuado' en caso contrario. | ESP-0008 |
| ILA-0008-R02 | Falta de validación de que el producto no tenga stock ni ventas históricas antes de eliminar. | Baja | 10% | 4 | Resuelto | v02 — ESP-0008 evalúa `tieneMovimientos` contra `TBL_LOTES` y `TBL_RESTRICCIONES_VENTA`; las ventas cuelgan de las remesas. | ESP-0008 |
| ILA-0008-R03 | Riesgo de dar de baja un producto con alerta de vencimiento activa sin advertencia previa. | Media | 50% | 3 | Pendiente | Validar y notificar si hay alertas activas antes de confirmar. | ESP-0008, ESP-0013 |

## Módulo 3 — Gestión de Documentación Tributaria (EDU-0003) · ILA-0009 a ILA-0012 · prefijo DOC

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0009-R01 | No se valida que la venta exista antes de registrar el comprobante. | Baja | 10% | 3 | Resuelto | v02 — ESP-0009 exige una venta sin comprobante asociado y la selecciona desde DOC-CMB-CODIGO-VENTA. | ESP-0009 |
| ILA-0009-R02 | Ambigüedad sobre si numeroComprobante debe ser único o puede repetirse entre tipos. | Baja | 10% | 3 | Resuelto | v02 — ESP-0009 valida unicidad global de `numero_comprobante`. | ESP-0009 |
| ILA-0009-R03 | Inconsistencia si el monto del comprobante no coincide con el de la venta. | Baja | 10% | 4 | Resuelto | v02 — ESP-0009 recupera `monto_total` de la venta y lo muestra en solo lectura. | ESP-0009 |
| ILA-0010-R01 | Ambigüedad sobre si la búsqueda combinada (fecha/tipo/número) exige todos los campos o admite filtros parciales. | Media-alta | 75% | 2 | Pendiente | Declarar los filtros como opcionales e independientes. | ESP-0010 |
| ILA-0010-R02 | Exposición de datos tributarios a la técnica, que consulta comprobantes con la misma vista que la dueña. | Baja | 10% | 4 | Pendiente | Evaluar columnas visibles por rol; hoy la matriz de permisos autoriza ILA-0010 a ambos roles. | ART-MKP-DOC-0002, Matriz de permisos |
| ILA-0011-R01 | No se define si un comprobante ya reportado en declaración tributaria puede modificarse (solo existe 'Emitido'/'Anulado'). | Alta | 90% | 5 | Pendiente | Definir un estado o marca de "declarado" que bloquee la edición. | ESP-0011 |
| ILA-0011-R02 | Riesgo de que actualizar codigoVenta rompa la trazabilidad con la venta original. | Baja | 10% | 3 | Resuelto | v02 — ESP-0011 muestra `id_venta` en solo lectura. | ESP-0011 |
| ILA-0012-R01 | No se define si la eliminación es física o lógica. | Baja | 10% | 5 | Resuelto | v02 — ESP-0012 aplica baja lógica `estado_documento = 'Anulado'`. | ESP-0012 |
| ILA-0012-R02 | Riesgo de anular un comprobante ya presentado ante la autoridad tributaria sin advertencia. | Media-alta | 75% | 5 | Pendiente | Mismo mecanismo que ILA-0011-R01: marca de "declarado" que bloquee la anulación. | ESP-0012 |
| ILA-0012-R03 | Falta de validación de integridad referencial con TBL_REGISTRO_VENTAS al eliminar. | Baja | 10% | 2 | Resuelto | v02 — La baja lógica conserva el registro y su FK. Ver ILA-0004-R03 para el caso inverso (eliminar la venta). | ESP-0012 |

## Módulo 4 — Gestión de Alertas de Productos Vencidos (EDU-0004) · ILA-0013 a ILA-0016 · prefijo ALV

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0013-R01 | Ambigüedad sobre si las alertas provienen del proceso automático o del registro manual. | Baja | 10% | 1 | Resuelto | ESP-0013 consolida ambas fuentes (`UNION(proximosAVencer, retirados)`). | ESP-0013 |
| ILA-0013-R02 | No se define el origen ni el valor de `umbral_meses` (¿configurable o fijo?). EDU-0011 menciona 2 meses. | Alta | 90% | 2 | Pendiente | Definir si es editable desde un panel de configuración o valor fijo, y alinearlo con EDU-0011. | ESP-0013 |
| ILA-0013-R03 | El proceso programado que genera alertas se remite a "un requisito no funcional aparte" que no existe en 08-requisitos-no-funcionales.md. | Baja | 10% | 3 | Resuelto | v04 — Condición 1 de RNF-0010: evaluación al iniciar la aplicación y cada 24 horas, con el mismo cálculo de ESP-0013. | ESP-0013, 08-RNF |
| ILA-0014-R01 | Ambigüedad sobre si loteDefectuoso y alertaDigemid son excluyentes o pueden marcarse ambos. | Media | 50% | 1 | Pendiente | Aclarar en el diccionario de datos. | ESP-0014, Diccionario de datos |
| ILA-0014-R02 | Flujo de corrección ante validación fallida. | Baja | 10% | 1 | Resuelto | ESP-0014 define "SEÑALAR CAMPO_INVALIDO / PERMITIR CORRECCION". | ESP-0014 |
| ILA-0014-R03 | No se valida si la remesa ya tiene una alerta activa antes de registrar otra (duplicidad). | Media-alta | 75% | 2 | Pendiente | Validar que no exista ya un retiro para el mismo lote. | ESP-0014 |
| ILA-0015-R01 | Criterio para mantener o levantar el bloqueo de venta al modificar una alerta. | Baja | 10% | 1 | Resuelto | v02 — ESP-0015 mantiene el bloqueo mientras la alerta exista; el levantamiento corresponde a ESP-0016. | ESP-0015 |
| ILA-0015-R02 | Editar manualmente una alerta de origen automático sobrescribe la lógica del proceso. | Baja | 10% | 2 | Resuelto | v02 — Las alertas automáticas se calculan al consultar (ESP-0013) y no se persisten; ESP-0015 solo edita `TBL_PRODUCTOS_RETIRADOS`. | ESP-0015 |
| ILA-0016-R01 | No se define si eliminar la alerta reactiva la remesa para la venta. | Baja | 10% | 3 | Resuelto | v02 — ESP-0016 restituye `estado_lote = 'Disponible'`. | ESP-0016 |
| ILA-0016-R02 | Pérdida del historial de alertas atendidas al usar ELIMINAR físico. | Media-alta | 75% | 3 | Pendiente | Evaluar eliminación lógica, igual que ESP-0020/ESP-0032. | ESP-0016 |
| ILA-0016-R03 | ESP-0016 operaba sobre `alerta.lote`, pero `TBL_PRODUCTOS_RETIRADOS` no tenía `id_lote`: la especificación no era ejecutable contra el esquema. | Baja | 10% | 4 | Resuelto | v03 — Diccionario v06.00 agrega `id_lote` (FK anulable, `ON DELETE SET NULL`); ESP-0014 v03.00 lo registra y ESP-0016 v03.00 lo usa. Es anulable para no crear una dependencia Lotes → Alertas. | ESP-0014, ESP-0016, Diccionario de datos |
| ILA-0016-R04 | Eliminar la alerta de una remesa ya vencida la devolvía a 'Disponible' y la volvía vendible. | Baja | 10% | 5 | Resuelto | v03 — ESP-0016 v03.00 e ILA-0016 v08.00 rechazan la eliminación si `fecha_vencimiento <= FECHA_SISTEMA`; la remesa vencida se gestiona por devolución o baja. Prueba T-001 agregada en 09-pruebas. | ESP-0016, ESP-0001 |

## Módulo 5 — Gestión de Métodos de Pago (EDU-0009) · ILA-0017 a ILA-0020 · prefijo PAG

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0017-R01 | No se valida si nombreMetodo ya existe, permitiendo duplicados. | Baja | 10% | 2 | Resuelto | v02 — ESP-0017 valida unicidad de `nombre_metodo`. | ESP-0017 |
| ILA-0017-R02 | Falta de precondición que valide permisos administrativos. | Baja | 10% | 2 | Resuelto | v02 — ESP-0017 valida `ROL(SESION_USUARIO) = 'Administrador'`. | ESP-0017 |
| ILA-0018-R01 | No se define si la consulta muestra solo métodos activos o también inactivos. | Baja | 10% | 1 | Resuelto | v02 — ESP-0018 aplica filtro por estado desde PAG-CMB-ESTADO-METODO. | ESP-0018 |
| ILA-0019-R01 | Cambiar el estado a "Inactivo" afecta ventas ya registradas con ese método. | Baja | 10% | 2 | Resuelto | v02 — La postcondición de ESP-0019 verifica referencias de `TBL_REGISTRO_VENTAS` intactas. | ESP-0019 |
| ILA-0019-R02 | Renombrar nombreMetodo rompe la trazabilidad con ventas históricas. | Baja | 10% | 3 | Resuelto | v02 — Las ventas referencian `id_metodo_pago`, que es inmutable. | ESP-0019 |
| ILA-0020-R01 | Física o lógica. | Baja | 10% | 1 | Resuelto | ESP-0020: física si no hay ventas, baja lógica 'Inactivo' en caso contrario. | ESP-0020 |
| ILA-0020-R02 | Visibilidad de un método dado de baja en históricos. | Baja | 10% | 2 | Resuelto | v02 — Se excluye de VEN-CMB-METODO-PAGO y las ventas conservan su referencia (ESP-0020). | ESP-0020 |
| ILA-0020-R03 | Falta de mensaje claro al intentar dar de baja el único método de pago activo. | Media-baja | 25% | 2 | Pendiente | Bloquear la desactivación si es el único método activo. | ART-MKP-PAG-0004 |

## Módulo 6 — Gestión de Reportes de Ventas (EDU-0010 v05.00) · ILA-0021 a ILA-0024 · prefijo REP

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0021-R01 | No se define el catálogo cerrado de valores de `tipo_reporte` (criterios de agrupación de las ventas). El tipo 'Mermas' se descartó el 13/10/2026 por no tener respaldo en ningún requisito. | Alta | 90% | 2 | Pendiente | Confirmar con la dueña (FUE-0002) los criterios de agrupación y enumerarlos en `VALIDAR tipoReporte` (ESP-0021 v04.00). | ESP-0021, ILA-0021, Diccionario de datos |
| ILA-0021-R02 | No se valida que fechaFin sea posterior a fechaInicio. | Baja | 10% | 2 | Resuelto | v02 — ESP-0021 valida `fechaInicio <= fechaFin`. | ESP-0021 |
| ILA-0021-R03 | Lentitud o timeout al consolidar ventas para rangos de fechas muy amplios. | Media | 50% | 3 | Pendiente | Optimizar con índices por fecha y limitar rango máximo. | ESP-0021 |
| ILA-0022-R01 | No se define qué ocurre si el archivo del reporte ya no existe físicamente al visualizarlo (el plan de pruebas ya lo prueba, pero la especificación no define la respuesta). | Media | 50% | 2 | Pendiente | Definir mensaje de error o regeneración on-demand. | ESP-0022, 09-pruebas |
| ILA-0022-R02 | Falta de mensaje si TBL_REPORTES_VENTAS está vacía al consultar. | Baja | 10% | 1 | Pendiente | Mensaje "No hay reportes generados aún". | ESP-0022 |
| ILA-0022-R03 | ESP-0022 cargaba `total_recaudado` en REP-TBL-REPORTES, pero esa columna no existía en `TBL_REPORTES_VENTAS`. | Baja | 10% | 3 | Resuelto | v03 — Diccionario v06.00 agrega `total_recaudado`; ESP-0021 v04.00 lo registra y ESP-0023 v03.00 lo recalcula. | ESP-0022, ILA-0022, Diccionario de datos |
| ILA-0023-R01 | La especificación dependía de una versión ampliada de EDU-0010. | Baja | 10% | 5 | Resuelto | v02 — EDU-0010 v05.00 autoriza Actualizar y Eliminar los reportes generados. | ESP-0023, EDU-0010 |
| ILA-0023-R02 | No se define un límite para el nuevo intervalo de fechas al recalcular. | Media | 50% | 2 | Pendiente | Definir rango máximo permitido (ej. 12 meses). | ESP-0023 |
| ILA-0023-R03 | Inconsistencia si el recálculo se ejecuta mientras se registran nuevas ventas. | Media | 50% | 2 | Pendiente | Tomar una fotografía (snapshot) consistente de los datos al recalcular. | ESP-0023 |
| ILA-0023-R04 | ESP-0023 regeneraba con `reporte.formato`, que no se persistía, y su consulta no unía `TBL_USUARIOS`: el reporte regenerado perdía el `usuario_responsable` que exige EDU-0010. | Baja | 10% | 3 | Resuelto | v03 — Diccionario v06.00 agrega `formato`; ESP-0023 v03.00 e ILA-0023 v03.00 unen `TBL_USUARIOS` y consolidan igual que ESP-0021. Prueba T-006 agregada en 09-pruebas. | ESP-0023, ESP-0021, Diccionario de datos |
| ILA-0024-R01 | La especificación dependía de la ampliación de EDU-0010. | Baja | 10% | 5 | Resuelto | v02 — EDU-0010 v05.00. | ESP-0024, EDU-0010 |
| ILA-0024-R02 | No se define si eliminar un reporte también elimina el archivo físico. | Baja | 10% | 2 | Resuelto | v02 — ESP-0024 ejecuta `ELIMINAR ARCHIVO(reporte.ruta_archivo)` dentro de la transacción. | ESP-0024 |
| ILA-0024-R03 | El mensaje de confirmación debe aclarar que las ventas no se ven afectadas. | Baja | 10% | 1 | Resuelto | Ya definido en ESP-0024. | ESP-0024 |

## Módulo 7 — Gestión de Devoluciones a Proveedores (EDU-0011) · ILA-0025 a ILA-0028 · prefijo DEV

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0025-R01 | No se valida si la remesa tiene stock suficiente para la devolución. | Baja | 10% | 3 | Resuelto | v02 — ESP-0025 v03.00 valida `cantidad <= lote.stock_actual`. | ESP-0025 |
| ILA-0025-R02 | Inconsistencia entre cabecera y detalle si la transacción falla a medias. | Baja | 10% | 4 | Resuelto | v02 — ESP-0025 registra cabecera y detalle en una sola transacción. | ESP-0025 |
| ILA-0025-R03 | Falta de validación de que el proveedor seleccionado esté activo. | Media | 50% | 2 | En proceso | La precondición exige al menos un proveedor 'Activo' y se valida `lote.id_proveedor = proveedor`; falta filtrar DEV-CMB-PROVEEDOR solo con activos. | ESP-0025 |
| ILA-0026-R01 | Ambigüedad sobre si la consulta permite filtrar por proveedor o rango de fechas. | Media-alta | 75% | 2 | Pendiente | Agregar filtros opcionales por proveedor y fecha. | ESP-0026 |
| ILA-0026-R02 | Comportamiento si la orden consultada no tiene detalle. | Baja | 10% | 1 | Resuelto | v02 — ESP-0025 y ESP-0027 validan `detalle <> VACIA`, y ESP-0028 da de baja la orden al vaciarla: no puede existir orden vigente sin detalle. | ESP-0025, ESP-0027 |
| ILA-0027-R01 | No se define si una orden ya enviada al proveedor puede actualizarse. | Baja | 10% | 3 | Resuelto | v02 — ESP-0027 bloquea las órdenes 'Completada'. | ESP-0027 |
| ILA-0027-R02 | Inconsistencia de stock si se modifica la cantidad devuelta. | Baja | 10% | 3 | Resuelto | v02 — ESP-0027 descuenta `TBL_LOTES.stock_actual` en la misma transacción al completar la orden. | ESP-0027 |
| ILA-0028-R01 | Decisión entre eliminación física y lógica. | Baja | 10% | 4 | Resuelto | v02 — ESP-0028 adopta baja lógica `estado_orden = 'Eliminada'`. | ESP-0028 |
| ILA-0028-R02 | Pérdida de trazabilidad de devoluciones eliminadas. | Baja | 10% | 4 | Resuelto | v02 — La cabecera se conserva con estado 'Eliminada' y las órdenes completadas no pueden eliminarse. | ESP-0028 |
| ILA-0028-R03 | Falla de stock tras eliminar la orden. | Baja | 10% | 3 | Resuelto | v02 — No aplica: ESP-0028 verifica `stock_actual = SIN_MODIFICACION`; el stock solo cambia al completar (ESP-0027). | ESP-0028 |

## Módulo 8 — Gestión de Alertas de Restricciones de Venta (EDU-0012) · ILA-0029 a ILA-0032 · prefijo RES

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0029-R01 | Catálogo de restricciones: el tipo ya está cerrado, pero `condicion_cliente` es texto libre. | Media | 50% | 2 | En proceso | ESP-0029 valida `tipoRestriccion EN ('Informativa', 'Bloqueante')`; falta catálogo cerrado de condiciones con apoyo de un experto en farmacia. | ESP-0029 |
| ILA-0029-R02 | No se valida si ya existe una restricción activa para el mismo medicamento y condición antes de registrar otra. | Media-alta | 75% | 2 | Pendiente | Validar unicidad de restricción activa por medicamento y condición. | ESP-0029 |
| ILA-0029-R03 | Riesgo de que la restricción registrada no bloquee de inmediato la venta. | Baja | 10% | 5 | Resuelto | v02 — ESP-0001 consulta `TBL_RESTRICCIONES_VENTA` en cada línea y rechaza las 'Bloqueante' activas. | ESP-0029, ESP-0001 |
| ILA-0030-R01 | Ambigüedad sobre si la búsqueda por medicamento es parcial o exacta. | Baja | 10% | 2 | Resuelto | v02 — ESP-0030 selecciona el medicamento desde RES-CMB-MEDICAMENTO y filtra por `id_producto`. | ESP-0030 |
| ILA-0030-R02 | La consulta no distingue restricciones activas de inactivas. | Baja | 10% | 2 | Resuelto | v02 — ESP-0030 muestra `estado_alerta` en RES-TBL-RESTRICCIONES-VENTA. | ESP-0030 |
| ILA-0031-R01 | Desactivar una restricción deja sin protección una venta próxima. | Media | 50% | 4 | Pendiente | Exigir confirmación adicional y registro de auditoría al desactivar. | ESP-0031 |
| ILA-0031-R02 | Falta de bloqueo optimista si dos usuarios modifican la misma restricción. | Baja | 10% | 1 | Pendiente | Control de concurrencia por versión de fila. | ESP-0031 |
| ILA-0032-R01 | Física o lógica. | Baja | 10% | 1 | Resuelto | ESP-0032 aplica baja lógica `estado_alerta = 'Inactivo'`. | ESP-0032 |
| ILA-0032-R02 | Ambigüedad sobre si una restricción inactiva sigue visible en la tabla principal o solo en un histórico. | Media | 50% | 1 | Pendiente | Filtro por estado con "Activo" por defecto, igual que ESP-0018. | ESP-0032, ESP-0030 |
| ILA-0032-R03 | Desactivar la única restricción de un medicamento sensible no genera advertencia adicional. | Media | 50% | 3 | Pendiente | Mostrar advertencia si se desactiva la última restricción activa. | ESP-0032 |

---

## Módulo 9 — Gestión de Usuarios (EDU-0013) · ILA-0033 a ILA-0036 · prefijo USR

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0033-R01 | No se define una política mínima de contraseña (longitud, composición): ESP-0033 solo exige que no esté vacía. | Media | 50% | 3 | Pendiente | Definir la política en RNF-0006 y validarla en ESP-0033 y ESP-0035. | ESP-0033, ESP-0035, RNF-0006 |
| ILA-0033-R02 | `ENCRIPTAR(contrasena)` no precisa el mecanismo: un cifrado reversible permitiría recuperar las contraseñas si se filtra la base. | Media-alta | 75% | 4 | Pendiente | Especificar hash con sal (p. ej. PBKDF2 o bcrypt) en lugar de cifrado reversible. | ESP-0033, ESP-0035, RNF-0006 |
| ILA-0034-R01 | Ambigüedad sobre si el filtro por nombre de usuario admite coincidencia parcial (`nombre_usuario = nombreUsuario`). | Media | 50% | 1 | Pendiente | Especificar el tipo de búsqueda. | ESP-0034 |
| ILA-0035-R01 | Degradar o desactivar la última cuenta administradora dejaría el sistema sin nadie que lo administre. | Baja | 10% | 5 | Resuelto | ESP-0035 cuenta los administradores activos y cancela la operación si quedaría ninguno. | ESP-0035 |
| ILA-0035-R02 | No se define si un cambio de rol o estado afecta a una sesión ya abierta de esa cuenta o solo al siguiente inicio de sesión. | Media | 50% | 3 | Pendiente | Revalidar rol y estado en cada operación, o cerrar la sesión de la cuenta modificada. | ESP-0035, RNF-0006 |
| ILA-0036-R01 | La baja física solo verifica ventas y devoluciones, pero `TBL_COTIZACIONES_PROVEEDOR.id_usuario` también referencia la cuenta: eliminar un usuario que solo registró cotizaciones viola la integridad referencial. | Baja | 10% | 4 | Resuelto | v04 — Cubierto por el mecanismo de RG-011: el motor rechaza el borrado de una cuenta referenciada desde cualquier tabla, incluida `TBL_COTIZACIONES_PROVEEDOR`. Prueba T-006 agregada. | ESP-0036, ESP-0049 |
| ILA-0036-R02 | No existe ninguna ilación ni especificación de inicio de sesión: todas las precondiciones validan `SESION_USUARIO = ACTIVA` y ESP-0036 remite a un requisito "aún no redactado" que impida el ingreso de cuentas inactivas. | Baja | 10% | 4 | Resuelto | v04 — La condición 1 de RNF-0006 ya establece la autenticación y el rechazo de cuentas inactivas; ESP-0036 v02.00 e ILA-0036 v02.00 citan esa condición en lugar de un requisito "aún no redactado". | ESP-0036, RNF-0006 |

## Módulo 10 — Gestión de Proveedores (EDU-0014) · ILA-0037 a ILA-0040 · prefijo PRV

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0037-R01 | La validación del RUC se limita a once dígitos numéricos: acepta RUC con prefijo o dígito verificador inválidos. | Media | 50% | 2 | Pendiente | Validar prefijo (10/15/17/20) y dígito verificador según el algoritmo de SUNAT. | ESP-0037, ESP-0039 |
| ILA-0038-R01 | La consulta exige RUC o razón social y filtra por igualdad exacta: no permite listar todos los proveedores ni buscar por parte del nombre. | Media-alta | 75% | 2 | Pendiente | Permitir consulta sin filtros y búsqueda parcial por razón social. | ESP-0038 |
| ILA-0039-R01 | El RUC es editable aunque el proveedor ya tenga productos, remesas o devoluciones registradas, lo que altera retroactivamente la identidad tributaria del histórico. | Media | 50% | 3 | Pendiente | Declarar el RUC de solo lectura cuando el proveedor tenga registros asociados. | ESP-0039 |
| ILA-0040-R01 | La baja física verifica productos y órdenes de devolución, pero no `TBL_LOTES.id_proveedor` (v05.00) ni `TBL_COTIZACIONES_PROVEEDOR.id_proveedor`: eliminar un proveedor con remesas o cotizaciones viola la integridad referencial. | Baja | 10% | 4 | Resuelto | v04 — Cubierto por el mecanismo de RG-011, que alcanza también a `TBL_LOTES.id_proveedor` y `TBL_COTIZACIONES_PROVEEDOR.id_proveedor`. Prueba T-006 agregada. | ESP-0040, ESP-0041, ESP-0049 |

## Módulo 11 — Gestión de Lotes (EDU-0015) · ILA-0041 a ILA-0044 · prefijo LOT

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0041-R01 | Se puede registrar una remesa con `stock_actual = 0` en estado 'Disponible', mientras ESP-0001 marca 'Agotado' toda remesa que llega a cero. | Media | 50% | 2 | Pendiente | Exigir `stock > 0` al registrar, o asignar 'Agotado' cuando sea cero. | ESP-0041, ESP-0001 |
| ILA-0041-R02 | Se acepta una remesa que vence dentro del umbral de alerta sin ninguna advertencia al registrarla. | Media | 50% | 2 | Pendiente | Advertir al registrar si `fecha_vencimiento` cae dentro del umbral de ESP-0013. | ESP-0041, ESP-0013 |
| ILA-0043-R01 | El estado de la remesa es editable libremente: se puede pasar a 'Disponible' una remesa 'Bloqueado por devolucion', lo que evita la regla de ILA-0016 (alerta activa o remesa vencida). | Baja | 10% | 4 | Resuelto | v04 — ESP-0043 v03.00 e ILA-0043 v03.00 muestran el estado en solo lectura y lo derivan de las existencias; una remesa 'Bloqueado por devolucion' conserva su estado. Pruebas T-006 y T-012 agregadas. | ESP-0043, ESP-0016 |
| ILA-0043-R02 | ESP-0043 no valida que la nueva fecha de vencimiento sea futura, a diferencia de ESP-0041. | Media | 50% | 3 | Pendiente | Aplicar la misma validación `fechaVencimiento > FECHA_SISTEMA`, o justificar la excepción (corrección de un dato mal digitado). | ESP-0043 |
| ILA-0044-R01 | La baja lógica pone `stock_actual = 0` sin registrar cuántas unidades se descartaron ni por qué. | Media | 50% | 2 | Pendiente | Registrar las unidades dadas de baja y el motivo. | ESP-0044 |

## Módulo 12 — Gestión de Reabastecimiento (EDU-0016) · ILA-0045 a ILA-0048 · prefijo REA

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0045-R01 | El proceso periódico que agrega a la lista los medicamentos bajo `stock_minimo` no tiene especificación ni requisito no funcional (Anexo de 02-ilaciones, punto 4). | Baja | 10% | 4 | Resuelto | v04 — Condición 2 de RNF-0010: ejecución cada 15 días, origen 'Automático', prioridad 'Normal', sin duplicar ítems. | Anexo 02-ilaciones, 08-RNF |
| ILA-0045-R02 | Un ítem no sale de la lista cuando se repone el stock: permanece hasta que la dueña lo elimina manualmente. | Baja | 10% | 2 | Resuelto | v04 — Decisión documentada en RNF-0010: el proceso no retira ítems; la dueña los retira al realizar el pedido (ESP-0048). | ESP-0048, proceso periódico |
| ILA-0046-R01 | La vista de la lista no muestra el stock consolidado ni el stock mínimo, datos que la dueña necesita para decidir qué cotizar. | Media | 50% | 2 | Pendiente | Agregar ambas columnas a REA-TBL-LISTA-REABASTECIMIENTO. | ESP-0046 |
| ILA-0047-R01 | No se define si el proceso periódico respeta o sobrescribe una prioridad cambiada manualmente. | Baja | 10% | 2 | Resuelto | v04 — RNF-0010 establece que el proceso no modifica la prioridad de los ítems existentes. Prueba T-006 agregada. | ESP-0047, proceso periódico |
| ILA-0048-R01 | Si la dueña elimina un ítem automático cuyo medicamento sigue bajo el mínimo, la siguiente corrida del proceso lo vuelve a agregar. | Baja | 10% | 2 | Resuelto | v04 — RNF-0010 fija el comportamiento: el ítem vuelve a agregarse mientras el medicamento siga bajo el mínimo. Prueba T-006 agregada. | ESP-0048, proceso periódico |

## Módulo 13 — Comparación de Precios de Proveedores (EDU-0017) · ILA-0049 a ILA-0052 · prefijo COT

| Clave | Riesgo identificado | Prob. | % | Impacto | Estado | Plan de mitigación | Artefactos |
|---|---|---|---|---|---|---|---|
| ILA-0049-R01 | El procedimiento no valida que el proveedor esté activo ni que el producto esté disponible; solo la precondición exige que exista al menos uno. | Media | 50% | 2 | Pendiente | Filtrar COT-CMB-PROVEEDOR y COT-CMB-PRODUCTO a registros vigentes. | ESP-0049 |
| ILA-0049-R02 | El precio cotizado admite 0 y no se limita a dos decimales. | Media | 50% | 2 | Pendiente | Exigir `precio > 0` y máximo dos decimales, igual que ILA-0005-R02. | ESP-0049, ESP-0051 |
| ILA-0050-R01 | La exportación filtra por un `proveedor_seleccionado` que la especificación no define: no queda claro si el CSV incluye los ítems donde ese proveedor ofrece el menor precio o todos los que cotizó. | Media-alta | 75% | 3 | Pendiente | Definir el criterio de asignación de ítems a proveedor y confirmarlo con la dueña. | ESP-0050 |
| ILA-0050-R02 | La comparativa toma la cotización más reciente de cada proveedor sin límite de antigüedad: un precio de hace meses compite como vigente. | Media | 50% | 2 | Pendiente | Mostrar la fecha de cada cotización y marcar las que superen una antigüedad definida. | ESP-0050 |
| ILA-0051-R01 | Editar una cotización modifica el historial de precios sin dejar rastro del valor anterior. | Media | 50% | 2 | Pendiente | Limitar la edición a corrección inmediata o registrar el cambio. | ESP-0051 |
| ILA-0052-R01 | La eliminación física de una cotización la retira del historial de precios. | Baja | 10% | 1 | Resuelto | Decisión documentada en ESP-0052: una cotización mal cargada no debe conservarse como historial legítimo. | ESP-0052 |

---

## Resumen de riesgos resueltos (verde)

| Bloque | Claves |
|---|---|
| Generales | RG-011 |
| 1 Ventas | ILA-0001-R03, ILA-0001-R04, ILA-0002-R02, ILA-0003-R01, ILA-0003-R03, ILA-0004-R01, ILA-0004-R02 |
| 2 Inventario | ILA-0005-R01, ILA-0005-R03, ILA-0005-R04, ILA-0007-R01, ILA-0007-R02, ILA-0007-R03, ILA-0008-R01, ILA-0008-R02 |
| 3 Documentación | ILA-0009-R01, ILA-0009-R02, ILA-0009-R03, ILA-0011-R02, ILA-0012-R01, ILA-0012-R03 |
| 4 Alertas de vencimiento | ILA-0013-R01, ILA-0013-R03, ILA-0014-R02, ILA-0015-R01, ILA-0015-R02, ILA-0016-R01, ILA-0016-R03, ILA-0016-R04 |
| 5 Métodos de pago | ILA-0017-R01, ILA-0017-R02, ILA-0018-R01, ILA-0019-R01, ILA-0019-R02, ILA-0020-R01, ILA-0020-R02 |
| 6 Reportes | ILA-0021-R02, ILA-0022-R03, ILA-0023-R01, ILA-0023-R04, ILA-0024-R01, ILA-0024-R02, ILA-0024-R03 |
| 7 Devoluciones | ILA-0025-R01, ILA-0025-R02, ILA-0026-R02, ILA-0027-R01, ILA-0027-R02, ILA-0028-R01, ILA-0028-R02, ILA-0028-R03 |
| 8 Restricciones | ILA-0029-R03, ILA-0030-R01, ILA-0030-R02, ILA-0032-R01 |
| 9 Usuarios | ILA-0035-R01, ILA-0036-R01, ILA-0036-R02 |
| 10 Proveedores | ILA-0040-R01 |
| 11 Lotes | ILA-0043-R01 |
| 12 Reabastecimiento | ILA-0045-R01, ILA-0045-R02, ILA-0047-R01, ILA-0048-R01 |
| 13 Comparación de precios | ILA-0052-R01 |

**Total: 66 resueltos.**

## Riesgos abiertos de mayor severidad (impacto ≥ 4 y probabilidad ≥ 50 %)

| Clave | Riesgo |
|---|---|
| RG-006 | Acceso no autorizado al equipo, exponiendo datos tributarios y de ventas. |
| RG-007 | Corrupción de la BD por cierre inesperado del sistema mientras una transacción está en curso. |
| RG-010 | Pérdida de información por ausencia de copias de seguridad periódicas de DB_FARMASIL. |
| ILA-0004-R03 | Una venta con comprobante 'Anulado' sí puede eliminarse físicamente, pero `TBL_COMPROBANTES_TRIBUTARIOS.id_venta` sigue apuntando a ella: violación de integridad referencial y pérdida del vínculo que la auditoría tributaria necesita. |
| ILA-0011-R01 | No se define si un comprobante ya reportado en declaración tributaria puede modificarse (solo existe 'Emitido'/'Anulado'). |
| ILA-0012-R02 | Riesgo de anular un comprobante ya presentado ante la autoridad tributaria sin advertencia. |
| ILA-0031-R01 | Desactivar una restricción deja sin protección una venta próxima. |
| ILA-0033-R02 | `ENCRIPTAR(contrasena)` no precisa el mecanismo: un cifrado reversible permitiría recuperar las contraseñas si se filtra la base. |

No queda ningún riesgo en estado **Retrasado**.

## Totales

| Bloque | Filas | Resuelto | En proceso | Pendiente |
|---|---|---|---|---|
| RG | 11 | 1 | 4 | 6 |
| Módulos 1-8 | 89 | 55 | 3 | 31 |
| Módulos 9-13 | 27 | 10 | 0 | 17 |
| **Total** | **127** | **66** | **7** | **54** |
