# Educciones de Requisitos — FARMASIL

> **Versión corregida y consolidada (v04.00 — 05/09/2026).** Cambios respecto a la versión anterior:
> - Se completó el campo **Código ilación** en las 8 educciones. En la versión anterior las 8 decían `Pendiente`, lo que rompía la cadena de trazabilidad EDU→ILA→ESP exigida por el catálogo de requisitos.
> - Se documentó la **reserva de los códigos EDU-0005 a EDU-0008**, que corresponden a requisitos reclasificados como no funcionales. El salto de numeración es intencional y no debe renumerarse.
> - **EDU-0004, EDU-0011 y EDU-0012** tenían `Fuente = Ninguno`. Se corrigió: las tres se originan en el Registro de Entrevista 1, y se documentó en el campo Comentario la pregunta concreta que les dio origen, siguiendo el criterio del capítulo 4 del catálogo de requisitos.
> - Se unificó el formato de fecha a `dd/mm/aaaa` y se retiró el marcado inconsistente heredado de la exportación del documento original.
> - Se corrigieron tildes y mayúsculas en los nombres de EDU-0003 y EDU-0012.
> - Se hizo explícito en EDU-0004 el umbral de 2 meses, que ya estaba declarado en EDU-0011 pero en EDU-0004 aparecía solo como "configuración del sistema". Ambas educciones ahora expresan el mismo criterio.
>
> **Ampliación del 05/09/2026.** Tras una consulta posterior a la dueña de la farmacia se incorporaron dos educciones nuevas y se amplió una existente:
> - **EDU-0013 (Gestión de usuarios)**, nueva. La dueña confirmó que requiere verificación de identidad para saber qué personal realiza cada venta. Es además el módulo que hace implementable a RNF-0006 (control de acceso y gestión de roles).
> - **EDU-0014 (Gestión de proveedores)**, nueva. Formaliza el registro de las droguerías y distribuidoras con RUC, razón social y teléfono, dato que hasta ahora se usaba en el modelo (`TBL_PROVEEDORES`, referenciada desde productos y devoluciones) sin ninguna educción que lo respaldara.
> - **EDU-0010 (Reportes)** se amplió para autorizar la administración de los reportes ya generados, resolviendo la contradicción con ILA-0023 e ILA-0024.
>
> **Ampliación del 05/09/2026 (lotes).** Tras la decisión del equipo de incorporar `TBL_LOTES` al modelo, se agregó **EDU-0015 (Gestión de lotes)** y se ajustaron EDU-0002, EDU-0004 y EDU-0011, que hasta ahora describían el lote como un atributo del medicamento.
>
> El inicio y cierre de sesión **no** se incorporan como educción: son un requisito de seguridad ya cubierto por RNF-0006, y una sesión no es una entidad sobre la que aplique el ciclo CRUD que estructura este catálogo.

## Convenciones aplicadas

- Código de educción: `EDU-DDDD`, correlativo, con los saltos de numeración documentados.
- Versión: `DD.DD`, incrementada en cada refinamiento del requisito.
- Fecha: `dd/mm/aaaa`, correspondiente a la versión vigente de la tabla.
- Ningún campo queda vacío. Cuando no hay contenido se consigna `Ninguno`.
- Componentes de mockup con guion (`VEN-BTN-CREAR-VENTA`); tablas y campos de base de datos con guion bajo (`DB_FARMASIL.TBL_PRODUCTOS`).

## Nota sobre códigos reservados

Los códigos **EDU-0005, EDU-0006, EDU-0007 y EDU-0008** no se encuentran en uso. Los requisitos que ocupaban ese rango fueron analizados durante la trazabilidad y resultaron ser **requisitos no funcionales**, por lo que se retiraron de la etapa de educción y se trasladaron al catálogo de requisitos no funcionales (`RequisitosNoFuncionales.md`).

Los códigos se mantienen reservados y **no se reasignan**, de modo que el salto de numeración deje constancia de la depuración realizada. Las educciones vigentes son once: EDU-0001 a EDU-0004 y EDU-0009 a EDU-0015.

