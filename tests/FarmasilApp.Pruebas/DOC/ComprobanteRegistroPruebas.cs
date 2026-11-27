using FarmasilApp.Modelo.DOC;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.VEN;
using FarmasilApp.Pruebas.VistaModelo;
using FarmasilApp.VistaModelo.DOC;
namespace FarmasilApp.Pruebas.DOC;

/// <summary>
/// ESP-0009: lista de ventas pendientes, monto tomado de la venta y flujo de la VistaModelo.
/// </summary>
public class ComprobanteRegistroPruebas
{
    private const string FechaDeEmision = "24/11/2026";

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-004")]
    public void LaListaPendienteExcluyeLasVentasConComprobanteEmitido()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            IdsVentasConComprobanteEmitido = new[] { 1 }
        };
        var servicio = CrearServicio(repositorio, CrearVentas());

        var pendientes = servicio.ConsultarVentasPendientes();

        Assert.Equal(new[] { 2, 3 }, pendientes.Select(v => v.IdVenta));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-004")]
    public void LaListaPendienteIncluyeLaVentaCuyoComprobanteEstaAnulado()
    {
        // La venta 3 solo tiene un comprobante Anulado, así que el repositorio no la informa como Emitida.
        var repositorio = new ComprobanteRepositorioFalso
        {
            IdsVentasConComprobanteEmitido = new[] { 1 }
        };
        var servicio = CrearServicio(repositorio, CrearVentas());

        var pendientes = servicio.ConsultarVentasPendientes();

        Assert.Contains(pendientes, v => v.IdVenta == 3);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-006")]
    public void AlSeleccionarUnaVentaSeMuestraSuMontoYNoSePuedeEditar()
    {
        var vistaModelo = CrearVistaModelo(new ComprobanteRepositorioFalso());

        vistaModelo.VentaSeleccionada = vistaModelo.VentasPendientes.Single(v => v.IdVenta == 2);

        Assert.Equal(4.00m, vistaModelo.MontoTotalSeleccionado);
        Assert.Null(typeof(ComprobantesViewModel)
            .GetProperty(nameof(ComprobantesViewModel.MontoTotalSeleccionado))!.SetMethod);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-004")]
    public void AlIniciarLaListaDeVentasNoIncluyeLasEmitidas()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            IdsVentasConComprobanteEmitido = new[] { 1 }
        };

        var vistaModelo = CrearVistaModelo(repositorio);

        Assert.Equal(new[] { 2, 3 }, vistaModelo.VentasPendientes.Select(v => v.IdVenta));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-002")]
    public void ConfirmarLaEmisionRegistraYActualizaLaTablaDeComprobantes()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var vistaModelo = CrearVistaModelo(repositorio);
        PrepararEmision(vistaModelo, idVenta: 2, numero: "B001-000099");

        vistaModelo.ConfirmarEmisionCommand.Execute(null);

        Assert.Equal(2, repositorio.UltimaVentaRegistrada);
        Assert.Equal("B001-000099", repositorio.UltimoNumeroComprobanteRegistrado);
        Assert.Contains(vistaModelo.Comprobantes, c => c.NumeroComprobante == "B001-000099");
        Assert.DoesNotContain(vistaModelo.VentasPendientes, v => v.IdVenta == 2);
        Assert.False(vistaModelo.PrevisualizacionVisible);
        Assert.Equal(MensajesComprobantes.RegistroCorrecto, vistaModelo.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-002")]
    public void CancelarLaEmisionNoRegistraNada()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var vistaModelo = CrearVistaModelo(repositorio);
        PrepararEmision(vistaModelo, idVenta: 2, numero: "B001-000099");

        vistaModelo.CancelarEmisionCommand.Execute(null);

        Assert.Null(repositorio.UltimaVentaRegistrada);
        Assert.False(vistaModelo.PrevisualizacionVisible);
        Assert.Contains(vistaModelo.VentasPendientes, v => v.IdVenta == 2);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-001")]
    public void UnNumeroRepetidoMuestraElMensajeYConservaLaVentaPendiente()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[]
            {
                new ComprobanteTributario
                {
                    NumeroComprobante = "B001-000001",
                    TipoComprobante = "Boleta",
                    IdVenta = 1,
                    EstadoDocumento = "Emitido"
                }
            }
        };
        var vistaModelo = CrearVistaModelo(repositorio);
        PrepararEmision(vistaModelo, idVenta: 2, numero: "B001-000001");

        vistaModelo.ConfirmarEmisionCommand.Execute(null);

        Assert.Equal(MensajesComprobantes.NumeroDuplicado, vistaModelo.Mensaje);
        Assert.Null(repositorio.UltimaVentaRegistrada);
        Assert.Contains(vistaModelo.VentasPendientes, v => v.IdVenta == 2);
    }

    private static void PrepararEmision(ComprobantesViewModel vistaModelo, int idVenta, string numero)
    {
        vistaModelo.VentaSeleccionada = vistaModelo.VentasPendientes.Single(v => v.IdVenta == idVenta);
        vistaModelo.TipoComprobanteRegistro = "Boleta";
        vistaModelo.NumeroComprobanteRegistro = numero;
        vistaModelo.FechaEmisionRegistroTexto = FechaDeEmision;
        vistaModelo.CrearComprobanteCommand.Execute(null);
    }

    private static ComprobantesViewModel CrearVistaModelo(ComprobanteRepositorioFalso repositorio)
    => new(
        CrearServicio(repositorio, CrearVentas()),
        new PdfServicioFalso(),
        new SesionSimulada()
    );
    private static ComprobanteServicio CrearServicio(ComprobanteRepositorioFalso repositorio, VentasFalsas ventas)
        => new(repositorio, ventas, new SesionSimulada());

    private static VentasFalsas CrearVentas()
    {
        var ventas = new VentasFalsas();
        ventas.Agregar(Venta(1, 5.00m));
        ventas.Agregar(Venta(2, 4.00m));
        ventas.Agregar(Venta(3, 2.50m));
        return ventas;
    }

    private static VentaDto Venta(int id, decimal monto) => new()
    {
        IdVenta = id,
        FechaHora = new DateTime(2026, 11, 20, 10, 0, 0),
        MontoTotal = monto,
        IdMetodoPago = 1,
        IdUsuario = 2,
        Detalle = new[]
        {
            new DetalleVentaDto
            {
                IdLote = 1,
                NombreProducto = "Paracetamol",
                NumeroLote = "PAR-001",
                Cantidad = 1,
                PrecioUnitario = monto
            }
        }
    };
}
