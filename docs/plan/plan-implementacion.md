# FARMASIL_APP — Plan de implementación y roles del equipo (8 nov – 19 dic 2026)

> Versión v01.00 · 09/11/2026 · Responsable: R1 (Leyni). Redactado el 08/11/2026; esta versión lo ajusta a las decisiones registradas hasta el 09/11/2026 (DEC-029, DEC-042 y DEC-055 a DEC-059).
>
> Los issues del tablero citan este plan por número de sección. No se reordenan las secciones: si una cambia de contenido, se actualiza aquí y se avisa en el issue que la cita.

## 1. Punto de partida

La planeación está cerrada y lo que queda es implementar: 6 sprints de una semana, del domingo 8 de noviembre al sábado 19 de diciembre de 2026. Las tres fases de arquitectura ya están entregadas, incluida la Vista Física. Solo se agrega un diagrama nuevo si una ambigüedad del catálogo lo exige para un proceso crítico.

Todo lo necesario para programar ya está en el repositorio `Farmasil_App`:

| Insumo | Ubicación en el repo | Para qué sirve al implementar |
| --- | --- | --- |
| Especificaciones (ESP), ilaciones (ILA) y educciones (EDU) | `catalogo/` | Qué hace cada operación, sus precondiciones, validaciones y mensajes |
| Diccionario de datos | `modelo/05-diccionario-datos.md` | Script de creación de la base `DB_FARMASIL` |
| Vista lógica (VL-01 a VL-04) | `docs/diagramas/plaintext-VistaLogica/` | Paquetes, capas Vista → VistaModelo → Modelo → Persistencia e interfaces entre módulos |
| Vista de procesos (VP-00 a VP-13, VP-B, VP-T1, VP-T2) | `docs/diagramas/plaintext-VistaProcesos/` | Transacciones, baja condicional, arranque y procesos programados |
| Vista física (VF-01, VF-02) | `docs/diagramas/plaintext-VistaFisica/` | Despliegue en el equipo de la farmacia, respaldo y restauración |
| Arquitectura centrada en datos (ACD-01 a ACD-04) | `docs/diagramas/plaintext-CentradaDatos/` | Qué tablas escribe cada módulo, lecturas cruzadas permitidas y vistas de stock (DEC-055, DEC-056) |
| Diagramas de comportamiento (EST, SEC) | `docs/diagramas/plaintext-Comportamiento/` | Estados de la remesa y de la orden de devolución, secuencias de venta, reabastecimiento y comparación de precios |
| Matriz de permisos | `diseno/07-matriz-permisos.md` | Qué puede hacer cada rol (Administrador, Técnico) |
| Mockups y guía de nomenclatura | `diseno/Mockups/`, `diseno/06-guia-nomenclatura-mockups.md` | Apariencia de cada pantalla y nombres de los controles |
| Requisitos no funcionales | `catalogo/08-requisitos-no-funcionales.md` | Rendimiento, seguridad, procesos programados (RNF-0010), stack (RNF-0008) |
| Plan de pruebas | `pruebas de software/09-pruebas-software.md` | Las pruebas que cada módulo debe pasar |
| Registro de pendientes y decisiones | `seguimiento/11-registro-pendientes.md` | Decisiones vigentes (DEC) y preguntas abiertas (PEN) |
| Mapa del repositorio | `architecture.md` | Dónde está cada artefacto y cuál manda si dos discrepan |

**Stack (RNF-0008 v02.00, DEC-028):** C# sobre .NET 10 LTS, WPF con patrón MVVM, SQL Server 2025 Express, publicación autocontenida win-x64 para un único equipo con Windows 11. El acceso a datos usa ADO.NET con `Microsoft.Data.SqlClient`, sin ORM.

Las imágenes `VL_01_Trazabilidad.png` y `Vista Logica.png` son solo para exposición; para implementar se usan los `.puml` (DEC-024).

## 2. Cómo se organiza el equipo

Cada integrante es dueño de módulos completos, de la pantalla a la base de datos, en lugar de dividir el equipo en backend y frontend. Además, cinco roles transversales se encargan de las piezas comunes: líder técnica, datos, interfaz, calidad y documentación de usuario.

