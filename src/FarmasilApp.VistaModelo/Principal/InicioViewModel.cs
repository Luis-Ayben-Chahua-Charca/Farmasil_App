using System;
using System.Diagnostics;
using System.IO;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.Principal;

/// <summary>
/// Pantalla de inicio con un ejemplo mínimo de generación PDF para el issue #84.
/// La VistaModelo depende del contrato IPdfServicio y no conoce PDFsharp/MigraDoc.
/// </summary>
public sealed class InicioViewModel : ViewModelBase
{
    private readonly IPdfServicio _pdfServicio;
    public const string MensajeError = "No se pudo generar el PDF. Inténtalo de nuevo o avisa al responsable del sistema.";

    private string _estadoPdf = "Pulsa el botón para generar un PDF demostrativo.";

    public InicioViewModel(IPdfServicio pdfServicio)
    {
        _pdfServicio = pdfServicio ?? throw new ArgumentNullException(nameof(pdfServicio));
        GenerarPdfCommand = new RelayCommand(GenerarPdfEjemplo);
    }

    public string Mensaje => "FARMASIL: ejemplo de integración PDFsharp + MigraDoc 6.2.4";

    public string EstadoPdf
    {
        get => _estadoPdf;
        private set => Establecer(ref _estadoPdf, value);
    }

    public RelayCommand GenerarPdfCommand { get; }

    private void GenerarPdfEjemplo()
    {
        try
        {
            var ruta = _pdfServicio.GenerarEjemplo();
            EstadoPdf = $"PDF generado correctamente: {Path.GetFileName(ruta)}. Ubicación: Documentos/FarmasilApp/PDF";
        }
        catch (Exception excepcion)
        {
            // DEC-072: el detalle técnico va al registro y a la usuaria se le muestra un mensaje genérico.
            Trace.TraceError("[PDF] GenerarEjemplo: {0}", excepcion);
            EstadoPdf = MensajeError;
        }
    }
}
