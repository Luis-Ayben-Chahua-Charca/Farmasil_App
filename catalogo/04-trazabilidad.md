# Matriz de Trazabilidad — FARMASIL

**Versión:** 02.00
**Fecha:** 05/09/2026
**Autor:** AUT-0001

Sigue la estructura de la plantilla oficial de trazabilidad de la asignatura (Educción, Ilación, Especificación, Otros artefactos), ampliada con las columnas que el catálogo necesita para verificarse: rol autorizado, estado y cobertura de pruebas.

Fuentes de esta matriz: `01-educciones.md`, `02-ilaciones.md`, `03-especificaciones.md`, `05-diccionario-datos.md` v04.00, `06-guia-nomenclatura-mockups.md` v04.00, `07-matriz-permisos.md` v02.00, `08-requisitos-no-funcionales.md` y `09-pruebas-software.md`.

---

## 1. Matriz principal

| Educción | Ilación | Especificación | Otros artefactos |
| --- | --- | --- | --- |
| EDU-0001 | ILA-0001 | ESP-0001 | ART-MKP-VEN-0001 |
|  | ILA-0002 | ESP-0002 | ART-MKP-VEN-0001, ART-MKP-VEN-0002 |
|  | ILA-0003 | ESP-0003 | ART-MKP-VEN-0001, ART-MKP-VEN-0003 |
|  | ILA-0004 | ESP-0004 | ART-MKP-VEN-0001, ART-MKP-VEN-0004 |
| EDU-0002 | ILA-0005 | ESP-0005 | ART-MKP-INV-0001 |
|  | ILA-0006 | ESP-0006 | ART-MKP-INV-0002 |
|  | ILA-0007 | ESP-0007 | ART-MKP-INV-0001, ART-MKP-INV-0003 |
|  | ILA-0008 | ESP-0008 | ART-MKP-INV-0004 |
| EDU-0003 | ILA-0009 | ESP-0009 | ART-MKP-DOC-0001 |
|  | ILA-0010 | ESP-0010 | ART-MKP-DOC-0001, ART-MKP-DOC-0002 |
|  | ILA-0011 | ESP-0011 | ART-MKP-DOC-0001, ART-MKP-DOC-0003 |
|  | ILA-0012 | ESP-0012 | ART-MKP-DOC-0001, ART-MKP-DOC-0004 |
| EDU-0004 | ILA-0013 | ESP-0013 | ART-MKP-ALV-0001 |
|  | ILA-0014 | ESP-0014 | ART-MKP-ALV-0001 |
|  | ILA-0015 | ESP-0015 | ART-MKP-ALV-0001, ART-MKP-ALV-0002 |
|  | ILA-0016 | ESP-0016 | ART-MKP-ALV-0001, ART-MKP-ALV-0003 |
| EDU-0009 | ILA-0017 | ESP-0017 | ART-MKP-PAG-0001 |
|  | ILA-0018 | ESP-0018 | ART-MKP-PAG-0001, ART-MKP-PAG-0002 |
|  | ILA-0019 | ESP-0019 | ART-MKP-PAG-0001, ART-MKP-PAG-0003 |
|  | ILA-0020 | ESP-0020 | ART-MKP-PAG-0001, ART-MKP-PAG-0004 |
| EDU-0010 | ILA-0021 | ESP-0021 | ART-MKP-REP-0001 |
|  | ILA-0022 | ESP-0022 | ART-MKP-REP-0002 |
|  | ILA-0023 | ESP-0023 | ART-MKP-REP-0002, ART-MKP-REP-0003 |
|  | ILA-0024 | ESP-0024 | ART-MKP-REP-0002 |
| EDU-0011 | ILA-0025 | ESP-0025 | ART-MKP-DEV-0001 |
|  | ILA-0026 | ESP-0026 | ART-MKP-DEV-0001, ART-MKP-DEV-0002 |
|  | ILA-0027 | ESP-0027 | ART-MKP-DEV-0001, ART-MKP-DEV-0003 |
|  | ILA-0028 | ESP-0028 | ART-MKP-DEV-0001, ART-MKP-DEV-0004 |
| EDU-0012 | ILA-0029 | ESP-0029 | ART-MKP-RES-0001 |
|  | ILA-0030 | ESP-0030 | ART-MKP-RES-0002 |
|  | ILA-0031 | ESP-0031 | ART-MKP-RES-0003 |
|  | ILA-0032 | ESP-0032 | ART-MKP-RES-0004 |
| EDU-0013 | ILA-0033 | ESP-0033 | ART-MKP-USR-0001 |
|  | ILA-0034 | ESP-0034 | ART-MKP-USR-0002 |
|  | ILA-0035 | ESP-0035 | ART-MKP-USR-0003 |
|  | ILA-0036 | ESP-0036 | ART-MKP-USR-0004 |
| EDU-0014 | ILA-0037 | ESP-0037 | ART-MKP-PRV-0001 |
|  | ILA-0038 | ESP-0038 | ART-MKP-PRV-0002 |
|  | ILA-0039 | ESP-0039 | ART-MKP-PRV-0003 |
|  | ILA-0040 | ESP-0040 | ART-MKP-PRV-0004 |
| EDU-0015 | ILA-0041 | ESP-0041 | ART-MKP-LOT-0001 |
|  | ILA-0042 | ESP-0042 | ART-MKP-LOT-0002 |
|  | ILA-0043 | ESP-0043 | ART-MKP-LOT-0003 |
|  | ILA-0044 | ESP-0044 | ART-MKP-LOT-0004 |

