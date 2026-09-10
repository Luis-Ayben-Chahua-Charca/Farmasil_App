# Especificaciones de Requisitos — FARMASIL


# Módulo 1 — Gestión de Ventas (EDU-0001)

| Código especificación | ESP-0001 |
| --- | --- |
| Nombre | Creación del registro de venta |
| Versión | 04.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código ilación | ILA-0001 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_LOTES, estado_lote = 'Disponible' Y stock_actual > 0)**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_METODOS_PAGO, estado = 'Activo')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-VEN-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**fechaHora = FECHA_HORA_SISTEMA**<br>&nbsp;&nbsp;**MOSTRAR fechaHora EN VEN-LBL-FECHA-VENTA**<br>&nbsp;&nbsp;**detalle = LISTA_VACIA**<br>&nbsp;&nbsp;**REPETIR**<br>&nbsp;&nbsp;&nbsp;&nbsp;**producto = VEN-CMB-PRODUCTO-VENTA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**lote = SUGERIR_FEFO(DB_FARMASIL.TBL_LOTES, id_producto = producto Y estado_lote = 'Disponible' Y stock_actual > 0)**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR lote EN VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**lote = VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**cantidad = VEN-NUM-CANTIDAD-VENTA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**PRESIONAR VEN-BTN-AGREGAR-PRODUCTO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR cantidad = ENTERO > 0**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR cantidad <= lote.stock_actual**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI lote.estado_lote <> 'Disponible' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La remesa tiene una alerta activa y no puede venderse"**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**RECHAZAR LINEA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**restriccion = CONSULTAR DB_FARMASIL.TBL_RESTRICCIONES_VENTA DONDE id_producto = producto Y estado_alerta = 'Activo'**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI restriccion.tipo_restriccion = 'Bloqueante' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR restriccion.motivo_advertencia**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**RECHAZAR LINEA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SINO SI restriccion.tipo_restriccion = 'Informativa' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR restriccion.motivo_advertencia**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**precioUnitario = producto.precio_venta**<br>&nbsp;&nbsp;&nbsp;&nbsp;**AGREGAR (lote, cantidad, precioUnitario) A detalle**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR VEN-TBL-DETALLE-VENTA**<br>&nbsp;&nbsp;**HASTA QUE USUARIO NO AGREGUE MAS PRODUCTOS**<br>&nbsp;&nbsp;**SI USUARIO PRESIONA VEN-BTN-QUITAR-PRODUCTO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**QUITAR LINEA_SELECCIONADA DE detalle**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR VEN-TBL-DETALLE-VENTA**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**montoTotal = SUMA(detalle.cantidad \* detalle.precioUnitario)**<br>&nbsp;&nbsp;**MOSTRAR montoTotal EN VEN-LBL-MONTO-TOTAL**<br>&nbsp;&nbsp;**metodoPago = VEN-CMB-METODO-PAGO**<br>&nbsp;&nbsp;**PRESIONAR VEN-BTN-CREAR-VENTA**<br>&nbsp;&nbsp;**VALIDAR detalle <> VACIA**<br>&nbsp;&nbsp;**VALIDAR metodoPago**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (fechaHora, montoTotal, metodoPago, SESION_USUARIO.id_usuario) EN DB_FARMASIL.TBL_REGISTRO_VENTAS**<br>&nbsp;&nbsp;&nbsp;&nbsp;**idVenta = ULTIMO_ID_GENERADO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA linea EN detalle HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (idVenta, linea.lote, linea.cantidad, linea.precioUnitario) EN DB_FARMASIL.TBL_DETALLE_VENTAS**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES.stock_actual = stock_actual - linea.cantidad DONDE id_lote = linea.lote**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI DB_FARMASIL.TBL_LOTES.stock_actual = 0 ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES.estado_lote = 'Agotado'**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR VEN-TBL-REGISTRO-VENTAS**<br>&nbsp;&nbsp;**MOSTRAR "Venta registrada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REGISTRO_VENTAS.monto_total = SUMA(DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = idVenta)**<br>&nbsp;&nbsp;**VERIFICAR CONTAR(DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = idVenta) = CONTAR(detalle)**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.stock_actual = DESCONTADO**<br>&nbsp;&nbsp;**VERIFICAR CADA LINEA DE DB_FARMASIL.TBL_DETALLE_VENTAS TIENE id_lote <> NULO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REGISTRO_VENTAS.id_usuario = SESION_USUARIO.id_usuario**<br>&nbsp;&nbsp;**VERIFICAR VEN-TBL-REGISTRO-VENTAS = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-VEN-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Reescrita para la venta de varios productos en una misma transacción. La versión anterior registraba un único producto directamente en la cabecera, sin usar **TBL_DETALLE_VENTAS**, y no descontaba stock ni verificaba restricciones sanitarias.<br>Depende de los componentes **VEN-BTN-AGREGAR-PRODUCTO**, **VEN-BTN-QUITAR-PRODUCTO**, **VEN-TBL-DETALLE-VENTA** y **VEN-LBL-MONTO-TOTAL**, aún por incorporar al mockup. |

| Código especificación | ESP-0002 |
| --- | --- |
| Nombre | Consulta del registro de venta |
| Versión | 03.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código ilación | ILA-0002 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REGISTRO_VENTAS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-VEN-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**PRESIONAR VEN-BTN-LEER-VENTA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-VEN-0002**<br>&nbsp;&nbsp;**fecha = VEN-FEC-FECHA-VENTA**<br>&nbsp;&nbsp;**producto = VEN-CMB-PRODUCTO-VENTA**<br>&nbsp;&nbsp;**PRESIONAR VEN-BTN-LEER-VENTA**<br>&nbsp;&nbsp;**VALIDAR fecha**<br>&nbsp;&nbsp;**VALIDAR producto**<br>&nbsp;&nbsp;**SI producto <> VACIO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_REGISTRO_VENTAS UNIENDO DB_FARMASIL.TBL_DETALLE_VENTAS UNIENDO DB_FARMASIL.TBL_LOTES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**APLICAR FILTRO id_producto = producto**<br>&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_REGISTRO_VENTAS**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**APLICAR FILTRO fecha_hora = fecha**<br>&nbsp;&nbsp;**CARGAR VEN-TBL-REGISTRO-VENTAS**<br>&nbsp;&nbsp;**SI USUARIO SELECCIONA venta EN VEN-TBL-REGISTRO-VENTAS ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = venta**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CARGAR VEN-TBL-DETALLE-VENTA**<br>&nbsp;&nbsp;**FIN SI**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR VEN-TBL-REGISTRO-VENTAS = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR VEN-TBL-DETALLE-VENTA = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL = SIN_MODIFICACION**<br>&nbsp;&nbsp;**MOSTRAR RESULTADOS_CONSULTA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-VEN-0001, ART-MKP-VEN-0002 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | En la versión anterior el filtro por producto se aplicaba sobre **TBL_REGISTRO_VENTAS**, tabla que no contiene el producto: la consulta no habría devuelto nada. Ahora se resuelve contra la tabla de detalle.<br>Se agregó la visualización del detalle de la venta seleccionada.<br>Esta especificación corregía además un error estructural de la versión anterior de `Especificaciones.md`, donde la fila de Precondición había sido reemplazada por una segunda fila de Nombre. |

| Código especificación | ESP-0003 |
| --- | --- |
| Nombre | Actualización del registro de venta |
| Versión | 03.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código ilación | ILA-0003 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REGISTRO_VENTAS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, id_venta = venta Y estado_documento = 'Emitido')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-VEN-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR venta EN VEN-TBL-REGISTRO-VENTAS**<br>&nbsp;&nbsp;**PRESIONAR VEN-BTN-ACTUALIZAR-VENTA**<br>&nbsp;&nbsp;**SI EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, id_venta = venta Y estado_documento = 'Emitido') ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La venta tiene comprobante emitido. Anule el comprobante antes de modificarla"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CARGAR ART-MKP-VEN-0003**<br>&nbsp;&nbsp;**detalleAnterior = CONSULTAR DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = venta**<br>&nbsp;&nbsp;**detalleNuevo = detalleAnterior**<br>&nbsp;&nbsp;**CARGAR VEN-TBL-DETALLE-VENTA CON detalleNuevo**<br>&nbsp;&nbsp;**metodoPago = VEN-CMB-METODO-PAGO**<br>&nbsp;&nbsp;**SI USUARIO PRESIONA VEN-BTN-AGREGAR-PRODUCTO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**AGREGAR (VEN-CMB-PRODUCTO-VENTA, VEN-NUM-CANTIDAD-VENTA) A detalleNuevo**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**SI USUARIO PRESIONA VEN-BTN-QUITAR-PRODUCTO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**QUITAR LINEA_SELECCIONADA DE detalleNuevo**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**montoTotal = SUMA(detalleNuevo.cantidad \* detalleNuevo.precioUnitario)**<br>&nbsp;&nbsp;**MOSTRAR montoTotal EN VEN-LBL-MONTO-TOTAL**<br>&nbsp;&nbsp;**PRESIONAR VEN-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR detalleNuevo <> VACIA**<br>&nbsp;&nbsp;**VALIDAR metodoPago**<br>&nbsp;&nbsp;**PARA CADA linea EN detalleNuevo HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;**diferencia = linea.cantidad - CANTIDAD_ANTERIOR(linea.lote, detalleAnterior)**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR diferencia <= linea.lote.stock_actual**<br>&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_REGISTRO_VENTAS SET monto_total = montoTotal, id_metodo_pago = metodoPago**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SINCRONIZAR DB_FARMASIL.TBL_DETALLE_VENTAS CON detalleNuevo**<br>&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA lote AFECTADO HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES.stock_actual = stock_actual - diferencia**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR VEN-TBL-REGISTRO-VENTAS**<br>&nbsp;&nbsp;**MOSTRAR "Venta actualizada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REGISTRO_VENTAS.monto_total = SUMA(DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = venta)**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.stock_actual = AJUSTADO_POR_DIFERENCIA**<br>&nbsp;&nbsp;**VERIFICAR VENTA_CON_COMPROBANTE_EMITIDO = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR VEN-TBL-REGISTRO-VENTAS = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-VEN-0001, ART-MKP-VEN-0003 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Se incorporó el bloqueo de las ventas con comprobante tributario emitido y el ajuste diferencial de stock, que es lo que evita que actualizar una venta descuadre el inventario.<br>La versión anterior modificaba únicamente la cabecera, dejando el detalle intacto y el monto total desincronizado. |

| Código especificación | ESP-0004 |
| --- | --- |
| Nombre | Eliminación del registro de venta |
| Versión | 03.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código ilación | ILA-0004 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REGISTRO_VENTAS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, id_venta = venta Y estado_documento = 'Emitido')**<br>&nbsp;&nbsp;**VALIDAR VEN-TBL-REGISTRO-VENTAS = HABILITADA**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR venta EN VEN-TBL-REGISTRO-VENTAS**<br>&nbsp;&nbsp;**PRESIONAR VEN-BTN-ELIMINAR-VENTA**<br>&nbsp;&nbsp;**SI EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, id_venta = venta Y estado_documento = 'Emitido') ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La venta tiene comprobante emitido. Anule el comprobante antes de eliminarla"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CARGAR ART-MKP-VEN-0004**<br>&nbsp;&nbsp;**MOSTRAR VEN-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR VEN-TXT-MSJ = "¿Está seguro de eliminar este registro de venta? El stock de los productos será restituido."**<br>&nbsp;&nbsp;**SI PRESIONAR VEN-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**detalle = CONSULTAR DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = venta**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA linea EN detalle HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES.stock_actual = stock_actual + linea.cantidad DONDE id_lote = linea.lote**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = venta**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_REGISTRO_VENTAS DONDE id_venta = venta**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Venta eliminada correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR VEN-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR VEN-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR VEN-TBL-REGISTRO-VENTAS**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ELIMINADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR NO EXISTE(DB_FARMASIL.TBL_DETALLE_VENTAS, id_venta = venta)**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.stock_actual = RESTITUIDO**<br>&nbsp;&nbsp;**VERIFICAR VENTA_CON_COMPROBANTE_EMITIDO = SIN_ELIMINAR**<br>&nbsp;&nbsp;**VERIFICAR VEN-TBL-REGISTRO-VENTAS = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-VEN-0001, ART-MKP-VEN-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Se incorporó la eliminación en cascada del detalle, la restitución del stock y el bloqueo cuando existe comprobante emitido.<br>La versión anterior eliminaba solo la cabecera, lo que habría dejado líneas huérfanas en **TBL_DETALLE_VENTAS** y una violación de integridad referencial. También quedaban en el archivo original los textos de plantilla sin completar en los campos Importancia, Estado y Comentario. |

