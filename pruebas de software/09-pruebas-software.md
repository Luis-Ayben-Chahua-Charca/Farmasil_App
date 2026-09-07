# Pruebas de Software — Proyecto FARMASIL

| Artefacto | Pruebas de software |
| --- | --- |
| Versión | 03.00 |
| Fecha | 05/09/2026 |
| Responsable | AUT-0001 |

> **Versión 03.00.** Reduce el plan a las pruebas que el sistema necesita. La versión 02.00 contenía 294 pruebas, de las cuales unas cuarenta repetían el mismo caso de verificación de rol en cada especificación, seis pruebas de carga estaban dispersas por los módulos cuando bastaban tres consolidadas, y varios tipos aparecían una sola vez con una etiqueta forzada, elegidos para que ningún tipo del catálogo quedara sin representar. Esta versión aplica el criterio de que el tipo de prueba lo determina el riesgo del sistema y no la extensión del catálogo.
>
> Sustituye también a la versión 01.00, derivada de especificaciones ya superadas: describía la venta de un único producto, no contemplaba el descuento de stock ni las restricciones sanitarias, y probaba operaciones que dejaron de existir.

## Criterio de diseño

Cada prueba de este documento existe porque cubre una clase de defecto que ninguna otra cubre. El diseño se apoya en cuatro tipos, que son los que encuentran defectos en un sistema de gestión transaccional:

- **T-002 caja blanca**, el camino básico de cada procedimiento. Una por especificación, sin excepción.
- **T-001 caja negra**, particiones de equivalencia y valores límite sobre las entradas de cada operación.
- **T-006 integración**, el efecto que una operación produce sobre otro módulo. Es el tipo más denso del plan porque aquí está el riesgo real: el stock, los bloqueos por lote y la relación entre venta y comprobante.
- **T-012 seguridad**, reservada a las transacciones y a los tres casos de control de acceso con lógica propia.

**Verificación de rol.** Se prueba una vez por módulo y de forma sistemática en la sección de requisitos no funcionales, recorriendo las 44 operaciones contra la Matriz de Permisos. Repetirla en cada especificación producía cuarenta veces el mismo caso.

**Rendimiento y concurrencia.** Se concentran en la sección de requisitos no funcionales. No son opcionales: RNF-0005 fija un máximo de 4 segundos por venta y RNF-0003 exige dos usuarios simultáneos, de modo que sin ellas esos dos requisitos quedarían sin forma de darse por aprobados. Pero una vez consolidadas, no necesitan repetirse por módulo.

**Sobre la priorización.** Se evaluó marcar cada prueba como crítica o secundaria, y se descartó: al aplicar el criterio de "sin esta prueba no puede afirmarse que el requisito se cumple", 198 de las 223 quedaban como críticas, con lo que la distinción no discriminaba nada. La razón es que en un sistema transaccional casi toda prueba custodia la integridad de los datos: el stock, el bloqueo por remesa, la relación entre venta y comprobante o la atomicidad de una transacción no admiten quedar sin verificar. La priorización útil aquí es por módulo, y está al final del documento.

## Tipos de prueba utilizados

| Clave | Descripción | Uso en este plan |
| --- | --- | --- |
| T-001 | Prueba de caja negra | Particiones y valores límite de las entradas |
| T-002 | Prueba de caja blanca | Camino básico de cada procedimiento |
| T-003 | Prueba de complejidad ciclomática | Operaciones con tres o más ramas |
| T-004 | Prueba unitaria | Validaciones de campo con lógica propia |
| T-005 | Prueba de aceptación | Demostración al cliente de las funciones que solicitó |
| T-006 | Prueba de integración | Efectos entre módulos |
| T-007 | Prueba de regresión | Integridad referencial e histórico tras bajas y cambios |
| T-008 | Prueba de carga | Concurrencia y volumen de datos |
| T-010 | Prueba de escalabilidad | Crecimiento del volumen a varios años |
| T-012 | Prueba de seguridad | Transacciones, control de acceso y credenciales |
| T-015 | Prueba de compatibilidad | Visualización en el equipo objetivo |
| T-016 | Prueba de usabilidad | Operación sin capacitación previa |
| T-017 | Prueba de rendimiento | Límite de 4 segundos por venta |
| T-018 | Prueba de configuración | Esquema y parámetros del sistema |
| T-019 | Prueba de conversión | Restauración de la base desde respaldo |
| T-020 | Prueba de instalación | Despliegue en el equipo objetivo |

**Tipos del catálogo que no se utilizan, con su motivo.** T-009 prueba de estrés: el sistema opera con dos usuarios en un solo equipo según RNF-0003 y RNF-0007, de modo que no existe un escenario de sobrecarga que probar más allá de la prueba de carga. T-011 portabilidad: el despliegue es sobre un único equipo definido, ya cubierto por T-020. T-013 interoperabilidad: el sistema no intercambia información con ningún sistema externo. T-014 componente: la arquitectura no se compone de módulos desarrollados por terceros.

T-019 se emplea para la restauración desde respaldo por ser el tipo más cercano disponible; el catálogo del capítulo 10 no contempla una prueba de recuperación específica.

---

## Módulo 1 — Gestión de Ventas (EDU-0001)