**Códigos reservados.** EDU-0005 a EDU-0008 no aparecen en esta matriz: fueron retirados de la etapa de educción por haberse reclasificado como requisitos no funcionales, y sus códigos se mantienen sin reasignar. El salto es intencional.

---

## 2. Matriz extendida

| Módulo | Educción | Ilación | Fase CRUD | Especificación | Rol autorizado | Estado | Tipos de prueba |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | EDU-0001 | ILA-0001 | Crear | ESP-0001 | A y T | Pendiente | T-001, T-002, T-004, T-005, T-008, T-012 |
| 1 | EDU-0001 | ILA-0002 | Leer | ESP-0002 | A y T | Pendiente | T-001, T-002, T-008 |
| 1 | EDU-0001 | ILA-0003 | Actualizar | ESP-0003 | A | Pendiente | T-001, T-002, T-005, T-015 |
| 1 | EDU-0001 | ILA-0004 | Eliminar | ESP-0004 | A | Pendiente | T-001, T-002, T-007 |
| 2 | EDU-0002 | ILA-0005 | Crear | ESP-0005 | A | Pendiente | T-001, T-002, T-004, T-005, T-015 |
| 2 | EDU-0002 | ILA-0006 | Leer | ESP-0006 | A y T | Concluido | T-001, T-002, T-008 |
| 2 | EDU-0002 | ILA-0007 | Actualizar | ESP-0007 | A | Pendiente | T-001, T-002, T-005, T-015 |
| 2 | EDU-0002 | ILA-0008 | Eliminar | ESP-0008 | A | Pendiente | T-001, T-002, T-007 |
| 3 | EDU-0003 | ILA-0009 | Crear | ESP-0009 | A y T | Pendiente | T-001, T-002, T-004, T-005, T-012 |
| 3 | EDU-0003 | ILA-0010 | Leer | ESP-0010 | A y T | Concluido | T-001, T-002, T-008 |
| 3 | EDU-0003 | ILA-0011 | Actualizar | ESP-0011 | A | Concluido | T-001, T-002, T-005 |
| 3 | EDU-0003 | ILA-0012 | Eliminar | ESP-0012 | A | Concluido | T-001, T-002, T-013 |
| 4 | EDU-0004 | ILA-0013 | Leer | ESP-0013 | A y T | Concluido | T-002, T-008, T-013 |
| 4 | EDU-0004 | ILA-0014 | Crear | ESP-0014 | A y T | Concluido | T-001, T-002, T-004, T-005 |
| 4 | EDU-0004 | ILA-0015 | Actualizar | ESP-0015 | A | Concluido | T-001, T-002, T-005 |
| 4 | EDU-0004 | ILA-0016 | Eliminar | ESP-0016 | A | Concluido | T-001, T-002, T-007 |
| 5 | EDU-0009 | ILA-0017 | Crear | ESP-0017 | A | Concluido | T-001, T-002, T-005 |
| 5 | EDU-0009 | ILA-0018 | Leer | ESP-0018 | A y T | Concluido | T-002, T-008 |
| 5 | EDU-0009 | ILA-0019 | Actualizar | ESP-0019 | A | Concluido | T-001, T-002, T-005 |
| 5 | EDU-0009 | ILA-0020 | Eliminar | ESP-0020 | A | Concluido | T-001, T-002, T-007 |
| 6 | EDU-0010 | ILA-0021 | Crear | ESP-0021 | A | Pendiente | T-001, T-002, T-005, T-008, T-013 |
| 6 | EDU-0010 | ILA-0022 | Leer | ESP-0022 | A | Concluido | T-002, T-008, T-015 |
| 6 | EDU-0010 | ILA-0023 | Actualizar | ESP-0023 | A | Concluido | T-001, T-002, T-007 |
| 6 | EDU-0010 | ILA-0024 | Eliminar | ESP-0024 | A | Concluido | T-001, T-002, T-007 |
| 7 | EDU-0011 | ILA-0025 | Crear | ESP-0025 | A | Pendiente | T-001, T-002, T-005, T-006 |
| 7 | EDU-0011 | ILA-0026 | Leer | ESP-0026 | A | Concluido | T-002, T-008 |
| 7 | EDU-0011 | ILA-0027 | Actualizar | ESP-0027 | A | Pendiente | T-002, T-005, T-006 |
| 7 | EDU-0011 | ILA-0028 | Eliminar | ESP-0028 | A | Concluido | T-001, T-002, T-007 |
| 8 | EDU-0012 | ILA-0029 | Crear | ESP-0029 | A | Concluido | T-001, T-002, T-005, T-012 |
| 8 | EDU-0012 | ILA-0030 | Leer | ESP-0030 | A y T | Concluido | T-002, T-008 |
| 8 | EDU-0012 | ILA-0031 | Actualizar | ESP-0031 | A | Concluido | T-001, T-002, T-005 |
| 8 | EDU-0012 | ILA-0032 | Eliminar | ESP-0032 | A | Concluido | T-001, T-002, T-007, T-020 |
| 9 | EDU-0013 | ILA-0033 | Crear | ESP-0033 | A | Pendiente | **sin pruebas** |
| 9 | EDU-0013 | ILA-0034 | Leer | ESP-0034 | A | Pendiente | **sin pruebas** |
| 9 | EDU-0013 | ILA-0035 | Actualizar | ESP-0035 | A | Pendiente | **sin pruebas** |
| 9 | EDU-0013 | ILA-0036 | Eliminar | ESP-0036 | A | Pendiente | **sin pruebas** |
| 10 | EDU-0014 | ILA-0037 | Crear | ESP-0037 | A | Pendiente | **sin pruebas** |
| 10 | EDU-0014 | ILA-0038 | Leer | ESP-0038 | A | Pendiente | **sin pruebas** |
| 10 | EDU-0014 | ILA-0039 | Actualizar | ESP-0039 | A | Pendiente | **sin pruebas** |
| 10 | EDU-0014 | ILA-0040 | Eliminar | ESP-0040 | A | Pendiente | **sin pruebas** |
| 11 | EDU-0015 | ILA-0041 | Crear | ESP-0041 | A | Pendiente | **sin pruebas** |
| 11 | EDU-0015 | ILA-0042 | Leer | ESP-0042 | A y T | Pendiente | **sin pruebas** |
| 11 | EDU-0015 | ILA-0043 | Actualizar | ESP-0043 | A | Pendiente | **sin pruebas** |
| 11 | EDU-0015 | ILA-0044 | Eliminar | ESP-0044 | A | Pendiente | **sin pruebas** |

