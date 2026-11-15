# Guía de pruebas exploratorias

## Propósito

Esta guía ayuda a revisar FARMASIL desde la perspectiva de quien usa sus pantallas. Describe solo las pantallas de consulta que tienen una vista WPF y un VistaModelo en el repositorio, además de la pantalla inicial provisional. Las especificaciones y el plan de pruebas sirven para definir qué observar; los mockups solo ayudan a contrastar la apariencia y los rótulos. El alcance se contrastó con el [mapa del repositorio](../../architecture.md), el [contexto de arquitectura y módulos](../../docs/arquitectura/Contexto_Arquitectura_FARMASIL_ParaAgentesIA.md) y las responsabilidades de R4 en el [plan, sección 3](../../docs/plan/plan-implementacion.md#3-roles-y-responsabilidades).

## Cómo usar la guía

1. Registra una sesión con la plantilla de este documento.
2. Confirma los datos iniciales y el entorno antes de abrir una pantalla.
3. Sigue la misión del módulo y registra tanto los resultados correctos como las diferencias.
4. Si una pantalla no está disponible desde la aplicación, anota el bloqueo; no supongas navegación ni funciones que no estén implementadas.
5. Informa los defectos con el formato de la sección [Reporte de defectos](#reporte-de-defectos).

## Índice

- [Alcance y evidencia](#alcance-y-evidencia)
- [Entorno de pruebas](#entorno-de-pruebas)
- [Pantalla inicial provisional](#pantalla-inicial-provisional)
- [Inventario (INV)](#inventario-inv)
  - [Consulta de productos](#consulta-de-productos-esp-0006)
- [Proveedores (PRV)](#proveedores-prv)
  - [Consulta de proveedores](#consulta-de-proveedores-esp-0038)
- [Índice propuesto del manual de usuario](#indice-propuesto-del-manual-de-usuario)
- [Plantilla de sesión](#plantilla-de-sesion)
- [Reporte de defectos](#reporte-de-defectos)
- [Fuentes y diferencias por confirmar](#fuentes-y-diferencias-por-confirmar)

## Alcance y evidencia

En la capa Vista están presentes `InicioView`, `InventarioView` y `ProveedoresView`. Las dos últimas muestran consultas; no se incluyen altas, actualizaciones ni bajas, aunque esas operaciones aparezcan en especificaciones o mockups. Los nombres de las pantallas se confirman en [Plantillas.xaml](../../src/FarmasilApp.Vista/Comun/Plantillas.xaml) y su implementación en las vistas [InventarioView.xaml](../../src/FarmasilApp.Vista/INV/InventarioView.xaml), [ProveedoresView.xaml](../../src/FarmasilApp.Vista/PRV/ProveedoresView.xaml) e [InicioView.xaml](../../src/FarmasilApp.Vista/Principal/InicioView.xaml).

**Límites de acceso comprobados:** la aplicación inicia en `InicioView`, cuyo texto es provisional. No se encontró una pantalla de inicio de sesión ni un menú visible para abrir consultas. `ProveedoresViewModel` está registrado en [RegistroDependencias.cs](../../src/FarmasilApp.VistaModelo/Comun/RegistroDependencias.cs), pero no hay una ruta visible desde Inicio. `InventarioViewModel` tampoco figura en ese registro. Si no se puede llegar a una pantalla, registra el bloqueo como hallazgo y no des por disponible un flujo navegable.

No se encontró una vista WPF para los otros módulos del catálogo (`VEN`, `DOC`, `ALV`, `PAG`, `REP`, `DEV`, `RES`, `USR`, `LOT`, `REA` y `COT`). Sus mockups, especificaciones, servicios o simulaciones no bastan para afirmar que sus pantallas estén implementadas; por ello esta guía no describe sus flujos.

## Entorno de pruebas

| Aspecto | Información documentada |
| --- | --- |
| Stack | C# sobre .NET 10 LTS, WPF con MVVM y SQL Server 2025 Express. El acceso a datos usa ADO.NET con `Microsoft.Data.SqlClient`, sin ORM. |
| Equipo | Windows 11; la aplicación y la instancia `SQLEXPRESS` se ejecutan en el mismo equipo. |
| Base | `DB_FARMASIL`. El README documenta `database/recrear.ps1` para crearla y cargar datos iniciales. |
| Configuración de prueba | Para apuntar a otra base, el README describe `src/FarmasilApp.Vista/appsettings.local.json` y `DB_FARMASIL_PRUEBAS`; ese archivo local no se versiona. |
| Herramientas | SDK de .NET 10 (10.0.100 o posterior de la serie 10.0) y Git. Visual Studio 2026 Community con «Desarrollo de escritorio de .NET» es recomendado; VS Code con C# Dev Kit o Rider son alternativas. |
| Comprobaciones documentadas | `dotnet build FarmasilApp.slnx --configuration Release` y `dotnet test FarmasilApp.slnx --configuration Release --no-build`. |

Referencias: [README.md](../../README.md), [CONTRIBUTING.md](../../CONTRIBUTING.md) y [RNF-0008](../../catalogo/08-requisitos-no-funcionales.md). Usa una base de pruebas, no una base de operación. Las pruebas de servicio y VistaModelo de Inventario y Proveedores usan repositorios falsos; no confirman que las pantallas consulten una instancia preparada.

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

<a id="indice-propuesto-del-manual-de-usuario"></a>
## Índice propuesto del manual de usuario

El repositorio no contiene todavía un manual de usuario. Para la versión basada en pantallas comprobables, se propone este índice limitado a tareas implementadas:

1. **Inventario (INV)**
   - Consultar productos.
   - Buscar productos por identificador o nombre.
   - Filtrar por proveedor o acción terapéutica.
   - Comparar resultados en tarjetas y tabla; seleccionar un producto para ver su ficha.
2. **Proveedores (PRV)**
   - Consultar proveedores.
   - Buscar por RUC o razón social.
   - Filtrar por estado.
   - Comparar resultados en tarjetas y tabla; seleccionar un proveedor para ver su ficha.

El acceso a estas pantallas desde la ventana inicial queda **Por confirmar**. No se incluyen tareas de administración de registros ni las pantallas de los demás módulos porque no están presentes en la capa Vista actual.

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

- El [plan de implementación, sección 6](../../docs/plan/plan-implementacion.md) dice que Usuarios queda completo en S1; sin embargo, el árbol actual de `src/FarmasilApp.Vista` no contiene una vista `USR`, y `tests` tampoco contiene una carpeta de pruebas `USR`. La guía toma las vistas presentes en el código como evidencia de pantalla, no el calendario.
- [README.md](../../README.md) informa 257 pruebas; la versión actual del [plan de pruebas](<../../pruebas de software/09-pruebas-software.md>) informa 275. Se conserva la diferencia documental y esta guía no fija un total.
- [CONTRIBUTING.md](../../CONTRIBUTING.md) describe que el esqueleto todavía no se conecta a SQL Server; en el código actual existen repositorios SQL para Inventario y Proveedores. La ejecución contra una base configurada sigue dependiendo del entorno de la sesión.
- Los mockups de [Inventario](../../diseno/Mockups/fuente/ART-MKP-INV-0001.html) y [Proveedores](../../diseno/Mockups/fuente/ART-MKP-PRV-0001.html) muestran más controles y operaciones que las vistas WPF actuales. Para esta guía prevalecen las pantallas implementadas y las especificaciones vigentes, no los flujos que solo aparecen en HTML.
- Los roles y permisos se documentan en la [matriz de permisos](../../diseno/07-matriz-permisos.md). Como la aplicación actual no permite iniciar sesión desde su interfaz, la autorización en ejecución queda **Por confirmar** en una exploración de usuario.
