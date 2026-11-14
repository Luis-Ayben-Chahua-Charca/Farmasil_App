# DEC-001 — Selección de biblioteca para generación de PDF

| Campo | Información |
|---|---|
| Código | DEC-001 |
| Proyecto | FARMASIL_APP |
| Módulos involucrados | DOC y REP |
| Fecha | 14/11/2026 |
| Estado | Propuesta para revisión |
| Responsable de propuesta | R6 - Documentos |

## 1. Contexto

Los módulos de Documentación Tributaria (DOC) y Reportes de Ventas (REP) requieren generar documentos PDF desde FarmasilApp.

Como el proyecto se encuentra en desarrollo, se necesita evaluar una biblioteca compatible con C#/.NET 10 y WPF, cuya licencia permita su utilización en un producto que pueda entregarse a un cliente.

La evaluación inicial se realizó mediante una prueba de concepto independiente, con información ficticia y sin conexión con la base de datos, ventas reales, SUNAT ni la lógica definitiva de DOC y REP. La prueba se implementó en el proyecto independiente `Farmasil.Interfaces`; todavía no está integrada en la solución oficial `FarmasilApp` del repositorio.

## 2. Alternativas evaluadas

### Alternativa A: PDFsharp + MigraDoc

- Lenguaje: C# / .NET.
- Paquete probado: `PDFsharp-MigraDoc-WPF`.
- Versión probada: `6.2.4`.
- Plataforma: Windows / WPF.
- Compatibilidad publicada: incluye `net10.0-windows`.
- Licencia indicada en NuGet para la versión 6.2.4: MIT.
- Capacidades relevantes: documentos con párrafos, tablas, imágenes y secciones.
- Sitios oficiales:
  - https://docs.pdfsharp.net/
  - https://www.nuget.org/packages/PDFsharp-MigraDoc-WPF/6.2.4

### Alternativa B: QuestPDF

- Lenguaje: C# / .NET.
- Capacidades relevantes: composición de documentos, tablas, imágenes, encabezados y pies de página.
- Licencia: Community License o licencia comercial, según la elegibilidad y condiciones aplicables.
- No se instaló ni se ejecutó una prueba con esta alternativa en esta PoC.
- Sitio oficial: https://www.questpdf.com/
- Condiciones de la Community License: https://www.questpdf.com/license/community.html

### Alternativa C: iText Core

- Lenguaje: .NET / Java.
- Capacidades relevantes: creación y procesamiento de documentos PDF.
- Licencia: AGPL o comercial, según modalidad de distribución y uso.
- No se instaló ni se ejecutó una prueba con esta alternativa en esta PoC.
- Sitio oficial: https://itextpdf.com/products/itext-core

## 3. Comparación

| Criterio | PDFsharp + MigraDoc 6.2.4 | QuestPDF | iText Core |
|---|---|---|---|
| Uso desde una aplicación WPF/.NET | Paquete WPF específico; compatibilidad publicada con `net10.0-windows` | Biblioteca .NET; integración en este proyecto no probada | Biblioteca .NET disponible; integración en este proyecto no probada |
| Documentos con tablas y párrafos | Sí | Sí | Sí |
| Licencia revisada | MIT para la versión 6.2.4 | Community License condicionada o comercial | AGPL o comercial |
| Prueba en esta PoC | Sí | No | No |
| Resultado observado en esta PoC | PDF generado y abierto correctamente | No evaluado funcionalmente | No evaluado funcionalmente |

La comparación funcional de QuestPDF e iText es documental; no equivale a una prueba técnica de esas alternativas en FarmasilApp.

## 4. Decisión propuesta

Se propone seleccionar **PDFsharp + MigraDoc, versión 6.2.4**, como biblioteca candidata para la generación de documentos PDF en FarmasilApp, sujeta a revisión y aprobación del responsable del proyecto.

La propuesta se fundamenta en que:

1. El paquete `PDFsharp-MigraDoc-WPF` publica compatibilidad con `net10.0-windows`, marco objetivo utilizado por la capa WPF del proyecto.
2. NuGet identifica la licencia de la versión 6.2.4 como MIT.
3. MigraDoc permite estructurar documentos con párrafos, tablas, imágenes y secciones, necesidades habituales para documentos de DOC y REP.
4. La biblioteca se instaló mediante NuGet en el proyecto independiente `Farmasil.Interfaces`.
5. La prueba de concepto generó y abrió correctamente un PDF con datos ficticios.

