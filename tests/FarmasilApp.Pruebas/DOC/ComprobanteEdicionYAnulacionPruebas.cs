using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DOC;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.VEN;

namespace FarmasilApp.Pruebas.DOC;

public sealed class ComprobanteEdicionYAnulacionPruebas
{
    [Fact]
    [Trait("Especificacion", "ESP-0011")]
    public void ActualizarCambiaDatosCorregiblesYConservaLaVenta()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[] { CrearComprobante(5, 17, "B001-0005", "Emitido") }
        };
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        servicio.Actualizar(5, "Factura", "F001-0099", "25/11/2026");

        var actualizado = Assert.Single(repositorio.Resultados);
        Assert.Equal(5, repositorio.UltimoIdActualizado);
        Assert.Equal("F001-0099", actualizado.NumeroComprobante);
        Assert.Equal("Factura", actualizado.TipoComprobante);
        Assert.Equal(new DateTime(2026, 11, 25), actualizado.FechaEmision);
        Assert.Equal(17, actualizado.IdVenta);
        Assert.Equal("Emitido", actualizado.EstadoDocumento);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0011")]
    public void ActualizarRechazaComprobanteAnulado()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[] { CrearComprobante(5, 17, "B001-0005", "Anulado") }
        };
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        var error = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Actualizar(5, "Boleta", "B001-0006", "25/11/2026"));

        Assert.Equal(MensajesComprobantes.ComprobanteAnuladoNoEditable, error.Message);
        Assert.Null(repositorio.UltimoIdActualizado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0011")]
    public void ActualizarRechazaNumeroQueYaPerteneceAOtroComprobante()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[]
            {
                CrearComprobante(5, 17, "B001-0005", "Emitido"),
                CrearComprobante(6, 18, "F001-0006", "Anulado")
            }
        };
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        var error = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Actualizar(5, "Boleta", "F001-0006", "25/11/2026"));

        Assert.Equal(MensajesComprobantes.NumeroDuplicado, error.Message);
        Assert.Null(repositorio.UltimoIdActualizado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0012")]
    public void AnularConservaElComprobanteYLiberaLaVentaParaNuevaDocumentacion()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[] { CrearComprobante(5, 17, "B001-0005", "Emitido") },
            IdsVentasConComprobanteEmitido = new[] { 17 }
        };
        var ventas = new VentasFalsas();
        ventas.Agregar(new VentaDto { IdVenta = 17, FechaHora = new DateTime(2026, 11, 25) });
        var servicio = CrearServicio(repositorio, ventas);

        servicio.Anular(5);

        var conservado = Assert.Single(repositorio.Resultados);
        Assert.Equal(5, repositorio.UltimoIdAnulado);
        Assert.Equal("Anulado", conservado.EstadoDocumento);
        Assert.Equal(17, conservado.IdVenta);
        Assert.Contains(servicio.ConsultarVentasPendientes(), venta => venta.IdVenta == 17);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0011")]
    public void TecnicoNoPuedeActualizarComprobantes()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[] { CrearComprobante(5, 17, "B001-0005", "Emitido") }
        };
        var servicio = new ComprobanteServicio(repositorio, new VentasFalsas(), new SesionSimulada(Rol.Tecnico));

        var error = Assert.Throws<AccesoDenegadoException>(
            () => servicio.Actualizar(5, "Boleta", "B001-0006", "25/11/2026"));

        Assert.Equal(MensajesComprobantes.AccesoAdministracionDenegado, error.Message);
        Assert.Null(repositorio.UltimoIdActualizado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0012")]
    public void TecnicoNoPuedeAnularComprobantes()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[] { CrearComprobante(5, 17, "B001-0005", "Emitido") }
        };
        var servicio = new ComprobanteServicio(repositorio, new VentasFalsas(), new SesionSimulada(Rol.Tecnico));

        Assert.Throws<AccesoDenegadoException>(() => servicio.Anular(5));
        Assert.Null(repositorio.UltimoIdAnulado);
    }

    private static ComprobanteServicio CrearServicio(
        ComprobanteRepositorioFalso repositorio,
        VentasFalsas ventas) => new(repositorio, ventas, new SesionSimulada());

    private static ComprobanteTributario CrearComprobante(
        int idComprobante,
        int idVenta,
        string numero,
        string estado) => new()
    {
        IdComprobante = idComprobante,
        IdVenta = idVenta,
        NumeroComprobante = numero,
        TipoComprobante = numero.StartsWith("F", StringComparison.Ordinal) ? "Factura" : "Boleta",
        FechaEmision = new DateTime(2026, 11, 20),
        EstadoDocumento = estado
    };
}
