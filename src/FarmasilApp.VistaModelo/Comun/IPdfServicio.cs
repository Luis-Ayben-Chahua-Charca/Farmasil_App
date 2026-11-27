using FarmasilApp.Modelo.DOC;

namespace FarmasilApp.VistaModelo.Comun;

/// <summary>Contrato para generar documentos PDF sin acoplar la VistaModelo a PDFsharp/MigraDoc.</summary>
public interface IPdfServicio
{
    /// <summary>Genera el PDF demostrativo legado del issue #84.</summary>
    string GenerarEjemplo();

    /// <summary>Genera un borrador PDF con los datos de la previsualización DOC.</summary>
    string GenerarComprobanteBorrador(DatosComprobantePdf datos);
}