Esta propuesta no constituye todavía la aprobación definitiva para la solución oficial ni una autorización legal del producto final. Antes de distribuir el producto, el equipo deberá conservar los avisos de licencia y revisar las obligaciones aplicables a las dependencias incluidas.

## 5. Evidencia de la prueba de concepto

La demostración se implementó en el proyecto independiente `Farmasil.Interfaces`, dentro de la carpeta `PruebaPDF`.

Archivos implementados:

- `GeneradorPdfPoC.cs`
- `PruebaPdfWindow.xaml`
- `PruebaPdfWindow.xaml.cs`

La ventana de prueba genera el archivo `Prueba-FarmasilApp.pdf` con datos ficticios:

- Número de prueba: `TEST-001`.
- Producto: Paracetamol 500 mg.
- Cantidad: 2 unidades.
- Precio unitario: S/ 5.00.
- Importe total: S/ 10.00.

**Resultado observado:** el PDF se generó, se guardó en la carpeta de documentos y se abrió automáticamente. La compilación del proyecto de prueba terminó con cero errores.

La prueba utiliza información ficticia y no representa un comprobante tributario válido. Tampoco demuestra integración con la base de datos, las ventas reales, SUNAT/OSE ni con la solución oficial `FarmasilApp.Vista`.

Las capturas de instalación, compilación y resultado deben adjuntarse al informe de evidencias de la PoC o a la incidencia correspondiente.

## 6. Consecuencias de la decisión

### Positivas

- Se cuenta con una biblioteca candidata que tiene un paquete específico para WPF y compatibilidad publicada con .NET 10 para Windows.
- Permite construir documentos con contenido estructurado, como párrafos y tablas.
- Se dispone de una prueba de concepto funcional inicial.
- La versión evaluada figura con licencia MIT en NuGet.

### Consideraciones y restricciones

- La versión evaluada debe mantenerse identificada como `6.2.4` mientras esta DEC esté vigente.
- No se debe actualizar a otra versión sin volver a revisar su licencia y compatibilidad. En particular, no se debe asumir que las condiciones de la versión 6.2.4 se aplican a versiones posteriores.
- Deben conservarse los avisos de copyright y licencia exigidos por MIT al redistribuir el software.
- La prueba actual está en `Farmasil.Interfaces`, un proyecto independiente; todavía debe decidirse si se traslada o se replica la prueba en la solución oficial `FarmasilApp`.
- La integración futura con DOC y REP, la generación a partir de datos reales y la validación de documentos tributarios quedan fuera del alcance de esta PoC.
- La compatibilidad de la biblioteca no sustituye la revisión de las licencias y avisos de todas las dependencias transitivas.

## 7. Alcance y limitaciones

Esta DEC documenta una selección candidata y una prueba de concepto inicial. No declara terminada la integración de generación de PDF en DOC o REP.

Quedan fuera del alcance:

- Conexión con la base de datos.
- Generación de comprobantes a partir de ventas reales.
- Integración con SUNAT u OSE.
- Integración definitiva con los módulos DOC y REP.
- Validación tributaria, firma, envío o autorización de documentos.
- Pruebas de aceptación sobre la solución oficial completa.

## 8. Referencias

- NuGet — PDFsharp-MigraDoc-WPF 6.2.4: https://www.nuget.org/packages/PDFsharp-MigraDoc-WPF/6.2.4
- Documentación oficial PDFsharp/MigraDoc: https://docs.pdfsharp.net/
- QuestPDF — Community License: https://www.questpdf.com/license/community.html
- iText — iText Core y licenciamiento: https://itextpdf.com/products/itext-core

## 9. Aprobación

| Rol | Estado |
|---|---|
| Propuesta técnica — R6 Documentos | Elaborada |
| Revisión del responsable del proyecto | Pendiente |
| Aprobación de la decisión | Pendiente |

La presente DEC queda como **propuesta técnica pendiente de revisión y aprobación**. La selección solo deberá considerarse definitiva después de dicha validación.
