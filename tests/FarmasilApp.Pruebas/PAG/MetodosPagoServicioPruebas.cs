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
}