`A` = Administrador (ACT-0001) · `T` = Técnico (ACT-0002). Los tipos de prueba corresponden al catálogo del capítulo 10 del Catálogo de Requisitos, según `09-pruebas-software.md`.

---

## 3. Trazabilidad hacia los requisitos no funcionales

| Requisito no funcional | Atributo de calidad | Artefactos funcionales relacionados | Relación |
| --- | --- | --- | --- |
| RNF-0001 Funcionamiento sin conexión permanente | Disponibilidad | Todos los módulos | El catálogo completo opera sobre una base local `DB_FARMASIL`; ninguna especificación requiere un servicio remoto. |
| RNF-0002 Facilidad de uso sin capacitación | Usabilidad | Los mockups de los once módulos | Verificado por las pruebas T-016 y T-005. |
| RNF-0003 Soporte de usuarios simultáneos | Concurrencia | ESP-0001, ESP-0003, ESP-0004, ESP-0041, ESP-0043 | Operaciones transaccionales que pueden ejecutarse en paralelo sobre el mismo lote o la misma venta. Cubierto por T-008 y T-012. |
| RNF-0004 Tiempo de recuperación del servicio | Fiabilidad | `DB_FARMASIL` completa | Sin ilación asociada: es un procedimiento de operación, no una función de usuario. |
| RNF-0005 Velocidad en el registro de ventas | Rendimiento | ESP-0001 | Fija el límite de 4 segundos para la transacción completa, incluido el descuento de stock de la remesa. Cubierto por T-017 y T-008. |
| RNF-0006 Control de acceso, autorización y protección de credenciales | Seguridad | EDU-0013, ILA-0033 a ILA-0036, ESP-0033 a ESP-0036, y las 44 precondiciones | El módulo 9 hace implementable a este requisito. Sus seis condiciones se verifican en: autenticación e identificación en ESP-0001 y ESP-0025 mediante `id_usuario`; autorización en las 44 precondiciones según la Matriz de Permisos; cuenta inactiva en ESP-0036; credenciales en ESP-0033 y ESP-0034; continuidad de la administración en ESP-0035. |
| RNF-0007 Funcionamiento en el equipo objetivo | Compatibilidad | Todos los módulos | Verificado por las pruebas T-015 y T-011. |
| RNF-0008 Stack tecnológico | Restricción de implementación | Todos los módulos | Determina la correspondencia de tipos de la sección de tipos del Diccionario de Datos v04.00. |
| RNF-0009 Integridad y respaldo de DB_FARMASIL | Integridad | Las 14 tablas de `DB_FARMASIL` | Las 44 especificaciones operan sobre el esquema declarado en este requisito. Las transacciones declaradas en cada especificación son el mecanismo por el que se cumple la parte de integridad. |