| Clave | Requisito (Nombre / Operación) | Descripción de la prueba | Tipo |
| --- | --- | --- | --- |
| ESP-0001 | Creación del registro de venta (Crear) | Camino básico: selección de medicamento, propuesta de remesa por FEFO, cantidad, cálculo del monto, método de pago y confirmación. | T-002 |
|  |  | Partición de equivalencia sobre la cantidad: menor al stock, igual al stock, mayor al stock, cero, negativa y no numérica. | T-001 |
|  |  | Con tres remesas disponibles de vencimientos distintos, verificar que el sistema proponga la más próxima a vencer. | T-004 |
|  |  | Registrar una venta con cuatro medicamentos y verificar que se creen cuatro filas de detalle con su id_lote y una sola cabecera. | T-006 |
|  |  | Verificar que el stock de cada remesa quede descontado y que la que llegue a cero pase a 'Agotado'. | T-006 |
|  |  | Intentar agregar un medicamento con restricción 'Bloqueante' activa y verificar que la línea se rechace con su motivo. | T-006 |
|  |  | Intentar agregar una remesa bloqueada y verificar que se rechace, comprobando que las demás del mismo medicamento sigan ofreciéndose. | T-006 |
|  |  | Interrumpir la transacción entre la cabecera y el detalle y verificar que no quede una venta sin líneas ni stock descontado. | T-012 |
|  |  | Verificar que la venta registre el id_usuario de la sesión activa. | T-012 |
|  |  | Presentar al cliente la pantalla de venta con varios productos y el monto total calculado. | T-005 |
| ESP-0002 | Consulta del registro de venta (Leer) | Camino básico: aplicar filtros de fecha y medicamento y desplegar resultados. | T-002 |
|  |  | Partición sobre los filtros: coincidencia exacta, parcial, sin resultados y filtros vacíos. | T-001 |
|  |  | Verificar que el filtro por medicamento se resuelva uniendo detalle y lotes, y devuelva las ventas correctas. | T-006 |
|  |  | Seleccionar una venta y verificar que el detalle muestre el número de lote de cada línea. | T-006 |
| ESP-0003 | Actualización del registro de venta (Actualizar) | Camino básico: seleccionar venta sin comprobante, modificar líneas y método de pago, recalcular y confirmar. | T-002 |
|  |  | Complejidad ciclomática: ramas de comprobante emitido, agregar línea y quitar línea. | T-003 |
|  |  | Intentar actualizar una venta con comprobante 'Emitido' y verificar que la operación se cancele. | T-006 |
|  |  | Aumentar la cantidad de una línea y verificar que el stock se descuente solo por la diferencia. | T-006 |
|  |  | Reducir la cantidad de una línea y verificar que la diferencia se devuelva al stock de la remesa. | T-007 |
|  |  | Verificar que el monto_total resultante coincida con la suma del nuevo detalle. | T-004 |
| ESP-0004 | Eliminación del registro de venta (Eliminar) | Camino básico: seleccionar venta sin comprobante, confirmar y verificar la eliminación en cascada. | T-002 |
|  |  | Complejidad ciclomática: ramas de comprobante emitido, confirmación afirmativa y negativa. | T-003 |
|  |  | Verificar que se restituya el stock de cada remesa antes de eliminar líneas y cabecera. | T-006 |
|  |  | Verificar que no queden filas huérfanas en el detalle tras la eliminación. | T-007 |
|  |  | Intentar eliminar una venta con comprobante 'Emitido' y verificar el rechazo. | T-006 |
|  |  | Presionar el botón de cancelar del modal y verificar que no se produzca ningún cambio. | T-001 |
| Módulo 1 | Control de acceso del módulo | Con sesión 'Tecnico', verificar que ESP-0001 y ESP-0002 se ejecuten y que ESP-0003 y ESP-0004 sean rechazadas. | T-012 |

## Módulo 2 — Gestión de Inventario (EDU-0002)

| Clave | Requisito (Nombre / Operación) | Descripción de la prueba | Tipo |
| --- | --- | --- | --- |
| ESP-0005 | Creación del producto de inventario (Crear) | Camino básico: nombre, acción terapéutica, precio y proveedor, validación y registro con estado 'Disponible'. | T-002 |
|  |  | Partición sobre el precio: positivo, cero, negativo y no numérico; y sobre el nombre vacío. | T-001 |
|  |  | Verificar que el identificador se genere automáticamente y se muestre sin permitir edición. | T-004 |
|  |  | Verificar que un medicamento recién creado, sin remesas, no se ofrezca en el módulo de ventas. | T-006 |
| ESP-0006 | Consulta del producto de inventario (Leer) | Camino básico: búsqueda por identificador o nombre y despliegue del resultado. | T-002 |
|  |  | Partición sobre los criterios: identificador existente, inexistente, nombre parcial y campos vacíos. | T-001 |
|  |  | Verificar que el stock mostrado sea la suma de las existencias de todas las remesas del medicamento. | T-006 |
|  |  | Crear una segunda remesa y verificar que el stock consolidado se actualice sin duplicar la fila del producto. | T-007 |
| ESP-0007 | Actualización del producto de inventario (Actualizar) | Camino básico: seleccionar producto, modificar los campos de catálogo y confirmar. | T-002 |
|  |  | Verificar que el identificador permanezca inalterado y que las referencias desde lotes y restricciones sigan siendo válidas. | T-007 |
|  |  | Modificar el precio y verificar que las ventas ya registradas conserven el precio vigente al momento de la venta. | T-006 |
| ESP-0008 | Eliminación del producto de inventario (Eliminar) | Camino básico: seleccionar producto, confirmar y verificar la baja aplicada. | T-002 |
|  |  | Complejidad ciclomática: ramas de producto con movimientos, sin movimientos y confirmación negativa. | T-003 |
|  |  | Dar de baja un medicamento con remesas y verificar que se aplique baja lógica con estado 'Descontinuado'. | T-006 |
|  |  | Dar de baja un medicamento sin remesas ni restricciones y verificar que se elimine físicamente. | T-001 |
|  |  | Verificar que en ningún caso se produzca una violación de integridad referencial. | T-007 |
| Módulo 2 | Control de acceso del módulo | Con sesión 'Tecnico', verificar que solo ESP-0006 se ejecute y que las otras tres sean rechazadas, conforme a RNF-0006. | T-012 |

