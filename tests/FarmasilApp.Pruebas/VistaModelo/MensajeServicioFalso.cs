using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.Pruebas.VistaModelo;

/// <summary>
/// <see cref="IMensajeServicio"/> de prueba: no abre ventanas, anota lo que se mostró y responde a la confirmación con
/// <see cref="RespuestaConfirmacion"/>. Sirve para probar cualquier VistaModelo que confirme una baja o muestre mensajes.
/// </summary>
internal sealed class MensajeServicioFalso : IMensajeServicio
{
    /// <summary>Lo que responde la dueña en el modal: <c>true</c> es USR-BTN-CONFIRMAR-SI.</summary>
    public bool RespuestaConfirmacion { get; set; } = true;

    public List<(string Modulo, string? Titulo, string Mensaje)> Confirmaciones { get; } = new();

    public List<(string Titulo, string Mensaje)> Exitos { get; } = new();

    public List<(string Titulo, string Mensaje)> Errores { get; } = new();

    public bool Confirmar(string modulo, string? titulo, string mensaje)
    {
        Confirmaciones.Add((modulo, titulo, mensaje));
        return RespuestaConfirmacion;
    }

    public void MostrarExito(string titulo, string mensaje) => Exitos.Add((titulo, mensaje));

    public void MostrarError(string titulo, string mensaje) => Errores.Add((titulo, mensaje));
}
