# Especificaciones de Requisitos — FARMASIL


# Módulo 1 — Gestión de Ventas (EDU-0001)

| Código especificación | ESP-0001 |
| --- | --- |
| Nombre | Creación del registro de venta |
| Versión | 05.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código ilación | ILA-0001 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_LOTES, ES_VENDIBLE(lote, FECHA_SISTEMA))**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_METODOS_PAGO, estado = 'Activo')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-VEN-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**fechaHora = FECHA_HORA_SISTEMA**<br>&nbsp;&nbsp;**MOSTRAR fechaHora EN VEN-LBL-FECHA-VENTA**<br>&nbsp;&nbsp;**detalle = LISTA_VACIA**<br>&nbsp;&nbsp;**REPETIR**<br>&nbsp;&nbsp;&nbsp;&nbsp;**producto = VEN-CMB-PRODUCTO-VENTA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**lote = SUGERIR_FEFO(DB_FARMASIL.TBL_LOTES, id_producto = producto Y ES_VENDIBLE(lote, fechaHora))**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR lote EN VEN-CMB-LOTE-VENTA, QUE LISTA SOLO LAS REMESAS CON ES_VENDIBLE(lote, fechaHora)**<br>&nbsp;&nbsp;&nbsp;&nbsp;**lote = VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**cantidad = VEN-NUM-CANTIDAD-VENTA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**PRESIONAR VEN-BTN-AGREGAR-PRODUCTO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR cantidad = ENTERO > 0**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI lote YA FIGURA EN detalle ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**cantidadTotal = cantidad + LINEA_DE(detalle, lote).cantidad**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**cantidadTotal = cantidad**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR cantidadTotal <= lote.stock_actual**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI producto.estado_producto = 'Descontinuado' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "El medicamento está descontinuado"**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**RECHAZAR LINEA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI lote.fecha_vencimiento <= FECHA(fechaHora) ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La remesa está vencida y no puede venderse"**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**RECHAZAR LINEA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI lote.estado_lote <> 'Disponible' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La remesa tiene una alerta activa y no puede venderse"**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**RECHAZAR LINEA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**restricciones = CONSULTAR DB_FARMASIL.TBL_RESTRICCIONES_VENTA DONDE id_producto = producto Y estado_alerta = 'Activo'**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI EXISTE(restricciones, tipo_restriccion = 'Bloqueante') ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA restriccion EN restricciones CON tipo_restriccion = 'Bloqueante' HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR restriccion.motivo_advertencia**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**RECHAZAR LINEA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA restriccion EN restricciones CON tipo_restriccion = 'Informativa' HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR restriccion.motivo_advertencia**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**precioUnitario = producto.precio_venta**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI lote YA FIGURA EN detalle ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**LINEA_DE(detalle, lote).cantidad = cantidadTotal**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**AGREGAR (lote, cantidad, precioUnitario) A detalle**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR VEN-TBL-DETALLE-VENTA**<br>&nbsp;&nbsp;**HASTA QUE USUARIO NO AGREGUE MAS PRODUCTOS**<br>&nbsp;&nbsp;**SI USUARIO PRESIONA VEN-BTN-QUITAR-PRODUCTO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**QUITAR LINEA_SELECCIONADA DE detalle**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR VEN-TBL-DETALLE-VENTA**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**montoTotal = SUMA(detalle.cantidad \* detalle.precioUnitario)**<br>&nbsp;&nbsp;**MOSTRAR montoTotal EN VEN-LBL-MONTO-TOTAL**<br>&nbsp;&nbsp;**metodoPago = VEN-CMB-METODO-PAGO**<br>&nbsp;&nbsp;**PRESIONAR VEN-BTN-CREAR-VENTA**<br>&nbsp;&nbsp;**VALIDAR detalle <> VACIA**<br>&nbsp;&nbsp;**VALIDAR metodoPago**<br>&nbsp;&nbsp;**PARA CADA linea EN detalle HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI NO ES_VENDIBLE(linea.lote, FECHA_HORA_SISTEMA) O linea.cantidad > linea.lote.stock_actual ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La remesa " + linea.lote.numero_lote + " ya no está disponible para la venta. Retire la línea"**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (fechaHora, montoTotal, metodoPago, SESION_USUARIO.id_usuario) EN DB_FARMASIL.TBL_REGISTRO_VENTAS**<br>&nbsp;&nbsp;&nbsp;&nbsp;**idVenta = ULTIMO_ID_GENERADO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA linea EN detalle HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (idVenta, linea.lote, linea.cantidad, linea.precioUnitario) EN DB_FARMASIL.TBL_DETALLE_VENTAS**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR_STOCK_LOTE(linea.lote, stock_actual - linea.cantidad)**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR VEN-TBL-REGISTRO-VENTAS**<br>&nbsp;&nbsp;**MOSTRAR "Venta registrada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REGISTRO_VENTAS.monto_total = SUMA(DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = idVenta)**<br>&nbsp;&nbsp;**VERIFICAR CONTAR(DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = idVenta) = CONTAR(detalle)**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.stock_actual = DESCONTADO**<br>&nbsp;&nbsp;**VERIFICAR TODA REMESA VENDIDA CON stock_actual = 0 TIENE estado_lote = 'Agotado'**<br>&nbsp;&nbsp;**VERIFICAR NINGUNA LINEA TIENE UNA REMESA VENCIDA NI UN MEDICAMENTO DESCONTINUADO**<br>&nbsp;&nbsp;**VERIFICAR NO EXISTEN DOS LINEAS DE LA MISMA REMESA EN LA VENTA**<br>&nbsp;&nbsp;**VERIFICAR CADA LINEA DE DB_FARMASIL.TBL_DETALLE_VENTAS TIENE id_lote <> NULO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REGISTRO_VENTAS.id_usuario = SESION_USUARIO.id_usuario**<br>&nbsp;&nbsp;**VERIFICAR VEN-TBL-REGISTRO-VENTAS = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-VEN-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Reescrita para la venta de varios productos en una misma transacción. La versión anterior registraba un único producto directamente en la cabecera, sin usar **TBL_DETALLE_VENTAS**, y no descontaba stock ni verificaba restricciones sanitarias.<br>Depende de los componentes **VEN-BTN-AGREGAR-PRODUCTO**, **VEN-BTN-QUITAR-PRODUCTO**, **VEN-TBL-DETALLE-VENTA** y **VEN-LBL-MONTO-TOTAL**, aún por incorporar al mockup.<br>**Versión 05.00:** (auditorías de la base de datos, AUD-gpt-001, AUD-claude-005, 015 y 021; DEC-033, DEC-034, DEC-036). El predicado `ES_VENDIBLE(lote, fecha)` es verdadero cuando el medicamento está en 'Disponible', la remesa está en 'Disponible', `stock_actual` > 0 y `fecha_vencimiento` es posterior a la fecha de la venta. Se aplica al sugerir la remesa FEFO, al llenar **VEN-CMB-LOTE-VENTA**, al agregar una línea y al confirmar la venta; los mensajes distinguen la remesa vencida del medicamento descontinuado. Una remesa solo aparece una vez por venta: si se agrega de nuevo, se acumula su cantidad y la validación de stock usa la cantidad acumulada. Se evalúan todas las restricciones activas del medicamento: si alguna es 'Bloqueante' se rechaza la línea, y las 'Informativa' se muestran todas. El descuento de stock usa `ACTUALIZAR_STOCK_LOTE` (definida en ESP-0043), que fija stock y estado en una sola sentencia. |

| Código especificación | ESP-0002 |
| --- | --- |
| Nombre | Consulta del registro de venta |
| Versión | 04.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código ilación | ILA-0002 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REGISTRO_VENTAS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-VEN-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**PRESIONAR VEN-BTN-LEER-VENTA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-VEN-0002**<br>&nbsp;&nbsp;**fecha = VEN-FEC-FECHA-VENTA**<br>&nbsp;&nbsp;**producto = VEN-CMB-PRODUCTO-VENTA**<br>&nbsp;&nbsp;**PRESIONAR VEN-BTN-LEER-VENTA**<br>&nbsp;&nbsp;**VALIDAR fecha**<br>&nbsp;&nbsp;**VALIDAR producto**<br>&nbsp;&nbsp;**SI producto <> VACIO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_REGISTRO_VENTAS UNIENDO DB_FARMASIL.TBL_DETALLE_VENTAS UNIENDO DB_FARMASIL.TBL_LOTES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**APLICAR FILTRO id_producto = producto**<br>&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_REGISTRO_VENTAS**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**APLICAR FILTRO fecha_hora >= fecha Y fecha_hora < fecha + 1 DIA**<br>&nbsp;&nbsp;**CARGAR VEN-TBL-REGISTRO-VENTAS**<br>&nbsp;&nbsp;**SI USUARIO SELECCIONA venta EN VEN-TBL-REGISTRO-VENTAS ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = venta**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CARGAR VEN-TBL-DETALLE-VENTA**<br>&nbsp;&nbsp;**FIN SI**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR VEN-TBL-REGISTRO-VENTAS = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR VEN-TBL-DETALLE-VENTA = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL = SIN_MODIFICACION**<br>&nbsp;&nbsp;**MOSTRAR RESULTADOS_CONSULTA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-VEN-0001, ART-MKP-VEN-0002 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | En la versión anterior el filtro por producto se aplicaba sobre **TBL_REGISTRO_VENTAS**, tabla que no contiene el producto: la consulta no habría devuelto nada. Ahora se resuelve contra la tabla de detalle.<br>Se agregó la visualización del detalle de la venta seleccionada.<br>Esta especificación corregía además un error estructural de la versión anterior de `Especificaciones.md`, donde la fila de Precondición había sido reemplazada por una segunda fila de Nombre.<br>**Versión 04.00:** (AUD-claude-017, AUD-gpt-012; DEC-037) el filtro por día es un intervalo semiabierto: `fecha_hora` es `DATETIME2` y comparar con `=` solo habría coincidido con la medianoche. |

| Código especificación | ESP-0003 |
| --- | --- |
| Nombre | Actualización del registro de venta |
| Versión | 04.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código ilación | ILA-0003 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REGISTRO_VENTAS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, id_venta = venta Y estado_documento = 'Emitido')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-VEN-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR venta EN VEN-TBL-REGISTRO-VENTAS**<br>&nbsp;&nbsp;**PRESIONAR VEN-BTN-ACTUALIZAR-VENTA**<br>&nbsp;&nbsp;**SI EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, id_venta = venta Y estado_documento = 'Emitido') ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La venta tiene comprobante emitido. Anule el comprobante antes de modificarla"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CARGAR ART-MKP-VEN-0003**<br>&nbsp;&nbsp;**detalleAnterior = CONSULTAR DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = venta**<br>&nbsp;&nbsp;**detalleNuevo = detalleAnterior**<br>&nbsp;&nbsp;**CARGAR VEN-TBL-DETALLE-VENTA CON detalleNuevo**<br>&nbsp;&nbsp;**metodoPago = VEN-CMB-METODO-PAGO**<br>&nbsp;&nbsp;**SI USUARIO PRESIONA VEN-BTN-AGREGAR-PRODUCTO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**AGREGAR_LINEA(detalleNuevo, VEN-CMB-PRODUCTO-VENTA, VEN-NUM-CANTIDAD-VENTA) CON LOS PASOS DE AGREGAR LINEA DE ESP-0001**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**SI USUARIO PRESIONA VEN-BTN-QUITAR-PRODUCTO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**QUITAR LINEA_SELECCIONADA DE detalleNuevo**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**montoTotal = SUMA(detalleNuevo.cantidad \* detalleNuevo.precioUnitario)**<br>&nbsp;&nbsp;**MOSTRAR montoTotal EN VEN-LBL-MONTO-TOTAL**<br>&nbsp;&nbsp;**PRESIONAR VEN-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR detalleNuevo <> VACIA**<br>&nbsp;&nbsp;**VALIDAR metodoPago**<br>&nbsp;&nbsp;**PARA CADA linea EN detalleNuevo HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;**diferencia = linea.cantidad - CANTIDAD_ANTERIOR(linea.lote, detalleAnterior)**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR diferencia <= linea.lote.stock_actual**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI diferencia > 0 ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR ES_VENDIBLE(linea.lote, FECHA_HORA_SISTEMA)**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_REGISTRO_VENTAS SET monto_total = montoTotal, id_metodo_pago = metodoPago**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SINCRONIZAR DB_FARMASIL.TBL_DETALLE_VENTAS CON detalleNuevo**<br>&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA lote EN (LOTES DE detalleAnterior UNIDOS A LOTES DE detalleNuevo) HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**diferencia = CANTIDAD_NUEVA(lote, detalleNuevo) - CANTIDAD_ANTERIOR(lote, detalleAnterior)**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR_STOCK_LOTE(lote, stock_actual - diferencia)**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR VEN-TBL-REGISTRO-VENTAS**<br>&nbsp;&nbsp;**MOSTRAR "Venta actualizada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REGISTRO_VENTAS.monto_total = SUMA(DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = venta)**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.stock_actual = AJUSTADO_POR_DIFERENCIA**<br>&nbsp;&nbsp;**VERIFICAR TODA REMESA AFECTADA CON stock_actual = 0 TIENE estado_lote = 'Agotado'**<br>&nbsp;&nbsp;**VERIFICAR LAS LINEAS EXISTENTES CONSERVAN SU precio_unitario**<br>&nbsp;&nbsp;**VERIFICAR NO EXISTEN DOS LINEAS DE LA MISMA REMESA EN LA VENTA**<br>&nbsp;&nbsp;**VERIFICAR VENTA_CON_COMPROBANTE_EMITIDO = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR VEN-TBL-REGISTRO-VENTAS = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-VEN-0001, ART-MKP-VEN-0003 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Se incorporó el bloqueo de las ventas con comprobante tributario emitido y el ajuste diferencial de stock, que es lo que evita que actualizar una venta descuadre el inventario.<br>La versión anterior modificaba únicamente la cabecera, dejando el detalle intacto y el monto total desincronizado.<br>**Versión 04.00:** (AUD-claude-005, 015; AUD-gpt-018; DEC-033, DEC-034, DEC-036) las líneas nuevas reutilizan el paso de agregar línea de ESP-0001 (FEFO, predicado de vendibilidad, restricciones activas y `precio_venta` vigente); las existentes conservan su `precio_unitario`. Si se agrega una remesa que ya figura, se acumula su cantidad. El ajuste de stock recorre las remesas de ambos detalles, de modo que las líneas quitadas restituyen su stock, y cada remesa afectada actualiza su estado con `ACTUALIZAR_STOCK_LOTE`. |