## Mapa de módulos

| Módulo | Educción | Nombre | Ilaciones | Especificaciones |
| --- | --- | --- | --- | --- |
| 1 | EDU-0001 | Gestión de ventas | ILA-0001 a ILA-0004 | ESP-0001 a ESP-0004 |
| 2 | EDU-0002 | Gestión de inventario | ILA-0005 a ILA-0008 | ESP-0005 a ESP-0008 |
| 3 | EDU-0003 | Gestión de documentación tributaria | ILA-0009 a ILA-0012 | ESP-0009 a ESP-0012 |
| 4 | EDU-0004 | Gestión de alertas de productos vencidos | ILA-0013 a ILA-0016 | ESP-0013 a ESP-0016 |
| 5 | EDU-0009 | Gestión de métodos de pago | ILA-0017 a ILA-0020 | ESP-0017 a ESP-0020 |
| 6 | EDU-0010 | Gestión de reportes de ventas | ILA-0021 a ILA-0024 | ESP-0021 a ESP-0024 |
| 7 | EDU-0011 | Gestión de devoluciones a proveedores | ILA-0025 a ILA-0028 | ESP-0025 a ESP-0028 |
| 8 | EDU-0012 | Gestión de alertas de restricciones de venta | ILA-0029 a ILA-0032 | ESP-0029 a ESP-0032 |
| 9 | EDU-0013 | Gestión de usuarios | ILA-0033 a ILA-0036 | ESP-0033 a ESP-0036 |
| 10 | EDU-0014 | Gestión de proveedores | ILA-0037 a ILA-0040 | ESP-0037 a ESP-0040 |
| 11 | EDU-0015 | Gestión de lotes | ILA-0041 a ILA-0044 | ESP-0041 a ESP-0044 |

---

## Módulo 1 — Gestión de ventas

| Código educción | EDU-0001 |
| --- | --- |
| Nombre | Gestión de ventas |
| Versión | 04.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0002 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código ilación | ILA-0001, ILA-0002, ILA-0003, ILA-0004 |
| Descripción | La técnica farmacéutica manifiesta la necesidad de registrar digitalmente las ventas realizadas en la farmacia, almacenando información como:<br>• Fecha<br>• Productos vendidos<br>• Cantidades<br>• Método de pago<br>El módulo debe permitir crear, leer, actualizar y borrar registros de ventas. |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Actualmente las ventas se registran de manera manual mediante boletas físicas y apoyo parcial del sistema POS. |

---

## Módulo 2 — Gestión de inventario

| Código educción | EDU-0002 |
| --- | --- |
| Nombre | Gestión de inventario |
| Versión | 05.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0001 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0005, ILA-0006, ILA-0007, ILA-0008 |
| Descripción | El módulo debe permitir al usuario la creación, lectura, actualización y eliminación de los medicamentos del catálogo de la farmacia, cuyos datos son:<br>• Código<br>• Nombre del producto<br>• Acción terapéutica<br>• Precio de venta<br>• Proveedor que lo abastece<br>El número de lote, la fecha de vencimiento y las existencias no son atributos del medicamento sino de cada remesa recibida, y se gestionan en EDU-0015. |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La técnica farmacéutica manifiesta la necesidad de mantener información actualizada del stock y productos farmacéuticos en tiempo real para evitar pérdidas y desabastecimiento.<br>El actor pasó de ACT-0002 a ACT-0001: en la Sección 1 del Registro de Entrevista 1, a la pregunta "¿Quién autoriza el ingreso de nuevos productos al inventario?", se responde que la dueña autoriza personalmente. RNF-0006 reserva además la modificación de precios y la eliminación de productos al rol Administrador. La consulta de inventario permanece disponible para ambos roles, según la Matriz de Actores, Roles y Permisos.<br>En la versión 05.00 el lote, el vencimiento y el stock se trasladaron a EDU-0015: un mismo medicamento convive en el estante en remesas con vencimientos distintos, y tratarlos como atributos del producto obligaba a duplicar el medicamento en el catálogo. |

---

