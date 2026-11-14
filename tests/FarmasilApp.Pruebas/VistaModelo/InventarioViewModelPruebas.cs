using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Pruebas.INV;
using FarmasilApp.VistaModelo.INV;

namespace FarmasilApp.Pruebas.VistaModelo;

public class InventarioViewModelPruebas
{
    private static InventarioViewModel Crear(RepositorioDeInventarioFalso repositorio) =>
        new(new InventarioServicio(repositorio, new ProveedoresSimulado()));

    [Fact]
    public async Task InicializarCargaElFiltroDeProveedorPorRazonSocial()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        repositorio.IdsDeProveedor.AddRange(new[] { 1, 3 });
        var vistaModelo = Crear(repositorio);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        Assert.Equal(
            new[] { "Todos", "Drogueria Andina SAC", "Laboratorios del Pacifico SAC" },
            vistaModelo.Proveedores.Select(p => p.Nombre));
        Assert.Same(ProveedorFiltro.Todos, vistaModelo.ProveedorSeleccionado);
    }

    [Fact]
    public async Task ConTodosSeleccionadoLaConsultaNoFiltraPorProveedor()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        repositorio.IdsDeProveedor.Add(1);
        var vistaModelo = Crear(repositorio);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        Assert.Null(repositorio.UltimaConsulta?.IdProveedor);
    }

    [Fact]
    public async Task AlElegirUnProveedorLaConsultaFiltraPorSuIdentificador()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        repositorio.IdsDeProveedor.AddRange(new[] { 1, 3 });
        var vistaModelo = Crear(repositorio);
        await vistaModelo.InicializarCommand.ExecuteAsync();

        vistaModelo.ProveedorSeleccionado = vistaModelo.Proveedores.Single(p => p.IdProveedor == 3);
        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.Equal(3, repositorio.UltimaConsulta?.IdProveedor);
    }

    [Fact]
    public async Task SiLaListaDeProveedoresSePierdeLaConsultaNoFiltra()
    {
        // Al vaciarse la lista, el ComboBox deja la selección en nulo.
        var repositorio = new RepositorioDeInventarioFalso();
        var vistaModelo = Crear(repositorio);
        await vistaModelo.InicializarCommand.ExecuteAsync();

        vistaModelo.ProveedorSeleccionado = null;
        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.Null(repositorio.UltimaConsulta?.IdProveedor);
    }

    [Fact]
    public async Task SiFallaLaCargaDeProveedoresQuedaSoloTodosYSeAvisa()
    {
        var repositorio = new RepositorioDeInventarioFalso { FallaAlObtenerProveedores = true };
        var vistaModelo = Crear(repositorio);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        Assert.Equal(new[] { "Todos" }, vistaModelo.Proveedores.Select(p => p.Nombre));
        Assert.StartsWith("No se pudieron cargar los filtros del inventario.", vistaModelo.Mensaje);
    }
}
