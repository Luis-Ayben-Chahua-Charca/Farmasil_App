namespace FarmasilApp.Modelo.DEV;

/// <summary>
/// Orden de devolución 'Pendiente' que incluye una remesa, tal como la ofrece <see cref="IDevoluciones"/>
/// (TBL_ORDENES_DEVOLUCION y TBL_DETALLE_DEVOLUCION).
/// </summary>
public sealed class OrdenPendienteDto
{
    public int IdDevolucion { get; init; }

    public DateOnly FechaCreacion { get; init; }

    /// <summary>Unidades de la remesa que la orden va a devolver.</summary>
    public int CantidadDevolver { get; init; }
}
