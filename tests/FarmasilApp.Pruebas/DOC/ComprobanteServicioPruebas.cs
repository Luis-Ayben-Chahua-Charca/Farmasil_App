
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DOC;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.VEN;

namespace FarmasilApp.Pruebas.DOC;

public class ComprobanteServicioPruebas
{
    // =========================================================
    // PRUEBAS DE CONSULTA - ESP-0010
    // =========================================================

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

        Assert.Equal(MensajesComprobantes.TipoInvalido, excepcion.Message);
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
            () => new ComprobanteServicio(
                null!,
                new VentasFalsas(),
                new SesionSimulada()));

        Assert.Throws<ArgumentNullException>(
            () => new ComprobanteServicio(
                new ComprobanteRepositorioFalso(),
                null!,
                new SesionSimulada()));

        Assert.Throws<ArgumentNullException>(
            () => new ComprobanteServicio(
                new ComprobanteRepositorioFalso(),
                new VentasFalsas(),
                null!));
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

        CrearServicio(repositorio, new VentasFalsas())
            .Consultar("Todos", numero, null);

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

        CrearServicio(repositorio, new VentasFalsas())
            .ConsultarConFechaTexto("Todos", null, fecha);

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

        CrearServicio(repositorio, new VentasFalsas())
            .ConsultarConFechaTexto("Todos", null, fecha);

        Assert.Equal(
            new DateTime(2026, 11, 17),
            repositorio.UltimoFiltro!.FechaEmision);
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
        var servicio = new ComprobanteServicio(
            repositorio,
            new VentasFalsas(),
            sesion);

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
        var sesion = new SesionDeConsulta(Rol.Administrador, repositorio)
        {
            Denegar = true
        };

        var servicio = new ComprobanteServicio(
            repositorio,
            new VentasFalsas(),
            sesion);

        Assert.Throws<AccesoDenegadoException>(() =>
        {
            if (fechaTexto)
                servicio.ConsultarConFechaTexto("Todos", null, null);
            else
                servicio.Consultar("Todos", null, null);
        });

        Assert.Equal(0, repositorio.Consultas);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0010")]
    [Trait("Tecnica", "T-012")]
    public void RolFueraDeLaMatrizSeRechazaAntesDelRepositorio()
    {
        var repositorio = new ComprobanteRepositorioFalso();

        var servicio = new ComprobanteServicio(
            repositorio,
            new VentasFalsas(),
            new SesionDeConsulta((Rol)99, repositorio));

        Assert.Throws<AccesoDenegadoException>(
            () => servicio.Consultar("Todos", null, null));

        Assert.Equal(0, repositorio.Consultas);
    }

    // =========================================================
    // PRUEBAS DE REGISTRO - ESP-0009
    // =========================================================

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-002")]
    public void RegistrarConDatosValidosGuardaElComprobante()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var ventas = CrearVentasConUnaVenta(2, 4.00m);

        var servicio = CrearServicio(repositorio, ventas);

        servicio.Registrar(
            " Boleta ",
            " B001-000002 ",
            "24/11/2026",
            2);

        Assert.Equal(
            "B001-000002",
            repositorio.UltimoNumeroComprobanteRegistrado);

        Assert.Equal(
            "Boleta",
            repositorio.UltimoTipoComprobanteRegistrado);

        Assert.Equal(
            new DateTime(2026, 11, 24),
            repositorio.UltimaFechaEmisionRegistrada);

        Assert.Equal(2, repositorio.UltimaVentaRegistrada);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-002")]
    public void RegistrarConNumeroDuplicadoLanzaReglaDeNegocioException()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            Resultados = new[]
            {
                Comprobante(10, 1, "B001-000002", "Emitido")
            }
        };

        var ventas = CrearVentasConUnaVenta(2, 4.00m);
        var servicio = CrearServicio(repositorio, ventas);

        var error = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Registrar(
                "Boleta",
                "B001-000002",
                "24/11/2026",
                2));

        Assert.Equal(MensajesComprobantes.NumeroDuplicado, error.Message);
        Assert.Null(repositorio.UltimoNumeroComprobanteRegistrado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-002")]
    public void RegistrarConVentaYaEmitidaLanzaReglaDeNegocioException()
    {
        var repositorio = new ComprobanteRepositorioFalso
        {
            IdsVentasConComprobanteEmitido = new[] { 2 }
        };

        var ventas = CrearVentasConUnaVenta(2, 4.00m);
        var servicio = CrearServicio(repositorio, ventas);

        var error = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Registrar(
                "Boleta",
                "B001-000003",
                "24/11/2026",
                2));

        Assert.Equal(MensajesComprobantes.VentaYaEmitida, error.Message);
        Assert.Null(repositorio.UltimoNumeroComprobanteRegistrado);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-001")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RegistrarSinNumeroLanzaReglaDeNegocioException(string? numero)
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        var error = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Registrar(
                "Boleta",
                numero,
                "24/11/2026",
                2));

        Assert.Equal(MensajesComprobantes.NumeroObligatorio, error.Message);
        Assert.Null(repositorio.UltimoNumeroComprobanteRegistrado);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-001")]
    [InlineData("31/02/2026")]
    [InlineData("29/02/2025")]
    [InlineData("fecha")]
    public void RegistrarConFechaInvalidaLanzaReglaDeNegocioException(string fecha)
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        var error = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Registrar(
                "Boleta",
                "B001-000004",
                fecha,
                2));

        Assert.Equal(MensajesComprobantes.FechaInvalida, error.Message);
        Assert.Null(repositorio.UltimoNumeroComprobanteRegistrado);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-001")]
    [InlineData("Nota de crédito")]
    [InlineData("Boleta electrónica")]
    public void RegistrarConTipoInvalidoLanzaReglaDeNegocioException(string tipo)
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        var error = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Registrar(
                tipo,
                "B001-000005",
                "24/11/2026",
                2));

        Assert.Equal(MensajesComprobantes.TipoInvalido, error.Message);
        Assert.Null(repositorio.UltimoNumeroComprobanteRegistrado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-001")]
    public void RegistrarConNumeroMayorAVeinteCaracteresLanzaReglaDeNegocioException()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        var error = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Registrar(
                "Boleta",
                "123456789012345678901",
                "24/11/2026",
                2));

        Assert.Equal(MensajesComprobantes.NumeroDemasiadoLargo, error.Message);
        Assert.Null(repositorio.UltimoNumeroComprobanteRegistrado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-002")]
    public void RegistrarConVentaInexistenteLanzaReglaDeNegocioException()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var servicio = CrearServicio(repositorio, new VentasFalsas());

        var error = Assert.Throws<ReglaDeNegocioException>(
            () => servicio.Registrar(
                "Boleta",
                "B001-000006",
                "24/11/2026",
                999));

        Assert.Equal(MensajesComprobantes.VentaNoEncontrada, error.Message);
        Assert.Null(repositorio.UltimoNumeroComprobanteRegistrado);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-012")]
    [InlineData(Rol.Administrador)]
    [InlineData(Rol.Tecnico)]
    public void AdministradorYTecnicoPuedenRegistrar(Rol rol)
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var ventas = CrearVentasConUnaVenta(2, 4.00m);
        var sesion = new SesionDeRegistro(rol);

        var servicio = new ComprobanteServicio(repositorio, ventas, sesion);

        servicio.Registrar(
            "Boleta",
            "B001-000007",
            "24/11/2026",
            2);

        Assert.Equal(rol, sesion.RolExigido);
        Assert.Equal(2, repositorio.UltimaVentaRegistrada);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0009")]
    [Trait("Tecnica", "T-012")]
    public void RolNoAutorizadoNoPuedeRegistrar()
    {
        var repositorio = new ComprobanteRepositorioFalso();
        var ventas = CrearVentasConUnaVenta(2, 4.00m);
        var sesion = new SesionDeRegistro((Rol)99);

        var servicio = new ComprobanteServicio(repositorio, ventas, sesion);

        Assert.Throws<AccesoDenegadoException>(
            () => servicio.Registrar(
                "Boleta",
                "B001-000008",
                "24/11/2026",
                2));

        Assert.Null(repositorio.UltimoNumeroComprobanteRegistrado);
    }

    // =========================================================
    // MÉTODOS AUXILIARES
    // =========================================================

    private sealed class SesionDeConsulta(
        Rol rol,
        ComprobanteRepositorioFalso repositorio) : ISesion
    {
        public SesionUsuario UsuarioActual { get; } =
            new() { IdUsuario = 1, Rol = rol };

        public Rol? RolExigido { get; private set; }

        public int ConsultasAlExigirRol { get; private set; }

        public bool Denegar { get; init; }

        public void ExigirRol(Rol exigido)
        {
            RolExigido = exigido;
            ConsultasAlExigirRol = repositorio.Consultas;

            if (Denegar || exigido != rol)
            {
                throw new AccesoDenegadoException(
                    MensajesComprobantes.AccesoDenegado);
            }
        }
    }

    private sealed class SesionDeRegistro(Rol rol) : ISesion
    {
        public SesionUsuario UsuarioActual { get; } =
            new() { IdUsuario = 1, Rol = rol };

        public Rol? RolExigido { get; private set; }

        public void ExigirRol(Rol exigido)
        {
            RolExigido = exigido;

            if (exigido != rol)
            {
                throw new AccesoDenegadoException(
                    MensajesComprobantes.AccesoDenegado);
            }
        }
    }

    private static ComprobanteServicio CrearServicio(
        ComprobanteRepositorioFalso repositorio,
        VentasFalsas ventas)
    {
        return new ComprobanteServicio(
            repositorio,
            ventas,
            new SesionSimulada());
    }

    private static VentasFalsas CrearVentasConUnaVenta(
        int idVenta,
        decimal monto)
    {
        var ventas = new VentasFalsas();

        ventas.Agregar(new VentaDto
        {
            IdVenta = idVenta,
            MontoTotal = monto
        });

        return ventas;
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
