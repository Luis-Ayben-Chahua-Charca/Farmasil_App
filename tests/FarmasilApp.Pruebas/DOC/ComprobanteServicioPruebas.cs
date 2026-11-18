using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DOC;
using FarmasilApp.Modelo.Seguridad;
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
    [Trait("Tecnica", "T-001")]
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
    [Trait("Tecnica", "T-001")]
    [InlineData("Nota de crédito")]
    [InlineData("boleta electrónica")]
    public void TipoNoPermitidoLanzaReglaDeNegocioException(string tipo)
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Consultar(tipo, null, null));

        Assert.Equal(
            MensajesComprobantes.TipoInvalido,
            excepcion.Message);

        Assert.Null(repositorio.UltimoFiltro);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-001")]
    public void NumeroMayorAVeinteCaracteresLanzaReglaDeNegocioException()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Consultar(null, "123456789012345678901", null));

        Assert.Equal(
            MensajesComprobantes.NumeroDemasiadoLargo,
            excepcion.Message);

        Assert.Null(repositorio.UltimoFiltro);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-006")]
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
    [Trait("Tecnica", "T-001")]
    public void RepositorioYVentasSonObligatorios()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ComprobanteServicio(null!, new VentasFalsas(), new SesionSimulada()));

        Assert.Throws<ArgumentNullException>(
            () => new ComprobanteServicio(
                new ComprobanteRepositorioFalso(),
                null!,
                new SesionSimulada()));

        Assert.Throws<ArgumentNullException>(
            () => new ComprobanteServicio(new ComprobanteRepositorioFalso(), new VentasFalsas(), null!));
    }

    [Theory]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-001")]
    [InlineData("Todos")]
    [InlineData("todos")]
    [InlineData("12345678901234567890")]
    public void NumeroConservaElTextoInclusoSiEsTodosOAlcanzaElLimite(string numero)
    {
        var repositorio = new ComprobanteRepositorioFalso();
        CrearServicio(repositorio, new VentasFalsas()).Consultar("Todos", numero, null);
        Assert.Equal(numero, repositorio.UltimoFiltro!.NumeroComprobante);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-001")]
    [InlineData("31/02/2026")]
    [InlineData("29/02/2025")]
    [InlineData("fecha")]
    public void FechaInvalidaSeRechazaAntesDeConsultarElRepositorio(string fecha)
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = CrearServicio(repositorio, new VentasFalsas());
        var error = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.ConsultarConFechaTexto("Todos", null, fecha));
        Assert.Equal(MensajesComprobantes.FechaInvalida, error.Message);
        Assert.Equal(0, repositorio.Consultas);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-001")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FechaTextoVaciaNoAplicaFiltro(string? fecha)
    {
        var repositorio = new ComprobanteRepositorioFalso();
        CrearServicio(repositorio, new VentasFalsas()).ConsultarConFechaTexto("Todos", null, fecha);
        Assert.Null(repositorio.UltimoFiltro!.FechaEmision);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-001")]
    [InlineData("17/11/2026")]
    [InlineData(" 17/11/2026 15:42:10 ")]
    public void FechaTextoValidaSeInterpretaPorDiaYMesYSeReduceAlDia(string fecha)
    {
        var repositorio = new ComprobanteRepositorioFalso();
        CrearServicio(repositorio, new VentasFalsas()).ConsultarConFechaTexto("Todos", null, fecha);
        Assert.Equal(new DateTime(2026, 11, 17), repositorio.UltimoFiltro!.FechaEmision);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-012")]
    [InlineData(Rol.Administrador)]
    [InlineData(Rol.Tecnico)]
    public void AmbosRolesConsultanYSeExigeLaSesionAntesDelRepositorio(Rol rol)
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var sesion = new SesionDeConsulta(rol, repositorio);
        var servicio = new ComprobanteServicio(repositorio, new VentasFalsas(), sesion);
        servicio.ConsultarConFechaTexto("Todos", null, null);
        Assert.Equal(rol, sesion.RolExigido);
        Assert.Equal(0, sesion.ConsultasAlExigirRol);
        Assert.Equal(1, repositorio.Consultas);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-012")]
    [InlineData(false)]
    [InlineData(true)]
    public void UnaSesionQueDeniegaElAccesoImpideConsultarEnAmbasEntradas(bool fechaTexto)
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var sesion = new SesionDeConsulta(Rol.Administrador, repositorio) { Denegar = true };
        var servicio = new ComprobanteServicio(repositorio, new VentasFalsas(), sesion);
        Assert.Throws<AccesoDenegadoException>(() =>
        {
            if (fechaTexto) servicio.ConsultarConFechaTexto("Todos", null, null);
            else servicio.Consultar("Todos", null, null);
        });
        Assert.Equal(0, repositorio.Consultas);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-012")]
    public void RolFueraDeLaMatrizSeRechazaAntesDelRepositorio()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = new ComprobanteServicio(repositorio, new VentasFalsas(),
            new SesionDeConsulta((Rol)99, repositorio));
        Assert.Throws<AccesoDenegadoException>(() => servicio.Consultar("Todos", null, null));
        Assert.Equal(0, repositorio.Consultas);
    }

    private sealed class SesionDeConsulta(Rol rol, ComprobanteRepositorioFalso repositorio) : ISesion
    {
        public SesionUsuario UsuarioActual { get; } = new() { IdUsuario = 1, Rol = rol };
        public Rol? RolExigido { get; private set; }
        public int ConsultasAlExigirRol { get; private set; }
        public bool Denegar { get; init; }

        public void ExigirRol(Rol exigido)
        {
            RolExigido = exigido;
            ConsultasAlExigirRol = repositorio.Consultas;
            if (Denegar || exigido != rol)
                throw new AccesoDenegadoException(MensajesComprobantes.AccesoDenegado);
        }
    }

    private static ComprobanteServicio CrearServicio(
        ComprobanteRepositorioFalso repositorio,
        VentasFalsas ventas)
    {
        return new ComprobanteServicio(repositorio, ventas, new SesionSimulada());
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