Por qué por módulo y no por capa:

- **El stack es nuevo para varios.** Quien hace un módulo completo aprende WPF, MVVM y SQL Server de punta a punta. Separado por capas, cada uno aprende media aplicación.
- **Menos esperas.** Con capas separadas, la pantalla espera al servicio y el servicio espera al repositorio. Con módulos completos, cada persona termina algo que se ejecuta y se prueba sola.
- **La arquitectura lo permite.** Los módulos comparten datos a través de `DB_FARMASIL` y se llaman entre sí solo por interfaces (`ILotes`, `IPagos`, `IRestricciones`, `ISesion` en VL-04). Si esas interfaces y el script de la base existen desde el sprint 1, todos pueden avanzar en paralelo usando datos de prueba.
- **Responsabilidad clara.** Cada issue tiene un solo dueño y un módulo terminado se puede demostrar.

Cada módulo sigue las mismas cuatro capas de la vista lógica, y el módulo de referencia (Usuarios) las deja como plantilla:

1. **Vista:** ventana XAML, siguiendo el mockup y la guía de nomenclatura.
2. **VistaModelo:** propiedades enlazadas y comandos de los botones (`ICommand`).
3. **Modelo:** servicio con las reglas de las especificaciones, más las entidades.
4. **Persistencia:** repositorio con las consultas SQL y la unidad de trabajo para las transacciones.

