namespace FarmasilApp.Modelo.ALV;

/// <summary>Lectura del umbral activo y de los retiros históricos, propiedad del módulo ALV.</summary>
public interface IAlertasRepositorio
{
    int ObtenerUmbralMesesActivo();
    IReadOnlyList<AlertaConsulta> ConsultarRetiros();
}
