using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.VEN;

namespace FarmasilApp.Pruebas.VEN;

/// <summary>Detalle de la venta en curso (ESP-0001; DEC-036, DEC-067).</summary>
[Trait("Especificacion", "ESP-0001")]
[Trait("Tecnica", "T-002")]
public class CarritoVentaPruebas
{
    private static RemesaVendibleDto Remesa(int idLote, decimal precio, int idProducto = 1) => new()
    {
        IdLote = idLote,
        IdProducto = idProducto,
        NombreMedicamento = $"Medicamento {idProducto}",
        NumeroLote = $"LOTE-{idLote}",
        PrecioVenta = precio,
        FechaVencimiento = new DateOnly(2027, 11, 14),
        StockActual = 100,
    };

    [Fact]
    public void UnCarritoNuevoEstaVacioYSuTotalEsCero()
    {
        var carrito = new CarritoVenta();

        Assert.Empty(carrito.Lineas);
        Assert.Equal(0m, carrito.MontoTotal);
    }

    [Fact]
    public void AgregarUnaRemesaCreaUnaLineaConSuPrecioYSuSubtotal()
    {
        var carrito = new CarritoVenta();

        carrito.Agregar(Remesa(1, 2.50m), 3);

        var linea = Assert.Single(carrito.Lineas);
        Assert.Equal(1, linea.IdLote);
        Assert.Equal("LOTE-1", linea.NumeroLote);
        Assert.Equal(3, linea.Cantidad);
        Assert.Equal(2.50m, linea.PrecioUnitario);
        Assert.Equal(7.50m, linea.Subtotal);
        Assert.Equal(7.50m, carrito.MontoTotal);
    }

    [Fact]
    public void AgregarLaMismaRemesaAcumulaLaCantidadEnUnaSolaLinea()
    {
        var carrito = new CarritoVenta();
        carrito.Agregar(Remesa(1, 2.50m), 2);

        carrito.Agregar(Remesa(1, 2.50m), 3);

        var linea = Assert.Single(carrito.Lineas);
        Assert.Equal(5, linea.Cantidad);
        Assert.Equal(12.50m, carrito.MontoTotal);
        Assert.Equal(5, carrito.CantidadDe(1));
    }

    [Fact]
    public void LaLineaExistenteConservaSuPrecioUnitarioAunqueCambieElPrecioVigente()
    {
        var carrito = new CarritoVenta();
        carrito.Agregar(Remesa(1, 2.50m), 1);

        carrito.Agregar(Remesa(1, 9.99m), 1);

        Assert.Equal(2.50m, Assert.Single(carrito.Lineas).PrecioUnitario);
    }

    [Fact]
    public void ElTotalSumaTodasLasLineasSinErrorDeRedondeo()
    {
        var carrito = new CarritoVenta();
        carrito.Agregar(Remesa(1, 0.10m), 3);
        carrito.Agregar(Remesa(2, 0.20m, idProducto: 2), 3);

        Assert.Equal(0.90m, carrito.MontoTotal);
    }

    [Fact]
    public void QuitarRetiraSoloLaLineaIndicadaYActualizaElTotal()
    {
        var carrito = new CarritoVenta();
        carrito.Agregar(Remesa(1, 2.50m), 2);
        carrito.Agregar(Remesa(2, 4.00m, idProducto: 2), 1);

        var quitada = carrito.Quitar(1);

        Assert.True(quitada);
        Assert.Equal(new[] { 2 }, carrito.Lineas.Select(linea => linea.IdLote));
        Assert.Equal(4.00m, carrito.MontoTotal);
        Assert.Equal(0, carrito.CantidadDe(1));
    }

    [Fact]
    public void QuitarUnaRemesaQueNoFiguraNoCambiaNadaYDevuelveFalso()
    {
        var carrito = new CarritoVenta();
        carrito.Agregar(Remesa(1, 2.50m), 1);

        Assert.False(carrito.Quitar(99));

        Assert.Single(carrito.Lineas);
    }

    [Fact]
    public void CorregirUnaCantidadSeHaceQuitandoYVolviendoAAgregar()
    {
        var carrito = new CarritoVenta();
        carrito.Agregar(Remesa(1, 2.50m), 5);

        carrito.Quitar(1);
        carrito.Agregar(Remesa(1, 2.50m), 2);

        Assert.Equal(2, Assert.Single(carrito.Lineas).Cantidad);
    }

    [Fact]
    public void VaciarDejaElCarritoSinLineas()
    {
        var carrito = new CarritoVenta();
        carrito.Agregar(Remesa(1, 2.50m), 1);

        carrito.Vaciar();

        Assert.Empty(carrito.Lineas);
        Assert.Equal(0m, carrito.MontoTotal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UnaCantidadNoPositivaSeRechazaEnElCarrito(int cantidad)
    {
        var carrito = new CarritoVenta();

        Assert.Throws<ArgumentOutOfRangeException>(() => carrito.Agregar(Remesa(1, 2.50m), cantidad));
    }

    [Fact]
    public void UnaRemesaNulaSeRechaza()
    {
        Assert.Throws<ArgumentNullException>(() => new CarritoVenta().Agregar(null!, 1));
    }
}
