# Matriz de Actores, Roles y Permisos — FARMASIL

**Versión:** 02.00
**Fecha:** 05/09/2026
**Autor:** AUT-0001

> **Por qué existe este documento.** Los códigos ACT-0001 y ACT-0002 se usaban en las tres etapas del catálogo sin que ningún artefacto los definiera, y ninguna ilación declaraba qué rol puede ejecutar qué operación. Cada una resolvía el control de acceso por su cuenta, de tres maneras distintas: unas exigían "sesión activa con rol de administrador", otras "permisos para modificar información" sin decir de quién, y veinte se conformaban con "sesión activa", lo que en la práctica autoriza a cualquier usuario del sistema a cualquier operación.
>
> Este documento define el catálogo de actores, su correspondencia con el rol almacenado en la base de datos, y la matriz de permisos por operación. Es la fuente única para el control de acceso del catálogo.

---

## 1. Catálogo de actores

| Código | Actor | Descripción | Rol en `TBL_USUARIOS.rol` |
| --- | --- | --- | --- |
| ACT-0001 | Dueña de la farmacia | Propietaria del negocio. Autoriza el ingreso de productos, define precios, gestiona proveedores, devoluciones, restricciones sanitarias y las cuentas del personal, y consulta los reportes. | `'Administrador'` |
| ACT-0002 | Técnica farmacéutica | Personal que atiende el mostrador en los horarios establecidos. Registra ventas, emite comprobantes y consulta inventario y restricciones. | `'Tecnico'` |

**Correspondencia con RNF-0006.** El requisito no funcional describe los mismos dos perfiles: administrador, con capacidad para modificar precios, eliminar productos y ver reportes de ventas; y operador, que solo registra ventas y consulta stock.

**Discrepancia terminológica.** **Resuelta el 05/09/2026.** RNF-0006 v03.00 adopta `'Administrador'` y `'Tecnico'`, los mismos valores que persiste el Diccionario de Datos, y delega en esta matriz los permisos de cada rol en lugar de enumerarlos en su descripción.

---

## 2. Regla de declaración del control de acceso

Toda ilación declara el rol requerido en su precondición, con una de estas dos fórmulas y ninguna otra:

- `El usuario tiene una sesión activa con rol 'Administrador'.`
- `El usuario tiene una sesión activa con rol 'Administrador' o 'Tecnico'.`

Toda especificación lo traduce a pseudocódigo inmediatamente después de validar la sesión:

- `VALIDAR ROL(SESION_USUARIO) = 'Administrador'`
- `VALIDAR ROL(SESION_USUARIO) EN ('Administrador', 'Tecnico')`

Quedan eliminadas las formulaciones genéricas del tipo "el usuario tiene permisos para modificar información", que no eran verificables porque no nombraban ningún rol.

**El actor de la educción es el actor principal del módulo; la matriz gobierna operación por operación.** Un módulo cuyo actor principal es la técnica puede tener operaciones reservadas al administrador, y a la inversa.

---

## 3. Matriz de permisos

`A` = Administrador · `T` = Técnico

| Módulo | Ilación | Operación | A | T |
| --- | --- | --- | :-: | :-: |
| 1 Ventas | ILA-0001 | Crear venta | Sí | Sí |
| 1 Ventas | ILA-0002 | Consultar ventas | Sí | Sí |
| 1 Ventas | ILA-0003 | Actualizar venta | Sí | No |
| 1 Ventas | ILA-0004 | Eliminar venta | Sí | No |
| 2 Inventario | ILA-0005 | Crear producto | Sí | No |
| 2 Inventario | ILA-0006 | Consultar inventario | Sí | Sí |
| 2 Inventario | ILA-0007 | Actualizar producto | Sí | No |
| 2 Inventario | ILA-0008 | Dar de baja producto | Sí | No |
| 3 Documentación | ILA-0009 | Registrar comprobante | Sí | Sí |
| 3 Documentación | ILA-0010 | Consultar comprobantes | Sí | Sí |
| 3 Documentación | ILA-0011 | Actualizar comprobante | Sí | No |
| 3 Documentación | ILA-0012 | Anular comprobante | Sí | No |
| 4 Alertas vencimiento | ILA-0013 | Consultar alertas | Sí | Sí |
| 4 Alertas vencimiento | ILA-0014 | Registrar alerta | Sí | Sí |
| 4 Alertas vencimiento | ILA-0015 | Modificar alerta | Sí | No |
| 4 Alertas vencimiento | ILA-0016 | Eliminar alerta | Sí | No |
| 5 Métodos de pago | ILA-0017 | Crear método | Sí | No |
| 5 Métodos de pago | ILA-0018 | Consultar métodos | Sí | Sí |
| 5 Métodos de pago | ILA-0019 | Actualizar método | Sí | No |
| 5 Métodos de pago | ILA-0020 | Desactivar método | Sí | No |
| 6 Reportes | ILA-0021 | Generar reporte | Sí | No |
| 6 Reportes | ILA-0022 | Consultar reporte | Sí | No |
| 6 Reportes | ILA-0023 | Actualizar reporte | Sí | No |
| 6 Reportes | ILA-0024 | Eliminar reporte | Sí | No |
| 7 Devoluciones | ILA-0025 | Crear orden | Sí | No |
| 7 Devoluciones | ILA-0026 | Consultar orden | Sí | No |
| 7 Devoluciones | ILA-0027 | Actualizar orden | Sí | No |
| 7 Devoluciones | ILA-0028 | Eliminar orden | Sí | No |
| 8 Restricciones | ILA-0029 | Crear restricción | Sí | No |
| 8 Restricciones | ILA-0030 | Consultar restricciones | Sí | Sí |
| 8 Restricciones | ILA-0031 | Modificar restricción | Sí | No |
| 8 Restricciones | ILA-0032 | Desactivar restricción | Sí | No |
| 9 Usuarios | ILA-0033 a ILA-0036 | Todas | Sí | No |
| 10 Proveedores | ILA-0037 a ILA-0040 | Todas | Sí | No |
| 11 Lotes | ILA-0041 | Registrar remesa | Sí | No |
| 11 Lotes | ILA-0042 | Consultar remesas | Sí | Sí |
| 11 Lotes | ILA-0043 | Actualizar remesa | Sí | No |
| 11 Lotes | ILA-0044 | Dar de baja remesa | Sí | No |

