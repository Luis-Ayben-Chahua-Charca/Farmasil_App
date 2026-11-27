using System.Diagnostics;

namespace FarmasilApp.Modelo.ALV;

/// <summary>
/// VP-00 / VP-T2: ejecuta la evaluación al arrancar y luego cada 24 horas
/// mientras la aplicación esté abierta. Un fallo no bloquea WPF ni detiene el ciclo.
/// </summary>
public sealed class ProcesoDiarioVencimientos
{
    public static readonly TimeSpan Intervalo = TimeSpan.FromHours(24);

    private readonly EvaluacionVencimientosServicio _evaluador;
    private readonly Func<TimeSpan, CancellationToken, Task> _esperar;

    public ProcesoDiarioVencimientos(EvaluacionVencimientosServicio evaluador, TimeProvider reloj)
        : this(evaluador, (tiempo, ct) => Task.Delay(tiempo, reloj, ct))
    {
        ArgumentNullException.ThrowIfNull(reloj);
    }

    /// <summary>La espera inyectable permite verificar 24 h con tiempo controlado, sin esperar un día.</summary>
    public ProcesoDiarioVencimientos(EvaluacionVencimientosServicio evaluador,
        Func<TimeSpan, CancellationToken, Task> esperar)
    {
        _evaluador = evaluador ?? throw new ArgumentNullException(nameof(evaluador));
        _esperar = esperar ?? throw new ArgumentNullException(nameof(esperar));
    }

    public Task EjecutarAsync(CancellationToken cancelacion) => Task.Run(async () =>
    {
        while (!cancelacion.IsCancellationRequested)
        {
            try
            {
                // Evaluar realiza I/O sincrónico; nunca hacerlo en el hilo de interfaz.
                _evaluador.Evaluar();
            }
            catch (Exception ex)
            {
                Trace.TraceError("[RNF-0010.1] Evaluación de vencimientos: {0}", ex);
                // VP-04: si falla, no se publica aviso ni se detiene la aplicación.
            }

            try { await _esperar(Intervalo, cancelacion).ConfigureAwait(false); }
            catch (OperationCanceledException) when (cancelacion.IsCancellationRequested) { break; }
        }
    });
}
