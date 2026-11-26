# Guía de pruebas exploratorias

| Artefacto | Guía de pruebas exploratorias de USR, PRV, INV y PAG |
| --- | --- |
| Versión | 03.00 |
| Fecha | 25/11/2026 |
| Responsable | R4 — Pruebas exploratorias y documentación de usuario. Sesiones de cierre de S2: R1, por ausencia de R4. |

## Historial de versiones

| Versión | Fecha | Cambio |
| --- | --- | --- |
| 01.00 | 16/11/2026 | Primera versión (issue #78): lista común de revisión por pantalla, misiones de exploración de Inventario, Proveedores y Usuarios, plantillas de sesión y de defectos, e índice del manual de usuario por módulo. |
| 02.00 | 23/11/2026 | Issue #93: recorridos de PAG, columnas de resultado observado y evidencia, y borrador del manual de USR y PAG. |
| 02.01 | 25/11/2026 | Issue #93: capturas de los recorridos en `evidencias-pdf/`. |
| 03.00 | 25/11/2026 | Issue #93 (revisión del PR #188): se alinea con `main`. PAG tiene alta, actualización y baja (#186) y el menú tiene siete módulos. Los recorridos llevan los datos de prueba de cada rama. Se agregan los hallazgos de la revisión estática y la lista de capturas por repetir. El índice y el manual de USR y PAG pasan a [docs/manual/manual-usuario.md](../manual/manual-usuario.md). |

## Propósito

Esta guía sirve para explorar desde la interfaz los módulos USR, PRV, INV y PAG. Los resultados esperados salen de las especificaciones vigentes (ESP-0005 a ESP-0008, ESP-0017 a ESP-0020 y ESP-0033 a ESP-0040), de la [guía de nomenclatura](../../diseno/06-guia-nomenclatura-mockups.md) y de la [matriz de permisos](../../diseno/07-matriz-permisos.md). Los mockups HTML son solo una referencia visual.

## Cómo usar la guía

1. Prepara la base y la cuenta como dice [Entorno de pruebas](#entorno-de-pruebas).
2. Recorre la [lista común de revisión](#lista-comun-de-revision) en cada pantalla y después la tabla del módulo, fila por fila, con los datos indicados.
3. Anota en «Resultado observado» lo que pasó, también cuando coincide con lo esperado. En «Evidencia», el nombre de la captura.
4. Haz primero todo lo de `tecnico_prueba` (menú, Inventario y Métodos de pago). La baja de Usuarios deja esa cuenta 'Inactivo', y después ya no se puede entrar con ella; para repetir, recrea la base.
5. Lo que no coincide se registra como hallazgo de uso (`HAL-NNN`, DEC-081) y, si es un error, como issue con el formato de [Reporte de defectos](#reporte-de-defectos).

## Índice

- [Alcance](#alcance)
- [Entorno de pruebas](#entorno-de-pruebas)
- [Lista común de revisión](#lista-comun-de-revision)
- [Inicio de sesión y ventana principal](#inicio-de-sesion-y-ventana-principal)
- [Inventario (INV)](#inventario-inv)
- [Proveedores (PRV)](#proveedores-prv)
- [Usuarios (USR)](#usuarios-usr)
- [Métodos de pago (PAG)](#metodos-de-pago-pag)
- [Hallazgos de la revisión estática](#hallazgos-de-la-revision-estatica)
- [Capturas](#capturas)
- [Plantilla de sesión](#plantilla-de-sesion)
- [Reporte de defectos](#reporte-de-defectos)
- [Fuentes y diferencias por confirmar](#fuentes-y-diferencias-por-confirmar)

## Alcance

Las cuatro pantallas tienen consulta, alta, actualización y baja: [InventarioView.xaml](../../src/FarmasilApp.Vista/INV/InventarioView.xaml), [ProveedoresView.xaml](../../src/FarmasilApp.Vista/PRV/ProveedoresView.xaml), [UsuariosView.xaml](../../src/FarmasilApp.Vista/USR/UsuariosView.xaml) y [MetodosPagoView.xaml](../../src/FarmasilApp.Vista/PAG/MetodosPagoView.xaml). Se llega a ellas desde el menú de la ventana principal, después del [inicio de sesión](../../src/FarmasilApp.Vista/USR/InicioSesionView.xaml).

El menú tiene siete módulos ([RegistroDependencias.cs](../../src/FarmasilApp.VistaModelo/Comun/RegistroDependencias.cs)):

| Módulo del menú | Administrador | Técnico |
| --- | --- | --- |
| Ventas | Sí | Sí |
| Comprobantes tributarios | Sí | Sí |
| Inventario | Sí | Sí; solo consulta |
| Alertas de vencimiento | Sí | Sí |
| Métodos de pago | Sí | Sí; solo consulta |
| Usuarios | Sí | No: deshabilitado con el motivo |
| Proveedores | Sí | No: deshabilitado con el motivo |

Ventas, Comprobantes tributarios y Alertas de vencimiento quedan fuera de esta guía; solo se comprueba que aparezcan en el menú.

## Entorno de pruebas

| Aspecto | Qué usar |
| --- | --- |
| Base | `DB_FARMASIL_PRUEBAS`, creada y cargada con los datos de prueba como explica el [README](../../README.md#levantar-la-base-de-datos). Nunca una base con datos reales. Recréala antes de la sesión para empezar desde los datos conocidos. |
| Conexión | La aplicación apunta a `DB_FARMASIL_PRUEBAS` con el `appsettings.local.json` que describe el README. Ese archivo no se versiona. |
| Cuentas | `admin_prueba` ('Administrador') y `tecnico_prueba` ('Tecnico'), de `database/002_datos_prueba.sql`. Las contraseñas no se escriben en el registro de sesión ni aparecen en las capturas. |
| Ejecución | `dotnet run --project src/FarmasilApp.Vista`, o desde Visual Studio. |

### Datos de prueba que usa cada recorrido

| Dato | Para qué sirve |
| --- | --- |
| Droguería Andina SAC (RUC 20100070970) | Tiene una orden de devolución 'Pendiente': su baja se rechaza (ESP-0040). |
| Distribuidora Salud Sur SAC y Laboratorios del Pacífico SAC | Tienen productos y remesas: su baja queda en 'Inactivo'. |
| Proveedor Inactivo SAC | Estado 'Inactivo'; no debe ofrecerse en el combo de proveedor de Inventario. |
| Omeprazol 20 mg | Sin remesas: stock físico 0. |
| Paracetamol 500 mg | Con remesas y ventas: su baja queda en 'Descontinuado'. |
| Producto Descontinuado | Ya está en 'Descontinuado'. |
| Efectivo y Yape | Usados en ventas: su baja queda en 'Inactivo'. |
| Tarjeta | 'Inactivo' y sin ventas: su baja lo elimina. |
| `admin_prueba` y `tecnico_prueba` | Las dos tienen ventas registradas. |

<a id="lista-comun-de-revision"></a>
## Lista común de revisión

Se aplica a cualquier pantalla, antes de su recorrido. Para leer el identificador de un control se usa una herramienta de inspección de UI Automation de Windows (por ejemplo, Accessibility Insights o Inspect), que muestra el `AutomationId` del control bajo el cursor.

| Qué revisar | Qué se espera | Regla |
| --- | --- | --- |
| Identificadores | Cada control que nombra la especificación existe y su `AutomationId` es exactamente el de la [guía de nomenclatura](../../diseno/06-guia-nomenclatura-mockups.md). No hay controles con identificadores inventados. | DEC-060 |
| Búsqueda y filtros | El buscador se llama `<MOD>-SRH-…` y los filtros de lista cerrada `<MOD>-CMB-FILTRO-…`. Cada filtro permite volver a «Todos». Sin criterios, la consulta hace lo que dice su especificación. | DEC-061 |
| Tarjetas y tabla | Las tarjetas (`<MOD>-CRD-…` dentro de `<MOD>-PNL-…`) y la tabla (`<MOD>-TBL-…`) muestran el mismo resultado. Seleccionar una tarjeta o una fila carga la ficha. | DEC-062 |
| Actualización | La actualización tiene dos pasos: el botón que la inicia y `<MOD>-BTN-CONFIRMAR-ACTUALIZACION`, con el rótulo «Guardar cambios». | DEC-046 |
| Baja | La baja pide confirmación en el modal `<MOD>-MDL-CONFIRMAR-ELIMINACION`, con el texto `<MOD>-TXT-MSJ` exacto de la especificación y los botones `<MOD>-BTN-CONFIRMAR-SI` («Sí, Confirmar Acción») y `<MOD>-BTN-CONFIRMAR-NO` («Cancelar»). Escape cancela y Enter no confirma. | DEC-047 |
| Mensajes | Los mensajes de éxito y de las reglas de negocio son los de la especificación, palabra por palabra. Cuando la especificación no fija el texto, el mensaje es claro y no muestra detalles técnicos. | Especificación de la operación |
| Permisos | Lo reservado al Administrador se ve deshabilitado para el Técnico y explica el motivo al pasar el cursor: «Solo disponible para el Administrador.». | RNF-0006, DEC-075 |
| Datos | Una consulta no modifica datos. Un resultado vacío muestra un mensaje. Si falla la conexión, la pantalla lo informa sin cerrarse. | Postcondiciones de cada especificación |
| Presentación | Con la ventana a su tamaño normal se leen todas las columnas de la tabla y más de una fila. | Uso |

## Inicio de sesión y ventana principal

| Flujo | Qué hacer y qué se espera | Resultado observado | Evidencia |
| --- | --- | --- | --- |
| Inicio de sesión correcto | Entrar con `admin_prueba`. Se abre la ventana principal con el menú y el pie «admin_prueba · Administrador»; la contraseña queda vacía. Repetir con `tecnico_prueba`. | | COM-01, COM-02; Técnico: COM-03 |
| Rechazo | Usuario inexistente y contraseña errónea muestran el mismo mensaje, `Usuario o contraseña incorrectos.`; la contraseña se borra. | | |
| Menú del Administrador | Los siete módulos están habilitados. | | COM-02 |
| Menú del Técnico | Usuarios y Proveedores se ven deshabilitados y, al pasar el cursor, dicen `Solo disponible para el Administrador.`; los otros cinco están habilitados. | | COM-04 |
| Cierre de sesión | `USR-BTN-CERRAR-SESION` vacía el menú y vuelve al inicio de sesión. | | |

| Control | Uso |
| --- | --- |
| `USR-TXT-USUARIO-SESION`, `USR-PWD-CONTRASENA-SESION`, `USR-BTN-VER-CONTRASENA-SESION` | Usuario, contraseña enmascarada y botón para mostrarla. |
| `USR-BTN-INICIAR-SESION`, `USR-LBL-ERROR-SESION` | Envía las credenciales y muestra el error. |
| `USR-BTN-CERRAR-SESION` | Cierra la sesión. |
| Botones de módulos del menú | Sin identificador: la guía los deja pendientes (PEN-037). |

| Situación | Mensaje |
| --- | --- |
| Usuario inexistente o contraseña incorrecta | `Usuario o contraseña incorrectos.` |
| Cuenta inactiva | `La cuenta está inactiva. Consulte con un administrador.` |
| Falla técnica | `No se pudo iniciar sesión. Inténtelo nuevamente.` |

## Inventario (INV)

Administrador: las cuatro operaciones. Técnico: solo la consulta (ESP-0006); crear, actualizar y dar de baja se ven deshabilitados con el motivo.

| Flujo | Qué hacer y qué se espera | Resultado observado | Evidencia |
| --- | --- | --- | --- |
| Consulta (ESP-0006) | Sin criterios: 7 productos. Buscar «ibu» (parcial), el ID 3 (exacto), el proveedor Laboratorios del Pacífico y la acción terapéutica «Antiinflamatorio», por separado y combinados. Tarjetas y tabla muestran lo mismo. Omeprazol 20 mg muestra stock físico 0. Seleccionar una tarjeta o una fila carga la ficha. | | INV-01 |
| Alta (ESP-0005) | Crear «Cetirizina 10 mg», «Antihistaminico», precio 3.50, Droguería Andina, stock mínimo 5: `Producto registrado correctamente`, ID generado y estado 'Disponible'. Probar también nombre vacío, precio negativo, precio con letras y stock mínimo negativo. El combo de proveedor no ofrece Proveedor Inactivo SAC. | | INV-02 |
| Actualización (ESP-0007) | Seleccionar Amoxicilina 500 mg, «Actualizar producto», cambiar el precio a 7.00 y «Guardar cambios»: `Producto actualizado correctamente`; el ID no cambia. | | INV-03 |
| Baja (ESP-0008) | Con «Cetirizina 10 mg» (sin remesas): confirmar y el producto desaparece. Con Paracetamol 500 mg (con remesas): queda 'Descontinuado'. En el modal, Cancelar y Escape no cambian nada; Enter no confirma. | | INV-04, INV-05 |
| Permisos | Con `tecnico_prueba`: la consulta funciona y los botones de alta, actualización y baja están deshabilitados y muestran el motivo. | | INV-06 |

| Control | Rótulo o uso |
| --- | --- |
| `INV-SRH-PRODUCTO`, `INV-CMB-FILTRO-PROVEEDOR`, `INV-CMB-FILTRO-ACCION-TERAPEUTICA`, `INV-BTN-LEER-PRODUCTO` | Búsqueda, filtros y «Buscar». |
| `INV-PNL-CATALOGO`, `INV-CRD-PRODUCTO`, `INV-BTN-SELECCIONAR-PRODUCTO`, `INV-TBL-PRODUCTOS` | Tarjetas con «Seleccionar» y tabla. |
| `INV-LBL-ID-PRODUCTO`, `INV-TXT-NOMBRE-PRODUCTO`, `INV-TXT-ACCION-TERAPEUTICA`, `INV-NUM-PRECIO-PRODUCTO`, `INV-CMB-PROVEEDOR`, `INV-NUM-STOCK-MINIMO` | Ficha; el ID es de solo lectura. |
| `INV-BTN-CREAR-PRODUCTO`, `INV-BTN-ACTUALIZAR-PRODUCTO`, `INV-BTN-CONFIRMAR-ACTUALIZACION`, `INV-BTN-ELIMINAR-PRODUCTO` | «Crear producto», «Actualizar producto», «Guardar cambios» y «Dar de baja». |

| Situación | Mensaje |
| --- | --- |
| Con o sin resultados | `Productos encontrados: {cantidad}.` / `No se encontraron productos.` |
| Alta, actualización y baja correctas | `Producto registrado correctamente` / `Producto actualizado correctamente` / `Producto dado de baja correctamente` |
| Validaciones | `El nombre del producto es obligatorio.`, `La acción terapéutica es obligatoria.`, `El precio de venta debe ser mayor o igual a cero.`, `El precio de venta debe ser un número válido.`, `El stock mínimo debe ser mayor o igual a cero.`, `Seleccione un proveedor.`, `El proveedor seleccionado no está activo.` |
| Confirmación de baja | `¿Está seguro de dar de baja este producto?` |

Riesgos pendientes de la gestión de riesgos: `ILA-0006-R01` (búsqueda exacta o parcial; la ESP vigente dice parcial), `ILA-0006-R02` (indicador de vencimiento, que la ESP no define) e `ILA-0006-R03` (sin paginación). Anota lo que observes sin resolver aquí la diferencia.

## Proveedores (PRV)

Solo el Administrador.

| Flujo | Qué hacer y qué se espera | Resultado observado | Evidencia |
| --- | --- | --- | --- |
| Consulta (ESP-0038) | Sin criterios: 4 proveedores. Buscar «20100070970» (RUC exacto) y «salud» (razón social parcial); filtro de estado «Activo», «Inactivo» y «Todos», solo y con texto. La tabla incluye RUC, razón social, teléfono y estado. | | PRV-01 |
| Alta (ESP-0037) | Crear RUC 20512345678, «Farmacéutica Norte SAC», sin teléfono, 'Activo': `Proveedor registrado correctamente`. Probar RUC de 10 y 12 dígitos, con letras y repetido (20100070970). Después, en Inventario, el nuevo proveedor aparece en el combo. | | PRV-02 |
| Actualización (ESP-0039) | Seleccionar «Farmacéutica Norte SAC», «Actualizar proveedor», poner el teléfono 054111222 y «Guardar cambios»: `Proveedor actualizado correctamente`. Cambiarlo a 'Inactivo': deja de ofrecerse en el combo de Inventario. Probar el RUC de otro proveedor: rechazo. | | PRV-03 |
| Baja (ESP-0040) | Droguería Andina SAC: `El proveedor tiene órdenes de devolución pendientes de despacho` y no hay modal. Distribuidora Salud Sur SAC: confirmar y queda 'Inactivo'. «Farmacéutica Norte SAC»: confirmar y desaparece. Cancelar y Escape no cambian nada. | | PRV-04, PRV-05 |
| Permisos | Con `tecnico_prueba`, Proveedores está deshabilitado en el menú (ver [Inicio de sesión](#inicio-de-sesion-y-ventana-principal)). | | COM-04 |

| Control | Rótulo o uso |
| --- | --- |
| `PRV-SRH-PROVEEDOR`, `PRV-CMB-FILTRO-ESTADO`, `PRV-BTN-LEER-PROVEEDOR` | Búsqueda, filtro de estado y «Buscar». |
| `PRV-PNL-PROVEEDORES`, `PRV-CRD-PROVEEDOR`, `PRV-TBL-PROVEEDORES` | Tarjetas y tabla. La vista agrega `PRV-BTN-SELECCIONAR-PROVEEDOR`, que la guía no define (ver [E1](#hallazgos-de-la-revision-estatica)). |
| `PRV-TXT-RUC`, `PRV-TXT-RAZON-SOCIAL`, `PRV-TXT-TELEFONO`, `PRV-CMB-ESTADO-PROVEEDOR` | Ficha. |
| `PRV-BTN-CREAR-PROVEEDOR`, `PRV-BTN-ACTUALIZAR-PROVEEDOR`, `PRV-BTN-CONFIRMAR-ACTUALIZACION`, `PRV-BTN-ELIMINAR-PROVEEDOR` | «Crear proveedor», «Actualizar proveedor», «Guardar cambios» y «Dar de baja». |

| Situación | Mensaje |
| --- | --- |
| Con o sin resultados | `Proveedores encontrados: {cantidad}.` / `No se encontraron proveedores.` |
| Alta, actualización y baja correctas | `Proveedor registrado correctamente` / `Proveedor actualizado correctamente` / `Proveedor dado de baja correctamente` |
| Validaciones | `El RUC debe tener exactamente 11 dígitos numéricos.`, `Ya existe un proveedor con ese RUC.`, `Ingrese la razón social del proveedor.`, `La razón social no puede superar los 150 caracteres.`, `El teléfono no puede superar los 15 caracteres.` |
| Órdenes pendientes | `El proveedor tiene órdenes de devolución pendientes de despacho` |
| Confirmación de baja | `¿Está seguro de dar de baja este proveedor? Los productos y devoluciones ya registrados no se verán afectados.` |

Riesgo pendiente: `ILA-0038-R01` (la consulta exigiría criterio e igualdad exacta; la ESP vigente admite consulta sin criterios y búsqueda parcial).

## Usuarios (USR)

Solo el Administrador.

| Flujo | Qué hacer y qué se espera | Resultado observado | Evidencia |
| --- | --- | --- | --- |
| Consulta (ESP-0034) | Sin criterios: 2 cuentas. Buscar «admin» y filtrar por rol «Tecnico» y «Todos». Tarjetas y tabla muestran usuario, rol y estado; nunca la contraseña. Anota si la búsqueda es exacta o parcial (`ILA-0034-R01`). | | USR-01 |
| Alta (ESP-0033) | Crear `cajero_prueba`, una contraseña cualquiera, 'Tecnico', 'Activo': `Usuario registrado correctamente`. Probar nombre vacío, solo espacios, más de 50 caracteres y repetido con otras mayúsculas (`ADMIN_PRUEBA`). | | USR-02 |
| Actualización (ESP-0035) | Seleccionar `cajero_prueba`, «Actualizar», cambiar el rol a 'Administrador' sin escribir contraseña y «Guardar cambios»: `Usuario actualizado correctamente` y la contraseña anterior sigue sirviendo. Con la cuenta de la sesión, cambiar su rol o estado: rechazo. | | USR-03 |
| Baja (ESP-0036) | `admin_prueba` (la sesión): `No es posible dar de baja la cuenta con la que se encuentra abierta la sesión`. `cajero_prueba` (sin operaciones): confirmar y desaparece. `tecnico_prueba` (con ventas): confirmar y queda 'Inactivo'. Cancelar y Escape no cambian nada. | | USR-04, USR-05 |
| Permisos | Con `tecnico_prueba`, Usuarios está deshabilitado en el menú. | | COM-04 |

| Control | Rótulo o uso |
| --- | --- |
| `USR-SRH-USUARIO`, `USR-CMB-FILTRO-ROL`, `USR-BTN-LEER-USUARIO` | Búsqueda, filtro de rol y «Buscar». |
| `USR-PNL-CUENTAS`, `USR-CRD-USUARIO`, `USR-TBL-USUARIOS` | Tarjetas (sin botón: se seleccionan con un clic) y tabla. |
| `USR-TXT-NOMBRE-USUARIO`, `USR-PWD-CONTRASENA`, `USR-CMB-ROL`, `USR-CMB-ESTADO-USUARIO` | Ficha; la contraseña va enmascarada. |
| `USR-BTN-CREAR-USUARIO`, `USR-BTN-ACTUALIZAR-USUARIO`, `USR-BTN-CONFIRMAR-ACTUALIZACION`, `USR-BTN-ELIMINAR-USUARIO` | «Crear», «Actualizar», «Guardar cambios» y «Eliminar». |

| Situación | Mensaje |
| --- | --- |
| Con o sin resultados | `Cuentas encontradas: {cantidad}.` / `No se encontraron cuentas.` |
| Alta, actualización y baja correctas | `Usuario registrado correctamente` / `Usuario actualizado correctamente` / `Cuenta dada de baja correctamente` |
| Reglas | `El sistema debe conservar al menos una cuenta administradora activa`, `No es posible cambiar el rol ni el estado de la cuenta con la que se encuentra abierta la sesión`, `No es posible dar de baja la cuenta con la que se encuentra abierta la sesión` |
| Validaciones | `Ingrese el nombre de usuario.`, `El nombre de usuario no puede superar los 50 caracteres.`, `Ya existe una cuenta con ese nombre de usuario.`, `Ingrese la contraseña.` |
| Confirmación de baja | `¿Está seguro de dar de baja esta cuenta? El histórico de operaciones que registró se conservará.` |

Riesgos pendientes: `ILA-0033-R01` (sin política mínima de contraseña), `ILA-0034-R01` (búsqueda exacta o parcial) e `ILA-0035-R02` (efecto de un cambio de rol sobre una sesión ya abierta).

## Métodos de pago (PAG)

Administrador: las cuatro operaciones (#186). Técnico: solo la consulta (ESP-0018); Crear, Modificar y Eliminar se ven deshabilitados con el motivo.

| Flujo | Qué hacer y qué se espera | Resultado observado | Evidencia |
| --- | --- | --- | --- |
| Consulta (ESP-0018) | Sin criterios: 3 métodos. Buscar «ya» (parcial) y filtrar por «Activo», «Inactivo» y «Todos», solo y con texto. Tarjetas y tabla muestran lo mismo. | | PAG-01 |
| Alta (ESP-0017) | Crear «Plin», 'Activo': `Método de pago registrado correctamente`; aparece en el combo de método de la pantalla de Ventas. Probar nombre vacío, más de 50 caracteres y repetido («yape»). | | PAG-03 |
| Actualización (ESP-0019) | Seleccionar «Plin», «Modificar», cambiarlo a 'Inactivo' y «Guardar cambios»: `Método de pago actualizado correctamente`; deja de ofrecerse en Ventas. Probar renombrarlo a «Efectivo»: rechazo. | | PAG-04 |
| Baja (ESP-0020) | Tarjeta (sin ventas): confirmar y desaparece. Yape (con ventas): confirmar y queda 'Inactivo'. Cancelar y Escape no cambian nada. | | PAG-05, PAG-06 |
| Permisos | Con `tecnico_prueba`: la consulta funciona y Crear, Modificar y Eliminar están deshabilitados con el motivo. | | PAG-02 |

| Control | Rótulo o uso |
| --- | --- |
| `PAG-SRH-METODO`, `PAG-CMB-FILTRO-ESTADO`, `PAG-BTN-LEER-METODO` | Búsqueda, filtro de estado y «Buscar». |
| `PAG-PNL-METODOS-PAGO`, `PAG-CRD-METODO-PAGO`, `PAG-TBL-METODOS-PAGO` | Tarjetas y tabla. |
| `PAG-TXT-METODO-PAGO`, `PAG-CMB-ESTADO-METODO` | Ficha. |
| `PAG-BTN-CREAR-METODO`, `PAG-BTN-ACTUALIZAR-METODO`, `PAG-BTN-CONFIRMAR-ACTUALIZACION`, `PAG-BTN-ELIMINAR-METODO` | «Crear», «Modificar», «Guardar cambios» y «Eliminar». |

| Situación | Mensaje |
| --- | --- |
| Con o sin resultados | `Métodos de pago encontrados: {cantidad}.` / `No se encontraron métodos de pago.` |
| Alta, actualización y baja correctas | `Método de pago registrado correctamente` / `Método de pago actualizado correctamente` / `Método de pago dado de baja correctamente` |
| Validaciones | `El nombre del método de pago es obligatorio.`, `El nombre del método de pago no puede exceder los 50 caracteres.`, `Ya existe un método de pago con ese nombre.` |
| Confirmación de baja | `¿Está seguro de dar de baja este método de pago? Las ventas ya registradas con él no se verán afectadas.` |

<a id="hallazgos-de-la-revision-estatica"></a>
## Hallazgos de la revisión estática

Revisión del XAML, las VistaModelos y los mensajes contra las especificaciones, la guía de nomenclatura y la matriz de permisos (25/11/2026), y de las capturas de la versión 02.01. Los mensajes de éxito, de confirmación y de las reglas de negocio coinciden con las especificaciones, y el menú coincide con la matriz de permisos.

| Código | Hallazgo | Estado |
| --- | --- | --- |
| E1 | La tarjeta de Proveedores tiene un botón «Seleccionar» con `PRV-BTN-SELECCIONAR-PROVEEDOR`, que no está en la guía de nomenclatura. ESP-0038 a ESP-0040 seleccionan el proveedor en `PRV-TBL-PROVEEDORES` o en `PRV-PNL-PROVEEDORES`, sin botón (DEC-060). | Confirmado en el código: registrar como issue. |
| C1 | En las capturas, las tablas de Inventario y de Proveedores muestran una o dos filas con la ventana a su tamaño normal. | Confirmar en la sesión. |
| C2 | En las capturas, la columna «Estado» de la tabla de Usuarios queda cortada a la derecha. | Confirmar en la sesión. |

## Capturas

Las capturas están en [evidencias-pdf/](evidencias-pdf/), sin contraseñas ni rutas personales. Estas no muestran lo que dice su nombre y hay que repetirlas en la sesión, con el mismo nombre de archivo:

| Captura | Qué muestra hoy | Qué debe mostrar |
| --- | --- | --- |
| COM-03-inicio-tecnico | La pantalla de inicio, sin la cuenta | La ventana principal con «tecnico_prueba · Tecnico» |
| COM-04-menu-tecnico | El menú del Administrador | El menú del Técnico, con Usuarios y Proveedores deshabilitados |
| INV-02-alta-resultado | La selección de Amoxicilina | El mensaje y el producto nuevo con su ID |
| INV-06-permisos-tecnico | La sesión del Administrador | Inventario con la sesión del Técnico y los botones deshabilitados |
| PAG-02-consulta-tecnico | La sesión del Administrador | Métodos de pago con la sesión del Técnico y los botones deshabilitados |
| PAG-04-actualizacion-resultado | Solo la tabla, sin mensaje | El mensaje y el método actualizado |
| PRV-03-actualizacion-resultado | La lista después de un alta | El mensaje y el proveedor actualizado |
| USR-02-alta-resultado | Texto en el buscador, sin cuenta nueva | El mensaje y la cuenta nueva |
| USR-03-actualizacion-resultado | Solo la tabla, sin mensaje | El mensaje y la cuenta actualizada |

[Evidencia-POC-generacion-PDF.md](evidencias-pdf/Evidencia-POC-generacion-PDF.md) y sus capturas `01` a `03` son de la prueba de concepto del PDF, no de esta guía.

<a id="plantilla-de-sesion"></a>
## Plantilla de sesión

```text
Fecha:
Duración:
Participante:
Cuenta y rol:
Módulo / pantalla:
Entorno (SO, commit de la aplicación, instancia y base):

Pasos realizados:
1.
2.
3.

Resultado esperado:
Resultado observado:
Hallazgos (HAL-NNN):
Evidencia:
```

## Reporte de defectos

Un error encontrado en la sesión se registra como `HAL-NNN` en [seguimiento/15-hallazgos-de-uso.md](../../seguimiento/15-hallazgos-de-uso.md) (DEC-081) y se corrige con su propio issue, que incluye:

```text
Módulo y pantalla:
Cuenta y rol:
Pasos para reproducir:
1.
2.
3.
Resultado esperado (con la ESP):
Resultado obtenido:
Evidencia:
```

No adjuntes contraseñas ni datos reales.

## Fuentes y diferencias por confirmar

- Las especificaciones están en [03-especificaciones.md](../../catalogo/03-especificaciones.md), los riesgos en [10-gestion-riesgos.md](../../riesgos/10-gestion-riesgos.md) y las pruebas del plan en [09-pruebas-software.md](<../../pruebas de software/09-pruebas-software.md>).
- Los riesgos `ILA-0006-R01`, `ILA-0034-R01` e `ILA-0038-R01` siguen pendientes en la gestión de riesgos; esta guía no los resuelve.
- Las pruebas automatizadas usan repositorios falsos. Esta guía es la evidencia manual sobre la base de pruebas.