---

# Módulo 2 — Gestión de Inventario (EDU-0002)

| Código especificación | ESP-0005 |
| --- | --- |
| Nombre | Creación del producto de inventario |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0005 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_PROVEEDORES, estado = 'Activo')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-INV-0001**<br>&nbsp;&nbsp;**VALIDAR INV-BTN-CREAR-PRODUCTO = HABILITADO**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**nombre = INV-TXT-NOMBRE-PRODUCTO**<br>&nbsp;&nbsp;**accionTerapeutica = INV-TXT-ACCION-TERAPEUTICA**<br>&nbsp;&nbsp;**precio = INV-NUM-PRECIO-PRODUCTO**<br>&nbsp;&nbsp;**proveedor = INV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**PRESIONAR INV-BTN-CREAR-PRODUCTO**<br>&nbsp;&nbsp;**VALIDAR nombre <> VACIO**<br>&nbsp;&nbsp;**VALIDAR accionTerapeutica <> VACIO**<br>&nbsp;&nbsp;**VALIDAR precio = DECIMAL >= 0**<br>&nbsp;&nbsp;**VALIDAR proveedor**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (nombre, accionTerapeutica, precio, 'Disponible', proveedor) EN DB_FARMASIL.TBL_PRODUCTOS**<br>&nbsp;&nbsp;&nbsp;&nbsp;**idProducto = ULTIMO_ID_GENERADO**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**MOSTRAR idProducto EN INV-LBL-ID-PRODUCTO**<br>&nbsp;&nbsp;**ACTUALIZAR INV-TBL-PRODUCTOS**<br>&nbsp;&nbsp;**MOSTRAR "Producto registrado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PRODUCTOS.estado_producto = 'Disponible'**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PRODUCTOS.id_proveedor <> NULO**<br>&nbsp;&nbsp;**VERIFICAR INV-TBL-PRODUCTOS = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR PRODUCTO SIN REMESAS NO SE OFRECE EN VEN-CMB-PRODUCTO-VENTA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-INV-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | El identificador pasó de captura manual a generación automática, coherente con la clave primaria autoincremental del diccionario, y se muestra en **INV-LBL-ID-PRODUCTO**.<br>Se incorporó `id_proveedor`, campo obligatorio que la versión anterior no capturaba.<br>Con la incorporación de **TBL_LOTES** en el Diccionario de Datos v04.00, el número de lote, la fecha de vencimiento y las existencias se registran como remesas en ESP-0041, y esta especificación quedó reducida a los datos de catálogo del medicamento.<br>Depende del componente **INV-CMB-PROVEEDOR**, aún por incorporar al mockup. |

| Código especificación | ESP-0006 |
| --- | --- |
| Nombre | Consulta del producto de inventario |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0006 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-INV-0002**<br>&nbsp;&nbsp;**VALIDAR INV-BTN-LEER-PRODUCTO = HABILITADO**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**idProducto = INV-TXT-ID-PRODUCTO**<br>&nbsp;&nbsp;**nombre = INV-TXT-NOMBRE-PRODUCTO**<br>&nbsp;&nbsp;**PRESIONAR INV-BTN-LEER-PRODUCTO**<br>&nbsp;&nbsp;**VALIDAR idProducto O nombre <> VACIO**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_PRODUCTOS**<br>&nbsp;&nbsp;**APLICAR FILTRO id_producto = idProducto**<br>&nbsp;&nbsp;**APLICAR FILTRO nombre = nombre**<br>&nbsp;&nbsp;**CARGAR INV-TBL-PRODUCTOS CON (id_producto, nombre, accion_terapeutica, precio_venta, estado_producto, SUMA(TBL_LOTES.stock_actual))**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR INV-TBL-PRODUCTOS = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PRODUCTOS = SIN_MODIFICACION**<br>&nbsp;&nbsp;**MOSTRAR RESULTADOS_CONSULTA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-INV-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se explicitaron las columnas devueltas por la consulta, incorporando stock, lote y estado, que son los datos que el personal necesita durante la atención y que la versión anterior no declaraba. |

| Código especificación | ESP-0007 |
| --- | --- |
| Nombre | Actualización del producto de inventario |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0007 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-INV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR producto EN INV-TBL-PRODUCTOS**<br>&nbsp;&nbsp;**PRESIONAR INV-BTN-ACTUALIZAR-PRODUCTO**<br>&nbsp;&nbsp;**CARGAR ART-MKP-INV-0003**<br>&nbsp;&nbsp;**MOSTRAR producto.id_producto EN INV-LBL-ID-PRODUCTO**<br>&nbsp;&nbsp;**nombre = INV-TXT-NOMBRE-PRODUCTO**<br>&nbsp;&nbsp;**accionTerapeutica = INV-TXT-ACCION-TERAPEUTICA**<br>&nbsp;&nbsp;**precio = INV-NUM-PRECIO-PRODUCTO**<br>&nbsp;&nbsp;**proveedor = INV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**PRESIONAR INV-BTN-ACTUALIZAR-PRODUCTO**<br>&nbsp;&nbsp;**VALIDAR nombre <> VACIO**<br>&nbsp;&nbsp;**VALIDAR precio = DECIMAL >= 0**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_PRODUCTOS DONDE id_producto = producto.id_producto**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR INV-TBL-PRODUCTOS**<br>&nbsp;&nbsp;**MOSTRAR "Producto actualizado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PRODUCTOS.id_producto = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR REFERENCIAS(TBL_LOTES, TBL_RESTRICCIONES_VENTA) = INTACTAS**<br>&nbsp;&nbsp;**VERIFICAR INV-TBL-PRODUCTOS = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-INV-0001, ART-MKP-INV-0003 |
| Importancia | Media |
| Estado | Pendiente |
| Comentario | Se declaró de solo lectura el identificador, que es clave foránea en tres tablas del modelo, y se alinearon los campos editables con los incorporados en ESP-0005. |

| Código especificación | ESP-0008 |
| --- | --- |
| Nombre | Eliminación del producto de inventario |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0008 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-INV-0004**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR producto EN INV-TBL-PRODUCTOS**<br>&nbsp;&nbsp;**PRESIONAR INV-BTN-ELIMINAR-PRODUCTO**<br>&nbsp;&nbsp;**MOSTRAR INV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR INV-TXT-MSJ = "¿Está seguro de dar de baja este producto?"**<br>&nbsp;&nbsp;**tieneMovimientos = EXISTE(DB_FARMASIL.TBL_LOTES, id_producto = producto) O EXISTE(DB_FARMASIL.TBL_RESTRICCIONES_VENTA, id_producto = producto)**<br>&nbsp;&nbsp;**SI PRESIONAR INV-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI tieneMovimientos = FALSO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_PRODUCTOS DONDE id_producto = producto**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_PRODUCTOS.estado_producto = 'Descontinuado'**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Producto dado de baja correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR INV-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR INV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR INV-TBL-PRODUCTOS**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR BAJA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR PRODUCTO_CON_MOVIMIENTOS.estado_producto = 'Descontinuado'**<br>&nbsp;&nbsp;**VERIFICAR HISTORICO_VENTAS = INTACTO**<br>&nbsp;&nbsp;**VERIFICAR PRODUCTO NO DISPONIBLE EN VEN-CMB-PRODUCTO-VENTA**<br>&nbsp;&nbsp;**VERIFICAR INTEGRIDAD_REFERENCIAL = SIN_VIOLACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-INV-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La versión anterior eliminaba físicamente el producto sin verificar sus referencias en tres tablas del modelo. Ahora la eliminación física solo procede cuando no hay movimientos, y en caso contrario se aplica baja lógica.<br>Depende de la incorporación del valor 'Descontinuado' al dominio de `estado_producto`, ya aplicada en el Diccionario de Datos v03.00 pero pendiente de reflejarse en el motor. |

---

# Módulo 3 — Gestión de Documentación Tributaria (EDU-0003)

| Código especificación | ESP-0009 |
| --- | --- |
| Nombre | Registro de comprobante tributario |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0002, FUE-0003 |
| Experto | Ninguno |
| Código ilación | ILA-0009 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_REGISTRO_VENTAS SIN COMPROBANTE ASOCIADO)**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DOC-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**PRESIONAR DOC-BTN-CREAR-COMPROBANTE**<br>&nbsp;&nbsp;**venta = DOC-CMB-CODIGO-VENTA**<br>&nbsp;&nbsp;**montoTotal = CONSULTAR DB_FARMASIL.TBL_REGISTRO_VENTAS.monto_total DONDE id_venta = venta**<br>&nbsp;&nbsp;**MOSTRAR montoTotal EN DOC-LBL-MONTO-TOTAL**<br>&nbsp;&nbsp;**tipoComprobante = DOC-CMB-TIPO-COMPROBANTE**<br>&nbsp;&nbsp;**numeroComprobante = DOC-TXT-NUMERO-COMPROBANTE**<br>&nbsp;&nbsp;**fechaEmision = DOC-FEC-FECHA-EMISION**<br>&nbsp;&nbsp;**PRESIONAR DOC-BTN-CONFIRMAR-SI**<br>&nbsp;&nbsp;**VALIDAR tipoComprobante EN ('Boleta', 'Factura')**<br>&nbsp;&nbsp;**VALIDAR numeroComprobante <> VACIO**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, numero_comprobante = numeroComprobante)**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, id_venta = venta Y estado_documento = 'Emitido')**<br>&nbsp;&nbsp;**VALIDAR fechaEmision**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (numeroComprobante, tipoComprobante, fechaEmision, venta, 'Emitido') EN DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR DOC-TBL-COMPROBANTES**<br>&nbsp;&nbsp;**MOSTRAR "Comprobante registrado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS.estado_documento = 'Emitido'**<br>&nbsp;&nbsp;**VERIFICAR RELACION(venta, comprobante) = UNO_A_UNO**<br>&nbsp;&nbsp;**VERIFICAR VENTA_DOCUMENTADA = BLOQUEADA_PARA_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR DOC-TBL-COMPROBANTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DOC-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La precondición anterior exigía que la tabla de comprobantes ya tuviera entradas para poder crear uno, lo que hacía imposible el primer registro del sistema. Se reemplazó por la existencia de una venta pendiente de documentar.<br>El monto dejó de capturarse manualmente: se recupera de la venta y se muestra en solo lectura, eliminando la posibilidad de que comprobante y venta declaren importes distintos.<br>Depende del campo `numero_comprobante`, incorporado en el Diccionario de Datos v03.00. |

