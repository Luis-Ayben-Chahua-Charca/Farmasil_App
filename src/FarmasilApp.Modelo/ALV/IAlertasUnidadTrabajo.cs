namespace FarmasilApp.Modelo.ALV;

/// <summary>Operaciones ALV que comparten UNA transacción SQL, sin escribir tablas de LOT ni DEV.</summary>
public interface IAlertasUnidadTrabajo
{
    AlertaRetiro? ObtenerRetiro(int idRetiro);
    int Insertar(NuevaAlertaRetiro retiro);
    bool Actualizar(int idRetiro, bool esDefectuoso, string? codigoDigemid);
    bool Eliminar(int idRetiro);
    int ContarRetiros(int idLote);
}