| Código especificación | ESP-0004 |
| --- | --- |
| Nombre | Eliminación del registro de venta |
| Versión | 05.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código ilación | ILA-0004 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REGISTRO_VENTAS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, id_venta = venta)**<br>&nbsp;&nbsp;**VALIDAR VEN-TBL-REGISTRO-VENTAS = HABILITADA**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR venta EN VEN-TBL-REGISTRO-VENTAS**<br>&nbsp;&nbsp;**PRESIONAR VEN-BTN-ELIMINAR-VENTA**<br>&nbsp;&nbsp;**SI EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, id_venta = venta) ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La venta tiene un comprobante tributario asociado y no puede eliminarse. Si requiere corrección, anule el comprobante y actualice la venta"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CARGAR ART-MKP-VEN-0004**<br>&nbsp;&nbsp;**MOSTRAR VEN-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR VEN-TXT-MSJ = "¿Está seguro de eliminar este registro de venta? El stock de los productos será restituido."**<br>&nbsp;&nbsp;**SI PRESIONAR VEN-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**detalle = CONSULTAR DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = venta**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA linea EN detalle HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR_STOCK_LOTE(linea.lote, stock_actual + linea.cantidad)**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_DETALLE_VENTAS DONDE id_venta = venta**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_REGISTRO_VENTAS DONDE id_venta = venta**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Venta eliminada correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR VEN-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR VEN-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR VEN-TBL-REGISTRO-VENTAS**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ELIMINADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR NO EXISTE(DB_FARMASIL.TBL_DETALLE_VENTAS, id_venta = venta)**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.stock_actual = RESTITUIDO**<br>&nbsp;&nbsp;**VERIFICAR TODA REMESA 'Agotado' NO VENCIDA QUE RECUPERA EXISTENCIAS QUEDA 'Disponible'**<br>&nbsp;&nbsp;**VERIFICAR VENTA_CON_COMPROBANTE('Emitido' O 'Anulado') = SIN_ELIMINAR**<br>&nbsp;&nbsp;**VERIFICAR VEN-TBL-REGISTRO-VENTAS = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-VEN-0001, ART-MKP-VEN-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Se incorporó la eliminación del detalle por la operación (no en cascada del motor), la restitución del stock y el bloqueo cuando existe comprobante emitido.<br>La versión anterior eliminaba solo la cabecera, lo que habría dejado líneas huérfanas en **TBL_DETALLE_VENTAS** y una violación de integridad referencial. También quedaban en el archivo original los textos de plantilla sin completar en los campos Importancia, Estado y Comentario.<br>**Versión 04.00:** la eliminación se bloquea ante cualquier comprobante asociado, no solo ante uno 'Emitido'. Un comprobante 'Anulado' es un documento tributario que debe conservarse junto con la venta que lo originó, y su clave foránea impediría además el borrado. La corrección de la venta se hace con ESP-0003, que la admite cuando el comprobante está anulado.<br>**Versión 05.00:** (AUD-claude-001, 007; AUD-gpt-002; DEC-034) al restituir el stock se actualiza el estado de cada remesa con `ACTUALIZAR_STOCK_LOTE`: una remesa 'Agotado' no vencida que recupera existencias vuelve a 'Disponible'. El detalle lo elimina la operación y no el motor: las claves foráneas son `ON DELETE NO ACTION`. |

---

# Módulo 2 — Gestión de Inventario (EDU-0002)

| Código especificación | ESP-0005 |
| --- | --- |
| Nombre | Creación del producto de inventario |
| Versión | 04.00 |
| Fecha | 11/11/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0005 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_PROVEEDORES, estado = 'Activo')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-INV-0001**<br>&nbsp;&nbsp;**VALIDAR INV-BTN-CREAR-PRODUCTO = HABILITADO**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**nombre = INV-TXT-NOMBRE-PRODUCTO**<br>&nbsp;&nbsp;**accionTerapeutica = INV-TXT-ACCION-TERAPEUTICA**<br>&nbsp;&nbsp;**precio = INV-NUM-PRECIO-PRODUCTO**<br>&nbsp;&nbsp;**proveedor = INV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**stockMinimo = INV-NUM-STOCK-MINIMO**<br>&nbsp;&nbsp;**PRESIONAR INV-BTN-CREAR-PRODUCTO**<br>&nbsp;&nbsp;**VALIDAR nombre <> VACIO**<br>&nbsp;&nbsp;**VALIDAR accionTerapeutica <> VACIO**<br>&nbsp;&nbsp;**VALIDAR precio = DECIMAL >= 0**<br>&nbsp;&nbsp;**VALIDAR proveedor**<br>&nbsp;&nbsp;**VALIDAR stockMinimo = ENTERO >= 0**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (nombre, accionTerapeutica, precio, 'Disponible', proveedor, stockMinimo) EN DB_FARMASIL.TBL_PRODUCTOS**<br>&nbsp;&nbsp;&nbsp;&nbsp;**idProducto = ULTIMO_ID_GENERADO**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**MOSTRAR idProducto EN INV-LBL-ID-PRODUCTO**<br>&nbsp;&nbsp;**ACTUALIZAR INV-TBL-PRODUCTOS**<br>&nbsp;&nbsp;**MOSTRAR "Producto registrado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PRODUCTOS.estado_producto = 'Disponible'**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PRODUCTOS.id_proveedor <> NULO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PRODUCTOS.stock_minimo >= 0**<br>&nbsp;&nbsp;**VERIFICAR INV-TBL-PRODUCTOS = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR PRODUCTO SIN REMESAS NO SE OFRECE EN VEN-CMB-PRODUCTO-VENTA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-INV-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | El identificador pasó de captura manual a generación automática, coherente con la clave primaria autoincremental del diccionario, y se muestra en **INV-LBL-ID-PRODUCTO**.<br>Se incorporó `id_proveedor`, campo obligatorio que la versión anterior no capturaba.<br>Con la incorporación de **TBL_LOTES** en el Diccionario de Datos v04.00, el número de lote, la fecha de vencimiento y las existencias se registran como remesas en ESP-0041, y esta especificación quedó reducida a los datos de catálogo del medicamento.<br>**Versión 03.00:** se incorpora la captura de `stock_minimo`, campo nuevo del Diccionario de Datos v05.00 (ver ILA-0005 v09.00).<br>**Versión 04.00:** (DEC-042, C4) `INV-CMB-PROVEEDOR` e `INV-NUM-STOCK-MINIMO` se mantienen como campos obligatorios de la operación aunque no estén representados de forma completa en el mockup aprobado; la especificación prevalece y no se modifica el PNG. |

| Código especificación | ESP-0006 |
| --- | --- |
| Nombre | Consulta del producto de inventario |
| Versión | 05.00 |
| Fecha | 11/11/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0006 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-INV-0001**<br>&nbsp;&nbsp;**VALIDAR INV-BTN-LEER-PRODUCTO = HABILITADO**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**busqueda = INV-SRH-PRODUCTO**<br>&nbsp;&nbsp;**proveedor = INV-CMB-FILTRO-PROVEEDOR**<br>&nbsp;&nbsp;**accionTerapeutica = INV-CMB-FILTRO-ACCION-TERAPEUTICA**<br>&nbsp;&nbsp;**PRESIONAR INV-BTN-LEER-PRODUCTO**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_PRODUCTOS UNIENDO A LA IZQUIERDA DB_FARMASIL.VW_STOCK_FISICO POR id_producto**<br>&nbsp;&nbsp;**SI busqueda <> VACIO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**APLICAR FILTRO id_producto = busqueda O nombre CONTIENE busqueda**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**SI proveedor <> VACIO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**APLICAR FILTRO id_proveedor = proveedor**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**SI accionTerapeutica <> VACIO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**APLICAR FILTRO accion_terapeutica = accionTerapeutica**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CARGAR INV-CRD-PRODUCTO EN INV-PNL-CATALOGO Y INV-TBL-PRODUCTOS CON (id_producto, nombre, accion_terapeutica, precio_venta, estado_producto, COALESCE(stock_fisico, 0))**<br>&nbsp;&nbsp;**SI SELECCIONAR producto POR INV-BTN-SELECCIONAR-PRODUCTO EN INV-CRD-PRODUCTO O EN UNA FILA DE INV-TBL-PRODUCTOS ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CARGAR FICHA CON producto**<br>&nbsp;&nbsp;**FIN SI**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR INV-PNL-CATALOGO = ACTUALIZADO**<br>&nbsp;&nbsp;**VERIFICAR INV-TBL-PRODUCTOS = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PRODUCTOS = SIN_MODIFICACION**<br>&nbsp;&nbsp;**MOSTRAR RESULTADOS_CONSULTA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-INV-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se explicitaron las columnas devueltas por la consulta, incorporando stock, lote y estado, que son los datos que el personal necesita durante la atención y que la versión anterior no declaraba. El detalle por remesa se consulta en el módulo 11 mediante ESP-0042.<br>**Versión 03.00:** (AUD-claude-016, AUD-gpt-019; DEC-040, PEN-024) la columna de existencias es el stock físico: la suma de `stock_actual` de todas las remesas del medicamento, incluidas las bloqueadas, agotadas o vencidas. El reabastecimiento usa en cambio el stock vendible (RNF-0010). Provisional hasta que la dueña confirme qué debe mostrar Inventario.<br>**Versión 04.00:** (DEC-042, DEC-058, PEN-010, PEN-027) la consulta se integra en **ART-MKP-INV-0001**. La caja de búsqueda permite localizar por identificador o nombre; los filtros visibles de proveedor y categoría se incorporan a la consulta, interpretando provisionalmente la categoría mediante `accion_terapeutica`. El stock físico se obtiene de `VW_STOCK_FISICO` mediante unión izquierda y un medicamento sin remesas muestra 0. El visor 3D y las imágenes del catálogo quedan fuera del alcance funcional.<br>**Versión 05.00:** (issue #158, que completa la adecuación del #72; DEC-061, DEC-062) el buscador de texto pasa de **INV-TXT-BUSQUEDA-PRODUCTO** a **INV-SRH-PRODUCTO**. El resultado se carga en las tarjetas **INV-CRD-PRODUCTO** de **INV-PNL-CATALOGO** y en **INV-TBL-PRODUCTOS**; la selección, desde la tarjeta con **INV-BTN-SELECCIONAR-PRODUCTO** o desde una fila, carga la ficha. Se restaura la nota de la versión original que la versión 04.00 había resumido. |

| Código especificación | ESP-0007 |
| --- | --- |
| Nombre | Actualización del producto de inventario |
| Versión | 05.00 |
| Fecha | 11/11/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0007 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-INV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR producto POR INV-BTN-SELECCIONAR-PRODUCTO EN INV-CRD-PRODUCTO O EN UNA FILA DE INV-TBL-PRODUCTOS**<br>&nbsp;&nbsp;**PRESIONAR INV-BTN-ACTUALIZAR-PRODUCTO**<br>&nbsp;&nbsp;**CARGAR ART-MKP-INV-0001 CON INFORMACION VIGENTE**<br>&nbsp;&nbsp;**MOSTRAR producto.id_producto EN INV-LBL-ID-PRODUCTO**<br>&nbsp;&nbsp;**nombre = INV-TXT-NOMBRE-PRODUCTO**<br>&nbsp;&nbsp;**accionTerapeutica = INV-TXT-ACCION-TERAPEUTICA**<br>&nbsp;&nbsp;**precio = INV-NUM-PRECIO-PRODUCTO**<br>&nbsp;&nbsp;**proveedor = INV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**stockMinimo = INV-NUM-STOCK-MINIMO**<br>&nbsp;&nbsp;**PRESIONAR INV-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR nombre <> VACIO**<br>&nbsp;&nbsp;**VALIDAR precio = DECIMAL >= 0**<br>&nbsp;&nbsp;**VALIDAR stockMinimo = ENTERO >= 0**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_PRODUCTOS DONDE id_producto = producto.id_producto**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR INV-PNL-CATALOGO Y INV-TBL-PRODUCTOS**<br>&nbsp;&nbsp;**MOSTRAR "Producto actualizado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PRODUCTOS.id_producto = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR REFERENCIAS(TBL_LOTES, TBL_RESTRICCIONES_VENTA) = INTACTAS**<br>&nbsp;&nbsp;**VERIFICAR INV-PNL-CATALOGO = ACTUALIZADO**<br>&nbsp;&nbsp;**VERIFICAR INV-TBL-PRODUCTOS = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-INV-0001 |
| Importancia | Media |
| Estado | Concluido |
| Comentario | Se declaró de solo lectura el identificador, que es clave foránea en tres tablas del modelo, y se alinearon los campos editables con los incorporados en ESP-0005.<br>**Versión 03.00:** se agrega `stockMinimo` (**INV-NUM-STOCK-MINIMO**) a los campos editables, alineado con ESP-0005 y con ILA-0007 v09.00.<br>**Versión 04.00:** (DEC-042, DEC-046) la actualización se realiza en la pantalla única **ART-MKP-INV-0001**. `INV-BTN-ACTUALIZAR-PRODUCTO` inicia la edición y `INV-BTN-CONFIRMAR-ACTUALIZACION`, con el rótulo «Guardar cambios», confirma la modificación. Los campos exigidos por la especificación se mantienen aunque el mockup no los muestre completamente (C4).<br>**Versión 05.00:** (issue #158, que completa la adecuación del #72; DEC-062) el producto a actualizar se selecciona en una tarjeta **INV-CRD-PRODUCTO** con **INV-BTN-SELECCIONAR-PRODUCTO** o en una fila de **INV-TBL-PRODUCTOS**, y el resultado se refleja en **INV-PNL-CATALOGO** y en la tabla. |

| Código especificación | ESP-0008 |
| --- | --- |
| Nombre | Eliminación del producto de inventario |
| Versión | 06.00 |
| Fecha | 11/11/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0008 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-INV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR producto EN INV-TBL-PRODUCTOS**<br>&nbsp;&nbsp;**PRESIONAR INV-BTN-ELIMINAR-PRODUCTO**<br>&nbsp;&nbsp;**MOSTRAR INV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR INV-TXT-MSJ = "¿Está seguro de dar de baja este producto?"**<br>&nbsp;&nbsp;**SI PRESIONAR INV-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**resultado = INTENTAR ELIMINAR DB_FARMASIL.TBL_PRODUCTOS DONDE id_producto = producto**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI resultado = RECHAZADO_POR_REFERENCIAS ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_PRODUCTOS.estado_producto = 'Descontinuado'**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Producto dado de baja correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR INV-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR INV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR INV-TBL-PRODUCTOS**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR BAJA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR PRODUCTO_CON_MOVIMIENTOS.estado_producto = 'Descontinuado'**<br>&nbsp;&nbsp;**VERIFICAR HISTORICO_VENTAS = INTACTO**<br>&nbsp;&nbsp;**VERIFICAR PRODUCTO NO DISPONIBLE EN VEN-CMB-PRODUCTO-VENTA**<br>&nbsp;&nbsp;**VERIFICAR INTEGRIDAD_REFERENCIAL = SIN_VIOLACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-INV-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La versión anterior eliminaba físicamente el producto sin verificar sus referencias en tres tablas del modelo. Ahora la eliminación física solo procede cuando no hay movimientos, y en caso contrario se aplica baja lógica.<br>Depende de la incorporación del valor 'Descontinuado' al dominio de `estado_producto`, ya aplicada en el Diccionario de Datos v03.00.<br>El valor 'Descontinuado' forma parte del dominio de `estado_producto` definido en el Diccionario de Datos.<br>**Versión 03.00:** Baja condicional según DEC-009 y DEC-029 (VP-B). El módulo ya no consulta tablas de otros módulos para decidirlo (RG-011), y la verificación cubre cualquier referencia, incluidas las que la versión anterior omitía (`TBL_LISTA_REABASTECIMIENTO` y `TBL_COTIZACIONES_PROVEEDOR`).<br>**Versión 04.00:** la baja condicional se refiere a DEC-009 y DEC-029 (VP-B) en lugar de describir el mecanismo; DEC-029 corrige el fundamento de DEC-022.<br>**Versión 05.00:** (DEC-042, C1) la baja se ejecuta desde la pantalla única **ART-MKP-INV-0001** y conserva el modal de confirmación identificado por los componentes `INV-MDL-*`.<br>**Versión 06.00:** (issue #158, que completa la adecuación del #72) se restauran las notas de las versiones anteriores que la versión 05.00 había resumido: la dependencia del valor 'Descontinuado' en el dominio de `estado_producto` y las referencias que la versión anterior omitía en la verificación. No cambia el procedimiento. |

---

# Módulo 3 — Gestión de Documentación Tributaria (EDU-0003)

| Código especificación | ESP-0009 |
| --- | --- |
| Nombre | Registro de comprobante tributario |
| Versión | 03.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0002, FUE-0003 |
| Experto | Ninguno |
| Código ilación | ILA-0009 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_REGISTRO_VENTAS SIN COMPROBANTE 'Emitido')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DOC-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**PRESIONAR DOC-BTN-CREAR-COMPROBANTE**<br>&nbsp;&nbsp;**venta = DOC-CMB-CODIGO-VENTA**<br>&nbsp;&nbsp;**montoTotal = CONSULTAR DB_FARMASIL.TBL_REGISTRO_VENTAS.monto_total DONDE id_venta = venta**<br>&nbsp;&nbsp;**MOSTRAR montoTotal EN DOC-LBL-MONTO-TOTAL**<br>&nbsp;&nbsp;**tipoComprobante = DOC-CMB-TIPO-COMPROBANTE**<br>&nbsp;&nbsp;**numeroComprobante = DOC-TXT-NUMERO-COMPROBANTE**<br>&nbsp;&nbsp;**fechaEmision = DOC-FEC-FECHA-EMISION**<br>&nbsp;&nbsp;**PRESIONAR DOC-BTN-CONFIRMAR-SI**<br>&nbsp;&nbsp;**VALIDAR tipoComprobante EN ('Boleta', 'Factura')**<br>&nbsp;&nbsp;**VALIDAR numeroComprobante <> VACIO**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, numero_comprobante = numeroComprobante)**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, id_venta = venta Y estado_documento = 'Emitido')**<br>&nbsp;&nbsp;**VALIDAR fechaEmision**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (numeroComprobante, tipoComprobante, fechaEmision, venta, 'Emitido') EN DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR DOC-TBL-COMPROBANTES**<br>&nbsp;&nbsp;**MOSTRAR "Comprobante registrado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS.estado_documento = 'Emitido'**<br>&nbsp;&nbsp;**VERIFICAR COMPROBANTES 'Emitido' DE venta <= 1**<br>&nbsp;&nbsp;**VERIFICAR VENTA_DOCUMENTADA = BLOQUEADA_PARA_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR DOC-TBL-COMPROBANTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DOC-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La precondición anterior exigía que la tabla de comprobantes ya tuviera entradas para poder crear uno, lo que hacía imposible el primer registro del sistema. Se reemplazó por la existencia de una venta pendiente de documentar.<br>El monto dejó de capturarse manualmente: se recupera de la venta y se muestra en solo lectura, eliminando la posibilidad de que comprobante y venta declaren importes distintos.<br>Depende del campo `numero_comprobante`, incorporado en el Diccionario de Datos v03.00.<br>**Versión 03.00:** (AUD-claude-014, AUD-gpt-017; DEC-017) la precondición exige una venta sin comprobante 'Emitido', y no sin comprobante alguno: tras anular un comprobante se puede emitir otro para la misma venta. La postcondición expresa «a lo sumo un comprobante 'Emitido' por venta», que además hace cumplir el índice único filtrado del Diccionario de Datos v10.00. |

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
| Versión | 03.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | FUE-0002, FUE-0003 |
| Experto | Ninguno |
| Código ilación | ILA-0011 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR comprobante.estado_documento = 'Emitido'**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DOC-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR comprobante EN DOC-TBL-COMPROBANTES**<br>&nbsp;&nbsp;**PRESIONAR DOC-BTN-ACTUALIZAR-COMPROBANTE**<br>&nbsp;&nbsp;**SI comprobante.estado_documento = 'Anulado' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "El comprobante está anulado y no admite modificación"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DOC-0003**<br>&nbsp;&nbsp;**MOSTRAR comprobante.id_venta EN DOC-CMB-CODIGO-VENTA = SOLO_LECTURA**<br>&nbsp;&nbsp;**tipoComprobante = DOC-CMB-TIPO-COMPROBANTE**<br>&nbsp;&nbsp;**numeroComprobante = DOC-TXT-NUMERO-COMPROBANTE**<br>&nbsp;&nbsp;**fechaEmision = DOC-FEC-FECHA-EMISION**<br>&nbsp;&nbsp;**PRESIONAR DOC-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR tipoComprobante EN ('Boleta', 'Factura')**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS, numero_comprobante = numeroComprobante Y id_comprobante <> comprobante)**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS DONDE id_comprobante = comprobante**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR DOC-TBL-COMPROBANTES**<br>&nbsp;&nbsp;**MOSTRAR "Comprobante actualizado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS.id_venta = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR COMPROBANTES 'Emitido' DE venta <= 1**<br>&nbsp;&nbsp;**VERIFICAR DOC-TBL-COMPROBANTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DOC-0001, ART-MKP-DOC-0003 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se enumeraron los campos corregibles, que la versión anterior dejaba como "datos permitidos" sin precisar, y se declaró de solo lectura la venta asociada: reasignar un comprobante a otra venta rompería la relación venta–comprobante del modelo (a lo sumo un comprobante 'Emitido' por venta).<br>**Versión 03.00:** (AUD-claude-014, AUD-gpt-017; DEC-017) se elimina la referencia a la relación uno a uno, que contradecía DEC-017. |

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
| Versión | 04.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0007 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0013 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_ALERTAS_VENCIMIENTO <> VACIA**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS = DISPONIBLE**<br>&nbsp;&nbsp;**CARGAR ART-MKP-ALV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**PRESIONAR ALV-BTN-LEER-ALERTA**<br>&nbsp;&nbsp;**umbral = CONSULTAR DB_FARMASIL.TBL_ALERTAS_VENCIMIENTO.umbral_meses DONDE estado = 'Activo'**<br>&nbsp;&nbsp;**proximosAVencer = CONSULTAR DB_FARMASIL.TBL_LOTES DONDE estado_lote = 'Disponible' Y stock_actual > 0 Y fecha_vencimiento <= FECHA_SISTEMA + umbral**<br>&nbsp;&nbsp;**retirados = CONSULTAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS**<br>&nbsp;&nbsp;**resultado = UNION(proximosAVencer, retirados)**<br>&nbsp;&nbsp;**CARGAR ALV-TBL-ALERTAS-VENCIMIENTO CON resultado**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR ALV-TBL-ALERTAS-VENCIMIENTO = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-ALV-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La generación automática de nuevas alertas la ejecuta un proceso programado del sistema según el umbral configurado, y se documenta en la condición 1 de RNF-0010 (versión 03.00). Lo que esta especificación describe es la consulta del resultado consolidado.<br>**Versión 04.00:** (AUD-claude-031; DEC-040) la consulta cuenta las mismas remesas que el proceso 1 de RNF-0010: en estado 'Disponible' y con existencias. Antes la pantalla y el aviso del proceso de fondo podían dar conteos distintos para la misma regla. |