## Módulo 3 — Gestión de documentación tributaria

| Código educción | EDU-0003 |
| --- | --- |
| Nombre | Gestión de documentación tributaria |
| Versión | 04.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0006 |
| Actor | ACT-0002 |
| Fuente | FUE-0002, FUE-0003 |
| Experto | Ninguno |
| Código ilación | ILA-0009, ILA-0010, ILA-0011, ILA-0012 |
| Descripción | La dueña de la farmacia manifiesta la necesidad de registrar y conservar comprobantes relacionados a las ventas para facilitar el control tributario y el cumplimiento ante la SUNAT.<br>Por lo que el módulo deberá permitir la creación, lectura, actualización y eliminación de comprobantes de pago. |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La información tributaria es importante para agilizar el proceso de declaración y pago de impuestos. |

---

## Módulo 4 — Gestión de alertas de productos vencidos

| Código educción | EDU-0004 |
| --- | --- |
| Nombre | Gestión de alertas de productos vencidos |
| Versión | 05.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0002 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0013, ILA-0014, ILA-0015, ILA-0016 |
| Descripción | El sistema debe emitir alertas y bloquear para la venta aquellas remesas de medicamentos, registradas según EDU-0015, que cumplan con los siguientes criterios de riesgo:<br>• Lote defectuoso registrado<br>• Fecha de vencimiento dentro del margen de anticipación configurado en el sistema, cuyo valor por defecto es de 2 meses<br>• Lote con alerta sanitaria activa emitida por la DIGEMID<br>El módulo debe crear, actualizar y eliminar alertas, tanto en un panel de alertas como en el módulo de ventas. |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La farmacia requiere anticipación suficiente para gestionar devoluciones a proveedores.<br>El presente requisito tuvo origen en las siguientes preguntas del Registro de Entrevista 1:<br>• Sección 3, pregunta "¿Desea alertas de productos próximos a vencer?", respondida con un margen de 2 meses de anticipación.<br>• Sección 6, pregunta sobre el rastreo de lotes específicos ante una alerta sanitaria o retiro de producto, donde se indica que la química farmacéutica recibe las alertas de la DIGEMID.<br>Pendiente asignar el código FUE definitivo al Registro de Entrevista 1 en el catálogo de fuentes.<br>En la versión 05.00 la alerta pasó a levantarse sobre la remesa y no sobre el medicamento: bloquear el producto completo porque una de sus remesas vence impedía vender las remesas sanas del mismo estante. |

---

## Módulo 5 — Gestión de métodos de pago

| Código educción | EDU-0009 |
| --- | --- |
| Nombre | Gestión de métodos de pago |
| Versión | 04.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0002 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0017, ILA-0018, ILA-0019, ILA-0020 |
| Descripción | La dueña de la farmacia manifiesta la necesidad de que el sistema permita gestionar los métodos de pago utilizados en las ventas. El módulo deberá permitir registrar, visualizar, modificar y desactivar métodos de pago como efectivo, tarjeta, Yape y Plin, para que posteriormente puedan ser seleccionados durante el proceso de venta. |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | El negocio utiliza pagos en efectivo y aplicaciones digitales como Yape, Plin y tarjeta, integrados a través del POS. |

---

## Módulo 6 — Gestión de reportes de ventas

| Código educción | EDU-0010 |
| --- | --- |
| Nombre | Gestión de reportes de ventas |
| Versión | 05.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0021, ILA-0022, ILA-0023, ILA-0024 |
| Descripción | La dueña manifiesta la necesidad de obtener reportes periódicos relacionados con ventas y movimientos de productos. Dichos reportes deberán incluir:<br>• El detalle del dinero recaudado<br>• La cantidad de productos vendidos<br>• Las fechas de las transacciones<br>• La identificación del personal que realizó la venta<br>El módulo permitirá generar los reportes, mostrarlos en un panel dedicado y administrar los reportes ya generados, pudiendo recalcular su intervalo o eliminarlos. Las ventas que dan origen a un reporte nunca se ven afectadas por estas operaciones. |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Actualmente no existe un control consolidado de ventas.<br>La versión anterior declaraba que el módulo no permitiría modificación ni eliminación, lo que dejaba incompletas dos de las cuatro fases del CRUD y contradecía a ILA-0023 e ILA-0024. Se amplió el alcance distinguiendo el reporte generado, que sí es administrable, de las ventas de origen, que son inmutables.<br>La identificación del personal que realizó la venta depende de EDU-0013 y del campo `id_usuario` en `TBL_REGISTRO_VENTAS`. |

