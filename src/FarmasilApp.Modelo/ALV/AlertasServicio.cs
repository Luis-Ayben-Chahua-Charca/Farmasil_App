using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DEV;
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
    private readonly IDevoluciones? _devoluciones;

    public AlertasServicio(IAlertasRepositorio repositorio, ILotes lotes, ISesion sesion, TimeProvider reloj)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        _lotes = lotes ?? throw new ArgumentNullException(nameof(lotes));
        _sesion = sesion ?? throw new ArgumentNullException(nameof(sesion));
        _reloj = reloj ?? throw new ArgumentNullException(nameof(reloj));
    }

    /// <summary>Compatibilidad con las pruebas y consumidores de consulta de la tarea #87.</summary>
    public AlertasServicio(IAlertasRepositorio repositorio, ILotes lotes, ISesion sesion,
        TimeProvider reloj, IDevoluciones devoluciones)
        : this(repositorio, lotes, sesion, reloj)
    {
        _devoluciones = devoluciones ?? throw new ArgumentNullException(nameof(devoluciones));
    }

    /// <summary>Remesas ofrecidas por el contrato de LOT para ESP-0014, sin acceder a su tabla.</summary>
    public IReadOnlyList<DatosRemesaDto> RemesasParaRegistrar()
    {
        ExigirRolPermitidoParaRegistrar();
        return _lotes.RemesasDisponibles();
    }

    /// <summary>ESP-0014. Retiro persistido en ALV y orden de bloqueo cursada a ILotes.</summary>
    public int Registrar(int idLote, bool defectuoso, string? codigoDigemid)
    {
        ExigirRolPermitidoParaRegistrar();
        if (idLote <= 0) throw new ReglaDeNegocioException(MensajesAlertas.RemesaObligatoria);
        var codigo = NormalizarCodigo(codigoDigemid);
        var remesa = _lotes.RemesasDisponibles().SingleOrDefault(x => x.IdLote == idLote)
            ?? throw new ReglaDeNegocioException(MensajesAlertas.RemesaObligatoria);
        var retiro = new NuevaAlertaRetiro
        {
            IdLote = remesa.IdLote,
            NombreProducto = remesa.NombreMedicamento,
            NumeroLote = remesa.NumeroLote,
            FechaVencimiento = remesa.FechaVencimiento,
            EsLoteDefectuoso = defectuoso,
            AlertaDigemid = codigo,
            FechaRegistro = Hoy
        };
        return _repositorio.EjecutarTransaccion(u =>
        {
            var id = u.Insertar(retiro);
            _lotes.Bloquear(idLote); // PEN-039: hoy el contrato LOT aún no comparte esta transacción SQL.
            return id;
        });
    }

    /// <summary>ESP-0015. No toca remesa, copias históricas ni estado de lote.</summary>
    public void Modificar(int idRetiro, bool defectuoso, string? codigoDigemid)
    {
        _sesion.ExigirRol(Rol.Administrador);
        if (idRetiro <= 0) throw new ReglaDeNegocioException(MensajesAlertas.RetiroObligatorio);
        var codigo = NormalizarCodigo(codigoDigemid);
        _repositorio.EjecutarTransaccion(u =>
        {
            if (u.ObtenerRetiro(idRetiro) is null || !u.Actualizar(idRetiro, defectuoso, codigo))
                throw new ReglaDeNegocioException(MensajesAlertas.RetiroObligatorio);
            return true;
        });
    }

    /// <summary>Comprobación previa al diálogo; Eliminar repite las verificaciones bajo transacción.</summary>
    public void VerificarEliminacion(int idRetiro)
    {
        _sesion.ExigirRol(Rol.Administrador);
        if (idRetiro <= 0) throw new ReglaDeNegocioException(MensajesAlertas.RetiroObligatorio);
        _repositorio.EjecutarTransaccion(u =>
        {
            var retiro = u.ObtenerRetiro(idRetiro)
                ?? throw new ReglaDeNegocioException(MensajesAlertas.RetiroObligatorio);
            VerificarRestriccionesEliminacion(retiro);
            return true;
        });
    }

    /// <summary>ESP-0016. Retira una fila; solo llama Liberar si no quedan retiros para la remesa.</summary>
    public string Eliminar(int idRetiro)
    {
        _sesion.ExigirRol(Rol.Administrador);
        if (idRetiro <= 0) throw new ReglaDeNegocioException(MensajesAlertas.RetiroObligatorio);
        return _repositorio.EjecutarTransaccion(u =>
        {
            var retiro = u.ObtenerRetiro(idRetiro)
                ?? throw new ReglaDeNegocioException(MensajesAlertas.RetiroObligatorio);
            VerificarRestriccionesEliminacion(retiro);
            if (!u.Eliminar(idRetiro)) throw new ReglaDeNegocioException(MensajesAlertas.RetiroObligatorio);
            if (!retiro.IdLote.HasValue) return MensajesAlertas.Eliminada;
            if (u.ContarRetiros(retiro.IdLote.Value) > 0) return MensajesAlertas.EliminadaConOtrosRetiros;
            _lotes.Liberar(retiro.IdLote.Value); // PEN-039: coordinación transaccional de #99 y #101.
            return MensajesAlertas.Eliminada;
        });
    }

    private void VerificarRestriccionesEliminacion(AlertaRetiro retiro)
    {
        if (!retiro.IdLote.HasValue) return; // Retiro histórico, FK ya anulada.
        var idLote = retiro.IdLote.Value;
        if (_devoluciones is null) throw new InvalidOperationException("No está registrado IDevoluciones.");
        if (_devoluciones.OrdenesPendientesDeRemesa(idLote).Count != 0)
            throw new ReglaDeNegocioException(MensajesAlertas.DevolucionPendiente);
        var remesa = _lotes.DatosRemesas(new[] { idLote }).SingleOrDefault();
        if (remesa is null) throw new InvalidOperationException("La remesa referenciada no está disponible en ILotes.");
        if (remesa.FechaVencimiento <= Hoy)
            throw new ReglaDeNegocioException(MensajesAlertas.RemesaVencida);
    }

    private void ExigirRolPermitidoParaRegistrar()
    {
        var rol = _sesion.UsuarioActual.Rol;
        _sesion.ExigirRol(rol);
        if (rol is not (Rol.Administrador or Rol.Tecnico))
            throw new AccesoDenegadoException(MensajesAlertas.RolNoPermiteRegistrar);
    }

    private DateOnly Hoy => DateOnly.FromDateTime(_reloj.GetLocalNow().DateTime);

    private static string? NormalizarCodigo(string? codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return null;
        var valor = codigo.Trim();
        if (valor.Length > 100) throw new ReglaDeNegocioException(MensajesAlertas.CodigoDigemidLargo);
        return valor;
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