| Código especificación | ESP-0014 |
| --- | --- |
| Nombre | Registro manual de alerta de producto vencido |
| Versión | 03.00 |
| Fecha | 13/10/2026 |
| Autor de la plantilla | AUT-0007 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0014 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_LOTES <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-ALV-0001**<br>&nbsp;&nbsp;**VALIDAR ALV-BTN-CREAR-ALERTA = HABILITADO**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**lote = ALV-CMB-LOTE-ALERTA**<br>&nbsp;&nbsp;**MOSTRAR lote.fecha_vencimiento EN ALV-LBL-FECHA-VENCIMIENTO**<br>&nbsp;&nbsp;**loteDefectuoso = ALV-CHK-LOTE-DEFECTUOSO**<br>&nbsp;&nbsp;**alertaDigemid = ALV-TXT-ALERTA-DIGEMID**<br>&nbsp;&nbsp;**PRESIONAR ALV-BTN-CREAR-ALERTA**<br>&nbsp;&nbsp;**VALIDAR lote EXISTE EN DB_FARMASIL.TBL_LOTES**<br>&nbsp;&nbsp;**SI VALIDACION = FALSA ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SEÑALAR CAMPO_INVALIDO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**PERMITIR CORRECCION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (lote, lote.producto.nombre, lote.numero_lote, lote.fecha_vencimiento, loteDefectuoso, alertaDigemid, FECHA_SISTEMA) EN DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES.estado_lote = 'Bloqueado por devolucion' DONDE id_lote = lote**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR ALV-TBL-ALERTAS-VENCIMIENTO**<br>&nbsp;&nbsp;**MOSTRAR "Alerta registrada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.estado_lote = 'Bloqueado por devolucion'**<br>&nbsp;&nbsp;**VERIFICAR REMESA NO DISPONIBLE EN VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;**VERIFICAR OTRAS REMESAS DEL MISMO MEDICAMENTO = DISPONIBLES**<br>&nbsp;&nbsp;**VERIFICAR REMESA DISPONIBLE PARA ORDEN_DEVOLUCION**<br>&nbsp;&nbsp;**VERIFICAR ALV-TBL-ALERTAS-VENCIMIENTO = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-ALV-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | EDU-0004 exige que el sistema no solo emita la alerta sino que bloquee el lote para la venta. La versión anterior registraba la alerta y dejaba el producto plenamente vendible.<br>El producto pasó de capturarse como texto libre a seleccionarse del inventario: sin el identificador, el bloqueo del producto es imposible.<br>**Versión 03.00:** el retiro se registra con `id_lote` (FK a **TBL_LOTES**, Diccionario de Datos v06.00). Sin ese identificador, ESP-0016 no podía localizar la remesa que debía restituir. El nombre del medicamento y el número de lote se siguen copiando como texto para la lectura histórica. |

| Código especificación | ESP-0015 |
| --- | --- |
| Nombre | Modificación de alerta de producto vencido |
| Versión | 03.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0007 |
| Actor | ACT-0001 |
| Fuente | FUE-0005 |
| Experto | Ninguno |
| Código ilación | ILA-0015 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-ALV-0001**<br>&nbsp;&nbsp;**VALIDAR ALV-BTN-ACTUALIZAR-ALERTA = HABILITADO**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR alerta EN ALV-TBL-ALERTAS-VENCIMIENTO**<br>&nbsp;&nbsp;**CARGAR ART-MKP-ALV-0002**<br>&nbsp;&nbsp;**lote = alerta.id_lote**<br>&nbsp;&nbsp;**MOSTRAR lote EN ALV-CMB-LOTE-ALERTA = SOLO_LECTURA**<br>&nbsp;&nbsp;**MOSTRAR lote.fecha_vencimiento EN ALV-LBL-FECHA-VENCIMIENTO**<br>&nbsp;&nbsp;**loteDefectuoso = ALV-CHK-LOTE-DEFECTUOSO**<br>&nbsp;&nbsp;**alertaDigemid = ALV-TXT-ALERTA-DIGEMID**<br>&nbsp;&nbsp;**PRESIONAR ALV-BTN-ACTUALIZAR-ALERTA**<br>&nbsp;&nbsp;**PRESIONAR ALV-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR lote = alerta.id_lote (SIN_MODIFICACION)**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS SET es_lote_defectuoso, alerta_digemid DONDE id_retiro = alerta (id_lote, nombre_producto, numero_lote Y fecha_vencimiento SIN_MODIFICACION)**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR ALV-TBL-ALERTAS-VENCIMIENTO**<br>&nbsp;&nbsp;**MOSTRAR "Alerta actualizada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR BLOQUEO_DE_VENTA = VIGENTE**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS.id_lote = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR ALV-TBL-ALERTAS-VENCIMIENTO = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-ALV-0001, ART-MKP-ALV-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se incorporó el paso de confirmación explícito, coherente con el resto de operaciones de actualización del catálogo, y se mantiene vigente el bloqueo de venta mientras la alerta exista.<br>**Versión 03.00:** (AUD-claude-002, AUD-gpt-004; DEC-035) la remesa de un retiro es de solo lectura, igual que las claves foráneas de ESP-0047 y ESP-0051. Cambiarla dejaba la remesa anterior bloqueada sin retiro que la sostuviera y la nueva vendible, y desalineaba las copias históricas. Para cambiar de remesa se elimina la alerta (ESP-0016) y se registra otra (ESP-0014). Las copias `nombre_producto`, `numero_lote` y `fecha_vencimiento` no se editan. El combo **ALV-CMB-LOTE-ALERTA** de **ART-MKP-ALV-0002** debe mostrarse de solo lectura. |

| Código especificación | ESP-0016 |
| --- | --- |
| Nombre | Eliminación de alerta de producto vencido |
| Versión | 04.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0007 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0016 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_DETALLE_DEVOLUCION, id_lote = alerta.id_lote Y orden.estado_orden = 'Pendiente')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-ALV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR alerta EN ALV-TBL-ALERTAS-VENCIMIENTO**<br>&nbsp;&nbsp;**PRESIONAR ALV-BTN-ELIMINAR-ALERTA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-ALV-0003**<br>&nbsp;&nbsp;**MOSTRAR ALV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR ALV-TXT-MSJ = "¿Está seguro de eliminar esta alerta? La remesa volverá a estar disponible para la venta."**<br>&nbsp;&nbsp;**SI EXISTE(DB_FARMASIL.TBL_DETALLE_DEVOLUCION, id_lote = alerta.id_lote Y orden.estado_orden = 'Pendiente') ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La remesa forma parte de una orden de devolución pendiente"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**lote = CONSULTAR DB_FARMASIL.TBL_LOTES DONDE id_lote = alerta.id_lote**<br>&nbsp;&nbsp;**SI lote <> NULO Y lote.fecha_vencimiento <= FECHA_SISTEMA ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La remesa está vencida y no puede volver a la venta. Gestione su devolución o su baja"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**SI PRESIONAR ALV-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS DONDE id_retiro = alerta**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI lote <> NULO Y NO EXISTE(DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS, id_lote = alerta.id_lote) ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES.estado_lote = ('Disponible' SI stock_actual > 0, 'Agotado' EN OTRO CASO) DONDE id_lote = alerta.id_lote**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI lote <> NULO Y EXISTE(DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS, id_lote = alerta.id_lote) ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Alerta eliminada. La remesa sigue bloqueada porque tiene otra alerta vigente"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Alerta eliminada correctamente"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**SINO SI PRESIONAR ALV-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR ALV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR ALV-TBL-ALERTAS-VENCIMIENTO**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ELIMINADO = VERDADERO**<br>&nbsp;&nbsp;**SI NO QUEDA OTRO RETIRO CON id_lote = alerta.id_lote ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.estado_lote = 'Disponible' SI stock_actual > 0, 'Agotado' EN OTRO CASO**<br>&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.estado_lote = 'Bloqueado por devolucion'**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**VERIFICAR REMESA CON estado_lote = 'Disponible' DISPONIBLE EN VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;**VERIFICAR REMESA_VENCIDA.estado_lote <> 'Disponible'**<br>&nbsp;&nbsp;**VERIFICAR ALV-TBL-ALERTAS-VENCIMIENTO = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-ALV-0001, ART-MKP-ALV-0003 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se incorporó la restitución del estado del producto, contraparte lógica del bloqueo aplicado en ESP-0014: sin ella, eliminar la alerta dejaba el producto bloqueado de forma permanente.<br>Se agregó la verificación de que el producto no esté comprometido en una devolución en curso.<br>**Versión 03.00:** la remesa se localiza por `alerta.id_lote`, campo incorporado a **TBL_PRODUCTOS_RETIRADOS** en el Diccionario de Datos v06.00; antes la especificación leía un `alerta.lote` que la tabla no tenía. Se agrega además el rechazo cuando la remesa ya está vencida: restituirla a 'Disponible' la habría vuelto vendible, en contra de EDU-0004. Una remesa vencida se gestiona por devolución (ESP-0025) o por baja (ESP-0044). Si la remesa ya fue eliminada físicamente, `id_lote` es nulo y solo se elimina el registro de retiro.<br>**Versión 04.00:** (AUD-claude-006, AUD-gpt-002, AUD-gpt-003; DEC-034, PEN-020) la remesa solo vuelve a la venta si no queda otro retiro con el mismo `id_lote`, no tiene devoluciones pendientes y no está vencida. Al liberarla queda 'Disponible' si tiene existencias y 'Agotado' si no (una sola sentencia, porque el `CHECK` de la remesa no admite 'Disponible' con cero unidades). Se admiten varios retiros por remesa hasta que la dueña responda (PEN-020). |

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
| Versión | 04.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0002 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0020 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_METODOS_PAGO <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PAG-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR metodo EN PAG-TBL-METODOS-PAGO**<br>&nbsp;&nbsp;**PRESIONAR PAG-BTN-ELIMINAR-METODO**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PAG-0004**<br>&nbsp;&nbsp;**MOSTRAR PAG-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR PAG-TXT-MSJ = "¿Está seguro de dar de baja este método de pago? Las ventas ya registradas con él no se verán afectadas."**<br>&nbsp;&nbsp;**SI PRESIONAR PAG-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**resultado = INTENTAR ELIMINAR DB_FARMASIL.TBL_METODOS_PAGO DONDE id_metodo_pago = metodo**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI resultado = RECHAZADO_POR_REFERENCIAS ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_METODOS_PAGO.estado = 'Inactivo'**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Método de pago dado de baja correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR PAG-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR PAG-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR PAG-TBL-METODOS-PAGO**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR BAJA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR METODO_CON_VENTAS.estado = 'Inactivo'**<br>&nbsp;&nbsp;**VERIFICAR METODO NO DISPONIBLE EN VEN-CMB-METODO-PAGO**<br>&nbsp;&nbsp;**VERIFICAR HISTORICO_VENTAS = INTACTO**<br>&nbsp;&nbsp;**VERIFICAR INTEGRIDAD_REFERENCIAL = SIN_VIOLACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-PAG-0001, ART-MKP-PAG-0004 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La postcondición anterior era contradictoria: para una eliminación afirmaba que el método "queda registrado correctamente".<br>La eliminación física era además inviable, porque `id_metodo_pago` es clave foránea en **TBL_REGISTRO_VENTAS**. Se resolvió con baja lógica cuando existen ventas asociadas.<br>**Versión 03.00:** Baja condicional según DEC-009 y DEC-029 (VP-B). El módulo ya no consulta tablas de otros módulos para decidirlo (RG-011), y la verificación cubre cualquier referencia, incluidas las que la versión anterior omitía.<br>**Versión 04.00:** la baja condicional se refiere a DEC-009 y DEC-029 (VP-B) en lugar de describir el mecanismo; DEC-029 corrige el fundamento de DEC-022. |

