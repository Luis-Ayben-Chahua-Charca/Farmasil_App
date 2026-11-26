using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.RES;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.VEN;

namespace FarmasilApp.Pruebas.VEN;

/// <summary>
/// ESP-0001 contra los contratos simulados (plan, sección 10), con los datos de database/002_datos_prueba.sql: remesa 1
/// PAR-001 vendible con 20 unidades, remesa 2 bloqueada, remesa 3 vencida, remesa 4 AMO-PROX-001 de un producto con
/// restricción bloqueante y remesa 5 agotada. El producto 1 tiene una restricción informativa.
/// </summary>
[Trait("Especificacion", "ESP-0001")]
[Trait("Tecnica", "T-002")]
public class VentaServicioPruebas
{
    private static readonly DateOnly Hoy = new(2026, 11, 14);

    private static VentaServicio CrearServicio(Rol rol = Rol.Administrador)
    {
        var reloj = new RelojFijo(Hoy);
        return new VentaServicio(new LotesSimulado(reloj), new RestriccionesSimulado(), new PagosSimulado(), new SesionSimulada(rol), reloj);
    }

    private static RemesaVendibleDto RemesaDe(int idLote, int idProducto, decimal precio = 2.50m) => new()
    {
        IdLote = idLote,
        IdProducto = idProducto,
        NombreMedicamento = $"Medicamento {idProducto}",
        NumeroLote = $"LOTE-{idLote}",
        PrecioVenta = precio,
        FechaVencimiento = Hoy.AddMonths(6),
        StockActual = 20,
    };

    [Fact]
    public void ElCatalogoListaSoloLosMedicamentosVendibles()
    {
        var medicamentos = CrearServicio().MedicamentosVendibles();

        // Ibuprofeno (remesa vencida), Loratadina (agotada), Diclofenaco (bloqueada), Omeprazol (sin remesas) y el descontinuado no aparecen.
        Assert.Equal(new[] { 1, 3 }, medicamentos.Select(m => m.IdProducto));
    }

    [Fact]
    public void LaBusquedaFiltraPorNombreSinDistinguirMayusculas()
    {
        var medicamentos = CrearServicio().MedicamentosVendibles("  AMOXI ");

        Assert.Equal(3, Assert.Single(medicamentos).IdProducto);
    }

    [Fact]
    public void ElFiltroDeAccionTerapeuticaSeAplicaJuntoConLaBusqueda()
    {
        var servicio = CrearServicio();

        Assert.Equal(1, Assert.Single(servicio.MedicamentosVendibles(null, "Analgesico y antipiretico")).IdProducto);
        Assert.Empty(servicio.MedicamentosVendibles("amoxi", "Analgesico y antipiretico"));
    }

    [Fact]
    public void SinCriteriosNoFiltra()
    {
        Assert.Equal(2, CrearServicio().MedicamentosVendibles("  ", "").Count);
    }

    [Fact]
    public void LaRemesaFefoSugeridaEsLaDeVencimientoMasProximo()
    {
        var remesas = CrearServicio().RemesasSugeridas(1);

        // La remesa 2 (PAR-002) está bloqueada: solo la 1 es vendible.
        Assert.Equal(new[] { 1 }, remesas.Select(r => r.IdLote));
    }

    [Fact]
    public void UnMedicamentoSinRemesasVendiblesNoTieneSugerencia()
    {
        Assert.Empty(CrearServicio().RemesasSugeridas(6));
    }

    [Fact]
    public void LosMetodosDePagoSonLosActivosDeIPagos()
    {
        var metodos = CrearServicio().MetodosPagoActivos();

        Assert.Equal(new[] { "Efectivo", "Yape" }, metodos.Select(m => m.Nombre));
    }

    [Fact]
    public void AgregarUnaLineaValidaLaDejaEnElCarritoConElPrecioDelMedicamento()
    {
        var servicio = CrearServicio();
        var carrito = new CarritoVenta();
        var remesa = servicio.RemesasSugeridas(1).First();

        servicio.AgregarLinea(carrito, remesa, 4);

        var linea = Assert.Single(carrito.Lineas);
        Assert.Equal((1, 4, 2.50m), (linea.IdLote, linea.Cantidad, linea.PrecioUnitario));
        Assert.Equal(10.00m, carrito.MontoTotal);
    }

    [Fact]
    public void AgregarLaMismaRemesaAcumulaYValidaElStockConLaCantidadTotal()
    {
        var servicio = CrearServicio();
        var carrito = new CarritoVenta();
        var remesa = servicio.RemesasSugeridas(1).First();
        servicio.AgregarLinea(carrito, remesa, 15);

        // 15 + 6 = 21 supera las 20 unidades de la remesa, aunque 6 solo no las supere.
        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => servicio.AgregarLinea(carrito, remesa, 6));

        Assert.Equal(string.Format(MensajesVentas.StockInsuficiente, 20), excepcion.Message);
        Assert.Equal(15, carrito.CantidadDe(1));

        servicio.AgregarLinea(carrito, remesa, 5);
        Assert.Equal(20, Assert.Single(carrito.Lineas).Cantidad);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void UnaCantidadNoPositivaSeRechazaConSuMensaje(int cantidad)
    {
        var servicio = CrearServicio();
        var carrito = new CarritoVenta();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.AgregarLinea(carrito, servicio.RemesasSugeridas(1).First(), cantidad));

