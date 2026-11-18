# Evidencia de integración PDFsharp + MigraDoc en FarmasilApp

| Campo | Información |
|---|---|
| Issue | #84 — Prueba de concepto de generación de PDF |
| Decisión | DEC-078 |
| Paquete | `PDFsharp-MigraDoc-WPF` 6.2.4 (MIT, versión fijada) |
| Proyecto | `src/FarmasilApp.Vista/FarmasilApp.Vista.csproj` (`net10.0-windows`) |
| Contrato | `src/FarmasilApp.VistaModelo/Comun/IPdfServicio.cs` |
| Implementación | `src/FarmasilApp.Vista/Comun/PdfServicio.cs` |
| Ejemplo de uso | `InicioViewModel` / `InicioView` |

## 1. Propósito

Demostrar que la solución oficial `FarmasilApp` puede generar un archivo PDF desde su aplicación WPF mediante PDFsharp + MigraDoc 6.2.4. La demostración se ejecuta desde la pantalla inicial y utiliza datos descriptivos ficticios; no genera un comprobante tributario real.

## 2. Capturas que deben adjuntarse

Guardar las imágenes en esta misma carpeta, con los siguientes nombres. Las capturas se deben obtener después de ejecutar la solución integrada `FarmasilApp.slnx`, no desde el proyecto independiente `Farmasil.Interfaces`.

| Archivo | Evidencia exigida | Cuidado de privacidad |
|---|---|---|
| `01-resumen-compilacion.png` | Resumen de compilación de `FarmasilApp.slnx`, con número de proyectos, errores y advertencias visibles. | Recortar la ruta local completa y cualquier `C:\Users\...` o nombre de usuario. |
| `02-ejecucion-farmasilapp.png` | Pantalla inicial de FarmasilApp con el botón «Generar PDF de ejemplo» y el mensaje de generación correcta. | No mostrar rutas personales; se puede recortar el diálogo para que solo aparezca el nombre del archivo. |
| `03-pdf-generado.png` | PDF generado y abierto, donde se vea el título, la versión 6.2.4 y la indicación de que los datos son ficticios. | No incluir la ruta de Documentos ni el nombre de usuario de Windows. |

**No reutilizar como evidencia de integración las capturas antiguas de `Farmasil.Interfaces`:** esas capturas documentan la PoC independiente previa, no la compilación ni la ejecución dentro de `FarmasilApp` que exige la revisión actual.

## 3. Comandos de verificación

Ejecutar desde la raíz del repositorio en Windows con .NET 10 instalado:

```powershell
dotnet restore FarmasilApp.slnx
dotnet build FarmasilApp.slnx --no-restore
dotnet list package --vulnerable --include-transitive
```

Adjuntar la salida real del build y de la auditoría de vulnerabilidades al PR. No marcar como aprobada la verificación de vulnerabilidades si el comando reporta paquetes vulnerables; revisar cada resultado y resolverlo o documentar la excepción con el responsable.

## 4. Resultado esperado del ejemplo

Al pulsar «Generar PDF de ejemplo», el servicio crea un archivo con nombre `Ejemplo-PDF-FarmasilApp-AAAAmmdd-HHmmss.pdf` en `Documentos/FarmasilApp/PDF`. `InicioViewModel` muestra en pantalla el nombre del archivo generado. Si la generación falla, muestra un mensaje genérico y el detalle va al registro (DEC-072).

## 5. Alcance y limitaciones

Esta evidencia solo demuestra la generación técnica de un PDF desde la aplicación WPF oficial. No demuestra conexión con la base de datos, generación a partir de ventas reales, emisión tributaria, firma digital ni envío a SUNAT/OSE. Esos trabajos permanecen fuera del issue #84.