---

# Módulo 6 — Gestión de Reportes de Ventas (EDU-0010)

| Código especificación | ESP-0021 |
| --- | --- |
| Nombre | Generación de reportes de ventas |
| Versión | 05.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0021 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REGISTRO_VENTAS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_DETALLE_VENTAS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REPORTES_VENTAS = DISPONIBLE**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REP-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**tipoReporte = REP-CMB-TIPO-REPORTE**<br>&nbsp;&nbsp;**formato = REP-CMB-FORMATO-REPORTE**<br>&nbsp;&nbsp;**fechaInicio = REP-FEC-FECHA-INICIO**<br>&nbsp;&nbsp;**fechaFin = REP-FEC-FECHA-FIN**<br>&nbsp;&nbsp;**PRESIONAR REP-BTN-GENERAR-REPORTE**<br>&nbsp;&nbsp;**VALIDAR fechaInicio <= fechaFin**<br>&nbsp;&nbsp;**VALIDAR tipoReporte**<br>&nbsp;&nbsp;**VALIDAR formato EN ('PDF', 'Excel', 'HTML')**<br>&nbsp;&nbsp;**datos = CONSULTAR DB_FARMASIL.TBL_REGISTRO_VENTAS UNIENDO DB_FARMASIL.TBL_DETALLE_VENTAS UNIENDO DB_FARMASIL.TBL_USUARIOS**<br>&nbsp;&nbsp;**APLICAR FILTRO fecha_hora >= fechaInicio Y fecha_hora < fechaFin + 1 DIA**<br>&nbsp;&nbsp;**CONSOLIDAR (monto_recaudado, cantidad_productos, fechas, usuario_responsable) DESDE datos**<br>&nbsp;&nbsp;**totalRecaudado = SUMA(DB_FARMASIL.TBL_REGISTRO_VENTAS.monto_total) DE LAS VENTAS DEL INTERVALO**<br>&nbsp;&nbsp;**rutaArchivo = GENERAR_ARCHIVO(datos, formato)**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (tipoReporte, formato, fechaInicio, fechaFin, totalRecaudado, FECHA_HORA_SISTEMA, rutaArchivo) EN DB_FARMASIL.TBL_REPORTES_VENTAS**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR REP-TBL-REPORTES**<br>&nbsp;&nbsp;**MOSTRAR "Reporte generado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REPORTE_GENERADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REPORTES_VENTAS = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR REPORTE CONTIENE usuario_responsable POR CADA VENTA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REGISTRO_VENTAS = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR ARCHIVO DISPONIBLE EN rutaArchivo**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-REP-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Corresponde a la fase Crear del CRUD. La consulta de un reporte existente, antes mezclada aquí, se separó hacia ESP-0022.<br>La consolidación une ahora con **TBL_USUARIOS** para obtener el responsable de cada venta, que EDU-0010 exige y que la dueña confirmó.<br>Queda Pendiente hasta que el campo `id_usuario` incorporado al Diccionario de Datos v03.00 se refleje en el motor.<br>**Nota v03.00 — `usuario_responsable` es un valor calculado, no una columna.** El campo que consolida el paso `CONSOLIDAR (..., usuario_responsable)` se obtiene en el momento de generar el reporte mediante el `UNIENDO DB_FARMASIL.TBL_USUARIOS` (a partir de `TBL_REGISTRO_VENTAS.id_usuario`) y solo vive en el archivo de salida. No se persiste: el `REGISTRAR` sobre `DB_FARMASIL.TBL_REPORTES_VENTAS` guarda únicamente el índice (tipo, formato, fechas, total recaudado, fecha de generación y ruta), tal como documenta el Diccionario de Datos v06.00.<br>**Versión 04.00 — se descarta el tipo 'Mermas'.** No tenía respaldo en EDU-0010 ni había sido solicitado por la dueña, y el procedimiento no tenía rama propia para él. `tipo_reporte` queda como criterio de agrupación de las ventas consolidadas; su dominio cerrado (`VALIDAR tipoReporte`) sigue pendiente de confirmación por la dueña.<br>**Versión 04.00 — el índice guarda `formato` y `total_recaudado`.** ESP-0022 muestra el total recaudado de cada reporte y ESP-0023 regenera el archivo en su formato original, pero ninguno de los dos valores se persistía. Se agregan ambas columnas a **TBL_REPORTES_VENTAS** (Diccionario de Datos v06.00).<br>**Versión 05.00:** (AUD-claude-017, AUD-gpt-012; DEC-037) el intervalo es semiabierto: las ventas de todo el último día quedan incluidas. Con `ENTRE fechaInicio Y fechaFin` sobre un `DATETIME2` se excluían las ventas del último día posteriores a la medianoche y el `total_recaudado` salía menor. |

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
| Versión | 04.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código ilación | ILA-0023 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_REPORTES_VENTAS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REP-0002**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR reporte EN REP-TBL-REPORTES**<br>&nbsp;&nbsp;**PRESIONAR REP-BTN-ACTUALIZAR-REPORTE**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REP-0003 CON (reporte.fecha_inicio, reporte.fecha_fin)**<br>&nbsp;&nbsp;**fechaInicio = REP-FEC-FECHA-INICIO**<br>&nbsp;&nbsp;**fechaFin = REP-FEC-FECHA-FIN**<br>&nbsp;&nbsp;**PRESIONAR REP-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR fechaInicio <= fechaFin**<br>&nbsp;&nbsp;**datos = CONSULTAR DB_FARMASIL.TBL_REGISTRO_VENTAS UNIENDO DB_FARMASIL.TBL_DETALLE_VENTAS UNIENDO DB_FARMASIL.TBL_USUARIOS = SOLO_LECTURA**<br>&nbsp;&nbsp;**APLICAR FILTRO fecha_hora >= fechaInicio Y fecha_hora < fechaFin + 1 DIA**<br>&nbsp;&nbsp;**CONSOLIDAR (monto_recaudado, cantidad_productos, fechas, usuario_responsable) DESDE datos**<br>&nbsp;&nbsp;**totalRecaudado = SUMA(DB_FARMASIL.TBL_REGISTRO_VENTAS.monto_total) DE LAS VENTAS DEL INTERVALO**<br>&nbsp;&nbsp;**rutaArchivo = REGENERAR_ARCHIVO(datos, reporte.formato)**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_REPORTES_VENTAS SET fecha_inicio, fecha_fin, total_recaudado, fecha_generacion, ruta_archivo DONDE id_reporte = reporte**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR REP-TBL-REPORTES**<br>&nbsp;&nbsp;**MOSTRAR "Reporte actualizado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR REPORTE CONTIENE usuario_responsable POR CADA VENTA**<br>&nbsp;&nbsp;**VERIFICAR FORMATO_ARCHIVO = reporte.formato**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_REGISTRO_VENTAS = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_DETALLE_VENTAS = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR REP-TBL-REPORTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-REP-0002, ART-MKP-REP-0003 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Reemplaza a la antigua especificación "Restricción de modificación", que describía la ausencia de una operación y dejaba incompleta la fase Actualizar del CRUD.<br>Solo se actualiza el índice y los parámetros del reporte. Las dos tablas de origen se consultan en modo de solo lectura, y la postcondición lo verifica explícitamente.<br>EDU-0010 v05.00 amplió el alcance del módulo para autorizar esta operación.<br>**Versión 03.00:** la regeneración consolida los mismos datos que ESP-0021, incluida la unión con **TBL_USUARIOS**; sin ella, el reporte recalculado perdía la identificación del personal que exige EDU-0010. `reporte.formato` y `total_recaudado` existen ahora como columnas de **TBL_REPORTES_VENTAS** (Diccionario de Datos v06.00).<br>**Versión 04.00:** (AUD-claude-017, AUD-gpt-012; DEC-037) la regeneración usa el mismo intervalo semiabierto que ESP-0021. |

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
| Versión | 05.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0025 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_LOTES, estado_lote EN ('Bloqueado por devolucion', 'Disponible') Y stock_actual > 0)**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_PROVEEDORES, estado = 'Activo')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DEV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**fechaCreacion = DEV-FEC-FECHA-DEVOLUCION**<br>&nbsp;&nbsp;**proveedor = DEV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**MOSTRAR EN DEV-CMB-LOTE-DEVOLUCION CONSULTAR DB_FARMASIL.TBL_LOTES DONDE id_proveedor = proveedor Y estado_lote EN ('Bloqueado por devolucion', 'Disponible') Y stock_actual > 0 ORDENADO POR fecha_vencimiento ASCENDENTE**<br>&nbsp;&nbsp;**motivo = DEV-CMB-MOTIVO-DEVOLUCION**<br>&nbsp;&nbsp;**detalle = LISTA_VACIA**<br>&nbsp;&nbsp;**REPETIR**<br>&nbsp;&nbsp;&nbsp;&nbsp;**lote = DEV-CMB-LOTE-DEVOLUCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**cantidad = DEV-NUM-CANTIDAD-DEVOLVER**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR lote.id_proveedor = proveedor**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR lote.estado_lote EN ('Bloqueado por devolucion', 'Disponible')**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR cantidad = ENTERO > 0**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI lote YA FIGURA EN detalle ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**cantidadTotal = cantidad + LINEA_DE(detalle, lote).cantidad**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**cantidadTotal = cantidad**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR cantidadTotal <= lote.stock_actual**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI lote YA FIGURA EN detalle ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**LINEA_DE(detalle, lote).cantidad = cantidadTotal**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SINO**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**AGREGAR (lote, cantidad) A detalle**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DEV-TBL-DETALLE-DEVOLUCION**<br>&nbsp;&nbsp;**HASTA QUE USUARIO NO AGREGUE MAS REMESAS**<br>&nbsp;&nbsp;**comentario = DEV-TXA-COMENTARIO**<br>&nbsp;&nbsp;**SI motivo = 'Por Depuracion de Error' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR comentario <> VACIO**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**PRESIONAR DEV-BTN-CREAR-DEVOLUCION**<br>&nbsp;&nbsp;**VALIDAR detalle <> VACIA**<br>&nbsp;&nbsp;**VALIDAR motivo EN ('Por Vencimiento', 'Por Depuracion de Error')**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (fechaCreacion, motivo, proveedor, comentario, 'Pendiente', SESION_USUARIO.id_usuario) EN DB_FARMASIL.TBL_ORDENES_DEVOLUCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**idDevolucion = ULTIMO_ID_GENERADO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA linea EN detalle HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (idDevolucion, linea.lote, linea.cantidad) EN DB_FARMASIL.TBL_DETALLE_DEVOLUCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR DEV-TBL-ORDENES-DEVOLUCION**<br>&nbsp;&nbsp;**MOSTRAR "Orden de devolución registrada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION.estado_orden = 'Pendiente'**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION.id_usuario = SESION_USUARIO.id_usuario**<br>&nbsp;&nbsp;**VERIFICAR CONTAR(DB_FARMASIL.TBL_DETALLE_DEVOLUCION DONDE id_devolucion = idDevolucion) = CONTAR(detalle)**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.stock_actual = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR NO EXISTEN DOS LINEAS DE LA MISMA REMESA EN LA ORDEN**<br>&nbsp;&nbsp;**VERIFICAR DEV-TBL-ORDENES-DEVOLUCION = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DEV-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | **Versión 04.00:** el combo **DEV-CMB-LOTE-DEVOLUCION** lista las remesas del proveedor con existencias, en estado 'Bloqueado por devolucion' o 'Disponible', ordenadas por fecha de vencimiento para que las más próximas a vencer aparezcan primero. La versión anterior exigía en la precondición remesas 'Bloqueado por devolucion' y no definía el listado. No se aplica el umbral de `TBL_ALERTAS_VENCIMIENTO`: leerlo desde Devoluciones crearía una dependencia Devoluciones → Alertas de Vencimiento, que junto con la existente Alertas → Devoluciones formaría un ciclo en la arquitectura genérica (DEC-023). El stock queda reservado de hecho hasta completar la orden: si entretanto se venden unidades de la remesa, ESP-0027 lo detecta al completar (DEC-019).<br>**Corrección v03.00:** `VALIDAR lote.producto.id_proveedor = proveedor` se reemplazó por `VALIDAR lote.id_proveedor = proveedor`. La versión anterior resolvía el proveedor a través del producto, lo que era incorrecto desde que `TBL_LOTES` tiene su propio `id_proveedor`: un medicamento puede tener remesas de más de un proveedor, y la devolución siempre corresponde al proveedor real de la remesa, no al proveedor habitual del catálogo. El comentario de v02.00 se conserva a continuación.<br>Reescrita sobre el par cabecera y detalle. La versión anterior registraba un único medicamento e ignoraba **TBL_DETALLE_DEVOLUCION**.<br>Se incorporaron `cantidad_devolver`, que es NOT NULL y no se capturaba en ninguna parte, y `comentario`, obligatorio cuando el motivo es la depuración de un error.<br>El stock no se modifica al registrar: el descuento ocurre cuando la orden pasa a 'Completada' en ESP-0027, que es cuando la mercancía sale físicamente del almacén.<br>Depende de los componentes **DEV-TBL-DETALLE-DEVOLUCION**, **DEV-NUM-CANTIDAD-DEVOLVER** y **DEV-TXA-COMENTARIO**, aún por incorporar al mockup.<br>**Versión 05.00:** (AUD-claude-005; DEC-036) una remesa aparece una sola vez por orden: si se agrega de nuevo, se acumula su cantidad y la validación de stock usa la cantidad acumulada. |

| Código especificación | ESP-0026 |
| --- | --- |
| Nombre | Consulta de orden de devolución |
| Versión | 04.00 |
| Fecha | 09/11/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0026 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DEV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**CARGAR DEV-TBL-ORDENES-DEVOLUCION CON (id_devolucion, fecha_creacion, proveedor, motivo_devolucion, estado_orden)**<br>&nbsp;&nbsp;**SELECCIONAR orden EN DEV-TBL-ORDENES-DEVOLUCION**<br>&nbsp;&nbsp;**PRESIONAR DEV-BTN-LEER-DEVOLUCION**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DEV-0002**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION UNIENDO DB_FARMASIL.TBL_PROVEEDORES**<br>&nbsp;&nbsp;**detalle = CONSULTAR DB_FARMASIL.TBL_DETALLE_DEVOLUCION DONDE id_devolucion = orden**<br>&nbsp;&nbsp;**remesas = ILotes.datosRemesas(detalle.id_lote) CON (id_lote, numero_lote, nombre_medicamento)**<br>&nbsp;&nbsp;**CARGAR DEV-TBL-DETALLE-DEVOLUCION CON (remesas.nombre_medicamento, remesas.numero_lote, detalle.cantidad_devolver)**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DEV-TBL-ORDENES-DEVOLUCION = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DEV-TBL-DETALLE-DEVOLUCION = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DEV-0001, ART-MKP-DEV-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La postcondición anterior decía únicamente que "el sistema permanece invariable", sin declarar el resultado de la consulta.<br>El detalle se resuelve contra **TBL_DETALLE_DEVOLUCION**, el número de lote de **TBL_LOTES** a través de `id_lote` y el nombre del medicamento de **TBL_PRODUCTOS** (Diccionario de Datos v04.00).<br>**Versión 03.00:** se corrige la referencia anterior, que situaba el lote en **TBL_PRODUCTOS**, donde ya no existe desde la incorporación de **TBL_LOTES**.<br>**Versión 04.00:** (DEC-057) el número de lote y el nombre del medicamento de cada línea se obtienen por la interfaz **ILotes**, que devuelve los datos de la remesa, como en DEC-044. Devoluciones ya no une **TBL_LOTES** ni **TBL_PRODUCTOS**: leer **TBL_PRODUCTOS** suponía una dependencia Devoluciones → Inventario que la arquitectura genérica no tiene. |

