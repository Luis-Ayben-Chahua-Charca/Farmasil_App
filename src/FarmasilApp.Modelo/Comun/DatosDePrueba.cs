namespace FarmasilApp.Modelo.Comun;

/// <summary>Medicamento de database/002_datos_prueba.sql.</summary>
internal sealed record ProductoDePrueba(
    int IdProducto,
    string Nombre,
    string AccionTerapeutica,
    decimal PrecioVenta,
    EstadoProducto Estado,
    int IdProveedor,
    int StockMinimo);

/// <summary>Remesa de database/002_datos_prueba.sql.</summary>
internal sealed record LoteDePrueba(
    int IdLote,
    int IdProducto,
    string NumeroLote,
    DateOnly FechaVencimiento,
    int StockActual,
    EstadoLote Estado,
    int IdProveedor);

/// <summary>
/// Fuente única de los datos fijos que usan las implementaciones simuladas de los módulos que dependen del catálogo y de las remesas
/// (Inventario, Lotes). Reproduce database/002_datos_prueba.sql con los mismos ids, que son los de una base recién creada (IDENTITY).
/// Los vencimientos son relativos a la fecha de hoy, como en el script (DATEADD(MONTH, n, hoy)). Se borra junto con las simuladas.
/// </summary>
internal static class DatosDePrueba
{
    public static IReadOnlyList<ProductoDePrueba> Productos { get; } = Array.AsReadOnly(new[]
    {
        new ProductoDePrueba(1, "Paracetamol 500 mg", "Analgesico y antipiretico", 2.50m, EstadoProducto.Disponible, 1, 10),
        new ProductoDePrueba(2, "Ibuprofeno 400 mg", "Antiinflamatorio", 4.00m, EstadoProducto.Disponible, 2, 8),
        new ProductoDePrueba(3, "Amoxicilina 500 mg", "Antibiotico", 6.50m, EstadoProducto.Disponible, 3, 10),
        new ProductoDePrueba(4, "Loratadina 10 mg", "Antihistaminico", 3.00m, EstadoProducto.Disponible, 1, 5),
        new ProductoDePrueba(5, "Diclofenaco 50 mg", "Antiinflamatorio", 5.00m, EstadoProducto.Disponible, 3, 6),
        new ProductoDePrueba(6, "Omeprazol 20 mg", "Gastroprotector", 4.50m, EstadoProducto.Disponible, 2, 8),
        new ProductoDePrueba(7, "Producto Descontinuado", "Producto historico", 2.00m, EstadoProducto.Descontinuado, 1, 0),
    });

    /// <summary>
    /// Las seis remesas del script: PAR-001 disponible, PAR-002 bloqueada, IBU-VENC-001 vencida, AMO-PROX-001 por vencer,
    /// LOR-AGOT-001 agotada y DIC-BLOQ-001 bloqueada. Omeprazol queda sin remesas, a propósito.
    /// </summary>
    public static IReadOnlyList<LoteDePrueba> Lotes(DateOnly hoy) => Array.AsReadOnly(new[]
    {
        new LoteDePrueba(1, 1, "PAR-001", hoy.AddMonths(12), 20, EstadoLote.Disponible, 1),
        new LoteDePrueba(2, 1, "PAR-002", hoy.AddMonths(10), 5, EstadoLote.BloqueadoPorDevolucion, 1),
        new LoteDePrueba(3, 2, "IBU-VENC-001", hoy.AddMonths(-1), 8, EstadoLote.Disponible, 2),
        new LoteDePrueba(4, 3, "AMO-PROX-001", hoy.AddMonths(1), 12, EstadoLote.Disponible, 3),
        new LoteDePrueba(5, 4, "LOR-AGOT-001", hoy.AddMonths(6), 0, EstadoLote.Agotado, 1),
        new LoteDePrueba(6, 5, "DIC-BLOQ-001", hoy.AddMonths(8), 6, EstadoLote.BloqueadoPorDevolucion, 3),
    });
}
