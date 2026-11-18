using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.VEN;

namespace FarmasilApp.Modelo.DOC;

/// <summary>Servicio de consulta de comprobantes tributarios (ESP-0010).</summary>
public sealed class ComprobanteServicio
{
    private readonly IComprobanteRepositorio _repositorio;
    private readonly IVentas _ventas;

    public ComprobanteServicio(
        IComprobanteRepositorio repositorio,
        IVentas ventas)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
        _ventas = ventas
            ?? throw new ArgumentNullException(nameof(ventas));
    }

    public IReadOnlyList<ComprobanteTributario> Consultar(
        string? tipoComprobante,
        string? numeroComprobante,
        DateTime? fechaEmision)
    {
        var tipo = Normalizar(tipoComprobante);
        if (tipo is not null && tipo is not ("Boleta" or "Factura"))
        {
            throw new ReglaDeNegocioException(
                "El tipo de comprobante debe ser Boleta o Factura.");
        }

        var numero = Normalizar(numeroComprobante);
        if (numero is { Length: > 20 })
        {
            throw new ReglaDeNegocioException(
                "El número de comprobante no puede superar 20 caracteres.");
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

    private static string? Normalizar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        var normalizado = valor.Trim();
        return string.Equals(normalizado, "Todos", StringComparison.OrdinalIgnoreCase)
            ? null
            : normalizado;
    }
}
