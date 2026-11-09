> Documento histórico (v01). Lo vigente está en el diccionario de datos v10.00 y en DEC-028 a DEC-041 de seguimiento/11-registro-pendientes.md.

# Vista Lógica — FARMASIL_APP (v01.00)

> Primera versión de la vista lógica. Fuentes: arquitectura genérica cerrada (13 componentes, 18 dependencias; `Contexto_Arquitectura_FARMASIL_ParaAgentesIA.md`), Diccionario de Datos v09.00, ESP-0001 a ESP-0052, RNF-0006, RNF-0008 y RNF-0010. Las fuentes de los diagramas están en `docs/diagramas/plaintext-VistaLogica/` y sus imágenes en `docs/diagramas/`.

> **Imágenes solo para exposición.** `docs/diagramas/VL_01_Trazabilidad.png` (paquetes con las especificaciones de cada clase) y `docs/diagramas/Vista Logica.png` (modelo de dominio) se elaboraron a mano para exponer el artefacto y no tienen fuente PlantUML. No son fuente de diseño: para implementar se usan únicamente los `.puml` de `docs/diagramas/plaintext-VistaLogica/` y sus PNG generados (VL-01 a VL-04). Si una de esas imágenes difiere de los `.puml`, prevalecen los `.puml` (DEC-024).

## 1. Artefactos

| Código | Diagrama | Qué responde | Sección de la plantilla |
| --- | --- | --- | --- |
| VL-01 | Diagrama de paquetes | Qué paquete lógico corresponde a cada componente de la arquitectura genérica y qué clases contiene | Mecanismos comunes: paquetes |
| VL-02 | Diagrama de clases del dominio | Qué clases realizan cada paquete, con sus atributos, operaciones, responsabilidades, asociaciones, multiplicidades y composiciones | Clases; relaciones; restricciones |
| VL-03 | Mecanismos comunes | Generalizaciones, realización de interfaces, enumeraciones (estereotipo «enumeration») y procesos programados | Generalización; estereotipos |
| VL-04 | Ejemplo Modelo-Vista-Controlador (módulo Ventas) | Cómo se reparten Vista, Controlador (VistaModelo) y Modelo en WPF, y cómo el modelo respeta las dependencias del paquete | Modelo Vista Controlador; dependencia |
| Tabla 2 (este documento) | Mapeo de clases a tablas | A qué tabla y columna corresponde cada clase y atributo, y con qué patrón | Mapeo de clases a bases de datos relacionales |

## 2. Metodología (texto para la sección 2 de la plantilla)

La vista lógica se construyó a partir de la arquitectura genérica, planteando sobre cada uno de sus componentes la pregunta de cómo interviene de manera lógica. El procedimiento siguió cinco pasos:

