# Contribuir a FARMASIL_APP

> Guía rápida para cualquier integrante del equipo. Sigue estos pasos para el issue que tengas asignado — no necesitas acceso a la configuración del repositorio para nada de esto.

## 1. Antes de empezar

```bash
git checkout main
git pull
git checkout -b <tipo>/<numero-issue>-<descripcion-corta>
```

- **`<tipo>`:** `feat` (funcionalidad nueva), `docs` (documento de arquitectura), `fix` (corrección de bug), `chore` (tooling/config), `test` (pruebas)
- **`<numero-issue>`:** el número del issue del tablero que estás trabajando
- **`<descripcion-corta>`:** minúsculas, sin tildes ni `ñ`, palabras separadas por guiones

Ejemplo: `git checkout -b docs/5-cu01-actor-dueno`

**Luego, mueve tu tarjeta en el tablero de Projects de Todo a In Progress.** Este paso es manual — nadie lo hace automático por ti.

## 2. Mientras trabajas: commits

```
<tipo>(<alcance>): <descripción en imperativo, minúscula, sin punto final>

<cuerpo opcional: qué y por qué>

Refs #<numero-issue>
```

Ejemplo:
```bash
git commit -m "docs(actor-dueno): agregar caso de uso CU-01 actor dueño" -m "Describe el actor dueño y sus casos de uso principales del módulo." -m "Refs #5"
```

Commits pequeños y frecuentes — no uno solo gigante al final. Si necesitas un mensaje de varias líneas y `-m -m` se te complica, usa `git commit` sin `-m` para abrir tu editor, o `git commit -F mensaje.txt`.

## 3. Cuando termines: Pull Request

```bash
git push -u origin <tu-rama>
```

Abre el PR en GitHub (verás un banner "Compare & pull request" apenas hagas push — no se abre solo, tienes que darle clic). La descripción se llena sola con la plantilla del repositorio: complétala.

- **Título:** igual al commit, ej. `docs(actor-dueno): agregar caso de uso CU-01 actor dueño`
- **Descripción:** debe incluir `Closes #<numero-issue>` — sin esto, el issue no se cierra solo al fusionar.
- **Checklist antes de avisar que está listo:**
  - [ ] CI en verde: los checks `build-and-test` e `issue-link` deben pasar
  - [ ] Nada marcado `[pendiente]` sin explicar por qué
  - [ ] Cumple lo pedido en el issue

## 4. Después de abrir el PR

**No lo fusiones tú mismo** — el squash & merge lo hace Leyni. GitHub no deja fusionar un PR hacia `main` hasta que se cumplan estas reglas:

| Regla | Qué significa para ti |
| --- | --- |
| Todo cambio entra por PR | No se puede hacer push directo a `main`. |
| `build-and-test` en verde | La solución compila y las pruebas pasan. Si falla, abre el detalle del check, corrige en tu rama y vuelve a hacer push. |
| `issue-link` en verde | La descripción del PR tiene `Closes #<numero-issue>`. Si falta, edita la descripción y el check se vuelve a ejecutar solo. |
| Aprobación de Leyni | Es la responsable del código de todo el repositorio (`.github/CODEOWNERS`). Si haces push después de la aprobación, la aprobación se descarta y hay que pedirla de nuevo. |
| Conversaciones resueltas | Cada comentario de revisión debe quedar resuelto antes de fusionar. |

Una vez que lo revise y fusione:

- tu rama se elimina sola,
- el issue se cierra solo,
- tu tarjeta pasa a Done sola.

No hace falta que hagas nada más ni que borres la rama a mano.

## Checklist rápido

1. `git checkout main && git pull`
2. `git checkout -b tipo/numero-descripcion`
3. Mover tu tarjeta a In Progress (manual)
4. Commits pequeños, `tipo(alcance): descripción`, `Refs #numero`
5. `git push -u origin tu-rama`
6. Abrir el PR con `Closes #numero` y esperar los checks en verde
7. Avisar que está listo y esperar la aprobación y el merge — listo, el resto es automático