## Módulo 3 — Gestión de Documentación Tributaria (EDU-0003)

| Clave | Requisito (Nombre / Operación) | Descripción de la prueba | Tipo |
| --- | --- | --- | --- |
| ESP-0009 | Registro de comprobante tributario (Crear) | Camino básico: seleccionar venta sin documentar, elegir tipo, registrar número y fecha, y confirmar con estado 'Emitido'. | T-002 |
|  |  | Verificar que el combo de ventas liste únicamente las que aún no tienen comprobante asociado. | T-004 |
|  |  | Intentar registrar un número de comprobante ya existente y verificar el rechazo. | T-001 |
|  |  | Verificar que el monto se recupere de la venta y no sea editable, de modo que comprobante y venta no puedan declarar importes distintos. | T-006 |
|  |  | Emitir un comprobante y verificar que la venta quede bloqueada para modificación y eliminación. | T-006 |
|  |  | Ejecutar la operación sobre una base con la tabla de comprobantes vacía y verificar que el primer registro sea posible. | T-001 |
|  |  | Presentar al cliente la emisión de una boleta y una factura. | T-005 |
| ESP-0010 | Consulta de comprobantes tributarios (Leer) | Camino básico: aplicar filtros de fecha, tipo y número y desplegar el resultado. | T-002 |
|  |  | Partición sobre los filtros, incluyendo búsqueda sin resultados. | T-001 |
|  |  | Verificar que los comprobantes anulados permanezcan visibles con su estado, requisito de auditoría tributaria. | T-006 |
| ESP-0011 | Actualización de comprobante tributario (Actualizar) | Camino básico: seleccionar comprobante 'Emitido', corregir tipo, número y fecha, y confirmar. | T-002 |
|  |  | Intentar actualizar un comprobante 'Anulado' y verificar que la operación se cancele. | T-001 |
|  |  | Verificar que la venta asociada esté en solo lectura y que la relación uno a uno se conserve. | T-004 |
| ESP-0012 | Anulación de comprobante tributario (Eliminar) | Camino básico: seleccionar comprobante, confirmar y verificar el cambio a estado 'Anulado'. | T-002 |
|  |  | Verificar que el registro se conserve físicamente y siga siendo consultable para auditoría. | T-006 |
|  |  | Anular un comprobante y verificar que la venta asociada quede nuevamente habilitada para modificación y eliminación. | T-007 |
|  |  | Verificar que no exista ninguna ruta de código que elimine físicamente un comprobante emitido. | T-002 |
| Módulo 3 | Control de acceso del módulo | Con sesión 'Tecnico', verificar que ESP-0009 y ESP-0010 se ejecuten y que ESP-0011 y ESP-0012 sean rechazadas. | T-012 |

## Módulo 4 — Gestión de Alertas de Productos Vencidos (EDU-0004)

| Clave | Requisito (Nombre / Operación) | Descripción de la prueba | Tipo |
| --- | --- | --- | --- |
| ESP-0013 | Consulta de alertas de productos vencidos (Leer) | Camino básico: leer el umbral, comparar los vencimientos de las remesas y consolidar con las ya retiradas. | T-002 |
|  |  | Valores límite sobre el umbral: remesa que vence justo en el límite, un día antes y un día después. | T-001 |
|  |  | Verificar que el resultado identifique cada entrada por medicamento y número de lote. | T-006 |
|  |  | Modificar el umbral en la base y verificar que la consulta refleje el nuevo criterio sin cambios en el código. | T-018 |
| ESP-0014 | Registro manual de alerta de producto vencido (Crear) | Camino básico: seleccionar remesa, marcar lote defectuoso, registrar código DIGEMID y confirmar. | T-002 |
|  |  | Verificar que la fecha de vencimiento se muestre derivada de la remesa y no sea editable. | T-004 |
|  |  | Verificar que la alerta y el bloqueo de la remesa se apliquen de forma atómica: si una falla, ninguna se aplica. | T-012 |
|  |  | Levantar una alerta sobre una remesa y verificar que las demás del mismo medicamento sigan vendiéndose. | T-006 |
|  |  | Verificar que la remesa bloqueada deje de ofrecerse en ventas y quede disponible para una orden de devolución. | T-006 |
|  |  | Verificar que el registro conserve el nombre del medicamento y el número de lote como copia de texto. | T-007 |
| ESP-0015 | Modificación de alerta de producto vencido (Actualizar) | Camino básico: seleccionar alerta, modificar remesa o condiciones y confirmar. | T-002 |
|  |  | Cambiar la remesa asociada y verificar que la fecha de vencimiento mostrada se actualice a la de la nueva. | T-004 |
|  |  | Verificar que el bloqueo de venta se mantenga vigente durante y después de la modificación. | T-007 |
| ESP-0016 | Eliminación de alerta de producto vencido (Eliminar) | Camino básico: seleccionar alerta, confirmar y verificar la eliminación con restitución del estado de la remesa. | T-002 |
|  |  | Complejidad ciclomática: ramas de devolución pendiente, confirmación afirmativa y negativa. | T-003 |
|  |  | Intentar eliminar la alerta de una remesa comprometida en una devolución pendiente y verificar el rechazo. | T-006 |
|  |  | Verificar que tras la eliminación la remesa vuelva a 'Disponible' y se ofrezca de nuevo en ventas. | T-006 |
| Módulo 4 | Control de acceso del módulo | Con sesión 'Tecnico', verificar que ESP-0013 y ESP-0014 se ejecuten y que ESP-0015 y ESP-0016 sean rechazadas. | T-012 |

