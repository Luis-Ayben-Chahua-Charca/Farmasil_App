# Requisitos No Funcionales — FARMASIL

---

| Código | RNF-0001 |
| --- | --- |
| Nombre | Funcionamiento sin conexión a internet permanente |
| Atributo de calidad | Disponibilidad |
| Versión | 01.00 |
| Fecha | 06/05/2026 |
| Autor de la plantilla | AUT-0004 |
| Fuente | Ninguno |
| Experto | Ninguno |
| Descripción | El sistema debe ser capaz de operar plenamente sin una conexión a internet constante, permitiendo el registro de ventas y consulta de stock de forma local. |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La farmacia solo cuenta con internet mediante datos móviles intermitentes. |

| Código | RNF-0002 |
| --- | --- |
| Nombre | Facilidad de uso de la interfaz sin capacitación |
| Atributo de calidad | Usabilidad |
| Versión | 02.00 |
| Fecha | 09/05/2026 |
| Autor de la plantilla | AUT-0004 |
| Fuente | Ninguno |
| Experto | Ninguno |
| Descripción | A pesar de ser un sistema completo y complejo en sus funciones, la interfaz debe ser lo suficientemente intuitiva para ser operada por las técnicas en farmacia sin entrenamiento previo. |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La dueña no posee experiencia previa en el uso de sistemas de información. |

| Código | RNF-0003 |
| --- | --- |
| Nombre | Soporte de usuarios simultáneos |
| Atributo de calidad | Concurrencia |
| Versión | 01.00 |
| Fecha | 06/05/2026 |
| Autor de la plantilla | AUT-0004 |
| Fuente | Ninguno |
| Experto | Ninguno |
| Descripción | El sistema debe permitir que al menos 2 personas operen y realicen transacciones simultáneamente sin pérdida de integridad de datos. |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Ninguno |

| Código | RNF-0004 |
| --- | --- |
| Nombre | Tiempo de recuperación del servicio |
| Atributo de calidad | Fiabilidad |
| Versión | 01.00 |
| Fecha | 06/05/2026 |
| Autor de la plantilla | AUT-0004 |
| Fuente | Ninguno |
| Experto | Ninguno |
| Descripción | En caso de daño en el hardware, el sistema y sus datos deben poder restablecerse en un plazo máximo de 15 días calendario. |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Ninguno |

| Código | RNF-0005 |
| --- | --- |
| Nombre | Velocidad en el registro de ventas |
| Atributo de calidad | Rendimiento |
| Versión | 02.00 |
| Fecha | 19/07/2026 |
| Autor de la plantilla | AUT-0007 |
| Fuente | Entrevista 2 |
| Experto | Ninguno |
| Descripción | El sistema debe registrar y guardar una venta completamente en un tiempo máximo de 4 segundos desde que el usuario confirma la transacción, incluyendo el descuento automático del stock de la remesa vendida. |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Este límite de tiempo, obtenido en la Entrevista 2, se utiliza como criterio para seleccionar el hardware objetivo (RNF-0007) y el lenguaje y framework de desarrollo (RNF-0008). Pendiente anexar el archivo de la Entrevista 2 al proyecto para asignar el código de fuente (FUE) definitivo. |

