using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.LOT;

/// <summary>
/// Implementación simulada de <see cref="ILotes"/> mientras LOT no esté listo (plan, sección 10).
/// Devuelve siempre las remesas de database/002_datos_prueba.sql, con los mismos ids y con vencimientos relativos
/// a la fecha de hoy. No tiene estado: <see cref="Descontar"/>, <see cref="Restituir"/>, <see cref="Bloquear"/> y
/// <see cref="Liberar"/> solo validan sus argumentos, así que el stock no baja. Las pruebas de flujo con stock
/// se hacen contra la base.
/// </summary>
public sealed class LotesSimulado : ILotes
{
    private static readonly EstadoLote[] EstadosConExistencias = { EstadoLote.Disponible, EstadoLote.BloqueadoPorDevolucion };

    private readonly TimeProvider _reloj;

    public LotesSimulado(TimeProvider reloj)
    {
        _reloj = reloj ?? throw new ArgumentNullException(nameof(reloj));
    }

    private DateOnly Hoy => DateOnly.FromDateTime(_reloj.GetLocalNow().DateTime);

    public IReadOnlyList<MedicamentoVendibleDto> MedicamentosVendibles()
    {
        var hoy = Hoy;
        var vendibles = DatosDePrueba.Lotes(hoy).Where(lote => EsVendible(lote, hoy)).ToArray();

        return DatosDePrueba.Productos
            .Where(producto => vendibles.Any(lote => lote.IdProducto == producto.IdProducto))
            .Select(producto => new MedicamentoVendibleDto
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                AccionTerapeutica = producto.AccionTerapeutica,
                PrecioVenta = producto.PrecioVenta,
                StockVendible = vendibles.Where(lote => lote.IdProducto == producto.IdProducto).Sum(lote => lote.StockActual),
            })
            .ToArray();
    }

    public IReadOnlyList<RemesaVendibleDto> SugerirFefo(int idProducto)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idProducto);

        var hoy = Hoy;
        return DatosDePrueba.Lotes(hoy)
            .Where(lote => lote.IdProducto == idProducto && EsVendible(lote, hoy))
            .OrderBy(lote => lote.FechaVencimiento)
            .ThenBy(lote => lote.IdLote)
            .Select(lote =>
            {
                var producto = Producto(lote);
                return new RemesaVendibleDto
                {
                    IdLote = lote.IdLote,
                    IdProducto = lote.IdProducto,
                    NombreMedicamento = producto.Nombre,
                    PrecioVenta = producto.PrecioVenta,
                    NumeroLote = lote.NumeroLote,
                    FechaVencimiento = lote.FechaVencimiento,
                    StockActual = lote.StockActual,
                };
            })
            .ToArray();
    }

    public IReadOnlyList<DatosRemesaDto> DatosRemesas(IReadOnlyCollection<int> idsLote)
    {
        ArgumentNullException.ThrowIfNull(idsLote);
        foreach (var idLote in idsLote)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idLote);
        }

        var lotes = DatosDePrueba.Lotes(Hoy);
        return idsLote
            .Distinct()
            .Select(idLote => lotes.FirstOrDefault(lote => lote.IdLote == idLote))
            .OfType<LoteDePrueba>()
            .Select(Datos)
            .ToArray();
    }

    public IReadOnlyList<DatosRemesaDto> RemesasDevolvibles(int idProveedor)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idProveedor);

        return DatosDePrueba.Lotes(Hoy)
            .Where(lote => lote.IdProveedor == idProveedor && TieneExistencias(lote))
            .OrderBy(lote => lote.FechaVencimiento)
            .ThenBy(lote => lote.IdLote)
            .Select(Datos)
            .ToArray();
    }

    public IReadOnlyList<DatosRemesaDto> RemesasPorVencer(DateOnly fechaLimite)
    {
        return DatosDePrueba.Lotes(Hoy)
            .Where(lote => lote.Estado == EstadoLote.Disponible && lote.StockActual > 0 && lote.FechaVencimiento <= fechaLimite)
            .OrderBy(lote => lote.FechaVencimiento)
            .ThenBy(lote => lote.IdLote)
            .Select(Datos)
            .ToArray();
    }

    public IReadOnlyList<DatosRemesaDto> RemesasDisponibles()
    {
        return DatosDePrueba.Lotes(Hoy)
            .Where(TieneExistencias)
            .OrderBy(lote => lote.FechaVencimiento)
            .ThenBy(lote => lote.IdLote)
            .Select(Datos)
            .ToArray();
    }

    public void Descontar(int idLote, int cantidad)
    {
        ValidarLoteYCantidad(idLote, cantidad);
    }

    public void Restituir(int idLote, int cantidad)
    {
        ValidarLoteYCantidad(idLote, cantidad);
    }

    public void Bloquear(int idLote)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idLote);
    }

    public void Liberar(int idLote)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idLote);
    }

    private static void ValidarLoteYCantidad(int idLote, int cantidad)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idLote);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cantidad);
    }

    private static ProductoDePrueba Producto(LoteDePrueba lote)
    {
        return DatosDePrueba.Productos.Single(producto => producto.IdProducto == lote.IdProducto);
    }

    private static bool TieneExistencias(LoteDePrueba lote)
    {
        return lote.StockActual > 0 && EstadosConExistencias.Contains(lote.Estado);
    }

    private static bool EsVendible(LoteDePrueba lote, DateOnly hoy)
    {
        return Producto(lote).Estado == EstadoProducto.Disponible
            && lote.Estado == EstadoLote.Disponible
            && lote.StockActual > 0
            && lote.FechaVencimiento > hoy;
    }

    private static DatosRemesaDto Datos(LoteDePrueba lote)
    {
        return new DatosRemesaDto
        {
            IdLote = lote.IdLote,
            NumeroLote = lote.NumeroLote,
            FechaVencimiento = lote.FechaVencimiento,
            NombreMedicamento = Producto(lote).Nombre,
            IdProveedor = lote.IdProveedor,
            EstadoLote = lote.Estado,
            StockActual = lote.StockActual,
        };
    }
}
