# Insumo — Pool de Diagramas y Estilos de Arquitectura (FARMASIL_APP)

> Documento de trabajo, no un entregable. Recoge **todo** el catálogo de diagramas UML y estilos arquitectónicos que enseña `ArquitecturaSoftware.pdf` (M.Sc. Percy Huertas Niquén), y los ordena según en qué fase del curso encajan y qué le resuelven específicamente a FARMASIL. Sirve de base para generar los diagramas de esta y las próximas entregas — no solo "arquitectura genérica".
>
> **Cómo leer las tablas:** *Prioridad* es orden sugerido de trabajo dentro de su lista, no obligación. *Qué resuelve en FARMASIL* apunta a una ambigüedad, riesgo o vacío ya documentado en el catálogo — no es una justificación genérica de manual. *Estado* se actualiza a mano conforme se van construyendo.

---

## 0. El pool completo, tal como aparece en el PPT del curso

**Fases del curso (`Objetivos por etapas`):**
- **Fase 1** (EC1 + Examen 1): Actas de trabajo · Catálogo de requisitos · **Arquitectura genérica** · **Diagrama de comportamiento**
- **Fase 2** (EC2 + Examen 2): Actas de trabajo · Vista lógica · Vista de procesos · Vista física
- **Fase 3** (EC3 + Examen 3): Actas de trabajo · Arquitectura centrada en interfaces de usuario · Arquitectura centrada en datos · Arquitectura centrada en el flujo de datos

**UML — parte estática:** clase · objetos · estructura (composite structure) · componentes · paquetes · despliegue

**UML — parte dinámica:** casos de uso · actividades · estados · secuencia · interacción · comunicación · tiempos (UML 2.0)

**Modelo 4+1 (vistas, no diagramas):** escenario · lógica/conceptual · desarrollo (estático) · procesos (dinámico) · física · interfaces de usuario

**Estilos de arquitectura:** orientada al flujo de datos (lotes secuenciales, tuberías y filtros) · centrada en el dato (repositorio, pizarra) · jerárquica (subrutina principal, maestro-esclavo, capas) · comunicación asíncrona / invocación implícita (con/sin buffer)

---

## A. Encajan en "Arquitectura Genérica" (Fase 1 — vista de desarrollo)

Todo lo que sigue describe el aspecto **estático** de los módulos sin comprometer decisiones de despliegue (vista física) ni de coordinación en tiempo de ejecución (vista de procesos) — el criterio que separa "arquitectura genérica" del resto, según la propia definición del curso de la vista de desarrollo.

| # | Diagrama / estilo | Prioridad | Qué resuelve en FARMASIL | Estado |
|---|---|---|---|---|
| 1 | **Diagrama de componentes** | Ya construido | Es el artefacto central de esta entrega: los módulos como componentes, con sus dependencias reales extraídas del diccionario de datos. | Hecho para 11 módulos (lollipops pendientes); **insumo de corrección para 13 módulos ya disponible** en `claude/Insumo_Matriz_Interfaces_ArquitecturaGenerica_v2.md` (matriz 13×13 + explicación de interfaces, tras integrar Reabastecimiento y Comparación de Precios de la Entrevista 3) — falta trasladarlo al diagrama visual en FigJam. |
| 2 | **Diagrama de paquetes** | Alta | Agrupa los 11 componentes en subsistemas de mayor nivel — retoma la agrupación informal que ya habíamos esbozado en la primera iteración en FigJam (núcleo / catálogo y stock / operación diaria / derivados) y la formaliza como artefacto propio. | Pendiente |
| 3 | **Arquitectura por capas** (estilo, no diagrama UML) | Alta | Corta el sistema por responsabilidad técnica (presentación / lógica de negocio / acceso a datos) en vez de por módulo de negocio. Encaja bien con RNF-0001 (operación local) y RNF-0007 (un solo equipo, sin servidor separado) — visto en detalle en la conversación anterior. | Pendiente |
| 4 | **Diagrama de estructura compuesta** (composite structure) | Media | Vista de "caja blanca" opcional para detallar cómo se realiza internamente una interfaz de un componente puntual — el candidato más claro es Ventas, que coordina Alertas de Restricción, Alertas de Vencimiento, Lotes y Métodos de Pago en una sola operación. Es una extensión del diagrama de componentes, no un artefacto nuevo. | Pendiente / opcional |

---

## B. No encajan en "arquitectura genérica" pero sí aplican al proyecto

Agrupados por el entregable/fase del curso al que pertenecen realmente, para no perderlos de vista cuando llegue el momento.

### B.1 — Fase 1, el otro entregable: "Diagrama de comportamiento" (vista dinámica)