| Código | RNF-0006 |
| --- | --- |
| Nombre | Control de acceso, autorización y protección de credenciales |
| Atributo de calidad | Seguridad |
| Versión | 05.00 |
| Fecha | 13/10/2026 |
| Autor de la plantilla | AUT-0001 |
| Fuente | Entrevista 2 |
| Experto | Ninguno |
| Descripción | El sistema debe controlar quién accede y qué puede hacer cada persona dentro de él, conforme a las siguientes condiciones:<br>**1. Autenticación.** El ingreso al sistema se realiza únicamente mediante nombre de usuario y contraseña, validados contra **DB_FARMASIL.TBL_USUARIOS**. Una cuenta cuyo `estado` sea 'Inactivo' no puede iniciar sesión, aunque sus credenciales sean correctas.<br>**2. Identificación del usuario en las operaciones.** La sesión activa identifica al usuario durante toda su duración, y su identificador queda registrado en cada venta, orden de devolución y cotización de proveedor que registre, de modo que toda operación sea atribuible a una persona concreta.<br>**3. Autorización por rol.** Cada cuenta tiene asignado un rol. Los roles vigentes son 'Administrador', correspondiente a la dueña de la farmacia, y 'Tecnico', correspondiente al personal que atiende el mostrador. Los permisos de cada rol sobre cada operación del sistema son los establecidos en la Matriz de Actores, Roles y Permisos, documento que constituye la fuente única para esta materia. El sistema verifica el rol antes de ejecutar cualquier operación, y no confía en que la interfaz haya impedido el acceso.<br>**4. Comportamiento ante una operación no autorizada.** Las opciones que el rol del usuario no permite se muestran visibles pero deshabilitadas, indicando el motivo cuando el usuario intenta activarlas. No se ocultan, para que el personal conozca las funciones que el sistema ofrece, ni se dejan activas para rechazarlas después.<br>**5. Protección de credenciales.** La contraseña se almacena como hash irreversible con sal individual (PBKDF2, bcrypt o un algoritmo equivalente), nunca en texto plano ni con un cifrado reversible. El sistema no la muestra en pantalla, no la devuelve en el resultado de ninguna consulta y no la incluye en reportes ni en archivos de registro. Los formularios que la capturan enmascaran su contenido.<br>**6. Continuidad de la administración.** El sistema debe conservar en todo momento al menos una cuenta activa con rol 'Administrador'. Ninguna operación puede dejar al sistema sin capacidad de administración. |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Requisito derivado de la Entrevista 2. Pendiente anexar el archivo de la entrevista para asignar el código FUE definitivo.<br>**Refundido en la versión 03.00.** La versión anterior describía el control de acceso en una sola frase que enumeraba tres capacidades del administrador y dos del operador. Esa redacción presentaba cuatro problemas: llamaba *operador* a un perfil que el Diccionario de Datos almacena como `'Tecnico'` y las educciones llaman técnica en farmacia; enumeraba cinco capacidades cuando el catálogo tiene 44 operaciones, y lo hacía con la palabra "solo", de modo que contradecía a la Matriz de Permisos; no mencionaba el encriptado de la contraseña, pese a que ESP-0033 y ESP-0034 lo invocaban citando este requisito; y no contemplaba que una cuenta dada de baja no pueda iniciar sesión, condición que ILA-0036 y ESP-0036 afirman en su postcondición sin que ningún requisito la respaldara.<br>**Extensibilidad.** La condición 3 delega los permisos concretos en la Matriz de Permisos en lugar de enumerarlos aquí. Esa delegación es deliberada: incorporar un rol adicional en el futuro exige actualizar un único artefacto, y no reescribir la descripción de este requisito ni las precondiciones de las 44 ilaciones.<br>**Versión 04.00:** la condición 2 incorpora las cotizaciones de proveedor, que registran `id_usuario` desde el Diccionario de Datos v05.00.<br>**Versión 05.00:** la condición 5 precisa el mecanismo: hash con sal en lugar de "encriptada", término que admitía un cifrado reversible. |

| Código | RNF-0007 |
| --- | --- |
| Nombre | Funcionamiento en el equipo objetivo |
| Atributo de calidad | Compatibilidad |
| Versión | 03.00 |
| Fecha | 19/07/2026 |
| Autor de la plantilla | AUT-0007 |
| Fuente | Entrevista 2 |
| Experto | Ninguno |
| Descripción | El sistema debe ejecutarse correctamente en el equipo definido como objetivo de despliegue: sistema operativo Windows 11, procesador Intel Core i5-12500 (o equivalente), 16 GB de memoria RAM y 1 TB de almacenamiento, sin necesidad de instalar programas adicionales ni licencias de software externo distintas a las ya contempladas en el proyecto (SQL Server Express). |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Reemplaza la especificación mínima anterior (Windows 10, doble núcleo, 4 GB RAM), actualizada tras confirmar en la Entrevista 2 el equipo real sobre el cual se instalará el sistema. Esta especificación, junto con RNF-0005, fue la base para seleccionar el stack tecnológico definido en RNF-0008. |