---

## Módulo 7 — Gestión de devoluciones a proveedores

| Código educción | EDU-0011 |
| --- | --- |
| Nombre | Gestión de devoluciones a proveedores |
| Versión | 05.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0025, ILA-0026, ILA-0027, ILA-0028 |
| Descripción | El sistema debe incluir un módulo de gestión de devoluciones que consolide automáticamente en un listado de "Cuarentena" o "Por vencer" todas las remesas de medicamentos cuyo margen de vida útil sea igual o menor a 2 meses, permitiendo generar, listar, modificar y eliminar órdenes de devolución. Cada línea de una orden identifica la remesa concreta que se devuelve, ya que el proveedor tramita la devolución contra el número de lote. |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La dueña manifestó que los proveedores no aceptan la devolución de los productos si el defecto no se reporta con al menos 2 meses de anticipación al vencimiento.<br>El presente requisito tuvo origen en las siguientes preguntas del Registro de Entrevista 1:<br>• Sección 1, preguntas "¿Manejan devoluciones? ¿En qué condiciones?" y "¿Cómo se maneja la devolución de productos vencidos o en mal estado?".<br>• Sección 6, pregunta sobre la anticipación necesaria para las alertas de devolución a proveedores.<br>Pendiente asignar el código FUE definitivo al Registro de Entrevista 1 en el catálogo de fuentes. |

---

## Módulo 8 — Gestión de alertas de restricciones de venta

| Código educción | EDU-0012 |
| --- | --- |
| Nombre | Gestión de alertas de restricciones de venta |
| Versión | 05.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0029, ILA-0030, ILA-0031, ILA-0032 |
| Descripción | Se necesita que el sistema emita alertas en pantalla cuando se detecte la venta de productos con restricciones legales o sanitarias para poblaciones vulnerables (VIH, cáncer, mujeres embarazadas y neonatos). Por lo que el módulo deberá generarlas, almacenarlas, mostrarlas y permitir eliminarlas, además de contar con un panel dedicado a su gestión. |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La dueña comenta que con frecuencia llegan pacientes solicitando medicamentos cuya venta está restringida para el tipo de cliente que intenta adquirirlos.<br>El presente requisito tuvo origen en la Sección 1 del Registro de Entrevista 1, pregunta "¿Existen restricciones de venta por edad o tipo de cliente?", respondida indicando limitaciones para personas con VIH, cáncer, embarazadas y neonatos.<br>Pendiente asignar el código FUE definitivo al Registro de Entrevista 1 en el catálogo de fuentes.<br>El actor pasó de ACT-0002 a ACT-0001: definir qué medicamento se bloquea para qué grupo de riesgo es una decisión de farmacovigilancia que corresponde a la dueña, quien además mantiene el contacto con la química farmacéutica y con DIGEMID. La consulta de las restricciones permanece disponible para ambos roles, ya que la técnica necesita verificarlas durante la atención. |

---

## Módulo 9 — Gestión de usuarios

