# Guía de pruebas exploratorias

| Artefacto | Guía de pruebas exploratorias e índice del manual de usuario |
| --- | --- |
| Versión | 02.00 |
| Fecha | 23/11/2026 |
| Responsable | R4 — Pruebas exploratorias y documentación de usuario |

## Historial de versiones

| Versión | Fecha | Cambio |
| --- | --- | --- |
| 01.00 | 16/11/2026 | Primera versión (issue #78): lista común de revisión por pantalla, misiones de exploración de Inventario, Proveedores y Usuarios, plantillas de sesión y de defectos, e índice del manual de usuario por módulo. |
| 02.00 | 23/11/2026 | Issue #93: se actualiza el alcance según el código vigente; se agregan los recorridos de PAG, las columnas de resultado observado y evidencia y borradores del manual de USR y PAG. |

## Propósito

Esta guía sirve para explorar desde la interfaz los módulos USR, PRV, INV y PAG y para preparar el índice del manual de usuario. Los flujos esperados proceden de las especificaciones vigentes; las vistas WPF y sus VistaModelos permiten distinguir lo implementado de lo pendiente. Los HTML de `diseno/Mockups/fuente/` se usan solo como referencia visual y de rótulos, nunca como evidencia de ejecución. Los resultados observados y las evidencias se completan únicamente durante una sesión real.

## Cómo usar la guía

1. Registra una sesión con la plantilla de este documento.
2. Confirma los datos iniciales y el entorno antes de abrir una pantalla.
3. Recorre la [lista común de revisión](#lista-comun-de-revision) y después la misión del módulo; registra tanto los resultados correctos como las diferencias.
4. Si una pantalla u operación no está disponible, registra el bloqueo o déjala pendiente; no supongas navegación ni funciones a partir del mockup.
5. Informa los defectos con el formato de la sección [Reporte de defectos](#reporte-de-defectos).

## Índice

- [Alcance y evidencia](#alcance-y-evidencia)
- [Entorno de pruebas](#entorno-de-pruebas)
- [Lista común de revisión](#lista-comun-de-revision)
- [Inicio de sesión y ventana principal](#inicio-de-sesion-y-ventana-principal)
- [Inventario (INV)](#inventario-inv)
  - [Pantalla de Inventario](#pantalla-de-inventario)
- [Proveedores (PRV)](#proveedores-prv)
  - [Pantalla de Proveedores](#pantalla-de-proveedores)
- [Usuarios (USR)](#usuarios-usr)
  - [Pantalla de Usuarios](#pantalla-de-usuarios)
- [Métodos de pago (PAG)](#metodos-de-pago-pag)
  - [Pantalla de Métodos de pago](#pantalla-de-metodos-de-pago)
- [Índice del manual de usuario](#indice-del-manual-de-usuario)
- [Borrador del manual: USR y PAG](#borrador-del-manual-usr-y-pag)
- [Plantilla de sesión](#plantilla-de-sesion)
- [Reporte de defectos](#reporte-de-defectos)
- [Fuentes y diferencias por confirmar](#fuentes-y-diferencias-por-confirmar)

## Alcance y evidencia

Las vistas WPF identificadas para este issue son [InicioSesionView.xaml](../../src/FarmasilApp.Vista/USR/InicioSesionView.xaml), `InventarioView`, `ProveedoresView`, `UsuariosView` y `MetodosPagoView`; las cuatro pantallas de módulo se enlazan desde [Plantillas.xaml](../../src/FarmasilApp.Vista/Comun/Plantillas.xaml). El inicio de sesión conduce al menú por rol y a la pantalla de inicio. El menú registra INV y PAG para Administrador y Técnico, y PRV y USR solo para Administrador, de acuerdo con la [matriz de permisos](../../diseno/07-matriz-permisos.md).

**Alcance de implementación observado en los archivos (no equivale a una prueba ejecutada):** las vistas y VistaModelos de INV, PRV y USR contienen consulta, alta, actualización y baja; PAG contiene consulta y selección, pero su vista y VistaModelo no incluyen comandos de alta, actualización o baja. Para esas tres operaciones de PAG se documenta el flujo especificado y se marca pendiente de implementación/verificación. La disponibilidad, navegación real, conexión y respuestas visuales quedan por confirmar al ejecutar la aplicación.

Esta preparación fue documental: no se ejecutó la aplicación, pruebas, compilación ni scripts, y no se creó ni modificó ninguna base de datos. Por tanto, las columnas de resultado y evidencia permanecen en blanco y no se registran defectos como si se hubieran reproducido.

## Entorno de pruebas

| Aspecto | Información documentada |
| --- | --- |
| Stack | C# sobre .NET 10 LTS, WPF con MVVM y SQL Server 2025 Express. El acceso a datos usa ADO.NET con `Microsoft.Data.SqlClient`, sin ORM. |
| Equipo | Windows 11; la aplicación y la instancia `SQLEXPRESS` se ejecutan en el mismo equipo. |
| Base | Las sesiones exploratorias usan `DB_FARMASIL_PRUEBAS`, creada con `database/recrear.ps1 -Base DB_FARMASIL_PRUEBAS`, que también carga los datos de prueba. Nunca se explora sobre una base con datos reales. |
| Configuración de prueba | Para que la aplicación apunte a `DB_FARMASIL_PRUEBAS`, el README describe `src/FarmasilApp.Vista/appsettings.local.json`; ese archivo local no se versiona. |
| Cuentas de prueba | `admin_prueba` ('Administrador') y `tecnico_prueba` ('Tecnico'), cargadas por `database/002_datos_prueba.sql`, donde está documentada su contraseña. Confirmar su disponibilidad y usar únicamente una base de pruebas al preparar una sesión exploratoria. |
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
| Permisos | Contrasta el menú con la [matriz de permisos](../../diseno/07-matriz-permisos.md): los módulos no permitidos deben aparecer deshabilitados y explicar el motivo (RNF-0006, DEC-075). No supongas que los controles de operaciones inexistentes deben estar presentes. | RNF-0006, condición 4; DEC-075 |
| Datos | Una consulta no modifica datos. Un resultado vacío muestra un mensaje, no una pantalla en blanco. Si falla la conexión, la pantalla lo informa sin cerrarse. | Postcondiciones de cada especificación |
| Acceso | La pantalla se abre desde el menú por rol. Si no se puede llegar a ella, se registra como bloqueo. | Plan, sección 3 (R4) |

## Inicio de sesión y ventana principal

La ventana principal inicia en `InicioSesionView`. Tras una autenticación aceptada, muestra el menú correspondiente al rol y navega a `InicioView`; el menú incluye un botón para cerrar sesión. La pantalla de Inicio y su generación de PDF quedan fuera de las misiones de este issue, salvo por verificar que se llega a ella después del acceso. La navegación está enlazada en [MainViewModel.cs](../../src/FarmasilApp.VistaModelo/Principal/MainViewModel.cs), [MainWindow.xaml](../../src/FarmasilApp.Vista/Principal/MainWindow.xaml) y [RegistroDependencias.cs](../../src/FarmasilApp.VistaModelo/Comun/RegistroDependencias.cs).

| Flujo | Revisión y resultado esperado según la fuente | Resultado observado | Evidencia |
| --- | --- | --- | --- |
| Inicio de sesión (RNF-0006, DEC-080) | Con credenciales válidas de una cuenta activa, debe abrirse la sesión, mostrarse el nombre y rol, aparecer el menú según permisos y quedar vacía la contraseña. Una cuenta inactiva no inicia sesión. | | |
| Rechazo de autenticación | Usuario inexistente y contraseña errónea muestran el mismo mensaje; la contraseña se borra y vuelve a enmascararse después del intento. Un error técnico muestra el mensaje general, sin detalles técnicos. | | |
| Menú y cierre de sesión | INV y PAG están disponibles para Administrador y Técnico; PRV y USR solo para Administrador. Para el Técnico, las opciones no permitidas están deshabilitadas y explican el motivo. Cerrar sesión vacía el menú y vuelve al inicio de sesión. | | |

**Controles documentados**

| Nombre exacto | Uso |
| --- | --- |
| `USR-FRM-INICIO-SESION` | Formulario de inicio de sesión. |
| `USR-TXT-USUARIO-SESION` | Usuario. |
| `USR-PWD-CONTRASENA-SESION` | Contraseña enmascarada. |
| `USR-BTN-VER-CONTRASENA-SESION` | Alterna la visibilidad de la contraseña. |
| `USR-LBL-ERROR-SESION` | Mensajes del inicio de sesión. |
| `USR-BTN-INICIAR-SESION` | Envía las credenciales. |
| `USR-BTN-CERRAR-SESION` | Cierra la sesión. |
| Sin identificador AutomationId documentado | Botones de módulos en el menú; la guía de nomenclatura los deja pendientes (PEN-037). No asignarles un ID por inferencia. |

**Mensajes del inicio de sesión**

| Situación | Mensaje documentado |
| --- | --- |
| Usuario inexistente o contraseña incorrecta | `Usuario o contraseña incorrectos.` |
| Cuenta inactiva | `La cuenta está inactiva. Consulte con un administrador.` |
| Falla técnica | `No se pudo iniciar sesión. Inténtelo nuevamente.` |
| Módulo no disponible para el Técnico | `Solo disponible para el Administrador.` |

## Inventario (INV)

La vista [InventarioView.xaml](../../src/FarmasilApp.Vista/INV/InventarioView.xaml) y su VistaModelo incluyen consulta, alta, actualización y baja. El menú admite Administrador y Técnico; las operaciones de escritura son exclusivas del Administrador. La fuente de los resultados esperados es ESP-0005 a ESP-0008; el HTML [ART-MKP-INV-0001](../../diseno/Mockups/fuente/ART-MKP-INV-0001.html) sirve solo de referencia, pues sus controles y rótulos no sustituyen la especificación vigente.

<a id="pantalla-de-inventario"></a>
### Pantalla de Inventario

| Flujo | Revisión y resultado esperado según la especificación | Resultado observado | Evidencia |
| --- | --- | --- | --- |
| Consulta (ESP-0006) | Buscar por ID exacto o nombre parcial; probar filtros opcionales de proveedor y acción terapéutica, por separado y combinados, y consulta sin criterios. Tarjetas y tabla muestran los mismos productos y los campos ID, nombre, acción terapéutica, precio, estado y stock físico. Sin remesas, el stock físico es 0. Seleccionar una tarjeta o fila carga su ficha; consultar no modifica productos. | | |
| Alta (ESP-0005) | Crear producto con nombre, acción terapéutica, precio decimal no negativo, proveedor activo y stock mínimo entero no negativo. Debe generarse y mostrarse el ID, quedar el estado inicial «Disponible» y actualizarse la tabla. | | |
| Actualización (ESP-0007) | Seleccionar un producto, iniciar la edición y confirmar con «Guardar cambios». El ID no cambia; nombre, acción terapéutica, precio, proveedor y stock mínimo se actualizan, y se conservan las referencias existentes. | | |
| Baja (ESP-0008) | Seleccionar un producto y abrir la confirmación. Cancelar o Escape no cambia datos. Al confirmar, un producto sin referencias puede eliminarse físicamente; si tiene referencias, queda «Descontinuado». El historial se conserva y el producto deja de ofrecerse para ventas. Enter no confirma. | | |
| Permisos | Administrador puede consultar y ejecutar las cuatro operaciones. Técnico puede consultar; los botones de alta, actualización y baja están deshabilitados y muestran el motivo documentado. | | |

**Controles documentados**

| Nombre exacto | Rótulo o uso |
| --- | --- |
| `INV-SRH-PRODUCTO`, `INV-CMB-FILTRO-PROVEEDOR`, `INV-CMB-FILTRO-ACCION-TERAPEUTICA`, `INV-BTN-LEER-PRODUCTO` | Búsqueda por ID/nombre, filtros de consulta y botón «Buscar». |
| `INV-PNL-CATALOGO`, `INV-CRD-PRODUCTO`, `INV-BTN-SELECCIONAR-PRODUCTO`, `INV-TBL-PRODUCTOS` | Tarjetas y tabla de resultados; seleccionar la tarjeta o una fila carga la ficha. |
| `INV-LBL-ID-PRODUCTO`, `INV-TXT-NOMBRE-PRODUCTO`, `INV-TXT-ACCION-TERAPEUTICA`, `INV-NUM-PRECIO-PRODUCTO`, `INV-CMB-PROVEEDOR`, `INV-NUM-STOCK-MINIMO` | ID de solo lectura y campos de la ficha. |
| `INV-BTN-CREAR-PRODUCTO`, `INV-BTN-ACTUALIZAR-PRODUCTO`, `INV-BTN-CONFIRMAR-ACTUALIZACION`, `INV-BTN-ELIMINAR-PRODUCTO` | Alta, inicio de actualización, guardado y baja. |
| `INV-MDL-CONFIRMAR-ELIMINACION`, `INV-TXT-MSJ`, `INV-BTN-CONFIRMAR-SI`, `INV-BTN-CONFIRMAR-NO` | Controles de confirmación de baja según la nomenclatura y ESP-0008. |
| Sin identificador AutomationId en XAML | Texto de estado/mensaje de la vista. |

**Mensajes documentados en código y especificación**

| Situación | Mensaje |
| --- | --- |
| Hay resultados / sin resultados | `Productos encontrados: {cantidad}.` / `No se encontraron productos.` |
| Error de consulta / creación / actualización / baja | `No se pudo consultar el inventario. Inténtelo nuevamente.` / `No se pudo registrar el producto.` / `No se pudo actualizar el producto.` / `No se pudo dar de baja el producto.` |
| Alta / actualización / baja correctas | `Producto registrado correctamente` / `Producto actualizado correctamente` / `Producto dado de baja correctamente` |
| Validación de alta o actualización | `El nombre del producto es obligatorio.`, `El nombre del producto supera la longitud permitida.`, `La acción terapéutica es obligatoria.`, `La acción terapéutica supera la longitud permitida.`, `El precio de venta debe ser mayor o igual a cero.`, `El stock mínimo debe ser mayor o igual a cero.`, `Seleccione un proveedor.`, `El proveedor seleccionado no está activo.` |
| Falta selección para actualizar o dar de baja | `Seleccione un producto para actualizar.` / `Seleccione un producto para dar de baja.` |
| Confirmación de baja (ESP-0008) | `¿Está seguro de dar de baja este producto?` |
| Operación reservada al Administrador | `Solo disponible para el Administrador.` |

**Riesgos documentados**

| Riesgo | Observación para la sesión |
| --- | --- |
| `ILA-0006-R01` | El riesgo afirma que la búsqueda por nombre es de igualdad exacta; la versión vigente de `ESP-0006`, el plan de pruebas y el repositorio describen coincidencia parcial. La diferencia sigue registrada como pendiente en riesgos: anotar el resultado observado, sin resolver la discrepancia en esta guía. |
| `ILA-0006-R02` | El riesgo propone un indicador visual de alertas de vencimiento; `ESP-0006` no lo define y la vista actual no lo muestra. Registra lo observado sin tratar ese indicador como resultado obligatorio. |
| `ILA-0006-R03` | Riesgo pendiente por falta de un límite de registros/paginación documentado. Registrar problemas observables con catálogos grandes, sin asumir un límite esperado. |

Fuentes: [ESP-0006](../../catalogo/03-especificaciones.md), [guía de nomenclatura](../../diseno/06-guia-nomenclatura-mockups.md), [permisos](../../diseno/07-matriz-permisos.md), [pruebas de Inventario](<../../pruebas de software/09-pruebas-software.md>), [riesgos de Inventario](../../riesgos/10-gestion-riesgos.md), [InventarioViewModel.cs](../../src/FarmasilApp.VistaModelo/INV/InventarioViewModel.cs), [InventarioServicio.cs](../../src/FarmasilApp.Modelo/INV/InventarioServicio.cs), [InventarioRepositorio.cs](../../src/FarmasilApp.Persistencia/INV/InventarioRepositorio.cs), [pruebas del servicio](../../tests/FarmasilApp.Pruebas/INV/InventarioServicioPruebas.cs) y [pruebas del VistaModelo](../../tests/FarmasilApp.Pruebas/VistaModelo/InventarioViewModelPruebas.cs).

## Proveedores (PRV)

La vista [ProveedoresView.xaml](../../src/FarmasilApp.Vista/PRV/ProveedoresView.xaml) y su VistaModelo incluyen los cuatro flujos. El acceso al módulo es exclusivo del Administrador. El HTML [ART-MKP-PRV-0001](../../diseno/Mockups/fuente/ART-MKP-PRV-0001.html) se consulta como referencia; prevalecen las especificaciones, que incluyen el teléfono en la tabla y los controles de confirmación.

<a id="pantalla-de-proveedores"></a>
### Pantalla de Proveedores

| Flujo | Revisión y resultado esperado según la especificación | Resultado observado | Evidencia |
| --- | --- | --- | --- |
| Consulta (ESP-0038) | Con Administrador, buscar por RUC exacto o razón social parcial; probar filtro de estado «Todos», «Activo» e «Inactivo», su combinación con texto y consulta sin criterios. Tarjetas y tabla representan los mismos resultados; la tabla incluye RUC, razón social, teléfono y estado. Seleccionar tarjeta o fila carga la ficha. La consulta no modifica proveedores. | | |
| Alta (ESP-0037) | Crear proveedor con RUC de 11 dígitos numéricos no repetido, razón social no vacía, teléfono dentro del máximo documentado y estado válido. La tabla se actualiza; si queda Activo, debe poder elegirse en los módulos que requieren proveedor activo. | | |
| Actualización (ESP-0039) | Seleccionar y editar un proveedor; guardar con «Guardar cambios». RUC sigue único, la razón social no queda vacía, y los cambios se reflejan en la tabla. Al pasar a Inactivo, deja de ofrecerse en los selectores de productos y devoluciones; las referencias se conservan. | | |
| Baja (ESP-0040) | Si hay una orden de devolución pendiente de despacho, la operación se cancela y muestra el mensaje de regla. En otro caso, se presenta el modal con el texto especificado. Cancelar o Escape no cambia datos. Confirmar aplica la baja condicional, conserva referencias y actualiza la lista. Enter no confirma. | | |
| Permisos | En sesión Técnico, la opción Proveedores del menú está deshabilitada y explica que solo está disponible para Administrador. Las operaciones del módulo se exploran con una cuenta Administrador. | | |

**Controles documentados**

| Nombre exacto | Rótulo o uso |
| --- | --- |
| `PRV-SRH-PROVEEDOR`, `PRV-CMB-FILTRO-ESTADO`, `PRV-BTN-LEER-PROVEEDOR` | Búsqueda por RUC/razón social, filtro de estado y botón «Buscar». |
| `PRV-PNL-PROVEEDORES`, `PRV-CRD-PROVEEDOR`, `PRV-BTN-SELECCIONAR-PROVEEDOR`, `PRV-TBL-PROVEEDORES` | Tarjetas y tabla de resultados; la tabla incluye teléfono. |
| `PRV-TXT-RUC`, `PRV-TXT-RAZON-SOCIAL`, `PRV-TXT-TELEFONO`, `PRV-CMB-ESTADO-PROVEEDOR` | Campos de la ficha. |
| `PRV-BTN-CREAR-PROVEEDOR`, `PRV-BTN-ACTUALIZAR-PROVEEDOR`, `PRV-BTN-CONFIRMAR-ACTUALIZACION`, `PRV-BTN-ELIMINAR-PROVEEDOR` | Alta, inicio de actualización, guardado y baja. |
| `PRV-MDL-CONFIRMAR-ELIMINACION`, `PRV-TXT-MSJ`, `PRV-BTN-CONFIRMAR-SI`, `PRV-BTN-CONFIRMAR-NO` | Controles de confirmación de baja según la nomenclatura y ESP-0040. |
| Sin identificador AutomationId en XAML | Texto de estado/mensaje de la vista. |

**Mensajes documentados en código y especificación**

| Situación | Mensaje |
| --- | --- |
| Hay resultados / sin resultados | `Proveedores encontrados: {cantidad}.` / `No se encontraron proveedores.` |
| Error de consulta / escritura | `No se pudo consultar los proveedores. Inténtelo nuevamente.` / `No se pudo completar la operación. Inténtelo nuevamente.` |
| Alta / actualización / baja correctas | `Proveedor registrado correctamente` / `Proveedor actualizado correctamente` / `Proveedor dado de baja correctamente` |
| Validación de alta o actualización | `El RUC debe tener exactamente 11 dígitos numéricos.`, `Ya existe un proveedor con ese RUC.`, `Ingrese la razón social del proveedor.`, `La razón social no puede superar los 150 caracteres.`, `El teléfono no puede superar los 15 caracteres.`, `El estado debe ser Activo o Inactivo.` |
| Sin selección para actualizar o dar de baja | `Seleccione un proveedor para actualizar.` / `Seleccione un proveedor para dar de baja.` |
| Órdenes pendientes de despacho | `El proveedor tiene órdenes de devolución pendientes de despacho` |
| Confirmación de baja (ESP-0040) | `¿Está seguro de dar de baja este proveedor? Los productos y devoluciones ya registrados no se verán afectados.` |
| Opción del menú para Técnico / operación invocada sin rol | `Solo disponible para el Administrador.` / `Su rol no permite gestionar proveedores.` |

**Riesgo documentado**

`ILA-0038-R01` permanece pendiente y afirma que la consulta exige criterio y usa igualdad exacta. La versión vigente de `ESP-0038`, el plan de pruebas y el repositorio describen búsqueda parcial por razón social y consulta sin criterios. Registra lo observado sin resolver aquí la diferencia entre fuentes.

Fuentes: [ESP-0037 a ESP-0040](../../catalogo/03-especificaciones.md), [guía de nomenclatura](../../diseno/06-guia-nomenclatura-mockups.md), [permisos](../../diseno/07-matriz-permisos.md), [pruebas de Proveedores](<../../pruebas de software/09-pruebas-software.md>), [riesgos de Proveedores](../../riesgos/10-gestion-riesgos.md), [ProveedoresView.xaml](../../src/FarmasilApp.Vista/PRV/ProveedoresView.xaml), [ProveedoresViewModel.cs](../../src/FarmasilApp.VistaModelo/PRV/ProveedoresViewModel.cs), [ProveedoresServicio.cs](../../src/FarmasilApp.Modelo/PRV/ProveedoresServicio.cs), [ProveedoresRepositorio.cs](../../src/FarmasilApp.Persistencia/PRV/ProveedoresRepositorio.cs) y [MensajesProveedores.cs](../../src/FarmasilApp.Modelo/PRV/MensajesProveedores.cs).

## Usuarios (USR)

La vista [UsuariosView.xaml](../../src/FarmasilApp.Vista/USR/UsuariosView.xaml) y su VistaModelo incluyen consulta, alta, actualización y baja. El menú admite el módulo solo para Administrador. La pantalla de inicio de sesión se describe en [Inicio de sesión y ventana principal](#inicio-de-sesion-y-ventana-principal).

<a id="pantalla-de-usuarios"></a>
### Pantalla de Usuarios

| Flujo | Revisión y resultado esperado según la especificación | Resultado observado | Evidencia |
| --- | --- | --- | --- |
| Consulta (ESP-0034) | Buscar por nombre de usuario y filtrar por rol, incluso con «Todos». La lista y la tabla muestran el mismo nombre, rol y estado; no se muestra contraseña ni `id_usuario`. Seleccionar tarjeta o fila carga la ficha. Registrar si la búsqueda es exacta o parcial: la especificación describe igualdad y el riesgo `ILA-0034-R01` mantiene el criterio por confirmar. | | |
| Alta (ESP-0033) | Crear cuenta con nombre único de hasta 50 caracteres, contraseña no vacía, rol válido y estado válido. Un nombre solo con espacios cuenta como vacío; la comparación de duplicados no distingue mayúsculas. La contraseña está enmascarada y no aparece en la tabla. | | |
| Actualización (ESP-0035) | Seleccionar cuenta y pulsar «Actualizar»; la contraseña queda vacía. Guardar sin contraseña conserva la contraseña vigente. No debe ser posible dejar el sistema sin Administrador activo ni cambiar el rol/estado de la cuenta de la sesión; sí se permite cambiar su nombre y contraseña. | | |
| Baja (ESP-0036) | No se puede dar de baja la cuenta de la sesión. Para otra cuenta, mostrar el modal con el texto especificado; cancelar o Escape no cambia datos. Confirmar aplica baja condicional: las cuentas con operaciones quedan Inactivas y se conserva la trazabilidad. Enter no confirma. | | |
| Permisos | En sesión Técnico, la opción Usuarios del menú está deshabilitada y explica que solo está disponible para Administrador. Las operaciones del módulo se exploran con una cuenta Administrador. | | |

**Controles documentados**

| Nombre exacto | Rótulo o uso |
| --- | --- |
| `USR-SRH-USUARIO`, `USR-CMB-FILTRO-ROL`, `USR-BTN-LEER-USUARIO` | Búsqueda, filtro de rol («Todos») y botón «Buscar». |
| `USR-PNL-CUENTAS`, `USR-CRD-USUARIO`, `USR-TBL-USUARIOS` | Lista en tarjetas y tabla con usuario, rol y estado; la tarjeta no tiene botón y se selecciona con un clic. |
| `USR-TXT-NOMBRE-USUARIO`, `USR-PWD-CONTRASENA`, `USR-CMB-ROL`, `USR-CMB-ESTADO-USUARIO` | Campos de ficha; la contraseña se enmascara y se excluye de resultados. |
| `USR-BTN-CREAR-USUARIO`, `USR-BTN-ACTUALIZAR-USUARIO`, `USR-BTN-CONFIRMAR-ACTUALIZACION`, `USR-BTN-ELIMINAR-USUARIO` | Alta, inicio de actualización, guardado y baja. |
| `USR-MDL-CONFIRMAR-ELIMINACION`, `USR-TXT-MSJ`, `USR-BTN-CONFIRMAR-SI`, `USR-BTN-CONFIRMAR-NO` | Modal común de confirmación de la baja. |

**Mensajes documentados**

| Situación | Mensaje |
| --- | --- |
| Hay resultados / sin resultados | `Cuentas encontradas: {cantidad}.` / `No se encontraron cuentas.` |
| Alta / actualización / baja correctas | `Usuario registrado correctamente` / `Usuario actualizado correctamente` / `Cuenta dada de baja correctamente` |
| No queda Administrador activo | `El sistema debe conservar al menos una cuenta administradora activa` |
| Cambio de rol/estado o baja de cuenta propia | `No es posible cambiar el rol ni el estado de la cuenta con la que se encuentra abierta la sesión` / `No es posible dar de baja la cuenta con la que se encuentra abierta la sesión` |
| Confirmación de baja (ESP-0036) | `¿Está seguro de dar de baja esta cuenta? El histórico de operaciones que registró se conservará.` |
| Validaciones (textos no fijados por ESP) | `Ingrese el nombre de usuario.`, `El nombre de usuario no puede superar los 50 caracteres.`, `Ya existe una cuenta con ese nombre de usuario.`, `Ingrese la contraseña.`, `Seleccione un rol válido.` y `Seleccione un estado válido.` |
| Opción del menú para Técnico / operación invocada sin rol | `Solo disponible para el Administrador.` / `Su rol no permite gestionar usuarios.` |
| Error de consulta / operación | `No se pudo consultar las cuentas. Inténtelo nuevamente.` / `No se pudo completar la operación. Inténtelo nuevamente.` |

**Riesgos documentados**

| Riesgo | Observación para la sesión |
| --- | --- |
| `ILA-0033-R01` | No hay una política mínima de contraseña: la especificación solo exige que no esté vacía. Anota si una contraseña muy corta se acepta, sin tratarlo como defecto. |
| `ILA-0034-R01` | Está pendiente definir si la búsqueda por nombre de usuario es exacta o parcial. Registra cuál se observa. |
| `ILA-0035-R02` | No está definido si un cambio de rol o estado afecta a una sesión ya abierta de esa cuenta. Registrar lo que se observe, sin convertirlo en una regla no especificada. |

Fuentes: [ESP-0033 a ESP-0036](../../catalogo/03-especificaciones.md), [guía de nomenclatura](../../diseno/06-guia-nomenclatura-mockups.md), [permisos](../../diseno/07-matriz-permisos.md), [pruebas de Usuarios](<../../pruebas de software/09-pruebas-software.md>), [riesgos de Usuarios](../../riesgos/10-gestion-riesgos.md), [UsuariosView.xaml](../../src/FarmasilApp.Vista/USR/UsuariosView.xaml), [UsuariosViewModel.cs](../../src/FarmasilApp.VistaModelo/USR/UsuariosViewModel.cs), [MensajesUsuarios.cs](../../src/FarmasilApp.Modelo/USR/MensajesUsuarios.cs), [UsuariosServicio.cs](../../src/FarmasilApp.Modelo/USR/UsuariosServicio.cs) y [UsuariosRepositorio.cs](../../src/FarmasilApp.Persistencia/USR/UsuariosRepositorio.cs).

## Métodos de pago (PAG)

La vista [MetodosPagoView.xaml](../../src/FarmasilApp.Vista/PAG/MetodosPagoView.xaml) y su VistaModelo implementan actualmente la consulta (ESP-0018); la ficha es de solo lectura y no hay comandos WPF para alta, actualización o baja. Para esas operaciones, las especificaciones y el HTML [ART-MKP-PAG-0001](../../diseno/Mockups/fuente/ART-MKP-PAG-0001.html) sirven de referencia para registrar qué falta, no para considerar implementado el flujo.

<a id="pantalla-de-metodos-de-pago"></a>
### Pantalla de Métodos de pago

| Flujo | Revisión y resultado esperado según la especificación | Resultado observado | Evidencia |
| --- | --- | --- | --- |
| Consulta (ESP-0018) | Administrador y Técnico pueden consultar. Buscar nombre parcial y filtrar por estado («Todos», «Activo», «Inactivo»), probando filtros separados, combinados y sin criterios. Las tarjetas y la tabla muestran los mismos métodos y estado; seleccionar tarjeta o fila carga la ficha de solo lectura. La consulta no modifica datos. | | |
| Alta (ESP-0017) — pendiente | La especificación requiere nombre no vacío y único y estado válido; al registrar se actualizan lista y tabla. La vista WPF actual no expone formulario editable ni comando de alta: no ejecutar ni registrar como probado. | | |
| Actualización (ESP-0019) — pendiente | Según la especificación, seleccionar el método, iniciar edición y guardar nombre único y estado válido; conservar las ventas históricas al cambiar a Inactivo. Los controles de edición/guardado están en la nomenclatura y el mockup, pero no en la vista WPF actual. | | |
| Baja (ESP-0020) — pendiente | Según la especificación, confirmar la baja condicional; con referencias el método queda Inactivo y las ventas históricas se conservan. La vista WPF actual no expone botón ni modal para esta operación. | | |

**Controles documentados**

| Nombre exacto | Estado y uso |
| --- | --- |
| `PAG-CMB-FILTRO-ESTADO`, `PAG-SRH-METODO`, `PAG-BTN-LEER-METODO` | Presentes en XAML: filtro, búsqueda y botón «Buscar». |
| `PAG-PNL-METODOS-PAGO`, `PAG-CRD-METODO-PAGO`, `PAG-TBL-METODOS-PAGO` | Presentes en XAML: lista de tarjetas, tarjeta seleccionable y tabla. |
| `PAG-TXT-METODO-PAGO`, `PAG-CMB-ESTADO-METODO` | Presentes en XAML; ficha de solo lectura. |
| `PAG-BTN-CREAR-METODO`, `PAG-BTN-ACTUALIZAR-METODO`, `PAG-BTN-CONFIRMAR-ACTUALIZACION`, `PAG-BTN-ELIMINAR-METODO` | Documentados en especificaciones/nomenclatura/mockup; no presentes en la vista WPF actual. Pendientes. |
| `PAG-MDL-CONFIRMAR-ELIMINACION`, `PAG-TXT-MSJ`, `PAG-BTN-CONFIRMAR-SI`, `PAG-BTN-CONFIRMAR-NO` | Documentados para ESP-0020; no presentes en la vista WPF actual. Pendientes. |

**Mensajes documentados**

| Situación | Mensaje |
| --- | --- |
| Hay resultados / sin resultados | `Métodos de pago encontrados: {cantidad}.` / `No se encontraron métodos de pago.` |
| Falla la consulta | `No se pudieron consultar los métodos de pago. Inténtelo nuevamente.` |
| Alta / actualización / baja correctas (texto de especificación; flujo WPF pendiente) | `Método de pago registrado correctamente` / `Método de pago actualizado correctamente` / `Método de pago dado de baja correctamente` |
| Confirmación de baja (ESP-0020; flujo WPF pendiente) | `¿Está seguro de dar de baja este método de pago? Las ventas ya registradas con él no se verán afectadas.` |

Fuentes: [ESP-0017 a ESP-0020](../../catalogo/03-especificaciones.md), [guía de nomenclatura](../../diseno/06-guia-nomenclatura-mockups.md), [permisos](../../diseno/07-matriz-permisos.md), [pruebas de Métodos de pago](<../../pruebas de software/09-pruebas-software.md>), [MetodosPagoView.xaml](../../src/FarmasilApp.Vista/PAG/MetodosPagoView.xaml), [MetodosPagoViewModel.cs](../../src/FarmasilApp.VistaModelo/PAG/MetodosPagoViewModel.cs) y [ART-MKP-PAG-0001.html](../../diseno/Mockups/fuente/ART-MKP-PAG-0001.html).

<a id="indice-del-manual-de-usuario"></a>
## Índice del manual de usuario

El repositorio no contiene todavía un manual de usuario completo. Este índice fija su estructura por módulo y operación, con los roles de la [matriz de permisos](../../diseno/07-matriz-permisos.md). El estado «vista implementada» describe únicamente los archivos del repositorio; no indica que la interfaz haya sido ejecutada o explorada.

1. **Introducción**: qué es FARMASIL, los dos roles (Administrador y Técnico) y cómo se lee este manual.
2. **Inicio y cierre de sesión** (RNF-0006): ingresar con usuario y contraseña, y qué pasa con una cuenta inactiva. **(vista implementada; exploración pendiente)**
3. **Ventana principal y menú**: cómo abrir cada módulo y por qué algunas opciones se ven deshabilitadas para el Técnico. **(implementación presente; exploración pendiente)**
4. **Ventas (VEN)** — implementación y pantallas pendientes de verificación.
   - Registrar una venta (ESP-0001) — Administrador y Técnico.
   - Consultar ventas (ESP-0002) — Administrador y Técnico.
   - Modificar una venta (ESP-0003) — Administrador.
   - Eliminar una venta (ESP-0004) — Administrador.
5. **Inventario (INV)**: vista con CRUD identificada; exploración pendiente.
   - Registrar un producto (ESP-0005) — Administrador.
   - Consultar el inventario (ESP-0006) — Administrador y Técnico.
   - Actualizar un producto (ESP-0007) — Administrador.
   - Dar de baja un producto (ESP-0008) — Administrador. **(vista implementada; exploración pendiente)**
6. **Documentación tributaria (DOC)** — implementación y pantallas pendientes de verificación.
   - Emitir un comprobante (ESP-0009) — Administrador y Técnico.
   - Consultar comprobantes (ESP-0010) — Administrador y Técnico.
   - Corregir un comprobante (ESP-0011) — Administrador.
   - Anular un comprobante (ESP-0012) — Administrador.
7. **Alertas de vencimiento (ALV)** — implementación y pantallas pendientes de verificación.
   - Consultar las alertas (ESP-0013) — Administrador y Técnico.
   - Registrar una alerta manualmente (ESP-0014) — Administrador y Técnico.
   - Modificar una alerta (ESP-0015) — Administrador.
   - Eliminar una alerta (ESP-0016) — Administrador.
8. **Métodos de pago (PAG)**
   - Registrar un método de pago (ESP-0017) — Administrador. **(pendiente en la vista actual)**
   - Consultar los métodos de pago (ESP-0018) — Administrador y Técnico. **(vista implementada; exploración pendiente)**
   - Actualizar un método de pago (ESP-0019) — Administrador. **(pendiente en la vista actual)**
   - Desactivar un método de pago (ESP-0020) — Administrador. **(pendiente en la vista actual)**
9. **Reportes de ventas (REP)**: solo Administrador; implementación y pantallas pendientes de verificación.
   - Generar un reporte (ESP-0021).
   - Consultar los reportes (ESP-0022).
   - Cambiar el intervalo de un reporte (ESP-0023).
   - Eliminar un reporte (ESP-0024).
10. **Devoluciones a proveedores (DEV)**: solo Administrador; implementación y pantallas pendientes de verificación.
    - Registrar una orden de devolución (ESP-0025).
    - Consultar las órdenes (ESP-0026).
    - Actualizar una orden (ESP-0027).
    - Eliminar una orden (ESP-0028).
11. **Restricciones de venta (RES)**: implementación y pantallas pendientes de verificación.
    - Registrar una restricción (ESP-0029) — Administrador.
    - Consultar las restricciones (ESP-0030) — Administrador y Técnico.
    - Modificar una restricción (ESP-0031) — Administrador.
    - Desactivar una restricción (ESP-0032) — Administrador.
12. **Usuarios (USR)**: solo Administrador. **(vista implementada; exploración pendiente)**
    - Crear una cuenta (ESP-0033).
    - Consultar las cuentas (ESP-0034).
    - Actualizar una cuenta (ESP-0035).
    - Dar de baja una cuenta (ESP-0036).
13. **Proveedores (PRV)**: solo Administrador. **(vista implementada; exploración pendiente)**
    - Registrar un proveedor (ESP-0037).
    - Consultar los proveedores (ESP-0038).
    - Actualizar un proveedor (ESP-0039).
    - Desactivar un proveedor (ESP-0040).
14. **Lotes (LOT)**: implementación y pantallas pendientes de verificación.
    - Registrar una remesa (ESP-0041) — Administrador.
    - Consultar las remesas (ESP-0042) — Administrador y Técnico.
    - Actualizar una remesa (ESP-0043) — Administrador.
    - Dar de baja una remesa (ESP-0044) — Administrador.
15. **Reabastecimiento (REA)**: solo Administrador; implementación y pantallas pendientes de verificación.
    - Agregar un ítem a la lista de reabastecimiento (ESP-0045).
    - Consultar la lista (ESP-0046).
    - Actualizar un ítem (ESP-0047).
    - Eliminar un ítem (ESP-0048).
16. **Comparación de precios de proveedores (COT)**: solo Administrador; implementación y pantallas pendientes de verificación.
    - Registrar una cotización (ESP-0049).
    - Comparar precios y exportar por proveedor (ESP-0050).
    - Actualizar una cotización (ESP-0051).
    - Eliminar una cotización (ESP-0052).
17. **Anexos**: mensajes del sistema por módulo y glosario (remesa, stock físico y vendible, comprobante, baja lógica).

<a id="borrador-del-manual-usr-y-pag"></a>
## Borrador del manual: USR y PAG

Este borrador se basa en especificaciones, permisos y controles del repositorio. Debe validarse con la aplicación antes de publicarse como manual final. Los pasos de PAG que no tienen controles en la vista WPF se presentan como flujos especificados pendientes, no como acciones disponibles.

### Usuarios (USR)

El módulo Usuarios es exclusivo del Administrador. Las contraseñas no aparecen en los resultados ni en la tabla.

1. **Iniciar sesión:** escribe el nombre de usuario y la contraseña en `USR-TXT-USUARIO-SESION` y `USR-PWD-CONTRASENA-SESION`, y activa `USR-BTN-INICIAR-SESION`. La contraseña aparece enmascarada; el botón `USR-BTN-VER-CONTRASENA-SESION` permite mostrarla temporalmente. Si la cuenta está inactiva, consulta con un Administrador. Si las credenciales no coinciden, verifica los datos antes de volver a intentarlo. La contraseña se borra al terminar cada intento.
2. **Abrir Usuarios:** desde el menú, elige Usuarios. Si la opción está deshabilitada, la sesión no tiene el rol requerido. `USR-BTN-CERRAR-SESION` termina la sesión y vuelve al formulario de acceso.
3. **Consultar cuentas:** escribe un nombre en `USR-SRH-USUARIO`, elige un rol en `USR-CMB-FILTRO-ROL` o «Todos» y pulsa `USR-BTN-LEER-USUARIO`. La lista y la tabla muestran usuario, rol y estado. Selecciona una tarjeta o fila para cargar la ficha; la contraseña no se muestra.
4. **Crear cuenta:** completa nombre de usuario, contraseña, rol y estado, y pulsa `USR-BTN-CREAR-USUARIO`. El nombre debe ser único y no superar 50 caracteres; la contraseña no puede estar vacía.
5. **Actualizar cuenta:** selecciona la cuenta, pulsa `USR-BTN-ACTUALIZAR-USUARIO`, modifica la ficha y pulsa `USR-BTN-CONFIRMAR-ACTUALIZACION` («Guardar cambios»). La contraseña aparece vacía: déjala así para conservar la existente o escribe una nueva. El sistema debe conservar al menos un Administrador activo; la cuenta de la sesión no puede cambiar su propio rol ni estado.
6. **Dar de baja:** selecciona una cuenta distinta de la cuenta de la sesión, pulsa `USR-BTN-ELIMINAR-USUARIO` y confirma en el modal. Si cancelas, no se aplica la baja. Las cuentas con operaciones se conservan como Inactivas.

### Métodos de pago (PAG)

Administrador y Técnico pueden abrir el módulo para consultar métodos. La vista actual es de solo lectura.

1. **Consultar:** escribe parte del nombre en `PAG-SRH-METODO`, elige un estado en `PAG-CMB-FILTRO-ESTADO` («Todos», «Activo» o «Inactivo») y pulsa `PAG-BTN-LEER-METODO`. Sin criterios se consultan todos. Las tarjetas y la tabla deben mostrar los mismos métodos y estado.
2. **Ver un método:** selecciona una tarjeta de `PAG-PNL-METODOS-PAGO` o una fila de `PAG-TBL-METODOS-PAGO`; la ficha muestra el nombre y estado en modo de solo lectura.
3. **Crear, actualizar o dar de baja:** las especificaciones asignan estas operaciones al Administrador, pero sus controles y comandos no están en la vista WPF actual. Los flujos están pendientes de implementación/verificación; no sigas estos pasos en la versión actual. Cuando estén disponibles, aplicar ESP-0017, ESP-0019 y ESP-0020: los métodos activos se ofrecen para ventas, y dar de baja un método no altera las ventas ya registradas.

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
Evidencia (referencia o ruta):
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
Evidencia (referencia, datos de prueba o registro relevante):
```

Describe pasos concretos y conserva los datos de prueba necesarios para repetir el resultado. No adjuntes credenciales ni datos reales de clientes.

## Fuentes y diferencias por confirmar

- [README.md](../../README.md) informa 257 pruebas; la versión actual del [plan de pruebas](<../../pruebas de software/09-pruebas-software.md>) informa 275. Se conserva la diferencia documental y esta guía no fija un total.
- La presencia de vistas, servicios y repositorios en el código no demuestra que la aplicación se conecte correctamente a la base preparada; esa conexión queda pendiente de la sesión exploratoria.
- Los HTML de [Inventario](../../diseno/Mockups/fuente/ART-MKP-INV-0001.html), [Proveedores](../../diseno/Mockups/fuente/ART-MKP-PRV-0001.html), [Usuarios](../../diseno/Mockups/fuente/ART-MKP-USR-0001.html) y [Métodos de pago](../../diseno/Mockups/fuente/ART-MKP-PAG-0001.html) son referencias de diseño. En particular, el mockup PAG muestra botones CRUD que no están en la vista WPF actual; no se consideran comportamiento disponible ni evidencia.
- La [matriz de permisos](../../diseno/07-matriz-permisos.md), el inicio de sesión y el menú por rol están representados en el código. Su funcionamiento visual y autorización efectiva permanecen **pendientes de verificación** porque no se ejecutó la aplicación.
- El issue #93 pide registrar como issues los errores reproducibles. No se registra ningún error antes de realizar la exploración; completar ese criterio y la definición de terminado de la [sección 8 del plan](../../docs/plan/plan-implementacion.md#8-definicion-de-terminado-y-forma-de-trabajo) después de ejecutarla y completar la CI requerida.
