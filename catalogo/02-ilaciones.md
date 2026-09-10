# Ilaciones de Requisitos — FARMASIL

# Módulo 1 — Gestión de ventas (EDU-0001)

| Código ilación | ILA-0001 |
| --- | --- |
| Nombre | Creación del registro de venta |
| Versión | 06.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código educción | EDU-0001 |
| Código especificación | ESP-0001 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador' o 'Tecnico'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_LOTES** tiene al menos una remesa con `estado_lote` = 'Disponible' y `stock_actual` mayor a cero.<br>5. La tabla **DB_FARMASIL.TBL_METODOS_PAGO** tiene al menos un método con `estado` = 'Activo'.<br>6. Se carga el mockup **ART-MKP-VEN-0001**. |
| Procedimiento | 1. El usuario accede al módulo de ventas y visualiza el mockup **ART-MKP-VEN-0001**.<br>2. El sistema registra automáticamente la fecha y hora de la transacción y la muestra en **VEN-LBL-FECHA-VENTA**, que no es editable.<br>3. El usuario selecciona un medicamento en **VEN-CMB-PRODUCTO-VENTA**, que lista únicamente medicamentos con al menos una remesa disponible y con existencias.<br>4. El sistema propone en **VEN-CMB-LOTE-VENTA** la remesa disponible con la fecha de vencimiento más próxima, según el criterio FEFO, y permite al usuario elegir otra cuando la caja física que tiene en la mano es distinta.<br>5. El usuario ingresa la cantidad en **VEN-NUM-CANTIDAD-VENTA**.<br>6. El usuario presiona **VEN-BTN-AGREGAR-PRODUCTO**.<br>7. El sistema valida que la cantidad sea un entero positivo y no exceda el `stock_actual` de la remesa seleccionada, recupera el `precio_venta` del medicamento y agrega la línea a **VEN-TBL-DETALLE-VENTA**.<br>8. El sistema verifica el medicamento contra **DB_FARMASIL.TBL_RESTRICCIONES_VENTA** y la remesa contra su `estado_lote`. Si existe una restricción de tipo 'Bloqueante' o la remesa no está disponible, el sistema impide agregar la línea y muestra el motivo.<br>9. El usuario repite los pasos 3 a 8 por cada medicamento que forme parte de la venta.<br>10. El usuario puede retirar una línea agregada por error seleccionándola y presionando **VEN-BTN-QUITAR-PRODUCTO**.<br>11. El sistema calcula el monto total como la suma de cantidad por precio unitario de todas las líneas y lo muestra en **VEN-LBL-MONTO-TOTAL**.<br>12. El usuario selecciona el método de pago en **VEN-CMB-METODO-PAGO**.<br>13. El usuario presiona **VEN-BTN-CREAR-VENTA**.<br>14. El sistema valida que exista al menos una línea de detalle y que se haya seleccionado un método de pago.<br>15. El sistema inicia una transacción, registra la cabecera en **DB_FARMASIL.TBL_REGISTRO_VENTAS**, registra una fila por cada línea en **DB_FARMASIL.TBL_DETALLE_VENTAS** con el `id_lote` correspondiente, descuenta las cantidades vendidas del `stock_actual` en **DB_FARMASIL.TBL_LOTES**, marca como 'Agotado' toda remesa cuyo stock quede en cero y confirma la transacción.<br>16. El sistema actualiza **VEN-TBL-REGISTRO-VENTAS** y notifica el registro exitoso. |
| Postcondición | 1. La cabecera de la venta queda almacenada en **DB_FARMASIL.TBL_REGISTRO_VENTAS** con su `monto_total` igual a la suma del detalle.<br>2. Existe una fila en **DB_FARMASIL.TBL_DETALLE_VENTAS** por cada remesa vendida, identificada por su `id_lote`, con la cantidad y el precio unitario vigentes al momento de la venta.<br>3. El `stock_actual` de cada remesa vendida queda descontado en **DB_FARMASIL.TBL_LOTES**.<br>4. La venta se despliega en **VEN-TBL-REGISTRO-VENTAS**.<br>5. La venta queda disponible para la emisión de su comprobante tributario según ILA-0009.<br>6. La venta queda vinculada a las remesas exactas que salieron del estante, lo que hace posible el rastreo sanitario ante un retiro de lote de la DIGEMID. |
| Código de artefactos asociados | ART-MKP-VEN-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Reescrita para soportar la venta de varios productos en una misma transacción, operando sobre la cabecera **TBL_REGISTRO_VENTAS** y su detalle **TBL_DETALLE_VENTAS**. La versión anterior registraba un solo producto por venta e ignoraba la tabla de detalle, que ya existía en el modelo.<br>Se incorporó el descuento de stock, ausente en toda la versión anterior del catálogo, y la verificación cruzada contra restricciones de venta (EDU-0012) y alertas de vencimiento (EDU-0004), que hasta ahora quedaban como módulos aislados sin efecto sobre la venta.<br>Queda en estado Pendiente porque requiere incorporar al mockup **ART-MKP-VEN-0001** los componentes **VEN-BTN-AGREGAR-PRODUCTO**, **VEN-BTN-QUITAR-PRODUCTO**, **VEN-TBL-DETALLE-VENTA** y **VEN-LBL-MONTO-TOTAL**. Ver Anexo. |

| Código ilación | ILA-0002 |
| --- | --- |
| Nombre | Consulta del registro de venta |
| Versión | 06.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código educción | EDU-0001 |
| Código especificación | ESP-0002 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador' o 'Tecnico'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_REGISTRO_VENTAS** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-VEN-0001**. |
| Procedimiento | 1. El usuario presiona **VEN-BTN-LEER-VENTA**.<br>2. El sistema carga el mockup **ART-MKP-VEN-0002**.<br>3. El usuario aplica los filtros de búsqueda disponibles: fecha en **VEN-FEC-FECHA-VENTA** y producto en **VEN-CMB-PRODUCTO-VENTA**.<br>4. El usuario presiona **VEN-BTN-LEER-VENTA**.<br>5. El sistema valida los criterios ingresados.<br>6. El sistema consulta **DB_FARMASIL.TBL_REGISTRO_VENTAS** y, cuando el filtro incluye un medicamento, la resuelve contra **DB_FARMASIL.TBL_DETALLE_VENTAS** y **DB_FARMASIL.TBL_LOTES**.<br>7. El sistema despliega las cabeceras encontradas en **VEN-TBL-REGISTRO-VENTAS**.<br>8. Al seleccionar una venta, el sistema muestra los productos que la componen, con el número de lote de cada línea, en **VEN-TBL-DETALLE-VENTA**. |
| Postcondición | 1. El sistema devuelve los registros correspondientes a los criterios de búsqueda aplicados en **VEN-TBL-REGISTRO-VENTAS**.<br>2. El detalle de productos de la venta seleccionada se muestra en **VEN-TBL-DETALLE-VENTA**.<br>3. Ningún dato es modificado en **DB_FARMASIL**. |
| Código de artefactos asociados | ART-MKP-VEN-0001, ART-MKP-VEN-0002 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Se corrigió el estado, que decía "conluido". Se agregó la resolución del filtro por producto contra la tabla de detalle y la visualización del detalle de la venta seleccionada, coherente con la venta de varios productos introducida en ILA-0001.<br>Queda Pendiente por depender del componente **VEN-TBL-DETALLE-VENTA**. Ver Anexo. |

| Código ilación | ILA-0003 |
| --- | --- |
| Nombre | Actualización del registro de venta |
| Versión | 05.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código educción | EDU-0001 |
| Código especificación | ESP-0003 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_REGISTRO_VENTAS** tiene entradas válidas.<br>5. La venta seleccionada no tiene un comprobante tributario asociado en estado 'Emitido' en **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS**.<br>6. Se carga el mockup **ART-MKP-VEN-0001**. |
| Procedimiento | 1. El usuario selecciona un registro de venta en **VEN-TBL-REGISTRO-VENTAS**.<br>2. El usuario presiona **VEN-BTN-ACTUALIZAR-VENTA**.<br>3. El sistema verifica que la venta no tenga comprobante emitido. Si lo tiene, cancela la operación e indica que la venta debe anularse mediante ILA-0004.<br>4. El sistema carga el mockup **ART-MKP-VEN-0003** con la cabecera y el detalle actuales de la venta.<br>5. El usuario modifica el método de pago en **VEN-CMB-METODO-PAGO**, la cantidad de una línea en **VEN-NUM-CANTIDAD-VENTA**, agrega productos con **VEN-BTN-AGREGAR-PRODUCTO** o retira líneas con **VEN-BTN-QUITAR-PRODUCTO**.<br>6. El sistema recalcula el monto total y lo muestra en **VEN-LBL-MONTO-TOTAL**.<br>7. El usuario presiona **VEN-BTN-CONFIRMAR-ACTUALIZACION**.<br>8. El sistema valida las cantidades contra el `stock_actual` de la remesa correspondiente.<br>9. El sistema inicia una transacción, actualiza la cabecera en **DB_FARMASIL.TBL_REGISTRO_VENTAS**, sincroniza las líneas en **DB_FARMASIL.TBL_DETALLE_VENTAS**, aplica sobre **DB_FARMASIL.TBL_LOTES** la diferencia de stock resultante entre el detalle anterior y el nuevo, y confirma la transacción.<br>10. El sistema actualiza **VEN-TBL-REGISTRO-VENTAS** y notifica la actualización exitosa. |
| Postcondición | 1. La cabecera y el detalle de la venta quedan actualizados de forma consistente en **DB_FARMASIL.TBL_REGISTRO_VENTAS** y **DB_FARMASIL.TBL_DETALLE_VENTAS**.<br>2. El `monto_total` refleja la suma del nuevo detalle.<br>3. El `stock_actual` de las remesas afectadas queda ajustado en **DB_FARMASIL.TBL_LOTES** por la diferencia entre el detalle anterior y el nuevo.<br>4. La información actualizada se muestra en **VEN-TBL-REGISTRO-VENTAS**.<br>5. Ninguna venta con comprobante tributario emitido resulta modificada. |
| Código de artefactos asociados | ART-MKP-VEN-0001, ART-MKP-VEN-0003 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Se restauró la numeración de los pasos, que se reiniciaba a mitad del procedimiento, y se corrigieron los componentes **VEN-TXT-FECHA**, **VEN-TXT-PRODUCTO** y **VEN-CANTIDAD**, que no respetaban la nomenclatura.<br>Se agregó la restricción de no modificar ventas con comprobante tributario emitido y el ajuste diferencial de stock, ambos ausentes en la versión anterior. |

| Código ilación | ILA-0004 |
| --- | --- |
| Nombre | Eliminación del registro de venta |
| Versión | 06.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0001 |
| Experto | Ninguno |
| Código educción | EDU-0001 |
| Código especificación | ESP-0004 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_REGISTRO_VENTAS** tiene entradas válidas.<br>5. La venta seleccionada no tiene un comprobante tributario asociado en estado 'Emitido' en **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS**.<br>6. **VEN-TBL-REGISTRO-VENTAS** está habilitada para seleccionar una entrada. |
| Procedimiento | 1. El usuario selecciona un registro de venta en **VEN-TBL-REGISTRO-VENTAS**.<br>2. El usuario presiona **VEN-BTN-ELIMINAR-VENTA**.<br>3. El sistema verifica que la venta no tenga comprobante tributario emitido. Si lo tiene, cancela la operación e indica que el comprobante debe anularse primero según ILA-0012.<br>4. El sistema carga el mockup **ART-MKP-VEN-0004** y muestra el modal **VEN-MDL-CONFIRMAR-ELIMINACION** con el mensaje **VEN-TXT-MSJ**: "¿Está seguro de eliminar este registro de venta? El stock de los productos será restituido."<br>5. Si el usuario presiona **VEN-BTN-CONFIRMAR-SI**, el sistema inicia una transacción, restituye al `stock_actual` de **DB_FARMASIL.TBL_LOTES** las cantidades de cada línea, elimina las líneas asociadas en **DB_FARMASIL.TBL_DETALLE_VENTAS**, elimina la cabecera en **DB_FARMASIL.TBL_REGISTRO_VENTAS** y confirma la transacción.<br>6. Si el usuario presiona **VEN-BTN-CONFIRMAR-NO**, el sistema cierra el modal sin realizar cambios.<br>7. El sistema actualiza **VEN-TBL-REGISTRO-VENTAS**. |
| Postcondición | 1. La cabecera de la venta y todas sus líneas de detalle quedan eliminadas de **DB_FARMASIL.TBL_REGISTRO_VENTAS** y **DB_FARMASIL.TBL_DETALLE_VENTAS**, sin dejar detalle huérfano.<br>2. El `stock_actual` de las remesas involucradas queda restituido en **DB_FARMASIL.TBL_LOTES**.<br>3. La venta deja de mostrarse en **VEN-TBL-REGISTRO-VENTAS**.<br>4. Ninguna venta con comprobante tributario emitido resulta eliminada. |
| Código de artefactos asociados | ART-MKP-VEN-0001, ART-MKP-VEN-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | El procedimiento cargaba **ART-MKP-VEN-0003** mientras la fila de artefactos declaraba **ART-MKP-VEN-0004**; se corrigió a favor del mockup de eliminación.<br>Se agregó la eliminación en cascada del detalle, que de otro modo quedaba huérfano y rompía la integridad referencial, la restitución del stock y el bloqueo de la eliminación cuando existe comprobante tributario emitido. |

---

# Módulo 2 — Gestión de inventario (EDU-0002)

| Código ilación | ILA-0005 |
| --- | --- |
| Nombre | Creación del producto de inventario |
| Versión | 08.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código educción | EDU-0002 |
| Código especificación | ESP-0005 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_PROVEEDORES** tiene al menos un proveedor con `estado` = 'Activo'.<br>5. Se carga el mockup **ART-MKP-INV-0001**.<br>6. Los campos **INV-TXT-NOMBRE-PRODUCTO**, **INV-TXT-ACCION-TERAPEUTICA**, **INV-NUM-PRECIO-PRODUCTO** e **INV-CMB-PROVEEDOR** se encuentran habilitados.<br>7. El botón **INV-BTN-CREAR-PRODUCTO** se encuentra habilitado. |
| Procedimiento | 1. El usuario visualiza el formulario de gestión de inventario en el mockup **ART-MKP-INV-0001**.<br>2. El usuario ingresa el nombre del producto en **INV-TXT-NOMBRE-PRODUCTO**.<br>3. El usuario ingresa la acción terapéutica en **INV-TXT-ACCION-TERAPEUTICA**.<br>4. El usuario ingresa el precio de venta en **INV-NUM-PRECIO-PRODUCTO**.<br>5. El usuario selecciona el proveedor que abastece el producto en **INV-CMB-PROVEEDOR**.<br>6. El usuario verifica la información ingresada y presiona **INV-BTN-CREAR-PRODUCTO**.<br>7. El sistema valida que el nombre no esté vacío y que el precio sea un valor no negativo.<br>8. El sistema genera automáticamente el identificador del producto y registra el nuevo producto en **DB_FARMASIL.TBL_PRODUCTOS** con `estado_producto` = 'Disponible'.<br>9. El sistema muestra el identificador generado en **INV-LBL-ID-PRODUCTO**, que no es editable y actualiza **INV-TBL-PRODUCTOS**. |
| Postcondición | 1. El producto queda registrado en **DB_FARMASIL.TBL_PRODUCTOS** con todos sus campos obligatorios completos.<br>2. El producto aparece en **INV-TBL-PRODUCTOS** con estado 'Disponible'.<br>3. El producto queda disponible para recibir sus remesas según ILA-0041. Hasta que exista al menos una remesa con existencias, el medicamento no se ofrece en el módulo de ventas. |
| Código de artefactos asociados | ART-MKP-INV-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | El procedimiento anterior pedía al usuario que ingresara manualmente el ID del producto, pero `id_producto` es clave primaria autoincremental según el Diccionario de Datos; se corrigió a generación automática con visualización de solo lectura.<br>Faltaba `id_proveedor`, campo obligatorio que el registro no puede completar sin él.<br>Con la incorporación de **TBL_LOTES** en el Diccionario de Datos v04.00, el número de lote, la fecha de vencimiento y las existencias dejaron de ser atributos del medicamento y se registran como remesas en ILA-0041. Esta ilación quedó reducida a los datos de catálogo.<br>Queda Pendiente porque requiere incorporar **INV-CMB-PROVEEDOR** al mockup **ART-MKP-INV-0001**. Ver Anexo. |

