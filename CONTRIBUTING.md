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

## Preparar el entorno

Necesitas Windows 11: WPF solo compila y se ejecuta en Windows (RNF-0008).

| Herramienta | ¿Obligatoria? | Para qué |
| --- | --- | --- |
| SDK de .NET 10 (10.0.100 o posterior de la serie 10.0) | Sí | Compilar, probar y ejecutar. Incluye el compilador de C#, el comando `dotnet` y WPF: no se instala C# ni WPF aparte |
| Git | Sí | Trabajar con el repositorio |
| SQL Server 2025 Express, instancia `SQLEXPRESS` | Sí, instálalo ahora | La base `DB_FARMASIL`. El esqueleto todavía no se conecta, pero el script de la base (#70) llega esta semana |
| `sqlcmd` (`winget install sqlcmd`) | Sí | Lo usa `database/recrear.ps1` para crear la base |
| SQL Server Management Studio | Recomendada | Ver tablas y probar consultas |
| Visual Studio 2026 Community, carga de trabajo «Desarrollo de escritorio de .NET» | Recomendada | Diseñador de XAML y depurador. Es el entorno del recorrido |
| VS Code con C# Dev Kit, o Rider | Alternativa | Se aceptan si compilas y pruebas con `dotnet` antes de subir. No tienen el diseñador de XAML de Visual Studio |

**Comprueba la instalación** desde la carpeta del repositorio:

```bash
dotnet --version        # debe mostrar 10.0.x
dotnet build FarmasilApp.slnx --configuration Release
dotnet test FarmasilApp.slnx --configuration Release --no-build
```

`global.json` exige el SDK 10. Si solo tienes el 8, `dotnet` falla con un mensaje que lo indica: instala el SDK 10 y vuelve a probar.

**Paquetes.** Los paquetes de NuGet se descargan solos con el primer `dotnet build`, y esa vez necesitas internet. Todos están en la serie 10.0.x (RNF-0008) y su versión está fija en cada `.csproj`. El acceso a datos es ADO.NET con `Microsoft.Data.SqlClient`, sin ORM. Para agregar un paquete, avisa en el grupo antes de abrir el PR; R1 lo aprueba.

**Dónde está cada configuración:**

| Archivo | Qué fija |
| --- | --- |
| `global.json` | La versión del SDK |
| `Directory.Build.props` | Opciones comunes a todos los proyectos (nulabilidad, `using` implícitos, versión del lenguaje) |
| `FarmasilApp.slnx` | Los cinco proyectos de la solución |
| `src/*/*.csproj`, `tests/*/*.csproj` | Las referencias entre capas y los paquetes de cada proyecto |
| `src/FarmasilApp.Vista/appsettings.json` | La cadena de conexión `DB_FARMASIL` (ver «Desarrollo local» en `README.md`) |
| `src/FarmasilApp.Vista/appsettings.local.json` | Tu configuración local, opcional, para apuntar a otra instancia o a `DB_FARMASIL_PRUEBAS`. No se versiona |

No subas archivos de tu editor (`.vs/`, `.vscode/`, `.idea/`): están en `.gitignore`.

## Antes de subir cambios de código

Corre los mismos pasos que la CI. Si fallan en tu equipo, fallarán en el PR:

```bash
dotnet build FarmasilApp.slnx --configuration Release
dotnet test FarmasilApp.slnx --configuration Release --no-build
```

La compilación no debe dejar advertencias nuevas. Si tu issue cambia el esquema de la base, recréala con `database/recrear.ps1` antes de probar.

## Cómo agregar una pantalla

Tu primer issue de código es la consulta de tu módulo. Sigue estos pasos, que son los que usa el esqueleto y la consulta de Usuarios (#68). `<Modulo>` es el prefijo de tu módulo (`INV`, `PRV`, …) y `Xxx`, el nombre de la pantalla.

1. **VistaModelo.** Crea `src/FarmasilApp.VistaModelo/<Modulo>/XxxViewModel.cs`, que hereda de `ViewModelBase`. Cada botón es una propiedad `RelayCommand`; cada dato que muestra la pantalla, una propiedad que avisa sus cambios.
2. **Regístrala** en `AgregarVistaModelo` (`src/FarmasilApp.VistaModelo/Comun/RegistroDependencias.cs`).
3. **Vista.** Crea `src/FarmasilApp.Vista/<Modulo>/XxxView.xaml`, un `UserControl`. Cada componente lleva en `AutomationProperties.AutomationId` el identificador de la guía de nomenclatura (`diseno/06-guia-nomenclatura-mockups.md`). `x:Name` solo cuando el XAML lo necesita, con guion bajo en lugar de guion (DEC-060). El code-behind solo tiene el constructor.
4. **Asocia** la VistaModelo con su Vista con un `DataTemplate` en `src/FarmasilApp.Vista/Comun/Plantillas.xaml`.
5. **Navega** a la pantalla con `INavegacionServicio.NavegarA<XxxViewModel>()`.
6. **Modelo.** Pon el servicio y las entidades en `src/FarmasilApp.Modelo/<Modulo>/`, y regístralos en `AgregarModelo` (`src/FarmasilApp.Modelo/Comun/RegistroDependencias.cs`). Si necesitas otro módulo, usa su interfaz (`ILotes`, `IPagos`, …), nunca sus clases ni sus tablas (DEC-055).
7. **Persistencia.** Pon el repositorio en `src/FarmasilApp.Persistencia/<Modulo>/` y regístralo en `AgregarPersistencia`. Toda consulta SQL usa parámetros.
8. **Pruebas.** Agrégalas en `tests/FarmasilApp.Pruebas/<Modulo>/`, según `pruebas de software/09-pruebas-software.md`.

Dentro de cada proyecto, el código va en la carpeta de su módulo (plan, sección 2). Si la carpeta no existe, créala: el esqueleto solo trae `Comun`, `Principal` y `Seguridad`, porque git no guarda carpetas vacías.

## Checklist rápido

1. `git checkout main && git pull`
2. `git checkout -b tipo/numero-descripcion`
3. Mover tu tarjeta a In Progress (manual)
4. Commits pequeños, `tipo(alcance): descripción`, `Refs #numero`
5. `git push -u origin tu-rama`
6. Abrir el PR con `Closes #numero` y esperar los checks en verde
7. Avisar que está listo y esperar la aprobación y el merge — listo, el resto es automático