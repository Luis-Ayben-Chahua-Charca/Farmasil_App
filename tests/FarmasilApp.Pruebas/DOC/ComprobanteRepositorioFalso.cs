using FarmasilApp.Modelo.DOC;

namespace FarmasilApp.Pruebas.DOC;

/// <summary>Repositorio de comprobantes configurable para pruebas unitarias de DOC.</summary>
internal sealed class ComprobanteRepositorioFalso : IComprobanteRepositorio
{
    public IReadOnlyList<ComprobanteTributario> Resultados { get; set; } = Array.Empty<ComprobanteTributario>();

    public FiltroConsultaComprobantes? UltimoFiltro { get; private set; }

    public bool FallarAlConsultar { get; set; }

    public IReadOnlyList<ComprobanteTributario> Consultar(FiltroConsultaComprobantes filtro)
    {
        UltimoFiltro = filtro;

        if (FallarAlConsultar)
        {
            throw new InvalidOperationException("Falla simulada del repositorio DOC.");
        }

        return Resultados;
    }
}
