using FarmasilApp.VistaModelo.Comun;
using FarmasilApp.Modelo.DOC;

namespace FarmasilApp.Pruebas.VistaModelo;

/// <summary>
/// <see cref="IPdfServicio"/> de prueba: no escribe ningún archivo. Devuelve <see cref="RutaGenerada"/> o lanza
/// <see cref="Excepcion"/> si se la asigna. Sirve para probar cualquier VistaModelo que genere PDF.
/// </summary>
internal sealed class PdfServicioFalso : IPdfServicio
{
    public string RutaGenerada { get; set; } = @"C:\Pruebas\Ejemplo-PDF-FarmasilApp.pdf";

    public Exception? Excepcion { get; set; }

    public int Llamadas { get; private set; }

    public DatosComprobantePdf? UltimosDatosComprobante { get; private set; }

    public string GenerarComprobanteBorrador(DatosComprobantePdf datos)
    {
        Llamadas++;
        UltimosDatosComprobante = datos ?? throw new ArgumentNullException(nameof(datos));
        if (Excepcion is not null)
        {
            throw Excepcion;
        }

        return RutaGenerada;
    }

    public string GenerarEjemplo()
    {
        Llamadas++;
        if (Excepcion is not null)
        {
            throw Excepcion;
        }

        return RutaGenerada;
    }
}
