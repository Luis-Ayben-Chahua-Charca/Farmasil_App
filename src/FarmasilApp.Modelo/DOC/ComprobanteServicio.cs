using System.Globalization;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.VEN;

namespace FarmasilApp.Modelo.DOC;

/// <summary>Reglas de consulta, registro, actualización y anulación de comprobantes (ESP-0009 a ESP-0012).</summary>
public sealed class ComprobanteServicio
{
    private readonly IComprobanteRepositorio _repositorio;
    private readonly IVentas _ventas;
    private readonly ISesion _sesion;

    public ComprobanteServicio(IComprobanteRepositorio repositorio, IVentas ventas, ISesion sesion)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        _ventas = ventas ?? throw new ArgumentNullException(nameof(ventas));
        _sesion = sesion ?? throw new ArgumentNullException(nameof(sesion));
    }

    /// <summary>Obtiene las ventas que todavía no tienen un comprobante emitido.</summary>
    public IReadOnlyList<VentaDto> ConsultarVentasPendientes()
    {
        ExigirAcceso();
        var idsConComprobanteEmitido = _repositorio
            .ConsultarIdsVentasConComprobanteEmitido()
            .ToHashSet();

        return _ventas
            .ConsultarVentas(new FiltroVentas())
            .Where(venta => !idsConComprobanteEmitido.Contains(venta.IdVenta))
            .OrderBy(venta => venta.IdVenta)
            .ToArray();
    }

    /// <summary>Valida los datos que se muestran en la previsualización, sin registrar nada.</summary>
    public DateTime ValidarDatosParaPrevisualizacion(
        string? tipoComprobante,
        string? numeroComprobante,
        string? fechaEmisionTexto)
    {
        ExigirAcceso();
        return ValidarDatosBasicos(tipoComprobante, numeroComprobante, fechaEmisionTexto).Fecha;
    }

    /// <summary>Registra un comprobante para una venta existente.</summary>
    public void Registrar(
        string? tipoComprobante,
        string? numeroComprobante,
        string? fechaEmisionTexto,
        int idVenta)
    {
        ExigirAcceso();
        var datos = ValidarDatosBasicos(tipoComprobante, numeroComprobante, fechaEmisionTexto);

        if (idVenta <= 0 || _ventas.ObtenerVenta(idVenta) is null)
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.VentaNoEncontrada);
        }

        if (ExisteNumeroDuplicado(datos.Numero, idExcluir: null))
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.NumeroDuplicado);
        }

        if (_repositorio.ConsultarIdsVentasConComprobanteEmitido().Contains(idVenta))
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.VentaYaEmitida);
        }

        _repositorio.Registrar(datos.Numero, datos.Tipo, datos.Fecha, idVenta);
    }

    /// <summary>Obtiene un comprobante para iniciar la edición. La operación es exclusiva del Administrador.</summary>
    public ComprobanteTributario ObtenerPorIdParaActualizar(int idComprobante)
    {
        ExigirAdministrador();
        var comprobante = ObtenerComprobanteExistente(idComprobante);
        ExigirEmitido(comprobante);
        return EnriquecerConVenta(comprobante);
    }

    /// <summary>Actualiza tipo, número y fecha; nunca cambia la venta asociada.</summary>
    public void Actualizar(
        int idComprobante,
        string? tipoComprobante,
        string? numeroComprobante,
        string? fechaEmisionTexto)
    {
        ExigirAdministrador();
        var comprobante = ObtenerComprobanteExistente(idComprobante);
        ExigirEmitido(comprobante);
        var datos = ValidarDatosBasicos(tipoComprobante, numeroComprobante, fechaEmisionTexto);

        if (ExisteNumeroDuplicado(datos.Numero, idComprobante))
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.NumeroDuplicado);
        }

        if (!_repositorio.Actualizar(
            idComprobante,
            datos.Numero,
            datos.Tipo,
            datos.Fecha))
        {
            // Otra operación pudo anularlo después de leerlo; la actualización condicional evita reactivarlo.
            throw new ReglaDeNegocioException(MensajesComprobantes.ComprobanteNoEmitido);
        }
    }

    /// <summary>
    /// Anula lógicamente un comprobante Emitido. La fila se conserva para auditoría;
    /// el índice único filtrado permite documentar nuevamente la venta después de la anulación.
    /// </summary>
    public void Anular(int idComprobante)
    {
        ExigirAdministrador();
        var comprobante = ObtenerComprobanteExistente(idComprobante);
        ExigirEmitido(comprobante);

        if (!_repositorio.Anular(idComprobante))
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.ComprobanteNoEmitido);
        }
    }

    public IReadOnlyList<ComprobanteTributario> Consultar(
        string? tipoComprobante,
        string? numeroComprobante,
        DateTime? fechaEmision)
    {
        ExigirAcceso();
        return ConsultarValidado(tipoComprobante, numeroComprobante, fechaEmision, null);
    }

    /// <summary>Conserva el texto de fecha para rechazar valores inválidos antes de consultar (DEC-079).</summary>
    public IReadOnlyList<ComprobanteTributario> ConsultarConFechaTexto(
        string? tipoComprobante,
        string? numeroComprobante,
        string? fechaEmision)
    {
        ExigirAcceso();
        return ConsultarValidado(tipoComprobante, numeroComprobante, null, fechaEmision);
    }

    private IReadOnlyList<ComprobanteTributario> ConsultarValidado(
        string? tipoComprobante,
        string? numeroComprobante,
        DateTime? fechaEmision,
        string? fechaTexto)
    {
        var tipo = Normalizar(tipoComprobante);
        if (string.Equals(tipo, "Todos", StringComparison.OrdinalIgnoreCase))
        {
            tipo = null;
        }

        if (tipo is not null && tipo is not ("Boleta" or "Factura"))
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.TipoInvalido);
        }

        var numero = Normalizar(numeroComprobante);
        if (numero is { Length: > 20 })
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.NumeroDemasiadoLargo);
        }

        if (!string.IsNullOrWhiteSpace(fechaTexto))
        {
            if (!DateTime.TryParse(
                fechaTexto,
                CultureInfo.GetCultureInfo("es-PE"),
                DateTimeStyles.AllowWhiteSpaces,
                out var fecha))
            {
                throw new ReglaDeNegocioException(MensajesComprobantes.FechaInvalida);
            }

            fechaEmision = fecha;
        }

        var filtro = new FiltroConsultaComprobantes
        {
            TipoComprobante = tipo,
            NumeroComprobante = numero,
            FechaEmision = fechaEmision?.Date
        };

        var comprobantes = _repositorio.Consultar(filtro);
        var resultado = new List<ComprobanteTributario>(comprobantes.Count);

        foreach (var comprobante in comprobantes)
        {
            resultado.Add(EnriquecerConVenta(comprobante));
        }

        return resultado;
    }

    private ComprobanteTributario ObtenerComprobanteExistente(int idComprobante)
    {
        if (idComprobante <= 0)
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.ComprobanteNoEncontrado);
        }

        var comprobante = _repositorio.ObtenerPorId(idComprobante);
        if (comprobante is null)
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.ComprobanteNoEncontrado);
        }

        return comprobante;
    }

    private static void ExigirEmitido(ComprobanteTributario comprobante)
    {
        if (string.Equals(comprobante.EstadoDocumento, "Anulado", StringComparison.OrdinalIgnoreCase))
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.ComprobanteAnuladoNoEditable);
        }

        if (!string.Equals(comprobante.EstadoDocumento, "Emitido", StringComparison.OrdinalIgnoreCase))
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.ComprobanteNoEmitido);
        }
    }

    private ComprobanteTributario EnriquecerConVenta(ComprobanteTributario comprobante)
    {
        var venta = _ventas.ObtenerVenta(comprobante.IdVenta);
        return new ComprobanteTributario
        {
            IdComprobante = comprobante.IdComprobante,
            NumeroComprobante = comprobante.NumeroComprobante,
            TipoComprobante = comprobante.TipoComprobante,
            FechaEmision = comprobante.FechaEmision,
            IdVenta = comprobante.IdVenta,
            CodigoVenta = venta is null
                ? $"Venta #{comprobante.IdVenta}"
                : $"VEN-{venta.FechaHora:yyyy}-{venta.IdVenta:000}",
            MontoTotal = venta?.MontoTotal ?? 0m,
            EstadoDocumento = comprobante.EstadoDocumento
        };
    }

    private bool ExisteNumeroDuplicado(string numero, int? idExcluir)
    {
        return _repositorio.Consultar(new FiltroConsultaComprobantes
        {
            NumeroComprobante = numero
        }).Any(comprobante =>
            (!idExcluir.HasValue || comprobante.IdComprobante != idExcluir.Value)
            && string.Equals(
                comprobante.NumeroComprobante,
                numero,
                StringComparison.OrdinalIgnoreCase));
    }

    private static (string Tipo, string Numero, DateTime Fecha) ValidarDatosBasicos(
        string? tipoComprobante,
        string? numeroComprobante,
        string? fechaEmisionTexto)
    {
        var tipo = Normalizar(tipoComprobante);
        if (tipo is not ("Boleta" or "Factura"))
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.TipoInvalido);
        }

        var numero = Normalizar(numeroComprobante);
        if (numero is null)
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.NumeroObligatorio);
        }

        if (numero.Length > 20)
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.NumeroDemasiadoLargo);
        }

        if (!DateTime.TryParse(
            fechaEmisionTexto,
            CultureInfo.GetCultureInfo("es-PE"),
            DateTimeStyles.AllowWhiteSpaces,
            out var fecha))
        {
            throw new ReglaDeNegocioException(MensajesComprobantes.FechaInvalida);
        }

        return (tipo, numero, fecha.Date);
    }

    private void ExigirAcceso()
    {
        var rol = _sesion.UsuarioActual.Rol;
        if (rol is not (Rol.Administrador or Rol.Tecnico))
        {
            throw new AccesoDenegadoException(MensajesComprobantes.AccesoDenegado);
        }

        _sesion.ExigirRol(rol);
    }

    private void ExigirAdministrador()
    {
        if (_sesion.UsuarioActual.Rol != Rol.Administrador)
        {
            throw new AccesoDenegadoException(MensajesComprobantes.AccesoAdministracionDenegado);
        }

        _sesion.ExigirRol(Rol.Administrador);
    }

    private static string? Normalizar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