## Módulo 5 — Gestión de Métodos de Pago (EDU-0009)

| Clave | Requisito (Nombre / Operación) | Descripción de la prueba | Tipo |
| --- | --- | --- | --- |
| ESP-0017 | Registro de método de pago (Crear) | Camino básico: ingresar nombre y estado, validar unicidad y registrar. | T-002 |
|  |  | Partición sobre el nombre: válido, duplicado, vacío y con longitud superior a la permitida. | T-001 |
|  |  | Registrar un método 'Activo' y verificar que aparezca en el combo de ventas; registrarlo 'Inactivo' y verificar que no aparezca. | T-006 |
| ESP-0018 | Consulta de métodos de pago (Leer) | Camino básico: aplicar el filtro de estado y desplegar el resultado. | T-002 |
|  |  | Partición sobre el filtro: 'Activo', 'Inactivo' y sin filtro. | T-001 |
| ESP-0019 | Actualización de método de pago (Actualizar) | Camino básico: seleccionar método, modificar nombre o estado y confirmar. | T-002 |
|  |  | Intentar asignar un nombre que ya pertenece a otro método y verificar el rechazo. | T-001 |
|  |  | Cambiar un método a 'Inactivo' y verificar que deje de ofrecerse pero que las ventas históricas conserven su referencia. | T-007 |
| ESP-0020 | Desactivación de método de pago (Eliminar) | Camino básico: seleccionar método, confirmar y verificar la baja aplicada. | T-002 |
|  |  | Complejidad ciclomática: ramas de método con ventas, sin ventas y confirmación negativa. | T-003 |
|  |  | Dar de baja un método usado en ventas y verificar que se aplique baja lógica con estado 'Inactivo'. | T-006 |
|  |  | Verificar que el histórico de ventas quede intacto y sin violaciones de integridad referencial. | T-007 |
| Módulo 5 | Control de acceso del módulo | Con sesión 'Tecnico', verificar que solo ESP-0018 se ejecute y que las otras tres sean rechazadas. | T-012 |

## Módulo 6 — Gestión de Reportes de Ventas (EDU-0010)

| Clave | Requisito (Nombre / Operación) | Descripción de la prueba | Tipo |
| --- | --- | --- | --- |
| ESP-0021 | Generación de reportes de ventas (Crear) | Camino básico: seleccionar tipo, formato e intervalo, consolidar y registrar el reporte. | T-002 |
|  |  | Valores límite sobre el intervalo: inicio igual al fin, inicio posterior al fin, intervalo sin ventas e intervalo de un año. | T-001 |
|  |  | Verificar que el reporte incluya el usuario responsable de cada venta, resolviendo la unión con la tabla de usuarios. | T-006 |
|  |  | Verificar que la consolidación no modifique ninguna venta ni línea de detalle. | T-007 |
|  |  | Presentar al cliente un reporte de ventas diarias con el detalle del personal responsable. | T-005 |
| ESP-0022 | Consulta de reportes de ventas (Leer) | Camino básico: listar los reportes y abrir el archivo seleccionado en solo lectura. | T-002 |
|  |  | Verificar que el reporte se visualice en el formato con el que fue generado y no se regenere al consultarlo. | T-004 |
|  |  | Consultar un reporte cuyo archivo fue movido o borrado y verificar el manejo del error. | T-001 |
| ESP-0023 | Actualización del intervalo de un reporte (Actualizar) | Camino básico: seleccionar reporte, modificar el intervalo, recalcular y actualizar el registro. | T-002 |
|  |  | Comparar las dos tablas de origen antes y después de la operación y verificar que sean idénticas. | T-007 |
|  |  | Valores límite sobre el nuevo intervalo, incluyendo inicio posterior al fin. | T-001 |
| ESP-0024 | Eliminación de un reporte generado (Eliminar) | Camino básico: seleccionar reporte, confirmar y verificar la eliminación del índice y del archivo. | T-002 |
|  |  | Verificar que las ventas y su detalle permanezcan íntegros tras la eliminación. | T-007 |
|  |  | Eliminar un reporte cuyo archivo ya no existe en disco y verificar que el índice se elimine sin dejar el sistema inconsistente. | T-006 |
| Módulo 6 | Control de acceso del módulo | Con sesión 'Tecnico', verificar que las cuatro operaciones sean rechazadas, conforme a RNF-0006. | T-012 |

