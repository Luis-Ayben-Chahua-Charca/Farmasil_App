using FarmasilApp.Modelo.ALV;

namespace FarmasilApp.Pruebas.ALV;

public sealed class AlertasRepositorioFalso : IAlertasRepositorio
{
    public int Umbral { get; set; } = 2;
    public List<AlertaConsulta> Retiros { get; } = new();

    public T EjecutarTransaccion<T>(Func<IAlertasUnidadTrabajo, T> operacion) => throw new NotSupportedException("El falso de ESP-0013 no simula escrituras.");

    public int ObtenerUmbralMesesActivo() => Umbral;
    public void ActualizarFechaUltimaVerificacion(DateTime fechaHoraLocal) { }
    public IReadOnlyList<AlertaConsulta> ConsultarRetiros() => Retiros.ToArray();
}
