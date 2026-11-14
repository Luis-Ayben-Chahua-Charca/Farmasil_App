namespace FarmasilApp.Modelo.DOC;

/// <summary>
/// Contrato de lectura de comprobantes tributarios (ESP-0010).
/// </summary>
public interface IComprobanteRepositorio
{
    IReadOnlyList<ComprobanteTributario> Consultar(FiltroConsultaComprobantes filtro);
}