| Código | RNF-0008 |
| --- | --- |
| Nombre | Stack tecnológico de implementación |
| Atributo de calidad | Restricción de implementación |
| Versión | 01.00 |
| Fecha | 19/07/2026 |
| Autor de la plantilla | AUT-0007 |
| Fuente | Ninguno |
| Experto | Ninguno |
| Descripción | El sistema debe desarrollarse utilizando el lenguaje C#, el framework de interfaz WPF, .NET 8 como plataforma de ejecución y SQL Server Express como motor de base de datos. |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Decisión técnica del equipo de desarrollo, no impuesta directamente por el cliente, basada en la capacidad del hardware objetivo (RNF-0007) y en el requisito de rendimiento de 4 segundos por transacción (RNF-0005). Al ser una restricción de implementación, su fuente es el análisis técnico interno del equipo y no la entrevista. |

| Código | RNF-0009 |
| --- | --- |
| Nombre | Integridad y respaldo de la base de datos DB_FARMASIL |
| Atributo de calidad | Integridad |
| Versión | 06.00 |
| Fecha | 13/10/2026 |
| Autor de la plantilla | AUT-0004 |
| Fuente | Ninguno |
| Experto | Ninguno |
| Descripción | El sistema debe almacenar la información de: Usuarios (TBL_USUARIOS), Proveedores (TBL_PROVEEDORES), Productos (TBL_PRODUCTOS), Lotes (TBL_LOTES), Ventas (TBL_REGISTRO_VENTAS), Detalle de ventas (TBL_DETALLE_VENTAS), Comprobantes tributarios (TBL_COMPROBANTES_TRIBUTARIOS), Métodos de pago (TBL_METODOS_PAGO), Alertas de vencimiento (TBL_ALERTAS_VENCIMIENTO), Productos retirados (TBL_PRODUCTOS_RETIRADOS), Órdenes de devolución (TBL_ORDENES_DEVOLUCION), Detalle de devolución (TBL_DETALLE_DEVOLUCION), Restricciones de venta (TBL_RESTRICCIONES_VENTA), Reportes de ventas (TBL_REPORTES_VENTAS), Lista de reabastecimiento (TBL_LISTA_REABASTECIMIENTO) y Cotizaciones de proveedores (TBL_COTIZACIONES_PROVEEDOR), en una base de datos estructurada (DB_FARMASIL), garantizando que los registros no se dupliquen, no se pierdan y mantengan consistencia entre los módulos del sistema. Además, la base de datos debe permitir realizar copias de respaldo periódicas para recuperar la información en caso de fallas del equipo, errores del usuario o pérdida de datos. |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Este requisito es necesario debido a que la farmacia actualmente maneja parte de su información de forma manual y ha presentado pérdida de registros físicos.<br>En la versión 05.00 se incorporó `TBL_LOTES`, que eleva el modelo a catorce tablas, conforme al Diccionario de Datos v04.00.<br>**Versión 06.00:** se incorporan `TBL_LISTA_REABASTECIMIENTO` y `TBL_COTIZACIONES_PROVEEDOR`, que elevan el modelo a dieciséis tablas, conforme al Diccionario de Datos v06.00. |

| Código | RNF-0010 |
| --- | --- |
| Nombre | Procesos programados del sistema |
| Atributo de calidad | Adecuación funcional |
| Versión | 01.00 |
| Fecha | 13/10/2026 |
| Autor de la plantilla | AUT-0001 |
| Fuente | Entrevista 1, Entrevista 3 |
| Experto | Ninguno |
| Descripción | El sistema debe ejecutar, sin intervención del usuario y de forma local (RNF-0001), los dos procesos siguientes:<br>**1. Evaluación de vencimientos.** Al iniciar la aplicación, y luego cada 24 horas mientras permanezca abierta, el sistema identifica las remesas con `estado_lote` = 'Disponible' cuya `fecha_vencimiento` sea igual o anterior a la fecha actual más el umbral `umbral_meses` de **DB_FARMASIL.TBL_ALERTAS_VENCIMIENTO**, y muestra un aviso con la cantidad encontrada que conduce a **ALV-TBL-ALERTAS-VENCIMIENTO**. El cálculo es el mismo de ESP-0013 y no persiste registros: las alertas por vencimiento se derivan de **DB_FARMASIL.TBL_LOTES** en cada evaluación, y los retiros manuales siguen registrándose por ESP-0014.<br>**2. Generación de la lista de reabastecimiento.** Cada 15 días, el sistema agrega a **DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO**, con origen 'Automático', prioridad 'Normal' y la fecha actual, cada medicamento con `estado_producto` = 'Disponible' cuya suma de `stock_actual` en sus remesas 'Disponible' sea inferior a `stock_minimo` y que no figure ya en la lista. El proceso no modifica la prioridad de los ítems existentes ni retira ítems de la lista: un ítem lo retira la dueña cuando el pedido se realizó (ESP-0048), y si el medicamento sigue bajo el mínimo vuelve a agregarse en la siguiente ejecución. Si al iniciar la aplicación han pasado 15 días o más desde la última ejecución, el proceso se ejecuta en ese momento. |
| Importancia | Importante |
| Estado | Pendiente |
| Comentario | Reúne en un solo requisito los dos procesos que el catálogo describía sin requisito formal: la generación de alertas a la que remitían ILA-0013 y ESP-0013, y la generación automática de la lista de reabastecimiento que mencionaban ILA-0045, ESP-0045 y el Anexo de `02-ilaciones.md`.<br>La periodicidad de 15 días del segundo proceso proviene de la Entrevista 3. La frecuencia diaria del primero es una decisión del equipo, pensada para que las remesas próximas a vencer se vean al empezar cada jornada; puede ajustarse con la dueña.<br>La fecha de la última ejecución del proceso 2 se conserva en la configuración local de la aplicación, no en `DB_FARMASIL`. |

