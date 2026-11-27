namespace FarmasilApp.Modelo.ALV;

/// <summary>Acceso a las tablas propiedad de ALV; el cambio de estado de la remesa se hace solo por ILotes.</summary>
public interface IAlertasRepositorio
{
    int ObtenerUmbralMesesActivo();

    /// <summary>RNF-0010.1: marca la regla activa solo tras completar la evaluación con éxito.</summary>
    void ActualizarFechaUltimaVerificacion(DateTime fechaHoraLocal);
    IReadOnlyList<AlertaConsulta> ConsultarRetiros();

    /// <summary>La implementación SQL confirma o revierte todas las escrituras ALV de este delegado.</summary>
    T EjecutarTransaccion<T>(Func<IAlertasUnidadTrabajo, T> operacion);
}