**Resumen del perfil Técnico.** Puede registrar y consultar ventas, emitir y consultar comprobantes, consultar inventario y remesas, consultar y levantar alertas de vencimiento, consultar métodos de pago y consultar restricciones sanitarias. No puede modificar ni eliminar nada, salvo dentro de la venta que está registrando.

---

## 4. Criterios aplicados

1. **Lo que RNF-0006 reserva explícitamente al administrador.** Modificar precios (ILA-0007), eliminar productos (ILA-0008) y ver reportes de ventas (ILA-0021 a ILA-0024).
2. **Lo que RNF-0006 concede explícitamente al operador.** Registrar ventas (ILA-0001) y consultar stock (ILA-0006).
3. **Alta de productos.** En la Sección 1 del Registro de Entrevista 1, a la pregunta sobre quién autoriza el ingreso de nuevos productos al inventario, se responde que la dueña autoriza personalmente. Por eso ILA-0005 es exclusiva del administrador, y el actor principal de EDU-0002 pasó de ACT-0002 a ACT-0001.
4. **Toda operación de actualización o eliminación es del administrador.** Es el criterio que unifica el resto de la matriz y el que resuelve las veinte ilaciones que solo pedían "sesión activa". Modificar o borrar una venta, anular un comprobante o levantar una alerta tienen consecuencias contables o sanitarias que no corresponden al mostrador.
5. **Excepción razonada: registrar una alerta de vencimiento (ILA-0014).** Se concede al técnico porque es quien detecta físicamente el lote defectuoso durante la atención, y porque levantar una alerta bloquea el producto, es decir, la operación es conservadora. Quitarla queda reservada al administrador.
6. **Excepción razonada: emitir un comprobante (ILA-0009).** Se concede al técnico porque la boleta se entrega al cliente en el mostrador, en el mismo acto de la venta, según lo declarado en la Sección 6 de la Entrevista 1.
7. **Consulta de remesas (ILA-0042).** Se concede al técnico porque necesita saber qué lote tiene disponible y cuándo vence antes de despachar. El registro y la modificación de remesas quedan con el administrador, por el mismo criterio que el alta de productos: la dueña recibe la mercancía del proveedor.
8. **Farmacovigilancia.** Definir qué medicamento se bloquea para qué grupo de riesgo corresponde a la dueña, que es quien mantiene el contacto con la química farmacéutica y con DIGEMID. Por eso el actor principal de EDU-0012 pasó de ACT-0002 a ACT-0001, conservando la consulta para ambos roles.

---

## 5. Contradicciones corregidas

| # | Dónde | Contradicción | Resolución |
| --- | --- | --- | --- |
| 1 | ILA-0007, ILA-0008 | Declaraban ACT-0002 como actor, cuando RNF-0006 reserva expresamente al administrador la modificación de precios y la eliminación de productos. | Restringidas al rol Administrador. |
| 2 | ILA-0003, ILA-0004 | Permitían modificar y eliminar ventas con solo tener sesión activa, cuando RNF-0006 declara que el operador "solo puede registrar ventas y consultar stock". | Restringidas al rol Administrador. |
| 3 | ILA-0022 | Permitía consultar reportes con solo sesión activa, cuando RNF-0006 reserva los reportes al administrador. | Restringida al rol Administrador. |
| 4 | ILA-0025 a ILA-0028 | Declaraban ACT-0001 como actor pero su precondición solo exigía sesión activa, sin verificar el rol: la restricción era decorativa. | Precondición alineada con el actor declarado. |
| 5 | ILA-0005 | Permitía a la técnica dar de alta productos, incluyendo su precio, contra lo declarado en la entrevista y en RNF-0006. | Restringida al rol Administrador. |
| 6 | ILA-0029, ILA-0031, ILA-0032 | Declaraban ACT-0002 para operaciones de farmacovigilancia. | Restringidas al rol Administrador; la consulta permanece para ambos. |
| 7 | Veinte ilaciones | Exigían únicamente "sesión activa", lo que autoriza cualquier operación a cualquier usuario y vacía de contenido a RNF-0006. | Todas declaran ahora el rol requerido. |
| 8 | Seis ilaciones | Exigían "permisos para modificar información" o "permisos de eliminación", formulaciones no verificables porque no nombran ningún rol. | Sustituidas por la fórmula única de la sección 2. |

---

## 6. Pendiente

1. ~~**RNF-0006 debe actualizarse.**~~ **Resuelto:** refundido en la versión 03.00.
2. ~~**Falta el requisito de la cuenta inactiva.**~~ **Resuelto:** es la condición 1 de RNF-0006 v03.00.
3. ~~**Falta definir el comportamiento ante un intento no autorizado.**~~ **Resuelto:** la condición 4 de RNF-0006 v03.00 establece que la opción se muestra visible pero deshabilitada, indicando el motivo al intentar activarla. Aplica a los mockups de los once módulos.
4. **El catálogo de fuentes sigue sin existir.** ACT-0001 y ACT-0002 ya están definidos aquí; FUE-0001 a FUE-0005 continúan sin definición.