| # | Diagrama | Prioridad | Qué resuelve en FARMASIL | Estado |
|---|---|---|---|---|
| 5 | **Diagrama de estados** | Alta | Resuelve directamente el bloqueante documentado en `Insumo_Gestion_Riesgos_FARMASIL.md`: ILA-0028 tiene marcado **"PENDIENTE DE DECISIÓN"** entre eliminación física o lógica de una orden de devolución. También aplica a `estado_lote` (Disponible/Agotado) y `estado_producto` (Disponible/Bloqueado por devolución/Descontinuado). | Pendiente |
| 6 | **Diagrama de secuencia** | Alta | Formaliza el orden exacto de la validación cruzada en "crear venta" (Ventas → Alertas de Restricción → Alertas de Vencimiento → descuento en Lotes), hoy solo implícito en el pseudocódigo de ESP-0001. También cubre el riesgo ILA-0025-R02 (atomicidad entre `TBL_ORDENES_DEVOLUCION` y `TBL_DETALLE_DEVOLUCION`). | Pendiente |
| 7 | **Diagrama de casos de uso** | Media | Vista de escenario del 4+1: actores (ACT-0001, ACT-0002) contra las 44 operaciones. En gran parte ya está cubierto en forma tabular por `07-matriz-permisos.md`, así que su valor aquí es más de presentación que de resolver una ambigüedad nueva. | Pendiente |
| 8 | **Diagrama de actividades** | Media | Útil para procesos con ramificaciones y decisiones, como el flujo completo de devoluciones (ILA-0025 a ILA-0028), que hoy tiene tres riesgos abiertos sobre validaciones no definidas (stock suficiente, proveedor activo, edición de orden ya enviada). | Pendiente |
| 9 | **Diagrama de comunicación / interacción** | Baja | Mismo contenido que el de secuencia con otro layout (énfasis en objetos en vez de en el tiempo). Redundante si ya se construye el de secuencia; no priorizar. | — |
| 10 | **Diagrama de tiempos** | Baja / nicho | Solo tendría sentido si se quiere documentar formalmente RNF-0005 (venta en ≤4s) como restricción temporal explícita. Opcional, poco común en un catálogo de este tamaño. | — |

### B.2 — Fase 2: Vista lógica, de procesos y física

| # | Diagrama | Prioridad | Qué resuelve en FARMASIL | Estado |
|---|---|---|---|---|
| 11 | **Diagrama de clases** | Alta | Formaliza el modelo de datos (14 tablas del Diccionario de Datos) con atributos, relaciones y ahora sí, métodos. Es el lugar correcto para resolver la "decisión abierta" que `02-ilaciones.md` marca como "el punto más frágil del modelo": `TBL_PRODUCTOS` maneja un solo lote/fecha de vencimiento por fila cuando debería ser una relación uno-a-muchos con `TBL_LOTES`. | Pendiente |
| 12 | **Diagrama de objetos** | Baja | Instancia concreta del diagrama de clases — útil como ejemplo pedagógico (una venta real con sus líneas y lotes) más que como artefacto de diseño nuevo. | — |
| 13 | **Diagrama de despliegue** | Media | Vista física: mapea el software contra el único equipo que exige RNF-0007 (Windows 11, i5, 16 GB RAM, SQL Server Express). Sencillo de construir una vez decidida la arquitectura por capas. | Pendiente |

### B.3 — Fase 3: Arquitecturas centradas

| # | Estilo | Prioridad | Qué resuelve en FARMASIL | Estado |
|---|---|---|---|---|
| 14 | **Arquitectura centrada en datos (repositorio)** | Alta | Encaja de forma casi literal: FARMASIL ya opera así, con `DB_FARMASIL` como repositorio central compartido por los 11 módulos y toda la comunicación entre ellos pasando por esa base de datos. Va a ser la más fácil de argumentar de las tres. | Pendiente |
| 15 | **Arquitectura centrada en interfaces de usuario** | Media | Vista de navegación sobre los mockups (`ART-MKP-*`) ya diseñados o pendientes de diseñar — formaliza cómo se navega entre las pantallas de los 11 módulos. | Pendiente |
| 16 | **Arquitectura centrada en el flujo de datos** (tuberías y filtros / lotes secuenciales) | Baja | Relevancia real baja: FARMASIL no transforma datos en pipeline, es un sistema transaccional CRUD sobre una base relacional. Habría que documentarla más por completitud del entregable de Fase 3 que porque describa algo verdadero del sistema. | Pendiente |

---

## C. Excluidos del pool — no aplican a FARMASIL

Para que quede constancia de que se revisaron y se descartaron a propósito, no por omisión:

- **Arquitectura de pizarra (blackboard).** Pensada para sistemas con reconocimiento de voz, patrones o IA — FARMASIL no tiene ningún componente de ese tipo.
- **Maestro-esclavo.** Resuelve tolerancia a fallos en sistemas distribuidos; FARMASIL es mono-usuario-local sobre un solo equipo (RNF-0007).
- **Subrutina principal.** Es un patrón de reutilización de código de bajo nivel (típico de C++), no un nivel de abstracción arquitectónica útil para este catálogo.
- **Arquitectura de comunicación asíncrona / invocación implícita.** Pensada para desacoplar eventos entre componentes gráficos interactivos o IDEs; no hay evidencia en el catálogo de que FARMASIL necesite ese desacoplamiento — sus operaciones son transacciones síncronas de mostrador.

---

## Cómo seguir usando este documento

Cada vez que se construya uno de los diagramas listados, actualizar su columna *Estado* aquí (Pendiente → En progreso → Hecho, con el enlace o la ubicación del artefacto). Este archivo es el mapa completo del pool; los documentos de cada entregable (como `ArquitecturaGenerica.md`) solo deben contener el resultado ya trabajado, no la lista completa de opciones.
