using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DOC;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.VEN;
using FarmasilApp.Pruebas.DOC;
using FarmasilApp.VistaModelo.DOC;

namespace FarmasilApp.Pruebas.VistaModelo;

public class ComprobantesViewModelPruebas
{
    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-002")]
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
    [Trait("Tecnica", "T-002")]
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
    [Trait("Tecnica", "T-002")]
    public void ConsultaSinResultadosMuestraMensajeYListaVacia()
    {
        var vistaModelo = Crear(new ComprobanteRepositorioFalso(), new VentasFalsas());

        vistaModelo.ConsultarCommand.Execute(null);

        Assert.Empty(vistaModelo.Comprobantes);
        Assert.Equal(MensajesComprobantes.SinResultados, vistaModelo.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-001")]
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
        Assert.Equal(MensajesComprobantes.TipoInvalido, vistaModelo.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-002")]
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
            MensajesComprobantes.ErrorConsulta,
            vistaModelo.Mensaje);
        Assert.False(vistaModelo.Consultando);
        Assert.True(vistaModelo.ConsultarCommand.CanExecute(null));
    }

    [Theory]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-001")]
    [InlineData(false)]
    [InlineData(true)]
    public void FechaInvalidaCancelaInclusoDespuesDeUnaConsultaConFechaValida(bool consultaPrevia)
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[] { new ComprobanteTributario { IdVenta = 1 } }
        };
        var vistaModelo = Crear(repositorio, new VentasFalsas());
        if (consultaPrevia)
        {
            vistaModelo.FechaEmision = new DateTime(2026, 11, 17);
            vistaModelo.ConsultarCommand.Execute(null);
            Assert.Single(vistaModelo.Comprobantes);
        }

        var llamadasPrevias = repositorio.Consultas;
        vistaModelo.FechaEmisionTexto = "31/02/2026";
        vistaModelo.ConsultarCommand.Execute(null);

        Assert.Equal(llamadasPrevias, repositorio.Consultas);
        Assert.Empty(vistaModelo.Comprobantes);
        Assert.Equal("31/02/2026", vistaModelo.FechaEmisionTexto);
        Assert.Equal(MensajesComprobantes.FechaInvalida, vistaModelo.Mensaje);
        Assert.True(vistaModelo.ConsultarCommand.CanExecute(null));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-002")]
    public void FechaDelCalendarioAlimentaElFiltroYElTextoVacioLoQuita()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var vistaModelo = Crear(repositorio, new VentasFalsas());
        vistaModelo.FechaEmision = new DateTime(2026, 11, 17);
        Assert.Equal("17/11/2026", vistaModelo.FechaEmisionTexto);
        vistaModelo.ConsultarCommand.Execute(null);
        Assert.Equal(new DateTime(2026, 11, 17), repositorio.UltimoFiltro!.FechaEmision);

        vistaModelo.FechaEmisionTexto = string.Empty;
        vistaModelo.ConsultarCommand.Execute(null);
        Assert.Null(repositorio.UltimoFiltro!.FechaEmision);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-001")]
    public void ElCalendarioPermiteVolverALaMismaFechaTrasUnaEntradaInvalida()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var vistaModelo = Crear(repositorio, new VentasFalsas());
        var fecha = new DateTime(2026, 11, 17);
        vistaModelo.FechaEmision = fecha;
        vistaModelo.FechaEmisionTexto = "31/02/2026";
        Assert.Null(vistaModelo.FechaEmision);

        vistaModelo.FechaEmision = fecha;
        vistaModelo.ConsultarCommand.Execute(null);
        Assert.Equal("17/11/2026", vistaModelo.FechaEmisionTexto);
        Assert.Equal(fecha, repositorio.UltimoFiltro!.FechaEmision);
    }

    private static ComprobantesViewModel Crear(
        ComprobanteRepositorioFalso repositorio,
        VentasFalsas ventas) =>
        new(new ComprobanteServicio(repositorio, ventas, new SesionSimulada()));
}
