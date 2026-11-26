using FarmasilApp.Modelo.VEN;

namespace FarmasilApp.Pruebas.DOC;

/// <summary>Implementación controlada de IVentas para probar la resolución del importe.</summary>
internal sealed class VentasFalsas : IVentas
{
    private readonly Dictionary<int, VentaDto> _ventas = new();

    public List<int> IdsConsultados { get; } = new();

    public void Agregar(VentaDto venta) => _ventas[venta.IdVenta] = venta;

    public IReadOnlyList<VentaDto> ConsultarVentas(FiltroVentas filtro) => _ventas.Values.OrderBy(v => v.IdVenta).ToArray();

    public VentaDto? ObtenerVenta(int idVenta)
    {
        IdsConsultados.Add(idVenta);
        return _ventas.GetValueOrDefault(idVenta);
    }
}