**Educciones reclasificadas.** Los requisitos que ocupaban EDU-0005 a EDU-0008 se trasladaron al catálogo de requisitos no funcionales. Su trazabilidad se conserva por la nota de códigos reservados de `01-educciones.md` y por esta matriz.

---

## 4. Trazabilidad hacia el modelo de datos

| Tabla de `DB_FARMASIL` | Especificaciones que la escriben | Especificaciones que la leen |
| --- | --- | --- |
| TBL_USUARIOS | ESP-0033, ESP-0035, ESP-0036 | ESP-0034, ESP-0021 |
| TBL_PROVEEDORES | ESP-0037, ESP-0039, ESP-0040 | ESP-0038, ESP-0005, ESP-0007, ESP-0025, ESP-0026 |
| TBL_PRODUCTOS | ESP-0005, ESP-0007, ESP-0008 | ESP-0006, ESP-0001, ESP-0026, ESP-0029, ESP-0030, ESP-0041, ESP-0042 |
| TBL_LOTES | ESP-0041, ESP-0043, ESP-0044, ESP-0001, ESP-0003, ESP-0004, ESP-0014, ESP-0016, ESP-0027 | ESP-0042, ESP-0006, ESP-0013, ESP-0002, ESP-0025, ESP-0026 |
| TBL_REGISTRO_VENTAS | ESP-0001, ESP-0003, ESP-0004 | ESP-0002, ESP-0009, ESP-0010, ESP-0020, ESP-0021, ESP-0023, ESP-0036 |
| TBL_DETALLE_VENTAS | ESP-0001, ESP-0003, ESP-0004 | ESP-0002, ESP-0021, ESP-0023, ESP-0044 |
| TBL_COMPROBANTES_TRIBUTARIOS | ESP-0009, ESP-0011, ESP-0012 | ESP-0010, ESP-0003, ESP-0004 |
| TBL_METODOS_PAGO | ESP-0017, ESP-0019, ESP-0020 | ESP-0018, ESP-0001 |
| TBL_ALERTAS_VENCIMIENTO | **ninguna** | ESP-0013, ESP-0025 |
| TBL_PRODUCTOS_RETIRADOS | ESP-0014, ESP-0015, ESP-0016 | ESP-0013 |
| TBL_ORDENES_DEVOLUCION | ESP-0025, ESP-0027, ESP-0028 | ESP-0026, ESP-0016, ESP-0036, ESP-0040 |
| TBL_DETALLE_DEVOLUCION | ESP-0025, ESP-0027, ESP-0028 | ESP-0026, ESP-0016 |
| TBL_RESTRICCIONES_VENTA | ESP-0029, ESP-0031, ESP-0032 | ESP-0030, ESP-0001, ESP-0008 |
| TBL_REPORTES_VENTAS | ESP-0021, ESP-0023, ESP-0024 | ESP-0022 |