| Código especificación | ESP-0010 |
| --- | --- |
| Nombre | Consulta de comprobantes tributarios |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0002, FUE-0003 |
| Experto | Ninguno |
| Código ilación | ILA-0010 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DOC-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**PRESIONAR DOC-BTN-LEER-COMPROBANTE**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DOC-0002**<br>&nbsp;&nbsp;**fechaEmision = DOC-FEC-FECHA-EMISION**<br>&nbsp;&nbsp;**tipoComprobante = DOC-CMB-TIPO-COMPROBANTE**<br>&nbsp;&nbsp;**numeroComprobante = DOC-TXT-NUMERO-COMPROBANTE**<br>&nbsp;&nbsp;**PRESIONAR DOC-BTN-LEER-COMPROBANTE**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS UNIENDO DB_FARMASIL.TBL_REGISTRO_VENTAS**<br>&nbsp;&nbsp;**APLICAR FILTRO fecha_emision = fechaEmision**<br>&nbsp;&nbsp;**APLICAR FILTRO tipo_comprobante = tipoComprobante**<br>&nbsp;&nbsp;**APLICAR FILTRO numero_comprobante = numeroComprobante**<br>&nbsp;&nbsp;**CARGAR DOC-TBL-COMPROBANTES CON (numero_comprobante, tipo_comprobante, fecha_emision, monto_total, estado_documento)**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR COMPROBANTES_ANULADOS = VISIBLES**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR DOC-TBL-COMPROBANTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DOC-0001, ART-MKP-DOC-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | El monto se resuelve contra la tabla de ventas, ya que no es una columna de la tabla de comprobantes.<br>Los comprobantes anulados permanecen visibles con su estado, requisito de auditoría tributaria. |

| Código especificación | ESP-0011 |
| --- | --- |
| Nombre | Actualización de comprobante tributario |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | FUE-0002, FUE-0003 |
| Experto | Ninguno |
| Código ilación | ILA-0011 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR comprobante.estado_documento = 'Emitido'**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DOC-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR comprobante EN DOC-TBL-COMPROBANTES**<br>&nbsp;&nbsp;**PRESIONAR DOC-BTN-ACTUALIZAR-COMPROBANTE**<br>&nbsp;&nbsp;**SI comprobante.estado_documento = 'Anulado' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "El comprobante está anulado y no admite modificación"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DOC-0003**<br>&nbsp;&nbsp;**MOSTRAR comprobante.id_venta EN DOC-CMB-CODIGO-VENTA = SOLO_LECTURA**<br>&nbsp;&nbsp;**tipoComprobante = DOC-CMB-TIPO-COMPROBANTE**<br>&nbsp;&nbsp;**numeroComprobante = DOC-TXT-NUMERO-COMPROBANTE**<br>&nbsp;&nbsp;**fechaEmision = DOC-FEC-FECHA-EMISION**<br>&nbsp;&nbsp;**PRESIONAR DOC-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR tipoComprobante EN ('Boleta', 'Factura')**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, numero_comprobante = numeroComprobante Y id_comprobante <> comprobante)**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS DONDE id_comprobante = comprobante**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR DOC-TBL-COMPROBANTES**<br>&nbsp;&nbsp;**MOSTRAR "Comprobante actualizado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS.id_venta = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR RELACION(venta, comprobante) = UNO_A_UNO**<br>&nbsp;&nbsp;**VERIFICAR DOC-TBL-COMPROBANTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DOC-0001, ART-MKP-DOC-0003 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se enumeraron los campos corregibles, que la versión anterior dejaba como "datos permitidos" sin precisar, y se declaró de solo lectura la venta asociada: reasignar un comprobante a otra venta rompería la relación uno a uno del modelo. |

| Código especificación | ESP-0012 |
| --- | --- |
| Nombre | Anulación de comprobante tributario |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | FUE-0002, FUE-0003 |
| Experto | Ninguno |
| Código ilación | ILA-0012 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR comprobante.estado_documento = 'Emitido'**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DOC-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR comprobante EN DOC-TBL-COMPROBANTES**<br>&nbsp;&nbsp;**PRESIONAR DOC-BTN-ELIMINAR-COMPROBANTE**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DOC-0004**<br>&nbsp;&nbsp;**MOSTRAR DOC-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR DOC-TXT-MSJ = "¿Está seguro de anular este comprobante? El documento se conservará con estado Anulado."**<br>&nbsp;&nbsp;**SI PRESIONAR DOC-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS.estado_documento = 'Anulado' DONDE id_comprobante = comprobante**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Comprobante anulado correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR DOC-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR DOC-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR DOC-TBL-COMPROBANTES**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR ANULACION_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS.estado_documento = 'Anulado'**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CONSERVADO_PARA_AUDITORIA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR VENTA_ASOCIADA = HABILITADA_PARA_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR DOC-TBL-COMPROBANTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DOC-0001, ART-MKP-DOC-0004 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La fase Eliminar del CRUD se implementa como baja lógica. Un documento tributario emitido no puede borrarse del registro, y el diccionario define el valor 'Anulado' precisamente para este caso.<br>La postcondición anterior afirmaba que se eliminaba "el registro de venta", confundiendo el comprobante con la venta. |

---

# Módulo 4 — Gestión de Alertas de Productos Vencidos (EDU-0004)

| Código especificación | ESP-0013 |
| --- | --- |
| Nombre | Consulta de alertas de productos vencidos |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0007 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0013 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_ALERTAS_VENCIMIENTO <> VACIA**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS = DISPONIBLE**<br>&nbsp;&nbsp;**CARGAR ART-MKP-ALV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**PRESIONAR ALV-BTN-LEER-ALERTA**<br>&nbsp;&nbsp;**umbral = CONSULTAR DB_FARMASIL.TBL_ALERTAS_VENCIMIENTO.umbral_meses DONDE estado = 'Activo'**<br>&nbsp;&nbsp;**proximosAVencer = CONSULTAR DB_FARMASIL.TBL_LOTES DONDE fecha_vencimiento <= FECHA_SISTEMA + umbral**<br>&nbsp;&nbsp;**retirados = CONSULTAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS**<br>&nbsp;&nbsp;**resultado = UNION(proximosAVencer, retirados)**<br>&nbsp;&nbsp;**CARGAR ALV-TBL-ALERTAS-VENCIMIENTO CON resultado**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR ALV-TBL-ALERTAS-VENCIMIENTO = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-ALV-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La generación automática de nuevas alertas la ejecuta un proceso programado del sistema según el umbral configurado, y se documenta como requisito no funcional aparte. Lo que esta especificación describe es la consulta del resultado consolidado. |

| Código especificación | ESP-0014 |
| --- | --- |
| Nombre | Registro manual de alerta de producto vencido |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0007 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0014 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_LOTES <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-ALV-0001**<br>&nbsp;&nbsp;**VALIDAR ALV-BTN-CREAR-ALERTA = HABILITADO**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**lote = ALV-CMB-LOTE-ALERTA**<br>&nbsp;&nbsp;**MOSTRAR lote.fecha_vencimiento EN ALV-LBL-FECHA-VENCIMIENTO**<br>&nbsp;&nbsp;**loteDefectuoso = ALV-CHK-LOTE-DEFECTUOSO**<br>&nbsp;&nbsp;**alertaDigemid = ALV-TXT-ALERTA-DIGEMID**<br>&nbsp;&nbsp;**PRESIONAR ALV-BTN-CREAR-ALERTA**<br>&nbsp;&nbsp;**VALIDAR lote EXISTE EN DB_FARMASIL.TBL_LOTES**<br>&nbsp;&nbsp;**SI VALIDACION = FALSA ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SEÑALAR CAMPO_INVALIDO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**PERMITIR CORRECCION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (lote.producto.nombre, lote.numero_lote, lote.fecha_vencimiento, loteDefectuoso, alertaDigemid, FECHA_SISTEMA) EN DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES.estado_lote = 'Bloqueado por devolucion' DONDE id_lote = lote**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR ALV-TBL-ALERTAS-VENCIMIENTO**<br>&nbsp;&nbsp;**MOSTRAR "Alerta registrada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.estado_lote = 'Bloqueado por devolucion'**<br>&nbsp;&nbsp;**VERIFICAR REMESA NO DISPONIBLE EN VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;**VERIFICAR OTRAS REMESAS DEL MISMO MEDICAMENTO = DISPONIBLES**<br>&nbsp;&nbsp;**VERIFICAR REMESA DISPONIBLE PARA ORDEN_DEVOLUCION**<br>&nbsp;&nbsp;**VERIFICAR ALV-TBL-ALERTAS-VENCIMIENTO = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-ALV-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | EDU-0004 exige que el sistema no solo emita la alerta sino que bloquee el lote para la venta. La versión anterior registraba la alerta y dejaba el producto plenamente vendible.<br>El producto pasó de capturarse como texto libre a seleccionarse del inventario: sin el identificador, el bloqueo del producto es imposible. |

| Código especificación | ESP-0015 |
| --- | --- |
| Nombre | Modificación de alerta de producto vencido |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0007 |
| Actor | ACT-0001 |
| Fuente | FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0015 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-ALV-0001**<br>&nbsp;&nbsp;**VALIDAR ALV-BTN-ACTUALIZAR-ALERTA = HABILITADO**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR alerta EN ALV-TBL-ALERTAS-VENCIMIENTO**<br>&nbsp;&nbsp;**CARGAR ART-MKP-ALV-0002**<br>&nbsp;&nbsp;**lote = ALV-CMB-LOTE-ALERTA**<br>&nbsp;&nbsp;**MOSTRAR lote.fecha_vencimiento EN ALV-LBL-FECHA-VENCIMIENTO**<br>&nbsp;&nbsp;**loteDefectuoso = ALV-CHK-LOTE-DEFECTUOSO**<br>&nbsp;&nbsp;**alertaDigemid = ALV-TXT-ALERTA-DIGEMID**<br>&nbsp;&nbsp;**PRESIONAR ALV-BTN-ACTUALIZAR-ALERTA**<br>&nbsp;&nbsp;**PRESIONAR ALV-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR lote**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS DONDE id_retiro = alerta**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR ALV-TBL-ALERTAS-VENCIMIENTO**<br>&nbsp;&nbsp;**MOSTRAR "Alerta actualizada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR BLOQUEO_DE_VENTA = VIGENTE**<br>&nbsp;&nbsp;**VERIFICAR ALV-TBL-ALERTAS-VENCIMIENTO = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-ALV-0001, ART-MKP-ALV-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se incorporó el paso de confirmación explícito, coherente con el resto de operaciones de actualización del catálogo, y se mantiene vigente el bloqueo de venta mientras la alerta exista. |

| Código especificación | ESP-0016 |
| --- | --- |
| Nombre | Eliminación de alerta de producto vencido |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0007 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0016 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_DETALLE_DEVOLUCION, id_lote = alerta.lote Y orden.estado_orden = 'Pendiente')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-ALV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR alerta EN ALV-TBL-ALERTAS-VENCIMIENTO**<br>&nbsp;&nbsp;**PRESIONAR ALV-BTN-ELIMINAR-ALERTA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-ALV-0003**<br>&nbsp;&nbsp;**MOSTRAR ALV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR ALV-TXT-MSJ = "¿Está seguro de eliminar esta alerta? La remesa volverá a estar disponible para la venta."**<br>&nbsp;&nbsp;**SI EXISTE(DB_FARMASIL.TBL_DETALLE_DEVOLUCION, id_lote = alerta.lote Y orden.estado_orden = 'Pendiente') ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La remesa forma parte de una orden de devolución pendiente"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**SI PRESIONAR ALV-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS DONDE id_retiro = alerta**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES.estado_lote = 'Disponible' DONDE id_lote = alerta.lote**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Alerta eliminada correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR ALV-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR ALV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR ALV-TBL-ALERTAS-VENCIMIENTO**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ELIMINADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.estado_lote = 'Disponible'**<br>&nbsp;&nbsp;**VERIFICAR REMESA DISPONIBLE EN VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;**VERIFICAR ALV-TBL-ALERTAS-VENCIMIENTO = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-ALV-0001, ART-MKP-ALV-0003 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se incorporó la restitución del estado del producto, contraparte lógica del bloqueo aplicado en ESP-0014: sin ella, eliminar la alerta dejaba el producto bloqueado de forma permanente.<br>Se agregó la verificación de que el producto no esté comprometido en una devolución en curso. |