---

## Requisitos identificados y no incorporados

Se dejan documentados para que la omisión conste como decisión y no como olvido.

| Tema | Situación | Motivo |
| --- | --- | --- |
| Generación automática de alertas de vencimiento y de la lista de reabastecimiento | Incorporado como RNF-0010 el 13/10/2026 | ILA-0013 y el Anexo de `02-ilaciones.md` remitían a un requisito que no existía. |
| Gestión de roles configurable | No incorporado | Ningún enunciado del cliente la solicita. Ver la nota de evolución más abajo. |
| Alertas de bajo stock, registro de clientes, tipo y presentación del producto, regla de descuentos | No incorporados | Solicitados por la dueña en la Entrevista 1 pero fuera del alcance acordado. Documentados en el Diccionario de Datos v04.00 como vacíos del modelo pendientes de decisión. |

## Nota de evolución: gestión de roles

El sistema opera hoy con dos roles fijos, definidos por el cliente. Si en el futuro se requiriera un tercero, la vía de crecimiento sería normalizar el rol en una tabla `TBL_ROLES`, incorporar `TBL_PERMISOS` y `TBL_ROL_PERMISO`, y añadir el módulo funcional que administre esas entidades.

No se incorpora ahora por tres razones. Ningún enunciado del cliente lo solicita, y sería la única educción del catálogo sin fuente trazable. La escala del negocio no lo justifica: un equipo de despliegue (RNF-0007), dos usuarios simultáneos (RNF-0003) y dos perfiles declarados por la dueña. Y convertir los permisos en datos modificables en tiempo de ejecución debilitaría la verificación: las precondiciones de las 44 ilaciones dejarían de garantizar quién puede ejecutar cada operación, para depender del contenido de una tabla.

La escalabilidad se resuelve por la vía barata en la condición 3 de RNF-0006: al delegar los permisos concretos en la Matriz de Permisos en lugar de enumerarlos en el texto del requisito, incorporar un rol adicional afecta a un solo artefacto.


> **Actualización del 05/09/2026.** Cambios respecto a la versión anterior:
> - **RNF-0006 refundido a la versión 03.00.** Pasa de describir el control de acceso en una sola frase enumerativa a cubrir las cinco características de seguridad que el catálogo necesita: autenticación, identificación del usuario en la trazabilidad, autorización por rol, comportamiento ante intento no autorizado y protección de credenciales. Se detallan los motivos en el comentario del propio requisito.
> - **RNF-0009 actualizado a la versión 05.00** para incorporar `TBL_LOTES`, que eleva el modelo a catorce tablas.
> - El resto de los requisitos se reproduce sin cambios.

> **Actualización del 13/10/2026.** Cambios respecto a la versión anterior:
> - **RNF-0010 nuevo**, procesos programados del sistema: evaluación de vencimientos y generación de la lista de reabastecimiento.
> - **RNF-0009 actualizado a la versión 06.00**: dieciséis tablas, conforme al Diccionario de Datos v06.00.
> - **RNF-0006 actualizado a la versión 04.00**: la condición 2 incorpora las cotizaciones de proveedor.
> - **RNF-0006 actualizado a la versión 05.00**: la contraseña se almacena como hash con sal.
