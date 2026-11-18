
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DOC;
using FarmasilApp.Modelo.VEN;

namespace FarmasilApp.Pruebas.DOC;

public class ComprobanteServicioPruebas
{
    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarNormalizaFiltrosYReduceLaFechaADia()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        var fecha = new DateTime(2026, 11, 17, 15, 42, 10);

        servicio.Consultar(" Boleta ", " B001-000123 ", fecha);

        Assert.NotNull(repositorio.UltimoFiltro);
        Assert.Equal("Boleta", repositorio.UltimoFiltro.TipoComprobante);
        Assert.Equal("B001-000123", repositorio.UltimoFiltro.NumeroComprobante);
        Assert.Equal(fecha.Date, repositorio.UltimoFiltro.FechaEmision);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-002")]
    public void TodosYLosFiltrosVaciosSeConviertenEnNulos()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        servicio.Consultar("Todos", "   ", null);

        Assert.NotNull(repositorio.UltimoFiltro);
        Assert.Null(repositorio.UltimoFiltro.TipoComprobante);
        Assert.Null(repositorio.UltimoFiltro.NumeroComprobante);
        Assert.Null(repositorio.UltimoFiltro.FechaEmision);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-012")]
    [InlineData("Nota de crédito")]
    [InlineData("boleta electrónica")]
    public void TipoNoPermitidoLanzaReglaDeNegocioException(string tipo)
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Consultar(tipo, null, null));

        Assert.Equal(
            "El tipo de comprobante debe ser Boleta o Factura.",
            excepcion.Message);

        Assert.Null(repositorio.UltimoFiltro);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-012")]
    public void NumeroMayorAVeinteCaracteresLanzaReglaDeNegocioException()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Consultar(null, "123456789012345678901", null));

        Assert.Equal(
            "El número de comprobante no puede superar 20 caracteres.",
            excepcion.Message);

        Assert.Null(repositorio.UltimoFiltro);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-002")]
    public void ConsultaObtieneElImporteMedianteIVentasYConservaComprobantesAnulados()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[]
            {
                Comprobante(10, 25, "B001-000010", "Anulado"),
                Comprobante(11, 26, "F001-000011", "Emitido")
            }
        };

        var ventas = new VentasFalsas();

        ventas.Agregar(new VentaDto
        {
            IdVenta = 25,
            MontoTotal = 18.50m
        });

        ventas.Agregar(new VentaDto
        {
            IdVenta = 26,
            MontoTotal = 42.00m
        });

        var resultados = CrearServicio(repositorio, ventas)
            .Consultar(null, null, null);

        Assert.Equal(new[] { 25, 26 }, ventas.IdsConsultados);

        Assert.Equal(
            new[] { 18.50m, 42.00m },
            resultados.Select(item => item.MontoTotal));

        Assert.Equal("Anulado", resultados[0].EstadoDocumento);
        Assert.Equal("Emitido", resultados[1].EstadoDocumento);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-002")]
    public void SiIVentasNoEncuentraLaVentaElImporteQuedaEnCero()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[]
            {
                Comprobante(1, 999, "B001-000001", "Emitido")
            }
        };

        var resultado = Assert.Single(
            CrearServicio(repositorio, new VentasFalsas())
                .Consultar(null, null, null));

        Assert.Equal(0m, resultado.MontoTotal);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-012")]
    public void RepositorioYVentasSonObligatorios()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ComprobanteServicio(null!, new VentasFalsas()));

        Assert.Throws<ArgumentNullException>(
            () => new ComprobanteServicio(
                new ComprobanteRepositorioFalso(),
                null!));
    }

    private static ComprobanteServicio CrearServicio(
        ComprobanteRepositorioFalso repositorio,
        VentasFalsas ventas)
    {
        return new ComprobanteServicio(repositorio, ventas);
    }

    private static ComprobanteTributario Comprobante(
        int idComprobante,
        int idVenta,
        string numero,
        string estado)
    {
        return new ComprobanteTributario
        {
            IdComprobante = idComprobante,
            IdVenta = idVenta,
            NumeroComprobante = numero,
            TipoComprobante = numero.StartsWith(
                "F",
                StringComparison.Ordinal) ? "Factura" : "Boleta",
            FechaEmision = new DateTime(2026, 11, 17),
            EstadoDocumento = estado
        };
    }
}