---

# Módulo 5 — Gestión de Métodos de Pago (EDU-0009)

| Código especificación | ESP-0017 |
| --- | --- |
| Nombre | Registro de método de pago |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0002 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0017 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PAG-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**nombreMetodo = PAG-TXT-METODO-PAGO**<br>&nbsp;&nbsp;**estado = PAG-CMB-ESTADO-METODO**<br>&nbsp;&nbsp;**PRESIONAR PAG-BTN-CREAR-METODO**<br>&nbsp;&nbsp;**VALIDAR nombreMetodo <> VACIO**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_METODOS_PAGO, nombre_metodo = nombreMetodo)**<br>&nbsp;&nbsp;**VALIDAR estado EN ('Activo', 'Inactivo')**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (nombreMetodo, estado) EN DB_FARMASIL.TBL_METODOS_PAGO**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR PAG-TBL-METODOS-PAGO**<br>&nbsp;&nbsp;**MOSTRAR "Método de pago registrado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_METODOS_PAGO.nombre_metodo = UNICO**<br>&nbsp;&nbsp;**VERIFICAR PAG-TBL-METODOS-PAGO = ACTUALIZADA**<br>&nbsp;&nbsp;**SI estado = 'Activo' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR METODO DISPONIBLE EN VEN-CMB-METODO-PAGO**<br>&nbsp;&nbsp;**FIN SI**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-PAG-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se incorporó la validación de unicidad exigida por la restricción UNIQUE del diccionario, ausente en la versión anterior, y se invirtió el orden de la postcondición, que declaraba la actualización de la interfaz antes que la persistencia en base de datos. |

| Código especificación | ESP-0018 |
| --- | --- |
| Nombre | Consulta de métodos de pago |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0002 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0018 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_METODOS_PAGO <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PAG-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PAG-0002**<br>&nbsp;&nbsp;**estado = PAG-CMB-ESTADO-METODO**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_METODOS_PAGO**<br>&nbsp;&nbsp;**APLICAR FILTRO estado = estado**<br>&nbsp;&nbsp;**CARGAR PAG-TBL-METODOS-PAGO CON (id_metodo_pago, nombre_metodo, estado)**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR PAG-TBL-METODOS-PAGO = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_METODOS_PAGO = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-PAG-0001, ART-MKP-PAG-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | El procedimiento anterior tenía dos pasos y no explicaba de dónde salía la información mostrada; se completó el flujo de consulta.<br>Es una de las nueve operaciones disponibles para ambos roles: la técnica necesita conocer qué métodos están habilitados durante la atención. |

| Código especificación | ESP-0019 |
| --- | --- |
| Nombre | Actualización de método de pago |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0002 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0019 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_METODOS_PAGO <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PAG-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR metodo EN PAG-TBL-METODOS-PAGO**<br>&nbsp;&nbsp;**PRESIONAR PAG-BTN-ACTUALIZAR-METODO**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PAG-0003**<br>&nbsp;&nbsp;**nombreMetodo = PAG-TXT-METODO-PAGO**<br>&nbsp;&nbsp;**estado = PAG-CMB-ESTADO-METODO**<br>&nbsp;&nbsp;**PRESIONAR PAG-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR nombreMetodo <> VACIO**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_METODOS_PAGO, nombre_metodo = nombreMetodo Y id_metodo_pago <> metodo)**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_METODOS_PAGO DONDE id_metodo_pago = metodo**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR PAG-TBL-METODOS-PAGO**<br>&nbsp;&nbsp;**MOSTRAR "Método de pago actualizado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR PAG-TBL-METODOS-PAGO = ACTUALIZADA**<br>&nbsp;&nbsp;**SI estado = 'Inactivo' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR METODO NO DISPONIBLE EN VEN-CMB-METODO-PAGO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR REFERENCIAS(TBL_REGISTRO_VENTAS) = INTACTAS**<br>&nbsp;&nbsp;**FIN SI**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-PAG-0001, ART-MKP-PAG-0003 |
| Importancia | Media |
| Estado | Concluido |
| Comentario | Se explicitó que la desactivación de un método no afecta a las ventas ya registradas con él, punto que enlaza esta especificación con ESP-0020. |

| Código especificación | ESP-0020 |
| --- | --- |
| Nombre | Desactivación de método de pago |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0002 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0020 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_METODOS_PAGO <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PAG-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR metodo EN PAG-TBL-METODOS-PAGO**<br>&nbsp;&nbsp;**PRESIONAR PAG-BTN-ELIMINAR-METODO**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PAG-0004**<br>&nbsp;&nbsp;**MOSTRAR PAG-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR PAG-TXT-MSJ = "¿Está seguro de dar de baja este método de pago? Las ventas ya registradas con él no se verán afectadas."**<br>&nbsp;&nbsp;**tieneVentas = EXISTE(DB_FARMASIL.TBL_REGISTRO_VENTAS, id_metodo_pago = metodo)**<br>&nbsp;&nbsp;**SI PRESIONAR PAG-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI tieneVentas = FALSO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_METODOS_PAGO DONDE id_metodo_pago = metodo**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_METODOS_PAGO.estado = 'Inactivo'**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Método de pago dado de baja correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR PAG-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR PAG-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR PAG-TBL-METODOS-PAGO**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR BAJA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR METODO_CON_VENTAS.estado = 'Inactivo'**<br>&nbsp;&nbsp;**VERIFICAR METODO NO DISPONIBLE EN VEN-CMB-METODO-PAGO**<br>&nbsp;&nbsp;**VERIFICAR HISTORICO_VENTAS = INTACTO**<br>&nbsp;&nbsp;**VERIFICAR INTEGRIDAD_REFERENCIAL = SIN_VIOLACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-PAG-0001, ART-MKP-PAG-0004 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La postcondición anterior era contradictoria: para una eliminación afirmaba que el método "queda registrado correctamente".<br>La eliminación física era además inviable, porque `id_metodo_pago` es clave foránea en **TBL_REGISTRO_VENTAS**. Se resolvió con baja lógica cuando existen ventas asociadas. |

---

# Módulo 6 — Gestión de Reportes de Ventas (EDU-0010)

| Código especificación | ESP-0021 |
| --- | --- |
| Nombre | Generación de reportes de ventas |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0021 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REGISTRO_VENTAS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_DETALLE_VENTAS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REPORTES_VENTAS = DISPONIBLE**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REP-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**tipoReporte = REP-CMB-TIPO-REPORTE**<br>&nbsp;&nbsp;**formato = REP-CMB-FORMATO-REPORTE**<br>&nbsp;&nbsp;**fechaInicio = REP-FEC-FECHA-INICIO**<br>&nbsp;&nbsp;**fechaFin = REP-FEC-FECHA-FIN**<br>&nbsp;&nbsp;**PRESIONAR REP-BTN-GENERAR-REPORTE**<br>&nbsp;&nbsp;**VALIDAR fechaInicio <= fechaFin**<br>&nbsp;&nbsp;**VALIDAR tipoReporte**<br>&nbsp;&nbsp;**VALIDAR formato EN ('PDF', 'Excel', 'HTML')**<br>&nbsp;&nbsp;**datos = CONSULTAR DB_FARMASIL.TBL_REGISTRO_VENTAS UNIENDO DB_FARMASIL.TBL_DETALLE_VENTAS UNIENDO DB_FARMASIL.TBL_USUARIOS**<br>&nbsp;&nbsp;**APLICAR FILTRO fecha_hora ENTRE fechaInicio Y fechaFin**<br>&nbsp;&nbsp;**CONSOLIDAR (monto_recaudado, cantidad_productos, fechas, usuario_responsable) DESDE datos**<br>&nbsp;&nbsp;**rutaArchivo = GENERAR_ARCHIVO(datos, formato)**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (tipoReporte, fechaInicio, fechaFin, FECHA_HORA_SISTEMA, rutaArchivo) EN DB_FARMASIL.TBL_REPORTES_VENTAS**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR REP-TBL-REPORTES**<br>&nbsp;&nbsp;**MOSTRAR "Reporte generado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REPORTE_GENERADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REPORTES_VENTAS = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR REPORTE CONTIENE usuario_responsable POR CADA VENTA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REGISTRO_VENTAS = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR ARCHIVO DISPONIBLE EN rutaArchivo**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-REP-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Corresponde a la fase Crear del CRUD. La consulta de un reporte existente, antes mezclada aquí, se separó hacia ESP-0022.<br>La consolidación une ahora con **TBL_USUARIOS** para obtener el responsable de cada venta, que EDU-0010 exige y que la dueña confirmó.<br>Queda Pendiente hasta que el campo `id_usuario` incorporado al Diccionario de Datos v03.00 se refleje en el motor. |

| Código especificación | ESP-0022 |
| --- | --- |
| Nombre | Consulta de reportes de ventas |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0022 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REPORTES_VENTAS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REP-0002**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**PRESIONAR REP-BTN-LEER-REPORTE**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_REPORTES_VENTAS**<br>&nbsp;&nbsp;**CARGAR REP-TBL-REPORTES CON (id_reporte, tipo_reporte, fecha_inicio, fecha_fin, total_recaudado)**<br>&nbsp;&nbsp;**SELECCIONAR reporte EN REP-TBL-REPORTES**<br>&nbsp;&nbsp;**ABRIR ARCHIVO(reporte.ruta_archivo) = SOLO_LECTURA**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR REP-TBL-REPORTES = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REPORTES_VENTAS = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-REP-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Corresponde a la fase Leer del CRUD. El reporte se visualiza en el formato con el que fue generado, en lugar de permitir elegir formato durante la consulta, lo que habría implicado regenerarlo.<br>Restringida al rol Administrador, conforme a RNF-0006, que reserva los reportes de ventas a ese perfil. |

| Código especificación | ESP-0023 |
| --- | --- |
| Nombre | Actualización del intervalo de un reporte de ventas |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0023 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REPORTES_VENTAS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REP-0002**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR reporte EN REP-TBL-REPORTES**<br>&nbsp;&nbsp;**PRESIONAR REP-BTN-ACTUALIZAR-REPORTE**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REP-0003 CON (reporte.fecha_inicio, reporte.fecha_fin)**<br>&nbsp;&nbsp;**fechaInicio = REP-FEC-FECHA-INICIO**<br>&nbsp;&nbsp;**fechaFin = REP-FEC-FECHA-FIN**<br>&nbsp;&nbsp;**PRESIONAR REP-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR fechaInicio <= fechaFin**<br>&nbsp;&nbsp;**datos = CONSULTAR DB_FARMASIL.TBL_REGISTRO_VENTAS UNIENDO DB_FARMASIL.TBL_DETALLE_VENTAS = SOLO_LECTURA**<br>&nbsp;&nbsp;**APLICAR FILTRO fecha_hora ENTRE fechaInicio Y fechaFin**<br>&nbsp;&nbsp;**rutaArchivo = REGENERAR_ARCHIVO(datos, reporte.formato)**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_REPORTES_VENTAS SET fecha_inicio, fecha_fin, fecha_generacion, ruta_archivo DONDE id_reporte = reporte**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR REP-TBL-REPORTES**<br>&nbsp;&nbsp;**MOSTRAR "Reporte actualizado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REGISTRO_VENTAS = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_DETALLE_VENTAS = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR REP-TBL-REPORTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-REP-0002, ART-MKP-REP-0003 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Reemplaza a la antigua especificación "Restricción de modificación", que describía la ausencia de una operación y dejaba incompleta la fase Actualizar del CRUD.<br>Solo se actualiza el índice y los parámetros del reporte. Las dos tablas de origen se consultan en modo de solo lectura, y la postcondición lo verifica explícitamente.<br>EDU-0010 v05.00 amplió el alcance del módulo para autorizar esta operación. |

