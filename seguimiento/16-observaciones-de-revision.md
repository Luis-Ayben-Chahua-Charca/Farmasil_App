# Observaciones de revisión — FARMASIL

| Artefacto | Observaciones de revisión |
| --- | --- |
| Versión | 01.00 |
| Fecha | 24/11/2026 |
| Responsable | AUT-0001 |

## Historial de versiones

| Versión | Fecha | Cambio |
| --- | --- | --- |
| 01.00 | 24/11/2026 | Se crea el documento y se registra su convención (DEC-085). Primeras observaciones, OBS-001 a OBS-012, tomadas de la revisión de los PR #182, #184, #185, #186 y #187 (sprint S2). El registro es parcial: queda pendiente una auditoría de los PR anteriores (sección 7). |

> **Registro parcial.** Este documento no recoge todas las observaciones desde el sprint S1; falta la auditoría descrita en la sección 7.

## 1. Para qué existe

Al revisar un PR aparecen defectos reales que no bloquean su fusión: algo que hoy no se manifiesta, un detalle de calidad, un documento que conviene alinear. Si solo se mencionan en el comentario del PR, se pierden al fusionarlo. Este documento los recoge, uno por fila, para resolverlos al cierre de cada sprint.

No sustituye a otros documentos:

| Documento | Qué registra | Cuándo NO va aquí |
| --- | --- | --- |
| `11-registro-pendientes.md`, `PEN-NNN` | Una pregunta sin respuesta que bloquea un artefacto | Si la observación es una pregunta para la dueña o el equipo, va como `PEN` |
| `11-registro-pendientes.md`, `DEC-NNN` | Una decisión ya tomada | Si resolver la observación obliga a cambiar una decisión, primero se registra la `DEC` nueva |
| `15-hallazgos-de-uso.md`, `HAL-NNN` | Lo que solo se descubre usando la aplicación a mano | Si se vio leyendo código o documentos durante una revisión, va aquí |
| Comentarios de un PR | Lo que bloquea esa fusión | Si el defecto bloquea el PR, se corrige o se pide cambios; no se archiva aquí |
| Issues de GitHub | El trabajo de corregir | Aquí se anota el número cuando la observación se convierte en trabajo |

## 2. Cómo se usa

1. **Registrar.** Quien revisa un PR (R1, R8 o un agente de IA) anota cada observación que no bloquea con el siguiente `OBS-NNN` libre, al terminar la revisión y antes de fusionar. Basta una frase, el módulo y el PR de origen.
2. **Clasificar.** Se asigna tipo y gravedad. Una observación nueva queda en estado *Nuevo*.
3. **Resolver al cierre del sprint.** En la revisión de cierre se recorre la sección 4. Cada observación se corrige, se convierte en `PEN` (si falta una decisión), se asigna a un issue ya planificado o se descarta con su razón. Un grupo de observaciones del mismo módulo puede corregirse en un solo issue con la etiqueta `mod:XXX`.
4. **Cerrar.** Al fusionarse la corrección, la fila pasa a la sección 5 con la fecha y el PR. Si se decide no corregirla, pasa igual con estado *Descartado* y la razón.
5. **Numeración.** Antes de registrar, se comprueba el último `OBS-NNN` en `main` y en las ramas remotas. Si varios PR en paralelo registran observaciones, el que fusiona segundo toma los números libres y la versión siguiente del documento.

**Agentes de IA.** Al terminar una revisión, el agente lista las observaciones que no bloquean y propone sus filas para este documento; no las deja solo en el mensaje final. Una observación no se corrige dentro de otro PR: tiene su propio issue.

**Tipos:** *Defecto latente* (no se manifiesta hoy, sí cuando otro módulo cambie), *Calidad* (código o diseño interno mejorable), *Cobertura* (falta una prueba automática), *Documento* (catálogo, plan de pruebas o mockup por alinear), *Diseño* (decisión de pantalla que conviene revisar).
**Gravedad:** *Media* si puede causar un error visible al cambiar el contexto; *Baja* si es cosmético, de estilo o de bajo riesgo.
**Estados:** *Nuevo*, *En análisis*, *En issue*, *Corregido*, *Descartado*.

## 3. Plantilla de una entrada

| Campo | Qué escribir |
| --- | --- |
| Observación | Qué se vio, en una o dos frases, y por qué no bloquea. |
| Módulo | Prefijo del módulo (`USR`, `INV`…) o `GEN` si es transversal |
| PR de origen | PR en cuya revisión se vio |
| Relacionado | `ESP`, `DEC`, `PEN`, `HAL` o issue que toca |
| Issue | Número del issue que la corrige, cuando exista |

## 4. Observaciones abiertas

