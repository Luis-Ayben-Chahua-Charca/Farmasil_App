# Catálogo de Requisitos — FARMASIL

Sistema de gestión para farmacia. Universidad Nacional de San Agustín, Ingeniería de Requisitos.

## Contenido

| Archivo | Versión | Descripción |
| --- | --- | --- |
| `01-educciones.md` | v05.00 y posteriores | 11 educciones. Requisitos tal como los expresó el cliente. |
| `02-ilaciones.md` | — | 44 ilaciones. Cuatro fases CRUD por educción. |
| `03-especificaciones.md` | — | 44 especificaciones en pseudocódigo. |
| `04-trazabilidad.md` | v02.00 | Matriz EDU↔ILA↔ESP↔artefactos, más trazabilidad hacia RNF, modelo de datos y pruebas. |
| `05-diccionario-datos.md` | v04.00 | 14 tablas de `DB_FARMASIL`, correspondencia de tipos y relaciones. |
| `06-guia-nomenclatura-mockups.md` | v04.00 | Prefijos por módulo, tipos de componente e inventario completo. |
| `07-matriz-permisos.md` | v02.00 | Actores, roles y permisos por operación. |
| `08-requisitos-no-funcionales.md` | — | 9 requisitos no funcionales. RNF-0006 v03.00 refundido en seguridad. |
| `09-pruebas-software.md` | v03.00 | 223 pruebas sobre las 44 especificaciones y los 9 requisitos no funcionales. |

## Estructura del catálogo

11 módulos, cada uno con una educción, cuatro ilaciones y cuatro especificaciones.

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

Los códigos EDU-0005 a EDU-0008 están reservados y no se reasignan: corresponden a requisitos reclasificados como no funcionales. El salto de numeración es intencional.

## Convenciones

- Componentes de interfaz: guion y mayúsculas (`VEN-BTN-CREAR-VENTA`).
- Tablas y campos de base de datos: guion bajo, con punto separador (`DB_FARMASIL.TBL_PRODUCTOS`).
- Versión `DD.DD`, fecha `dd/mm/aaaa`, ningún campo vacío (`Ninguno` cuando no aplica).
- Toda operación que escriba en más de una tabla se ejecuta dentro de una transacción.
