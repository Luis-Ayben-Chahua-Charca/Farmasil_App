# Selección de biblioteca para generación de PDF

| Campo | Información |
|---|---|
| Proyecto | FARMASIL_APP |
| Decisión | DEC-078 |
| Módulos involucrados | DOC y REP |
| Fecha de aprobación | 17/11/2026 |
| Estado | **Aprobada** |
| Decidió | Leyni |
| Responsable de implementación | R6 — Documentos |

## 1. Contexto

Los módulos de Documentación Tributaria (DOC) y Reportes de Ventas (REP) requieren generar documentos PDF desde FarmasilApp. La biblioteca debe poder utilizarse desde C#/.NET 10 y WPF, y su licencia debe ser compatible con la distribución prevista del producto.

La comparación y la prueba de concepto inicial se realizaron con información ficticia. La integración aprobada se incorpora ahora a la solución oficial `FarmasilApp`: el paquete se fija en la capa WPF `FarmasilApp.Vista`, y el contrato del servicio se declara en `FarmasilApp.VistaModelo` para mantener la separación de capas.

## 2. Alternativas evaluadas

### Alternativa A: PDFsharp + MigraDoc

- Paquete seleccionado: `PDFsharp-MigraDoc-WPF`.
- Versión aprobada y fijada: `6.2.4`.
- Plataforma: Windows / WPF.
- Marco objetivo de la capa Vista: `net10.0-windows`.
- Licencia indicada por NuGet para 6.2.4: MIT.
- MigraDoc permite construir documentos estructurados con párrafos, tablas, imágenes y secciones.
- Fuentes: [NuGet — PDFsharp-MigraDoc-WPF 6.2.4](https://www.nuget.org/packages/PDFsharp-MigraDoc-WPF/6.2.4) y [documentación oficial](https://docs.pdfsharp.net/).

### Alternativa B: QuestPDF

- Lenguaje: C# / .NET.
- Capacidades relevantes: composición de documentos, tablas, imágenes, encabezados y pies de página.
- Licencia: Community License sujeta a condiciones de elegibilidad o licencia comercial.
- No se instaló ni se ejecutó una prueba con esta alternativa en esta PoC.
- Fuentes: [sitio oficial](https://www.questpdf.com/) y [Community License](https://www.questpdf.com/license/community.html).

### Alternativa C: iText Core

- Lenguaje: .NET / Java.
- Capacidades relevantes: creación y procesamiento de documentos PDF.
- Licencia: AGPL o comercial, según modalidad de distribución y uso.
- No se instaló ni se ejecutó una prueba con esta alternativa en esta PoC.
- Fuente: [iText Core](https://itextpdf.com/products/itext-core).

## 3. Comparación

| Criterio | PDFsharp + MigraDoc 6.2.4 | QuestPDF | iText Core |
|---|---|---|---|
| Uso desde WPF/.NET 10 | Paquete WPF específico; NuGet publica compatibilidad con `net10.0-windows` | Biblioteca .NET; integración en este proyecto no probada | Biblioteca .NET; integración en este proyecto no probada |
| Documentos con párrafos y tablas | Sí | Sí | Sí |
| Licencia revisada | MIT para 6.2.4 | Community License condicionada o comercial | AGPL o comercial |
| Prueba funcional de la PoC | Sí | No | No |
| Resultado de la PoC | PDF generado con datos ficticios | No evaluado funcionalmente | No evaluado funcionalmente |

La comparación funcional de QuestPDF e iText es documental; no equivale a una prueba técnica de esas alternativas en FarmasilApp.

## 4. Decisión aprobada

Se aprueba **PDFsharp + MigraDoc mediante el paquete `PDFsharp-MigraDoc-WPF`, versión `6.2.4`**, para la generación de PDF en FarmasilApp.

La aprobación tiene las siguientes condiciones obligatorias:

1. **Última versión MIT:** NuGet identifica la versión 6.2.4 como MIT. Esta es la última versión de esta línea publicada bajo MIT.
2. **Versión fijada:** `6.2.4` queda fijada explícitamente en `FarmasilApp.Vista.csproj`. No se permite actualización automática ni cambio de versión sin una nueva decisión registrada.
3. **Sin correcciones posteriores bajo MIT:** la versión 6.2.4 queda congelada; no recibirá correcciones nuevas bajo MIT. El proyecto acepta expresamente esa limitación de mantenimiento.
4. **Condición para actualizar:** desde la versión 6.5.0, NuGet identifica la licencia `AGPL-3.0-only`. Cualquier actualización posterior exige una nueva decisión y elegir una licencia comercial o cumplir íntegramente las obligaciones aplicables de AGPL-3.0-only.
5. **Avisos:** deben conservarse los avisos de copyright y licencia MIT que acompañen al paquete y sus redistribuciones.
6. **No actualizar por rutina:** herramientas de actualización de NuGet, actualizaciones masivas o recomendaciones de Visual Studio no autorizan por sí solas un cambio de versión.

La decisión fue tomada por **Leyni** y registrada como **DEC-078** en `seguimiento/11-registro-pendientes.md`.

## 5. Integración mínima en FarmasilApp

La solución oficial usa una separación de capas. La integración sigue ese patrón:

- `FarmasilApp.VistaModelo/Comun/IPdfServicio.cs`: contrato del servicio PDF.
- `FarmasilApp.Vista/Comun/PdfServicio.cs`: implementación WPF que construye el documento MigraDoc, lo renderiza y guarda el PDF.
- `FarmasilApp.Vista/App.xaml.cs`: registro de `IPdfServicio` con su implementación.
- `FarmasilApp.VistaModelo/Principal/InicioViewModel.cs` y `FarmasilApp.Vista/Principal/InicioView.xaml`: ejemplo mínimo ejecutable desde la pantalla inicial.
- `FarmasilApp.Vista/FarmasilApp.Vista.csproj`: referencia fijada a `PDFsharp-MigraDoc-WPF` 6.2.4.

El ejemplo genera un PDF demostrativo con datos ficticios y lo guarda en `Documentos/FarmasilApp/PDF`. No genera un comprobante tributario real ni implica integración con SUNAT/OSE o con datos de ventas.

## 6. Prueba y límites

La prueba de concepto independiente inicial demostró que PDFsharp + MigraDoc 6.2.4 podía generar un PDF desde WPF con datos ficticios. La revisión de esta decisión exige además compilar y ejecutar el ejemplo dentro de la solución oficial FarmasilApp.

Antes de aprobar el PR deben adjuntarse las evidencias obtenidas desde el entorno Windows del proyecto:

- resumen de compilación de `FarmasilApp.slnx`;
- pantalla de FarmasilApp ejecutando el ejemplo;
- PDF resultante abierto o visible.

También debe ejecutarse y guardarse la salida de:

```bash
dotnet list package --vulnerable --include-transitive
```

No se afirma que estas verificaciones se hayan ejecutado en esta entrega del archivo: deben realizarse en el entorno Windows con .NET 10 antes del PR.

Quedan fuera del alcance de este ejemplo:

- generación de comprobantes con ventas reales;
- conexión con la base de datos;
- integración con SUNAT u OSE;
- validación, firma, envío o autorización tributaria;
- integración funcional definitiva con DOC y REP.

## 7. Referencias

- NuGet — [PDFsharp-MigraDoc-WPF 6.2.4](https://www.nuget.org/packages/PDFsharp-MigraDoc-WPF/6.2.4), licencia MIT.
- NuGet — [PDFsharp-MigraDoc-WPF 6.5.0](https://www.nuget.org/packages/PDFsharp-MigraDoc-WPF/6.5.0), licencia AGPL-3.0-only.
- [Documentación oficial PDFsharp/MigraDoc](https://docs.pdfsharp.net/).
- [Política oficial de licencias de PDFsharp](https://docs.pdfsharp.net/LICENSE.html).
- Registro de decisiones: DEC-078.

## 8. Aprobación

| Rol | Estado |
|---|---|
| Comparación y prueba de concepto — R6 Documentos | Aprobadas |
| Revisión del responsable del proyecto | Aprobada por Leyni |
| Decisión tecnológica | **Aprobada — DEC-078** |