| Código educción | EDU-0013 |
| --- | --- |
| Nombre | Gestión de usuarios |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Consulta posterior al cliente |
| Experto | Ninguno |
| Código ilación | ILA-0033, ILA-0034, ILA-0035, ILA-0036 |
| Descripción | La dueña de la farmacia manifiesta la necesidad de verificar la identidad del personal que opera el sistema, de modo que toda venta y toda orden de devolución quede asociada a la persona que la realizó. El módulo debe permitir crear, consultar, actualizar y dar de baja las cuentas del personal, gestionando:<br>• Nombre de usuario<br>• Contraseña<br>• Rol dentro del sistema (administrador o técnico)<br>• Estado de la cuenta (activo o inactivo)<br>La administración de las cuentas es responsabilidad exclusiva de la dueña. |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La dueña indicó en una consulta posterior que sí desea una comprobación de identidad para saber quién realiza cada venta, ampliando lo declarado inicialmente.<br>Este módulo es el que hace implementable a RNF-0006 (control de acceso y gestión de roles): RNF-0006 define el atributo de calidad de seguridad y el ingreso mediante usuario y contraseña, mientras esta educción define la gestión funcional de las cuentas sobre las que ese control opera. El inicio y el cierre de sesión permanecen documentados en RNF-0006 y no se descomponen en ilaciones, por no constituir un ciclo CRUD sobre una entidad del negocio.<br>Pendiente registrar formalmente la consulta al cliente como fuente y asignarle su código FUE. |

---

## Módulo 10 — Gestión de proveedores

| Código educción | EDU-0014 |
| --- | --- |
| Nombre | Gestión de proveedores |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0037, ILA-0038, ILA-0039, ILA-0040 |
| Descripción | La dueña manifiesta la necesidad de mantener el registro de las droguerías y distribuidoras farmacéuticas que abastecen a la farmacia, para poder identificarlas al momento de tramitar una devolución. El módulo debe permitir crear, consultar, actualizar y dar de baja proveedores, gestionando:<br>• RUC<br>• Razón social<br>• Teléfono de contacto<br>• Estado del proveedor (activo o inactivo)<br>La administración de los proveedores es responsabilidad exclusiva de la dueña. |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La tabla `TBL_PROVEEDORES` ya existía en el Diccionario de Datos y es referenciada desde `TBL_PRODUCTOS` y `TBL_ORDENES_DEVOLUCION`, pero ninguna educción respaldaba su existencia ni definía cómo se alimenta. Esta educción cierra ese vacío.<br>El presente requisito tuvo origen en la Sección 1 del Registro de Entrevista 1, preguntas "¿Cómo seleccionan a los proveedores?" y "¿Manejan devoluciones? ¿En qué condiciones?", donde se describe que las droguerías y distribuidoras exhiben catálogos y que la devolución se coordina directamente con ellas.<br>Pendiente asignar el código FUE definitivo al Registro de Entrevista 1 en el catálogo de fuentes. |

---

## Módulo 11 — Gestión de lotes

| Código educción | EDU-0015 |
| --- | --- |
| Nombre | Gestión de lotes |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0041, ILA-0042, ILA-0043, ILA-0044 |
| Descripción | La farmacia recibe un mismo medicamento en remesas sucesivas, con números de lote y fechas de vencimiento distintos, que conviven en el estante. El módulo debe permitir crear, consultar, actualizar y dar de baja las remesas de cada medicamento, gestionando:<br>• Medicamento del catálogo al que pertenece<br>• Número de lote<br>• Fecha de vencimiento<br>• Existencias de la remesa<br>• Fecha de ingreso al almacén<br>• Estado de la remesa<br>Las ventas descuentan de una remesa concreta, las alertas de vencimiento se levantan sobre una remesa, y las órdenes de devolución identifican la remesa que se retira. |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | El presente requisito tuvo origen en la Sección 6 del Registro de Entrevista 1, en la pregunta sobre si el sistema debería permitir rastrear lotes específicos ante una alerta sanitaria o retiro de producto, respondida afirmativamente e indicando que la química farmacéutica recibe esas alertas de la DIGEMID. Sin una entidad de remesa, ese rastreo es imposible: el sistema no podría responder qué se vendió de un lote retirado.<br>Se constituye como educción propia y no como ampliación de EDU-0002 porque la remesa es una entidad distinta del medicamento, con su propio ciclo CRUD. Incorporarla al módulo de inventario habría dejado a esa educción con ocho operaciones, rompiendo la estructura de cuatro fases por educción que sigue todo el catálogo.<br>Pendiente asignar el código FUE definitivo al Registro de Entrevista 1 en el catálogo de fuentes. |
