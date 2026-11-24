using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Pruebas.PAG;

public class MetodosPagoServicioPruebas
{
    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarNormalizaNombreYEstadoAntesDeLlamarAlRepositorio()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        Servicio(repositorio).Consultar("  Tarjeta  ", "  Inactivo  ");

        Assert.Equal(("Tarjeta", "Inactivo"), repositorio.UltimaConsulta);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarSinCriteriosDevuelveMetodosActivosEInactivos()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));
        repositorio.Agregar(Metodo(2, "Tarjeta", "Inactivo"));

        var resultados = Servicio(repositorio).Consultar(null, null);

        Assert.Equal(new[] { "Activo", "Inactivo" }, resultados.Select(metodo => metodo.Estado));
        Assert.Equal((null, null), repositorio.UltimaConsulta);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    [InlineData("Tarjeta", null, "Tarjeta", null)]
    [InlineData("arj", null, "arj", null)]
    [InlineData(null, "Activo", null, "Activo")]
    [InlineData(null, "Inactivo", null, "Inactivo")]
    [InlineData("Tar", "Inactivo", "Tar", "Inactivo")]
    public void ConsultarPasaLosCriteriosOpcionalesAlRepositorio(
        string? nombreMetodo,
        string? estado,
        string? nombreEsperado,
        string? estadoEsperado)
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        Servicio(repositorio).Consultar(nombreMetodo, estado);

        Assert.Equal((nombreEsperado, estadoEsperado), repositorio.UltimaConsulta);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarConEstadoNoPermitidoLanzaReglaDeNegocioException()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Consultar(null, "Pendiente"));

        Assert.Equal(MensajesMetodosPago.EstadoInvalido, excepcion.Message);
        Assert.Null(repositorio.UltimaConsulta);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-012")]
    [InlineData(Rol.Administrador)]
    [InlineData(Rol.Tecnico)]
    public void ConsultarPermiteAdministradorYTecnico(Rol rol)
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        Servicio(repositorio, rol).Consultar(null, null);

        Assert.Equal((null, null), repositorio.UltimaConsulta);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-002")]
    public void CrearRecortaElNombreYRegistraElEstado()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        var id = Servicio(repositorio).Crear("  Tarjeta  ", "Inactivo");

        Assert.Equal(1, id);
        Assert.Equal(("Tarjeta", "Inactivo"), repositorio.UltimoCrear);
        Assert.Equal(("Tarjeta", (int?)null), repositorio.UltimaExistenciaNombre);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaUnNombreVacioAntesDeConsultarDuplicados()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("  ", "Activo"));

        Assert.Equal(MensajesMetodosPago.NombreVacio, excepcion.Message);
        Assert.Null(repositorio.UltimaExistenciaNombre);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaUnNombreQueSuperaCincuentaCaracteres()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear(new string('x', 51), "Activo"));

        Assert.Equal(MensajesMetodosPago.NombreDemasiadoLargo, excepcion.Message);
        Assert.Null(repositorio.UltimaExistenciaNombre);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaUnNombreDuplicadoSinGuardar()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear(" efectivo ", "Activo"));

        Assert.Equal(MensajesMetodosPago.NombreRepetido, excepcion.Message);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaUnEstadoNoPermitido()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("Tarjeta", "Pendiente"));

        Assert.Equal(MensajesMetodosPago.EstadoInvalido, excepcion.Message);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-006")]
    public void MetodosActivosDevuelveSoloActivosOrdenadosPorId()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(Metodo(3, "Tarjeta", "Activo"));
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));
        repositorio.Agregar(Metodo(2, "Yape", "Inactivo"));

        var metodos = Servicio(repositorio).MetodosActivos();

        Assert.Equal(new[] { 1, 3 }, metodos.Select(metodo => metodo.IdMetodoPago));
        Assert.Equal(new[] { "Efectivo", "Tarjeta" }, metodos.Select(metodo => metodo.Nombre));
        Assert.Equal((null, "Activo"), repositorio.UltimaConsulta);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-006")]
    public void CrearMetodoActivoLoOfreceEnVentasYCrearInactivoNo()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        var servicio = Servicio(repositorio);
        servicio.Crear("Pago activo", "Activo");
        servicio.Crear("Pago inactivo", "Inactivo");

        var metodos = servicio.MetodosActivos();

        Assert.Equal("Pago activo", Assert.Single(metodos).Nombre);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-012")]
    public void CrearRechazaAlTecnicoAntesDeConsultarElRepositorio()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        Assert.Throws<AccesoDenegadoException>(
            () => Servicio(repositorio, Rol.Tecnico).Crear("Tarjeta", "Activo"));

        Assert.Null(repositorio.UltimaExistenciaNombre);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0019")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarRecortaElNombreYActualizaNombreYEstado()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(Metodo(5, "Efectivo", "Activo"));

        Servicio(repositorio).Actualizar(5, "  Caja  ", "Inactivo");

        Assert.Equal((5, "Caja", "Inactivo"), repositorio.UltimoActualizar);
        Assert.Equal(("Caja", (int?)5), repositorio.UltimaExistenciaNombre);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0019")]
    [Trait("Tecnica", "T-001")]
    public void ActualizarExcluyeElMetodoActualDeLaComprobacionDeUnicidad()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(Metodo(5, "Efectivo", "Activo"));

        Servicio(repositorio).Actualizar(5, "Efectivo", "Activo");

        Assert.Equal(("Efectivo", (int?)5), repositorio.UltimaExistenciaNombre);
        Assert.Equal("Efectivo", Assert.Single(repositorio.Resultados).NombreMetodo);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0019")]
    [Trait("Tecnica", "T-001")]
    public void ActualizarRechazaElNombreDeOtroMetodo()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(Metodo(5, "Efectivo", "Activo"));
        repositorio.Agregar(Metodo(8, "Tarjeta", "Activo"));

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Actualizar(5, "Tarjeta", "Activo"));

        Assert.Equal(MensajesMetodosPago.NombreRepetido, excepcion.Message);
        Assert.Null(repositorio.UltimoActualizar);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0019")]
    [Trait("Tecnica", "T-007")]
    public void DesactivarUnMetodoLoRetiraDeVentasNuevasYSinAlterarElHistorico()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));
        repositorio.MarcarConVentas(1);
        var historicoAntes = repositorio.VentasHistoricas.ToArray();
        var servicio = Servicio(repositorio);

        servicio.Actualizar(1, "Efectivo", "Inactivo");

        Assert.Empty(servicio.MetodosActivos());
        Assert.Equal(historicoAntes, repositorio.VentasHistoricas);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0019")]
    [Trait("Tecnica", "T-012")]
    public void ActualizarRechazaAlTecnicoAntesDeConsultarElRepositorio()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        Assert.Throws<AccesoDenegadoException>(
            () => Servicio(repositorio, Rol.Tecnico).Actualizar(1, "Tarjeta", "Activo"));

        Assert.Null(repositorio.UltimaExistenciaNombre);
        Assert.Null(repositorio.UltimoActualizar);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0020")]
    [Trait("Tecnica", "T-002")]
    public void DarDeBajaSinReferenciasEliminaElMetodo()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));

        var resultado = Servicio(repositorio).DarDeBaja(1);

        Assert.Equal(ResultadoBajaMetodoPago.Eliminado, resultado);
        Assert.Empty(repositorio.Resultados);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0020")]
    [Trait("Tecnica", "T-003")]
    public void DarDeBajaConReferenciasDesactivaElMetodo()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));
        repositorio.MarcarConVentas(1);

        var resultado = Servicio(repositorio).DarDeBaja(1);

        Assert.Equal(ResultadoBajaMetodoPago.Desactivado, resultado);
        Assert.Equal("Inactivo", Assert.Single(repositorio.Resultados).Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0020")]
    [Trait("Tecnica", "T-006")]
    public void DarDeBajaConReferenciasRetiraElMetodoDeVentasNuevas()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));
        repositorio.MarcarConVentas(1);
        var servicio = Servicio(repositorio);

        servicio.DarDeBaja(1);

        Assert.Empty(servicio.MetodosActivos());
    }

    [Fact]
    [Trait("Especificacion", "ESP-0020")]
    [Trait("Tecnica", "T-007")]
    public void DarDeBajaConReferenciasConservaLasVentasHistoricas()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));
        repositorio.MarcarConVentas(1);
        var historicoAntes = repositorio.VentasHistoricas.ToArray();

        Servicio(repositorio).DarDeBaja(1);

        Assert.Equal(historicoAntes, repositorio.VentasHistoricas);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0020")]
    [Trait("Tecnica", "T-012")]
    public void DarDeBajaRechazaAlTecnicoAntesDeConsultarElRepositorio()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        Assert.Throws<AccesoDenegadoException>(
            () => Servicio(repositorio, Rol.Tecnico).DarDeBaja(1));

        Assert.Null(repositorio.UltimaBaja);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-002")]
    public void ElRepositorioEsObligatorio()
    {
        Assert.Throws<ArgumentNullException>(
            () => new MetodosPagoServicio(new RepositorioDeMetodosPagoFalso(), null!));
    }

    private static MetodosPagoServicio Servicio(
        RepositorioDeMetodosPagoFalso repositorio,
        Rol rol = Rol.Administrador) =>
        new(repositorio, new SesionSimulada(rol));

    private static MetodoPagoConsulta Metodo(int id, string nombre, string estado) =>
        new()
        {
            IdMetodoPago = id,
            NombreMetodo = nombre,
            Estado = estado
        };

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-002")]
    public void CrearCaminoBasicoValidaUnicidadYRegistraMetodoPago()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        var servicio = new MetodosPagoServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var idCreado = servicio.Crear("  Yape  ", "Activo");

        Assert.Equal(1, idCreado);
        Assert.Equal(("Yape", null), repositorio.UltimaExistenciaNombre);
        Assert.Equal(("Yape", "Activo"), repositorio.UltimoCrear);
        Assert.Single(repositorio.Resultados);
        Assert.Equal("Yape", repositorio.Resultados[0].NombreMetodo);
        Assert.Equal("Activo", repositorio.Resultados[0].Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-001")]
    public void CrearParticionSobreNombreValidoDuplicadoVacioYExcesoLongitud()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 1,
            NombreMetodo = "Efectivo",
            Estado = "Activo",
        });
        var servicio = new MetodosPagoServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var idValido = servicio.Crear("Transferencia", "Activo");
        Assert.Equal(2, idValido);
        Assert.Equal(("Transferencia", null), repositorio.UltimaExistenciaNombre);
        Assert.Equal(("Transferencia", "Activo"), repositorio.UltimoCrear);
        Assert.Equal(2, repositorio.Resultados.Count);

        var exDuplicado = Assert.Throws<ReglaDeNegocioException>(() =>
            servicio.Crear("efectivo", "Activo"));
        Assert.Equal(MensajesMetodosPago.NombreRepetido, exDuplicado.Message);
        Assert.Equal(("Transferencia", "Activo"), repositorio.UltimoCrear);
        Assert.Equal(2, repositorio.Resultados.Count);

        var exVacio = Assert.Throws<ReglaDeNegocioException>(() =>
            servicio.Crear("   ", "Activo"));
        Assert.Equal(MensajesMetodosPago.NombreVacio, exVacio.Message);
        Assert.Equal(("Transferencia", "Activo"), repositorio.UltimoCrear);
        Assert.Equal(2, repositorio.Resultados.Count);

        var nombreLargo = new string('A', MetodosPagoServicio.LongitudMaximaNombre + 1);
        var exLongitud = Assert.Throws<ReglaDeNegocioException>(() =>
            servicio.Crear(nombreLargo, "Activo"));
        Assert.Equal(MensajesMetodosPago.NombreDemasiadoLargo, exLongitud.Message);
        Assert.Equal(("Transferencia", "Activo"), repositorio.UltimoCrear);
        Assert.Equal(2, repositorio.Resultados.Count);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarCaminoBasicoFiltraPorNombreYEstado()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 1,
            NombreMetodo = "Yape",
            Estado = "Activo",
        });
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 2,
            NombreMetodo = "Yape QR",
            Estado = "Inactivo",
        });
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 3,
            NombreMetodo = "Plin",
            Estado = "Activo",
        });

        var servicio = new MetodosPagoServicio(
            repositorio,
            new SesionSimulada(Rol.Tecnico));

        var resultados = servicio.Consultar("  Yape  ", "  Activo  ");

        Assert.Equal(("Yape", "Activo"), repositorio.UltimaConsulta);
        var item = Assert.Single(resultados);
        Assert.Equal(1, item.IdMetodoPago);
        Assert.Equal("Yape", item.NombreMetodo);
        Assert.Equal("Activo", item.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarParticionDeCriteriosNombreParcialEstadoCombinacionYSinFiltros()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 1,
            NombreMetodo = "Yape",
            Estado = "Activo",
        });
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 2,
            NombreMetodo = "Yape QR",
            Estado = "Inactivo",
        });
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 3,
            NombreMetodo = "Efectivo",
            Estado = "Activo",
        });

        var servicio = new MetodosPagoServicio(
            repositorio,
            new SesionSimulada(Rol.Tecnico));

        // Nombre exacto
        var porNombreExacto = servicio.Consultar("Efectivo", null);
        Assert.Equal(("Efectivo", null), repositorio.UltimaConsulta);
        Assert.Equal(new[] { 3 }, porNombreExacto.Select(x => x.IdMetodoPago));

        // Nombre parcial
        var porNombreParcial = servicio.Consultar("Yape", null);
        Assert.Equal(("Yape", null), repositorio.UltimaConsulta);
        Assert.Equal(new[] { 1, 2 }, porNombreParcial.Select(x => x.IdMetodoPago));

        // Estado Activo / Inactivo
        var activos = servicio.Consultar(null, "Activo");
        Assert.Equal((null, "Activo"), repositorio.UltimaConsulta);
        Assert.Equal(new[] { 1, 3 }, activos.Select(x => x.IdMetodoPago));

        var inactivos = servicio.Consultar(null, "Inactivo");
        Assert.Equal((null, "Inactivo"), repositorio.UltimaConsulta);
        Assert.Equal(new[] { 2 }, inactivos.Select(x => x.IdMetodoPago));

        // Combinación de nombre y estado
        var combinados = servicio.Consultar("Yape", "Inactivo");
        Assert.Equal(("Yape", "Inactivo"), repositorio.UltimaConsulta);
        Assert.Equal(new[] { 2 }, combinados.Select(x => x.IdMetodoPago));

        // Sin criterios
        var todos = servicio.Consultar(null, null);
        Assert.Equal((null, null), repositorio.UltimaConsulta);
        Assert.Equal(new[] { 1, 2, 3 }, todos.Select(x => x.IdMetodoPago));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0019")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarCaminoBasicoModificaNombreYEstadoConfirmandoCambios()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 4,
            NombreMetodo = "Tarjeta",
            Estado = "Activo",
        });
        var servicio = new MetodosPagoServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        servicio.Actualizar(4, "  Tarjeta Debito  ", "Inactivo");

        Assert.Equal(("Tarjeta Debito", 4), repositorio.UltimaExistenciaNombre);
        Assert.Equal((4, "Tarjeta Debito", "Inactivo"), repositorio.UltimoActualizar);
        Assert.Equal("Tarjeta Debito", repositorio.Resultados[0].NombreMetodo);
        Assert.Equal("Inactivo", repositorio.Resultados[0].Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0019")]
    [Trait("Tecnica", "T-001")]
    public void ActualizarConNombreDeOtroMetodoLanzaExcepcionYNoActualiza()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 1,
            NombreMetodo = "Yape",
            Estado = "Activo",
        });
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 2,
            NombreMetodo = "Plin",
            Estado = "Activo",
        });
        var servicio = new MetodosPagoServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            servicio.Actualizar(2, "  yape  ", "Activo"));

        Assert.Equal(MensajesMetodosPago.NombreRepetido, ex.Message);
        Assert.Equal(("yape", 2), repositorio.UltimaExistenciaNombre);
        Assert.Null(repositorio.UltimoActualizar);
        Assert.Equal("Plin", repositorio.Resultados[1].NombreMetodo);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0020")]
    [Trait("Tecnica", "T-002")]
    public void DarDeBajaCaminoBasicoAplicaBajaYRetornaResultado()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 5,
            NombreMetodo = "Billetera",
            Estado = "Activo",
        });
        var servicio = new MetodosPagoServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var resultado = servicio.DarDeBaja(5);

        Assert.Equal(5, repositorio.UltimaBaja);
        Assert.Equal(ResultadoBajaMetodoPago.Eliminado, resultado);
        Assert.Equal(ResultadoBajaMetodoPago.Eliminado, repositorio.UltimoResultadoBaja);
        Assert.Empty(repositorio.Resultados);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0020")]
    [Trait("Tecnica", "T-003")]
    public void DarDeBajaRamasSinVentasYConVentas()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 1,
            NombreMetodo = "Efectivo",
            Estado = "Activo",
        });
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 2,
            NombreMetodo = "Yape",
            Estado = "Activo",
        });
        repositorio.MarcarConVentas(2);

        var servicio = new MetodosPagoServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        // Rama sin ventas -> eliminado
        var resultadoSinVentas = servicio.DarDeBaja(1);
        Assert.Equal(ResultadoBajaMetodoPago.Eliminado, resultadoSinVentas);
        Assert.Equal(1, repositorio.UltimaBaja);
        Assert.DoesNotContain(repositorio.Resultados, m => m.IdMetodoPago == 1);

        // Rama con ventas -> desactivado (estado "Inactivo")
        var resultadoConVentas = servicio.DarDeBaja(2);
        Assert.Equal(ResultadoBajaMetodoPago.Desactivado, resultadoConVentas);
        Assert.Equal(2, repositorio.UltimaBaja);
        var metodoConVentas = Assert.Single(repositorio.Resultados, m => m.IdMetodoPago == 2);
        Assert.Equal("Inactivo", metodoConVentas.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0020")]
    [Trait("Tecnica", "T-006")]
    public void DarDeBajaMetodoConVentasAplicaBajaLogicaConEstadoInactivo()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 7,
            NombreMetodo = "POS Visa",
            Estado = "Activo",
        });
        repositorio.MarcarConVentas(7);

        var servicio = new MetodosPagoServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var resultado = servicio.DarDeBaja(7);

        Assert.Equal(7, repositorio.UltimaBaja);
        Assert.Equal(ResultadoBajaMetodoPago.Desactivado, resultado);
        Assert.Equal(ResultadoBajaMetodoPago.Desactivado, repositorio.UltimoResultadoBaja);
        var metodo = Assert.Single(repositorio.Resultados, m => m.IdMetodoPago == 7);
        Assert.Equal("Inactivo", metodo.Estado);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public void SesionTecnicoPermiteConsultarYRechazaCrearActualizarYDarDeBaja()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Agregar(new MetodoPagoConsulta
        {
            IdMetodoPago = 1,
            NombreMetodo = "Efectivo",
            Estado = "Activo",
        });

        var servicio = new MetodosPagoServicio(
            repositorio,
            new SesionSimulada(Rol.Tecnico));

        // ESP-0018 (Consultar): permitido para Técnico
        var consulta = servicio.Consultar(null, "Activo");
        Assert.Single(consulta);
        Assert.Equal((null, "Activo"), repositorio.UltimaConsulta);

        // ESP-0017 (Crear): rechazado
        Assert.Throws<AccesoDenegadoException>(() =>
            servicio.Crear("Yape", "Activo"));
        Assert.Null(repositorio.UltimoCrear);

        // ESP-0019 (Actualizar): rechazado
        Assert.Throws<AccesoDenegadoException>(() =>
            servicio.Actualizar(1, "Efectivo Caja", "Activo"));
        Assert.Null(repositorio.UltimoActualizar);

        // ESP-0020 (DarDeBaja): rechazado
        Assert.Throws<AccesoDenegadoException>(() =>
            servicio.DarDeBaja(1));
        Assert.Null(repositorio.UltimaBaja);
    }
}