| Código especificación | ESP-0024 |
| --- | --- |
| Nombre | Eliminación de un reporte de ventas generado |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0024 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REPORTES_VENTAS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REP-0002**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR reporte EN REP-TBL-REPORTES**<br>&nbsp;&nbsp;**PRESIONAR REP-BTN-ELIMINAR-REPORTE**<br>&nbsp;&nbsp;**MOSTRAR REP-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR REP-TXT-MSJ = "¿Está seguro de eliminar este reporte? Las ventas originales no se verán afectadas."**<br>&nbsp;&nbsp;**SI PRESIONAR REP-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR ARCHIVO(reporte.ruta_archivo)**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_REPORTES_VENTAS DONDE id_reporte = reporte**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Reporte eliminado correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR REP-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR REP-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR REP-TBL-REPORTES**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ELIMINADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REGISTRO_VENTAS = INTACTA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_DETALLE_VENTAS = INTACTA**<br>&nbsp;&nbsp;**VERIFICAR REP-TBL-REPORTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-REP-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Reemplaza a la antigua especificación "Restricción de eliminación". La eliminación física es legítima aquí porque el reporte es un archivo de salida sin referencias foráneas.<br>EDU-0010 v05.00 amplió el alcance del módulo para autorizar esta operación. |

---

# Módulo 7 — Gestión de Devoluciones a Proveedores (EDU-0011)

| Código especificación | ESP-0025 |
| --- | --- |
| Nombre | Registro de orden de devolución |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0025 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_LOTES, estado_lote = 'Bloqueado por devolucion')**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_PROVEEDORES, estado = 'Activo')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DEV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**fechaCreacion = DEV-FEC-FECHA-DEVOLUCION**<br>&nbsp;&nbsp;**proveedor = DEV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**motivo = DEV-CMB-MOTIVO-DEVOLUCION**<br>&nbsp;&nbsp;**detalle = LISTA_VACIA**<br>&nbsp;&nbsp;**REPETIR**<br>&nbsp;&nbsp;&nbsp;&nbsp;**lote = DEV-CMB-LOTE-DEVOLUCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**cantidad = DEV-NUM-CANTIDAD-DEVOLVER**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR lote.producto.id_proveedor = proveedor**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR cantidad = ENTERO > 0**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR cantidad <= lote.stock_actual**<br>&nbsp;&nbsp;&nbsp;&nbsp;**AGREGAR (lote, cantidad) A detalle**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DEV-TBL-DETALLE-DEVOLUCION**<br>&nbsp;&nbsp;**HASTA QUE USUARIO NO AGREGUE MAS REMESAS**<br>&nbsp;&nbsp;**comentario = DEV-TXA-COMENTARIO**<br>&nbsp;&nbsp;**SI motivo = 'Por Depuracion de Error' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR comentario <> VACIO**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**PRESIONAR DEV-BTN-CREAR-DEVOLUCION**<br>&nbsp;&nbsp;**VALIDAR detalle <> VACIA**<br>&nbsp;&nbsp;**VALIDAR motivo EN ('Por Vencimiento', 'Por Depuracion de Error')**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (fechaCreacion, motivo, proveedor, comentario, 'Pendiente', SESION_USUARIO.id_usuario) EN DB_FARMASIL.TBL_ORDENES_DEVOLUCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**idDevolucion = ULTIMO_ID_GENERADO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA linea EN detalle HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (idDevolucion, linea.lote, linea.cantidad) EN DB_FARMASIL.TBL_DETALLE_DEVOLUCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR DEV-TBL-ORDENES-DEVOLUCION**<br>&nbsp;&nbsp;**MOSTRAR "Orden de devolución registrada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION.estado_orden = 'Pendiente'**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION.id_usuario = SESION_USUARIO.id_usuario**<br>&nbsp;&nbsp;**VERIFICAR CONTAR(DB_FARMASIL.TBL_DETALLE_DEVOLUCION DONDE id_devolucion = idDevolucion) = CONTAR(detalle)**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.stock_actual = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR DEV-TBL-ORDENES-DEVOLUCION = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DEV-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Reescrita sobre el par cabecera y detalle. La versión anterior registraba un único medicamento e ignoraba **TBL_DETALLE_DEVOLUCION**.<br>Se incorporaron `cantidad_devolver`, que es NOT NULL y no se capturaba en ninguna parte, y `comentario`, obligatorio cuando el motivo es la depuración de un error.<br>El stock no se modifica al registrar: el descuento ocurre cuando la orden pasa a 'Completada' en ESP-0027, que es cuando la mercancía sale físicamente del almacén.<br>Depende de los componentes **DEV-TBL-DETALLE-DEVOLUCION**, **DEV-NUM-CANTIDAD-DEVOLVER** y **DEV-TXA-COMENTARIO**, aún por incorporar al mockup. |

| Código especificación | ESP-0026 |
| --- | --- |
| Nombre | Consulta de orden de devolución |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0026 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DEV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**CARGAR DEV-TBL-ORDENES-DEVOLUCION CON (id_devolucion, fecha_creacion, proveedor, motivo_devolucion, estado_orden)**<br>&nbsp;&nbsp;**SELECCIONAR orden EN DEV-TBL-ORDENES-DEVOLUCION**<br>&nbsp;&nbsp;**PRESIONAR DEV-BTN-LEER-DEVOLUCION**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DEV-0002**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION UNIENDO DB_FARMASIL.TBL_PROVEEDORES**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_DETALLE_DEVOLUCION UNIENDO DB_FARMASIL.TBL_LOTES UNIENDO DB_FARMASIL.TBL_PRODUCTOS DONDE id_devolucion = orden**<br>&nbsp;&nbsp;**CARGAR DEV-TBL-DETALLE-DEVOLUCION CON (nombre, numero_lote, cantidad_devolver)**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DEV-TBL-ORDENES-DEVOLUCION = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DEV-TBL-DETALLE-DEVOLUCION = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DEV-0001, ART-MKP-DEV-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La postcondición anterior decía únicamente que "el sistema permanece invariable", sin declarar el resultado de la consulta.<br>El detalle se resuelve contra **TBL_DETALLE_DEVOLUCION**, y el lote se obtiene de **TBL_PRODUCTOS**, campo incorporado en el Diccionario de Datos v03.00. |

| Código especificación | ESP-0027 |
| --- | --- |
| Nombre | Actualización de orden de devolución |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0027 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION <> VACIA**<br>&nbsp;&nbsp;**VALIDAR orden.estado_orden = 'Pendiente'**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DEV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR orden EN DEV-TBL-ORDENES-DEVOLUCION**<br>&nbsp;&nbsp;**PRESIONAR DEV-BTN-ACTUALIZAR-DEVOLUCION**<br>&nbsp;&nbsp;**SI orden.estado_orden = 'Completada' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La orden ya fue despachada y no admite modificación"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DEV-0003 CON cabecera Y detalle VIGENTES**<br>&nbsp;&nbsp;**proveedor = DEV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**motivo = DEV-CMB-MOTIVO-DEVOLUCION**<br>&nbsp;&nbsp;**comentario = DEV-TXA-COMENTARIO**<br>&nbsp;&nbsp;**detalleNuevo = DEV-TBL-DETALLE-DEVOLUCION**<br>&nbsp;&nbsp;**estadoNuevo = ESTADO_SELECCIONADO**<br>&nbsp;&nbsp;**PRESIONAR DEV-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR detalleNuevo <> VACIA**<br>&nbsp;&nbsp;**VALIDAR estadoNuevo EN ('Pendiente', 'Completada')**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION DONDE id_devolucion = orden**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SINCRONIZAR DB_FARMASIL.TBL_DETALLE_DEVOLUCION CON detalleNuevo**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI estadoNuevo = 'Completada' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA linea EN detalleNuevo HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES.stock_actual = stock_actual - linea.cantidad_devolver DONDE id_lote = linea.lote**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR DEV-TBL-ORDENES-DEVOLUCION**<br>&nbsp;&nbsp;**MOSTRAR "Orden de devolución actualizada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**SI estadoNuevo = 'Completada' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.stock_actual = DESCONTADO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR ORDEN = NO_MODIFICABLE**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**VERIFICAR CABECERA Y DETALLE = CONSISTENTES**<br>&nbsp;&nbsp;**VERIFICAR DEV-TBL-ORDENES-DEVOLUCION = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DEV-0001, ART-MKP-DEV-0003 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La postcondición anterior afirmaba que "los medicamentos actualizan su estado correspondiente" sin precisar cuál era ese estado ni qué campo se modificaba. Se sustituyó por el descuento efectivo del stock al completarse la orden.<br>Se incorporó el bloqueo de las órdenes ya despachadas.<br>Depende de los mismos componentes pendientes que ESP-0025. |

| Código especificación | ESP-0028 |
| --- | --- |
| Nombre | Eliminación de orden de devolución |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0028 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION <> VACIA**<br>&nbsp;&nbsp;**VALIDAR orden.estado_orden = 'Pendiente'**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DEV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR orden EN DEV-TBL-ORDENES-DEVOLUCION**<br>&nbsp;&nbsp;**PRESIONAR DEV-BTN-ELIMINAR-DEVOLUCION**<br>&nbsp;&nbsp;**SI orden.estado_orden = 'Completada' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La mercancía fue despachada. La orden debe conservarse"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DEV-0004**<br>&nbsp;&nbsp;**MOSTRAR DEV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR DEV-TXT-MSJ = "¿Está seguro de que desea eliminar la orden?"**<br>&nbsp;&nbsp;**SI PRESIONAR DEV-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_DETALLE_DEVOLUCION DONDE id_devolucion = orden**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION.estado_orden = 'Eliminada' DONDE id_devolucion = orden**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Orden de devolución eliminada correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR DEV-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR DEV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR DEV-TBL-ORDENES-DEVOLUCION**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION.estado_orden = 'Eliminada'**<br>&nbsp;&nbsp;**VERIFICAR NO EXISTE(DB_FARMASIL.TBL_DETALLE_DEVOLUCION, id_devolucion = orden)**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.stock_actual = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.estado_lote = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR ORDEN_COMPLETADA = SIN_ELIMINAR**<br>&nbsp;&nbsp;**VERIFICAR DEV-TBL-ORDENES-DEVOLUCION = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DEV-0001, ART-MKP-DEV-0004 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La eliminación se resolvió como baja lógica usando el valor 'Eliminada' que el diccionario ya define en `estado_orden`.<br>La postcondición anterior afirmaba que los medicamentos actualizaban su estado al eliminar la orden, lo cual es incorrecto: si la devolución se cancela, el producto debe conservar el bloqueo que la motivó. |

---

# Módulo 8 — Gestión de Alertas de Restricciones de Venta (EDU-0012)

