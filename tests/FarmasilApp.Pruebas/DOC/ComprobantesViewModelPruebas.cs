using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DOC;
using FarmasilApp.Modelo.VEN;
using FarmasilApp.Pruebas.DOC;
using FarmasilApp.VistaModelo.DOC;

namespace FarmasilApp.Pruebas.VistaModelo;

public class ComprobantesViewModelPruebas
{
    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-012")]
    public void InicializaFiltrosYComandoDeConsulta()
    {
        var vistaModelo = Crear(new ComprobanteRepositorioFalso(), new VentasFalsas());

        Assert.Equal(new[] { "Todos", "Boleta", "Factura" }, vistaModelo.TiposComprobante);
        Assert.Equal("Todos", vistaModelo.TipoComprobanteSeleccionado);
        Assert.Empty(vistaModelo.Comprobantes);
        Assert.True(vistaModelo.ConsultarCommand.CanExecute(null));
        Assert.False(vistaModelo.Consultando);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-012")]
    public void ConsultarCargaResultadosYMuestraCantidad()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[]
            {
                new ComprobanteTributario
                {
                    IdComprobante = 1,
                    IdVenta = 10,
                    NumeroComprobante = "B001-000001",
                    TipoComprobante = "Boleta",
                    FechaEmision = new DateTime(2026, 11, 17),
                    EstadoDocumento = "Emitido"
                }
            }
        };
        var ventas = new VentasFalsas();
        ventas.Agregar(new VentaDto { IdVenta = 10, MontoTotal = 35.90m });
        var vistaModelo = Crear(repositorio, ventas);

        vistaModelo.ConsultarCommand.Execute(null);

        var comprobante = Assert.Single(vistaModelo.Comprobantes);
        Assert.Equal(35.90m, comprobante.MontoTotal);
        Assert.Equal("Se encontraron 1 comprobante(s).", vistaModelo.Mensaje);
        Assert.False(vistaModelo.Consultando);
        Assert.True(vistaModelo.ConsultarCommand.CanExecute(null));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-012")]
    public void ConsultaSinResultadosMuestraMensajeYListaVacia()
    {
        var vistaModelo = Crear(new ComprobanteRepositorioFalso(), new VentasFalsas());

        vistaModelo.ConsultarCommand.Execute(null);

        Assert.Empty(vistaModelo.Comprobantes);
        Assert.Equal("No se encontraron comprobantes con los filtros indicados.", vistaModelo.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-012")]
    public void ReglaDeNegocioMuestraSuMensajeYLimpiaResultados()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[]
            {
                new ComprobanteTributario { IdComprobante = 1, IdVenta = 10 }
            }
        };
        var vistaModelo = Crear(repositorio, new VentasFalsas());
        vistaModelo.ConsultarCommand.Execute(null);
        vistaModelo.TipoComprobanteSeleccionado = "Nota de crédito";

        vistaModelo.ConsultarCommand.Execute(null);

        Assert.Empty(vistaModelo.Comprobantes);
        Assert.Equal("El tipo de comprobante debe ser Boleta o Factura.", vistaModelo.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-012")]
    public void ErrorTecnicoMuestraMensajeGenericoYLimpiaResultados()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[]
            {
                new ComprobanteTributario { IdComprobante = 1, IdVenta = 10 }
            }
        };
        var vistaModelo = Crear(repositorio, new VentasFalsas());
        vistaModelo.ConsultarCommand.Execute(null);
        repositorio.FallarAlConsultar = true;

        vistaModelo.ConsultarCommand.Execute(null);

        Assert.Empty(vistaModelo.Comprobantes);
        Assert.Equal(
            "No se pudo realizar la consulta. Verifique la conexión con DB_FARMASIL.",
            vistaModelo.Mensaje);
        Assert.False(vistaModelo.Consultando);
        Assert.True(vistaModelo.ConsultarCommand.CanExecute(null));
    }

    private static ComprobantesViewModel Crear(
        ComprobanteRepositorioFalso repositorio,
        VentasFalsas ventas) =>
        new(new ComprobanteServicio(repositorio, ventas));
}
