using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.LOT;

namespace FarmasilApp.Modelo.ALV;

/// <summary>
/// RNF-0010.1: usa el mismo contrato y predicado de ESP-0013; solo escribe
/// fecha_ultima_verif en la regla de ALV y publica un evento cuando ha terminado.
/// No inserta retiros ni modifica existencias o estados de lote.
/// </summary>
public sealed class EvaluacionVencimientosServicio
{
    private readonly IAlertasRepositorio _repositorio;
    private readonly ILotes _lotes;
    private readonly TimeProvider _reloj;
    private readonly CanalAvisosVencimientos _avisos;

    public EvaluacionVencimientosServicio(IAlertasRepositorio repositorio, ILotes lotes,
        TimeProvider reloj, CanalAvisosVencimientos avisos)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        _lotes = lotes ?? throw new ArgumentNullException(nameof(lotes));
        _reloj = reloj ?? throw new ArgumentNullException(nameof(reloj));
        _avisos = avisos ?? throw new ArgumentNullException(nameof(avisos));
    }

    public AvisoVencimientos Evaluar()
    {
        var umbral = _repositorio.ObtenerUmbralMesesActivo();
        if (umbral <= 0)
            throw new InvalidOperationException("La regla activa debe tener umbral_meses positivo.");

        var hoy = DateOnly.FromDateTime(_reloj.GetLocalNow().DateTime);
        var limite = hoy.AddMonths(umbral);
        var cantidad = _lotes.RemesasPorVencer(limite)
            .Count(r => r.EstadoLote == EstadoLote.Disponible && r.StockActual > 0
                && r.FechaVencimiento <= limite);

        // La fecha se escribe solo después de completar la evaluación correctamente (DEC-018/032).
        // DateTime local coincide con la columna DATETIME2(0) sin zona horaria del esquema.
        var finalizacion = _reloj.GetLocalNow();
        _repositorio.ActualizarFechaUltimaVerificacion(finalizacion.DateTime);

        var aviso = new AvisoVencimientos(cantidad, limite, finalizacion);
        _avisos.Publicar(aviso);
        return aviso;
    }
}