**Hallazgo.** `TBL_ALERTAS_VENCIMIENTO` es la única tabla del modelo que ninguna especificación escribe. Su contenido, incluido el umbral de meses, solo se lee. Nadie puede configurarla desde el sistema.

---

## 5. Cobertura y huecos

| Verificación | Resultado |
| --- | --- |
| Educciones con sus cuatro ilaciones | 11 de 11 |
| Ilaciones con especificación | 44 de 44 |
| Especificaciones con ilación de origen | 44 de 44 |
| Correspondencia uno a uno EDU→ILA→ESP | Sin huecos ni duplicados |
| Ilaciones con rol autorizado declarado | 44 de 44 |
| Estado coincidente entre ilación y especificación | 44 de 44 |
| Especificaciones con pruebas definidas | 44 de 44 |
| Mockups referenciados que existen | **28 de 40** |

### Huecos abiertos

1. ~~**Faltan pruebas y las existentes están desactualizadas.**~~ **Resuelto el 05/09/2026.** `09-pruebas-software.md` v03.00 cubre las 44 especificaciones con 223 pruebas, más las seis condiciones de RNF-0006 y los nueve requisitos no funcionales.
2. **Faltan los datos de prueba.** El documento define qué se prueba, no con qué juego de datos de carga inicial.
3. **Doce mockups sin diseñar**: ART-MKP-USR-0001 a 0004, ART-MKP-PRV-0001 a 0004 y ART-MKP-LOT-0001 a 0004.
4. **`TBL_ALERTAS_VENCIMIENTO` sin CRUD**, según la sección 4.
5. **Catálogo de fuentes inexistente.** FUE-0001 a FUE-0005 se citan en 26 artefactos sin que ningún documento los defina. Siete ilaciones citan la Entrevista 1 y cuatro la consulta posterior al cliente, ambas sin código.
6. ~~**RNF-0006 pendiente de actualizar.**~~ **Resuelto el 05/09/2026.** Refundido en la versión 03.00, que cubre autenticación, identificación en la trazabilidad, autorización por rol, comportamiento ante intento no autorizado, protección de credenciales y continuidad de la administración. Ver `08-requisitos-no-funcionales.md`.
7. **Falta el requisito de la generación automática de alertas de vencimiento**, al que ILA-0013 remite sin que exista.
