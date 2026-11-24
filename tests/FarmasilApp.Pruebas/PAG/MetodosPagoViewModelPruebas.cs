using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Pruebas.VistaModelo;
using FarmasilApp.VistaModelo.PAG;

namespace FarmasilApp.Pruebas.PAG;

public class MetodosPagoViewModelPruebas
{
    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    public async Task InicializarConsultaTodosLosMetodosYPreparaElFiltroDeEstado()
    {
        var (vistaModelo, repositorio, _) = Crear();
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));
        repositorio.Agregar(Metodo(2, "Yape", "Activo"));
        repositorio.Agregar(Metodo(3, "Tarjeta", "Inactivo"));

        await vistaModelo.InicializarCommand.ExecuteAsync();

        Assert.Equal(
            new[] { MetodosPagoViewModel.TodosLosEstados, "Activo", "Inactivo" },
            vistaModelo.FiltrosEstado);
        Assert.Equal(new[] { "Activo", "Inactivo" }, vistaModelo.EstadosFicha);
        Assert.Equal(new[] { "Efectivo", "Yape", "Tarjeta" }, vistaModelo.Metodos.Select(metodo => metodo.NombreMetodo));
        Assert.Equal((null, null), repositorio.UltimaConsulta);
        Assert.Equal("Métodos de pago encontrados: 3.", vistaModelo.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    public async Task VolverATodosQuitaElFiltroDeEstado()
    {
        var (vistaModelo, repositorio, _) = Crear();
        vistaModelo.EstadoFiltro = "Activo";
        await vistaModelo.BuscarCommand.ExecuteAsync();

        vistaModelo.EstadoFiltro = MetodosPagoViewModel.TodosLosEstados;
        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.Equal((null, null), repositorio.UltimaConsulta);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    public async Task SinResultadosMuestraQueNoSeEncontraronMetodos()
    {
        var (vistaModelo, _, _) = Crear();
        vistaModelo.Busqueda = "Cheque";

        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.Empty(vistaModelo.Metodos);
        Assert.Equal("No se encontraron métodos de pago.", vistaModelo.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-002")]
    public async Task BuscarPasaElNombreParcialYElEstadoSeleccionados()
    {
        var (vistaModelo, repositorio, _) = Crear();
        repositorio.Agregar(Metodo(3, "Tarjeta", "Inactivo"));
        vistaModelo.Busqueda = "arj";
        vistaModelo.EstadoFiltro = "Inactivo";

        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.Equal(("arj", "Inactivo"), repositorio.UltimaConsulta);
        Assert.Equal("Tarjeta", Assert.Single(vistaModelo.Metodos).NombreMetodo);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    public async Task UnErrorDeConsultaSeMuestraSinDejarResultadosAntiguos()
    {
        var (vistaModelo, repositorio, _) = Crear();
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));
        await vistaModelo.BuscarCommand.ExecuteAsync();
        repositorio.FallaAlConsultar = true;

        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.Empty(vistaModelo.Metodos);
        Assert.Null(vistaModelo.MetodoSeleccionado);
        Assert.Equal(MensajesMetodosPago.ErrorConsulta, vistaModelo.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-002")]
    public async Task CrearGuardaElMetodoYRefrescaLaColeccionCompartidaPorTarjetasYTabla()
    {
        var (vistaModelo, repositorio, mensajes) = Crear();
        vistaModelo.NombreMetodo = "  Tarjeta  ";
        vistaModelo.EstadoFicha = "Inactivo";

        await vistaModelo.CrearCommand.ExecuteAsync();

        Assert.Equal(("Tarjeta", "Inactivo"), repositorio.UltimoCrear);
        Assert.Equal(MensajesMetodosPago.MetodoRegistrado, Assert.Single(mensajes.Exitos).Mensaje);
        Assert.Empty(mensajes.Errores);
        Assert.Equal("Tarjeta", Assert.Single(vistaModelo.Metodos).NombreMetodo);
        Assert.Equal("Inactivo", vistaModelo.Metodos[0].Estado);
        Assert.Null(vistaModelo.MetodoSeleccionado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-001")]
    public async Task CrearMuestraLaValidacionDelServicioSinGuardar()
    {
        var (vistaModelo, repositorio, mensajes) = Crear();
        vistaModelo.NombreMetodo = " ";

        await vistaModelo.CrearCommand.ExecuteAsync();

        Assert.Null(repositorio.UltimoCrear);
        Assert.Equal(MensajesMetodosPago.NombreVacio, Assert.Single(mensajes.Errores).Mensaje);
        Assert.Empty(mensajes.Exitos);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0019")]
    [Trait("Tecnica", "T-002")]
    public async Task ActualizarExigeIniciarEdicionYRefrescaTrasGuardarCambios()
    {
        var (vistaModelo, repositorio, mensajes) = Crear();
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));
        await vistaModelo.InicializarCommand.ExecuteAsync();
        vistaModelo.MetodoSeleccionado = vistaModelo.Metodos[0];
        vistaModelo.ActualizarCommand.Execute(null);

        Assert.True(vistaModelo.EnEdicion);
        Assert.Null(repositorio.UltimoActualizar);
        vistaModelo.NombreMetodo = "Caja";
        vistaModelo.EstadoFicha = "Inactivo";

        await vistaModelo.ConfirmarActualizacionCommand.ExecuteAsync();

        Assert.Equal((1, "Caja", "Inactivo"), repositorio.UltimoActualizar);
        Assert.Equal(MensajesMetodosPago.MetodoActualizado, Assert.Single(mensajes.Exitos).Mensaje);
        var actualizado = Assert.Single(vistaModelo.Metodos);
        Assert.Equal("Caja", actualizado.NombreMetodo);
        Assert.Equal("Inactivo", actualizado.Estado);
        Assert.False(vistaModelo.EnEdicion);
        Assert.Null(vistaModelo.MetodoSeleccionado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0020")]
    [Trait("Tecnica", "T-003")]
    public async Task CancelarLaConfirmacionDeBajaConservaElMetodo()
    {
        var (vistaModelo, repositorio, mensajes) = Crear(respuestaConfirmacion: false);
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));
        await vistaModelo.InicializarCommand.ExecuteAsync();
        var seleccionado = vistaModelo.Metodos[0];
        vistaModelo.MetodoSeleccionado = seleccionado;

        await vistaModelo.EliminarCommand.ExecuteAsync();

        Assert.Equal(
            ("PAG", (string?)null, MensajesMetodosPago.ConfirmarBaja),
            Assert.Single(mensajes.Confirmaciones));
        Assert.Null(repositorio.UltimaBaja);
        Assert.Same(seleccionado, vistaModelo.MetodoSeleccionado);
        Assert.Single(vistaModelo.Metodos);
        Assert.Empty(mensajes.Exitos);
        Assert.Empty(mensajes.Errores);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0020")]
    [Trait("Tecnica", "T-003")]
    [InlineData(false, ResultadoBajaMetodoPago.Eliminado, 0)]
    [InlineData(true, ResultadoBajaMetodoPago.Desactivado, 1)]
    public async Task ConfirmarLaBajaRefrescaLaColeccionTrasEliminarODesactivar(
        bool conReferencias,
        ResultadoBajaMetodoPago resultadoEsperado,
        int cantidadEsperada)
    {
        var (vistaModelo, repositorio, mensajes) = Crear();
        repositorio.Agregar(Metodo(1, "Efectivo", "Activo"));
        if (conReferencias)
        {
            repositorio.MarcarConVentas(1);
        }

        await vistaModelo.InicializarCommand.ExecuteAsync();
        vistaModelo.MetodoSeleccionado = vistaModelo.Metodos[0];

        await vistaModelo.EliminarCommand.ExecuteAsync();

        Assert.Equal(1, repositorio.UltimaBaja);
        Assert.Equal(resultadoEsperado, repositorio.UltimoResultadoBaja);
        Assert.Equal(MensajesMetodosPago.ConfirmarBaja, Assert.Single(mensajes.Confirmaciones).Mensaje);
        Assert.Equal(MensajesMetodosPago.MetodoDadoDeBaja, Assert.Single(mensajes.Exitos).Mensaje);
        Assert.Empty(mensajes.Errores);
        Assert.Equal(cantidadEsperada, vistaModelo.Metodos.Count);
        if (conReferencias)
        {
            Assert.Equal("Inactivo", vistaModelo.Metodos[0].Estado);
        }
        else
        {
            Assert.Null(vistaModelo.MetodoSeleccionado);
        }
    }

    [Fact]
    [Trait("Especificacion", "ESP-0017")]
    [Trait("Tecnica", "T-012")]
    public void TecnicoNoPuedeUsarControlesDeEscritura()
    {
        var (vistaModelo, _, _) = Crear(Rol.Tecnico);
        vistaModelo.MetodoSeleccionado = Metodo(1, "Efectivo", "Activo");

        Assert.False(vistaModelo.EsAdministrador);
        Assert.Equal("Solo disponible para el Administrador.", vistaModelo.MotivoAccionesAdministrador);
        Assert.False(vistaModelo.CrearCommand.CanExecute(null));
        Assert.False(vistaModelo.ActualizarCommand.CanExecute(null));
        Assert.False(vistaModelo.ConfirmarActualizacionCommand.CanExecute(null));
        Assert.False(vistaModelo.EliminarCommand.CanExecute(null));
    }

    private static (MetodosPagoViewModel VistaModelo, RepositorioDeMetodosPagoFalso Repositorio, MensajeServicioFalso Mensajes) Crear(
        Rol rol = Rol.Administrador,
        bool respuestaConfirmacion = true)
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        var sesion = new SesionSimulada(rol);
        var mensajes = new MensajeServicioFalso { RespuestaConfirmacion = respuestaConfirmacion };
        var vistaModelo = new MetodosPagoViewModel(
            new MetodosPagoServicio(repositorio, sesion),
            mensajes,
            sesion);
        return (vistaModelo, repositorio, mensajes);
    }

    private static MetodoPagoConsulta Metodo(int id, string nombre, string estado) =>
        new()
        {
            IdMetodoPago = id,
            NombreMetodo = nombre,
            Estado = estado
        };
}
