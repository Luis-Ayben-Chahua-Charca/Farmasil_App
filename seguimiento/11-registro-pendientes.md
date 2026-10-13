# Registro de Pendientes y Decisiones — FARMASIL

| Artefacto | Registro de pendientes y decisiones |
| --- | --- |
| Versión | 01.00 |
| Fecha | 13/10/2026 |
| Responsable | AUT-0001 |

## 1. Para qué existe y por qué tiene esta forma

El catálogo arrastraba pendientes dispersos en seis lugares distintos: comentarios de ilaciones y especificaciones, el Anexo de `02-ilaciones.md`, los huecos de `04-trazabilidad.md`, los vacíos del Diccionario de Datos, los pendientes de `09-pruebas-software.md` y los riesgos abiertos. Ninguno decía quién debía resolverlos ni qué bloqueaban, y varios ya estaban resueltos en un documento mientras otro seguía declarándolos abiertos.

En la industria, este problema se resuelve con tres herramientas que suelen convivir:

| Práctica | Qué registra | Dónde se usa |
| --- | --- | --- |
| **Product backlog** (Scrum) | Trabajo planificado: entregables, historias, tareas | Tablero del equipo (GitHub Projects, Jira) |
| **Lista de cuestiones abiertas** (*open issues*, sección 18 de la plantilla Volere; lista de TBD de IEEE 29148) | Lo que el documento de requisitos todavía no puede afirmar, porque falta una respuesta | Dentro de la especificación de requisitos |
| **Registro de decisiones** (*decision log*, parte del RAID log de gestión de proyectos) | Qué se decidió, cuándo, por quién y por qué | Junto a la documentación del proyecto |

Este proyecto ya tiene el backlog en GitHub Projects y los riesgos en `riesgos/10-gestion-riesgos.md`. Lo que faltaba son las dos piezas restantes, y este documento las reúne en una sola fuente:

- **Sección 3, cuestiones abiertas.** Cada pendiente tiene un código `PEN-NNN`, un responsable, lo que bloquea y un criterio de cierre verificable.
- **Sección 5, registro de decisiones.** Cada decisión que cerró una cuestión tiene un código `DEC-NNN`, para que nadie reabra una discusión ya resuelta sin saber por qué se tomó.

No se duplica el backlog ni los riesgos: un pendiente que se convierte en trabajo pasa a un issue de GitHub y aquí se anota su número; un riesgo abierto se sigue en el documento de riesgos.

## 2. Cómo se usa

1. **Abrir.** Toda cuestión que impida dar por cerrado un artefacto se registra aquí con el siguiente `PEN-NNN` libre, aunque también se mencione en el comentario del artefacto.
2. **Clasificar.** Se asigna tipo y responsable. Las del tipo *Cliente* se agrupan en la agenda de la sección 4 para llevarlas juntas a la siguiente reunión con la dueña.
3. **Trabajar.** Cuando un pendiente se convierte en trabajo, se abre un issue en GitHub y su número se anota en la columna *Issue*. El estado pasa a *En curso*.
4. **Cerrar.** Al cumplirse el criterio de cierre, la fila se mueve a la sección 6 con la fecha y el commit, y si hubo una decisión se registra en la sección 5.
5. **Revisar.** Al cierre de cada sprint se recorre la tabla: lo que siga bloqueando un entregable del sprint siguiente sube de prioridad.

**Tipos:** *Cliente* (requiere respuesta de la dueña), *Equipo* (decisión interna), *Documentación* (trabajo sin decisión pendiente), *Diseño UI* (mockups), *Alcance* (entra o no en el producto).
**Estados:** *Abierto*, *En espera del cliente*, *En curso*, *Cerrado*.
**Prioridad:** *Alta* si bloquea la implementación (sprint 7, desde el 25/10/2026); *Media* si bloquea el cierre de un artefacto del catálogo; *Baja* en otro caso.

## 3. Cuestiones abiertas

