# Hallazgos de uso — FARMASIL

| Artefacto | Hallazgos de uso |
| --- | --- |
| Versión | 01.00 |
| Fecha | 22/11/2026 |
| Responsable | AUT-0001 |

## Historial de versiones

| Versión | Fecha | Cambio |
| --- | --- | --- |
| 01.00 | 22/11/2026 | Issue #88: se crea el documento y se registra su convención (DEC-081). Primeros hallazgos, HAL-001 a HAL-003, tomados de la prueba manual del inicio de sesión y de Usuarios. |

## 1. Para qué existe

Hay cosas que ninguna especificación, revisión de código ni prueba automática descubre: solo se ven usando la aplicación. Un botón que sobra, dos listas que muestran lo mismo, un campo que pierde el foco. Este documento reúne esos **hallazgos de uso** para que no se pierdan en un chat o en la memoria de quien los vio, y para que cada uno termine en su propio issue o se descarte con una razón.

No sustituye a otros documentos:

| Documento | Qué registra | Cuándo NO va aquí |
| --- | --- | --- |
| `11-registro-pendientes.md`, `PEN-NNN` | Una pregunta sin respuesta que bloquea un artefacto | Si el hallazgo es una pregunta para la dueña o el equipo, va como `PEN` |
| `11-registro-pendientes.md`, `DEC-NNN` | Una decisión ya tomada | Si el hallazgo obliga a cambiar una decisión, el cambio se registra como `DEC` nueva |
| Comentarios de un PR | Defectos de un cambio que aún se revisa | Si el defecto está en un PR abierto, se comenta en el PR |
| Issues de GitHub | El trabajo de corregir | Aquí se anota el número cuando el hallazgo se convierte en trabajo |

## 2. Cómo se usa

1. **Registrar.** Quien pruebe a mano la aplicación anota el hallazgo con el siguiente `HAL-NNN` libre, sin esperar a entenderlo del todo. Basta una frase, el módulo y cómo se vio. Pueden registrar la dueña del proyecto, R4 en las pruebas exploratorias y cualquiera que pruebe una pantalla.
2. **Clasificar.** Se asigna tipo y gravedad. Un hallazgo nuevo queda en estado *Nuevo*.
3. **Analizar.** R1 decide qué hacer con él y pasa el estado a *En análisis*. Si corregirlo contradice una `DEC-NNN`, una especificación o un mockup aprobado, **no se corrige primero**: se señala la decisión afectada, se registra la `DEC` nueva que la cambia (y se consulta a la dueña si es de diseño) y solo entonces se abre el issue.
4. **Convertir en issue.** Cada hallazgo que se corrige tiene su **propio issue**, con la etiqueta del módulo (`mod:XXX`), y su número se anota en la columna *Issue*. El estado pasa a *En issue*. Varios hallazgos del mismo módulo pueden compartir un issue solo si se corrigen juntos en el mismo cambio.
5. **Cerrar.** Al fusionarse la corrección, la fila se mueve a la sección 5 con la fecha y el PR. Si se decide no corregirlo, se mueve igual con estado *Descartado* y la razón.
6. **Revisar.** Al cierre de cada sprint se recorre la sección 4: lo que lleve más de un sprint en *Nuevo* o *En análisis* se vuelve a priorizar.

**Agentes de IA.** Antes de proponer un cambio en una pantalla, se revisan los `HAL-NNN` abiertos de su módulo. Si el cambio los toca, se cita el código. Un hallazgo se corrige solo con su issue; no se aprovecha otra tarea para arreglarlo en silencio.

**Tipos:** *Defecto* (no hace lo que debe), *Usabilidad* (funciona, pero confunde o estorba), *Diseño* (redundancia o decisión de pantalla que conviene revisar), *Mejora* (algo útil que no se pidió).
**Gravedad:** *Alta* si impide completar una operación; *Media* si confunde o obliga a rodeos; *Baja* si es cosmético o menor.
**Estados:** *Nuevo*, *En análisis*, *En issue*, *Corregido*, *Descartado*.

## 3. Plantilla de una entrada

| Campo | Qué escribir |
| --- | --- |
| Hallazgo | Qué se vio, en una o dos frases. Qué se esperaba frente a qué ocurrió. |
| Módulo | Prefijo del módulo (`USR`, `INV`…) o `GEN` si es transversal |
| Cómo se vio | Pasos mínimos para repetirlo, cuenta usada y, si ayuda, una captura |
| Relacionado | `ESP`, `DEC`, `PEN`, mockup o issue que toca |
| Origen | Quién lo encontró, cuándo y en qué prueba |

## 4. Hallazgos abiertos

| Código | Hallazgo | Módulo | Tipo | Gravedad | Estado | Registrado | Relacionado | Issue |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| HAL-001 | El botón «Actualizar» (`USR-BTN-ACTUALIZAR-USUARIO`) parece innecesario. Seleccionar una cuenta, en una tarjeta o en una fila de la tabla, ya carga la ficha, y «Actualizar» solo la pasa a edición. Al usuario le resulta confuso. Posible solución: quitar el botón y habilitar «Guardar cambios» al modificar un campo. | USR | Usabilidad | Media | Nuevo | 22/11/2026 | ESP-0035, ILA-0035, DEC-062, DEC-068, issue #68 | |
| HAL-002 | Las tarjetas de cuentas (`USR-PNL-CUENTAS`) y la tabla (`USR-TBL-USUARIOS`) muestran los mismos datos: nombre de usuario, rol y estado. Son dos listas redundantes de la misma consulta. Quitar una contradice DEC-062 y el mockup `ART-MKP-USR-0001`, así que antes del issue hace falta una `DEC` nueva y, de preferencia, la opinión de la dueña (agenda de la sección 4 del registro). | USR | Diseño | Baja | Nuevo | 22/11/2026 | DEC-062, DEC-068, ART-MKP-USR-0001 | |
| HAL-003 | En el inicio de sesión, al inhabilitarse el formulario mientras se verifica, el campo de contraseña pierde el foco y no vuelve a tenerlo. Si el intento falla, hay que hacer clic otra vez para escribir de nuevo. Debería recuperar el foco al terminar. | USR | Usabilidad | Baja | Nuevo | 22/11/2026 | DEC-080, issue #88 | |

## 5. Hallazgos cerrados

| Código | Hallazgo | Estado | Cerrado | PR o razón |
| --- | --- | --- | --- | --- |
| | Sin entradas todavía. | | | |

## 6. Origen de los hallazgos de esta versión

HAL-001 a HAL-003 los registró R1 (Leyni) el 22/11/2026 al probar a mano el inicio de sesión (#88) y la pantalla de Usuarios (#68) contra `DB_FARMASIL_PRUEBAS`, con las cuentas `admin_prueba` y `tecnico_prueba`. HAL-003 lo advirtió el agente al implementar el #88 y lo confirmó esa prueba.