## Módulo 7 — Gestión de Devoluciones a Proveedores (EDU-0011)

| Clave | Requisito (Nombre / Operación) | Descripción de la prueba | Tipo |
| --- | --- | --- | --- |
| ESP-0025 | Registro de orden de devolución (Crear) | Camino básico: registrar fecha, proveedor y motivo, agregar remesas con sus cantidades, comentario y confirmar. | T-002 |
|  |  | Verificar que el combo de remesas liste únicamente las del proveedor seleccionado y en condición de cuarentena o por vencer. | T-004 |
|  |  | Partición sobre la cantidad a devolver: menor al stock, igual al stock, mayor al stock, cero y negativa. | T-001 |
|  |  | Registrar una orden con tres remesas y verificar que se creen tres líneas de detalle, cada una con su id_lote. | T-006 |
|  |  | Seleccionar el motivo 'Por Depuracion de Error' con el comentario vacío y verificar que el sistema exija completarlo. | T-001 |
|  |  | Verificar que al registrar la orden el stock de las remesas permanezca sin modificación, por quedar 'Pendiente'. | T-006 |
|  |  | Presentar al cliente la orden generada con el número de lote de cada línea, para confirmar que sirve al trámite con el proveedor. | T-005 |
| ESP-0026 | Consulta de orden de devolución (Leer) | Camino básico: listar las órdenes, seleccionar una y desplegar cabecera y detalle. | T-002 |
|  |  | Verificar que el detalle muestre por cada línea el medicamento, el número de lote y la cantidad. | T-006 |
|  |  | Partición sobre el estado de la orden: 'Pendiente', 'Completada' y 'Eliminada'. | T-001 |
| ESP-0027 | Actualización de orden de devolución (Actualizar) | Camino básico: seleccionar orden 'Pendiente', modificar cabecera y líneas, y confirmar. | T-002 |
|  |  | Complejidad ciclomática: ramas de orden completada, cambio de estado a 'Completada' y actualización sin cambio de estado. | T-003 |
|  |  | Intentar modificar una orden 'Completada' y verificar que la operación se cancele. | T-001 |
|  |  | Marcar una orden como 'Completada' y verificar que el stock de cada remesa se descuente y que las que lleguen a cero pasen a 'Agotado'. | T-006 |
|  |  | Verificar que el descuento de stock y el cambio de estado ocurran dentro de la misma transacción. | T-012 |
| ESP-0028 | Eliminación de orden de devolución (Eliminar) | Camino básico: seleccionar orden 'Pendiente', confirmar y verificar el cambio a estado 'Eliminada'. | T-002 |
|  |  | Intentar eliminar una orden 'Completada' y verificar el rechazo, ya que la mercancía fue despachada. | T-001 |
|  |  | Verificar que las líneas se eliminen sin dejar filas huérfanas y que la cabecera se conserve para auditoría. | T-007 |
|  |  | Verificar que las remesas conserven su stock y su bloqueo, ya que la mercancía nunca salió del almacén. | T-006 |
| Módulo 7 | Control de acceso del módulo | Con sesión 'Tecnico', verificar que las cuatro operaciones sean rechazadas. | T-012 |

## Módulo 8 — Gestión de Alertas de Restricciones de Venta (EDU-0012)

| Clave | Requisito (Nombre / Operación) | Descripción de la prueba | Tipo |
| --- | --- | --- | --- |
| ESP-0029 | Registro de restricción de venta (Crear) | Camino básico: seleccionar medicamento, tipo, condición del cliente y motivo, y confirmar. | T-002 |
|  |  | Partición sobre el tipo de restricción: 'Informativa', 'Bloqueante' y valor fuera de dominio. | T-001 |
|  |  | Registrar una restricción 'Bloqueante' y verificar que la venta de ese medicamento quede impedida. | T-006 |
|  |  | Registrar una 'Informativa' y verificar que la venta muestre la advertencia pero permita continuar. | T-006 |
|  |  | Verificar que la restricción aplique al medicamento completo y no a una remesa concreta. | T-006 |
|  |  | Presentar al cliente la advertencia mostrada al intentar vender un medicamento restringido a una gestante. | T-005 |
| ESP-0030 | Consulta de restricciones de venta (Leer) | Camino básico: seleccionar medicamento, consultar y desplegar las restricciones asociadas. | T-002 |
|  |  | Consultar un medicamento sin restricciones y verificar que el resultado vacío se maneje correctamente. | T-001 |
|  |  | Verificar la legibilidad del motivo de advertencia en pantalla sin necesidad de desplazamiento. | T-016 |
| ESP-0031 | Modificación de restricción de venta (Actualizar) | Camino básico: seleccionar restricción, modificar sus campos y confirmar. | T-002 |
|  |  | Cambiar una restricción de 'Informativa' a 'Bloqueante' y verificar que la venta pase a impedirse en la siguiente operación. | T-006 |
|  |  | Partición sobre el motivo de advertencia: vacío, breve y extenso cercano al límite del campo. | T-001 |
| ESP-0032 | Desactivación de restricción de venta (Eliminar) | Camino básico: seleccionar restricción, confirmar y verificar el cambio a 'Inactivo'. | T-002 |
|  |  | Verificar que la restricción desactivada deje de participar en la validación de la venta. | T-006 |
|  |  | Verificar que el registro se conserve físicamente para auditoría sanitaria. | T-007 |
| Módulo 8 | Control de acceso del módulo | Con sesión 'Tecnico', verificar que solo ESP-0030 se ejecute y que las otras tres sean rechazadas. | T-012 |

