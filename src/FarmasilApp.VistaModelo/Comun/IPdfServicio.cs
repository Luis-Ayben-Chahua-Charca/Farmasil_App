namespace FarmasilApp.VistaModelo.Comun;

/// <summary>Contrato para generar documentos PDF sin que la VistaModelo dependa de PDFsharp/MigraDoc.</summary>
public interface IPdfServicio
{
    /// <summary>Genera un PDF demostrativo y devuelve la ruta completa del archivo creado.</summary>
    string GenerarEjemplo();
}