1. **De componentes a paquetes.** Cada uno de los trece componentes se convirtió en un paquete lógico con el mismo nombre, y las dieciocho dependencias entre componentes se conservaron como dependencias «use» entre paquetes. Usuarios se trató como paquete transversal de seguridad, igual que en la arquitectura genérica, y se agregó un paquete común para los mecanismos compartidos.
2. **De tablas a clases.** Cada tabla del Diccionario de Datos dio lugar a una clase persistente del paquete que la escribe. Los atributos toman el tipo de la tabla de correspondencia del diccionario (C# / .NET 8), y los dominios cerrados se convirtieron en enumeraciones.
3. **De especificaciones a operaciones y restricciones.** Las reglas de negocio de las especificaciones que dependen solo del estado de una entidad se asignaron a la propia clase como operaciones (por ejemplo, `Lote.descontar`, `ComprobanteTributario.anular`, `OrdenDevolucion.completar`). Las reglas que no se pueden expresar con la notación gráfica se escribieron como restricciones.
4. **De claves foráneas a asociaciones.** Cada clave foránea se convirtió en una asociación con multiplicidad. La navegabilidad sigue el sentido de la dependencia entre paquetes, de modo que ninguna clase conoce a otra de un paquete que no esté entre sus dependencias. Las relaciones cabecera-detalle se modelaron como composición.
5. **Validación.** Cada asociación se contrastó con el grafo de dependencias de la arquitectura genérica, y cada atributo con su columna del diccionario (Tabla 2).

## 3. Descripción del artefacto (texto base para la sección 7)

### 3.1 Paquetes (VL-01)

Los trece paquetes de negocio se agrupan en seis bloques, ordenados según cuántos paquetes dependen de cada uno: catálogo base (Métodos de Pago, Proveedores), inventario físico (Inventario, Lotes), operación diaria (Ventas, Restricciones de Venta), derivados de la operación diaria (Documentación Tributaria, Reportes de Ventas), gestión de proveedores (Devoluciones, Alertas de Vencimiento) y abastecimiento (Reabastecimiento, Comparación de Precios). El grafo de dependencias es acíclico. Los paquetes Seguridad y Común son importados por todos los paquetes de negocio, y esas dependencias no se dibujan porque la autoría y el control de acceso (RNF-0006) son aspectos transversales.

### 3.2 Clases del dominio (VL-02)

El modelo tiene dieciséis clases persistentes, una por tabla. Tres decisiones de la arquitectura genérica se reflejan directamente en las clases:

- `DetalleVenta` se asocia con `Lote`, no con `Producto`. El medicamento vendido se obtiene navegando `Lote.medicamento`.
- `Lote` es una clase propia, con su proveedor real (`entregadoPor`), distinto del proveedor habitual del producto (`proveedorHabitual`).
- `Producto` no conoce sus remesas. Su stock consolidado lo calculan quienes dependen de Lotes (Reabastecimiento).

Las composiciones son dos: `Venta`–`DetalleVenta` y `OrdenDevolucion`–`DetalleDevolucion`. En la segunda, la multiplicidad del detalle es 0..* porque una orden eliminada conserva su cabecera sin líneas (ESP-0028), con la restricción de que toda orden no eliminada tiene al menos una línea. Entre `Venta` y `ComprobanteTributario` la multiplicidad es 1 a 0..*, con la restricción de que a lo sumo un comprobante esté emitido (ESP-0009): una venta puede acumular comprobantes anulados.

`ReporteVentas` y `ReglaAlertaVencimiento` no participan en asociaciones: el primero es un índice de archivos generados y la segunda es la configuración del umbral de vencimiento.

### 3.3 Mecanismos comunes (VL-03)

- **Generalización.** `RegistroDesactivable` reúne el estado Activo/Inactivo y la operación `desactivar()` que comparten `Usuario`, `MetodoPago`, `Proveedor` y `RestriccionVenta` (ESP-0020, ESP-0032, ESP-0036, ESP-0040). `ProcesoProgramado` generaliza los dos procesos de RNF-0010: `EvaluacionVencimientos` (al iniciar y cada 24 horas) y `GeneracionListaReabastecimiento` (cada 15 días).
- **Realización.** `Venta`, `OrdenDevolucion` y `CotizacionProveedor` realizan `IAuditable`, que registra al usuario autor. La autoría queda así en una sola relación con `Usuario`, en lugar de tres asociaciones de negocio.
- **Estereotipos.** «entity» para clases persistentes, «service» para procesos sin estado persistente, «enumeration» para los doce dominios cerrados del diccionario y, en el diagrama de dominio, «desactivable» y «auditable» como referencia a VL-03. `tipoReporte` queda como texto porque su dominio está pendiente con la dueña (PEN-001).
- **Restricciones.** Se escriben entre llaves junto a la clase. Por ejemplo, en `Lote`: `{stockActual >= 0}`, `{unique (medicamento, numeroLote)}` y `{desbloquear solo si no estaVencido(hoy)}`.

### 3.4 Modelo-Vista-Controlador (VL-04)

El ejemplo se toma del registro de venta (ESP-0001). Como la interfaz se implementa en WPF (RNF-0008), el patrón se realiza con su variante Modelo-Vista-VistaModelo: la VistaModelo cumple el papel del controlador.

- **Vista.** `VentaView` contiene los controles de ESP-0001 y solo se enlaza a la VistaModelo.
- **Controlador (VistaModelo).** `VentaViewModel` expone colecciones observables y comandos (`ICommand`), y delega toda regla de negocio en el modelo.
- **Modelo.** `VentaServicio` usa únicamente las interfaces de los paquetes de los que depende Ventas (`ILotes`, `IRestricciones`, `IPagos`) y la sesión para la autoría. Persiste la venta mediante un repositorio y una unidad de trabajo, que garantizan una sola transacción para la cabecera, las líneas y el descuento de stock.
- **Precio.** El precio y el nombre del medicamento llegan a Ventas dentro de la remesa sugerida por `ILotes`, de modo que no aparece una dependencia directa de Ventas hacia Inventario.

## 4. Mapeo de clases a tablas (Tabla 2)

Patrones según Fowler, *Patterns of Enterprise Application Architecture*: *Identity Field* (IF), *Foreign Key Mapping* (FKM), *Dependent Mapping* (DM) y *Concrete Table Inheritance* (CTI).

| Clase | Tabla | Atributos con nombre o tratamiento distinto | Patrón |
| --- | --- | --- | --- |
| Usuario | TBL_USUARIOS | `contrasenaHash` → `contrasena`; `rol` → CHECK ('Administrador','Tecnico'); `estado` heredado | IF, CTI |
| MetodoPago | TBL_METODOS_PAGO | `estado` heredado | IF, CTI |
| Proveedor | TBL_PROVEEDORES | `estado` heredado | IF, CTI |
| Producto | TBL_PRODUCTOS | `proveedorHabitual` → `id_proveedor` | IF, FKM |
| Lote | TBL_LOTES | `medicamento` → `id_producto`; `entregadoPor` → `id_proveedor`; UNIQUE (`id_producto`, `numero_lote`) | IF, FKM |
| Venta | TBL_REGISTRO_VENTAS | `pagadaCon` → `id_metodo_pago`; `registradoPor` → `id_usuario` | IF, FKM |
| DetalleVenta | TBL_DETALLE_VENTAS | cabecera → `id_venta`; `remesaVendida` → `id_lote` | IF, DM, FKM |
| RestriccionVenta | TBL_RESTRICCIONES_VENTA | `estado` heredado → `estado_alerta`; `aplicaA` → `id_producto` | IF, CTI, FKM |
| ComprobanteTributario | TBL_COMPROBANTES_TRIBUTARIOS | `documenta` → `id_venta` | IF, FKM |
| ReporteVentas | TBL_REPORTES_VENTAS | — (sin claves foráneas) | IF |
| OrdenDevolucion | TBL_ORDENES_DEVOLUCION | `remitidaA` → `id_proveedor`; `registradoPor` → `id_usuario` | IF, FKM |
| DetalleDevolucion | TBL_DETALLE_DEVOLUCION | cabecera → `id_devolucion`; `remesaDevuelta` → `id_lote` | IF, DM, FKM |
| ReglaAlertaVencimiento | TBL_ALERTAS_VENCIMIENTO | `fechaUltimaVerif` → `fecha_ultima_verif` | IF |
| ProductoRetirado | TBL_PRODUCTOS_RETIRADOS | `remesaRetirada` → `id_lote` (anulable, ON DELETE SET NULL) | IF, FKM |
| ItemReabastecimiento | TBL_LISTA_REABASTECIMIENTO | `aReabastecer` → `id_producto` | IF, FKM |
| CotizacionProveedor | TBL_COTIZACIONES_PROVEEDOR | `cotiza` → `id_producto`; `ofrecidaPor` → `id_proveedor`; `registradoPor` → `id_usuario` | IF, FKM |

**Elementos sin tabla:**

- **`RegistroDesactivable` y `IAuditable`.** Sus atributos se repiten en la tabla de cada subclase (CTI): `estado` e `id_usuario`.
- **Enumeraciones.** Se guardan como `NVARCHAR` con restricción CHECK. En el código, los valores con espacios o tilde se traducen (por ejemplo, `BloqueadoPorDevolucion` → 'Bloqueado por devolucion', `Automatico` → 'Automático').
- **Procesos programados.** No tienen tabla. La última ejecución de `GeneracionListaReabastecimiento` se guarda en la configuración local (RNF-0010).
- **Atributos calculados.** Ninguno se persiste: `Venta.calcularMontoTotal()` se guarda en `monto_total` al crear la venta (ESP-0001), y el stock consolidado no existe como columna.

## 5. Observaciones (insumo para la sección 9)

1. **Generalizaciones del dominio.** El dominio no tiene jerarquías naturales de herencia entre entidades de negocio. Las dos generalizaciones del modelo son mecanismos compartidos (baja lógica y procesos programados), y no se introdujeron jerarquías artificiales para ilustrar la relación.
2. **Comprobantes por venta.** VL-02 modela varios comprobantes por venta, con uno solo emitido, como admite ESP-0009. El Diccionario de Datos v08.00 corrigió la relación, que declaraba 1:1 identificadora (PEN-014, cerrado).
3. **SEC-01 frente a la vista lógica.** SEC-01 consultaba el precio a Inventario desde Ventas. Corregido: el nombre y el precio llegan con la remesa sugerida por Lotes, y el descuento de stock lo hace Lotes, como en VL-04 (PEN-013, cerrado).
4. **Fecha de última verificación.** `fecha_ultima_verif` la actualiza el proceso de evaluación de vencimientos al terminar cada ejecución (RNF-0010 v02.00; PEN-015, cerrado).
5. **Configuración de alertas.** `ReglaAlertaVencimiento` no tiene operaciones de administración, porque ninguna educción la gestiona (PEN-011).
6. **Restricciones incorporadas en la versión 09.00 del diccionario.** VL-02 declara que una orden solo se completa si cada remesa conserva stock suficiente (DEC-019) y que un medicamento figura a lo sumo una vez en la lista de reabastecimiento (DEC-020). El combo de remesas de ESP-0025 no usa el umbral de vencimiento, para no crear una dependencia Devoluciones → Alertas de Vencimiento (DEC-023).