| Código especificación | ESP-0027 |
| --- | --- |
| Nombre | Actualización de orden de devolución |
| Versión | 04.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0027 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION <> VACIA**<br>&nbsp;&nbsp;**VALIDAR orden.estado_orden = 'Pendiente'**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DEV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR orden EN DEV-TBL-ORDENES-DEVOLUCION**<br>&nbsp;&nbsp;**PRESIONAR DEV-BTN-ACTUALIZAR-DEVOLUCION**<br>&nbsp;&nbsp;**SI orden.estado_orden = 'Completada' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La orden ya fue despachada y no admite modificación"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CARGAR ART-MKP-DEV-0003 CON cabecera Y detalle VIGENTES**<br>&nbsp;&nbsp;**MOSTRAR orden.id_proveedor EN DEV-CMB-PROVEEDOR = SOLO_LECTURA**<br>&nbsp;&nbsp;**proveedor = orden.id_proveedor**<br>&nbsp;&nbsp;**motivo = DEV-CMB-MOTIVO-DEVOLUCION**<br>&nbsp;&nbsp;**comentario = DEV-TXA-COMENTARIO**<br>&nbsp;&nbsp;**detalleNuevo = DEV-TBL-DETALLE-DEVOLUCION**<br>&nbsp;&nbsp;**estadoNuevo = ESTADO_SELECCIONADO**<br>&nbsp;&nbsp;**PRESIONAR DEV-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR detalleNuevo <> VACIA**<br>&nbsp;&nbsp;**VALIDAR motivo EN ('Por Vencimiento', 'Por Depuracion de Error')**<br>&nbsp;&nbsp;**SI motivo = 'Por Depuracion de Error' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR comentario <> VACIO**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**VALIDAR detalleNuevo TIENE UNA SOLA LINEA POR REMESA**<br>&nbsp;&nbsp;**PARA CADA linea EN detalleNuevo HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR linea.cantidad_devolver = ENTERO > 0**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR linea.lote.id_proveedor = proveedor**<br>&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;**VALIDAR estadoNuevo EN ('Pendiente', 'Completada')**<br>&nbsp;&nbsp;**SI estadoNuevo = 'Completada' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA linea EN detalleNuevo HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**lote = CONSULTAR DB_FARMASIL.TBL_LOTES DONDE id_lote = linea.lote**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI linea.cantidad_devolver > lote.stock_actual ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La remesa " + lote.numero_lote + " ya no tiene stock suficiente para completar la orden. Ajuste el detalle"**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION DONDE id_devolucion = orden**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SINCRONIZAR DB_FARMASIL.TBL_DETALLE_DEVOLUCION CON detalleNuevo**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI estadoNuevo = 'Completada' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**PARA CADA linea EN detalleNuevo HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR_STOCK_LOTE(linea.lote, stock_actual - linea.cantidad_devolver)**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR DEV-TBL-ORDENES-DEVOLUCION**<br>&nbsp;&nbsp;**MOSTRAR "Orden de devolución actualizada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**SI estadoNuevo = 'Completada' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.stock_actual = DESCONTADO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.stock_actual >= 0**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR ORDEN = NO_MODIFICABLE**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**VERIFICAR CABECERA Y DETALLE = CONSISTENTES**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_ORDENES_DEVOLUCION.id_proveedor = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR CADA LINEA TIENE lote.id_proveedor = orden.id_proveedor**<br>&nbsp;&nbsp;**VERIFICAR DEV-TBL-ORDENES-DEVOLUCION = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-DEV-0001, ART-MKP-DEV-0003 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | **Versión 03.00:** al pasar la orden a 'Completada' se valida, antes de descontar, que cada remesa conserve stock suficiente para la cantidad a devolver. Registrar la orden no descuenta stock (ESP-0025), y entretanto Ventas puede vender unidades de la misma remesa: sin esta validación el stock podía quedar negativo. Si una remesa no alcanza, se cancela la operación y se pide ajustar el detalle; Ventas no descuenta lo reservado, porque eso crearía una dependencia Ventas → Devoluciones (DEC-019). Se agrega el paso de la remesa 'Disponible' a 'Agotado' cuando su stock llega a cero, que la ilación ya exigía.<br>La postcondición anterior afirmaba que "los medicamentos actualizan su estado correspondiente" sin precisar cuál era ese estado ni qué campo se modificaba. Se sustituyó por el descuento efectivo del stock al completarse la orden.<br>Se incorporó el bloqueo de las órdenes ya despachadas.<br>Depende de los mismos componentes pendientes que ESP-0025.<br>**Versión 04.00:** (AUD-claude-005, 018; AUD-gpt-007, 010, 011; DEC-036, DEC-038) el proveedor de la orden es siempre de solo lectura. Al editar se repiten las validaciones de la creación: cantidad mayor que cero en cada línea, comentario obligatorio con el motivo 'Por Depuracion de Error' y proveedor real de cada remesa igual al de la orden; además, una sola línea por remesa. Al completar la orden, el descuento de stock y el cambio de estado se hacen con `ACTUALIZAR_STOCK_LOTE`. |

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
| Versión | 02.00 |
| Fecha | 13/10/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Consulta posterior al cliente |
| Experto | Ninguno |
| Código ilación | ILA-0033 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**CARGAR ART-MKP-USR-0001**<br>&nbsp;&nbsp;**VALIDAR USR-BTN-CREAR-USUARIO = HABILITADO**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**nombreUsuario = USR-TXT-NOMBRE-USUARIO**<br>&nbsp;&nbsp;**contrasena = USR-PWD-CONTRASENA**<br>&nbsp;&nbsp;**rol = USR-CMB-ROL**<br>&nbsp;&nbsp;**estado = USR-CMB-ESTADO-USUARIO**<br>&nbsp;&nbsp;**PRESIONAR USR-BTN-CREAR-USUARIO**<br>&nbsp;&nbsp;**VALIDAR nombreUsuario <> VACIO**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_USUARIOS, nombre_usuario = nombreUsuario)**<br>&nbsp;&nbsp;**VALIDAR contrasena <> VACIA**<br>&nbsp;&nbsp;**VALIDAR rol EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR estado EN ('Activo', 'Inactivo')**<br>&nbsp;&nbsp;**contrasenaHash = HASH_CON_SAL(contrasena)**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (nombreUsuario, contrasenaHash, rol, estado) EN DB_FARMASIL.TBL_USUARIOS**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR USR-TBL-USUARIOS**<br>&nbsp;&nbsp;**MOSTRAR "Usuario registrado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_USUARIOS.contrasena = HASH_CON_SAL**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_USUARIOS.nombre_usuario = UNICO**<br>&nbsp;&nbsp;**VERIFICAR USR-TBL-USUARIOS NO CONTIENE COLUMNA contrasena**<br>&nbsp;&nbsp;**SI estado = 'Activo' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR CUENTA HABILITADA PARA INICIO DE SESION SEGUN CONDICION 1 DE RNF-0006**<br>&nbsp;&nbsp;**FIN SI**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-USR-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Especificación nueva. La tabla **TBL_USUARIOS** existía en el modelo y era referenciada desde **TBL_ORDENES_DEVOLUCION**, pero ningún procedimiento alimentaba su contenido.<br>La contraseña se transforma en un hash con sal antes de persistirse y nunca se devuelve en la tabla de la interfaz, conforme al atributo de seguridad de RNF-0006.<br>Queda Pendiente porque el mockup **ART-MKP-USR-0001** aún no ha sido diseñado.<br>**Versión 02.00:** se precisa el mecanismo de protección: la contraseña se almacena como hash irreversible con sal individual (PBKDF2, bcrypt o equivalente), conforme a RNF-0006 v05.00; un cifrado reversible permitiría recuperar todas las contraseñas si se filtrara la base. |

| Código especificación | ESP-0034 |
| --- | --- |
| Nombre | Consulta de usuarios |
| Versión | 02.00 |
| Fecha | 13/10/2026 |
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
| Comentario | La exclusión explícita de la contraseña en el resultado responde a la condición 5 de RNF-0006 v03.00: ni siquiera su hash debe viajar hacia la interfaz.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-USR-0002**.<br>**Versión 02.00:** se ajusta el término: la contraseña se guarda como hash con sal, no encriptada (RNF-0006 v05.00). |

| Código especificación | ESP-0035 |
| --- | --- |
| Nombre | Actualización de usuario |
| Versión | 02.00 |
| Fecha | 13/10/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Consulta posterior al cliente |
| Experto | Ninguno |
| Código ilación | ILA-0035 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_USUARIOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-USR-0003**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR cuenta EN USR-TBL-USUARIOS**<br>&nbsp;&nbsp;**PRESIONAR USR-BTN-ACTUALIZAR-USUARIO**<br>&nbsp;&nbsp;**CARGAR ART-MKP-USR-0003 CON (cuenta.nombre_usuario, cuenta.rol, cuenta.estado)**<br>&nbsp;&nbsp;**LIMPIAR USR-PWD-CONTRASENA**<br>&nbsp;&nbsp;**nombreUsuario = USR-TXT-NOMBRE-USUARIO**<br>&nbsp;&nbsp;**contrasena = USR-PWD-CONTRASENA**<br>&nbsp;&nbsp;**rol = USR-CMB-ROL**<br>&nbsp;&nbsp;**estado = USR-CMB-ESTADO-USUARIO**<br>&nbsp;&nbsp;**PRESIONAR USR-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR nombreUsuario <> VACIO**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_USUARIOS, nombre_usuario = nombreUsuario Y id_usuario <> cuenta)**<br>&nbsp;&nbsp;**administradoresActivos = CONTAR(DB_FARMASIL.TBL_USUARIOS DONDE rol = 'Administrador' Y estado = 'Activo' Y id_usuario <> cuenta)**<br>&nbsp;&nbsp;**SI administradoresActivos = 0 Y (rol <> 'Administrador' O estado <> 'Activo') ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "El sistema debe conservar al menos una cuenta administradora activa"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_USUARIOS SET nombre_usuario, rol, estado DONDE id_usuario = cuenta**<br>&nbsp;&nbsp;&nbsp;&nbsp;**SI contrasena <> VACIA ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_USUARIOS.contrasena = HASH_CON_SAL(contrasena)**<br>&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR USR-TBL-USUARIOS**<br>&nbsp;&nbsp;**MOSTRAR "Usuario actualizado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_USUARIOS.id_usuario = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR CONTAR(DB_FARMASIL.TBL_USUARIOS DONDE rol = 'Administrador' Y estado = 'Activo') >= 1**<br>&nbsp;&nbsp;**VERIFICAR REFERENCIAS(TBL_ORDENES_DEVOLUCION, TBL_REGISTRO_VENTAS) = INTACTAS**<br>&nbsp;&nbsp;**VERIFICAR USR-TBL-USUARIOS = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-USR-0003 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La validación del último administrador activo evita el bloqueo total del sistema: si la única cuenta administradora se degrada a técnico o se desactiva, nadie podría volver a administrar usuarios.<br>El campo de contraseña se presenta vacío y solo se actualiza si el administrador escribe una nueva, de modo que editar el rol no obliga a restablecer credenciales.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-USR-0003**.<br>**Versión 02.00:** la nueva contraseña se guarda como hash con sal, igual que en ESP-0033. |

| Código especificación | ESP-0036 |
| --- | --- |
| Nombre | Desactivación de usuario |
| Versión | 03.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Consulta posterior al cliente |
| Experto | Ninguno |
| Código ilación | ILA-0036 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_USUARIOS <> VACIA**<br>&nbsp;&nbsp;**VALIDAR cuenta <> SESION_USUARIO.id_usuario**<br>&nbsp;&nbsp;**CARGAR ART-MKP-USR-0004**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR cuenta EN USR-TBL-USUARIOS**<br>&nbsp;&nbsp;**PRESIONAR USR-BTN-ELIMINAR-USUARIO**<br>&nbsp;&nbsp;**SI cuenta = SESION_USUARIO.id_usuario ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "No es posible dar de baja la cuenta con la que se encuentra abierta la sesión"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**MOSTRAR USR-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR USR-TXT-MSJ = "¿Está seguro de dar de baja esta cuenta? El histórico de operaciones que registró se conservará."**<br>&nbsp;&nbsp;**SI PRESIONAR USR-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**resultado = INTENTAR ELIMINAR DB_FARMASIL.TBL_USUARIOS DONDE id_usuario = cuenta**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI resultado = RECHAZADO_POR_REFERENCIAS ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_USUARIOS.estado = 'Inactivo' DONDE id_usuario = cuenta**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Cuenta dada de baja correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR USR-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR USR-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR USR-TBL-USUARIOS**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR BAJA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR CUENTA_CON_OPERACIONES.estado = 'Inactivo'**<br>&nbsp;&nbsp;**VERIFICAR TRAZABILIDAD DE VENTAS Y DEVOLUCIONES = CONSERVADA**<br>&nbsp;&nbsp;**VERIFICAR CUENTA NO PUEDE INICIAR SESION SEGUN CONDICION 1 DE RNF-0006**<br>&nbsp;&nbsp;**VERIFICAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VERIFICAR INTEGRIDAD_REFERENCIAL = SIN_VIOLACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-USR-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La baja lógica es obligatoria cuando la cuenta registró operaciones: eliminarla físicamente destruiría la verificación de identidad que la dueña solicitó.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-USR-0004**. El rechazo del inicio de sesión de una cuenta inactiva lo establece la condición 1 de RNF-0006.<br>**Versión 02.00:** Baja condicional según DEC-009 y DEC-029 (VP-B). El módulo ya no consulta tablas de otros módulos para decidirlo (RG-011), y la verificación cubre cualquier referencia, incluidas las que la versión anterior omitía (`TBL_COTIZACIONES_PROVEEDOR.id_usuario`).<br>**Versión 03.00:** la baja condicional se refiere a DEC-009 y DEC-029 (VP-B) en lugar de describir el mecanismo; DEC-029 corrige el fundamento de DEC-022. |

---

# Módulo 10 — Gestión de Proveedores (EDU-0014)

| Código especificación | ESP-0037 |
| --- | --- |
| Nombre | Registro de proveedor |
| Versión | 02.00 |
| Fecha | 11/11/2026 |
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
| Estado | Concluido |
| Comentario | Especificación nueva. La tabla **TBL_PROVEEDORES** existía y era referenciada desde **TBL_PRODUCTOS** y **TBL_ORDENES_DEVOLUCION**, pero ningún procedimiento la alimentaba: ESP-0025 exigía proveedores válidos como precondición sin que existiera forma de crearlos.<br>La validación de once dígitos corresponde a la restricción `NVARCHAR(11)` con UNIQUE del Diccionario de Datos v03.00.<br>**Versión 02.00:** (DEC-042 C2; AUD-MKP-claude-112, AUD-MKP-gpt-089) el registro usa el panel «Ficha de Proveedor» de **ART-MKP-PRV-0001**. **PRV-TXT-RUC**, **PRV-TXT-RAZON-SOCIAL**, **PRV-TXT-TELEFONO** y **PRV-CMB-ESTADO-PROVEEDOR** identifican solo los campos de la ficha; la barra superior usa **PRV-SRH-PROVEEDOR** y **PRV-CMB-FILTRO-ESTADO** (ESP-0038). |