        Assert.Equal(MensajesVentas.CantidadInvalida, excepcion.Message);
        Assert.Empty(carrito.Lineas);
    }

    [Fact]
    public void SinRemesaElegidaSeRechaza()
    {
        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => CrearServicio().AgregarLinea(new CarritoVenta(), null, 1));

        Assert.Equal(MensajesVentas.SinRemesa, excepcion.Message);
    }

    [Fact]
    public void UnaRemesaVencidaSeRechazaConElMensajeDeLaEspecificacion()
    {
        var carrito = new CarritoVenta();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => CrearServicio().AgregarLinea(carrito, RemesaDe(3, 2), 1));

        Assert.Equal(MensajesVentas.RemesaVencida, excepcion.Message);
        Assert.Empty(carrito.Lineas);
    }

    [Fact]
    public void UnaRemesaBloqueadaPorAlertaSeRechazaConElMensajeDeLaEspecificacion()
    {
        var carrito = new CarritoVenta();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => CrearServicio().AgregarLinea(carrito, RemesaDe(2, 1), 1));

        Assert.Equal(MensajesVentas.RemesaConAlerta, excepcion.Message);
        Assert.Empty(carrito.Lineas);
    }

    [Fact]
    public void UnaRemesaAgotadaSeRechazaPorStock()
    {
        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => CrearServicio().AgregarLinea(new CarritoVenta(), RemesaDe(5, 4), 1));

        Assert.Equal(string.Format(MensajesVentas.StockInsuficiente, 0), excepcion.Message);
    }

    [Fact]
    public void UnaRemesaQueNoExisteSeRechaza()
    {
        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => CrearServicio().AgregarLinea(new CarritoVenta(), RemesaDe(99, 1), 1));

        Assert.Equal(MensajesVentas.RemesaNoDisponible, excepcion.Message);
    }

    [Fact]
    public void UnaRestriccionBloqueanteRechazaLaLineaConSuMotivo()
    {
        var servicio = CrearServicio();
        var carrito = new CarritoVenta();
        var remesa = servicio.RemesasSugeridas(3).First();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => servicio.AgregarLinea(carrito, remesa, 1));

        Assert.Equal("No completar la venta si el cliente presenta alergia.", excepcion.Message);
        Assert.Empty(carrito.Lineas);
    }

    [Fact]
    public void UnaRestriccionInformativaAgregaLaLineaYDevuelveSuAviso()
    {
        var servicio = CrearServicio();
        var carrito = new CarritoVenta();

        var avisos = servicio.AgregarLinea(carrito, servicio.RemesasSugeridas(1).First(), 1);

        Assert.Equal(new[] { "Consultar dosis recomendada antes de la venta." }, avisos);
        Assert.Single(carrito.Lineas);
    }

    [Fact]
    public void SinRestriccionesNoHayAvisos()
    {
        var servicio = CrearServicio();
        var carrito = new CarritoVenta();
        var remesaSinRestricciones = RemesaDe(1, 2);

        // La remesa 1 está vendible; el producto 2 no tiene restricciones activas.
        var avisos = servicio.AgregarLinea(carrito, remesaSinRestricciones, 1);

        Assert.Empty(avisos);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public void LaVentaEsDeLosDosRolesPeroUnaSesionSinSesionSeRechaza()
    {
        foreach (var rol in new[] { Rol.Administrador, Rol.Tecnico })
        {
            Assert.NotEmpty(CrearServicio(rol).MedicamentosVendibles());
        }

        var reloj = new RelojFijo(Hoy);
        var sinSesion = new VentaServicio(
            new LotesSimulado(reloj), new RestriccionesSimulado(), new PagosSimulado(), new SinSesionFalsa(), reloj);
        Assert.Throws<AccesoDenegadoException>(() => sinSesion.MedicamentosVendibles());
        Assert.Throws<AccesoDenegadoException>(() => sinSesion.MetodosPagoActivos());
    }

    [Fact]
    public void LaFechaDeLaVentaEsLaDelSistema()
    {
        Assert.Equal(Hoy, DateOnly.FromDateTime(CrearServicio().FechaHoraSistema()));
    }

    [Fact]
    public void ElConstructorRechazaDependenciasNulas()
    {
        var reloj = new RelojFijo(Hoy);
        var lotes = new LotesSimulado(reloj);

        Assert.Throws<ArgumentNullException>(() => new VentaServicio(null!, new RestriccionesSimulado(), new PagosSimulado(), new SesionSimulada(), reloj));
        Assert.Throws<ArgumentNullException>(() => new VentaServicio(lotes, null!, new PagosSimulado(), new SesionSimulada(), reloj));
        Assert.Throws<ArgumentNullException>(() => new VentaServicio(lotes, new RestriccionesSimulado(), null!, new SesionSimulada(), reloj));
        Assert.Throws<ArgumentNullException>(() => new VentaServicio(lotes, new RestriccionesSimulado(), new PagosSimulado(), null!, reloj));
        Assert.Throws<ArgumentNullException>(() => new VentaServicio(lotes, new RestriccionesSimulado(), new PagosSimulado(), new SesionSimulada(), null!));
    }

    /// <summary>Sesión sin usuario: <see cref="ISesion.UsuarioActual"/> lanza, como la sesión real antes de iniciar sesión.</summary>
    private sealed class SinSesionFalsa : ISesion
    {
        public SesionUsuario UsuarioActual => throw new AccesoDenegadoException(MensajesSesion.SinSesion);

        public void ExigirRol(Rol rol) => throw new AccesoDenegadoException(MensajesSesion.SinSesion);
    }
}