| Código ilación | ILA-0006 |
| --- | --- |
| Nombre | Consulta del producto de inventario |
| Versión | 08.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código educción | EDU-0002 |
| Código especificación | ESP-0006 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador' o 'Tecnico'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_PRODUCTOS** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-INV-0002**.<br>6. Los campos **INV-TXT-ID-PRODUCTO** e **INV-TXT-NOMBRE-PRODUCTO** se encuentran habilitados.<br>7. El botón **INV-BTN-LEER-PRODUCTO** se encuentra habilitado. |
| Procedimiento | 1. El usuario visualiza el formulario de consulta de producto en el mockup **ART-MKP-INV-0002**.<br>2. El usuario ingresa el identificador del producto en **INV-TXT-ID-PRODUCTO** o el nombre del producto en **INV-TXT-NOMBRE-PRODUCTO**.<br>3. El usuario presiona **INV-BTN-LEER-PRODUCTO**.<br>4. El sistema procesa los criterios de búsqueda ingresados.<br>5. El sistema consulta los registros almacenados en **DB_FARMASIL.TBL_PRODUCTOS**.<br>6. El sistema muestra los resultados encontrados en **INV-TBL-PRODUCTOS**, incluyendo el estado del medicamento y el stock consolidado, calculado como la suma de las existencias de sus remesas en **DB_FARMASIL.TBL_LOTES**.<br>7. El usuario revisa la información mostrada. |
| Postcondición | 1. Los productos consultados se visualizan en **INV-TBL-PRODUCTOS** con su stock consolidado.<br>1-bis. El detalle por remesa, con número de lote y vencimiento, se consulta en el módulo 11 según ILA-0042.<br>2. No se modifica ninguna información registrada en **DB_FARMASIL.TBL_PRODUCTOS**. |
| Código de artefactos asociados | ART-MKP-INV-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Permitirá localizar medicamentos de forma rápida y eficiente. Se explicitó en el resultado de la consulta el stock, el lote y el estado, que son los datos que el personal necesita durante la atención. |

| Código ilación | ILA-0007 |
| --- | --- |
| Nombre | Actualización del producto de inventario |
| Versión | 08.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código educción | EDU-0002 |
| Código especificación | ESP-0007 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_PRODUCTOS** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-INV-0001**.<br>6. El botón **INV-BTN-ACTUALIZAR-PRODUCTO** se encuentra habilitado. |
| Procedimiento | 1. El usuario visualiza la tabla de productos **INV-TBL-PRODUCTOS** en el mockup **ART-MKP-INV-0001**.<br>2. El usuario identifica y selecciona el producto que desea actualizar.<br>3. El usuario presiona **INV-BTN-ACTUALIZAR-PRODUCTO**.<br>4. El sistema carga el formulario de actualización en el mockup **ART-MKP-INV-0003** con la información vigente del producto.<br>5. El usuario modifica los campos editables: **INV-TXT-NOMBRE-PRODUCTO**, **INV-TXT-ACCION-TERAPEUTICA**, **INV-NUM-PRECIO-PRODUCTO** e **INV-CMB-PROVEEDOR**. El identificador se muestra en **INV-LBL-ID-PRODUCTO** y no es editable. El número de lote, el vencimiento y las existencias se modifican sobre la remesa correspondiente, según ILA-0043.<br>6. El usuario verifica la información y presiona **INV-BTN-ACTUALIZAR-PRODUCTO**.<br>7. El sistema valida la información ingresada con los mismos criterios de la creación.<br>8. El sistema actualiza el registro correspondiente en **DB_FARMASIL.TBL_PRODUCTOS**.<br>9. El sistema actualiza **INV-TBL-PRODUCTOS**. |
| Postcondición | 1. Los cambios quedan registrados en **DB_FARMASIL.TBL_PRODUCTOS**.<br>2. La información actualizada se visualiza en **INV-TBL-PRODUCTOS**.<br>3. El identificador del producto permanece inalterado, preservando las referencias desde **TBL_LOTES** y **TBL_RESTRICCIONES_VENTA**. |
| Código de artefactos asociados | ART-MKP-INV-0001, ART-MKP-INV-0003 |
| Importancia | Media |
| Estado | Pendiente |
| Comentario | Se declaró explícitamente que el identificador no es editable, dado que es clave foránea en tres tablas del modelo. Se alinearon los campos editables con los incorporados en ILA-0005.<br>Queda Pendiente por la misma dependencia de mockup que ILA-0005. |

| Código ilación | ILA-0008 |
| --- | --- |
| Nombre | Eliminación del producto de inventario |
| Versión | 08.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0003 |
| Actor | ACT-0001 |
| Fuente | FUE-0004, FUE-0005 |
| Experto | Ninguno |
| Código educción | EDU-0002 |
| Código especificación | ESP-0008 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_PRODUCTOS** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-INV-0004**.<br>6. El producto a dar de baja se encuentra visible en **INV-TBL-PRODUCTOS**. |
| Procedimiento | 1. El usuario identifica y selecciona el producto que desea dar de baja en **INV-TBL-PRODUCTOS**.<br>2. El usuario verifica la información mostrada y presiona **INV-BTN-ELIMINAR-PRODUCTO**.<br>3. El sistema carga el mockup **ART-MKP-INV-0004** y muestra el modal **INV-MDL-CONFIRMAR-ELIMINACION** con el mensaje **INV-TXT-MSJ**: "¿Está seguro de dar de baja este producto?"<br>4. El sistema verifica si el producto tiene remesas registradas en **DB_FARMASIL.TBL_LOTES**, restricciones en **DB_FARMASIL.TBL_RESTRICCIONES_VENTA**, o movimientos derivados de sus remesas en **DB_FARMASIL.TBL_DETALLE_VENTAS** y **DB_FARMASIL.TBL_DETALLE_DEVOLUCION**.<br>5. Si el usuario presiona **INV-BTN-CONFIRMAR-SI** y el producto no tiene movimientos asociados, el sistema elimina el registro de **DB_FARMASIL.TBL_PRODUCTOS**.<br>6. Si el usuario presiona **INV-BTN-CONFIRMAR-SI** y el producto sí tiene movimientos asociados, el sistema realiza una baja lógica actualizando `estado_producto` a 'Descontinuado', conservando el registro y su histórico.<br>7. Si el usuario presiona **INV-BTN-CONFIRMAR-NO**, el sistema cierra el modal sin realizar cambios.<br>8. El sistema actualiza **INV-TBL-PRODUCTOS**. |
| Postcondición | 1. El producto sin movimientos queda eliminado de **DB_FARMASIL.TBL_PRODUCTOS**.<br>2. El producto con movimientos queda con `estado_producto` = 'Descontinuado' y conserva su histórico de ventas y devoluciones.<br>3. En ambos casos el producto deja de ofrecerse en **VEN-CMB-PRODUCTO-VENTA**.<br>4. No se produce ninguna violación de integridad referencial. |
| Código de artefactos asociados | ART-MKP-INV-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La versión anterior eliminaba físicamente el producto sin considerar que `id_producto` es clave foránea en **TBL_DETALLE_VENTAS**, **TBL_DETALLE_DEVOLUCION** y **TBL_RESTRICCIONES_VENTA**: la operación habría fallado o habría destruido el histórico de ventas.<br>El procedimiento anterior también pedía ingresar el ID del producto y además seleccionarlo de la tabla, dos pasos redundantes; se dejó únicamente la selección.<br>Queda Pendiente porque el valor 'Descontinuado' debe incorporarse al dominio de `estado_producto` en el Diccionario de Datos, que hoy solo contempla 'Disponible' y 'Bloqueado por devolucion'. Ver Anexo. |
---

# Módulo 3 — Gestión de documentación tributaria (EDU-0003)

| Código ilación | ILA-0009 |
| --- | --- |
| Nombre | Registrar comprobante tributario |
| Versión | 06.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0002, FUE-0003 |
| Experto | Ninguno |
| Código educción | EDU-0003 |
| Código especificación | ESP-0009 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador' o 'Tecnico'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. Existe al menos una venta registrada en **DB_FARMASIL.TBL_REGISTRO_VENTAS** que aún no tiene comprobante tributario asociado.<br>5. Se carga el mockup **ART-MKP-DOC-0001**. |
| Procedimiento | 1. El usuario presiona **DOC-BTN-CREAR-COMPROBANTE**.<br>2. El usuario selecciona la venta a documentar en **DOC-CMB-CODIGO-VENTA**, que lista únicamente las ventas sin comprobante asociado.<br>3. El sistema recupera el `monto_total` de la venta seleccionada desde **DB_FARMASIL.TBL_REGISTRO_VENTAS** y lo muestra en **DOC-LBL-MONTO-TOTAL**, que no es editable.<br>4. El usuario selecciona el tipo de comprobante en **DOC-CMB-TIPO-COMPROBANTE**, con los valores 'Boleta' o 'Factura'.<br>5. El usuario registra el número de comprobante en **DOC-TXT-NUMERO-COMPROBANTE**.<br>6. El usuario registra la fecha de emisión en **DOC-FEC-FECHA-EMISION**.<br>7. El usuario presiona **DOC-BTN-CONFIRMAR-SI**.<br>8. El sistema valida que el número de comprobante no esté duplicado y que la venta seleccionada no tenga ya un comprobante emitido.<br>9. El sistema registra el comprobante en **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS** con `estado_documento` = 'Emitido'.<br>10. El sistema actualiza **DOC-TBL-COMPROBANTES**. |
| Postcondición | 1. El comprobante queda registrado en **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS** con estado 'Emitido'.<br>2. El comprobante queda asociado de forma unívoca a una única venta de **DB_FARMASIL.TBL_REGISTRO_VENTAS**.<br>3. El registro aparece en **DOC-TBL-COMPROBANTES**.<br>4. La venta documentada queda bloqueada para modificación y eliminación directas, según ILA-0003 e ILA-0004. |
| Código de artefactos asociados | ART-MKP-DOC-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La precondición anterior exigía que **TBL_COMPROBANTES_TRIBUTARIOS** ya tuviera entradas válidas para poder crear un comprobante, lo que hace imposible el primer registro del sistema. Se reemplazó por la precondición correcta: que exista una venta pendiente de documentar.<br>El monto dejó de ser un campo de captura manual: el diccionario no define un campo de monto en la tabla de comprobantes porque el importe pertenece a la venta. Ahora se recupera y se muestra en solo lectura, eliminando la posibilidad de que el comprobante y la venta declaren montos distintos.<br>Se corrigió el artefacto **ART-MKP-DOC--0001**, que tenía doble guion.<br>Queda Pendiente porque `numero_comprobante` no existe en el Diccionario de Datos y la emisión formal de un comprobante lo requiere. Ver Anexo. |

| Código ilación | ILA-0010 |
| --- | --- |
| Nombre | Consultar comprobantes tributarios |
| Versión | 05.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0002, FUE-0003 |
| Experto | Ninguno |
| Código educción | EDU-0003 |
| Código especificación | ESP-0010 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador' o 'Tecnico'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-DOC-0001**. |
| Procedimiento | 1. El usuario accede al módulo de documentación tributaria y visualiza el listado de comprobantes registrados en **DOC-TBL-COMPROBANTES**.<br>2. El usuario presiona **DOC-BTN-LEER-COMPROBANTE**.<br>3. El sistema carga el mockup **ART-MKP-DOC-0002**.<br>4. El usuario aplica los filtros disponibles: fecha en **DOC-FEC-FECHA-EMISION**, tipo en **DOC-CMB-TIPO-COMPROBANTE** o número en **DOC-TXT-NUMERO-COMPROBANTE**.<br>5. El usuario presiona **DOC-BTN-LEER-COMPROBANTE**.<br>6. El sistema consulta **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS** y resuelve el monto de cada comprobante contra **DB_FARMASIL.TBL_REGISTRO_VENTAS**.<br>7. El sistema muestra los resultados en **DOC-TBL-COMPROBANTES**, incluyendo el estado del documento. |
| Postcondición | 1. El sistema muestra la información conforme a los filtros de búsqueda aplicados en **DOC-TBL-COMPROBANTES**.<br>2. Los comprobantes anulados se identifican por su estado y permanecen visibles para efectos de auditoría.<br>3. No se modifica ninguna información en **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS**. |
| Código de artefactos asociados | ART-MKP-DOC-0001, ART-MKP-DOC-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se corrigió el componente **DOC-TLB-COMPROBANTES**, que invertía las letras del prefijo de tabla, y la referencia a la tabla inexistente **DOC_TBL_COMPROBANTES_TRIBUTARIOS**.<br>Facilita la revisión de información tributaria para auditorías y declaraciones. |

| Código ilación | ILA-0011 |
| --- | --- |
| Nombre | Actualizar comprobante tributario |
| Versión | 06.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | FUE-0002, FUE-0003 |
| Experto | Ninguno |
| Código educción | EDU-0003 |
| Código especificación | ESP-0011 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS** tiene entradas válidas.<br>5. El comprobante seleccionado se encuentra en estado 'Emitido'.<br>6. Se carga el mockup **ART-MKP-DOC-0001**. |
| Procedimiento | 1. El usuario selecciona un comprobante registrado en **DOC-TBL-COMPROBANTES**.<br>2. El usuario presiona **DOC-BTN-ACTUALIZAR-COMPROBANTE**.<br>3. El sistema verifica que el comprobante no se encuentre anulado. Si lo está, cancela la operación.<br>4. El sistema carga el mockup **ART-MKP-DOC-0003** con la información vigente del comprobante.<br>5. El usuario modifica los campos corregibles: **DOC-CMB-TIPO-COMPROBANTE**, **DOC-TXT-NUMERO-COMPROBANTE** y **DOC-FEC-FECHA-EMISION**. La venta asociada, mostrada en **DOC-CMB-CODIGO-VENTA**, permanece de solo lectura.<br>6. El usuario presiona **DOC-BTN-CONFIRMAR-ACTUALIZACION**.<br>7. El sistema valida que el nuevo número de comprobante no esté duplicado.<br>8. El sistema actualiza el registro en **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS**.<br>9. El sistema actualiza **DOC-TBL-COMPROBANTES**. |
| Postcondición | 1. La información del comprobante queda actualizada en **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS**.<br>2. La venta de origen del comprobante permanece inalterada, preservando la relación uno a uno del modelo.<br>3. Los cambios aparecen en **DOC-TBL-COMPROBANTES**. |
| Código de artefactos asociados | ART-MKP-DOC-0001, ART-MKP-DOC-0003 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | El procedimiento anterior decía que el usuario "modifica datos permitidos" sin precisar cuáles; se enumeraron los campos corregibles y se declaró de solo lectura la venta asociada, ya que reasignar un comprobante a otra venta rompería la relación uno a uno definida en el modelo ER.<br>Se corrigió el código **ILA_0011** y la referencia **DB_FARMASIL_TBL_COMPROBANTES_TRIBUTARIOS**, que usaba guion bajo en lugar del punto separador. |

