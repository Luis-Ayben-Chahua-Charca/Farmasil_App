using System.Globalization;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.VEN;

namespace FarmasilApp.Modelo.DOC;

/// <summary>Servicio de consulta de comprobantes tributarios (ESP-0010).</summary>
public sealed class ComprobanteServicio
{
    private readonly IComprobanteRepositorio _repositorio;
    private readonly IVentas _ventas;
    private readonly ISesion _sesion;

    public ComprobanteServicio(
        IComprobanteRepositorio repositorio,
        IVentas ventas,
        ISesion sesion)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
        _ventas = ventas
            ?? throw new ArgumentNullException(nameof(ventas));
        _sesion = sesion
            ?? throw new ArgumentNullException(nameof(sesion));
    }


/// <summary>
/// Obtiene las ventas que todavía no tienen un comprobante emitido.
/// </summary>
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
/// <summary>
/// Registra un comprobante para una venta existente.
/// </summary>
public void Registrar(
    string? tipoComprobante,
    string? numeroComprobante,
    string? fechaEmisionTexto,
    int idVenta)
{
    ExigirAcceso();

    // 1. Validar tipo de comprobante.
    var tipo = Normalizar(tipoComprobante);

    if (tipo is not ("Boleta" or "Factura"))
    {
        throw new ReglaDeNegocioException(
            MensajesComprobantes.TipoInvalido);
    }

    // 2. Validar número.
    var numero = Normalizar(numeroComprobante);

    if (numero is null)
    {
        throw new ReglaDeNegocioException(
            MensajesComprobantes.NumeroObligatorio);
    }

    if (numero.Length > 20)
    {
        throw new ReglaDeNegocioException(
            MensajesComprobantes.NumeroDemasiadoLargo);
    }

    // 3. Validar fecha con formato peruano.
    if (!DateTime.TryParse(
        fechaEmisionTexto,
        CultureInfo.GetCultureInfo("es-PE"),
        DateTimeStyles.AllowWhiteSpaces,
        out var fecha))
    {
        throw new ReglaDeNegocioException(
            MensajesComprobantes.FechaInvalida);
    }

    // 4. Comprobar que la venta exista.
    if (idVenta <= 0 || _ventas.ObtenerVenta(idVenta) is null)
    {
        throw new ReglaDeNegocioException(
            MensajesComprobantes.VentaNoEncontrada);
    }

    // 5. Comprobar que el número no esté registrado.
    var comprobantesExistentes = _repositorio.Consultar(
        new FiltroConsultaComprobantes
        {
            NumeroComprobante = numero
        });

    if (comprobantesExistentes.Any(comprobante =>
        string.Equals(
            comprobante.NumeroComprobante,
            numero,
            StringComparison.OrdinalIgnoreCase)))
    {
        throw new ReglaDeNegocioException(
            MensajesComprobantes.NumeroDuplicado);
    }

    // 6. Comprobar que la venta no tenga otro comprobante emitido.
    if (_repositorio
        .ConsultarIdsVentasConComprobanteEmitido()
        .Contains(idVenta))
    {
        throw new ReglaDeNegocioException(
            MensajesComprobantes.VentaYaEmitida);
    }

    // 7. Registrar en SQL Server.
    _repositorio.Registrar(
        numero,
        tipo,
        fecha.Date,
        idVenta);
}
    public IReadOnlyList<ComprobanteTributario> Consultar(
        string? tipoComprobante,
        string? numeroComprobante,
        DateTime? fechaEmision)
    {
        ExigirAcceso();
        return ConsultarValidado(tipoComprobante, numeroComprobante, fechaEmision, null);
    }

    /// <summary>Conserva la entrada de fecha para rechazar texto inválido antes de consultar (DEC-079).</summary>
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
        if (string.Equals(tipo, "Todos", StringComparison.OrdinalIgnoreCase)) tipo = null;
        if (tipo is not null && tipo is not ("Boleta" or "Factura"))
        {
            throw new ReglaDeNegocioException(
                MensajesComprobantes.TipoInvalido);
        }

        var numero = Normalizar(numeroComprobante);
        if (numero is { Length: > 20 })
        {
            throw new ReglaDeNegocioException(
                MensajesComprobantes.NumeroDemasiadoLargo);
        }

        if (!string.IsNullOrWhiteSpace(fechaTexto))
        {
            if (!DateTime.TryParse(fechaTexto, CultureInfo.GetCultureInfo("es-PE"),
                    DateTimeStyles.AllowWhiteSpaces, out var fecha))
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
            var venta = _ventas.ObtenerVenta(comprobante.IdVenta);
            resultado.Add(new ComprobanteTributario
            {
                IdComprobante = comprobante.IdComprobante,
                NumeroComprobante = comprobante.NumeroComprobante,
                TipoComprobante = comprobante.TipoComprobante,
                FechaEmision = comprobante.FechaEmision,
                IdVenta = comprobante.IdVenta,
                MontoTotal = venta?.MontoTotal ?? 0m,
                EstadoDocumento = comprobante.EstadoDocumento
            });
        }

        return resultado;
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

    private static string? Normalizar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        return valor.Trim();
    }
}