## Módulo 9 — Gestión de Usuarios (EDU-0013)

| Clave | Requisito (Nombre / Operación) | Descripción de la prueba | Tipo |
| --- | --- | --- | --- |
| ESP-0033 | Creación de usuario (Crear) | Camino básico: ingresar nombre, contraseña, rol y estado, validar y registrar la cuenta. | T-002 |
|  |  | Partición sobre el nombre de usuario y el rol: duplicado, vacío y rol fuera de dominio. | T-001 |
|  |  | Inspeccionar la tabla tras el registro y verificar que la contraseña no esté almacenada en texto plano. | T-012 |
|  |  | Verificar que el campo de contraseña enmascare el contenido y que la tabla de la interfaz no tenga columna de contraseña. | T-012 |
|  |  | Crear una cuenta 'Activo' y verificar que pueda iniciar sesión; crearla 'Inactivo' y verificar que no pueda. | T-006 |
| ESP-0034 | Consulta de usuarios (Leer) | Camino básico: aplicar filtros por nombre y rol y desplegar el resultado. | T-002 |
|  |  | Inspeccionar la respuesta y verificar que no incluya el campo de contraseña ni en su forma encriptada. | T-012 |
|  |  | Verificar que el resultado muestre el estado de cada cuenta, para distinguir activas de dadas de baja. | T-004 |
| ESP-0035 | Actualización de usuario (Actualizar) | Camino básico: seleccionar cuenta, modificar nombre, rol o estado y confirmar. | T-002 |
|  |  | Complejidad ciclomática: ramas de contraseña vacía, contraseña nueva y bloqueo por último administrador. | T-003 |
|  |  | Dejar el campo de contraseña vacío y verificar que la vigente se conserve intacta tras la actualización. | T-004 |
|  |  | Ingresar una contraseña nueva y verificar que quede encriptada y que la anterior deje de ser válida. | T-012 |
|  |  | Degradar a 'Tecnico' la única cuenta administradora activa y verificar que el sistema cancele la operación. | T-012 |
|  |  | Desactivar la única cuenta administradora activa y verificar que el sistema cancele la operación. | T-012 |
| ESP-0036 | Desactivación de usuario (Eliminar) | Camino básico: seleccionar cuenta ajena, confirmar y verificar la baja aplicada. | T-002 |
|  |  | Complejidad ciclomática: ramas de cuenta propia, con operaciones, sin operaciones y confirmación negativa. | T-003 |
|  |  | Intentar dar de baja la cuenta con la que está abierta la sesión y verificar que el sistema lo impida. | T-012 |
|  |  | Dar de baja una cuenta que registró ventas y verificar que se aplique baja lógica, conservando la atribución. | T-006 |
|  |  | Intentar iniciar sesión con una cuenta recién desactivada y credenciales correctas, y verificar la denegación. | T-012 |
|  |  | Verificar que en ningún caso se produzca una violación de integridad referencial. | T-007 |
| Módulo 9 | Control de acceso del módulo | Con sesión 'Tecnico', verificar que las cuatro operaciones sean rechazadas. | T-012 |

## Módulo 10 — Gestión de Proveedores (EDU-0014)

| Clave | Requisito (Nombre / Operación) | Descripción de la prueba | Tipo |
| --- | --- | --- | --- |
| ESP-0037 | Registro de proveedor (Crear) | Camino básico: ingresar RUC, razón social, teléfono y estado, validar y registrar. | T-002 |
|  |  | Valores límite sobre el RUC: diez dígitos, once, doce, con letras, con espacios y duplicado. | T-001 |
|  |  | Verificar que un RUC con ceros a la izquierda se conserve íntegro, dado que el campo es de texto. | T-004 |
|  |  | Registrar un proveedor 'Activo' y verificar que aparezca en los combos de inventario y devoluciones. | T-006 |
|  |  | Registrar un proveedor sin teléfono y verificar que la operación se complete, por ser campo opcional. | T-001 |
| ESP-0038 | Consulta de proveedores (Leer) | Camino básico: buscar por RUC o razón social y desplegar el resultado. | T-002 |
|  |  | Partición sobre los criterios: RUC exacto, razón social parcial, ambos vacíos y sin resultados. | T-001 |
| ESP-0039 | Actualización de proveedor (Actualizar) | Camino básico: seleccionar proveedor, modificar sus datos y confirmar. | T-002 |
|  |  | Intentar asignar un RUC que ya pertenece a otro proveedor y verificar el rechazo. | T-001 |
|  |  | Cambiar a 'Inactivo' y verificar que deje de ofrecerse pero que productos y órdenes ya asociados conserven su referencia. | T-007 |
| ESP-0040 | Desactivación de proveedor (Eliminar) | Camino básico: seleccionar proveedor, confirmar y verificar la baja aplicada. | T-002 |
|  |  | Complejidad ciclomática: ramas de devolución pendiente, con registros, sin registros y confirmación negativa. | T-003 |
|  |  | Intentar dar de baja un proveedor con una orden 'Pendiente' y verificar el rechazo, para que ninguna orden quede sin destinatario. | T-006 |
|  |  | Dar de baja un proveedor con productos asociados y verificar que se aplique baja lógica. | T-006 |
|  |  | Verificar que no se produzca ninguna violación de integridad referencial. | T-007 |
| Módulo 10 | Control de acceso del módulo | Con sesión 'Tecnico', verificar que las cuatro operaciones sean rechazadas. | T-012 |

