namespace FarmasilApp.Modelo.ALV;

/// <summary>Resultado del proceso diario RNF-0010.1, sin retiros registrados ni filtros de pantalla.</summary>
public sealed class AvisoVencimientos : EventArgs
{
    public AvisoVencimientos(int cantidadRemesas, DateOnly fechaLimite, DateTimeOffset fechaEvaluacion)
    {
        CantidadRemesas = cantidadRemesas;
        FechaLimite = fechaLimite;
        FechaEvaluacion = fechaEvaluacion;
    }

    public int CantidadRemesas { get; }
    public DateOnly FechaLimite { get; }
    public DateTimeOffset FechaEvaluacion { get; }
}

/// <summary>Evento desacoplado entre la evaluación de fondo y quien presenta el aviso (DEC-021).</summary>
public sealed class CanalAvisosVencimientos
{
    public event EventHandler<AvisoVencimientos>? AvisoPublicado;

    public void Publicar(AvisoVencimientos aviso)
    {
        ArgumentNullException.ThrowIfNull(aviso);
        AvisoPublicado?.Invoke(this, aviso);
    }
}
