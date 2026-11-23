using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Pruebas.PRV;
using FarmasilApp.VistaModelo.PRV;

namespace FarmasilApp.Pruebas.VistaModelo;

public class ProveedoresViewModelPruebas
{
    private static ProveedoresViewModel Crear(
        RepositorioDeProveedoresFalso repositorio,
        MensajeServicioFalso? mensajes = null,
        Rol rol = Rol.Administrador)
    {
        var sesion = new SesionSimulada(rol);
        return new(
            new ProveedoresServicio(repositorio, sesion),
            mensajes ?? new MensajeServicioFalso(),
            sesion);
    }

    // =============================
    // ESP-0038: Consulta (existentes actualizados)
    // =============================

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public async Task AlInicializarEstadosEsTodosActivoInactivoYElSeleccionadoEsTodos()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        var vistaModelo = Crear(repositorio);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        Assert.Equal(new[] { "Todos", "Activo", "Inactivo" }, vistaModelo.Estados);
        Assert.Equal("Todos", vistaModelo.EstadoSeleccionado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    public async Task ConTodosSeleccionadoLaConsultaLlegaSinFiltroDeEstado()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        var vistaModelo = Crear(repositorio);
        await vistaModelo.InicializarCommand.ExecuteAsync();

        vistaModelo.EstadoSeleccionado = "Todos";
        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.NotNull(repositorio.UltimaConsulta);
        Assert.Null(repositorio.UltimaConsulta.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    public async Task AlElegirActivoLaConsultaLlegaConActivo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        var vistaModelo = Crear(repositorio);
        await vistaModelo.InicializarCommand.ExecuteAsync();

        vistaModelo.EstadoSeleccionado = "Activo";
        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.NotNull(repositorio.UltimaConsulta);
        Assert.Equal("Activo", repositorio.UltimaConsulta.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public async Task ConsultaConResultadosLlenaProveedoresYMuestraMensaje()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        repositorio.Resultados.Add(new ProveedorConsulta
        {
            IdProveedor = 1,
            Ruc = "20100070970",
            RazonSocial = "Drogueria Andina SAC",
            Telefono = "054223344",
            Estado = "Activo"
        });
        repositorio.Resultados.Add(new ProveedorConsulta
        {
            IdProveedor = 2,
            Ruc = "20200080980",
            RazonSocial = "Laboratorios del Sur SAC",
            Telefono = null,
            Estado = "Activo"
        });
        var vistaModelo = Crear(repositorio);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        Assert.Equal(2, vistaModelo.Proveedores.Count);
        Assert.Equal("Proveedores encontrados: 2.", vistaModelo.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    public async Task ConsultaSinResultadosDejaProveedoresVacioYMuestraMensaje()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        var vistaModelo = Crear(repositorio);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        Assert.Empty(vistaModelo.Proveedores);
        Assert.Equal("No se encontraron proveedores.", vistaModelo.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public async Task SiElRepositorioFallaProveedoresQuedaVacioYMuestraMensajeDeError()
    {
        var repositorio = new RepositorioDeProveedoresFalso { FallaAlConsultar = true };
        var vistaModelo = Crear(repositorio);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        Assert.Empty(vistaModelo.Proveedores);
        Assert.Equal(MensajesProveedores.ErrorConsulta, vistaModelo.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public async Task SeleccionarProveedorCargaLaFichaYDespuesDeConsultaVuelveANulo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        var proveedor = new ProveedorConsulta
        {
            IdProveedor = 1,
            Ruc = "20100070970",
            RazonSocial = "Drogueria Andina SAC",
            Telefono = "054223344",
            Estado = "Activo"
        };
        repositorio.Resultados.Add(proveedor);
        var vistaModelo = Crear(repositorio);
        await vistaModelo.InicializarCommand.ExecuteAsync();

        vistaModelo.SeleccionarProveedorCommand.Execute(vistaModelo.Proveedores[0]);

        Assert.NotNull(vistaModelo.ProveedorSeleccionado);
        Assert.Equal("Drogueria Andina SAC", vistaModelo.ProveedorSeleccionado.RazonSocial);
        Assert.Equal("20100070970", vistaModelo.Ruc);
        Assert.Equal("Drogueria Andina SAC", vistaModelo.RazonSocial);

        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.Null(vistaModelo.ProveedorSeleccionado);
    }

    // =============================
    // ESP-0037: Crear
    // =============================

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-001")]
    public async Task CrearConDatosValidosMuestraMensajeDeExito()
    {
        var repositorio = new RepositorioDeProveedoresFalso { IdProveedorADevolver = 5 };
        var mensajes = new MensajeServicioFalso();
        var vistaModelo = Crear(repositorio, mensajes);

        vistaModelo.Ruc = "20100070970";
        vistaModelo.RazonSocial = "Drogueria Andina SAC";
        vistaModelo.Telefono = "054223344";

        await vistaModelo.CrearCommand.ExecuteAsync();

        Assert.Single(mensajes.Exitos);
        Assert.Equal(MensajesProveedores.Titulo, mensajes.Exitos[0].Titulo);
        Assert.Equal(MensajesProveedores.ProveedorRegistrado, mensajes.Exitos[0].Mensaje);
        Assert.Equal(5, vistaModelo.IdProveedorFicha);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-002")]
    public async Task CrearConRucInvalidoMuestraError()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        var mensajes = new MensajeServicioFalso();
        var vistaModelo = Crear(repositorio, mensajes);

        vistaModelo.Ruc = "12345";
        vistaModelo.RazonSocial = "Drogueria SAC";

        await vistaModelo.CrearCommand.ExecuteAsync();

        Assert.Single(mensajes.Errores);
        Assert.Equal(MensajesProveedores.RucInvalido, mensajes.Errores[0].Mensaje);
        Assert.Null(repositorio.UltimoCreado);
    }

    // =============================
    // ESP-0039: Actualizar
    // =============================

    [Fact]
    [Trait("Especificacion", "ESP-0039")]
    [Trait("Tecnica", "T-001")]
    public async Task ConfirmarActualizacionConDatosValidosMuestraMensajeDeExito()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        repositorio.Resultados.Add(new ProveedorConsulta
        {
            IdProveedor = 1,
            Ruc = "20100070970",
            RazonSocial = "Drogueria Andina SAC",
            Telefono = "054223344",
            Estado = "Activo"
        });
        var mensajes = new MensajeServicioFalso();
        var vistaModelo = Crear(repositorio, mensajes);
        await vistaModelo.InicializarCommand.ExecuteAsync();

        // Seleccionar y editar
        vistaModelo.ProveedorSeleccionado = vistaModelo.Proveedores[0];
        vistaModelo.RazonSocial = "Drogueria Andina SAC Actualizada";

        // Forzar EnEdicion
        vistaModelo.ActualizarCommand.Execute(null);

        await vistaModelo.ConfirmarActualizacionCommand.ExecuteAsync();

        Assert.Single(mensajes.Exitos);
        Assert.Equal(MensajesProveedores.ProveedorActualizado, mensajes.Exitos[0].Mensaje);
    }

    // =============================
    // ESP-0040: Baja
    // =============================

    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-001")]
    public async Task DarDeBajaConConfirmacionMuestraMensajeDeExito()
    {
        var repositorio = new RepositorioDeProveedoresFalso
        {
            OrdenesDevPendientes = false,
            ResultadoBajaADevolver = ResultadoBajaProveedor.Eliminado
        };
        repositorio.Resultados.Add(new ProveedorConsulta
        {
            IdProveedor = 1,
            Ruc = "20100070970",
            RazonSocial = "Drogueria Andina SAC",
            Telefono = "054223344",
            Estado = "Activo"
        });
        var mensajes = new MensajeServicioFalso { RespuestaConfirmacion = true };
        var vistaModelo = Crear(repositorio, mensajes);
        await vistaModelo.InicializarCommand.ExecuteAsync();

        vistaModelo.ProveedorSeleccionado = vistaModelo.Proveedores[0];
        await vistaModelo.EliminarCommand.ExecuteAsync();

        Assert.Single(mensajes.Confirmaciones);
        Assert.Equal(MensajesProveedores.ConfirmarBaja, mensajes.Confirmaciones[0].Mensaje);
        Assert.Single(mensajes.Exitos);
        Assert.Equal(MensajesProveedores.ProveedorDadoDeBaja, mensajes.Exitos[0].Mensaje);
        Assert.Equal(1, repositorio.UltimaBaja);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-002")]
    public async Task DarDeBajaConOrdenesPendientesMuestraError()
    {
        var repositorio = new RepositorioDeProveedoresFalso { OrdenesDevPendientes = true };
        repositorio.Resultados.Add(new ProveedorConsulta
        {
            IdProveedor = 1,
            Ruc = "20100070970",
            RazonSocial = "Drogueria Andina SAC",
            Telefono = "054223344",
            Estado = "Activo"
        });
        var mensajes = new MensajeServicioFalso();
        var vistaModelo = Crear(repositorio, mensajes);
        await vistaModelo.InicializarCommand.ExecuteAsync();

        vistaModelo.ProveedorSeleccionado = vistaModelo.Proveedores[0];
        await vistaModelo.EliminarCommand.ExecuteAsync();

        Assert.Single(mensajes.Errores);
        Assert.Equal(MensajesProveedores.OrdenesPendientes, mensajes.Errores[0].Mensaje);
        Assert.Empty(mensajes.Confirmaciones);
        Assert.Null(repositorio.UltimaBaja);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-002")]
    public async Task DarDeBajaConConfirmacionCanceladaNoEjecuta()
    {
        var repositorio = new RepositorioDeProveedoresFalso { OrdenesDevPendientes = false };
        repositorio.Resultados.Add(new ProveedorConsulta
        {
            IdProveedor = 1,
            Ruc = "20100070970",
            RazonSocial = "Drogueria Andina SAC",
            Telefono = "054223344",
            Estado = "Activo"
        });
        var mensajes = new MensajeServicioFalso { RespuestaConfirmacion = false };
        var vistaModelo = Crear(repositorio, mensajes);
        await vistaModelo.InicializarCommand.ExecuteAsync();

        vistaModelo.ProveedorSeleccionado = vistaModelo.Proveedores[0];
        await vistaModelo.EliminarCommand.ExecuteAsync();

        Assert.Single(mensajes.Confirmaciones);
        Assert.Null(repositorio.UltimaBaja);
    }

    // =============================
    // DEC-075: Administrador-only
    // =============================

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void MotivoAccionesAdministradorEsNuloParaAdministrador()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        var vistaModelo = Crear(repositorio, rol: Rol.Administrador);

        Assert.Null(vistaModelo.MotivoAccionesAdministrador);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void MotivoAccionesAdministradorExplicaParaTecnico()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        var vistaModelo = Crear(repositorio, rol: Rol.Tecnico);

        Assert.NotNull(vistaModelo.MotivoAccionesAdministrador);
        Assert.Contains("Administrador", vistaModelo.MotivoAccionesAdministrador);
    }
}
