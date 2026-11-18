namespace FarmasilApp.VistaModelo.Comun;

/// <summary>Presenta confirmaciones y mensajes de resultado desde la capa de presentación.</summary>
public interface IMensajeServicio
{
    /// <summary>
    /// Muestra el modal de confirmación de baja del módulo con el mensaje de su especificación.
    /// Si <paramref name="titulo"/> es nulo o vacío, usa el título del mockup ART-MKP-CONFIRM-0001.
    /// </summary>
    bool Confirmar(string modulo, string? titulo, string mensaje);

    void MostrarExito(string titulo, string mensaje);

    void MostrarError(string titulo, string mensaje);
}
