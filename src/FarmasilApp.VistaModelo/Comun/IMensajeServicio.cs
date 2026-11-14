namespace FarmasilApp.VistaModelo.Comun;

/// <summary>Presenta confirmaciones y mensajes de resultado desde la capa de presentación.</summary>
public interface IMensajeServicio
{
    bool Confirmar(string modulo, string titulo, string mensaje);

    void MostrarExito(string titulo, string mensaje);

    void MostrarError(string titulo, string mensaje);
}
