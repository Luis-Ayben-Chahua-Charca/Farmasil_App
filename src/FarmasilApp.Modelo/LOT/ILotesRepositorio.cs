using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.LOT;

/// <summary>
/// Persistencia propia del módulo LOT para ESP-0041 a ESP-0044.
/// LOT escribe únicamente TBL_LOTES y puede leer las tablas de devoluciones
/// para las precondiciones autorizadas por DEC-055.
/// </summary>
public interface ILotesRepositorio
{
    /// <summary>
    /// Consulta las remesas aplicando únicamente los filtros informados,
    /// ordenadas por fecha de vencimiento ascendente.
    /// </summary>
    IReadOnlyList<RemesaConsulta> Consultar(
        int? idProducto,
        string? numeroLote,
        EstadoLote? estado);

    /// <summary>Obtiene una remesa por identificador o null si no existe.</summary>
    RemesaConsulta? ObtenerPorId(int idLote);

    /// <summary>
    /// Indica si ya existe el par producto-número de lote.
    /// excluirIdLote permite validar una actualización sin colisionar consigo misma.
    /// </summary>
    bool ExisteNumeroLote(
        int idProducto,
        string numeroLote,
        int? excluirIdLote = null);

    /// <summary>ESP-0041. Registra una nueva remesa y devuelve su identificador.</summary>
    int Crear(NuevaRemesa remesa);

    /// <summary>
    /// Indica si la remesa aparece en cualquier detalle de devolución,
    /// independientemente del estado de la orden.
    /// </summary>
    bool TieneDetalleDevolucion(int idLote);

    /// <summary>
    /// Cantidad total comprometida para la remesa en órdenes de devolución pendientes.
    /// </summary>
    int CantidadComprometidaPendiente(int idLote);

    /// <summary>
    /// ESP-0043. Actualiza únicamente los campos editables de la remesa
    /// y persiste el estado derivado en la misma sentencia que el stock.
    /// </summary>
    void Actualizar(
        int idLote,
        ActualizacionRemesa remesa,
        EstadoLote estadoNuevo);

    /// <summary>
    /// Indica si la remesa participa en alguna orden de devolución pendiente.
    /// </summary>
    bool TieneDevolucionPendiente(int idLote);

    /// <summary>
    /// ESP-0044. Intenta el borrado físico y, si una referencia lo impide,
    /// aplica la baja lógica según DEC-009 y DEC-029.
    /// </summary>
    ResultadoBajaRemesa DarDeBaja(int idLote);
}