| Código | Pendiente | Tipo | Responsable | Bloquea | Prioridad | Estado | Criterio de cierre | Origen | Issue |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| PEN-001 | Dominio cerrado de `tipo_reporte`: qué criterios de agrupación de ventas ofrece el módulo de reportes. | Cliente | Dueña | ESP-0021 e ILA-0021 en estado Pendiente; combo REP-CMB-TIPO-REPORTE | Alta | En espera del cliente | Valores enumerados en `VALIDAR tipoReporte` de ESP-0021 y en el Diccionario de Datos | ILA-0021-R01, ESP-0021 v04.00 | — |
| PEN-002 | Qué ítems entran en el CSV de cada proveedor (los que cotizó o solo aquellos donde ofrece el menor precio) y con qué columnas. | Cliente | Dueña | ESP-0050; mockup ART-MKP-COT-0002 | Media | En espera del cliente | Criterio y columnas escritos en ESP-0050 e ILA-0050 | ILA-0050-R01, Anexo C.5 de `02-ilaciones.md` | — |
| PEN-003 | Si existe un estado "declarado ante SUNAT" para los comprobantes, que impida modificarlos o anularlos. | Cliente | Dueña | ESP-0011, ESP-0012 | Media | En espera del cliente | Estado incorporado al dominio de `estado_documento` o decisión explícita de no hacerlo | ILA-0011-R01, ILA-0012-R02 | — |
| PEN-004 | Criterio de aceptación de la facilidad de uso: qué se considera aprobado al observar a una técnica sin capacitación. | Cliente | Dueña | Prueba de RNF-0002 | Baja | En espera del cliente | Umbral medible (por ejemplo, tiempo máximo por venta) escrito en RNF-0002 y en `09-pruebas-software.md` | Pendiente 3 de `09-pruebas-software.md` | — |
| PEN-005 | Confirmar la frecuencia diaria de la evaluación de vencimientos, fijada por el equipo en RNF-0010. | Cliente | Dueña | Ninguno (hay un valor vigente) | Baja | En espera del cliente | Frecuencia confirmada o ajustada en RNF-0010 | RNF-0010 v01.00 | — |
| PEN-006 | Pasar a limpio y aprobar con la dueña los mockups pendientes: 20 por diseñar (USR, PRV, LOT, REA y COT), más `ART-MKP-CONFIRM-0001`, que no sigue la nomenclatura. La aprobación puede modificar especificaciones. | Diseño UI | Equipo, con aprobación de la dueña | Estado Concluido de las especificaciones que dependen de esos mockups | Alta | Abierto | Mockups publicados en `diseno/Mockups/` con nombre conforme a la guía, y especificaciones ajustadas a lo aprobado | Huecos 3 y 8 de `04-trazabilidad.md` | — |
| PEN-007 | Catálogo de fuentes: FUE-0001 a FUE-0005 se citan sin que ningún documento los defina; varias ilaciones citan entrevistas sin código. | Documentación | Equipo | Trazabilidad hacia las fuentes | Media | Abierto | Tabla de fuentes publicada y citas uniformadas | Hueco 5 de `04-trazabilidad.md`, Anexo C.2 | — |
| PEN-008 | Subir el registro de la Entrevista 2 y asignar su código FUE a RNF-0005, RNF-0006 y RNF-0007. | Documentación | Equipo | Fuente formal de tres requisitos no funcionales | Media | Abierto | Archivo en el proyecto y código FUE en los tres requisitos | Comentarios de RNF-0005 y RNF-0006 | — |
| PEN-009 | Juego de datos de prueba: medicamentos, remesas con vencimientos escalonados, proveedores, cuentas de ambos roles y ventas históricas. | Documentación | Equipo | Ejecución del plan de pruebas | Alta | Abierto | Script o archivo de carga inicial versionado en el repositorio | Pendiente 1 de `09-pruebas-software.md`, hueco 2 de `04-trazabilidad.md` | — |
| PEN-010 | Alcance: registro de clientes e historial de compras, y tipo y presentación del producto (genérico o comercial, precio por unidad o por conjunto), pedidos por la dueña en la Entrevista 1. | Alcance | Leyni con la dueña | Modelo de datos si se incorporan | Media | Abierto | Decisión registrada en la sección 5 y, si entran, educción nueva | Vacíos 2 y 4 del Diccionario de Datos | — |
| PEN-011 | Configuración del umbral de alertas: `TBL_ALERTAS_VENCIMIENTO` no tiene ninguna operación que la administre, y el umbral queda fijo. | Equipo | Leyni | Ninguno (hay un valor vigente) | Baja | Abierto | Decisión de mantenerlo fijo (documentada) o especificación de su administración | ILA-0013-R02, vacío 5 del Diccionario de Datos, hueco 4 de `04-trazabilidad.md` | — |
| PEN-012 | Revisar los 57 riesgos aún abiertos (50 Pendiente y 7 En proceso) antes de implementar cada módulo. La mayoría son mejoras de detalle: paginación, búsqueda parcial, validación del RUC. | Equipo | Leyni | Implementación de cada módulo | Media | Abierto | Cada riesgo del módulo resuelto o aceptado de forma explícita al iniciar su sprint | `riesgos/10-gestion-riesgos.md` v06.00 | — |

## 4. Agenda para la próxima reunión con la dueña

Preguntas en el orden en que conviene hacerlas, para salir de la reunión con todo lo que bloquea:

1. **Reportes de ventas (PEN-001).** ¿Qué reportes quiere generar? Por ejemplo: ventas por día, por semana, por mes, por producto o por personal.
2. **Mockups (PEN-006).** Revisión de los diseños pasados a limpio. Anotar cada cambio que pida, porque puede modificar especificaciones.
3. **Exportación a proveedores (PEN-002).** Cuando exporta el CSV para un proveedor, ¿qué medicamentos deben aparecer y qué columnas necesita el proveedor?
4. **Comprobantes (PEN-003).** Una vez que declara sus comprobantes ante SUNAT, ¿necesita que el sistema impida modificarlos o anularlos?
5. **Alcance (PEN-010).** ¿Sigue necesitando el registro de clientes y la distinción entre genérico y comercial, o pueden quedar para una versión posterior?
6. **Avisos de vencimiento (PEN-005).** ¿Le basta con ver el aviso de remesas por vencer una vez al día, al abrir el sistema?
7. **Facilidad de uso (PEN-004).** ¿En cuánto tiempo esperaría que una técnica nueva registre una venta sin ayuda?

