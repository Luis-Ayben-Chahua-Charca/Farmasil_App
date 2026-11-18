using System.IO;
using FarmasilApp.VistaModelo.Comun;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;

namespace FarmasilApp.Vista.Comun;

/// <summary>Implementación WPF de IPdfServicio con PDFsharp + MigraDoc 6.2.4 (DEC-078).</summary>
public sealed class PdfServicio : IPdfServicio
{
    public string GenerarEjemplo()
    {
        var carpeta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "FarmasilApp",
            "PDF");
        Directory.CreateDirectory(carpeta);

        var nombre = $"Ejemplo-PDF-FarmasilApp-{DateTime.Now:yyyyMMdd-HHmmss}.pdf";
        var ruta = Path.Combine(carpeta, nombre);

        var documento = new Document();
        documento.Info.Title = "Ejemplo de generación PDF — FarmasilApp";
        documento.Info.Subject = "Prueba técnica de PDFsharp + MigraDoc 6.2.4";
        documento.Info.Author = "FarmasilApp";

        // El acceso al estilo puede devolver null según las anotaciones de la biblioteca.
        var estiloNormal = documento.Styles[StyleNames.Normal];
        if (estiloNormal is not null)
        {
            estiloNormal.Font.Name = "Arial";
            estiloNormal.Font.Size = 10;
        }

        var seccion = documento.AddSection();
        seccion.PageSetup.TopMargin = Unit.FromCentimeter(2);
        seccion.PageSetup.BottomMargin = Unit.FromCentimeter(2);

        var titulo = seccion.AddParagraph("FARMASILAPP");
        titulo.Format.Font.Size = 20;
        titulo.Format.Font.Bold = true;
        titulo.Format.Alignment = ParagraphAlignment.Center;
        titulo.Format.SpaceAfter = Unit.FromCentimeter(0.4);

        var subtitulo = seccion.AddParagraph("Ejemplo de generación de PDF");
        subtitulo.Format.Alignment = ParagraphAlignment.Center;
        subtitulo.Format.SpaceAfter = Unit.FromCentimeter(0.8);

        var aviso = seccion.AddParagraph("Documento demostrativo — datos ficticios");
        aviso.Format.Font.Bold = true;
        aviso.Format.SpaceAfter = Unit.FromCentimeter(0.5);

        var tabla = seccion.AddTable();
        tabla.Borders.Width = 0.5;
        tabla.AddColumn(Unit.FromCentimeter(5));
        tabla.AddColumn(Unit.FromCentimeter(10));

        AgregarFila(tabla, "Campo", "Valor", encabezado: true);
        AgregarFila(tabla, "Aplicación", "FarmasilApp");
        AgregarFila(tabla, "Biblioteca", "PDFsharp + MigraDoc");
        AgregarFila(tabla, "Versión", "6.2.4 (MIT; fijada por DEC-078)");
        AgregarFila(tabla, "Propósito", "Demostrar la generación de un archivo PDF desde WPF");

        seccion.AddParagraph().Format.SpaceAfter = Unit.FromCentimeter(0.4);
        var pie = seccion.AddParagraph("Esta salida es una prueba técnica. No es un comprobante tributario.");
        pie.Format.Font.Italic = true;
        pie.Format.Font.Size = 8;

        var renderizador = new PdfDocumentRenderer
        {
            Document = documento
        };
        renderizador.RenderDocument();
        renderizador.PdfDocument.Save(ruta);

        return ruta;
    }

    private static void AgregarFila(Table tabla, string campo, string valor, bool encabezado = false)
    {
        var fila = tabla.AddRow();
        if (encabezado)
        {
            fila.Format.Font.Bold = true;
            fila.Shading.Color = Colors.LightGray;
        }

        fila.Cells[0].AddParagraph(campo);
        fila.Cells[1].AddParagraph(valor);
    }
}