| Código especificación | ESP-0038 |
| --- | --- |
| Nombre | Consulta de proveedores |
| Versión | 02.00 |
| Fecha | 11/11/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0038 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PROVEEDORES <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PRV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**busqueda = PRV-SRH-PROVEEDOR**<br>&nbsp;&nbsp;**estadoFiltro = PRV-CMB-FILTRO-ESTADO**<br>&nbsp;&nbsp;**PRESIONAR PRV-BTN-LEER-PROVEEDOR**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_PROVEEDORES**<br>&nbsp;&nbsp;**SI busqueda <> VACIO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**APLICAR FILTRO ruc = busqueda O razon_social CONTIENE busqueda**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**SI estadoFiltro <> VACIO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR estadoFiltro EN ('Activo', 'Inactivo')**<br>&nbsp;&nbsp;&nbsp;&nbsp;**APLICAR FILTRO estado = estadoFiltro**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**CARGAR PRV-TBL-PROVEEDORES CON (id_proveedor NO VISIBLE, ruc, razon_social, telefono, estado)**<br>&nbsp;&nbsp;**CARGAR PRV-PNL-PROVEEDORES CON UNA PRV-CRD-PROVEEDOR (razon_social, ruc, estado) POR CADA PROVEEDOR DEL RESULTADO**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR PRV-TBL-PROVEEDORES = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR PRV-PNL-PROVEEDORES = MISMO RESULTADO QUE PRV-TBL-PROVEEDORES**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PROVEEDORES = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-PRV-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Permite ubicar los datos de contacto de la droguería al momento de coordinar el retiro de un lote, que según la Entrevista 1 se gestiona por comunicación directa con el proveedor.<br>**Versión 02.00:** adecuación a la pantalla única **ART-MKP-PRV-0001** (DEC-042). C1: se cita ART-MKP-PRV-0001 en lugar de 0002 (AUD-MKP-claude-116, AUD-MKP-gpt-088). C2: un solo criterio de texto en **PRV-SRH-PROVEEDOR**, que busca por RUC exacto o razón social parcial, y el filtro **PRV-CMB-FILTRO-ESTADO**, separados de los campos de la ficha; **PRV-TBL-PROVEEDORES** identifica solo la tabla y la lista de tarjetas usa **PRV-PNL-PROVEEDORES** y **PRV-CRD-PROVEEDOR**, con el mismo resultado (AUD-MKP-claude-111, AUD-MKP-claude-112, AUD-MKP-gpt-089). C3: filtro opcional de estado sobre la columna `estado`, sin columnas nuevas (AUD-MKP-claude-113, AUD-MKP-gpt-090). C4: la tabla muestra el teléfono aunque el mockup no lo tenga (AUD-MKP-claude-114, AUD-MKP-gpt-092). Por DEC-063, y no por C4, `id_proveedor` deja de mostrarse y se conserva como clave de selección no visible; el RUC es la clave visible del proveedor. Ya no se exige un criterio: sin criterios se listan todos los proveedores. La pantalla es exclusiva del Administrador según la matriz de permisos; para el Técnico la interfaz oculta o inhabilita el módulo (DEC-050, AUD-MKP-gpt-093). |

| Código especificación | ESP-0039 |
| --- | --- |
| Nombre | Actualización de proveedor |
| Versión | 02.00 |
| Fecha | 11/11/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0039 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PROVEEDORES <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PRV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR proveedor EN PRV-TBL-PROVEEDORES O EN PRV-PNL-PROVEEDORES**<br>&nbsp;&nbsp;**PRESIONAR PRV-BTN-ACTUALIZAR-PROVEEDOR**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PRV-0001 CON INFORMACION VIGENTE**<br>&nbsp;&nbsp;**HABILITAR PRV-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**ruc = PRV-TXT-RUC**<br>&nbsp;&nbsp;**razonSocial = PRV-TXT-RAZON-SOCIAL**<br>&nbsp;&nbsp;**telefono = PRV-TXT-TELEFONO**<br>&nbsp;&nbsp;**estado = PRV-CMB-ESTADO-PROVEEDOR**<br>&nbsp;&nbsp;**PRESIONAR PRV-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR ruc = 11 DIGITOS NUMERICOS**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_PROVEEDORES, ruc = ruc Y id_proveedor <> proveedor)**<br>&nbsp;&nbsp;**VALIDAR razonSocial <> VACIO**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_PROVEEDORES DONDE id_proveedor = proveedor**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR PRV-TBL-PROVEEDORES**<br>&nbsp;&nbsp;**MOSTRAR "Proveedor actualizado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_PROVEEDORES.id_proveedor = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR REFERENCIAS(TBL_PRODUCTOS, TBL_ORDENES_DEVOLUCION) = INTACTAS**<br>&nbsp;&nbsp;**SI estado = 'Inactivo' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR PROVEEDOR NO DISPONIBLE EN INV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR PROVEEDOR NO DISPONIBLE EN DEV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**VERIFICAR PRV-TBL-PROVEEDORES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-PRV-0001 |
| Importancia | Media |
| Estado | Concluido |
| Comentario | Permite mantener vigentes los datos de contacto de las distribuidoras, que cambian con más frecuencia que la propia relación comercial.<br>**Versión 02.00:** (DEC-042 C1 y C4, DEC-046) la actualización se hace en la ficha de la pantalla única **ART-MKP-PRV-0001** (AUD-MKP-claude-116, AUD-MKP-gpt-088). **PRV-BTN-ACTUALIZAR-PROVEEDOR** inicia la edición y **PRV-BTN-CONFIRMAR-ACTUALIZACION**, rotulado «Guardar cambios», confirma los cambios. El mockup no muestra ese botón, pero se conserva porque la especificación lo exige (AUD-MKP-claude-115, AUD-MKP-gpt-091). |

| Código especificación | ESP-0040 |
| --- | --- |
| Nombre | Desactivación de proveedor |
| Versión | 04.00 |
| Fecha | 11/11/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0040 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PROVEEDORES <> VACIA**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_ORDENES_DEVOLUCION, id_proveedor = proveedor Y estado_orden = 'Pendiente')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-PRV-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR proveedor EN PRV-TBL-PROVEEDORES O EN PRV-PNL-PROVEEDORES**<br>&nbsp;&nbsp;**PRESIONAR PRV-BTN-ELIMINAR-PROVEEDOR**<br>&nbsp;&nbsp;**SI EXISTE(DB_FARMASIL.TBL_ORDENES_DEVOLUCION, id_proveedor = proveedor Y estado_orden = 'Pendiente') ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "El proveedor tiene órdenes de devolución pendientes de despacho"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**MOSTRAR PRV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR PRV-TXT-MSJ = "¿Está seguro de dar de baja este proveedor? Los productos y devoluciones ya registrados no se verán afectados."**<br>&nbsp;&nbsp;**SI PRESIONAR PRV-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**resultado = INTENTAR ELIMINAR DB_FARMASIL.TBL_PROVEEDORES DONDE id_proveedor = proveedor**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI resultado = RECHAZADO_POR_REFERENCIAS ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_PROVEEDORES.estado = 'Inactivo' DONDE id_proveedor = proveedor**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Proveedor dado de baja correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR PRV-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR PRV-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR PRV-TBL-PROVEEDORES**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR BAJA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR PROVEEDOR_CON_REGISTROS.estado = 'Inactivo'**<br>&nbsp;&nbsp;**VERIFICAR PROVEEDOR NO DISPONIBLE EN INV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**VERIFICAR PROVEEDOR NO DISPONIBLE EN DEV-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**VERIFICAR NINGUNA ORDEN PENDIENTE = SIN_DESTINATARIO**<br>&nbsp;&nbsp;**VERIFICAR INTEGRIDAD_REFERENCIAL = SIN_VIOLACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-PRV-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se aplica el mismo criterio de baja lógica del resto del catálogo, más una verificación adicional: un proveedor con una devolución aún no despachada no puede darse de baja, porque la orden quedaría sin destinatario.<br>**Versión 02.00:** Baja condicional según DEC-009 y DEC-029 (VP-B). El módulo ya no consulta tablas de otros módulos para decidirlo (RG-011), y la verificación cubre cualquier referencia, incluidas las que la versión anterior omitía (`TBL_LOTES.id_proveedor` y `TBL_COTIZACIONES_PROVEEDOR.id_proveedor`). La verificación de órdenes pendientes se conserva porque es una regla de negocio, no de integridad.<br>**Versión 03.00:** la baja condicional se refiere a DEC-009 y DEC-029 (VP-B) en lugar de describir el mecanismo; DEC-029 corrige el fundamento de DEC-022.<br>**Versión 04.00:** (DEC-042 C1 y C4, DEC-047) la baja se inicia desde la pantalla única **ART-MKP-PRV-0001** (AUD-MKP-claude-116, AUD-MKP-gpt-088). El modal usa los componentes `PRV-MDL-*` de la plantilla `ART-MKP-CONFIRM-0001` con el texto de esta especificación; el HTML del mockup muestra un texto más corto que no se usa (AUD-MKP-claude-150). La lectura de **TBL_ORDENES_DEVOLUCION** es la lectura H4 autorizada por DEC-055. |

---

# Módulo 11 — Gestión de Lotes (EDU-0015)

| Código especificación | ESP-0041 |
| --- | --- |
| Nombre | Registro de remesa |
| Versión | 06.00 |
| Fecha | 11/11/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0041 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_PRODUCTOS, estado_producto = 'Disponible')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-LOT-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**producto = LOT-CMB-PRODUCTO**<br>&nbsp;&nbsp;**proveedor = LOT-CMB-PROVEEDOR-LOTE (VALOR INICIAL producto.id_proveedor)**<br>&nbsp;&nbsp;**numeroLote = LOT-TXT-NUMERO-LOTE**<br>&nbsp;&nbsp;**fechaVencimiento = LOT-FEC-FECHA-VENCIMIENTO**<br>&nbsp;&nbsp;**stock = LOT-NUM-STOCK-LOTE**<br>&nbsp;&nbsp;**fechaIngreso = LOT-FEC-FECHA-INGRESO**<br>&nbsp;&nbsp;**PRESIONAR LOT-BTN-CREAR-LOTE**<br>&nbsp;&nbsp;**VALIDAR producto EXISTE EN DB_FARMASIL.TBL_PRODUCTOS**<br>&nbsp;&nbsp;**VALIDAR proveedor EXISTE EN DB_FARMASIL.TBL_PROVEEDORES**<br>&nbsp;&nbsp;**VALIDAR numeroLote <> VACIO**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_LOTES, id_producto = producto Y numero_lote = numeroLote)**<br>&nbsp;&nbsp;**VALIDAR fechaVencimiento > FECHA_SISTEMA**<br>&nbsp;&nbsp;**VALIDAR stock = ENTERO > 0**<br>&nbsp;&nbsp;**VALIDAR fechaIngreso <= FECHA_SISTEMA**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (producto, numeroLote, fechaVencimiento, stock, 'Disponible', fechaIngreso, proveedor) EN DB_FARMASIL.TBL_LOTES**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR LOT-TBL-LOTES**<br>&nbsp;&nbsp;**MOSTRAR "Remesa registrada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR (id_producto, numero_lote) = UNICO EN DB_FARMASIL.TBL_LOTES**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.estado_lote = 'Disponible'**<br>&nbsp;&nbsp;**VERIFICAR REMESA DISPONIBLE EN VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;**VERIFICAR STOCK_FISICO(producto) = INCREMENTADO EN stock**<br>&nbsp;&nbsp;**VERIFICAR LOT-TBL-LOTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-LOT-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Especificación nueva, derivada de la incorporación de **TBL_LOTES** al Diccionario de Datos v04.00.<br>La validación de unicidad sobre la combinación de medicamento y número de lote es la que sostiene el rastreo sanitario: si la misma remesa pudiera registrarse dos veces, el sistema no podría responder inequívocamente qué se vendió de un lote retirado por la DIGEMID.<br>**Versión 02.00:** se incorpora `proveedor` (**LOT-CMB-PROVEEDOR-LOTE**) para registrar `TBL_LOTES.id_proveedor`, campo nuevo del Diccionario de Datos v05.00 (ver ILA-0041 v02.00).<br>**Versión 03.00:** las existencias iniciales deben ser mayores que cero. Una remesa registrada con cero quedaba 'Disponible' sin unidades, en contra de la regla de ESP-0001 que marca 'Agotado' toda remesa sin existencias.<br>**Versión 04.00:** (DEC-040) se renombra `STOCK_CONSOLIDADO` a `STOCK_FISICO`, la suma de `stock_actual` de todas las remesas del medicamento. Las existencias iniciales mayores que cero sostienen el `CHECK` de la remesa del Diccionario de Datos v10.00, que no admite 'Disponible' con cero unidades.<br>**Versión 05.00:** (DEC-042, C4) la operación se mantiene en **ART-MKP-LOT-0001**. `LOT-CMB-PROVEEDOR-LOTE` continúa siendo obligatorio aunque no esté representado de forma completa en el mockup; la especificación prevalece.<br>**Versión 06.00:** (issue #158, que completa la adecuación del #72) se restauran las notas de las versiones anteriores que la versión 05.00 había resumido: el fundamento de la unicidad, el origen del proveedor de la remesa y por qué las existencias iniciales deben ser mayores que cero y lo exige el `CHECK` estado/stock. No cambia el procedimiento. |

