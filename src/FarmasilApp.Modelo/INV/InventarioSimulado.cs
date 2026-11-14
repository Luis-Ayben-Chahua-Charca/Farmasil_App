using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Implementación simulada de <see cref="IInventario"/> mientras INV no esté listo (plan, sección 10).
/// Devuelve siempre los medicamentos de database/002_datos_prueba.sql, con los mismos ids
/// (los que tiene una base recién creada). No tiene estado.
/// </summary>
public sealed class InventarioSimulado : IInventario
{
    private static readonly IReadOnlyList<MedicamentoDto> Todos = Array.AsReadOnly(
        DatosDePrueba.Productos
            .Select(producto => new MedicamentoDto
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                AccionTerapeutica = producto.AccionTerapeutica,
                PrecioVenta = producto.PrecioVenta,
                Estado = producto.Estado,
                IdProveedor = producto.IdProveedor,
                StockMinimo = producto.StockMinimo,
            })
            .ToArray());

    public IReadOnlyList<MedicamentoDto> Medicamentos()
    {
        return Todos;
    }

    public MedicamentoDto? ObtenerMedicamento(int idProducto)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idProducto);

        return Todos.FirstOrDefault(medicamento => medicamento.IdProducto == idProducto);
    }
}