## Módulo 11 — Gestión de Lotes (EDU-0015)

| Clave | Requisito (Nombre / Operación) | Descripción de la prueba | Tipo |
| --- | --- | --- | --- |
| ESP-0041 | Registro de remesa (Crear) | Camino básico: seleccionar medicamento, ingresar lote, vencimiento, existencias y fecha de ingreso, y confirmar. | T-002 |
|  |  | Registrar dos remesas del mismo medicamento con el mismo número de lote y verificar el rechazo por unicidad. | T-012 |
|  |  | Registrar el mismo número de lote para dos medicamentos distintos y verificar que sea admitido, ya que la unicidad es por medicamento. | T-001 |
|  |  | Valores límite sobre el vencimiento y las existencias: fecha anterior a hoy, igual a hoy, posterior; existencias cero, positivas y negativas. | T-001 |
|  |  | Registrar tres remesas del mismo medicamento con vencimientos distintos y verificar que coexistan sin duplicar la fila del producto. | T-006 |
|  |  | Verificar que el stock consolidado del medicamento se incremente en las unidades recibidas. | T-006 |
|  |  | Presentar al cliente el registro de dos remesas del mismo medicamento con vencimientos distintos. | T-005 |
| ESP-0042 | Consulta de remesas (Leer) | Camino básico: aplicar filtros por medicamento, lote y estado, y desplegar el resultado. | T-002 |
|  |  | Verificar que el resultado se muestre ordenado por vencimiento ascendente, como contraparte visual del criterio FEFO. | T-004 |
|  |  | Partición sobre el filtro de estado: 'Disponible', 'Bloqueado por devolucion', 'Agotado' y sin filtro. | T-001 |
| ESP-0043 | Actualización de remesa (Actualizar) | Camino básico: seleccionar remesa, modificar lote, vencimiento, existencias o estado, y confirmar. | T-002 |
|  |  | Verificar que el medicamento esté en solo lectura y que no exista ruta alguna para reasignar la remesa a otro. | T-012 |
|  |  | Reducir las existencias por debajo de lo comprometido en una devolución pendiente y verificar que se cancele la operación. | T-006 |
|  |  | Intentar asignar un número de lote que ya existe para el mismo medicamento y verificar el rechazo. | T-001 |
|  |  | Cambiar el estado a 'Agotado' y verificar que la remesa deje de ofrecerse en ventas. | T-006 |
| ESP-0044 | Baja de remesa (Eliminar) | Camino básico: seleccionar remesa, confirmar y verificar la baja aplicada. | T-002 |
|  |  | Complejidad ciclomática: ramas de devolución pendiente, con movimientos, sin movimientos y confirmación negativa. | T-003 |
|  |  | Intentar dar de baja una remesa comprometida en una devolución pendiente y verificar el rechazo. | T-006 |
|  |  | Dar de baja una remesa con ventas y verificar que se aplique baja lógica con estado 'Agotado' y existencias en cero. | T-006 |
|  |  | Tras la baja lógica, consultar las ventas históricas de esa remesa y verificar que el rastreo por lote siga siendo posible. | T-012 |
|  |  | Verificar que el stock consolidado del medicamento se reduzca en las existencias dadas de baja. | T-006 |
| Módulo 11 | Control de acceso del módulo | Con sesión 'Tecnico', verificar que solo ESP-0042 se ejecute y que las otras tres sean rechazadas. | T-012 |

---

## Pruebas de los requisitos no funcionales

