using FarmasilApp.Modelo.DOC;

namespace FarmasilApp.Pruebas.DOC;

/// <summary>
/// Repositorio de comprobantes configurable para pruebas unitarias de DOC.
/// </summary>
internal sealed class ComprobanteRepositorioFalso : IComprobanteRepositorio
{
    public IReadOnlyList<ComprobanteTributario> Resultados
        { get; set; } = Array.Empty<ComprobanteTributario>();

    public IReadOnlyList<int> IdsVentasConComprobanteEmitido
        { get; set; } = Array.Empty<int>();

    public FiltroConsultaComprobantes? UltimoFiltro { get; private set; }

    public int Consultas { get; private set; }

    public bool FallarAlConsultar { get; set; }

    public string? UltimoNumeroComprobanteRegistrado { get; private set; }

    public string? UltimoTipoComprobanteRegistrado { get; private set; }

    public DateTime? UltimaFechaEmisionRegistrada { get; private set; }

    public int? UltimaVentaRegistrada { get; private set; }

    public IReadOnlyList<int> ConsultarIdsVentasConComprobanteEmitido()
    {
        return IdsVentasConComprobanteEmitido;
    }

    public void Registrar(
        string numeroComprobante,
        string tipoComprobante,
        DateTime fechaEmision,
        int idVenta)
    {
        UltimoNumeroComprobanteRegistrado = numeroComprobante;
        UltimoTipoComprobanteRegistrado = tipoComprobante;
        UltimaFechaEmisionRegistrada = fechaEmision;
        UltimaVentaRegistrada = idVenta;

        // Como la base real: el comprobante queda Emitido y aparece en las consultas siguientes.
        Resultados = Resultados
            .Append(new ComprobanteTributario
            {
                NumeroComprobante = numeroComprobante,
                TipoComprobante = tipoComprobante,
                FechaEmision = fechaEmision,
                IdVenta = idVenta,
                EstadoDocumento = "Emitido"
            })
            .ToArray();
        IdsVentasConComprobanteEmitido = IdsVentasConComprobanteEmitido.Append(idVenta).ToArray();
    }

    public IReadOnlyList<ComprobanteTributario> Consultar(
        FiltroConsultaComprobantes filtro)
    {
        Consultas++;
        UltimoFiltro = filtro;

        if (FallarAlConsultar)
        {
            throw new InvalidOperationException(
                "Falla simulada del repositorio DOC.");
        }

        return Resultados;
    }
}