| Código ilación | ILA-0012 |
| --- | --- |
| Nombre | Anulación de comprobante tributario |
| Versión | 05.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | FUE-0002, FUE-0003 |
| Experto | Ninguno |
| Código educción | EDU-0003 |
| Código especificación | ESP-0012 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS** tiene entradas válidas.<br>5. El comprobante seleccionado se encuentra en estado 'Emitido'.<br>6. Se carga el mockup **ART-MKP-DOC-0001**. |
| Procedimiento | 1. El usuario selecciona un comprobante en **DOC-TBL-COMPROBANTES**.<br>2. El usuario presiona **DOC-BTN-ELIMINAR-COMPROBANTE**.<br>3. El sistema carga el mockup **ART-MKP-DOC-0004** y muestra el modal **DOC-MDL-CONFIRMAR-ELIMINACION** con el mensaje **DOC-TXT-MSJ**: "¿Está seguro de anular este comprobante? El documento se conservará con estado Anulado."<br>4. Si el usuario presiona **DOC-BTN-CONFIRMAR-SI**, el sistema actualiza `estado_documento` a 'Anulado' en **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS**.<br>5. Si el usuario presiona **DOC-BTN-CONFIRMAR-NO**, el sistema cierra el modal sin realizar cambios.<br>6. El sistema actualiza **DOC-TBL-COMPROBANTES**. |
| Postcondición | 1. El comprobante queda marcado como 'Anulado' en **DB_FARMASIL.TBL_COMPROBANTES_TRIBUTARIOS** y se conserva para efectos de auditoría.<br>2. La venta asociada queda nuevamente habilitada para modificación o eliminación según ILA-0003 e ILA-0004.<br>3. El nuevo estado se refleja en **DOC-TBL-COMPROBANTES**. |
| Código de artefactos asociados | ART-MKP-DOC-0001, ART-MKP-DOC-0004 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La versión anterior eliminaba físicamente el comprobante. Un documento tributario emitido no puede borrarse del registro: el propio Diccionario de Datos define el valor 'Anulado' en `estado_documento` precisamente para este caso. La fase Eliminar del CRUD se implementa como baja lógica.<br>La postcondición anterior además decía que se eliminaba "el registro de venta", confundiendo el comprobante con la venta.<br>Se corrigieron el código **ILA_0012** y los componentes escritos con guion bajo (**DOC_BTN_ELIMINAR_COMPROBANTE**, **DOC_MDL_CONFIRMACION_ELIMINACION**, **DOC_BTN_CONFIRMAR_SI**, **DOC_BTN_CONFIRMAR_NO**). |

---

# Módulo 4 — Gestión de alertas de productos vencidos (EDU-0004)

| Código ilación | ILA-0013 |
| --- | --- |
| Nombre | Consulta de alertas de productos vencidos |
| Versión | 07.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0007 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0005 |
| Experto | Ninguno |
| Código educción | EDU-0004 |
| Código especificación | ESP-0013 |
| Precondición | 1. La base de datos **DB_FARMASIL** debe encontrarse operativa.<br>2. El usuario tiene una sesión activa con rol 'Administrador' o 'Tecnico'.<br>3. La tabla **DB_FARMASIL.TBL_ALERTAS_VENCIMIENTO** debe encontrarse disponible y contener el umbral configurado en el campo `umbral_meses`.<br>4. La tabla **DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS** debe encontrarse disponible.<br>5. Se carga el mockup **ART-MKP-ALV-0001** y **ALV-TBL-ALERTAS-VENCIMIENTO** se encuentra habilitada. |
| Procedimiento | 1. El usuario accede al panel de alertas de productos vencidos.<br>2. El usuario presiona **ALV-BTN-LEER-ALERTA**.<br>3. El sistema consulta el `umbral_meses` configurado en **DB_FARMASIL.TBL_ALERTAS_VENCIMIENTO**.<br>4. El sistema consulta las remesas registradas en **DB_FARMASIL.TBL_LOTES** y compara su `fecha_vencimiento` contra el umbral.<br>5. El sistema consulta las alertas ya generadas, automática o manualmente, en **DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS**.<br>6. El sistema carga y muestra el resultado consolidado en **ALV-TBL-ALERTAS-VENCIMIENTO**. |
| Postcondición | 1. **ALV-TBL-ALERTAS-VENCIMIENTO** se muestra actualizada con las remesas próximas a vencer y las ya retiradas, identificadas por medicamento y número de lote.<br>2. Ningún dato es modificado en **DB_FARMASIL**. |
| Código de artefactos asociados | ART-MKP-ALV-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La generación física de nuevas alertas por vencimiento la realiza un proceso programado del sistema, según el umbral de **TBL_ALERTAS_VENCIMIENTO**; esa generación automática se documenta como proceso de sistema y requisito no funcional aparte, no como una operación CRUD manual del usuario. Lo que el usuario realiza aquí es la consulta del resultado.<br>Se unificó la referencia al umbral, que antes se describía como "días/meses" mientras el diccionario define `umbral_meses`. |

| Código ilación | ILA-0014 |
| --- | --- |
| Nombre | Registro manual de alerta de producto vencido |
| Versión | 07.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0007 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0005 |
| Experto | Ninguno |
| Código educción | EDU-0004 |
| Código especificación | ESP-0014 |
| Precondición | 1. La base de datos **DB_FARMASIL** debe encontrarse operativa.<br>2. El usuario tiene una sesión activa con rol 'Administrador' o 'Tecnico'.<br>3. La tabla **DB_FARMASIL.TBL_LOTES** debe encontrarse disponible y la remesa reportada debe existir en el inventario.<br>4. El sistema carga el mockup **ART-MKP-ALV-0001**.<br>5. **ALV-CMB-LOTE-ALERTA**, **ALV-CHK-LOTE-DEFECTUOSO** y **ALV-TXT-ALERTA-DIGEMID** se encuentran habilitados.<br>6. **ALV-BTN-CREAR-ALERTA** se encuentra activo. |
| Procedimiento | 1. El usuario accede al módulo de gestión de alertas de productos vencidos.<br>2. El usuario selecciona la remesa afectada en **ALV-CMB-LOTE-ALERTA**, que lista las remesas del inventario identificadas por su medicamento y su número de lote.<br>3. El sistema muestra la fecha de vencimiento de la remesa en **ALV-LBL-FECHA-VENCIMIENTO**, que no es editable.<br>4. El usuario marca **ALV-CHK-LOTE-DEFECTUOSO** si el lote falló el control de calidad físico.<br>5. El usuario registra el código de la resolución sanitaria en **ALV-TXT-ALERTA-DIGEMID**, si la alerta proviene de DIGEMID.<br>6. El usuario verifica la información ingresada y presiona **ALV-BTN-CREAR-ALERTA**.<br>7. El sistema valida que los campos obligatorios contengan información correcta. Si algún campo es inválido, lo señala y permite corregirlo antes de reintentar.<br>8. El sistema inicia una transacción, registra la alerta en **DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS**, conservando el nombre del medicamento y el número de lote, actualiza el `estado_lote` de la remesa afectada a 'Bloqueado por devolucion' en **DB_FARMASIL.TBL_LOTES**, y confirma la transacción.<br>9. El sistema actualiza **ALV-TBL-ALERTAS-VENCIMIENTO** y confirma el registro exitoso. |
| Postcondición | 1. La alerta queda almacenada en **DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS**.<br>2. La remesa afectada queda con `estado_lote` = 'Bloqueado por devolucion' y deja de ofrecerse en **VEN-CMB-LOTE-VENTA**. Las demás remesas del mismo medicamento continúan disponibles para la venta.<br>3. La alerta aparece en **ALV-TBL-ALERTAS-VENCIMIENTO**.<br>4. La remesa queda disponible para ser incluida en una orden de devolución según ILA-0025. |
| Código de artefactos asociados | ART-MKP-ALV-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | EDU-0004 exige que el sistema no solo emita la alerta sino que además **bloquee para la venta** el lote afectado. La versión anterior registraba la alerta pero dejaba el producto plenamente vendible, incumpliendo el requisito. Se incorporó la actualización del estado del producto dentro de la misma transacción.<br>Permite registrar preventivamente productos reportados por DIGEMID o con lote defectuoso, con opción explícita de corregir un campo erróneo antes de confirmar. |

| Código ilación | ILA-0015 |
| --- | --- |
| Nombre | Modificación de alerta de producto vencido |
| Versión | 07.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0007 |
| Actor | ACT-0001 |
| Fuente | FUE-0005 |
| Experto | Ninguno |
| Código educción | EDU-0004 |
| Código especificación | ESP-0015 |
| Precondición | 1. La base de datos **DB_FARMASIL** debe encontrarse operativa.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. La tabla **DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS** debe encontrarse disponible y existe al menos una alerta registrada.<br>4. El sistema carga el mockup **ART-MKP-ALV-0001** y **ALV-TBL-ALERTAS-VENCIMIENTO** está disponible.<br>5. **ALV-BTN-ACTUALIZAR-ALERTA** está activo. |
| Procedimiento | 1. El usuario selecciona una alerta desde **ALV-TBL-ALERTAS-VENCIMIENTO**.<br>2. El sistema carga el mockup **ART-MKP-ALV-0002** con la información vigente de la alerta.<br>3. El usuario modifica **ALV-CMB-LOTE-ALERTA**, **ALV-CHK-LOTE-DEFECTUOSO** o **ALV-TXT-ALERTA-DIGEMID** según corresponda. La fecha de vencimiento se muestra en **ALV-LBL-FECHA-VENCIMIENTO** y se actualiza al cambiar de remesa.<br>4. El usuario presiona **ALV-BTN-ACTUALIZAR-ALERTA** y confirma con **ALV-BTN-CONFIRMAR-ACTUALIZACION**.<br>5. El sistema valida la nueva información.<br>6. El sistema actualiza el registro en **DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS**.<br>7. El sistema actualiza **ALV-TBL-ALERTAS-VENCIMIENTO** y confirma la actualización exitosa. |
| Postcondición | 1. La alerta queda actualizada en **DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS**.<br>2. Los cambios son visibles en **ALV-TBL-ALERTAS-VENCIMIENTO**.<br>3. El bloqueo de venta de la remesa afectada se mantiene mientras la alerta siga vigente. |
| Código de artefactos asociados | ART-MKP-ALV-0001, ART-MKP-ALV-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Permite corregir la información de una alerta cuando el riesgo sanitario se aclara o se resuelve. Se incorporó el paso de confirmación explícito, coherente con el resto de operaciones de actualización del catálogo. |

| Código ilación | ILA-0016 |
| --- | --- |
| Nombre | Eliminación de alerta de producto vencido |
| Versión | 07.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0007 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0004 |
| Código especificación | ESP-0016 |
| Precondición | 1. La base de datos **DB_FARMASIL** debe encontrarse operativa.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. La tabla **DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS** debe encontrarse disponible y debe existir al menos una alerta registrada.<br>4. La alerta seleccionada no forma parte de una orden de devolución vigente en **DB_FARMASIL.TBL_DETALLE_DEVOLUCION**.<br>5. **ALV-TBL-ALERTAS-VENCIMIENTO** debe encontrarse disponible y **ALV-BTN-ELIMINAR-ALERTA** activo. |
| Procedimiento | 1. El usuario selecciona una alerta desde **ALV-TBL-ALERTAS-VENCIMIENTO**.<br>2. El usuario presiona **ALV-BTN-ELIMINAR-ALERTA**.<br>3. El sistema carga el mockup **ART-MKP-ALV-0003** y muestra el modal **ALV-MDL-CONFIRMAR-ELIMINACION** con el mensaje **ALV-TXT-MSJ**: "¿Está seguro de eliminar esta alerta? La remesa volverá a estar disponible para la venta."<br>4. El sistema verifica que la alerta no esté asociada a una orden de devolución vigente.<br>5. Si el usuario presiona **ALV-BTN-CONFIRMAR-SI**, el sistema inicia una transacción, elimina el registro de **DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS**, restituye el `estado_lote` de la remesa afectada a 'Disponible' en **DB_FARMASIL.TBL_LOTES** y confirma la transacción.<br>6. Si el usuario presiona **ALV-BTN-CONFIRMAR-NO**, el sistema cierra el modal sin realizar cambios.<br>7. El sistema actualiza **ALV-TBL-ALERTAS-VENCIMIENTO**. |
| Postcondición | 1. El registro es eliminado de **DB_FARMASIL.TBL_PRODUCTOS_RETIRADOS**.<br>2. La remesa recupera el estado 'Disponible' y vuelve a ofrecerse en **VEN-CMB-LOTE-VENTA**.<br>3. La alerta deja de mostrarse en **ALV-TBL-ALERTAS-VENCIMIENTO**. |
| Código de artefactos asociados | ART-MKP-ALV-0001, ART-MKP-ALV-0003 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | El campo Fuente decía "Ninguno" pese a que el requisito proviene del Registro de Entrevista 1, igual que el resto del módulo; se corrigió y queda pendiente asignarle su código FUE definitivo.<br>Se agregó la restitución del estado del producto, que es la contraparte lógica del bloqueo aplicado en ILA-0014: sin ella, eliminar la alerta dejaba el producto bloqueado de forma permanente. También se agregó la verificación de que la alerta no esté comprometida en una devolución en curso. |
---

# Módulo 5 — Gestión de métodos de pago (EDU-0009)

| Código ilación | ILA-0017 |
| --- | --- |
| Nombre | Registro de métodos de pago |
| Versión | 06.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0002 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código educción | EDU-0009 |
| Código especificación | ESP-0017 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. Se carga el mockup **ART-MKP-PAG-0001**. |
| Procedimiento | 1. El usuario accede al módulo de métodos de pago.<br>2. El usuario ingresa el nombre del método de pago en **PAG-TXT-METODO-PAGO**.<br>3. El usuario selecciona el estado inicial del método en **PAG-CMB-ESTADO-METODO**.<br>4. El usuario presiona **PAG-BTN-CREAR-METODO**.<br>5. El sistema valida que el nombre no esté vacío y que no exista ya un método con el mismo nombre, dado que `nombre_metodo` es único.<br>6. El sistema registra el método en **DB_FARMASIL.TBL_METODOS_PAGO**.<br>7. El sistema actualiza **PAG-TBL-METODOS-PAGO**. |
| Postcondición | 1. El método de pago queda registrado en **DB_FARMASIL.TBL_METODOS_PAGO**.<br>2. El método aparece en **PAG-TBL-METODOS-PAGO**.<br>3. Si su estado es 'Activo', el método queda disponible para ser seleccionado en **VEN-CMB-METODO-PAGO** durante el registro de una venta. |
| Código de artefactos asociados | ART-MKP-PAG-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se corrigió la referencia a la tabla inexistente **PAG_TBL_METODOS_PAGO** y se invirtió el orden de la postcondición, que declaraba primero la actualización de la interfaz y después la persistencia en base de datos.<br>Se agregó la validación de unicidad exigida por la restricción UNIQUE del diccionario y el efecto sobre el módulo de ventas. |