| Clave | Requisito | Descripción de la prueba | Tipo |
| --- | --- | --- | --- |
| RNF-0001 | Funcionamiento sin conexión permanente | Desconectar el equipo de la red y ejecutar el registro de una venta, la consulta de inventario y la generación de un reporte. | T-001 |
| RNF-0002 | Facilidad de uso sin capacitación | Sesión con una técnica sin entrenamiento previo, midiendo si completa un registro de venta y una consulta de stock sin asistencia. | T-016 |
| RNF-0003 | Soporte de usuarios simultáneos | Dos sesiones registrando ventas de la misma remesa en paralelo, verificando que no se produzcan lecturas sucias ni stock negativo. | T-008 |
| RNF-0004 | Tiempo de recuperación del servicio | Restaurar la base desde una copia de respaldo en un equipo nuevo y medir el tiempo total hasta dejar el sistema operativo. | T-019 |
| RNF-0005 | Velocidad en el registro de ventas | Medir el tiempo desde la confirmación de la venta hasta el fin de la transacción, incluido el descuento de stock, sobre el equipo objetivo. | T-017 |
| RNF-0005 | Velocidad bajo volumen real | Repetir la medición anterior con el volumen de un año de operación cargado, para verificar que el límite de 4 segundos se mantenga. | T-008 |
| RNF-0006 | Condición 1 — Autenticación | Iniciar sesión con credenciales válidas, con contraseña incorrecta, con usuario inexistente y con cuenta 'Inactivo'. | T-012 |
| RNF-0006 | Condición 2 — Identificación en la trazabilidad | Registrar una venta y una devolución desde dos cuentas distintas y verificar que cada operación quede atribuida a su usuario. | T-012 |
| RNF-0006 | Condición 3 — Autorización por rol | Recorrer las 44 operaciones con una sesión 'Tecnico' y verificar que las 35 restringidas sean rechazadas y las 9 permitidas se ejecuten, conforme a la Matriz de Permisos. | T-012 |
| RNF-0006 | Condición 3 — Verificación en el sistema | Invocar directamente una operación restringida sin pasar por la interfaz y verificar que el sistema la rechace igualmente. | T-012 |
| RNF-0006 | Condición 4 — Operación no autorizada | Con sesión 'Tecnico', verificar que las opciones restringidas se muestren visibles pero deshabilitadas, indicando el motivo al intentar activarlas. | T-016 |
| RNF-0006 | Condición 5 — Protección de credenciales | Inspeccionar la tabla, los resultados de consulta, los reportes y los archivos de registro, y verificar que la contraseña no aparezca en texto plano en ninguno. | T-012 |
| RNF-0006 | Condición 6 — Continuidad de la administración | Intentar dejar el sistema sin cuenta administradora activa por las dos vías posibles: degradación y desactivación. | T-012 |
| RNF-0007 | Funcionamiento en el equipo objetivo | Instalar y ejecutar el sistema en Windows 11 con la especificación declarada, sin instalar software adicional. | T-020 |
| RNF-0007 | Visualización en el equipo objetivo | Verificar la visualización de los mockups de los once módulos en la resolución del equipo objetivo. | T-015 |
| RNF-0008 | Stack tecnológico | Verificar que los importes se manejen con el tipo decimal y no con float o double, comprobando que no aparezcan errores de redondeo en el monto total. | T-004 |
| RNF-0009 | Integridad del esquema | Verificar que las catorce tablas existan con sus claves primarias, foráneas y restricciones de unicidad según el Diccionario de Datos v04.00. | T-018 |
| RNF-0009 | Atomicidad de las operaciones | Interrumpir cada una de las operaciones transaccionales del catálogo a mitad de ejecución y verificar que ninguna deje datos parciales. | T-012 |
| RNF-0009 | Respaldo y restauración | Ejecutar una copia de respaldo, modificar datos, restaurar y verificar que el estado coincida con el del momento de la copia. | T-019 |
| RNF-0009 | Crecimiento del volumen de datos | Cargar el volumen estimado de cinco años de operación y verificar que los tiempos de consulta y de registro se mantengan dentro de los límites, considerando el tamaño máximo de SQL Server Express. | T-010 |

---

## Cobertura

| Verificación | Resultado |
| --- | --- |
| Pruebas definidas | 223 |
| Especificaciones con al menos una prueba | 44 de 44 |
| Especificaciones con prueba de camino básico (T-002) | 44 de 44 |
| Módulos con verificación de rol | 11 de 11 |
| Especificaciones con prueba de complejidad ciclomática (T-003) | 10, las que tienen tres o más ramas |
| Condiciones de RNF-0006 con prueba propia | 6 de 6 |
| Requisitos no funcionales con al menos una prueba | 9 de 9 |
| Tipos de prueba utilizados | 16 de 20, con el motivo documentado de los cuatro restantes |

## Orden de ejecución por riesgo

Si el tiempo obliga a escalonar la ejecución, este es el orden que concentra antes los defectos más costosos.

| Prioridad | Módulos | Motivo |
| --- | --- | --- |
| 1 | 1 Ventas, 11 Lotes | Concentran el descuento de stock, el criterio FEFO y el vínculo entre venta y remesa. Un defecto aquí corrompe el inventario y arrastra a los módulos 4 y 7. |
| 2 | 4 Alertas de vencimiento, 7 Devoluciones | Operan sobre el estado de las remesas y sobre el stock. Dependen de que el módulo 11 sea correcto. |
| 3 | 3 Documentación tributaria, 9 Usuarios | Un defecto tiene consecuencia legal en el primer caso y de seguridad en el segundo, pero no propaga corrupción al resto del modelo. |
| 4 | 2 Inventario, 8 Restricciones, 10 Proveedores | Catálogos maestros. Sus defectos se detectan pronto durante el uso normal. |
| 5 | 5 Métodos de pago, 6 Reportes | Menor acoplamiento y menor consecuencia. Los reportes son de solo lectura sobre datos ya validados. |

Las pruebas de los requisitos no funcionales se ejecutan al final, cuando el sistema está desplegado en el equipo objetivo, salvo las seis condiciones de RNF-0006, que deben verificarse antes de dar por aprobado cualquier módulo porque condicionan las precondiciones de las 44 operaciones.

## Pendientes

1. **Datos de prueba.** Este documento define qué se prueba, no con qué juego de datos. Falta preparar la carga inicial: medicamentos, remesas con vencimientos escalonados, proveedores, cuentas de ambos roles y ventas históricas.
2. **Pruebas de la generación automática de alertas.** No se incluyen porque el requisito no funcional que describe ese proceso programado aún no ha sido redactado.
3. **Criterio de aceptación de RNF-0002.** Se prueba por observación; conviene fijar con el cliente qué se considera aprobado, por ejemplo el tiempo máximo que una técnica sin entrenamiento debe tardar en completar una venta.
