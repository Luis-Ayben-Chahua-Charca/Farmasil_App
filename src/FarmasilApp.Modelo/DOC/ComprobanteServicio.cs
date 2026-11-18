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
