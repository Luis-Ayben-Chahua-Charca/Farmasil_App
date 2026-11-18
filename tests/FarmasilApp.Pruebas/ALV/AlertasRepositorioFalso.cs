using FarmasilApp.Modelo.ALV;

namespace FarmasilApp.Pruebas.ALV;

public sealed class AlertasRepositorioFalso : IAlertasRepositorio
{
    public int Umbral { get; set; } = 2;
    public List<AlertaConsulta> Retiros { get; } = new();

    public int ObtenerUmbralMesesActivo() => Umbral;
    public IReadOnlyList<AlertaConsulta> ConsultarRetiros() => Retiros.ToArray();
}
