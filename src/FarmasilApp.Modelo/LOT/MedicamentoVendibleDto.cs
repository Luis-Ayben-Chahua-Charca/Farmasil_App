namespace FarmasilApp.Modelo.LOT;

/// <summary>
/// Medicamento con alguna remesa vendible, tal como lo ofrece <see cref="ILotes.MedicamentosVendibles"/> (DEC-044).
/// </summary>
public sealed class MedicamentoVendibleDto
{
    public int IdProducto { get; init; }

    public string Nombre { get; init; } = string.Empty;

    public string AccionTerapeutica { get; init; } = string.Empty;

    public decimal PrecioVenta { get; init; }

    /// <summary>Suma de las existencias de las remesas vendibles del medicamento (DEC-040).</summary>
    public int StockVendible { get; init; }
}