| Código ilación | ILA-0018 |
| --- | --- |
| Nombre | Consulta de métodos de pago |
| Versión | 05.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0002 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código educción | EDU-0009 |
| Código especificación | ESP-0018 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador' o 'Tecnico'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_METODOS_PAGO** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-PAG-0001**. |
| Procedimiento | 1. El usuario accede al módulo de métodos de pago.<br>2. El sistema carga el mockup **ART-MKP-PAG-0002**.<br>3. El sistema consulta **DB_FARMASIL.TBL_METODOS_PAGO**.<br>4. El usuario puede filtrar el listado por estado en **PAG-CMB-ESTADO-METODO**.<br>5. El sistema muestra los métodos registrados con su estado en **PAG-TBL-METODOS-PAGO**. |
| Postcondición | 1. El sistema muestra los métodos de pago registrados y su estado en **PAG-TBL-METODOS-PAGO**.<br>2. Los datos permanecen sin modificación en **DB_FARMASIL.TBL_METODOS_PAGO**. |
| Código de artefactos asociados | ART-MKP-PAG-0001, ART-MKP-PAG-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se corrigió la fecha, que estaba registrada como "2106//26", y el componente **PAG-TLB-METODOS-PAGO**, que invertía las letras del prefijo de tabla.<br>El procedimiento anterior tenía solo dos pasos y no explicaba de dónde salía la información mostrada; se completó el flujo de consulta.<br>Facilita la verificación de los métodos habilitados, permitiendo al personal conocer cuáles pueden utilizarse en las ventas. |

| Código ilación | ILA-0019 |
| --- | --- |
| Nombre | Actualización de métodos de pago |
| Versión | 05.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0002 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código educción | EDU-0009 |
| Código especificación | ESP-0019 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_METODOS_PAGO** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-PAG-0001**. |
| Procedimiento | 1. El usuario selecciona un método de pago en **PAG-TBL-METODOS-PAGO**.<br>2. El usuario presiona **PAG-BTN-ACTUALIZAR-METODO**.<br>3. El sistema carga el mockup **ART-MKP-PAG-0003** con la información vigente del método.<br>4. El usuario modifica el nombre en **PAG-TXT-METODO-PAGO** o el estado en **PAG-CMB-ESTADO-METODO**.<br>5. El usuario presiona **PAG-BTN-CONFIRMAR-ACTUALIZACION**.<br>6. El sistema valida que el nuevo nombre no colisione con otro método existente.<br>7. El sistema actualiza el registro en **DB_FARMASIL.TBL_METODOS_PAGO**.<br>8. El sistema actualiza **PAG-TBL-METODOS-PAGO**. |
| Postcondición | 1. El método de pago queda actualizado en **DB_FARMASIL.TBL_METODOS_PAGO**.<br>2. Los cambios se reflejan en **PAG-TBL-METODOS-PAGO**.<br>3. Si el método pasó a estado 'Inactivo', deja de ofrecerse en **VEN-CMB-METODO-PAGO**, pero las ventas ya registradas con ese método conservan su referencia. |
| Código de artefactos asociados | ART-MKP-PAG-0001, ART-MKP-PAG-0003 |
| Importancia | Media |
| Estado | Concluido |
| Comentario | La precondición y el procedimiento habían perdido por completo la numeración de sus pasos; se restauró.<br>Se explicitó que la desactivación no afecta el histórico de ventas, punto que enlaza esta ilación con ILA-0020. |

| Código ilación | ILA-0020 |
| --- | --- |
| Nombre | Desactivación de métodos de pago |
| Versión | 05.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0002 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código educción | EDU-0009 |
| Código especificación | ESP-0020 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_METODOS_PAGO** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-PAG-0001**. |
| Procedimiento | 1. El usuario selecciona un método de pago registrado en **PAG-TBL-METODOS-PAGO**.<br>2. El usuario presiona **PAG-BTN-ELIMINAR-METODO**.<br>3. El sistema carga el mockup **ART-MKP-PAG-0004** y muestra el modal **PAG-MDL-CONFIRMAR-ELIMINACION** con el mensaje **PAG-TXT-MSJ**: "¿Está seguro de dar de baja este método de pago? Las ventas ya registradas con él no se verán afectadas."<br>4. El sistema verifica si el método tiene ventas asociadas en **DB_FARMASIL.TBL_REGISTRO_VENTAS**.<br>5. Si el usuario presiona **PAG-BTN-CONFIRMAR-SI** y el método no tiene ventas asociadas, el sistema elimina el registro de **DB_FARMASIL.TBL_METODOS_PAGO**.<br>6. Si el usuario presiona **PAG-BTN-CONFIRMAR-SI** y el método sí tiene ventas asociadas, el sistema actualiza su `estado` a 'Inactivo', conservando el registro.<br>7. Si el usuario presiona **PAG-BTN-CONFIRMAR-NO**, el sistema cierra el modal sin realizar cambios.<br>8. El sistema actualiza **PAG-TBL-METODOS-PAGO**. |
| Postcondición | 1. El método sin ventas asociadas queda eliminado de **DB_FARMASIL.TBL_METODOS_PAGO**.<br>2. El método con ventas asociadas queda en estado 'Inactivo' y conserva la referencia desde el histórico de ventas.<br>3. En ambos casos el método deja de ofrecerse en **VEN-CMB-METODO-PAGO**.<br>4. No se produce ninguna violación de integridad referencial. |
| Código de artefactos asociados | ART-MKP-PAG-0001, ART-MKP-PAG-0004 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La postcondición anterior era contradictoria: para una operación de eliminación afirmaba que "el método de pago queda registrado correctamente".<br>La eliminación física era además inviable, porque `id_metodo_pago` es clave foránea en **TBL_REGISTRO_VENTAS**: borrar un método usado habría roto el histórico de ventas. Se resolvió con eliminación física solo cuando no hay referencias y baja lógica en caso contrario, usando el campo `estado` que el diccionario ya define.<br>Evita el uso de métodos de pago no autorizados o en desuso sin sacrificar la trazabilidad contable. |

---

# Módulo 6 — Gestión de reportes de ventas (EDU-0010)

| Código ilación | ILA-0021 |
| --- | --- |
| Nombre | Generación de reportes de ventas |
| Versión | 07.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código educción | EDU-0010 |
| Código especificación | ESP-0021 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_REGISTRO_VENTAS** contiene registros válidos.<br>5. La tabla **DB_FARMASIL.TBL_DETALLE_VENTAS** contiene el detalle de productos vendidos.<br>6. La tabla **DB_FARMASIL.TBL_REPORTES_VENTAS** está disponible para almacenar el índice del reporte.<br>7. Se carga el mockup **ART-MKP-REP-0001**. |
| Procedimiento | 1. El usuario accede al módulo de reportes de ventas.<br>2. El usuario selecciona el tipo de reporte en **REP-CMB-TIPO-REPORTE**.<br>3. El usuario selecciona el formato de salida en **REP-CMB-FORMATO-REPORTE**.<br>4. El usuario registra la fecha de inicio en **REP-FEC-FECHA-INICIO** y la fecha de fin en **REP-FEC-FECHA-FIN**.<br>5. El usuario presiona **REP-BTN-GENERAR-REPORTE**.<br>6. El sistema valida que la fecha de inicio no sea posterior a la fecha de fin.<br>7. El sistema consolida la información de **DB_FARMASIL.TBL_REGISTRO_VENTAS** y **DB_FARMASIL.TBL_DETALLE_VENTAS** para el intervalo indicado, obteniendo el monto recaudado, la cantidad de productos vendidos, las fechas de las transacciones y el usuario responsable de cada venta.<br>8. El sistema registra el índice del nuevo reporte en **DB_FARMASIL.TBL_REPORTES_VENTAS**.<br>9. El sistema actualiza **REP-TBL-REPORTES**. |
| Postcondición | 1. El sistema genera el reporte con la información consolidada del intervalo solicitado.<br>2. El reporte queda registrado en **DB_FARMASIL.TBL_REPORTES_VENTAS**.<br>3. El reporte queda disponible para su descarga y para consultas posteriores.<br>4. Las ventas de origen permanecen sin modificación. |
| Código de artefactos asociados | ART-MKP-REP-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Corresponde a la fase Crear del CRUD. La consulta de un reporte existente, antes mezclada aquí, se separó hacia ILA-0022.<br>Queda Pendiente porque EDU-0010 exige que el reporte incluya "la identificación del personal que realizó la venta", requisito que la dueña confirmó expresamente, pero **TBL_REGISTRO_VENTAS** no tiene ninguna columna que vincule la venta con un usuario. La gestión de las cuentas queda cubierta por EDU-0013, y resta agregar el campo `id_usuario` a la tabla de ventas. Ver Anexo. |

| Código ilación | ILA-0022 |
| --- | --- |
| Nombre | Consulta de reportes de ventas |
| Versión | 07.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código educción | EDU-0010 |
| Código especificación | ESP-0022 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Debe existir al menos un reporte registrado en **DB_FARMASIL.TBL_REPORTES_VENTAS**.<br>4. Se carga el mockup **ART-MKP-REP-0002**. |
| Procedimiento | 1. El usuario accede al módulo de reportes de ventas.<br>2. El usuario presiona **REP-BTN-LEER-REPORTE**.<br>3. El sistema consulta **DB_FARMASIL.TBL_REPORTES_VENTAS**.<br>4. El sistema despliega en **REP-TBL-REPORTES** el identificador, el tipo, el período y el total recaudado de cada reporte.<br>5. El usuario selecciona un reporte y el sistema lo visualiza en el formato con el que fue generado, en modo de solo lectura. |
| Postcondición | 1. El reporte queda disponible para su consulta.<br>2. La información se presenta correctamente en **REP-TBL-REPORTES**.<br>3. No se modifica ninguna información en **DB_FARMASIL.TBL_REPORTES_VENTAS**. |
| Código de artefactos asociados | ART-MKP-REP-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Corresponde a la fase Leer del CRUD. Se precisó que el reporte se visualiza en el formato con el que fue generado, en lugar de permitir elegir formato durante la consulta, lo que habría implicado regenerarlo. |

| Código ilación | ILA-0023 |
| --- | --- |
| Nombre | Actualización del intervalo de un reporte de ventas |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código educción | EDU-0010 |
| Código especificación | ESP-0023 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. La tabla **DB_FARMASIL.TBL_REPORTES_VENTAS** contiene al menos un reporte generado.<br>4. Se carga el mockup **ART-MKP-REP-0002**.<br>5. **REP-BTN-ACTUALIZAR-REPORTE** se encuentra activo. |
| Procedimiento | 1. El usuario selecciona un reporte desde **REP-TBL-REPORTES**.<br>2. El usuario presiona **REP-BTN-ACTUALIZAR-REPORTE**.<br>3. El sistema carga el mockup **ART-MKP-REP-0003** con el intervalo actual del reporte.<br>4. El usuario modifica **REP-FEC-FECHA-INICIO** o **REP-FEC-FECHA-FIN**.<br>5. El usuario presiona **REP-BTN-CONFIRMAR-ACTUALIZACION**.<br>6. El sistema valida el nuevo intervalo.<br>7. El sistema recalcula el reporte consultando en modo de solo lectura **DB_FARMASIL.TBL_REGISTRO_VENTAS** y **DB_FARMASIL.TBL_DETALLE_VENTAS**.<br>8. El sistema actualiza únicamente el registro del reporte en **DB_FARMASIL.TBL_REPORTES_VENTAS**.<br>9. El sistema actualiza **REP-TBL-REPORTES**. |
| Postcondición | 1. El reporte queda actualizado con el nuevo intervalo en **DB_FARMASIL.TBL_REPORTES_VENTAS**.<br>2. **DB_FARMASIL.TBL_REGISTRO_VENTAS** y **DB_FARMASIL.TBL_DETALLE_VENTAS** permanecen sin ninguna modificación.<br>3. Los cambios son visibles en **REP-TBL-REPORTES**. |
| Código de artefactos asociados | ART-MKP-REP-0002, ART-MKP-REP-0003 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Reemplaza a la antigua ilación "Restricción de modificación", que no describía una operación sino la ausencia de ella, dejando incompleta la fase Actualizar del CRUD. Solo actualiza el índice y los parámetros del reporte, nunca las ventas de origen.<br>Los campos Versión y Código educción contenían texto explicativo entre paréntesis; se trasladó a este comentario para que ambos campos conserven únicamente su valor codificado.<br>EDU-0010 v05.00 amplió formalmente el alcance del módulo para autorizar la administración de los reportes generados, por lo que esta ilación ya no contradice a su requisito padre. |

| Código ilación | ILA-0024 |
| --- | --- |
| Nombre | Eliminación de un reporte de ventas generado |
| Versión | 02.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0005 |
| Actor | ACT-0001 |
| Fuente | FUE-0002 |
| Experto | Ninguno |
| Código educción | EDU-0010 |
| Código especificación | ESP-0024 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. La tabla **DB_FARMASIL.TBL_REPORTES_VENTAS** contiene al menos un reporte generado.<br>4. Se carga el mockup **ART-MKP-REP-0002**.<br>5. **REP-BTN-ELIMINAR-REPORTE** se encuentra activo. |
| Procedimiento | 1. El usuario selecciona un reporte desde **REP-TBL-REPORTES**.<br>2. El usuario presiona **REP-BTN-ELIMINAR-REPORTE**.<br>3. El sistema carga el modal **REP-MDL-CONFIRMAR-ELIMINACION** con el mensaje **REP-TXT-MSJ**: "¿Está seguro de eliminar este reporte? Las ventas originales no se verán afectadas."<br>4. Si el usuario presiona **REP-BTN-CONFIRMAR-SI**, el sistema elimina el índice y el archivo asociado de **DB_FARMASIL.TBL_REPORTES_VENTAS**.<br>5. Si el usuario presiona **REP-BTN-CONFIRMAR-NO**, el sistema cierra el modal sin cambios.<br>6. El sistema actualiza **REP-TBL-REPORTES**. |
| Postcondición | 1. El reporte queda eliminado de **DB_FARMASIL.TBL_REPORTES_VENTAS**.<br>2. **DB_FARMASIL.TBL_REGISTRO_VENTAS** y **DB_FARMASIL.TBL_DETALLE_VENTAS** permanecen íntegras y sin cambios.<br>3. El reporte deja de mostrarse en **REP-TBL-REPORTES**. |
| Código de artefactos asociados | ART-MKP-REP-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Reemplaza a la antigua ilación "Restricción de eliminación". Solo elimina el reporte generado, que es un archivo de salida sin referencias foráneas, nunca las ventas de origen. La eliminación física es legítima en este caso.<br>Los campos Versión y Código educción contenían texto explicativo entre paréntesis; se trasladó a este comentario.<br>EDU-0010 v05.00 amplió formalmente el alcance del módulo, por lo que esta ilación ya no contradice a su requisito padre. |
---

# Módulo 7 — Gestión de devoluciones a proveedores (EDU-0011)

