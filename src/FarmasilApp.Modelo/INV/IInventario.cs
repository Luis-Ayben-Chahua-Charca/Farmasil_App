namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Interfaz provista por Inventario. La usan Restricciones de Venta, Lotes, Reabastecimiento y Comparación de Precios.
/// Firma formal: matriz de interfaces, sección 5 bis (DEC-069).
/// </summary>
public interface IInventario
{
    /// <summary>Todos los medicamentos del catálogo, también los descontinuados, por identificador.</summary>
    IReadOnlyList<MedicamentoDto> Medicamentos();

    /// <summary>Medicamento con ese identificador o <c>null</c> si no existe.</summary>
    /// <exception cref="ArgumentOutOfRangeException">El identificador no es positivo.</exception>
    MedicamentoDto? ObtenerMedicamento(int idProducto);
}
