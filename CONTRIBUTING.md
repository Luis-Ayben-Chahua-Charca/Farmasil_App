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

Abre el PR en GitHub (verás un banner "Compare & pull request" apenas hagas push — no se abre solo, tienes que darle clic):

- **Título:** igual al commit, ej. `docs(actor-dueno): agregar caso de uso CU-01 actor dueño`
- **Descripción:** debe incluir `Closes #<numero-issue>` — sin esto, el issue no se cierra solo al fusionar.
- **Checklist antes de avisar que está listo:**
  - [ ] CI en verde (si el repo ya tiene checks configurados para tu tipo de cambio)
  - [ ] Nada marcado `[pendiente]` sin explicar por qué
  - [ ] Cumple lo pedido en el issue

## 4. Después de abrir el PR

**No lo fusiones tú mismo** — el squash & merge lo hace Leyni. Una vez que lo revise y fusione:

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
6. Abrir el PR con `Closes #numero`
7. Avisar que está listo y esperar el merge — listo, el resto es automático