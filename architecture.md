# architecture.md — Mapa de navegación de FARMASIL_APP

> Punto de entrada para agentes de IA y personas nuevas. Responde tres preguntas: **qué es el sistema**, **dónde vive cada artefacto** y **cuál es la fuente de verdad cuando dos documentos discrepan**. No repite el contenido de otros documentos: los enlaza. Para el detalle de dependencias entre módulos, ve a `docs/arquitectura/Contexto_Arquitectura_FARMASIL_ParaAgentesIA.md`.

---

## 1. El sistema en 30 segundos

FARMASIL_APP es un sistema de gestión de inventarios y ventas para una farmacia, de un solo equipo y sin internet permanente.

| Aspecto | Valor | Fuente |
| --- | --- | --- |
| Plataforma | C#, WPF, .NET 10 LTS, publicación autocontenida win-x64 sin recorte | `catalogo/08-requisitos-no-funcionales.md` (RNF-0008), DEC-028 |
| Base de datos | SQL Server 2025 Express, base `DB_FARMASIL`, 16 tablas | `modelo/05-diccionario-datos.md` |
| Despliegue | Un único nodo: Windows 11, i5-12500, 16 GB, 1 TB | RNF-0007, diagrama `VF-01` |
| Usuarios | Dueña y Técnica, un usuario a la vez | RNF-0003, `diseno/07-matriz-permisos.md` |
| Módulos | 13 (VEN, INV, DOC, ALV, PAG, REP, DEV, RES, USR, PRV, LOT, REA, COT) | `Contexto_Arquitectura_FARMASIL_ParaAgentesIA.md` §2 |
| Catálogo | 13 EDU, 52 ILA, 52 ESP, 10 RNF | `catalogo/` |

---

## 2. Orden de lectura recomendado

1. `AGENTS.md`: reglas obligatorias (commits, `git add`, atribución, imágenes que se ignoran).
2. Este archivo.
3. `docs/arquitectura/Contexto_Arquitectura_FARMASIL_ParaAgentesIA.md`: módulos, grafo de dependencias y decisiones de diseño cerradas.
4. `seguimiento/11-registro-pendientes.md`: `PEN-NNN` abiertos y `DEC-NNN` tomados. Revísalo antes de proponer cualquier cambio.
5. El artefacto concreto de la tarea (sección 3).

---

## 3. Dónde está cada cosa

### 3.1 Requisitos y modelo

| Necesitas | Ruta |
| --- | --- |
| Qué debe hacer el sistema (educciones → ilaciones → especificaciones) | `catalogo/01-educciones.md`, `02-ilaciones.md`, `03-especificaciones.md` |
| Trazabilidad EDU↔ILA↔ESP | `catalogo/04-trazabilidad.md` |
| Requisitos no funcionales (RNF-0001 a RNF-0010) | `catalogo/08-requisitos-no-funcionales.md` |
| Esquema de base de datos | `modelo/05-diccionario-datos.md` |
| Permisos por rol | `diseno/07-matriz-permisos.md` |
| Pantallas | `diseno/Mockups/` (fuentes HTML en `diseno/Mockups/fuente/`); convención de nombres en `diseno/06-guia-nomenclatura-mockups.md` |
| Pruebas de software | `pruebas de software/09-pruebas-software.md` |
| Riesgos | `riesgos/10-gestion-riesgos.md` |
| Pendientes y decisiones | `seguimiento/11-registro-pendientes.md` |
| Auditorías y revisiones | `seguimiento/12-*`, `13-*`, `14-*` |
| Plan de implementación (roles, sprints, definición de terminado) | `docs/plan/plan-implementacion.md` |

### 3.2 Vistas de arquitectura

| Vista | Documento | Diagramas (PNG) | Fuente PlantUML |
| --- | --- | --- | --- |
| Arquitectura genérica | `docs/arquitectura/Insumo_*.md` | `docs/arquitectura/ArquitecturaGenerica_Nivel1.png`, `_Nivel2.png` | n/d (ver `Insumo_Matriz_Interfaces_ArquitecturaGenerica_v2.md`) |
| Lógica | `docs/arquitectura/VistaLogica_v01.md` (histórico) | `docs/diagramas/VL-01` a `VL-04` | `docs/diagramas/plaintext-VistaLogica/` |
| Procesos | n/d | `docs/diagramas/VP-*` | `docs/diagramas/plaintext-VistaProcesos/` |
| Física (despliegue) | pendiente de ubicar en el repo | `docs/diagramas/VF-01_Despliegue.png`, `VF-02_RespaldoRestauracion.png` | `docs/diagramas/plaintext-VistaFisica/` |
| Casos de uso | n/d | `docs/diagramas/CU-*` | `docs/diagramas/plaintext-CasosUso/` |
| Comportamiento (estados y secuencia) | n/d | `docs/diagramas/EST-*`, `SEC-*` | `docs/diagramas/plaintext-Comportamiento/` |

### 3.3 Convenciones de los diagramas

- Cada diagrama tiene un `.puml` fuente en una carpeta `plaintext-<Vista>/` y un PNG generado en `docs/diagramas/`, con el mismo nombre base.
- Prefijos: `CU` casos de uso, `EST` estados, `SEC` secuencia, `VL` vista lógica, `VP` vista de procesos, `VF` vista física.
- Si el PNG y el `.puml` difieren, prevalece el `.puml`.
- **Se ignoran** `docs/diagramas/VL_01_Trazabilidad.png` y `docs/diagramas/Vista Logica.png`: son imágenes manuales de exposición, sin fuente (DEC-024, `AGENTS.md` §8).

