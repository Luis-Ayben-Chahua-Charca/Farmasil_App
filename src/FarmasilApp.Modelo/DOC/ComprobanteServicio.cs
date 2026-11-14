namespace FarmasilApp.Modelo.DOC;

/// <summary>
/// Servicio de consulta de comprobantes tributarios (ESP-0010).
/// Normaliza los filtros de presentación antes de delegar en Persistencia.
/// </summary>
public sealed class ComprobanteServicio
{
    private readonly IComprobanteRepositorio _repositorio;

    public ComprobanteServicio(IComprobanteRepositorio repositorio)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public IReadOnlyList<ComprobanteTributario> Consultar(
        string? tipoComprobante,
        string? numeroComprobante,
        DateTime? fechaEmision)
    {
        var tipo = Normalizar(tipoComprobante);
        if (tipo is not null && tipo is not ("Boleta" or "Factura"))
        {
            throw new ArgumentException(
                "El tipo de comprobante debe ser Boleta o Factura.",
                nameof(tipoComprobante));
        }

        var numero = Normalizar(numeroComprobante);
        if (numero is { Length: > 20 })
        {
            throw new ArgumentException(
                "El número de comprobante no puede superar 20 caracteres.",
                nameof(numeroComprobante));
        }

        var filtro = new FiltroConsultaComprobantes
        {
            TipoComprobante = tipo,
            NumeroComprobante = numero,
            FechaEmision = fechaEmision?.Date
        };

        return _repositorio.Consultar(filtro);
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
