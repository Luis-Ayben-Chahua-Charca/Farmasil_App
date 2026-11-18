using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Modelo.ALV;

/// <summary>
/// Consulta ESP-0013. Reutiliza ILotes.RemesasPorVencer, la misma fuente de remesas
/// que debe consumir el proceso 1 de RNF-0010 (DEC-040 y DEC-069).
/// No escribe en ninguna tabla ni altera los lotes.
/// </summary>
public sealed class AlertasServicio
{
    private readonly IAlertasRepositorio _repositorio;
    private readonly ILotes _lotes;
    private readonly ISesion _sesion;
    private readonly TimeProvider _reloj;

    public AlertasServicio(IAlertasRepositorio repositorio, ILotes lotes, ISesion sesion, TimeProvider reloj)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        _lotes = lotes ?? throw new ArgumentNullException(nameof(lotes));
        _sesion = sesion ?? throw new ArgumentNullException(nameof(sesion));
        _reloj = reloj ?? throw new ArgumentNullException(nameof(reloj));
    }

    public ResultadoAlertas Consultar(int? filtroLote = null, string? filtroEstado = null)
    {
        var rol = _sesion.UsuarioActual.Rol;
        if (rol != Rol.Administrador && rol != Rol.Tecnico)
            throw new AccesoDenegadoException("No tiene permisos para consultar alertas.");

        if (filtroLote is <= 0)
            throw new ArgumentOutOfRangeException(nameof(filtroLote), "La remesa debe ser positiva.");

        var estado = string.IsNullOrWhiteSpace(filtroEstado) ? "Todos" : filtroEstado.Trim();
        if (estado != "Todos" && estado != "Por vencer" && estado != "Retirados")
            throw new ArgumentException("El estado debe ser Todos, Por vencer o Retirados.", nameof(filtroEstado));

        // Se lee cada vez: cambiar la regla en la base cambia la consulta sin recompilar.
        var umbral = _repositorio.ObtenerUmbralMesesActivo();
        if (umbral <= 0)
            throw new InvalidOperationException("La regla activa debe tener umbral_meses positivo.");

        var hoy = DateOnly.FromDateTime(_reloj.GetLocalNow().DateTime);
        var limite = hoy.AddMonths(umbral);
        var proximas = _lotes.RemesasPorVencer(limite)
            // Defensa de coherencia: idéntico predicado de RNF-0010, sin cota inferior de fecha.
            .Where(r => r.EstadoLote == EstadoLote.Disponible
                && r.StockActual > 0 && r.FechaVencimiento <= limite)
            .Select(r => new AlertaConsulta
            {
                IdLote = r.IdLote,
                Medicamento = r.NombreMedicamento,
                NumeroLote = r.NumeroLote,
                FechaVencimiento = r.FechaVencimiento,
                EsLoteDefectuoso = null, // el defecto físico solo se registra en retiros
                Origen = "Por vencer"
            }).ToArray();

        var retiros = _repositorio.ConsultarRetiros();
        var alertas = Enumerable.Empty<AlertaConsulta>();
        if (estado is "Todos" or "Por vencer") alertas = alertas.Concat(proximas);
        if (estado is "Todos" or "Retirados") alertas = alertas.Concat(retiros);
        if (filtroLote.HasValue) alertas = alertas.Where(a => a.IdLote == filtroLote.Value);

        return new ResultadoAlertas
        {
            UmbralMeses = umbral,
            FechaLimite = limite,
            // No aplicar filtros de pantalla al conteo que debe igualar RNF-0010.
            TotalProximasAVencer = proximas.Length,
            Alertas = alertas.OrderBy(a => a.FechaVencimiento)
                .ThenBy(a => a.Medicamento)
                .ThenBy(a => a.NumeroLote)
                .ThenBy(a => a.Origen).ToArray()
        };
    }
}
