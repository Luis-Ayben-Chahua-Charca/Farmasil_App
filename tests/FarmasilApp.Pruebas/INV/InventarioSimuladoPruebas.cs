using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.INV;

namespace FarmasilApp.Pruebas.INV;

/// <summary>Los datos esperados son los de database/002_datos_prueba.sql: siete medicamentos con ids 1 a 7.</summary>
public class InventarioSimuladoPruebas
{
    [Fact]
    public void LosMedicamentosSonLosSieteDeLosDatosDePrueba()
    {
        IInventario inventario = new InventarioSimulado();

        var medicamentos = inventario.Medicamentos();

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7 }, medicamentos.Select(m => m.IdProducto));
    }

    [Fact]
    public void ElParacetamolTraeSusDatosDeCatalogo()
    {
        var paracetamol = new InventarioSimulado().ObtenerMedicamento(1);

        Assert.NotNull(paracetamol);
        Assert.Equal("Paracetamol 500 mg", paracetamol.Nombre);
        Assert.Equal("Analgesico y antipiretico", paracetamol.AccionTerapeutica);
        Assert.Equal(2.50m, paracetamol.PrecioVenta);
        Assert.Equal(EstadoProducto.Disponible, paracetamol.Estado);
        Assert.Equal(1, paracetamol.IdProveedor);
        Assert.Equal(10, paracetamol.StockMinimo);
    }

    [Fact]
    public void ElMedicamentoDescontinuadoSeDevuelveConSuEstado()
    {
        var descontinuado = new InventarioSimulado().ObtenerMedicamento(7);

        Assert.NotNull(descontinuado);
        Assert.Equal(EstadoProducto.Descontinuado, descontinuado.Estado);
        Assert.Equal(0, descontinuado.StockMinimo);
    }

    [Fact]
    public void UnMedicamentoQueNoExisteDevuelveNulo()
    {
        Assert.Null(new InventarioSimulado().ObtenerMedicamento(99));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ObtenerConUnIdentificadorNoPositivoFalla(int idProducto)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new InventarioSimulado().ObtenerMedicamento(idProducto));
    }

    [Fact]
    public void ElProveedorDeCadaMedicamentoExisteEnLosProveedoresDePrueba()
    {
        // Los ids de proveedor del catálogo son los de IProveedores (1 a 3 activos).
        var medicamentos = new InventarioSimulado().Medicamentos();

        Assert.All(medicamentos, m => Assert.InRange(m.IdProveedor, 1, 3));
    }
}
