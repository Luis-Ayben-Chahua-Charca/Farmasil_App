# Evidencia de prueba de concepto: generación de PDF

| Campo | Información |
|---|---|
| Proyecto | FARMASIL_APP |
| Incidencia | #84 — Prueba de concepto de generación de PDF |
| Módulos relacionados | DOC y REP |
| Fecha de la prueba | 14/11/2026 |
| Biblioteca probada | PDFsharp + MigraDoc para WPF |
| Versión instalada | 6.2.4 |
| Proyecto utilizado para la PoC | `Farmasil.Interfaces` (aplicación WPF independiente de prueba) |

## 1. Objetivo

Comprobar que una aplicación WPF desarrollada en C# puede generar un archivo PDF mediante PDFsharp + MigraDoc. La prueba utiliza datos ficticios y se realiza de forma independiente, sin conexión a la base de datos ni a la lógica de los módulos DOC y REP.

## 2. Instalación de la biblioteca

Se instaló mediante NuGet el paquete `PDFsharp-MigraDoc-WPF`, versión `6.2.4`. La instalación finalizó correctamente y NuGet confirmó la instalación de `PDFsharp-MigraDoc-WPF 6.2.4` y `PDFsharp-WPF 6.2.4`, además de sus dependencias.

![Paquete PDFsharp-MigraDoc-WPF 6.2.4 instalado mediante NuGet]
![alt text](<Evidencia doc 02.png>)

## 3. Implementación de la prueba

Se creó una demostración WPF independiente con los siguientes archivos:

- `GeneradorPdfPoC.cs`: construye y guarda el documento PDF.
- `PruebaPdfWindow.xaml`: presenta los datos ficticios y el botón para generar el PDF.
- `PruebaPdfWindow.xaml.cs`: ejecuta el generador y abre el archivo resultante.

## 4. Compilación

La solución de prueba compiló correctamente. La salida de Visual Studio mostró un proyecto compilado, cero errores y cero advertencias.
![alt text](<Evidencia doc 01.png>)

## 5. Resultado obtenido

Al pulsar el botón **Generar PDF**, la aplicación creó y abrió el archivo `Prueba-FarmasilApp.pdf`.

El documento de prueba contiene estos datos ficticios:

| Campo | Valor |
|---|---|
| Número de prueba | TEST-001 |
| Producto | Paracetamol 500 mg |
| Cantidad | 2 |
| Precio unitario | S/ 5.00 |
| Importe total | S/ 10.00 |

**Resultado observado:** el PDF se generó, se guardó en la carpeta `Documentos/FarmasilApp_PDF` y se abrió automáticamente.

![alt text](<Evidencia doc 03.png>)


## 6. Alcance y limitaciones

Esta evidencia demuestra únicamente la generación técnica de un PDF desde una aplicación WPF usando información ficticia. No demuestra todavía:

- integración con la solución oficial `FarmasilApp.Vista` del repositorio;
- conexión con la base de datos;
- generación de comprobantes a partir de ventas reales;
- integración con SUNAT u OSE;
- integración definitiva con los módulos DOC y REP;
- aprobación final de la biblioteca para el producto entregado al cliente.

La prueba se realizó en el proyecto independiente `Farmasil.Interfaces`. Su incorporación a la solución oficial y la aprobación de la decisión tecnológica quedan sujetas a la revisión del equipo.

## 7. Conclusión

La prueba de concepto fue satisfactoria: PDFsharp + MigraDoc `6.2.4` permitió generar y abrir un documento PDF desde una ventana WPF con datos ficticios. Se adjuntan las capturas de instalación, compilación y resultado como evidencia para la revisión de la incidencia #84.