La solución tiene un proyecto por cada una de esas cuatro capas y un proyecto de pruebas, en formato `.slnx` (decidido en el #53). Dentro de cada proyecto, el código se agrupa en una carpeta por módulo.

## 3. Roles y responsabilidades

Hay ocho roles. Seis tienen módulos asignados; los de calidad y de pruebas exploratorias recorren todos. La columna Integrante registra la asignación vigente; el perfil sugerido sirvió para elegir, pero no es requisito saber el stack.

| Rol | Integrante | Responsabilidad transversal | Módulos | Perfil sugerido |
| --- | --- | --- | --- | --- |
| R1. Líder técnica e integradora | Leyni | Estructura de la solución, clases base MVVM, interfaces entre módulos, revisión y fusión de PR, coherencia del catálogo | Usuarios (USR, referencia), Ventas (VEN) | Conoce el catálogo y la arquitectura completos |
| R2. Responsable de datos | Jael | Script de `DB_FARMASIL` desde el diccionario, datos de prueba (PEN-009), patrón de baja condicional con `XACT_ABORT OFF` y punto de guardado (DEC-009, DEC-029) | Inventario (INV), Lotes (LOT, regla FEFO) | Le gusta SQL y el modelado de datos |
| R3. Responsable de interfaz y facilitación | Franco | Ventana principal y menú por rol, estilos compartidos, modal de confirmación genérico, controles comunes; tablero, actas y seguimiento diario | Métodos de pago (PAG), Restricciones de venta (RES) | Buen ojo para diseño; organizado |
| R4. Pruebas exploratorias y documentación de usuario | Raul | Pruebas exploratorias desde la pantalla de cada módulo fusionado, registro de errores como issues, manual de usuario y material de capacitación | Ninguno: recorre todos desde la pantalla | Ordenado y buen redactor; piensa como usuario |
| R5. Desarrollo, familia proveedores | Oscar | Interfaz `IProveedores` | Proveedores (PRV), Devoluciones (DEV), Comparación de precios (COT) | Cualquiera |
| R6. Desarrollo, familia documentos | Fernando | Generación de archivos (PDF y hoja de cálculo), compartida por DOC y REP | Documentación tributaria (DOC), Reportes de ventas (REP) | Cualquiera; mejor si ya generó PDF desde código |
| R7. Desarrollo, alertas y procesos programados | Jhoer | Procesos en segundo plano de RNF-0010 (vencimientos diario, reabastecimiento cada 15 días) | Alertas de vencimiento (ALV), Reabastecimiento (REA) | Cualquiera; mejor si ya trabajó con hilos o tareas asíncronas |
| R8. Responsable de calidad y entrega | Gustavo | Proyecto de pruebas, CI en GitHub Actions, pruebas del plan 09, pruebas de integración, instalador autocontenido, guion de la prueba de aceptación | Ninguno: prueba todos | Detallista; aplica lo de Pruebas de Software |

Reglas del reparto:

- Cada módulo tiene un solo dueño, que responde por él aunque reciba ayuda.
- R8 hace la primera revisión de cada PR contra el plan de pruebas; R1 da la aprobación final, que es obligatoria para fusionar. Así R1 solo revisa arquitectura y coherencia, y no se convierte en cuello de botella.
- Si un integrante se atrasa, su módulo pasa al rol con menos carga en ese sprint, normalmente R3 o R8. La decisión se toma en la revisión del sprint, no al final.

## 4. Reparto de los módulos y dependencias

Los 13 módulos se terminan antes del cierre del sprint 4 (5 de diciembre) para dejar dos semanas a integración y puesta en producción. La prioridad dice qué se protege si el tiempo no alcanza (sección 9).

| Módulo | Dueño | Prioridad | Complejidad | Depende de | Sprint objetivo |
| --- | --- | --- | --- | --- | --- |
| Usuarios (USR) | R1 | 1. Núcleo | Baja | — | S1 |
| Proveedores (PRV) | R5 | 1. Núcleo | Baja | — | S2 |
| Inventario (INV) | R2 | 1. Núcleo | Media | — | S2 |
| Métodos de pago (PAG) | R3 | 1. Núcleo | Baja | — | S2 |
| Lotes (LOT) | R2 | 1. Núcleo | Media | INV, PRV | S3 |
| Ventas (VEN) | R1 | 1. Núcleo | Alta | LOT, PAG, RES, USR | S2 a S4 |
| Restricciones de venta (RES) | R3 | 2. Cumplimiento | Baja | INV | S3 |
| Alertas de vencimiento (ALV) | R7 | 2. Cumplimiento | Media | LOT | S2 a S3 |
| Documentación tributaria (DOC) | R6 | 2. Cumplimiento | Alta | VEN | S2 a S3 |
| Devoluciones a proveedores (DEV) | R5 | 3. Gestión | Media | LOT, PRV | S3 |
| Reportes de ventas (REP) | R6 | 3. Gestión | Alta | VEN, USR | S4 |
| Reabastecimiento (REA) | R7 | 3. Gestión | Media | INV, LOT | S4 |
| Comparación de precios (COT) | R5 | 3. Gestión | Media | REA, PRV | S4 |

Un módulo no espera a que su dependencia esté terminada. Trabaja contra la interfaz definida en el sprint 1 y contra los datos de prueba de la base. Por ejemplo, DOC puede construirse en S2 sobre ventas cargadas por script, aunque la pantalla de Ventas aún no exista. La dependencia solo se vuelve real en las pruebas de integración.

## 5. Plan de aprendizaje del stack

El aprendizaje ocurre en el sprint 1 y sobre código real: nadie hace ejercicios descartables. El primer issue de cada integrante es la operación de consulta de su propio módulo, copiando el patrón del módulo de referencia (Usuarios).

1. **Instalación (domingo 8 a lunes 9).** Visual Studio 2026 Community con la carga de trabajo de escritorio .NET, SDK de .NET 10, SQL Server 2025 Express, SQL Server Management Studio y Git. Cada integrante confirma en el grupo que la solución compila y que la base se crea con el script.
2. **Recorrido del esqueleto (miércoles 11, unas 2 horas, grabado).** R1 muestra cómo nace una pantalla en las cuatro capas. Ejemplo: la consulta de Usuarios, desde el botón en XAML hasta la consulta SQL, pasando por el enlace de datos, el comando, el servicio y el repositorio. R2 muestra el script de la base y los datos de prueba, y R8 cómo correr las pruebas.
3. **Primer issue (jueves 12 a sábado 14).** Cada integrante implementa la consulta de su primer módulo y abre su primer PR. El primer PR de cada persona se revisa en una llamada corta, no solo por escrito.
4. **Parejas en los módulos difíciles (sprints 2 y 3).** R1 acompaña a R6 en DOC y R2 acompaña a R7 en ALV, porque ambos módulos dependen de piezas de datos y de transacciones.

Conceptos mínimos que todos deben manejar al terminar el sprint 1:

| Concepto | Dónde aparece en FARMASIL |
| --- | --- |
| `DataContext` y enlace de datos (`Binding`) | Cada vista se conecta a su VistaModelo |
| `INotifyPropertyChanged` | La pantalla se actualiza al cambiar un valor (total de la venta, stock) |
| `ICommand` | Cada botón (`<MOD>-BTN-...`) ejecuta un comando, sin código en el code-behind |
| Transacción y unidad de trabajo | Registrar una venta descuenta varias remesas en una sola transacción (VP-01) |
| Clave foránea y baja condicional | Eliminar un registro con referencias lo da de baja lógica (DEC-009, VP-B) |
| Consulta parametrizada | Toda consulta SQL usa parámetros, nunca texto concatenado |

## 6. Calendario de los 6 sprints

Los sprints duran de domingo a sábado: S1 a S4 son desarrollo, S5 es integración y S6 es puesta en producción. Cada celda es el trabajo de ese rol en esa semana y se convierte en uno o más issues del tablero.

| Sprint | R1 Líder técnica | R2 Datos | R3 Interfaz y facilitación | R4 Pruebas exploratorias y manual | R5 Proveedores | R6 Documentos | R7 Alertas y procesos | R8 Calidad y entrega |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **S1** 8–14 nov | Estructura de la solución, clases base, interfaces entre módulos; USR completo; adecuar USR y VEN; documentar el cambio de Ventas pedido por la dueña | Script de `DB_FARMASIL` y datos de prueba; adecuar INV y LOT; consulta INV | Ventana principal, menú por rol, estilos, modal de confirmación; adecuar PAG; consulta PAG | Guía de pruebas exploratorias por módulo; estructura del manual de usuario | Adecuar PRV; consulta PRV | Adecuar DOC; prueba de concepto de PDF; consulta DOC | Adecuar ALV; consulta ALV | CI en GitHub Actions y proyecto de pruebas; pruebas de USR |
| **S2** 15–21 nov | VEN: carrito y VistaModelo contra interfaces simuladas | INV completo | PAG completo; adecuar RES | Pruebas exploratorias de USR, PRV, INV y PAG; errores registrados como issues | PRV completo; adecuar DEV | DOC: alta y consulta sobre ventas de prueba | ALV: reglas de alerta y productos retirados | Pruebas de PRV, INV y PAG |
| **S3** 22–28 nov | VEN con LOT, PAG y RES reales: FEFO y transacción única (VP-01) | LOT completo: FEFO, estados (EST-01), `ILotes` | RES completo | Pruebas exploratorias de LOT, RES, DEV, ALV y DOC; manual del núcleo | DEV completo (EST-02, DEC-019); adecuar COT | DOC: previsualización y PDF; adecuar REP | Proceso diario de vencimientos con aviso a la interfaz (RNF-0010, DEC-021); adecuar REA | Pruebas de LOT, RES, DEV, ALV y DOC; primera prueba de venta completa |
| **S4** 29 nov–5 dic | VEN: actualización y baja; integración del núcleo | Apoyo a VEN y REA en stock consolidado; script de datos iniciales reales | Consistencia visual de todas las pantallas | Pruebas exploratorias de VEN, REP, REA y COT; manual de los módulos restantes | COT: comparación y CSV por proveedor (PEN-002) | REP completo | REA y proceso de reabastecimiento cada 15 días | Pruebas de integración; pruebas de REP, REA y COT |
| **S5** 6–12 dic | Integración, corrección de errores, revisión final | Carga de datos reales con la dueña; respaldo según la Vista Física | Correcciones de interfaz; apoyo al manual de usuario | Manual de usuario final y material de capacitación | Corrección de errores; apoyo a pruebas | Corrección de errores; apoyo a pruebas | Corrección de errores; apoyo a pruebas | Instalador autocontenido win-x64; prueba en equipo limpio; regresión completa |
| **S6** 13–19 dic | Puesta en producción y entrega formal | SQL Server y base en el equipo de la farmacia | Capacitación a la dueña y a la técnica | Apoyo en la capacitación; registro de observaciones de la prueba de aceptación | Correcciones de la prueba de aceptación | Correcciones de la prueba de aceptación | Correcciones de la prueba de aceptación; acta de cierre | Prueba de aceptación con la dueña |

El tablero nuevo reemplaza a los sprints anteriores sin desarrollo. Tiene seis iteraciones (S1 a S6) con estas fechas, un issue por celda y cada issue asignado a su rol.

## 7. Adecuación del catálogo a los mockups

El catálogo se adecua a los mockups módulo por módulo, a cargo del dueño de cada uno, antes de programarlo. No se hace como una etapa previa que detenga a todo el equipo. La revisión del 1 de noviembre (`seguimiento/12-revision-mockups.md`) lista 120 discrepancias por módulo, y la auditoría de mockups y catálogo (`seguimiento/14-auditoria-mockups-*.md`, DEC-042 a DEC-054) las consolida; cada dueño parte de la sección de su módulo.

**Criterios generales (C1 a C4, DEC-042):**

1. **Una pantalla por módulo.** Cada mockup `ART-MKP-<MOD>-0001` reúne consulta, alta, actualización y baja. Las especificaciones citan ese mockup en lugar de los `0002` a `0004`, que no existen y no se diseñarán.
2. **Un identificador por componente.** Cuando un mismo ID rotula dos controles distintos (por ejemplo, el buscador y el catálogo de Ventas), se asigna un ID propio a cada uno según la guía de nomenclatura. El nombre corregido se usa en el mockup, en la especificación y en el `x:Name` del XAML.
3. **Lo que aparece en el mockup sin respaldo en una especificación queda fuera de alcance**, salvo que la dueña lo pida. Esto incluye el visor 3D, las indicaciones y sugerencias, el envío a OSE/SUNAT, el historial de eventos, el tiempo de entrega y el botón Generar Lista. Los filtros de búsqueda adicionales sí se aceptan y se agregan a la especificación de consulta.
4. **Si la especificación pide algo que el mockup no muestra, manda la especificación.** La pantalla WPF lo incluye y el mockup no se rehace.

**Procedimiento por módulo:**

- [ ] El dueño aplica los cuatro criterios a las filas de su módulo en el informe 12 y en la auditoría 14.
- [ ] Actualiza las ilaciones y especificaciones del módulo y, si aparece un campo nuevo, el diccionario de datos, con nueva versión y fecha.
- [ ] Lo que no se resuelva con los criterios se anota como pendiente (PEN) en `seguimiento/11-registro-pendientes.md`, sin inventar la respuesta.
- [ ] R1 revisa la coherencia entre EDU, ILA, ESP y el diccionario antes de fusionar.

**Plazos:** los seis módulos del núcleo (USR, PRV, INV, LOT, PAG, VEN) quedan adecuados el miércoles 11 de noviembre. Los demás se adecuan en la semana anterior a su sprint objetivo.

**Aprobación de la dueña:** la dueña revisó los mockups y solo pidió un cambio importante, en Ventas. Los demás módulos se mantienen como están (PEN-006). El cambio de Ventas se documenta primero en su educción y sus ilaciones, y después en las especificaciones, antes de programar VEN.

## 8. Definición de terminado y forma de trabajo

Un módulo está terminado cuando sus cuatro operaciones pasan las pruebas del plan 09 en CI y R1 lo fusionó en `main`. Que "funcione en mi máquina" no cuenta como terminado.

**Definición de terminado de un módulo:**

- [ ] Las cuatro operaciones (consulta, alta, actualización, baja) cumplen sus especificaciones, incluidos los mensajes y las validaciones.
- [ ] Los controles usan los nombres de la guía de nomenclatura, ya corregidos en la adecuación (sección 7).
- [ ] Los permisos por rol respetan la matriz de permisos.
- [ ] Las pruebas del módulo en `09-pruebas-software.md` están automatizadas y pasan en GitHub Actions.
- [ ] No hay lógica en el code-behind y no hay consultas SQL armadas concatenando texto.
- [ ] Pasó la CI, R8 revisó el PR y R1 lo aprobó y fusionó.

**Definición de terminado de un issue** (la casilla «Cumple la definición de terminado del plan de implementación (sección 8)» de cada issue se refiere a esta lista):

- [ ] Se cumplen todos los criterios de aceptación del issue.
- [ ] La rama sigue la convención `<tipo>/<número-issue>-<descripcion-corta>` y cada commit lleva `Refs #<número>`.
- [ ] El PR lleva `Closes #<número>` y pasan las comprobaciones `build-and-test` e `issue-link`.
- [ ] Si el issue cambia código: no hay lógica en el code-behind, toda consulta SQL usa parámetros y el código respeta la capa y la carpeta de su módulo (sección 2).
- [ ] Si el issue cambia una tabla, una interfaz entre módulos o el esquema: se avisó en el grupo antes de abrir el PR y el diccionario de datos se actualizó en el mismo PR (sección 10).
- [ ] Si el issue cambia el catálogo o el diccionario: el documento tiene versión y fecha nuevas, y EDU, ILA, ESP y el diccionario siguen siendo coherentes.
- [ ] Lo que quedó sin decidir está registrado como PEN, y lo que se decidió como DEC, en `seguimiento/11-registro-pendientes.md`.
- [ ] R8 revisó el PR y R1 lo aprobó (los PR de R1 se revisan después de fusionados, sección 10).

**Flujo de un issue** (el detalle está en `CONTRIBUTING.md`):

1. Mover la tarjeta a In Progress y crear la rama `<tipo>/<número-issue>-<descripcion-corta>` desde `main`, con el tipo de `CONTRIBUTING.md` (`feat`, `docs`, `fix`, `chore` o `test`).
2. Hacer commits pequeños con Conventional Commits y `Refs #<número>`.
3. Abrir el PR con `Closes #<número>`. Conviene un PR por operación o por par de operaciones, no uno por módulo entero.
4. R8 revisa contra las pruebas y R1 revisa la arquitectura y la coherencia, en un plazo de 24 horas. Sin la aprobación de R1 y sin la CI en verde, GitHub no permite fusionar.
5. Solo R1 hace el squash & merge.

**Ritmo semanal:**

| Día | Qué se hace | Quién lo conduce |
| --- | --- | --- |
| Domingo | Planificación del sprint (30 min): se eligen los issues de la semana | R3 |
| Lunes a sábado | Reporte diario escrito en el grupo: qué hice, qué haré, qué me bloquea | Cada integrante; R3 consolida |
| Miércoles | Llamada corta de bloqueos (15 min) | R3 |
| Sábado | Revisión del sprint: demostración de lo fusionado y acta | R3, con R1 |

**Reglas que evitan bloqueos:**

- Quien cambie una interfaz entre módulos o una tabla de la base avisa en el grupo antes de abrir el PR.
- Un bloqueo de más de un día se escala en el reporte diario. No se espera a la llamada del miércoles.
- Las dudas sobre una regla de negocio se resuelven en el catálogo, no con una decisión propia en el código. Si el catálogo no la responde, se registra como PEN y se consulta a R1.

## 9. Prioridades y recorte si el tiempo no alcanza

El 5 de diciembre se congela el desarrollo de funcionalidades. Lo que no esté fusionado en esa fecha no entra en la entrega del 19 de diciembre y pasa a una segunda entrega en enero. El núcleo (prioridad 1) no se recorta nunca, porque sin él la farmacia no puede vender.

**Puntos de control:**

| Fecha | Debe estar fusionado | Si no se cumple |
| --- | --- | --- |
| Sábado 14 nov | Esqueleto, base de datos con datos de prueba, CI, USR completo y la consulta de cada módulo | Se recorta la parte de aprendizaje y R1 asigna parejas fijas |
| Sábado 21 nov | PRV, INV y PAG completos | Ayuda de R3 y R8 al módulo atrasado del núcleo |
| Sábado 28 nov | LOT, RES, DEV, ALV y DOC; venta completa con datos de prueba | Se activa el orden de recorte |
| Sábado 5 dic | Los 13 módulos; venta real de punta a punta (VEN + LOT + PAG + DOC) | Lo pendiente pasa a enero |

**Orden de recorte** (se recorta primero lo de arriba):

1. Comparación de precios (COT)
2. Reabastecimiento (REA) y su proceso programado
3. Reportes de ventas (REP)
4. Devoluciones a proveedores (DEV)

Alertas de vencimiento (ALV), Restricciones de venta (RES) y Documentación tributaria (DOC) solo se recortan si el núcleo está en riesgo. ALV es la última en salir, porque evita vender productos vencidos.

## 10. Integración del código y de la base de datos

La integración es continua: cada PR fusionado ya está integrado en `main`, y no existe una fase final de juntar ramas. Se puede trabajar en paralelo porque cada módulo depende solo de dos cosas compartidas: las interfaces entre módulos y el esquema de la base.

**Ramas.** Se crea una rama por issue, no por módulo, y dura de 1 a 3 días. Una rama de módulo que vive tres semanas termina en un conflicto enorme al final. Cada integrante actualiza su rama con `main` al empezar el día (`git pull origin main`).

**Interfaces entre módulos.** R1 define en el sprint 1 todos los contratos que se usan entre módulos (`ILotes`, `IPagos`, `IRestricciones`, `ISesion`, `IProveedores` y los demás que surjan de VL-01). Mientras un módulo no esté listo, quien lo necesita usa una implementación simulada que devuelve datos fijos. Las implementaciones se registran en un solo lugar, al arrancar la aplicación. Cambiar la simulada por la real es cambiar una línea.

**Base de datos como código.** El esquema vive en el repositorio como scripts SQL, no en la máquina de nadie:

| Archivo | Contenido | Dueño |
| --- | --- | --- |
| `database/001_esquema.sql` | Las 16 tablas del diccionario, con claves, restricciones `CHECK` y `UNIQUE`, y las vistas `VW_STOCK_FISICO` y `VW_STOCK_VENDIBLE` (DEC-056, DEC-058) | R2 |
| `database/002_datos_prueba.sql` | Datos de prueba para todos los módulos (PEN-009) | R2 |
| `database/recrear.ps1` | Borra y vuelve a crear la base local con los dos scripts; recibe el nombre de la base para crear también `DB_FARMASIL_PRUEBAS` | R2 |

- Cada integrante tiene su propio SQL Server Express local y recrea la base cuando lo necesita. Nadie comparte una base remota.
- Un cambio de esquema se hace en un PR propio, que actualiza el script y el diccionario de datos en el mismo commit. Lo revisan R2 y R1, y se avisa en el grupo antes de fusionarlo.
- Hasta la puesta en producción, el esquema se corrige dentro de `001_esquema.sql`. Desde la producción, cada cambio es un script nuevo numerado (`003_...`), porque la base de la farmacia ya tendrá datos reales.
- Antes de escribir `001_esquema.sql` se auditó la normalización del diccionario de datos; sus observaciones se aplicaron en el #63 (Diccionario de Datos v10.00).
- Cada módulo escribe solo en sus propias tablas; lo que necesita de una tabla ajena lo pide por la interfaz de su dueño, salvo las lecturas cruzadas que admite DEC-055.
- La aplicación se autentica ante SQL Server con la cuenta de Windows. Esta forma es provisional (PEN-035, registrado en el #53) y se confirma antes de la puesta en producción.

**Hitos de integración:**

| Fecha | Qué queda integrado en `main` |
| --- | --- |
| Sábado 14 nov | Solución que compila con todos los módulos, base creada por script, implementaciones simuladas para todas las interfaces, CI en verde |
| Sábado 28 nov | Núcleo sin simulaciones: venta real con LOT, PAG y RES |
| Sábado 5 dic | Ninguna implementación simulada; pruebas de integración en CI |
| Sábado 12 dic | Instalador probado en un equipo limpio con los datos reales de la farmacia |

**Revisión de los PR de R1.** R1 fusiona sus propios PR cuando la CI pasa, sin esperar aprobación. R8 los revisa después de fusionados, dentro de las 48 horas, y lo que encuentre se registra como issue.