| Código especificación | ESP-0029 |
| --- | --- |
| Nombre | Registro de restricción de venta |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0006 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0029 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_RESTRICCIONES_VENTA = DISPONIBLE**<br>&nbsp;&nbsp;**CARGAR ART-MKP-RES-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**medicamento = RES-CMB-MEDICAMENTO**<br>&nbsp;&nbsp;**tipoRestriccion = RES-CMB-TIPO-RESTRICCION**<br>&nbsp;&nbsp;**condicionCliente = RES-TXT-CONDICION-CLIENTE**<br>&nbsp;&nbsp;**motivoAdvertencia = RES-TXA-MOTIVO-ADVERTENCIA**<br>&nbsp;&nbsp;**estadoAlerta = RES-CMB-ESTADO-ALERTA**<br>&nbsp;&nbsp;**PRESIONAR RES-BTN-CREAR-RESTRICCION**<br>&nbsp;&nbsp;**VALIDAR medicamento EXISTE EN DB_FARMASIL.TBL_PRODUCTOS**<br>&nbsp;&nbsp;**VALIDAR tipoRestriccion EN ('Informativa', 'Bloqueante')**<br>&nbsp;&nbsp;**VALIDAR condicionCliente <> VACIO**<br>&nbsp;&nbsp;**VALIDAR motivoAdvertencia <> VACIO**<br>&nbsp;&nbsp;**VALIDAR estadoAlerta EN ('Activo', 'Inactivo')**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (medicamento, condicionCliente, tipoRestriccion, motivoAdvertencia, estadoAlerta) EN DB_FARMASIL.TBL_RESTRICCIONES_VENTA**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR RES-TBL-RESTRICCIONES-VENTA**<br>&nbsp;&nbsp;**MOSTRAR "Restricción registrada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR RESTRICCION ASOCIADA A medicamento**<br>&nbsp;&nbsp;**VERIFICAR RES-TBL-RESTRICCIONES-VENTA = ACTUALIZADA**<br>&nbsp;&nbsp;**SI tipoRestriccion = 'Bloqueante' Y estadoAlerta = 'Activo' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR MEDICAMENTO BLOQUEADO EN ESP-0001**<br>&nbsp;&nbsp;**FIN SI**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-RES-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | El módulo usaba el prefijo **ALR_** con guion bajo y una tabla inexistente, **ALR_TBL_ALERTAS_RESTRICCION_VENTA**, empleando el mismo nombre para la tabla de base de datos y la de la interfaz. Se separaron y se unificó al prefijo **RES**.<br>El medicamento se capturaba como texto libre pese a ser clave foránea; ahora se selecciona del inventario.<br>La operación quedó restringida al administrador: definir qué medicamento se bloquea para qué grupo de riesgo es una decisión de farmacovigilancia. |

| Código especificación | ESP-0030 |
| --- | --- |
| Nombre | Consulta de restricciones de venta |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0006 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0030 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_RESTRICCIONES_VENTA <> VACIA**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-RES-0002**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**medicamento = RES-CMB-MEDICAMENTO**<br>&nbsp;&nbsp;**PRESIONAR RES-BTN-LEER-RESTRICCION**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_RESTRICCIONES_VENTA UNIENDO DB_FARMASIL.TBL_PRODUCTOS**<br>&nbsp;&nbsp;**APLICAR FILTRO id_producto = medicamento**<br>&nbsp;&nbsp;**CARGAR RES-TBL-RESTRICCIONES-VENTA CON (nombre, condicion_cliente, tipo_restriccion, motivo_advertencia, estado_alerta)**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR RES-TBL-RESTRICCIONES-VENTA = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_RESTRICCIONES_VENTA = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-RES-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Es una de las nueve operaciones disponibles para ambos roles: la técnica necesita verificar las advertencias sanitarias de un medicamento antes de completar una venta. |

| Código especificación | ESP-0031 |
| --- | --- |
| Nombre | Modificación de restricción de venta |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0006 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0031 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_RESTRICCIONES_VENTA <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-RES-0003**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR restriccion EN RES-TBL-RESTRICCIONES-VENTA**<br>&nbsp;&nbsp;**PRESIONAR RES-BTN-ACTUALIZAR-RESTRICCION**<br>&nbsp;&nbsp;**CARGAR ART-MKP-RES-0003 CON INFORMACION VIGENTE**<br>&nbsp;&nbsp;**medicamento = RES-CMB-MEDICAMENTO**<br>&nbsp;&nbsp;**tipoRestriccion = RES-CMB-TIPO-RESTRICCION**<br>&nbsp;&nbsp;**condicionCliente = RES-TXT-CONDICION-CLIENTE**<br>&nbsp;&nbsp;**motivoAdvertencia = RES-TXA-MOTIVO-ADVERTENCIA**<br>&nbsp;&nbsp;**estadoAlerta = RES-CMB-ESTADO-ALERTA**<br>&nbsp;&nbsp;**PRESIONAR RES-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR tipoRestriccion EN ('Informativa', 'Bloqueante')**<br>&nbsp;&nbsp;**VALIDAR condicionCliente <> VACIO**<br>&nbsp;&nbsp;**VALIDAR motivoAdvertencia <> VACIO**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_RESTRICCIONES_VENTA DONDE id_restriccion = restriccion**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR RES-TBL-RESTRICCIONES-VENTA**<br>&nbsp;&nbsp;**MOSTRAR "Restricción actualizada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR RES-TBL-RESTRICCIONES-VENTA = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR NUEVA_CONFIGURACION APLICADA EN VALIDACIONES POSTERIORES DE ESP-0001**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-RES-0003 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se incorporó el paso de confirmación explícito, pese a que el componente **RES-BTN-CONFIRMAR-ACTUALIZACION** ya estaba definido, y se aplicaron las correcciones de nomenclatura y tipo del módulo. |

| Código especificación | ESP-0032 |
| --- | --- |
| Nombre | Desactivación de restricción de venta |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0006 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0032 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_RESTRICCIONES_VENTA <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-RES-0004**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR restriccion EN RES-TBL-RESTRICCIONES-VENTA**<br>&nbsp;&nbsp;**PRESIONAR RES-BTN-ELIMINAR-RESTRICCION**<br>&nbsp;&nbsp;**MOSTRAR RES-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR RES-TXT-MSJ = "¿Está seguro de desactivar esta restricción? El medicamento dejará de mostrar la advertencia durante la venta."**<br>&nbsp;&nbsp;**SI PRESIONAR RES-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_RESTRICCIONES_VENTA.estado_alerta = 'Inactivo' DONDE id_restriccion = restriccion**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Restricción desactivada correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR RES-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR RES-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR RES-TBL-RESTRICCIONES-VENTA**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_RESTRICCIONES_VENTA.estado_alerta = 'Inactivo'**<br>&nbsp;&nbsp;**VERIFICAR RESTRICCION NO PARTICIPA EN VALIDACIONES DE ESP-0001**<br>&nbsp;&nbsp;**VERIFICAR HISTORIAL_CONSERVADO_PARA_AUDITORIA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR RES-TBL-RESTRICCIONES-VENTA = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-RES-0004 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La fase Eliminar se resuelve como baja lógica, criterio adecuado tratándose de información de farmacovigilancia que debe conservarse para auditoría sanitaria. |

---

# Módulo 9 — Gestión de Usuarios (EDU-0013)

| Código especificación | ESP-0033 |
| --- | --- |
| Nombre | Creación de usuario |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Consulta posterior al cliente |
| Experto | Ninguno |
| Código ilación | ILA-0033 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**CARGAR ART-MKP-USR-0001**<br>&nbsp;&nbsp;**VALIDAR USR-BTN-CREAR-USUARIO = HABILITADO**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**nombreUsuario = USR-TXT-NOMBRE-USUARIO**<br>&nbsp;&nbsp;**contrasena = USR-PWD-CONTRASENA**<br>&nbsp;&nbsp;**rol = USR-CMB-ROL**<br>&nbsp;&nbsp;**estado = USR-CMB-ESTADO-USUARIO**<br>&nbsp;&nbsp;**PRESIONAR USR-BTN-CREAR-USUARIO**<br>&nbsp;&nbsp;**VALIDAR nombreUsuario <> VACIO**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_USUARIOS, nombre_usuario = nombreUsuario)**<br>&nbsp;&nbsp;**VALIDAR contrasena <> VACIA**<br>&nbsp;&nbsp;**VALIDAR rol EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR estado EN ('Activo', 'Inactivo')**<br>&nbsp;&nbsp;**contrasenaEncriptada = ENCRIPTAR(contrasena)**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (nombreUsuario, contrasenaEncriptada, rol, estado) EN DB_FARMASIL.TBL_USUARIOS**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR USR-TBL-USUARIOS**<br>&nbsp;&nbsp;**MOSTRAR "Usuario registrado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_USUARIOS.contrasena = ENCRIPTADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_USUARIOS.nombre_usuario = UNICO**<br>&nbsp;&nbsp;**VERIFICAR USR-TBL-USUARIOS NO CONTIENE COLUMNA contrasena**<br>&nbsp;&nbsp;**SI estado = 'Activo' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR CUENTA HABILITADA PARA INICIO DE SESION SEGUN CONDICION 1 DE RNF-0006**<br>&nbsp;&nbsp;**FIN SI**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-USR-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Especificación nueva. La tabla **TBL_USUARIOS** existía en el modelo y era referenciada desde **TBL_ORDENES_DEVOLUCION**, pero ningún procedimiento alimentaba su contenido.<br>La contraseña se encripta antes de persistirse y nunca se devuelve en la tabla de la interfaz, conforme al atributo de seguridad de RNF-0006.<br>Queda Pendiente porque el mockup **ART-MKP-USR-0001** aún no ha sido diseñado. |

| Código especificación | ESP-0034 |
| --- | --- |
| Nombre | Consulta de usuarios |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Consulta posterior al cliente |
| Experto | Ninguno |
| Código ilación | ILA-0034 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_USUARIOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-USR-0002**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**nombreUsuario = USR-TXT-NOMBRE-USUARIO**<br>&nbsp;&nbsp;**rol = USR-CMB-ROL**<br>&nbsp;&nbsp;**PRESIONAR USR-BTN-LEER-USUARIO**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_USUARIOS**<br>&nbsp;&nbsp;**APLICAR FILTRO nombre_usuario = nombreUsuario**<br>&nbsp;&nbsp;**APLICAR FILTRO rol = rol**<br>&nbsp;&nbsp;**EXCLUIR COLUMNA contrasena DEL RESULTADO**<br>&nbsp;&nbsp;**CARGAR USR-TBL-USUARIOS CON (id_usuario, nombre_usuario, rol, estado)**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR RESULTADO NO CONTIENE contrasena**<br>&nbsp;&nbsp;**VERIFICAR USR-TBL-USUARIOS = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_USUARIOS = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-USR-0002 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La exclusión explícita de la contraseña en el resultado responde a la condición 5 de RNF-0006 v03.00: ni siquiera en su forma encriptada debe viajar hacia la interfaz.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-USR-0002**. |