| Código ilación | ILA-0025 |
| --- | --- |
| Nombre | Registrar orden de devolución |
| Versión | 05.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0011 |
| Código especificación | ESP-0025 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_LOTES** tiene al menos una remesa con `estado_lote` = 'Bloqueado por devolucion', o con fecha de vencimiento dentro del umbral configurado en **DB_FARMASIL.TBL_ALERTAS_VENCIMIENTO**.<br>5. La tabla **DB_FARMASIL.TBL_PROVEEDORES** tiene entradas válidas con `estado` = 'Activo'.<br>6. Se carga el mockup **ART-MKP-DEV-0001**. |
| Procedimiento | 1. El usuario accede al módulo de devoluciones.<br>2. El usuario registra la fecha de creación de la orden en **DEV-FEC-FECHA-DEVOLUCION**.<br>3. El usuario selecciona el proveedor al que se remitirá la mercancía en **DEV-CMB-PROVEEDOR**.<br>4. El usuario selecciona el motivo de la devolución en **DEV-CMB-MOTIVO-DEVOLUCION**, con los valores 'Por Vencimiento' o 'Por Depuracion de Error'.<br>5. El usuario selecciona la remesa a devolver en **DEV-CMB-LOTE-DEVOLUCION**, que lista únicamente remesas de medicamentos abastecidos por el proveedor seleccionado y en condición de cuarentena o por vencer, identificadas por medicamento y número de lote.<br>6. El usuario registra las unidades a devolver en **DEV-NUM-CANTIDAD-DEVOLVER** y agrega la línea al detalle mostrado en **DEV-TBL-DETALLE-DEVOLUCION**.<br>7. El usuario repite los pasos 5 y 6 por cada remesa que forme parte de la orden.<br>8. El usuario registra observaciones en **DEV-TXA-COMENTARIO**, obligatorio cuando el motivo es la depuración de un error.<br>9. El usuario presiona **DEV-BTN-CREAR-DEVOLUCION**.<br>10. El sistema valida que la orden tenga al menos una remesa, que cada línea tenga una cantidad entera positiva que no exceda el `stock_actual` de su remesa y que todas correspondan al proveedor seleccionado.<br>11. El sistema inicia una transacción, registra la cabecera en **DB_FARMASIL.TBL_ORDENES_DEVOLUCION** con `estado_orden` = 'Pendiente' y el `id_usuario` de la sesión activa, registra una fila por cada remesa en **DB_FARMASIL.TBL_DETALLE_DEVOLUCION** y confirma la transacción.<br>12. El sistema actualiza **DEV-TBL-ORDENES-DEVOLUCION**. |
| Postcondición | 1. La cabecera de la orden queda almacenada en **DB_FARMASIL.TBL_ORDENES_DEVOLUCION** con estado 'Pendiente' y el usuario responsable registrado.<br>2. Existe una fila en **DB_FARMASIL.TBL_DETALLE_DEVOLUCION** por cada remesa incluida en la orden, identificada por su `id_lote`.<br>3. La orden aparece en **DEV-TBL-ORDENES-DEVOLUCION**.<br>4. El `stock_actual` de las remesas aún no se modifica: el descuento se aplica cuando la orden pasa a estado 'Completada' según ILA-0027. |
| Código de artefactos asociados | ART-MKP-DEV-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La versión anterior cargaba el mockup **ART-MKP-VEN-0001**, del módulo de ventas, y declaraba sus artefactos asociados como "Pendiente"; el procedimiento además decía que el usuario "guarda el formulario de venta". Se corrigieron ambas cosas.<br>La orden se modelaba con un único medicamento, ignorando **TBL_DETALLE_DEVOLUCION**, que existe en el modelo precisamente para agrupar varios productos en una misma orden. Se reescribió sobre el par cabecera y detalle.<br>Se incorporaron `motivo_devolucion`, `estado_orden` e `id_usuario`, campos obligatorios de la tabla que la ilación no contemplaba, y se corrigieron las referencias a las tablas inexistentes **BD-FARMASIL.INV_TBL_PRODUCTOS** y **DEV_TBL_ORDENES_DEVOLUCION**.<br>El actor era ACT-0002, en contradicción con EDU-0011, que asigna esta responsabilidad a ACT-0001; se alineó con la educción.<br>Queda Pendiente porque el mockup **ART-MKP-DEV-0001** debe incorporar la tabla de detalle de la orden. Ver Anexo. |

| Código ilación | ILA-0026 |
| --- | --- |
| Nombre | Consultar orden de devolución |
| Versión | 04.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0011 |
| Código especificación | ESP-0026 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_ORDENES_DEVOLUCION** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-DEV-0001**. |
| Procedimiento | 1. El sistema muestra el listado de órdenes de devolución en **DEV-TBL-ORDENES-DEVOLUCION**, con su código, fecha, proveedor, motivo y estado.<br>2. El usuario selecciona una orden específica y presiona **DEV-BTN-LEER-DEVOLUCION**.<br>3. El sistema carga el mockup **ART-MKP-DEV-0002**.<br>4. El sistema consulta **DB_FARMASIL.TBL_ORDENES_DEVOLUCION** y **DB_FARMASIL.TBL_DETALLE_DEVOLUCION**, resuelve la remesa contra **DB_FARMASIL.TBL_LOTES**, el nombre del medicamento contra **DB_FARMASIL.TBL_PRODUCTOS** y el proveedor contra **DB_FARMASIL.TBL_PROVEEDORES**.<br>5. El sistema muestra el detalle de la orden: datos de cabecera y, por cada medicamento, su nombre, número de lote y cantidad a devolver. |
| Postcondición | 1. El sistema muestra la orden y su detalle completo.<br>2. Ningún dato es modificado en **DB_FARMASIL**. |
| Código de artefactos asociados | ART-MKP-DEV-0001, ART-MKP-DEV-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se corrigieron el código **ILA_0026**, el código de educción **EDU-00011**, que tenía cinco dígitos, y las referencias a la tabla inexistente **DEV_TBL_ORDENES_DEVOLUCION**.<br>La postcondición decía únicamente "el sistema permanece invariable", sin declarar el resultado de la consulta; se completó.<br>El campo Fuente decía "Ninguno" pese a que el requisito proviene del Registro de Entrevista 1; se corrigió y queda pendiente asignarle su código FUE definitivo. |

| Código ilación | ILA-0027 |
| --- | --- |
| Nombre | Actualizar orden de devolución |
| Versión | 04.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0011 |
| Código especificación | ESP-0027 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_ORDENES_DEVOLUCION** tiene entradas válidas.<br>5. La orden seleccionada se encuentra en estado 'Pendiente'.<br>6. Se carga el mockup **ART-MKP-DEV-0001**. |
| Procedimiento | 1. El usuario busca y selecciona la orden de devolución en **DEV-TBL-ORDENES-DEVOLUCION**.<br>2. El usuario presiona **DEV-BTN-ACTUALIZAR-DEVOLUCION**.<br>3. El sistema verifica que la orden se encuentre en estado 'Pendiente'. Si ya está 'Completada', cancela la operación e informa que la orden fue despachada.<br>4. El sistema carga el mockup **ART-MKP-DEV-0003** con la cabecera y el detalle vigentes.<br>5. El usuario modifica el proveedor en **DEV-CMB-PROVEEDOR**, el motivo en **DEV-CMB-MOTIVO-DEVOLUCION**, las observaciones en **DEV-TXA-COMENTARIO** o las líneas de remesas y sus cantidades en **DEV-NUM-CANTIDAD-DEVOLVER**.<br>6. El usuario puede marcar la orden como despachada seleccionando el estado 'Completada'.<br>7. El usuario presiona **DEV-BTN-CONFIRMAR-ACTUALIZACION**.<br>8. El sistema inicia una transacción y actualiza la cabecera en **DB_FARMASIL.TBL_ORDENES_DEVOLUCION** y sus líneas en **DB_FARMASIL.TBL_DETALLE_DEVOLUCION**.<br>9. Si la orden pasó a 'Completada', el sistema descuenta del `stock_actual` de **DB_FARMASIL.TBL_LOTES** las cantidades devueltas de cada línea y marca como 'Agotado' toda remesa cuyo stock quede en cero.<br>10. El sistema confirma la transacción y actualiza **DEV-TBL-ORDENES-DEVOLUCION**. |
| Postcondición | 1. La cabecera y el detalle de la orden quedan actualizados de forma consistente.<br>2. Si la orden fue completada, el `stock_actual` de las remesas devueltas queda descontado en **DB_FARMASIL.TBL_LOTES** y la orden ya no admite modificaciones.<br>3. La orden actualizada se muestra en **DEV-TBL-ORDENES-DEVOLUCION**. |
| Código de artefactos asociados | ART-MKP-DEV-0001, ART-MKP-DEV-0003 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Se corrigieron el código **ILA_0027**, el código de educción **EDU-00011** y el componente **DEV-BTN-CONFIRMAR-ACTUALIZACIÓN**, que llevaba tilde a diferencia del resto de identificadores.<br>La postcondición anterior afirmaba que "los medicamentos actualizan su estado correspondiente" sin precisar cuál era ese estado ni qué campo se modificaba. Se sustituyó por el descuento efectivo de stock al completarse la orden, que es el momento en que la mercancía sale físicamente del almacén.<br>Se agregó el bloqueo de las órdenes ya despachadas y se alineó el actor con EDU-0011.<br>Queda Pendiente por la dependencia de mockup indicada en ILA-0025. |

| Código ilación | ILA-0028 |
| --- | --- |
| Nombre | Eliminar orden de devolución |
| Versión | 04.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0004 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0011 |
| Código especificación | ESP-0028 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_ORDENES_DEVOLUCION** tiene entradas válidas.<br>5. La orden seleccionada se encuentra en estado 'Pendiente'.<br>6. Se carga el mockup **ART-MKP-DEV-0001**. |
| Procedimiento | 1. El usuario busca y selecciona la orden de devolución en **DEV-TBL-ORDENES-DEVOLUCION**.<br>2. El usuario presiona **DEV-BTN-ELIMINAR-DEVOLUCION**.<br>3. El sistema verifica que la orden no esté en estado 'Completada'. Si lo está, cancela la operación, ya que la mercancía fue despachada y el registro debe conservarse.<br>4. El sistema carga el mockup **ART-MKP-DEV-0004** y muestra el modal **DEV-MDL-CONFIRMAR-ELIMINACION** con el mensaje **DEV-TXT-MSJ**: "¿Está seguro de que desea eliminar la orden?"<br>5. Si el usuario presiona **DEV-BTN-CONFIRMAR-SI**, el sistema inicia una transacción, elimina las líneas asociadas en **DB_FARMASIL.TBL_DETALLE_DEVOLUCION**, actualiza `estado_orden` a 'Eliminada' en **DB_FARMASIL.TBL_ORDENES_DEVOLUCION** y confirma la transacción.<br>6. Si el usuario presiona **DEV-BTN-CONFIRMAR-NO**, el sistema cierra el modal sin realizar cambios.<br>7. El sistema actualiza **DEV-TBL-ORDENES-DEVOLUCION**. |
| Postcondición | 1. La orden queda con `estado_orden` = 'Eliminada' y deja de listarse entre las órdenes vigentes, conservándose para auditoría.<br>2. El detalle asociado queda eliminado, sin dejar filas huérfanas en **DB_FARMASIL.TBL_DETALLE_DEVOLUCION**.<br>3. Las remesas involucradas conservan su `stock_actual` y su estado de bloqueo, ya que la mercancía nunca salió del almacén.<br>4. Ninguna orden despachada resulta eliminada. |
| Código de artefactos asociados | ART-MKP-DEV-0001, ART-MKP-DEV-0004 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La fila de artefactos declaraba **ART-MKP-DEV-0003**, el mockup de actualización, mientras el procedimiento cargaba **ART-MKP-DEV-0004**; se corrigió a favor del mockup de eliminación.<br>La eliminación se resolvió como baja lógica usando el valor 'Eliminada' que el propio diccionario define en `estado_orden`, en lugar del borrado físico anterior.<br>La postcondición afirmaba que los medicamentos "actualizan su estado correspondiente" al eliminar la orden, lo cual es incorrecto: si la devolución se cancela, el producto debe conservar el bloqueo que motivó la orden.<br>Se corrigieron el código **ILA_0028**, el código de educción **EDU-00011** y los componentes escritos con guion bajo. |

---

# Módulo 8 — Gestión de alertas de restricciones de venta (EDU-0012)

| Código ilación | ILA-0029 |
| --- | --- |
| Nombre | Registrar alerta de restricción de venta |
| Versión | 06.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0006 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0012 |
| Código especificación | ESP-0029 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. La tabla **DB_FARMASIL.TBL_PRODUCTOS** tiene registros válidos.<br>4. La tabla **DB_FARMASIL.TBL_RESTRICCIONES_VENTA** está creada.<br>5. Se carga el mockup **ART-MKP-RES-0001**. |
| Procedimiento | 1. El usuario accede al módulo de alertas de restricción de venta.<br>2. El usuario selecciona el medicamento afectado en **RES-CMB-MEDICAMENTO**.<br>3. El usuario selecciona el tipo de restricción en **RES-CMB-TIPO-RESTRICCION**, con los valores 'Informativa' o 'Bloqueante'.<br>4. El usuario registra la condición del cliente en **RES-TXT-CONDICION-CLIENTE**.<br>5. El usuario registra el motivo de advertencia en **RES-TXA-MOTIVO-ADVERTENCIA**, que es el texto que verá el personal durante la venta.<br>6. El usuario selecciona el estado de la alerta en **RES-CMB-ESTADO-ALERTA**.<br>7. El usuario presiona **RES-BTN-CREAR-RESTRICCION**.<br>8. El sistema valida que los campos obligatorios estén completos y que el medicamento exista en el inventario.<br>9. El sistema registra la información en **DB_FARMASIL.TBL_RESTRICCIONES_VENTA**.<br>10. El sistema actualiza **RES-TBL-RESTRICCIONES-VENTA**. |
| Postcondición | 1. La restricción queda almacenada en **DB_FARMASIL.TBL_RESTRICCIONES_VENTA**, asociada al medicamento seleccionado.<br>2. El registro se visualiza en **RES-TBL-RESTRICCIONES-VENTA**.<br>3. Si la restricción es de tipo 'Bloqueante' y su estado es 'Activo', el sistema impedirá agregar ese medicamento a una venta según el paso de verificación de ILA-0001. |
| Código de artefactos asociados | ART-MKP-RES-0001 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | El módulo usaba el prefijo **ALR_** en todos sus identificadores mientras las especificaciones ya empleaban **RES-**; se unificó al prefijo vigente y se sustituyó el guion bajo por el guion.<br>La ilación invocaba una tabla llamada **ALR_TBL_ALERTAS_RESTRICCION_VENTA** que no existe en el modelo, y usaba ese mismo nombre para referirse indistintamente a la tabla de base de datos y a la tabla de la interfaz. Se separaron: **DB_FARMASIL.TBL_RESTRICCIONES_VENTA** para el almacenamiento y **RES-TBL-RESTRICCIONES-VENTA** para la visualización.<br>El medicamento se capturaba como texto libre pese a que `id_producto` es clave foránea; se corrigió a selección desde el inventario. |

