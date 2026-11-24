namespace FarmasilApp.Modelo.DOC;

/// <summary>
/// Contrato de consulta y registro de comprobantes tributarios.
/// </summary>
public interface IComprobanteRepositorio
{
    /// <summary>
    /// Consulta los comprobantes según los filtros indicados.
    /// </summary>
    IReadOnlyList<ComprobanteTributario> Consultar(
        FiltroConsultaComprobantes filtro);

    /// <summary>
    /// Obtiene los identificadores de ventas que ya tienen
    /// un comprobante con estado Emitido.
    /// </summary>
    IReadOnlyList<int> ConsultarIdsVentasConComprobanteEmitido();

    /// <summary>
    /// Registra un comprobante tributario para una venta.
    /// </summary>
    void Registrar(
        string numeroComprobante,
        string tipoComprobante,
        DateTime fechaEmision,
        int idVenta);
}