### 3.4 Vista física: qué dicen los dos diagramas

| Diagrama | Contenido | Estado |
| --- | --- | --- |
| `VF-01_Despliegue` | Un nodo «device» (PC de la farmacia) con tres entornos de ejecución: .NET 10 (`FarmasilApp.exe`), SQL Server 2025 Express (instancia SQLEXPRESS, `DB_FARMASIL`) y Programador de tareas de Windows (script T-SQL de respaldo). Almacenamiento de respaldo como nodo externo. App y motor se comunican por memoria compartida, sin puerto de red. | Borrador |
| `VF-02_RespaldoRestauracion` | Secuencia del respaldo periódico (`BACKUP DATABASE` → `.bak` → `RESTORE VERIFYONLY`) y de la restauración en un equipo nuevo. | Borrador 00.01 |

Decisiones aún abiertas que los diagramas marcan como `[POR DEFINIR]`:

- Autenticación de la aplicación ante el motor (cuenta de Windows o inicio de sesión SQL): PEN-035, con la cuenta de Windows como comportamiento provisional.
- Respaldo: frecuencia, destino, disparador (Programador de tareas o la aplicación), prueba de restauración y aviso de falla.

No las des por resueltas al implementar; si necesitas una, regístrala como `PEN-NNN` (`AGENTS.md` §7).

---

## 4. Fuente de verdad cuando hay discrepancia

| Tema | Prevalece | Sobre |
| --- | --- | --- |
| Esquema de datos | `modelo/05-diccionario-datos.md` (versión más reciente) | Texto de ilaciones, especificaciones o vistas antiguas |
| Plataforma y versiones | RNF-0008 v02.00 y DEC-028 | Cualquier mención de .NET 8 en documentos anteriores |
| Diagramas | `.puml` | PNG |
| Dependencias entre módulos | Grafo de `Contexto_Arquitectura_FARMASIL_ParaAgentesIA.md` §3 | Cualquier acoplamiento improvisado |
| Permisos | `diseno/07-matriz-permisos.md` (DEC-050) | Anotaciones en mockups |
| Decisiones ya tomadas | `DEC-NNN` en `seguimiento/11-registro-pendientes.md` | Preferencias del agente |
| Vista lógica | Diccionario v10.00 y DEC-028 a DEC-041 | `VistaLogica_v01.md` (histórico) |

---

## 5. Reglas que más se rompen

1. **No hagas `git commit`, merge, rebase ni PR sin pedido explícito.** `git add` solo con rutas específicas, nunca `.` ni `-A`.
2. Propón commits escribiendo `.git/COMMIT_PROPUESTA.txt` (formato de `CONTRIBUTING.md`, UTF-8 sin BOM, LF) y muestra el texto completo y el comando `git commit -F .git/COMMIT_PROPUESTA.txt`. Sin atribución al agente.
3. El árbol de trabajo suele tener cambios ajenos sin comitear: no los toques ni los agregues.
4. Respeta el grafo de dependencias entre módulos. Usuarios es transversal (autenticación y auditoría), no una dependencia de negocio.
5. Literales de dominio sin tilde en `CHECK` y `enum` (DEC-031).
6. Solo Leyni hace el squash & merge.

---

## 6. Flujo de trabajo y ramas

- Ramas: `<tipo>/<n.º issue>-<descripción-corta>` (`docs`, `feat`, `fix`, `chore`, `test`). Ejemplo vigente: `docs/147-vista-fisica`.
- Commits: `<tipo>(<alcance>): <descripción imperativa>` + `Refs #<n.º>`. PR con `Closes #<n.º>`.
- CI (`.github/workflows/`): `build-and-test` e `issue-link` deben estar en verde.
- Detalle completo: `CONTRIBUTING.md`. Para trabajar un issue existe la skill `work-issue` en `.claude/skills/`.

---

## 7. Mapa de carpetas

```
ProyectoFarmasil/
├── AGENTS.md                  reglas para agentes (leer primero)
├── architecture.md            este archivo
├── CONTRIBUTING.md            flujo de ramas, commits y PR
├── README.md                  índice y tabla de módulos
├── .github/                   CODEOWNERS, plantilla de PR, workflows de CI
├── .claude/skills/            skills del proyecto (work-issue)
├── .tablero/                  scripts del tablero de GitHub Projects
├── catalogo/                  01 educciones, 02 ilaciones, 03 especificaciones,
│                              04 trazabilidad, 08 requisitos no funcionales
├── modelo/                    05 diccionario de datos
├── diseno/                    06 guía de mockups, 07 matriz de permisos, Mockups/
├── docs/
│   ├── arquitectura/          contexto para agentes, insumos, vista lógica, niveles 1 y 2
│   ├── plan/                  plan de implementación (roles, sprints, definición de terminado)
│   └── diagramas/             PNG de todas las vistas + plaintext-*/ con los .puml
├── pruebas de software/       09 pruebas
├── riesgos/                   10 gestión de riesgos
└── seguimiento/               11 pendientes y decisiones, 12–14 auditorías
```

---

## 8. Mantenimiento de este archivo

Actualízalo cuando se añada una vista, una carpeta o un documento nuevo, o cuando cambie una regla de la sección 4. Si algo del detalle ya vive en otro documento, enlázalo en vez de copiarlo.