| Código especificación | ESP-0035 |
| --- | --- |
| Nombre | Actualización de usuario |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Consulta posterior al cliente |
| Experto | Ninguno |
| Código ilación | ILA-0035 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_USUARIOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-USR-0003**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR cuenta EN USR-TBL-USUARIOS**<br>&nbsp;&nbsp;**PRESIONAR USR-BTN-ACTUALIZAR-USUARIO**<br>&nbsp;&nbsp;**CARGAR ART-MKP-USR-0003 CON (cuenta.nombre_usuario, cuenta.rol, cuenta.estado)**<br>&nbsp;&nbsp;**LIMPIAR USR-PWD-CONTRASENA**<br>&nbsp;&nbsp;**nombreUsuario = USR-TXT-NOMBRE-USUARIO**<br>&nbsp;&nbsp;**contrasena = USR-PWD-CONTRASENA**<br>&nbsp;&nbsp;**rol = USR-CMB-ROL**<br>&nbsp;&nbsp;**estado = USR-CMB-ESTADO-USUARIO**<br>&nbsp;&nbsp;**PRESIONAR USR-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR nombreUsuario <> VACIO**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_USUARIOS, nombre_usuario = nombreUsuario Y id_usuario <> cuenta)**<br>&nbsp;&nbsp;**administradoresActivos = CONTAR(DB_FARMASIL.TBL_USUARIOS DONDE rol = 'Administrador' Y estado = 'Activo' Y id_usuario <> cuenta)**<br>&nbsp;&nbsp;**SI administradoresActivos = 0 Y (rol <> 'Administrador' O estado <> 'Activo') ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "El sistema debe conservar al menos una cuenta administradora activa"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_USUARIOS SET nombre_usuario, rol, estado DONDE id_usuario = cuenta**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI contrasena <> VACIA ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_USUARIOS.contrasena = ENCRIPTAR(contrasena)**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR USR-TBL-USUARIOS**<br>&nbsp;&nbsp;**MOSTRAR "Usuario actualizado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_USUARIOS.id_usuario = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR CONTAR(DB_FARMASIL.TBL_USUARIOS DONDE rol = 'Administrador' Y estado = 'Activo') >= 1**<br>&nbsp;&nbsp;**VERIFICAR REFERENCIAS(TBL_ORDENES_DEVOLUCION, TBL_REGISTRO_VENTAS) = INTACTAS**<br>&nbsp;&nbsp;**VERIFICAR USR-TBL-USUARIOS = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-USR-0003 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La validación del último administrador activo evita el bloqueo total del sistema: si la única cuenta administradora se degrada a técnico o se desactiva, nadie podría volver a administrar usuarios.<br>El campo de contraseña se presenta vacío y solo se actualiza si el administrador escribe una nueva, de modo que editar el rol no obliga a restablecer credenciales.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-USR-0003**. |

| Código especificación | ESP-0036 |
| --- | --- |
| Nombre | Desactivación de usuario |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Consulta posterior al cliente |
| Experto | Ninguno |
| Código ilación | ILA-0036 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_USUARIOS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR cuenta <> SESION_USUARIO.id_usuario**<br>&nbsp;&nbsp;**CARGAR ART-MKP-USR-0004**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR cuenta EN USR-TBL-USUARIOS**<br>&nbsp;&nbsp;**PRESIONAR USR-BTN-ELIMINAR-USUARIO**<br>&nbsp;&nbsp;**SI cuenta = SESION_USUARIO.id_usuario ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "No es posible dar de baja la cuenta con la que se encuentra abierta la sesión"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**MOSTRAR USR-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR USR-TXT-MSJ = "¿Está seguro de dar de baja esta cuenta? El histórico de operaciones que registró se conservará."**<br>&nbsp;&nbsp;**tieneOperaciones = EXISTE(DB_FARMASIL.TBL_ORDENES_DEVOLUCION, id_usuario = cuenta) O EXISTE(DB_FARMASIL.TBL_REGISTRO_VENTAS, id_usuario = cuenta)**<br>&nbsp;&nbsp;**SI PRESIONAR USR-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI tieneOperaciones = FALSO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_USUARIOS DONDE id_usuario = cuenta**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_USUARIOS.estado = 'Inactivo' DONDE id_usuario = cuenta**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Cuenta dada de baja correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR USR-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR USR-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR USR-TBL-USUARIOS**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR BAJA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR CUENTA_CON_OPERACIONES.estado = 'Inactivo'**<br>&nbsp;&nbsp;**VERIFICAR TRAZABILIDAD DE VENTAS Y DEVOLUCIONES = CONSERVADA**<br>&nbsp;&nbsp;**VERIFICAR CUENTA NO PUEDE INICIAR SESION SEGUN CONDICION 1 DE RNF-0006**<br>&nbsp;&nbsp;**VERIFICAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VERIFICAR INTEGRIDAD_REFERENCIAL = SIN_VIOLACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-USR-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La baja lógica es obligatoria cuando la cuenta registró operaciones: eliminarla físicamente destruiría la verificación de identidad que la dueña solicitó.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-USR-0004** y por el requisito, aún no redactado, que impide el inicio de sesión de una cuenta inactiva. |

---

# Módulo 10 — Gestión de Proveedores (EDU-0014)

| Código especificación | ESP-0037 |
| --- | --- |
| Nombre | Registro de proveedor |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0037 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PRV-0001**<br>&nbsp;&nbsp;**VALIDAR PRV-BTN-CREAR-PROVEEDOR = HABILITADO**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**ruc = PRV-TXT-RUC**<br>&nbsp;&nbsp;**razonSocial = PRV-TXT-RAZON-SOCIAL**<br>&nbsp;&nbsp;**telefono = PRV-TXT-TELEFONO**<br>&nbsp;&nbsp;**estado = PRV-CMB-ESTADO-PROVEEDOR**<br>&nbsp;&nbsp;**PRESIONAR PRV-BTN-CREAR-PROVEEDOR**<br>&nbsp;&nbsp;**VALIDAR ruc = 11 DIGITOS NUMERICOS**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_PROVEEDORES, ruc = ruc)**<br>&nbsp;&nbsp;**VALIDAR razonSocial <> VACIO**<br>&nbsp;&nbsp;**VALIDAR estado EN ('Activo', 'Inactivo')**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (ruc, razonSocial, telefono, estado) EN DB_FARMASIL.TBL_PROVEEDORES**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR PRV-TBL-PROVEEDORES**<br>&nbsp;&nbsp;**MOSTRAR "Proveedor registrado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PROVEEDORES.ruc = UNICO**<br>&nbsp;&nbsp;**VERIFICAR PRV-TBL-PROVEEDORES = ACTUALIZADA**<br>&nbsp;&nbsp;**SI estado = 'Activo' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR PROVEEDOR DISPONIBLE EN INV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR PROVEEDOR DISPONIBLE EN DEV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**FIN SI**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-PRV-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Especificación nueva. La tabla **TBL_PROVEEDORES** existía y era referenciada desde **TBL_PRODUCTOS** y **TBL_ORDENES_DEVOLUCION**, pero ningún procedimiento la alimentaba: ESP-0025 exigía proveedores válidos como precondición sin que existiera forma de crearlos.<br>La validación de once dígitos corresponde a la restricción `NVARCHAR(11)` con UNIQUE del Diccionario de Datos v03.00.<br>Queda Pendiente porque el mockup **ART-MKP-PRV-0001** aún no ha sido diseñado. |

| Código especificación | ESP-0038 |
| --- | --- |
| Nombre | Consulta de proveedores |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0038 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PROVEEDORES <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PRV-0002**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**ruc = PRV-TXT-RUC**<br>&nbsp;&nbsp;**razonSocial = PRV-TXT-RAZON-SOCIAL**<br>&nbsp;&nbsp;**PRESIONAR PRV-BTN-LEER-PROVEEDOR**<br>&nbsp;&nbsp;**VALIDAR ruc O razonSocial <> VACIO**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_PROVEEDORES**<br>&nbsp;&nbsp;**APLICAR FILTRO ruc = ruc**<br>&nbsp;&nbsp;**APLICAR FILTRO razon_social = razonSocial**<br>&nbsp;&nbsp;**CARGAR PRV-TBL-PROVEEDORES CON (id_proveedor, ruc, razon_social, telefono, estado)**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR PRV-TBL-PROVEEDORES = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PROVEEDORES = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-PRV-0002 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Permite ubicar los datos de contacto de la droguería al momento de coordinar el retiro de un lote, que según la Entrevista 1 se gestiona por comunicación directa con el proveedor.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-PRV-0002**. |

| Código especificación | ESP-0039 |
| --- | --- |
| Nombre | Actualización de proveedor |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0039 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PROVEEDORES <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PRV-0003**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR proveedor EN PRV-TBL-PROVEEDORES**<br>&nbsp;&nbsp;**PRESIONAR PRV-BTN-ACTUALIZAR-PROVEEDOR**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PRV-0003 CON INFORMACION VIGENTE**<br>&nbsp;&nbsp;**ruc = PRV-TXT-RUC**<br>&nbsp;&nbsp;**razonSocial = PRV-TXT-RAZON-SOCIAL**<br>&nbsp;&nbsp;**telefono = PRV-TXT-TELEFONO**<br>&nbsp;&nbsp;**estado = PRV-CMB-ESTADO-PROVEEDOR**<br>&nbsp;&nbsp;**PRESIONAR PRV-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR ruc = 11 DIGITOS NUMERICOS**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_PROVEEDORES, ruc = ruc Y id_proveedor <> proveedor)**<br>&nbsp;&nbsp;**VALIDAR razonSocial <> VACIO**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_PROVEEDORES DONDE id_proveedor = proveedor**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR PRV-TBL-PROVEEDORES**<br>&nbsp;&nbsp;**MOSTRAR "Proveedor actualizado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PROVEEDORES.id_proveedor = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR REFERENCIAS(TBL_PRODUCTOS, TBL_ORDENES_DEVOLUCION) = INTACTAS**<br>&nbsp;&nbsp;**SI estado = 'Inactivo' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR PROVEEDOR NO DISPONIBLE EN INV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR PROVEEDOR NO DISPONIBLE EN DEV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**VERIFICAR PRV-TBL-PROVEEDORES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-PRV-0003 |
| Importancia | Media |
| Estado | Pendiente |
| Comentario | Permite mantener vigentes los datos de contacto de las distribuidoras, que cambian con más frecuencia que la propia relación comercial.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-PRV-0003**. |

| Código especificación | ESP-0040 |
| --- | --- |
| Nombre | Desactivación de proveedor |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0040 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PROVEEDORES <> VACIA**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_ORDENES_DEVOLUCION, id_proveedor = proveedor Y estado_orden = 'Pendiente')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PRV-0004**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR proveedor EN PRV-TBL-PROVEEDORES**<br>&nbsp;&nbsp;**PRESIONAR PRV-BTN-ELIMINAR-PROVEEDOR**<br>&nbsp;&nbsp;**SI EXISTE(DB_FARMASIL.TBL_ORDENES_DEVOLUCION, id_proveedor = proveedor Y estado_orden = 'Pendiente') ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "El proveedor tiene órdenes de devolución pendientes de despacho"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**MOSTRAR PRV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR PRV-TXT-MSJ = "¿Está seguro de dar de baja este proveedor? Los productos y devoluciones ya registrados no se verán afectados."**<br>&nbsp;&nbsp;**tieneRegistros = EXISTE(DB_FARMASIL.TBL_PRODUCTOS, id_proveedor = proveedor) O EXISTE(DB_FARMASIL.TBL_ORDENES_DEVOLUCION, id_proveedor = proveedor)**<br>&nbsp;&nbsp;**SI PRESIONAR PRV-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI tieneRegistros = FALSO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_PROVEEDORES DONDE id_proveedor = proveedor**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_PROVEEDORES.estado = 'Inactivo' DONDE id_proveedor = proveedor**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Proveedor dado de baja correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR PRV-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR PRV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR PRV-TBL-PROVEEDORES**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR BAJA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR PROVEEDOR_CON_REGISTROS.estado = 'Inactivo'**<br>&nbsp;&nbsp;**VERIFICAR PROVEEDOR NO DISPONIBLE EN INV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**VERIFICAR PROVEEDOR NO DISPONIBLE EN DEV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**VERIFICAR NINGUNA ORDEN PENDIENTE = SIN_DESTINATARIO**<br>&nbsp;&nbsp;**VERIFICAR INTEGRIDAD_REFERENCIAL = SIN_VIOLACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-PRV-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Se aplica el mismo criterio de baja lógica del resto del catálogo, más una verificación adicional: un proveedor con una devolución aún no despachada no puede darse de baja, porque la orden quedaría sin destinatario.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-PRV-0004**. |

