using System.IO;
using System.Globalization;
using FarmasilApp.Modelo.DOC;
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
        var documento = CrearDocumentoBase(
            "Ejemplo de generación PDF — FarmasilApp",
            "Prueba técnica de PDFsharp + MigraDoc 6.2.4");
        var seccion = documento.AddSection();
        seccion.PageSetup.TopMargin = Unit.FromCentimeter(2);
        seccion.PageSetup.BottomMargin = Unit.FromCentimeter(2);

        AgregarTitulo(seccion, "Ejemplo de generación de PDF");
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

        return Guardar(documento, $"Ejemplo-PDF-FarmasilApp-{DateTime.Now:yyyyMMdd-HHmmss}.pdf");
    }

    public string GenerarComprobanteBorrador(DatosComprobantePdf datos)
    {
        ArgumentNullException.ThrowIfNull(datos);
        ArgumentException.ThrowIfNullOrWhiteSpace(datos.TipoComprobante);
        ArgumentException.ThrowIfNullOrWhiteSpace(datos.NumeroComprobante);
        ArgumentException.ThrowIfNullOrWhiteSpace(datos.CodigoVenta);
        ArgumentNullException.ThrowIfNull(datos.Detalle);

        var documento = CrearDocumentoBase(
            $"Borrador {datos.TipoComprobante} {datos.NumeroComprobante}",
            "Previsualización interna de Documentación Tributaria de FarmasilApp");
        var seccion = documento.AddSection();
        seccion.PageSetup.TopMargin = Unit.FromCentimeter(1.6);
        seccion.PageSetup.BottomMargin = Unit.FromCentimeter(1.6);

        AgregarTitulo(seccion, "FARMASILAPP");
        var tipo = seccion.AddParagraph($"BORRADOR DE {datos.TipoComprobante.ToUpperInvariant()}");
        tipo.Format.Font.Size = 15;
        tipo.Format.Font.Bold = true;
        tipo.Format.Alignment = ParagraphAlignment.Center;
        tipo.Format.SpaceAfter = Unit.FromCentimeter(0.25);

        var advertencia = seccion.AddParagraph("BORRADOR — SIN VALIDEZ TRIBUTARIA");
        advertencia.Format.Font.Bold = true;
        advertencia.Format.Font.Color = Colors.DarkRed;
        advertencia.Format.Alignment = ParagraphAlignment.Center;
        advertencia.Format.SpaceAfter = Unit.FromCentimeter(0.5);

        var cabecera = seccion.AddTable();
        cabecera.Borders.Width = 0.5;
        cabecera.AddColumn(Unit.FromCentimeter(4.2));
        cabecera.AddColumn(Unit.FromCentimeter(12.0));
        AgregarFila(cabecera, "Número", datos.NumeroComprobante, encabezado: true);
        AgregarFila(cabecera, "Fecha de emisión", datos.FechaEmision.ToString("dd/MM/yyyy"));
        AgregarFila(cabecera, "Venta asociada", datos.CodigoVenta, encabezado: true);
        AgregarFila(cabecera, "Fecha de venta", datos.FechaVenta.ToString("dd/MM/yyyy HH:mm"));

        seccion.AddParagraph().Format.SpaceAfter = Unit.FromCentimeter(0.35);
        var tituloDetalle = seccion.AddParagraph("Detalle de productos");
        tituloDetalle.Format.Font.Bold = true;
        tituloDetalle.Format.SpaceAfter = Unit.FromCentimeter(0.2);

        var tabla = seccion.AddTable();
        tabla.Borders.Width = 0.5;
        tabla.AddColumn(Unit.FromCentimeter(5.4));
        tabla.AddColumn(Unit.FromCentimeter(2.5));
        tabla.AddColumn(Unit.FromCentimeter(1.8));
        tabla.AddColumn(Unit.FromCentimeter(2.8));
        tabla.AddColumn(Unit.FromCentimeter(3.0));

        var encabezado = tabla.AddRow();
        encabezado.HeadingFormat = true;
        encabezado.Format.Font.Bold = true;
        encabezado.Shading.Color = Colors.LightGray;
        encabezado.Cells[0].AddParagraph("Producto");
        encabezado.Cells[1].AddParagraph("Lote");
        encabezado.Cells[2].AddParagraph("Cantidad");
        encabezado.Cells[3].AddParagraph("Precio unitario");
        encabezado.Cells[4].AddParagraph("Subtotal");

        foreach (var linea in datos.Detalle)
        {
            var fila = tabla.AddRow();
            fila.Cells[0].AddParagraph(linea.NombreProducto);
            fila.Cells[1].AddParagraph(linea.NumeroLote);
            fila.Cells[2].AddParagraph(linea.Cantidad.ToString());
            var cultura = CultureInfo.GetCultureInfo("es-PE");
            fila.Cells[3].AddParagraph(linea.PrecioUnitario.ToString("C2", cultura));
            fila.Cells[4].AddParagraph(linea.Subtotal.ToString("C2", cultura));
        }

        seccion.AddParagraph().Format.SpaceAfter = Unit.FromCentimeter(0.25);
        var total = seccion.AddParagraph(
            $"TOTAL DE LA VENTA: {datos.MontoTotal.ToString("C2", CultureInfo.GetCultureInfo("es-PE"))}");
        total.Format.Font.Bold = true;
        total.Format.Font.Size = 13;
        total.Format.Alignment = ParagraphAlignment.Right;
        total.Format.SpaceBefore = Unit.FromCentimeter(0.2);

        var nota = seccion.AddParagraph(
            "Este archivo es únicamente una vista previa generada con los datos disponibles en FarmasilApp. " +
            "No contiene datos fiscales no proporcionados, firma digital, código QR ni autorización de SUNAT/OSE.");
        nota.Format.Font.Size = 8;
        nota.Format.Font.Italic = true;
        nota.Format.SpaceBefore = Unit.FromCentimeter(0.8);

        var nombreSeguro = string.Concat(datos.NumeroComprobante.Select(caracter =>
            Path.GetInvalidFileNameChars().Contains(caracter) ? '_' : caracter));
        var nombre = $"Borrador-{nombreSeguro}-{DateTime.Now:yyyyMMdd-HHmmss-fff}.pdf";
        return Guardar(documento, nombre);
    }

    private static Document CrearDocumentoBase(string titulo, string asunto)
    {
        var documento = new Document();
        documento.Info.Title = titulo;
        documento.Info.Subject = asunto;
        documento.Info.Author = "FarmasilApp";

        var estiloNormal = documento.Styles[StyleNames.Normal];
        if (estiloNormal is not null)
        {
            estiloNormal.Font.Name = "Arial";
            estiloNormal.Font.Size = 9;
        }

        return documento;
    }

    private static void AgregarTitulo(Section seccion, string texto)
    {
        var titulo = seccion.AddParagraph(texto);
        titulo.Format.Font.Size = 20;
        titulo.Format.Font.Bold = true;
        titulo.Format.Alignment = ParagraphAlignment.Center;
        titulo.Format.SpaceAfter = Unit.FromCentimeter(0.35);
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

    private static string Guardar(Document documento, string nombre)
    {
        var carpeta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "FarmasilApp",
            "PDF");
        Directory.CreateDirectory(carpeta);

        var ruta = Path.Combine(carpeta, nombre);
        var renderizador = new PdfDocumentRenderer { Document = documento };
        renderizador.RenderDocument();
        renderizador.PdfDocument.Save(ruta);
        return ruta;
    }
}