| Código especificación | ESP-0042 |
| --- | --- |
| Nombre | Consulta de remesas |
| Versión | 03.00 |
| Fecha | 11/11/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0042 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_LOTES <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-LOT-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**producto = LOT-CMB-FILTRO-PRODUCTO**<br>&nbsp;&nbsp;**numeroLote = LOT-SRH-NUMERO-LOTE**<br>&nbsp;&nbsp;**estado = LOT-CMB-FILTRO-ESTADO-LOTE**<br>&nbsp;&nbsp;**PRESIONAR LOT-BTN-LEER-LOTE**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_LOTES UNIENDO DB_FARMASIL.TBL_PRODUCTOS**<br>&nbsp;&nbsp;**APLICAR FILTRO id_producto = producto**<br>&nbsp;&nbsp;**APLICAR FILTRO numero_lote = numeroLote**<br>&nbsp;&nbsp;**APLICAR FILTRO estado_lote = estado**<br>&nbsp;&nbsp;**ORDENAR POR fecha_vencimiento ASCENDENTE**<br>&nbsp;&nbsp;**CARGAR LOT-CRD-REMESA EN LOT-PNL-REMESAS Y LOT-TBL-LOTES CON (nombre, numero_lote, fecha_vencimiento, stock_actual, estado_lote)**<br>&nbsp;&nbsp;**SI SELECCIONAR lote EN LOT-CRD-REMESA O EN UNA FILA DE LOT-TBL-LOTES ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CARGAR FICHA CON lote**<br>&nbsp;&nbsp;**FIN SI**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR LOT-PNL-REMESAS = ORDENADA POR fecha_vencimiento ASCENDENTE**<br>&nbsp;&nbsp;**VERIFICAR LOT-TBL-LOTES = ORDENADA POR fecha_vencimiento ASCENDENTE**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-LOT-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Es una de las operaciones disponibles para ambos roles: la técnica necesita saber qué remesa tiene disponible y cuándo vence antes de despachar.<br>El orden ascendente por vencimiento es la contraparte visual del criterio FEFO que aplica ESP-0001.<br>**Versión 02.00:** (DEC-042, C1-C4) la consulta se integra en **ART-MKP-LOT-0001**. Los controles de búsqueda reciben identificadores propios para no compartirlos con los campos de la ficha. La tabla conserva las columnas exigidas por la especificación: medicamento, número de lote, vencimiento, stock y estado, aunque el mockup no muestre todas explícitamente.<br>**Versión 03.00:** (issue #158, que completa la adecuación del #72; DEC-061, DEC-062) el filtro de texto pasa de **LOT-TXT-FILTRO-NUMERO-LOTE** a **LOT-SRH-NUMERO-LOTE**. El resultado se carga en las tarjetas **LOT-CRD-REMESA** de **LOT-PNL-REMESAS** y en **LOT-TBL-LOTES**; la selección, en tarjeta o en fila, carga la ficha. |

| Código especificación | ESP-0043 |
| --- | --- |
| Nombre | Actualización de remesa |
| Versión | 07.00 |
| Fecha | 11/11/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0043 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_LOTES <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-LOT-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR lote EN LOT-CRD-REMESA O EN UNA FILA DE LOT-TBL-LOTES**<br>&nbsp;&nbsp;**PRESIONAR LOT-BTN-ACTUALIZAR-LOTE**<br>&nbsp;&nbsp;**CARGAR ART-MKP-LOT-0001 CON INFORMACION VIGENTE**<br>&nbsp;&nbsp;**MOSTRAR lote.id_producto EN LOT-CMB-PRODUCTO = SOLO_LECTURA**<br>&nbsp;&nbsp;**numeroLote = LOT-TXT-NUMERO-LOTE**<br>&nbsp;&nbsp;**fechaVencimiento = LOT-FEC-FECHA-VENCIMIENTO**<br>&nbsp;&nbsp;**stock = LOT-NUM-STOCK-LOTE**<br>&nbsp;&nbsp;**MOSTRAR lote.estado_lote EN LOT-CMB-ESTADO-LOTE = SOLO_LECTURA**<br>&nbsp;&nbsp;**proveedor = LOT-CMB-PROVEEDOR-LOTE**<br>&nbsp;&nbsp;**PRESIONAR LOT-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_LOTES, id_producto = lote.id_producto Y numero_lote = numeroLote Y id_lote <> lote)**<br>&nbsp;&nbsp;**VALIDAR stock = ENTERO >= 0**<br>&nbsp;&nbsp;**SI fechaVencimiento <> lote.fecha_vencimiento ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VALIDAR fechaVencimiento > FECHA_SISTEMA**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**VALIDAR proveedor EXISTE EN DB_FARMASIL.TBL_PROVEEDORES**<br>&nbsp;&nbsp;**SI proveedor <> lote.id_proveedor Y EXISTE(DB_FARMASIL.TBL_DETALLE_DEVOLUCION, id_lote = lote) ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "El proveedor no puede cambiarse: la remesa figura en una orden de devolución"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**comprometido = SUMA(DB_FARMASIL.TBL_DETALLE_DEVOLUCION.cantidad_devolver DONDE id_lote = lote Y orden.estado_orden = 'Pendiente')**<br>&nbsp;&nbsp;**SI stock < comprometido ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Las existencias no pueden ser inferiores a las unidades comprometidas en devoluciones pendientes"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**estadoNuevo = ESTADO_LOTE_DERIVADO(lote.estado_lote, stock, fechaVencimiento)**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES SET numero_lote, fecha_vencimiento, stock_actual, id_proveedor, estado_lote = estadoNuevo DONDE id_lote = lote**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR LOT-PNL-REMESAS Y LOT-TBL-LOTES**<br>&nbsp;&nbsp;**MOSTRAR "Remesa actualizada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.id_lote = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LOTES.id_producto = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR REFERENCIAS(TBL_DETALLE_VENTAS, TBL_DETALLE_DEVOLUCION) = INTACTAS**<br>&nbsp;&nbsp;**VERIFICAR estado_lote = DERIVADO DE stock_actual Y DEL ESTADO PREVIO**<br>&nbsp;&nbsp;**VERIFICAR REMESA 'Bloqueado por devolucion' = SIN_CAMBIO_DE_ESTADO**<br>&nbsp;&nbsp;**VERIFICAR estado_lote <> 'Disponible' O stock_actual > 0**<br>&nbsp;&nbsp;**SI LA REMESA FIGURA EN ALGUN DETALLE DE DEVOLUCION, VERIFICAR id_proveedor = SIN_MODIFICACION**<br>&nbsp;&nbsp;**SI estadoNuevo <> 'Disponible' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR REMESA NO DISPONIBLE EN VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**VERIFICAR LOT-PNL-REMESAS = ACTUALIZADO**<br>&nbsp;&nbsp;**VERIFICAR LOT-TBL-LOTES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-LOT-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | El medicamento asociado se declara de solo lectura por la misma razón que el identificador: las ventas ya registradas apuntan a esta remesa, y reasignarla a otro medicamento reescribiría retroactivamente qué se vendió.<br>La verificación contra las unidades comprometidas evita que una corrección de inventario deje una orden de devolución reclamando existencias que ya no hay.<br>**Versión 02.00:** el proveedor de la remesa (**LOT-CMB-PROVEEDOR-LOTE**) es editable y se persiste en `TBL_LOTES.id_proveedor` mediante el `ACTUALIZAR` (ver ILA-0043 v02.00).<br>**Versión 03.00:** el estado de la remesa deja de ser editable. Sus transiciones las gobiernan la venta (ESP-0001), las alertas (ESP-0014 y ESP-0016) y la baja (ESP-0044); editarlo a mano permitía devolver a la venta una remesa bloqueada por alerta o vencida. Aquí solo se deriva de las existencias: una remesa 'Bloqueado por devolucion' conserva su estado; en otro caso, cero pasa a 'Agotado', y una remesa 'Agotado' no vencida que recibe existencias vuelve a 'Disponible'.<br>**Versión 04.00:** si se modifica la fecha de vencimiento, la nueva fecha debe ser posterior a la actual, igual que en ESP-0041. Se permite conservar sin cambios la fecha de una remesa ya vencida, para no impedir la corrección de otros campos.<br>**Versión 05.00:** (AUD-claude-001, 018; AUD-gpt-002, 007; DEC-034, DEC-038) esta especificación es el hogar de la regla única del estado de la remesa, que el resto invoca por su nombre. `ESTADO_LOTE_DERIVADO(estadoActual, stock, fechaVencimiento)`: 'Bloqueado por devolucion' conserva su estado (solo cambia por alertas y devoluciones); 'Disponible' con stock 0 pasa a 'Agotado'; 'Agotado' con stock mayor que 0 y no vencida pasa a 'Disponible'; en otro caso conserva su estado. `ACTUALIZAR_STOCK_LOTE(lote, nuevoStock)` fija `stock_actual` y `estado_lote = ESTADO_LOTE_DERIVADO(...)` en una sola sentencia, porque el `CHECK (estado_lote <> 'Disponible' OR stock_actual > 0)` no admite el estado intermedio. Además se rechaza cambiar `id_proveedor` si la remesa aparece en cualquier detalle de devolución, de cualquier estado: la devolución exige que el proveedor de cada línea sea el de la orden.<br>**Versión 06.00:** (DEC-042, C1 y C4) la actualización se integra en **ART-MKP-LOT-0001**. El medicamento asociado a la remesa permanece en solo lectura aunque el mockup lo represente como un selector editable; reasignarlo modificaría retroactivamente la trazabilidad de las ventas y devoluciones.<br>**Versión 07.00:** (issue #158, que completa la adecuación del #72; DEC-062) la remesa a actualizar se selecciona en una tarjeta **LOT-CRD-REMESA** o en una fila de **LOT-TBL-LOTES**, y el resultado se refleja en **LOT-PNL-REMESAS** y en la tabla. |

| Código especificación | ESP-0044 |
| --- | --- |
| Nombre | Baja de remesa |
| Versión | 06.00 |
| Fecha | 11/11/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código ilación | ILA-0044 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_LOTES <> VACIA**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_DETALLE_DEVOLUCION, id_lote = lote Y orden.estado_orden = 'Pendiente')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-LOT-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR lote EN LOT-CRD-REMESA O EN UNA FILA DE LOT-TBL-LOTES**<br>&nbsp;&nbsp;**PRESIONAR LOT-BTN-ELIMINAR-LOTE**<br>&nbsp;&nbsp;**SI EXISTE(DB_FARMASIL.TBL_DETALLE_DEVOLUCION, id_lote = lote Y orden.estado_orden = 'Pendiente') ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "La remesa está comprometida en una orden de devolución pendiente"**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CANCELAR OPERACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**MOSTRAR LOT-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR LOT-TXT-MSJ = "¿Está seguro de dar de baja esta remesa? El histórico de ventas que la involucra se conservará."**<br>&nbsp;&nbsp;**SI PRESIONAR LOT-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**resultado = INTENTAR ELIMINAR DB_FARMASIL.TBL_LOTES DONDE id_lote = lote**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**SI resultado = RECHAZADO_POR_REFERENCIAS ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LOTES SET estado_lote = 'Agotado', stock_actual = 0 DONDE id_lote = lote**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Remesa dada de baja correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR LOT-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR LOT-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR LOT-PNL-REMESAS Y LOT-TBL-LOTES**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR BAJA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR REMESA_CON_MOVIMIENTOS.estado_lote = 'Agotado'**<br>&nbsp;&nbsp;**VERIFICAR TRAZABILIDAD DE VENTAS POR LOTE = CONSERVADA**<br>&nbsp;&nbsp;**VERIFICAR REMESA NO DISPONIBLE EN VEN-CMB-LOTE-VENTA**<br>&nbsp;&nbsp;**VERIFICAR STOCK_FISICO(producto) = REDUCIDO**<br>&nbsp;&nbsp;**VERIFICAR INTEGRIDAD_REFERENCIAL = SIN_VIOLACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-LOT-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La baja lógica es obligatoria cuando la remesa registró ventas: eliminarla físicamente destruiría precisamente el rastreo sanitario que motivó la creación de este módulo.<br>**Versión 02.00:** Baja condicional según DEC-009 y DEC-029 (VP-B). El módulo ya no consulta tablas de otros módulos para decidirlo (RG-011), y la verificación cubre cualquier referencia, incluidas las que la versión anterior omitía. La verificación de devoluciones pendientes se conserva porque es una regla de negocio, no de integridad. Un retiro de `TBL_PRODUCTOS_RETIRADOS` no impide el borrado: su FK es `ON DELETE SET NULL`.<br>**Versión 03.00:** (DEC-040) se renombra `STOCK_CONSOLIDADO` a `STOCK_FISICO`.<br>**Versión 04.00:** la baja condicional se refiere a DEC-009 y DEC-029 (VP-B) en lugar de describir el mecanismo; DEC-029 corrige el fundamento de DEC-022.<br>**Versión 05.00:** (DEC-042, C1) la baja se ejecuta desde la pantalla única **ART-MKP-LOT-0001** y utiliza el modal de confirmación identificado por los componentes `LOT-MDL-*`.<br>**Versión 06.00:** (issue #158, que completa la adecuación del #72; DEC-062) la remesa a dar de baja se selecciona en una tarjeta **LOT-CRD-REMESA** o en una fila de **LOT-TBL-LOTES**, y el resultado se refleja en **LOT-PNL-REMESAS** y en la tabla. |

---

# Módulo 12 — Gestión de Reabastecimiento (EDU-0016)

| Código especificación | ESP-0045 |
| --- | --- |
| Nombre | Incorporación manual de ítem a la lista de reabastecimiento |
| Versión | 02.00 |
| Fecha | 13/10/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 3 |
| Experto | Ninguno |
| Código ilación | ILA-0045 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_PRODUCTOS, estado_producto = 'Disponible')**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REA-0001**<br>&nbsp;&nbsp;**VALIDAR REA-BTN-CREAR-ITEM = HABILITADO**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**producto = REA-CMB-PRODUCTO**<br>&nbsp;&nbsp;**prioridad = REA-CMB-PRIORIDAD**<br>&nbsp;&nbsp;**fechaInclusion = FECHA_SISTEMA**<br>&nbsp;&nbsp;**MOSTRAR fechaInclusion EN REA-LBL-FECHA-INCLUSION**<br>&nbsp;&nbsp;**PRESIONAR REA-BTN-CREAR-ITEM**<br>&nbsp;&nbsp;**VALIDAR producto EXISTE EN DB_FARMASIL.TBL_PRODUCTOS**<br>&nbsp;&nbsp;**VALIDAR NO EXISTE(DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO, id_producto = producto)**<br>&nbsp;&nbsp;**VALIDAR prioridad EN ('Normal', 'Urgente')**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (producto, 'Manual', prioridad, fechaInclusion) EN DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR REA-TBL-LISTA-REABASTECIMIENTO**<br>&nbsp;&nbsp;**MOSTRAR "Ítem agregado a la lista de reabastecimiento"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO.origen = 'Manual'**<br>&nbsp;&nbsp;**VERIFICAR REA-TBL-LISTA-REABASTECIMIENTO = ACTUALIZADA**<br>&nbsp;&nbsp;**SI prioridad = 'Urgente' ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**VERIFICAR ITEM RESALTADO EN ROJO**<br>&nbsp;&nbsp;**FIN SI**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-REA-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Especificación nueva, derivada de la Entrevista 3. Complementa la incorporación automática del proceso periódico del sistema (condición 2 de RNF-0010, versión 02.00).<br>Queda Pendiente porque el mockup **ART-MKP-REA-0001** aún no ha sido diseñado. |

| Código especificación | ESP-0046 |
| --- | --- |
| Nombre | Consulta de la lista de reabastecimiento |
| Versión | 01.00 |
| Fecha | 02/10/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 3 |
| Experto | Ninguno |
| Código ilación | ILA-0046 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REA-0002**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**prioridad = REA-CMB-FILTRO-PRIORIDAD**<br>&nbsp;&nbsp;**origen = REA-CMB-FILTRO-ORIGEN**<br>&nbsp;&nbsp;**PRESIONAR REA-BTN-LEER-ITEM**<br>&nbsp;&nbsp;**CONSULTAR DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO UNIENDO DB_FARMASIL.TBL_PRODUCTOS**<br>&nbsp;&nbsp;**APLICAR FILTRO prioridad = prioridad**<br>&nbsp;&nbsp;**APLICAR FILTRO origen = origen**<br>&nbsp;&nbsp;**ORDENAR POR prioridad = 'Urgente' DESCENDENTE**<br>&nbsp;&nbsp;**CARGAR REA-TBL-LISTA-REABASTECIMIENTO CON (nombre, origen, prioridad, fecha_inclusion)**<br>&nbsp;&nbsp;**RESALTAR EN ROJO FILAS DONDE prioridad = 'Urgente'**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR REA-TBL-LISTA-REABASTECIMIENTO = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-REA-0001, ART-MKP-REA-0002 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Es la vista desde la que la dueña decide qué ítems enviar a cotizar según EDU-0017. Queda Pendiente por la dependencia del mockup **ART-MKP-REA-0002**. |

| Código especificación | ESP-0047 |
| --- | --- |
| Nombre | Actualización de ítem de la lista de reabastecimiento |
| Versión | 01.00 |
| Fecha | 02/10/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 3 |
| Experto | Ninguno |
| Código ilación | ILA-0047 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REA-0003**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR item EN REA-TBL-LISTA-REABASTECIMIENTO**<br>&nbsp;&nbsp;**PRESIONAR REA-BTN-ACTUALIZAR-ITEM**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REA-0003 CON INFORMACION VIGENTE**<br>&nbsp;&nbsp;**MOSTRAR item.id_producto EN REA-CMB-PRODUCTO = SOLO_LECTURA**<br>&nbsp;&nbsp;**MOSTRAR item.origen EN REA-LBL-ORIGEN = SOLO_LECTURA**<br>&nbsp;&nbsp;**prioridad = REA-CMB-PRIORIDAD**<br>&nbsp;&nbsp;**PRESIONAR REA-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR prioridad EN ('Normal', 'Urgente')**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO.prioridad = prioridad DONDE id_reabastecimiento = item**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR REA-TBL-LISTA-REABASTECIMIENTO**<br>&nbsp;&nbsp;**MOSTRAR "Ítem actualizado correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO.id_producto = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO.origen = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR REA-TBL-LISTA-REABASTECIMIENTO = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-REA-0001, ART-MKP-REA-0003 |
| Importancia | Media |
| Estado | Pendiente |
| Comentario | El medicamento y el origen se declaran de solo lectura: reasignar la identidad de un ítem ya existente equivaldría a crear uno nuevo. Queda Pendiente por la dependencia del mockup **ART-MKP-REA-0003**. |

