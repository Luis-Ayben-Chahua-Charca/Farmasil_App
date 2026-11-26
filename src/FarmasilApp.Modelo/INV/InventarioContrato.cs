using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Adaptador real del contrato IInventario respaldado por IInventarioRepositorio.
/// Permite que otros módulos consulten el catálogo vigente sin acceder directamente
/// a las tablas propiedad de INV.
/// </summary>
public sealed class InventarioContrato : IInventario
{
    private readonly IInventarioRepositorio _repositorio;

    public InventarioContrato(
        IInventarioRepositorio repositorio)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public IReadOnlyList<MedicamentoDto> Medicamentos()
    {
        return _repositorio
            .Consultar(
                null,
                null,
                null)
            .Select(Convertir)
            .ToArray();
    }

    public MedicamentoDto? ObtenerMedicamento(
        int idProducto)
    {
        ArgumentOutOfRangeException
            .ThrowIfNegativeOrZero(
                idProducto);

        return _repositorio
            .Consultar(
                idProducto.ToString(),
                null,
                null)
            .Where(
                producto =>
                    producto.IdProducto == idProducto)
            .Select(Convertir)
            .SingleOrDefault();
    }

    private static MedicamentoDto Convertir(
        ProductoInventario producto)
    {
        if (!Enum.TryParse<EstadoProducto>(
                producto.EstadoProducto,
                ignoreCase: false,
                out var estado))
        {
            throw new InvalidOperationException(
                $"Estado de producto desconocido: '{producto.EstadoProducto}'.");
        }

        return new MedicamentoDto
        {
            IdProducto = producto.IdProducto,
            Nombre = producto.Nombre,
            AccionTerapeutica = producto.AccionTerapeutica,
            PrecioVenta = producto.PrecioVenta,
            Estado = estado,
            IdProveedor = producto.IdProveedor,
            StockMinimo = producto.StockMinimo,
        };
    }
}
