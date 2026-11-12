# Catálogo de Requisitos — FARMASIL

Sistema de gestión para farmacia. Universidad Nacional de San Agustín, Ingeniería de Requisitos.

## Contenido

| Archivo | Versión | Descripción |
| --- | --- | --- |
| `01-educciones.md` | v05.00 y posteriores | 13 educciones. Requisitos tal como los expresó el cliente. |
| `02-ilaciones.md` | — | 52 ilaciones. Cuatro fases CRUD por educción. |
| `03-especificaciones.md` | — | 52 especificaciones en pseudocódigo. |
| `04-trazabilidad.md` | v04.00 | Matriz EDU↔ILA↔ESP↔artefactos, más trazabilidad hacia RNF, modelo de datos y pruebas. |
| `05-diccionario-datos.md` | v09.00 | 16 tablas de `DB_FARMASIL`, correspondencia de tipos y relaciones. |
| `06-guia-nomenclatura-mockups.md` | v05.00 | Prefijos por módulo, tipos de componente e inventario completo. |
| `07-matriz-permisos.md` | v03.00 | Actores, roles y permisos por operación. |
| `08-requisitos-no-funcionales.md` | — | 10 requisitos no funcionales. RNF-0006 refundido en seguridad; RNF-0010 documenta los procesos programados. |
| `09-pruebas-software.md` | v05.00 | 257 pruebas sobre las 52 especificaciones y los 10 requisitos no funcionales. |
| `10-gestion-riesgos.md` | v07.00 | 127 riesgos (11 generales y 116 de las 52 ilaciones), 74 resueltos, con su estado verificado contra el catálogo vigente. Insumo de la plantilla GRI_0002. |
| `11-registro-pendientes.md` | v03.00 | Cuestiones abiertas (PEN), agenda para la dueña y registro de decisiones (DEC). Carpeta `seguimiento/`. |

El plan de implementación (roles, calendario de sprints y definición de terminado) está en [`docs/plan/plan-implementacion.md`](docs/plan/plan-implementacion.md).

## Estructura del catálogo

13 módulos, cada uno con una educción, cuatro ilaciones y cuatro especificaciones.

| # | Módulo | Educción | Ilaciones | Especificaciones | Prefijo UI |
| --- | --- | --- | --- | --- | --- |
| 1 | Ventas | EDU-0001 | ILA-0001 a 0004 | ESP-0001 a 0004 | VEN |
| 2 | Inventario | EDU-0002 | ILA-0005 a 0008 | ESP-0005 a 0008 | INV |
| 3 | Documentación tributaria | EDU-0003 | ILA-0009 a 0012 | ESP-0009 a 0012 | DOC |
| 4 | Alertas de vencimiento | EDU-0004 | ILA-0013 a 0016 | ESP-0013 a 0016 | ALV |
| 5 | Métodos de pago | EDU-0009 | ILA-0017 a 0020 | ESP-0017 a 0020 | PAG |
| 6 | Reportes de ventas | EDU-0010 | ILA-0021 a 0024 | ESP-0021 a 0024 | REP |
| 7 | Devoluciones | EDU-0011 | ILA-0025 a 0028 | ESP-0025 a 0028 | DEV |
| 8 | Restricciones de venta | EDU-0012 | ILA-0029 a 0032 | ESP-0029 a 0032 | RES |
| 9 | Usuarios | EDU-0013 | ILA-0033 a 0036 | ESP-0033 a 0036 | USR |
| 10 | Proveedores | EDU-0014 | ILA-0037 a 0040 | ESP-0037 a 0040 | PRV |
| 11 | Lotes | EDU-0015 | ILA-0041 a 0044 | ESP-0041 a 0044 | LOT |
| 12 | Reabastecimiento | EDU-0016 | ILA-0045 a 0048 | ESP-0045 a 0048 | REA |
| 13 | Comparación de precios de proveedores | EDU-0017 | ILA-0049 a 0052 | ESP-0049 a 0052 | COT |

Los códigos EDU-0005 a EDU-0008 están reservados y no se reasignan: corresponden a requisitos reclasificados como no funcionales. El salto de numeración es intencional.

## Convenciones

- Componentes de interfaz: guion y mayúsculas (`VEN-BTN-CREAR-VENTA`).
- Tablas y campos de base de datos: guion bajo, con punto separador (`DB_FARMASIL.TBL_PRODUCTOS`).
- Versión `DD.DD`, fecha `dd/mm/aaaa`, ningún campo vacío (`Ninguno` cuando no aplica).
- Toda operación que escriba en más de una tabla se ejecuta dentro de una transacción.

## Desarrollo local

Para preparar el entorno (herramientas, paquetes y verificación), sigue «Preparar el entorno» en [`CONTRIBUTING.md`](CONTRIBUTING.md).

La aplicación es C# sobre .NET 10 LTS con WPF y SQL Server 2025 Express (RNF-0008, DEC-028). La solución es `FarmasilApp.slnx`, con un proyecto por capa (Vista, VistaModelo, Modelo y Persistencia) y uno de pruebas (`tests/FarmasilApp.Pruebas`).

Requisitos: Windows 11, el SDK de .NET 10 (`global.json`) y SQL Server 2025 Express con la instancia `SQLEXPRESS` en el mismo equipo (VF-01).

Compilar y probar, con los mismos pasos que la CI:

```bash
dotnet build FarmasilApp.slnx --configuration Release
dotnet test FarmasilApp.slnx --configuration Release --no-build
```

Ejecutar la aplicación:

```bash
dotnet run --project src/FarmasilApp.Vista
```

### Cadena de conexión

La aplicación lee `ConnectionStrings:DB_FARMASIL` de `src/FarmasilApp.Vista/appsettings.json`:

```text
Server=.\SQLEXPRESS;Database=DB_FARMASIL;Integrated Security=true;TrustServerCertificate=true
```

- Se conecta con la cuenta de Windows de quien ejecuta la aplicación. Esta forma es provisional (PEN-035) y no lleva usuario ni contraseña: no se guardan credenciales en el repositorio.
- `TrustServerCertificate=true` es necesario porque el cliente cifra la conexión por defecto y la instancia local de Express no tiene un certificado de confianza.
- Para usar otra instancia o base, por ejemplo `DB_FARMASIL_PRUEBAS`, crea `src/FarmasilApp.Vista/appsettings.local.json` con la misma estructura. Ese archivo no se versiona (`*.local.json` está en `.gitignore`) y prevalece sobre `appsettings.json`.
- La base se crea con los scripts de `database/`, que escribe R2 (plan de implementación, sección 10).

### Datos de prueba

Al ejecutar `database/recrear.ps1` se cargan datos iniciales para pruebas locales.

| Usuario | Contraseña | Rol | Estado |
| --- | --- | --- | --- |
| `admin_prueba` | `Farmasil2026!` | Administrador | Activo |
| `tecnico_prueba` | `Farmasil2026!` | Tecnico | Activo |

Estas credenciales son exclusivamente para el entorno local de desarrollo y pruebas.
Las contraseñas se almacenan en `TBL_USUARIOS` como hashes PBKDF2-SHA256 con sal, nunca como texto plano.
