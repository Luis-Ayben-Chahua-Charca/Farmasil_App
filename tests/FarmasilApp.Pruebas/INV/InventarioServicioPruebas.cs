using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.PRV;

namespace FarmasilApp.Pruebas.INV;

public class InventarioServicioPruebas
{
    private static InventarioServicio Servicio(RepositorioDeInventarioFalso repositorio) =>
        new(repositorio, new ProveedoresSimulado());

    [Fact]
    public void LosProveedoresDelFiltroSeMuestranPorRazonSocial()
    {
        // Los ids salen de los productos de Inventario; el nombre, de IProveedores (DEC-065).
        var repositorio = new RepositorioDeInventarioFalso();
        repositorio.IdsDeProveedor.AddRange(new[] { 1, 2, 3 });

        var proveedores = Servicio(repositorio).ObtenerProveedores();

        Assert.Equal(
            new[] { "Drogueria Andina SAC", "Distribuidora Salud Sur SAC", "Laboratorios del Pacifico SAC" },
            proveedores.Select(p => p.Nombre));
        Assert.Equal(new int?[] { 1, 2, 3 }, proveedores.Select(p => p.IdProveedor));
    }

    [Fact]
    public void UnProveedorInactivoConProductosTambienSeMuestraConSuNombre()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        repositorio.IdsDeProveedor.Add(4);

        var proveedor = Assert.Single(Servicio(repositorio).ObtenerProveedores());

        Assert.Equal("Proveedor Inactivo SAC", proveedor.Nombre);
    }

    [Fact]
    public void UnProveedorQueIProveedoresNoConoceSeMuestraPorSuIdentificador()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        repositorio.IdsDeProveedor.Add(99);

        var proveedor = Assert.Single(Servicio(repositorio).ObtenerProveedores());

        Assert.Equal(99, proveedor.IdProveedor);
        Assert.Equal("Proveedor 99", proveedor.Nombre);
    }

    [Fact]
    public void SinProductosNoHayProveedoresQueFiltrar()
    {
        Assert.Empty(Servicio(new RepositorioDeInventarioFalso()).ObtenerProveedores());
    }

    [Fact]
    public void TodosNoFiltraPorProveedor()
    {
        Assert.Null(ProveedorFiltro.Todos.IdProveedor);
        Assert.Equal("Todos", ProveedorFiltro.Todos.Nombre);
    }

    [Fact]
    public void ConsultarNormalizaLosCriteriosAntesDeLlamarAlRepositorio()
    {
        var repositorio = new RepositorioDeInventarioFalso();

        Servicio(repositorio).Consultar("  para  ", 3, "   ");

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Equal("para", consulta.Value.Busqueda);
        Assert.Equal(3, consulta.Value.IdProveedor);
        Assert.Null(consulta.Value.Accion);
    }

    [Fact]
    public void ElRepositorioYIProveedoresSonObligatorios()
    {
        Assert.Throws<ArgumentNullException>(() => new InventarioServicio(null!, new ProveedoresSimulado()));
        Assert.Throws<ArgumentNullException>(() => new InventarioServicio(new RepositorioDeInventarioFalso(), null!));
    }
}
