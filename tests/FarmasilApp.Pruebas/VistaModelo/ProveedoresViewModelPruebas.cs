using FarmasilApp.Modelo.PRV;
using FarmasilApp.Pruebas.PRV;
using FarmasilApp.VistaModelo.PRV;

namespace FarmasilApp.Pruebas.VistaModelo;

public class ProveedoresViewModelPruebas
{
    private static ProveedoresViewModel Crear(RepositorioDeProveedoresFalso repositorio) =>
        new(new ProveedoresServicio(repositorio));

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
        Assert.Equal("No se pudo consultar los proveedores. Inténtelo nuevamente.", vistaModelo.Mensaje);
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

        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.Null(vistaModelo.ProveedorSeleccionado);
    }
}
