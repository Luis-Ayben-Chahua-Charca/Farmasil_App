# Guía de pruebas exploratorias

| Artefacto | Guía de pruebas exploratorias e índice del manual de usuario |
| --- | --- |
| Versión | 01.00 |
| Fecha | 16/11/2026 |
| Responsable | R4 — Pruebas exploratorias y documentación de usuario |

## Historial de versiones

| Versión | Fecha | Cambio |
| --- | --- | --- |
| 01.00 | 16/11/2026 | Primera versión (issue #78): lista común de revisión por pantalla, misiones de exploración de Inventario, Proveedores y Usuarios, plantillas de sesión y de defectos, e índice del manual de usuario por módulo. |

## Propósito

Esta guía ayuda a revisar FARMASIL desde la perspectiva de quien usa sus pantallas. Tiene dos partes: una [lista común](#lista-comun-de-revision) de qué revisar en cualquier pantalla, que se aplica a cada módulo cuando se fusiona su pantalla, y las misiones de exploración de las pantallas que ya tienen una vista WPF y un VistaModelo en el repositorio, además de la pantalla inicial provisional. Las especificaciones y el plan de pruebas sirven para definir qué observar; los mockups solo ayudan a contrastar la apariencia y los rótulos. El alcance se contrastó con el [mapa del repositorio](../../architecture.md), el [contexto de arquitectura y módulos](../../docs/arquitectura/Contexto_Arquitectura_FARMASIL_ParaAgentesIA.md) y las responsabilidades de R4 en el [plan, sección 3](../../docs/plan/plan-implementacion.md#3-roles-y-responsabilidades).

## Cómo usar la guía

1. Registra una sesión con la plantilla de este documento.
2. Confirma los datos iniciales y el entorno antes de abrir una pantalla.
3. Recorre la [lista común de revisión](#lista-comun-de-revision) y después la misión del módulo; registra tanto los resultados correctos como las diferencias.
4. Si una pantalla no está disponible desde la aplicación, anota el bloqueo; no supongas navegación ni funciones que no estén implementadas.
5. Informa los defectos con el formato de la sección [Reporte de defectos](#reporte-de-defectos).

## Índice

- [Alcance y evidencia](#alcance-y-evidencia)
- [Entorno de pruebas](#entorno-de-pruebas)
- [Lista común de revisión](#lista-comun-de-revision)
- [Pantalla inicial provisional](#pantalla-inicial-provisional)
- [Inventario (INV)](#inventario-inv)
  - [Consulta de productos](#consulta-de-productos-esp-0006)
- [Proveedores (PRV)](#proveedores-prv)
  - [Consulta de proveedores](#consulta-de-proveedores-esp-0038)
- [Usuarios (USR)](#usuarios-usr)
  - [Gestión de cuentas](#gestion-de-cuentas-esp-0033-a-esp-0036)
- [Índice del manual de usuario](#indice-del-manual-de-usuario)
- [Plantilla de sesión](#plantilla-de-sesion)
- [Reporte de defectos](#reporte-de-defectos)
- [Fuentes y diferencias por confirmar](#fuentes-y-diferencias-por-confirmar)

## Alcance y evidencia

En la capa Vista están presentes `InicioView`, `InventarioView`, `ProveedoresView` y `UsuariosView`. Inventario y Proveedores muestran solo la consulta; no se incluyen sus altas, actualizaciones ni bajas, aunque esas operaciones aparezcan en especificaciones o mockups. Usuarios reúne sus cuatro operaciones (ESP-0033 a ESP-0036). Los nombres de las pantallas se confirman en [Plantillas.xaml](../../src/FarmasilApp.Vista/Comun/Plantillas.xaml) y su implementación en las vistas [InventarioView.xaml](../../src/FarmasilApp.Vista/INV/InventarioView.xaml), [ProveedoresView.xaml](../../src/FarmasilApp.Vista/PRV/ProveedoresView.xaml), [UsuariosView.xaml](../../src/FarmasilApp.Vista/USR/UsuariosView.xaml) e [InicioView.xaml](../../src/FarmasilApp.Vista/Principal/InicioView.xaml).

**Límites de acceso comprobados:** la aplicación inicia en `InicioView`, cuyo texto es provisional. No se encontró una pantalla de inicio de sesión ni un menú visible para abrir consultas; el menú por rol llega con la ventana principal (#74) y el inicio de sesión, con el #88. `ProveedoresViewModel` y `UsuariosViewModel` están registrados en [RegistroDependencias.cs](../../src/FarmasilApp.VistaModelo/Comun/RegistroDependencias.cs), pero no hay una ruta visible desde Inicio. `InventarioViewModel` todavía no figura en ese registro. Si no se puede llegar a una pantalla, registra el bloqueo como hallazgo y no des por disponible un flujo navegable.

No se encontró una vista WPF para los otros módulos del catálogo (`VEN`, `DOC`, `ALV`, `PAG`, `REP`, `DEV`, `RES`, `LOT`, `REA` y `COT`). Sus mockups, especificaciones, servicios o simulaciones no bastan para afirmar que sus pantallas estén implementadas; por ello esta guía no describe sus flujos.

## Entorno de pruebas

| Aspecto | Información documentada |
| --- | --- |
| Stack | C# sobre .NET 10 LTS, WPF con MVVM y SQL Server 2025 Express. El acceso a datos usa ADO.NET con `Microsoft.Data.SqlClient`, sin ORM. |
| Equipo | Windows 11; la aplicación y la instancia `SQLEXPRESS` se ejecutan en el mismo equipo. |
| Base | Las sesiones exploratorias usan `DB_FARMASIL_PRUEBAS`, creada con `database/recrear.ps1 -Base DB_FARMASIL_PRUEBAS`, que también carga los datos de prueba. Nunca se explora sobre una base con datos reales. |
| Configuración de prueba | Para que la aplicación apunte a `DB_FARMASIL_PRUEBAS`, el README describe `src/FarmasilApp.Vista/appsettings.local.json`; ese archivo local no se versiona. |
| Cuentas de prueba | `admin_prueba` ('Administrador') y `tecnico_prueba` ('Tecnico'), cargadas por `database/002_datos_prueba.sql`, donde está documentada su contraseña. Se usan cuando exista el inicio de sesión (#88). |
| Herramientas | SDK de .NET 10 (10.0.100 o posterior de la serie 10.0) y Git. Visual Studio 2026 Community con «Desarrollo de escritorio de .NET» es recomendado; VS Code con C# Dev Kit o Rider son alternativas. |
| Comprobaciones documentadas | `dotnet build FarmasilApp.slnx --configuration Release` y `dotnet test FarmasilApp.slnx --configuration Release --no-build`. |

Referencias: [README.md](../../README.md), [CONTRIBUTING.md](../../CONTRIBUTING.md) y [RNF-0008](../../catalogo/08-requisitos-no-funcionales.md). Usa una base de pruebas, no una base de operación. Las pruebas de servicio y VistaModelo de Inventario, Proveedores y Usuarios usan repositorios falsos; no confirman que las pantallas consulten una instancia preparada.

<a id="lista-comun-de-revision"></a>
## Lista común de revisión

Se aplica a cualquier pantalla, antes de su misión. Cada punto sale de una regla que ya rige el proyecto; si una pantalla no la cumple, es un hallazgo. Para leer el identificador de un control se usa una herramienta de inspección de UI Automation de Windows, que muestra el `AutomationId` del control bajo el cursor.

| Qué revisar | Qué se espera | Regla |
| --- | --- | --- |
| Identificadores | Cada control que nombra la especificación existe y su `AutomationId` es exactamente el de la [guía de nomenclatura](../../diseno/06-guia-nomenclatura-mockups.md). No hay controles con identificadores inventados. | DEC-060 |
| Búsqueda y filtros | El buscador se llama `<MOD>-SRH-…` y los filtros de lista cerrada `<MOD>-CMB-FILTRO-…`. Cada filtro permite volver a «Todos». Sin criterios, la consulta hace lo que dice su especificación. | DEC-061 |
| Tarjetas y tabla | Las tarjetas (`<MOD>-CRD-…` dentro de `<MOD>-PNL-…`) y la tabla (`<MOD>-TBL-…`) muestran el mismo resultado. Seleccionar una tarjeta o una fila carga la ficha. | DEC-062 |
| Actualización | La actualización tiene dos pasos: el botón que la inicia y `<MOD>-BTN-CONFIRMAR-ACTUALIZACION`, con el rótulo «Guardar cambios». | DEC-046 |
| Baja | La baja pide confirmación en el modal `<MOD>-MDL-CONFIRMAR-ELIMINACION`, con el texto `<MOD>-TXT-MSJ` exacto de la especificación y los botones `<MOD>-BTN-CONFIRMAR-SI` («Sí, Confirmar Acción») y `<MOD>-BTN-CONFIRMAR-NO` («Cancelar»). Escape cancela y Enter no confirma. | DEC-047, [CONTRIBUTING](../../CONTRIBUTING.md) |
| Mensajes | Los mensajes de éxito y de las reglas de negocio son los de la especificación, palabra por palabra. Cuando la especificación no fija el texto, el mensaje es claro y no muestra detalles técnicos (excepciones, trazas, nombres de tablas). | Especificación de la operación |
| Permisos | Con una sesión 'Tecnico', las operaciones que la [matriz de permisos](../../diseno/07-matriz-permisos.md) reserva al Administrador se muestran deshabilitadas e indican el motivo al intentar activarlas; DEC-050 admite también ocultarlas. Anota cuál de los dos comportamientos se observa. | RNF-0006, condición 4; DEC-050 |
| Datos | Una consulta no modifica datos. Un resultado vacío muestra un mensaje, no una pantalla en blanco. Si falla la conexión, la pantalla lo informa sin cerrarse. | Postcondiciones de cada especificación |
| Acceso | La pantalla se abre desde el menú por rol. Si no se puede llegar a ella, se registra como bloqueo. | Plan, sección 3 (R4) |

## Pantalla inicial provisional

La ventana principal presenta la vista seleccionada por `MainViewModel`; el estado inicial comprobable es `InicioView`, con el texto **«FARMASIL: esqueleto de la solución»**. No hay controles de usuario en esta vista. La misión se limita a registrar ese estado inicial y comprobar si la aplicación abre; no incluye acciones de acceso ni navegación que todavía no estén en la interfaz.

## Inventario (INV)

**Misión de exploración:** cuando se pueda abrir la consulta, buscar productos con los criterios disponibles, contrastar las tarjetas con la tabla y seleccionar un producto. Registrar el comportamiento del stock físico, los filtros, el mensaje mostrado y cualquier dificultad para llegar a la pantalla. Esta misión se apoya en `ESP-0006` y en las particiones `T-001`, `T-002`, `T-006` y `T-007` del plan de pruebas.

### Consulta de productos (ESP-0006)

| Estado o datos previos | Qué explorar y resultado documentado |
| --- | --- |
| La especificación requiere `DB_FARMASIL` disponible, conexión exitosa, sesión activa con rol `'Administrador'` o `'Tecnico'`, y `DB_FARMASIL.TBL_PRODUCTOS` no vacía. La aplicación actual inicia en la pantalla provisional y no ofrece inicio de sesión ni navegación visible; esas precondiciones no se pueden completar desde el flujo actual. | Buscar por identificador o nombre, aplicar opcionalmente proveedor y acción terapéutica, y ejecutar `Buscar`. El plan de pruebas considera identificador existente/inexistente, nombre parcial, cada filtro, combinación de filtros y filtros vacíos. |
| Para datos, usar productos existentes en la base de pruebas. La pantalla muestra `id_producto`, `nombre`, `accion_terapeutica`, `precio_venta`, `estado_producto` y `stock_fisico`. | Comparar el resultado de `INV-PNL-CATALOGO` con `INV-TBL-PRODUCTOS`. La especificación espera el mismo catálogo con stock físico; un producto sin remesas muestra 0. La consulta no debe modificar `TBL_PRODUCTOS`. |
| Una consulta devuelve cero o más resultados. | Seleccionar una tarjeta o una fila y comprobar que la ficha muestre el producto elegido. El código enlaza también la selección de la tabla a la ficha. |

**Controles visibles en la vista WPF**

| Nombre exacto | Rótulo o uso visible |
| --- | --- |
| `INV-SRH-PRODUCTO` | Campo «Producto»; el tooltip indica búsqueda por ID o nombre. |
| `INV-CMB-FILTRO-PROVEEDOR` | Filtro «Proveedor». |
| `INV-CMB-FILTRO-ACCION-TERAPEUTICA` | Filtro «Acción terapéutica». |
| `INV-BTN-LEER-PRODUCTO` | Botón «Buscar». |
| `INV-PNL-CATALOGO` | Catálogo en tarjetas. |
| `INV-CRD-PRODUCTO` | Tarjeta de producto. |
| `INV-BTN-SELECCIONAR-PRODUCTO` | Botón «Seleccionar» de la tarjeta. |
| `INV-TBL-PRODUCTOS` | Tabla con ID, Producto, Acción terapéutica, Precio (S/), Estado y Stock físico. |
| Sin identificador AutomationId en XAML | La ficha del producto y el texto de estado/mensaje no tienen un identificador AutomationId propio en la vista actual. |

**Mensajes del VistaModelo**

| Situación | Mensaje comprobable en código |
| --- | --- |
| Hay resultados | `Productos encontrados: {cantidad}.` |
| Sin resultados | `No se encontraron productos.` |
| Falla la consulta | `No se pudo consultar el inventario. Inténtelo nuevamente.` |
| Falla la carga de filtros | Se antepone `No se pudieron cargar los filtros del inventario.` al mensaje de la consulta. |

**Riesgos documentados**

| Riesgo | Observación para la sesión |
| --- | --- |
| `ILA-0006-R01` | El riesgo afirma que la búsqueda por nombre es de igualdad exacta; la versión vigente de `ESP-0006`, el plan de pruebas y el repositorio describen coincidencia parcial. La diferencia sigue registrada como pendiente en riesgos: anotar el resultado observado, sin resolver la discrepancia en esta guía. |
| `ILA-0006-R02` | El riesgo propone un indicador visual de alertas de vencimiento; `ESP-0006` no lo define y la vista actual no lo muestra. Registra lo observado sin tratar ese indicador como resultado obligatorio. |
| `ILA-0006-R03` | Riesgo pendiente por falta de un límite de registros/paginación documentado. Registrar problemas observables con catálogos grandes, sin asumir un límite esperado. |

Fuentes: [ESP-0006](../../catalogo/03-especificaciones.md), [guía de nomenclatura](../../diseno/06-guia-nomenclatura-mockups.md), [permisos](../../diseno/07-matriz-permisos.md), [pruebas de Inventario](<../../pruebas de software/09-pruebas-software.md>), [riesgos de Inventario](../../riesgos/10-gestion-riesgos.md), [InventarioViewModel.cs](../../src/FarmasilApp.VistaModelo/INV/InventarioViewModel.cs), [InventarioServicio.cs](../../src/FarmasilApp.Modelo/INV/InventarioServicio.cs), [InventarioRepositorio.cs](../../src/FarmasilApp.Persistencia/INV/InventarioRepositorio.cs), [pruebas del servicio](../../tests/FarmasilApp.Pruebas/INV/InventarioServicioPruebas.cs) y [pruebas del VistaModelo](../../tests/FarmasilApp.Pruebas/VistaModelo/InventarioViewModelPruebas.cs).

## Proveedores (PRV)

**Misión de exploración:** cuando se pueda abrir la consulta como Administrador, localizar proveedores por RUC o razón social, aplicar el filtro de estado, comparar tarjetas y tabla, y seleccionar un resultado para revisar su ficha. Registrar los mensajes y cualquier bloqueo de acceso. La misión se limita a `ESP-0038` y a las pruebas existentes de servicio y VistaModelo.

### Consulta de proveedores (ESP-0038)

| Estado o datos previos | Qué explorar y resultado documentado |
| --- | --- |
| La especificación requiere `DB_FARMASIL` disponible, conexión exitosa, sesión activa con rol `'Administrador'` y `DB_FARMASIL.TBL_PROVEEDORES` no vacía. La pantalla de inicio actual no proporciona sesión ni ruta visible hasta Proveedores. | Buscar por RUC exacto o parte de la razón social. El filtro opcional acepta `Activo` o `Inactivo`; sin criterios, la especificación y el plan de pruebas esperan listar todos. |
| Usar proveedores existentes en la base de pruebas. El resultado contiene RUC, razón social, teléfono y estado; `id_proveedor` es una clave de selección no visible. | Comparar `PRV-PNL-PROVEEDORES` y `PRV-TBL-PROVEEDORES`: ambos deben representar el mismo resultado. El teléfono figura en la tabla. |
| La vista permite seleccionar una tarjeta; la tabla también enlaza la selección. | Confirmar que la ficha muestre razón social, RUC, teléfono y estado del proveedor seleccionado. La selección vuelve a nulo después de una nueva consulta según las pruebas del VistaModelo. |

**Controles visibles en la vista WPF**

| Nombre exacto | Rótulo o uso visible |
| --- | --- |
| `PRV-SRH-PROVEEDOR` | Campo «Proveedor»; el tooltip indica búsqueda por RUC o razón social. |
| `PRV-CMB-FILTRO-ESTADO` | Filtro «Estado»; el VistaModelo ofrece «Todos», «Activo» e «Inactivo». |
| `PRV-BTN-LEER-PROVEEDOR` | Botón «Buscar». |
| `PRV-PNL-PROVEEDORES` | Lista de resultados en tarjetas. |
| `PRV-CRD-PROVEEDOR` | Tarjeta con razón social, RUC y estado. |
| `PRV-BTN-SELECCIONAR-PROVEEDOR` | Botón «Seleccionar» de la tarjeta. |
| `PRV-TBL-PROVEEDORES` | Tabla con RUC, Razón social, Teléfono y Estado. |
| Sin identificador AutomationId en XAML | La ficha del proveedor y el texto de estado/mensaje no tienen un identificador AutomationId propio en la vista actual. |

**Mensajes del VistaModelo**

| Situación | Mensaje comprobable en código |
| --- | --- |
| Hay resultados | `Proveedores encontrados: {cantidad}.` |
| Sin resultados | `No se encontraron proveedores.` |
| Falla la consulta | `No se pudo consultar los proveedores. Inténtelo nuevamente.` |

**Riesgo documentado**

`ILA-0038-R01` permanece pendiente y afirma que la consulta exige criterio y usa igualdad exacta. La versión vigente de `ESP-0038`, el plan de pruebas y el repositorio describen búsqueda parcial por razón social y consulta sin criterios. Registra lo observado sin resolver aquí la diferencia entre fuentes.

Fuentes: [ESP-0038](../../catalogo/03-especificaciones.md), [guía de nomenclatura](../../diseno/06-guia-nomenclatura-mockups.md), [permisos](../../diseno/07-matriz-permisos.md), [pruebas de Proveedores](<../../pruebas de software/09-pruebas-software.md>), [riesgos de Proveedores](../../riesgos/10-gestion-riesgos.md), [ProveedoresViewModel.cs](../../src/FarmasilApp.VistaModelo/PRV/ProveedoresViewModel.cs), [ProveedoresServicio.cs](../../src/FarmasilApp.Modelo/PRV/ProveedoresServicio.cs), [ProveedoresRepositorio.cs](../../src/FarmasilApp.Persistencia/PRV/ProveedoresRepositorio.cs), [pruebas del servicio](../../tests/FarmasilApp.Pruebas/PRV/ProveedoresServicioPruebas.cs) y [pruebas del VistaModelo](../../tests/FarmasilApp.Pruebas/VistaModelo/ProveedoresViewModelPruebas.cs).

## Usuarios (USR)

**Misión de exploración:** cuando se pueda abrir la pantalla como Administrador, buscar cuentas, crear una cuenta nueva, modificarla y darla de baja, y comprobar que las reglas de protección de la administración se cumplan: no quedarse sin una cuenta administradora activa y no alterar la cuenta con la que se está trabajando. Registrar cada mensaje y cualquier bloqueo de acceso. La misión se apoya en `ESP-0033` a `ESP-0036`, en el módulo 9 del plan de pruebas y en RNF-0006.

<a id="gestion-de-cuentas-esp-0033-a-esp-0036"></a>
### Gestión de cuentas (ESP-0033 a ESP-0036)

| Estado o datos previos | Qué explorar y resultado documentado |
| --- | --- |
| Las cuatro especificaciones exigen una sesión activa con rol `'Administrador'`. La pantalla está registrada, pero todavía no hay inicio de sesión (#88) ni menú (#74) para llegar a ella. | Con una sesión 'Tecnico', cualquier operación se rechaza con el mensaje de acceso denegado. |
| Consulta (ESP-0034): cuentas de los datos de prueba. El resultado muestra nombre de usuario, rol y estado; `id_usuario` es una clave de selección no visible y la contraseña nunca aparece (DEC-068). | Buscar por nombre de usuario y filtrar por rol; comparar `USR-PNL-CUENTAS` con `USR-TBL-USUARIOS`. Un clic en una tarjeta o en una fila carga la ficha; las tarjetas no tienen botón. |
| Creación (ESP-0033): ficha vacía. | Probar nombre vacío, nombre solo con espacios, nombre repetido escrito con otras mayúsculas, nombre de más de 50 caracteres, contraseña vacía, y rol o estado sin elegir (DEC-074). Comprobar que la contraseña se ve enmascarada. |
| Actualización (ESP-0035): cuenta seleccionada. Al abrir la edición, el campo de contraseña queda vacío. | Guardar sin escribir contraseña conserva la vigente. Degradar o desactivar a la única cuenta administradora activa se cancela. La cuenta de la sesión no puede cambiar su propio rol ni su estado, pero sí su nombre y su contraseña (DEC-073). |
| Baja (ESP-0036): cuenta seleccionada. | Dar de baja la cuenta de la sesión se impide. Con otra cuenta se abre el modal; «Cancelar» o Escape no cambian nada. Una cuenta con ventas, devoluciones o cotizaciones queda 'Inactivo' en lugar de borrarse (DEC-009). |

**Controles visibles en la vista WPF**

| Nombre exacto | Rótulo o uso visible |
| --- | --- |
| `USR-SRH-USUARIO` | Campo «Nombre de usuario» de la búsqueda. |
| `USR-CMB-FILTRO-ROL` | Filtro «Rol», con la opción «Todos». |
| `USR-BTN-LEER-USUARIO` | Botón «Buscar». |
| `USR-PNL-CUENTAS` | Lista «Cuentas de Usuario» en tarjetas. |
| `USR-CRD-USUARIO` | Tarjeta de una cuenta, sin botón: un clic la selecciona. |
| `USR-TBL-USUARIOS` | «Tabla de Usuarios», con Usuario, Rol y Estado. |
| `USR-TXT-NOMBRE-USUARIO` | Campo «Nombre de usuario» de la ficha. |
| `USR-PWD-CONTRASENA` | Campo «Contraseña», enmascarado. |
| `USR-CMB-ROL` | Rol de la cuenta en la ficha. |
| `USR-CMB-ESTADO-USUARIO` | Estado de la cuenta en la ficha. |
| `USR-BTN-CREAR-USUARIO` | Botón «Crear». |
| `USR-BTN-ACTUALIZAR-USUARIO` | Botón «Actualizar», que inicia la edición. |
| `USR-BTN-CONFIRMAR-ACTUALIZACION` | Botón «Guardar cambios». |
| `USR-BTN-ELIMINAR-USUARIO` | Botón «Eliminar», que inicia la baja. |
| `USR-MDL-CONFIRMAR-ELIMINACION`, `USR-TXT-MSJ`, `USR-BTN-CONFIRMAR-SI`, `USR-BTN-CONFIRMAR-NO` | Modal común de confirmación de la baja. |

**Mensajes**

| Situación | Mensaje comprobable en código |
| --- | --- |
| Hay resultados | `Cuentas encontradas: {cantidad}.` |
| Sin resultados | `No se encontraron cuentas.` |
| Cuenta creada | `Usuario registrado correctamente` |
| Cuenta actualizada | `Usuario actualizado correctamente` |
| Se quedaría sin administradora activa | `El sistema debe conservar al menos una cuenta administradora activa` |
| Cambiar el rol o el estado de la cuenta propia | `No es posible cambiar el rol ni el estado de la cuenta con la que se encuentra abierta la sesión` |
| Dar de baja la cuenta propia | `No es posible dar de baja la cuenta con la que se encuentra abierta la sesión` |
| Texto del modal de baja | `¿Está seguro de dar de baja esta cuenta? El histórico de operaciones que registró se conservará.` |
| Cuenta dada de baja | `Cuenta dada de baja correctamente` |
| Validaciones (textos no fijados por la especificación) | `Ingrese el nombre de usuario.`, `El nombre de usuario no puede superar los 50 caracteres.`, `Ya existe una cuenta con ese nombre de usuario.`, `Ingrese la contraseña.`, `Seleccione un rol válido.` y `Seleccione un estado válido.` |
| Sesión 'Tecnico' | `Su rol no permite gestionar usuarios.` |
| Falla técnica | `No se pudo consultar las cuentas. Inténtelo nuevamente.` o `No se pudo completar la operación. Inténtelo nuevamente.` |

**Riesgos documentados**

| Riesgo | Observación para la sesión |
| --- | --- |
| `ILA-0033-R01` | No hay una política mínima de contraseña: la especificación solo exige que no esté vacía. Anota si una contraseña muy corta se acepta, sin tratarlo como defecto. |
| `ILA-0034-R01` | Está pendiente definir si la búsqueda por nombre de usuario es exacta o parcial. Registra cuál se observa. |
| `ILA-0035-R02` | No está definido si un cambio de rol o estado afecta a una sesión ya abierta de esa cuenta. Se podrá observar cuando exista el inicio de sesión (#88). |

Fuentes: [ESP-0033 a ESP-0036](../../catalogo/03-especificaciones.md), [guía de nomenclatura](../../diseno/06-guia-nomenclatura-mockups.md), [permisos](../../diseno/07-matriz-permisos.md), [pruebas de Usuarios](<../../pruebas de software/09-pruebas-software.md>), [riesgos de Usuarios](../../riesgos/10-gestion-riesgos.md), [UsuariosView.xaml](../../src/FarmasilApp.Vista/USR/UsuariosView.xaml), [UsuariosViewModel.cs](../../src/FarmasilApp.VistaModelo/USR/UsuariosViewModel.cs), [MensajesUsuarios.cs](../../src/FarmasilApp.Modelo/USR/MensajesUsuarios.cs), [UsuariosServicio.cs](../../src/FarmasilApp.Modelo/USR/UsuariosServicio.cs), [UsuariosRepositorio.cs](../../src/FarmasilApp.Persistencia/USR/UsuariosRepositorio.cs) y [pruebas del servicio](../../tests/FarmasilApp.Pruebas/USR/UsuariosServicioPruebas.cs).

<a id="indice-del-manual-de-usuario"></a>
## Índice del manual de usuario

El repositorio no contiene todavía un manual de usuario. Este índice fija su estructura: un capítulo por módulo, en el orden del catálogo, con las tareas de sus cuatro especificaciones y el rol que puede hacerlas según la [matriz de permisos](../../diseno/07-matriz-permisos.md). Cada tarea se redacta cuando su pantalla se fusiona y se explora; las marcadas con **(pantalla disponible)** ya tienen vista en el repositorio, aunque todavía no se llegue a ellas desde el menú (#74).

1. **Introducción**: qué es FARMASIL, los dos roles (Administrador y Técnico) y cómo se lee este manual.
2. **Inicio y cierre de sesión** (RNF-0006): ingresar con usuario y contraseña, y qué pasa con una cuenta inactiva. Pendiente del #88.
3. **Ventana principal y menú**: cómo abrir cada módulo y por qué algunas opciones se ven deshabilitadas para el Técnico. Pendiente del #74.
4. **Ventas (VEN)**
   - Registrar una venta (ESP-0001) — Administrador y Técnico.
   - Consultar ventas (ESP-0002) — Administrador y Técnico.
   - Modificar una venta (ESP-0003) — Administrador.
   - Eliminar una venta (ESP-0004) — Administrador.
5. **Inventario (INV)**
   - Registrar un producto (ESP-0005) — Administrador.
   - Consultar el inventario (ESP-0006) — Administrador y Técnico. **(pantalla disponible)**
   - Actualizar un producto (ESP-0007) — Administrador.
   - Dar de baja un producto (ESP-0008) — Administrador.
6. **Documentación tributaria (DOC)**
   - Emitir un comprobante (ESP-0009) — Administrador y Técnico.
   - Consultar comprobantes (ESP-0010) — Administrador y Técnico.
   - Corregir un comprobante (ESP-0011) — Administrador.
   - Anular un comprobante (ESP-0012) — Administrador.
7. **Alertas de vencimiento (ALV)**
   - Consultar las alertas (ESP-0013) — Administrador y Técnico.
   - Registrar una alerta manualmente (ESP-0014) — Administrador y Técnico.
   - Modificar una alerta (ESP-0015) — Administrador.
   - Eliminar una alerta (ESP-0016) — Administrador.
8. **Métodos de pago (PAG)**
   - Registrar un método de pago (ESP-0017) — Administrador.
   - Consultar los métodos de pago (ESP-0018) — Administrador y Técnico.
   - Actualizar un método de pago (ESP-0019) — Administrador.
   - Desactivar un método de pago (ESP-0020) — Administrador.
9. **Reportes de ventas (REP)**: solo Administrador.
   - Generar un reporte (ESP-0021).
   - Consultar los reportes (ESP-0022).
   - Cambiar el intervalo de un reporte (ESP-0023).
   - Eliminar un reporte (ESP-0024).
10. **Devoluciones a proveedores (DEV)**: solo Administrador.
    - Registrar una orden de devolución (ESP-0025).
    - Consultar las órdenes (ESP-0026).
    - Actualizar una orden (ESP-0027).
    - Eliminar una orden (ESP-0028).
11. **Restricciones de venta (RES)**
    - Registrar una restricción (ESP-0029) — Administrador.
    - Consultar las restricciones (ESP-0030) — Administrador y Técnico.
    - Modificar una restricción (ESP-0031) — Administrador.
    - Desactivar una restricción (ESP-0032) — Administrador.
12. **Usuarios (USR)**: solo Administrador. **(pantalla disponible)**
    - Crear una cuenta (ESP-0033).
    - Consultar las cuentas (ESP-0034).
    - Actualizar una cuenta (ESP-0035).
    - Dar de baja una cuenta (ESP-0036).
13. **Proveedores (PRV)**: solo Administrador.
    - Registrar un proveedor (ESP-0037).
    - Consultar los proveedores (ESP-0038). **(pantalla disponible)**
    - Actualizar un proveedor (ESP-0039).
    - Desactivar un proveedor (ESP-0040).
14. **Lotes (LOT)**
    - Registrar una remesa (ESP-0041) — Administrador.
    - Consultar las remesas (ESP-0042) — Administrador y Técnico.
    - Actualizar una remesa (ESP-0043) — Administrador.
    - Dar de baja una remesa (ESP-0044) — Administrador.
15. **Reabastecimiento (REA)**: solo Administrador.
    - Agregar un ítem a la lista de reabastecimiento (ESP-0045).
    - Consultar la lista (ESP-0046).
    - Actualizar un ítem (ESP-0047).
    - Eliminar un ítem (ESP-0048).
16. **Comparación de precios de proveedores (COT)**: solo Administrador.
    - Registrar una cotización (ESP-0049).
    - Comparar precios y exportar por proveedor (ESP-0050).
    - Actualizar una cotización (ESP-0051).
    - Eliminar una cotización (ESP-0052).
17. **Anexos**: mensajes del sistema por módulo y glosario (remesa, stock físico y vendible, comprobante, baja lógica).

<a id="plantilla-de-sesion"></a>
## Plantilla de sesión

```text
Fecha:
Duración:
Participante:
Módulo / pantalla:
Misión:
Entorno (SO, versión de la aplicación, instancia/base):
Datos y estado previo:

Pasos realizados:
1.
2.
3.

Resultado esperado:
Resultado observado:
Hallazgos:
Preguntas / puntos por confirmar:
Capturas o ubicación de evidencias:
```

## Reporte de defectos

Al registrar un defecto como issue, incluye:

```text
Módulo y pantalla:
Pasos para reproducir:
1.
2.
3.
Resultado esperado:
Resultado obtenido:
Evidencia (captura, datos de prueba o registro relevante):
```

Describe pasos concretos y conserva los datos de prueba necesarios para repetir el resultado. No adjuntes credenciales ni datos reales de clientes.

## Fuentes y diferencias por confirmar

- [README.md](../../README.md) informa 257 pruebas; la versión actual del [plan de pruebas](<../../pruebas de software/09-pruebas-software.md>) informa 275. Se conserva la diferencia documental y esta guía no fija un total.
- [CONTRIBUTING.md](../../CONTRIBUTING.md) describe que el esqueleto todavía no se conecta a SQL Server; en el código actual existen repositorios SQL para Inventario y Proveedores. La ejecución contra una base configurada sigue dependiendo del entorno de la sesión.
- Los mockups de [Inventario](../../diseno/Mockups/fuente/ART-MKP-INV-0001.html) y [Proveedores](../../diseno/Mockups/fuente/ART-MKP-PRV-0001.html) muestran más controles y operaciones que las vistas WPF actuales. Para esta guía prevalecen las pantallas implementadas y las especificaciones vigentes, no los flujos que solo aparecen en HTML.
- Los roles y permisos se documentan en la [matriz de permisos](../../diseno/07-matriz-permisos.md). Como la aplicación actual no permite iniciar sesión desde su interfaz, la autorización en ejecución queda **Por confirmar** en una exploración de usuario.