## 5. Registro de decisiones

| Código | Fecha | Decisión | Motivo | Decidió | Artefactos |
| --- | --- | --- | --- | --- | --- |
| DEC-001 | 12/10/2026 | `usuario_responsable` es un valor calculado al generar el reporte, no una columna. | `TBL_REPORTES_VENTAS` es un índice sin claves foráneas; el responsable se obtiene de `TBL_REGISTRO_VENTAS.id_usuario`. | Leyni | ESP-0021, ILA-0021, Diccionario de Datos |
| DEC-002 | 12/10/2026 | Los nombres de actor de los casos de uso son los de la Matriz de Permisos. | Seis nombres distintos para dos actores. | Leyni | CU-01 a CU-08 |
| DEC-003 | 12/10/2026 | No se elaboran más diagramas de casos de uso. | El docente los pidió solo cuando resuelven una ambigüedad; las que quedaban se resuelven mejor con los diagramas de comportamiento. | Equipo | `docs/diagramas/` |
| DEC-004 | 12/10/2026 | Los mockups quedan en espera hasta que la dueña apruebe los diseños pasados a limpio. | La aprobación puede cambiar las especificaciones. | Leyni | PEN-006 |
| DEC-005 | 13/10/2026 | Se descarta el tipo de reporte 'Mermas'. | No tenía respaldo en ningún requisito ni la dueña lo pidió. | Leyni | ESP-0021, ILA-0021, Diccionario de Datos |
| DEC-006 | 13/10/2026 | `TBL_PRODUCTOS_RETIRADOS.id_lote` es una clave foránea anulable con `ON DELETE SET NULL`. | Permite restituir la remesa sin crear una dependencia Lotes → Alertas de Vencimiento. | Instancia maestra | ESP-0014, ESP-0016, Diccionario de Datos |
| DEC-007 | 13/10/2026 | `TBL_REPORTES_VENTAS` guarda `formato` y `total_recaudado`. | ESP-0022 y ESP-0023 ya los usaban sin que existieran. | Instancia maestra | ESP-0021, ESP-0023, Diccionario de Datos |
| DEC-008 | 13/10/2026 | Una remesa vencida no vuelve a la venta al eliminar su alerta. | Contradecía EDU-0004. | Instancia maestra | ESP-0016, ILA-0016 |
| DEC-009 | 13/10/2026 | Las bajas condicionales intentan el borrado físico y aplican la baja lógica si el motor lo rechaza por una clave foránea. | Evita que un módulo consulte tablas de otros, lo que creaba dependencias inversas, y cubre automáticamente cualquier referencia. | Leyni | ESP-0008, ESP-0020, ESP-0036, ESP-0040, ESP-0044, contexto de arquitectura |
| DEC-010 | 13/10/2026 | El estado de la remesa no es editable; se deriva de sus existencias. | Editarlo permitía devolver a la venta remesas bloqueadas o vencidas. | Leyni | ESP-0043, ILA-0043 |
| DEC-011 | 13/10/2026 | Los procesos programados se documentan en un solo requisito, RNF-0010. | ILA-0013 e ILA-0045 remitían a requisitos que no existían. | Leyni | RNF-0010 |
| DEC-012 | 13/10/2026 | El inicio de sesión no es un módulo: lo cubre la condición 1 de RNF-0006. | No tiene las cuatro operaciones de un módulo; es la condición que todas las precondiciones asumen. | Leyni | RNF-0006, ESP-0036 |
| DEC-013 | 13/10/2026 | No se puede eliminar una venta con cualquier comprobante asociado, emitido o anulado. | Un comprobante anulado es un documento tributario que se conserva con su venta; la corrección se hace actualizando la venta. | Leyni | ESP-0004, ILA-0004 |
| DEC-014 | 13/10/2026 | La contraseña se guarda como hash irreversible con sal individual. | "Encriptada" admitía un cifrado reversible. | Leyni | RNF-0006, ESP-0033, ESP-0035, Diccionario de Datos |
| DEC-015 | 13/10/2026 | Las pruebas cubren solo lo necesario para demostrar calidad, sin sobreingeniería. | Criterio del equipo para el plan de pruebas. | Leyni | `09-pruebas-software.md` |

## 6. Cuestiones cerradas

Se registran aquí las cuestiones que tenían un `PEN-NNN` asignado al cerrarse. Las resueltas antes de crear este registro constan en la sección 5 y en el historial de commits de la rama `docs/49-diagramas-comportamiento`.

| Código | Pendiente | Fecha de cierre | Decisión | Commit |
| --- | --- | --- | --- | --- |
| — | — | — | — | — |