---

# Módulo 11 — Gestión de Lotes (EDU-0015)

| Código especificación | ESP-0041 |
| --- | --- |
| Nombre | Registro de remesa |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0041 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_PRODUCTOS, estado_producto = 'Disponible')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-LOT-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**producto = LOT-CMB-PRODUCTO**<br>&nbsp;&nbsp;**numeroLote = LOT-TXT-NUMERO-LOTE**<br>&nbsp;&nbsp;**fechaVencimiento = LOT-FEC-FECHA-VENCIMIENTO**<br>&nbsp;&nbsp;**stock = LOT-NUM-STOCK-LOTE**<br>&nbsp;&nbsp;**fechaIngreso = LOT-FEC-FECHA-INGRESO**<br>&nbsp;&nbsp;**PRESIONAR LOT-BTN-CREAR-LOTE**<br>&nbsp;&nbsp;**VALIDAR producto EXISTE EN DB_FARMASIL.TBL_PRODUCTOS**<br>&nbsp;&nbsp;**VALIDAR numeroLote <> VACIO**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_LOTES, id_producto = producto Y numero_lote = numeroLote)**<br>&nbsp;&nbsp;**VALIDAR fechaVencimiento > FECHA_SISTEMA**<br>&nbsp;&nbsp;**VALIDAR stock = ENTERO >= 0**<br>&nbsp;&nbsp;**VALIDAR fechaIngreso <= FECHA_SISTEMA**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (producto, numeroLote, fechaVencimiento, stock, 'Disponible', fechaIngreso) EN DB_FARMASIL.TBL_LOTES**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR LOT-TBL-LOTES**<br>&nbsp;&nbsp;**MOSTRAR "Remesa registrada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR (id_producto, numero_lote) = UNICO EN DB_FARMASIL.TBL_LOTES**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.estado_lote = 'Disponible'**<br>&nbsp;&nbsp;**VERIFICAR REMESA DISPONIBLE EN VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;**VERIFICAR STOCK_CONSOLIDADO(producto) = INCREMENTADO EN stock**<br>&nbsp;&nbsp;**VERIFICAR LOT-TBL-LOTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-LOT-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Especificación nueva, derivada de la incorporación de **TBL_LOTES** al Diccionario de Datos v04.00.<br>La validación de unicidad sobre la combinación de medicamento y número de lote es la que sostiene el rastreo sanitario: si la misma remesa pudiera registrarse dos veces, el sistema no podría responder inequívocamente qué se vendió de un lote retirado por la DIGEMID.<br>Queda Pendiente porque el mockup **ART-MKP-LOT-0001** aún no ha sido diseñado. |

| Código especificación | ESP-0042 |
| --- | --- |
| Nombre | Consulta de remesas |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0042 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_LOTES <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-LOT-0002**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**producto = LOT-CMB-PRODUCTO**<br>&nbsp;&nbsp;**numeroLote = LOT-TXT-NUMERO-LOTE**<br>&nbsp;&nbsp;**estado = LOT-CMB-ESTADO-LOTE**<br>&nbsp;&nbsp;**PRESIONAR LOT-BTN-LEER-LOTE**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_LOTES UNIENDO DB_FARMASIL.TBL_PRODUCTOS**<br>&nbsp;&nbsp;**APLICAR FILTRO id_producto = producto**<br>&nbsp;&nbsp;**APLICAR FILTRO numero_lote = numeroLote**<br>&nbsp;&nbsp;**APLICAR FILTRO estado_lote = estado**<br>&nbsp;&nbsp;**ORDENAR POR fecha_vencimiento ASCENDENTE**<br>&nbsp;&nbsp;**CARGAR LOT-TBL-LOTES CON (nombre, numero_lote, fecha_vencimiento, stock_actual, estado_lote)**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR LOT-TBL-LOTES = ORDENADA POR fecha_vencimiento ASCENDENTE**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-LOT-0002 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Es una de las operaciones disponibles para ambos roles: la técnica necesita saber qué remesa tiene disponible y cuándo vence antes de despachar.<br>El orden ascendente por vencimiento es la contraparte visual del criterio FEFO que aplica ESP-0001.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-LOT-0002**. |

| Código especificación | ESP-0043 |
| --- | --- |
| Nombre | Actualización de remesa |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0043 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_LOTES <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-LOT-0003**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR lote EN LOT-TBL-LOTES**<br>&nbsp;&nbsp;**PRESIONAR LOT-BTN-ACTUALIZAR-LOTE**<br>&nbsp;&nbsp;**CARGAR ART-MKP-LOT-0003 CON INFORMACION VIGENTE**<br>&nbsp;&nbsp;**MOSTRAR lote.id_producto EN LOT-CMB-PRODUCTO = SOLO_LECTURA**<br>&nbsp;&nbsp;**numeroLote = LOT-TXT-NUMERO-LOTE**<br>&nbsp;&nbsp;**fechaVencimiento = LOT-FEC-FECHA-VENCIMIENTO**<br>&nbsp;&nbsp;**stock = LOT-NUM-STOCK-LOTE**<br>&nbsp;&nbsp;**estado = LOT-CMB-ESTADO-LOTE**<br>&nbsp;&nbsp;**PRESIONAR LOT-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_LOTES, id_producto = lote.id_producto Y numero_lote = numeroLote Y id_lote <> lote)**<br>&nbsp;&nbsp;**VALIDAR stock = ENTERO >= 0**<br>&nbsp;&nbsp;**comprometido = SUMA(DB_FARMASIL.TBL_DETALLE_DEVOLUCION.cantidad_devolver DONDE id_lote = lote Y orden.estado_orden = 'Pendiente')**<br>&nbsp;&nbsp;**SI stock < comprometido ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Las existencias no pueden ser inferiores a las unidades comprometidas en devoluciones pendientes"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES DONDE id_lote = lote**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR LOT-TBL-LOTES**<br>&nbsp;&nbsp;**MOSTRAR "Remesa actualizada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.id_lote = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.id_producto = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR REFERENCIAS(TBL_DETALLE_VENTAS, TBL_DETALLE_DEVOLUCION) = INTACTAS**<br>&nbsp;&nbsp;**SI estado <> 'Disponible' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR REMESA NO DISPONIBLE EN VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**VERIFICAR LOT-TBL-LOTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-LOT-0003 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | El medicamento asociado se declara de solo lectura por la misma razón que el identificador: las ventas ya registradas apuntan a esta remesa, y reasignarla a otro medicamento reescribiría retroactivamente qué se vendió.<br>La verificación contra las unidades comprometidas evita que una corrección de inventario deje una orden de devolución reclamando existencias que ya no hay.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-LOT-0003**. |

| Código especificación | ESP-0044 |
| --- | --- |
| Nombre | Baja de remesa |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0044 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_LOTES <> VACIA**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_DETALLE_DEVOLUCION, id_lote = lote Y orden.estado_orden = 'Pendiente')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-LOT-0004**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR lote EN LOT-TBL-LOTES**<br>&nbsp;&nbsp;**PRESIONAR LOT-BTN-ELIMINAR-LOTE**<br>&nbsp;&nbsp;**SI EXISTE(DB_FARMASIL.TBL_DETALLE_DEVOLUCION, id_lote = lote Y orden.estado_orden = 'Pendiente') ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La remesa está comprometida en una orden de devolución pendiente"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**MOSTRAR LOT-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR LOT-TXT-MSJ = "¿Está seguro de dar de baja esta remesa? El histórico de ventas que la involucra se conservará."**<br>&nbsp;&nbsp;**tieneMovimientos = EXISTE(DB_FARMASIL.TBL_DETALLE_VENTAS, id_lote = lote) O EXISTE(DB_FARMASIL.TBL_DETALLE_DEVOLUCION, id_lote = lote)**<br>&nbsp;&nbsp;**SI PRESIONAR LOT-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI tieneMovimientos = FALSO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_LOTES DONDE id_lote = lote**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES SET estado_lote = 'Agotado', stock_actual = 0 DONDE id_lote = lote**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Remesa dada de baja correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR LOT-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR LOT-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR LOT-TBL-LOTES**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR BAJA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR REMESA_CON_MOVIMIENTOS.estado_lote = 'Agotado'**<br>&nbsp;&nbsp;**VERIFICAR TRAZABILIDAD DE VENTAS POR LOTE = CONSERVADA**<br>&nbsp;&nbsp;**VERIFICAR REMESA NO DISPONIBLE EN VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;**VERIFICAR STOCK_CONSOLIDADO(producto) = REDUCIDO**<br>&nbsp;&nbsp;**VERIFICAR INTEGRIDAD_REFERENCIAL = SIN_VIOLACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-LOT-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La baja lógica es obligatoria cuando la remesa registró ventas: eliminarla físicamente destruiría precisamente el rastreo sanitario que motivó la creación de este módulo.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-LOT-0004**. |



> **Documento consolidado.** Reúne las 40 especificaciones del catálogo en un solo archivo, reemplazando a `Especificaciones.md`, `Especificaciones_Modulos_1_3_5_7.md` y `Especificaciones_Modulos_2_4_6_8.md`.
>
> Cada especificación traduce a pseudocódigo la ilación correspondiente del archivo `02-ilaciones.md`, y opera sobre el esquema definido en el Diccionario de Datos v03.00 con los componentes de interfaz de la Guía de Estilo de Nomenclatura v03.00.
>
> **Documento completo.** Contiene las 44 especificaciones, ESP-0001 a ESP-0044, correspondientes a los once módulos del catálogo.
>
> **Ampliación del 05/09/2026 (lotes).** Con la incorporación de `TBL_LOTES` al Diccionario de Datos v04.00 se agregaron ESP-0041 a ESP-0044 y se ajustaron las dieciséis especificaciones de los módulos 1, 2, 4 y 7. Los cambios de fondo: el detalle de venta registra `id_lote` y el descuento de stock opera sobre la remesa; la selección de remesa sigue el criterio FEFO; el bloqueo por vencimiento pasa de `estado_producto` a `estado_lote`; y las líneas de devolución identifican la remesa que el proveedor exige.

## Convenciones aplicadas

- Componentes de mockup: guion `-` (`VEN-BTN-CREAR-VENTA`). Tablas y campos de base de datos: guion bajo `_`, con punto separador (`DB_FARMASIL.TBL_PRODUCTOS`).
- Pseudocódigo indentado por nivel de anidamiento: `INICIO`/`FIN`, `INICIAR TRANSACCION`/`CONFIRMAR TRANSACCION`, `SI`/`FIN SI`, `PARA CADA`/`FIN PARA`.
- Toda operación que escriba en más de una tabla se ejecuta dentro de una transacción, para que no queden cabeceras sin detalle ni stock descontado sin venta.
- El campo Fuente hereda la fuente de la ilación de la que deriva la especificación. En la versión anterior las dieciséis decían `Ninguno`, lo que rompía la cadena de trazabilidad hacia la fuente original.
- El campo Estado replica el de su ilación: `Pendiente` cuando la operación depende de un componente de mockup por diseñar o de una decisión externa, `Concluido` en caso contrario.