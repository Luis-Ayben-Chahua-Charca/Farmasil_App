namespace FarmasilApp.Modelo.ALV;

/// <summary>Textos literales de ESP-0014/0015/0016 (DEC-072).</summary>
public static class MensajesAlertas
{
    public const string Titulo = "Alertas de vencimiento";
    public const string Registrada = "Alerta registrada correctamente";
    public const string Actualizada = "Alerta actualizada correctamente";
    public const string ConfirmarEliminacion = "¿Está seguro de eliminar esta alerta? La remesa solo volverá a estar disponible si no quedan otros retiros, no está vencida y tiene existencias.";
    public const string DevolucionPendiente = "La remesa forma parte de una orden de devolución pendiente";
    public const string RemesaVencida = "La remesa está vencida y no puede volver a la venta. Gestione su devolución o su baja";
    public const string EliminadaConOtrosRetiros = "Alerta eliminada. La remesa sigue bloqueada porque tiene otra alerta vigente";
    public const string Eliminada = "Alerta eliminada correctamente";
    public const string SoloAdministrador = "Solo disponible para el Administrador.";
    public const string RemesaObligatoria = "Seleccione una remesa válida.";
    public const string RetiroObligatorio = "Seleccione una alerta registrada.";
    public const string CodigoDigemidLargo = "El código de alerta DIGEMID no puede superar los 100 caracteres.";
    public const string ErrorTecnico = "No se pudo completar la operación. Inténtelo nuevamente.";
}
