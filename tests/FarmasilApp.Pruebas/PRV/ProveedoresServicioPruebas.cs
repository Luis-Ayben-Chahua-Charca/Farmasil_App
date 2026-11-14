using FarmasilApp.Modelo.PRV;

namespace FarmasilApp.Pruebas.PRV;

public class ProveedoresServicioPruebas
{
    private static ProveedoresServicio Servicio(RepositorioDeProveedoresFalso repositorio) =>
        new(repositorio);

    [Fact]
    public void ConsultarNormalizaLosCriteriosAntesDeLlamarAlRepositorio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar("  Andina  ", "  Activo  ");

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Equal("Andina", consulta.Value.Busqueda);
        Assert.Equal("Activo", consulta.Value.Estado);
    }

    [Fact]
    public void ConsultarConBusquedaVaciaLaPasaComoNulo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar("   ", null);

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Null(consulta.Value.Busqueda);
        Assert.Null(consulta.Value.Estado);
    }

    [Fact]
    public void ConsultarConEstadoVacioLoPasaComoNulo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar(null, "   ");

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Null(consulta.Value.Estado);
    }

    [Fact]
    public void ConsultarSinCriteriosListaTodos()
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

        var resultados = Servicio(repositorio).Consultar(null, null);

        Assert.Single(resultados);
        Assert.Equal("Drogueria Andina SAC", resultados[0].RazonSocial);

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Null(consulta.Value.Busqueda);
        Assert.Null(consulta.Value.Estado);
    }

    [Fact]
    public void ConsultarPorRucExactoPasaElCriterioAlRepositorio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar("20100070970", null);

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Equal("20100070970", consulta.Value.Busqueda);
    }

    [Fact]
    public void ConsultarPorRazonSocialParcialPasaElCriterioAlRepositorio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar("Andina", null);

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Equal("Andina", consulta.Value.Busqueda);
    }

    [Fact]
    public void ConsultarPorEstadoActivoPasaElFiltroAlRepositorio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar(null, "Activo");

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Equal("Activo", consulta.Value.Estado);
    }

    [Fact]
    public void ConsultarPorEstadoInactivoPasaElFiltroAlRepositorio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar(null, "Inactivo");

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Equal("Inactivo", consulta.Value.Estado);
    }

    [Fact]
    public void ConsultarCombinandoBusquedaYEstadoPasaAmbosCriterios()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar("Salud", "Activo");

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Equal("Salud", consulta.Value.Busqueda);
        Assert.Equal("Activo", consulta.Value.Estado);
    }

    [Fact]
    public void ConsultarSinResultadosDevuelveListaVacia()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        var resultados = Servicio(repositorio).Consultar("NoExiste", null);

        Assert.Empty(resultados);
    }

    [Fact]
    public void ConsultarDevuelveProveedorConTelefonoNulo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        repositorio.Resultados.Add(new ProveedorConsulta
        {
            IdProveedor = 4,
            Ruc = "20501112223",
            RazonSocial = "Proveedor Inactivo SAC",
            Telefono = null,
            Estado = "Inactivo"
        });

        var resultados = Servicio(repositorio).Consultar(null, null);

        Assert.Single(resultados);
        Assert.Null(resultados[0].Telefono);
        Assert.Equal("Inactivo", resultados[0].Estado);
    }

    [Fact]
    public void ElRepositorioEsObligatorio()
    {
        Assert.Throws<ArgumentNullException>(() => new ProveedoresServicio(null!));
    }
}