| Código especificación | ESP-0048 |
| --- | --- |
| Nombre | Eliminación de ítem de la lista de reabastecimiento |
| Versión | 01.00 |
| Fecha | 02/10/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 3 |
| Experto | Ninguno |
| Código ilación | ILA-0048 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-REA-0004**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR item EN REA-TBL-LISTA-REABASTECIMIENTO**<br>&nbsp;&nbsp;**PRESIONAR REA-BTN-ELIMINAR-ITEM**<br>&nbsp;&nbsp;**MOSTRAR REA-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR REA-TXT-MSJ = "¿Está seguro de quitar este ítem de la lista de reabastecimiento?"**<br>&nbsp;&nbsp;**SI PRESIONAR REA-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO DONDE id_reabastecimiento = item**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Ítem eliminado correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR REA-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR REA-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR REA-TBL-LISTA-REABASTECIMIENTO**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ELIMINADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR REA-TBL-LISTA-REABASTECIMIENTO = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR INTEGRIDAD_REFERENCIAL = SIN_VIOLACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-REA-0001, ART-MKP-REA-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Eliminación física: ninguna otra tabla depende de un ítem de esta lista (las cotizaciones de EDU-0017 se asocian al medicamento y al proveedor, no al ítem). Queda Pendiente por la dependencia del mockup **ART-MKP-REA-0004**. |

---

# Módulo 13 — Gestión de Comparación de Precios de Proveedores (EDU-0017)

| Código especificación | ESP-0049 |
| --- | --- |
| Nombre | Registro de cotización de proveedor |
| Versión | 01.00 |
| Fecha | 02/10/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 3 |
| Experto | Ninguno |
| Código ilación | ILA-0049 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR EXISTE(DB_FARMASIL.TBL_PROVEEDORES, estado = 'Activo')**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_PRODUCTOS <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-COT-0001**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**producto = COT-CMB-PRODUCTO**<br>&nbsp;&nbsp;**proveedor = COT-CMB-PROVEEDOR**<br>&nbsp;&nbsp;**precio = COT-NUM-PRECIO-COTIZADO**<br>&nbsp;&nbsp;**fechaCotizacion = COT-FEC-FECHA-COTIZACION**<br>&nbsp;&nbsp;**promocion = COT-TXA-PROMOCION**<br>&nbsp;&nbsp;**PRESIONAR COT-BTN-CREAR-COTIZACION**<br>&nbsp;&nbsp;**VALIDAR precio = DECIMAL >= 0**<br>&nbsp;&nbsp;**VALIDAR fechaCotizacion <= FECHA_SISTEMA**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**REGISTRAR (producto, proveedor, precio, fechaCotizacion, promocion, SESION_USUARIO.id_usuario) EN DB_FARMASIL.TBL_COTIZACIONES_PROVEEDOR**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR COT-TBL-COTIZACIONES**<br>&nbsp;&nbsp;**MOSTRAR "Cotización registrada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_CREADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_COTIZACIONES_PROVEEDOR = FILA_NUEVA_SIN_SOBRESCRIBIR**<br>&nbsp;&nbsp;**VERIFICAR COT-TBL-COTIZACIONES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-COT-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La obtención de precios es manual (teléfono, visita presencial o catálogo en PDF/CSV); no hay integración con proveedores. Cada registro es una fila nueva, nunca se sobrescribe una cotización anterior. Queda Pendiente porque el mockup **ART-MKP-COT-0001** aún no ha sido diseñado. |

| Código especificación | ESP-0050 |
| --- | --- |
| Nombre | Consulta comparativa de precios y exportación por proveedor |
| Versión | 02.00 |
| Fecha | 08/11/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 3 |
| Experto | Ninguno |
| Código ilación | ILA-0050 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_COTIZACIONES_PROVEEDOR <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-COT-0002**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**producto = COT-CMB-FILTRO-PRODUCTO**<br>&nbsp;&nbsp;**PRESIONAR COT-BTN-LEER-COTIZACION**<br>&nbsp;&nbsp;**cotizaciones = CONSULTAR DB_FARMASIL.TBL_COTIZACIONES_PROVEEDOR**<br>&nbsp;&nbsp;**APLICAR FILTRO id_producto = producto**<br>&nbsp;&nbsp;**PARA CADA (producto, proveedor) EN cotizaciones HACER**<br>&nbsp;&nbsp;&nbsp;&nbsp;**vigente = COTIZACION CON fecha_cotizacion = MAXIMA(fecha_cotizacion) PARA (producto, proveedor) Y, SI HAY EMPATE, id_cotizacion = MAXIMO**<br>&nbsp;&nbsp;&nbsp;&nbsp;**AGREGAR vigente A comparativa**<br>&nbsp;&nbsp;**FIN PARA**<br>&nbsp;&nbsp;**CARGAR COT-TBL-COMPARATIVA-PRECIOS CON comparativa**<br>&nbsp;&nbsp;**DESTACAR EN comparativa EL PRECIO MAS BAJO POR FILA**<br>&nbsp;&nbsp;**proveedorExportar = COT-CMB-PROVEEDOR-EXPORTAR**<br>&nbsp;&nbsp;**PRESIONAR COT-BTN-EXPORTAR-PROVEEDOR**<br>&nbsp;&nbsp;**itemsProveedor = CONSULTAR DB_FARMASIL.TBL_LISTA_REABASTECIMIENTO UNIENDO comparativa DONDE proveedor_seleccionado = proveedorExportar**<br>&nbsp;&nbsp;**archivoCsv = GENERAR_ARCHIVO_CSV(itemsProveedor)**<br>&nbsp;&nbsp;**PONER_A_DISPOSICION archivoCsv**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR CONSULTA_REALIZADA = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR COT-TBL-COMPARATIVA-PRECIOS = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR archivoCsv CONTIENE SOLO ITEMS DE proveedorExportar**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL = SIN_MODIFICACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-COT-0001, ART-MKP-COT-0002 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La exportación es efímera y sin historial (decisión confirmada con la dueña): no se registra ninguna fila nueva en `DB_FARMASIL` por el hecho de exportar, a diferencia de `TBL_REPORTES_VENTAS` en EDU-0010. Queda Pendiente porque el mockup **ART-MKP-COT-0002** aún no ha sido diseñado.<br>**Versión 02.00:** (AUD-claude-022, AUD-gpt-013; DEC-039, PEN-023) si hay dos cotizaciones del mismo día para el mismo medicamento y proveedor, la vigente es la de mayor `id_cotizacion`. El desempate representa el registro más reciente, no necesariamente la cotización comercial posterior. Es provisional hasta que la dueña responda. |

| Código especificación | ESP-0051 |
| --- | --- |
| Nombre | Actualización de cotización |
| Versión | 01.00 |
| Fecha | 02/10/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 3 |
| Experto | Ninguno |
| Código ilación | ILA-0051 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_COTIZACIONES_PROVEEDOR <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-COT-0003**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR cotizacion EN COT-TBL-COTIZACIONES**<br>&nbsp;&nbsp;**PRESIONAR COT-BTN-ACTUALIZAR-COTIZACION**<br>&nbsp;&nbsp;**CARGAR ART-MKP-COT-0003 CON INFORMACION VIGENTE**<br>&nbsp;&nbsp;**MOSTRAR cotizacion.id_producto EN COT-CMB-PRODUCTO = SOLO_LECTURA**<br>&nbsp;&nbsp;**MOSTRAR cotizacion.id_proveedor EN COT-CMB-PROVEEDOR = SOLO_LECTURA**<br>&nbsp;&nbsp;**precio = COT-NUM-PRECIO-COTIZADO**<br>&nbsp;&nbsp;**fechaCotizacion = COT-FEC-FECHA-COTIZACION**<br>&nbsp;&nbsp;**promocion = COT-TXA-PROMOCION**<br>&nbsp;&nbsp;**PRESIONAR COT-BTN-CONFIRMAR-ACTUALIZACION**<br>&nbsp;&nbsp;**VALIDAR precio = DECIMAL >= 0**<br>&nbsp;&nbsp;**VALIDAR fechaCotizacion <= FECHA_SISTEMA**<br>&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**ACTUALIZAR DB_FARMASIL.TBL_COTIZACIONES_PROVEEDOR DONDE id_cotizacion = cotizacion**<br>&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;**ACTUALIZAR COT-TBL-COTIZACIONES**<br>&nbsp;&nbsp;**MOSTRAR "Cotización actualizada correctamente"**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ACTUALIZADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_COTIZACIONES_PROVEEDOR.id_producto = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR DB_FARMASIL.TBL_COTIZACIONES_PROVEEDOR.id_proveedor = SIN_MODIFICACION**<br>&nbsp;&nbsp;**VERIFICAR COT-TBL-COTIZACIONES = ACTUALIZADA**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-COT-0001, ART-MKP-COT-0003 |
| Importancia | Media |
| Estado | Pendiente |
| Comentario | Corrige un error de digitación en una cotización ya registrada; no sustituye al registro de una cotización nueva (ESP-0049), que es lo que preserva el historial. El medicamento y el proveedor se declaran de solo lectura. Queda Pendiente por la dependencia del mockup **ART-MKP-COT-0003**. |

| Código especificación | ESP-0052 |
| --- | --- |
| Nombre | Eliminación de cotización |
| Versión | 01.00 |
| Fecha | 02/10/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 3 |
| Experto | Ninguno |
| Código ilación | ILA-0052 |
| Precondición | **INICIO**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL = DISPONIBLE**<br>&nbsp;&nbsp;**VALIDAR CONEXION(DB_FARMASIL) = EXITOSA**<br>&nbsp;&nbsp;**VALIDAR SESION_USUARIO = ACTIVA**<br>&nbsp;&nbsp;**VALIDAR ROL(SESION_USUARIO) = 'Administrador'**<br>&nbsp;&nbsp;**VALIDAR DB_FARMASIL.TBL_COTIZACIONES_PROVEEDOR <> VACIA**<br>&nbsp;&nbsp;**CARGAR ART-MKP-COT-0004**<br>**FIN** |
| Procedimiento | **INICIO**<br>&nbsp;&nbsp;**SELECCIONAR cotizacion EN COT-TBL-COTIZACIONES**<br>&nbsp;&nbsp;**PRESIONAR COT-BTN-ELIMINAR-COTIZACION**<br>&nbsp;&nbsp;**MOSTRAR COT-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**MOSTRAR COT-TXT-MSJ = "¿Está seguro de eliminar esta cotización? Se perderá del historial de precios de este proveedor."**<br>&nbsp;&nbsp;**SI PRESIONAR COT-BTN-CONFIRMAR-SI ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**INICIAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;**ELIMINAR DB_FARMASIL.TBL_COTIZACIONES_PROVEEDOR DONDE id_cotizacion = cotizacion**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CONFIRMAR TRANSACCION**<br>&nbsp;&nbsp;&nbsp;&nbsp;**MOSTRAR "Cotización eliminada correctamente"**<br>&nbsp;&nbsp;**SINO SI PRESIONAR COT-BTN-CONFIRMAR-NO ENTONCES**<br>&nbsp;&nbsp;&nbsp;&nbsp;**CERRAR COT-MDL-CONFIRMAR-ELIMINACION**<br>&nbsp;&nbsp;**FIN SI**<br>&nbsp;&nbsp;**ACTUALIZAR COT-TBL-COTIZACIONES**<br>**FIN** |
| Postcondición | **INICIO**<br>&nbsp;&nbsp;**VERIFICAR REGISTRO_ELIMINADO = VERDADERO**<br>&nbsp;&nbsp;**VERIFICAR COT-TBL-COTIZACIONES = ACTUALIZADA**<br>&nbsp;&nbsp;**VERIFICAR INTEGRIDAD_REFERENCIAL = SIN_VIOLACION**<br>**FIN** |
| Código de artefactos asociados | ART-MKP-COT-0001, ART-MKP-COT-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Eliminación física: una cotización incorrecta (por ejemplo, cargada para el proveedor equivocado) no debe conservarse como historial legítimo. Queda Pendiente por la dependencia del mockup **ART-MKP-COT-0004**. |



> **Documento consolidado.** Reúne las 40 especificaciones del catálogo en un solo archivo, reemplazando a `Especificaciones.md`, `Especificaciones_Modulos_1_3_5_7.md` y `Especificaciones_Modulos_2_4_6_8.md`.
>
> Cada especificación traduce a pseudocódigo la ilación correspondiente del archivo `02-ilaciones.md`, y opera sobre el esquema definido en el Diccionario de Datos v03.00 con los componentes de interfaz de la Guía de Estilo de Nomenclatura v03.00.
>
> **Documento completo.** Contiene las 44 especificaciones, ESP-0001 a ESP-0044, correspondientes a los once módulos del catálogo.
>
> **Ampliación del 05/09/2026 (lotes).** Con la incorporación de `TBL_LOTES` al Diccionario de Datos v04.00 se agregaron ESP-0041 a ESP-0044 y se ajustaron las dieciséis especificaciones de los módulos 1, 2, 4 y 7. Los cambios de fondo: el detalle de venta registra `id_lote` y el descuento de stock opera sobre la remesa; la selección de remesa sigue el criterio FEFO; el bloqueo por vencimiento pasa de `estado_producto` a `estado_lote`; y las líneas de devolución identifican la remesa que el proveedor exige.

> **Ampliación del 02/10/2026 (reabastecimiento y comparación de precios).** Se agregaron ESP-0045 a ESP-0052 (módulos 12 y 13), derivadas de la Entrevista 3; el catálogo suma ahora 52 especificaciones. Se corrigió ESP-0025 (v03.00: valida `lote.id_proveedor` y no el proveedor del producto) y se ampliaron ESP-0005, ESP-0007, ESP-0041 y ESP-0043 para capturar `stock_minimo` y `TBL_LOTES.id_proveedor`, según el Diccionario de Datos v05.00.

> **Corrección del 08/11/2026 (auditorías de la base de datos, issue #63).** Se ajustaron ESP-0001 a ESP-0004, ESP-0006, ESP-0009, ESP-0011, ESP-0013, ESP-0015, ESP-0016, ESP-0021, ESP-0023, ESP-0025, ESP-0027, ESP-0041, ESP-0043, ESP-0044 y ESP-0050. Cambios de fondo: predicado de vendibilidad (DEC-033); regla única del estado de la remesa (DEC-034); remesa de un retiro de solo lectura (DEC-035); una línea por remesa y todas las restricciones activas (DEC-036); intervalos de fecha semiabiertos (DEC-037); proveedor de las devoluciones (DEC-038); desempate de la cotización vigente (DEC-039); stock físico y stock vendible (DEC-040); y la aplicación de DEC-017 a ESP-0009 y ESP-0011.

> **Adecuación del 11/11/2026 (issue #72, INV y LOT).** Se aplican los criterios C1–C4 de DEC-042 a Inventario y Lotes. Todas las operaciones de ambos módulos se integran en `ART-MKP-INV-0001` y `ART-MKP-LOT-0001`; se separan los identificadores de filtros y campos de captura; los filtros visibles se incorporan a las consultas; el filtro de categoría de Inventario usa provisionalmente `accion_terapeutica` (PEN-010); el visor 3D queda fuera de alcance (PEN-027); ESP-0006 obtiene el stock físico mediante `VW_STOCK_FISICO`; ESP-0007 adopta `INV-BTN-CONFIRMAR-ACTUALIZACION` (DEC-046); y el medicamento de una remesa permanece en solo lectura durante la actualización.

## Convenciones aplicadas

- Componentes de mockup: guion `-` (`VEN-BTN-CREAR-VENTA`). Tablas y campos de base de datos: guion bajo `_`, con punto separador (`DB_FARMASIL.TBL_PRODUCTOS`).
- Pseudocódigo indentado por nivel de anidamiento: `INICIO`/`FIN`, `INICIAR TRANSACCION`/`CONFIRMAR TRANSACCION`, `SI`/`FIN SI`, `PARA CADA`/`FIN PARA`.
- Toda operación que escriba en más de una tabla se ejecuta dentro de una transacción, para que no queden cabeceras sin detalle ni stock descontado sin venta.
- El campo Fuente hereda la fuente de la ilación de la que deriva la especificación. En la versión anterior las dieciséis decían `Ninguno`, lo que rompía la cadena de trazabilidad hacia la fuente original.
- El campo Estado replica el de su ilación: `Pendiente` cuando la operación depende de un componente de mockup por diseñar o de una decisión externa, `Concluido` en caso contrario.