using FarmasilApp.Modelo.LOT;

namespace FarmasilApp.Modelo.VEN;

/// <summary>
/// Detalle de la venta en curso (ESP-0001, <c>VEN-TBL-DETALLE-VENTA</c>). Vive en memoria: salir de la pantalla lo
/// descarta (DEC-067) y no escribe en ninguna tabla.
/// </summary>
/// <remarks>
/// <para>Solo guarda las líneas y calcula el total. Las validaciones que dependen de otros módulos (vendibilidad,
/// stock vigente, restricciones) las hace <see cref="VentaServicio.AgregarLinea"/> antes de llamar a
/// <see cref="Agregar"/>.</para>
/// <para>Una remesa aparece una sola vez: si se agrega de nuevo, se acumula la cantidad (DEC-036). Para corregir una
/// cantidad se quita la línea y se vuelve a agregar (DEC-067).</para>
/// </remarks>
public sealed class CarritoVenta
{
    private readonly List<LineaVenta> _lineas = new();

    public IReadOnlyList<LineaVenta> Lineas => _lineas;

    /// <summary>Suma de cantidad por precio unitario de cada línea (ESP-0001, <c>VEN-LBL-MONTO-TOTAL</c>).</summary>
    public decimal MontoTotal => _lineas.Sum(linea => linea.Subtotal);

    /// <summary>Cantidad que el detalle ya tiene de la remesa; 0 si no figura.</summary>
    public int CantidadDe(int idLote) => _lineas.FirstOrDefault(linea => linea.IdLote == idLote)?.Cantidad ?? 0;

    /// <summary>
    /// Agrega la remesa con esa cantidad, o suma la cantidad si la remesa ya figura. La línea existente conserva su
    /// precio unitario.
    /// </summary>
    /// <exception cref="ArgumentNullException">La remesa es nula.</exception>
    /// <exception cref="ArgumentOutOfRangeException">La cantidad no es positiva.</exception>
    public void Agregar(RemesaVendibleDto remesa, int cantidad)
    {
        ArgumentNullException.ThrowIfNull(remesa);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cantidad);

        var indice = _lineas.FindIndex(linea => linea.IdLote == remesa.IdLote);
        if (indice < 0)
        {
            _lineas.Add(new LineaVenta
            {
                IdLote = remesa.IdLote,
                IdProducto = remesa.IdProducto,
                NombreMedicamento = remesa.NombreMedicamento,
                NumeroLote = remesa.NumeroLote,
                Cantidad = cantidad,
                PrecioUnitario = remesa.PrecioVenta,
            });
            return;
        }

        var actual = _lineas[indice];
        _lineas[indice] = new LineaVenta
        {
            IdLote = actual.IdLote,
            IdProducto = actual.IdProducto,
            NombreMedicamento = actual.NombreMedicamento,
            NumeroLote = actual.NumeroLote,
            Cantidad = actual.Cantidad + cantidad,
            PrecioUnitario = actual.PrecioUnitario,
        };
    }

    /// <summary>Retira la línea de esa remesa. Devuelve <c>false</c> si no figuraba.</summary>
    public bool Quitar(int idLote)
    {
        var indice = _lineas.FindIndex(linea => linea.IdLote == idLote);
        if (indice < 0)
        {
            return false;
        }

        _lineas.RemoveAt(indice);
        return true;
    }

    public void Vaciar() => _lineas.Clear();
}
