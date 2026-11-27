using FarmasilApp.Modelo.DOC;

namespace FarmasilApp.Pruebas.DOC;

/// <summary>Repositorio en memoria para pruebas de las operaciones DOC.</summary>
internal sealed class ComprobanteRepositorioFalso : IComprobanteRepositorio
{
    public IReadOnlyList<ComprobanteTributario> Resultados { get; set; } = Array.Empty<ComprobanteTributario>();
    public IReadOnlyList<int> IdsVentasConComprobanteEmitido { get; set; } = Array.Empty<int>();
    public FiltroConsultaComprobantes? UltimoFiltro { get; private set; }
    public int Consultas { get; private set; }
    public bool FallarAlConsultar { get; set; }
    public string? UltimoNumeroComprobanteRegistrado { get; private set; }
    public string? UltimoTipoComprobanteRegistrado { get; private set; }
    public DateTime? UltimaFechaEmisionRegistrada { get; private set; }
    public int? UltimaVentaRegistrada { get; private set; }
    public int? UltimoIdActualizado { get; private set; }
    public int? UltimoIdAnulado { get; private set; }

    public IReadOnlyList<int> ConsultarIdsVentasConComprobanteEmitido() => IdsVentasConComprobanteEmitido;

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

    var nuevoId = Resultados.Count == 0
        ? 1
        : Resultados.Max(c => c.IdComprobante) + 1;

    var nuevoComprobante = new ComprobanteTributario
    {
        IdComprobante = nuevoId,
        NumeroComprobante = numeroComprobante,
        TipoComprobante = tipoComprobante,
        FechaEmision = fechaEmision,
        IdVenta = idVenta,
        EstadoDocumento = "Emitido"
    };

    Resultados = Resultados
        .Append(nuevoComprobante)
        .ToArray();

    IdsVentasConComprobanteEmitido = IdsVentasConComprobanteEmitido
        .Append(idVenta)
        .Distinct()
        .ToArray();
}

    public bool Actualizar(int idComprobante, string numeroComprobante, string tipoComprobante, DateTime fechaEmision)
    {
        var actual = ObtenerPorId(idComprobante);
        if (actual is null || actual.EstadoDocumento != "Emitido")
        {
            return false;
        }

        UltimoIdActualizado = idComprobante;
        Resultados = Resultados.Select(item => item.IdComprobante == idComprobante
            ? Clonar(item, numeroComprobante, tipoComprobante, fechaEmision, item.EstadoDocumento)
            : item).ToArray();
        return true;
    }

    public bool Anular(int idComprobante)
    {
        var actual = ObtenerPorId(idComprobante);
        if (actual is null || actual.EstadoDocumento != "Emitido")
        {
            return false;
        }

        UltimoIdAnulado = idComprobante;
        Resultados = Resultados.Select(item => item.IdComprobante == idComprobante
            ? Clonar(item, item.NumeroComprobante, item.TipoComprobante, item.FechaEmision, "Anulado")
            : item).ToArray();
        IdsVentasConComprobanteEmitido = Resultados
            .Where(item => item.EstadoDocumento == "Emitido")
            .Select(item => item.IdVenta)
            .ToArray();
        return true;
    }

    public ComprobanteTributario? ObtenerPorId(int idComprobante) =>
        Resultados.FirstOrDefault(item => item.IdComprobante == idComprobante);

    public IReadOnlyList<ComprobanteTributario> Consultar(FiltroConsultaComprobantes filtro)
    {
        Consultas++;
        UltimoFiltro = filtro;
        if (FallarAlConsultar)
        {
            throw new InvalidOperationException("Falla simulada del repositorio DOC.");
        }

        return Resultados;
    }

    private static ComprobanteTributario Clonar(
        ComprobanteTributario origen,
        string numero,
        string tipo,
        DateTime fecha,
        string estado) => new()
    {
        IdComprobante = origen.IdComprobante,
        NumeroComprobante = numero,
        TipoComprobante = tipo,
        FechaEmision = fecha,
        IdVenta = origen.IdVenta,
        MontoTotal = origen.MontoTotal,
        EstadoDocumento = estado,
        CodigoVenta = origen.CodigoVenta
    };
}