| Código | Observación | Módulo | Tipo | Gravedad | Estado | PR de origen | Relacionado | Issue |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| OBS-001 | `MotivoAccionesAdministrador` no notifica cambios: si la sesión cambia con la pantalla abierta, el `ToolTip` y la habilitación de los botones conservan el rol anterior. Hoy no se manifiesta porque las vistas se crean al abrir el módulo. | GEN (INV, PRV, PAG) | Defecto latente | Baja | Nuevo | #182, #184, #186 | #88 (fusionado en #183), DEC-075 | |
| OBS-002 | El SQL de `InventarioRepositorio` no tiene pruebas automáticas; solo lo cubren las manuales. | INV | Cobertura | Media | Nuevo | #182 | #94 | |
| OBS-003 | Los filtros de la consulta se reinician después de cada alta, actualización o baja, y la persona pierde el contexto de lo que estaba mirando. | INV | Diseño | Baja | Nuevo | #182 | #93 | |
| OBS-004 | No hay un botón «Nuevo» para volver a la ficha en blanco después de seleccionar un producto. Ni la ESP ni el mockup lo prevén, así que agregarlo exige decidir. | INV | Diseño | Baja | Nuevo | #182 | #93 | |
| OBS-005 | `VerificarBaja` y `DarDeBaja` no son atómicas: una orden de devolución pendiente creada entre las dos no se detectaría. Riesgo bajo. Si se decide cerrarlo, va como `PEN`. | PRV | Defecto latente | Baja | Nuevo | #184 | ESP-0040, DEC-029 | |
| OBS-006 | En el PNG de `ART-MKP-RES-0001` la tabla de restricciones desborda la tarjeta por la derecha y la columna «Estado» queda cortada. | RES | Documento | Baja | Nuevo | #185 | #92 | |
| OBS-007 | La fila de ESP-0030 en el plan de pruebas 09 sigue diciendo «seleccionar medicamento, consultar…» y no menciona la búsqueda ni los filtros nuevos. | RES | Documento | Baja | Nuevo | #185 | ESP-0030, DEC-082, #102, #104 | |
| OBS-008 | ESP-0030 no valida el valor de los filtros de tipo y estado; la ESP-0038 de PRV sí valida `estadoFiltro`. Son combos cerrados, de riesgo bajo. | RES | Documento | Baja | Nuevo | #185 | ESP-0030, ESP-0038, DEC-079 | |
| OBS-009 | `PagosSimulado` y `PagosSimuladoPruebas` siguen en el código sin uso en producción. Pasa lo mismo con `ProveedoresSimulado`. Se retiran cuando Ventas use las implementaciones reales. | PAG, PRV | Calidad | Baja | Nuevo | #186, #184 | #112 | |
| OBS-010 | `ExigirRolDeConsulta` hace `ExigirRol(Administrador)` o `ExigirRol(Tecnico)` según el rol de la sesión, lo que equivale a «cualquiera de los dos roles». Un método de dos roles lo diría más claro. | PAG | Calidad | Baja | Nuevo | #186 | DEC-069 | |
| OBS-011 | `AlertasServicio` tiene dos constructores públicos con `IDevoluciones?` opcional «por compatibilidad»: si falta la dependencia, `Eliminar` falla en ejecución. Un solo constructor con la dependencia obligatoria fallaría al construirse. | ALV | Calidad | Media | Nuevo | #187 | #109 | |
| OBS-012 | `AlertasServicio.Consultar` lanza `ArgumentException` para un estado de filtro inválido, aunque DEC-072 y DEC-079 la reservan para errores de programación. El combo es cerrado, así que hoy no es alcanzable. Viene del #87. | ALV | Calidad | Baja | Nuevo | #187 | DEC-072, DEC-079, #87 | |

## 5. Observaciones cerradas

| Código | Observación | Estado | Cerrado | PR o razón |
| --- | --- | --- | --- | --- |
| | Sin entradas todavía. | | | |

## 6. Origen de las observaciones de esta versión

OBS-001 a OBS-012 salen de la revisión que R1 (Leyni) pidió, con ayuda de un agente de IA, de los PR #182 (INV), #184 (PRV), #185 (RES), #186 (PAG) y #187 (ALV) del sprint S2, entre el 23 y el 24/11/2026. La atomicidad entre ALV y LOT, que también salió de la revisión del #187, ya está registrada como `PEN-039` y no se repite aquí.

## 7. Cobertura de este documento y auditoría pendiente

**Este documento no recoge todas las observaciones del proyecto.** Solo contiene las de los cinco PR citados en la sección 6, que son las que estaban a mano al crearlo. Quedan fuera:

- Las observaciones de los demás PR revisados o fusionados desde el sprint S1 (por ejemplo, #167, #168, #169, #174, #176, #181 y #183), cuyo detalle solo quedó en los comentarios de cada PR o en mensajes de revisión que no se conservaron.
- Cualquier observación de un PR de S1 que se fusionó sin revisión formal.

**Pendiente: una auditoría para encontrarlas.** Hay que recorrer, PR por PR desde S1, los comentarios y las revisiones de GitHub, los mensajes de commit y los registros de revisión que existan, y anotar aquí cada observación que no bloqueó. Hasta que esa auditoría se haga y se anote su fecha y su alcance en esta sección, ninguna persona debe dar por resuelto el sprint S2 ni los anteriores con solo vaciar la sección 4: la ausencia de una observación en este documento no significa que no exista.

La auditoría es el issue #190.

Estado de la auditoría: **no realizada**.