| Código ilación | ILA-0030 |
| --- | --- |
| Nombre | Consultar alerta de restricción de venta |
| Versión | 06.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0006 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0012 |
| Código especificación | ESP-0030 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador' o 'Tecnico'.<br>3. La tabla **DB_FARMASIL.TBL_RESTRICCIONES_VENTA** contiene registros válidos.<br>4. La tabla **DB_FARMASIL.TBL_PRODUCTOS** contiene registros válidos.<br>5. Se carga el mockup **ART-MKP-RES-0002**. |
| Procedimiento | 1. El usuario accede al módulo de alertas de restricción de venta.<br>2. El usuario selecciona el medicamento a consultar en **RES-CMB-MEDICAMENTO**.<br>3. El usuario presiona **RES-BTN-LEER-RESTRICCION**.<br>4. El sistema consulta **DB_FARMASIL.TBL_RESTRICCIONES_VENTA** y resuelve el nombre del medicamento contra **DB_FARMASIL.TBL_PRODUCTOS**.<br>5. El sistema muestra los resultados en **RES-TBL-RESTRICCIONES-VENTA**, indicando la condición del cliente, el tipo de restricción y el estado de cada regla. |
| Postcondición | 1. El sistema muestra las restricciones asociadas al medicamento consultado en **RES-TBL-RESTRICCIONES-VENTA**.<br>2. La información permanece sin modificaciones en **DB_FARMASIL.TBL_RESTRICCIONES_VENTA**. |
| Código de artefactos asociados | ART-MKP-RES-0002 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | Se aplicaron las mismas correcciones de nomenclatura que en ILA-0029.<br>Facilita la verificación previa de medicamentos que requieren advertencias antes de completar una venta. |

| Código ilación | ILA-0031 |
| --- | --- |
| Nombre | Modificar alerta de restricción de venta |
| Versión | 06.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0006 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0012 |
| Código especificación | ESP-0031 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Existen registros en **DB_FARMASIL.TBL_RESTRICCIONES_VENTA** y la alerta seleccionada existe.<br>4. Se carga el mockup **ART-MKP-RES-0003**. |
| Procedimiento | 1. El usuario accede al módulo de alertas de restricción de venta y consulta la alerta que desea modificar.<br>2. El usuario selecciona el registro en **RES-TBL-RESTRICCIONES-VENTA**.<br>3. El usuario presiona **RES-BTN-ACTUALIZAR-RESTRICCION**.<br>4. El sistema carga la información existente en el mockup **ART-MKP-RES-0003**.<br>5. El usuario modifica el medicamento en **RES-CMB-MEDICAMENTO**, el tipo de restricción en **RES-CMB-TIPO-RESTRICCION**, la condición del cliente en **RES-TXT-CONDICION-CLIENTE**, el motivo de advertencia en **RES-TXA-MOTIVO-ADVERTENCIA** o el estado en **RES-CMB-ESTADO-ALERTA**.<br>6. El usuario presiona **RES-BTN-CONFIRMAR-ACTUALIZACION**.<br>7. El sistema valida la información ingresada.<br>8. El sistema actualiza el registro en **DB_FARMASIL.TBL_RESTRICCIONES_VENTA**.<br>9. El sistema actualiza **RES-TBL-RESTRICCIONES-VENTA**. |
| Postcondición | 1. La restricción queda actualizada en **DB_FARMASIL.TBL_RESTRICCIONES_VENTA**.<br>2. La información modificada se visualiza en **RES-TBL-RESTRICCIONES-VENTA**.<br>3. El sistema aplicará la nueva configuración en las validaciones de venta posteriores. |
| Código de artefactos asociados | ART-MKP-RES-0003 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | El procedimiento anterior no incluía el paso de confirmación pese a que el componente **RES-BTN-CONFIRMAR-ACTUALIZACION** ya estaba definido en las especificaciones; se incorporó.<br>Se aplicaron las mismas correcciones de nomenclatura que en ILA-0029. |

| Código ilación | ILA-0032 |
| --- | --- |
| Nombre | Desactivar alerta de restricción de venta |
| Versión | 06.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0006 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0012 |
| Código especificación | ESP-0032 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Existen registros en **DB_FARMASIL.TBL_RESTRICCIONES_VENTA** y la alerta seleccionada existe.<br>4. Se carga el mockup **ART-MKP-RES-0004**. |
| Procedimiento | 1. El usuario accede al módulo de alertas de restricción de venta y consulta la alerta que desea desactivar.<br>2. El usuario selecciona el registro correspondiente en **RES-TBL-RESTRICCIONES-VENTA**.<br>3. El usuario presiona **RES-BTN-ELIMINAR-RESTRICCION**.<br>4. El sistema muestra el modal **RES-MDL-CONFIRMAR-ELIMINACION** con el mensaje **RES-TXT-MSJ**: "¿Está seguro de desactivar esta restricción? El medicamento dejará de mostrar la advertencia durante la venta."<br>5. Si el usuario presiona **RES-BTN-CONFIRMAR-SI**, el sistema actualiza `estado_alerta` a 'Inactivo' en **DB_FARMASIL.TBL_RESTRICCIONES_VENTA**.<br>6. Si el usuario presiona **RES-BTN-CONFIRMAR-NO**, el sistema cierra el modal sin realizar cambios.<br>7. El sistema actualiza **RES-TBL-RESTRICCIONES-VENTA**. |
| Postcondición | 1. La restricción queda marcada como 'Inactivo' en **DB_FARMASIL.TBL_RESTRICCIONES_VENTA**.<br>2. La restricción deja de participar en la validación de ventas descrita en ILA-0001.<br>3. Se conserva el historial de la restricción para auditoría sanitaria.<br>4. El nuevo estado se refleja en **RES-TBL-RESTRICCIONES-VENTA**. |
| Código de artefactos asociados | ART-MKP-RES-0004 |
| Importancia | Vital |
| Estado | Concluido |
| Comentario | La ilación ya resolvía correctamente la fase Eliminar como baja lógica, criterio adecuado tratándose de información de farmacovigilancia. Se ajustó el nombre para que refleje la operación real y se aplicaron las correcciones de nomenclatura del módulo. |

---

# Módulo 9 — Gestión de usuarios (EDU-0013)

| Código ilación | ILA-0033 |
| --- | --- |
| Nombre | Creación de usuario |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Consulta posterior al cliente |
| Experto | Ninguno |
| Código educción | EDU-0013 |
| Código especificación | ESP-0033 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. Se carga el mockup **ART-MKP-USR-0001**.<br>5. Los campos **USR-TXT-NOMBRE-USUARIO**, **USR-PWD-CONTRASENA**, **USR-CMB-ROL** y **USR-CMB-ESTADO-USUARIO** se encuentran habilitados.<br>6. El botón **USR-BTN-CREAR-USUARIO** se encuentra habilitado. |
| Procedimiento | 1. La dueña accede al módulo de gestión de usuarios.<br>2. La dueña ingresa el nombre de usuario en **USR-TXT-NOMBRE-USUARIO**.<br>3. La dueña ingresa la contraseña en **USR-PWD-CONTRASENA**, cuyo contenido se muestra enmascarado.<br>4. La dueña selecciona el rol en **USR-CMB-ROL**, con los valores 'Administrador' o 'Tecnico'.<br>5. La dueña selecciona el estado inicial de la cuenta en **USR-CMB-ESTADO-USUARIO**.<br>6. La dueña presiona **USR-BTN-CREAR-USUARIO**.<br>7. El sistema valida que el nombre de usuario no esté vacío y que no exista otra cuenta con el mismo nombre, dado que `nombre_usuario` es único.<br>8. El sistema encripta la contraseña antes de persistirla, conforme a la condición 5 de RNF-0006.<br>9. El sistema registra la cuenta en **DB_FARMASIL.TBL_USUARIOS**.<br>10. El sistema actualiza **USR-TBL-USUARIOS**. |
| Postcondición | 1. La cuenta queda registrada en **DB_FARMASIL.TBL_USUARIOS** con la contraseña almacenada de forma encriptada.<br>2. La cuenta aparece en **USR-TBL-USUARIOS** sin exponer la contraseña en ninguna columna.<br>3. Si su estado es 'Activo', la cuenta queda habilitada para iniciar sesión según la condición 1 de RNF-0006 y para ser registrada como responsable de una venta o de una orden de devolución. |
| Código de artefactos asociados | ART-MKP-USR-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Módulo nuevo, derivado de la confirmación de la dueña de que requiere saber qué personal realiza cada venta.<br>La tabla **TBL_USUARIOS** ya existe en el Diccionario de Datos y es referenciada desde **TBL_ORDENES_DEVOLUCION**, pero ninguna ilación alimentaba su contenido: las cuentas existían en el modelo sin ningún procedimiento que las creara.<br>Queda Pendiente porque requiere el mockup **ART-MKP-USR-0001**, que aún no ha sido diseñado. Ver Anexo. |

| Código ilación | ILA-0034 |
| --- | --- |
| Nombre | Consulta de usuarios |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Consulta posterior al cliente |
| Experto | Ninguno |
| Código educción | EDU-0013 |
| Código especificación | ESP-0034 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_USUARIOS** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-USR-0002**. |
| Procedimiento | 1. La dueña accede al módulo de gestión de usuarios.<br>2. La dueña ingresa el nombre de usuario a buscar en **USR-TXT-NOMBRE-USUARIO** o filtra por rol en **USR-CMB-ROL**.<br>3. La dueña presiona **USR-BTN-LEER-USUARIO**.<br>4. El sistema consulta **DB_FARMASIL.TBL_USUARIOS**.<br>5. El sistema muestra en **USR-TBL-USUARIOS** el identificador, el nombre de usuario, el rol y el estado de cada cuenta encontrada. |
| Postcondición | 1. Las cuentas consultadas se visualizan en **USR-TBL-USUARIOS**.<br>2. El campo de contraseña no se muestra ni se devuelve en ningún resultado de la consulta.<br>3. No se modifica ninguna información en **DB_FARMASIL.TBL_USUARIOS**. |
| Código de artefactos asociados | ART-MKP-USR-0002 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La exclusión explícita de la contraseña en el resultado de la consulta responde al atributo de seguridad declarado en RNF-0006.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-USR-0002**. |

| Código ilación | ILA-0035 |
| --- | --- |
| Nombre | Actualización de usuario |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Consulta posterior al cliente |
| Experto | Ninguno |
| Código educción | EDU-0013 |
| Código especificación | ESP-0035 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_USUARIOS** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-USR-0003**. |
| Procedimiento | 1. La dueña selecciona una cuenta en **USR-TBL-USUARIOS**.<br>2. La dueña presiona **USR-BTN-ACTUALIZAR-USUARIO**.<br>3. El sistema carga el mockup **ART-MKP-USR-0003** con la información vigente de la cuenta, dejando **USR-PWD-CONTRASENA** en blanco.<br>4. La dueña modifica el nombre de usuario en **USR-TXT-NOMBRE-USUARIO**, el rol en **USR-CMB-ROL** o el estado en **USR-CMB-ESTADO-USUARIO**. Si desea restablecer la contraseña, ingresa una nueva en **USR-PWD-CONTRASENA**; si deja el campo en blanco, la contraseña vigente se conserva.<br>5. La dueña presiona **USR-BTN-CONFIRMAR-ACTUALIZACION**.<br>6. El sistema valida que el nuevo nombre de usuario no colisione con otra cuenta.<br>7. El sistema verifica que la operación no deje al sistema sin ninguna cuenta activa con rol 'Administrador'.<br>8. El sistema actualiza el registro en **DB_FARMASIL.TBL_USUARIOS**, encriptando la contraseña si fue modificada.<br>9. El sistema actualiza **USR-TBL-USUARIOS**. |
| Postcondición | 1. La cuenta queda actualizada en **DB_FARMASIL.TBL_USUARIOS**.<br>2. El identificador de la cuenta permanece inalterado, preservando las referencias desde **TBL_ORDENES_DEVOLUCION** y desde el histórico de ventas.<br>3. Siempre existe al menos una cuenta activa con rol de administrador.<br>4. Los cambios se reflejan en **USR-TBL-USUARIOS**. |
| Código de artefactos asociados | ART-MKP-USR-0003 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La validación del paso 7 evita el bloqueo total del sistema: si la única cuenta administradora se degrada a técnico o se desactiva, nadie podría volver a administrar usuarios.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-USR-0003**. |

| Código ilación | ILA-0036 |
| --- | --- |
| Nombre | Desactivación de usuario |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Consulta posterior al cliente |
| Experto | Ninguno |
| Código educción | EDU-0013 |
| Código especificación | ESP-0036 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_USUARIOS** tiene entradas válidas.<br>5. La cuenta seleccionada no es la cuenta con la que se encuentra abierta la sesión actual.<br>6. Se carga el mockup **ART-MKP-USR-0004**. |
| Procedimiento | 1. La dueña selecciona una cuenta en **USR-TBL-USUARIOS**.<br>2. La dueña presiona **USR-BTN-ELIMINAR-USUARIO**.<br>3. El sistema muestra el modal **USR-MDL-CONFIRMAR-ELIMINACION** con el mensaje **USR-TXT-MSJ**: "¿Está seguro de dar de baja esta cuenta? El histórico de operaciones que registró se conservará."<br>4. El sistema verifica si la cuenta tiene operaciones asociadas en **DB_FARMASIL.TBL_ORDENES_DEVOLUCION** o en **DB_FARMASIL.TBL_REGISTRO_VENTAS**.<br>5. Si la dueña presiona **USR-BTN-CONFIRMAR-SI** y la cuenta no tiene operaciones asociadas, el sistema elimina el registro de **DB_FARMASIL.TBL_USUARIOS**.<br>6. Si la dueña presiona **USR-BTN-CONFIRMAR-SI** y la cuenta sí tiene operaciones asociadas, el sistema actualiza su `estado` a 'Inactivo', conservando el registro.<br>7. Si la dueña presiona **USR-BTN-CONFIRMAR-NO**, el sistema cierra el modal sin realizar cambios.<br>8. El sistema actualiza **USR-TBL-USUARIOS**. |
| Postcondición | 1. La cuenta sin operaciones asociadas queda eliminada de **DB_FARMASIL.TBL_USUARIOS**.<br>2. La cuenta con operaciones asociadas queda en estado 'Inactivo' y conserva la trazabilidad de las ventas y devoluciones que registró.<br>3. En ambos casos la cuenta deja de poder iniciar sesión.<br>4. La sesión activa nunca resulta desactivada a sí misma.<br>5. No se produce ninguna violación de integridad referencial. |
| Código de artefactos asociados | ART-MKP-USR-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La baja lógica es obligatoria aquí: eliminar físicamente una cuenta que registró ventas o devoluciones destruiría precisamente la verificación de identidad que la dueña solicitó.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-USR-0004**. |

---

# Módulo 10 — Gestión de proveedores (EDU-0014)

| Código ilación | ILA-0037 |
| --- | --- |
| Nombre | Registro de proveedor |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0014 |
| Código especificación | ESP-0037 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. Se carga el mockup **ART-MKP-PRV-0001**.<br>5. Los campos **PRV-TXT-RUC**, **PRV-TXT-RAZON-SOCIAL**, **PRV-TXT-TELEFONO** y **PRV-CMB-ESTADO-PROVEEDOR** se encuentran habilitados.<br>6. El botón **PRV-BTN-CREAR-PROVEEDOR** se encuentra habilitado. |
| Procedimiento | 1. La dueña accede al módulo de gestión de proveedores.<br>2. La dueña ingresa el RUC de la empresa en **PRV-TXT-RUC**.<br>3. La dueña ingresa la razón social en **PRV-TXT-RAZON-SOCIAL**.<br>4. La dueña ingresa el teléfono de contacto en **PRV-TXT-TELEFONO**, campo opcional.<br>5. La dueña selecciona el estado inicial en **PRV-CMB-ESTADO-PROVEEDOR**.<br>6. La dueña presiona **PRV-BTN-CREAR-PROVEEDOR**.<br>7. El sistema valida que el RUC tenga once dígitos numéricos, que no exista ya un proveedor con el mismo RUC y que la razón social no esté vacía.<br>8. El sistema registra el proveedor en **DB_FARMASIL.TBL_PROVEEDORES**.<br>9. El sistema actualiza **PRV-TBL-PROVEEDORES**. |
| Postcondición | 1. El proveedor queda registrado en **DB_FARMASIL.TBL_PROVEEDORES**.<br>2. El proveedor aparece en **PRV-TBL-PROVEEDORES**.<br>3. Si su estado es 'Activo', el proveedor queda disponible para ser asociado a un producto en **INV-CMB-PROVEEDOR** y para ser destinatario de una orden de devolución en **DEV-CMB-PROVEEDOR**. |
| Código de artefactos asociados | ART-MKP-PRV-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Módulo nuevo. La tabla **TBL_PROVEEDORES** ya existía en el modelo y es referenciada desde **TBL_PRODUCTOS** y **TBL_ORDENES_DEVOLUCION**, pero ninguna ilación la alimentaba: ILA-0025 exigía como precondición que hubiera proveedores válidos sin que existiera procedimiento alguno para crearlos.<br>La validación de once dígitos corresponde a la restricción `VARCHAR(11)` y UNIQUE definida en el Diccionario de Datos.<br>Queda Pendiente porque requiere el mockup **ART-MKP-PRV-0001**, que aún no ha sido diseñado. Ver Anexo. |

