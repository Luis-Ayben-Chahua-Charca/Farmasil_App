namespace FarmasilApp.Modelo.DOC;

/// <summary>Contrato de consulta, registro, actualización y anulación de comprobantes tributarios.</summary>
public interface IComprobanteRepositorio
{
    /// <summary>Consulta comprobantes según los filtros indicados.</summary>
    IReadOnlyList<ComprobanteTributario> Consultar(FiltroConsultaComprobantes filtro);

    /// <summary>Obtiene un comprobante por su identificador interno.</summary>
    ComprobanteTributario? ObtenerPorId(int idComprobante);

    /// <summary>Obtiene las ventas que ya tienen un comprobante con estado Emitido.</summary>
    IReadOnlyList<int> ConsultarIdsVentasConComprobanteEmitido();

    /// <summary>Registra un comprobante tributario para una venta.</summary>
    void Registrar(string numeroComprobante, string tipoComprobante, DateTime fechaEmision, int idVenta);

    /// <summary>
    /// Actualiza los datos corregibles de un comprobante que continúa Emitido.
    /// Devuelve false si ya no existe o dejó de estar Emitido.
    /// </summary>
    bool Actualizar(int idComprobante, string numeroComprobante, string tipoComprobante, DateTime fechaEmision);

    /// <summary>Marca como Anulado un comprobante Emitido, sin eliminarlo físicamente.</summary>
    bool Anular(int idComprobante);
}