| Código ilación | ILA-0038 |
| --- | --- |
| Nombre | Consulta de proveedores |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0014 |
| Código especificación | ESP-0038 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_PROVEEDORES** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-PRV-0002**. |
| Procedimiento | 1. La dueña accede al módulo de gestión de proveedores.<br>2. La dueña ingresa el RUC en **PRV-TXT-RUC** o la razón social en **PRV-TXT-RAZON-SOCIAL** como criterio de búsqueda.<br>3. La dueña presiona **PRV-BTN-LEER-PROVEEDOR**.<br>4. El sistema consulta **DB_FARMASIL.TBL_PROVEEDORES**.<br>5. El sistema muestra en **PRV-TBL-PROVEEDORES** el identificador, el RUC, la razón social, el teléfono y el estado de cada proveedor encontrado. |
| Postcondición | 1. Los proveedores consultados se visualizan en **PRV-TBL-PROVEEDORES**.<br>2. No se modifica ninguna información en **DB_FARMASIL.TBL_PROVEEDORES**. |
| Código de artefactos asociados | ART-MKP-PRV-0002 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Permite a la dueña ubicar rápidamente los datos de contacto de la droguería o distribuidora al momento de coordinar el retiro de un lote, que según la Entrevista 1 se gestiona por comunicación directa con el proveedor.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-PRV-0002**. |

| Código ilación | ILA-0039 |
| --- | --- |
| Nombre | Actualización de proveedor |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0014 |
| Código especificación | ESP-0039 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_PROVEEDORES** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-PRV-0003**. |
| Procedimiento | 1. La dueña selecciona un proveedor en **PRV-TBL-PROVEEDORES**.<br>2. La dueña presiona **PRV-BTN-ACTUALIZAR-PROVEEDOR**.<br>3. El sistema carga el mockup **ART-MKP-PRV-0003** con la información vigente del proveedor.<br>4. La dueña modifica el RUC en **PRV-TXT-RUC**, la razón social en **PRV-TXT-RAZON-SOCIAL**, el teléfono en **PRV-TXT-TELEFONO** o el estado en **PRV-CMB-ESTADO-PROVEEDOR**.<br>5. La dueña presiona **PRV-BTN-CONFIRMAR-ACTUALIZACION**.<br>6. El sistema valida el formato del RUC y que no colisione con el de otro proveedor registrado.<br>7. El sistema actualiza el registro en **DB_FARMASIL.TBL_PROVEEDORES**.<br>8. El sistema actualiza **PRV-TBL-PROVEEDORES**. |
| Postcondición | 1. El proveedor queda actualizado en **DB_FARMASIL.TBL_PROVEEDORES**.<br>2. El identificador del proveedor permanece inalterado, preservando las referencias desde **TBL_PRODUCTOS** y **TBL_ORDENES_DEVOLUCION**.<br>3. Si el proveedor pasó a estado 'Inactivo', deja de ofrecerse en **INV-CMB-PROVEEDOR** y en **DEV-CMB-PROVEEDOR**, pero los productos y las órdenes ya asociados conservan su referencia.<br>4. Los cambios se reflejan en **PRV-TBL-PROVEEDORES**. |
| Código de artefactos asociados | ART-MKP-PRV-0003 |
| Importancia | Media |
| Estado | Pendiente |
| Comentario | Permite mantener vigentes los datos de contacto de las distribuidoras, que cambian con más frecuencia que la propia relación comercial.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-PRV-0003**. |

| Código ilación | ILA-0040 |
| --- | --- |
| Nombre | Desactivación de proveedor |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0014 |
| Código especificación | ESP-0040 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_PROVEEDORES** tiene entradas válidas.<br>5. El proveedor seleccionado no tiene órdenes de devolución en estado 'Pendiente' en **DB_FARMASIL.TBL_ORDENES_DEVOLUCION**.<br>6. Se carga el mockup **ART-MKP-PRV-0004**. |
| Procedimiento | 1. La dueña selecciona un proveedor en **PRV-TBL-PROVEEDORES**.<br>2. La dueña presiona **PRV-BTN-ELIMINAR-PROVEEDOR**.<br>3. El sistema verifica que el proveedor no tenga órdenes de devolución pendientes de despacho. Si las tiene, cancela la operación e informa que deben completarse o eliminarse primero.<br>4. El sistema muestra el modal **PRV-MDL-CONFIRMAR-ELIMINACION** con el mensaje **PRV-TXT-MSJ**: "¿Está seguro de dar de baja este proveedor? Los productos y devoluciones ya registrados no se verán afectados."<br>5. El sistema verifica si el proveedor tiene productos asociados en **DB_FARMASIL.TBL_PRODUCTOS** u órdenes históricas en **DB_FARMASIL.TBL_ORDENES_DEVOLUCION**.<br>6. Si la dueña presiona **PRV-BTN-CONFIRMAR-SI** y el proveedor no tiene registros asociados, el sistema lo elimina de **DB_FARMASIL.TBL_PROVEEDORES**.<br>7. Si la dueña presiona **PRV-BTN-CONFIRMAR-SI** y el proveedor sí tiene registros asociados, el sistema actualiza su `estado` a 'Inactivo', conservando el registro.<br>8. Si la dueña presiona **PRV-BTN-CONFIRMAR-NO**, el sistema cierra el modal sin realizar cambios.<br>9. El sistema actualiza **PRV-TBL-PROVEEDORES**. |
| Postcondición | 1. El proveedor sin registros asociados queda eliminado de **DB_FARMASIL.TBL_PROVEEDORES**.<br>2. El proveedor con registros asociados queda en estado 'Inactivo' y conserva las referencias desde productos y órdenes de devolución.<br>3. En ambos casos el proveedor deja de ofrecerse en **INV-CMB-PROVEEDOR** y en **DEV-CMB-PROVEEDOR**.<br>4. Ninguna orden de devolución pendiente queda sin destinatario.<br>5. No se produce ninguna violación de integridad referencial. |
| Código de artefactos asociados | ART-MKP-PRV-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Se aplica el mismo criterio de baja lógica del resto del catálogo, más una verificación adicional: un proveedor con una devolución aún no despachada no puede darse de baja, porque la orden quedaría sin destinatario.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-PRV-0004**. |

---

# Anexo — Pendientes derivados de esta revisión

## A. Cambios en la base de datos — **aplicados en el Diccionario de Datos v03.00**

| # | Tabla | Cambio | Origen |
| --- | --- | --- | --- |
| 1 | `TBL_REGISTRO_VENTAS` | Agregar `id_usuario INTEGER FK (TBL_USUARIOS)` | Confirmado por la dueña: requiere verificación de identidad para saber quién realiza cada venta. Lo exigen EDU-0010 y EDU-0013. `TBL_ORDENES_DEVOLUCION` ya tiene este campo; la tabla de ventas no. Bloquea ILA-0021. |
| 2 | `TBL_PRODUCTOS` | Agregar `numero_lote VARCHAR(30)` | Ya identificado en `Cambios_Modelo_ER_DB_FARMASIL.md` y presente en el ERD, pero aún ausente del Diccionario de Datos oficial. Lo exigen EDU-0002 y EDU-0004. Bloquea ILA-0005. |
| 3 | `TBL_PRODUCTOS` | Ampliar el dominio de `estado_producto` con el valor `'Descontinuado'` | Necesario para la baja lógica de productos con histórico de ventas. Bloquea ILA-0008. |
| 4 | `TBL_COMPROBANTES_TRIBUTARIOS` | Agregar `numero_comprobante VARCHAR(20) NOT NULL UNIQUE` | La tabla solo tiene `id_comprobante` autoincremental, que es un identificador interno y no el número de serie del documento fiscal. El componente **DOC-TXT-NUMERO-COMPROBANTE** ya existe en las especificaciones sin campo donde persistirse. Bloquea ILA-0009. |

**Venta de varios productos por transacción: no requiere cambio.** `TBL_REGISTRO_VENTAS` y `TBL_DETALLE_VENTAS` ya modelan la cabecera y su detalle con la cardinalidad correcta. El problema estaba en las ilaciones y especificaciones del módulo 1, que ignoraban la tabla de detalle. Con ILA-0001 a ILA-0004 reescritas, el catálogo queda alineado con el modelo.

**Múltiples lotes de un mismo medicamento: decisión abierta.** El documento de cambios al modelo ER ya señala que `TBL_PRODUCTOS` maneja un solo lote y una sola fecha de vencimiento por fila, y lo califica como el punto más frágil del modelo. Al introducir el descuento de stock y las devoluciones por lote, esa limitación se vuelve más visible: el sistema no puede distinguir qué lote se vendió ni cuál se devuelve. Resolverlo requiere una tabla `TBL_LOTES` en relación uno a muchos con `TBL_PRODUCTOS`, lo que impacta los módulos 1, 2, 4 y 7. Es una decisión de alcance que debe tomar el equipo, no un ajuste menor.

## B. Componentes de interfaz que deben incorporarse a los mockups

| Mockup | Componentes nuevos | Motivo |
| --- | --- | --- |
| `ART-MKP-VEN-0001`, `ART-MKP-VEN-0003` | **VEN-BTN-AGREGAR-PRODUCTO**, **VEN-BTN-QUITAR-PRODUCTO**, **VEN-TBL-DETALLE-VENTA**, **VEN-LBL-MONTO-TOTAL** | Venta de varios productos en una misma transacción. |
| `ART-MKP-INV-0001`, `ART-MKP-INV-0003` | **INV-CMB-PROVEEDOR** | Campo obligatorio del diccionario que ningún mockup permite capturar. El lote, el vencimiento y las existencias se retiran de este mockup y pasan al módulo 11. |
| `ART-MKP-LOT-0001` a `ART-MKP-LOT-0004` | Módulo completo, prefijo **LOT** | Mockups aún no diseñados. Gestión de lotes (EDU-0015). |
| `ART-MKP-VEN-0001`, `ART-MKP-VEN-0003` | **VEN-CMB-LOTE-VENTA** | Selección de la remesa de la que se descuenta, según el criterio FEFO. |
| `ART-MKP-DEV-0001`, `ART-MKP-DEV-0003` | Tabla de detalle de la orden de devolución | Varios medicamentos por orden. |
| `ART-MKP-USR-0001` a `ART-MKP-USR-0004` | Módulo completo, prefijo **USR** | Mockups aún no diseñados. Gestión de usuarios (EDU-0013). |
| `ART-MKP-PRV-0001` a `ART-MKP-PRV-0004` | Módulo completo, prefijo **PRV** | Mockups aún no diseñados. Gestión de proveedores (EDU-0014). |

Estos componentes deben nombrarse siguiendo la Guía de Estilo de Nomenclatura de Mockups antes de trasladarse a las especificaciones.

## C. Decisiones que requieren validación del equipo o del cliente

1. ~~**EDU-0010 contradice a ILA-0023 e ILA-0024.**~~ **Resuelto el 05/09/2026.** EDU-0010 se amplió a la versión 05.00 autorizando la administración de los reportes generados y dejando explícito que las ventas de origen son inmutables. Ambas ilaciones pasaron a estado Concluido.
2. **Catálogo de fuentes.** Siete ilaciones citan el Registro de Entrevista 1 sin código FUE, porque no existe un catálogo de fuentes en el proyecto. Los códigos FUE-0001 a FUE-0005 se usan sin que ningún documento defina a qué corresponden.
3. **Catálogo de actores.** ACT-0001 y ACT-0002 se usan en las tres etapas sin estar definidos en ninguna parte. La corrección del actor en las ilaciones del módulo 7 se hizo por coherencia con su educción, no contra un catálogo.

---

# Módulo 11 — Gestión de lotes (EDU-0015)

| Código ilación | ILA-0041 |
| --- | --- |
| Nombre | Registro de remesa |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0015 |
| Código especificación | ESP-0041 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_PRODUCTOS** tiene al menos un medicamento con `estado_producto` = 'Disponible'.<br>5. Se carga el mockup **ART-MKP-LOT-0001**.<br>6. El botón **LOT-BTN-CREAR-LOTE** se encuentra habilitado. |
| Procedimiento | 1. La dueña accede al módulo de gestión de lotes.<br>2. La dueña selecciona el medicamento al que corresponde la remesa en **LOT-CMB-PRODUCTO**.<br>3. La dueña ingresa el número de lote impreso por el laboratorio en **LOT-TXT-NUMERO-LOTE**.<br>4. La dueña selecciona la fecha de vencimiento de la remesa en **LOT-FEC-FECHA-VENCIMIENTO**.<br>5. La dueña ingresa las unidades recibidas en **LOT-NUM-STOCK-LOTE**.<br>6. La dueña registra la fecha de ingreso al almacén en **LOT-FEC-FECHA-INGRESO**.<br>7. La dueña presiona **LOT-BTN-CREAR-LOTE**.<br>8. El sistema valida que la fecha de vencimiento sea posterior a la fecha actual, que las unidades sean un entero no negativo y que no exista ya una remesa con el mismo número de lote para ese medicamento.<br>9. El sistema registra la remesa en **DB_FARMASIL.TBL_LOTES** con `estado_lote` = 'Disponible'.<br>10. El sistema actualiza **LOT-TBL-LOTES**. |
| Postcondición | 1. La remesa queda almacenada en **DB_FARMASIL.TBL_LOTES** asociada a su medicamento.<br>2. La combinación de medicamento y número de lote es única en la tabla.<br>3. La remesa aparece en **LOT-TBL-LOTES**.<br>4. La remesa queda disponible para ser seleccionada en **VEN-CMB-LOTE-VENTA** durante el registro de una venta.<br>5. El stock consolidado del medicamento, mostrado en **INV-TBL-PRODUCTOS**, se incrementa en las unidades recibidas. |
| Código de artefactos asociados | ART-MKP-LOT-0001 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Ilación nueva, derivada de la incorporación de **TBL_LOTES** al Diccionario de Datos v04.00.<br>La restricción de unicidad sobre medicamento y número de lote es la que sostiene el rastreo sanitario: si el mismo lote pudiera registrarse dos veces, no habría forma de responder inequívocamente qué se vendió de una remesa retirada por la DIGEMID.<br>Queda Pendiente porque el mockup **ART-MKP-LOT-0001** aún no ha sido diseñado. |

| Código ilación | ILA-0042 |
| --- | --- |
| Nombre | Consulta de remesas |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001, ACT-0002 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0015 |
| Código especificación | ESP-0042 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador' o 'Tecnico'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_LOTES** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-LOT-0002**. |
| Procedimiento | 1. El usuario accede al módulo de gestión de lotes.<br>2. El usuario aplica los filtros disponibles: medicamento en **LOT-CMB-PRODUCTO**, número de lote en **LOT-TXT-NUMERO-LOTE** o estado en **LOT-CMB-ESTADO-LOTE**.<br>3. El usuario presiona **LOT-BTN-LEER-LOTE**.<br>4. El sistema consulta **DB_FARMASIL.TBL_LOTES** y resuelve el nombre del medicamento contra **DB_FARMASIL.TBL_PRODUCTOS**.<br>5. El sistema muestra en **LOT-TBL-LOTES** el medicamento, el número de lote, la fecha de vencimiento, las existencias y el estado de cada remesa, ordenadas por fecha de vencimiento ascendente. |
| Postcondición | 1. Las remesas consultadas se visualizan en **LOT-TBL-LOTES**.<br>2. El orden por vencimiento ascendente permite identificar de un vistazo qué remesa debe despacharse primero.<br>3. No se modifica ninguna información en **DB_FARMASIL.TBL_LOTES**. |
| Código de artefactos asociados | ART-MKP-LOT-0002 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | Es una de las operaciones disponibles para ambos roles: la técnica necesita saber qué remesa tiene disponible y cuándo vence antes de despachar un medicamento.<br>El orden por vencimiento ascendente es la contraparte visual del criterio FEFO que aplica ILA-0001.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-LOT-0002**. |

| Código ilación | ILA-0043 |
| --- | --- |
| Nombre | Actualización de remesa |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0015 |
| Código especificación | ESP-0043 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_LOTES** tiene entradas válidas.<br>5. Se carga el mockup **ART-MKP-LOT-0003**. |
| Procedimiento | 1. La dueña selecciona una remesa en **LOT-TBL-LOTES**.<br>2. La dueña presiona **LOT-BTN-ACTUALIZAR-LOTE**.<br>3. El sistema carga el mockup **ART-MKP-LOT-0003** con la información vigente de la remesa.<br>4. La dueña modifica el número de lote en **LOT-TXT-NUMERO-LOTE**, la fecha de vencimiento en **LOT-FEC-FECHA-VENCIMIENTO**, las existencias en **LOT-NUM-STOCK-LOTE** o el estado en **LOT-CMB-ESTADO-LOTE**. El medicamento asociado, mostrado en **LOT-CMB-PRODUCTO**, permanece de solo lectura.<br>5. La dueña presiona **LOT-BTN-CONFIRMAR-ACTUALIZACION**.<br>6. El sistema valida que el nuevo número de lote no colisione con otra remesa del mismo medicamento y que las existencias sean un entero no negativo.<br>7. El sistema verifica que las existencias declaradas no sean inferiores a las unidades ya comprometidas en órdenes de devolución pendientes de esa remesa.<br>8. El sistema actualiza el registro en **DB_FARMASIL.TBL_LOTES**.<br>9. El sistema actualiza **LOT-TBL-LOTES**. |
| Postcondición | 1. La remesa queda actualizada en **DB_FARMASIL.TBL_LOTES**.<br>2. El identificador de la remesa permanece inalterado, preservando las referencias desde **TBL_DETALLE_VENTAS** y **TBL_DETALLE_DEVOLUCION**.<br>3. El medicamento asociado a la remesa permanece inalterado: reasignar una remesa a otro medicamento falsearía el histórico de ventas ya registrado.<br>4. Si la remesa pasó a 'Bloqueado por devolucion' o 'Agotado', deja de ofrecerse en **VEN-CMB-LOTE-VENTA**.<br>5. Los cambios se reflejan en **LOT-TBL-LOTES**. |
| Código de artefactos asociados | ART-MKP-LOT-0003 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | El medicamento asociado se declara de solo lectura por la misma razón que el identificador: las ventas ya registradas apuntan a esta remesa, y cambiarle el medicamento reescribiría retroactivamente qué se vendió.<br>La verificación del paso 7 evita que una corrección de inventario deje una orden de devolución comprometiendo unidades que ya no existen.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-LOT-0003**. |

| Código ilación | ILA-0044 |
| --- | --- |
| Nombre | Baja de remesa |
| Versión | 01.00 |
| Fecha | 05/09/2026 |
| Autor de la plantilla | AUT-0001 |
| Actor | ACT-0001 |
| Fuente | Entrevista 1 |
| Experto | Ninguno |
| Código educción | EDU-0015 |
| Código especificación | ESP-0044 |
| Precondición | 1. La base de datos **DB_FARMASIL** está creada.<br>2. El usuario tiene una sesión activa con rol 'Administrador'.<br>3. Se valida la conexión con la base de datos **DB_FARMASIL**.<br>4. La tabla **DB_FARMASIL.TBL_LOTES** tiene entradas válidas.<br>5. La remesa seleccionada no forma parte de una orden de devolución en estado 'Pendiente'.<br>6. Se carga el mockup **ART-MKP-LOT-0004**. |
| Procedimiento | 1. La dueña selecciona una remesa en **LOT-TBL-LOTES**.<br>2. La dueña presiona **LOT-BTN-ELIMINAR-LOTE**.<br>3. El sistema verifica que la remesa no esté comprometida en una orden de devolución pendiente. Si lo está, cancela la operación e informa el motivo.<br>4. El sistema muestra el modal **LOT-MDL-CONFIRMAR-ELIMINACION** con el mensaje **LOT-TXT-MSJ**: "¿Está seguro de dar de baja esta remesa? El histórico de ventas que la involucra se conservará."<br>5. El sistema verifica si la remesa tiene ventas registradas en **DB_FARMASIL.TBL_DETALLE_VENTAS** o devoluciones históricas en **DB_FARMASIL.TBL_DETALLE_DEVOLUCION**.<br>6. Si la dueña presiona **LOT-BTN-CONFIRMAR-SI** y la remesa no tiene movimientos, el sistema la elimina de **DB_FARMASIL.TBL_LOTES**.<br>7. Si la dueña presiona **LOT-BTN-CONFIRMAR-SI** y la remesa sí tiene movimientos, el sistema actualiza su `estado_lote` a 'Agotado' y sus existencias a cero, conservando el registro.<br>8. Si la dueña presiona **LOT-BTN-CONFIRMAR-NO**, el sistema cierra el modal sin realizar cambios.<br>9. El sistema actualiza **LOT-TBL-LOTES**. |
| Postcondición | 1. La remesa sin movimientos queda eliminada de **DB_FARMASIL.TBL_LOTES**.<br>2. La remesa con movimientos queda con `estado_lote` = 'Agotado' y existencias en cero, conservando la trazabilidad de las ventas que la involucran.<br>3. En ambos casos la remesa deja de ofrecerse en **VEN-CMB-LOTE-VENTA**.<br>4. El stock consolidado del medicamento en **INV-TBL-PRODUCTOS** se reduce en las existencias dadas de baja.<br>5. No se produce ninguna violación de integridad referencial. |
| Código de artefactos asociados | ART-MKP-LOT-0004 |
| Importancia | Vital |
| Estado | Pendiente |
| Comentario | La baja lógica es obligatoria cuando la remesa registró ventas: eliminarla físicamente destruiría precisamente el rastreo sanitario que motivó la creación de este módulo.<br>Queda Pendiente por la dependencia del mockup **ART-MKP-LOT-0004**. |




> **Versión corregida y consolidada (05/09/2026).** Las 32 ilaciones fueron revisadas contra el Diccionario de Datos v2.00, el documento de Cambios al Modelo ER, las educciones v04.00 y las especificaciones de los módulos 1-8. Resumen de las correcciones aplicadas:
>
> **1. Nomenclatura de base de datos.** Las ilaciones invocaban tablas que no existen en el Diccionario de Datos: `INV_TBL_PRODUCTOS`, `VEN_TBL_REGISTRO_VENTAS`, `PAG_TBL_METODOS_PAGO`, `DOC_TBL_COMPROBANTES`, `DEV_TBL_ORDENES_DEVOLUCION` y `ALR_TBL_ALERTAS_RESTRICCION_VENTA`. El prefijo de módulo pertenece a los componentes de interfaz, no al esquema. Todas fueron reemplazadas por su nombre real (`TBL_PRODUCTOS`, `TBL_REGISTRO_VENTAS`, `TBL_METODOS_PAGO`, `TBL_COMPROBANTES_TRIBUTARIOS`, `TBL_ORDENES_DEVOLUCION`, `TBL_RESTRICCIONES_VENTA`). También se corrigió `BD_FARMASIL` por `DB_FARMASIL` y las referencias con guion bajo en vez de punto (`DB_FARMASIL_TBL_...`).
>
> **2. Códigos malformados.** `ILA_0011`, `ILA_0012`, `ILA_0025`, `ILA_0026`, `ILA_0027` y `ILA_0028` usaban guion bajo. `EDU-00011` (cinco dígitos) aparecía en ILA-0026, ILA-0027 e ILA-0028. Corregidos.
>
> **3. Venta de varios productos en una sola transacción.** Las ilaciones del módulo 1 modelaban la venta de un único producto por transacción e ignoraban por completo `TBL_DETALLE_VENTAS`, que ya existe en el modelo justamente para eso. Se reescribieron ILA-0001 a ILA-0004 para operar sobre la cabecera y su detalle. **No requiere cambio en la base de datos**: el modelo ya lo soporta, era la ilación la que estaba por detrás del modelo.
>
> **4. Descuento de stock.** Ninguna ilación descontaba `stock_actual` al vender ni lo restituía al anular o devolver. El sistema podía vender indefinidamente sin afectar el inventario. Corregido en ILA-0001, ILA-0003, ILA-0004 y ILA-0025.
>
> **5. Bajas físicas que rompen la integridad referencial.** Eliminar un producto, un método de pago, una venta o un comprobante de forma física rompe las claves foráneas del histórico y, en el caso de los comprobantes, es inadmisible tributariamente. Se convirtieron en bajas lógicas usando los campos de estado que el propio diccionario ya define, salvo en los casos donde la eliminación física es legítima.
>
> **6. Precondiciones incoherentes.** Varias operaciones de creación exigían que la tabla destino ya tuviera entradas válidas, lo que hace imposible el primer registro (ILA-0009 es el caso más claro). Se reemplazaron por las precondiciones que la operación realmente necesita.
>
> **7. Consistencia de artefactos.** ILA-0004 cargaba `ART-MKP-VEN-0003` en el procedimiento mientras declaraba `ART-MKP-VEN-0004` en artefactos asociados; ILA-0025 cargaba un mockup del módulo de ventas y declaraba sus artefactos como "Pendiente"; ILA-0028 declaraba `ART-MKP-DEV-0003` para una eliminación que carga `ART-MKP-DEV-0004`; `ART-MKP-DOC--0001` tenía doble guion.
>
> **8. Normalización formal.** Se unificó el marcado (12 de las 32 tablas venían con la primera columna en negrita), se corrigieron los estados `conluido` y `Concluida`, la fecha inválida `2106//26`, la numeración perdida de los pasos en ILA-0003 y ILA-0019, el nombre de componente `DOC-TLB-COMPROBANTES`, y se unificaron los identificadores de interfaz del módulo 8, que usaban el prefijo `ALR_` mientras las especificaciones ya usaban `RES-`.
>
> **Ampliación del 05/09/2026.** Se incorporaron los módulos 9 y 10 con ocho ilaciones nuevas (ILA-0033 a ILA-0040), derivadas de las educciones EDU-0013 (Gestión de usuarios) y EDU-0014 (Gestión de proveedores). Ambas tablas ya existían en el Diccionario de Datos y eran referenciadas por otros módulos, pero ninguna ilación las alimentaba: ILA-0025 exigía proveedores válidos como precondición sin que existiera procedimiento alguno para crearlos.
> El inicio y cierre de sesión no se descomponen en ilaciones por estar cubiertos como requisito de seguridad en RNF-0006.
> Con EDU-0010 ampliada, ILA-0023 e ILA-0024 dejan de contradecir a su educción padre y pasan a estado Concluido.
>
> **Actualización del 05/09/2026 (nomenclatura y modelo).** Se aplicaron los cambios de la Guía de Estilo de Nomenclatura v03.00: prefijo **USR** para usuarios y **PRV** para proveedores, tipo **FEC** para todos los campos de fecha, y la unificación de los nombres de las cuatro fases del CRUD (CREAR, LEER, ACTUALIZAR, ELIMINAR más CONFIRMAR-SI / CONFIRMAR-NO). Se corrigió además **DEV-CMB-ESTADO-PRODUCTO**, que capturaba el motivo de la devolución y no el estado del producto.
> Los cuatro cambios de base de datos del Anexo A quedaron aplicados en el Diccionario de Datos v03.00.
>
> **Revisión de tipos de dato del 05/09/2026.** Se verificó cada componente contra el tipo del campo que captura en el Diccionario de Datos v03.00 y contra los controles disponibles en WPF según RNF-0008. Correcciones aplicadas: los importes y cantidades pasan al prefijo **NUM**; `es_lote_defectuoso`, que es un valor lógico, pasa de ComboBox a **CHK**; `alerta_digemid`, que es un código de texto libre, pasa de ComboBox a **TXT**; el producto de una alerta pasa de texto libre a **CMB**, porque la ilación necesita el identificador del producto para bloquearlo; los valores autogenerados o calculados pasan a **LBL**; la contraseña pasa a **PWD**, que corresponde al PasswordBox de WPF. Se detectó además que ILA-0025 nunca capturaba `cantidad_devolver` ni `comentario`, ambos campos declarados en el diccionario, uno de ellos NOT NULL.
>
> **Ampliación del 05/09/2026 (lotes).** Tras la decisión del equipo de incorporar `TBL_LOTES` al modelo, se agregó el módulo 11 con ILA-0041 a ILA-0044 y se ajustaron las dieciséis ilaciones de los módulos 1, 2, 4 y 7. Los cambios de fondo: la venta descuenta de una remesa concreta según el criterio FEFO y registra su `id_lote` en el detalle, lo que hace posible el rastreo sanitario; el bloqueo por vencimiento pasa del medicamento a la remesa, de modo que una remesa vencida ya no impide vender las sanas del mismo estante; la devolución identifica la remesa que el proveedor exige; y el módulo de inventario queda reducido a los datos de catálogo del medicamento.
>
> **Criterio de estado:** se mantiene `Concluido` cuando la corrección fue formal o de coherencia interna. Se marca `Pendiente` cuando el cambio exige una decisión externa a la ilación: un componente de mockup nuevo, un ajuste al diccionario de datos o una ampliación de la educción. El Anexo al final del documento lista esos pendientes.

## Convenciones aplicadas

- Componentes de interfaz: guion y prefijo de módulo (`VEN-BTN-CREAR-VENTA`). Prefijos vigentes: `VEN`, `INV`, `DOC`, `ALV`, `PAG`, `REP`, `DEV`, `RES`.
- Tablas y campos de base de datos: guion bajo, siempre con el punto entre base de datos y tabla (`DB_FARMASIL.TBL_PRODUCTOS`).
- Versión `DD.DD`, fecha `dd/mm/aaaa`, ningún campo vacío (`Ninguno` cuando no aplica).
- Cada educción se descompone en cuatro ilaciones que representan las fases del CRUD: Crear, Leer, Actualizar y Eliminar.

---
